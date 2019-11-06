namespace InfoJenkins.ConsoleApp
{
    using InfoJenkins.Dto;
    using InfoJenkins.Reader;
    using LoggerLibrary;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class ProcessManager : IProcessManager
    {
        private readonly IJenkinsReader jenkinsReader;
        private readonly ILogger logger;

        public ProcessManager(IJenkinsReader jenkinsReader, ILogger logger)
        {
            this.jenkinsReader = jenkinsReader;
            this.logger = logger;
        }

        public void StartProcess(Uri baseAddress)
        {
            var jobs = jenkinsReader.GetJobs(baseAddress);

            if (jobs != null && jobs.Any())
            {
                var failingJobs = jobs.Where(job => job.LastBuild != null && job.LastBuild.Result != null && job.LastBuild.Result.Equals("FAILURE")).ToList();
                Console.WriteLine("Some jobs in the watched cycle are failing: " + Environment.NewLine);
                if (failingJobs.Any())
                {
                    System.IO.Directory.Delete("./TestReports", true);
                    System.IO.Directory.CreateDirectory("./TestReports");
                    Thread.Sleep(100);
                    foreach (var job in failingJobs.Where(job => (DateTime.Now - job.LastFailedBuild.Timestamp).Days < 17))
                    {
                        var jsonJobString = JsonConvert.SerializeObject(job.LastFailedBuild, Formatting.Indented);
                        using (TextWriter writer = new StreamWriter($"./TestReports/{job.Name}.json", false))
                        {
                            writer.Write(jsonJobString);
                        }
                        Console.WriteLine(job.Name);
                    }
                }
                else
                    Console.WriteLine("None of the watched jobs are failing");
            }
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Press the enter key to exit the program");
            Console.ReadLine();
        }

        private void WriteSequenceJobs(IEnumerable<JobCollectionItem> jobs)
        {
            var counters = new Dictionary<int, int>();
            var maxLevels = jobs.Max(d => d.TabIndex + 1);

            for (var i = 0; i < maxLevels; i++)
            {
                counters.Add(i, 0);
            }
            logger.WriteContent("<pre>");
            foreach (var job in jobs.Where(d => d.TabIndex <= 1))
            {
                counters[job.TabIndex] += 1;
                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < job.TabIndex + 1; i++)
                {
                    sb.AppendFormat("{0}.", counters[i]);
                }
                sb.AppendFormat(" {0}", job.Job.Name);
                logger.WriteContent(sb.ToString(), job.TabIndex + 1);
            }
            logger.WriteContent("</pre>");
        }
    }
}