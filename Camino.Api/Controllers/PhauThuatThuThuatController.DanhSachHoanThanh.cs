using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpPost("TuongTrinhLai")]
        public async Task<ActionResult> TuongTrinhLai(TuongTrinhLai tuongTrinhLai)
        {
            await _phauThuatThuThuatService.TuongTrinhLai(tuongTrinhLai);
            return Ok();
        }

        [HttpPost("GetDanhSachDaPhauThuatThuThuatHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDaPhauThuatThuThuatHienTaiAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncDSHTPhauThuatThuThuat(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageDanhSachDaPhauThuatThuThuatHienTaiAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageDanhSachDaPhauThuatThuThuatHienTaiAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncDSHTPhauThuatThuThuat(queryInfo);
            return Ok(gridData);
        }

        #region Kết luận danh sách  người bệnh đã hoàn thành pttt

        [HttpGet("DanhSachHoanThanhPTTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> DanhSachHoanThanhPTTT(long yeuCauDichVuKyThuatId, long soLan)
        {
            var entity = await _phauThuatThuThuatService.GetThongKetLuanDaHoanThanh(yeuCauDichVuKyThuatId);
            return Ok(entity);
        }

        [HttpGet("ThongTinBenhNhanPTTTHoanThanh")]
        public async Task<ActionResult> ThongTinBenhNhanPTTTHoanThanh(long yeuCauDichVuKyThuatId)
        {
            var entity = await _phauThuatThuThuatService.GetThongTinBenhNhanPTTTHoanThanh(yeuCauDichVuKyThuatId);
            return Ok(entity);
        }
        #endregion

        #region Theo dõi người bệnh đã hoàn thành pttt

        [HttpPost("GetDataForGridAsyncChiSoSinhHieuHoanThanhPTTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChiSoSinhHieuHoanThanhPTTT([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncChiSoSinhHieuPTTT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncChiSoSinhHieuHoanThanhPTTT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncChiSoSinhHieuHoanThanhPTTT([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncChiSoSinhHieuPTTT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncKhamCacCoQuan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncKhamCacCoQuan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridAsyncLichSuKhamCacCoQuan(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsyncKhamCacCoQuan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncKhamCacCoQuan([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetTotalPageForGridAsyncLichSuKhamCacCoQuan(queryInfo);
            return Ok(gridData);
        }
               
        #region Cận lâm sàng

        [HttpPost("GetDanhSachCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachCanLamSang([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataCanLamSangForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridCanLamSang")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridCanLamSang([FromBody]QueryInfo queryInfo)
        {
            var gridData = _phauThuatThuThuatService.GetTotalCanLamSangPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion


        [HttpPost("GetDichVuHoanThanh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult> GetDichVuHoanThanh([FromBody]LookupQueryInfo model, long noiThucHienId, long yctnId, long soLan)
        {
            var dichVuHoanThanhs = await _phauThuatThuThuatService.GetDichVuHoanThanh(model, noiThucHienId, yctnId, soLan);
            return Ok(dichVuHoanThanhs);
        }

        [HttpPost("DichVuDaTuongTrinhPTTT")]
        public async Task<ActionResult> DichVuDaTuongTrinhPTTT(LichSuDichVuKyThuatDaTuongTrinhVo lichSuDichVuKyThuatDaTuongTrinhVo)
        {
            var entity = await _phauThuatThuThuatService.GetDichVuDaTuongTrinhPTTT(lichSuDichVuKyThuatDaTuongTrinhVo);
            return Ok(entity);
        }
        #endregion
    }
}
