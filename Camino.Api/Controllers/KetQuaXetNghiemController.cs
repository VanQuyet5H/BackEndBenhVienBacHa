using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.KetQuaXetNghiem;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaXetNghiem;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Helpers;
using Camino.Services.DuyetKetQuaXetNghiems;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KetQuaXetNghiem;
using Camino.Services.Localization;
using Camino.Services.ThietBiXetNghiems;
using Camino.Services.TiepNhanBenhNhan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class KetQuaXetNghiemController : CaminoBaseController
    {
        private readonly IKetQuaXetNghiemService _ketQuaXetNghiemService;
        private readonly IThietBiXetNghiemService _thietBiXetNghiemService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ITiepNhanBenhNhanService _tiepNhanBenhNhanService;
        private readonly IDuyetKetQuaXetNghiemService _duyetKqXetNghiemService;

        public KetQuaXetNghiemController(IKetQuaXetNghiemService ketQuaXetNghiemService, IThietBiXetNghiemService thietBiXetNghiemService
        , ILocalizationService localizationService, IUserAgentHelper userAgentHelper
            , IExcelService excelService
            , ITiepNhanBenhNhanService tiepNhanBenhNhanService
            , IDuyetKetQuaXetNghiemService duyetKqXetNghiemService
        )
        {
            _ketQuaXetNghiemService = ketQuaXetNghiemService;
            _localizationService = localizationService;
            _excelService = excelService;
            _userAgentHelper = userAgentHelper;
            _thietBiXetNghiemService = thietBiXetNghiemService;
            _tiepNhanBenhNhanService = tiepNhanBenhNhanService;
            _duyetKqXetNghiemService = duyetKqXetNghiemService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ketQuaXetNghiemService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ketQuaXetNghiemService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ketQuaXetNghiemService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _ketQuaXetNghiemService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KetQuaXetNghiem)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KetQuaXetNghiemViewKetQuaViewModel>> Get(long id)
        {
            var phienXetNghiemData = _duyetKqXetNghiemService.GetPhienXetNghiemData(id);
            if (phienXetNghiemData == null)
                return NotFound();
            var resultData = new KetQuaXetNghiemViewKetQuaViewModel
            {
                Id = id,
                YeuCauTiepNhanId = phienXetNghiemData.YeuCauTiepNhanId,
                MaSoBHYT = phienXetNghiemData.MaSoBHYT,
                CoBHYT = phienXetNghiemData.CoBHYT,
                BarCodeID = phienXetNghiemData.BarCodeID,
                MaYeuCauTiepNhan = phienXetNghiemData.MaYeuCauTiepNhan,
                MaBN = phienXetNghiemData.MaBN,
                HoTen = phienXetNghiemData.HoTen,
                NgaySinh = phienXetNghiemData.NgaySinh,
                ThangSinh = phienXetNghiemData.ThangSinh,
                NamSinh = phienXetNghiemData.NamSinh,
                GioiTinh = phienXetNghiemData.GioiTinh,
                BHYTMucHuong = phienXetNghiemData.BHYTMucHuong,
                LoaiYeuCauTiepNhan = phienXetNghiemData.LoaiYeuCauTiepNhan,
                SoDienThoai = phienXetNghiemData.SoDienThoai,
                DiaChi = phienXetNghiemData.DiaChi,
                ChanDoan = phienXetNghiemData.ChanDoan,                
                NhanVienThucHienId = phienXetNghiemData.NguoiThucHienId,
                ChanDoanDuoi = phienXetNghiemData.KetLuan,
                GhiChu = phienXetNghiemData.GhiChu,
                TenCongTy = phienXetNghiemData.TenCongTy,
                LaCapCuu = phienXetNghiemData.LaCapCuu,
                Phong = phienXetNghiemData.Phong,
                KhoaChiDinh = phienXetNghiemData.KhoaChiDinh,
                //TrangThai = phienXetNghiemData.TrangThai
            };
            if (phienXetNghiemData.NguoiThucHienId == null)
            {
                resultData.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            }

            //resultData.Phong = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
            //resultData.KhoaChiDinh = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");

            //add detail
            //var lstYeuCauDichVuKyThuatId = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();
            var lstYeuCauDichVuKyThuatId = phienXetNghiemData.PhienXetNghiemChiTietDataVos.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

            foreach (var ycId in lstYeuCauDichVuKyThuatId)
            {
                if (!phienXetNghiemData.PhienXetNghiemChiTietDataVos.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                var res = phienXetNghiemData.PhienXetNghiemChiTietDataVos.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                listChiTiet.AddRange(res);
            }

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();

            resultData.dataChild = addDetailDataChildV2(phienXetNghiemData, _duyetKqXetNghiemService.GetTenMayXetNghiems(), _duyetKqXetNghiemService.GetTenNhanViens(), _duyetKqXetNghiemService.GetTenDichVuXetNghiems(), listChiTiet, listChiTiet, new List<ListDataChild>(), true);


            foreach (var item in resultData.dataChild)
            {
                item.NguoiThucHien = phienXetNghiemData.NguoiThucHien;
                //item.NguoiThucHien = phienXetNghiem?.NhanVienThucHien?.User?.HoTen;
            }



            foreach (var detail in resultData.dataChild)
            {
                detail.DanhSachLoaiMauDaCoKetQua = resultData.dataChild
                    .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
                    .Distinct().ToList();


                //var lstTongCong = phienXetNghiem.YeuCauTiepNhan.YeuCauDichVuKyThuats
                //        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                //        .Select(p => p.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription())
                //        .Distinct().Where(p => p != null).ToList();
                var lstTongCong = phienXetNghiemData.PhienXetNghiemChiTietDataVos
                    .Where(p => p.LoaiMauXetNghiem != null && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                    .Select(p => p.LoaiMauXetNghiem.GetDescription())
                    .Distinct().ToList();


                var lstLoaiMauKhongDat = new List<string>();

                detail.DanhSachLoaiMau = lstTongCong;
                detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
            }

            #region BVHD-3941
            //if (phienXetNghiem.YeuCauTiepNhan?.CoBHTN == true)
            //{
            //    resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(phienXetNghiem.YeuCauTiepNhanId).Result;
            //}
            if (phienXetNghiemData.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(phienXetNghiemData.YeuCauTiepNhanId).Result;
            }
            #endregion

            return Ok(resultData);
        }

        private KetQuaXetNghiemViewKetQuaViewModel GetOld(long id)
        {
            //var phienXetNghiem = _ketQuaXetNghiemService.GetById(id
            //    ,
            //    u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
            //    .Include(x => x.MauXetNghiems)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.DichVuKyThuatBenhVien).ThenInclude(p => p.DichVuXetNghiem)
            //    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.DanToc)
            //    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.TinhThanh)
            //    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NgheNghiep)
            //    //.Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.Icdchinh)
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.HopDongKhamSucKhoe).ThenInclude(x => x.CongTyKhamSucKhoe)
            //    .Include(x => x.BenhNhan)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
            //    //.Include(x => x.YeuCauNhapKhoDuocPhamChiTiets).ThenInclude(x => x.KhoViTri)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
            //    .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiChiDinh).ThenInclude(x => x.KhoaPhong)
            //     .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.NoiTruPhieuDieuTri)
            //     .Include(x=>x.NhanVienThucHien).ThenInclude(x=>x.User)
            //    //BVHD-3800
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauNhapVien).ThenInclude(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauTiepNhan)
            //    );

            var phienXetNghiem = _duyetKqXetNghiemService.GetChiTietById(id);

            if (phienXetNghiem == null)
                return null;

            var resultData = new KetQuaXetNghiemViewKetQuaViewModel
            {
                Id = phienXetNghiem.Id,
                YeuCauTiepNhanId = phienXetNghiem.YeuCauTiepNhanId,
                //Thong tin hanh chinh
                MaSoBHYT = phienXetNghiem.YeuCauTiepNhan.BHYTMaSoThe,
                CoBHYT = phienXetNghiem.YeuCauTiepNhan.CoBHYT,
                BarCodeID = phienXetNghiem.BarCodeId,
                MaYeuCauTiepNhan = phienXetNghiem.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = phienXetNghiem.BenhNhan.MaBN,
                HoTen = phienXetNghiem.YeuCauTiepNhan.HoTen,
                NgaySinh = phienXetNghiem.YeuCauTiepNhan.NgaySinh,
                ThangSinh = phienXetNghiem.YeuCauTiepNhan.ThangSinh,
                NamSinh = phienXetNghiem.YeuCauTiepNhan.NamSinh,
                GioiTinh = phienXetNghiem.YeuCauTiepNhan.GioiTinh,
                BHYTMucHuong = phienXetNghiem.YeuCauTiepNhan.BHYTMucHuong,
                LoaiYeuCauTiepNhan = phienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan,
                SoDienThoai = phienXetNghiem.YeuCauTiepNhan.SoDienThoaiDisplay ?? phienXetNghiem.YeuCauTiepNhan.NguoiLienHeSoDienThoai?.ApplyFormatPhone(),
                DiaChi = phienXetNghiem.YeuCauTiepNhan.DiaChiDayDu,
                ChanDoan = phienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru ?
                (phienXetNghiem.YeuCauTiepNhan.YeuCauKhamBenhs.Any(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                    ? phienXetNghiem.YeuCauTiepNhan?.YeuCauKhamBenhs?
                        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.IcdchinhId != null)
                        .Select(p => p.ChanDoanSoBoGhiChu).ToList().Distinct().Join(" ; ")
                    : "") :
                    (phienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru ?
                    phienXetNghiem.PhienXetNghiemChiTiets.Select(o => o.YeuCauDichVuKyThuat?.NoiTruPhieuDieuTri?.ChanDoanChinhGhiChu).ToList().Distinct().Join(" ; ") :
                    (phienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? "Khám sức khỏe" : ""))

                ,
                KhoaChiDinh = "",
                Phong = "",

                //
                NhanVienThucHienId = phienXetNghiem.NhanVienThucHienId,
                ChanDoanDuoi = phienXetNghiem.KetLuan,
                GhiChu = phienXetNghiem.GhiChu,

                //BVHD-3364
                TenCongTy = phienXetNghiem.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe ? phienXetNghiem.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten : null,

                //BVHD-3800
                LaCapCuu = phienXetNghiem.YeuCauTiepNhan.LaCapCuu ?? phienXetNghiem.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan?.LaCapCuu
            };
            //
            if (phienXetNghiem.NhanVienThucHienId == null)
            {
                resultData.NhanVienThucHienId = _userAgentHelper.GetCurrentUserId();
            }

            resultData.Phong = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.Ten).Distinct().Join(" , ");
            resultData.KhoaChiDinh = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuat).Select(p => p.NoiChiDinh).Select(p => p.KhoaPhong).Select(p => p.Ten).Distinct().Join(" , ");

            //add detail
            var lstYeuCauDichVuKyThuatId = phienXetNghiem.PhienXetNghiemChiTiets.Select(p => p.YeuCauDichVuKyThuatId).Distinct().ToList();

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();

            foreach (var ycId in lstYeuCauDichVuKyThuatId)
            {
                if (!phienXetNghiem.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                var res = phienXetNghiem.PhienXetNghiemChiTiets.Where(p => p.YeuCauDichVuKyThuatId == ycId).Last().KetQuaXetNghiemChiTiets.ToList();
                listChiTiet.AddRange(res);
            }


            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();

            resultData.dataChild = addDetailDataChild(listChiTiet, listChiTiet, new List<ListDataChild>(), true);


            foreach (var item in resultData.dataChild)
            {
                item.NguoiThucHien = phienXetNghiem?.NhanVienThucHien?.User?.HoTen;
            }



            foreach (var detail in resultData.dataChild)
            {
                detail.DanhSachLoaiMauDaCoKetQua = resultData.dataChild
                    .Where(p => p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).Select(p => p.LoaiMau)
                    .Distinct().ToList();


                var lstTongCong = phienXetNghiem.YeuCauTiepNhan.YeuCauDichVuKyThuats
                        .Where(p => p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId)
                        .Select(p => p.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription())
                        .Distinct().Where(p => p != null).ToList();


                var lstLoaiMauKhongDat = new List<string>();

                //đã bỏ xét mẫu không đạt
                //foreach (var loaiMau in lstTongCong)
                //{
                //    var mauXetNghiem = _ketQuaXetNghiemService.GetById(id, p => p.Include(o => o.MauXetNghiems))
                //            .MauXetNghiems
                //            .Where(p => p.LoaiMauXetNghiem.GetDescription() == loaiMau && p.NhomDichVuBenhVienId == detail.NhomDichVuBenhVienId).LastOrDefault();
                //    if (mauXetNghiem != null && mauXetNghiem.DatChatLuong != true)
                //    {
                //        lstLoaiMauKhongDat.Add(mauXetNghiem.LoaiMauXetNghiem.GetDescription());
                //    }
                //}

                detail.DanhSachLoaiMau = lstTongCong;
                detail.DanhSachLoaiMauKhongDat = lstLoaiMauKhongDat;
                //detail.CheckBox = true;
            }

            #region BVHD-3941
            if (phienXetNghiem.YeuCauTiepNhan?.CoBHTN == true)
            {
                resultData.TenCongTyBaoHiemTuNhan = _tiepNhanBenhNhanService.GetThongTinBaoHiemTuNhanAsync(phienXetNghiem.YeuCauTiepNhanId).Result;
            }
            #endregion

            return resultData;
        }

        [HttpPost("GetListLookupMayXetNghiem")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListLookupMayXetNghiem(DropDownListRequestModel model)
        {
            var lookup = await _ketQuaXetNghiemService.GetListMayXetNghiem(model);
            return Ok(lookup);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult> Luu([FromBody] KetQuaXetNghiemViewKetQuaViewModel model)
        {
            var mayXetNghiems = await _thietBiXetNghiemService.GetTatCaMayXetNghiemAsync();
            var phienXetNghiem = await _ketQuaXetNghiemService.GetByIdAsync(model.Id, u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets));

            foreach (var child in model.dataChild)
            {
                var ketQuaXetNghiemChiTiet = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).First(p => p.Id == child.Id);
                ketQuaXetNghiemChiTiet.MayXetNghiemId = child.MayXetNghiemId;
                ketQuaXetNghiemChiTiet.MauMayXetNghiemId = mayXetNghiems.FirstOrDefault(o => o.Id == child.MayXetNghiemId)?.MauMayXetNghiemID;
                ketQuaXetNghiemChiTiet.ToDamGiaTri = child.ToDamGiaTri;
                if (child.GiaTriNhapTay != ketQuaXetNghiemChiTiet.GiaTriNhapTay && !(string.IsNullOrEmpty(child.GiaTriNhapTay) && string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay)))
                {
                    ketQuaXetNghiemChiTiet.ThoiDiemNhanKetQua = DateTime.Now;
                    child.ThoiDiemNhanKetQua = DateTime.Now;
                }
                ketQuaXetNghiemChiTiet.GiaTriNhapTay = child.GiaTriNhapTay;
                if (!string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay) && ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua == null)
                {
                    ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua = DateTime.Now;
                    phienXetNghiem.ChoKetQua = false;
                }
            }

            phienXetNghiem.NhanVienThucHienId = model.NhanVienThucHienId;
            phienXetNghiem.PhongThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            phienXetNghiem.KetLuan = model.ChanDoanDuoi;
            phienXetNghiem.GhiChu = model.GhiChu;

            await _ketQuaXetNghiemService.UpdateAsync(phienXetNghiem);

            return Ok(model.dataChild);
        }

        [HttpPost("DuyetPhienXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KetQuaXetNghiem)]
        public async Task<ActionResult> DuyetPhienXetNghiem([FromBody] KetQuaXetNghiemViewKetQuaViewModel model)
        {
            if (model.dataChild.All(z => z.CheckBox != true))
            {
                throw new ApiException(_localizationService.GetResource("KetQuaXetNghiem.DataChild.Selected"));
            }
            var mayXetNghiems = await _thietBiXetNghiemService.GetTatCaMayXetNghiemAsync();
            var phienXetNghiem = await _ketQuaXetNghiemService.GetByIdAsync(model.Id,
                            u => u.Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets)
                                  .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                  .Include(x => x.PhienXetNghiemChiTiets).ThenInclude(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat));

            //update entity
            foreach (var child in model.dataChild)
            {
                var ketQuaXetNghiemChiTiet = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(p => p.KetQuaXetNghiemChiTiets).First(p => p.Id == child.Id);
                ketQuaXetNghiemChiTiet.MayXetNghiemId = child.MayXetNghiemId;
                ketQuaXetNghiemChiTiet.MauMayXetNghiemId = mayXetNghiems.FirstOrDefault(o => o.Id == child.MayXetNghiemId)?.MauMayXetNghiemID;
                ketQuaXetNghiemChiTiet.ToDamGiaTri = child.ToDamGiaTri;
                if (child.GiaTriNhapTay != ketQuaXetNghiemChiTiet.GiaTriNhapTay && !(string.IsNullOrEmpty(child.GiaTriNhapTay) && string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay)))
                {
                    ketQuaXetNghiemChiTiet.ThoiDiemNhanKetQua = DateTime.Now;
                    child.ThoiDiemNhanKetQua = DateTime.Now;
                }
                ketQuaXetNghiemChiTiet.GiaTriNhapTay = child.GiaTriNhapTay;
                if (!string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay) && ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua == null)
                {
                    ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua = DateTime.Now;
                    phienXetNghiem.ChoKetQua = false;
                }
            }
            var noiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var userId = model.NhanVienThucHienId ?? _userAgentHelper.GetCurrentUserId();
            var now = DateTime.Now;
            phienXetNghiem.NhanVienThucHienId = model.NhanVienThucHienId;
            phienXetNghiem.PhongThucHienId = noiThucHienId;
            phienXetNghiem.KetLuan = model.ChanDoanDuoi;
            phienXetNghiem.GhiChu = model.GhiChu;
            //p.DaGoiDuyet == null &&
            var lstDaTaChildCheckBox = model.dataChild.Where(z => z.CheckBox == true).ToList();
            var phienXetNghiemChiTiets = phienXetNghiem.PhienXetNghiemChiTiets.Where(p => lstDaTaChildCheckBox.Any(z => z.DichVuXetNghiemId == p.DichVuKyThuatBenhVien?.DichVuXetNghiemId)).ToList();

            foreach (var phienXetNghiemChiTiet in phienXetNghiemChiTiets)
            {
                phienXetNghiemChiTiet.DaGoiDuyet = true;
                if (phienXetNghiemChiTiet.YeuCauDichVuKyThuat != null)
                {
                    phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NoiThucHienId = noiThucHienId;
                    phienXetNghiemChiTiet.YeuCauDichVuKyThuat.ThoiDiemThucHien = now;
                    phienXetNghiemChiTiet.YeuCauDichVuKyThuat.NhanVienThucHienId = userId;
                }
            }
            await _ketQuaXetNghiemService.UpdateAsync(phienXetNghiem);
            var daGoiDuyetTatCa = phienXetNghiem.PhienXetNghiemChiTiets.All(z => z.DaGoiDuyet == true);
            return Ok(daGoiDuyetTatCa);
        }

        private List<ListDataChild> addDetailDataChildV2(PhienXetNghiemDataVo phienXetNghiemDataVo, List<LookupItemVo> tenMayXetNghiems, List<LookupItemVo> tenNhanViens, List<LookupItemVo> tenDichVuXetNghiems, List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
            , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<ListDataChild> result
            , bool theFirst = false, int level = 1)
        {
            //var result = new List<ListDataChild>();
            if (!lstChiTietNhomChild.Any() && theFirst != true) return result;

            List<long> lstIdSearch = new List<long>();
            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();

                //var isHaveYeuCauChayLai = lstParent.Any(p => p.PhienXetNghiemChiTiet.ChayLaiKetQua == true);

                foreach (var parent in lstParent)
                {
                    var phienXetNghiemChiTietVo = phienXetNghiemDataVo.PhienXetNghiemChiTietDataVos.First(o => o.Id == parent.PhienXetNghiemChiTietId);
                    var ketQua = new ListDataChild
                    {
                        Id = parent.Id,
                        Ten = phienXetNghiemChiTietVo.TenDichVu,
                        yeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        YeuCauTiepNhanId = phienXetNghiemDataVo.YeuCauTiepNhanId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = tenMayXetNghiems.FirstOrDefault(o => o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = tenNhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                        LoaiMau = phienXetNghiemChiTietVo.LoaiMauXetNghiem?.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = phienXetNghiemChiTietVo.DaGoiDuyet,
                        CheckBox = phienXetNghiemChiTietVo.DaGoiDuyet == true,
                        CheckBoxParent = phienXetNghiemChiTietVo.DaGoiDuyet == true,
                        //structure tree
                        Level = level,
                        //YeuCauChayLai = parent.PhienXetNghiemChiTiet.ChayLaiKetQua,
                        //DaDuyet = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null ? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.DuocDuyet : false,
                        DaDuyet = phienXetNghiemChiTietVo.DaGoiDuyet == true && phienXetNghiemChiTietVo.ThoiDiemKetLuan != null,
                        //NguoiYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //LyDoYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //NgayDuyetDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //    ? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                        ////
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                        Nhom = phienXetNghiemChiTietVo.TenNhomDichVuBenhVien,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                            && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),

                        DaDuyetChiTiet = parent.DaDuyet ?? false,
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId
                    };
                    //var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                    //ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);
                    //
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    result.Add(ketQua);
                }
            }
            else
            {

                if (lstChiTietNhomChild != null)
                {

                    //var isHaveYeuCauChayLai = lstChiTietNhomChild.Any(p => p.PhienXetNghiemChiTiet.ChayLaiKetQua == true);

                    foreach (var parent in lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                    {
                        var phienXetNghiemChiTietVo = phienXetNghiemDataVo.PhienXetNghiemChiTietDataVos.First(o => o.Id == parent.PhienXetNghiemChiTietId);
                        var ketQua = new ListDataChild
                        {
                            Id = parent.Id,
                            Ten = tenDichVuXetNghiems.FirstOrDefault(o => o.KeyId == parent.DichVuXetNghiemId)?.DisplayName,
                            yeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                            YeuCauTiepNhanId = phienXetNghiemDataVo.YeuCauTiepNhanId,
                            GiaTriCu = parent.GiaTriCu,
                            GiaTriNhapTay = parent.GiaTriNhapTay,
                            GiaTriTuMay = parent.GiaTriTuMay,
                            GiaTriDuyet = parent.GiaTriDuyet,
                            ToDamGiaTri = parent.ToDamGiaTri,
                            CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                            GiaTriMin = parent.GiaTriMin,
                            GiaTriMax = parent.GiaTriMax,
                            DonVi = parent.DonVi,
                            ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                            ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                            MayXetNghiemId = parent.MayXetNghiemId,
                            TenMayXetNghiem = tenMayXetNghiems.FirstOrDefault(o => o.KeyId == parent.MayXetNghiemId)?.DisplayName,
                            ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                            NguoiDuyet = tenNhanViens.FirstOrDefault(o => o.KeyId == parent.NhanVienDuyetId)?.DisplayName,
                            LoaiMau = phienXetNghiemChiTietVo.LoaiMauXetNghiem?.GetDescription(),
                            DichVuXetNghiemId = parent.DichVuXetNghiemId,
                            DaGoiDuyet = phienXetNghiemChiTietVo.DaGoiDuyet,
                            //structure tree
                            Level = level,
                            //YeuCauChayLai = parent.PhienXetNghiemChiTiet.ChayLaiKetQua,
                            DaDuyet = phienXetNghiemChiTietVo.DaGoiDuyet == true && phienXetNghiemChiTietVo.ThoiDiemKetLuan != null,
                        //    NguoiYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //    LyDoYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //    NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //    NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //    NgayDuyetDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //    ? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                            LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                            Nhom = phienXetNghiemChiTietVo.TenNhomDichVuBenhVien,
                            IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                                && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                            NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,

                            //DaDuyetChiTiet = parent.DaDuyet ?? false,
                            DaDuyetChiTiet = parent.DaDuyet ?? false,
                            DichVuXetNghiemChaId = parent.DichVuXetNghiemChaId
                        };
                        //var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                        //ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);
                        //
                        lstIdSearch.Add(parent.DichVuXetNghiemId);
                        var index = result.FindIndex(x => x.DichVuXetNghiemId == parent.DichVuXetNghiemChaId);
                        if (index >= 0)
                        {
                            var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                            result.Insert(index + 1 + listChilds, ketQua);
                        }
                    }
                }
            }

            lstIdSearch = lstIdSearch.Distinct().ToList();
            var lstChiTietChild = lstChiTietNhomConLai.Where(p => lstIdSearch.Any(o => o == p.DichVuXetNghiemChaId)).ToList();
            level++;
            return addDetailDataChildV2(phienXetNghiemDataVo, tenMayXetNghiems, tenNhanViens, tenDichVuXetNghiems, lstChiTietNhomConLai, lstChiTietChild, result, false, level);
            //return result;
        }

        private List<ListDataChild> addDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai
            , List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<ListDataChild> result
            , bool theFirst = false, int level = 1)
        {
            //var result = new List<ListDataChild>();
            if (!lstChiTietNhomChild.Any() && theFirst != true) return result;

            List<long> lstIdSearch = new List<long>();
            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();

                var isHaveYeuCauChayLai = lstParent.Any(p => p.PhienXetNghiemChiTiet.ChayLaiKetQua == true);

                foreach (var parent in lstParent)
                {
                    var ketQua = new ListDataChild
                    {
                        Id = parent.Id,
                        Ten = parent.YeuCauDichVuKyThuat.TenDichVu,
                        yeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        YeuCauTiepNhanId = parent.PhienXetNghiemChiTiet.PhienXetNghiem.YeuCauTiepNhanId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        GiaTriMin = parent.GiaTriMin,
                        GiaTriMax = parent.GiaTriMax,
                        DonVi = parent.DonVi,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        LoaiMau = parent.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet.DaGoiDuyet,
                        CheckBox = parent.PhienXetNghiemChiTiet.DaGoiDuyet == true,
                        CheckBoxParent = parent.PhienXetNghiemChiTiet.DaGoiDuyet == true,
                        //structure tree
                        Level = level,
                        //YeuCauChayLai = parent.PhienXetNghiemChiTiet.ChayLaiKetQua,
                        //DaDuyet = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null ? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.DuocDuyet : false,
                        DaDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet == true && parent.PhienXetNghiemChiTiet?.ThoiDiemKetLuan != null,
                        //NguoiYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //LyDoYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //NgayDuyetDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //    ? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                        //
                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                            && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),

                        DaDuyetChiTiet = (isHaveYeuCauChayLai && parent.PhienXetNghiemChiTiet.ChayLaiKetQua != true) ? true : (parent.DaDuyet ?? false),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId
                    };
                    //var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                    //ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);
                    //
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    result.Add(ketQua);
                }
            }
            else
            {

                if (lstChiTietNhomChild != null)
                {

                    var isHaveYeuCauChayLai = lstChiTietNhomChild.Any(p => p.PhienXetNghiemChiTiet.ChayLaiKetQua == true);

                    foreach (var parent in lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId))
                    {
                        var ketQua = new ListDataChild
                        {
                            Id = parent.Id,
                            Ten = parent.DichVuXetNghiem?.Ten,
                            yeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                            YeuCauTiepNhanId = parent.PhienXetNghiemChiTiet.PhienXetNghiem.YeuCauTiepNhanId,
                            GiaTriCu = parent.GiaTriCu,
                            GiaTriNhapTay = parent.GiaTriNhapTay,
                            GiaTriTuMay = parent.GiaTriTuMay,
                            GiaTriDuyet = parent.GiaTriDuyet,
                            ToDamGiaTri = parent.ToDamGiaTri,
                            CSBT = LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// LISHelper.GetChiSoTrungBinh(parent.GiaTriMin, parent.GiaTriMax),// (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                            GiaTriMin = parent.GiaTriMin,
                            GiaTriMax = parent.GiaTriMax,
                            DonVi = parent.DonVi,
                            ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                            ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                            MayXetNghiemId = parent.MayXetNghiemId,
                            TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                            ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                            NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                            LoaiMau = parent.NhomDichVuBenhVien.Ten,
                            DichVuXetNghiemId = parent.DichVuXetNghiemId,
                            DaGoiDuyet = parent.PhienXetNghiemChiTiet.DaGoiDuyet,
                            //structure tree
                            Level = level,
                            //YeuCauChayLai = parent.PhienXetNghiemChiTiet.ChayLaiKetQua,
                            DaDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet == true && parent.PhienXetNghiemChiTiet?.ThoiDiemKetLuan != null,
                        //    NguoiYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienYeuCau.User.HoTen : "",
                        //    LyDoYeuCau = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.LyDoYeuCau : "",
                        //    NguoiDuyetChayLai = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NhanVienDuyet?.User.HoTen : "",
                        //    NgayYeuCauDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayYeuCau.ApplyFormatDateTime() : "",
                        //    NgayDuyetDisplay = parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem != null
                        //? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet != null
                        //    ? (parent.PhienXetNghiemChiTiet.YeuCauChayLaiXetNghiem.NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : "")
                        //    : "",
                            LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                                                                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                                                                            , parent.GiaTriTuMay),
                            Nhom = parent.NhomDichVuBenhVien.Ten,
                            IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                                && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                            NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,

                            //DaDuyetChiTiet = parent.DaDuyet ?? false,
                            DaDuyetChiTiet = (isHaveYeuCauChayLai && parent.PhienXetNghiemChiTiet.ChayLaiKetQua != true) ? true : (parent.DaDuyet ?? false),
                            DichVuXetNghiemChaId = parent.DichVuXetNghiemChaId
                        };
                        //var lstChiTietChild = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId).ToList();
                        //ketQua.Items = addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild);
                        //
                        lstIdSearch.Add(parent.DichVuXetNghiemId);
                        var index = result.FindIndex(x => x.DichVuXetNghiemId == parent.DichVuXetNghiemChaId);
                        if (index >= 0)
                        {
                            var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                            result.Insert(index + 1 + listChilds, ketQua);
                        }
                    }
                }
            }

            lstIdSearch = lstIdSearch.Distinct().ToList();
            var lstChiTietChild = lstChiTietNhomConLai.Where(p => lstIdSearch.Any(o => o == p.DichVuXetNghiemChaId)).ToList();
            level++;
            return addDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, result, false, level);
            //return result;
        }

        [HttpPost("ExportKetQuaXetNghiem")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DuyetKetQuaXetNghiem)]
        public async Task<ActionResult> ExportKetQuaXetNghiem(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _ketQuaXetNghiemService.GetDataForGridAsync(queryInfo);
            var data = gridData.Data.Select(p => (KetQuaXetNghiemGridVo)p).ToList();
            var dataExcel = data.Map<List<KetQuaXetNghiemExportExcel>>();
            foreach (var item in dataExcel)
            {
                queryInfo.AdditionalSearchString = item.Id + "";
                var gridChildData = await _ketQuaXetNghiemService.GetDataForGridChildAsync(queryInfo);
                var dataChild = gridChildData.Data.Select(p => (KetQuaXetNghiemChildGridVo)p).ToList();
                var dataChildExcel = dataChild.Map<List<KetQuaXetNghiemExportExcelChild>>();
                item.KetQuaXetNghiemExportExcelChild.AddRange(dataChildExcel);
            }

            var lstValueObject = new List<(string, string)>
            {
                (nameof(KetQuaXetNghiemExportExcel.BarCode), "BarCode"),
                (nameof(KetQuaXetNghiemExportExcel.MaTN), "Mã TN"),
                (nameof(KetQuaXetNghiemExportExcel.MaBN), "Mã BN"),
                (nameof(KetQuaXetNghiemExportExcel.HoTen), "Họ Tên"),
                (nameof(KetQuaXetNghiemExportExcel.GioiTinh), "Giới Tính"),
                (nameof(KetQuaXetNghiemExportExcel.NamSinh), "Năm Sinh"),
                (nameof(KetQuaXetNghiemExportExcel.DiaChi), "Địa Chỉ"),
                (nameof(KetQuaXetNghiemExportExcel.TrangThaiDisplay), "Trạng Thái"),
                (nameof(KetQuaXetNghiemExportExcel.NguoiThucHien), "Người Thực Hiện"),
                (nameof(KetQuaXetNghiemExportExcel.NgayThucHienDisplay), "Ngày Thực Hiện"),
                (nameof(KetQuaXetNghiemExportExcel.NguoiDuyetKQ), "Người Duyệt KQ"),
                (nameof(KetQuaXetNghiemExportExcel.NgayDuyetKQDisplay), "Ngày Duyệt KQ"),
                (nameof(KetQuaXetNghiemExportExcel.KetQuaXetNghiemExportExcelChild), "")
            };


            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Danh Sách Kết Quả Xét Nghiệm", labelName: "Danh Sách Kết Quả Xét Nghiệm");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=KetQuaXetNghiem" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";


            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("TrangThaiPhienXetNghiemGanNhat")]
        public async Task<ActionResult> TrangThaiPhienXetNghiemGanNhat(long phienXetNghiemId)
        {
            var type = await _ketQuaXetNghiemService.TrangThaiKQXNGanNhat(phienXetNghiemId);
            return Ok(type);
        }
    }
}
