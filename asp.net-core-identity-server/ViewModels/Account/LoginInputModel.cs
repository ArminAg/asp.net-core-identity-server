using System.ComponentModel.DataAnnotations;

namespace asp.net_core_identity_server.ViewModels.Account
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}