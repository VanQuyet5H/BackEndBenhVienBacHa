namespace Camino.Core.Configuration
{
    public class S3UploadConfig
    {
        public string SourceTemporaryBucket { get; set; }
        public string DestinationBucket { get; set; }
        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessId { get; set; }
        public int ExpirationTimeForUploadFile { get; set; }
        public int ExpirationTimeForSharedFile { get; set; }
    }
}