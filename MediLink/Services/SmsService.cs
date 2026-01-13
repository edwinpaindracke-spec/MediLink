using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

public class SmsService
{
    private readonly IConfiguration _config;

    public SmsService(IConfiguration config)
    {
        _config = config;
    }

    public void SendSms(string phone, string message)
    {
        var accountSid = _config["Twilio:AccountSid"];
        var authToken = _config["Twilio:AuthToken"];
        var fromPhone = _config["Twilio:FromPhone"];

        TwilioClient.Init(accountSid, authToken);

        MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(fromPhone),
            to: new Twilio.Types.PhoneNumber(phone)
        );
    }
}
