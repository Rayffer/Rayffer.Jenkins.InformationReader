namespace LoggerLibrary
{
    using System.IO;

    public class WriterFile : IWriter
    {
        private readonly string fileName;
        private readonly object objLocker = new object();

        public WriterFile(string fileName)
        {
            this.fileName = fileName;
        }

        public void Write(string text)
        {
            lock (objLocker)
            {
                using (var sw = new StreamWriter(fileName, append: true))
                {
                    sw.WriteLine(text);
                }
            }
        }
    }
}