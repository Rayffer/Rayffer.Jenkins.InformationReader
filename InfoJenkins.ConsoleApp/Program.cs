namespace InfoJenkins.ConsoleApp
{
    using LoggerLibrary;
    using System;
    using System.Configuration;
    using System.IO;
    using Unity;
    using Unity.Injection;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["JenkinsURL"])
                && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["StartingFilteringJobName"])
                && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["EndingFilteringJobName"]))
            {
                Uri baseUri = new Uri(ConfigurationManager.AppSettings["JenkinsURL"]);

                IProcessManager processManager;
                using (IUnityContainer container = new UnityContainer())
                {
                    ConfigureUnity(container);
                    processManager = container.Resolve<IProcessManager>();
                }

                processManager.StartProcess(baseUri);
            }

            Console.WriteLine("There are some appsettings missing, please check that the appsettings has the following keys with a value set: JenkinsURL, StartingFilteringJobName, EndingFilteringJobName");
            Console.WriteLine();
            Console.WriteLine("The application will now exit, press enter to end the program");
            Console.ReadLine();
        }

        private static void ConfigureUnity(IUnityContainer container)
        {
            InfoJenkins.Reader.ConfigureUnity.Configure(container);
            string fileName = GetFileName();

            // Log
            container.RegisterType<IStringManager, StringManager>();
            container.RegisterType<IWriter, WriterConsole>(nameof(WriterConsole));
            container.RegisterType<IWriter, WriterFile>(nameof(WriterFile),
                new InjectionConstructor
                (
                    new InjectionParameter<string>(fileName)
                ));

            IWriter[] writers = new IWriter[]
            {
                container.Resolve<IWriter>(nameof(WriterConsole)),
                container.Resolve<IWriter>(nameof(WriterFile))
            };

            container.RegisterType<ILogger, Logger>
            (
                new InjectionConstructor
                (
                    typeof(IStringManager),
                   new InjectionParameter<IWriter[]>(writers)
                )
            );

            // Local
            container.RegisterType<IProcessManager, ProcessManager>();
        }

        private static string GetFileName()
        {
            string fileName = @"c:\result.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            return fileName;
        }
    }
}