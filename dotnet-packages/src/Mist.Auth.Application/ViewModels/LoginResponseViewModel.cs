namespace Mist.Auth.Application.ViewModels
{
    public class LoginResponseViewModel
    {
        public string AccessToken { get; set; }
        public UserTokenViewModel UserToken { get; set; }
    }

    public class UserTokenViewModel
    {
        public string Email { get; set; }
    }
}
