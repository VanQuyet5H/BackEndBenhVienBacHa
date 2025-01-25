using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models;
using Camino.Api.Models.Error;
using Camino.Core.Domain.Entities.NoiDungMauLoiDanBacSi;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Microsoft.AspNetCore.Mvc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        #region thêm/xóa/sửa nội dung mẫu lời dặn bác sĩ
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridNoiDungMauLoiDanBacSi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridNoiDungMauLoiDanBacSiAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridNoiDungMauLoiDanBacSiAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridNoiDungMauLoiDanBacSi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridNoiDungMauLoiDanBacSiAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridNoiDungMauLoiDanBacSiAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("GetThongTinNoiDungMauLoiDanBacSi")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<NoiDungMauLoiDanBacSiViewModel>> GetThongTinNoiDungMauLoiDanBacSi(long id)
        {
            var result = await _noiDungMauLoiDanBacSiService.GetByIdAsync(id);
            return result.ToModel<NoiDungMauLoiDanBacSiViewModel>();
        }

        [HttpPost("GetListNoiDungMau")]
        public async Task<ActionResult<ICollection<NoiDungMauLookupItemVo>>> GetListNoiDungMauAsync(
            DropDownListRequestModel model , long LoaiBenhAn)
        {
            var lookup = await _noiDungMauLoiDanBacSiService.GetListNoiDungMauAsync(model, LoaiBenhAn);
            return Ok(lookup);
        }


        [HttpPost("LuuNoiDungMauLoiDanBacSi")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuNoiDungMauLoiDanBacSi([FromBody] NoiDungMauLoiDanBacSiViewModel viewModel)
        {
            if (viewModel.Id != 0)
            {
                var noiDungMauLoiDanBS = await _noiDungMauLoiDanBacSiService.GetByIdAsync(viewModel.Id);
                viewModel.ToEntity(noiDungMauLoiDanBS);
                await _noiDungMauLoiDanBacSiService.UpdateAsync(noiDungMauLoiDanBS);
            }
            else
            {
                var noiDungMauLoiDanBS = viewModel.ToEntity<NoiDungMauLoiDanBacSi>();
                await _noiDungMauLoiDanBacSiService.AddAsync(noiDungMauLoiDanBS);
            }

            return Ok();
        }

        [HttpDelete("XoaNoiDungMauLoiDanBacSi")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<NoiDungMauLoiDanBacSiViewModel>> XoaNoiDungMauLoiDanBacSi(long id)
        {
            var result = await _noiDungMauLoiDanBacSiService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            await _noiDungMauLoiDanBacSiService.DeleteByIdAsync(id);
            return Ok();
        }

        #endregion

        #region Kiểm tra loại bệnh án 

        [HttpGet("GetThongTinLoaiBenhAn/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<int> GetThongTinLoaiBenhAn(long yeuCauTiepNhanId)
        {
            var loaiThongTinBenhAn = _dieuTriNoiTruService.GetThongTinLoaiBenhAn(yeuCauTiepNhanId);
            return Ok(loaiThongTinBenhAn);
        }

        #endregion
        
        #region Lấy thông bệnh án trẻ sơ sinh

        [HttpGet("GetThongTinBenhAnTreSoSinh/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnTreSoSinh(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnNoiKhoaNhi = _dieuTriNoiTruService.GetThongTinBenhAnTreSoSinh(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnNoiKhoaNhi);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnTreSoSinh")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnTreSoSinh(ThongTinBenhAn thongTin)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTin.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnTreSoSinh(thongTin);
            return NoContent();
        }

        #endregion

        #region Lấy thông tin bệnh án nội khoa

        [HttpGet("GetThongTinBenhAnNoiKhoaNhi/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnNoiKhoaNhi(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnNoiKhoaNhi = _dieuTriNoiTruService.GetThongTinBenhAnNoiKhoaNhi(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnNoiKhoaNhi);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnNoiKhoaNhi")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnNoiKhoaNhi(ThongTinBenhAn thongTin)
        {
            KiemTraChuanDoanPhanBiet(thongTin);
            KiemTraChuanKemTheo(thongTin);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTin.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnNoiKhoaNhi(thongTin);
            return NoContent();
        }

        #endregion

        #region Lấy thông tin bệnh án nhi khoa

        [HttpGet("GetThongTinBenhAnNhiKhoa/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnNhiKhoa(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnNoiKhoaNhi = _dieuTriNoiTruService.GetThongTinBenhAnNhiKhoa(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnNoiKhoaNhi);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnNhiKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnNhiKhoa(ThongTinBenhAn thongTin)
        {          
            KiemTraChuanDoanPhanBiet(thongTin);
            KiemTraChuanKemTheo(thongTin);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTin.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnNhiKhoa(thongTin);
            return NoContent();
        }


        #endregion

        #region Lấy thông tin bệnh án phụ khoa

        [HttpGet("GetThongTinBenhAnPhuKhoa/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnPhuKhoa(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnPhuKhoa = _dieuTriNoiTruService.GetThongTinBenhAnPhuKhoa(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnPhuKhoa);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnPhuKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnPhuKhoa(ThongTinBenhAn thongTinBenhAnPhuKhoa)
        {
            KiemTraChuanDoanPhanBiet(thongTinBenhAnPhuKhoa);
            KiemTraChuanKemTheo(thongTinBenhAnPhuKhoa);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTinBenhAnPhuKhoa.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnPhuKhoa(thongTinBenhAnPhuKhoa);
            return NoContent();
        }

        #endregion

        #region Lấy thông tin bệnh án ngoại khoa

        [HttpGet("GetThongTinBenhAnNgoaiKhoa/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnNgoaiKhoa(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnPhuKhoa = _dieuTriNoiTruService.GetThongTinBenhAnNgoaiKhoa(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnPhuKhoa);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnNgoaiKhoa")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnNgoaiKhoa(ThongTinBenhAn thongTinBenhAnNgoaiKhoa)
        {
            KiemTraChuanDoanPhanBiet(thongTinBenhAnNgoaiKhoa);
            KiemTraChuanKemTheo(thongTinBenhAnNgoaiKhoa);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTinBenhAnNgoaiKhoa.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnNgoaiKhoa(thongTinBenhAnNgoaiKhoa);
            return NoContent();
        }

        #endregion

        #region Lấy thông tin bệnh án sản khoa mỗ thường
      
        [HttpGet("GetThongTinBenhAnSanKhoaMo/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnSanKhoaMo(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnPhuKhoa = _dieuTriNoiTruService.GetThongTinBenhAnSK(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnPhuKhoa);
        }

        [HttpPost("LuuHoacCapNhatThongTinBASanKhoaMo")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBASanKhoaMo(ThongTinBenhAn thongTinBenhAnNgoaiKhoa)
        {
            KiemTraChuanDoanPhanBiet(thongTinBenhAnNgoaiKhoa);
            KiemTraChuanKemTheo(thongTinBenhAnNgoaiKhoa);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTinBenhAnNgoaiKhoa.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnSK(thongTinBenhAnNgoaiKhoa);
            return NoContent();
        }
      
        [HttpGet("GetThongTinBenhAnSanKhoaMoThuong/{yeuCauTiepNhanId}")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public ActionResult<ThongTinBenhAn> GetThongTinBenhAnSanKhoaMoThuong(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnPhuKhoa = _dieuTriNoiTruService.GetThongTinBenhAnSanKhoaMoThuong(yeuCauTiepNhanId);
            return Ok(thongTinBenhAnPhuKhoa);
        }

        [HttpPost("LuuHoacCapNhatThongTinBenhAnSanKhoaMoThuong")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> LuuHoacCapNhatThongTinBenhAnSanKhoaMoThuong(ThongTinBenhAn thongTin)
        {          
            KiemTraChuanDoanPhanBiet(thongTin);
            KiemTraChuanKemTheo(thongTin);

            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(thongTin.YeuCauTiepNhanId);

            _dieuTriNoiTruService.LuuHoacCapNhatThongTinBenhAnSanKhoaMoThuong(thongTin);
            return NoContent();
        }

        #endregion        

        private void KiemTraChuanDoanPhanBiet(ThongTinBenhAn thongTinBenhAn)
        {
            //kiểm tra chẩn đoán phân biệt
            var lstChanDoanPhanBietId = thongTinBenhAn.ChuanDoanPhanBiets.Where(x => x.Id == 0).Select(x => x.ICD).ToList();
            if (lstChanDoanPhanBietId.Count() != lstChanDoanPhanBietId.Distinct().Count())
            {
                throw new ApiException("Chuẩn đoán phân biệt không được trùng");
            }
        }

        private void KiemTraChuanKemTheo(ThongTinBenhAn thongTinBenhAn)
        {
            //kiểm tra chẩn đoán phân biệt
            var lstChanDoankemTheoId = thongTinBenhAn.ChuanDoanKemTheos.Where(x => x.Id == 0).Select(x => x.ICD).ToList();
            if (lstChanDoankemTheoId.Count() != lstChanDoankemTheoId.Distinct().Count())
            {
                throw new ApiException("Chuẩn đoán kèm theo không được trùng");
            }
        }

        #region InPhieuKhamBenhNoiTru
        [HttpPost("InPhieuKhamBenhNoiTru")]
        public ActionResult InPhieuKhamBenhNoiTru(long noiTruBenhAnId)
        {
            var result = _dieuTriNoiTruService.InPhieuKhamBenhNoiTru(noiTruBenhAnId);
            return Ok(result);
        }
        #endregion
    }
}
