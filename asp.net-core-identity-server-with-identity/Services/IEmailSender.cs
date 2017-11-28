using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net_core_identity_server_with_identity.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
