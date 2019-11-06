namespace LoggerLibrary
{
    public interface IStringManager
    {
        string GetTitle(string title);

        string GetContent(string content, int tabIndex = 0);
    }
}