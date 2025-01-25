using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.TiemChung;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class TiemChungController
    {
        [HttpPost("LuuThongTinTheoDoi")]
        public async Task<ActionResult> LuuThongTinTheoDoi([FromBody]TiemChungTheoDoiSauTiemViewModel tiemChungTheoDoiSauTiemViewModel)
        {
            var yeuCauTiepNhan = await _tiemChungService.GetByIdAsync(tiemChungTheoDoiSauTiemViewModel.YeuCauTiepNhanId, o => o.Include(p => p.YeuCauDichVuKyThuats)
                                                                                                                               .ThenInclude(p => p.KhamSangLocTiemChung));

            var yeuCauDichVuKyThuatKhamSangLocSauTiem = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(p => p.Id == tiemChungTheoDoiSauTiemViewModel.Id)
                                                                                           .Select(p => p.KhamSangLocTiemChung)
                                                                                           .FirstOrDefault();

            if(yeuCauDichVuKyThuatKhamSangLocSauTiem == null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            tiemChungTheoDoiSauTiemViewModel.ToEntity(yeuCauDichVuKyThuatKhamSangLocSauTiem);

            await _tiemChungService.UpdateAsync(yeuCauTiepNhan);

            return Ok();
        }

        [HttpPost("GetNhanViens")]
        public async Task<ActionResult> GetNhanViens([FromBody]DropDownListRequestModel model)
        {
            var lstNhanVien = await _tiemChungService.GetNhanViensAsync(model);

            return Ok(lstNhanVien);
        }

        [HttpPost("GetPhanUngSauTiems")]
        public ActionResult GetPhanUngSauTiems([FromBody]DropDownListRequestModel model)
        {
            var lstPhanUngSauTiem = _tiemChungService.GetPhanUngSauTiems(model);

            return Ok(lstPhanUngSauTiem);
        }

        [HttpPost("GetNoiXuTris")]
        public ActionResult GetNoiXuTris([FromBody]DropDownListRequestModel model)
        {
            var lstNoiXuTri = _tiemChungService.GetNoiXuTris(model);

            return Ok(lstNoiXuTri);
        }

        [HttpPost("GetTinhTrangHienTais")]
        public ActionResult GetTinhTrangHienTais([FromBody]DropDownListRequestModel model)
        {
            var lstTinhTrangHienTai = _tiemChungService.GetTinhTrangHienTais(model);

            return Ok(lstTinhTrangHienTai);
        }
    }
}