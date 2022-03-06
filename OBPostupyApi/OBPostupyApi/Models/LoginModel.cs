namespace OBPostupyApi.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsPermanent { get; set; }
    }
}
