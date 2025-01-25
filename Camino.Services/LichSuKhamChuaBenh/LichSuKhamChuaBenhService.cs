using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNBPT;
using Camino.Core.Domain.ValueObject.BangKiemAnToanNguoiBenhPTTuPhongDieuTri;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs;
using Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri;
using Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenMau;
using Camino.Core.Domain.ValueObject.XetNghiem;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.LichSuKhamChuaBenh
{
    [ScopedDependency(ServiceType = typeof(ILichSuKhamChuaBenhService))]
    public class LichSuKhamChuaBenhService : MasterFileService<YeuCauTiepNhan>, ILichSuKhamChuaBenhService
    {
        private readonly IRepository<BenhNhan> _benhNhanRepository;
        private readonly ILoggerManager _logger;
        public LichSuKhamChuaBenhService(IRepository<YeuCauTiepNhan> repository
        , ILoggerManager logger, IRepository<BenhNhan> benhNhanRepository) : base(repository)
        {
            _benhNhanRepository = benhNhanRepository;
            _logger = logger;
        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridTimKiemNguoiBenhDaTiepNhanAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new LichSuKhamChuaBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuKhamChuaBenhTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
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

            var query =
                BaseRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.BenhNhanId != null
                                && (timKiemNangCaoObj.NamSinh == null || x.BenhNhan.NamSinh == timKiemNangCaoObj.NamSinh)
                                && (timKiemNangCaoObj.SoDienThoai == null
                                    || (!string.IsNullOrEmpty(x.SoDienThoai) && x.SoDienThoai.ToLower().Contains(timKiemNangCaoObj.SoDienThoai.ToLower()))
                                    || (!string.IsNullOrEmpty(x.BenhNhan.SoDienThoai) && x.BenhNhan.SoDienThoai.ToLower().Contains(timKiemNangCaoObj.SoDienThoai.ToLower())))
                                && (tuNgay == null || x.ThoiDiemTiepNhan.Date >= tuNgay.Value.Date)
                                && (denNgay == null || x.ThoiDiemTiepNhan.Date <= denNgay.Value.Date)
                    )
                    .Select(x => x.BenhNhan)
                    .Select(item => new LichSuKhamChuaBenhGridVo()
                    {
                        Id = item.Id,
                        MaBN = item.MaBN,
                        BHYTMaSoThe = item.BHYTMaSoThe,
                        HoTen = item.HoTen,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        GioiTinh = item.GioiTinh,
                        SoChungMinhThu = item.SoChungMinhThu,
                        DiaChi = item.DiaChiDayDu,
                        SoDienThoai = item.SoDienThoai,
                        SoDienThoaiDisplay = item.SoDienThoaiDisplay
                    })
                    .Distinct()
                .OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridTimKiemNguoiBenhDaTiepNhanAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new LichSuKhamChuaBenhTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<LichSuKhamChuaBenhTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
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

            var query =
                BaseRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.BenhNhan.MaBN, x => x.HoTen)
                    .Where(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.BenhNhanId != null
                                && (timKiemNangCaoObj.NamSinh == null || x.BenhNhan.NamSinh == timKiemNangCaoObj.NamSinh)
                                && (timKiemNangCaoObj.SoDienThoai == null
                                    || (!string.IsNullOrEmpty(x.SoDienThoai) && x.SoDienThoai.ToLower().Contains(timKiemNangCaoObj.SoDienThoai.ToLower()))
                                    || (!string.IsNullOrEmpty(x.BenhNhan.SoDienThoai) && x.BenhNhan.SoDienThoai.ToLower().Contains(timKiemNangCaoObj.SoDienThoai.ToLower())))
                                && (tuNgay == null || x.ThoiDiemTiepNhan.Date >= tuNgay.Value.Date)
                                && (denNgay == null || x.ThoiDiemTiepNhan.Date <= denNgay.Value.Date)
                    )
                    .Select(x => x.BenhNhan)
                    .Select(item => new LichSuKhamChuaBenhGridVo()
                    {
                        Id = item.Id,
                        MaBN = item.MaBN,
                        HoTen = item.HoTen,
                        NgaySinh = item.NgaySinh,
                        ThangSinh = item.ThangSinh,
                        NamSinh = item.NamSinh,
                        GioiTinh = item.GioiTinh,
                        SoChungMinhThu = item.SoChungMinhThu,
                        DiaChi = item.DiaChiDayDu,
                        SoDienThoai = item.SoDienThoai
                    })
                    .Distinct();

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region get data
        public async Task<LichSuKhamChuaBenhTheoNguoiBenhVo> GetLichSuKhamChuaBenhTheoNguoiBenhAsnc(long nguoiBenhId)
        {
            var lichSuKham = new LichSuKhamChuaBenhTheoNguoiBenhVo();
            var lstYeuCauTiepNhan = await GetLichSuKhamYeuCauTiepNhanAsnc(nguoiBenhId);

            foreach (var yeuCauTiepNhan in lstYeuCauTiepNhan)
            {
                var newTiepNhanItem = new LichSuTiepNhanNguoiBenhVo()
                {
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    MaYeuCauTiepNhan = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    NgayTiepNhan = yeuCauTiepNhan.ThoiDiemTiepNhan
                };

                #region dịch vụ khám
                var lstYeuCauKham = yeuCauTiepNhan.YeuCauKhamBenhs.Where(a => a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).ToList();
                var lstYeCauDichVuKyThuatNoiTru = new List<YeuCauDichVuKyThuat>();
                if (lstYeuCauKham.Any())
                {
                    var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                    {
                        LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.KhamBenh,
                        TenDichVu = Enums.LoaiLichSuKhamChuaBenh.KhamBenh.GetDescription(),
                        IsGroupItem = true
                    };
                    newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                }
                foreach (var yeuCauKhamBenh in lstYeuCauKham)
                {
                    var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                    {
                        LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.KhamBenh,
                        IsGroupItem = false,

                        LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.DichVuKhamBenh,
                        YeuCauDichVuId = yeuCauKhamBenh.Id,
                        TenDichVu = yeuCauKhamBenh.TenDichVu,
                        ThoiDiemKhamTu = yeuCauKhamBenh.ThoiDiemHoanThanh ?? yeuCauKhamBenh.ThoiDiemThucHien
                    };
                    newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);

                    if (yeuCauKhamBenh.CoKeToa == true)
                    {
                        if (yeuCauKhamBenh.YeuCauKhamBenhDonThuocs.Any())
                        {
                            var newLichSuDonThuoc = new ChiTietKhamChuaBenhVo()
                            {
                                LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.KhamBenh,
                                IsGroupItem = false,

                                LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc,
                                YeuCauDichVuId = yeuCauKhamBenh.Id,
                                TenDichVu = Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc.GetDescription() + " (" + yeuCauKhamBenh.TenDichVu + ")",
                                ThoiDiemKhamTu = yeuCauKhamBenh.ThoiDiemHoanThanh ?? yeuCauKhamBenh.ThoiDiemThucHien
                            };
                            newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuDonThuoc);
                        }

                        if (yeuCauKhamBenh.YeuCauKhamBenhDonVTYTs.Any())
                        {
                            var newLichSuDonVatTu = new ChiTietKhamChuaBenhVo()
                            {
                                LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.KhamBenh,
                                IsGroupItem = false,

                                LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.DonVatTu,
                                YeuCauDichVuId = yeuCauKhamBenh.Id,
                                TenDichVu = Enums.LoaiLichSuKhamChuaBenhChiTiet.DonVatTu.GetDescription() + " (" + yeuCauKhamBenh.TenDichVu + ")",
                                ThoiDiemKhamTu = yeuCauKhamBenh.ThoiDiemHoanThanh ?? yeuCauKhamBenh.ThoiDiemThucHien
                            };
                            newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuDonVatTu);
                        }
                    }

                    if (yeuCauKhamBenh.YeuCauNhapViens.Any())
                    {
                        lstYeCauDichVuKyThuatNoiTru.AddRange(yeuCauKhamBenh.YeuCauNhapViens
                            .SelectMany(x => x.YeuCauTiepNhans)
                            .SelectMany(x => x.YeuCauDichVuKyThuats)
                            .Where(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                        && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                            || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                            || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                            .ToList());
                    }
                }
                #endregion

                #region dịch vụ cls
                var lstYeuCauCLS = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                                && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                                    || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                                                    || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)).ToList();
                if (lstYeCauDichVuKyThuatNoiTru.Any())
                {
                    lstYeuCauCLS.AddRange(lstYeCauDichVuKyThuatNoiTru);
                }

                if (lstYeuCauCLS.Any())
                {
                    var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                    {
                        LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.CanLamSang,
                        TenDichVu = Enums.LoaiLichSuKhamChuaBenh.CanLamSang.GetDescription(),
                        IsGroupItem = true
                    };
                    newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                }
                foreach (var yeuCauCLS in lstYeuCauCLS)
                {
                    var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                    {
                        LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.CanLamSang,
                        IsGroupItem = false,

                        LoaiLichSuKhamChuaBenhChiTiet = yeuCauCLS.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? Enums.LoaiLichSuKhamChuaBenhChiTiet.DichVuXetNghiem : Enums.LoaiLichSuKhamChuaBenhChiTiet.DichVuCDHATDCN,
                        YeuCauDichVuId = yeuCauCLS.Id,
                        TenDichVu = yeuCauCLS.TenDichVu,
                        ThoiDiemKhamTu = yeuCauCLS.ThoiDiemHoanThanh ?? (yeuCauCLS.ThoiDiemKetLuan ?? yeuCauCLS.ThoiDiemThucHien)
                    };
                    newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                }
                #endregion

                #region y lệnh

                var yeuCauTiepNhanNoiTru = yeuCauTiepNhan.YeuCauKhamBenhs
                    .Where(x => x.YeuCauNhapViens.Any(a => a.YeuCauTiepNhans.Any(c => c.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                                                                      && c.NoiTruBenhAn != null
                                                                                      && (c.NoiTruBenhAn.NoiTruPhieuDieuTris.Any()
                                                                                          || c.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(d => d.YeuCauDichVuKyThuatId == null
                                                                                                                                                     && d.YeuCauTruyenMauId == null
                                                                                                                                                     && d.YeuCauVatTuBenhVienId == null
                                                                                                                                                     && d.NoiTruChiDinhDuocPhamId == null)
                                                                                          || c.NoiTruHoSoKhacs.Any()))))
                    .SelectMany(x => x.YeuCauNhapViens)
                    .SelectMany(x => x.YeuCauTiepNhans)
                    .FirstOrDefault(x => x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
                if (yeuCauTiepNhanNoiTru != null)
                {
                    var newGroupPhieuDieuTri = new ChiTietKhamChuaBenhVo()
                    {
                        LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.YLenh,
                        TenDichVu = Enums.LoaiLichSuKhamChuaBenh.YLenh.GetDescription(),
                        IsGroupItem = true
                    };
                    newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newGroupPhieuDieuTri);

                    #region Phiếu điều trị
                    if (yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.Any())
                    {
                        var phieuDieuTriDauTien = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderBy(x => x.NgayDieuTri).First();
                        var phieuDieuTriCuoiCung = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(x => x.NgayDieuTri).First();
                        var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                        {
                            LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.YLenh,
                            IsGroupItem = false,

                            LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuDieuTri,
                            TenDichVu = Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuDieuTri.GetDescription(),
                            NoiTruBenhAnId = yeuCauTiepNhanNoiTru.NoiTruBenhAn.Id,
                            ThoiDiemKhamTu = phieuDieuTriDauTien.NgayDieuTri,
                            ThoiDiemKhamDen = phieuDieuTriCuoiCung.NgayDieuTri
                        };
                        newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                    }
                    #endregion

                    #region Phiếu chăm sóc
                    var phieuChamSocs = yeuCauTiepNhanNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs
                        .Where(d => d.YeuCauDichVuKyThuatId == null
                                  && d.YeuCauTruyenMauId == null
                                  && d.YeuCauVatTuBenhVienId == null
                                  && d.NoiTruChiDinhDuocPhamId == null)
                        .ToList();
                    if (phieuChamSocs.Any())
                    {
                        var phieuChamSocDauTien = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien;
                        var phieuChamSocCuoiCung = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now;
                        var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                        {
                            LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.YLenh,
                            IsGroupItem = false,

                            LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuChamSoc,
                            TenDichVu = Enums.LoaiLichSuKhamChuaBenhChiTiet.PhieuChamSoc.GetDescription(),
                            NoiTruBenhAnId = yeuCauTiepNhanNoiTru.NoiTruBenhAn.Id,
                            ThoiDiemKhamTu = phieuChamSocDauTien,
                            ThoiDiemKhamDen = phieuChamSocCuoiCung
                        };
                        newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                    }
                    #endregion

                    #region Hồ sơ khác
                    if (yeuCauTiepNhanNoiTru.NoiTruHoSoKhacs.Any())
                    {
                        var phieuChamSocDauTien = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien;
                        var phieuChamSocCuoiCung = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now;
                        foreach (var hoSoKhac in yeuCauTiepNhanNoiTru.NoiTruHoSoKhacs)
                        {
                            var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                            {
                                LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.YLenh,
                                IsGroupItem = false,

                                NoiTruHoSoKhacId = hoSoKhac.Id,
                                LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.HoSoKhac,
                                TenDichVu = hoSoKhac.LoaiHoSoDieuTriNoiTru.GetDescription(),
                                LoaiHoSoDieuTriNoiTru = hoSoKhac.LoaiHoSoDieuTriNoiTru,
                                NoiTruBenhAnId = yeuCauTiepNhanNoiTru.NoiTruBenhAn.Id,
                                ThoiDiemKhamTu = phieuChamSocDauTien,
                                ThoiDiemKhamDen = phieuChamSocCuoiCung
                            };

                            if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuSoKet15NgayDieuTri)
                            {
                                var detail = JsonConvert.DeserializeObject<PhieuSoKet15NgayDieuTriVo>(hoSoKhac.ThongTinHoSo);
                                //newLichSuChiTiet.ThoiDiemKhamTu = detail.TuNgay;
                                //newLichSuChiTiet.ThoiDiemKhamDen = detail.DenNgay;
                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.TuNgay?.ToLocalTime().ApplyFormatDate()} - {detail.DenNgay?.ToLocalTime().ApplyFormatDate()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhPhauThuat)
                            {
                                var detail = JsonConvert.DeserializeObject<InModelBangKiemAnToanNBPT>(hoSoKhac.ThongTinHoSo);
                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.NgayGioDuaBNDiPT.ToLocalTime().ApplyFormatDateTimeSACH()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh)
                            {
                                var detail = !string.IsNullOrEmpty(hoSoKhac.ThongTinHoSo)
                                    ? JsonConvert.DeserializeObject<BangTheoDoiHoiTinhVo>(hoSoKhac.ThongTinHoSo)
                                    : new BangTheoDoiHoiTinhVo();

                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.NgayThucHien?.ToLocalTime().ApplyFormatDate()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanNguoiBenhTuPhongMoVePhongDieuTri)
                            {
                                var replaceData = hoSoKhac.ThongTinHoSo.Replace("\\n", "<br/>");
                                var detail = JsonConvert.DeserializeObject<InBangKiemBenhNhanPhauThuatVeKhoaDieuTri>(replaceData);
                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.NgayNhan.ToLocalTime().ApplyFormatDateTimeSACH()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc)
                            {
                                var detail = JsonConvert.DeserializeObject<HoSoKhacBangTheoDoiGayMeHoiSucVo>(hoSoKhac.ThongTinHoSo);
                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.NgayThucHien?.ApplyFormatDate()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTheoDoiTruyenMau)
                            {
                                var detail = JsonConvert.DeserializeObject<InPhieuTheoDoiTruyenMau>(hoSoKhac.ThongTinHoSo);
                                newLichSuChiTiet.ThoiDiemKhamTu = null;
                                newLichSuChiTiet.ThoiDiemKhamDen = null;
                                newLichSuChiTiet.TenDichVu +=
                                    $" - {detail.BatDauTruyenHoi?.ToLocalTime().ApplyFormatDateTimeSACH()}";
                            }
                            else if (hoSoKhac.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh)
                            {
                                if (yeuCauTiepNhanNoiTru.NoiTruBenhAn.LoaiBenhAn != Enums.LoaiBenhAn.SanKhoaMo 
                                    && yeuCauTiepNhanNoiTru.NoiTruBenhAn.LoaiBenhAn != Enums.LoaiBenhAn.SanKhoaThuong)
                                {
                                    continue;
                                }
                            }
                            newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                        }
                    }
                    #endregion

                    #region Đơn thuốc
                    var donThuocRaViens = yeuCauTiepNhanNoiTru.NoiTruDonThuocs.ToList();
                    if (donThuocRaViens.Any())
                    {
                        var thoiDiemNhapVien = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien;
                        var thoiDiemRaVien = yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemRaVien ?? DateTime.Now;
                        var newLichSuChiTiet = new ChiTietKhamChuaBenhVo()
                        {
                            LoaiLichSuKhamChuaBenh = Enums.LoaiLichSuKhamChuaBenh.YLenh,
                            IsGroupItem = false,

                            LoaiLichSuKhamChuaBenhChiTiet = Enums.LoaiLichSuKhamChuaBenhChiTiet.DonThuoc,
                            TenDichVu = "Thuốc ra viện",
                            NoiTruBenhAnId = yeuCauTiepNhanNoiTru.NoiTruBenhAn.Id,
                            ThoiDiemKhamTu = thoiDiemNhapVien,
                            ThoiDiemKhamDen = thoiDiemRaVien
                        };
                        newTiepNhanItem.ChiTietKhamChuaBenhs.Add(newLichSuChiTiet);
                    }
                    #endregion
                }

                #endregion

                lichSuKham.LichSuTiepNhans.Add(newTiepNhanItem);
            }

            return lichSuKham;
        }

        public async Task<List<YeuCauTiepNhan>> GetLichSuKhamYeuCauTiepNhanAsnc(long? nguoiBenhId = null, long? yeuCauTiepNhanId = null)
        {
            _logger.LogInfo($"GetLichSuKhamYeuCauTiepNhanAsnc begin nguoiBenhId:{nguoiBenhId}, yeuCauTiepNhanId:{yeuCauTiepNhanId}");
            var yeuCauTiepNhans = new List<YeuCauTiepNhan>();
            if (nguoiBenhId != null || yeuCauTiepNhanId != null)
            {
                //yeuCauTiepNhans = BaseRepository.TableNoTracking
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruHoSoKhacs)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruDonThuocs)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                //    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs)
                //    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                //    .Where(x => ((nguoiBenhId != null && x.BenhNhanId == nguoiBenhId) || (yeuCauTiepNhanId != null && x.Id == yeuCauTiepNhanId))

                //                // lịch sử khám nội trú sẽ hiện chung với YCTN trước của chính nó
                //                && x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                //                && (
                //                    x.YeuCauKhamBenhs.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                //                    || x.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                //                                                       && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                //                                                       || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                //                                                       || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                //                    || x.YeuCauKhamBenhs.Any(a => a.YeuCauNhapViens.Any(b => b.YeuCauTiepNhans.Any(c => c.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                //                                                                                                        && c.NoiTruBenhAn != null
                //                                                                                                        && (c.NoiTruBenhAn.NoiTruPhieuDieuTris.Any()
                //                                                                                                            || c.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(d => d.YeuCauDichVuKyThuatId == null
                //                                                                                                                                                                       && d.YeuCauTruyenMauId == null
                //                                                                                                                                                                       && d.YeuCauVatTuBenhVienId == null
                //                                                                                                                                                                       && d.NoiTruChiDinhDuocPhamId == null)
                //                                                                                                            || c.NoiTruHoSoKhacs.Any()))))
                //                ))
                //    .OrderByDescending(x => x.ThoiDiemTiepNhan)
                //    .ToList();

                var yeuCauTiepNhanAlls = BaseRepository.TableNoTracking
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTriChiTietYLenhs)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruHoSoKhacs)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.NoiTruDonThuocs)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauNhapViens).ThenInclude(x => x.YeuCauTiepNhans).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonThuocs)
                    .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.YeuCauKhamBenhDonVTYTs)
                    .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhienXetNghiemChiTiets)
                    .Where(x => ((nguoiBenhId != null && x.BenhNhanId == nguoiBenhId) || (yeuCauTiepNhanId != null && x.Id == yeuCauTiepNhanId))
                                // lịch sử khám nội trú sẽ hiện chung với YCTN trước của chính nó
                                && x.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)                    
                    .ToList();
                yeuCauTiepNhans = yeuCauTiepNhanAlls.Where(x =>                                 
                                (
                                    x.YeuCauKhamBenhs.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                                    || x.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                       && (a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                       || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                                                       || a.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                                    || x.YeuCauKhamBenhs.Any(a => a.YeuCauNhapViens.Any(b => b.YeuCauTiepNhans.Any(c => c.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                                                                                                        && c.NoiTruBenhAn != null
                                                                                                                        && (c.NoiTruBenhAn.NoiTruPhieuDieuTris.Any()
                                                                                                                            || c.NoiTruBenhAn.NoiTruPhieuDieuTriChiTietYLenhs.Any(d => d.YeuCauDichVuKyThuatId == null
                                                                                                                                                                                       && d.YeuCauTruyenMauId == null
                                                                                                                                                                                       && d.YeuCauVatTuBenhVienId == null
                                                                                                                                                                                       && d.NoiTruChiDinhDuocPhamId == null)
                                                                                                                            || c.NoiTruHoSoKhacs.Any()))))
                                )).OrderByDescending(x => x.ThoiDiemTiepNhan).ToList();
            }
            _logger.LogInfo($"GetLichSuKhamYeuCauTiepNhanAsnc end nguoiBenhId:{nguoiBenhId}, yeuCauTiepNhanId:{yeuCauTiepNhanId}");
            return yeuCauTiepNhans;
        }
        #endregion
    }
}
