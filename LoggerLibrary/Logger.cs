namespace LoggerLibrary
{
    public class Logger : ILogger
    {
        private readonly IStringManager stringManager;
        private readonly IWriter[] writers;

        public Logger(IStringManager stringManager, params IWriter[] writers)
        {
            this.stringManager = stringManager;
            this.writers = writers ?? new IWriter[] { };
        }

        public void WriteContent(string content, int tabIndex = 0)
        {
            string text = stringManager.GetContent(content, tabIndex);
            Write(text);
        }

        public void WriteTitle(string title)
        {
            string text = stringManager.GetTitle(title);
            Write(text);
        }

        private void Write(string text)
        {
            foreach (var writer in writers)
            {
                writer.Write(text);
            }
        }
    }
}