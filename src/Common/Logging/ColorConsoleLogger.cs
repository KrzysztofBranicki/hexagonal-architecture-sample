using System;
using System.Runtime.CompilerServices;

namespace Common.Logging
{
    public class ColorConsoleLogger : ILogger
    {
        public void Error(Exception exception, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            var text = FormatLogText("ERROR", exception.ToString(), memberName, filePath, lineNumber);
            WriteInColor(text, ConsoleColor.DarkRed);
        }

        public void Error(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            var text = FormatLogText("ERROR", message, memberName, filePath, lineNumber);
            WriteInColor(text, ConsoleColor.DarkRed);
        }

        public void Warning(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            var text = FormatLogText("WARNING", message, memberName, filePath, lineNumber);
            WriteInColor(text, ConsoleColor.DarkYellow);
        }

        public void Info(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            var text = FormatLogText("INFO", message, memberName, filePath, lineNumber);
            WriteInColor(text, ConsoleColor.DarkBlue);
        }

        public void Debug(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            var text = FormatLogText("DEBUG", message, memberName, filePath, lineNumber);
            Console.WriteLine(text);
        }

        private string FormatLogText(string level, string message, string memberName, string filePath, int lineNumber)
        {
            return $"[{DateTime.Now}][{level}] Method: {memberName} in file: {filePath} line number: {lineNumber}, Message: {message}";
        }

        private void WriteInColor(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }
    }
}
