using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaVatTuTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaVatTuController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetKhoVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoVatTuForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaVatTuDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaVatTuTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetKhoVatTuChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult> TuChoiDuyetVatTuNhapKho(ThongTinLyDoHuyDuyetVatTuTaiKhoa thongTinLyDoHuyNhapKhoVatTu)
        {
            var tuChoiDuyetKho = await _yeuCauMuaDuTruVatTuService.TuChoiDuyetTaiKhoa(thongTinLyDoHuyNhapKhoVatTu);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult> DuyetVatTuNhapKho(DuyetDuTruMuaVatTuViewModel duTruMuaVatTuTaiKhoaChiTietVo)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruVatTuService.DuyetTaiKhoa(duTruMuaVatTuTaiKhoaChiTietVo);
            return Ok(tinhTrangDuyet);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyetTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult> HuyDuyetTaiKhoa(long id)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruVatTuService.HuyDuyetTaiKhoa(id);
            return Ok(tinhTrangDuyet);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruVatTuTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<DuTruMuaVatTuViewModel> GetThongTinDuTruVatTuTaiKhoa(long id)
        {
            var thongTinDuyetKhoVatTu = _yeuCauMuaDuTruVatTuService.GetThongTinDuTruVatTuTaiKhoa(id);
            return Ok(thongTinDuyetKhoVatTu);
        }


        #region Gởi tai khoa

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinGoiTaiKhoa/{phongBenhVienId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<GetThongTinGoiVatTuTaiKhoa> GetThongTinGoiTaiKhoa(long phongBenhVienId)
        {
            var getThongTinGoiTaiKhoa = _yeuCauMuaDuTruVatTuService.GetThongTinGoiTaiKhoa(phongBenhVienId);
            return Ok(getThongTinGoiTaiKhoa);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVTTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<GridDataSource> GetDuTruMuaVtTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVTTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<GridDataSource> GetTotalDuTruMuaVtTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetTotalDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVTTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<GridDataSource> GetDuTruMuaVtTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVTTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<GridDataSource> GetTotalDuTruMuaVtTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruVatTuService.GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GoiThongTinTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<long> GoiThongTinTaiKhoa([FromBody]DuyetDuTruMuaVatTuViewModel model)
        {
            var duTruMuaVatTuTheoKhoaId = _yeuCauMuaDuTruVatTuService.GoiThongTinTaiKhoa(model);
            return duTruMuaVatTuTheoKhoaId;
        }

        [HttpPost("InPhieuDuTruMuaTaiKhoa")]
        public string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaVatTuTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruVatTuService.InPhieuDuTruMuaTaiKhoa(phieuInDuTruMuaTaiKhoa);
            return result;
        }
        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruVatTuTaiKhoaDaXuLy/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public ActionResult<DuTruMuaVatTuViewModel> GetThongTinDuTruVatTuTaiKhoaDaXuLy(long id)
        {
            var thongTinDuyetKhoVatTu = _yeuCauMuaDuTruVatTuService.GetThongTinDuTruVatTuTaiKhoaDaXuLy(id);
            return Ok(thongTinDuyetKhoVatTu);
        }
        
        #region Ds THDT Tai Khoa Da Xu Ly

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataTHDTTaiKhoaDaXuLyForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalTHDTTaiKhoaDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT đã xử lý chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuTHDTChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaVatTuTHDTChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuTHDTChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuTHDTChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaVatTuTHDTChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuTHDTChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT đã xử lý chi tiết child

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaDuocPhamTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT Chi tiết phiếu mua vật tư dự trù
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaVatTuChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaVatTuChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDuTruMuaVatTuChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaVatTuChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT Tu Choi

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaVatTuTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaVatTuTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDataDuTruMuaVatTuTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaVatTuTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaVatTuTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDuTruMuaVatTuTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT Tu Choi Chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetDSTHDTTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetDSTHDTTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachTongHopDuTruMuaVatTuTaiKhoa)]
        public async Task<ActionResult<GridDataSource>> GetTotalDSTHDTTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruVatTuService.GetTotalDSTHDTTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

    }
}
