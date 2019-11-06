namespace LoggerLibrary
{
    public interface ILogger
    {
        void WriteTitle(string title);

        void WriteContent(string content, int tabIndex = 0  );
    }
}