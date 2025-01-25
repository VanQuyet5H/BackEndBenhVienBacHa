using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpPost("GetListNhomChucDanh")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListNhomChucDanh([FromBody] DropDownListRequestModel model)
        {
            var lstNhomChucDanh = _phauThuatThuThuatService.GetListNhomChucDanh(model);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListBacSiDieuDuong")]
        public async Task<ActionResult> GetListBacSiDieuDuong([FromBody]DropDownListRequestModel model, EnumNhomChucDanh nhomChucDanh)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListBacSiDieuDuong(model, nhomChucDanh);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListBacSiAutoComplete")]
        public async Task<ActionResult<string>> GetListBacSiAutoComplete([FromBody]DropDownListRequestModel model)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListBacSiAutoComplete(model);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListDieuDuongAutoComplete")]
        public async Task<ActionResult<string>> GetListDieuDuongAutoComplete([FromBody]DropDownListRequestModel model)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListDieuDuongAutoComplete(model);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListPhauThuatAutoComplete")]
        public async Task<ActionResult<string>> GetListPhauThuatAutoComplete([FromBody]DropDownListRequestModel model)
        {
            var lstNhomChucDanh = await _phauThuatThuThuatService.GetListPhauThuatAutoComplete(model);
            return Ok(lstNhomChucDanh);
        }

        [HttpPost("GetListVaiTroBacSi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListVaiTroBacSi([FromBody] DropDownListRequestModel model)
        {
            var lstVaiTroBacSi = _phauThuatThuThuatService.GetListVaiTroBacSi(model);
            return Ok(lstVaiTroBacSi);
        }

        [HttpPost("GetListVaiTroDieuDuong")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public ActionResult GetListVaiTroDieuDuong([FromBody] LookupQueryInfo model)
        {
            var lstVaiTroDieuDuong = _phauThuatThuThuatService.GetListVaiTroDieuDuong(model);
            return Ok(lstVaiTroDieuDuong);
        }

        [HttpPost("GetListPtttBn")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetListPtttBn([FromBody]LookupQueryInfo model, long noiThucHienId, long yctnId, bool? IsTuongTrinhLai)
        {
            var lstTuVong = await _phauThuatThuThuatService.GetListPtttBn(model, noiThucHienId, yctnId, IsTuongTrinhLai.GetValueOrDefault());
            return Ok(lstTuVong);
        }

        [HttpGet("LoadEkip")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> LoadEkip(long ycdvktId)
        {
            var ekips = await _phauThuatThuThuatService.LoadEkip(ycdvktId);
            return Ok(ekips);
        }

        [HttpPost("SaveEkip")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> SaveEkip([FromBody]EkipFormViewModel ekipModel)
        {
            var ycktPttt = await _yeuCauDichVuKyThuatService.GetByIdAsync(ekipModel.YcdvktId,
                x => x.Include(w => w.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .ThenInclude(w => w.PhauThuatThuThuatEkipBacSis)
                    .Include(w => w.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .ThenInclude(w => w.PhauThuatThuThuatEkipDieuDuongs)
                    .Include(w => w.PhongBenhVienHangDois));
            ekipModel.ToEntity(ycktPttt);
            ycktPttt.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;
            var phongBenhVienHangDoiEntity = ycktPttt.PhongBenhVienHangDois
                .FirstOrDefault(e =>
                    e.YeuCauTiepNhanId == ekipModel.YctnId && e.PhongBenhVienId == ekipModel.NoiThucHienId);

            if (phongBenhVienHangDoiEntity != null)
            {
                phongBenhVienHangDoiEntity.TrangThai = EnumTrangThaiHangDoi.DangKham;
            }

            //Cập nhật: bỏ đồng bộ thời gian thuê phòng và thời gian PTTT
            //if (ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT != null && ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat == null)
            //{
            //    //BVHD-3882
            //    var thuePhong = await _phauThuatThuThuatService.GetThongTinThuePhongTheoDichVuKyThuatAsync(ycktPttt.Id);
            //    if (thuePhong != null)
            //    {
            //        ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat = thuePhong.ThoiDiemBatDau;
            //        ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucPhauThuat = thuePhong.ThoiDiemKetThuc;
            //    }
            //    else
            //    {
            //        ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat = System.DateTime.Now;
            //    }
            //}

            //BVHD-3877
            if (ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
            {
                ycktPttt.YeuCauDichVuKyThuatTuongTrinhPTTT.GhiChuCaPTTT = ekipModel.GhiChuCaPTTT;
            }

            var bSi = ekipModel.Ekips.FirstOrDefault(e => e.VaiTroBacSi == EnumVaiTroBacSi.PhauThuatVienChinh || e.VaiTroDieuDuong == EnumVaiTroDieuDuong.PhauThuatVienChinh);

            if (bSi != null)
            {
                var bSiUserEntity = await _userService.GetByIdAsync(bSi.BacSiId.GetValueOrDefault());
                ycktPttt.NhanVienThucHienId = bSi.BacSiId.GetValueOrDefault();
                await _yeuCauDichVuKyThuatService.UpdateAsync(ycktPttt);
                return Ok(bSiUserEntity.HoTen);
            }
            await _yeuCauDichVuKyThuatService.UpdateAsync(ycktPttt);
            return Ok();
        }
    }
}