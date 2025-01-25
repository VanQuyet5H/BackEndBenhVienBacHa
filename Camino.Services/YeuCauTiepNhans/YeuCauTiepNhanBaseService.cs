using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using Camino.Core.Domain.ValueObject.BenefitInsurance;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.BenhNhans;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using NLog;

namespace Camino.Services.YeuCauTiepNhans
{
    public class YeuCauTiepNhanBaseService : MasterFileService<YeuCauTiepNhan>, IYeuCauTiepNhanBaseService
    {
        protected readonly IUserAgentHelper _userAgentHelper;
        protected readonly ICauHinhService _cauHinhService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ITaiKhoanBenhNhanService _taiKhoanBenhNhanService;
        public YeuCauTiepNhanBaseService(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IUserAgentHelper userAgentHelper, ICauHinhService cauHinhService, ILocalizationService localizationService, ITaiKhoanBenhNhanService taiKhoanBenhNhanService) : base(yeuCauTiepNhanRepository)
        {
            _userAgentHelper = userAgentHelper;
            _cauHinhService = cauHinhService;
            _localizationService = localizationService;
            _taiKhoanBenhNhanService = taiKhoanBenhNhanService;
        }

        public async Task PrepareDichVuAndAddAsync(YeuCauTiepNhan ycTiepNhan)
        {
            if(ycTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            {
                //bao lanh thanh toan khong BHYT
                //if (ycTiepNhan.CoBHTN == true)
                //{
                //    foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o=>!o.DuocHuongBaoHiem && o.YeuCauGoiDichVuId == null && o.YeuCauGoiDichVu == null))
                //    {
                //        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauKhamBenh.NoiDangKyId != null)
                //        {
                //            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => !o.DuocHuongBaoHiem && o.YeuCauGoiDichVuId == null && o.YeuCauGoiDichVu == null))
                //    {
                //        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                //        {
                //            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                //        }
                //    }
                //}
                //var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhanId);
                //soDuTk -= GetSoTienCanThanhToanNgoaiTru(ycTiepNhan);
                BaoLanhDvNgoaiGoiMarketing(ycTiepNhan, 0);
                BaoLanhDvTrongGoiMarketing(ycTiepNhan);

                //set lan thuc hien cho dv pttt
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                {
                    yeuCauDichVuKyThuat.LanThucHien = 1;
                }
                //bo duyet tu dong
                //if (ycTiepNhan.CoBHYT == true)
                //{
                //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                //    if (cauHinh.DuyetBHYTTuDong)
                //    {
                //        DuyetBHYT(ycTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, 0);
                //    }
                //}
                var ycGoiMarketingDangThucHiens = BaseRepository.Table.Where(o =>
                    o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing &&
                    o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien &&
                    o.BenhNhanId == ycTiepNhan.BenhNhanId).ToList();
                foreach (var ycGoiMarketingDangThucHien in ycGoiMarketingDangThucHiens)
                {
                    ycGoiMarketingDangThucHien.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy;
                }
            }
            else if(ycTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o => o.CreatedOn == null))
                {
                    if(yeuCauKhamBenh.GoiKhamSucKhoeId != null)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                    else if (yeuCauKhamBenh.KhongTinhPhi == true)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauKhamBenh.DuocHuongBaoHiem)
                        {
                            yeuCauKhamBenh.MucHuongBaoHiem = 0;
                            yeuCauKhamBenh.BaoHiemChiTra = false;
                        }
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => o.CreatedOn == null))
                {
                    if(yeuCauDichVuKyThuat.GoiKhamSucKhoeId != null)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                    else if (yeuCauDichVuKyThuat.KhongTinhPhi == true)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                        {
                            yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                            yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                        }
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                {
                    yeuCauDichVuKyThuat.LanThucHien = 1;
                }
            }

            await AddAsync(ycTiepNhan);
        }

        protected void BaoLanhDvTrongGoiMarketing(YeuCauTiepNhan ycTiepNhan)
        {
            BenhNhan benhNhan = null;
            if (ycTiepNhan.YeuCauKhamBenhs.Any(o => o.YeuCauGoiDichVu != null || o.YeuCauGoiDichVuId != null) || ycTiepNhan.YeuCauDichVuKyThuats.Any(o => o.YeuCauGoiDichVu != null || o.YeuCauGoiDichVuId != null))
            {
                benhNhan = BaseRepository.Context.Set<BenhNhan>().AsNoTracking()
                    .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                    .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(o => o.YeuCauDichVuKyThuats)
                    .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                    .FirstOrDefault(o => o.Id == ycTiepNhan.BenhNhanId);
            }
            if (benhNhan != null)
            {
                List<ThongTinThanhToanGoiDichVuVo> thongTinThanhToanGoiDichVuVos = benhNhan.YeuCauGoiDichVus
                .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && o.DaQuyetToan != true)
                .Select(o => new ThongTinThanhToanGoiDichVuVo
                {
                    YeuCauGoiDichVuId = o.Id,
                    SoTienBenhNhanDaChi = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    DanhSachDichVuDaBaoLanhSuDung = o.YeuCauKhamBenhs
                        .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                    !ycTiepNhan.YeuCauKhamBenhs.Where(h=>h.TrangThai==Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(h=>h.Id).Contains(y.Id) &&
                                    y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                        .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                        .Concat(o.YeuCauDichVuKyThuats
                            .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                        !ycTiepNhan.YeuCauDichVuKyThuats.Where(h => h.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(h => h.Id).Contains(y.Id) &&
                                        y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                        .Concat(o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                            .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                        .ToList()
                }).ToList();
                thongTinThanhToanGoiDichVuVos = thongTinThanhToanGoiDichVuVos.Concat(benhNhan.YeuCauGoiDichVuSoSinhs
                        .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && o.DaQuyetToan != true)
                        .Select(o => new ThongTinThanhToanGoiDichVuVo
                        {
                            YeuCauGoiDichVuId = o.Id,
                            SoTienBenhNhanDaChi = o.SoTienBenhNhanDaChi.GetValueOrDefault(),
                            DanhSachDichVuDaBaoLanhSuDung = o.YeuCauKhamBenhs
                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                            !ycTiepNhan.YeuCauKhamBenhs.Where(h => h.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(h => h.Id).Contains(y.Id) &&
                                            y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                                .Concat(o.YeuCauDichVuKyThuats
                                    .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                !ycTiepNhan.YeuCauDichVuKyThuats.Where(h => h.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(h => h.Id).Contains(y.Id) &&
                                                y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                    .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                                .Concat(o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                                    .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                    .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                                .ToList()
                        }).ToList())
                    .ToList();

                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o => (o.YeuCauGoiDichVuId != null  || o.YeuCauGoiDichVu != null) && o.KhongTinhPhi != true))
                {
                    if (yeuCauKhamBenh.CreatedOn == null || yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    {
                        var chuongTrinhGoiDichVuId = yeuCauKhamBenh.YeuCauGoiDichVu?.ChuongTrinhGoiDichVuId ?? benhNhan.YeuCauGoiDichVus.FirstOrDefault(o => o.Id == yeuCauKhamBenh.YeuCauGoiDichVuId)?.ChuongTrinhGoiDichVuId;

                        if (ycTiepNhan.LaCapCuu == true || ycTiepNhan.QuyetToanTheoNoiTru == true || ycTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(o=>GetChuongTrinhGoiDichVuBHTNBaoLanhs().Any(ct=>ct.CongTyBaoHiemTuNhanId == o.CongTyBaoHiemTuNhanId && ct.Id == chuongTrinhGoiDichVuId)))
                        {
                            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauKhamBenh.NoiDangKyId != null)
                            {
                                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                            }
                        }
                        else
                        {
                            var thongTinThanhToanGoiDichVuVo = thongTinThanhToanGoiDichVuVos.FirstOrDefault(o => o.YeuCauGoiDichVuId == (yeuCauKhamBenh.YeuCauGoiDichVuId ?? yeuCauKhamBenh.YeuCauGoiDichVu?.Id));
                            if (thongTinThanhToanGoiDichVuVo != null && thongTinThanhToanGoiDichVuVo.SoTienConBaoLanh >= yeuCauKhamBenh.DonGiaSauChietKhau.GetValueOrDefault())
                            {
                                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauKhamBenh.NoiDangKyId != null)
                                {
                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                                }
                                thongTinThanhToanGoiDichVuVo.DanhSachDichVuDaBaoLanhSuDung.Add((yeuCauKhamBenh.DonGiaSauChietKhau.GetValueOrDefault(), 1));
                            }
                        }                        
                    }                    
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => (o.YeuCauGoiDichVuId != null || o.YeuCauGoiDichVu != null) && o.KhongTinhPhi != true))
                {
                    if (yeuCauDichVuKyThuat.CreatedOn == null || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                    {
                        var chuongTrinhGoiDichVuId = yeuCauDichVuKyThuat.YeuCauGoiDichVu?.ChuongTrinhGoiDichVuId ?? benhNhan.YeuCauGoiDichVus.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId)?.ChuongTrinhGoiDichVuId;

                        if (ycTiepNhan.LaCapCuu == true || ycTiepNhan.QuyetToanTheoNoiTru == true || ycTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(o => GetChuongTrinhGoiDichVuBHTNBaoLanhs().Any(ct => ct.CongTyBaoHiemTuNhanId == o.CongTyBaoHiemTuNhanId && ct.Id == chuongTrinhGoiDichVuId)))
                        {
                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                            {
                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                            }
                        }
                        else
                        {
                            var thongTinThanhToanGoiDichVuVo = thongTinThanhToanGoiDichVuVos.FirstOrDefault(o => o.YeuCauGoiDichVuId == (yeuCauDichVuKyThuat.YeuCauGoiDichVuId ?? yeuCauDichVuKyThuat.YeuCauGoiDichVu?.Id));
                            if (thongTinThanhToanGoiDichVuVo != null && thongTinThanhToanGoiDichVuVo.SoTienConBaoLanh >= yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault())
                            {
                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                {
                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                }
                                thongTinThanhToanGoiDichVuVo.DanhSachDichVuDaBaoLanhSuDung.Add((yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault(), yeuCauDichVuKyThuat.SoLan));
                            }
                        }
                    }
                    
                }
            }
        }
        protected void BaoLanhDvNgoaiGoiMarketing(YeuCauTiepNhan ycTiepNhan, decimal soTienConLai)
        {
            foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs)
            {
                if ((yeuCauKhamBenh.CreatedOn == null || yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan) && yeuCauKhamBenh.YeuCauGoiDichVuId == null && yeuCauKhamBenh.YeuCauGoiDichVu == null)
                {
                    if (yeuCauKhamBenh.KhongTinhPhi == true)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauKhamBenh.DuocHuongBaoHiem)
                        {
                            yeuCauKhamBenh.MucHuongBaoHiem = 0;
                            yeuCauKhamBenh.BaoHiemChiTra = false;
                        }
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                    else if (ycTiepNhan.CoBHTN == true || ycTiepNhan.LaCapCuu == true || ycTiepNhan.QuyetToanTheoNoiTru == true)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                    else
                    {
                        decimal soTienCanBaoLanh = yeuCauKhamBenh.Gia 
                            - (yeuCauKhamBenh.BaoHiemChiTra != true ? 0 : (yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100))
                            - yeuCauKhamBenh.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - yeuCauKhamBenh.SoTienMienGiam.GetValueOrDefault() - yeuCauKhamBenh.SoTienBenhNhanDaChi.GetValueOrDefault();
                        if (yeuCauKhamBenh.CreatedOn == null) 
                        {
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soTienCanBaoLanh < 0 || soTienConLai >= soTienCanBaoLanh)
                            {
                                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauKhamBenh.NoiDangKyId != null)
                                {
                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                                }
                                soTienConLai -= soTienCanBaoLanh;
                            }
                        }
                        else
                        {
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soTienCanBaoLanh < 0 || soTienConLai >= 0)
                            {
                                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauKhamBenh.NoiDangKyId != null)
                                {
                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                                }
                            }
                        }
                    }                    
                }
            }
            foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats)
            {
                if ((yeuCauDichVuKyThuat.CreatedOn == null || yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan) && yeuCauDichVuKyThuat.YeuCauGoiDichVuId == null && yeuCauDichVuKyThuat.YeuCauGoiDichVu == null)
                {
                    if (yeuCauDichVuKyThuat.KhongTinhPhi == true)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                        {
                            yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                            yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                        }
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                    else if (ycTiepNhan.CoBHTN == true || ycTiepNhan.LaCapCuu == true || ycTiepNhan.QuyetToanTheoNoiTru == true)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                    else
                    {
                        decimal soTienCanBaoLanh = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.BaoHiemChiTra != true ? 0 : (yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100))) * yeuCauDichVuKyThuat.SoLan
                            - yeuCauDichVuKyThuat.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - yeuCauDichVuKyThuat.SoTienMienGiam.GetValueOrDefault() - yeuCauDichVuKyThuat.SoTienBenhNhanDaChi.GetValueOrDefault();
                        if(yeuCauDichVuKyThuat.CreatedOn == null)
                        {
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soTienCanBaoLanh < 0 || soTienConLai >= soTienCanBaoLanh)
                            {
                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                {
                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                }
                                soTienConLai -= soTienCanBaoLanh;
                            }
                        }
                        else
                        {
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soTienCanBaoLanh < 0 || soTienConLai >= 0)
                            {
                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                {
                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                }
                            }
                        }
                    }
                }
            }
        }
        public async Task PrepareForAddDichVuAndUpdateAsync(YeuCauTiepNhan ycTiepNhan)
        {
            if (ycTiepNhan.CoBHYT == true && (ycTiepNhan.BenhNhanId != null || (ycTiepNhan.BenhNhan != null && ycTiepNhan.BenhNhan.Id != 0)))
            {
                var benhNhanId = ycTiepNhan.BenhNhanId != null ? ycTiepNhan.BenhNhanId : ycTiepNhan.BenhNhan.Id;
                var yeuCauTiepNhanCoBHYYTMoiNhatCuaBenhNhan = BaseRepository.TableNoTracking
                    .Where(x => x.BenhNhanId == benhNhanId 
                                && x.CoBHYT == true
                                && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault();
                if (yeuCauTiepNhanCoBHYYTMoiNhatCuaBenhNhan != null)
                {
                    if (!yeuCauTiepNhanCoBHYYTMoiNhatCuaBenhNhan.MaYeuCauTiepNhan.Equals(ycTiepNhan.MaYeuCauTiepNhan))
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.BenhNhanDaTaoYCTNMoiCoBHYT"));
                    }
                }
            }

            foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauKhamBenhs.SelectMany(o => o.YeuCauDichVuKyThuats.Where(k => k.CreatedOn == null)))
            {
                if (!ycTiepNhan.YeuCauDichVuKyThuats.Contains(yeuCauDichVuKyThuat))
                {
                    ycTiepNhan.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuat);
                }
            }
            foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauKhamBenhs.SelectMany(o => o.YeuCauDuocPhamBenhViens.Where(k => k.CreatedOn == null)))
            {
                if (!ycTiepNhan.YeuCauDuocPhamBenhViens.Contains(yeuCauDuocPhamBenhVien))
                {
                    ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                }
            }
            foreach (var yeuCauVatTuBenhVien in ycTiepNhan.YeuCauKhamBenhs.SelectMany(o => o.YeuCauVatTuBenhViens.Where(k => k.CreatedOn == null)))
            {
                if (!ycTiepNhan.YeuCauVatTuBenhViens.Contains(yeuCauVatTuBenhVien))
                {
                    ycTiepNhan.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                }
            }
            foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDichVuKyThuats.SelectMany(o => o.YeuCauDuocPhamBenhViens.Where(k => k.CreatedOn == null)))
            {
                if (!ycTiepNhan.YeuCauDuocPhamBenhViens.Contains(yeuCauDuocPhamBenhVien))
                {
                    ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                }
            }
            foreach (var yeuCauVatTuBenhVien in ycTiepNhan.YeuCauDichVuKyThuats.SelectMany(o => o.YeuCauVatTuBenhViens.Where(k => k.CreatedOn == null)))
            {
                if (!ycTiepNhan.YeuCauVatTuBenhViens.Contains(yeuCauVatTuBenhVien))
                {
                    ycTiepNhan.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                }
            }
            
            if (ycTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            {
                //var soDuTk = await GetSoTienDaTamUngAsync(ycTiepNhan.Id);
                //soDuTk -= GetSoTienCanThanhToanNgoaiTru(ycTiepNhan);

                //foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o =>
                //    o.CreatedOn == null && o.KhongTinhPhi == true))
                //{
                //    yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                //    if (yeuCauKhamBenh.DuocHuongBaoHiem)
                //    {
                //        yeuCauKhamBenh.MucHuongBaoHiem = 0;
                //        yeuCauKhamBenh.BaoHiemChiTra = false;
                //    }
                //    if (yeuCauKhamBenh.NoiDangKyId != null)
                //    {
                //        yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //            yeuCauKhamBenh.NoiDangKyId.Value));
                //    }
                //}
                //foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                //    o.CreatedOn == null && o.KhongTinhPhi == true))
                //{
                //    yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                //    if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                //    {
                //        yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                //        yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                //    }
                //    if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                //    {
                //        yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //            yeuCauDichVuKyThuat.NoiThucHienId.Value));
                //    }
                //}

                ////bao lanh thanh toan khong BHYT
                //if (ycTiepNhan.CoBHTN == true)
                //{
                //    foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o =>
                //        o.CreatedOn == null && !o.DuocHuongBaoHiem && o.KhongTinhPhi != true &&
                //        o.YeuCauGoiDichVuId == null))
                //    {
                //        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauKhamBenh.NoiDangKyId != null)
                //        {
                //            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //                yeuCauKhamBenh.NoiDangKyId.Value));
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                //        o.CreatedOn == null && !o.DuocHuongBaoHiem && o.KhongTinhPhi != true && o.YeuCauGoiDichVuId == null))
                //    {
                //        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                //        {
                //            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //                yeuCauDichVuKyThuat.NoiThucHienId.Value));
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o =>
                //        o.CreatedOn == null && !o.DuocHuongBaoHiem && o.KhongTinhPhi != true &&
                //        o.YeuCauGoiDichVuId == null))
                //    {
                //        decimal soTienCanBaoLanh = yeuCauKhamBenh.Gia;
                //        if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                //        {
                //            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //            if (yeuCauKhamBenh.NoiDangKyId != null)
                //            {
                //                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //                    yeuCauKhamBenh.NoiDangKyId.Value));
                //            }
                //            soDuTk -= soTienCanBaoLanh;
                //        }
                //    }
                //    foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                //        o.CreatedOn == null && !o.DuocHuongBaoHiem && o.KhongTinhPhi != true && 
                //        o.YeuCauGoiDichVuId == null))
                //    {
                //        decimal soTienCanBaoLanh = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                //        if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                //        {
                //            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                //            {
                //                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                //                    yeuCauDichVuKyThuat.NoiThucHienId.Value));
                //            }
                //            soDuTk -= soTienCanBaoLanh;
                //        }
                //    }
                //}
                var soDuTk = await GetSoTienDaTamUngAsync(ycTiepNhan.Id);
                soDuTk -= GetSoTienCanThanhToanNgoaiTru(ycTiepNhan);
                BaoLanhDvNgoaiGoiMarketing(ycTiepNhan, soDuTk);
                //bao lanh DV trong goi
                LoadYeuCauTiepNhanCongTyBaoHiemTuNhan(ycTiepNhan);
                BaoLanhDvTrongGoiMarketing(ycTiepNhan);

                //set lan thuc hien cho dv pttt
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                    o.CreatedOn == null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                {
                    foreach (var dv in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                        o.CreatedOn != null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                    {
                        var tuongTrinhPTTT = BaseRepository.Context.Entry(dv)
                            .Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                        if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                    }
                    var max = ycTiepNhan.YeuCauDichVuKyThuats.Max(o => o.LanThucHien);
                    var maxDangThucHien = ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                        (o.CreatedOn != null || o.LanThucHien != null) &&
                        o.NoiThucHienId == yeuCauDichVuKyThuat.NoiThucHienId &&
                        o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                        (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                         o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                        (o.YeuCauDichVuKyThuatTuongTrinhPTTT == null ||
                         o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true) &&
                        o.TheoDoiSauPhauThuatThuThuatId == null).Max(o => o.LanThucHien);
                    yeuCauDichVuKyThuat.LanThucHien = maxDangThucHien ?? (max != null ? (max + 1) : 0);
                }

                foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDuocPhamBenhViens.Where(
                    o => o.CreatedOn == null))
                {
                    if (yeuCauDuocPhamBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDuocPhamBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                            yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                        }
                    }
                }
                foreach (var yeuCauVatTuBenhVien in ycTiepNhan.YeuCauVatTuBenhViens.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauVatTuBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauVatTuBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauVatTuBenhVien.MucHuongBaoHiem = 0;
                            yeuCauVatTuBenhVien.BaoHiemChiTra = false;
                        }
                    }
                }
                //bo duyet tu dong
                //if (ycTiepNhan.CoBHYT == true)
                //{
                //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                //    if (cauHinh.DuyetBHYTTuDong)
                //    {
                //        DuyetBHYT(ycTiepNhan, (long) Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong,
                //            (long) Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                //    }
                //}
            }
            else if(ycTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
            {
                if (ycTiepNhan.NoiTruBenhAn != null)
                {
                    foreach (var yeuCauDichVuKyThuat in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.YeuCauDichVuKyThuats.Where(k => k.CreatedOn == null)))
                    {
                        if (!ycTiepNhan.YeuCauDichVuKyThuats.Contains(yeuCauDichVuKyThuat))
                        {
                            ycTiepNhan.YeuCauDichVuKyThuats.Add(yeuCauDichVuKyThuat);
                        }
                    }
                    foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.YeuCauDuocPhamBenhViens.Where(k => k.CreatedOn == null)))
                    {
                        if (!ycTiepNhan.YeuCauDuocPhamBenhViens.Contains(yeuCauDuocPhamBenhVien))
                        {
                            ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                    }
                    foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.NoiTruChiDinhDuocPhams.SelectMany(c => c.YeuCauDuocPhamBenhViens.Where(k => k.CreatedOn == null))))
                    {
                        if (!ycTiepNhan.YeuCauDuocPhamBenhViens.Contains(yeuCauDuocPhamBenhVien))
                        {
                            ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                    }
                    foreach (var yeuCauVatTuBenhVien in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.YeuCauVatTuBenhViens.Where(k => k.CreatedOn == null)))
                    {
                        if (!ycTiepNhan.YeuCauVatTuBenhViens.Contains(yeuCauVatTuBenhVien))
                        {
                            ycTiepNhan.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                        }
                    }
                    foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.YeuCauDichVuKyThuats.SelectMany(kt=>kt.YeuCauDuocPhamBenhViens).Where(k => k.CreatedOn == null)))
                    {
                        if (!ycTiepNhan.YeuCauDuocPhamBenhViens.Contains(yeuCauDuocPhamBenhVien))
                        {
                            ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
                        }
                    }
                    foreach (var yeuCauVatTuBenhVien in ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.SelectMany(o => o.YeuCauDichVuKyThuats.SelectMany(kt => kt.YeuCauVatTuBenhViens).Where(k => k.CreatedOn == null)))
                    {
                        if (!ycTiepNhan.YeuCauVatTuBenhViens.Contains(yeuCauVatTuBenhVien))
                        {
                            ycTiepNhan.YeuCauVatTuBenhViens.Add(yeuCauVatTuBenhVien);
                        }
                    }
                }
                

                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauDichVuKyThuat.KhongTinhPhi == true)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                        {
                            yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                            yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                        }
                    }
                    else
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                    }
                    if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                    {
                        yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                            yeuCauDichVuKyThuat.NoiThucHienId.Value));
                    }
                    //set lan thuc hien cho dv pttt
                    if (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                    {
                        foreach (var dv in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                            o.CreatedOn != null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                        {
                            var tuongTrinhPTTT = BaseRepository.Context.Entry(dv)
                                .Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                            if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                        }
                        var max = ycTiepNhan.YeuCauDichVuKyThuats.Max(o => o.LanThucHien);
                        var maxDangThucHien = ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                            (o.CreatedOn != null || o.LanThucHien != null) &&
                            o.NoiThucHienId == yeuCauDichVuKyThuat.NoiThucHienId &&
                            o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                             o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            (o.YeuCauDichVuKyThuatTuongTrinhPTTT == null ||
                             o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true) &&
                            o.TheoDoiSauPhauThuatThuThuatId == null).Max(o => o.LanThucHien);
                        yeuCauDichVuKyThuat.LanThucHien = maxDangThucHien ?? (max != null ? (max + 1) : 0);
                    }

                    //them vao phien xet nghiem
                    //if (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                    //{
                    //    var phienXetNghiems = BaseRepository.Context.Entry(ycTiepNhan).Collection(o => o.PhienXetNghiems);
                    //    if (!phienXetNghiems.IsLoaded || ycTiepNhan.PhienXetNghiems.Any(o =>
                    //            BaseRepository.Context.Entry(o).Collection(x => x.PhienXetNghiemChiTiets).IsLoaded ==
                    //            false))
                    //    {
                    //        phienXetNghiems.Query().Include(b => b.BenhNhan).Include(b => b.MauXetNghiems).ThenInclude(b => b.PhieuGoiMauXetNghiem)
                    //            .Include(b => b.PhienXetNghiemChiTiets).Load();
                    //    }
                    //    var phienXetNghiemHienTais = ycTiepNhan.PhienXetNghiems.Where(o => o.ThoiDiemKetLuan == null)
                    //        .OrderBy(o => o.ThoiDiemBatDau).ToList();
                    //    if (phienXetNghiemHienTais.Any())
                    //    {
                    //        var dichVuKyThuatBenhVien = BaseRepository.Context.Entry(yeuCauDichVuKyThuat)
                    //            .Reference(o => o.DichVuKyThuatBenhVien);
                    //        if (!dichVuKyThuatBenhVien.IsLoaded) dichVuKyThuatBenhVien.Load();
                    //        var mauXetNghiem = phienXetNghiemHienTais.SelectMany(o=>o.MauXetNghiems).FirstOrDefault(o =>
                    //            o.NhomDichVuBenhVienId == yeuCauDichVuKyThuat.NhomDichVuBenhVienId &&
                    //            o.LoaiMauXetNghiem == yeuCauDichVuKyThuat.DichVuKyThuatBenhVien.LoaiMauXetNghiem);

                    //        if (mauXetNghiem != null)
                    //        {
                    //            var phienXetNghiemChiTiet = new PhienXetNghiemChiTiet()
                    //            {
                    //                PhienXetNghiem = mauXetNghiem.PhienXetNghiem,
                    //                NhomDichVuBenhVienId = yeuCauDichVuKyThuat.NhomDichVuBenhVienId,
                    //                YeuCauDichVuKyThuatId = yeuCauDichVuKyThuat.Id,
                    //                DichVuKyThuatBenhVienId = yeuCauDichVuKyThuat.DichVuKyThuatBenhVienId,
                    //                LanThucHien =
                    //                    mauXetNghiem.PhienXetNghiem.PhienXetNghiemChiTiets
                    //                        .Where(o => o.NhomDichVuBenhVienId == yeuCauDichVuKyThuat.NhomDichVuBenhVienId)
                    //                        .OrderBy(o => o.LanThucHien).LastOrDefault()?.LanThucHien ?? 1
                    //            };
                    //            if (mauXetNghiem.PhieuGoiMauXetNghiem?.DaNhanMau == true)
                    //            {
                    //                AddKetQuaXetNghiemChiTiet(phienXetNghiemChiTiet,
                    //                    GetDichVuXetNghiems().First(o =>
                    //                        o.Id == yeuCauDichVuKyThuat.DichVuKyThuatBenhVien.DichVuXetNghiemId),
                    //                    mauXetNghiem.PhienXetNghiem.BenhNhan, GetDichVuXetNghiems(), GetDichVuXetNghiemKetNoiChiSos());
                    //            }
                    //            mauXetNghiem.PhienXetNghiem.PhienXetNghiemChiTiets.Add(phienXetNghiemChiTiet);
                    //        }
                    //    }
                    //}
                }
                
                foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDuocPhamBenhViens.Where(
                    o => o.CreatedOn == null))
                {
                    if (yeuCauDuocPhamBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDuocPhamBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                            yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                        }
                    }
                    else
                    {
                        yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                    }
                }
                foreach (var yeuCauVatTuBenhVien in ycTiepNhan.YeuCauVatTuBenhViens.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauVatTuBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauVatTuBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauVatTuBenhVien.MucHuongBaoHiem = 0;
                            yeuCauVatTuBenhVien.BaoHiemChiTra = false;
                        }
                    }
                    else
                    {
                        yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                    }
                }
            }

            // cập nhật cho phần khám sức khỏe đoàn
            else if (ycTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
            {
                var soDuTk = await GetSoTienDaTamUngAsync(ycTiepNhan.Id);
                soDuTk -= GetSoTienCanThanhToanNgoaiTru(ycTiepNhan);
                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs.Where(o =>o.CreatedOn == null))
                {
                    if (yeuCauKhamBenh.GoiKhamSucKhoeId != null)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                    else
                    {
                        if (yeuCauKhamBenh.KhongTinhPhi == true)
                        {
                            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                            if (yeuCauKhamBenh.NoiDangKyId != null)
                            {
                                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                    yeuCauKhamBenh.NoiDangKyId.Value));
                            }
                        }
                        else
                        {
                            decimal soTienCanBaoLanh = yeuCauKhamBenh.Gia;
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                            {
                                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauKhamBenh.NoiDangKyId != null)
                                {
                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                        yeuCauKhamBenh.NoiDangKyId.Value));
                                }
                                soDuTk -= soTienCanBaoLanh;
                            }
                        }
                    }
                    
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauDichVuKyThuat.GoiKhamSucKhoeId != null)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                    else
                    {
                        if (yeuCauDichVuKyThuat.KhongTinhPhi == true)
                        {
                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                            {
                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                    yeuCauDichVuKyThuat.NoiThucHienId.Value));
                            }
                        }
                        else
                        {
                            decimal soTienCanBaoLanh = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                            if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                            {
                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                {
                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan,
                                        yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                }
                                soDuTk -= soTienCanBaoLanh;
                            }
                        }
                    }

                    //set lan thuc hien cho dv pttt
                    if (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                    {
                        foreach (var dv in ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                            o.CreatedOn != null && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                        {
                            var tuongTrinhPTTT = BaseRepository.Context.Entry(dv)
                                .Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                            if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                        }
                        var max = ycTiepNhan.YeuCauDichVuKyThuats.Max(o => o.LanThucHien);
                        var maxDangThucHien = ycTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                            (o.CreatedOn != null || o.LanThucHien != null) &&
                            o.NoiThucHienId == yeuCauDichVuKyThuat.NoiThucHienId &&
                            o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien ||
                             o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                            (o.YeuCauDichVuKyThuatTuongTrinhPTTT == null ||
                             o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true) &&
                            o.TheoDoiSauPhauThuatThuThuatId == null).Max(o => o.LanThucHien);
                        yeuCauDichVuKyThuat.LanThucHien = maxDangThucHien ?? (max != null ? (max + 1) : 0);
                    }
                }

                foreach (var yeuCauDuocPhamBenhVien in ycTiepNhan.YeuCauDuocPhamBenhViens.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauDuocPhamBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauDuocPhamBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                            yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                        }
                    }
                }
                foreach (var yeuCauVatTuBenhVien in ycTiepNhan.YeuCauVatTuBenhViens.Where(o => o.CreatedOn == null))
                {
                    if (yeuCauVatTuBenhVien.KhongTinhPhi == true)
                    {
                        yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                        if (yeuCauVatTuBenhVien.DuocHuongBaoHiem)
                        {
                            yeuCauVatTuBenhVien.MucHuongBaoHiem = 0;
                            yeuCauVatTuBenhVien.BaoHiemChiTra = false;
                        }
                    }
                }

            }
            //await UpdateAsync(ycTiepNhan);
            BaseRepository.Context.SaveChanges();
        }

        public async Task PrepareForDeleteDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan, bool isSaveChange = true)
        {
            //bool duyetLaiBHYT = false;
            bool huyDichVuTrongGoi = false;
            foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => o.WillDelete))
            {
                //update 08/11/2022 bỏ xóa trực tiếp 
                //var willDelete = true;
                var willDelete = false;
                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yeuCauKhamBenh.KhongTinhPhi != true)
                {
                    var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.TaiKhoanBenhNhanChis);
                    if (!taiKhoanBenhNhanChis.IsLoaded)
                        taiKhoanBenhNhanChis.Load();

                    if (yeuCauKhamBenh.TaiKhoanBenhNhanChis.Any(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu))
                    {
                        willDelete = false;
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.XoaDichVuDaThuTien"));
                    }
                }
                if (yeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                }
                YeuCauGoiDichVu yeuCauGoiDichVu = null;
                if (yeuCauKhamBenh.YeuCauGoiDichVuId != null)
                {
                    huyDichVuTrongGoi = true;
                    LoadGoiDichVuBenhNhan(yeuCauTiepNhan);
                    yeuCauGoiDichVu =
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.FirstOrDefault(o =>o.Id == yeuCauKhamBenh.YeuCauGoiDichVuId) ??
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.FirstOrDefault(o =>o.Id == yeuCauKhamBenh.YeuCauGoiDichVuId);
                    
                    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauKhamBenhs);
                    if (!yeuCauKhamBenhs.IsLoaded)
                        yeuCauKhamBenhs.Load();

                    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauDichVuKyThuats);
                    if (!yeuCauDichVuKyThuats.IsLoaded)
                        yeuCauDichVuKyThuats.Load();
                }
                
                if (yeuCauKhamBenh.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                    {
                        willDelete = false;
                    }
                    XoaKhoiHangDoi(yeuCauKhamBenh);
                }
                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                if (yeuCauKhamBenh.NhanVienDuyetBaoHiemId != null || !yeuCauKhamBenh.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauKhamBenh.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    willDelete = false;
                }
                yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;
                if (yeuCauGoiDichVu != null && yeuCauGoiDichVu.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                    yeuCauGoiDichVu.BoPhanMarketingDangKy == false
                    && yeuCauGoiDichVu.YeuCauKhamBenhs.All(o =>
                        o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) &&
                    yeuCauGoiDichVu.YeuCauDichVuKyThuats.All(o =>
                        o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    yeuCauGoiDichVu.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy;
                    yeuCauGoiDichVu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                }
                if (!willDelete)
                {
                    yeuCauKhamBenh.WillDelete = false;
                }
            }
            foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats)
            {
                if(!yeuCauDichVuKyThuat.WillDelete)
                {
                    continue;
                }
                //update 08/11/2022 bỏ xóa trực tiếp 
                //var willDelete = true;
                var willDelete = false;
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yeuCauDichVuKyThuat.KhongTinhPhi != true)
                {
                    var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Collection(o => o.TaiKhoanBenhNhanChis);
                    if (!taiKhoanBenhNhanChis.IsLoaded)
                        taiKhoanBenhNhanChis.Load();

                    if (yeuCauDichVuKyThuat.TaiKhoanBenhNhanChis.Any(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu))
                    {
                        willDelete = false;
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.XoaDichVuDaThuTien"));
                    }
                }
                if (yeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                }
                YeuCauGoiDichVu yeuCauGoiDichVu = null;
                if (yeuCauDichVuKyThuat.YeuCauGoiDichVuId != null)
                {
                    huyDichVuTrongGoi = true;
                    LoadGoiDichVuBenhNhan(yeuCauTiepNhan);
                    yeuCauGoiDichVu =
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId) ??
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId);

                    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauKhamBenhs);
                    if (!yeuCauKhamBenhs.IsLoaded)
                        yeuCauKhamBenhs.Load();

                    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauDichVuKyThuats);
                    if (!yeuCauDichVuKyThuats.IsLoaded)
                        yeuCauDichVuKyThuats.Load();
                }

                if (yeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                {
                    var phienXetNghiemChiTiets = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Collection(o => o.PhienXetNghiemChiTiets);
                    if (!phienXetNghiemChiTiets.IsLoaded)
                        phienXetNghiemChiTiets.Load();
                }

                
                
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                {
                    if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                    {
                        willDelete = false;
                    }
                    XoaKhoiHangDoi(yeuCauDichVuKyThuat);
                }
                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                var tuongTrinhPTTT = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                if (yeuCauDichVuKyThuat.NhanVienDuyetBaoHiemId != null || !yeuCauDichVuKyThuat.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauDichVuKyThuat.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0) || yeuCauDichVuKyThuat.PhienXetNghiemChiTiets.Any() || yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
                {
                    willDelete = false;
                }

                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                if (yeuCauGoiDichVu != null && yeuCauGoiDichVu.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                    yeuCauGoiDichVu.BoPhanMarketingDangKy == false
                    && yeuCauGoiDichVu.YeuCauKhamBenhs.All(o =>
                        o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) &&
                    yeuCauGoiDichVu.YeuCauDichVuKyThuats.All(o =>
                        o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    yeuCauGoiDichVu.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy;
                    yeuCauGoiDichVu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                }
                if (!willDelete)
                {
                    yeuCauDichVuKyThuat.WillDelete = false;
                }
            }

            foreach (var yeuCauDuocPhamBenhVien in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(o => o.WillDelete))
            {
                //update 08/11/2022 bỏ xóa trực tiếp 
                //var willDelete = true;
                var willDelete = false;
                if (yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yeuCauDuocPhamBenhVien.KhongTinhPhi != true)
                {
                    var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauDuocPhamBenhVien).Collection(o => o.TaiKhoanBenhNhanChis);
                    if (!taiKhoanBenhNhanChis.IsLoaded)
                        taiKhoanBenhNhanChis.Load();

                    if (yeuCauDuocPhamBenhVien.TaiKhoanBenhNhanChis.Any(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu))
                    {
                        willDelete = false;
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.XoaDichVuDaThuTien"));
                    }
                }
                if (yeuCauDuocPhamBenhVien.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.ChuaThucHien || yeuCauDuocPhamBenhVien.YeuCauLinhDuocPhamId != null)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                }
                
                if (yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                {
                    willDelete = false;
                }
                
                yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                

                if (yeuCauDuocPhamBenhVien.NhanVienDuyetBaoHiemId != null || !yeuCauDuocPhamBenhVien.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauDuocPhamBenhVien.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    willDelete = false;
                }
                yeuCauDuocPhamBenhVien.TrangThai = Enums.EnumYeuCauDuocPhamBenhVien.DaHuy;
                if (!willDelete)
                {
                    yeuCauDuocPhamBenhVien.WillDelete = false;
                }
            }

            foreach (var yeuCauVatTuBenhVien in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(o => o.WillDelete))
            {
                //update 08/11/2022 bỏ xóa trực tiếp 
                //var willDelete = true;
                var willDelete = false;
                if (yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && yeuCauVatTuBenhVien.KhongTinhPhi != true)
                {
                    var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauVatTuBenhVien).Collection(o => o.TaiKhoanBenhNhanChis);
                    if (!taiKhoanBenhNhanChis.IsLoaded)
                        taiKhoanBenhNhanChis.Load();

                    if (yeuCauVatTuBenhVien.TaiKhoanBenhNhanChis.Any(o => o.LoaiChiTienBenhNhan == Enums.LoaiChiTienBenhNhan.HoanThu))
                    {
                        willDelete = false;
                    }
                    else
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.XoaDichVuDaThuTien"));
                    }
                }
                if (yeuCauVatTuBenhVien.TrangThai != Enums.EnumYeuCauVatTuBenhVien.ChuaThucHien || yeuCauVatTuBenhVien.YeuCauLinhVatTuId != null)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                }
                
                if (yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                {
                    willDelete = false;
                }
                yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                

                if (yeuCauVatTuBenhVien.NhanVienDuyetBaoHiemId != null || !yeuCauVatTuBenhVien.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauVatTuBenhVien.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    willDelete = false;
                }
                //if (yeuCauVatTuBenhVien.DuocHuongBaoHiem && yeuCauVatTuBenhVien.BaoHiemChiTra != false)
                //{
                //    duyetLaiBHYT = true;
                //}
                yeuCauVatTuBenhVien.TrangThai = Enums.EnumYeuCauVatTuBenhVien.DaHuy;
                if (!willDelete)
                {
                    yeuCauVatTuBenhVien.WillDelete = false;
                }
            }

            foreach (var donThuocThanhToanChiTiet in yeuCauTiepNhan.DonThuocThanhToans.SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.WillDelete))
            {
                if (donThuocThanhToanChiTiet.DonThuocThanhToan != null && donThuocThanhToanChiTiet.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.XoaDichVuDaThuTien"));
                }
                if (donThuocThanhToanChiTiet.DonThuocThanhToan!=null && donThuocThanhToanChiTiet.DonThuocThanhToan.TrangThai != Enums.TrangThaiDonThuocThanhToan.ChuaXuatThuoc)
                {
                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                }
                if (donThuocThanhToanChiTiet.NhanVienDuyetBaoHiemId != null)
                {
                    var duyetBaoHiemChiTiets = BaseRepository.Context.Entry(donThuocThanhToanChiTiet).Collection(o => o.DuyetBaoHiemChiTiets);
                    if (!duyetBaoHiemChiTiets.IsLoaded) duyetBaoHiemChiTiets.Load();
                    foreach (var duyetBaoHiemChiTiet in donThuocThanhToanChiTiet.DuyetBaoHiemChiTiets)
                    {
                        duyetBaoHiemChiTiet.WillDelete = true;
                    }
                }
                //if (donThuocThanhToanChiTiet.DuocHuongBaoHiem && donThuocThanhToanChiTiet.BaoHiemChiTra != false)
                //{
                //    duyetLaiBHYT = true;
                //}
            }

            //bao lanh thanh toan khong BHYT
            if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
            {
                var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk);
                //bao lanh thanh toan dich vu trong goi
                if (huyDichVuTrongGoi)
                {
                    BaoLanhDvTrongGoiMarketing(yeuCauTiepNhan);
                }
                //foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs.Where(o =>
                //    o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && !o.DuocHuongBaoHiem))
                //{
                //    decimal soTienCanBaoLanh = yeuCauKhamBenh.Gia;
                //    if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= 0)
                //    {
                //        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauKhamBenh.NoiDangKyId != null)
                //        {
                //            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan,
                //                yeuCauKhamBenh.NoiDangKyId.Value));
                //        }
                //        soDuTk -= soTienCanBaoLanh;
                //    }
                //}
                //foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o =>
                //    o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && !o.DuocHuongBaoHiem))
                //{
                //    decimal soTienCanBaoLanh = yeuCauDichVuKyThuat.Gia * yeuCauDichVuKyThuat.SoLan;
                //    if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= 0)
                //    {
                //        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                //        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                //        {
                //            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan,
                //                yeuCauDichVuKyThuat.NoiThucHienId.Value));
                //        }
                //        soDuTk -= soTienCanBaoLanh;
                //    }
                //}
                ////bo duyet tu dong
                //if (duyetLaiBHYT)
                //{
                //    var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                //    if (cauHinh.DuyetBHYTTuDong)
                //    {
                //        DuyetBHYT(yeuCauTiepNhan, (long) Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong,
                //            (long) Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                //    }
                //}
            }            

            if(isSaveChange)
            {
                //await UpdateAsync(yeuCauTiepNhan);
                BaseRepository.Context.SaveChanges();
            }
        }

        public async Task PrepareForEditDichVuAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
            //var yeuCauKhamBenhEntries = BaseRepository.Context.ChangeTracker.Entries<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>().Where(o => o.State == EntityState.Modified);
            //var yeuCauDichVuKyThuatEntries = BaseRepository.Context.ChangeTracker.Entries<YeuCauDichVuKyThuat>().Where(o=>o.State == EntityState.Modified);
            //var yeuCauDuocPhamBenhVienEntries = BaseRepository.Context.ChangeTracker.Entries<YeuCauDuocPhamBenhVien>().Where(o => o.State == EntityState.Modified);
            //var yeuCauVatTuBenhVienEntries = BaseRepository.Context.ChangeTracker.Entries<YeuCauVatTuBenhVien>().Where(o => o.State == EntityState.Modified);

            var yeuCauKhamBenhEntries = new List<EntityEntry<BaseEntity>>();
            var yeuCauDichVuKyThuatEntries = new List<EntityEntry<BaseEntity>>();
            var yeuCauDuocPhamBenhVienEntries = new List<EntityEntry<BaseEntity>>();
            var yeuCauVatTuBenhVienEntries = new List<EntityEntry<BaseEntity>>();

            foreach (var e in BaseRepository.Context.ChangeTracker.Entries<BaseEntity>().Where(o => o.State == EntityState.Modified))
            {
                if(e.Entity is Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh)
                {
                    yeuCauKhamBenhEntries.Add(e);
                }
                else if(e.Entity is YeuCauDichVuKyThuat)
                {
                    yeuCauDichVuKyThuatEntries.Add(e);
                }
                else if (e.Entity is YeuCauDuocPhamBenhVien)
                {
                    yeuCauDuocPhamBenhVienEntries.Add(e);
                }
                else if (e.Entity is YeuCauVatTuBenhVien)
                {
                    yeuCauVatTuBenhVienEntries.Add(e);
                }
            }
            if (yeuCauKhamBenhEntries.Count > 0)
            {
                foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs)
                {
                    //var peYeuCauGoiDichVuId = BaseRepository.Context.Entry(yeuCauKhamBenh).Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.YeuCauGoiDichVuId) == o.Metadata.Name);
                    var peYeuCauGoiDichVuId = yeuCauKhamBenhEntries.FirstOrDefault(o => o.Entity.Id == yeuCauKhamBenh.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.YeuCauGoiDichVuId) == o.Metadata.Name);
                    if (peYeuCauGoiDichVuId != null)
                    {
                        if ((yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                             yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                             yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan ||
                             yeuCauKhamBenh.KhongTinhPhi == true) &&
                            (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham))
                        {
                            if (yeuCauKhamBenh.YeuCauGoiDichVuId != null)
                            {
                                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                                {
                                    var yeucaugoiDv = BaseRepository.Context.Set<YeuCauGoiDichVu>().AsNoTracking()
                                    .Include(o => o.YeuCauKhamBenhs)
                                    .Include(o => o.YeuCauDichVuKyThuats)
                                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                                    .FirstOrDefault(o => o.Id == yeuCauKhamBenh.YeuCauGoiDichVuId);
                                    if (yeucaugoiDv != null)
                                    {
                                        LoadYeuCauTiepNhanCongTyBaoHiemTuNhan(yeuCauTiepNhan);
                                        if (yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(o => GetChuongTrinhGoiDichVuBHTNBaoLanhs().Any(ct => ct.CongTyBaoHiemTuNhanId == o.CongTyBaoHiemTuNhanId && ct.Id == yeucaugoiDv.ChuongTrinhGoiDichVuId)))
                                        {
                                            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                            if (yeuCauKhamBenh.NoiDangKyId != null)
                                            {
                                                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                                            }
                                        }
                                        else
                                        {
                                            var soTienDaBaoLanhSuDung = yeucaugoiDv.YeuCauKhamBenhs
                                            .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                                        y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                                            .Concat(yeucaugoiDv.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                            y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                                            .Concat(yeucaugoiDv.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                                                .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                                            .ToList()
                                            .Select(o => o.Item1 * o.Item2).DefaultIfEmpty().Sum();
                                            if (yeucaugoiDv.SoTienBenhNhanDaChi.GetValueOrDefault() - soTienDaBaoLanhSuDung >= yeuCauKhamBenh.DonGiaSauChietKhau.GetValueOrDefault())
                                            {
                                                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                                if (yeuCauKhamBenh.NoiDangKyId != null)
                                                {
                                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                                                }
                                            }
                                        }
                                    }
                                }
                                BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                            }
                            else
                            {
                                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                                    soDuTk > GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan))
                                {
                                    yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                    if (yeuCauKhamBenh.NoiDangKyId != null)
                                    {
                                        yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan,
                                            yeuCauKhamBenh.NoiDangKyId.Value));
                                    }
                                }
                                BaoLanhDvTrongGoiMarketing(yeuCauTiepNhan);
                            }
                        }
                        else
                        {
                            throw new Exception(
                                _localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                    }
                    else
                    {
                        //var peGia = BaseRepository.Context.Entry(yeuCauKhamBenh).Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.Gia) == o.Metadata.Name);
                        var peGia = yeuCauKhamBenhEntries.FirstOrDefault(o => o.Entity.Id == yeuCauKhamBenh.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.Gia) == o.Metadata.Name);

                        if (peGia != null)
                        {
                            if ((yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                 yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                 yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauKhamBenh.KhongTinhPhi == true) &&
                                (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham))
                            {
                                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && soDuTk > GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan))
                                {
                                    yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                    if (yeuCauKhamBenh.NoiDangKyId != null)
                                    {
                                        yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan,
                                            yeuCauKhamBenh.NoiDangKyId.Value));
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                        //var peBacSiDangKy = BaseRepository.Context.Entry(yeuCauKhamBenh).Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.BacSiDangKyId) == o.Metadata.Name);
                        var peBacSiDangKy = yeuCauKhamBenhEntries.FirstOrDefault(o => o.Entity.Id == yeuCauKhamBenh.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.BacSiDangKyId) == o.Metadata.Name);
                        if (peBacSiDangKy != null)
                        {
                            if (yeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                        //var peNoiDangKy = BaseRepository.Context.Entry(yeuCauKhamBenh).Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.NoiDangKyId) == o.Metadata.Name);
                        var peNoiDangKy = yeuCauKhamBenhEntries.FirstOrDefault(o => o.Entity.Id == yeuCauKhamBenh.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh.NoiDangKyId) == o.Metadata.Name);
                        if (peNoiDangKy != null)
                        {
                            if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                            {
                                if (XoaKhoiHangDoi(yeuCauKhamBenh) && yeuCauKhamBenh.NoiDangKyId != null)
                                {
                                    yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan,
                                        yeuCauKhamBenh.NoiDangKyId.Value));
                                }
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                    }

                }
            }
            if (yeuCauDichVuKyThuatEntries.Count > 0)
            {
                foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats)
                {
                    //var peYeuCauGoiDichVuId = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.YeuCauGoiDichVuId) == o.Metadata.Name);
                    var peYeuCauGoiDichVuId = yeuCauDichVuKyThuatEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDichVuKyThuat.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.YeuCauGoiDichVuId) == o.Metadata.Name);

                    if (peYeuCauGoiDichVuId != null)
                    {
                        if ((yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                             yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                             yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauDichVuKyThuat.KhongTinhPhi == true) &&
                            (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien))
                        {
                            if (yeuCauDichVuKyThuat.YeuCauGoiDichVuId != null)
                            {
                                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                                {
                                    var yeucaugoiDv = BaseRepository.Context.Set<YeuCauGoiDichVu>().AsNoTracking()
                                        .Include(o => o.YeuCauKhamBenhs)
                                        .Include(o => o.YeuCauDichVuKyThuats)
                                        .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                                        .FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId);
                                    if (yeucaugoiDv != null)
                                    {
                                        LoadYeuCauTiepNhanCongTyBaoHiemTuNhan(yeuCauTiepNhan);
                                        if (yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any(o => GetChuongTrinhGoiDichVuBHTNBaoLanhs().Any(ct => ct.CongTyBaoHiemTuNhanId == o.CongTyBaoHiemTuNhanId && ct.Id == yeucaugoiDv.ChuongTrinhGoiDichVuId)))
                                        {
                                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                            {
                                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                            }
                                        }

                                        var soTienDaBaoLanhSuDung = yeucaugoiDv.YeuCauKhamBenhs
                                            .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                                        y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                            .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                                            .Concat(yeucaugoiDv.YeuCauDichVuKyThuats
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                            y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                                            .Concat(yeucaugoiDv.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                                                .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                .Select(y =>
                                                    (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                                            .ToList()
                                            .Select(o => o.Item1 * o.Item2).DefaultIfEmpty().Sum();
                                        if ((yeucaugoiDv.SoTienBenhNhanDaChi.GetValueOrDefault() - soTienDaBaoLanhSuDung) >=
                                            (yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault() * yeuCauDichVuKyThuat.SoLan))
                                        {
                                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                            {
                                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                            }
                                        }
                                    }
                                }
                                BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                            }
                            else
                            {
                                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                                    soDuTk > GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan))
                                {
                                    yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                    if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                    {
                                        yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                    }
                                }
                                BaoLanhDvTrongGoiMarketing(yeuCauTiepNhan);
                            }
                        }
                        else
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                    }
                    else
                    {
                        //var peKhongTinhPhi = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.KhongTinhPhi) == o.Metadata.Name);
                        //var peGia = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.Gia) == o.Metadata.Name);
                        //var peSoLan = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.SoLan) == o.Metadata.Name);

                        var peKhongTinhPhi = yeuCauDichVuKyThuatEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDichVuKyThuat.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.KhongTinhPhi) == o.Metadata.Name);
                        var peGia = yeuCauDichVuKyThuatEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDichVuKyThuat.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.Gia) == o.Metadata.Name);
                        var peSoLan = yeuCauDichVuKyThuatEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDichVuKyThuat.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.SoLan) == o.Metadata.Name);

                        if (peKhongTinhPhi != null && (peKhongTinhPhi.OriginalValue as bool?).GetValueOrDefault() != yeuCauDichVuKyThuat.KhongTinhPhi.GetValueOrDefault())
                        {
                            if (yeuCauDichVuKyThuat.KhongTinhPhi == true)
                            {
                                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                    yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                    yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                                {
                                    if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                                    {
                                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                        {
                                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                        }
                                    }
                                    yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                                    if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                                    {
                                        yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                                        yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                                        //bo duyet tu dong
                                        //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                        //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                        //{
                                        //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                        //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                        //}
                                    }
                                    BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                                }
                                else
                                {
                                    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                                }
                            }
                            else
                            {
                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;

                                if (yeuCauDichVuKyThuat.DuocHuongBaoHiem)
                                {
                                    yeuCauDichVuKyThuat.BaoHiemChiTra = null;
                                    //bo duyet tu dong
                                    //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                    //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                    //{
                                    //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                    //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                    //}
                                }
                            }
                        }

                        if (peGia != null)
                        {
                            if ((yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                 yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                 yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauDichVuKyThuat.KhongTinhPhi == true) &&
                                (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien))
                            {
                                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                                    soDuTk > GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan))
                                {
                                    yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                    if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                    {
                                        yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }

                        if (peSoLan != null)
                        {
                            if ((yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                 yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                 yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauDichVuKyThuat.KhongTinhPhi == true) &&
                                (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien))
                            {
                                if (yeuCauDichVuKyThuat.YeuCauGoiDichVuId != null)
                                {
                                    if (yeuCauDichVuKyThuat.BaoHiemChiTra == true)
                                    {
                                        yeuCauDichVuKyThuat.BaoHiemChiTra = null;
                                        //bo duyet tu dong
                                        //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                        //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                        //{
                                        //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                        //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                        //}
                                    }
                                    if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan)
                                    {
                                        var yeucaugoiDv = BaseRepository.Context.Set<YeuCauGoiDichVu>().AsNoTracking()
                                            .Include(o => o.YeuCauKhamBenhs)
                                            .Include(o => o.YeuCauDichVuKyThuats)
                                            .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                                            .FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId);
                                        if (yeucaugoiDv != null)
                                        {
                                            var soTienDaBaoLanhSuDung = yeucaugoiDv.YeuCauKhamBenhs
                                                .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                                                            y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)1))
                                                .Concat(yeucaugoiDv.YeuCauDichVuKyThuats
                                                    .Where(y => y.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                                                                y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                    .Select(y => (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLan)))
                                                .Concat(yeucaugoiDv.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                                                    .Where(y => y.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)
                                                    .Select(y =>
                                                        (y.DonGiaSauChietKhau.GetValueOrDefault(), (decimal)y.SoLuong)))
                                                .ToList()
                                                .Select(o => o.Item1 * o.Item2).DefaultIfEmpty().Sum();
                                            if (yeucaugoiDv.SoTienBenhNhanDaChi.GetValueOrDefault() - soTienDaBaoLanhSuDung >=
                                                (yeuCauDichVuKyThuat.DonGiaSauChietKhau.GetValueOrDefault() * yeuCauDichVuKyThuat.SoLan))
                                            {
                                                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                                if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                                {
                                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                                }
                                            }
                                        }
                                    }
                                    BaoLanhDvTrongGoiMarketing(yeuCauTiepNhan);
                                }
                                else
                                {
                                    if (yeuCauDichVuKyThuat.BaoHiemChiTra == true)
                                    {
                                        yeuCauDichVuKyThuat.BaoHiemChiTra = null;
                                        //bo duyet tu dong
                                        //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                        //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                        //{
                                        //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                        //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                        //}
                                    }

                                    if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                                        soDuTk > GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan))
                                    {
                                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                                        {
                                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                        }
                                    }
                                    BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                                }
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }

                        var peNoiThucHien = yeuCauDichVuKyThuatEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDichVuKyThuat.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDichVuKyThuat.NoiThucHienId) == o.Metadata.Name);
                        if (peNoiThucHien != null)
                        {
                            if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                            {
                                if (XoaKhoiHangDoi(yeuCauDichVuKyThuat) && yeuCauDichVuKyThuat.NoiThucHienId != null)
                                {
                                    yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(yeuCauTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                                }

                                //set lan thuc hien cho dv pttt
                                foreach (var dv in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat))
                                {
                                    var tuongTrinhPTTT = BaseRepository.Context.Entry(dv).Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                                    if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                                }
                                var max = yeuCauTiepNhan.YeuCauDichVuKyThuats.Max(o => o.LanThucHien);
                                var maxDangThucHien = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(o => o.Id != yeuCauDichVuKyThuat.Id && o.NoiThucHienId == yeuCauDichVuKyThuat.NoiThucHienId && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                    (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien) &&
                                                                                                    (o.YeuCauDichVuKyThuatTuongTrinhPTTT == null || o.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true) && o.TheoDoiSauPhauThuatThuThuatId == null).Max(o => o.LanThucHien);
                                yeuCauDichVuKyThuat.LanThucHien = maxDangThucHien ?? (max != null ? (max + 1) : 0);
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                    }

                }
            }
            if (yeuCauDuocPhamBenhVienEntries.Count > 0)
            {
                foreach (var yeuCauDuocPhamBenhVien in yeuCauTiepNhan.YeuCauDuocPhamBenhViens)
                {
                    //var peKhongTinhPhi = BaseRepository.Context.Entry(yeuCauDuocPhamBenhVien).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDuocPhamBenhVien.KhongTinhPhi) == o.Metadata.Name);
                    //var peSoLuong = BaseRepository.Context.Entry(yeuCauDuocPhamBenhVien).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDuocPhamBenhVien.SoLuong) == o.Metadata.Name);
                    var peKhongTinhPhi = yeuCauDuocPhamBenhVienEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDuocPhamBenhVien.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDuocPhamBenhVien.KhongTinhPhi) == o.Metadata.Name);
                    var peSoLuong = yeuCauDuocPhamBenhVienEntries.FirstOrDefault(o => o.Entity.Id == yeuCauDuocPhamBenhVien.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauDuocPhamBenhVien.SoLuong) == o.Metadata.Name);

                    if (peKhongTinhPhi != null && (peKhongTinhPhi.OriginalValue as bool?).GetValueOrDefault() != yeuCauDuocPhamBenhVien.KhongTinhPhi.GetValueOrDefault())
                    {
                        if (yeuCauDuocPhamBenhVien.KhongTinhPhi == true)
                        {
                            if (yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                            {
                                yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                                if (yeuCauDuocPhamBenhVien.DuocHuongBaoHiem)
                                {
                                    yeuCauDuocPhamBenhVien.MucHuongBaoHiem = 0;
                                    yeuCauDuocPhamBenhVien.BaoHiemChiTra = false;
                                    //bo duyet tu dong
                                    //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                    //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                    //{
                                    //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                    //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                    //}
                                }
                                BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                        else
                        {
                            yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;

                            if (yeuCauDuocPhamBenhVien.DuocHuongBaoHiem)
                            {
                                yeuCauDuocPhamBenhVien.BaoHiemChiTra = null;
                                //bo duyet tu dong
                                //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                //{
                                //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                //}
                            }
                        }
                    }

                    if (peSoLuong != null)
                    {
                        if ((yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                             yeuCauDuocPhamBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauDuocPhamBenhVien.KhongTinhPhi == true) &&
                            yeuCauDuocPhamBenhVien.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.ChuaThucHien && yeuCauDuocPhamBenhVien.YeuCauLinhDuocPhamId == null)
                        {
                            if (yeuCauDuocPhamBenhVien.BaoHiemChiTra == true)
                            {
                                yeuCauDuocPhamBenhVien.BaoHiemChiTra = null;
                                //bo duyet tu dong
                                //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                //{
                                //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, 0);
                                //}
                            }
                            BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                        }
                        else
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                    }
                }
            }
            if (yeuCauVatTuBenhVienEntries.Count > 0)
            {
                foreach (var yeuCauVatTuBenhVien in yeuCauTiepNhan.YeuCauVatTuBenhViens)
                {
                    //var peKhongTinhPhi = BaseRepository.Context.Entry(yeuCauVatTuBenhVien).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauVatTuBenhVien.KhongTinhPhi) == o.Metadata.Name);
                    //var peSoLuong = BaseRepository.Context.Entry(yeuCauVatTuBenhVien).Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauVatTuBenhVien.SoLuong) == o.Metadata.Name);
                    var peKhongTinhPhi = yeuCauVatTuBenhVienEntries.FirstOrDefault(o => o.Entity.Id == yeuCauVatTuBenhVien.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauVatTuBenhVien.KhongTinhPhi) == o.Metadata.Name);
                    var peSoLuong = yeuCauVatTuBenhVienEntries.FirstOrDefault(o => o.Entity.Id == yeuCauVatTuBenhVien.Id)?.Properties.FirstOrDefault(o => o.IsModified && nameof(YeuCauVatTuBenhVien.SoLuong) == o.Metadata.Name);

                    if (peKhongTinhPhi != null && (peKhongTinhPhi.OriginalValue as bool?).GetValueOrDefault() != yeuCauVatTuBenhVien.KhongTinhPhi.GetValueOrDefault())
                    {
                        if (yeuCauVatTuBenhVien.KhongTinhPhi == true)
                        {
                            if (yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan ||
                                yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                                yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)
                            {
                                yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;

                                if (yeuCauVatTuBenhVien.DuocHuongBaoHiem)
                                {
                                    yeuCauVatTuBenhVien.MucHuongBaoHiem = 0;
                                    yeuCauVatTuBenhVien.BaoHiemChiTra = false;
                                    //bo duyet tu dong
                                    //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                    //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                    //{
                                    //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                    //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                    //}
                                }
                                BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                            }
                            else
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                            }
                        }
                        else
                        {
                            yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;

                            if (yeuCauVatTuBenhVien.DuocHuongBaoHiem)
                            {
                                yeuCauVatTuBenhVien.BaoHiemChiTra = null;
                                //bo duyet tu dong
                                //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                //{
                                //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                //}
                            }
                        }
                    }

                    if (peSoLuong != null)
                    {
                        if ((yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                             yeuCauVatTuBenhVien.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan || yeuCauVatTuBenhVien.KhongTinhPhi == true) &&
                            yeuCauVatTuBenhVien.TrangThai == Enums.EnumYeuCauVatTuBenhVien.ChuaThucHien && yeuCauVatTuBenhVien.YeuCauLinhVatTuId == null)
                        {
                            if (yeuCauVatTuBenhVien.BaoHiemChiTra == true)
                            {
                                yeuCauVatTuBenhVien.BaoHiemChiTra = null;
                                //bo duyet tu dong
                                //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                                //if (cauHinh.DuyetBHYTTuDong && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                                //{
                                //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                                //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                                //}
                            }
                            BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, soDuTk - GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan));
                        }
                        else
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.ThayDoiDichVuKhongHopLe"));
                        }
                    }
                }
            }
            
            //await UpdateAsync(yeuCauTiepNhan);
            BaseRepository.Context.SaveChanges();
        }

        public async Task PrepareForEditYeuCauTiepNhanAndUpdateAsync(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var peCoBHTN = BaseRepository.Context.Entry(yeuCauTiepNhan).Properties
                .FirstOrDefault(o => o.IsModified && nameof(YeuCauTiepNhan.CoBHTN) == o.Metadata.Name);
            var peBHYTMucHuong = BaseRepository.Context.Entry(yeuCauTiepNhan).Properties
                .FirstOrDefault(o => o.IsModified && nameof(YeuCauTiepNhan.BHYTMucHuong) == o.Metadata.Name);
            var peCoBHYT = BaseRepository.Context.Entry(yeuCauTiepNhan).Properties
                .FirstOrDefault(o => o.IsModified && nameof(YeuCauTiepNhan.CoBHYT) == o.Metadata.Name);
            var peLaCapCuu = BaseRepository.Context.Entry(yeuCauTiepNhan).Properties
                .FirstOrDefault(o => o.IsModified && nameof(YeuCauTiepNhan.LaCapCuu) == o.Metadata.Name);

            if (peBHYTMucHuong != null || peCoBHYT != null)
            {
                var taiKhoanBenhNhanThus = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.TaiKhoanBenhNhanThus);
                if (!taiKhoanBenhNhanThus.IsLoaded) taiKhoanBenhNhanThus.Load();

                foreach(var taiKhoanBenhNhanThu in yeuCauTiepNhan.TaiKhoanBenhNhanThus.Where(o => o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true))
                {
                    var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(taiKhoanBenhNhanThu).Collection(o => o.TaiKhoanBenhNhanChis);
                    if (!taiKhoanBenhNhanChis.IsLoaded) taiKhoanBenhNhanChis.Load();
                }


                //if (yeuCauTiepNhan.TaiKhoanBenhNhanThus.Any(o => o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true))
                //{
                //    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiThuTien"));
                //}

                var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauKhamBenhs);
                if (!yeuCauKhamBenhs.IsLoaded) yeuCauKhamBenhs.Load();
                var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauTiepNhan)
                    .Collection(o => o.YeuCauDichVuKyThuats);
                if (!yeuCauDichVuKyThuats.IsLoaded) yeuCauDichVuKyThuats.Load();
                var yeuCauDichVuGiuongBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                    .Collection(o => o.YeuCauDichVuGiuongBenhViens);
                if (!yeuCauDichVuGiuongBenhViens.IsLoaded) yeuCauDichVuGiuongBenhViens.Load();
                var yeuCauDuocPhamBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                    .Collection(o => o.YeuCauDuocPhamBenhViens);
                if (!yeuCauDuocPhamBenhViens.IsLoaded) yeuCauDuocPhamBenhViens.Load();
                var yeuCauVatTuBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                    .Collection(o => o.YeuCauVatTuBenhViens);
                if (!yeuCauVatTuBenhViens.IsLoaded) yeuCauVatTuBenhViens.Load();
                var donThuocThanhToans = BaseRepository.Context.Entry(yeuCauTiepNhan)
                    .Collection(o => o.DonThuocThanhToans);
                if (!donThuocThanhToans.IsLoaded) donThuocThanhToans.Load();
                foreach (var donThuocThanhToan in yeuCauTiepNhan.DonThuocThanhToans)
                {
                    var donThuocThanhToanChiTiets = BaseRepository.Context.Entry(donThuocThanhToan)
                        .Collection(o => o.DonThuocThanhToanChiTiets);
                    if (!donThuocThanhToanChiTiets.IsLoaded) donThuocThanhToanChiTiets.Load();
                }

                if (yeuCauTiepNhan.CoBHYT == true)
                {
                    if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&yeuCauTiepNhan.BHYTMucHuong.GetValueOrDefault() != 0)
                    {
                        foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs)
                        {
                            if (yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() > 0)
                            {
                                yeuCauKhamBenh.DuocHuongBaoHiem = true;
                                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                {
                                    yeuCauKhamBenh.MucHuongBaoHiem = 0;
                                    yeuCauKhamBenh.BaoHiemChiTra = false;
                                }
                            }
                        }
                        foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats)
                        {
                            if (yeuCauDichVuKyThuat.YeuCauKhamBenhId != null && yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => o.DuocHuongBaoHiem).Select(o => o.Id)
                                    .Contains(yeuCauDichVuKyThuat.YeuCauKhamBenhId.Value) && yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() > 0)
                            {
                                yeuCauDichVuKyThuat.DuocHuongBaoHiem = true;
                                if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                {
                                    yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                                    yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                                }
                            }
                        }
                        //bo duyet tu dong
                        //var cauHinh = _cauHinhService.LoadSetting<CauHinhChung>();
                        //if (cauHinh.DuyetBHYTTuDong)
                        //{
                        //    var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                        //    soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
                        //    DuyetBHYT(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong,
                        //        (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong, soDuTk);
                        //}
                    }
                    else if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                    {
                        var yeuCauTiepNhanTheBHYTs = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauTiepNhanTheBHYTs);

                        if (!yeuCauTiepNhanTheBHYTs.IsLoaded) yeuCauTiepNhanTheBHYTs.Load();
                        if (yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                        {
                            foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats)
                            {
                                if (yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() > 0)
                                {
                                    yeuCauDichVuKyThuat.DuocHuongBaoHiem = true;
                                    if (yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                    {
                                        yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                                        yeuCauDichVuKyThuat.BaoHiemChiTra = false;
                                    }
                                }
                            }
                            foreach (var yeuCauDichVuGiuongBenhVien in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens)
                            {
                                if (yeuCauDichVuGiuongBenhVien.DonGiaBaoHiem.GetValueOrDefault() > 0)
                                {
                                    yeuCauDichVuGiuongBenhVien.DuocHuongBaoHiem = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(yeuCauTiepNhan.TaiKhoanBenhNhanThus.Any(o => o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true && o.TaiKhoanBenhNhanChis.Any(c=>c.MucHuongBaoHiem.GetValueOrDefault()>0 && c.DaHuy != true)))
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiThuTien"));
                    }
                    if (yeuCauTiepNhan.DonThuocThanhToans.Any(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.DonThuocThanhToanChiTiets.Any()))
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc"));
                    }
                    if (yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(o => o.LaDuocPhamBHYT && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauDuocPham"));
                    }
                    if (yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(o => o.LaVatTuBHYT && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauVatTu"));
                    }

                    if (yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                    {
                        var checkThuTienBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId 
                                                                                        && tn.TaiKhoanBenhNhanThus.Any(o => o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true
                                                                                                                            && o.TaiKhoanBenhNhanChis.Any(c => c.MucHuongBaoHiem.GetValueOrDefault() > 0 && c.DaHuy != true)));
                        if (checkThuTienBHYT)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiThuTien"));
                        }

                        var checkKeDonThuocBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                                                                        && tn.DonThuocThanhToans.Any(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.DonThuocThanhToanChiTiets.Any()));
                        if (checkKeDonThuocBHYT)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc"));
                        }

                        var checkYeuCauDuocPhamBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                                                                        && tn.YeuCauDuocPhamBenhViens.Any(o => o.LaDuocPhamBHYT && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy));
                        if (checkYeuCauDuocPhamBHYT)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauDuocPham"));
                        }

                        var checkYeuCauVatTuBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                                                                                        && tn.YeuCauVatTuBenhViens.Any(o => o.LaVatTuBHYT && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy));
                        if (checkYeuCauVatTuBHYT)
                        {
                            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc"));
                        }
                    }
                    else if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                    {
                        var yeuCauTiepNhanNoiTru = BaseRepository.TableNoTracking.Where(o => o.MaYeuCauTiepNhan == yeuCauTiepNhan.MaYeuCauTiepNhan && yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy).FirstOrDefault();
                        if (yeuCauTiepNhanNoiTru != null)
                        {
                            var checkThuTienBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhanNoiTru.Id
                                                                                            && tn.TaiKhoanBenhNhanThus.Any(o => o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.DaHuy != true
                                                                                                                                && o.TaiKhoanBenhNhanChis.Any(c => c.MucHuongBaoHiem.GetValueOrDefault() > 0 && c.DaHuy != true)));
                            if (checkThuTienBHYT)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiThuTien"));
                            }

                            var checkKeDonThuocBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhanNoiTru.Id
                                                                                            && tn.DonThuocThanhToans.Any(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.DonThuocThanhToanChiTiets.Any()));
                            if (checkKeDonThuocBHYT)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc"));
                            }

                            var checkYeuCauDuocPhamBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhanNoiTru.Id
                                                                                            && tn.YeuCauDuocPhamBenhViens.Any(o => o.LaDuocPhamBHYT && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy));
                            if (checkYeuCauDuocPhamBHYT)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKhiYeuCauDuocPham"));
                            }

                            var checkYeuCauVatTuBHYT = BaseRepository.TableNoTracking.Any(tn => tn.Id == yeuCauTiepNhanNoiTru.Id
                                                                                            && tn.YeuCauVatTuBenhViens.Any(o => o.LaVatTuBHYT && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy));
                            if (checkYeuCauVatTuBHYT)
                            {
                                throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.CapNhatBHYTSauKeDonThuoc"));
                            }
                        }
                    }
                    foreach (var yeuCauKhamBenh in yeuCauTiepNhan.YeuCauKhamBenhs)
                    {
                        if (yeuCauKhamBenh.DuocHuongBaoHiem && (yeuCauKhamBenh.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan || yeuCauKhamBenh.BaoHiemChiTra == false || yeuCauKhamBenh.MucHuongBaoHiem == 0))
                        {
                            yeuCauKhamBenh.DuocHuongBaoHiem = false;
                            yeuCauKhamBenh.MucHuongBaoHiem = 0;
                            yeuCauKhamBenh.BaoHiemChiTra = null;
                        }
                    }
                    foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats)
                    {
                        if (yeuCauDichVuKyThuat.DuocHuongBaoHiem && (yeuCauDichVuKyThuat.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan || yeuCauDichVuKyThuat.BaoHiemChiTra == false || yeuCauDichVuKyThuat.MucHuongBaoHiem == 0))
                        {
                            yeuCauDichVuKyThuat.DuocHuongBaoHiem = false;
                            yeuCauDichVuKyThuat.MucHuongBaoHiem = 0;
                            yeuCauDichVuKyThuat.BaoHiemChiTra = null;
                        }
                    }
                    foreach (var yeuCauDichVuGiuongBenhVien in yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens)
                    {
                        if (yeuCauDichVuGiuongBenhVien.DuocHuongBaoHiem && (yeuCauDichVuGiuongBenhVien.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan || yeuCauDichVuGiuongBenhVien.BaoHiemChiTra == false || yeuCauDichVuGiuongBenhVien.MucHuongBaoHiem == 0))
                        {
                            yeuCauDichVuGiuongBenhVien.DuocHuongBaoHiem = false;
                            yeuCauDichVuGiuongBenhVien.MucHuongBaoHiem = 0;
                            yeuCauDichVuGiuongBenhVien.BaoHiemChiTra = null;
                        }
                    }
                }
            }
            if (peCoBHTN != null || peLaCapCuu != null)
            {
                if ((peCoBHTN != null && yeuCauTiepNhan.CoBHTN == true) || (peLaCapCuu != null && yeuCauTiepNhan.LaCapCuu == true))
                {
                    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauKhamBenhs);
                    if (!yeuCauKhamBenhs.IsLoaded) yeuCauKhamBenhs.Load();
                    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauTiepNhan)
                        .Collection(o => o.YeuCauDichVuKyThuats);
                    if (!yeuCauDichVuKyThuats.IsLoaded) yeuCauDichVuKyThuats.Load();
                    var yeuCauDichVuGiuongBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                        .Collection(o => o.YeuCauDichVuGiuongBenhViens);
                    if (!yeuCauDichVuGiuongBenhViens.IsLoaded) yeuCauDichVuGiuongBenhViens.Load();
                    var yeuCauDuocPhamBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                        .Collection(o => o.YeuCauDuocPhamBenhViens);
                    if (!yeuCauDuocPhamBenhViens.IsLoaded) yeuCauDuocPhamBenhViens.Load();
                    var yeuCauVatTuBenhViens = BaseRepository.Context.Entry(yeuCauTiepNhan)
                        .Collection(o => o.YeuCauVatTuBenhViens);
                    if (!yeuCauVatTuBenhViens.IsLoaded) yeuCauVatTuBenhViens.Load();
                    var donThuocThanhToans = BaseRepository.Context.Entry(yeuCauTiepNhan)
                        .Collection(o => o.DonThuocThanhToans);
                    if (!donThuocThanhToans.IsLoaded) donThuocThanhToans.Load();
                    foreach (var donThuocThanhToan in yeuCauTiepNhan.DonThuocThanhToans)
                    {
                        var donThuocThanhToanChiTiets = BaseRepository.Context.Entry(donThuocThanhToan)
                            .Collection(o => o.DonThuocThanhToanChiTiets);
                        if (!donThuocThanhToanChiTiets.IsLoaded) donThuocThanhToanChiTiets.Load();
                    }

                    BaoLanhDvNgoaiGoiMarketing(yeuCauTiepNhan, 0);
                    BaoLanhDvTrongGoiMarketing(yeuCauTiepNhan);                    
                }
            }
            //await UpdateAsync(yeuCauTiepNhan);
            BaseRepository.Context.SaveChanges();
        }

        public void CapNhatThongTinHanhChinhVaoNgoaiTru(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, s =>
                s.Include(u => u.YeuCauNhapVien).ThenInclude(u => u.YeuCauKhamBenh).ThenInclude(u => u.YeuCauTiepNhan).ThenInclude(o => o.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .Include(o=>o.YeuCauTiepNhanTheBHYTs)
                .Include(o => o.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            );

            //Cập nhật ngược lại yêu cầu nhập viện
            if (yeuCauTiepNhan != null)
            {
                var yeuCauTiepNhanNgoaiTru = yeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.YeuCauTiepNhan;
                if (yeuCauTiepNhanNgoaiTru != null)
                {
                    //Thong tin hanh chinh
                    yeuCauTiepNhanNgoaiTru.HoTen = yeuCauTiepNhan.HoTen.ToUpper();
                   yeuCauTiepNhanNgoaiTru.NgaySinh = yeuCauTiepNhan.NgaySinh;
                   yeuCauTiepNhanNgoaiTru.ThangSinh = yeuCauTiepNhan.ThangSinh;
                   yeuCauTiepNhanNgoaiTru.NamSinh = yeuCauTiepNhan.NamSinh;
                   yeuCauTiepNhanNgoaiTru.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                   yeuCauTiepNhanNgoaiTru.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                   yeuCauTiepNhanNgoaiTru.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                   yeuCauTiepNhanNgoaiTru.DiaChi = yeuCauTiepNhan.DiaChi;
                   yeuCauTiepNhanNgoaiTru.QuocTichId = yeuCauTiepNhan.QuocTichId;
                   yeuCauTiepNhanNgoaiTru.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                   yeuCauTiepNhanNgoaiTru.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                   yeuCauTiepNhanNgoaiTru.Email = yeuCauTiepNhan.Email;
                   yeuCauTiepNhanNgoaiTru.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                   yeuCauTiepNhanNgoaiTru.GioiTinh = yeuCauTiepNhan.GioiTinh;
                   yeuCauTiepNhanNgoaiTru.NoiLamViec = yeuCauTiepNhan.NoiLamViec;
                   yeuCauTiepNhanNgoaiTru.DanTocId = yeuCauTiepNhan.DanTocId;
                    yeuCauTiepNhanNgoaiTru.HinhThucDenId = yeuCauTiepNhan.HinhThucDenId;
                    yeuCauTiepNhanNgoaiTru.NoiGioiThieuId = yeuCauTiepNhan.NoiGioiThieuId;

                    //Thong tin BHTN
                    if (yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                    {
                        yeuCauTiepNhanNgoaiTru.CoBHTN = true;
                        if (yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                        {
                            foreach (var bhtn in yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                            {
                                bhtn.WillDelete = true;
                            }
                        }

                        var bhtnList = yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Select(o =>
                            new YeuCauTiepNhanCongTyBaoHiemTuNhan
                            {
                                CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId,
                                MaSoThe = o.MaSoThe,
                                DiaChi = o.DiaChi,
                                SoDienThoai = o.SoDienThoai,
                                NgayHieuLuc = o.NgayHieuLuc,
                                NgayHetHan = o.NgayHetHan,
                            });
                        foreach (var bhtn in bhtnList)
                        {
                            yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(bhtn);
                        }
                    }
                    else
                    {
                        yeuCauTiepNhanNgoaiTru.CoBHTN = null;
                        if (yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                        {
                            foreach (var bhtn in yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                            {
                                bhtn.WillDelete = true;
                            }
                        }
                    }
                    //Thong tin BHYT
                    if (yeuCauTiepNhan.CoBHYT==true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                    {
                        //var theDauTien = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.FirstOrDefault();
                        //if (theDauTien != null)
                        //{
                        //    yeuCauTiepNhanNgoaiTru.CoBHYT = true;
                        //    yeuCauTiepNhanNgoaiTru.TuNhap = yeuCauTiepNhan.TuNhap;
                        //    yeuCauTiepNhanNgoaiTru.BHYTNgayDuocMienCungChiTra = theDauTien.NgayDuocMienCungChiTra;
                        //    yeuCauTiepNhanNgoaiTru.BHYTMaDKBD = theDauTien.MaDKBD;
                        //    yeuCauTiepNhanNgoaiTru.BHYTNgayDu5Nam = theDauTien.NgayDu5Nam;
                        //    yeuCauTiepNhanNgoaiTru.BHYTMucHuong = theDauTien.MucHuong;
                        //    yeuCauTiepNhanNgoaiTru.BHYTDiaChi = theDauTien.DiaChi;
                        //    yeuCauTiepNhanNgoaiTru.BHYTMaSoThe = theDauTien.MaSoThe;
                        //    yeuCauTiepNhanNgoaiTru.BHYTCoQuanBHXH = theDauTien.CoQuanBHXH;
                        //    yeuCauTiepNhanNgoaiTru.BHYTMaKhuVuc = theDauTien.MaKhuVuc;
                        //    yeuCauTiepNhanNgoaiTru.BHYTNgayHieuLuc = theDauTien.NgayHieuLuc;
                        //    yeuCauTiepNhanNgoaiTru.BHYTNgayHetHan = theDauTien.NgayHetHan;
                        //    yeuCauTiepNhanNgoaiTru.BHYTDuocMienCungChiTra = theDauTien.DuocMienCungChiTra;
                        //}

                        //BVHD-3754
                        yeuCauTiepNhanNgoaiTru.CoBHYT = true;
                        yeuCauTiepNhanNgoaiTru.TuNhap = yeuCauTiepNhan.TuNhap;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayDuocMienCungChiTra = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra;
                        yeuCauTiepNhanNgoaiTru.BHYTMaDKBD = yeuCauTiepNhan.BHYTMaDKBD;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayDu5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam;
                        yeuCauTiepNhanNgoaiTru.BHYTMucHuong = yeuCauTiepNhan.BHYTMucHuong;
                        yeuCauTiepNhanNgoaiTru.BHYTDiaChi = yeuCauTiepNhan.BHYTDiaChi;
                        yeuCauTiepNhanNgoaiTru.BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe;
                        yeuCauTiepNhanNgoaiTru.BHYTCoQuanBHXH = yeuCauTiepNhan.BHYTCoQuanBHXH;
                        yeuCauTiepNhanNgoaiTru.BHYTMaKhuVuc = yeuCauTiepNhan.BHYTMaKhuVuc;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan;
                        yeuCauTiepNhanNgoaiTru.BHYTDuocMienCungChiTra = yeuCauTiepNhan.BHYTDuocMienCungChiTra;
                        yeuCauTiepNhanNgoaiTru.BHYTGiayMienCungChiTraId = yeuCauTiepNhan.BHYTGiayMienCungChiTraId;
                        //yeuCauTiepNhanNgoaiTru.BHYTGiayMienCungChiTra = yeuCauTiepNhan.BHYTGiayMienCungChiTra;

                        yeuCauTiepNhanNgoaiTru.LyDoVaoVien = yeuCauTiepNhanNgoaiTru.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen;
                    }
                    else
                    {
                        yeuCauTiepNhanNgoaiTru.CoBHYT = null;
                        yeuCauTiepNhanNgoaiTru.TuNhap = null;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayDuocMienCungChiTra = null;
                        yeuCauTiepNhanNgoaiTru.BHYTMaDKBD = null;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayDu5Nam = null;
                        yeuCauTiepNhanNgoaiTru.BHYTMucHuong = null;
                        yeuCauTiepNhanNgoaiTru.BHYTDiaChi = null;
                        yeuCauTiepNhanNgoaiTru.BHYTMaSoThe = null;
                        yeuCauTiepNhanNgoaiTru.BHYTCoQuanBHXH = null;
                        yeuCauTiepNhanNgoaiTru.BHYTMaKhuVuc = null;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayHieuLuc = null;
                        yeuCauTiepNhanNgoaiTru.BHYTNgayHetHan = null;
                        yeuCauTiepNhanNgoaiTru.BHYTDuocMienCungChiTra = null;

                        //BVHD-3754
                        yeuCauTiepNhanNgoaiTru.BHYTGiayMienCungChiTraId = yeuCauTiepNhan.BHYTGiayMienCungChiTraId;
                        //yeuCauTiepNhanNgoaiTru.BHYTGiayMienCungChiTra = yeuCauTiepNhan.BHYTGiayMienCungChiTra;

                        yeuCauTiepNhanNgoaiTru.LyDoVaoVien = null;
                    }
                    //Thong tin Nguoi giam ho
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                    yeuCauTiepNhanNgoaiTru.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;

                    PrepareForEditYeuCauTiepNhanAndUpdateAsync(yeuCauTiepNhanNgoaiTru).GetAwaiter().GetResult();
                }
            }
        }
        public async Task CapNhatThongTinHanhChinhVaoNoiTru(YeuCauTiepNhan yeuCauTiepNhanUpdate, string theBHYTCuTrongYCTN)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanUpdate.Id, s =>
                s.Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauNhapViens).ThenInclude(u => u.YeuCauTiepNhans).ThenInclude(u => u.NoiTruBenhAn)
                 .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauNhapViens).ThenInclude(u => u.YeuCauTiepNhans).ThenInclude(u => u.YeuCauTiepNhanTheBHYTs)
                 .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauNhapViens).ThenInclude(u => u.YeuCauTiepNhans).ThenInclude(u => u.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                 .Include(u => u.YeuCauKhamBenhs).ThenInclude(u => u.YeuCauNhapViens).ThenInclude(u => u.YeuCauTiepNhans).ThenInclude(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                 .Include(u => u.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            );

            //Cập nhật ngược lại yêu cầu nhập viện
            if (yeuCauTiepNhan != null)
            {
                //var yeuCauTiepNhanNoiTru = yeuCauTiepNhan.YeuCauKhamBenhs?.FirstOrDefault(o => o.YeuCauNhapViens.Any())?.YeuCauNhapViens?.FirstOrDefault(o => o.YeuCauTiepNhans.Any())?.YeuCauTiepNhans?.FirstOrDefault();
                var yeuCauTiepNhanNoiTru = yeuCauTiepNhan.YeuCauKhamBenhs.SelectMany(o=>o.YeuCauNhapViens).SelectMany(o=>o.YeuCauTiepNhans).Where(o=>o.TrangThaiYeuCauTiepNhan!=Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy).FirstOrDefault();
                if (yeuCauTiepNhanNoiTru != null)
                {

                    #region Cập nhật thông tin hành chính
                    yeuCauTiepNhanNoiTru.HoTen = yeuCauTiepNhan.HoTen;
                    yeuCauTiepNhanNoiTru.NgaySinh = yeuCauTiepNhan.NgaySinh;
                    yeuCauTiepNhanNoiTru.ThangSinh = yeuCauTiepNhan.ThangSinh;
                    yeuCauTiepNhanNoiTru.NamSinh = yeuCauTiepNhan.NamSinh;
                    yeuCauTiepNhanNoiTru.PhuongXaId = yeuCauTiepNhan.PhuongXaId;
                    yeuCauTiepNhanNoiTru.TinhThanhId = yeuCauTiepNhan.TinhThanhId;
                    yeuCauTiepNhanNoiTru.QuanHuyenId = yeuCauTiepNhan.QuanHuyenId;
                    yeuCauTiepNhanNoiTru.DiaChi = yeuCauTiepNhan.DiaChi;
                    yeuCauTiepNhanNoiTru.QuocTichId = yeuCauTiepNhan.QuocTichId;
                    yeuCauTiepNhanNoiTru.SoDienThoai = yeuCauTiepNhan.SoDienThoai;
                    yeuCauTiepNhanNoiTru.SoChungMinhThu = yeuCauTiepNhan.SoChungMinhThu;
                    yeuCauTiepNhanNoiTru.Email = yeuCauTiepNhan.Email;
                    yeuCauTiepNhanNoiTru.NgheNghiepId = yeuCauTiepNhan.NgheNghiepId;
                    yeuCauTiepNhanNoiTru.GioiTinh = yeuCauTiepNhan.GioiTinh;
                    yeuCauTiepNhanNoiTru.NoiLamViec = yeuCauTiepNhan.NoiLamViec;

                    //BVHD-3634
                    yeuCauTiepNhanNoiTru.DanTocId = yeuCauTiepNhan.DanTocId;
                    yeuCauTiepNhanNoiTru.HinhThucDenId = yeuCauTiepNhan.HinhThucDenId;
                    yeuCauTiepNhanNoiTru.NoiGioiThieuId = yeuCauTiepNhan.NoiGioiThieuId;

                    //Thong tin BHTN
                    if (yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                    {
                        yeuCauTiepNhanNoiTru.CoBHTN = true;
                        if (yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                        {
                            foreach (var bhtn in yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                            {
                                bhtn.WillDelete = true;
                            }
                        }

                        var bhtnList = yeuCauTiepNhan.YeuCauTiepNhanCongTyBaoHiemTuNhans.Select(o =>
                            new YeuCauTiepNhanCongTyBaoHiemTuNhan
                            {
                                CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId,
                                MaSoThe = o.MaSoThe,
                                DiaChi = o.DiaChi,
                                SoDienThoai = o.SoDienThoai,
                                NgayHieuLuc = o.NgayHieuLuc,
                                NgayHetHan = o.NgayHetHan,
                            });
                        foreach (var bhtn in bhtnList)
                        {
                            yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(bhtn);
                        }
                    }
                    else
                    {
                        yeuCauTiepNhanNoiTru.CoBHTN = null;
                        if (yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Any())
                        {
                            foreach (var bhtn in yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                            {
                                bhtn.WillDelete = true;
                            }
                        }
                    }
                    //Thong tin BHYT

                    //Thong tin Nguoi giam ho
                    yeuCauTiepNhanNoiTru.NguoiLienHeHoTen = yeuCauTiepNhan.NguoiLienHeHoTen;
                    yeuCauTiepNhanNoiTru.NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId;
                    yeuCauTiepNhanNoiTru.NguoiLienHeSoDienThoai = yeuCauTiepNhan.NguoiLienHeSoDienThoai;
                    yeuCauTiepNhanNoiTru.NguoiLienHeEmail = yeuCauTiepNhan.NguoiLienHeEmail;
                    yeuCauTiepNhanNoiTru.NguoiLienHeTinhThanhId = yeuCauTiepNhan.NguoiLienHeTinhThanhId;
                    yeuCauTiepNhanNoiTru.NguoiLienHeQuanHuyenId = yeuCauTiepNhan.NguoiLienHeQuanHuyenId;
                    yeuCauTiepNhanNoiTru.NguoiLienHePhuongXaId = yeuCauTiepNhan.NguoiLienHePhuongXaId;
                    yeuCauTiepNhanNoiTru.NguoiLienHeDiaChi = yeuCauTiepNhan.NguoiLienHeDiaChi;
                    #endregion

                    #region update 20/05/2021: cập nhật thông tin thẻ BHYT vào nội trú
                    if (!string.Equals(yeuCauTiepNhan.BHYTMaSoThe, theBHYTCuTrongYCTN, StringComparison.OrdinalIgnoreCase) && yeuCauTiepNhanNoiTru.NoiTruBenhAn?.TinhTrangRaVien != null)
                    {
                        throw new Exception(_localizationService.GetResource("ThongTinDoiTuongTiepNhan.BenhAn.DaRaVien"));
                    }

                    if (yeuCauTiepNhan.CoBHYT == true)
                    {
                        yeuCauTiepNhanNoiTru.CoBHYT = yeuCauTiepNhan.CoBHYT;
                        yeuCauTiepNhanNoiTru.TuNhap = yeuCauTiepNhan.TuNhap;

                        //BVHD-3754
                        yeuCauTiepNhanNoiTru.LyDoVaoVien = yeuCauTiepNhanNoiTru.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen;

                        // kiểm tra thông tin thẻ cũ theo data truyền từ YeuCauTiepNhan
                        if (!string.Equals(yeuCauTiepNhan.BHYTMaSoThe, theBHYTCuTrongYCTN ?? ""))
                        {
                            var theBHYTNhapTrongNoiTruOld = yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.FirstOrDefault(x => string.Equals(x.MaSoThe, theBHYTCuTrongYCTN ?? ""));
                            if (theBHYTNhapTrongNoiTruOld != null)
                            {
                                XoaThongTinTheBHYTNoiTru(yeuCauTiepNhanNoiTru, theBHYTNhapTrongNoiTruOld);
                            }
                        }

                        // kiểm tra thông tin thẻ mới theo data truyền từ FE
                        var theBHYTNhapTrongNoiTruNew = yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.Where(x => x.WillDelete != true).FirstOrDefault(x => string.Equals(x.MaSoThe, yeuCauTiepNhan.BHYTMaSoThe ?? ""));
                        if (theBHYTNhapTrongNoiTruNew == null)
                        {
                            var theBHYTMoiEntity = new YeuCauTiepNhanTheBHYT()
                            {
                                BenhNhanId = yeuCauTiepNhan.BenhNhanId.Value,
                                MaSoThe = yeuCauTiepNhan.BHYTMaSoThe,
                                MucHuong = yeuCauTiepNhan.BHYTMucHuong ?? 0,
                                MaDKBD = yeuCauTiepNhan.BHYTMaDKBD,
                                NgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc.Value,
                                NgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan,
                                DiaChi = yeuCauTiepNhan.BHYTDiaChi,
                                CoQuanBHXH = yeuCauTiepNhan.BHYTCoQuanBHXH,
                                NgayDu5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam,
                                NgayDuocMienCungChiTra = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra,
                                MaKhuVuc = yeuCauTiepNhan.BHYTMaKhuVuc, //todo: cần kiểm tra lại
                                DuocMienCungChiTra = yeuCauTiepNhan.BHYTDuocMienCungChiTra,
                                GiayMienCungChiTraId = yeuCauTiepNhan.BHYTGiayMienCungChiTraId,
                                TinhTrangThe = yeuCauTiepNhan.TinhTrangThe,
                                IsCheckedBHYT = yeuCauTiepNhan.IsCheckedBHYT,
                                DuocGiaHanThe = null
                            };

                            yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.Add(theBHYTMoiEntity);

                            // thêm lịch sử chuyển đối tượng
                            yeuCauTiepNhanNoiTru.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                            {
                                DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                                MaSoThe = yeuCauTiepNhan.BHYTMaSoThe,
                                DiaChi = yeuCauTiepNhan.BHYTDiaChi,
                                MucHuong = yeuCauTiepNhan.BHYTMucHuong ?? 0,
                                NgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc,
                                NgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan,
                                MaDKBD = yeuCauTiepNhan.BHYTMaDKBD,
                                NgayDuocMienCungChiTra = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra,
                                DuocGiaHanThe = false,
                                DuocMienCungChiTra = yeuCauTiepNhan.BHYTGiayMienCungChiTra != null,
                                GiayMienCungChiTraId = theBHYTMoiEntity.GiayMienCungChiTraId,
                                GiayMienCungChiTra = theBHYTMoiEntity.GiayMienCungChiTra,
                                NgayDu5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam,
                                TinhTrangThe = yeuCauTiepNhan.TinhTrangThe,
                                MaKhuVuc = null, //todo: cần kiểm tra lại
                                IsCheckedBHYT = yeuCauTiepNhan.IsCheckedBHYT
                            });
                        }
                        else // nếu trùng MS thẻ thì cập nhật thông tin
                        {
                            //theBHYTNhapTrongNoiTruNew.MaSoThe = model.BHYTMaSoThe;
                            theBHYTNhapTrongNoiTruNew.MucHuong = yeuCauTiepNhan.BHYTMucHuong ?? 0;
                            theBHYTNhapTrongNoiTruNew.MaDKBD = yeuCauTiepNhan.BHYTMaDKBD;
                            theBHYTNhapTrongNoiTruNew.NgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc.Value;
                            theBHYTNhapTrongNoiTruNew.NgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan;
                            theBHYTNhapTrongNoiTruNew.DiaChi = yeuCauTiepNhan.BHYTDiaChi;
                            theBHYTNhapTrongNoiTruNew.CoQuanBHXH = yeuCauTiepNhan.BHYTCoQuanBHXH;
                            theBHYTNhapTrongNoiTruNew.NgayDu5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam;
                            theBHYTNhapTrongNoiTruNew.NgayDuocMienCungChiTra = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra;
                            theBHYTNhapTrongNoiTruNew.GiayMienCungChiTraId = yeuCauTiepNhan.BHYTGiayMienCungChiTraId;
                            theBHYTNhapTrongNoiTruNew.MaKhuVuc = null; //todo: cần kiểm tra lại
                            theBHYTNhapTrongNoiTruNew.TinhTrangThe = yeuCauTiepNhan.TinhTrangThe;
                            theBHYTNhapTrongNoiTruNew.IsCheckedBHYT = yeuCauTiepNhan.IsCheckedBHYT;

                            //BVHD-3754
                            theBHYTNhapTrongNoiTruNew.DuocMienCungChiTra = yeuCauTiepNhan.BHYTDuocMienCungChiTra;
                            theBHYTNhapTrongNoiTruNew.MaKhuVuc = yeuCauTiepNhan.BHYTMaKhuVuc;
                        }

                        //BVHD-3754: trường hợp thẻ BHYT ngoại trú cập nhật là thẻ BHYT của YCTN nội trú
                        if (string.IsNullOrEmpty(yeuCauTiepNhanNoiTru.BHYTMaSoThe) 
                            || yeuCauTiepNhanNoiTru.BHYTMaSoThe.Equals(yeuCauTiepNhan.BHYTMaSoThe))
                        {
                            if (string.IsNullOrEmpty(yeuCauTiepNhanNoiTru.BHYTMaSoThe))
                            {
                                yeuCauTiepNhanNoiTru.BHYTMaSoThe = yeuCauTiepNhan.BHYTMaSoThe;
                            }

                            yeuCauTiepNhanNoiTru.BHYTMucHuong = yeuCauTiepNhan.BHYTMucHuong ?? 0;
                            yeuCauTiepNhanNoiTru.BHYTMaDKBD = yeuCauTiepNhan.BHYTMaDKBD;
                            yeuCauTiepNhanNoiTru.BHYTNgayHieuLuc = yeuCauTiepNhan.BHYTNgayHieuLuc.Value;
                            yeuCauTiepNhanNoiTru.BHYTNgayHetHan = yeuCauTiepNhan.BHYTNgayHetHan;
                            yeuCauTiepNhanNoiTru.BHYTDiaChi = yeuCauTiepNhan.BHYTDiaChi;
                            yeuCauTiepNhanNoiTru.BHYTCoQuanBHXH = yeuCauTiepNhan.BHYTCoQuanBHXH;
                            yeuCauTiepNhanNoiTru.BHYTNgayDu5Nam = yeuCauTiepNhan.BHYTNgayDu5Nam;
                            yeuCauTiepNhanNoiTru.BHYTNgayDuocMienCungChiTra = yeuCauTiepNhan.BHYTNgayDuocMienCungChiTra;
                            yeuCauTiepNhanNoiTru.BHYTGiayMienCungChiTraId = yeuCauTiepNhan.BHYTGiayMienCungChiTraId;
                            yeuCauTiepNhanNoiTru.BHYTMaKhuVuc = null; //todo: cần kiểm tra lại
                            yeuCauTiepNhanNoiTru.TinhTrangThe = yeuCauTiepNhan.TinhTrangThe;
                            yeuCauTiepNhanNoiTru.IsCheckedBHYT = yeuCauTiepNhan.IsCheckedBHYT;
                            yeuCauTiepNhanNoiTru.BHYTDuocMienCungChiTra = yeuCauTiepNhan.BHYTDuocMienCungChiTra;
                            yeuCauTiepNhanNoiTru.BHYTMaKhuVuc = yeuCauTiepNhan.BHYTMaKhuVuc;
                        }

                    }
                    else
                    {
                        var theBHYTNhapTrongNoiTru = yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.Where(x => x.WillDelete != true).FirstOrDefault(x => string.Equals(x.MaSoThe, theBHYTCuTrongYCTN ?? ""));
                        if (theBHYTNhapTrongNoiTru != null)
                        {
                            XoaThongTinTheBHYTNoiTru(yeuCauTiepNhanNoiTru, theBHYTNhapTrongNoiTru);
                        }
                    }

                    if (!yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.Any() || yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.All(x => x.WillDelete))
                    {
                        yeuCauTiepNhanNoiTru.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
                        {
                            DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.DichVu
                        });

                        yeuCauTiepNhanNoiTru.CoBHYT = null;
                        yeuCauTiepNhanNoiTru.TuNhap = null;

                        //BVHD-3754
                        yeuCauTiepNhanNoiTru.BHYTMaSoThe = null;
                        yeuCauTiepNhanNoiTru.BHYTNgayDuocMienCungChiTra = null;
                        yeuCauTiepNhanNoiTru.BHYTMaDKBD = null;
                        yeuCauTiepNhanNoiTru.BHYTNgayDu5Nam = null;
                        yeuCauTiepNhanNoiTru.BHYTMucHuong = null;
                        yeuCauTiepNhanNoiTru.BHYTDiaChi = null;
                        yeuCauTiepNhanNoiTru.BHYTCoQuanBHXH = null;
                        yeuCauTiepNhanNoiTru.BHYTMaKhuVuc = null;
                        yeuCauTiepNhanNoiTru.BHYTNgayHieuLuc = null;
                        yeuCauTiepNhanNoiTru.BHYTNgayHetHan = null;
                        yeuCauTiepNhanNoiTru.BHYTDuocMienCungChiTra = null;
                        yeuCauTiepNhanNoiTru.BHYTGiayMienCungChiTraId = null;
                        yeuCauTiepNhanNoiTru.LyDoVaoVien = null;
                        yeuCauTiepNhanNoiTru.TinhTrangThe = null;
                        yeuCauTiepNhanNoiTru.IsCheckedBHYT = null;
                    }
                    #endregion

                    //BaseRepository.Update(yeuCauTiepNhanNoiTru);
                    //BaseRepository.Context.SaveChanges();
                    await PrepareForEditYeuCauTiepNhanAndUpdateAsync(yeuCauTiepNhanNoiTru);
                }
            }
        }

        #region Update:  cập nhật thông tin thẻ BHYT -> nội trú

        private void XoaThongTinTheBHYTNoiTru(YeuCauTiepNhan yeuCauTiepNhan, YeuCauTiepNhanTheBHYT theBHYT)
        {
            theBHYT.WillDelete = true;
            yeuCauTiepNhan.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong()
            {
                DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                MaSoThe = theBHYT.MaSoThe,
                DiaChi = theBHYT.DiaChi,
                MucHuong = theBHYT.MucHuong,
                NgayHieuLuc = theBHYT.NgayHieuLuc,
                NgayHetHan = theBHYT.NgayHetHan,
                MaDKBD = theBHYT.MaDKBD,
                NgayDuocMienCungChiTra = theBHYT.NgayDuocMienCungChiTra,
                DuocGiaHanThe = theBHYT.DuocGiaHanThe,
                DuocMienCungChiTra = theBHYT.GiayMienCungChiTra != null,
                GiayMienCungChiTraId = theBHYT.GiayMienCungChiTraId,
                NgayDu5Nam = theBHYT.NgayDu5Nam,
                TinhTrangThe = theBHYT.TinhTrangThe,
                MaKhuVuc = theBHYT.MaKhuVuc,
                IsCheckedBHYT = theBHYT.IsCheckedBHYT,
                DaHuy = true
            });
        }
        #endregion

        /*
        private void ThanhToanHuyYeuCauKham(YeuCauTiepNhan yeuCauTiepNhan, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        {
            if (!yeuCauKhamBenh.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                //var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.TaiKhoanBenhNhanChis);
                //if (!taiKhoanBenhNhanChis.IsLoaded) taiKhoanBenhNhanChis.Load();
                //if (yeuCauKhamBenh.TaiKhoanBenhNhanChis.Any(o =>o.NhanVienThucHienId != (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong))
                //{
                //    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.HuyYeuCauKhamThuNganDaThu"));
                //}
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);
                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauKhamBenh = yeuCauKhamBenh,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauKhamBenh.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauKhamBenh.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauKhamBenh.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauKhamBenh.SoTienBenhNhanDaChi = 0;
            }
            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
        }

        private void ThanhToanHuyYeuCauDichVuKyThuat(YeuCauTiepNhan yeuCauTiepNhan, YeuCauDichVuKyThuat yeuCauDichVuKyThuat)
        {
            if (!yeuCauDichVuKyThuat.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                //var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Collection(o => o.TaiKhoanBenhNhanChis);
                //if (!taiKhoanBenhNhanChis.IsLoaded) taiKhoanBenhNhanChis.Load();
                //if (yeuCauDichVuKyThuat.TaiKhoanBenhNhanChis.Any(o => o.NhanVienThucHienId != (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong))
                //{
                //    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.HuyYeuCauDVKTThuNganDaThu"));
                //}
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);

                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauDichVuKyThuat = yeuCauDichVuKyThuat,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauDichVuKyThuat.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauDichVuKyThuat.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauDichVuKyThuat.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauDichVuKyThuat.SoTienBenhNhanDaChi = 0;
            }
            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
            
        }

        private void ThanhToanHuyYeuCauDichVuGiuong(YeuCauTiepNhan yeuCauTiepNhan, YeuCauDichVuGiuongBenhVien yeuCauDichVuGiuong)
        {
            if (!yeuCauDichVuGiuong.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                //var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauDichVuGiuong).Collection(o => o.TaiKhoanBenhNhanChis);
                //if (!taiKhoanBenhNhanChis.IsLoaded) taiKhoanBenhNhanChis.Load();
                //if (yeuCauDichVuGiuong.TaiKhoanBenhNhanChis.Any(o => o.NhanVienThucHienId != (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong))
                //{
                //    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.HuyYeuCauGiuongThuNganDaThu"));
                //}
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);

                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauDichVuGiuongBenhVien = yeuCauDichVuGiuong,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauDichVuGiuong.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauDichVuGiuong.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauDichVuGiuong.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauDichVuGiuong.SoTienBenhNhanDaChi = 0;
            }
            yeuCauDichVuGiuong.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
        }

        private void ThanhToanHuyYeuCauGoiDichVu(YeuCauTiepNhan yeuCauTiepNhan, YeuCauGoiDichVu yeuCauGoiDichVu)
        {
            if (!yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                //var taiKhoanBenhNhanChis = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.TaiKhoanBenhNhanChis);
                //if (!taiKhoanBenhNhanChis.IsLoaded) taiKhoanBenhNhanChis.Load();
                //if (yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Any(o => o.NhanVienThucHienId != (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong))
                //{
                //    throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.HuyGoiDichVuThuNganDaThu"));
                //}
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);
                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauGoiDichVu = yeuCauGoiDichVu,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauGoiDichVu.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauGoiDichVu.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauGoiDichVu.SoTienBenhNhanDaChi = 0;
            }
            yeuCauGoiDichVu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
            XoaKhoiHangDoi(yeuCauGoiDichVu);
        }

        private void ThanhToanHuyYeuCauDuocPhamBenhVien(YeuCauTiepNhan yeuCauTiepNhan, YeuCauDuocPhamBenhVien yeuCauDuocPhamBenhVien)
        {
            if (!yeuCauDuocPhamBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);
                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauDuocPhamBenhVien = yeuCauDuocPhamBenhVien,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauDuocPhamBenhVien.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauDuocPhamBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauDuocPhamBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauDuocPhamBenhVien.SoTienBenhNhanDaChi = 0;
            }
            yeuCauDuocPhamBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
        }

        private void ThanhToanHuyYeuCauVatTuBenhVien(YeuCauTiepNhan yeuCauTiepNhan, YeuCauVatTuBenhVien yeuCauVatTuBenhVien)
        {
            if (!yeuCauVatTuBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault().AlmostEqual(0))
            {
                var huyDichVu = new TaiKhoanBenhNhanHuyDichVu
                {
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    NgayHuy = DateTime.Now,
                    NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                    NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanHuyDichVus.Add(huyDichVu);

                var hoanthu = new TaiKhoanBenhNhanThu
                {
                    TaiKhoanBenhNhanHuyDichVu = huyDichVu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi,
                    LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                    TienMat = 0,
                    ChuyenKhoan = 0,
                    POS = 0,
                    NoiDungThu = Enums.LoaiThuTienBenhNhan.HoanTraChiPhi.GetDescription(),
                    NgayThu = DateTime.Now,
                    SoQuyen = 1,
                    HoanTraYeuCauVatTuBenhVien = yeuCauVatTuBenhVien,
                    NhanVienThucHienId = (long) Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long) Enums.PhongHeThong.PhongThanhToanTuDong
                };
                yeuCauTiepNhan.TaiKhoanBenhNhanThus.Add(hoanthu);
                yeuCauVatTuBenhVien.TaiKhoanBenhNhanChis.Add(new TaiKhoanBenhNhanChi
                {
                    TaiKhoanBenhNhanThu = hoanthu,
                    TaiKhoanBenhNhan = GetTaiKhoanBenhNhan(yeuCauTiepNhan),
                    LoaiChiTienBenhNhan = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi,
                    TienChiPhi = (-1) * yeuCauVatTuBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    NoiDungChi = Enums.LoaiChiTienBenhNhan.ThanhToanChiPhi.GetDescription(),
                    NgayChi = DateTime.Now,
                    SoQuyen = 1,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    NhanVienThucHienId = (long)Enums.NhanVienHeThong.NhanVienThanhToanTuDong,
                    NoiThucHienId = (long)Enums.PhongHeThong.PhongThanhToanTuDong
                });
                
                GetTaiKhoanBenhNhan(yeuCauTiepNhan).SoDuTaiKhoan += yeuCauVatTuBenhVien.SoTienBenhNhanDaChi.GetValueOrDefault();
                yeuCauVatTuBenhVien.SoTienBenhNhanDaChi = 0;
            }
            yeuCauVatTuBenhVien.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
        }
        */
        public void DuyetBHYT(YeuCauTiepNhan ycTiepNhan, long nguoiDuyetId, long noiDuyetId, decimal soDuTk)
        {
            var soTienTheoMucHuong100 = GetSoTienBhytSeDuyetTheoMucHuong(ycTiepNhan, 100);
            var mucHuongHienTai = soTienTheoMucHuong100 < SoTienBHYTSeThanhToanToanBo() ? 100 : ycTiepNhan.BHYTMucHuong.GetValueOrDefault();
            
            //xac nhan
            var duyetBaoHiem = new DuyetBaoHiem
            {
                NhanVienDuyetBaoHiemId = nguoiDuyetId,
                ThoiDiemDuyetBaoHiem = DateTime.Now,
                NoiDuyetBaoHiemId = noiDuyetId
            };

            var dsDichVuKhamBenh = ycTiepNhan.YeuCauKhamBenhs
                .Where(p => p.CreatedOn != null && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false &&
                            p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .OrderBy(o => o.Id)
                .Concat(ycTiepNhan.YeuCauKhamBenhs.Where(p =>
                    p.CreatedOn == null && p.DuocHuongBaoHiem)).ToList();
            foreach (var yeuCauKhamBenh in dsDichVuKhamBenh)
            {
                if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                   continue;
                }
                int tiLeBaoHiemThanhToanDvKham = TiLeHuongBHYTTheoLanKham(dsDichVuKhamBenh.IndexOf(yeuCauKhamBenh));
                if (yeuCauKhamBenh.BaoHiemChiTra == null || (mucHuongHienTai == 100 && yeuCauKhamBenh.MucHuongBaoHiem != 100) || (yeuCauKhamBenh.TiLeBaoHiemThanhToan != tiLeBaoHiemThanhToanDvKham)
                    || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yeuCauKhamBenh.MucHuongBaoHiem > mucHuongHienTai))
                {
                    yeuCauKhamBenh.MucHuongBaoHiem = tiLeBaoHiemThanhToanDvKham != 0 ? mucHuongHienTai : 0;
                    yeuCauKhamBenh.BaoHiemChiTra = tiLeBaoHiemThanhToanDvKham != 0;
                    yeuCauKhamBenh.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhToanDvKham;
                    yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    yeuCauKhamBenh.NhanVienDuyetBaoHiemId = nguoiDuyetId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauKhamBenh = yeuCauKhamBenh,
                        SoLuong = 1,
                        TiLeBaoHiemThanhToan = yeuCauKhamBenh.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = yeuCauKhamBenh.MucHuongBaoHiem,
                        DonGiaBaoHiem = yeuCauKhamBenh.DonGiaBaoHiem
                    });
                    
                }
            }
            foreach (var yckt in ycTiepNhan.YeuCauDichVuKyThuats
                .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                    continue;
                }
                int tiLeBaoHiemThanhDvkt = yckt.TiLeBaoHiemThanhToan ?? 100;
                if (yckt.BaoHiemChiTra == null || (mucHuongHienTai == 100 && yckt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yckt.MucHuongBaoHiem > mucHuongHienTai))
                {
                    yckt.MucHuongBaoHiem = mucHuongHienTai;
                    yckt.BaoHiemChiTra = true;
                    yckt.TiLeBaoHiemThanhToan = tiLeBaoHiemThanhDvkt;
                    yckt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    yckt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauDichVuKyThuat = yckt,
                        SoLuong = yckt.SoLan,
                        TiLeBaoHiemThanhToan = yckt.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = yckt.MucHuongBaoHiem,
                        DonGiaBaoHiem = yckt.DonGiaBaoHiem
                    });
                }
            }

            foreach (var ycdp in ycTiepNhan.YeuCauDuocPhamBenhViens
                        .Where(p =>p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
            {
                if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                    continue;
                }
                if (ycdp.BaoHiemChiTra == null || (mucHuongHienTai == 100 && ycdp.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdp.MucHuongBaoHiem > mucHuongHienTai))
                {
                    ycdp.MucHuongBaoHiem = mucHuongHienTai;
                    ycdp.BaoHiemChiTra = true;
                    ycdp.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    ycdp.NhanVienDuyetBaoHiemId = nguoiDuyetId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauDuocPhamBenhVien = ycdp,
                        SoLuong = ycdp.SoLuong,
                        TiLeBaoHiemThanhToan = ycdp.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = ycdp.MucHuongBaoHiem,
                        DonGiaBaoHiem = ycdp.DonGiaBaoHiem
                    });
                }
            }

            foreach (var ycvt in ycTiepNhan.YeuCauVatTuBenhViens
                .Where(p =>p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
            {
                if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                    continue;
                }
                if (ycvt.BaoHiemChiTra == null || (mucHuongHienTai == 100 && ycvt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycvt.MucHuongBaoHiem > mucHuongHienTai))
                {
                    ycvt.MucHuongBaoHiem = mucHuongHienTai;
                    ycvt.BaoHiemChiTra = true;
                    ycvt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    ycvt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        YeuCauVatTuBenhVien = ycvt,
                        SoLuong = ycvt.SoLuong,
                        TiLeBaoHiemThanhToan = ycvt.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = ycvt.MucHuongBaoHiem,
                        DonGiaBaoHiem = ycvt.DonGiaBaoHiem
                    });
                }
            }

            foreach (var ycdt in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                        .SelectMany(w => w.DonThuocThanhToanChiTiets)
                        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false))
            {
                if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                {
                    continue;
                }
                if (ycdt.BaoHiemChiTra == null || (mucHuongHienTai == 100 && ycdt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdt.MucHuongBaoHiem > mucHuongHienTai))
                {
                    ycdt.MucHuongBaoHiem = mucHuongHienTai;
                    ycdt.BaoHiemChiTra = true;
                    ycdt.ThoiDiemDuyetBaoHiem = DateTime.Now;
                    ycdt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

                    duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
                    {
                        DonThuocThanhToanChiTiet = ycdt,
                        SoLuong = ycdt.SoLuong,
                        TiLeBaoHiemThanhToan = ycdt.TiLeBaoHiemThanhToan,
                        MucHuongBaoHiem = ycdt.MucHuongBaoHiem,
                        DonGiaBaoHiem = ycdt.DonGiaBaoHiem
                    });
                }
            }
            
            //bao lanh thanh toan
            if (ycTiepNhan.CoBHTN == true || ycTiepNhan.LaCapCuu == true)
            {
                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs)
                {
                    if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham &&
                        yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                        yeuCauKhamBenh.DuocHuongBaoHiem && yeuCauKhamBenh.BaoHiemChiTra != null && yeuCauKhamBenh.YeuCauGoiDichVuId == null && yeuCauKhamBenh.YeuCauGoiDichVu == null)
                    {
                        yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauKhamBenh.NoiDangKyId != null)
                        {
                            yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats)
                {
                    if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien &&
                        yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                        yeuCauDichVuKyThuat.DuocHuongBaoHiem && yeuCauDichVuKyThuat.BaoHiemChiTra != null && yeuCauDichVuKyThuat.YeuCauGoiDichVuId == null && yeuCauDichVuKyThuat.YeuCauGoiDichVu == null)
                    {
                        yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                        if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                        {
                            yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                        }
                    }
                }
            }
            else
            {
                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs)
                {
                    if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham &&
                        yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                        yeuCauKhamBenh.DuocHuongBaoHiem && yeuCauKhamBenh.BaoHiemChiTra != null && yeuCauKhamBenh.YeuCauGoiDichVuId == null && yeuCauKhamBenh.YeuCauGoiDichVu == null)
                    {
                        decimal soTienCanBaoLanh = yeuCauKhamBenh.Gia - (yeuCauKhamBenh.DonGiaBaoHiem.GetValueOrDefault() * yeuCauKhamBenh.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauKhamBenh.MucHuongBaoHiem.GetValueOrDefault() / 100);
                        if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                        {
                            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauKhamBenh.NoiDangKyId != null)
                            {
                                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
                            }
                            soDuTk -= soTienCanBaoLanh;
                        }
                    }
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats)
                {
                    if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien &&
                        yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
                        yeuCauDichVuKyThuat.DuocHuongBaoHiem && yeuCauDichVuKyThuat.BaoHiemChiTra != null && yeuCauDichVuKyThuat.YeuCauGoiDichVuId == null && yeuCauDichVuKyThuat.YeuCauGoiDichVu == null)
                    {
                        decimal soTienCanBaoLanh = (yeuCauDichVuKyThuat.Gia - (yeuCauDichVuKyThuat.DonGiaBaoHiem.GetValueOrDefault() * yeuCauDichVuKyThuat.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * yeuCauDichVuKyThuat.MucHuongBaoHiem.GetValueOrDefault() / 100)) * yeuCauDichVuKyThuat.SoLan;
                        if (soTienCanBaoLanh.SoTienTuongDuong(0) || soDuTk >= soTienCanBaoLanh)
                        {
                            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.BaoLanhThanhToan;
                            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
                            {
                                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
                            }
                            soDuTk -= soTienCanBaoLanh;
                        }
                    }
                }
            }

            //BenhNhan benhNhan = null;
            //if (ycTiepNhan.YeuCauKhamBenhs.Any(o => o.YeuCauGoiDichVuId != null) || ycTiepNhan.YeuCauDichVuKyThuats.Any(o => o.YeuCauGoiDichVuId != null))
            //{
            //    benhNhan = BaseRepository.Context.Set<BenhNhan>().AsNoTracking().Include(o => o.YeuCauGoiDichVus).FirstOrDefault(o => o.Id == ycTiepNhan.BenhNhanId);
            //}

            //foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs)
            //{
            //    if (yeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham &&
            //        yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
            //        yeuCauKhamBenh.DuocHuongBaoHiem && yeuCauKhamBenh.BaoHiemChiTra != null && yeuCauKhamBenh.YeuCauGoiDichVuId != null)
            //    {
            //        if (benhNhan != null && benhNhan.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.Id == yeuCauKhamBenh.YeuCauGoiDichVuId))
            //        {
            //            yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
            //            if (yeuCauKhamBenh.NoiDangKyId != null)
            //            {
            //                yeuCauKhamBenh.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauKhamBenh.NoiDangKyId.Value));
            //            }
            //        }
            //    }
            //}
            //foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats)
            //{
            //    if (yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien &&
            //        yeuCauDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan &&
            //        yeuCauDichVuKyThuat.DuocHuongBaoHiem && yeuCauDichVuKyThuat.BaoHiemChiTra != null && yeuCauDichVuKyThuat.YeuCauGoiDichVuId != null)
            //    {
            //        if (benhNhan != null && benhNhan.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan && o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId))
            //        {
            //            yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.DaThanhToan;
            //            if (yeuCauDichVuKyThuat.NoiThucHienId != null)
            //            {
            //                yeuCauDichVuKyThuat.PhongBenhVienHangDois.Add(XepVaoHangDoi(ycTiepNhan, yeuCauDichVuKyThuat.NoiThucHienId.Value));
            //            }
            //        }
            //    }
            //}
            if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
            {
                ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
            }
        }
        
        protected decimal GetSoTienBhytSeDuyetTheoMucHuong(YeuCauTiepNhan yeuCauTiepNhan, List<InsuranceConfirmVo> dsXacNhanDuocChiTra, int mucHuong)
        {
            decimal total = 0;
            total += yeuCauTiepNhan.YeuCauKhamBenhs.OrderBy(o => o.Id)
                .Where(o => dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(s => s.Id).Contains(o.Id))
                .Select((o, i) => o.DonGiaBaoHiem.GetValueOrDefault() * TiLeHuongBHYTTheoLanKham(i) / 100 * mucHuong / 100).Sum();

            foreach (var insuranceConfirmVo in dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DichVuKyThuat))
            {
                var yckt = yeuCauTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == insuranceConfirmVo.Id);
                total += yckt.DonGiaBaoHiem.GetValueOrDefault() * (yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? insuranceConfirmVo.TiLeTheoDichVu.GetValueOrDefault() : 100) / 100 * mucHuong / 100 * yckt.SoLan;
            }

            total += yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                    .Where(o => dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh).Select(s => s.Id).Contains(o.Id))
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * mucHuong / 100).Sum();

            total += yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                    .Where(o => dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DuocPham).Select(s => s.Id).Contains(o.Id))
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

            total += yeuCauTiepNhan.YeuCauVatTuBenhViens
                    .Where(o => dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.VatTuTieuHao).Select(s => s.Id).Contains(o.Id))
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

            total += yeuCauTiepNhan.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets)
                    .Where(o => dsXacNhanDuocChiTra.Where(s => s.GroupType == Enums.EnumNhomGoiDichVu.DonThuocThanhToan).Select(s => s.Id).Contains(o.Id))
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();
            return total;
        }
        protected decimal GetSoTienBhytSeDuyetTheoMucHuong(YeuCauTiepNhan yeuCauTiepNhan, int mucHuong)
        {
            decimal total = 0;

            var dsDichVuKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs
                .Where(p => p.CreatedOn != null && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .OrderBy(o => o.Id)
                .Concat(yeuCauTiepNhan.YeuCauKhamBenhs.Where(p =>
                    p.CreatedOn == null && p.DuocHuongBaoHiem)).ToList();

            total += dsDichVuKhamBenh.Select((o, i) => o.DonGiaBaoHiem.GetValueOrDefault() * TiLeHuongBHYTTheoLanKham(i) / 100 * mucHuong / 100).Sum();

            var dsDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats
                .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
                

            foreach (var yckt in dsDichVuKyThuat)
            {
                total += yckt.DonGiaBaoHiem.GetValueOrDefault() * (yckt.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? (yckt.TiLeBaoHiemThanhToan ?? 100) : 100) / 100 * mucHuong / 100 * yckt.SoLan;
            }

            total += yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens
                .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra != false && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * mucHuong / 100).Sum();

            total += yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                    .Where(p => p.DuocHuongBaoHiem && p.KhongTinhPhi != true && p.BaoHiemChiTra != false &&  p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

            total += yeuCauTiepNhan.YeuCauVatTuBenhViens
                    .Where(p => p.DuocHuongBaoHiem && p.KhongTinhPhi != true && p.BaoHiemChiTra != false &&  p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();

            total += yeuCauTiepNhan.DonThuocThanhToans
                    .Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                    .SelectMany(o => o.DonThuocThanhToanChiTiets)
                    .Where(p => !p.WillDelete && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false)
                    .Select(o => o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * mucHuong / 100 * (decimal)o.SoLuong).Sum();
            return total;
        }

        protected void LoadGoiDichVuBenhNhan(YeuCauTiepNhan yeuCauTiepNhan)
        {
            if (yeuCauTiepNhan.BenhNhan == null)
            {
                var benhNhanRef = BaseRepository.Context.Entry(yeuCauTiepNhan).Reference(o => o.BenhNhan);
                if (!benhNhanRef.IsLoaded)
                    benhNhanRef.Load();
            }
            if (yeuCauTiepNhan.BenhNhan != null)
            {
                var yeuCauGoiDichVus = BaseRepository.Context.Entry(yeuCauTiepNhan.BenhNhan).Collection(o => o.YeuCauGoiDichVus);
                if (!yeuCauGoiDichVus.IsLoaded)
                    yeuCauGoiDichVus.Load();
                var yeuCauGoiDichVuSoSinhs = BaseRepository.Context.Entry(yeuCauTiepNhan.BenhNhan).Collection(o => o.YeuCauGoiDichVuSoSinhs);
                if (!yeuCauGoiDichVuSoSinhs.IsLoaded)
                    yeuCauGoiDichVuSoSinhs.Load();
            }
        }

        private void LoadYeuCauTiepNhanCongTyBaoHiemTuNhan(YeuCauTiepNhan yeuCauTiepNhan)
        {            
            if (yeuCauTiepNhan.Id != 0)
            {
                var yeuCauTiepNhanCongTyBaoHiemTuNhans = BaseRepository.Context.Entry(yeuCauTiepNhan).Collection(o => o.YeuCauTiepNhanCongTyBaoHiemTuNhans);
                if (!yeuCauTiepNhanCongTyBaoHiemTuNhans.IsLoaded)
                    yeuCauTiepNhanCongTyBaoHiemTuNhans.Load();
            }
        }

        protected PhongBenhVienHangDoi XepVaoHangDoi(YeuCauTiepNhan yeuCauTiepNhan, long phongBenhVienId)
        {
            var lastStt = GetPhongBenhVienHangDois().Where(o => o.PhongBenhVienId == phongBenhVienId).OrderBy(o => o.SoThuTu).LastOrDefault();
            var stt = lastStt?.SoThuTu + 1 ?? 1;
            var phongBenhVienHangDoi = new PhongBenhVienHangDoi
            {
                PhongBenhVienId = phongBenhVienId,
                YeuCauTiepNhan = yeuCauTiepNhan,
                TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham,
                SoThuTu = stt
            };
            _phongBenhVienHangDoi.Add(phongBenhVienHangDoi);
            try
            {
                LogManager.GetCurrentClassLogger().Info($"XepVaoHangDoi phongBenhVienId{phongBenhVienId}, yeuCauTiepNhanId{yeuCauTiepNhan.Id}");
            }
            catch (Exception e)
            {
                
            }
            return phongBenhVienHangDoi;
        }

        protected bool XoaKhoiHangDoi(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        {
            var hangDois = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.PhongBenhVienHangDois);
            if(!hangDois.IsLoaded) hangDois.Load();
            foreach (var hangDoi in yeuCauKhamBenh.PhongBenhVienHangDois)
            {
                hangDoi.WillDelete = true;
            }
            return yeuCauKhamBenh.PhongBenhVienHangDois.Count > 0;
        }

        protected bool XoaKhoiHangDoi(YeuCauDichVuKyThuat yeuCauDichVuKyThuat)
        {
            var hangDois = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Collection(o => o.PhongBenhVienHangDois);
            if (!hangDois.IsLoaded) hangDois.Load();
            foreach (var hangDoi in yeuCauDichVuKyThuat.PhongBenhVienHangDois)
            {
                hangDoi.WillDelete = true;
            }
            return yeuCauDichVuKyThuat.PhongBenhVienHangDois.Count > 0;
        }

        protected void XoaKhoiHangDoi(YeuCauGoiDichVu yeuCauGoiDichVu)
        {
            var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauKhamBenhs);
            if (!yeuCauKhamBenhs.IsLoaded) yeuCauKhamBenhs.Load();
            var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauDichVuKyThuats);
            if (!yeuCauDichVuKyThuats.IsLoaded) yeuCauDichVuKyThuats.Load();

            foreach (var yeuCauKhamBenh in yeuCauGoiDichVu.YeuCauKhamBenhs)
            {
                XoaKhoiHangDoi(yeuCauKhamBenh);
            }
            foreach (var yeuCauDichVuKyThuat in yeuCauGoiDichVu.YeuCauDichVuKyThuats)
            {
                XoaKhoiHangDoi(yeuCauDichVuKyThuat);
            }
        }

        private TaiKhoanBenhNhan GetTaiKhoanBenhNhan(YeuCauTiepNhan yeuCauTiepNhan)
        {
            if (yeuCauTiepNhan.Id != 0)
            {
                if (yeuCauTiepNhan.BenhNhan == null)
                {
                    var benhNhanRef = BaseRepository.Context.Entry(yeuCauTiepNhan).Reference(o => o.BenhNhan);
                    if (!benhNhanRef.IsLoaded)
                        benhNhanRef.Load();
                }
                if (yeuCauTiepNhan.BenhNhan != null)
                {
                    if (yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan == null)
                    {
                        var taiKhoanBenhNhanRef = BaseRepository.Context.Entry(yeuCauTiepNhan.BenhNhan)
                            .Reference(o => o.TaiKhoanBenhNhan);
                        if (!taiKhoanBenhNhanRef.IsLoaded)
                            taiKhoanBenhNhanRef.Load();
                        if (yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan == null)
                        {
                            yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan = new TaiKhoanBenhNhan { SoDuTaiKhoan = 0 };
                        }
                    }
                    return yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan;
                }
            }
            else
            {
                if (yeuCauTiepNhan.BenhNhan != null)
                {
                    return yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan ??
                           (yeuCauTiepNhan.BenhNhan.TaiKhoanBenhNhan = new TaiKhoanBenhNhan { SoDuTaiKhoan = 0 });
                }
                 if (yeuCauTiepNhan.BenhNhanId.GetValueOrDefault() != 0)
                {
                    var benhNhan = BaseRepository.Context.Set<BenhNhan>().Include(o => o.TaiKhoanBenhNhan)
                        .FirstOrDefault(o => o.Id == yeuCauTiepNhan.BenhNhanId.GetValueOrDefault());
                    if (benhNhan != null)
                    {
                        return benhNhan.TaiKhoanBenhNhan ?? (benhNhan.TaiKhoanBenhNhan = new TaiKhoanBenhNhan { SoDuTaiKhoan = 0 });
                    }
                }
            }
            throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.BenhNhan.IsNull"));
        }

        public async Task<decimal> GetSoTienDaTamUngAsync(long yeuCauTiepNhanId)
        {
            return await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanId);
        }

        protected decimal GetSoTienCanThanhToanNgoaiTru(YeuCauTiepNhan yeuCauTiepNhan)
        {
            return _taiKhoanBenhNhanService.GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
        }

        private List<PhongBenhVienHangDoi> _phongBenhVienHangDoi;
        private List<PhongBenhVienHangDoi> GetPhongBenhVienHangDois()
        {
            return _phongBenhVienHangDoi ?? (_phongBenhVienHangDoi = BaseRepository.Context.Set<PhongBenhVienHangDoi>().AsNoTracking().ToList());
        }

        private List<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVuBHTNBaoLanhs;
        protected List<ChuongTrinhGoiDichVu> GetChuongTrinhGoiDichVuBHTNBaoLanhs()
        {
            return _chuongTrinhGoiDichVuBHTNBaoLanhs ?? (_chuongTrinhGoiDichVuBHTNBaoLanhs = BaseRepository.Context.Set<ChuongTrinhGoiDichVu>().AsNoTracking().Where(o=>o.CongTyBaoHiemTuNhanId != null && o.TamNgung != true && (o.DenNgay == null || o.DenNgay >= DateTime.Now)).ToList());
        }

        private List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> _dichVuXetNghiems;
        private List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> GetDichVuXetNghiems()
        {
            return _dichVuXetNghiems ?? (_dichVuXetNghiems = BaseRepository.Context.Set<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().AsNoTracking().ToList());
        }

        private List<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSos;
        private List<DichVuXetNghiemKetNoiChiSo> GetDichVuXetNghiemKetNoiChiSos()
        {
            return _dichVuXetNghiemKetNoiChiSos ?? (_dichVuXetNghiemKetNoiChiSos = BaseRepository.Context.Set<DichVuXetNghiemKetNoiChiSo>().AsNoTracking().ToList());
        }

        private decimal? _soTienBHYTSeThanhToanToanBo;
        protected decimal SoTienBHYTSeThanhToanToanBo()
        {
            if (_soTienBHYTSeThanhToanToanBo == null)
            {
                _soTienBHYTSeThanhToanToanBo = _cauHinhService.SoTienBHYTSeThanhToanToanBo().Result;
            }
            return _soTienBHYTSeThanhToanToanBo.Value;
        }

        private List<double> _getTiLeHuongBaoHiem5LanKhamDichVuBHYT;
        private List<double> GetTiLeHuongBaoHiem5LanKhamDichVuBHYT()
        {
            if (_getTiLeHuongBaoHiem5LanKhamDichVuBHYT == null)
            {
                _getTiLeHuongBaoHiem5LanKhamDichVuBHYT = _cauHinhService.GetTyLeHuongBaoHiem5LanKhamDichVuBHYT().Result;
            }
            return _getTiLeHuongBaoHiem5LanKhamDichVuBHYT;
        }

        protected int TiLeHuongBHYTTheoLanKham(int lanKham)
        {
            if (lanKham < GetTiLeHuongBaoHiem5LanKhamDichVuBHYT().Count)
                return (int) GetTiLeHuongBaoHiem5LanKhamDichVuBHYT()[lanKham];
            return 0;
        }
        
        //public async Task YeuCauNhapVienTuKhamNgoaiTruAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        //{
        //    if (yeuCauKhamBenh.CoNhapVien == true && yeuCauKhamBenh.KhoaPhongNhapVienId != null && yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId != null)
        //    {
        //        var yeuCauNhapVienExist = yeuCauKhamBenh.YeuCauNhapViens.FirstOrDefault();
        //        if (yeuCauNhapVienExist != null)
        //        {
        //            yeuCauNhapVienExist.BacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
        //            yeuCauNhapVienExist.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
        //            //yeuCauNhapVienExist.ThoiDiemChiDinh = DateTime.Now;
        //            yeuCauNhapVienExist.KhoaPhongNhapVienId = yeuCauKhamBenh.KhoaPhongNhapVienId.Value;
        //            yeuCauNhapVienExist.LyDoNhapVien = yeuCauKhamBenh.LyDoNhapVien;
        //            yeuCauNhapVienExist.ChanDoanNhapVienICDId = yeuCauKhamBenh.IcdchinhId;
        //            yeuCauNhapVienExist.ChanDoanNhapVienGhiChu = yeuCauKhamBenh.GhiChuICDChinh;

        //            var yeuCauNhapVienChanDoanKemTheos = BaseRepository.Context.Entry(yeuCauNhapVienExist).Collection(o => o.YeuCauNhapVienChanDoanKemTheos);
        //            if (!yeuCauNhapVienChanDoanKemTheos.IsLoaded)
        //                yeuCauNhapVienChanDoanKemTheos.Load();
        //            foreach (var yeuCauNhapVienChanDoanKemTheo in yeuCauNhapVienExist.YeuCauNhapVienChanDoanKemTheos)
        //            {
        //                yeuCauNhapVienChanDoanKemTheo.WillDelete = true;
        //            }

        //            var yeuCauKhamBenhICDKhacs = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.YeuCauKhamBenhICDKhacs);
        //            if (!yeuCauKhamBenhICDKhacs.IsLoaded)
        //                yeuCauKhamBenhICDKhacs.Load();
        //            foreach (var yeuCauKhamBenhIcdKhac in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
        //            {
        //                yeuCauNhapVienExist.YeuCauNhapVienChanDoanKemTheos.Add(new YeuCauNhapVienChanDoanKemTheo { ICDId = yeuCauKhamBenhIcdKhac.ICDId, GhiChu = yeuCauKhamBenhIcdKhac.GhiChu });
        //            }
        //            await BaseRepository.Context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            var yeuCauNhapVien = new YeuCauNhapVien
        //            {
        //                BenhNhanId = yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId.Value,
        //                BacSiChiDinhId = _userAgentHelper.GetCurrentUserId(),
        //                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
        //                ThoiDiemChiDinh = DateTime.Now,
        //                YeuCauKhamBenhId = yeuCauKhamBenh.Id,
        //                KhoaPhongNhapVienId = yeuCauKhamBenh.KhoaPhongNhapVienId.Value,
        //                LyDoNhapVien = yeuCauKhamBenh.LyDoNhapVien,
        //                LaCapCuu = false,
        //                ChanDoanNhapVienICDId = yeuCauKhamBenh.IcdchinhId,
        //                ChanDoanNhapVienGhiChu = yeuCauKhamBenh.GhiChuICDChinh
        //            };

        //            var yeuCauKhamBenhICDKhacs = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.YeuCauKhamBenhICDKhacs);
        //            if (!yeuCauKhamBenhICDKhacs.IsLoaded)
        //                yeuCauKhamBenhICDKhacs.Load();
        //            foreach (var yeuCauKhamBenhIcdKhac in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
        //            {
        //                yeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos.Add(new YeuCauNhapVienChanDoanKemTheo { ICDId = yeuCauKhamBenhIcdKhac.ICDId, GhiChu = yeuCauKhamBenhIcdKhac.GhiChu });
        //            }

        //            var chuaThuTienNgoaiTru = BaseRepository.TableNoTracking.Any(s => s.Id == yeuCauKhamBenh.YeuCauTiepNhanId
        //                    && (s.YeuCauKhamBenhs.Any(k => k.KhongTinhPhi != true && k.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
        //                       || s.YeuCauDichVuKyThuats.Any(k => k.KhongTinhPhi != true && k.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
        //                       || s.YeuCauDuocPhamBenhViens.Any(k => k.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
        //                       || s.YeuCauVatTuBenhViens.Any(k => k.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
        //                       || s.DonThuocThanhToans.Any(k => k.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && k.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))));

        //            if (chuaThuTienNgoaiTru)
        //            {
        //                BaseRepository.Context.Set<YeuCauNhapVien>().Add(yeuCauNhapVien);
        //                await BaseRepository.Context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                var yeuCauTiepNhanTruoc = yeuCauKhamBenh.YeuCauTiepNhan;
        //                var yeuCauTiepNhanNoiTru = GetYeuCauTiepNhanNoiTru(yeuCauTiepNhanTruoc, yeuCauNhapVien);
        //                await BaseRepository.AddAsync(yeuCauTiepNhanNoiTru);
        //            }
        //        }
        //    }
        //}

        public async Task<bool> KiemTraVaLuuYeuCauNhapVienTuKhamNgoaiTruAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        {
            if (yeuCauKhamBenh.CoNhapVien != true)
            {
                var yeuCauNhapVien = yeuCauKhamBenh.YeuCauNhapViens.OrderBy(o => o.Id).LastOrDefault();
                if (yeuCauNhapVien != null)
                {
                    var yeuCauTiepNhanNoiTru = BaseRepository.Table.Where(o => o.YeuCauNhapVienId == yeuCauNhapVien.Id)
                        .Include(o=>o.NoiTruBenhAn)
                        .Include(o => o.TaiKhoanBenhNhanThus)
                        .Include(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToan)
                        .Include(o => o.YeuCauTiepNhanTheBHYTs)
                        .Include(o => o.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                        .Include(o => o.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                        .Include(o => o.NoiTruBenhAn)
                        .FirstOrDefault();
                    if (yeuCauTiepNhanNoiTru == null)
                    {
                        foreach (var yeuCauNhapVienChanDoanKemTheo in yeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos)
                        {
                            yeuCauNhapVienChanDoanKemTheo.WillDelete = true;
                        }
                        yeuCauNhapVien.WillDelete = true;
                    }
                    else
                    {
                        if (yeuCauTiepNhanNoiTru.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        {
                            if (yeuCauTiepNhanNoiTru.TaiKhoanBenhNhanThus.Any(x => x.DaHuy != true))
                            {
                                throw new Exception(_localizationService.GetResource("HuyNhapVien.BenhNhan.PhatSinhChiPhi"));
                            }

                            if (yeuCauTiepNhanNoiTru.NoiTruBenhAn != null)
                            {
                                throw new Exception(_localizationService.GetResource("HuyNhapVien.YeuCauTiepNhan.DaTaoBenhAn"));
                            }

                            if (yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                            {
                                yeuCauTiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToan.QuyetToanTheoNoiTru = false;
                            }

                            foreach (var theBHYT in yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs)
                            {
                                theBHYT.WillDelete = true;
                            }
                            foreach (var lichSuChuyenDoiTuong in yeuCauTiepNhanNoiTru.YeuCauTiepNhanLichSuChuyenDoiTuongs)
                            {
                                lichSuChuyenDoiTuong.WillDelete = true;
                            }
                            foreach (var cauTiepNhanCongTyBaoHiem in yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                            {
                                cauTiepNhanCongTyBaoHiem.WillDelete = true;
                            }
                            yeuCauTiepNhanNoiTru.WillDelete = true;
                            foreach (var yeuCauNhapVienChanDoanKemTheo in yeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos)
                            {
                                yeuCauNhapVienChanDoanKemTheo.WillDelete = true;
                            }
                            yeuCauNhapVien.WillDelete = true;
                        }
                    }
                    await BaseRepository.Context.SaveChangesAsync();
                    return true;
                }
            }
            else if (yeuCauKhamBenh.CoNhapVien == true && yeuCauKhamBenh.KhoaPhongNhapVienId != null && yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId != null)
            {
                var yeuCauNhapVienExist = yeuCauKhamBenh.YeuCauNhapViens.OrderBy(o=>o.Id).LastOrDefault();
                bool nhapVienLai = false;
                if (yeuCauNhapVienExist != null)
                {
                    Enums.EnumTrangThaiYeuCauTiepNhan? trangThaiYeuCauNhapVien = yeuCauNhapVienExist.YeuCauTiepNhans.FirstOrDefault()?.TrangThaiYeuCauTiepNhan;
                    if (trangThaiYeuCauNhapVien == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    {
                        nhapVienLai = true;
                    }
                    else
                    {
                        yeuCauNhapVienExist.BacSiChiDinhId = _userAgentHelper.GetCurrentUserId();
                        yeuCauNhapVienExist.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeuCauNhapVienExist.KhoaPhongNhapVienId = yeuCauKhamBenh.KhoaPhongNhapVienId.Value;
                        yeuCauNhapVienExist.LyDoNhapVien = yeuCauKhamBenh.LyDoNhapVien;
                        yeuCauNhapVienExist.ChanDoanNhapVienICDId = yeuCauKhamBenh.IcdchinhId;
                        yeuCauNhapVienExist.ChanDoanNhapVienGhiChu = yeuCauKhamBenh.GhiChuICDChinh;

                        var yeuCauNhapVienChanDoanKemTheos = BaseRepository.Context.Entry(yeuCauNhapVienExist).Collection(o => o.YeuCauNhapVienChanDoanKemTheos);
                        if (!yeuCauNhapVienChanDoanKemTheos.IsLoaded)
                            yeuCauNhapVienChanDoanKemTheos.Load();
                        foreach (var yeuCauNhapVienChanDoanKemTheo in yeuCauNhapVienExist.YeuCauNhapVienChanDoanKemTheos)
                        {
                            yeuCauNhapVienChanDoanKemTheo.WillDelete = true;
                        }

                        var yeuCauKhamBenhICDKhacs = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.YeuCauKhamBenhICDKhacs);
                        if (!yeuCauKhamBenhICDKhacs.IsLoaded)
                            yeuCauKhamBenhICDKhacs.Load();
                        foreach (var yeuCauKhamBenhIcdKhac in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                        {
                            yeuCauNhapVienExist.YeuCauNhapVienChanDoanKemTheos.Add(new YeuCauNhapVienChanDoanKemTheo { ICDId = yeuCauKhamBenhIcdKhac.ICDId, GhiChu = yeuCauKhamBenhIcdKhac.GhiChu });
                        }
                        await BaseRepository.Context.SaveChangesAsync();
                        return true;
                    }
                }
                if(yeuCauNhapVienExist == null || nhapVienLai)
                {
                    if (yeuCauKhamBenh.YeuCauTiepNhan.YeuCauKhamBenhs.Any(o => o.Id != yeuCauKhamBenh.Id && o.YeuCauNhapViens.Any(nv=>nv.YeuCauTiepNhans.Count() == 0 || nv.YeuCauTiepNhans.Any(tn=>tn.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)))
                        
                        //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
                        //Nếu YCTN nội trú chưa có YCTN ngoại trú thì tạo mới YCTN ngoại trú với cùng mã tiếp nhận và ko cho phép nhập viện từ YCTN ngoại trú này
                        || (yeuCauKhamBenh.YeuCauTiepNhan.QuyetToanTheoNoiTru != null && yeuCauKhamBenh.YeuCauTiepNhan.QuyetToanTheoNoiTru == true)
                    )
                    {
                        throw new Exception(_localizationService.GetResource("YeuCauTiepNhanBase.DaCoYeuCauNhapVien"));
                    }
                    var yeuCauNhapVien = new YeuCauNhapVien
                    {
                        BenhNhanId = yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId.Value,
                        BacSiChiDinhId = _userAgentHelper.GetCurrentUserId(),
                        NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        ThoiDiemChiDinh = DateTime.Now,
                        YeuCauKhamBenhId = yeuCauKhamBenh.Id,
                        KhoaPhongNhapVienId = yeuCauKhamBenh.KhoaPhongNhapVienId.Value,
                        LyDoNhapVien = yeuCauKhamBenh.LyDoNhapVien,
                        LaCapCuu = false,
                        ChanDoanNhapVienICDId = yeuCauKhamBenh.IcdchinhId,
                        ChanDoanNhapVienGhiChu = yeuCauKhamBenh.GhiChuICDChinh
                    };

                    var yeuCauKhamBenhICDKhacs = BaseRepository.Context.Entry(yeuCauKhamBenh).Collection(o => o.YeuCauKhamBenhICDKhacs);
                    if (!yeuCauKhamBenhICDKhacs.IsLoaded)
                        yeuCauKhamBenhICDKhacs.Load();
                    foreach (var yeuCauKhamBenhIcdKhac in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
                    {
                        yeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos.Add(new YeuCauNhapVienChanDoanKemTheo { ICDId = yeuCauKhamBenhIcdKhac.ICDId, GhiChu = yeuCauKhamBenhIcdKhac.GhiChu });
                    }

                    BaseRepository.Context.Set<YeuCauNhapVien>().Add(yeuCauNhapVien);
                    await BaseRepository.Context.SaveChangesAsync();
                    //bỏ tự động cho nhập viện
                    //var chuaThuTienNgoaiTru = BaseRepository.TableNoTracking.Any(s => s.Id == yeuCauKhamBenh.YeuCauTiepNhanId
                    //        && (s.YeuCauKhamBenhs.Any(k => k.KhongTinhPhi != true && k.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                    //           || s.YeuCauDichVuKyThuats.Any(k => k.KhongTinhPhi != true && k.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                    //           || s.YeuCauDuocPhamBenhViens.Any(k => k.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                    //           || s.YeuCauVatTuBenhViens.Any(k => k.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                    //           || s.DonThuocThanhToans.Any(k => k.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && k.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && (k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || k.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))));

                    //if (chuaThuTienNgoaiTru)
                    //{
                    //    BaseRepository.Context.Set<YeuCauNhapVien>().Add(yeuCauNhapVien);
                    //    await BaseRepository.Context.SaveChangesAsync();
                    //}
                    //else
                    //{
                    //    var yeuCauTiepNhanTruoc = yeuCauKhamBenh.YeuCauTiepNhan;
                    //    var yeuCauTiepNhanNoiTru = GetYeuCauTiepNhanNoiTru(yeuCauTiepNhanTruoc, yeuCauNhapVien);
                    //    await BaseRepository.AddAsync(yeuCauTiepNhanNoiTru);
                    //}
                    return true;
                }
            }
            return false;
        }

        protected YeuCauTiepNhan GetYeuCauTiepNhanNoiTru(YeuCauTiepNhan yeuCauTiepNhanTruoc, YeuCauNhapVien yeuCauNhapVien)
        {
            var yeuCauTiepNhanNoiTru = new YeuCauTiepNhan
            {
                BenhNhanId = yeuCauTiepNhanTruoc.BenhNhanId,
                HoTen = yeuCauTiepNhanTruoc.HoTen,
                NgaySinh = yeuCauTiepNhanTruoc.NgaySinh,
                ThangSinh = yeuCauTiepNhanTruoc.ThangSinh,
                NamSinh = yeuCauTiepNhanTruoc.NamSinh,
                SoChungMinhThu = yeuCauTiepNhanTruoc.SoChungMinhThu,
                GioiTinh = yeuCauTiepNhanTruoc.GioiTinh,
                NhomMau = yeuCauTiepNhanTruoc.NhomMau,
                YeuToRh = yeuCauTiepNhanTruoc.YeuToRh,
                NgheNghiepId = yeuCauTiepNhanTruoc.NgheNghiepId,
                NoiLamViec = yeuCauTiepNhanTruoc.NoiLamViec,
                QuocTichId = yeuCauTiepNhanTruoc.QuocTichId,
                DanTocId = yeuCauTiepNhanTruoc.DanTocId,
                DiaChi = yeuCauTiepNhanTruoc.DiaChi,
                PhuongXaId = yeuCauTiepNhanTruoc.PhuongXaId,
                QuanHuyenId = yeuCauTiepNhanTruoc.QuanHuyenId,
                TinhThanhId = yeuCauTiepNhanTruoc.TinhThanhId,
                SoDienThoai = yeuCauTiepNhanTruoc.SoDienThoai,
                Email = yeuCauTiepNhanTruoc.Email,
                YeuCauNhapVien = yeuCauNhapVien,
                NoiGioiThieuId = yeuCauTiepNhanTruoc.NoiGioiThieuId,
                HinhThucDenId = yeuCauTiepNhanTruoc.HinhThucDenId,
                DuocUuDai = yeuCauTiepNhanTruoc.DuocUuDai,
                DoiTuongUuDaiId = yeuCauTiepNhanTruoc.DoiTuongUuDaiId,
                CongTyUuDaiId = yeuCauTiepNhanTruoc.CongTyUuDaiId,
                NguoiLienHeHoTen = yeuCauTiepNhanTruoc.NguoiLienHeHoTen,
                NguoiLienHeQuanHeNhanThanId = yeuCauTiepNhanTruoc.NguoiLienHeQuanHeNhanThanId,
                NguoiLienHeSoDienThoai = yeuCauTiepNhanTruoc.NguoiLienHeSoDienThoai,
                NguoiLienHeEmail = yeuCauTiepNhanTruoc.NguoiLienHeEmail,
                NguoiLienHeDiaChi = yeuCauTiepNhanTruoc.NguoiLienHeDiaChi,
                NguoiLienHePhuongXaId = yeuCauTiepNhanTruoc.NguoiLienHePhuongXaId,
                NguoiLienHeQuanHuyenId = yeuCauTiepNhanTruoc.NguoiLienHeQuanHuyenId,
                NguoiLienHeTinhThanhId = yeuCauTiepNhanTruoc.NguoiLienHeTinhThanhId,
                CoBHYT = yeuCauTiepNhanTruoc.CoBHYT,
                BHYTMaSoThe = yeuCauTiepNhanTruoc.BHYTMaSoThe,
                BHYTMucHuong = yeuCauTiepNhanTruoc.BHYTMucHuong,
                BHYTMaDKBD = yeuCauTiepNhanTruoc.BHYTMaDKBD,
                BHYTNgayHieuLuc = yeuCauTiepNhanTruoc.BHYTNgayHieuLuc,
                BHYTNgayHetHan = yeuCauTiepNhanTruoc.BHYTNgayHetHan,
                BHYTDiaChi = yeuCauTiepNhanTruoc.BHYTDiaChi,
                BHYTCoQuanBHXH = yeuCauTiepNhanTruoc.BHYTCoQuanBHXH,
                BHYTNgayDu5Nam = yeuCauTiepNhanTruoc.BHYTNgayDu5Nam,
                BHYTNgayDuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTNgayDuocMienCungChiTra,
                BHYTMaKhuVuc = yeuCauTiepNhanTruoc.BHYTMaKhuVuc,
                BHYTDuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTDuocMienCungChiTra,
                BHYTGiayMienCungChiTraId = yeuCauTiepNhanTruoc.BHYTGiayMienCungChiTraId,
                CoBHTN = yeuCauTiepNhanTruoc.CoBHTN,
                LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru,
                MaYeuCauTiepNhan = yeuCauTiepNhanTruoc.MaYeuCauTiepNhan,
                ThoiDiemTiepNhan = DateTime.Now,
                LyDoVaoVien = yeuCauTiepNhanTruoc.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen,
                TrieuChungTiepNhan = yeuCauTiepNhanTruoc.TrieuChungTiepNhan,
                LoaiTaiNan = yeuCauTiepNhanTruoc.LoaiTaiNan,
                DuocChuyenVien = yeuCauTiepNhanTruoc.DuocChuyenVien,
                GiayChuyenVienId = yeuCauTiepNhanTruoc.GiayChuyenVienId,
                ThoiGianChuyenVien = yeuCauTiepNhanTruoc.ThoiGianChuyenVien,
                NoiChuyenId = yeuCauTiepNhanTruoc.NoiChuyenId,
                SoChuyenTuyen = yeuCauTiepNhanTruoc.SoChuyenTuyen,
                TuyenChuyen = yeuCauTiepNhanTruoc.TuyenChuyen,
                LyDoChuyen = yeuCauTiepNhanTruoc.LyDoChuyen,
                DoiTuongUuTienKhamChuaBenhId = yeuCauTiepNhanTruoc.DoiTuongUuTienKhamChuaBenhId,
                TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien,
                ThoiDiemCapNhatTrangThai = DateTime.Now,
                TinhTrangThe = yeuCauTiepNhanTruoc.TinhTrangThe,
                IsCheckedBHYT = yeuCauTiepNhanTruoc.IsCheckedBHYT,
                TuNhap = yeuCauTiepNhanTruoc.TuNhap,
                NguoiGioiThieuId = yeuCauTiepNhanTruoc.NguoiGioiThieuId
            };

            if (yeuCauTiepNhanTruoc.CoBHYT == true)
            {
                yeuCauTiepNhanNoiTru.YeuCauTiepNhanTheBHYTs.Add(new YeuCauTiepNhanTheBHYT
                {
                    BenhNhanId = yeuCauTiepNhanTruoc.BenhNhanId.Value,
                    MaSoThe = yeuCauTiepNhanTruoc.BHYTMaSoThe,
                    MucHuong = yeuCauTiepNhanTruoc.BHYTMucHuong.GetValueOrDefault(),
                    MaDKBD = yeuCauTiepNhanTruoc.BHYTMaDKBD,
                    NgayHieuLuc = yeuCauTiepNhanTruoc.BHYTNgayHieuLuc.GetValueOrDefault(DateTime.Now),
                    NgayHetHan = yeuCauTiepNhanTruoc.BHYTNgayHetHan,
                    DiaChi = yeuCauTiepNhanTruoc.BHYTDiaChi,
                    CoQuanBHXH = yeuCauTiepNhanTruoc.BHYTCoQuanBHXH,
                    NgayDu5Nam = yeuCauTiepNhanTruoc.BHYTNgayDu5Nam,
                    NgayDuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTNgayDuocMienCungChiTra,
                    MaKhuVuc = yeuCauTiepNhanTruoc.BHYTMaKhuVuc,
                    DuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTDuocMienCungChiTra,
                    GiayMienCungChiTraId = yeuCauTiepNhanTruoc.BHYTGiayMienCungChiTraId,
                    IsCheckedBHYT = yeuCauTiepNhanTruoc.IsCheckedBHYT
                });
                yeuCauTiepNhanNoiTru.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong
                {
                    DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.BaoHiem,
                    MaSoThe = yeuCauTiepNhanTruoc.BHYTMaSoThe,
                    MucHuong = yeuCauTiepNhanTruoc.BHYTMucHuong.GetValueOrDefault(),
                    MaDKBD = yeuCauTiepNhanTruoc.BHYTMaDKBD,
                    NgayHieuLuc = yeuCauTiepNhanTruoc.BHYTNgayHieuLuc.GetValueOrDefault(DateTime.Now),
                    NgayHetHan = yeuCauTiepNhanTruoc.BHYTNgayHetHan,
                    DiaChi = yeuCauTiepNhanTruoc.BHYTDiaChi,
                    CoQuanBHXH = yeuCauTiepNhanTruoc.BHYTCoQuanBHXH,
                    NgayDu5Nam = yeuCauTiepNhanTruoc.BHYTNgayDu5Nam,
                    NgayDuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTNgayDuocMienCungChiTra,
                    MaKhuVuc = yeuCauTiepNhanTruoc.BHYTMaKhuVuc,
                    DuocMienCungChiTra = yeuCauTiepNhanTruoc.BHYTDuocMienCungChiTra,
                    GiayMienCungChiTraId = yeuCauTiepNhanTruoc.BHYTGiayMienCungChiTraId,
                    IsCheckedBHYT = yeuCauTiepNhanTruoc.IsCheckedBHYT
                });
            }
            else
            {
                yeuCauTiepNhanNoiTru.YeuCauTiepNhanLichSuChuyenDoiTuongs.Add(new YeuCauTiepNhanLichSuChuyenDoiTuong
                {
                    DoiTuongTiepNhan = Enums.DoiTuongTiepNhan.DichVu
                });
            }

            
            foreach (var yeuCauTiepNhanCongTyBaoHiemTuNhan in yeuCauTiepNhanTruoc.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                yeuCauTiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(new YeuCauTiepNhanCongTyBaoHiemTuNhan
                {
                    CongTyBaoHiemTuNhanId = yeuCauTiepNhanCongTyBaoHiemTuNhan.CongTyBaoHiemTuNhanId,
                    MaSoThe = yeuCauTiepNhanCongTyBaoHiemTuNhan.MaSoThe,
                    DiaChi = yeuCauTiepNhanCongTyBaoHiemTuNhan.DiaChi,
                    SoDienThoai = yeuCauTiepNhanCongTyBaoHiemTuNhan.SoDienThoai,
                    NgayHieuLuc = yeuCauTiepNhanCongTyBaoHiemTuNhan.NgayHieuLuc,
                    NgayHetHan = yeuCauTiepNhanCongTyBaoHiemTuNhan.NgayHetHan
                });
            }
            return yeuCauTiepNhanNoiTru;
        }

        public async Task XuLyHuyTatCaDichVuTruocKhiChuyenKhamAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId)
        {
            #region hủy dịch vụ khám hiện tại
            var yeuCauKhamCanHuy = yeuCauTiepNhan.YeuCauKhamBenhs.First(x => x.Id == yeuCauKhamBenhId);
            //yeuCauKhamCanHuy.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
            //XoaKhoiHangDoi(yeuCauKhamCanHuy);
            //yeuCauKhamCanHuy.WillDelete = true;

            //YeuCauGoiDichVu yeuCauGoiDichVu = null;
            //if (yeuCauKhamCanHuy.YeuCauGoiDichVuId != null)
            //{
            //    LoadGoiDichVuBenhNhan(yeuCauTiepNhan);
            //    yeuCauGoiDichVu = yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.First(o => o.Id == yeuCauKhamCanHuy.YeuCauGoiDichVuId);

            //    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauKhamBenhs);
            //    if (!yeuCauKhamBenhs.IsLoaded)
            //        yeuCauKhamBenhs.Load();

            //    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauDichVuKyThuats);
            //    if (!yeuCauDichVuKyThuats.IsLoaded)
            //        yeuCauDichVuKyThuats.Load();
            //}

            //yeuCauKhamCanHuy.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
            //XoaKhoiHangDoi(yeuCauKhamCanHuy);

            //if (yeuCauKhamCanHuy.NhanVienDuyetBaoHiemId != null)
            //{
            //    yeuCauKhamCanHuy.WillDelete = false;
            //}
            //yeuCauKhamCanHuy.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;

            //if (yeuCauGoiDichVu != null
            //    && yeuCauGoiDichVu.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan
            //    && yeuCauGoiDichVu.BoPhanMarketingDangKy == false
            //    && yeuCauGoiDichVu.YeuCauKhamBenhs.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //                                                && yeuCauGoiDichVu.YeuCauDichVuKyThuats.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            //{
            //    yeuCauGoiDichVu.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy;
            //    yeuCauGoiDichVu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
            //}
            #endregion

            #region hủy yêu cầu dịch vụ khám bệnh tiếp theo (con) nếu có
            foreach (var dichVuKham in yeuCauTiepNhan.YeuCauKhamBenhs.Where(x => (x.Id == yeuCauKhamBenhId || x.YeuCauKhamBenhTruocId == yeuCauKhamBenhId)
                                                                                 && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
            {
                //dichVuKham.WillDelete = true;

                YeuCauGoiDichVu yeuCauGoiDichVu = null;
                if (dichVuKham.YeuCauGoiDichVuId != null)
                {
                    LoadGoiDichVuBenhNhan(yeuCauTiepNhan);
                    yeuCauGoiDichVu =
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.FirstOrDefault(o => o.Id == dichVuKham.YeuCauGoiDichVuId) ??
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.FirstOrDefault(o => o.Id == dichVuKham.YeuCauGoiDichVuId);

                    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauKhamBenhs);
                    if (!yeuCauKhamBenhs.IsLoaded)
                        yeuCauKhamBenhs.Load();

                    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVu).Collection(o => o.YeuCauDichVuKyThuats);
                    if (!yeuCauDichVuKyThuats.IsLoaded)
                        yeuCauDichVuKyThuats.Load();
                }

                dichVuKham.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;
                dichVuKham.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                XoaKhoiHangDoi(dichVuKham);

                //if (dichVuKham.NhanVienDuyetBaoHiemId != null)
                //{
                //    dichVuKham.WillDelete = false;
                //}

                if (yeuCauGoiDichVu != null
                    && yeuCauGoiDichVu.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan
                    && yeuCauGoiDichVu.BoPhanMarketingDangKy == false
                    && yeuCauGoiDichVu.YeuCauKhamBenhs.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    && yeuCauGoiDichVu.YeuCauDichVuKyThuats.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    yeuCauGoiDichVu.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy;
                    yeuCauGoiDichVu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                }

                if (dichVuKham.WillDelete)
                {
                    foreach (var lichSu in dichVuKham.YeuCauKhamBenhLichSuTrangThais)
                    {
                        lichSu.WillDelete = true;
                    }
                }
            }
            #endregion

            #region hủy yêu cầu dịch vụ kỹ thuật nếu có
            foreach (var yeuCauDichVuKyThuat in yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                yeuCauDichVuKyThuat.WillDelete = true;


                YeuCauGoiDichVu yeuCauGoiDichVuKyThuat = null;
                if (yeuCauDichVuKyThuat.YeuCauGoiDichVuId != null)
                {
                    LoadGoiDichVuBenhNhan(yeuCauTiepNhan);
                    yeuCauGoiDichVuKyThuat =
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVus.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId) ??
                        yeuCauTiepNhan.BenhNhan.YeuCauGoiDichVuSoSinhs.FirstOrDefault(o => o.Id == yeuCauDichVuKyThuat.YeuCauGoiDichVuId);

                    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yeuCauGoiDichVuKyThuat).Collection(o => o.YeuCauKhamBenhs);
                    if (!yeuCauKhamBenhs.IsLoaded)
                        yeuCauKhamBenhs.Load();

                    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yeuCauGoiDichVuKyThuat).Collection(o => o.YeuCauDichVuKyThuats);
                    if (!yeuCauDichVuKyThuats.IsLoaded)
                        yeuCauDichVuKyThuats.Load();
                }

                yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                XoaKhoiHangDoi(yeuCauDichVuKyThuat);
                
                if (yeuCauDichVuKyThuat.NhanVienDuyetBaoHiemId != null || !yeuCauDichVuKyThuat.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauDichVuKyThuat.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    yeuCauDichVuKyThuat.WillDelete = false;
                }
                var tuongTrinhPTTT = BaseRepository.Context.Entry(yeuCauDichVuKyThuat).Reference(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT);
                if (!tuongTrinhPTTT.IsLoaded) tuongTrinhPTTT.Load();
                if (yeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT != null)
                {
                    yeuCauDichVuKyThuat.WillDelete = false;
                }

                if (yeuCauGoiDichVuKyThuat != null && yeuCauGoiDichVuKyThuat.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan
                                            && yeuCauGoiDichVuKyThuat.BoPhanMarketingDangKy == false
                                            && yeuCauGoiDichVuKyThuat.YeuCauKhamBenhs.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                            && yeuCauGoiDichVuKyThuat.YeuCauDichVuKyThuats.All(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
                {
                    yeuCauGoiDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy;
                    yeuCauGoiDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                }
            }
            #endregion

            #region hủy yêu cầu dược nếu có
            foreach (var yeuCauDuocPham in yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                                                                            && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
            {
                if (yeuCauDuocPham.XuatKhoDuocPhamChiTiet != null)
                {
                    foreach (var thongTinXuat in yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
                    {
                        thongTinXuat.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                    }
                }

                yeuCauDuocPham.WillDelete = true;

                yeuCauDuocPham.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                if (yeuCauDuocPham.NhanVienDuyetBaoHiemId != null || !yeuCauDuocPham.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauDuocPham.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    yeuCauDuocPham.WillDelete = false;
                }
                yeuCauDuocPham.TrangThai = Enums.EnumYeuCauDuocPhamBenhVien.DaHuy;
            }
            #endregion

            #region hủy yêu cầu vật tư nếu có
            foreach (var yeuCauVatTu in yeuCauTiepNhan.YeuCauVatTuBenhViens.Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                                                                             && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
            {
                if (yeuCauVatTu.XuatKhoVatTuChiTiet != null)
                {
                    foreach (var thongTinXuat in yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris)
                    {
                        thongTinXuat.NhapKhoVatTuChiTiet.SoLuongDaXuat -= thongTinXuat.SoLuongXuat;
                    }
                }
                yeuCauVatTu.WillDelete = true;

                yeuCauVatTu.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;
                if (yeuCauVatTu.NhanVienDuyetBaoHiemId != null || !yeuCauVatTu.SoTienMienGiam.GetValueOrDefault(0).Equals(0) || !yeuCauVatTu.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(0).Equals(0))
                {
                    yeuCauVatTu.WillDelete = false;
                }
                yeuCauVatTu.TrangThai = Enums.EnumYeuCauVatTuBenhVien.DaHuy;
            }
            #endregion

            #region hủy dơn thuốc, vật tư thanh toán
            XoaDonThuocTheoYeuCauKhamBenh(yeuCauTiepNhan, yeuCauKhamCanHuy);

            #endregion

            //await PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
            //await UpdateAsync(yeuCauTiepNhan);
            BaseRepository.Context.SaveChanges();
        }


        private void XoaDonThuocTheoYeuCauKhamBenh(YeuCauTiepNhan yeuCauTiepNhan, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKhamBenh)
        {
            //var yeuCauKhamBenh = _yeuCauKhamBenhRepository.GetById(yeuCauKhamBenhId,
            //    x => x.Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
            //        .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
            //        .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets)
            //        .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans)
            //        .Include(o => o.YeuCauTiepNhan).ThenInclude(o => o.YeuCauKhamBenhs)
            //        .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuKyThuats)
            //        .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDichVuGiuongBenhViens)
            //        .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauDuocPhamBenhViens)
            //        .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.YeuCauVatTuBenhViens)
            //        .Include(dt => dt.YeuCauTiepNhan).ThenInclude(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
            //        .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
            //        .Include(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
            //        .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
            //        .Include(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
            //        );
            

    //        var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
    //x => x.Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.DonThuocThanhToans)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.DonVTYTThanhToans)
    //            .Include(o => o.YeuCauKhamBenhs)
    //            .Include(o => o.YeuCauDichVuKyThuats)
    //            .Include(o => o.YeuCauDichVuGiuongBenhViens)
    //            .Include(o => o.YeuCauDuocPhamBenhViens)
    //            .Include(o => o.YeuCauVatTuBenhViens)
    //            .Include(o => o.DonThuocThanhToans).ThenInclude(o => o.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuyetBaoHiemChiTiets)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonThuocs).ThenInclude(dt => dt.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(dt => dt.DonThuocThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.XuatKhoVatTuChiTiet).ThenInclude(o => o.XuatKhoVatTu)
    //            .Include(a => a.YeuCauKhamBenhs).ThenInclude(o => o.YeuCauKhamBenhDonVTYTs).ThenInclude(dt => dt.YeuCauKhamBenhDonVTYTChiTiets).ThenInclude(dt => dt.DonVTYTThanhToanChiTiets).ThenInclude(dt => dt.XuatKhoVatTuChiTietViTri).ThenInclude(o => o.NhapKhoVatTuChiTiet)
    //            );

            //var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.First(x => x.Id == yeuCauKhamBenhId);

            if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any(o => o.DonVTYTThanhToans.Any(tt => tt.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)))
            {
                //return "Có đơn vật tư đã được thanh toán";
                throw new Exception(_localizationService.GetResource("DonVTYT.VTYTDaThanhToan"));
            }
            foreach (var yeuCauKhamBenhDonVTYT in yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs)
            {
                foreach (var yeuCauKhamBenhDonVTYTChiTiet in yeuCauKhamBenhDonVTYT.YeuCauKhamBenhDonVTYTChiTiets)
                {
                    foreach (var donVTYTThanhToanChiTiet in yeuCauKhamBenhDonVTYTChiTiet.DonVTYTThanhToanChiTiets)
                    {
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.NhapKhoVatTuChiTiet.SoLuongDaXuat -= donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.SoLuongXuat;
                        donVTYTThanhToanChiTiet.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.WillDelete = true;
                        donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.WillDelete = true;
                        if (donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null)
                            donVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.WillDelete = true;
                    }
                    yeuCauKhamBenhDonVTYTChiTiet.WillDelete = true;
                }
                foreach (var donVTYTThanhToan in yeuCauKhamBenhDonVTYT.DonVTYTThanhToans)
                {
                    donVTYTThanhToan.WillDelete = true;
                }
                yeuCauKhamBenhDonVTYT.WillDelete = true;
            }

            if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any(o => o.DonThuocThanhToans.Any(tt => tt.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan)))
            {
                //return "Có đơn thuốc đã được thanh toán";
                throw new Exception(_localizationService.GetResource("DonThuoc.ThuocDaThanhToan"));
            }

            //var duocHuongBaoHiem = false;
            //if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.SelectMany(o => o.YeuCauKhamBenhDonThuocChiTiets).SelectMany(o => o.DonThuocThanhToanChiTiets).Any(o => o.BaoHiemChiTra != false))
            //{
            //    duocHuongBaoHiem = true;
            //}

            foreach (var yeuCauKhamBenhDonThuoc in yeuCauKhamBenh.YeuCauKhamBenhDonThuocs)
            {
                foreach (var yeuCauKhamBenhDonThuocChiTiet in yeuCauKhamBenhDonThuoc.YeuCauKhamBenhDonThuocChiTiets)
                {
                    foreach (var donThuocThanhToanChiTiet in yeuCauKhamBenhDonThuocChiTiet.DonThuocThanhToanChiTiets)
                    {
                        foreach (var duyetBaoHiemChiTiet in donThuocThanhToanChiTiet.DuyetBaoHiemChiTiets)
                        {
                            duyetBaoHiemChiTiet.WillDelete = true;
                        }
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                        donThuocThanhToanChiTiet.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
                        donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                        if (donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
                            donThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                    }
                    yeuCauKhamBenhDonThuocChiTiet.WillDelete = true;
                }
                foreach (var donThuocThanhToan in yeuCauKhamBenhDonThuoc.DonThuocThanhToans)
                {
                    donThuocThanhToan.WillDelete = true;
                }
                yeuCauKhamBenhDonThuoc.WillDelete = true;
            }
            yeuCauKhamBenh.CoKeToa = null;
            //bo duyet tu dong
            //if (duocHuongBaoHiem)
            //{
            //    DuyetBHYTChoDonThuoc(yeuCauTiepNhan, (long)Enums.NhanVienHeThong.NhanVienDuyetBHYTTuDong, (long)Enums.PhongHeThong.PhongDuyetBHYTToanTuDong);
            //}
            //await BaseRepository.UpdateAsync(yeuCauTiepNhan);
        }
        //private void DuyetBHYTChoDonThuoc(YeuCauTiepNhan ycTiepNhan, long nguoiDuyetId, long noiDuyetId)
        //{
        //    var soTienBHYTSeThanhToanToanBo = _cauHinhService.SoTienBHYTSeThanhToanToanBo().Result;
        //    var soTienTheoMucHuong100 = GetSoTienBhytSeDuyetTheoMucHuong(ycTiepNhan, 100);
        //    var mucHuongHienTai = soTienTheoMucHuong100 <= soTienBHYTSeThanhToanToanBo ? 100 : ycTiepNhan.BHYTMucHuong.GetValueOrDefault();

        //    //xac nhan
        //    var duyetBaoHiem = new DuyetBaoHiem
        //    {
        //        NhanVienDuyetBaoHiemId = nguoiDuyetId,
        //        ThoiDiemDuyetBaoHiem = DateTime.Now,
        //        NoiDuyetBaoHiemId = noiDuyetId
        //    };

        //    var dsDichVuKhamBenh = ycTiepNhan.YeuCauKhamBenhs
        //        .Where(p => p.CreatedOn != null && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true &&
        //                    p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList();
        //    foreach (var yeuCauKhamBenh in dsDichVuKhamBenh)
        //    {
        //        if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && yeuCauKhamBenh.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yeuCauKhamBenh.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            yeuCauKhamBenh.MucHuongBaoHiem = mucHuongHienTai;
        //            yeuCauKhamBenh.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            yeuCauKhamBenh.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauKhamBenh = yeuCauKhamBenh,
        //                SoLuong = 1,
        //                TiLeBaoHiemThanhToan = yeuCauKhamBenh.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = yeuCauKhamBenh.MucHuongBaoHiem,
        //                DonGiaBaoHiem = yeuCauKhamBenh.DonGiaBaoHiem
        //            });
        //            if (yeuCauKhamBenh.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }
        //    foreach (var yckt in ycTiepNhan.YeuCauDichVuKyThuats
        //        .Where(p => p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
        //    {
        //        if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && yckt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && yckt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            yckt.MucHuongBaoHiem = mucHuongHienTai;
        //            yckt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            yckt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauDichVuKyThuat = yckt,
        //                SoLuong = yckt.SoLan,
        //                TiLeBaoHiemThanhToan = yckt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = yckt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = yckt.DonGiaBaoHiem
        //            });
        //            if (yckt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                yckt.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdp in ycTiepNhan.YeuCauDuocPhamBenhViens
        //                .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy))
        //    {
        //        if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && ycdp.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdp.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycdp.MucHuongBaoHiem = mucHuongHienTai;
        //            ycdp.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycdp.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauDuocPhamBenhVien = ycdp,
        //                SoLuong = ycdp.SoLuong,
        //                TiLeBaoHiemThanhToan = ycdp.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycdp.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycdp.DonGiaBaoHiem
        //            });
        //            if (ycdp.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdp.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycvt in ycTiepNhan.YeuCauVatTuBenhViens
        //        .Where(p => p.KhongTinhPhi != true && p.DuocHuongBaoHiem && p.BaoHiemChiTra == true && p.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy))
        //    {
        //        if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if ((mucHuongHienTai == 100 && ycvt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycvt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycvt.MucHuongBaoHiem = mucHuongHienTai;
        //            ycvt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycvt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                YeuCauVatTuBenhVien = ycvt,
        //                SoLuong = ycvt.SoLuong,
        //                TiLeBaoHiemThanhToan = ycvt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycvt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycvt.DonGiaBaoHiem
        //            });
        //            if (ycvt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycvt.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    foreach (var ycdt in ycTiepNhan.DonThuocThanhToans.Where(o => o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
        //                .SelectMany(w => w.DonThuocThanhToanChiTiets)
        //                .Where(p => !p.WillDelete && p.DuocHuongBaoHiem && p.BaoHiemChiTra != false))
        //    {
        //        if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //        {
        //            continue;
        //        }
        //        if (ycdt.BaoHiemChiTra == null || (mucHuongHienTai == 100 && ycdt.MucHuongBaoHiem != 100) || (mucHuongHienTai == ycTiepNhan.BHYTMucHuong.GetValueOrDefault() && ycdt.MucHuongBaoHiem > mucHuongHienTai))
        //        {
        //            ycdt.BaoHiemChiTra = true;
        //            ycdt.MucHuongBaoHiem = mucHuongHienTai;
        //            ycdt.ThoiDiemDuyetBaoHiem = DateTime.Now;
        //            ycdt.NhanVienDuyetBaoHiemId = nguoiDuyetId;

        //            duyetBaoHiem.DuyetBaoHiemChiTiets.Add(new DuyetBaoHiemChiTiet
        //            {
        //                DonThuocThanhToanChiTiet = ycdt,
        //                SoLuong = ycdt.SoLuong,
        //                TiLeBaoHiemThanhToan = ycdt.TiLeBaoHiemThanhToan,
        //                MucHuongBaoHiem = ycdt.MucHuongBaoHiem,
        //                DonGiaBaoHiem = ycdt.DonGiaBaoHiem
        //            });
        //            if (ycdt.DonThuocThanhToan?.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
        //            {
        //                ycdt.DonThuocThanhToan.TrangThaiThanhToan = Enums.TrangThaiThanhToan.CapNhatThanhToan;
        //            }
        //        }
        //    }

        //    if (duyetBaoHiem.DuyetBaoHiemChiTiets.Any())
        //    {
        //        ycTiepNhan.DuyetBaoHiems.Add(duyetBaoHiem);
        //    }
        //}

        public async Task<List<ChiPhiKhamChuaBenhNoiTruVo>> GetDanhSachChiPhiNoiTruChuaThu(long yeuCauTiepNhanId)
        {
            //bo Async 3/11
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.CongTyUuDai)
                    //.Include(o => o.YeuCauKhamBenhs)
                    //.Include(o => o.YeuCauDichVuKyThuats)
                    .Include(o => o.NoiTruBenhAn)
                    .Include(o => o.YeuCauTiepNhanTheBHYTs)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                    .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                    .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                    .Include(o => o.GiayMienGiamThem)
                    .Include(o => o.NhanVienDuyetMienGiamThem).ThenInclude(v => v.User)
                    .Include(o => o.TheVoucherYeuCauTiepNhans).ThenInclude(v => v.TheVoucher).ThenInclude(v => v.Voucher).ThenInclude(v => v.VoucherChiTietMienGiams));

            long? yeuCauTiepNhanNgoaiTruCanQuyetToanId = yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            YeuCauTiepNhan yeuCauTiepNhanNgoaiTruCanQuyetToan = null;
            if (yeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                yeuCauTiepNhanNgoaiTruCanQuyetToan = BaseRepository.GetById(yeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
                    x => x.Include(o => o.CongTyUuDai)
                        //.Include(o => o.YeuCauKhamBenhs)
                        //.Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                        .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                        .Include(o => o.GiayMienGiamThem)
                        .Include(o => o.NhanVienDuyetMienGiamThem).ThenInclude(v => v.User)
                        .Include(o => o.TheVoucherYeuCauTiepNhans).ThenInclude(v => v.TheVoucher)
                        .ThenInclude(v => v.Voucher).ThenInclude(v => v.VoucherChiTietMienGiams));
            }


            var queryDichVuKhamBenh = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauKhamBenhs).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis).Include(cc => cc.NoiDangKy).Include(cc => cc.BacSiDangKy).ThenInclude(cc => cc.User)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    DichVuBenhVienId = s.DichVuKhamBenhBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuKhamBenh,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuKhamBenh.GetDescription(),
                    NhomChiPhiBangKe = NhomChiPhiBangKe.KhamBenh,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    Soluong = 1,
                    DonGia = s.Gia,
                    LoaiGiaId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos = s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //    .Select(o=> new DanhSachMienGiamVo
                    //    {
                    //        LoaiMienGiam = o.LoaiMienGiam,
                    //        LoaiChietKhau = o.LoaiChietKhau,
                    //        TheVoucherId = o.TheVoucherId,
                    //        MaTheVoucher = o.MaTheVoucher,
                    //        DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //        NoiGioiThieuId = o.NoiGioiThieuId,
                    //        TiLe = o.TiLe,
                    //        SoTien = o.SoTien
                    //    }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,

                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    CheckedDefault = true,
                }).OrderBy(o => o.Id).ToListAsync();

            var queryDichVuKyThuat = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauDichVuKyThuats).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    DichVuBenhVienId = s.DichVuKyThuatBenhVienId,
                    NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuKyThuat,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuKyThuat.GetDescription(),
                    NhomChiPhiBangKe = s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? NhomChiPhiBangKe.XetNgiem 
                                        : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh ? NhomChiPhiBangKe.ChuanDoanHinhAnh
                                        : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ? NhomChiPhiBangKe.ThamDoChucNang
                                        : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? NhomChiPhiBangKe.ThuThuatPhauThuat
                                        : NhomChiPhiBangKe.DichVuKhac,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    Soluong = s.SoLan,
                    DonGia = s.Gia,
                    LoaiGiaId = s.NhomGiaDichVuKyThuatBenhVienId,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    CheckedDefault = true,
                }).ToListAsync();

            var queryDichVuTruyenMau = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauTruyenMaus).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    DichVuBenhVienId = s.MauVaChePhamId,
                    LoaiNhom = NhomChiPhiNoiTru.TruyenMau,
                    Ma = s.MaDichVu,
                    Nhom = NhomChiPhiNoiTru.TruyenMau.GetDescription(),
                    NhomChiPhiBangKe = NhomChiPhiBangKe.Mau,
                    TenDichVu = s.TenDichVu,
                    LoaiGia = string.Empty,
                   
                    Soluong = 1,
                    DonGia = s.DonGiaBan.GetValueOrDefault(),
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    CheckedDefault = true,
                }).ToListAsync();

            var queryDichVuGiuongChiPhiBenhVien = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.YeuCauGoiDichVuId == null && (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    Ma = s.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    NhomChiPhiBangKe = NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru,
                    TenDichVu = s.Ten,
                    LoaiGia = s.NhomGiaDichVuGiuongBenhVien.Ten,
                    LoaiGiaId = s.NhomGiaDichVuGiuongBenhVienId,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    //KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    //DuocHuongBHYT = s.DuocHuongBaoHiem,
                    //DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    //TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    CheckedDefault = true
                }).ToListAsync();
            var queryDichVuGiuongChiPhiBHYT = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ToListAsync();

            var queryDuocPham = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauDuocPhamBenhViens).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    DichVuBenhVienId = s.DuocPhamBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DuocPham,
                    Nhom = NhomChiPhiNoiTru.DuocPham.GetDescription(),
                    NhomChiPhiBangKe = NhomChiPhiBangKe.ThuocDichTruyen,
                    Ma = s.DuocPhamBenhVien.Ma,
                    TenDichVu = s.DuocPhamBenhVien.DuocPham.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.NoiCapThuoc != null ? s.NoiCapThuoc.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    CheckedDefault = true,
                }).ToListAsync();

            var queryVatTu = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauVatTuBenhViens).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    DichVuBenhVienId = s.VatTuBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.VatTuTieuHao,
                    Nhom = NhomChiPhiNoiTru.VatTuTieuHao.GetDescription(),
                    NhomChiPhiBangKe = s.YeuCauDichVuKyThuat != null && s.YeuCauDichVuKyThuat.LanThucHien != null ? NhomChiPhiBangKe.GoiVatTu : NhomChiPhiBangKe.VatTuYte,
                    SoThuTuGoiTrongNhomChiPhiBangKe = s.YeuCauDichVuKyThuat != null && s.YeuCauDichVuKyThuat.LanThucHien != null ? s.YeuCauDichVuKyThuat.LanThucHien.Value : 0,
                    Ma = s.VatTuBenhVien.Ma,
                    TenDichVu = s.VatTuBenhVien.VatTus.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.NoiCapVatTu != null ? s.NoiCapVatTu.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    CheckedDefault = true,
                }).ToListAsync();

            var queryToaThuoc = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.DonThuocThanhToans)
                .Where(dt =>
                    dt.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && dt.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy &&
                    (dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                     dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .SelectMany(o => o.DonThuocThanhToanChiTiets).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.DonThuocThanhToan.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.CreatedOn,
                    DichVuBenhVienId = s.DuocPhamId,
                    LoaiNhom = NhomChiPhiNoiTru.ToaThuoc,
                    Nhom = $"{NhomChiPhiNoiTru.ToaThuoc.GetDescription()}: {s.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.TenDichVu}",
                    NhomChiPhiBangKe = NhomChiPhiBangKe.ThuocDichTruyen,
                    Ma = string.Empty,
                    TenDichVu = s.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = "Nhà thuốc",
                    CheckedDefault = true,
                }).ToListAsync();

            await Task.WhenAll(queryDichVuKhamBenh, queryDichVuKyThuat, queryDichVuTruyenMau, queryDuocPham, queryVatTu, queryDichVuGiuongChiPhiBenhVien, queryDichVuGiuongChiPhiBHYT,
                queryToaThuoc);

            var ycKhamBenhIds = queryDichVuKhamBenh.Result.Select(o=>o.Id).ToList();
            var ycKyThuatIds = queryDichVuKyThuat.Result.Select(o => o.Id).ToList();
            var ycTruyenMauIds = queryDichVuTruyenMau.Result.Select(o => o.Id).ToList();
            var ycDuocPhamIds = queryDuocPham.Result.Select(o => o.Id).ToList();
            var ycVatTuIds = queryVatTu.Result.Select(o => o.Id).ToList();
            var ycGiuongIds = queryDichVuGiuongChiPhiBenhVien.Result.Select(o => o.Id).ToList();
            var ycToaThuocIds = queryToaThuoc.Result.Select(o => o.Id).ToList();

            var congTyBaoHiemTuNhanCongNos = BaseRepository.Context.Set<CongTyBaoHiemTuNhanCongNo>().AsNoTracking()
                .Where(o=> o.TaiKhoanBenhNhanThuId == null 
                            && ((o.YeuCauKhamBenhId != null && ycKhamBenhIds.Contains(o.YeuCauKhamBenhId.Value))
                                || (o.YeuCauDichVuKyThuatId != null && ycKyThuatIds.Contains(o.YeuCauDichVuKyThuatId.Value))
                                || (o.YeuCauTruyenMauId != null && ycTruyenMauIds.Contains(o.YeuCauTruyenMauId.Value))
                                || (o.YeuCauDuocPhamBenhVienId != null && ycDuocPhamIds.Contains(o.YeuCauDuocPhamBenhVienId.Value))
                                || (o.YeuCauVatTuBenhVienId != null && ycVatTuIds.Contains(o.YeuCauVatTuBenhVienId.Value))
                                || (o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null && ycGiuongIds.Contains(o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value))
                                || (o.DonThuocThanhToanChiTietId != null && ycToaThuocIds.Contains(o.DonThuocThanhToanChiTietId.Value))))
                .Select(o => new
                {
                    o.Id,
                    o.YeuCauKhamBenhId,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauTruyenMauId,
                    o.YeuCauDuocPhamBenhVienId,
                    o.YeuCauVatTuBenhVienId,
                    o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                    o.DonThuocThanhToanChiTietId,
                    o.CongTyBaoHiemTuNhanId,
                    o.SoTien
                })
                .ToList();

            var mienGiamChiPhis = BaseRepository.Context.Set<MienGiamChiPhi>().AsNoTracking()
                .Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0
                            && ((o.YeuCauKhamBenhId != null && ycKhamBenhIds.Contains(o.YeuCauKhamBenhId.Value))
                                || (o.YeuCauDichVuKyThuatId != null && ycKyThuatIds.Contains(o.YeuCauDichVuKyThuatId.Value))
                                || (o.YeuCauTruyenMauId != null && ycTruyenMauIds.Contains(o.YeuCauTruyenMauId.Value))
                                || (o.YeuCauDuocPhamBenhVienId != null && ycDuocPhamIds.Contains(o.YeuCauDuocPhamBenhVienId.Value))
                                || (o.YeuCauVatTuBenhVienId != null && ycVatTuIds.Contains(o.YeuCauVatTuBenhVienId.Value))
                                || (o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null && ycGiuongIds.Contains(o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId.Value))
                                || (o.DonThuocThanhToanChiTietId != null && ycToaThuocIds.Contains(o.DonThuocThanhToanChiTietId.Value))))
                .Select(o => new
                {
                    o.Id,
                    o.YeuCauKhamBenhId,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauTruyenMauId,
                    o.YeuCauDuocPhamBenhVienId,
                    o.YeuCauVatTuBenhVienId,
                    o.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId,
                    o.DonThuocThanhToanChiTietId,
                    o.LoaiMienGiam,
                    o.LoaiChietKhau,
                    o.TheVoucherId,
                    o.MaTheVoucher,
                    o.DoiTuongUuDaiId,
                    o.NoiGioiThieuId,
                    o.TiLe,
                    o.SoTien
                })
                .ToList();

            queryDichVuKhamBenh.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauKhamBenhId != null && cn.YeuCauKhamBenhId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauKhamBenhId != null && mg.YeuCauKhamBenhId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryDichVuKyThuat.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauDichVuKyThuatId != null && cn.YeuCauDichVuKyThuatId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauDichVuKyThuatId != null && mg.YeuCauDichVuKyThuatId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryDichVuTruyenMau.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauTruyenMauId != null && cn.YeuCauTruyenMauId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauTruyenMauId != null && mg.YeuCauTruyenMauId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryDuocPham.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauDuocPhamBenhVienId != null && cn.YeuCauDuocPhamBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauDuocPhamBenhVienId != null && mg.YeuCauDuocPhamBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryVatTu.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauVatTuBenhVienId != null && cn.YeuCauVatTuBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauVatTuBenhVienId != null && mg.YeuCauVatTuBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryDichVuGiuongChiPhiBenhVien.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null && cn.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId != null && mg.YeuCauDichVuGiuongBenhVienChiPhiBenhVienId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });
            queryToaThuoc.Result
                .ForEach(o => {
                    o.DanhSachCongNoChoThus = congTyBaoHiemTuNhanCongNos
                                                 .Where(cn => cn.DonThuocThanhToanChiTietId != null && cn.DonThuocThanhToanChiTietId == o.Id)
                                                 .Select(cn => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = cn.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = cn.SoTien })
                                                 .ToList();
                    o.DanhSachMienGiamVos = mienGiamChiPhis
                                                 .Where(mg => mg.DonThuocThanhToanChiTietId != null && mg.DonThuocThanhToanChiTietId == o.Id)
                                                 .Select(cn => new DanhSachMienGiamVo
                                                 {
                                                     LoaiMienGiam = cn.LoaiMienGiam,
                                                     LoaiChietKhau = cn.LoaiChietKhau,
                                                     TheVoucherId = cn.TheVoucherId,
                                                     MaTheVoucher = cn.MaTheVoucher,
                                                     DoiTuongUuDaiId = cn.DoiTuongUuDaiId,
                                                     NoiGioiThieuId = cn.NoiGioiThieuId,
                                                     TiLe = cn.TiLe,
                                                     SoTien = cn.SoTien
                                                 })
                                                 .ToList();
                });

            var result = new List<ChiPhiKhamChuaBenhNoiTruVo>();
            result.AddRange(queryDichVuKhamBenh.Result);
            result.AddRange(queryDichVuKyThuat.Result);
            result.AddRange(queryDichVuTruyenMau.Result);
            result.AddRange(queryDuocPham.Result);
            result.AddRange(queryVatTu.Result);
            result.AddRange(queryToaThuoc.Result);

            List<YeuCauDichVuGiuongBenhVienChiPhiBHYT> dichVuGiuongBenhVienChiPhiBhyts;
            List<ChiPhiKhamChuaBenhNoiTruVo> dichVuGiuongs;
            if (yeuCauTiepNhan.NoiTruBenhAn != null &&  yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
            {
                var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                dichVuGiuongBenhVienChiPhiBhyts = chiPhiGiuong.Item2;
                dichVuGiuongs = chiPhiGiuong.Item1.Where(o=>o.YeuCauGoiDichVuId == null).Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    Ma = s.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    NhomChiPhiBangKe = NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru,
                    TenDichVu = s.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    DuocHuongBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(),
                    KiemTraBHYTXacNhan = false,
                    DonGiaBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.DonGiaBaoHiem.GetValueOrDefault() ?? 0,
                    TiLeBaoHiemThanhToan = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.TiLeBaoHiemThanhToan.GetValueOrDefault() ?? 0,
                    MucHuongBaoHiem = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.MucHuongBaoHiem.GetValueOrDefault() ?? 0,
                    MaSoTheBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.MaSoTheBHYT ?? string.Empty,
                    DanhSachCongNoChoThus = (s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                    DanhSachMienGiamVos = (s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachMienGiamVo>() :
                        s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                            (k, v) => new DanhSachMienGiamVo
                            {
                                LoaiMienGiam = k.LoaiMienGiam,
                                LoaiChietKhau = k.LoaiChietKhau,
                                TheVoucherId = k.TheVoucherId,
                                MaTheVoucher = k.MaTheVoucher,
                                DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                                TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                                SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                            }).Where(o => o.SoTien != 0),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    CheckedDefault = true
                }).ToList();
            }
            else
            {
                dichVuGiuongBenhVienChiPhiBhyts = queryDichVuGiuongChiPhiBHYT.Result;
                dichVuGiuongs = queryDichVuGiuongChiPhiBenhVien.Result;
                foreach (var chiPhiKhamChuaBenhNoiTruVo in dichVuGiuongs.Where(o => o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan))
                {
                    var chiPhiGiuongBHYT = dichVuGiuongBenhVienChiPhiBhyts.FirstOrDefault(o => o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId == chiPhiKhamChuaBenhNoiTruVo.Id);
                    if (chiPhiGiuongBHYT != null)
                    {
                        chiPhiKhamChuaBenhNoiTruVo.DuocHuongBHYT = true;
                        chiPhiKhamChuaBenhNoiTruVo.KiemTraBHYTXacNhan = chiPhiGiuongBHYT.BaoHiemChiTra != null;
                        chiPhiKhamChuaBenhNoiTruVo.DonGiaBHYT = chiPhiGiuongBHYT.DonGiaBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.TiLeBaoHiemThanhToan = chiPhiGiuongBHYT.TiLeBaoHiemThanhToan.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.MucHuongBaoHiem = chiPhiGiuongBHYT.MucHuongBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.MaSoTheBHYT = chiPhiGiuongBHYT.MaSoTheBHYT ?? string.Empty;
                        chiPhiKhamChuaBenhNoiTruVo.YeuCauDichVuGiuongBenhVienChiPhiBHYTIds = new List<long>{chiPhiGiuongBHYT.Id};
                    }
                }
            }

            result.AddRange(dichVuGiuongs);

            //set muc huong BHYT
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                foreach (var chiPhiKhamChuaBenhVo in result.Where(o=>o.DuocHuongBHYT && o.KiemTraBHYTXacNhan == false))
                {
                    var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs
                        .Where(o =>o.NgayHieuLuc.Date <= chiPhiKhamChuaBenhVo.NgayPhatSinh.GetValueOrDefault().Date && 
                                    (o.NgayHetHan == null || o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date >= chiPhiKhamChuaBenhVo.NgayPhatSinh.GetValueOrDefault().Date))
                        .OrderByDescending(o => o.MucHuong).FirstOrDefault();
                    if (theBHYT != null)
                    {
                        chiPhiKhamChuaBenhVo.MucHuongBaoHiem = theBHYT.MucHuong;
                        chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan = chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan != 0 ? chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan : 100;
                    }
                }
            }

            foreach (var chiPhiKhamChuaBenhVo in result)
            {
                if (chiPhiKhamChuaBenhVo.CapNhatThanhToan)
                {
                    if (chiPhiKhamChuaBenhVo.TongCongNo > chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan)
                    {
                        var maxCongNo = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;
                        foreach (var danhSachCongNoChoThu in chiPhiKhamChuaBenhVo.DanhSachCongNoChoThus)
                        {
                            if (chiPhiKhamChuaBenhVo.TongCongNo > maxCongNo)
                            {
                                var giam = chiPhiKhamChuaBenhVo.TongCongNo - maxCongNo;
                                danhSachCongNoChoThu.SoTienCongNoChoThu = giam > danhSachCongNoChoThu.SoTienCongNoChoThu ? 0 : danhSachCongNoChoThu.SoTienCongNoChoThu - giam;
                            }
                        }
                    }
                }
            }
            //set mien giam
            var noiGioiThieus = BaseRepository.Context.Set<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>().AsNoTracking().ToList();
            foreach (var chiPhiKhamChuaBenhVo in result)
            {
                var danhSachMienGiamVos = chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.ToList();
                if (chiPhiKhamChuaBenhVo.CapNhatThanhToan)
                {
                    if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                    {
                        //remove DoiTuongUuDai
                        foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.DoiTuongUuDaiId != null))
                        {
                            if (danhSachMienGiamVo.DoiTuongUuDaiId !=
                                yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId)
                            {
                                danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                            }
                            else
                            {
                                danhSachMienGiamVo.DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten;

                                if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                                {
                                    var uuDai =
                                        yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai
                                            .DoiTuongUuDaiDichVuKhamBenhBenhViens.FirstOrDefault(o =>
                                                o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                                    if (uuDai != null)
                                    {
                                        danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
                                    }
                                    else
                                    {
                                        danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                                    }
                                }
                                else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                                {
                                    var uuDai =
                                        yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai
                                            .DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(o =>
                                                o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                                    if (uuDai != null)
                                    {
                                        danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
                                    }
                                    else
                                    {
                                        danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                                    }
                                }
                            }
                        }
                        //remove TheVoucher
                        foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.TheVoucherId != null))
                        {
                            if (!yeuCauTiepNhanNgoaiTruCanQuyetToan.TheVoucherYeuCauTiepNhans
                                .Select(o => o.TheVoucherId)
                                .Contains(danhSachMienGiamVo.TheVoucherId.GetValueOrDefault()))
                            {
                                danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                            }
                        }
                    }
                    else
                    {
                        //remove DoiTuongUuDai
                        foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.DoiTuongUuDaiId != null))
                        {
                            if (danhSachMienGiamVo.DoiTuongUuDaiId != yeuCauTiepNhan.DoiTuongUuDaiId)
                            {
                                danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                            }
                            else
                            {
                                danhSachMienGiamVo.DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten;

                                if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                                {
                                    var uuDai =
                                        yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens
                                            .FirstOrDefault(o =>
                                                o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                                    if (uuDai != null)
                                    {
                                        danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
                                    }
                                    else
                                    {
                                        danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                                    }
                                }
                                else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                                {
                                    var uuDai =
                                        yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(
                                            o => o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                                    if (uuDai != null)
                                    {
                                        danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
                                    }
                                    else
                                    {
                                        danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                                    }
                                }
                            }
                        }
                        //remove TheVoucher
                        foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.TheVoucherId != null))
                        {
                            if (!yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.Select(o => o.TheVoucherId)
                                .Contains(danhSachMienGiamVo.TheVoucherId.GetValueOrDefault()))
                            {
                                danhSachMienGiamVos.Remove(danhSachMienGiamVo);
                            }
                        }
                    }
                }
                //add DoiTuongUuDai
                if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                {
                    if (yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId != null && danhSachMienGiamVos.All(o =>
                            o.DoiTuongUuDaiId != yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId))
                    {
                        if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                        {
                            var uuDai =
                                yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens
                                    .FirstOrDefault(o =>
                                        o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                            if (uuDai != null)
                            {
                                danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                {
                                    LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
                                    DoiTuongUuDaiId = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId,
                                    DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                    TiLe = uuDai.TiLeUuDai
                                });
                            }
                        }
                        else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                        {
                            var uuDai =
                                yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens
                                    .FirstOrDefault(o =>
                                        o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                            if (uuDai != null)
                            {
                                danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                {
                                    LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
                                    DoiTuongUuDaiId = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId,
                                    DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                    TiLe = uuDai.TiLeUuDai
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (yeuCauTiepNhan.DoiTuongUuDaiId != null && danhSachMienGiamVos.All(o => o.DoiTuongUuDaiId != yeuCauTiepNhan.DoiTuongUuDaiId))
                    {
                        if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                        {
                            var uuDai = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens.FirstOrDefault(o => o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                            if (uuDai != null)
                            {
                                danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                {
                                    LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
                                    DoiTuongUuDaiId = yeuCauTiepNhan.DoiTuongUuDaiId,
                                    DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                    TiLe = uuDai.TiLeUuDai
                                });
                            }
                        }
                        else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                        {
                            var uuDai = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
                            if (uuDai != null)
                            {
                                danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                {
                                    LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
                                    DoiTuongUuDaiId = yeuCauTiepNhan.DoiTuongUuDaiId,
                                    DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                                    TiLe = uuDai.TiLeUuDai
                                });
                            }
                        }
                    }
                }
                //add TheVoucher
                if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
                {
                    foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhanNgoaiTruCanQuyetToan.TheVoucherYeuCauTiepNhans)
                    {
                        if (danhSachMienGiamVos.All(o => o.TheVoucherId != theVoucherYeuCauTiepNhan.TheVoucherId))
                        {
                            var voucher = theVoucherYeuCauTiepNhan.TheVoucher.Voucher;
                            if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                            {
                                if (voucher.ChietKhauTatCaDichVu == true)
                                {
                                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                    {
                                        LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                        TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                        MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                        LoaiChietKhau = voucher.LoaiChietKhau.Value,
                                        TiLe = voucher.TiLeChietKhau,
                                        SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
                                    });
                                }
                                else
                                {
                                    //var ycKhamBenh = yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
                                    var voucherKhamBenh = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.NhomDichVuKhamBenh == true || (o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.LoaiGiaId));
                                    if (voucherKhamBenh != null)
                                    {
                                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                        {
                                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                            LoaiChietKhau = voucherKhamBenh.LoaiChietKhau,
                                            TiLe = voucherKhamBenh.TiLeChietKhau,
                                            SoTien = voucherKhamBenh.SoTienChietKhau.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                            else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                            {
                                if (voucher.ChietKhauTatCaDichVu == true)
                                {
                                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                    {
                                        LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                        TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                        MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                        LoaiChietKhau = voucher.LoaiChietKhau.Value,
                                        TiLe = voucher.TiLeChietKhau,
                                        SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
                                    });
                                }
                                else
                                {
                                    //var ycDichVuKyThuat = yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
                                    var voucherDichVuKyThuat = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => (o.NhomDichVuBenhVienId != null && o.NhomDichVuBenhVienId == chiPhiKhamChuaBenhVo.NhomDichVuBenhVienId)
                                                                                                || (o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.LoaiGiaId));
                                    if (voucherDichVuKyThuat != null)
                                    {
                                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                        {
                                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                            LoaiChietKhau = voucherDichVuKyThuat.LoaiChietKhau,
                                            TiLe = voucherDichVuKyThuat.TiLeChietKhau,
                                            SoTien = voucherDichVuKyThuat.SoTienChietKhau.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhan.TheVoucherYeuCauTiepNhans)
                    {
                        if (danhSachMienGiamVos.All(o => o.TheVoucherId != theVoucherYeuCauTiepNhan.TheVoucherId))
                        {
                            var voucher = theVoucherYeuCauTiepNhan.TheVoucher.Voucher;
                            if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
                            {
                                if (voucher.ChietKhauTatCaDichVu == true)
                                {
                                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                    {
                                        LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                        TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                        MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                        LoaiChietKhau = voucher.LoaiChietKhau.Value,
                                        TiLe = voucher.TiLeChietKhau,
                                        SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
                                    });
                                }
                                else
                                {
                                    //var ycKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
                                    var voucherKhamBenh = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.NhomDichVuKhamBenh == true || (o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.LoaiGiaId));
                                    if (voucherKhamBenh != null)
                                    {
                                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                        {
                                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                            LoaiChietKhau = voucherKhamBenh.LoaiChietKhau,
                                            TiLe = voucherKhamBenh.TiLeChietKhau,
                                            SoTien = voucherKhamBenh.SoTienChietKhau.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                            else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
                            {
                                if (voucher.ChietKhauTatCaDichVu == true)
                                {
                                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                    {
                                        LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                        TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                        MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                        LoaiChietKhau = voucher.LoaiChietKhau.Value,
                                        TiLe = voucher.TiLeChietKhau,
                                        SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
                                    });
                                }
                                else
                                {
                                    //var ycDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
                                    var voucherDichVuKyThuat = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => (o.NhomDichVuBenhVienId != null && o.NhomDichVuBenhVienId == chiPhiKhamChuaBenhVo.NhomDichVuBenhVienId)
                                                                                                || (o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.LoaiGiaId));
                                    if (voucherDichVuKyThuat != null)
                                    {
                                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                                        {
                                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
                                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
                                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
                                            LoaiChietKhau = voucherDichVuKyThuat.LoaiChietKhau,
                                            TiLe = voucherDichVuKyThuat.TiLeChietKhau,
                                            SoTien = voucherDichVuKyThuat.SoTienChietKhau.GetValueOrDefault()
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                //xoa BHTN va mien giam khi sl = 0
                if (chiPhiKhamChuaBenhVo.Soluong.AlmostEqual(0))
                {
                    danhSachMienGiamVos = new List<DanhSachMienGiamVo>();
                    chiPhiKhamChuaBenhVo.DanhSachCongNoChoThus = new List<DanhSachCongNoVo>();
                }

                //add MienGiamThem
                if (!danhSachMienGiamVos.Exists(o =>
                    o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem &&
                    o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
                {
                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                    {
                        LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
                        TiLe = 0,
                        SoTien = 0
                    });
                }
                if (!danhSachMienGiamVos.Exists(o =>
                    o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem &&
                    o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
                {
                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
                    {
                        LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                        SoTien = 0
                    });
                }
                
                foreach (var mg in danhSachMienGiamVos)
                {
                    if (mg.NoiGioiThieuId != null)
                    {
                        mg.DoiTuongUuDai = noiGioiThieus.FirstOrDefault(o => o.Id == mg.NoiGioiThieuId)?.Ten;
                    }
                }
                chiPhiKhamChuaBenhVo.DanhSachMienGiamVos = danhSachMienGiamVos;

                var soTienTruocMienGiam = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;
                decimal soTienMienGiamTheoDv = 0;
                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
                {
                    mienGiamTheoTiLe.SoTien = Math.Round((soTienTruocMienGiam * mienGiamTheoTiLe.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
                {
                    soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
                }
                chiPhiKhamChuaBenhVo.SoTienMG = soTienMienGiamTheoDv;

                //chiPhiKhamChuaBenhVo.SoTienMG = danhSachMienGiamVos.Sum(o => o.SoTien);
            }

            return result;
        }

        public async Task<decimal> GetChiPhiNoiTruConPhaiThanhToan(long yeuCauTiepNhanId)
        {
            //bo Async 3/11
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.CongTyUuDai)
                    //.Include(o => o.YeuCauKhamBenhs)
                    //.Include(o => o.YeuCauDichVuKyThuats)
                    .Include(o => o.NoiTruBenhAn)
                    .Include(o => o.YeuCauTiepNhanTheBHYTs)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
                    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                    //.Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                    //.Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                    //.Include(o => o.GiayMienGiamThem)
                    //.Include(o => o.NhanVienDuyetMienGiamThem).ThenInclude(v => v.User)
                    //.Include(o => o.TheVoucherYeuCauTiepNhans).ThenInclude(v => v.TheVoucher).ThenInclude(v => v.Voucher).ThenInclude(v => v.VoucherChiTietMienGiams)
                );

            long? yeuCauTiepNhanNgoaiTruCanQuyetToanId = yeuCauTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            YeuCauTiepNhan yeuCauTiepNhanNgoaiTruCanQuyetToan = null;
            if (yeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
            {
                yeuCauTiepNhanNgoaiTruCanQuyetToan = BaseRepository.GetById(yeuCauTiepNhanNgoaiTruCanQuyetToanId.Value,
                    x => x.Include(o => o.CongTyUuDai)
                        .Include(o => o.YeuCauKhamBenhs)
                        .Include(o => o.YeuCauDichVuKyThuats)
                        .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                        .Include(o => o.DoiTuongUuDai).ThenInclude(ud => ud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                        .Include(o => o.GiayMienGiamThem)
                        .Include(o => o.NhanVienDuyetMienGiamThem).ThenInclude(v => v.User)
                        .Include(o => o.TheVoucherYeuCauTiepNhans).ThenInclude(v => v.TheVoucher)
                        .ThenInclude(v => v.Voucher).ThenInclude(v => v.VoucherChiTietMienGiams));
            }


            var queryDichVuKhamBenh = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauKhamBenhs)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    //DichVuBenhVienId = s.DichVuKhamBenhBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.DichVuKhamBenh,
                    //Ma = s.DichVuKhamBenhBenhVien.Ma,
                    //Nhom = NhomChiPhiNoiTru.DichVuKhamBenh.GetDescription(),
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.KhamBenh,
                    //TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    //LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    Soluong = 1,
                    DonGia = s.Gia,
                    //LoaiGiaId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos = s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //    .Select(o => new DanhSachMienGiamVo
                    //    {
                    //        LoaiMienGiam = o.LoaiMienGiam,
                    //        LoaiChietKhau = o.LoaiChietKhau,
                    //        TheVoucherId = o.TheVoucherId,
                    //        MaTheVoucher = o.MaTheVoucher,
                    //        DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //        NoiGioiThieuId = o.NoiGioiThieuId,
                    //        TiLe = o.TiLe,
                    //        SoTien = o.SoTien
                    //    }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //                                                                                (k, v) => new DanhSachMienGiamVo
                    //                                                                                {
                    //                                                                                    LoaiMienGiam = k.LoaiMienGiam,
                    //                                                                                    LoaiChietKhau = k.LoaiChietKhau,
                    //                                                                                    TheVoucherId = k.TheVoucherId,
                    //                                                                                    MaTheVoucher = k.MaTheVoucher,
                    //                                                                                    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //                                                                                    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //                                                                                    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //                                                                                }).Where(o => o.SoTien != 0).ToList(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,

                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()
                    //Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    //CheckedDefault = true,
                }).OrderBy(o => o.Id).ToListAsync();

            var queryDichVuKyThuat = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauDichVuKyThuats)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    //NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    //DichVuBenhVienId = s.DichVuKyThuatBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.DichVuKyThuat,
                    //Ma = s.DichVuKyThuatBenhVien.Ma,
                    //Nhom = NhomChiPhiNoiTru.DichVuKyThuat.GetDescription(),
                    //NhomChiPhiBangKe = s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? NhomChiPhiBangKe.XetNgiem
                    //                    : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh ? NhomChiPhiBangKe.ChuanDoanHinhAnh
                    //                    : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang ? NhomChiPhiBangKe.ThamDoChucNang
                    //                    : s.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat ? NhomChiPhiBangKe.ThuThuatPhauThuat
                    //                    : NhomChiPhiBangKe.DichVuKhac,
                    //TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    //LoaiGia = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    Soluong = s.SoLan,
                    DonGia = s.Gia,
                    //LoaiGiaId = s.NhomGiaDichVuKyThuatBenhVienId,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault()
                    //Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    //DoiTuongSuDung = s.DoiTuongSuDung,
                    //CheckedDefault = true,
                }).ToListAsync();

            var queryDichVuTruyenMau = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauTruyenMaus)
                .Where(yc => yc.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    //NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    //DichVuBenhVienId = s.MauVaChePhamId,
                    //LoaiNhom = NhomChiPhiNoiTru.TruyenMau,
                    //Ma = s.MaDichVu,
                    //Nhom = NhomChiPhiNoiTru.TruyenMau.GetDescription(),
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.Mau,
                    //TenDichVu = s.TenDichVu,
                    //LoaiGia = string.Empty,

                    Soluong = 1,
                    DonGia = s.DonGiaBan.GetValueOrDefault(),
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                    //Khoa = s.NoiThucHien != null ? s.NoiThucHien.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    //CheckedDefault = true,
                }).ToListAsync();

            var queryDichVuGiuongChiPhiBenhVien = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .Where(yc => yc.YeuCauGoiDichVuId == null && (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    //DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    //Ma = s.Ma,
                    //Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru,
                    //TenDichVu = s.Ten,
                    //LoaiGia = s.NhomGiaDichVuGiuongBenhVien.Ten,
                    //LoaiGiaId = s.NhomGiaDichVuGiuongBenhVienId,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    //KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    //DuocHuongBHYT = s.DuocHuongBaoHiem,
                    //DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    //TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    //MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                    //Khoa = s.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    //CheckedDefault = true
                }).ToListAsync();
            var queryDichVuGiuongChiPhiBHYT = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ToListAsync();

            var queryDuocPham = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauDuocPhamBenhViens)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    //NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    //DichVuBenhVienId = s.DuocPhamBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.DuocPham,
                    //Nhom = NhomChiPhiNoiTru.DuocPham.GetDescription(),
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.ThuocDichTruyen,
                    //Ma = s.DuocPhamBenhVien.Ma,
                    //TenDichVu = s.DuocPhamBenhVien.DuocPham.Ten,
                    //LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                    //Khoa = s.NoiCapThuoc != null ? s.NoiCapThuoc.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    //CheckedDefault = true,
                }).ToListAsync();

            var queryVatTu = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.YeuCauVatTuBenhViens)
                .Where(yc => yc.YeuCauGoiDichVuId == null && yc.KhongTinhPhi != true && yc.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy &&
                             (yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.ThoiDiemChiDinh,
                    //NgayPhatSinh = s.NoiTruPhieuDieuTri != null ? s.NoiTruPhieuDieuTri.NgayDieuTri : s.ThoiDiemChiDinh,
                    //DichVuBenhVienId = s.VatTuBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.VatTuTieuHao,
                    //Nhom = NhomChiPhiNoiTru.VatTuTieuHao.GetDescription(),
                    //NhomChiPhiBangKe = s.YeuCauDichVuKyThuat != null && s.YeuCauDichVuKyThuat.LanThucHien != null ? NhomChiPhiBangKe.GoiVatTu : NhomChiPhiBangKe.VatTuYte,
                    //SoThuTuGoiTrongNhomChiPhiBangKe = s.YeuCauDichVuKyThuat != null && s.YeuCauDichVuKyThuat.LanThucHien != null ? s.YeuCauDichVuKyThuat.LanThucHien.Value : 0,
                    //Ma = s.VatTuBenhVien.Ma,
                    //TenDichVu = s.VatTuBenhVien.VatTus.Ten,
                    //LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KhongTinhPhi = s.KhongTinhPhi,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                    //Khoa = s.NoiCapVatTu != null ? s.NoiCapVatTu.KhoaPhong.Ten : s.NoiChiDinh.KhoaPhong.Ten,
                    //CheckedDefault = true,
                }).ToListAsync();

            var queryToaThuoc = BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId || o.Id == yeuCauTiepNhanNgoaiTruCanQuyetToanId)
                .SelectMany(o => o.DonThuocThanhToans)
                .Where(dt =>
                    dt.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT && dt.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy &&
                    (dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan ||
                     dt.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan))
                .SelectMany(o => o.DonThuocThanhToanChiTiets)
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    DichVuNgoaiTru = s.DonThuocThanhToan.YeuCauTiepNhanId != yeuCauTiepNhanId,
                    NgayPhatSinh = s.CreatedOn,
                    //DichVuBenhVienId = s.DuocPhamId,
                    //LoaiNhom = NhomChiPhiNoiTru.ToaThuoc,
                    //Nhom = $"{NhomChiPhiNoiTru.ToaThuoc.GetDescription()}: {s.DonThuocThanhToan.YeuCauKhamBenhDonThuoc.YeuCauKhamBenh.TenDichVu}",
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.ThuocDichTruyen,
                    //Ma = string.Empty,
                    //TenDichVu = s.Ten,
                    //LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.DonGiaBan,
                    KiemTraBHYTXacNhan = s.BaoHiemChiTra != null,
                    DuocHuongBHYT = s.DuocHuongBaoHiem,
                    DonGiaBHYT = s.DonGiaBaoHiem.GetValueOrDefault(),
                    TiLeBaoHiemThanhToan = s.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                    MucHuongBaoHiem = s.MucHuongBaoHiem.GetValueOrDefault(),
                    //MaSoTheBHYT = s.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }).ToList(),
                    //DanhSachMienGiamVos =
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null && o.SoTien != 0)
                    //        .Select(o => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = o.LoaiMienGiam,
                    //            LoaiChietKhau = o.LoaiChietKhau,
                    //            TheVoucherId = o.TheVoucherId,
                    //            MaTheVoucher = o.MaTheVoucher,
                    //            DoiTuongUuDaiId = o.DoiTuongUuDaiId,
                    //            NoiGioiThieuId = o.NoiGioiThieuId,
                    //            TiLe = o.TiLe,
                    //            SoTien = o.SoTien
                    //        }).ToList(),
                    //.GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //(k, v) => new DanhSachMienGiamVo
                    //{
                    //    LoaiMienGiam = k.LoaiMienGiam,
                    //    LoaiChietKhau = k.LoaiChietKhau,
                    //    TheVoucherId = k.TheVoucherId,
                    //    MaTheVoucher = k.MaTheVoucher,
                    //    DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //    TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //    SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //}).Where(o => o.SoTien != 0).ToList(),
                    //SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.DonThuocThanhToan.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    SoTienMG = s.SoTienMienGiam.GetValueOrDefault(),
                    SoTienBaoHiemTuNhanChiTra = s.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault(),
                    //Khoa = "Nhà thuốc",
                    //CheckedDefault = true,
                }).ToListAsync();
                       

            await Task.WhenAll(queryDichVuKhamBenh, queryDichVuKyThuat, queryDichVuTruyenMau, queryDuocPham, queryVatTu, queryDichVuGiuongChiPhiBenhVien, queryDichVuGiuongChiPhiBHYT,
                queryToaThuoc);
            var result = new List<ChiPhiKhamChuaBenhNoiTruVo>();
            result.AddRange(queryDichVuKhamBenh.Result);
            result.AddRange(queryDichVuKyThuat.Result);
            result.AddRange(queryDichVuTruyenMau.Result);
            result.AddRange(queryDuocPham.Result);
            result.AddRange(queryVatTu.Result);
            result.AddRange(queryToaThuoc.Result);

            List<YeuCauDichVuGiuongBenhVienChiPhiBHYT> dichVuGiuongBenhVienChiPhiBhyts;
            List<ChiPhiKhamChuaBenhNoiTruVo> dichVuGiuongs;
            if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
            {
                var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                dichVuGiuongBenhVienChiPhiBhyts = chiPhiGiuong.Item2;
                dichVuGiuongs = chiPhiGiuong.Item1.Where(o => o.YeuCauGoiDichVuId == null).Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    //DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    //LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    //Ma = s.Ma,
                    //Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    //NhomChiPhiBangKe = NhomChiPhiBangKe.NgayGiuongDieuTriNoiTru,
                    //TenDichVu = s.Ten,
                    //LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    DuocHuongBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(),
                    KiemTraBHYTXacNhan = false,
                    DonGiaBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.DonGiaBaoHiem.GetValueOrDefault() ?? 0,
                    TiLeBaoHiemThanhToan = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.TiLeBaoHiemThanhToan.GetValueOrDefault() ?? 0,
                    MucHuongBaoHiem = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.MucHuongBaoHiem.GetValueOrDefault() ?? 0,
                    MaSoTheBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.MaSoTheBHYT ?? string.Empty,
                    //DanhSachCongNoChoThus = (s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                    //DanhSachMienGiamVos = (s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachMienGiamVo>() :
                    //    s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                    //        (k, v) => new DanhSachMienGiamVo
                    //        {
                    //            LoaiMienGiam = k.LoaiMienGiam,
                    //            LoaiChietKhau = k.LoaiChietKhau,
                    //            TheVoucherId = k.TheVoucherId,
                    //            MaTheVoucher = k.MaTheVoucher,
                    //            DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                    //            TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                    //            SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                    //        }).Where(o => o.SoTien != 0),
                    //GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    //NoiDungGhiChuMiemGiamId = s.NoiDungGhiChuMiemGiamId,
                    //CapNhatThanhToan = s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan,
                    //DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    //Khoa = s.KhoaPhong.Ten,
                    //DoiTuongSuDung = s.DoiTuongSuDung,
                    //CheckedDefault = true
                }).ToList();
            }
            else
            {
                dichVuGiuongBenhVienChiPhiBhyts = queryDichVuGiuongChiPhiBHYT.Result;
                dichVuGiuongs = queryDichVuGiuongChiPhiBenhVien.Result;
                foreach (var chiPhiKhamChuaBenhNoiTruVo in dichVuGiuongs.Where(o => o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan))
                {
                    var chiPhiGiuongBHYT = dichVuGiuongBenhVienChiPhiBhyts.FirstOrDefault(o => o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId == chiPhiKhamChuaBenhNoiTruVo.Id);
                    if (chiPhiGiuongBHYT != null)
                    {
                        chiPhiKhamChuaBenhNoiTruVo.DuocHuongBHYT = true;
                        chiPhiKhamChuaBenhNoiTruVo.KiemTraBHYTXacNhan = chiPhiGiuongBHYT.BaoHiemChiTra != null;
                        chiPhiKhamChuaBenhNoiTruVo.DonGiaBHYT = chiPhiGiuongBHYT.DonGiaBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.TiLeBaoHiemThanhToan = chiPhiGiuongBHYT.TiLeBaoHiemThanhToan.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.MucHuongBaoHiem = chiPhiGiuongBHYT.MucHuongBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.MaSoTheBHYT = chiPhiGiuongBHYT.MaSoTheBHYT ?? string.Empty;
                        chiPhiKhamChuaBenhNoiTruVo.YeuCauDichVuGiuongBenhVienChiPhiBHYTIds = new List<long> { chiPhiGiuongBHYT.Id };
                    }
                }
            }

            result.AddRange(dichVuGiuongs);

            //set muc huong BHYT
            if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
            {
                foreach (var chiPhiKhamChuaBenhVo in result.Where(o => o.DuocHuongBHYT && o.KiemTraBHYTXacNhan == false))
                {
                    var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs
                        .Where(o => o.NgayHieuLuc.Date <= chiPhiKhamChuaBenhVo.NgayPhatSinh.GetValueOrDefault().Date &&
                                    (o.NgayHetHan == null || o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date >= chiPhiKhamChuaBenhVo.NgayPhatSinh.GetValueOrDefault().Date))
                        .OrderByDescending(o => o.MucHuong).FirstOrDefault();
                    if (theBHYT != null)
                    {
                        chiPhiKhamChuaBenhVo.MucHuongBaoHiem = theBHYT.MucHuong;
                        chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan = chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan != 0 ? chiPhiKhamChuaBenhVo.TiLeBaoHiemThanhToan : 100;
                    }
                }
            }

            //foreach (var chiPhiKhamChuaBenhVo in result)
            //{
            //    if (chiPhiKhamChuaBenhVo.CapNhatThanhToan)
            //    {
            //        if (chiPhiKhamChuaBenhVo.TongCongNo > chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan)
            //        {
            //            var maxCongNo = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;
            //            foreach (var danhSachCongNoChoThu in chiPhiKhamChuaBenhVo.DanhSachCongNoChoThus)
            //            {
            //                if (chiPhiKhamChuaBenhVo.TongCongNo > maxCongNo)
            //                {
            //                    var giam = chiPhiKhamChuaBenhVo.TongCongNo - maxCongNo;
            //                    danhSachCongNoChoThu.SoTienCongNoChoThu = giam > danhSachCongNoChoThu.SoTienCongNoChoThu ? 0 : danhSachCongNoChoThu.SoTienCongNoChoThu - giam;
            //                }
            //            }
            //        }
            //    }
            //}
            //set mien giam

            //foreach (var chiPhiKhamChuaBenhVo in result)
            //{
            //    var danhSachMienGiamVos = chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.ToList();
            //    if (chiPhiKhamChuaBenhVo.CapNhatThanhToan)
            //    {
            //        if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
            //        {
            //            //remove DoiTuongUuDai
            //            foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.DoiTuongUuDaiId != null))
            //            {
            //                if (danhSachMienGiamVo.DoiTuongUuDaiId !=
            //                    yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId)
            //                {
            //                    danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                }
            //                else
            //                {
            //                    danhSachMienGiamVo.DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten;

            //                    if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //                    {
            //                        var uuDai =
            //                            yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai
            //                                .DoiTuongUuDaiDichVuKhamBenhBenhViens.FirstOrDefault(o =>
            //                                    o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                        if (uuDai != null)
            //                        {
            //                            danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
            //                        }
            //                        else
            //                        {
            //                            danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                        }
            //                    }
            //                    else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //                    {
            //                        var uuDai =
            //                            yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai
            //                                .DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(o =>
            //                                    o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                        if (uuDai != null)
            //                        {
            //                            danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
            //                        }
            //                        else
            //                        {
            //                            danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                        }
            //                    }
            //                }
            //            }
            //            //remove TheVoucher
            //            foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.TheVoucherId != null))
            //            {
            //                if (!yeuCauTiepNhanNgoaiTruCanQuyetToan.TheVoucherYeuCauTiepNhans
            //                    .Select(o => o.TheVoucherId)
            //                    .Contains(danhSachMienGiamVo.TheVoucherId.GetValueOrDefault()))
            //                {
            //                    danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //remove DoiTuongUuDai
            //            foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.DoiTuongUuDaiId != null))
            //            {
            //                if (danhSachMienGiamVo.DoiTuongUuDaiId != yeuCauTiepNhan.DoiTuongUuDaiId)
            //                {
            //                    danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                }
            //                else
            //                {
            //                    danhSachMienGiamVo.DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten;

            //                    if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //                    {
            //                        var uuDai =
            //                            yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens
            //                                .FirstOrDefault(o =>
            //                                    o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                        if (uuDai != null)
            //                        {
            //                            danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
            //                        }
            //                        else
            //                        {
            //                            danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                        }
            //                    }
            //                    else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //                    {
            //                        var uuDai =
            //                            yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(
            //                                o => o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                        if (uuDai != null)
            //                        {
            //                            danhSachMienGiamVo.TiLe = uuDai.TiLeUuDai;
            //                        }
            //                        else
            //                        {
            //                            danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                        }
            //                    }
            //                }
            //            }
            //            //remove TheVoucher
            //            foreach (var danhSachMienGiamVo in danhSachMienGiamVos.Where(o => o.TheVoucherId != null))
            //            {
            //                if (!yeuCauTiepNhan.TheVoucherYeuCauTiepNhans.Select(o => o.TheVoucherId)
            //                    .Contains(danhSachMienGiamVo.TheVoucherId.GetValueOrDefault()))
            //                {
            //                    danhSachMienGiamVos.Remove(danhSachMienGiamVo);
            //                }
            //            }
            //        }
            //    }
            //    //add DoiTuongUuDai
            //    if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
            //    {
            //        if (yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId != null && danhSachMienGiamVos.All(o =>
            //                o.DoiTuongUuDaiId != yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId))
            //        {
            //            if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //            {
            //                var uuDai =
            //                    yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens
            //                        .FirstOrDefault(o =>
            //                            o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                if (uuDai != null)
            //                {
            //                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                    {
            //                        LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
            //                        DoiTuongUuDaiId = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId,
            //                        DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten,
            //                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
            //                        TiLe = uuDai.TiLeUuDai
            //                    });
            //                }
            //            }
            //            else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //            {
            //                var uuDai =
            //                    yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens
            //                        .FirstOrDefault(o =>
            //                            o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                if (uuDai != null)
            //                {
            //                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                    {
            //                        LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
            //                        DoiTuongUuDaiId = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDaiId,
            //                        DoiTuongUuDai = yeuCauTiepNhanNgoaiTruCanQuyetToan.DoiTuongUuDai.Ten,
            //                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
            //                        TiLe = uuDai.TiLeUuDai
            //                    });
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (yeuCauTiepNhan.DoiTuongUuDaiId != null && danhSachMienGiamVos.All(o => o.DoiTuongUuDaiId != yeuCauTiepNhan.DoiTuongUuDaiId))
            //        {
            //            if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //            {
            //                var uuDai = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKhamBenhBenhViens.FirstOrDefault(o => o.DichVuKhamBenhBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                if (uuDai != null)
            //                {
            //                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                    {
            //                        LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
            //                        DoiTuongUuDaiId = yeuCauTiepNhan.DoiTuongUuDaiId,
            //                        DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten,
            //                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
            //                        TiLe = uuDai.TiLeUuDai
            //                    });
            //                }
            //            }
            //            else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //            {
            //                var uuDai = yeuCauTiepNhan.DoiTuongUuDai.DoiTuongUuDaiDichVuKyThuatBenhViens.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == chiPhiKhamChuaBenhVo.DichVuBenhVienId);
            //                if (uuDai != null)
            //                {
            //                    danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                    {
            //                        LoaiMienGiam = Enums.LoaiMienGiam.UuDai,
            //                        DoiTuongUuDaiId = yeuCauTiepNhan.DoiTuongUuDaiId,
            //                        DoiTuongUuDai = yeuCauTiepNhan.DoiTuongUuDai.Ten,
            //                        LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
            //                        TiLe = uuDai.TiLeUuDai
            //                    });
            //                }
            //            }
            //        }
            //    }
            //    //add TheVoucher
            //    if (chiPhiKhamChuaBenhVo.DichVuNgoaiTru && yeuCauTiepNhanNgoaiTruCanQuyetToan != null)
            //    {
            //        foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhanNgoaiTruCanQuyetToan.TheVoucherYeuCauTiepNhans)
            //        {
            //            if (danhSachMienGiamVos.All(o => o.TheVoucherId != theVoucherYeuCauTiepNhan.TheVoucherId))
            //            {
            //                var voucher = theVoucherYeuCauTiepNhan.TheVoucher.Voucher;
            //                if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //                {
            //                    if (voucher.ChietKhauTatCaDichVu == true)
            //                    {
            //                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                        {
            //                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                            LoaiChietKhau = voucher.LoaiChietKhau.Value,
            //                            TiLe = voucher.TiLeChietKhau,
            //                            SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
            //                        });
            //                    }
            //                    else
            //                    {
            //                        var ycKhamBenh = yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauKhamBenhs.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
            //                        var voucherKhamBenh = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.NhomDichVuKhamBenh == true || (o.DichVuKhamBenhBenhVienId == ycKhamBenh.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == ycKhamBenh.NhomGiaDichVuKhamBenhBenhVienId));
            //                        if (voucherKhamBenh != null)
            //                        {
            //                            danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                            {
            //                                LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                                TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                                MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                                LoaiChietKhau = voucherKhamBenh.LoaiChietKhau,
            //                                TiLe = voucherKhamBenh.TiLeChietKhau,
            //                                SoTien = voucherKhamBenh.SoTienChietKhau.GetValueOrDefault()
            //                            });
            //                        }
            //                    }
            //                }
            //                else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //                {
            //                    if (voucher.ChietKhauTatCaDichVu == true)
            //                    {
            //                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                        {
            //                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                            LoaiChietKhau = voucher.LoaiChietKhau.Value,
            //                            TiLe = voucher.TiLeChietKhau,
            //                            SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
            //                        });
            //                    }
            //                    else
            //                    {
            //                        var ycDichVuKyThuat = yeuCauTiepNhanNgoaiTruCanQuyetToan.YeuCauDichVuKyThuats.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
            //                        var voucherDichVuKyThuat = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => (o.NhomDichVuBenhVienId != null && o.NhomDichVuBenhVienId == ycDichVuKyThuat.NhomDichVuBenhVienId)
            //                                                                                    || (o.DichVuKyThuatBenhVienId == ycDichVuKyThuat.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == ycDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId));
            //                        if (voucherDichVuKyThuat != null)
            //                        {
            //                            danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                            {
            //                                LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                                TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                                MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                                LoaiChietKhau = voucherDichVuKyThuat.LoaiChietKhau,
            //                                TiLe = voucherDichVuKyThuat.TiLeChietKhau,
            //                                SoTien = voucherDichVuKyThuat.SoTienChietKhau.GetValueOrDefault()
            //                            });
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        foreach (var theVoucherYeuCauTiepNhan in yeuCauTiepNhan.TheVoucherYeuCauTiepNhans)
            //        {
            //            if (danhSachMienGiamVos.All(o => o.TheVoucherId != theVoucherYeuCauTiepNhan.TheVoucherId))
            //            {
            //                var voucher = theVoucherYeuCauTiepNhan.TheVoucher.Voucher;
            //                if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKhamBenh)
            //                {
            //                    if (voucher.ChietKhauTatCaDichVu == true)
            //                    {
            //                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                        {
            //                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                            LoaiChietKhau = voucher.LoaiChietKhau.Value,
            //                            TiLe = voucher.TiLeChietKhau,
            //                            SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
            //                        });
            //                    }
            //                    else
            //                    {
            //                        var ycKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
            //                        var voucherKhamBenh = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => o.NhomDichVuKhamBenh == true || (o.DichVuKhamBenhBenhVienId == ycKhamBenh.DichVuKhamBenhBenhVienId && o.NhomGiaDichVuKhamBenhBenhVienId == ycKhamBenh.NhomGiaDichVuKhamBenhBenhVienId));
            //                        if (voucherKhamBenh != null)
            //                        {
            //                            danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                            {
            //                                LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                                TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                                MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                                LoaiChietKhau = voucherKhamBenh.LoaiChietKhau,
            //                                TiLe = voucherKhamBenh.TiLeChietKhau,
            //                                SoTien = voucherKhamBenh.SoTienChietKhau.GetValueOrDefault()
            //                            });
            //                        }
            //                    }
            //                }
            //                else if (chiPhiKhamChuaBenhVo.LoaiNhom == NhomChiPhiNoiTru.DichVuKyThuat)
            //                {
            //                    if (voucher.ChietKhauTatCaDichVu == true)
            //                    {
            //                        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                        {
            //                            LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                            TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                            MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                            LoaiChietKhau = voucher.LoaiChietKhau.Value,
            //                            TiLe = voucher.TiLeChietKhau,
            //                            SoTien = voucher.SoTienChietKhau.GetValueOrDefault()
            //                        });
            //                    }
            //                    else
            //                    {
            //                        var ycDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.First(o => o.Id == chiPhiKhamChuaBenhVo.Id);
            //                        var voucherDichVuKyThuat = voucher.VoucherChiTietMienGiams.FirstOrDefault(o => (o.NhomDichVuBenhVienId != null && o.NhomDichVuBenhVienId == ycDichVuKyThuat.NhomDichVuBenhVienId)
            //                                                                                    || (o.DichVuKyThuatBenhVienId == ycDichVuKyThuat.DichVuKyThuatBenhVienId && o.NhomGiaDichVuKyThuatBenhVienId == ycDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId));
            //                        if (voucherDichVuKyThuat != null)
            //                        {
            //                            danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //                            {
            //                                LoaiMienGiam = Enums.LoaiMienGiam.Voucher,
            //                                TheVoucherId = theVoucherYeuCauTiepNhan.TheVoucherId,
            //                                MaTheVoucher = theVoucherYeuCauTiepNhan.TheVoucher.Ma,
            //                                LoaiChietKhau = voucherDichVuKyThuat.LoaiChietKhau,
            //                                TiLe = voucherDichVuKyThuat.TiLeChietKhau,
            //                                SoTien = voucherDichVuKyThuat.SoTienChietKhau.GetValueOrDefault()
            //                            });
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    //xoa BHTN va mien giam khi sl = 0
            //    if (chiPhiKhamChuaBenhVo.Soluong.AlmostEqual(0))
            //    {
            //        danhSachMienGiamVos = new List<DanhSachMienGiamVo>();
            //        chiPhiKhamChuaBenhVo.DanhSachCongNoChoThus = new List<DanhSachCongNoVo>();
            //    }

            //    //add MienGiamThem
            //    if (!danhSachMienGiamVos.Exists(o =>
            //        o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem &&
            //        o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
            //    {
            //        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //        {
            //            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
            //            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoTiLe,
            //            TiLe = 0,
            //            SoTien = 0
            //        });
            //    }
            //    if (!danhSachMienGiamVos.Exists(o =>
            //        o.LoaiMienGiam == Enums.LoaiMienGiam.MienGiamThem &&
            //        o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
            //    {
            //        danhSachMienGiamVos.Add(new DanhSachMienGiamVo
            //        {
            //            LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
            //            LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
            //            SoTien = 0
            //        });
            //    }
            //    var noiGioiThieus = BaseRepository.Context.Set<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>().AsNoTracking().ToList();
            //    foreach (var mg in danhSachMienGiamVos)
            //    {
            //        if (mg.NoiGioiThieuId != null)
            //        {
            //            mg.DoiTuongUuDai = noiGioiThieus.FirstOrDefault(o => o.Id == mg.NoiGioiThieuId)?.Ten;
            //        }
            //    }
            //    chiPhiKhamChuaBenhVo.DanhSachMienGiamVos = danhSachMienGiamVos;

            //    var soTienTruocMienGiam = chiPhiKhamChuaBenhVo.ThanhTien - chiPhiKhamChuaBenhVo.BHYTThanhToan;
            //    decimal soTienMienGiamTheoDv = 0;
            //    foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoTiLe))
            //    {
            //        mienGiamTheoTiLe.SoTien = Math.Round((soTienTruocMienGiam * mienGiamTheoTiLe.TiLe.GetValueOrDefault() / 100), 2, MidpointRounding.AwayFromZero);
            //        soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
            //    }
            //    foreach (var mienGiamTheoTiLe in chiPhiKhamChuaBenhVo.DanhSachMienGiamVos.Where(o => o.LoaiChietKhau == Enums.LoaiChietKhau.ChietKhauTheoSoTien))
            //    {
            //        soTienMienGiamTheoDv += mienGiamTheoTiLe.SoTien;
            //    }
            //    chiPhiKhamChuaBenhVo.SoTienMG = soTienMienGiamTheoDv;

            //    //chiPhiKhamChuaBenhVo.SoTienMG = danhSachMienGiamVos.Sum(o => o.SoTien);
            //}

            return result.Select(o=>o.BNConPhaiThanhToan).DefaultIfEmpty().Sum();
        }

        private int GetTiLeBHYTThanhToanTheoDichVuGiuong(int soLuongGhep)
        {
            if (soLuongGhep == 1)
                return 100;
            if (soLuongGhep == 2)
                return 50;
            if (soLuongGhep > 2)
                return 33;
            return 0;
        }

        protected (List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>, List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>) TinhChiPhiDichVuGiuong(YeuCauTiepNhan yeuCauTiepNhan)
        {
            var yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>();
            var yeuCauDichVuGiuongBenhVienChiPhiBHYTs = new List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>();
            var ngayKetThuc = yeuCauTiepNhan.NoiTruBenhAn?.ThoiDiemRaVien ?? DateTime.Now;
            //tinh giuong Benh Nhan
            var dvGiuongBenhNhans = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(o => o.ThoiDiemBatDauSuDung != null && o.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(o => o.ThoiDiemBatDauSuDung).ToList();
            if (dvGiuongBenhNhans.Count > 0)
            {
                var thoiDienBatDau = dvGiuongBenhNhans.First().ThoiDiemBatDauSuDung.Value;
                var ngayPhatSinh = thoiDienBatDau.Date;
                while (ngayPhatSinh < ngayKetThuc)
                {
                    var thoiDienBatDauTheoNgay = ngayPhatSinh.AddSeconds((thoiDienBatDau - thoiDienBatDau.Date).TotalSeconds);
                    if (thoiDienBatDau.Date == ngayPhatSinh || thoiDienBatDauTheoNgay < ngayKetThuc)
                    {
                        var dvGiuongBenhNhanTrongNgays = dvGiuongBenhNhans
                    .Where(o => o.ThoiDiemBatDauSuDung.Value.Date <= ngayPhatSinh &&
                                (o.ThoiDiemKetThucSuDung == null || o.ThoiDiemKetThucSuDung.Value.Date >= ngayPhatSinh)).OrderBy(o => o.ThoiDiemBatDauSuDung)
                    .ToList();
                        if (dvGiuongBenhNhanTrongNgays.Count > 0)
                        {
                            if (dvGiuongBenhNhanTrongNgays.Any(o => o.DuocHuongBaoHiem))
                            {
                                YeuCauDichVuGiuongBenhVien dvGiuongBHYTCao = null;
                                YeuCauDichVuGiuongBenhVien dvGiuongBHYTThap = null;
                                var dvGiuongBHYTHon4Hs = dvGiuongBenhNhanTrongNgays.Where(o => o.DuocHuongBaoHiem &&
                                                                                          ((o.ThoiDiemKetThucSuDung != null && o.ThoiDiemKetThucSuDung.Value.Date == ngayPhatSinh ? o.ThoiDiemKetThucSuDung.Value : ngayPhatSinh.AddDays(1)) - (o.ThoiDiemBatDauSuDung.Value.Date < ngayPhatSinh ? ngayPhatSinh : o.ThoiDiemBatDauSuDung.Value)).TotalHours >= 4)
                                                                                    .OrderBy(o => o.DonGiaBaoHiem.GetValueOrDefault())
                                                                                    .ToList();
                                if (dvGiuongBHYTHon4Hs.Any())
                                {
                                    dvGiuongBHYTThap = dvGiuongBHYTHon4Hs.First();
                                    dvGiuongBHYTCao = dvGiuongBHYTHon4Hs.Last();
                                }
                                else
                                {
                                    dvGiuongBHYTThap = dvGiuongBenhNhanTrongNgays.Where(o => o.DuocHuongBaoHiem).OrderBy(o => o.DonGiaBaoHiem.GetValueOrDefault()).First();
                                    dvGiuongBHYTCao = dvGiuongBenhNhanTrongNgays.Where(o => o.DuocHuongBaoHiem).OrderBy(o => o.DonGiaBaoHiem.GetValueOrDefault()).Last();
                                }
                                if (dvGiuongBHYTThap.DichVuGiuongBenhVienId != dvGiuongBHYTCao.DichVuGiuongBenhVienId)
                                {
                                    var dichVuGiuongBenhVienChiPhiBenhVienCao = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTCao.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongBHYTCao.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongBHYTCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTCao.Ten,
                                        Ma = dvGiuongBHYTCao.Ma,
                                        MaTT37 = dvGiuongBHYTCao.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTCao.LoaiGiuong,
                                        MoTa = dvGiuongBHYTCao.MoTa,
                                        Gia = dvGiuongBHYTCao.Gia.Value,
                                        BaoPhong = dvGiuongBHYTCao.BaoPhong,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTCao.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    var dichVuGiuongBenhVienChiPhiBenhVienThap = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTThap.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTThap.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongBHYTThap.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongBHYTThap.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTThap.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTThap.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTThap.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTThap.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTThap.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTThap.Ten,
                                        Ma = dvGiuongBHYTThap.Ma,
                                        MaTT37 = dvGiuongBHYTThap.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTThap.LoaiGiuong,
                                        MoTa = dvGiuongBHYTThap.MoTa,
                                        Gia = dvGiuongBHYTThap.Gia.Value,
                                        BaoPhong = dvGiuongBHYTThap.BaoPhong,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTThap.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienCao);
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienThap);

                                    var dichVuGiuongBenhVienChiPhiBHYTCao = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTCao.DichVuGiuongBenhVienId,
                                        GiuongBenhId = dvGiuongBHYTCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTCao.Ten,
                                        Ma = dvGiuongBHYTCao.Ma,
                                        MaTT37 = dvGiuongBHYTCao.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTCao.LoaiGiuong,
                                        MoTa = dvGiuongBHYTCao.MoTa,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        DuocHuongBaoHiem = true,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTCao.GhiChu,
                                        DonGiaBaoHiem = dvGiuongBHYTCao.DonGiaBaoHiem.GetValueOrDefault(),
                                        MucHuongBaoHiem = dvGiuongBHYTCao.MucHuongBaoHiem.GetValueOrDefault(),
                                        TiLeBaoHiemThanhToan = 100,
                                        HeThongTuPhatSinh = true
                                    };
                                    var dichVuGiuongBenhVienChiPhiBHYTThap = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTThap.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTThap.DichVuGiuongBenhVienId,
                                        GiuongBenhId = dvGiuongBHYTThap.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTThap.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTThap.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTThap.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTThap.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTThap.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTThap.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTThap.Ten,
                                        Ma = dvGiuongBHYTThap.Ma,
                                        MaTT37 = dvGiuongBHYTThap.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTThap.LoaiGiuong,
                                        MoTa = dvGiuongBHYTThap.MoTa,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        DuocHuongBaoHiem = true,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTThap.GhiChu,
                                        DonGiaBaoHiem = dvGiuongBHYTThap.DonGiaBaoHiem.GetValueOrDefault(),
                                        MucHuongBaoHiem = dvGiuongBHYTThap.MucHuongBaoHiem.GetValueOrDefault(),
                                        TiLeBaoHiemThanhToan = 100,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTCao);
                                    yeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTThap);

                                    dichVuGiuongBenhVienChiPhiBenhVienCao.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTCao);
                                    dichVuGiuongBenhVienChiPhiBenhVienThap.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTThap);
                                }
                                else
                                {
                                    var dichVuGiuongBenhVienChiPhiBenhVienCao = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTCao.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongBHYTCao.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongBHYTCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTCao.Ten,
                                        Ma = dvGiuongBHYTCao.Ma,
                                        MaTT37 = dvGiuongBHYTCao.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTCao.LoaiGiuong,
                                        MoTa = dvGiuongBHYTCao.MoTa,
                                        Gia = dvGiuongBHYTCao.Gia.Value,
                                        BaoPhong = dvGiuongBHYTCao.BaoPhong,
                                        SoLuong = 1,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTCao.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienCao);

                                    var dichVuGiuongBenhVienChiPhiBHYTCao = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBHYTCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBHYTCao.DichVuGiuongBenhVienId,
                                        GiuongBenhId = dvGiuongBHYTCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongBHYTCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongBHYTCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien ?? dvGiuongBHYTCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBHYTCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBHYTCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBHYTCao.Ten,
                                        Ma = dvGiuongBHYTCao.Ma,
                                        MaTT37 = dvGiuongBHYTCao.MaTT37,
                                        LoaiGiuong = dvGiuongBHYTCao.LoaiGiuong,
                                        MoTa = dvGiuongBHYTCao.MoTa,
                                        SoLuong = 1,
                                        SoLuongGhep = 1,
                                        DuocHuongBaoHiem = true,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBHYTCao.GhiChu,
                                        DonGiaBaoHiem = dvGiuongBHYTCao.DonGiaBaoHiem.GetValueOrDefault(),
                                        MucHuongBaoHiem = dvGiuongBHYTCao.MucHuongBaoHiem.GetValueOrDefault(),
                                        TiLeBaoHiemThanhToan = 100,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTCao);

                                    dichVuGiuongBenhVienChiPhiBenhVienCao.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYTCao);
                                }
                            }
                            else
                            {
                                YeuCauDichVuGiuongBenhVien dvGiuongThap = dvGiuongBenhNhanTrongNgays.OrderBy(o => o.Gia).First();
                                YeuCauDichVuGiuongBenhVien dvGiuongCao = dvGiuongBenhNhanTrongNgays.OrderBy(o => o.Gia).Last();
                                if (dvGiuongThap.DichVuGiuongBenhVienId != dvGiuongCao.DichVuGiuongBenhVienId)
                                {
                                    var dichVuGiuongBenhVienChiPhiBenhVienCao = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongCao.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongCao.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongCao.GiuongBenh?.PhongBenhVien ?? dvGiuongCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongCao.Ten,
                                        Ma = dvGiuongCao.Ma,
                                        MaTT37 = dvGiuongCao.MaTT37,
                                        LoaiGiuong = dvGiuongCao.LoaiGiuong,
                                        MoTa = dvGiuongCao.MoTa,
                                        Gia = dvGiuongCao.Gia.Value,
                                        BaoPhong = dvGiuongCao.BaoPhong,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongCao.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    var dichVuGiuongBenhVienChiPhiBenhVienThap = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongThap.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongThap.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongThap.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongThap.GiuongBenhId,
                                        GiuongBenh = dvGiuongThap.GiuongBenh,
                                        PhongBenhVienId = dvGiuongThap.GiuongBenh?.PhongBenhVienId ?? dvGiuongThap.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongThap.GiuongBenh?.PhongBenhVien ?? dvGiuongThap.NoiChiDinh,
                                        KhoaPhongId = dvGiuongThap.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongThap.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongThap.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongThap.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongThap.Ten,
                                        Ma = dvGiuongThap.Ma,
                                        MaTT37 = dvGiuongThap.MaTT37,
                                        LoaiGiuong = dvGiuongThap.LoaiGiuong,
                                        MoTa = dvGiuongThap.MoTa,
                                        Gia = dvGiuongThap.Gia.Value,
                                        BaoPhong = dvGiuongThap.BaoPhong,
                                        SoLuong = 0.5,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongThap.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienCao);
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienThap);
                                }
                                else
                                {
                                    var dichVuGiuongBenhVienChiPhiBenhVienCao = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongCao.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongCao.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = dvGiuongCao.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongCao.GiuongBenhId,
                                        GiuongBenh = dvGiuongCao.GiuongBenh,
                                        PhongBenhVienId = dvGiuongCao.GiuongBenh?.PhongBenhVienId ?? dvGiuongCao.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongCao.GiuongBenh?.PhongBenhVien ?? dvGiuongCao.NoiChiDinh,
                                        KhoaPhongId = dvGiuongCao.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongCao.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongCao.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongCao.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongCao.Ten,
                                        Ma = dvGiuongCao.Ma,
                                        MaTT37 = dvGiuongCao.MaTT37,
                                        LoaiGiuong = dvGiuongCao.LoaiGiuong,
                                        MoTa = dvGiuongCao.MoTa,
                                        Gia = dvGiuongCao.Gia.Value,
                                        BaoPhong = dvGiuongCao.BaoPhong,
                                        SoLuong = 1,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongCao.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.BenhNhan,
                                        HeThongTuPhatSinh = true
                                    };
                                    yeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVienCao);
                                }
                            }
                        }
                    }

                    ngayPhatSinh = ngayPhatSinh.AddDays(1);
                }
            }
            //tinh giuong Nguoi Nha
            var dvGiuongNguoiNhas = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(o => o.ThoiDiemBatDauSuDung != null && o.DoiTuongSuDung == Enums.DoiTuongSuDung.NguoiNha).OrderBy(o => o.ThoiDiemBatDauSuDung).ToList();
            if (dvGiuongNguoiNhas.Count > 0)
            {
                var thoiDienBatDau = dvGiuongNguoiNhas.First().ThoiDiemBatDauSuDung.Value;
                var ngayPhatSinh = thoiDienBatDau.Date;
                while (ngayPhatSinh < ngayKetThuc)
                {
                    var thoiDienBatDauTheoNgay = ngayPhatSinh.AddSeconds((thoiDienBatDau - thoiDienBatDau.Date).TotalSeconds);
                    if (thoiDienBatDau.Date == ngayPhatSinh || thoiDienBatDauTheoNgay < ngayKetThuc)
                    {
                        var dvGiuongNguoiNhaTrongNgays = dvGiuongNguoiNhas
                        .Where(o => o.ThoiDiemBatDauSuDung.Value.Date <= ngayPhatSinh &&
                                    (o.ThoiDiemKetThucSuDung == null || o.ThoiDiemKetThucSuDung.Value.Date >= ngayPhatSinh)).OrderBy(o => o.ThoiDiemBatDauSuDung)
                        .ToList();
                        if (dvGiuongNguoiNhaTrongNgays.Count > 0)
                        {
                            var soLuongGiuongNguoiNha = 1;
                            for (int i = 1; i < dvGiuongNguoiNhaTrongNgays.Count; i++)
                            {
                                var soLuongGiuongNguoiNhaTheoIndex = 1;
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (dvGiuongNguoiNhaTrongNgays[i].ThoiDiemBatDauSuDung < dvGiuongNguoiNhaTrongNgays[j].ThoiDiemKetThucSuDung.GetValueOrDefault(ngayPhatSinh.AddDays(1)))
                                    {
                                        soLuongGiuongNguoiNhaTheoIndex += 1;
                                        if (soLuongGiuongNguoiNhaTheoIndex > soLuongGiuongNguoiNha)
                                        {
                                            soLuongGiuongNguoiNha += 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            yeuCauDichVuGiuongBenhVienChiPhiBenhViens.AddRange(
                                dvGiuongNguoiNhaTrongNgays
                                    .OrderByDescending(o =>
                                        (o.ThoiDiemKetThucSuDung != null && o.ThoiDiemKetThucSuDung.Value.Date == ngayPhatSinh ? o.ThoiDiemKetThucSuDung.Value : ngayPhatSinh.AddDays(1)) -
                                        (o.ThoiDiemBatDauSuDung.Value.Date < ngayPhatSinh ? ngayPhatSinh : o.ThoiDiemBatDauSuDung.Value))
                                    .Take(soLuongGiuongNguoiNha)
                                    .Select(dvGiuongBenhVien => new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                    {
                                        NgayPhatSinh = ngayPhatSinh,
                                        YeuCauTiepNhanId = dvGiuongBenhVien.YeuCauTiepNhanId,
                                        DichVuGiuongBenhVienId = dvGiuongBenhVien.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId =
                                            dvGiuongBenhVien.NhomGiaDichVuGiuongBenhVienId.Value,
                                        GiuongBenhId = dvGiuongBenhVien.GiuongBenhId,
                                        GiuongBenh = dvGiuongBenhVien.GiuongBenh,
                                        PhongBenhVienId = dvGiuongBenhVien.GiuongBenh?.PhongBenhVienId ?? dvGiuongBenhVien.NoiChiDinhId,
                                        PhongBenhVien = dvGiuongBenhVien.GiuongBenh?.PhongBenhVien ?? dvGiuongBenhVien.NoiChiDinh,
                                        KhoaPhongId = dvGiuongBenhVien.GiuongBenh?.PhongBenhVien.KhoaPhongId ?? dvGiuongBenhVien.NoiChiDinh.KhoaPhongId,
                                        KhoaPhong = dvGiuongBenhVien.GiuongBenh?.PhongBenhVien.KhoaPhong ?? dvGiuongBenhVien.NoiChiDinh.KhoaPhong,
                                        Ten = dvGiuongBenhVien.Ten,
                                        Ma = dvGiuongBenhVien.Ma,
                                        MaTT37 = dvGiuongBenhVien.MaTT37,
                                        LoaiGiuong = dvGiuongBenhVien.LoaiGiuong,
                                        MoTa = dvGiuongBenhVien.MoTa,
                                        Gia = dvGiuongBenhVien.Gia.Value,
                                        BaoPhong = dvGiuongBenhVien.BaoPhong,
                                        SoLuong = 1,
                                        SoLuongGhep = 1,
                                        TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = dvGiuongBenhVien.GhiChu,
                                        DoiTuongSuDung = Enums.DoiTuongSuDung.NguoiNha,
                                        HeThongTuPhatSinh = true
                                    }));
                        }
                    }
                    ngayPhatSinh = ngayPhatSinh.AddDays(1);
                }
            }
            ApDungGoiMarketingVaoDichVuGiuong(yeuCauTiepNhan.BenhNhanId.GetValueOrDefault(), yeuCauDichVuGiuongBenhVienChiPhiBenhViens);
            return (yeuCauDichVuGiuongBenhVienChiPhiBenhViens, yeuCauDichVuGiuongBenhVienChiPhiBHYTs);
        }

        private void ApDungGoiMarketingVaoDichVuGiuong(long benhNhanId, List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> yeuCauDichVuGiuongBenhVienChiPhiBenhViens)
        {
            BenhNhan benhNhan = BaseRepository.Context.Set<BenhNhan>().AsNoTracking()
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                .Include(o => o.YeuCauGoiDichVuSoSinhs).ThenInclude(o => o.ChuongTrinhGoiDichVu).ThenInclude(o => o.ChuongTrinhGoiDichVuDichVuGiuongs)
                .FirstOrDefault(o => o.Id == benhNhanId);
            if (benhNhan != null)
            {
                List<ThongTinSuDungGiuongTrongGoiDichVuVo> thongTinSuDungGiuongs = benhNhan.YeuCauGoiDichVus
                    .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && o.DaQuyetToan != true)
                    .Select(o => new ThongTinSuDungGiuongTrongGoiDichVuVo
                    {
                        YeuCauGoiDichVuId = o.Id,
                        DanhSachDichVuGiuongTrongGoi = o.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Select(
                                y => new ThongTinGiuongTrongGoiDichVuVo
                                {
                                    DichVuGiuongBenhVienId = y.DichVuGiuongBenhVienId,
                                    NhomGiaDichVuGiuongBenhVienId = y.NhomGiaDichVuGiuongBenhVienId,
                                    SoLan = y.SoLan,
                                    DonGia = y.DonGia,
                                    DonGiaTruocChietKhau = y.DonGiaTruocChietKhau,
                                    DonGiaSauChietKhau = y.DonGiaSauChietKhau,
                                })
                            .ToList(),
                        DanhSachDichVuGiuongDaSuDung = o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                            .Select(y => new ThongTinGiuongTrongGoiDichVuVo
                            {
                                DichVuGiuongBenhVienId = y.DichVuGiuongBenhVienId,
                                NhomGiaDichVuGiuongBenhVienId = y.NhomGiaDichVuGiuongBenhVienId,
                                SoLan = y.SoLuong,
                                DonGia = y.Gia,
                                DonGiaTruocChietKhau = y.DonGiaTruocChietKhau.GetValueOrDefault(),
                                DonGiaSauChietKhau = y.DonGiaSauChietKhau.GetValueOrDefault(),
                            })
                            .ToList()
                    }).ToList();
                thongTinSuDungGiuongs = thongTinSuDungGiuongs.Concat(benhNhan.YeuCauGoiDichVuSoSinhs
                        .Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy && o.DaQuyetToan != true)
                        .Select(o => new ThongTinSuDungGiuongTrongGoiDichVuVo
                        {
                            YeuCauGoiDichVuId = o.Id,
                            DanhSachDichVuGiuongTrongGoi = o.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Select(
                                    y => new ThongTinGiuongTrongGoiDichVuVo
                                    {
                                        DichVuGiuongBenhVienId = y.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = y.NhomGiaDichVuGiuongBenhVienId,
                                        SoLan = y.SoLan,
                                        DonGia = y.DonGia,
                                        DonGiaTruocChietKhau = y.DonGiaTruocChietKhau,
                                        DonGiaSauChietKhau = y.DonGiaSauChietKhau,
                                    })
                                .ToList(),
                            DanhSachDichVuGiuongDaSuDung = o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                                .Select(y => new ThongTinGiuongTrongGoiDichVuVo
                                {
                                    DichVuGiuongBenhVienId = y.DichVuGiuongBenhVienId,
                                    NhomGiaDichVuGiuongBenhVienId = y.NhomGiaDichVuGiuongBenhVienId,
                                    SoLan = y.SoLuong,
                                    DonGia = y.Gia,
                                    DonGiaTruocChietKhau = y.DonGiaTruocChietKhau.GetValueOrDefault(),
                                    DonGiaSauChietKhau = y.DonGiaSauChietKhau.GetValueOrDefault(),
                                })
                                .ToList()
                        }).ToList()).OrderBy(o=>o.YeuCauGoiDichVuId)
                    .ToList();

                foreach (var yeuCauDichVuGiuongBenhVienChiPhiBenhVien in yeuCauDichVuGiuongBenhVienChiPhiBenhViens)
                {
                    foreach (var thongTinSuDungGiuongTrongGoiDichVuVo in thongTinSuDungGiuongs)
                    {
                        var danhSachDichVuGiuongTrongGoi = thongTinSuDungGiuongTrongGoiDichVuVo.DanhSachDichVuGiuongTrongGoi
                            .Where(o => o.DichVuGiuongBenhVienId == yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId
                                        && o.NhomGiaDichVuGiuongBenhVienId == yeuCauDichVuGiuongBenhVienChiPhiBenhVien.NhomGiaDichVuGiuongBenhVienId);
                        if (danhSachDichVuGiuongTrongGoi.Select(o => o.SoLan).DefaultIfEmpty().Sum()
                            >= (thongTinSuDungGiuongTrongGoiDichVuVo.DanhSachDichVuGiuongDaSuDung
                                    .Where(o => o.DichVuGiuongBenhVienId == yeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                                    .DichVuGiuongBenhVienId
                                                && o.NhomGiaDichVuGiuongBenhVienId == yeuCauDichVuGiuongBenhVienChiPhiBenhVien
                                                    .NhomGiaDichVuGiuongBenhVienId).Select(o => o.SoLan)
                                    .DefaultIfEmpty().Sum()
                                + yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuong))
                        {
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTinSuDungGiuongTrongGoiDichVuVo.YeuCauGoiDichVuId;
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia = danhSachDichVuGiuongTrongGoi.First().DonGia;
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = danhSachDichVuGiuongTrongGoi.First().DonGiaTruocChietKhau;
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = danhSachDichVuGiuongTrongGoi.First().DonGiaSauChietKhau;
                            thongTinSuDungGiuongTrongGoiDichVuVo.DanhSachDichVuGiuongDaSuDung.Add(
                                    new ThongTinGiuongTrongGoiDichVuVo
                                    {
                                        DichVuGiuongBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId,
                                        NhomGiaDichVuGiuongBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.NhomGiaDichVuGiuongBenhVienId,
                                        SoLan = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuong,
                                        DonGia = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia,
                                        DonGiaTruocChietKhau = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau.GetValueOrDefault(),
                                        DonGiaSauChietKhau = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau.GetValueOrDefault(),
                                    }
                                );
                            break;
                        }
                    }
                }
            }
        }

        //private void AddKetQuaXetNghiemChiTiet(PhienXetNghiemChiTiet phienXetNghiemChiTiet, Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem dichVuXetNghiem, BenhNhan benhNhan, List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiems, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos, List<KetQuaXetNghiemChiTiet> ketQuaXetNghiemLanTruocs = null)
        //{
        //    var ketQuaXetNghiemChiTiet = new KetQuaXetNghiemChiTiet
        //    {
        //        BarCodeID = phienXetNghiemChiTiet.PhienXetNghiem.BarCodeId,
        //        BarCodeNumber = phienXetNghiemChiTiet.PhienXetNghiem.BarCodeNumber,
        //        YeuCauDichVuKyThuatId = phienXetNghiemChiTiet.YeuCauDichVuKyThuatId,
        //        DichVuKyThuatBenhVienId = phienXetNghiemChiTiet.DichVuKyThuatBenhVienId,
        //        NhomDichVuBenhVienId = phienXetNghiemChiTiet.NhomDichVuBenhVienId,
        //        LanThucHien = phienXetNghiemChiTiet.LanThucHien,
        //        DichVuXetNghiemId = dichVuXetNghiem.Id,
        //        DichVuXetNghiemChaId = dichVuXetNghiem.DichVuXetNghiemChaId,
        //        DichVuXetNghiemMa = dichVuXetNghiem.Ma,
        //        DichVuXetNghiemTen = dichVuXetNghiem.Ten,
        //        CapDichVu = dichVuXetNghiem.CapDichVu,
        //        DonVi = dichVuXetNghiem.DonVi,
        //        SoThuTu = dichVuXetNghiem.SoThuTu,
        //        ThoiDiemGuiYeuCau = DateTime.Now
        //    };

        //    var ketQuaCu = ketQuaXetNghiemLanTruocs?.FirstOrDefault(o =>
        //        o.DichVuXetNghiemId == ketQuaXetNghiemChiTiet.DichVuXetNghiemId);
        //    if (ketQuaCu != null)
        //    {
        //        ketQuaXetNghiemChiTiet.GiaTriCu = string.IsNullOrEmpty(ketQuaCu.GiaTriDuyet)
        //            ? (string.IsNullOrEmpty(ketQuaCu.GiaTriNhapTay) ? ketQuaCu.GiaTriTuMay : ketQuaCu.GiaTriNhapTay)
        //            : ketQuaCu.GiaTriDuyet;
        //    }

        //    var dichVuXetNghiemKetNoiChiSo = dichVuXetNghiemKetNoiChiSos
        //        .Where(o => o.DichVuXetNghiemId == dichVuXetNghiem.Id && o.HieuLuc).OrderBy(o => o.Id).LastOrDefault();
        //    if (dichVuXetNghiemKetNoiChiSo != null)
        //    {
        //        ketQuaXetNghiemChiTiet.DichVuXetNghiemKetNoiChiSoId = dichVuXetNghiemKetNoiChiSo.Id;
        //        ketQuaXetNghiemChiTiet.MaChiSo = dichVuXetNghiemKetNoiChiSo.MaChiSo;
        //        ketQuaXetNghiemChiTiet.TiLe = dichVuXetNghiemKetNoiChiSo.TiLe;
        //        ketQuaXetNghiemChiTiet.MauMayXetNghiemId = dichVuXetNghiemKetNoiChiSo.MauMayXetNghiemId;
        //    }

        //    if (benhNhan.NamSinh != null)
        //    {
        //        var ngaySinh = new DateTime(benhNhan.NamSinh.Value, benhNhan.ThangSinh == null || benhNhan.ThangSinh == 0 ? 1 : benhNhan.ThangSinh.Value, benhNhan.NgaySinh == null || benhNhan.NgaySinh == 0 ? 1 : benhNhan.NgaySinh.Value);
        //        int tuoi = DateTime.Now.Year - ngaySinh.Year;
        //        if (ngaySinh > DateTime.Now.AddYears(-tuoi)) tuoi--;

        //        if (tuoi < 6 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm6Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm6Max)))
        //        {
        //            ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm6Min;
        //            ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm6Max;
        //        }
        //        else if (tuoi >= 6 && tuoi < 12 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm612Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm612Max)))
        //        {
        //            ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm612Min;
        //            ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm612Max;
        //        }
        //        else if (tuoi >= 12 && tuoi < 18 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEm1218Min) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEm1218Max)))
        //        {
        //            ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEm1218Min;
        //            ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEm1218Max;
        //        }
        //        else if (tuoi < 18 && (!string.IsNullOrEmpty(dichVuXetNghiem.TreEmMin) || !string.IsNullOrEmpty(dichVuXetNghiem.TreEmMax)))
        //        {
        //            ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.TreEmMin;
        //            ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.TreEmMax;
        //        }
        //        else
        //        {
        //            if (benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
        //            {
        //                ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.NamMin;
        //                ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.NamMax;
        //            }
        //            else if (benhNhan.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu)
        //            {
        //                ketQuaXetNghiemChiTiet.GiaTriMin = dichVuXetNghiem.NuMin;
        //                ketQuaXetNghiemChiTiet.GiaTriMax = dichVuXetNghiem.NuMax;
        //            }
        //        }
        //    }
        //    ketQuaXetNghiemChiTiet.GiaTriNguyHiemMin = dichVuXetNghiem.NguyHiemMin;
        //    ketQuaXetNghiemChiTiet.GiaTriNguyHiemMax = dichVuXetNghiem.NguyHiemMax;

        //    phienXetNghiemChiTiet.KetQuaXetNghiemChiTiets.Add(ketQuaXetNghiemChiTiet);
        //    foreach (var dichVuXetNghiemCon in dichVuXetNghiems.Where(o => o.DichVuXetNghiemChaId == dichVuXetNghiem.Id))
        //    {
        //        AddKetQuaXetNghiemChiTiet(phienXetNghiemChiTiet, dichVuXetNghiemCon, benhNhan, dichVuXetNghiems, dichVuXetNghiemKetNoiChiSos, ketQuaXetNghiemLanTruocs);
        //    }
        //}
        //
    }

}
