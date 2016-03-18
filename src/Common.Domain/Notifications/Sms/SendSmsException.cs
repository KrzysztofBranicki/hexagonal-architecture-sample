using System;

namespace Common.Domain.Notifications.Sms
{
    [Serializable]
    public class SendSmsException : Exception
    {
        public SendSmsException(string message) : base(message)
        { }
    }
}
