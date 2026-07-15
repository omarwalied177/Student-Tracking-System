
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;

namespace Company.PL.Helper
{
    public interface ITwilioService
    {

        public MessageResource SendSms(Sms sms);


    }
}
