using ShopAppBackend.Settings.Interfaces;

namespace ShopAppBackend.Settings
{
    public class UserSettings : IUserSettings
    {
        public string Secret { get; set; }

        public string PasswordSalt { get; set; }
    }
}
