namespace TotechsIdentity.AppSettings
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
        public string ImageContainer { get; set; } = string.Empty;
        public string ThumbnailContainer { get; set; } = string.Empty;
        public string BlobConnectionString { get; set; } = string.Empty;
    }
}
