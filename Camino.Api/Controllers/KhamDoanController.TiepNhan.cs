using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        #region Grid
        [HttpPost("GetDataNhanVienTheoHopDongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataNhanVienTheoHopDongForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataNhanVienTheoHopDongForGridAsyncVer3(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalNhanVienTheoHopDongForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalNhanVienTheoHopDongForGridAsync([FromBody]QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _khamDoanService.GetDataNhanVienTheoHopDongForGridAsyncVer3(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDanhSachDichVuTheoGoiKhamCuaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(TiepNhanDichVuChiDinhQueryVo hopDongQueryInfo)
        {
            var ds = await _khamDoanService.GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(hopDongQueryInfo);
            return Ok(ds);
        }

        [HttpPost("GetDataDichVuChiDinhKhamSucKhoeNhanVienForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetDataDichVuChiDinhKhamSucKhoeNhanVienForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalDichVuChiDinhKhamSucKhoeNhanVienForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetTotalDichVuChiDinhKhamSucKhoeNhanVienForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetTotalDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        #region Get data

        #endregion

        #region Xử lý data
        [HttpPut("XuLyLuuThongTinHopDongKhamNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyLuuThongTinHopDongKhamNhanVienAsync(YeuCauTiepNhanKhamSucKhoeViewModel model)
        {
            var hopDongNhanVienId = model.IsBatDauKhamTuDanhSach ? model.HopDongKhamSucKhoeNhanVienDanhSachId.Value : model.HopDongKhamSucKhoeNhanVien.Id;
            var hopDongKhamNhanVien = await _khamDoanService.GetThongTinHanhChinhNhanVienAsync(hopDongNhanVienId);

            if (model.Id != hopDongKhamNhanVien.Id)
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.HopDongKhamSucKhoeNhanVien.DaBatDauKham"));
            }
            if (model.IsBatDauKhamTuDanhSach)
            {
                model = hopDongKhamNhanVien.ToModel<YeuCauTiepNhanKhamSucKhoeViewModel>();
                var query = new TiepNhanDichVuChiDinhQueryVo()
                {
                    HopDongKhamSucKhoeNhanVienId = hopDongNhanVienId,
                    NgayThangNamSinh = model.HopDongKhamSucKhoeNhanVien.NgayThangNamSinh,
                    NamSinh = model.HopDongKhamSucKhoeNhanVien.NamSinh,
                    GioiTinh = model.HopDongKhamSucKhoeNhanVien.GioiTinh,
                    TinhTrangHonNhan = model.HopDongKhamSucKhoeNhanVien.TinhTrangHonNhan,
                    CoMangThai = model.HopDongKhamSucKhoeNhanVien.CoMangThai
                };
                var lstDichVuTrongGoi = await _khamDoanService.GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(query);
                model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.AddRange(lstDichVuTrongGoi.Select(item => new TiepNhanDichVuChiDinhViewModel()
                {
                    LoaiDichVu = item.LoaiDichVu,
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    DichVuBenhVienId = item.DichVuBenhVienId,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    LoaiGiaId = item.LoaiGiaId,
                    TenLoaiGia = item.TenLoaiGia,
                    SoLan = item.SoLan,
                    DonGiaBenhVien = item.DonGiaBenhVien,
                    DonGiaMoi = item.DonGiaMoi,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    NoiThucHienId = item.NoiThucHienId,
                    TenNoiThucHien = item.TenNoiThucHien,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    TinhTrang = item.TinhTrang,
                    TenTinhTrang = item.TenTinhTrang,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe
                }));
            }

            if (model.Id == 0 && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any() && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems.Any())
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.DichVuChiDinh.Required"));
            }

            var coQuyenCapNhatHanhChinh = true;
            var lstDichVuChiDinh = new List<TiepNhanDichVuChiDinhVo>();
            if (model.Id == 0)
            {
                //if (
                //    !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any(p => p.LaDichVuKham && p.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa)
                //    || !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any(p => p.LaDichVuKham && p.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa)
                //    || !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any(p => p.LaDichVuKham && p.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat)
                //    || !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any(p => p.LaDichVuKham && p.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong)
                //    || !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any(p => p.LaDichVuKham && p.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat))
                //{
                //    throw new ApiException(_localizationService.GetResource("KhamDoan.DichVuKhamSucKhoeDoans.Required"));
                //}

                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }
                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }
            }
            else
            {
                coQuyenCapNhatHanhChinh = _roleService.IsHavePermissionForUpdateInformationTNBN();
                if (!coQuyenCapNhatHanhChinh)
                {
                    model.HopDongKhamSucKhoeNhanVien.HoTen = hopDongKhamNhanVien.HoTen.ToUpper();
                    model.HopDongKhamSucKhoeNhanVien.NgaySinh = hopDongKhamNhanVien.NgaySinh;
                    model.HopDongKhamSucKhoeNhanVien.ThangSinh = hopDongKhamNhanVien.ThangSinh;
                    model.HopDongKhamSucKhoeNhanVien.NamSinh = hopDongKhamNhanVien.NamSinh;
                    model.HopDongKhamSucKhoeNhanVien.SoChungMinhThu = hopDongKhamNhanVien.SoChungMinhThu;
                    model.HopDongKhamSucKhoeNhanVien.GioiTinh = hopDongKhamNhanVien.GioiTinh;
                    model.HopDongKhamSucKhoeNhanVien.NgheNghiepId = hopDongKhamNhanVien.NgheNghiepId;
                    model.HopDongKhamSucKhoeNhanVien.QuocTichId = hopDongKhamNhanVien.QuocTichId;
                    model.HopDongKhamSucKhoeNhanVien.DanTocId = hopDongKhamNhanVien.DanTocId;
                    model.HopDongKhamSucKhoeNhanVien.DiaChi = hopDongKhamNhanVien.DiaChi;
                    model.HopDongKhamSucKhoeNhanVien.PhuongXaId = hopDongKhamNhanVien.PhuongXaId;
                    model.HopDongKhamSucKhoeNhanVien.QuanHuyenId = hopDongKhamNhanVien.QuanHuyenId;
                    model.HopDongKhamSucKhoeNhanVien.TinhThanhId = hopDongKhamNhanVien.TinhThanhId;
                    model.HopDongKhamSucKhoeNhanVien.NhomMau = (int?)hopDongKhamNhanVien.HopDongKhamSucKhoeNhanVien.NhomMau;
                    model.HopDongKhamSucKhoeNhanVien.YeuToRh = (int?)hopDongKhamNhanVien.HopDongKhamSucKhoeNhanVien.YeuToRh;
                    model.HopDongKhamSucKhoeNhanVien.Email = hopDongKhamNhanVien.Email;
                    model.HopDongKhamSucKhoeNhanVien.SoDienThoai = hopDongKhamNhanVien.SoDienThoai;
                    
                    //model.HopDongKhamSucKhoeNhanVien.Noi = yeuCauTiepNhan.NoiLamViec;
                }
            }

            model.ToEntity(hopDongKhamNhanVien);
            await _khamDoanService.XuLyLuuThongTinHopDongKhamNhanVienAsync(hopDongKhamNhanVien, lstDichVuChiDinh, coQuyenCapNhatHanhChinh);
            return Ok();
        }

        private TiepNhanDichVuChiDinhVo mapDichVuChiDinhToVo(TiepNhanDichVuChiDinhViewModel dichVu)
        {
            return new TiepNhanDichVuChiDinhVo()
            {
               // LoaiDichVu = dichVu.LoaiDichVu.Value,
                LoaiDichVuKyThuat = dichVu.LoaiDichVuKyThuat,
                DichVuBenhVienId = dichVu.DichVuBenhVienId,
                Ma = dichVu.Ma,
                Ten = dichVu.Ten,
                LoaiGiaId = dichVu.LoaiGiaId,
                SoLan = dichVu.SoLan,
                DonGiaBenhVien = dichVu.DonGiaBenhVien,
                DonGiaMoi = dichVu.DonGiaMoi,
                DonGiaUuDai = dichVu.DonGiaUuDai,
                DonGiaChuaUuDai = dichVu.DonGiaChuaUuDai,
                NoiThucHienId = dichVu.NoiThucHienId,
                GoiKhamSucKhoeId = dichVu.GoiKhamSucKhoeId,
                TinhTrang = dichVu.TinhTrang.Value,
                ChuyenKhoaKhamSucKhoe = dichVu.ChuyenKhoaKhamSucKhoe,

                //BVHD-3668
                GoiKhamSucKhoeDichVuPhatSinhId = dichVu.GoiKhamSucKhoeDichVuPhatSinhId
            };
        }
        private TiepNhanDichVuChiDinhViewModel mapDichVuChiDinhVoToViewModel(TiepNhanDichVuChiDinhVo dichVu)
        {
            return new TiepNhanDichVuChiDinhViewModel()
            {
                LoaiDichVuKyThuat = dichVu.LoaiDichVuKyThuat,
                DichVuBenhVienId = dichVu.DichVuBenhVienId,
                Ma = dichVu.Ma,
                Ten = dichVu.Ten,
                LoaiGiaId = dichVu.LoaiGiaId,
                SoLan = dichVu.SoLan,
                DonGiaBenhVien = dichVu.DonGiaBenhVien,
                DonGiaMoi = dichVu.DonGiaMoi,
                DonGiaUuDai = dichVu.DonGiaUuDai,
                DonGiaChuaUuDai = dichVu.DonGiaChuaUuDai,
                NoiThucHienId = dichVu.NoiThucHienId,
                GoiKhamSucKhoeId = dichVu.GoiKhamSucKhoeId,
                TinhTrang = dichVu.TinhTrang,
                TenTinhTrang = dichVu.TenTinhTrang,
                ChuyenKhoaKhamSucKhoe = dichVu.ChuyenKhoaKhamSucKhoe,
                TenLoaiGia = dichVu.TenLoaiGia,
                TenGoiKhamSucKhoe = dichVu.TenGoiKhamSucKhoe,
                

                //BVHD-3618
                LaGoiChung = dichVu.LaGoiChung,
                GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId = dichVu.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId,
                GoiKhamSucKhoeChungDichVuKyThuatNhanVienId = dichVu.GoiKhamSucKhoeChungDichVuKyThuatNhanVienId,

                //BVHD-3668
                GoiKhamSucKhoeDichVuPhatSinhId = dichVu.GoiKhamSucKhoeDichVuPhatSinhId,
                TenNoiThucHien = dichVu.TenNoiThucHien
            };
        }

        [HttpPost("KiemTraThemDichVuKhamSucKhoe")]
        public async Task<ActionResult<TiepNhanDichVuChiDinhViewModel>> KiemTraThemDichVuKhamSucKhoeAsync(TiepNhanDichVuChiDinhViewModel dichVuThem)
        {
            // kiểm tra số lượng còn lại trong gói
            if (dichVuThem.GoiKhamSucKhoeId != null)
            {
                var nhomDichVu = dichVuThem.LaDichVuKham ? Enums.EnumNhomGoiDichVu.DichVuKhamBenh : Enums.EnumNhomGoiDichVu.DichVuKyThuat;
                var soLuongConLaiTrongGoi = await _khamDoanService.GetSoLuongConLaiDichVuTrongGoiKhamSucKhoeAsync(dichVuThem.GoiKhamSucKhoeId.Value, dichVuThem.DichVuBenhVienId.Value, nhomDichVu, dichVuThem.YeuCauTiepNhanId);
                if(soLuongConLaiTrongGoi < ((dichVuThem.SoLan ?? 0) + (dichVuThem.SoLanChuaLuu ?? 0)))
                {
                    throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), dichVuThem.Ten, soLuongConLaiTrongGoi - (dichVuThem.SoLanChuaLuu ?? 0)));
                }
            }

            if (dichVuThem.LaDichVuKham)
            {
                dichVuThem.TinhTrang = (int) Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                dichVuThem.TenTinhTrang = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham.GetDescription();
            }
            else
            {
                dichVuThem.TinhTrang = (int) Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                dichVuThem.TenTinhTrang = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien.GetDescription();
            }

            if (dichVuThem.YeuCauTiepNhanId != null)
            {
                var dichVuThemVo = mapDichVuChiDinhToVo(dichVuThem);
                var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(dichVuThem.YeuCauTiepNhanId.Value);
                if (dichVuThemVo.LaDichVuKham
                    && yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Any(x => x.DichVuKhamBenhBenhVienId == dichVuThemVo.DichVuBenhVienId && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
                {
                    throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.DichVuKhamBenh.IsExists"));
                }
                await _khamDoanService.XuLyThemDichVuKhamSucKhoeChiDinhAsync(yeuCauTiepNhanChiTiet, dichVuThemVo);
            }
            else
            {
                dichVuThem.TenLoaiGia = await _khamDoanService.GetTenNhomGiaTheoLoaiDichVuAsync(dichVuThem.LoaiDichVu.Value, dichVuThem.LoaiGiaId.Value);
            }

            
            return dichVuThem;
        }

        [HttpPost("KiemTraThemDichVuKhamSucKhoeMultiselect")]
        public async Task<ActionResult<List<TiepNhanDichVuChiDinhViewModel>>> KiemTraThemDichVuKhamSucKhoeMultiselectAsync(TiepNhanDichVuChiDinhViewModelMultiselect dichVuThem)
        {
            // kiểm tra dịch vụ khám trùng
            await _khamDoanService.KiemTraTrungDichVuKhamTrongGoiKhamSucKhoeAsync(dichVuThem.DichVus, dichVuThem.DichVuGois, dichVuThem.DichVuThems, dichVuThem.YeuCauTiepNhanId);

            // kiểm tra số lượng còn lại trong gói
            //if (dichVuThem.GoiKhamSucKhoeId != null)

            //BVHD-3668
            var dichVuChonThemStr = dichVuThem.DichVus.First();
            var dichVuChonThemObj = JsonConvert.DeserializeObject<KeyIdStringDichVuKhamSucKhoeVo>(dichVuChonThemStr);
            if (dichVuChonThemObj.GoiKhamSucKhoeDichVuPhatSinhId != null || dichVuThem.GoiKhamSucKhoeId != null)
            {
                await _khamDoanService.KiemTraSoLuongConLaiNhieuDichVuTrongGoiKhamSucKhoeAsync(dichVuThem.GoiKhamSucKhoeId ?? dichVuChonThemObj.GoiKhamSucKhoeDichVuPhatSinhId ?? 0, dichVuThem.DichVus, dichVuThem.DichVuGois, dichVuThem.YeuCauTiepNhanId);
            }

            YeuCauTiepNhan yeuCauTiepNhanChiTiet = null;
            if (dichVuThem.YeuCauTiepNhanId != null)
            {
                //yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(dichVuThem.YeuCauTiepNhanId.Value);
                yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiThemDVKhamSucKhoeByIdAsync(dichVuThem.YeuCauTiepNhanId.Value);
            }

            var dichVuThems = await _khamDoanService.XuLyThemDichVuKhamSucKhoeChiDinhAsyncMultiselect(dichVuThem.DichVus, dichVuThem.HinhThucKhamBenh, dichVuThem.HopDongKhamSucKhoeId, yeuCauTiepNhanChiTiet, dichVuThem.HopDongKhamSucKhoeNhanVienId);
            var dichVuThemViewModels = new List<TiepNhanDichVuChiDinhViewModel>();
            foreach (var dichVu in dichVuThems)
            {
                dichVuThemViewModels.Add(mapDichVuChiDinhVoToViewModel(dichVu));
            }
            #region
            if (!string.IsNullOrEmpty(dichVuThem.BieuHienLamSang) || !string.IsNullOrEmpty(dichVuThem.DichTeSarsCoV2))
            {
                _khamBenhService.UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan((long)dichVuThem.YeuCauTiepNhanId, dichVuThem.BieuHienLamSang, dichVuThem.DichTeSarsCoV2);
            }
            #endregion BVHD-3761
            return dichVuThemViewModels;
        }

        [HttpPost("CapNhatGridItemDichVuKhamSucKhoeNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> CapNhatGridItemDichVuKhamSucKhoeNhanVienAsync(CapNhatGridDichVuKhamSucKhoeNhanVienViewModel viewModel)
        {
            //todo: cân nhắc kiểu tra điều kiện của dịch vụ trước khi update
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiSuaXoaDVKhamSucKhoeByIdAsync(viewModel.YeuCauTiepNhanId);
            var flagUpdate = false;

            if (viewModel.LaDichVuKham)
            {
                var item = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == viewModel.YeuCauDichVuBenhVienId);
                flagUpdate = true;
                if (viewModel.IsUpdateDonGia)
                {
                    if (item.Gia != viewModel.DonGia)
                    {
                        if (viewModel.DonGia == null)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.DonGia.Required"));
                        }
                        item.Gia = viewModel.DonGia.Value;
                    }
                }

                if (viewModel.IsUpdateNoiThucHien)
                {
                    if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.NoiThucHienId.Required"));
                    }

                    item.NoiThucHienId = item.NoiDangKyId = viewModel.NoiThucHienId;
                }
            }
            else
            {
                var item = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.First(x => x.Id == viewModel.YeuCauDichVuBenhVienId);
                flagUpdate = true;
                if (viewModel.IsUpdateDonGia)
                {
                    if (item.Gia != viewModel.DonGia)
                    {
                        if (viewModel.DonGia == null)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.DonGia.Required"));
                        }

                        item.Gia = viewModel.DonGia.Value;
                    }
                }

                if (viewModel.IsUpdateSoLan)
                {
                    if (viewModel.SoLan == null || viewModel.SoLan == 0)
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.SoLan.Required"));
                    }

                    item.SoLan = viewModel.SoLan != null ? viewModel.SoLan.Value : item.SoLan;
                }

                if (viewModel.IsUpdateNoiThucHien)
                {
                    if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.NoiThucHienId.Required"));
                    }

                    item.NoiThucHienId = viewModel.NoiThucHienId;
                }
            }

            if (!flagUpdate)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            await _khamDoanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }

        [HttpPost("XuLyXoaDichVuKhamSucKhoeChiDinh")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyXoaDichVuKhamSucKhoeChiDinhAsync(CapNhatGridDichVuKhamSucKhoeNhanVienViewModel xoaDichVuViewModel)
        {
            //var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(xoaDichVuViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhan = await _khamBenhService.GetYeuCauTiepNhanKhiSuaXoaDVKhamSucKhoeByIdAsync(xoaDichVuViewModel.YeuCauTiepNhanId);
            await _khamDoanService.XuLyXoaDichVuKhamSucKhoeChiDinhAsync(yeuCauTiepNhan, xoaDichVuViewModel.YeuCauDichVuBenhVienId, xoaDichVuViewModel.LaDichVuKham);

            //Cập nhật 31/03/2022
            if (xoaDichVuViewModel.XoaDichVuDaChiDinhTrongGoiChung == true)
            {
                await XuLyXoaDichVuGoiChungChuaBatDauKhamAsync(new DichVuGoiChungXoaChuaBatDauKhamVo()
                {
                    GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId = xoaDichVuViewModel.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId,
                    GoiKhamSucKhoeChungDichVuKyThuatNhanVienId = xoaDichVuViewModel.GoiKhamSucKhoeChungDichVuKyThuatNhanVienId
                });
            }
            return Ok();
        }

        [HttpPut("XuLyBatDauKhamNhieuNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyBatDauKhamNhieuNhanVienAsync(HopDongKhamNhanVienBatDauKhamIdViewModel hopDongKham)
        {
            if (!hopDongKham.HopDongKhamSucKhoeNhanVienIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.BatDauKhamNhieuNhanVien.IsEmpty"));
            }
            if (_khamDoanService.CheckHopDongKhamNhanVienDaBatDauKham(hopDongKham.HopDongKhamSucKhoeNhanVienIds))
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.HopDongKhamSucKhoeNhanVien.DaBatDauKham"));
            }

            var hopDongKhamSucKhoeNhanViens = _khamDoanService.GetHopDongKhamSucKhoeNhanViens(hopDongKham.HopDongKhamSucKhoeNhanVienIds);

            var tiepNhanKhamSucKhoeNews = new List<HopDongKhamNhanVienBatDauKhamDetailViewModel>();
            foreach (var hopDongNhanVienId in hopDongKham.HopDongKhamSucKhoeNhanVienIds)
            {
                var thongTinNhanVienKham = new YeuCauTiepNhan();
                thongTinNhanVienKham.HopDongKhamSucKhoeNhanVien = hopDongKhamSucKhoeNhanViens.First(o=>o.Id == hopDongNhanVienId);

                var newItem = new HopDongKhamNhanVienBatDauKhamDetailViewModel()
                {
                    HopDongKhamSucKhoeNhanVienId = hopDongNhanVienId,
                    HopDongKhamNhanVienModel = thongTinNhanVienKham.ToModel<YeuCauTiepNhanKhamSucKhoeViewModel>(),
                    HopDongKhamNhanVienEntity = thongTinNhanVienKham
                };
                tiepNhanKhamSucKhoeNews.Add(newItem);
            }
            var dichVuKhamBenhBenhViens = _khamDoanService.GetDichVuKhamBenhBenhViens();
            var dichVuKyThuatBenhViens = _khamDoanService.GetDichVuKyThuatBenhViens();
            var templateDichVuKhamSucKhoes = _khamDoanService.GetTemplateDichVuKhamSucKhoes();
            var icdKhamSucKhoe = _khamDoanService.GetIcdKhamSucKhoe();
            foreach (var tiepNhan in tiepNhanKhamSucKhoeNews)
            {
                var model = tiepNhan.HopDongKhamNhanVienModel;
                var hopDongKhamNhanVien = tiepNhan.HopDongKhamNhanVienEntity;

                var query = new TiepNhanDichVuChiDinhQueryVo()
                {
                    HopDongKhamSucKhoeNhanVienId = tiepNhan.HopDongKhamSucKhoeNhanVienId,
                    NgayThangNamSinh = model.HopDongKhamSucKhoeNhanVien.NgayThangNamSinh,
                    NamSinh = model.HopDongKhamSucKhoeNhanVien.NamSinh,
                    GioiTinh = model.HopDongKhamSucKhoeNhanVien.GioiTinh,
                    TinhTrangHonNhan = model.HopDongKhamSucKhoeNhanVien.TinhTrangHonNhan,
                    CoMangThai = model.HopDongKhamSucKhoeNhanVien.CoMangThai
                };
                var lstDichVuTrongGoi = await _khamDoanService.GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(query);
                model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.AddRange(lstDichVuTrongGoi.Select(item => new TiepNhanDichVuChiDinhViewModel()
                {
                    LoaiDichVu = item.LoaiDichVu,
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    DichVuBenhVienId = item.DichVuBenhVienId,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    LoaiGiaId = item.LoaiGiaId,
                    TenLoaiGia = item.TenLoaiGia,
                    SoLan = item.SoLan,
                    DonGiaBenhVien = item.DonGiaBenhVien,
                    DonGiaMoi = item.DonGiaMoi,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    NoiThucHienId = item.NoiThucHienId,
                    TenNoiThucHien = item.TenNoiThucHien,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    TinhTrang = item.TinhTrang,
                    TenTinhTrang = item.TenTinhTrang,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe
                }));

                if (model.Id == 0 && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any() && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems.Any())
                {
                    throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.DichVuChiDinh.Required"));
                }

                var lstDichVuChiDinh = new List<TiepNhanDichVuChiDinhVo>();
                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }
                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }

                model.ToEntity(hopDongKhamNhanVien);
                await _khamDoanService.XuLyLuuThongTinHopDongKhamNhanVienAsync(hopDongKhamNhanVien, lstDichVuChiDinh,true, dichVuKhamBenhBenhViens, dichVuKyThuatBenhViens,templateDichVuKhamSucKhoes, icdKhamSucKhoe);
            }
            return Ok();
        }

        [HttpPut("XuLyBatDauKhamNhieuNhanVienOld")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyBatDauKhamNhieuNhanVienAsyncOld(HopDongKhamNhanVienBatDauKhamIdViewModel hopDongKham)
        {
            if (!hopDongKham.HopDongKhamSucKhoeNhanVienIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.BatDauKhamNhieuNhanVien.IsEmpty"));
            }

            var tiepNhanKhamSucKhoeNews = new List<HopDongKhamNhanVienBatDauKhamDetailViewModel>();
            foreach (var hopDongNhanVienId in hopDongKham.HopDongKhamSucKhoeNhanVienIds)
            {
                var hopDongKhamNhanVien = await _khamDoanService.GetThongTinHanhChinhNhanVienAsync(hopDongNhanVienId);
                if (hopDongKhamNhanVien.Id != 0)
                {
                    throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.HopDongKhamSucKhoeNhanVien.DaBatDauKham"));
                }

                var newItem = new HopDongKhamNhanVienBatDauKhamDetailViewModel()
                {
                    HopDongKhamSucKhoeNhanVienId = hopDongNhanVienId,
                    HopDongKhamNhanVienModel = hopDongKhamNhanVien.ToModel<YeuCauTiepNhanKhamSucKhoeViewModel>(),
                    HopDongKhamNhanVienEntity = hopDongKhamNhanVien
                };
                tiepNhanKhamSucKhoeNews.Add(newItem);
            }

            foreach (var tiepNhan in tiepNhanKhamSucKhoeNews)
            {
                var model = tiepNhan.HopDongKhamNhanVienModel;
                var hopDongKhamNhanVien = tiepNhan.HopDongKhamNhanVienEntity;

                var query = new TiepNhanDichVuChiDinhQueryVo()
                {
                    HopDongKhamSucKhoeNhanVienId = tiepNhan.HopDongKhamSucKhoeNhanVienId,
                    NgayThangNamSinh = model.HopDongKhamSucKhoeNhanVien.NgayThangNamSinh,
                    NamSinh = model.HopDongKhamSucKhoeNhanVien.NamSinh,
                    GioiTinh = model.HopDongKhamSucKhoeNhanVien.GioiTinh,
                    TinhTrangHonNhan = model.HopDongKhamSucKhoeNhanVien.TinhTrangHonNhan,
                    CoMangThai = model.HopDongKhamSucKhoeNhanVien.CoMangThai
                };
                var lstDichVuTrongGoi = await _khamDoanService.GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(query);
                model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.AddRange(lstDichVuTrongGoi.Select(item => new TiepNhanDichVuChiDinhViewModel()
                {
                    LoaiDichVu = item.LoaiDichVu,
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    DichVuBenhVienId = item.DichVuBenhVienId,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    LoaiGiaId = item.LoaiGiaId,
                    TenLoaiGia = item.TenLoaiGia,
                    SoLan = item.SoLan,
                    DonGiaBenhVien = item.DonGiaBenhVien,
                    DonGiaMoi = item.DonGiaMoi,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    NoiThucHienId = item.NoiThucHienId,
                    TenNoiThucHien = item.TenNoiThucHien,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    TinhTrang = item.TinhTrang,
                    TenTinhTrang = item.TenTinhTrang,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe
                }));

                if (model.Id == 0 && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois.Any() && !model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems.Any())
                {
                    throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.DichVuChiDinh.Required"));
                }

                var lstDichVuChiDinh = new List<TiepNhanDichVuChiDinhVo>();
                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhTrongGois)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }
                foreach (var dichVu in model.HopDongKhamSucKhoeNhanVien.DichVuChiDinhThems)
                {
                    lstDichVuChiDinh.Add(mapDichVuChiDinhToVo(dichVu));
                }

                model.ToEntity(hopDongKhamNhanVien);
                await _khamDoanService.XuLyLuuThongTinHopDongKhamNhanVienAsync(hopDongKhamNhanVien, lstDichVuChiDinh);
            }
            return Ok();
        }


        [HttpPost("XuLyXoaDichVuGoiChungChuaBatDauKham")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyXoaDichVuGoiChungChuaBatDauKhamAsync(DichVuGoiChungXoaChuaBatDauKhamVo xoaDichVuVo)
        {
            await _khamDoanService.XuLyXoaDichVuGoiChungChuaBatDauKhamAsync(xoaDichVuVo);

            return Ok();
        }

        [HttpPut("XuLyQuayLaiChuaKhamNhieuNhanVien")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> XuLyQuayLaiChuaKhamNhieuNhanVienAsync(HopDongKhamNhanVienBatDauKhamIdViewModel hopDongKham)
        {
            if (!hopDongKham.HopDongKhamSucKhoeNhanVienIds.Any())
            {
                throw new ApiException(_localizationService.GetResource("TiepNhanKhamDoan.QuayLaiChuaKhamNhieuNhanVien.IsEmpty"));
            }
            
            // kiểm tra xem có YCTN ngoại trú nào được tạo từ YCTN KSK hay ko, nếu có thì không cho quay lại chưa khám
            await _khamDoanService.KiemTraNguoiBenhCanQuayLaiChuaKhamKSK(hopDongKham.HopDongKhamSucKhoeNhanVienIds);

            // xử lý quay lại chưa khám
            await _khamDoanService.XuLyQuayLaiChuaKhamNhieuNhanVienAsync(hopDongKham.HopDongKhamSucKhoeNhanVienIds);
            return Ok();
        }
        #endregion

        #region Xử lý html, pdf
        [HttpPost("GetFilePDFHoSoKhamSucKhoeFromHtml")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanTiepNhan)]
        public async Task<ActionResult> GetFilePDFHoSoKhamSucKhoeFromHtmlAsync(string obj)
        {
            
            if(!string.IsNullOrEmpty(obj))
            {
                var hoSoIn = JsonConvert.DeserializeObject<List<InHoSoKhamSucKhoeVo>>(obj);
                var phieuIn = string.Empty;
                var query = new PhieuInNhanVienKhamSucKhoeInfoVo()
                {
                    Id = hoSoIn.Any() ? hoSoIn.Select(d => d.HopDongKhamSucKhoeNhanVienId).First() : 0,
                    HostingName = hoSoIn.Any() ? hoSoIn.Select(d => d.HostingName).First() : ""
                };
                var list = new List<HtmlToPdfVo>();
                PhieuInNhanVienKhamSucKhoeViewModel htmlContent = new PhieuInNhanVienKhamSucKhoeViewModel();

                foreach (var item in hoSoIn)
                {
                    htmlContent.TenFile = item.TenFile;

                    var footerHtml = string.Empty;


                    switch (item.LoaiHoSoKhamSucKhoe)
                    {
                        case Enums.LoaiHoSoKhamSucKhoe.BangHuongDanKhamSucKhoe:
                            phieuIn = await _khamDoanService.XuLyInBangHuongDanKhamSucKhoeAsync(item);
                            // BVHD-3946
                            footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(0);
                            // BVHD-3946 
                            var htmlToPdfVoBangHuongDanKhamSucKhoe = new HtmlToPdfVo
                            {
                                Html = phieuIn,
                                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                                Bottom = 15
                            };
                            list.Add(htmlToPdfVoBangHuongDanKhamSucKhoe);
                            break;
                        case Enums.LoaiHoSoKhamSucKhoe.PhieuDangKyKhamSucKhoe:
                            phieuIn = await _khamDoanService.XuLyInPhieuDangKyKhamSucKhoeAsync(query);
                            // BVHD-3946
                            footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(1); 
                            // BVHD-3946 
                            var htmlToPdfVoPhieuDangKyKhamSucKhoe = new HtmlToPdfVo
                            {
                                Html = phieuIn,
                                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                                Bottom = 15
                            };
                            list.Add(htmlToPdfVoPhieuDangKyKhamSucKhoe);
                            break;
                        case Enums.LoaiHoSoKhamSucKhoe.SoKhamSucKhoeDinhKy:
                            phieuIn = await _khamDoanService.XuLyInKhamSucKhoeAsync(query);

                            // BVHD-3946
                            footerHtml = _khamDoanService.GetTemplatePhieuDangKyKham(0);
                            // BVHD-3946 

                            var htmlToPdfVoSoKhamSucKhoeDinhKy = new HtmlToPdfVo
                            {
                                Html = phieuIn,
                                FooterHtml = htmlContent.NoFooter != true ? footerHtml : "",
                                Bottom = 15
                            };
                            list.Add(htmlToPdfVoSoKhamSucKhoeDinhKy);
                            break;
                    }
                }


                var bytes = _pdfService.ExportMultiFilePdfFromHtml(list);
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + htmlContent.TenFile + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
                Response.ContentType = "application/pdf";
                return new FileContentResult(bytes, "application/pdf");
            }
            return null;
           
        }

        #endregion
        #region in dịch vụ ngoài gói khám đoàn
        [HttpPost("InDichVuChiDinhPhatSinh")]
        public ActionResult<string> InDichVuChiDinhPhatSinh([FromBody]InChiDinhDichVuNgoaiGoiKhamDoanViewModel inChiDinhDichVuNgoaiGoiKhamDoanViewModel)
        {
            var result = _khamDoanService.InDichVuChiDinhPhatSinh(inChiDinhDichVuNgoaiGoiKhamDoanViewModel.Hosting,
                                                                  inChiDinhDichVuNgoaiGoiKhamDoanViewModel.YeuCauTiepNhanId, 
                                                                  inChiDinhDichVuNgoaiGoiKhamDoanViewModel.DichVuChiDinhIns
                                                                  .Select(d=> new DichVuChiDinhInGridVo 
                                                                  { 
                                                                      DichVuChiDinhId = d.DichVuChiDinhId,
                                                                      NhomChiDinhId = d.NhomChiDinhId
                                                                  }).ToList());
            return result;
        }
        #endregion
       
    }
}
