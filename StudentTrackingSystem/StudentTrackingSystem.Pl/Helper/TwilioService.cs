//using StudentTrackingSystem.DAL.Models.Sms;
//using StudentTrackingSystem.PL.Settings;
//using Microsoft.CodeAnalysis;
//using Microsoft.Extensions.Options;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.TwiML.Messaging;

//namespace Company.PL.Helper
//{
//    public class TwilioService(IOptions<TwilioSettings> _options) : ITwilioService
//    {



//        public MessageResource SendSms(Sms sms)
//        {
//            //Intialize Connection
//            TwilioClient.Init(_options.Value.AccountSID, _options.Value.AuthouToken);


//            // build message
//            var message = MessageResource.Create(
//             body: sms.Body,
//             to: sms.To,
//             from: new Twilio.Types.PhoneNumber(_options.Value.PhoneNumber)
//             );


//            //return message 
//            return message;

//        }
//    }
//}
