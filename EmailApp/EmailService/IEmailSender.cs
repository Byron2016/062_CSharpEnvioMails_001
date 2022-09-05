using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        void SendHtmlEmail(Message message);

        Task SendEmailAsync(Message message);

        Task SendEmailWithAttachnents(MessageWithFiles message);
    }
}
