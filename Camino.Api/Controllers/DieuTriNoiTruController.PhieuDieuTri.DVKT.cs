using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Extensions;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Helpers;

namespace Camino.Api.Controllers
{
    public partial class DieuTriNoiTruController
    {
        [HttpPost("ThemYeuCauDichVuKyThuatMultiSelect")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauDichVuKyThuatMultiselectAsync([FromBody] PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel yeuCauViewModel)
        {
            var yeuCauVo = yeuCauViewModel.Map<ChiDinhDichVuKyThuatMultiselectVo>();
            //var yeuCauTiepNhanChiTiet = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauViewModel.YeuCauTiepNhanId
            //    , p => p.Include(o => o.YeuCauDichVuKyThuats)
            //    .Include(o => o.NoiTruBenhAn).ThenInclude(o => o.NoiTruPhieuDieuTris)
            //    );

            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            //20/12/2021: Xử lý chỉ include những entity cần thiết
            var yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(yeuCauViewModel.YeuCauTiepNhanId,
                a => a.Include(x => x.YeuCauDichVuKyThuats)
                            .Include(x => x.DoiTuongUuDai).ThenInclude(x => x.DoiTuongUuDaiDichVuKyThuatBenhViens).ThenInclude(x => x.DichVuKyThuatBenhVien)
                            .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris));

            if (yeuCauTiepNhanChiTiet != null && yeuCauTiepNhanChiTiet.NoiTruBenhAn != null && yeuCauTiepNhanChiTiet.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            // kiểm tra nếu là dịch vụ thêm từ gói
            var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                NoiTruPhieuDieuTriId = yeuCauViewModel.PhieuDieuTriId
            };
            if (yeuCauViewModel.DichVuKyThuatTuGois != null && yeuCauViewModel.DichVuKyThuatTuGois.Any())
            {
                dichVuChiDinhTheoGoiVo.DichVus.AddRange(yeuCauViewModel.DichVuKyThuatTuGois
                    .Select(item => new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                    {
                        Id = item.Id,
                        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId ?? 0,
                        TenDichVu = item.TenDichVu,
                        ChuongTrinhGoiDichVuId = item.ChuongTrinhGoiDichVuId ?? 0,
                        ChuongTrinhGoiDichVuChiTietId = item.ChuongTrinhGoiDichVuChiTietId ?? 0,
                        DichVuBenhVienId = item.DichVuBenhVienId ?? 0,
                        NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        SoLuongSuDung = 1
                    }));

                var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).ToList();
                foreach (var strId in lstDichVuKyThuatDangChon)
                {
                    var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);
                    if (dichVuChiDinhTheoGoiVo.DichVus.Any(x => x.DichVuBenhVienId == itemObj.DichVuId))
                    {
                        var dichVuLoaiBo = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.FirstOrDefault(x => x == strId);
                        yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Remove(dichVuLoaiBo);
                    }
                }

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);
            }
            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                await _dieuTriNoiTruService.XuLyThemYeuCauDichVuKyThuatMultiselectAsync(yeuCauVo, yeuCauTiepNhanChiTiet, yeuCauViewModel.PhieuDieuTriId);
            }

            //await _dieuTriNoiTruService.XuLyThemYeuCauDichVuKyThuatMultiselectAsync(yeuCauVo, yeuCauTiepNhanChiTiet, yeuCauViewModel.PhieuDieuTriId);

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            
            #region BVHD-3761
            if (!string.IsNullOrEmpty(yeuCauViewModel.BieuHienLamSang) || !string.IsNullOrEmpty(yeuCauViewModel.DichTeSarsCoV2))
            {
                _khamBenhService.UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan(yeuCauViewModel.YeuCauTiepNhanId, yeuCauViewModel.BieuHienLamSang, yeuCauViewModel.DichTeSarsCoV2);
            }
            #endregion BVHD-3761
            return Ok();
        }

        [HttpPost("CapNhatGridItemDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult> CapNhatGridItemDichVuKyThuatAsync(GridItemYeuCauDichVuKyThuatViewModel viewModel)
        {
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId);

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            //var yeuCauKhamBenhDangKham = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == viewModel.YeuCauKhamBenhId);
            var yeuCauTiepNhanChiTiet = new YeuCauTiepNhan();
            if (viewModel.IsDichVuKham != true)
            {
                if (viewModel.IsUpdateNoiThucHien || viewModel.IsUpdateNguoiThucHien || viewModel.IsUpdateBenhPhamXetNghiem)
                {
                    yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(viewModel.YeuCauTiepNhanId,
                        x => x.Include(a => a.YeuCauDichVuKyThuats));
                }
                else
                {
                    yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNoiTruByIdAsync(viewModel.YeuCauTiepNhanId);
                }
            }
            else
            {
                if (viewModel.IsUpdateNoiThucHien || viewModel.IsUpdateNguoiThucHien || viewModel.IsUpdateBenhPhamXetNghiem)
                {
                    yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(viewModel.YeuCauTiepNhanId,
                    x => x.Include(a => a.YeuCauKhamBenhs));
                }
                else
                {
                    yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKhamNgoaiTruByIdAsync(viewModel.YeuCauTiepNhanId);
                }
            }

            var flagUpdate = false;
            //BVHD-3575: bổ sung thêm dịch vụ khám
            if (viewModel.IsDichVuKham != true)
            {
                foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
                {
                    if (item.Id == viewModel.YeuCauDichVuKyThuatId)
                    {
                        flagUpdate = true;

                        long? yeuCauGoiDichVuKhuyenMaiId = null;
                        if (viewModel.IsUpdateLoaiGia)
                        {
                            if (viewModel.NhomGiaDichVuKyThuatBenhVienId != null &&
                                item.NhomGiaDichVuKyThuatBenhVienId != viewModel.NhomGiaDichVuKyThuatBenhVienId)
                            {
                                if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                                {
                                    throw new ApiException(
                                        _localizationService.GetResource("ChiDinh.LoaiGia.DaThanhToan"));
                                }

                                item.NhomGiaDichVuKyThuatBenhVienId = viewModel.NhomGiaDichVuKyThuatBenhVienId.Value;
                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(
                                    item.DichVuKyThuatBenhVienId, viewModel.NhomGiaDichVuKyThuatBenhVienId.Value);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(
                                        _localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                //BVHD-3825
                                // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                                if (item.MienGiamChiPhis.Any(x =>
                                    x.DaHuy != true && x.YeuCauGoiDichVuId != null &&
                                    (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                                {
                                    viewModel.IsSwapDichVuKhuyenMai = true;
                                    viewModel.LaDichVuKhuyenMai = true;
                                    yeuCauGoiDichVuKhuyenMaiId = item.MienGiamChiPhis
                                        .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null &&
                                                    (x.TaiKhoanBenhNhanThuId == null ||
                                                     x.TaiKhoanBenhNhanThu.DaHuy != true))
                                        .YeuCauGoiDichVuId.Value;
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        if (viewModel.IsUpdateSoLan)
                        {
                            if (viewModel.SoLan == null || viewModel.SoLan == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("DichVuKyThuat.SoLan.Range"));
                            }

                            // kiểm tra nếu là dịch vụ chỉ đinh từ gói marketing
                            if (item.YeuCauGoiDichVuId != null)
                            {
                                var soLuongConLai =
                                    await _khamBenhService.GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(
                                        item.YeuCauGoiDichVuId.Value, item.DichVuKyThuatBenhVienId);
                                var soLuongKhaDung = soLuongConLai + item.SoLan;
                                if (soLuongKhaDung < viewModel.SoLan)
                                {
                                    throw new ApiException(string.Format(
                                        _localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"),
                                        item.TenDichVu, soLuongKhaDung));
                                }
                            }

                            // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                            if (item.MienGiamChiPhis.Any(x =>
                                x.DaHuy != true && x.YeuCauGoiDichVuId != null &&
                                (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                            {
                                var yeuCauGoiId = item.MienGiamChiPhis
                                    .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null &&
                                                (x.TaiKhoanBenhNhanThuId == null ||
                                                 x.TaiKhoanBenhNhanThu.DaHuy != true))
                                    .YeuCauGoiDichVuId.Value;
                                var soLuongConLai =
                                    await _khamBenhService
                                        .GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(
                                            yeuCauGoiId, item.DichVuKyThuatBenhVienId);
                                var soLuongKhaDung = soLuongConLai + item.SoLan;
                                if (soLuongKhaDung < viewModel.SoLan)
                                {
                                    throw new ApiException(string.Format(
                                        _localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"),
                                        item.TenDichVu, soLuongKhaDung));
                                }

                                viewModel.IsSwapDichVuKhuyenMai = true;
                                viewModel.LaDichVuKhuyenMai = true;
                                yeuCauGoiDichVuKhuyenMaiId = yeuCauGoiId;
                            }

                            item.SoLan = viewModel.SoLan != null ? viewModel.SoLan.Value : item.SoLan;
                        }

                        if (viewModel.IsUpdateNoiThucHien)
                        {
                            if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                            {
                                throw new ApiException(
                                    _localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
                            }

                            var queryInfo = new DropDownListRequestModel()
                            {
                                //Id = viewModel.NoiThucHienId.Value,
                                ParameterDependencies = "{NoiThucHienId:" + viewModel.NoiThucHienId.Value + "}",
                                Take = 50
                            };

                            var lstBacSiKhams = await _yeuCauKhamBenhService.GetBacSiKhams(queryInfo);

                            item.NoiThucHienId = viewModel.NoiThucHienId;

                            // nếu nơi thực hiện mới ko có bác sĩ đang chọn, thì tự dộng chọn bác sĩ đầu tiên
                            if (lstBacSiKhams.Any() && !lstBacSiKhams.Any(x => x.KeyId == viewModel.NguoiThucHienId))
                            {
                                item.NhanVienThucHienId = lstBacSiKhams.First().KeyId;
                            }
                            else
                            {
                                item.NhanVienThucHienId = null;
                            }
                        }

                        if (viewModel.IsUpdateNguoiThucHien)
                        {
                            item.NhanVienThucHienId = viewModel.NguoiThucHienId;
                        }

                        if (viewModel.IsUpdateBenhPhamXetNghiem)
                        {
                            item.BenhPhamXetNghiem = viewModel.BenhPhamXetNghiem;
                        }

                        if (viewModel.IsUpdateGioThucHien)
                        {
                            var ngayDieuTri = item.NoiTruPhieuDieuTri?.NgayDieuTri ?? DateTime.Now;
                            var gioDangKy = viewModel.GioBatDau ?? DateTime.Now;
                            item.ThoiDiemDangKy =
                                ngayDieuTri.Date.AddSeconds(
                                    gioDangKy.Hour * 3600 +
                                    gioDangKy.Minute * 60); //viewModel.GioBatDau ?? DateTime.Now;
                        }

                        if (viewModel.IsUpdateDuocHuongBaoHiem)
                        {
                            item.DuocHuongBaoHiem = viewModel.DuocHuongBaoHiem ?? false;
                        }

                        if (viewModel.IsSwapDichVuGoi)
                        {
                            if (viewModel.LaDichVuTrongGoi == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKyThuatBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                    SoLuong = item.SoLan
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                item.Gia = thongTin.DonGia;
                                item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                                {
                                    item.SoTienBaoHiemTuNhanChiTra = null;
                                    item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                    {
                                        mienGiam.WillDelete = true;
                                    }

                                    foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    {
                                        congNo.WillDelete = true;
                                    }
                                }
                            }
                            else
                            {
                                item.DonGiaTruocChietKhau = null;
                                item.DonGiaSauChietKhau = null;
                                item.YeuCauGoiDichVuId = null;

                                var donGiaBenhVien =
                                    await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(
                                        item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(
                                        _localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        if (viewModel.IsUpdateTinhPhi)
                        {
                            if (item.YeuCauGoiDichVuId != null)
                            {
                                throw new ApiException(
                                    _localizationService.GetResource("ChiDinh.TinhPhi.LaDichVuTrongGoi"));
                            }

                            item.KhongTinhPhi = !viewModel.TinhPhi;
                        }

                        //BVHD-3825
                        if (viewModel.IsSwapDichVuKhuyenMai)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(
                                    _localizationService.GetResource(
                                        "DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                            }

                            if (viewModel.LaDichVuKhuyenMai == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKyThuatBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                    SoLuong = item.SoLan,
                                    NhomGiaId = item.NhomGiaDichVuKyThuatBenhVienId
                                };

                                //dùng cho trường hợp cập nhật số lượng hoặc loại giá
                                if (viewModel.IsUpdateLoaiGia || viewModel.IsUpdateSoLan)
                                {
                                    thongTin.YeuCauDichVuCapNhatSoLuongLoaiGiaId = item.Id;
                                    thongTin.YeucauGoiDichVuKhuyenMaiId = yeuCauGoiDichVuKhuyenMaiId;
                                }

                                await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(
                                    thongTin);

                                if (item.MienGiamChiPhis.Any(x =>
                                    x.DaHuy != true &&
                                    (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) &&
                                    x.YeuCauGoiDichVuId != null))
                                {
                                    //item.SoTienBaoHiemTuNhanChiTra = null;
                                    //item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a =>
                                        a.DaHuy != true &&
                                        (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) &&
                                        a.YeuCauGoiDichVuId != null))
                                    {
                                        mienGiam.DaHuy = true;
                                        mienGiam.WillDelete = true;

                                        var giamSoTienMienGiam =
                                            item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        if (giamSoTienMienGiam < 0)
                                        {
                                            giamSoTienMienGiam = 0;
                                        }

                                        item.SoTienMienGiam = giamSoTienMienGiam;
                                    }

                                    //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    //{
                                    //    congNo.WillDelete = true;
                                    //}
                                }

                                //trường hợp cập nhật số lượng hoặc loại giá thì giữ nguyên đơn giá chỉ định trước đó
                                if (!viewModel.IsUpdateLoaiGia && !viewModel.IsUpdateSoLan)
                                {
                                    item.Gia = thongTin.DonGia;
                                }

                                var thanhTien = item.SoLan * item.Gia;
                                var thanhTienMienGiam = item.SoLan * thongTin.DonGiaKhuyenMai.Value;

                                var tongTienMienGiam = (thanhTien > thanhTienMienGiam)
                                    ? (thanhTien - thanhTienMienGiam)
                                    : 0;
                                item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                                item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                    SoTien = item.SoTienMienGiam.Value,
                                    YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                                });

                                //BVHD-3575
                                item.KhongTinhPhi = null;
                            }
                            else
                            {
                                var donGiaBenhVien =
                                    await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(
                                        item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(
                                        _localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                                var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x =>
                                    x.DaHuy != true && x.YeuCauGoiDichVuId != null &&
                                    (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                                if (mienGiam != null)
                                {
                                    mienGiam.DaHuy = true;
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }

                                    item.SoTienMienGiam = giamSoTienMienGiam;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            #region //BVHD-3575: bổ sung thêm dịch vụ khám
            else
            {
                foreach (var item in yeuCauTiepNhanChiTiet.YeuCauKhamBenhs)
                {
                    if (item.Id == viewModel.YeuCauDichVuKyThuatId) // gán tạm id yêu cầu khám bằng biến này truyền từ FE
                    {
                        flagUpdate = true;
                        if (viewModel.IsSwapDichVuGoi)
                        {
                            if (viewModel.LaDichVuTrongGoi == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKhamBenhBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                    SoLuong = 1
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                item.Gia = thongTin.DonGia;
                                item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                                {
                                    item.SoTienBaoHiemTuNhanChiTra = null;
                                    item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                    {
                                        mienGiam.WillDelete = true;
                                    }

                                    foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    {
                                        congNo.WillDelete = true;
                                    }
                                }
                            }
                            else
                            {
                                item.DonGiaTruocChietKhau = null;
                                item.DonGiaSauChietKhau = null;
                                item.YeuCauGoiDichVuId = null;

                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(item.DichVuKhamBenhBenhVienId, item.NhomGiaDichVuKhamBenhBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        //BVHD-3825
                        if (viewModel.IsSwapDichVuKhuyenMai)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                            }

                            if (viewModel.LaDichVuKhuyenMai == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKhamBenhBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                    SoLuong = 1,
                                    NhomGiaId = item.NhomGiaDichVuKhamBenhBenhVienId
                                };
                                await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                                {
                                    //item.SoTienBaoHiemTuNhanChiTra = null;
                                    //item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                    {
                                        mienGiam.DaHuy = true;
                                        mienGiam.WillDelete = true;

                                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        if (giamSoTienMienGiam < 0)
                                        {
                                            giamSoTienMienGiam = 0;
                                        }
                                        item.SoTienMienGiam = giamSoTienMienGiam;
                                    }

                                    //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    //{
                                    //    congNo.WillDelete = true;
                                    //}
                                }

                                item.Gia = thongTin.DonGia;
                                var thanhTien = item.Gia;
                                var thanhTienMienGiam = thongTin.DonGiaKhuyenMai.Value;

                                var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                                item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                                item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                    SoTien = tongTienMienGiam,
                                    YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                                });
                                item.KhongTinhPhi = null;
                            }
                            else
                            {
                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(item.DichVuKhamBenhBenhVienId, item.NhomGiaDichVuKhamBenhBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                                var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                                if (mienGiam != null)
                                {
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    item.SoTienMienGiam = giamSoTienMienGiam;
                                }
                            }
                        }

                        if (viewModel.IsUpdateTinhPhi)
                        {
                            if (item.YeuCauGoiDichVuId != null)
                            {
                                throw new ApiException(
                                    _localizationService.GetResource("ChiDinh.TinhPhi.LaDichVuTrongGoi"));
                            }

                            item.KhongTinhPhi = !viewModel.TinhPhi;
                        }

                        if (viewModel.IsUpdateNoiThucHien)
                        {
                            if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                            {
                                throw new ApiException(
                                    _localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
                            }
                            item.NoiDangKyId = viewModel.NoiThucHienId;
                        }
                        break;
                    }
                }
            }
            #endregion

            if (!flagUpdate)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return Ok();
        }

        #region Update gói Marketing
        [HttpGet("KiemTraDangKyGoiDichVuTheoBenhNhan")]
        public async Task<ActionResult<bool>> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            var kiemTra = await _khamBenhService.KiemTraDangKyGoiDichVuTheoBenhNhanAsync(benhNhanId);
            return kiemTra;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId ?? 0);

            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo, true, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo, true);
            return Ok(gridData);
        }

        [HttpPost("KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList(), chiDinhViewModel.NoiTruPhieuDieuTriId);
            return dichVuTrung;
        }


        [HttpPost("ThemChiDinhGoiDichVuTheoBenhNhan")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuKyThuatNoiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            #region //BVHD-3575: bổ sung thêm dv khám từ nội trú
            var lstDichVuKhamThemTuNoiTru =
                yeuCauVo.YeuCauKhamBenhNews.Where(x => x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true).ToList();
            if (lstDichVuKhamThemTuNoiTru.Any())
            {
                if (yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null)
                {
                    await _dieuTriNoiTruService.XuLyTaoYeuCauNgoaiTruTheoNoiTru(yeuCauTiepNhanChiTiet);
                }

                //var yeuCauTiepNhanNgoaiTru = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                var yeuCauTiepNhanNgoaiTru = await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                foreach (var dichVuKham in lstDichVuKhamThemTuNoiTru)
                {
                    dichVuKham.YeuCauTiepNhanId = yeuCauTiepNhanNgoaiTru.Id;
                    yeuCauTiepNhanNgoaiTru.YeuCauKhamBenhs.Add(dichVuKham);
                }

                await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanNgoaiTru);
            }
            #endregion

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;
            return chiDinhDichVuResultVo;
        }

        [HttpPost("KiemTraDichVuKyThuatChiDinhCoTrongGoiCuaBenhNhan")]
        public async Task<ActionResult<DichVuChiDinhCoTrongGoiCuaBenhNhanVo>> KiemTraDichVuKyThuatChiDinhCoTrongGoiCuaBenhNhanAsync([FromBody] KhamBenhChiDinhDichVuKyThuatMultiselectViewModel yeuCauViewModel)
        {
            //todo: có update bỏ await
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(yeuCauViewModel.YeuCauTiepNhanId, x => x.Include(a => a.BenhNhan));
            var yeuCauVo = new DichVuChiDinhCoTrongGoiCuaBenhNhanVo()
            {
                BenhNhanId = yeuCauTiepNhan.BenhNhanId.Value
            };

            var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).Distinct().ToList();
            foreach (var strId in lstDichVuKyThuatDangChon)
            {
                var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);

                yeuCauVo.DichVuKyThuatIds.Add(itemObj.DichVuId);
            }

            await _khamBenhService.KiemTraDichVuChiDinhCoTrongGoiCuaBenhNhanAsync(yeuCauVo);

            if (yeuCauVo.DichVuChiDinhCoTrongGois.Any())
            {
                var messTemplate = _localizationService.GetResource("KhamBenh.ChiDinh.DichVuChiDingTrungTrongGoi");
                yeuCauVo.Message =
                    string.Format(messTemplate, string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenDichVu).Distinct().ToList()),
                        string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenGoiDichVu).Distinct().ToList()),
                        yeuCauTiepNhan.BenhNhan.HoTen);
            }

            return yeuCauVo;
        }

        [HttpPost("KiemTraValidationChiDinhDichVuTrongGoiMarketing")]
        public async Task<ActionResult> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            if (!chiDinhViewModel.DichVus.Any())
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
            }
            if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
            }

            var yeuCauVo = chiDinhViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);
            return Ok();
        }
        #endregion

        #region Update nhóm thường dùng
        [HttpPost("KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>>> KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.GoiDichVuIds, null, (chiDinhViewModel.LaPhauThuatThuThuat ?? false), chiDinhViewModel.PhieuDieuTriId);
            return dichVuTrung;
        }

        [HttpPost("ThemYeuGoiDichVuThuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhSachDieuTriNoiTru)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel yeuCauViewModel)
        {
            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuKyThuatNoiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();

            ////BVHD-3575
            //if (yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            //{
            //    var tiepNhanNgoaiTru = _yeuCauTiepNhanService.GetById(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
            //    yeuCauVo.TiepNhanNgoaiTruCoBHYT = tiepNhanNgoaiTru.CoBHYT;
            //}

            await _khamBenhService.XuLyThemGoiDichVuThuongDungAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);


            #region //BVHD-3575: bổ sung thêm dv khám từ nội trú
            var lstDichVuKhamThemTuNoiTru =
                yeuCauVo.YeuCauKhamBenhNews.Where(x => x.LaChiDinhTuNoiTru != null && x.LaChiDinhTuNoiTru == true).ToList();
            if (lstDichVuKhamThemTuNoiTru.Any())
            {
                if (yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null)
                {
                    await _dieuTriNoiTruService.XuLyTaoYeuCauNgoaiTruTheoNoiTru(yeuCauTiepNhanChiTiet);
                }

                var yeuCauTiepNhanNgoaiTru = await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauTiepNhanChiTiet.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
                foreach (var dichVuKham in lstDichVuKhamThemTuNoiTru)
                {
                    dichVuKham.YeuCauTiepNhanId = yeuCauTiepNhanNgoaiTru.Id;
                    yeuCauTiepNhanNgoaiTru.YeuCauKhamBenhs.Add(dichVuKham);
                }

                await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanNgoaiTru);
            }
            #endregion
            return Ok();
        }
        #endregion

        #region BVHD-3809
        [HttpPost("KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync([FromBody] PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel chiDinhViewModel)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVuKyThuatBenhVienChiDinhs, chiDinhViewModel.PhieuDieuTriId);
            return kiemtra;
        }


        #endregion

        #region BVHD-3575
        [HttpPost("KiemTraChiDinhDichVuKhamBenhDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhDichVuKhamBenhDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhDichVuKhamBenhViewModel chiDinhViewModel)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhDichVuKhamBenhDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVuKhamBenhBenhVienId.Value, chiDinhViewModel.PhieuDieuTriId.Value);
            return kiemtra;
        }

        [HttpPost("ThemYeuCauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauKhamBenh([FromBody] ChiDinhDichVuKhamBenhViewModel yeuCauViewModel)
        {
            if (yeuCauViewModel.Gia == null) // || yeuCauViewModel.Gia == 0)
            {
                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
            }

            // biến YeuCauTiepNhanId truyền từ FE là id của YCTN nội trú
            // cập nhật 30/11/2022 không cho thêm dịch vụ khám khi hồ sơ bệnh án đã kết thúc
            var yeuCauTiepNhanNoiTru = _yeuCauTiepNhanService.GetById(yeuCauViewModel.YeuCauTiepNhanId,d=>d.Include(g=>g.NoiTruBenhAn));

            if (yeuCauTiepNhanNoiTru != null && yeuCauTiepNhanNoiTru.NoiTruBenhAn != null && yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }


            var yeuCauTiepNhanChiTiet = new YeuCauTiepNhan();
            if (yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId == null)
            {
                await _dieuTriNoiTruService.XuLyTaoYeuCauNgoaiTruTheoNoiTru(yeuCauTiepNhanNoiTru);
            }
            //yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);
            yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuKhamNoiTruByIdAsync(yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value);

            yeuCauViewModel.YeuCauTiepNhanId = yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value;
            var phieuDieuTri = await _dieuTriNoiTruService.GetNoiTruPhieuDieuTriAsync(yeuCauViewModel.PhieuDieuTriId ?? 0);

            // kiểm tra nếu là dịch vụ thêm từ gói
            if (yeuCauViewModel.DichVuKhamBenhTuGoi != null)
            {
                var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
                {
                    YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                    YeuCauKhamBenhId = null,
                    NgayDieuTri = phieuDieuTri?.NgayDieuTri.Date,
                    NoiTruPhieuDieuTriId = yeuCauViewModel.PhieuDieuTriId
                };


                dichVuChiDinhTheoGoiVo.DichVus.Add(new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                {
                    Id = yeuCauViewModel.DichVuKhamBenhTuGoi.Id,
                    YeuCauGoiDichVuId = yeuCauViewModel.DichVuKhamBenhTuGoi.YeuCauGoiDichVuId ?? 0,
                    TenDichVu = yeuCauViewModel.DichVuKhamBenhTuGoi.TenDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauViewModel.DichVuKhamBenhTuGoi.ChuongTrinhGoiDichVuId ?? 0,
                    ChuongTrinhGoiDichVuChiTietId = yeuCauViewModel.DichVuKhamBenhTuGoi.ChuongTrinhGoiDichVuChiTietId ?? 0,
                    DichVuBenhVienId = yeuCauViewModel.DichVuKhamBenhTuGoi.DichVuBenhVienId ?? 0,
                    NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    SoLuongSuDung = 1,
                    NoiDangKyId = yeuCauViewModel.NoiDangKyId
                });

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);

                if (dichVuChiDinhTheoGoiVo.YeuCauKhamBenhNews.Any())
                {
                    foreach (var dichVuKham in dichVuChiDinhTheoGoiVo.YeuCauKhamBenhNews)
                    {
                        dichVuKham.YeuCauTiepNhanId = yeuCauTiepNhanChiTiet.Id;
                        yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(dichVuKham);
                    }
                }
            }
            // nếu là dịch vụ thêm bình thường
            else
            {

                var dichVuKhamBenhChiDinh = await _dichVuKhamBenhBenhVienService.GetByIdAsync(yeuCauViewModel.DichVuKhamBenhBenhVienId ?? 0,
                    x => x.Include(y => y.DichVuKhamBenhBenhVienGiaBaoHiems)
                        .Include(y => y.DichVuKhamBenh));
                var giaBaoHiem = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(x =>
                    x.TuNgay.Date <= DateTime.Now.Date &&
                    (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                var entity = yeuCauViewModel.ToEntity<YeuCauKhamBenh>();
                entity.YeuCauKhamBenhTruocId = null;
                entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                entity.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                entity.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                entity.TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                entity.BaoHiemChiTra = null;
                entity.KhongTinhPhi = yeuCauViewModel.TinhPhi != null ? !yeuCauViewModel.TinhPhi : null;
                entity.LaChiDinhTuNoiTru = true;
                entity.MaDichVuTT37 = dichVuKhamBenhChiDinh.DichVuKhamBenh?.MaTT37;
                
                DateTime? ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;
                var thoiDiemHienTai = DateTime.Now;
                if (ngayDieuTri != null)
                {
                    //ngayDieuTri = new DateTime(entity.ThoiDiemChiDinh.Year, entity.ThoiDiemChiDinh.Month, entity.ThoiDiemChiDinh.Day, entity.ThoiDiemChiDinh.Hour, entity.ThoiDiemChiDinh.Minute, entity.ThoiDiemChiDinh.Second);
                    var newThoiDiemDangKy = ngayDieuTri.Value.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri.Value.Date;
                    entity.ThoiDiemDangKy = newThoiDiemDangKy;
                }

                if (giaBaoHiem != null)
                {
                    entity.DonGiaBaoHiem = giaBaoHiem.Gia;
                    entity.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;
                }

                yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(entity);
            }

            await _tiepNhanBenhNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            return chiDinhDichVuResultVo;
        }
        #endregion
    }
}
