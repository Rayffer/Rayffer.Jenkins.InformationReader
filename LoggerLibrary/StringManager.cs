namespace LoggerLibrary
{
    using System.Text;

    public class StringManager : IStringManager
    {
        private const char tabCharacter = '|';
        private const char titleCharacter = '=';

        public string GetContent(string content, int tabIndex = 0)
        {
            var result = content;
            return result;
        }

        public string GetTitle(string title)
        {
            var lineDecoration = new string(titleCharacter, 80);
            var sb = new StringBuilder();

            sb.AppendLine(lineDecoration);
            sb.AppendLine(title);
            sb.AppendLine(lineDecoration);

            return sb.ToString();
        }
    }
}