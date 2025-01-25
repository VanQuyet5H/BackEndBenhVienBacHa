using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.General;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaSinhHieu;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region lịch sử khám bệnh
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncLichSuKhamBenh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)] //  check nhiều chỗ nên k xác định quyền
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncLichSuKhamBenh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncLichSuKhamBenh(queryInfo);
            return Ok(gridData);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncLichSuKhamBenh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)] //  check nhiều chỗ nên k xác định quyền
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncLichSuKhamBenh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncLichSuKhamBenh(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region lich sự khám bệnh BHYT update 1/6/2020
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncLichSuKhamBenhBHYT")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)] //  check nhiều chỗ nên k xác định quyền
        public async Task<ActionResult<GridDataSource>> GetYeuCauKhamBenh([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncLichSuKhamBenhBHYT(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetTotalPageForGridAsyncLichSuKhamBenhBHYT")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetYeuCauKhamBenhTotalPage([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetTotalPageForGridAsyncLichSuKhamBenhBHYT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncTrieuChungBenhSu")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncTrieuChungBenhSu(long ycKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncTrieuChungBenhSu(ycKhamBenhId);
            return Ok(gridData);
        }
        [HttpPost("GetDataAllKhamChuyenKhoaTheoBenhNhan")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ListAll> GetDataAllKhamChuyenKhoaTheoBenhNhan(long yeuCauTiepNhanId)
        {
            var gridData =  _yeuCauKhamBenhService.GetDataKhamCoQuanKhacTatCaChuyenKhoaTheoBenhNhan(yeuCauTiepNhanId);
            return gridData;
        }

        #endregion
        #region sinh hiệu
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncChiSoSinhHieu")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncChiSoSinhHieu(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncChiSoSinhHieu(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Bộ phận tổn thương
        [HttpPost("GetDataGridBoPhanTonThuong")]
        public async Task<ActionResult<GridDataSource>> GetDataGridBoPhanTonThuong(long idYCKB)
        {
            var gridData = await _yeuCauKhambenhBoPhanTonThuongService.GetDataGridBoPhanTonThuong(idYCKB);
            return Ok(gridData);
        }
        #endregion
        #region Dị ứng thuốc
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDiUngThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDiUngThuoc(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncDiUngThuoc(queryInfo);
            return Ok(gridData);
        }
      
        #endregion
        #region Tien Su Benh
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncTienSuBenh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncTienSuBenh(QueryInfo queryInfo)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncTienSuBenh(queryInfo);
            return Ok(gridData);
        }
        #endregion
        #region Khám bệnh
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDataForGridAsyncKhamBenh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<DanhSachDaKhamKhamBenhGridVo>> GetDataForGridAsyncKhamBenh(long yeuCauKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncKhamBenh(yeuCauKhamBenhId);
            return Ok(gridData);
        }
        [HttpGet("GetDataForGridAsyncKhamBenhRangHamMat")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<DanhSachDaKhamGridVo>> GetDataForGridAsyncKhamBenhRangHamMat(long yeuCauKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncKhamBenhRangHamMat(yeuCauKhamBenhId);
            return Ok(gridData);
        }
        // trieu chung lam sang 
        [HttpGet("GetTrieuChungLamSang")]
        public async Task<string> GetTrieuChungLamSang(long trieuChungId)
        {
            var name = await _yeuCauKhamBenhService.GetTrieuChungLamSang(trieuChungId);
            return name;
        }
        //
        [HttpGet("GetChuanDoanBanDau")]
        public async Task<string> GetChuanDoanBanDau(long chuanDoanId)
        {
            var name = await _yeuCauKhamBenhService.GetChuanDoanBanDau(chuanDoanId);
            return name;
        }
        #endregion
        #region Chỉ Định
        [HttpGet("GetDataForGridAsyncChiDinDichVuKhac")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public List<LichSuKhamBenhGridVo> GetDataForGridAsyncChiDinDichVuKhac(long yeuCauKhamBenhId,long yeuCauTiepNhanId)
        {
            var gridData =  _yeuCauKhamBenhService.GetDataForGridAsyncChiDinDichVuKhac(yeuCauKhamBenhId, yeuCauTiepNhanId);
            return gridData;
        }
        #endregion
        #region Kết luận
        [HttpGet("GetDataForGridAsyncKetLuan")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<KetLuanGridVo>> GetDataForGridAsyncKetLuan(long yeuCauKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetDataForGridAsyncKetLuan(yeuCauKhamBenhId);
            return Ok(gridData);
        }
        [HttpPost("GetDataForGridAsyncToaMau")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public ActionResult<GridDataSource> GetDataForGridAsyncToaMau(QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridAsyncToaMau(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetChuanDoanICDPhu")]
        public async Task<ActionResult<KetLuanGridVo>> GetChuanDoanICDPhu(long yeuCauKhamBenhId)
        {
            var gridData =  _yeuCauKhamBenhService.GetChuanDoanICDPhu(yeuCauKhamBenhId);
            return Ok(gridData);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncChildToaMau")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public ActionResult<GridDataSource> GetDataForGridAsyncChildToaMau([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridAsyncChildToaMau(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDataForGridAsyncChildToaThuoc")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public ActionResult<GridDataSource> GetDataForGridAsyncChildToaThuoc([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridAsyncChildToaThuoc(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDataForGridAsyncChildToaThuocList")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public List<ToaThuocGridVo> GetDataForGridAsyncChildToaThuocList(long yeuCauKhamBenhId)
        {
            var gridData = _yeuCauKhamBenhService.GetDataForGridAsyncChildToaThuocList(yeuCauKhamBenhId);
            return gridData;
        }
        #endregion
        #region PTTT

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("IsExitsDVKTPTTT")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult> IsExitsDVKTPTTT(long idYCKB)
        {
            var listPhauThuatThuThuat = await _yeuCauDichVuKyThuatService.IsExitsDVKTPTTT(idYCKB);
            return Ok(listPhauThuatThuThuat);
        }
        [HttpGet("GetLichSuThongTinTuongTrinh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetLichSuThongTinTuongTrinh(long yeuCauKhamBenhId)
        {
            var gridData = await _yeuCauKhamBenhService.GetLichSuThongTinTuongTrinh(yeuCauKhamBenhId);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetLichSuListPhauThuatThuThuat")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh)]
        public async Task<List<KhamBenhPhauThuatThuThuatGridVo>> GetLichSuListPhauThuatThuThuat(long yeuCauKhamBenhId)
        {
            var listPhauThuatThuThuat = await _yeuCauKhamBenhService.GetLichSuListPhauThuatThuThuat( yeuCauKhamBenhId);
            return listPhauThuatThuThuat;
        }
        #endregion
    }
}
