using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SixLabors.ImageSharp;


namespace Rise.Server
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;

        public MinioService()
        {
            _minioClient = new MinioClient()
                .WithEndpoint("s3.360zorg.me")
                .WithCredentials("pdfEYGFrqdP3iwhRsVrW", "txYOMuvcuXlI9ABudm6JMteqJjvaGtm52cXpuH6z")
                .WithSSL()
                .Build();
        }

        // Methode om een afbeelding te uploaden met een GUID
        public async Task<string> UploadImageAsync(string bucketName, Stream imageStream)
        {
            try
            {
                string objectName = Guid.NewGuid().ToString() + ".webp"; 

                byte[] imageData = ConvertImageToWebP(imageStream);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(new MemoryStream(imageData))
                    .WithObjectSize(imageData.Length)
                    .WithContentType("image/webp");

                await _minioClient.PutObjectAsync(putObjectArgs);
                Console.WriteLine($"Afbeelding geüpload met naam: {objectName}");

                string imageUrl = $"https://s3.360zorg.me/{bucketName}/{objectName}";

                return imageUrl;
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Minio fout: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
                throw;
            }
        }

        private byte[] ConvertImageToWebP(Stream imageStream)
        {
            using (var originalImage = Image.Load(imageStream))
            {
                using (var memoryStream = new MemoryStream())
                {
                    originalImage.SaveAsWebp(memoryStream);
                    return memoryStream.ToArray(); 
                }
            }
        }

        // Methode om een bucket aan te maken
        public async Task CreateBucketAsync(string bucketName)
        {
            try
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Minio fout bij het aanmaken van de bucket: {ex.Message}");
            }
        }

        // Methode om te controleren of een bucket bestaat
        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            try
            {

                return await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Minio fout bij het controleren van de bucket: {ex.Message}");
                return false;
            }
        }
    }
}


