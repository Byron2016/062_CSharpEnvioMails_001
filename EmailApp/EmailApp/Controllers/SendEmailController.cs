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
            var message = new Message(new string[] { "codemazetest@mailinator.com", "xgvqvsdo@sharklasers.com", "testaspcore@dispostable.com" }, "5-Test email", "5-This is the content from our email.");
            //_emailSender.SendEmail(message);
            _emailSender.SendHtmlEmail(message);

            return Ok();
        }

        //[HttpGet(Name = "AsyncEmailSent")]
        //public async Task<IActionResult> Sent(int i =  0)
        //{
        //    var message = new Message(new string[] { "codemazetest@mailinator.com", "xgvqvsdo@sharklasers.com", "testaspcore@dispostable.com" }, "1-Test async email", "1-This is the content from our async email.");
        //    await _emailSender.SendEmailAsync(message);
        //    Console.WriteLine($"dentro de SentAsyncEmail");
        //    //for (var i = 0; i < 10000; i++)
        //    //{
        //    //    Console.WriteLine($"dentro de SentAsyncEmail-{i}");
        //    //}

        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

            var message = new MessageWithFiles(new string[] { "codemazetest@mailinator.com" }, "Test mail with Attachments", "This is the content from our mail with attachments.", files);

            await _emailSender.SendEmailAsync(message);

            return Ok();
        }
    }
}
