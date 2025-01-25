using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace Camino.Services.TaiLieuDinhKem
{
    public interface ITaiLieuDinhKemService
	{
	    TaiLieuDinhKemResponse GetPreSignedUploadFileUrl(TaiLieuDinhKemRequest uploadFile);
	    string GetTaiLieuUrl(string duongDan, string tenGuid);
		Task<string> GetFileStreamReaderAsync(string duongDan, string tenGuid);
        Task<CopyObjectResponse> LuuTaiLieuAsync(string duongDan, string tenGuid);
        Task<DeleteObjectResponse> XoaTaiLieuAsync(string duongDan, string tenGuid);
		Task<bool> CreateFolderBucketAsync(string folderName);
		Task<CopyObjectResponse> CloneTaiLieuAsync(string tu, string tenGuid, string den, string tenGuidNew);
	    Task<LuuTaiLieuDinhKemResponse> LuuTaiLieuDinhKemAsync(LoaiTaiLieuDinhKem loaiTaiLieuDinhKem, string tenGuid, Stream content);
	    Task<LuuTaiLieuDinhKemResponse> LuuTaiLieuDinhKemAsync(LoaiTaiLieuDinhKem loaiTaiLieuDinhKem, string tenGuid, string filePath);
	    Stream GetObjectStream(string duongDan, string tenGuid);

	}
}
