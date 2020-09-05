using System.Collections.Generic;

namespace Mist.Auth.Application.ViewModels
{
    public class LoginResponseViewModel
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public JwtDataViewModel Data { get; set; }
    }
}
