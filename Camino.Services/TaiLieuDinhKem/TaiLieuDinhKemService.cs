using Camino.Core.DependencyInjection.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Camino.Core.Configuration;
using Camino.Core.Helpers;

namespace Camino.Services.TaiLieuDinhKem
{
	public class TaiLieuDinhKemResponse
	{
		public string UploadUrl { get; set; }
		//public string SaveUrl { get; set; }
		public string Ten { get; set; }
		public string TenGuid { get; set; }
		public string DuongDan { get; set; }
		public string DuongDanTmp { get; set; }
		public long KichThuoc { get; set; }
		public int LoaiTapTin { get; set; }
        public string MoTa { get; set; }
	}

	public class LuuTaiLieuDinhKemResponse
	{
		public string DuongDan { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }
	}

	public class TaiLieuDinhKemRequest
	{
		public string Ten { get; set; }
		public string DuongDan { get; set; }
		public long KichThuoc { get; set; }
		public string PhanMoRong { get; set; }

		public int LoaiTapTin { get; set; }

		public string ContentType => MimeTypeMap.GetMimeTypeFromFileName(Ten);
	}

	public enum LoaiTaiLieuDinhKem
	{
		BangPhiGuiXeCongTy = 1,
		BangPhiGuiXeCaNhan
	}

	[SingletonDependencyAttribute(ServiceType = typeof(ITaiLieuDinhKemService))]
	public class TaiLieuDinhKemService : ITaiLieuDinhKemService
	{
		private readonly S3UploadConfig _s3UploadConfig;
	    private readonly FileTypeConfig _fileTypeConfig;

        //      private static string SourceTemporaryBucket = "hapulico.filetmp";
        //private const string DestinationBucket = "hapulico.storage/dsfdf/";
        //private const string AwsAccessKeyId = "AKIAW6INU5MFUET2AW53";
        //private const string AwsSecretAccessId = "zUrfQug72DI1qZg2Bbpoiv2Tq6KUlpNqWtSrrGNO";


        private static int _expirationTimeForUploadFile;
		private static int _expirationTimeForSharedFile;
		private static readonly RegionEndpoint BucketRegion = RegionEndpoint.APSoutheast1;
		private static IAmazonS3 _s3Client;

		public TaiLieuDinhKemService(S3UploadConfig s3UploadConfig, FileTypeConfig fileTypeConfig)
		{
			_s3UploadConfig = s3UploadConfig;
		    _fileTypeConfig = fileTypeConfig;
            //initializer
            _expirationTimeForUploadFile = _s3UploadConfig.ExpirationTimeForUploadFile;//Day
			_expirationTimeForSharedFile = _s3UploadConfig.ExpirationTimeForSharedFile;
			_s3Client = new AmazonS3Client(_s3UploadConfig.AwsAccessKeyId, _s3UploadConfig.AwsSecretAccessId, BucketRegion);


		}

		private string GetBucketNameByLoaiTaiLieuDinhKem(LoaiTaiLieuDinhKem loaiTaiLieuDinhKem)
		{
			switch (loaiTaiLieuDinhKem)
			{
				case LoaiTaiLieuDinhKem.BangPhiGuiXeCongTy:
					return $"{_s3UploadConfig.DestinationBucket}/BangPhiGuiXeCongTy/";
				case LoaiTaiLieuDinhKem.BangPhiGuiXeCaNhan:
					return $"{_s3UploadConfig.DestinationBucket}/BangPhiGuiXeCaNhan/";
				default:
					return _s3UploadConfig.DestinationBucket;
			}
		}

		//need implement CloudFront for generating SaveURL
		//private string GenerateSaveUrl(string bucketLocation, string fileName) =>
		//	$"https://s3-{bucketLocation}.amazonaws.com/{DestinationBucket}/{fileName}";

		private async Task<PutBucketResponse> CreateBucketAsync(string bucketName)
		{

			if (!(await AmazonS3Util.DoesS3BucketExistAsync(_s3Client, bucketName)))
			{
				var putBucketRequest = new PutBucketRequest
				{
					BucketName = bucketName,
					UseClientRegion = true
				};
				return await _s3Client.PutBucketAsync(putBucketRequest);
			}
			return null;
		}

		public async Task<bool> CreateFolderBucketAsync(string folderPath)
		{
			if (!(await DoesFolderExistsAsync(folderPath)))
			{
				var putObjectResponse = await _s3Client.PutObjectAsync(
					new PutObjectRequest()
					{
						BucketName = _s3UploadConfig.DestinationBucket,
						Key = folderPath + "/",
						ContentType = null,
						StorageClass = S3StorageClass.Standard
					}
				);
				if (putObjectResponse.HttpStatusCode == HttpStatusCode.OK)
					return true;
			}
			return false;
			//create
		}
		private async Task<bool> DoesFolderExistsAsync(string folderPath)
		{
			ListObjectsV2Request request = new ListObjectsV2Request();
			request.BucketName = _s3UploadConfig.DestinationBucket;
			request.Prefix = folderPath + "/";
			request.MaxKeys = 1;
			ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(request);
			return response.S3Objects.Any();
		}

		private async Task<string> FindBucketLocationAsync(string bucketName)
		{
			GetBucketLocationResponse response = await _s3Client.GetBucketLocationAsync(new GetBucketLocationRequest()
			{
				BucketName = bucketName
			});
			if (response.HttpStatusCode == HttpStatusCode.OK)
				return response.Location.ToString();
			return "";
		}

		public string GetTaiLieuUrl(string duongDan, string tenGuid)
		{
			return _s3Client.GeneratePreSignedURL(duongDan, tenGuid, DateTime.UtcNow.AddMinutes(_expirationTimeForSharedFile).ToLocalTime(), null);
		}

        public Stream GetObjectStream(string duongDan, string tenGuid)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = duongDan,
                Key = tenGuid
            };
            return _s3Client.GetObjectAsync(request).Result.ResponseStream;
        }

        public async Task<string> GetFileStreamReaderAsync(string duongDan, string tenGuid)
		{
			GetObjectRequest request = new GetObjectRequest
			{
				BucketName = duongDan,
				Key = tenGuid
			};
			using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
			using (Stream responseStream = response.ResponseStream)
			using (StreamReader reader = new StreamReader(responseStream))
			{
				//string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
				//string contentType = response.Headers["Content-Type"];

				//using (var memstream = new MemoryStream())
				//{
	   //             #region MyRegion

				//    var text = "vule";
				//    Bitmap bitmap = new Bitmap(200, 30, System.Drawing.Imaging.PixelFormat.Format64bppArgb);
				//    Graphics graphics = Graphics.FromImage(bitmap);
				//    graphics.Clear(Color.White);
				//    graphics.DrawString(text, new System.Drawing.Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Red), new PointF(0.4F, 2.4F));
				//    bitmap.Save(Path.Combine("~/Image.jpg"), ImageFormat.Jpeg);
				//    bitmap.Dispose();
				//    var img = iTextSharp.text.Image.GetInstance(Path.Combine("~/Image.jpg"));
				//    img.SetAbsolutePosition(200, 400);

	   //             #endregion
	   //             var buffer = new byte[512];
				//    var bytesRead = default(int);
				//    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
				//        memstream.Write(buffer, 0, bytesRead);
				//    // Perform image manipulation 
				//    var transformedImage = memstream.ToArray();

				//    PdfReader pdfReader = new PdfReader(transformedImage);
				//    PdfContentByte waterMark;

	   //             using (PdfStamper stamper = new PdfStamper(pdfReader, memstream))
				//    {
				//        int pages = pdfReader.NumberOfPages;
				//        for (int i = 1; i <= pages; i++)
				//        {
				//            waterMark = stamper.GetUnderContent(i);
				//            waterMark.AddImage(img);
				//        }
				//    }
				//    var bytes = memstream.ToArray();


	   //             using (FileStream source = File.Open(@"E:\vule.pdf",
				//        FileMode.Create))
				//    {

				//        Console.WriteLine("Source length: {0}", source.Length.ToString());

				//        // Copy source to destination.
				//        source.Write(transformedImage);
				//    }

				//    File.Delete(Path.Combine("~/Image.jpg"));
	   //             //PutObjectRequest putRequest = new PutObjectRequest()
	   //             //{
	   //             //    BucketName = DestBucket,
	   //             //    Key = $"grayscale-{s3Event.Object.Key}",
	   //             //    ContentType = rs.Headers.ContentType,
	   //             //    ContentBody = transformedImage
	   //             //};
	   //             //await S3Client.PutObjectAsync(putRequest);
	   //         }

				return reader.ReadToEnd();
			}
		}

		public TaiLieuDinhKemResponse GetPreSignedUploadFileUrl(TaiLieuDinhKemRequest uploadFileRequest)
		{
			var key = $"{Guid.NewGuid()}.{uploadFileRequest.PhanMoRong}";
			var url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
			{
				BucketName = _s3UploadConfig.SourceTemporaryBucket,
				Expires = DateTime.UtcNow.AddDays(_expirationTimeForUploadFile).ToLocalTime(),
				Key = key,
				Headers =
					{
						ContentType = uploadFileRequest.ContentType, // "application/x-www-form-urlencoded; charset=UTF-8",
						ContentLength = uploadFileRequest.KichThuoc
					},
				ContentType = uploadFileRequest.ContentType,
				Verb = HttpVerb.PUT,
			});
			return new TaiLieuDinhKemResponse()
			{
				UploadUrl = url,
				Ten = uploadFileRequest.Ten,
				TenGuid = key,
				DuongDan = _s3UploadConfig.DestinationBucket, // + ....,
				DuongDanTmp = _s3UploadConfig.SourceTemporaryBucket,
				KichThuoc = uploadFileRequest.KichThuoc,
				LoaiTapTin = FileHelper.GetLoaiTapTin(uploadFileRequest.PhanMoRong, _fileTypeConfig)
            };

		}

		public async Task<LuuTaiLieuDinhKemResponse> LuuTaiLieuDinhKemAsync(LoaiTaiLieuDinhKem loaiTaiLieuDinhKem, string tenGuid, Stream content)
		{
			var bucketName = GetBucketNameByLoaiTaiLieuDinhKem(loaiTaiLieuDinhKem);
			PutObjectRequest putRequest = new PutObjectRequest()
			{
				BucketName = bucketName,
				Key = tenGuid,
				ContentType = MimeTypeMap.GetMimeTypeFromFileName(tenGuid),
				InputStream = content,
			};

			var response = await _s3Client.PutObjectAsync(putRequest);
			return new LuuTaiLieuDinhKemResponse{DuongDan = bucketName,HttpStatusCode = response.HttpStatusCode};
		}

	    public async Task<LuuTaiLieuDinhKemResponse> LuuTaiLieuDinhKemAsync(LoaiTaiLieuDinhKem loaiTaiLieuDinhKem, string tenGuid, string filePath)
	    {
	        var bucketName = GetBucketNameByLoaiTaiLieuDinhKem(loaiTaiLieuDinhKem);
	        PutObjectRequest putRequest = new PutObjectRequest()
	        {
	            BucketName = bucketName,
	            Key = tenGuid,
	            ContentType = MimeTypeMap.GetMimeTypeFromFileName(tenGuid),
	            FilePath = filePath,
	        };

	        var response = await _s3Client.PutObjectAsync(putRequest);
	        return new LuuTaiLieuDinhKemResponse { DuongDan = bucketName, HttpStatusCode = response.HttpStatusCode };
	    }

        public async Task<CopyObjectResponse> LuuTaiLieuAsync(string duongDan, string tenGuid)
		{
			
			var copyResponse = await _s3Client.CopyObjectAsync(new CopyObjectRequest
			{
				SourceBucket = _s3UploadConfig.SourceTemporaryBucket,
				SourceKey = tenGuid,
				DestinationBucket = duongDan,
				DestinationKey = tenGuid,
			});
			return copyResponse;
		}
		public async Task<DeleteObjectResponse> XoaTaiLieuAsync(string duongDan, string tenGuid)
		{
			var deleteResponse = await _s3Client.DeleteObjectAsync(new DeleteObjectRequest()
			{
				BucketName = duongDan,
				Key = tenGuid,
			});
			return deleteResponse;
		}
		
		public async Task<CopyObjectResponse> CloneTaiLieuAsync(string tu, string tenGuid, string den, string tenGuidNew)
		{
			var cloneResponse = await _s3Client.CopyObjectAsync(new CopyObjectRequest
			{
				SourceBucket = tu,
				SourceKey = tenGuid,
				DestinationBucket = den,
				DestinationKey = tenGuidNew,
			});
			return cloneResponse;
		}

	}
}
