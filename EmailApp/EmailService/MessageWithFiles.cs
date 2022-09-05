using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
