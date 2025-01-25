using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaDuocPhamController
    {
        #region Đang chờ xử lý
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoDuocPhamForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoDuocPhamForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }
                          
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamDangChoXuLyTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Ds THDT Tai Khoa Da Xu Ly
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataTHDTTaiKhoaDaXuLyForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalTHDTTaiKhoaDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT đã xử lý chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoDuocPhamChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT đã xử lý chi tiết child

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT Chi tiết phiếu mua dược phẩm dự trù
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDuocPhamChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDuTruMuaDuocPhamChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDuocPhamChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT Tu Choi

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT Tu Choi Chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDataDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDuTruMuaDuocPhamTuChoiTaiKhoaChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult> TuChoiDuyetDuocPhamNhapKho(ThongTinLyDoHuyDuyetTaiKhoa thongTinLyDoHuyNhapKhoDuocPham)
        {
            var tuChoiDuyetKho = await _yeuCauMuaDuTruDuocPhamService.TuChoiDuyetTaiKhoa(thongTinLyDoHuyNhapKhoDuocPham);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult> DuyetDuocPhamNhapKho(DuyetDuTruMuaDuocPhamViewModel duTruMuaDuocPhamTaiKhoaChiTietVo)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruDuocPhamService.DuyetTaiKhoa(duTruMuaDuocPhamTaiKhoaChiTietVo);
            return Ok(tinhTrangDuyet);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyetTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult> HuyDuyetTaiKhoa(long id)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruDuocPhamService.HuyDuyetTaiKhoa(id);
            return Ok(tinhTrangDuyet);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruDuocPhamTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public ActionResult<DuTruMuaDuocPhamViewModel> GetThongTinDuTruDuocPhamTaiKhoa(long id)
        {
            var thongTinDuyetKhoDuocPham = _yeuCauMuaDuTruDuocPhamService.GetThongTinDuTruDuocPhamTaiKhoa(id);
            return Ok(thongTinDuyetKhoDuocPham);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruDuocPhamTaiKhoaDaXuLy/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public ActionResult<DuTruMuaDuocPhamViewModel> GetThongTinDuTruDuocPhamTaiKhoaDaXuLy(long id)
        {
            var thongTinDuyetKhoDuocPham = _yeuCauMuaDuTruDuocPhamService.GetThongTinDuTruDuocPhamTaiKhoaDaXuLy(id);
            return Ok(thongTinDuyetKhoDuocPham);
        }

        #region Gởi tai khoa      

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinGoiTaiKhoa/{phongBenhVienId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public ActionResult<GetThongTinGoiTaiKhoa> GetThongTinGoiTaiKhoa(long phongBenhVienId)
        {
            var getThongTinGoiTaiKhoa = _yeuCauMuaDuTruDuocPhamService.GetThongTinGoiTaiKhoa(phongBenhVienId);
            return Ok(getThongTinGoiTaiKhoa);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDPTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDPTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDPTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDPTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaDPTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDPTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaDPTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDPTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruDuocPhamService.GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GoiThongTinTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachTongHopDuTruMuaDuocPhamTaiKhoa)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<long> GoiThongTinTaiKhoa([FromBody]DuyetDuTruMuaDuocPhamViewModel model)
        {
            var duTruMuaDuocPhamTheoKhoaId = _yeuCauMuaDuTruDuocPhamService.GoiThongTinTaiKhoa(model);
            return duTruMuaDuocPhamTheoKhoaId;
        }

        [HttpPost("InPhieuDuTruMuaTaiKhoa")]
        public string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruDuocPhamService.InPhieuDuTruMuaTaiKhoa(phieuInDuTruMuaTaiKhoa);
            return result;
        }

        #endregion
    }
}
