using Camino.Api.Auth;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;
using System.Linq;
using Camino.Api.Models.Error;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {

        [HttpPost("GetDataForGridAsyncTruyenMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncTruyenMau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetDataForGridAsyncTruyenMau(queryInfo);
            return Ok(gridData);
        }
        #region BVHD-3959
        [HttpPost("ApDungThoiGianDienBienTruyenMaus")]
        public async Task<ActionResult> ApDungThoiGianDienBienTruyenMaus([FromBody] ApDungThoiGianDienBienTruyenMauVo apDungThoiGianDienBienTruyenMauVo)
        {
            var yeuCauTruyenMauIds = apDungThoiGianDienBienTruyenMauVo.DataGridDichVuChons.Where(o => o.ChecBoxItem).Select(o => o.Id).ToList();
            if (!yeuCauTruyenMauIds.Any())
            {
                throw new ApiException("Vui lòng chọn yêu cầu truyền máu để áp dụng");
            }
            _dieuTriNoiTruService.ApDungThoiGianDienBienTruyenMau(yeuCauTruyenMauIds, apDungThoiGianDienBienTruyenMauVo.ThoiGianDienBien);
            return Ok(true);
        }
        #endregion

        [HttpPost("GetTotalPageForGridAsyncTruyenMau")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncTruyenMau([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dieuTriNoiTruService.GetTotalPageForGridAsyncTruyenMau(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetMauVaChePham")]
        public async Task<ActionResult<ICollection<MauVaChePhamTemplateVo>>> GetMauVaChePham(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetMauVaChePham(model);
            return Ok(lookup);
        }


        [HttpPost("GetNhomMauRH")]
        public async Task<ActionResult<ICollection<NhomMauTemplateVo>>> GetNhomMauRH(DropDownListRequestModel model)
        {
            var lookup = await _dieuTriNoiTruService.GetNhomMauRH(model);
            return Ok(lookup);
        }

        #region CRUD YeuCauTruyenMau
        [HttpPost("ThemYeuCauTruyenMau")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> ThemYeuCauTruyenMau(DieuTriNoiTruPhieuDieuTriTruyenMauViewModel truyenMauViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(truyenMauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(truyenMauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(truyenMauViewModel.YeuCauTiepNhanId,
                x => x.Include(a => a.NoiTruBenhAn).ThenInclude(a => a.NoiTruPhieuDieuTris)
                             .Include(a => a.YeuCauTruyenMaus));

            var truyenMauVo = new PhieuDieuTriTruyenMauVo
            {
                Id = truyenMauViewModel.Id,
                YeuCauTiepNhanId = truyenMauViewModel.YeuCauTiepNhanId,
                MauVaChePhamId = truyenMauViewModel.MauVaChePhamId.Value,
                NhomMau = truyenMauViewModel.NhomMau,
                YeuToRh = truyenMauViewModel.YeuToRh,
                TheTich = truyenMauViewModel.TheTich,
                ThoiGianBatDauTruyen = truyenMauViewModel.ThoiGianBatDauTruyen
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.ThemYeuCauTruyenMau(truyenMauVo, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);


            return NoContent();
        }


        [HttpPost("CapNhatYeuCauTruyenMau")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatYeuCauTruyenMau(DieuTriNoiTruPhieuDieuTriTruyenMauViewModel truyenMauViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(truyenMauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(truyenMauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(truyenMauViewModel.YeuCauTiepNhanId,
                x => x.Include(a => a.YeuCauTruyenMaus));

            var truyenMauVo = new PhieuDieuTriTruyenMauVo
            {
                Id = truyenMauViewModel.Id,
                YeuCauTiepNhanId = truyenMauViewModel.YeuCauTiepNhanId,
                MauVaChePhamId = truyenMauViewModel.MauVaChePhamId.Value,
                NhomMau = truyenMauViewModel.NhomMau,
                YeuToRh = truyenMauViewModel.YeuToRh,
                TheTich = truyenMauViewModel.TheTich,
                ThoiGianBatDauTruyen = truyenMauViewModel.ThoiGianBatDauTruyen
            };

            //Xử lý thêm yêu cầu dược phẩm bệnh viện
            await _dieuTriNoiTruService.CapNhatYeuCauTruyenMau(truyenMauVo, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }


        [HttpPost("XoaYeuCauTruyenMau")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> XoaYeuCauTruyenMau(DieuTriNoiTruPhieuDieuTriTruyenMauViewModel truyenMauViewModel)
        {
            await _dieuTriNoiTruService.KiemTraThoiDiemXuatVienBenhAn(truyenMauViewModel.YeuCauTiepNhanId);

            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(truyenMauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(truyenMauViewModel.YeuCauTiepNhanId,
                x => x.Include(a => a.YeuCauTruyenMaus));

            await _dieuTriNoiTruService.XoaYeuCauTruyenMau(truyenMauViewModel.Id, yeuCauTiepNhan);

            //Gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            return NoContent();
        }

       

        [HttpPost("InPhieuTruyenMau")]
        public async Task<ActionResult<string>> InPhieuTruyenMau(XacNhanInPhieuTruyenMau xacNhanIn)
        {
            var phieuIn = await _dieuTriNoiTruService.InPhieuTruyenMau(xacNhanIn);
            return phieuIn;
        }

        #endregion
    }
}
