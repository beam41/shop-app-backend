namespace ShopAppBackend.Settings
{
    public interface IUserSettings
    {
        public string Secret { get; set; }

        public string PasswordSalt { get; set; }
    }
}