using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public class TaiLieuDinhKemController : CaminoBaseController
	{
		private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
		public TaiLieuDinhKemController(ITaiLieuDinhKemService taiLieuDinhKemService)
		{
			_taiLieuDinhKemService = taiLieuDinhKemService;
		}

		[HttpGet("GetPreSignedUploadFileUrl")]
		public ActionResult GetPreSignedUploadFileUrl(string ten, long kichThuoc, string phanMoRong, string duongDan)
		{
			var model = new TaiLieuDinhKemRequest()
			{
				Ten = ten,
				PhanMoRong = phanMoRong,
				KichThuoc = kichThuoc,
				DuongDan = duongDan
			};
			var uploadInfoResponse = _taiLieuDinhKemService.GetPreSignedUploadFileUrl(model);
			return Ok(uploadInfoResponse);
		}

		[HttpGet("GetTaiLieuUrl")]  
		public ActionResult GetTaiLieuUrl(string duongDan, string tenGuid)
		{ 
			var url = _taiLieuDinhKemService.GetTaiLieuUrl(duongDan, tenGuid);
			return Ok(url);
		}
	   
		[HttpPut("LuuTaiLieuAsync")]
		public async Task<ActionResult> LuuTaiLieuAsync([FromForm]string duongDan, [FromForm]string tenGuid)
		{
			var response = await _taiLieuDinhKemService.LuuTaiLieuAsync(duongDan, tenGuid);
			return Ok(response);
		}

		[HttpDelete("XoaTaiLieuAsync")]
		public async Task<ActionResult> XoaTaiLieuAsync([FromForm]string duongDan, [FromForm]string tenGuid)
		{
			await _taiLieuDinhKemService.XoaTaiLieuAsync(duongDan, tenGuid);
			return NoContent();
		}


		[HttpPost("CloneTaiLieuAsync")]
		public async Task<ActionResult> CloneTaiLieuAsync([FromForm]string tu, [FromForm]string tenGuid, [FromForm]string den, [FromForm]string tenGuidNew)
		{
			var response = await _taiLieuDinhKemService.CloneTaiLieuAsync(tu, tenGuid, den, tenGuidNew);

			return Ok(response);
		}

	}
}