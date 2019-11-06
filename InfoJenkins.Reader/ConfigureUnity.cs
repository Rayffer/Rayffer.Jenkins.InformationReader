namespace InfoJenkins.Reader
{
    using Unity;

    public static class ConfigureUnity
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IJenkinsReader, JenkinsReader>();
        }
    }
}