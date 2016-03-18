namespace Common.Domain.Notifications.Sms
{
    public interface ISmsSender
    {
        void SendSms(string message, string recipient);
    }
}
