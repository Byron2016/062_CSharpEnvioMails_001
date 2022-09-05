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
						
				- Sending an Email in ASP.NET Core Asynchronously
					- Add a new method to interfase IEmailSender
						```cs
							Task SendEmailAsync(Message message);
						```
					- Implement interfase IEmailSender
						```cs
							public async Task SendEmailAsync(Message message)
							{
								var mailMessage = CreateEmailMessage(message);
								await SendAsync(mailMessage);
							}
						```
						
					- Add SendAsync method
						```cs
							private async Task SendAsync(MimeMessage mailMessage)
							{
								using (var client = new SmtpClient())
								{
									try
									{
										await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
										client.AuthenticationMechanisms.Remove("XOAUTH2");
										await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
										await client.SendAsync(mailMessage);
									}
									catch
									{
										//log an error message or throw an exception, or both.
										throw;
									}
									finally
									{
										await client.DisconnectAsync(true);
										client.Dispose();
									}
								}
							}
						```
						
					- Add Method to controller
						```cs
							[HttpGet(Name = "SentAsyncEmail")]
							public async Task<IActionResult> SentAsync()
							{
								var message = new Message(new string[] { "codemazetest@mailinator.com", "xgvqvsdo@sharklasers.com", "testaspcore@dispostable.com" }, "1-Test async email", "1-This is the content from our async email.");
								await _emailSender.SendEmailAsync(message);
					
								return Ok();
							}
						```
						
				- Adding File Attachments
					- Add a new POST action to our controller
						```cs
							[HttpPost]
							public async Task<IActionResult> Post()
							{
								var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
					
								var message = new MessageWithFiles(new string[] { "codemazetest@mailinator.com" }, "Test mail with Attachments", "This is the content from our mail with attachments.", files);
					
								await _emailSender.SendEmailAsync(message);
					
								return Ok();
							}
						```
						
					- Modify Message class to receive attachments (for our case we will create a new class)
						```cs
							namespace EmailService
							{
								public class MessageWithFiles
								{
									//properties
									public List<MailboxAddress> To { get; set; }
									public string Subject { get; set; }
									public string Content { get; set; }
									public IFormFileCollection Attachments { get; set; }
							
									//constructor
									public MessageWithFiles(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments)
									{
										int i = 0;
										To = new List<MailboxAddress>();
										//To.AddRange(to.Select(x => new MailboxAddress(subject, x)));
										To.AddRange(to.Select(x => {
											i++;
											return new MailboxAddress($"a-{i}", x);
										}));
										Subject = subject;
										Content = content;
										Attachments = attachments;
									}
								}
							}
						```
						
					- Modify method CreateEmailMessage )
						```cs
							namespace EmailService
							{
								public class EmailSender : IEmailSender
								{
									....
							
									private MimeMessage CreateEmailMessageWithAttachments(MessageWithFiles message)
									{
										var emailMessage = new MimeMessage();
										emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
										emailMessage.To.AddRange(message.To);
										emailMessage.Subject = message.Subject;
							
										var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };
										if (message.Attachments != null && message.Attachments.Any())
										{
											byte[] fileBytes;
											foreach (var attachment in message.Attachments)
											{
												using (var ms = new MemoryStream())
												{
													attachment.CopyTo(ms);
													fileBytes = ms.ToArray();
												}
												bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
											}
										}
										emailMessage.Body = bodyBuilder.ToMessageBody();
										return emailMessage;
									}
							
									....
								}
							}
						```
						
					- add the FormOptions configuration to the ConfigureServices method
						```cs
							using EmailService;
							using Microsoft.AspNetCore.Http.Features;
							namespace EmailApp
							{
								public class Program
								{
									public static void Main(string[] args)
									{
										...
							
										//Register the EmailService
										builder.Services.AddScoped<IEmailSender, EmailSender>();
							
										//Add Configure options
										builder.Services.Configure<FormOptions>(o => {
											o.ValueLengthLimit = int.MaxValue;
											o.MultipartBodyLengthLimit = int.MaxValue;
											o.MemoryBufferThreshold = int.MaxValue;
										});
							
										....
									}
								}
							}
						```
						
					- Test con Postman
						- Seleccionar el método Post method
						- Seleccionar Body
							- Form Type seleccionar File