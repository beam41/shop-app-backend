namespace ShopAppBackend.Settings.Interfaces
{
    public interface IUserSettings
    {
        public string Secret { get; set; }

        public string PasswordSalt { get; set; }
    }
}
