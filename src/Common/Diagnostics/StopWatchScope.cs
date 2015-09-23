using System;
using System.Diagnostics;

namespace Common.Diagnostics
{
    public class StopWatchScope : IDisposable
    {
        private readonly Action<TimeSpan, string> _resultHandler;
        private readonly Action<string> _messagePresenter;
        private readonly Stopwatch _stopWatch;

        public StopWatchScope(Action<string> messagePresenter = null) : this()
        {
            _messagePresenter = messagePresenter ?? (s => Debug.WriteLine("Runtime " + s));
        }

        public StopWatchScope(Action<TimeSpan, string> resultHandler) : this()
        {
            _resultHandler = resultHandler;
        }

        private StopWatchScope()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public void Dispose()
        {
            _stopWatch.Stop();
            var ts = _stopWatch.Elapsed;
            var formatedElapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            if(_resultHandler != null)
                _resultHandler(ts, formatedElapsedTime);
            else
                _messagePresenter(formatedElapsedTime);
        }
    }
}
