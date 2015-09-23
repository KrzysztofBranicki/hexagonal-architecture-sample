using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Common.Logging
{
    public class CompositeLogger : ILogger
    {
        private readonly List<ILogger> _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            _loggers = loggers.ToList();
        }

        public CompositeLogger(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers.ToList();
        }

        public void Error(Exception exception, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            _loggers.ForEach(x => x.Error(exception, memberName, filePath, lineNumber));
        }

        public void Error(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            _loggers.ForEach(x => x.Error(message, memberName, filePath, lineNumber));
        }

        public void Warning(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            _loggers.ForEach(x => x.Warning(message, memberName, filePath, lineNumber));
        }

        public void Info(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            _loggers.ForEach(x => x.Info(message, memberName, filePath, lineNumber));
        }

        public void Debug(string message, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            _loggers.ForEach(x => x.Debug(message, memberName, filePath, lineNumber));
        }
    }
}