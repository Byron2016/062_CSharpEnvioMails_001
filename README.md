# 062_CSharpEnvioMails_001
- Envio de mails
	- .NET 6.0
		- code-maze: How to Send an Email in ASP.NET Core
			- https://code-maze.com/aspnetcore-send-email/
				- Content:
					- Synchronous way to send an email/
					- HTML for the message body
					- Asynchronous way to send an email
					- Adding attachments to an email
					
				- Add new ASP.NET Core Web API project
					- Solution name: EmailApp
					- Project name: EmailApp
					- .NET 6.0
					
				- New Class Library (.NET Core) (A project for creating a class library that targets .NET or .NET standard)
					- Name: EmailService
					- .NET 6.0
					
				- Add a reference to the new class library.
				- Inside EmailService class library
					- Create at root level a new class with name EmailConfiguration.cs 
					
						```cs
							public class EmailConfiguration
							{
								public string From { get; set; } = String.Empty;
								public string SmtpServer { get; set; } = String.Empty;
								public int Port { get; set; }
								public string UserName { get; set; } = String.Empty;
								public string Password { get; set; } = String.Empty;
							}
						```
								
					- Populate EmailConfiguration class' properties with values from appsettings.json. 
						
						```cs
							"EmailConfiguration": {
								"From": "codemazetest@gmail.com",
								"SmtpServer": "smtp.gmail.com",
								"Port": 465,
								"Username": "codemazetest@gmail.com",
								"Password": "our test password"
							},
						```
									
					- Recover values from appsettings.json file and them like a service.

						```cs
							using EmailService;
							namespace EmailApp
							{
								public class Program
								{
									public static void Main(string[] args)
									{
										var builder = WebApplication.CreateBuilder(args);
							
										//Recover data from appsettings.json
										var emailConfig  = builder.Configuration
											.GetSection("EmailConfiguration")
											.Get<EmailConfiguration>();
										....
										// Add services.
										builder.Services.AddSingleton(emailConfig );
									}
								}
							}
						```
							
					- Add the NETCore.MailKit library to the EmailService project
						- Install-Package NETCore.MailKit -Version 2.1.0
						
					- Send a Test Email
						- Create a Message class
							- This class set the data related to our email recipients, subject, and content
								```cs
									namespace EmailService
									{
										public class Message
										{
											//properties
											public List<MailboxAddress> To { get; set; }
											public string Subject { get; set; }
											public string Content { get; set; }
									
											//constructor
											public Message(IEnumerable<string> to, string subject, string content)
											{
												To = new List<MailboxAddress>();
												To.AddRange(to.Select(x => new MailboxAddress("",x)));
												Subject = subject;
												Content = content;
											}
										}
									}
								```
									
					- Create a Interfase and implement it
					
						```cs
							namespace EmailService
							{
								public interface IEmailSender
								{
									void SendEmail(Message message);
								}
							}
						```
									
						```cs
							namespace EmailService
							{
								public class EmailSender : IEmailSender
								{
									private readonly EmailConfiguration _emailConfig;
									public EmailSender(EmailConfiguration emailConfig)
									{
										_emailConfig = emailConfig;
									}
									public void SendEmail(Message message)
									{
										var emailMessage = CreateEmailMessage(message);
										Send(emailMessage);
									}
								}
							}
						```
					- Implement two private methods into EmailSender class
					
						```cs
							private MimeMessage CreateEmailMessage(Message message)
							{
								var emailMessage = new MimeMessage();
								emailMessage.From.Add(new MailboxAddress("pepito de los palotes",_emailConfig.From));
								emailMessage.To.AddRange(message.To);
								emailMessage.Subject = message.Subject;
								emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
					
								return emailMessage;
							}
					
							private void Send(MimeMessage mailMessage)
							{
								using (var client = new SmtpClient())
								{
									try
									{
										client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
										client.AuthenticationMechanisms.Remove("XOAUTH2");
										client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
					
										client.Send(mailMessage);
									}
									catch
									{
										//log an error message or throw an exception or both.
										throw;
									}
									finally
									{
										client.Disconnect(true);
										client.Dispose();
									}
								}
							}
						```
						
				- Inside EmailApp project
					- Register this service in the Startup.cs class
					
						```cs
							namespace EmailApp
							{
								public class Program
								{
									public static void Main(string[] args)
									{
										....
							
										// Add services to the container.
										builder.Services.AddSingleton(emailConfig);
							
										//Register the EmailService
										builder.Services.AddScoped<IEmailSender, EmailSender>();
							
										....
								}
							}
						```
						
					- Add SendEmailController
						```cs
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
										var message = new Message(new string[] { "codemazetest@mailinator.com" }, "Test email", "This is the content from our email.");
										_emailSender.SendEmail(message);
							
										return Ok();
									}
								}
							}
						```
						
				- Add from and to names
				- Using HTML in the Email Body
					- 
						```cs
							private MimeMessage CreateHtmlEmailMessage(Message message)
							{
								var emailMessage = new MimeMessage();
								emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
								emailMessage.To.AddRange(message.To);
								emailMessage.Subject = message.Subject;
								//emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
								emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };
								return emailMessage;
							}
						```