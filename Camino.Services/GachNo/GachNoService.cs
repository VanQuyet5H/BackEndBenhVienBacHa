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
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GachNos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.GachNo
{
    [ScopedDependency(ServiceType = typeof(IGachNoService))]
    public class GachNoService : MasterFileService<Core.Domain.Entities.GachNos.GachNo>, IGachNoService
    {
        private IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private IRepository<BenhNhan> _benhNhanRepository;
        private IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;
        private IUserAgentHelper _userAgentHelper;
        private IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private IRepository<TaiKhoanBenhNhan> _taiKhoanBenhNhanRepository;
        private IRepository<AuditGachNo> _auditGachNoRepository;
        private IRepository<CongTyBaoHiemTuNhanCongNo> _congTyBaoHiemTuNhanCongNoRepository;
        public GachNoService(IRepository<Core.Domain.Entities.GachNos.GachNo> repository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<BenhNhan> benhNhanRepository,
            IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<TaiKhoanBenhNhan> taiKhoanBenhNhanRepository,
            IRepository<AuditGachNo> auditGachNoRepository,
            IRepository<CongTyBaoHiemTuNhanCongNo> congTyBaoHiemTuNhanCongNoRepository) : base(repository)
        {
            _cauHinhRepository = cauHinhRepository;
            _benhNhanRepository = benhNhanRepository;
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _userAgentHelper = userAgentHelper;
            _nhanVienRepository = nhanVienRepository;
            _taiKhoanBenhNhanRepository = taiKhoanBenhNhanRepository;
            _auditGachNoRepository = auditGachNoRepository;
            _congTyBaoHiemTuNhanCongNoRepository = congTyBaoHiemTuNhanCongNoRepository;
        }

        #region grid
        public async Task<GridDataSource> GetDataGachNoForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new GachNoTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(s => new GachNoGridVo()
            {
                Id = s.Id,
                SoChungTu = s.SoChungTu,
                LoaiDoiTuong = s.LoaiDoiTuong,
                LoaiThuChi = s.LoaiThuChi,
                NgayChungTu = s.NgayChungTu,
                NgayChungTuDisplay = s.NgayChungTu.ApplyFormatDate(),
                TaiKhoan = s.TaiKhoan,
                DienGiai = s.DienGiaiChung,
                VAT = s.VAT,
                KhoanMucPhi = s.KhoanMucPhi,
                TienHachToan = s.TienHachToan,
                TienThueHachToan = s.TienThueHachToan,
                TongTienHachToan = s.TongTienHachToan,
                SoHoaDon = s.SoHoaDon,
                NgayHoaDon = s.NgayHoaDon,
                NgayHoaDonDisplay = s.NgayHoaDon == null ? null : s.NgayHoaDon.Value.ApplyFormatDate(),
                MaKhachHang = s.BenhNhan != null ? s.BenhNhan.MaBN : s.CongTyBaoHiemTuNhan.Ma,
                TenKhachHang = s.BenhNhan != null ? s.BenhNhan.HoTen : s.CongTyBaoHiemTuNhan.Ten,
                MaSoThue = s.BenhNhan != null ? null : s.CongTyBaoHiemTuNhan.MaSoThue,
                TrangThai = s.TrangThai,
                LoaiTienTe = s.LoaiTienTe,

                NguoiXacNhanNhapLieu = s.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu ? s.NguoiXacNhanNhapLieu.User.HoTen : string.Empty,
                NgayXacNhanNhapLieu = s.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu ? s.LastTime : null
            }).ApplyLike(queryInfo.SearchTerms, g => g.SoChungTu,
                g => g.LoaiThuChi ,g => g.TaiKhoan, g => g.DienGiai, g => g.KhoanMucPhi, g => g.SoHoaDon, g => g.MaKhachHang, g => g.TenKhachHang, g => g.MaSoThue);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgayCT != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay) || p.NgayChungTu.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay) || p.NgayChungTu.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.TuNgayDenNgayHD != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date <= denNgay.Date)));
            }

            if (!string.IsNullOrEmpty(timKiemNangCaoObj.LoaiThuChi))
            {
                query = query.Where(p => string.IsNullOrEmpty(timKiemNangCaoObj.LoaiThuChi) || p.LoaiThuChi == timKiemNangCaoObj.LoaiThuChi);
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageGachNoForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new GachNoTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var query = BaseRepository.TableNoTracking.Select(s => new GachNoGridVo()
            {
                Id = s.Id,
                SoChungTu = s.SoChungTu,
                LoaiDoiTuong = s.LoaiDoiTuong,
                LoaiThuChi = s.LoaiThuChi,
                NgayChungTu = s.NgayChungTu,
                NgayChungTuDisplay = s.NgayChungTu.ApplyFormatDate(),
                TaiKhoan = s.TaiKhoan,
                DienGiai = s.DienGiaiChung,
                VAT = s.VAT,
                KhoanMucPhi = s.KhoanMucPhi,
                TienHachToan = s.TienHachToan,
                TienThueHachToan = s.TienThueHachToan,
                TongTienHachToan = s.TongTienHachToan,
                SoHoaDon = s.SoHoaDon,
                NgayHoaDon = s.NgayHoaDon,
                NgayHoaDonDisplay = s.NgayHoaDon == null ? null : s.NgayHoaDon.Value.ApplyFormatDate(),
                MaKhachHang = s.BenhNhan != null ? s.BenhNhan.MaBN : s.CongTyBaoHiemTuNhan.Ma,
                TenKhachHang = s.BenhNhan != null ? s.BenhNhan.HoTen : s.CongTyBaoHiemTuNhan.Ten,
                MaSoThue = s.BenhNhan != null ? null : s.CongTyBaoHiemTuNhan.MaSoThue,
                TrangThai = s.TrangThai,
                LoaiTienTe = s.LoaiTienTe
            }).ApplyLike(queryInfo.SearchTerms, g => g.SoChungTu,
                g => g.LoaiThuChi, g => g.TaiKhoan, g => g.DienGiai, g => g.KhoanMucPhi, g => g.SoHoaDon, g => g.MaKhachHang, g => g.TenKhachHang, g => g.MaSoThue);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgayCT != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay) || p.NgayChungTu.Date >= tuNgay.Date)
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay) || p.NgayChungTu.Date <= denNgay.Date));
            }

            if (timKiemNangCaoObj.TuNgayDenNgayHD != null && (!string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay) || !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay)))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                query = query.Where(p => (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date >= tuNgay.Date))
                                         && (string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay) || (p.NgayHoaDon != null && p.NgayHoaDon.Value.Date <= denNgay.Date)));
            }

            if (!string.IsNullOrEmpty(timKiemNangCaoObj.LoaiThuChi))
            {
                query = query.Where(p => string.IsNullOrEmpty(timKiemNangCaoObj.LoaiThuChi) || p.LoaiThuChi == timKiemNangCaoObj.LoaiThuChi);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _congTyBaoHiemTuNhanRepository.TableNoTracking
                .Select(item => new GachNoCongTyBHTNGridVo()
                {
                    Id = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DiaChi = item.DiaChi,
                    DienThoai = item.SoDienThoai,
                    MaSoThue = item.MaSoThue,
                    DonVi = item.DonVi
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoCongTyBHTNGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.Ma))
                {
                    query = query.Where(x => x.Ma.Contains(timKiemNangCaoObj.Ma));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.Ten))
                {
                    query = query.Where(x => x.Ten.Contains(timKiemNangCaoObj.Ten));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DiaChi))
                {
                    query = query.Where(x => x.DiaChi.Contains(timKiemNangCaoObj.DiaChi));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DienThoai))
                {
                    query = query.Where(x => x.DienThoai.Replace(" ", "").Contains(timKiemNangCaoObj.DienThoai.Replace(" ", "")));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaSoThue))
                {
                    query = query.Where(x => x.MaSoThue.Contains(timKiemNangCaoObj.MaSoThue));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo)
        {
            var query = _congTyBaoHiemTuNhanRepository.TableNoTracking
                .Select(item => new GachNoCongTyBHTNGridVo()
                {
                    Id = item.Id,
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DiaChi = item.DiaChi,
                    DienThoai = item.SoDienThoai,
                    MaSoThue = item.MaSoThue,
                    DonVi = item.DonVi
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoCongTyBHTNGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.Ma))
                {
                    query = query.Where(x => x.Ma.Contains(timKiemNangCaoObj.Ma));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.Ten))
                {
                    query = query.Where(x => x.Ten.Contains(timKiemNangCaoObj.Ten));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DiaChi))
                {
                    query = query.Where(x => x.DiaChi.Contains(timKiemNangCaoObj.DiaChi));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DienThoai))
                {
                    query = query.Where(x => x.DienThoai.Replace(" ", "").Contains(timKiemNangCaoObj.DienThoai.Replace(" ", "")));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaSoThue))
                {
                    query = query.Where(x => x.MaSoThue.Contains(timKiemNangCaoObj.MaSoThue));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataBenhNhanForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _benhNhanRepository.TableNoTracking
                .Select(item => new GachNoBenhNhanGridVo()
                {
                    Id = item.Id,
                    MaBN = item.MaBN,
                    HoTen = item.HoTen,
                    DiaChiDayDu = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,
                    SoChungMinhThu = item.SoChungMinhThu,
                    GioiTinh = item.GioiTinh,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoBenhNhanGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaBN))
                {
                    query = query.Where(x => x.MaBN.Contains(timKiemNangCaoObj.MaBN));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.HoTen))
                {
                    query = query.Where(x => x.HoTen.Contains(timKiemNangCaoObj.HoTen));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DiaChiDayDu))
                {
                    query = query.Where(x => x.DiaChiDayDu.Contains(timKiemNangCaoObj.DiaChiDayDu));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SoDienThoai))
                {
                    query = query.Where(x => x.SoDienThoai.Replace(" ", "").Contains(timKiemNangCaoObj.SoDienThoai.Replace(" ", "")));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SoChungMinhThu))
                {
                    query = query.Where(x => x.SoChungMinhThu.Contains(timKiemNangCaoObj.SoChungMinhThu));
                }
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageBenhNhanForGridAsync(QueryInfo queryInfo)
        {
            var query = _benhNhanRepository.TableNoTracking
                .Select(item => new GachNoBenhNhanGridVo()
                {
                    Id = item.Id,
                    MaBN = item.MaBN,
                    HoTen = item.HoTen,
                    DiaChiDayDu = item.DiaChiDayDu,
                    SoDienThoai = item.SoDienThoai,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,
                    SoChungMinhThu = item.SoChungMinhThu,
                    GioiTinh = item.GioiTinh,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                var timKiemNangCaoObj = JsonConvert.DeserializeObject<GachNoBenhNhanGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.MaBN))
                {
                    query = query.Where(x => x.MaBN.Contains(timKiemNangCaoObj.MaBN));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.HoTen))
                {
                    query = query.Where(x => x.HoTen.Contains(timKiemNangCaoObj.HoTen));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.DiaChiDayDu))
                {
                    query = query.Where(x => x.DiaChiDayDu.Contains(timKiemNangCaoObj.DiaChiDayDu));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SoDienThoai))
                {
                    query = query.Where(x => x.SoDienThoai.Replace(" ", "").Contains(timKiemNangCaoObj.SoDienThoai.Replace(" ", "")));
                }
                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SoChungMinhThu))
                {
                    query = query.Where(x => x.SoChungMinhThu.Contains(timKiemNangCaoObj.SoChungMinhThu));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region get list
        public async Task<List<LookupItemCauHinhVo>> GetListLoaiThuChiAsync(DropDownListRequestModel model)
        {
            var loaiThuChiDangChon = string.Empty;
            if (!string.IsNullOrEmpty(model.ParameterDependencies) &&
                !model.ParameterDependencies.Contains("undefined") && !model.ParameterDependencies.Contains("null"))
            {
                var getValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.ParameterDependencies);
                loaiThuChiDangChon = getValue.Values.First();
            }

            var cauHinhLoaiThuChi = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhGachNo.LoaiThuChi");
            var lstLoaiThuChi = new List<LookupItemCauHinhVo>();
            lstLoaiThuChi.AddRange(JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(cauHinhLoaiThuChi.Value));
            var queryString = string.IsNullOrEmpty(model.Query) ? "" : model.Query.RemoveVietnameseDiacritics();
            lstLoaiThuChi = lstLoaiThuChi.Where(x => x.DisplayName.RemoveVietnameseDiacritics().Contains(queryString)).ToList();

            if (!string.IsNullOrEmpty(loaiThuChiDangChon) && lstLoaiThuChi.All(x => x.KeyId != loaiThuChiDangChon))
            {
                lstLoaiThuChi.Add(new LookupItemCauHinhVo()
                {
                    KeyId = loaiThuChiDangChon,
                    DisplayName = loaiThuChiDangChon
                });
            }
            return lstLoaiThuChi;
        }

        public async Task<List<LookupLoaiTienTeItemVo>> GetListLoaiTienTeAsync(DropDownListRequestModel model)
        {
            var cauHinhTyGia = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhGachNo.TyGiaUSD");
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiTienTe>()
                .Select(item => new LookupLoaiTienTeItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item),
                    TyGia = (int)Enums.LoaiTienTe.VND == (int)item ? 1 : decimal.Parse(cauHinhTyGia.Value)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return lstEnums;
        }

        public async Task<List<LookupItemVo>> GetListDoiTuongAsync(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiDoiTuong>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return lstEnums;
        }

        public async Task<List<LookupCongTyBHTNItemVo>> GetListMaCongTyBaoHiemTuNhanAsync(DropDownListRequestModel model)
        {
            var lstMaBenhNhan = await _congTyBaoHiemTuNhanRepository.TableNoTracking
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Id)
                .Select(item => new LookupCongTyBHTNItemVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ma,
                    Ten = item.Ten,
                    MaSoThue = item.MaSoThue,
                    DonVi = item.DonVi,
                    DiaChi = item.DiaChi
                }).ApplyLike(model.Query, x => x.DisplayName)
                .Take(model.Take)
                .ToListAsync();
            return lstMaBenhNhan;
        }

        public async Task<List<LookupBenhNhanItemVo>> GetListMaBenhNhanAsync(DropDownListRequestModel model)
        {
            var lstMaBenhNhan = await _benhNhanRepository.TableNoTracking
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Id)
                .Select(item => new LookupBenhNhanItemVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.MaBN,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh,
                    SoChungMinhThu = item.SoChungMinhThu,
                    SoDienThoaiDisplay = item.SoDienThoaiDisplay,
                    DiaChiDayDu = item.DiaChiDayDu,
                    NgaySinh = item.NgaySinh,
                    ThangSinh = item.ThangSinh,
                    NamSinh = item.NamSinh
                }).ApplyLike(model.Query, x => x.DisplayName)
                .Take(model.Take)
                .ToListAsync();
            return lstMaBenhNhan;
        }

        public async Task<List<LookupItemCauHinhVo>> GetListTaiKhoanAsync(DropDownListRequestModel model)
        {
            LookupItemCauHinhVo taiKhoanDangChon = null;
            if (!string.IsNullOrEmpty(model.ParameterDependencies) &&
                !model.ParameterDependencies.Contains("undefined") && !model.ParameterDependencies.Contains("null"))
            {
                var getValue = JsonConvert.DeserializeObject<LookupItemCauHinhVo>(model.ParameterDependencies);
                taiKhoanDangChon = getValue;
            }

            var cauHinhTaiKhoan = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhGachNo.TaiKhoan");
            var lstTaiKhoan = JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(cauHinhTaiKhoan.Value);
            if (!string.IsNullOrEmpty(model.Query))
            {
                lstTaiKhoan = lstTaiKhoan.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                         .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            if (taiKhoanDangChon != null && lstTaiKhoan.All(x => x.KeyId != taiKhoanDangChon.KeyId))
            {
                lstTaiKhoan.Add(new LookupItemCauHinhVo()
                {
                    KeyId = taiKhoanDangChon.KeyId,
                    DisplayName = taiKhoanDangChon.KeyId,
                    GhiChu = taiKhoanDangChon.GhiChu
                });
            }

            return lstTaiKhoan;
        }
        public async Task<List<LookupItemCauHinhVo>> GetListSoTaiKhoanNganHangAsync(DropDownListRequestModel model)
        {
            var soTaiKhoanDangChon = string.Empty;
            if (!string.IsNullOrEmpty(model.ParameterDependencies) &&
                !model.ParameterDependencies.Contains("undefined") && !model.ParameterDependencies.Contains("null"))
            {
                var getValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.ParameterDependencies);
                soTaiKhoanDangChon = getValue.Values.First();
            }
            var cauHinhSoTaiKhoanNganHang = await _cauHinhRepository.TableNoTracking.FirstAsync(x => x.Name == "CauHinhGachNo.SoTaiKhoanNganHang");
            var lstTaiKhoanNganHang = JsonConvert.DeserializeObject<List<LookupItemCauHinhVo>>(cauHinhSoTaiKhoanNganHang.Value);
            if (!string.IsNullOrEmpty(model.Query))
            {
                lstTaiKhoanNganHang = lstTaiKhoanNganHang.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                         .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            if (!string.IsNullOrEmpty(soTaiKhoanDangChon) && lstTaiKhoanNganHang.All(x => x.KeyId != soTaiKhoanDangChon))
            {
                lstTaiKhoanNganHang.Add(new LookupItemCauHinhVo()
                {
                    KeyId = soTaiKhoanDangChon,
                    DisplayName = soTaiKhoanDangChon
                });
            }

            return lstTaiKhoanNganHang;
        }
        public async Task<List<LookupItemVo>> GetListLoaiChungTuAsync(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiChungTu>()
                .Select(item => new LookupItemVo
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }

            return lstEnums;
        }

        public async Task<List<LookupItemTemplateVo>> GetListBaoHiemTuNhanAsync(DropDownListRequestModel model)
        {
            var lstCongTy = await _congTyBaoHiemTuNhanRepository.TableNoTracking
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Ten)
                .Select(item => new LookupItemTemplateVo()
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    Ma = item.Ma,
                    Ten = item.Ten
                }).ApplyLike(model.Query, x => x.Ten, x => x.Ma)
                .Take(model.Take)
                .ToListAsync();
            return lstCongTy;
        }
        #endregion

        #region xử lý thêm/xóa/sửa
        public async Task<bool> KiemTraQuyenXacNhanNhapLieuAsync()
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var kiemTra = await _nhanVienRepository.TableNoTracking
                .Where(x => x.Id == currentUserId)
                .AnyAsync(x => x.NhanVienRoles.Any(a =>
                    a.Role.RoleFunctions.Any(b => b.DocumentType == Enums.DocumentType.CongNoBhtnXacNhanNhapLieu)));
            return kiemTra;
        }

        public async Task XuLyCapNhatCongNo(Core.Domain.Entities.GachNos.GachNo gachNo)
        {
            var taiKhoanBenhNhan = await _taiKhoanBenhNhanRepository.Table
                .Where(x => x.BenhNhan.Id == gachNo.BenhNhanId)
                .Include(x => x.TaiKhoanBenhNhanThus).FirstAsync();
            var thuPhi = new TaiKhoanBenhNhanThu
            {
                TaiKhoanBenhNhan = taiKhoanBenhNhan,
                LoaiThuTienBenhNhan = Enums.LoaiThuTienBenhNhan.ThuNo,
                LoaiNoiThu = Enums.LoaiNoiThu.ThuNgan,
                TienMat = gachNo.LoaiChungTu == Enums.LoaiChungTu.PhieuThu ? gachNo.TongTienHachToan : (decimal?) null,
                ChuyenKhoan = gachNo.LoaiChungTu == Enums.LoaiChungTu.BaoCoNganHang ? gachNo.TongTienHachToan : (decimal?)null,
                POS = 0,
                NoiDungThu = Enums.LoaiThuTienBenhNhan.ThuNo.GetDescription(),
                NgayThu = DateTime.Now,
                SoQuyen = 1, // gán tạm
                NhanVienThucHienId = _userAgentHelper.GetCurrentUserId(),
                NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId()
            };

            if (taiKhoanBenhNhan.SoDuTaiKhoan.SoTienTuongDuong(-1 * gachNo.TongTienHachToan))
            {
                taiKhoanBenhNhan.SoDuTaiKhoan = 0;
            }
            else
            {
                taiKhoanBenhNhan.SoDuTaiKhoan += gachNo.TongTienHachToan;
            }
            taiKhoanBenhNhan.TaiKhoanBenhNhanThus.Add(thuPhi);
            await _taiKhoanBenhNhanRepository.UpdateAsync(taiKhoanBenhNhan);
        }

        public async Task<List<GachNoHistoryVo>> GetLichSuGachNoAsync(long id)
        {
            var historys = await _auditGachNoRepository.TableNoTracking
                .Where(x => x.KeyValues == id)
                .Select(item => new GachNoHistoryVo()
                {
                    Id = item.Id,
                    ThoiGianThucHien = item.CreatedOn.Value,
                    NhanVienThucHienId = item.CreatedById,
                    TenNhanVienThucHien = item.NhanVienThucHien.User.HoTen,
                    Action = item.Action,
                    strOldValue = item.OldValues,
                    strNewValue = item.NewValues
                })
                .ToListAsync();

            Enums.LoaiTienTe? tienTeDisplay = null;
            foreach (var history in historys)
            {
                if (!string.IsNullOrEmpty(history.strNewValue))
                {
                    history.NewValue = JsonConvert.DeserializeObject<GachNoHistoryItemVo>(history.strNewValue);
                    if (history.NewValue.BenhNhanId != null)
                    {
                        var benhNhan = await _benhNhanRepository.GetByIdAsync(history.NewValue.BenhNhanId.Value);
                        history.NewValue.MaDoiTuong = benhNhan.MaBN;
                    }
                    else if (history.NewValue.CongTyBaoHiemTuNhanId != null)
                    {
                        var congTy = await _congTyBaoHiemTuNhanRepository.GetByIdAsync(history.NewValue.CongTyBaoHiemTuNhanId.Value);
                        history.NewValue.MaDoiTuong = congTy.Ma;
                    }

                    if (history.NewValue.LoaiTienTe != null)
                    {
                        tienTeDisplay = history.NewValue.LoaiTienTe;
                    }
                    //history.NewValue.TienTeDisplay = tienTeDisplay;
                    history.NewValue.LoaiTienTeDisplay = tienTeDisplay;
                }

                if (!string.IsNullOrEmpty(history.strOldValue))
                {
                    history.OldValue = JsonConvert.DeserializeObject<GachNoHistoryItemVo>(history.strOldValue);
                    if (history.OldValue.BenhNhanId != null)
                    {
                        var benhNhan = await _benhNhanRepository.GetByIdAsync(history.OldValue.BenhNhanId.Value);
                        history.OldValue.MaDoiTuong = benhNhan.MaBN;
                    }
                    else if (history.OldValue.CongTyBaoHiemTuNhanId != null)
                    {
                        var congTy = await _congTyBaoHiemTuNhanRepository.GetByIdAsync(history.OldValue.CongTyBaoHiemTuNhanId.Value);
                        history.OldValue.MaDoiTuong = congTy.Ma;
                    }

                    history.OldValue.LoaiTienTeDisplay = history.OldValue.LoaiTienTe ?? tienTeDisplay;
                }
            }
            return historys.OrderByDescending(x => x.Id).ToList();
        }
        #endregion

        #region Báo cáo gạch nợ

        public async Task<BaoCaoGachNoCongTyBhtnGridDatasourceVo> GetBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return new BaoCaoGachNoCongTyBhtnGridDatasourceVo()
                {
                    DataSource = new BaoCaoChiTietGachNoCongTyBhtnGrid(),
                    TotalPhatSinhNo = 0,
                    TotalPhatSinhCo = 0,
                    TotalCuoiKyNo = 0
                };
            }
            var query = BaseRepository.TableNoTracking
                .Where(x => x.CongTyBaoHiemTuNhanId != null
                            && x.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
                .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = item.CongTyBaoHiemTuNhanId.Value,
                    TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                    TaiKhoan = item.TaiKhoan,
                    MaTiepNhan = null,
                    SoChungTu = item.SoChungTu,
                    NgayChungTu = item.NgayChungTu,
                    SoHoaDon = item.SoHoaDon,
                    NgayHoaDon = item.NgayHoaDon,
                    DienGiai = item.DienGiaiChung,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    PhatSinhNo = 0,
                    PhatSinhCo = item.TongTienHachToan,
                    DauKyNo = 0,
                    DauKyCo = 0,
                    CuoiKyNo = 0,
                    CuoiKyCo = 0,

                    NgayThuTien = item.LastTime.Value,
                    SuDungGoi = item.SuDungGoi
                })
                .Union(_congTyBaoHiemTuNhanCongNoRepository.TableNoTracking.Where(o=>o.DaHuy != true && o.TaiKhoanBenhNhanThu != null && o.TaiKhoanBenhNhanThu.DaHuy != true)
                    .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                    {
                        CongTyId = item.CongTyBaoHiemTuNhanId,
                        TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                        TaiKhoan = null,
                        MaTiepNhan = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        SoChungTu = null,
                        NgayChungTu = null,
                        SoHoaDon = null,
                        NgayHoaDon = null,
                        DienGiai = item.TaiKhoanBenhNhanThu.TaiKhoanBenhNhan.BenhNhan.HoTen,
                        MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                        PhatSinhNo = item.SoTien,
                        PhatSinhCo = 0,
                        DauKyNo = 0,
                        DauKyCo = 0,
                        CuoiKyNo = 0,
                        CuoiKyCo = 0,

                        NgayThuTien = item.CreatedOn.Value,
                        SuDungGoi = (item.YeuCauKhamBenh.YeuCauGoiDichVuId != null && item.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                    || (item.YeuCauDichVuKyThuat.YeuCauGoiDichVuId != null && item.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                    || item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId != null
                    })
                );

            var timKiemNangCaoObj = new BaoCaoGachNoCongTyBhtnQueryInfo();
            if (queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj =
                    JsonConvert.DeserializeObject<BaoCaoGachNoCongTyBhtnQueryInfo>(queryInfo.AdditionalSearchString);

                if (timKiemNangCaoObj.CongTyId != null)
                {
                    query = query.Where(x => x.CongTyId == timKiemNangCaoObj.CongTyId);
                }

                //if (timKiemNangCaoObj.TuNgayDenNgayCT != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                //    query = query.Where(p => p.NgayChungTu == null || p.NgayChungTu.Value.Date <= denNgay.Date);
                //}

                //if (timKiemNangCaoObj.TuNgayDenNgayHD != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                //    query = query.Where(p => p.NgayHoaDon == null || p.NgayHoaDon.Value.Date <= denNgay.Date);
                //}

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                    query = query.Where(p => p.NgayThuTien.Date <= denNgay.Date);
                }

                if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DungGoi != timKiemNangCaoObj.TrangThai.KhongDungGoi)
                {
                    if (timKiemNangCaoObj.TrangThai.DungGoi && !timKiemNangCaoObj.TrangThai.KhongDungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi == true);
                    }
                    else if (timKiemNangCaoObj.TrangThai.KhongDungGoi && !timKiemNangCaoObj.TrangThai.DungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi != true);
                    }
                }
            }
            var data = await query.OrderBy(x => x.NgayThuTien).ToListAsync();
            var lstCongTyId = data.Select(x => new { x.CongTyId, x.TenCongTy}).Distinct().ToList();
            var gridData = new List<BaoCaoGachNoCongTyBhtnGridVo>();
            var dauKyNos = new List<BaoCaoGachNoCongTyBhtnGridVo>();
            foreach (var congTy in lstCongTyId)
            {
                var lstChiTietCongNoTheoCongTy = data.Where(x => x.CongTyId == congTy.CongTyId).ToList();
                var lstChiTietCongNoDauKyTheoCongTy = new List<BaoCaoGachNoCongTyBhtnGridVo>();
                var lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Select(x => x).ToList();

                //if (timKiemNangCaoObj.TuNgayDenNgayCT != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);

                //    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayChungTu == null || p.NgayChungTu.Value.Date < tuNgay.Date).ToList();
                //    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayChungTu != null && p.NgayChungTu.Value.Date >= tuNgay.Date).ToList();
                //}

                //if (timKiemNangCaoObj.TuNgayDenNgayCT != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayCT.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                //    //lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayChungTu == null).ToList();
                //    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayChungTu != null && p.NgayChungTu.Value.Date <= denNgay.Date).ToList();
                //}

                //if (timKiemNangCaoObj.TuNgayDenNgayHD != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);

                //    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayHoaDon == null || p.NgayHoaDon.Value.Date < tuNgay.Date).ToList();
                //    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayHoaDon != null && p.NgayHoaDon.Value.Date >= tuNgay.Date).ToList();
                //}

                //if (timKiemNangCaoObj.TuNgayDenNgayHD != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay))
                //{
                //    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgayHD.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                //    //lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayHoaDon == null).ToList();
                //    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayHoaDon != null && p.NgayHoaDon.Value.Date <= denNgay.Date).ToList();
                //}

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayThuTien < tuNgay.Date).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date >= tuNgay.Date).ToList();
                }

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);
                    
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date <= denNgay.Date).ToList();
                }

                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    var searchString = timKiemNangCaoObj.SearchString.Trim().RemoveVietnameseDiacritics().ToLower();

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(x =>
                        (string.IsNullOrEmpty(x.TaiKhoan) || !x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.MaTiepNhan) || !x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.DienGiai) || !x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoChungTu) || !x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoHoaDon) || !x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Where(x =>
                        (!string.IsNullOrEmpty(x.TaiKhoan) && x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.MaTiepNhan) && x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.DienGiai) && x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoHoaDon) && x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                }

                if (!lstChiTietCongNoTheoCongTyFilter.Any())
                {
                    continue;
                }

                #region Đầu kỳ
                var dauKy = new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = congTy.CongTyId,
                    TenCongTy = congTy.TenCongTy,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    DienGiai = Enums.DienGiaiGridGachNo.DauKy.GetDescription(),
                    NgayThuTien = lstChiTietCongNoTheoCongTy.First().NgayThuTien.AddMilliseconds(-1)
                };
                if (lstChiTietCongNoDauKyTheoCongTy.Any())
                {
                    dauKy.DauKyNo = dauKy.CuoiKyNo = lstChiTietCongNoDauKyTheoCongTy.Sum(x => x.PhatSinhNo - x.PhatSinhCo);
                }
                lstChiTietCongNoTheoCongTyFilter.Insert(0, dauKy);
                dauKyNos.Add(dauKy);
                #endregion
                var isFirst = true;
                decimal cuoiKy = 0;
                foreach (var chiTiet in lstChiTietCongNoTheoCongTyFilter)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        cuoiKy = chiTiet.CuoiKyNo;
                        continue;
                    }
                    else
                    {
                        chiTiet.CuoiKyNo = (cuoiKy + chiTiet.PhatSinhNo) - chiTiet.PhatSinhCo;
                        cuoiKy = chiTiet.CuoiKyNo;
                    }
                }
                gridData.AddRange(lstChiTietCongNoTheoCongTyFilter);
            }

            // gán data và grid data source
            gridData = gridData.GroupBy(x => x.CongTyId)
                .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = item.First().CongTyId,
                    TenCongTy = item.First().TenCongTy,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    PhatSinhNo = item.Sum(x => x.PhatSinhNo),
                    PhatSinhCo = item.Sum(x => x.PhatSinhCo),
                    CuoiKyNo = item.Last().CuoiKyNo
                }).ToList();

            var dataTakeByQueryInfo = gridData.OrderBy(o => o.TenCongTy).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            if (dataTakeByQueryInfo.Select(x => x.CongTyId).Distinct().ToList().Count > 1 && dataTakeByQueryInfo.Any())
            {
                var thongTinChung = new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = 0,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    DienGiai = Enums.DienGiaiGridGachNo.DauKy.GetDescription(),
                    DauKyNo = dauKyNos.Where(x => dataTakeByQueryInfo.Any(a => a.CongTyId == x.CongTyId)).Sum(x => x.DauKyNo),
                    CuoiKyNo = dataTakeByQueryInfo.Sum(x => x.CuoiKyNo)
                };
                dataTakeByQueryInfo.Insert(0, thongTinChung);
            }
            var newDataSource = new BaoCaoChiTietGachNoCongTyBhtnGrid
            {
                Data = dataTakeByQueryInfo.ToArray(),
                TotalRowCount = gridData.Count
            };
            var baoCaoGachNoCongTyBhtnGridDatasourceVo = new BaoCaoGachNoCongTyBhtnGridDatasourceVo()
            {
                DataSource = newDataSource,
                TotalPhatSinhNo = dataTakeByQueryInfo.Where(x => x.CongTyId != 0).Sum(x => x.PhatSinhNo),
                TotalPhatSinhCo = dataTakeByQueryInfo.Where(x => x.CongTyId != 0).Sum(x => x.PhatSinhCo),
                TotalCuoiKyNo = dataTakeByQueryInfo.Where(x => x.CongTyId != 0).Sum(x => x.CuoiKyNo)
            };
            return baoCaoGachNoCongTyBhtnGridDatasourceVo;
        }

        public async Task<BaoCaoChiTietGachNoCongTyBhtnGrid> GetChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            var congTyId = exportExcel ? 0 : long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                .Where(x => x.CongTyBaoHiemTuNhanId != null 
                            && ((exportExcel && congTyId == 0) || x.CongTyBaoHiemTuNhanId == congTyId)
                            && x.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
                .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = item.CongTyBaoHiemTuNhanId.Value,
                    TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                    TaiKhoan = item.TaiKhoan,
                    MaTiepNhan = null,
                    SoChungTu = item.SoChungTu,
                    NgayChungTu = item.NgayChungTu,
                    SoHoaDon = item.SoHoaDon,
                    NgayHoaDon = item.NgayHoaDon,
                    DienGiai = item.DienGiaiChung,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    PhatSinhNo = 0,
                    PhatSinhCo = item.TongTienHachToan,
                    DauKyNo = 0,
                    DauKyCo = 0,
                    CuoiKyNo = 0,
                    CuoiKyCo = 0,

                    NgayThuTien = item.LastTime.Value,
                    YeuCauTiepNhanId = 0,
                    SuDungGoi = item.SuDungGoi
                })
                .Union(_congTyBaoHiemTuNhanCongNoRepository.TableNoTracking
                    .Where(o => o.DaHuy != true && o.TaiKhoanBenhNhanThu != null && o.TaiKhoanBenhNhanThu.DaHuy != true)
                    .Where(x => (exportExcel && congTyId == 0) || x.CongTyBaoHiemTuNhanId == congTyId)
                    .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                    {
                        CongTyId = item.CongTyBaoHiemTuNhanId,
                        TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                        TaiKhoan = null,
                        MaTiepNhan = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        SoChungTu = null,
                        NgayChungTu = null,
                        SoHoaDon = null,
                        NgayHoaDon = null,
                        DienGiai = item.TaiKhoanBenhNhanThu.TaiKhoanBenhNhan.BenhNhan.HoTen,
                        MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                        PhatSinhNo = item.SoTien,
                        PhatSinhCo = 0,
                        DauKyNo = 0,
                        DauKyCo = 0,
                        CuoiKyNo = 0,
                        CuoiKyCo = 0,

                        NgayThuTien = item.CreatedOn.Value,
                        YeuCauTiepNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId,
                        SuDungGoi = (item.YeuCauKhamBenh.YeuCauGoiDichVuId != null && item.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                    || (item.YeuCauDichVuKyThuat.YeuCauGoiDichVuId != null && item.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                    || item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId != null
                    })
                );

            var timKiemNangCaoObj = new BaoCaoGachNoCongTyBhtnQueryInfo();
            if (queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj =
                    JsonConvert.DeserializeObject<BaoCaoGachNoCongTyBhtnQueryInfo>(queryInfo.AdditionalSearchString);

                if (exportExcel && timKiemNangCaoObj.CongTyId != null && timKiemNangCaoObj.CongTyId != 0)
                {
                    query = query.Where(p => p.CongTyId == timKiemNangCaoObj.CongTyId);
                }

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                    query = query.Where(p => p.NgayThuTien.Date <= denNgay.Date);
                }

                if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DungGoi != timKiemNangCaoObj.TrangThai.KhongDungGoi)
                {
                    if (timKiemNangCaoObj.TrangThai.DungGoi && !timKiemNangCaoObj.TrangThai.KhongDungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi == true);
                    }
                    else if (timKiemNangCaoObj.TrangThai.KhongDungGoi && !timKiemNangCaoObj.TrangThai.DungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi != true);
                    }
                }
            }
            var data = await query.OrderBy(x => x.NgayThuTien).ToListAsync();
            //var congTyDetail = data.Select(x => new { x.CongTyId, x.TenCongTy }).FirstOrDefault();
            var gridData = new List<BaoCaoGachNoCongTyBhtnGridVo>();

            var lstCongTy = data.Select(x => new { x.CongTyId, x.TenCongTy }).Distinct().ToList();
            foreach (var congTy in lstCongTy)
            {
                var lstChiTietCongNoTheoCongTy = data.Where(x => x.CongTyId == congTy.CongTyId).ToList();
                var lstChiTietCongNoDauKyTheoCongTy = new List<BaoCaoGachNoCongTyBhtnGridVo>();
                var lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Select(x => x).ToList();

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayThuTien < tuNgay.Date).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date >= tuNgay.Date).ToList();
                }

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date <= denNgay.Date).ToList();
                }

                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    var searchString = timKiemNangCaoObj.SearchString.Trim().RemoveVietnameseDiacritics().ToLower();

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(x =>
                        (string.IsNullOrEmpty(x.TaiKhoan) || !x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.MaTiepNhan) || !x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.DienGiai) || !x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoChungTu) || !x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoHoaDon) || !x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Where(x =>
                        (!string.IsNullOrEmpty(x.TaiKhoan) && x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.MaTiepNhan) && x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.DienGiai) && x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoHoaDon) && x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                }

                if (!lstChiTietCongNoTheoCongTyFilter.Any())
                {
                    continue;
                }

                #region Đầu kỳ
                var dauKy = new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = congTy.CongTyId,
                    TenCongTy = congTy.TenCongTy,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    DienGiai = Enums.DienGiaiGridGachNo.DauKy.GetDescription(),
                    NgayThuTien = lstChiTietCongNoTheoCongTy.First().NgayThuTien.AddMilliseconds(-1)
                };
                if (lstChiTietCongNoDauKyTheoCongTy.Any())
                {
                    dauKy.DauKyNo = dauKy.CuoiKyNo = lstChiTietCongNoDauKyTheoCongTy.Sum(x => x.PhatSinhNo - x.PhatSinhCo);
                }
                lstChiTietCongNoTheoCongTyFilter.Insert(0, dauKy);
                #endregion

                var isFirst = true;
                decimal cuoiKy = 0;
                foreach (var chiTiet in lstChiTietCongNoTheoCongTyFilter)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        cuoiKy = chiTiet.CuoiKyNo;
                        continue;
                    }
                    else
                    {
                        chiTiet.CuoiKyNo = (cuoiKy + chiTiet.PhatSinhNo) - chiTiet.PhatSinhCo;
                        cuoiKy = chiTiet.CuoiKyNo;
                    }
                }
                gridData.AddRange(lstChiTietCongNoTheoCongTyFilter);
            }

            // gán data và grid data source
            var dataTakeByQueryInfo = exportExcel ? gridData.OrderBy(o => o.NgayThuTien).ToArray() : gridData.OrderBy(o => o.NgayThuTien).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var newDataSource = new BaoCaoChiTietGachNoCongTyBhtnGrid
            {
                Data = dataTakeByQueryInfo,
                TotalRowCount = gridData.Count
            };
            return newDataSource;
        }
        public async Task<GridDataSource> GetTotalChiTietBaoCaoGachNoCongTyBaoHiemTuNhanForGridAsync(QueryInfo queryInfo)
        {
            var congTyId = long.Parse(queryInfo.SearchTerms);
            var query = BaseRepository.TableNoTracking
                .Where(x => x.CongTyBaoHiemTuNhanId == congTyId
                            && x.TrangThai == Enums.TrangThaiGachNo.XacNhanNhapLieu)
                .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = item.CongTyBaoHiemTuNhanId.Value,
                    TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                    TaiKhoan = item.TaiKhoan,
                    MaTiepNhan = null,
                    SoChungTu = item.SoChungTu,
                    NgayChungTu = item.NgayChungTu,
                    SoHoaDon = item.SoHoaDon,
                    NgayHoaDon = item.NgayHoaDon,
                    DienGiai = item.DienGiaiChung,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    PhatSinhNo = 0,
                    PhatSinhCo = item.TongTienHachToan,
                    DauKyNo = 0,
                    DauKyCo = 0,
                    CuoiKyNo = 0,
                    CuoiKyCo = 0,

                    NgayThuTien = item.LastTime.Value,
                    SuDungGoi = item.SuDungGoi
                })
                .Union(_congTyBaoHiemTuNhanCongNoRepository.TableNoTracking
                    .Where(o => o.DaHuy != true && o.TaiKhoanBenhNhanThu != null && o.TaiKhoanBenhNhanThu.DaHuy != true)
                    .Where(x => x.CongTyBaoHiemTuNhanId == congTyId)
                    .Select(item => new BaoCaoGachNoCongTyBhtnGridVo()
                    {
                        CongTyId = item.CongTyBaoHiemTuNhanId,
                        TenCongTy = item.CongTyBaoHiemTuNhan.Ten,
                        TaiKhoan = null,
                        MaTiepNhan = item.TaiKhoanBenhNhanThu.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        SoChungTu = null,
                        NgayChungTu = null,
                        SoHoaDon = null,
                        NgayHoaDon = null,
                        DienGiai = item.TaiKhoanBenhNhanThu.TaiKhoanBenhNhan.BenhNhan.HoTen,
                        MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                        PhatSinhNo = item.SoTien,
                        PhatSinhCo = 0,
                        DauKyNo = 0,
                        DauKyCo = 0,
                        CuoiKyNo = 0,
                        CuoiKyCo = 0,

                        NgayThuTien = item.CreatedOn.Value,
                        YeuCauTiepNhanId = item.TaiKhoanBenhNhanThu.YeuCauTiepNhanId,
                        SuDungGoi = (item.YeuCauKhamBenh.YeuCauGoiDichVuId != null && item.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                                    || (item.YeuCauDichVuKyThuat.YeuCauGoiDichVuId != null && item.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                    || item.YeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId != null
                    })
                );

            var timKiemNangCaoObj = new BaoCaoGachNoCongTyBhtnQueryInfo();
            if (queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj =
                    JsonConvert.DeserializeObject<BaoCaoGachNoCongTyBhtnQueryInfo>(queryInfo.AdditionalSearchString);

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                    query = query.Where(p => p.NgayThuTien.Date <= denNgay.Date);
                }

                if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DungGoi != timKiemNangCaoObj.TrangThai.KhongDungGoi)
                {
                    if (timKiemNangCaoObj.TrangThai.DungGoi && !timKiemNangCaoObj.TrangThai.KhongDungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi == true);
                    }
                    else if (timKiemNangCaoObj.TrangThai.KhongDungGoi && !timKiemNangCaoObj.TrangThai.DungGoi)
                    {
                        query = query.Where(x => x.SuDungGoi != true);
                    }
                }
            }
            var data = await query.OrderBy(x => x.NgayThuTien).ToListAsync();
            var congTyDetail = data.Select(x => new { x.CongTyId, x.TenCongTy }).FirstOrDefault();
            var gridData = new List<BaoCaoGachNoCongTyBhtnGridVo>();
            if (congTyDetail != null)
            {
                var lstChiTietCongNoTheoCongTy = data.Where(x => x.CongTyId == congTyDetail.CongTyId).ToList();
                var lstChiTietCongNoDauKyTheoCongTy = new List<BaoCaoGachNoCongTyBhtnGridVo>();
                var lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Select(x => x).ToList();

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgay);

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(p => p.NgayThuTien < tuNgay.Date).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date >= tuNgay.Date).ToList();
                }

                if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
                {
                    DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgay);

                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTyFilter.Where(p => p.NgayThuTien.Date <= denNgay.Date).ToList();
                }


                if (!string.IsNullOrEmpty(timKiemNangCaoObj.SearchString))
                {
                    var searchString = timKiemNangCaoObj.SearchString.Trim().RemoveVietnameseDiacritics().ToLower();

                    lstChiTietCongNoDauKyTheoCongTy = lstChiTietCongNoTheoCongTy.Where(x =>
                        (string.IsNullOrEmpty(x.TaiKhoan) || !x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.MaTiepNhan) || !x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.DienGiai) || !x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoChungTu) || !x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        && (string.IsNullOrEmpty(x.SoHoaDon) || !x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                    lstChiTietCongNoTheoCongTyFilter = lstChiTietCongNoTheoCongTy.Where(x =>
                        (!string.IsNullOrEmpty(x.TaiKhoan) && x.TaiKhoan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.MaTiepNhan) && x.MaTiepNhan.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.DienGiai) && x.DienGiai.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoChungTu) && x.SoChungTu.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))
                        || (!string.IsNullOrEmpty(x.SoHoaDon) && x.SoHoaDon.Trim().RemoveVietnameseDiacritics().ToLower().Contains(searchString))).ToList();
                }

                #region Đầu kỳ
                var dauKy = new BaoCaoGachNoCongTyBhtnGridVo()
                {
                    CongTyId = congTyDetail.CongTyId,
                    TenCongTy = congTyDetail.TenCongTy,
                    MaTienTe = Enums.LoaiTienTe.VND.GetDescription(),
                    DienGiai = Enums.DienGiaiGridGachNo.DauKy.GetDescription(),
                    NgayThuTien = lstChiTietCongNoTheoCongTy.First().NgayThuTien.AddMilliseconds(-1)
                };
                if (lstChiTietCongNoDauKyTheoCongTy.Any())
                {
                    dauKy.DauKyNo = dauKy.CuoiKyNo = lstChiTietCongNoDauKyTheoCongTy.Sum(x => x.PhatSinhNo - x.PhatSinhCo);
                }
                lstChiTietCongNoTheoCongTyFilter.Insert(0, dauKy);
                #endregion

                gridData.AddRange(lstChiTietCongNoTheoCongTyFilter);

                var isFirst = true;
                decimal cuoiKy = 0;
                foreach (var chiTiet in gridData)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        cuoiKy = chiTiet.CuoiKyNo;
                        continue;
                    }
                    else
                    {
                        chiTiet.CuoiKyNo = (cuoiKy + chiTiet.PhatSinhNo) - chiTiet.PhatSinhCo;
                        cuoiKy = chiTiet.CuoiKyNo;
                    }
                }
            }

            // gán data và grid data source
            var dataTakeByQueryInfo = gridData.OrderBy(o => o.NgayThuTien).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var newDataSource = new GridDataSource
            {
                Data = dataTakeByQueryInfo,
                TotalRowCount = gridData.Count
            };
            return newDataSource;
        }
        #endregion
    }
}
