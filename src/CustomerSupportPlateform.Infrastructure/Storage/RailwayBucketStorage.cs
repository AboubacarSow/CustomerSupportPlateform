using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomerSupportPlateform.Infrastructure.Storage;
internal class RailwayBucketStorage : IBlobStorage
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<RailwayBucketStorage> _logger;
    private const string uploadFolder = "Knowledges";

    public RailwayBucketStorage(IAmazonS3 amazonS3,
        IConfiguration configuration,
        ILogger<RailwayBucketStorage> logger)
    {
        _s3Client = amazonS3;
        _bucketName = configuration["BucketSettings:BucketName"]!;
        _logger = logger;
    }
    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = storageKey
            };
            await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting object from S3");
            throw;
        }
    }
    public async Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = storageKey
            };

            var response = await _s3Client.GetObjectAsync(getRequest,cancellationToken);
            return response.ResponseStream;

        }catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error occurred while downloading object from S3");
            throw;
        }
       
    }

    public async Task<(string,long,string)> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{uploadFolder}/{fileName}";
            var objectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                ContentType = contentType,
                InputStream = stream,
                Key = key
            };

            var response = await _s3Client.PutObjectAsync(objectRequest, cancellationToken);

            return (key, stream.Length,contentType);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error occurred while downloading object from S3");
            throw;
        }
        
    }
}



