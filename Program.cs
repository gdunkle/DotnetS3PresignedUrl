using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;

namespace DotnetS3PresignedUrl
{

    class Program
    {

        // Specify how long the presigned URL lasts, in hours
        private const double timeoutDuration = 12;
        private static IAmazonS3 s3Client;

        public static void Main(string[] args)
        {
            var profile = new Amazon.Runtime.SessionAWSCredentials(System.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"), System.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"), System.Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN"));

            string regionName = args[0];
            double duration = Convert.ToDouble(args[1]);
            string bucket = args[2];
            string key = args[3];
            string prefixedKey = DateTimeOffset.Now.ToUnixTimeSeconds() + "/" + key;
            var region = RegionEndpoint.GetBySystemName(regionName);
            s3Client = new AmazonS3Client(profile, region);
            PutBucketResponse response = UploadFile(bucket, key, prefixedKey).Result;
            string urlString = GeneratePreSignedURL(duration, bucket, prefixedKey);
            Console.WriteLine(urlString);
        }

        static async Task<PutBucketResponse> UploadFile(string bucketName, string file, string objectKey)
        {

            try
            {
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    FilePath = file
                };

                await s3Client.PutObjectAsync(putRequest1);

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return null;

        }

        static string GeneratePreSignedURL(double duration, string bucketName, string objectKey)
        {

            string urlString = "";
            string prefixedKey = DateTimeOffset.Now.ToUnixTimeSeconds() + "/" + objectKey;
            try
            {

                GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddHours(duration)
                };
                urlString = s3Client.GetPreSignedURL(request1);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return urlString;
        }
    }
}
