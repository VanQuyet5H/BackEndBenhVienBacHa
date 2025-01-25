using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoDoanhThuNhaThuocs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using Camino.Core.Data;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;

namespace Camino.Services.BaoCaoDoanhThuNhaThuocService
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoDoanhThuNhaThuocService))]
    public class BaoCaoDoanhThuNhaThuocService : MasterFileService<TaiKhoanBenhNhanThu>, IBaoCaoDoanhThuNhaThuocService
    {
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTietRepository;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<DonVTYTThanhToan> _donVTYTThanhToanRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Template> _templateRepository;

        public BaoCaoDoanhThuNhaThuocService(IRepository<TaiKhoanBenhNhanThu> repository,
            IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository,
            IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
            IRepository<DonVTYTThanhToanChiTiet> donVTYTThanhToanChiTietRepository,
            IRepository<DonThuocThanhToan> donThuocThanhToanRepository,
            IRepository<DonVTYTThanhToan> donVTYTThanhToanRepository,
            IRepository<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<User> userRepository,
            IRepository<Template> templateRepository,
            IUserAgentHelper userAgentHelper) : base(repository)
        {
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _userRepository = userRepository;
            _userAgentHelper = userAgentHelper;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
            _templateRepository = templateRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _donVTYTThanhToanRepository = donVTYTThanhToanRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new BaoCaoDoanhThuNhaThuocVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuNhaThuocVo>(queryInfo.AdditionalSearchString);
            }
            DateTime tuNgay = DateTime.Now.AddDays(-1);
            DateTime denNgay = DateTime.Now;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var dataPhieuThu = BaseRepository.TableNoTracking
                .Where(x => x.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc && x.NgayThu >= tuNgay && x.NgayThu < denNgay)
                .Select(item => new BaoCaoDoanhThuNhaThuocDaTaVo()
                {
                    Id = item.Id,
                    BaoCaoDoanhThuNhaThuocDaTaChiVos = item.TaiKhoanBenhNhanChis.Select(o => new BaoCaoDoanhThuNhaThuocDaTaChiVo { DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId, DonVTYTThanhToanChiTietId = o.DonVTYTThanhToanChiTietId }).ToList(),
                    Ngay = item.NgayThu,
                    MaYTe = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhan = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    EnumGioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    TienMat = item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan,
                    Pos = item.POS,
                    CongNo = item.CongNo,
                    NguoiBan = item.NhanVienThucHien.User.HoTen,
                    GhiChu = item.GhiChu,                    
                    BaoCaoDoanhThuNhaThuocDaTaCongNoVos = item.CongTyBaoHiemTuNhanCongNos.Select(o => new BaoCaoDoanhThuNhaThuocDaTaCongNoVo { SoTien = o.SoTien, TenCongTy = o.CongTyBaoHiemTuNhan.Ten }).ToList(),
                    SoHoaDon = item.SoPhieuHienThi,
                    LyDoHuyBanThuoc = item.LyDoHuy
                }).ToList();

            var dataPhieuHuy = BaseRepository.TableNoTracking
                .Where(x => x.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc && x.DaHuy == true && x.NgayHuy >= tuNgay && x.NgayHuy < denNgay)
                .Select(item => new BaoCaoDoanhThuNhaThuocDaTaVo()
                {
                    Id = item.Id,
                    Ngay = item.NgayHuy,
                    MaYTe = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhan = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    EnumGioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    TienMat = item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan,
                    Pos = item.POS,
                    CongNo = item.CongNo,
                    NguoiBan = item.NhanVienThucHien.User.HoTen,
                    GhiChu = item.GhiChu,
                    BaoCaoDoanhThuNhaThuocDaTaCongNoVos = item.CongTyBaoHiemTuNhanCongNos.Select(o => new BaoCaoDoanhThuNhaThuocDaTaCongNoVo { SoTien = o.SoTien, TenCongTy = o.CongTyBaoHiemTuNhan.Ten }).ToList(),
                    SoHoaDon = item.SoPhieuHienThi,
                    LyDoHuyBanThuoc = item.LyDoHuy

                }).ToList();
            List<long> donThuocThanhToanChiTietIds = dataPhieuThu.SelectMany(o => o.BaoCaoDoanhThuNhaThuocDaTaChiVos).Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId.GetValueOrDefault()).ToList();
            List<long> donVTYTThanhToanChiTietIds = dataPhieuThu.SelectMany(o => o.BaoCaoDoanhThuNhaThuocDaTaChiVos).Where(o => o.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTietId.GetValueOrDefault()).ToList();

            var donThuocs = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => donThuocThanhToanChiTietIds.Contains(o.Id) && o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null)
                .Select(o => new { DonThuocThanhToanChiTietId = o.Id, DonThuocThanhToanId = o.DonThuocThanhToanId, o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu }).ToList();

            var donVTYTs = _donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o => donVTYTThanhToanChiTietIds.Contains(o.Id) && o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null)
                .Select(o => new { DonnVTYTThanhToanChiTietId = o.Id, DonVTYTThanhToanId = o.DonVTYTThanhToanId, o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu }).ToList();

            var donThuocThanhToanIds = donThuocs.Select(o=>o.DonThuocThanhToanId).Distinct().ToList();
            var donVTYTThanhToanIds = donVTYTs.Select(o => o.DonVTYTThanhToanId).Distinct().ToList();

            var donThuocThanhToans = _donThuocThanhToanRepository.TableNoTracking
                .Where(o => donThuocThanhToanIds.Contains(o.Id))
                .Select(o => new
                { 
                    o.Id, 
                    KhamBenhNoiKeDonId = o.YeuCauKhamBenhDonThuoc != null ? o.YeuCauKhamBenhDonThuoc.NoiKeDonId : 0, 
                    NoiTruNoiKeDonId = o.NoiTruDonThuoc != null ? o.NoiTruDonThuoc.NoiKeDonId : 0
                })
                .ToList();

            var donVTYTThanhToans = _donVTYTThanhToanRepository.TableNoTracking
                .Where(o => donVTYTThanhToanIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    KhamBenhNoiKeDonId = o.YeuCauKhamBenhDonVTYT != null ? o.YeuCauKhamBenhDonVTYT.NoiKeDonId : 0
                })
                .ToList();

            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.Select(o => new { o.Id, KhoaPhong = o.KhoaPhong.Ten }).ToList();

            var gridData = new List<BaoCaoDoanhThuNhaThuocGridVo>();
            foreach (var item in dataPhieuThu)
            {
                var baoCaoDoanhThuNhaThuocGridVo = new BaoCaoDoanhThuNhaThuocGridVo()
                {
                    Id = item.Id,
                    Ngay = item.Ngay,
                    MaYTe = item.MaYTe,
                    BenhNhan = item.BenhNhan,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.EnumGioiTinh?.GetDescription(),
                    TienMat = item.TienMat == 0 ? null : item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan == 0 ? null : item.ChuyenKhoan,
                    Pos = item.Pos == 0 ? null : item.Pos,
                    NguoiBan = item.NguoiBan,
                    GhiChu = item.GhiChu,
                    SoHoaDon = item.SoHoaDon,
                    LyDoHuyBanThuoc = item.LyDoHuyBanThuoc,
                };
                var soPhieus = donThuocs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonThuocThanhToanChiTietId).Contains(o.DonThuocThanhToanChiTietId)).Select(o => o.SoPhieu).Distinct().ToList();
                soPhieus.AddRange(donVTYTs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonVTYTThanhToanChiTietId).Contains(o.DonnVTYTThanhToanChiTietId)).Select(o => o.SoPhieu).Distinct().ToList());
                if (soPhieus.Any())
                {
                    baoCaoDoanhThuNhaThuocGridVo.SoChungTu = string.Join(";", soPhieus);
                }
                baoCaoDoanhThuNhaThuocGridVo.CongNo = item.CongNo.GetValueOrDefault() + item.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.SoTien).DefaultIfEmpty().Sum();
                baoCaoDoanhThuNhaThuocGridVo.CongNo = baoCaoDoanhThuNhaThuocGridVo.CongNo == 0 ? null : baoCaoDoanhThuNhaThuocGridVo.CongNo;
                var chiTietCongNos = item.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.TenCongTy).Distinct().ToList();
                if (item.CongNo.GetValueOrDefault() > 0)
                {
                    chiTietCongNos.Insert(0, baoCaoDoanhThuNhaThuocGridVo.BenhNhan);
                }
                if (chiTietCongNos.Any())
                {
                    baoCaoDoanhThuNhaThuocGridVo.ChiTietCongNo = string.Join("; ", chiTietCongNos);
                }

                var phieuThuDonThuocThanhToanIds = donThuocs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonThuocThanhToanChiTietId).Contains(o.DonThuocThanhToanChiTietId)).Select(o => o.DonThuocThanhToanId).Distinct().ToList();
                var phieuThuDonVTYTThanhToanIds = donVTYTs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonVTYTThanhToanChiTietId).Contains(o.DonnVTYTThanhToanChiTietId)).Select(o => o.DonVTYTThanhToanId).Distinct().ToList();

                var noiChiDinhs = new List<string>();
                foreach (var phieuThuDonThuocThanhToanId in phieuThuDonThuocThanhToanIds)
                {
                    var donThuocThanhToan = donThuocThanhToans.FirstOrDefault(o => o.Id == phieuThuDonThuocThanhToanId);
                    if(donThuocThanhToan != null)
                    {
                        if(donThuocThanhToan.KhamBenhNoiKeDonId != 0)
                        {
                            noiChiDinhs.Add(phongBenhViens.First(o => o.Id == donThuocThanhToan.KhamBenhNoiKeDonId).KhoaPhong);
                        }
                        else if (donThuocThanhToan.NoiTruNoiKeDonId != 0)
                        {
                            noiChiDinhs.Add(phongBenhViens.First(o => o.Id == donThuocThanhToan.NoiTruNoiKeDonId).KhoaPhong);
                        }
                        else
                        {
                            noiChiDinhs.Add("Khoa Dược");
                        }
                    }
                }
                foreach (var phieuThuDonVTYTThanhToanId in phieuThuDonVTYTThanhToanIds)
                {
                    var donVTYTThanhToan = donVTYTThanhToans.FirstOrDefault(o => o.Id == phieuThuDonVTYTThanhToanId);
                    if (donVTYTThanhToan != null)
                    {
                        if (donVTYTThanhToan.KhamBenhNoiKeDonId != 0)
                        {
                            noiChiDinhs.Add(phongBenhViens.First(o => o.Id == donVTYTThanhToan.KhamBenhNoiKeDonId).KhoaPhong);
                        }
                        else
                        {
                            noiChiDinhs.Add("Khoa Dược");
                        }
                    }
                }
                baoCaoDoanhThuNhaThuocGridVo.KhoaChiDinh = string.Join("; ", noiChiDinhs.Distinct());

                gridData.Add(baoCaoDoanhThuNhaThuocGridVo);
            }
            foreach (var item in dataPhieuHuy)
            {
                var baoCaoDoanhThuNhaThuocGridVo = new BaoCaoDoanhThuNhaThuocGridVo()
                {
                    Id = item.Id,
                    Ngay = item.Ngay,
                    MaYTe = item.MaYTe,
                    BenhNhan = item.BenhNhan,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.EnumGioiTinh?.GetDescription(),
                    TienMat = item.TienMat == 0 ? null : item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan == 0 ? null : item.ChuyenKhoan,
                    Pos = item.Pos == 0 ? null : item.Pos,
                    NguoiBan = item.NguoiBan,
                    GhiChu = item.GhiChu,
                    SoHoaDon = item.SoHoaDon,
                    LyDoHuyBanThuoc = item.LyDoHuyBanThuoc,
                };

                baoCaoDoanhThuNhaThuocGridVo.CongNo = item.CongNo.GetValueOrDefault() + item.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.SoTien).DefaultIfEmpty().Sum();
                baoCaoDoanhThuNhaThuocGridVo.CongNo = baoCaoDoanhThuNhaThuocGridVo.CongNo == 0 ? null : baoCaoDoanhThuNhaThuocGridVo.CongNo;
                var chiTietCongNos = item.BaoCaoDoanhThuNhaThuocDaTaCongNoVos.Select(o => o.TenCongTy).Distinct().ToList();
                if (item.CongNo.GetValueOrDefault() > 0)
                {
                    chiTietCongNos.Insert(0, baoCaoDoanhThuNhaThuocGridVo.BenhNhan);
                }
                if (chiTietCongNos.Any())
                {
                    baoCaoDoanhThuNhaThuocGridVo.ChiTietCongNo = string.Join("; ", chiTietCongNos);
                }
                baoCaoDoanhThuNhaThuocGridVo.TienMat = baoCaoDoanhThuNhaThuocGridVo.TienMat != null ? baoCaoDoanhThuNhaThuocGridVo.TienMat * (-1) : null;
                baoCaoDoanhThuNhaThuocGridVo.ChuyenKhoan = baoCaoDoanhThuNhaThuocGridVo.ChuyenKhoan != null ? baoCaoDoanhThuNhaThuocGridVo.ChuyenKhoan * (-1) : null;
                baoCaoDoanhThuNhaThuocGridVo.Pos = baoCaoDoanhThuNhaThuocGridVo.Pos != null ? baoCaoDoanhThuNhaThuocGridVo.Pos * (-1) : null;
                baoCaoDoanhThuNhaThuocGridVo.CongNo = baoCaoDoanhThuNhaThuocGridVo.CongNo != null ? baoCaoDoanhThuNhaThuocGridVo.CongNo * (-1) : null;

                gridData.Add(baoCaoDoanhThuNhaThuocGridVo);
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);
            if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
            {
                gridData = gridData
                    .Where(o => (o.SoChungTu != null && o.SoChungTu.ToLower().Contains(timKiemNangCaoObj.SearchString.ToLower())) ||
                                (o.SoHoaDon != null && o.SoHoaDon.ToLower().Contains(timKiemNangCaoObj.SearchString.ToLower())) ||
                                (o.BenhNhan != null && o.BenhNhan.ConvertToUnSign().ToLower().Contains(timKiemNangCaoObj.SearchString.ConvertToUnSign().ToLower())))
                    .ToList();
            }
            return new GridDataSource
            {
                Data = gridData.OrderBy(o => o.Ngay).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(),
                TotalRowCount = gridData.Count
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new BaoCaoDoanhThuNhaThuocVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuNhaThuocVo>(queryInfo.AdditionalSearchString);
            }
            DateTime tuNgay = DateTime.Now.AddDays(-1);
            DateTime denNgay = DateTime.Now;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var dataPhieuThu = BaseRepository.TableNoTracking
                .Where(x => x.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc && x.NgayThu >= tuNgay && x.NgayThu < denNgay)
                .Select(item => new BaoCaoDoanhThuNhaThuocDaTaVo()
                {
                    Id = item.Id,
                    BaoCaoDoanhThuNhaThuocDaTaChiVos = item.TaiKhoanBenhNhanChis.Select(o => new BaoCaoDoanhThuNhaThuocDaTaChiVo { DonThuocThanhToanChiTietId = o.DonThuocThanhToanChiTietId, DonVTYTThanhToanChiTietId = o.DonVTYTThanhToanChiTietId }).ToList(),
                    Ngay = item.NgayThu,
                    MaYTe = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhan = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    EnumGioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    GhiChu = item.NoiDungThu,
                    SoHoaDon = item.SoPhieuHienThi,

                }).ToList();

            var dataPhieuHuy = BaseRepository.TableNoTracking
                .Where(x => x.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc && x.DaHuy == true && x.NgayHuy >= tuNgay && x.NgayHuy < denNgay)
                .Select(item => new BaoCaoDoanhThuNhaThuocDaTaVo()
                {
                    Id = item.Id,
                    Ngay = item.NgayHuy,
                    MaYTe = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    BenhNhan = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    EnumGioiTinh = item.YeuCauTiepNhan.GioiTinh,
                    GhiChu = item.NoiDungThu,
                    SoHoaDon = item.SoPhieuHienThi,
                }).ToList();
            List<long> donThuocThanhToanChiTietIds = dataPhieuThu.SelectMany(o => o.BaoCaoDoanhThuNhaThuocDaTaChiVos).Where(o => o.DonThuocThanhToanChiTietId != null).Select(o => o.DonThuocThanhToanChiTietId.GetValueOrDefault()).ToList();
            List<long> donVTYTThanhToanChiTietIds = dataPhieuThu.SelectMany(o => o.BaoCaoDoanhThuNhaThuocDaTaChiVos).Where(o => o.DonVTYTThanhToanChiTietId != null).Select(o => o.DonVTYTThanhToanChiTietId.GetValueOrDefault()).ToList();

            var donThuocs = _donThuocThanhToanChiTietRepository.TableNoTracking.Where(o => donThuocThanhToanChiTietIds.Contains(o.Id) && o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null)
                .Select(o => new { DonThuocThanhToanChiTietId = o.Id, o.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu }).ToList();

            var donVTYTs = _donVTYTThanhToanChiTietRepository.TableNoTracking.Where(o => donVTYTThanhToanChiTietIds.Contains(o.Id) && o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null)
                .Select(o => new { DonnVTYTThanhToanChiTietId = o.Id, o.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu }).ToList();

            var gridData = new List<BaoCaoDoanhThuNhaThuocGridVo>();
            foreach (var item in dataPhieuThu)
            {
                var baoCaoDoanhThuNhaThuocGridVo = new BaoCaoDoanhThuNhaThuocGridVo()
                {
                    Id = item.Id,
                    Ngay = item.Ngay,
                    MaYTe = item.MaYTe,
                    BenhNhan = item.BenhNhan,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.EnumGioiTinh?.GetDescription(),
                    TienMat = item.TienMat == 0 ? null : item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan == 0 ? null : item.ChuyenKhoan,
                    Pos = item.Pos == 0 ? null : item.Pos,
                    NguoiBan = item.NguoiBan,
                    GhiChu = item.GhiChu,
                    SoHoaDon = item.SoHoaDon
                };
                var soPhieus = donThuocs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonThuocThanhToanChiTietId).Contains(o.DonThuocThanhToanChiTietId)).Select(o => o.SoPhieu).Distinct().ToList();
                soPhieus.AddRange(donVTYTs.Where(o => item.BaoCaoDoanhThuNhaThuocDaTaChiVos.Select(chi => chi.DonVTYTThanhToanChiTietId).Contains(o.DonnVTYTThanhToanChiTietId)).Select(o => o.SoPhieu).Distinct().ToList());
                if (soPhieus.Any())
                {
                    baoCaoDoanhThuNhaThuocGridVo.SoChungTu = string.Join(";", soPhieus);
                }

                gridData.Add(baoCaoDoanhThuNhaThuocGridVo);
            }
            foreach (var item in dataPhieuHuy)
            {
                var baoCaoDoanhThuNhaThuocGridVo = new BaoCaoDoanhThuNhaThuocGridVo()
                {
                    Id = item.Id,
                    Ngay = item.Ngay,
                    MaYTe = item.MaYTe,
                    BenhNhan = item.BenhNhan,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.EnumGioiTinh?.GetDescription(),
                    TienMat = item.TienMat == 0 ? null : item.TienMat,
                    ChuyenKhoan = item.ChuyenKhoan == 0 ? null : item.ChuyenKhoan,
                    Pos = item.Pos == 0 ? null : item.Pos,
                    NguoiBan = item.NguoiBan,
                    GhiChu = item.GhiChu,
                    SoHoaDon = item.SoHoaDon
                };

                gridData.Add(baoCaoDoanhThuNhaThuocGridVo);
            }

            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);
            if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
            {
                gridData = gridData
                    .Where(o => (o.SoChungTu != null && o.SoChungTu.ToLower().Contains(timKiemNangCaoObj.SearchString.ToLower())) ||
                                (o.SoHoaDon != null && o.SoHoaDon.ToLower().Contains(timKiemNangCaoObj.SearchString.ToLower())) ||
                                (o.BenhNhan != null && o.BenhNhan.ConvertToUnSign().ToLower().Contains(timKiemNangCaoObj.SearchString.ConvertToUnSign().ToLower())))
                    .ToList();
            }
            return new GridDataSource { TotalRowCount = gridData.Count };
        }

        //public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        //{
        //    var timKiemNangCaoObj = new BaoCaoDoanhThuNhaThuocVo();
        //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
        //    {
        //        timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuNhaThuocVo>(queryInfo.AdditionalSearchString);
        //    }

        //    var query = BaseRepository.TableNoTracking
        //        .Where(x => x.DaHuy != true
        //                    && x.LoaiNoiThu == Enums.LoaiNoiThu.NhaThuoc)
        //        .Select(item => new BaoCaoDoanhThuNhaThuocGridVo()
        //        {
        //            Id = item.Id,
        //            SoChungTu = item.TaiKhoanBenhNhanChis.Any(a => a.DaHuy != true
        //                                                             && (a.DonThuocThanhToanChiTiet != null
        //                                                             && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri != null
        //                                                             && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet != null
        //                                                             && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
        //                                                             || (a.DonVTYTThanhToanChiTiet != null
        //                                                                 && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri != null
        //                                                                 && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet != null
        //                                                                 && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null))
        //                ? item.TaiKhoanBenhNhanChis.Where(a => a.DaHuy != true
        //                                                     && (a.DonThuocThanhToanChiTiet != null
        //                                                         && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri != null
        //                                                         && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet != null
        //                                                         && a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null)
        //                                                     || (a.DonVTYTThanhToanChiTiet != null
        //                                                         && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri != null
        //                                                         && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet != null
        //                                                         && a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu != null))
        //                    .Select(a => a.DonThuocThanhToanChiTiet != null
        //                        ? a.DonThuocThanhToanChiTiet.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu
        //                        : a.DonVTYTThanhToanChiTiet.XuatKhoVatTuChiTietViTri.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu).FirstOrDefault()
        //                : "",
        //            Ngay = item.NgayThu,
        //            MaYTe = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
        //            BenhNhan = item.YeuCauTiepNhan.HoTen,
        //            NgaySinh = item.YeuCauTiepNhan.NgaySinh,
        //            ThangSinh = item.YeuCauTiepNhan.ThangSinh,
        //            NamSinh = item.YeuCauTiepNhan.NamSinh,
        //            GioiTinh = item.YeuCauTiepNhan.GioiTinh.GetDescription(),
        //            TienMat = (item.TienMat == null || item.TienMat == 0) ? null : item.TienMat,
        //            ChuyenKhoan = (item.ChuyenKhoan == null || item.ChuyenKhoan == 0) ? null : item.ChuyenKhoan,
        //            Pos = (item.POS == null || item.POS == 0) ? null : item.POS,
        //            CongNo = (item.CongNo ?? 0) + (item.CongTyBaoHiemTuNhanCongNos.Any(a => a.DaHuy != true) ? item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true).Sum(a => a.SoTien) : 0),
        //            NguoiBan = item.NhanVienThucHien.User.HoTen,
        //            GhiChu = item.NoiDungThu,
        //            ChiTietCongNo = ((item.CongNo != null && item.CongNo != 0) || item.CongTyBaoHiemTuNhanCongNos.Any()) ?
        //                (!string.IsNullOrEmpty(item.YeuCauTiepNhan.HoTen) ? item.YeuCauTiepNhan.HoTen + "; " : "") + string.Join("; ", item.CongTyBaoHiemTuNhanCongNos.Select(x => x.CongTyBaoHiemTuNhan.Ten).Distinct()) : "",
        //            SoHoaDon = item.SoPhieuHienThi
        //        }).ApplyLike(timKiemNangCaoObj.SearchString, g => g.SoChungTu, g => g.BenhNhan, g => g.SoHoaDon, g => g.SoChungTu);

        //    DateTime? tuNgay = null;
        //    DateTime? denNgay = null;
        //    //kiểm tra tìm kiếm nâng cao
        //    if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
        //    {
        //        DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
        //        tuNgay = tuNgayTemp;

        //        query = query.Where(x => x.Ngay >= tuNgay);
        //    }

        //    if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
        //    {
        //        DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
        //        denNgay = denNgayTemp;

        //        query = query.Where(x => x.Ngay <= denNgay);
        //    }

        //    var countTask = query.CountAsync();
        //    await Task.WhenAll(countTask);
        //    return new GridDataSource { TotalRowCount = countTask.Result };
        //}       
        #endregion

        #region Excel

        public virtual byte[] ExportBaoCaoDoanhThuNhaThuoc(ICollection<BaoCaoDoanhThuNhaThuocGridVo> datas, QueryInfo query)
        {
            int indexSTT = 1;
            var ngayHienTai = DateTime.Now.Day;
            var thangHienTai = DateTime.Now.Month;
            var namHienTai = DateTime.Now.Year;
            var requestProperties = new[]
            {
                new PropertyByName<BaoCaoDoanhThuNhaThuocGridVo>("STT", p => indexSTT++)
            };

            var timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuNhaThuocVo>(query.AdditionalSearchString);
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            var hostingName = timKiemNangCaoObj.HostingName;

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DOANH THU NHÀ THUỐC");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 25;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 20;
                    worksheet.Column(15).Width = 25;
                    worksheet.Column(16).Width = 20;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 25;
                    worksheet.DefaultColWidth = 7;

                    //SET img 
                    //using (var range = worksheet.Cells["A1:C1"])
                    //{
                    //    var url = hostingName + "/assets/img/logo-bacha-full.png";
                    //    WebClient wc = new WebClient();
                    //    byte[] bytes = wc.DownloadData(url); // download file từ server
                    //    MemoryStream ms = new MemoryStream(bytes); //
                    //    Image img = Image.FromStream(ms); // chuyển đổi thành img
                    //    ExcelPicture pic = range.Worksheet.Drawings.AddPicture("Logo", img);
                    //    pic.SetPosition(0, 0, 0, 0);
                    //    var height = 120; // chiều cao từ A1 đến A6
                    //    var width = 510; // chiều rộng từ A1 đến D1
                    //    pic.SetSize(width, height);
                    //    range.Worksheet.Protection.IsProtected = false;
                    //    range.Worksheet.Protection.AllowSelectLockedCells = false;
                    //}

                    // đổi logo thành tên bệnh viện
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }



                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A7:Q7"])
                    {
                        range.Worksheet.Cells["A7:R7"].Merge = true;
                        range.Worksheet.Cells["A7:R7"].Value = "DOANH THU NHÀ THUỐC";
                        range.Worksheet.Cells["A7:R7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A7:R7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:R7"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A7:R7"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A7:R7"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A9:R9"])
                    {
                        range.Worksheet.Cells["A9:R9"].Merge = true;
                        range.Worksheet.Cells["A9:R9"].Value = "Thời gian từ: " + (tuNgay == null ? "" : ((DateTime)tuNgay).FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến " + (denNgay == null ? "" : ((DateTime)denNgay).FormatNgayGioTimKiemTrenBaoCao()));
                        range.Worksheet.Cells["A9:R9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A9:R9"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A9:R9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9:R9"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A9:R9"].Style.Font.Italic = true;
                    }

                    using (var range = worksheet.Cells["A11:R11"])
                    {
                        range.Worksheet.Cells["A11:R11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A11:R11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A11:R11"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A11:R11"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A11:R11"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A11:R11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A11"].Value = "STT";
                        range.Worksheet.Cells["B11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B11"].Value = "Số chứng từ";
                        range.Worksheet.Cells["C11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C11"].Value = "Ngày";
                        range.Worksheet.Cells["D11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D11"].Value = "Mã Y Tế";
                        range.Worksheet.Cells["E11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E11"].Value = "Tên Người Bệnh";
                        range.Worksheet.Cells["F11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F11"].Value = "NS";
                        range.Worksheet.Cells["G11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G11"].Value = "GT";
                        range.Worksheet.Cells["H11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H11"].Value = "Khoa chỉ định" ;
                        range.Worksheet.Cells["I11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I11"].Value = "Thành tiền";
                        range.Worksheet.Cells["J11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J11"].Value = "Tiền mặt";
                        range.Worksheet.Cells["K11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K11"].Value = "Chuyển khoản";
                        range.Worksheet.Cells["L11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L11"].Value = "Pos";
                        range.Worksheet.Cells["M11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["M11"].Value = "Công nợ";
                        range.Worksheet.Cells["N11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["N11"].Value = "Người bán";
                        range.Worksheet.Cells["O11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["O11"].Value = "Ghi chú";
                        range.Worksheet.Cells["P11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["P11"].Value = "Chi tiết Công nợ";
                        range.Worksheet.Cells["Q11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["Q11"].Value = "Số Hóa đơn";
                        range.Worksheet.Cells["R11"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["R11"].Value = "Lý do hủy bán thuốc";
                    }

                    var manager = new PropertyManager<BaoCaoDoanhThuNhaThuocGridVo>(requestProperties);
                    int index = 12; // bắt đầu đổ data từ dòng 12

                    ///////Đổ data vào bảng excel

                    var numberFormat = "#,##0.00";
                    foreach (var data in datas)
                    {
                        manager.CurrentObject = data;
                        //// format border, font chữ,....
                        worksheet.Cells["A" + index + ":P" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":P" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;
                        manager.WriteToXlsx(worksheet, index);

                        // Đổ data
                        worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                     
                        worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = data.SoChungTu;
                        worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Value = data.Ngay.Value.ToString("dd/MM/yyyy/ HH:mm:ss");
                        worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["D" + index].Value = data.MaYTe;
                        worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["E" + index].Value = data.BenhNhan;
                        worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["F" + index].Value = data.NgayThangnamSinh;
                        worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["G" + index].Value = data.GioiTinh;

                        worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);                     
                        worksheet.Cells["H" + index].Value = data.KhoaChiDinh;                                      

                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Value = data.ThanhTien;

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Value = data.TienMat;

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Value = data.ChuyenKhoan;

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Value = data.Pos;

                        worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["M" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["M" + index].Value = data.CongNo;

                        worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["N" + index].Value = data.NguoiBan;

                        worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["O" + index].Value = data.GhiChu;

                        worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["P" + index].Value = data.ChiTietCongNo;

                        worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["Q" + index].Value = data.SoHoaDon;

                        worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["R" + index].Value = data.LyDoHuyBanThuoc;
                        
                        index++;
                    }

                    var tongTien = datas.Sum(x => x.ThanhTien);
                    var tienMat = datas.Sum(x => x.TienMat);
                    var chuyenKhoan = datas.Sum(x => x.ChuyenKhoan);
                    var pos = datas.Sum(x => x.Pos);
                    var congNo = datas.Sum(x => x.CongNo);

                    #region row tổng cộng
                    worksheet.Cells["A" + index + ":R" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A" + index + ":R" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A" + index + ":R" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    worksheet.Cells["A" + index + ":R" + index].Style.Font.Color.SetColor(Color.Black);
                    worksheet.Cells["A" + index + ":R" + index].Style.Font.Bold = true;
                    worksheet.Cells["A" + index + ":R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["B" + index].Value = "Tổng tiền:";

                    worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["I" + index].Style.Numberformat.Format = numberFormat;
                    worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["I" + index].Value = tongTien ;

                    worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["J" + index].Style.Numberformat.Format = numberFormat;
                    worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["J" + index].Value = tienMat ;

                    worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["K" + index].Style.Numberformat.Format = numberFormat;
                    worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["K" + index].Value = chuyenKhoan ;

                    worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["L" + index].Style.Numberformat.Format = numberFormat;
                    worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["L" + index].Value = pos;

                    worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    worksheet.Cells["M" + index].Style.Numberformat.Format = numberFormat;
                    worksheet.Cells["M" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["M" + index].Value = congNo;

                    worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    index += 2;

                    #endregion

                    #region row tổng hợp

                    #region row tổng tiền
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = "Tổng tiền";
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["C" + index].Value = tongTien;
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index++;
                    }
                    #endregion

                    #region row tiền mặt
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = "Tiền mặt";
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["C" + index].Value = tienMat;
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["N" + index + ":Q" + index].Merge = true;
                        worksheet.Cells["N" + index + ":Q" + index].Value = string.Format("Ngày {0} tháng {1} năm {2}", ngayHienTai, thangHienTai, namHienTai);
                        worksheet.Cells["N" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Italic = true;
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Bold = true;

                        //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["L" + index].Value = data.CongNo?.ApplyFormatMoneyToDecimal();
                        //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["M" + index].Value = data.NguoiBan;
                        //worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["N" + index].Value = data.GhiChu;
                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index++;
                    }
                    #endregion

                    #region row chuyển khoản
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        // worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = "Chuyển khoản";
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["C" + index].Value = chuyenKhoan;
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["N" + index + ":Q" + index].Merge = true;
                        worksheet.Cells["N" + index + ":Q" + index].Value = "Người lập";
                        worksheet.Cells["N" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        //worksheet.Cells["L" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Bold = true;

                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index++;
                    }
                    #endregion

                    #region row pos
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        // worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = "Pos";
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["C" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["C" + index].Value = pos;
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        worksheet.Cells["N" + index + ":Q" + index].Merge = true;
                        worksheet.Cells["N" + index + ":Q" + index].Value = "(Ký, ghi rõ họ tên)";
                        worksheet.Cells["N" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        //worksheet.Cells["L" + index + ":N" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Italic = true;

                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index++;
                    }
                    #endregion

                    #region row công nợ
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["B" + index].Value = "Công nợ";
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["C" + index].Style.Numberformat.Format = numberFormat;
                        worksheet.Cells["C" + index].Value = congNo;
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index += 3;
                    }
                    #endregion

                    #region row tên nhân viên
                    using (var range = worksheet.Cells["A" + index + ":Q" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Bold = true;
                        //range.Worksheet.Cells["A" + index + ":P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        //worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        var tenNhanVien = _userRepository.GetById(_userAgentHelper.GetCurrentUserId());
                        worksheet.Cells["N" + index + ":Q" + index].Merge = true;
                        worksheet.Cells["N" + index + ":Q" + index].Value = tenNhanVien.HoTen;
                        worksheet.Cells["N" + index + ":Q" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["N" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["N" + index + ":Q" + index].Style.Font.Bold = true;

                        //worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        index++;
                    }
                    #endregion

                    #endregion

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        public virtual string InBaoCaoDoanhThuNhaThuoc(ICollection<BaoCaoDoanhThuNhaThuocGridVo> datas, QueryInfo query)
        {

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuInDoanThuNhaThuoc"));

            int stt = 1;
            var tableHTML = string.Empty;
            var content = string.Empty;

            var timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoDoanhThuNhaThuocVo>(query.AdditionalSearchString);
            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            var thoiGianTimKiem = "Thời gian từ: " + (tuNgay == null ? "" : ((DateTime)tuNgay).FormatNgayGioTimKiemTrenBaoCao()
                                                           + " - đến " + (denNgay == null ? "" : ((DateTime)denNgay).FormatNgayGioTimKiemTrenBaoCao()));


            var tongTien = ((double)datas.Sum(c => c.ThanhTien)).ApplyFormatMoneyToDouble();
            var tienMat = ((double)datas.Sum(c => c.TienMat)).ApplyFormatMoneyToDouble();
            var chuyenKhoan = ((double)datas.Sum(c => c.ChuyenKhoan)).ApplyFormatMoneyToDouble();
            var pos = ((double)datas.Sum(c => c.Pos)).ApplyFormatMoneyToDouble();
            var congNo = ((double)datas.Sum(c => c.CongNo)).ApplyFormatMoneyToDouble();

            foreach (var item in datas)
            {
                tableHTML += "<tr style='border: 1px solid #020000;'>" +
                                      $"<td>{stt}</td>" +
                                      $"<td>{item.SoChungTu}</td>" +
                                      $"<td>{item.NgayDisplay}</td>" +
                                      $"<td>{item.MaYTe} </td>" +
                                      $"<td>{item.BenhNhan} </td>" +
                                      $"<td>{item.NgayThangnamSinh} </td>" +
                                      $"<td>{item.GioiTinh} </td>" +

                                      $"<td style='text-align: right;'>{((double)item.ThanhTien).ApplyFormatMoneyToDouble()} </td>" +
                                      $"<td style='text-align: right;'>{((double)(item.TienMat ?? 0)).ApplyFormatMoneyToDouble()} </td>" +
                                      $"<td style='text-align: right;'>{((double)(item.ChuyenKhoan ?? 0)).ApplyFormatMoneyToDouble()} </td>" +
                                      $"<td style='text-align: right;'>{((double)(item.Pos ?? 0)).ApplyFormatMoneyToDouble()} </td>" +
                                      $"<td style='text-align: right;'>{((double)(item.CongNo ?? 0)).ApplyFormatMoneyToDouble()} </td>" +

                                      $"<td>{item.NguoiBan} </td>" +
                                      $"<td>{item.GhiChu} </td>" +
                                      $"<td>{item.ChiTietCongNo} </td>" +
                                      $"<td>{item.SoHoaDon} </td>" +
                                      $"<td>{item.LyDoHuyBanThuoc} </td>" +
                         "</tr>";
                stt++;
            }
            tableHTML += "<tr style='border: 1px solid #020000;font-weight: bold;'>" +
                                     $"<td></td>" +
                                     $"<td>Tổng tiền</td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td style='text-align: right;'>{tongTien} </td>" +
                                     $"<td style='text-align: right;'>{tienMat} </td>" +
                                     $"<td style='text-align: right;'>{chuyenKhoan} </td>" +
                                     $"<td style='text-align: right;'>{pos} </td>" +
                                     $"<td>{congNo} </td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                                     $"<td></td>" +
                        "</tr>";
            var data = new
            {
                ngaythu = thoiGianTimKiem,
                DoanhThuThuoc = tableHTML,
                tongTien,
                tienMat,
                chuyenKhoan,
                pos,
                congNo,
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year,
            };

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        #endregion
    }
}
