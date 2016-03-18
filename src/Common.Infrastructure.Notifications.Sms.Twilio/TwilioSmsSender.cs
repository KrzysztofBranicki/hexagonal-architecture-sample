using Common.Domain.Notifications.Sms;
using Twilio;

namespace Common.Infrastructure.Notifications.Sms.Twilio
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly string _senderPhoneNumber;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;

        public TwilioSmsSender(string twilioAccountSid, string twilioAuthToken, string senderPhoneNumber)
        {
            _twilioAccountSid = twilioAccountSid;
            _twilioAuthToken = twilioAuthToken;
            _senderPhoneNumber = senderPhoneNumber;
        }

        public void SendSms(string message, string recipient)
        {
            var twilio = new TwilioRestClient(_twilioAccountSid, _twilioAuthToken);

            var m = twilio.SendMessage(_senderPhoneNumber, recipient, message, string.Empty);
            if (m.RestException != null)
            {
                throw new SendSmsException(m.RestException.Message);
            }
        }
    }
}
