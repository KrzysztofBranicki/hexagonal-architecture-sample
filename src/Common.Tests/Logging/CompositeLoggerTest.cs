using Common.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Common.Tests.Logging
{
    [TestFixture]
    public class CompositeLoggerTest
    {
        [Test]
        public void All_loggers_should_receive_call_with_injected_caller_info()
        {
            var logger1 = Substitute.For<ILogger>();
            var logger2 = Substitute.For<ILogger>();
            var compositeLogger = new CompositeLogger(logger1, logger2);

            var className = "CompositeLoggerTest.cs";
            var methodName = "All_loggers_should_receive_call_with_injected_caller_info";
            var invokationLineNumber = 22;
            var message = "TestMessage";

            compositeLogger.Debug(message);

            logger1.Received().Debug(message, methodName, Arg.Is<string>(x => x.EndsWith(className)), invokationLineNumber);
            logger2.Received().Debug(message, methodName, Arg.Is<string>(x => x.EndsWith(className)), invokationLineNumber);
        }
    }
}
