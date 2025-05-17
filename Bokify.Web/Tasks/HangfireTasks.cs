using Microsoft.AspNetCore.Identity.UI.Services;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;


namespace Bokify.Web.Tasks
{
    public class HangfireTasks
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWhatsAppClient _whatsAppClient;

        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;

        public HangfireTasks(ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            IWhatsAppClient whatsAppClient,
            IEmailBodyBuilder emailBodyBuilder,
            IEmailSender emailSender)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _whatsAppClient = whatsAppClient;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
        }
        public async Task PrepareExpirationAlert()
        {
            var subscribers = _context.Subscribers
                .Include(s => s.Subscriptions)
                .Where(s => !s.IsBlackListed && s.Subscriptions.OrderByDescending(x => x.EndDate).First().EndDate == DateTime.Today.AddDays(5))
                .ToList();

            foreach (var subscriber in subscribers)
            {
                var endDate = subscriber.Subscriptions.Last().EndDate.ToString("d MMM, yyyy");

                //Send email and WhatsApp Message
                var placeholders = new Dictionary<string, string>()
                {
                    { "imageUrl", "https://res.cloudinary.com/salemgomaa/image/upload/c_thumb,w_200,g_face/v1747235868/Renew_at9wmj.jpg" },
                    { "header", $"Hello {subscriber.FirstName}," },
                    { "body", $"your subscription will be expired by {endDate} 🙁" }
                };

                var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

                await _emailSender.SendEmailAsync(
                    subscriber.Email,
                    "Bookify Subscription Expiration", body);

                if (subscriber.HasWhattsApp)
                {
                    var components = new List<WhatsAppComponent>()
                    {
                        new WhatsAppComponent
                        {
                            Type = "body",
                            Parameters = new List<object>()
                            {
                                new WhatsAppTextParameter { Text = subscriber.FirstName },
                                new WhatsAppTextParameter { Text = endDate },
                            }
                        }
                    };

                    var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01286582478" : subscriber.MobileNumber;

                    //Change 2 with your country code
                    await _whatsAppClient
                        .SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English,
                        WhatsAppTemplates.SubscriptionExpiration, components);
                }
            }
        }

    }
}