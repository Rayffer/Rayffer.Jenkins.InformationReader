namespace LoggerLibrary
{
    using System;

    public class WriterConsole : IWriter
    {
        public void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
}