namespace ShopAppBackend.Settings
{
    public class ImageSettings : IImageSettings
    {
        public int MaxWidth { get; set; }

        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }

        public string BlobPath { get; set; }
    }

    public interface IImageSettings
    {
        public int MaxWidth { get; set; }

        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }

        public string BlobPath { get; set; }
    }
}
