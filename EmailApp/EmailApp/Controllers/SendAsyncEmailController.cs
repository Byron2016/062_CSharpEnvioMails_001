using EmailService;
using Microsoft.AspNetCore.Mvc;

namespace EmailApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendAsyncEmailController : Controller
    {
        private readonly IEmailSender _emailSender;

        public SendAsyncEmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }


        [HttpGet(Name = "SentAsyncEmail")]
        public async Task<IActionResult> SentAsync()
        {
            var message = new Message(new string[] { "codemazetest@mailinator.com", "xgvqvsdo@sharklasers.com", "testaspcore@dispostable.com" }, "1-Test async email", "1-This is the content from our async email.");
            await _emailSender.SendEmailAsync(message);
            Console.WriteLine($"dentro de SentAsyncEmail");
            for (var i = 0; i < 10000; i++)
            {
                Console.WriteLine($"dentro de SentAsyncEmail-{i}");
            }

            return Ok();
        }
    }
}
