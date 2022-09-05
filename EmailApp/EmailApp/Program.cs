using EmailService;
using Microsoft.AspNetCore.Http.Features;

namespace EmailApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Recover data from appsettings.json
            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            // Add services to the container.
            builder.Services.AddSingleton(emailConfig);

            //Register the EmailService
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            //Add Configure options
            builder.Services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}