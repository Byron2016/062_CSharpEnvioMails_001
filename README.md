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

