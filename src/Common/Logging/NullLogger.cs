using System;

namespace Common.Logging
{
    public class NullLogger : ILogger
    {
        public static readonly ILogger Instance = new NullLogger();

        public void Error(Exception exception, string memberName = null, string filePath = null, int lineNumber = 0)
        { }

        public void Error(string message, string memberName = null, string filePath = null, int lineNumber = 0)
        { }

        public void Warning(string message, string memberName = null, string filePath = null, int lineNumber = 0)
        { }

        public void Info(string message, string memberName = null, string filePath = null, int lineNumber = 0)
        { }

        public void Debug(string message, string memberName = null, string filePath = null, int lineNumber = 0)
        { }
    }
}