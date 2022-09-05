using EmailService;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EmailApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendEmailController : Controller
    {
        private readonly IEmailSender _emailSender;

        public SendEmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet(Name = "SentEmail")]
        public IActionResult Sent()
        {
            var message = new Message(new string[] { "codemazetest@mailinator.com", "xgvqvsdo@sharklasers.com", "testaspcore@dispostable.com" }, "4-Test email", "4-This is the content from our email.");
            _emailSender.SendEmail(message);

            return Ok();
        }
    }
}
