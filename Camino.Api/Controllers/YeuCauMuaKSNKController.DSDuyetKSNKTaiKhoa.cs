using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoa;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaKSNKController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDuyetMuaDuTruTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDanhSachDuyetMuaDuTruTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDanhSachDuyetMuaDuTruTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKDangChoXuLyTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKDangChoXuLyTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }
        

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }
                       
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("TuChoiDuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult> TuChoiDuyetTaiKhoa(ThongTinLyDoHuyDuyetKSNKTaiKhoa thongTinLyDoHuyNhapKhoVatTu)
        {
            var tuChoiDuyetKho = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.TuChoiDuyetTaiKhoa(thongTinLyDoHuyNhapKhoVatTu);
            return Ok(tuChoiDuyetKho);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("DuyetTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult> DuyetTaiKhoa(DuyetDuTruMuaKSNKViewModel duTruMuaVatTuTaiKhoaChiTietVo)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.DuyetTaiKhoa(duTruMuaVatTuTaiKhoaChiTietVo);
            return Ok(tinhTrangDuyet);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("HuyDuyetTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult> HuyDuyetTaiKhoa(long id)
        {
            var tinhTrangDuyet = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.HuyDuyetTaiKhoa(id);
            return Ok(tinhTrangDuyet);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruKSNKTaiKhoa/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<DuTruMuaKSNKViewModel> GetThongTinDuTruKSNKTaiKhoa(long id)
        {
            var thongTinDuyetKhoVatTu = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetThongTinDuTruKSNKTaiKhoa(id);
            return Ok(thongTinDuyetKhoVatTu);
        }


        #region Gởi tai khoa

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinGoiTaiKhoa/{phongBenhVienId}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<GetThongTinGoiKSNKTaiKhoa> GetThongTinGoiTaiKhoa(long phongBenhVienId)
        {
            var getThongTinGoiTaiKhoa = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetThongTinGoiTaiKhoa(phongBenhVienId);
            return Ok(getThongTinGoiTaiKhoa);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<GridDataSource> GetDuTruMuaKSNKTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<GridDataSource> GetTotalDuTruMuaKSNKTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDanhTruMuaTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<GridDataSource> GetDuTruMuaKSTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSTaiKhoaChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<GridDataSource> GetTotalDuTruMuaKSTaiKhoaChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDanhTruMuaTaiKhoaChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }
        
        [HttpPost("GoiThongTinTaiKhoa")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.THDTMuaTaiKSNK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<long> GoiThongTinTaiKhoa([FromBody]DuyetDuTruMuaKSNKViewModel model)
        {
            var duTruMuaVatTuTheoKhoaId = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GoiThongTinTaiKhoa(model);
            return duTruMuaVatTuTheoKhoaId;
        }

        [HttpPost("InPhieuDuTruMuaTaiKhoa")]
        public string InPhieuDuTruMuaTaiKhoa(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoa(phieuInDuTruMuaTaiKhoa);
            return result;
        }

        [HttpPost("InPhieuDuTruMuaTaiKhoaDuocPham")]
        public string InPhieuDuTruMuaTaiKhoaDuocPham(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var result = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoaDuocPham(phieuInDuTruMuaTaiKhoa);
            return result;
        }

        [HttpPost("InPhieuVatTuVaDuocPhamTaiKhoa")]
        public string InPhieuVatTuVaDuocPhamTaiKhoa(PhieuInDuTruMuaKSNKTaiKhoa phieuInDuTruMuaTaiKhoa)
        {
            var htmlAll = string.Empty;
            if (phieuInDuTruMuaTaiKhoa != null && phieuInDuTruMuaTaiKhoa.DuTruMuaVatTuTheoKhoaId != null)
            {
                htmlAll = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoa(phieuInDuTruMuaTaiKhoa) + "<div class=\"pagebreak\"> </div>";
            }

            if (phieuInDuTruMuaTaiKhoa != null && phieuInDuTruMuaTaiKhoa.DuTruMuaDuocPhamTheoKhoaId != null)
            {
                htmlAll += _yeuCauMuaDuTruKiemSoatNhiemKhuanService.InPhieuDuTruMuaTaiKhoaDuocPham(phieuInDuTruMuaTaiKhoa);
            }        

            return htmlAll;
        }

        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetThongTinDuTruKSNKTaiKhoaDaXuLy/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public ActionResult<DuTruMuaKSNKViewModel> GetThongTinDuTruKSNKTaiKhoaDaXuLy(long id)
        {
            var thongTinDuyetKhoVatTu = _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetThongTinDuTruKSNKTaiKhoaDaXuLy(id);
            return Ok(thongTinDuyetKhoVatTu);
        }
        
        #region Ds THDT Tai Khoa Da Xu Ly

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataTHDTTaiKhoaDaXuLyForGridAsync(queryInfo, false);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalTHDTTaiKhoaDaXuLyForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalTHDTTaiKhoaDaXuLyForGridAsyncDetail(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT đã xử lý chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKTHDTChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKTHDTChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKTHDTChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKTHDTChiTietForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKTHDTChiTietForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTHDTChiTietForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT đã xử lý chi tiết child

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTaiKhoaChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT Chi tiết phiếu mua vật tư dự trù
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDuTruMuaKSNKChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDuTruMuaKSNKChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDuTruMuaKSNKChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKChiTietForGridAsyncChild")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKChiTietForGridAsyncChild([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKChiTietForGridAsyncChild(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region DS THDT Tu Choi

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataDuTruMuaKSNKTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDataDuTruMuaKSNKTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDataDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDuTruMuaKSNKTuChoiTaiKhoaForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDuTruMuaKSNKTuChoiTaiKhoaForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDuTruMuaKSNKTuChoiTaiKhoaForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region DS THDT Tu Choi Chi tiết

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetDSTHDTTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetDSTHDTTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalDSTHDTTuChoiChildForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.THDTMuaTaiKSNK)]
        public async Task<ActionResult<GridDataSource>> GetTotalDSTHDTTuChoiChildForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauMuaDuTruKiemSoatNhiemKhuanService.GetTotalDSTHDTTuChoiChildForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

    }
}
