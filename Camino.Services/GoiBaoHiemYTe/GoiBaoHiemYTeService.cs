using System;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BHYT;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoaPhongChuyenKhoas;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BHYT;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.HamGuiHoSoWatchings;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.BaoCaoThucHienCls;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using RestSharp;
using Camino.Core.Infrastructure;
using System.Net.Http;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.HopDongThauVatTus;

namespace Camino.Services.GoiBaoHiemYTe
{
    [ScopedDependency(ServiceType = typeof(IGoiBaoHiemYTeService))]
    public partial class GoiBaoHiemYTeService
        : MasterFileService<YeuCauTiepNhan>, IGoiBaoHiemYTeService
    {
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<KhoaPhongChuyenKhoa> _khoaPhongChuyenKhoaRepository;
        private readonly IRepository<Core.Domain.Entities.BHYT.DuLieuGuiCongBHYT> _duLieuGuiCongBHYT;
        private readonly IRepository<Core.Domain.Entities.BHYT.YeuCauTiepNhanDuLieuGuiCongBHYT> _yeuCauTiepNhanDuLieuGuiCongBHYT;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<NoiTruHoSoKhac> _noiTruHoSoKhacRepository;
        private readonly IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        private const string FormatNgayBHYT = "yyyyMMdd";
        private const string FormatNgayGioBHYT = "yyyyMMddHHmm";
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        private readonly IRepository<HopDongThauDuocPham> _hopDongThauDuocPhamRepository;
        private readonly IRepository<HopDongThauVatTu> _hopDongThauVatTuRepository;
        private readonly IRepository<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhamRepository;
        private readonly ILoggerManager _logger;

        public GoiBaoHiemYTeService(
            IRepository<YeuCauTiepNhan> repository, IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
            IRepository<Core.Domain.Entities.BHYT.DuLieuGuiCongBHYT> duLieuGuiCongBHYT,
            ICauHinhService cauHinhService, IRepository<ICD> icdRepository,
            IRepository<Core.Domain.Entities.BHYT.YeuCauTiepNhanDuLieuGuiCongBHYT> yeuCauTiepNhanDuLieuGuiCongBHYT,
            IRepository<KhoaPhongChuyenKhoa> khoaPhongChuyenKhoaRepository,
            IRepository<NoiTruHoSoKhac> noiTruHoSoKhacRepository,
            IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository,
            IRepository<User> userRepository,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            ILoggerManager logger,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuatTuongTrinhPTTT> yeuCauDichVuKyThuatTuongTrinhPTTTRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
            IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository,
            IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository,
            IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository,
            IRepository<NoiTruChiDinhDuocPham> noiTruChiDinhDuocPhamRepository,
            IRepository<HopDongThauDuocPham> hopDongThauDuocPhamRepository,
            IRepository<HopDongThauVatTu> hopDongThauVatTuRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKhamBenhBenhViens.DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository
        ) : base(repository)
        {
            _icdRepository = icdRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _hopDongThauVatTuRepository = hopDongThauVatTuRepository;
            _benhVienRepository = benhVienRepository;
            _duLieuGuiCongBHYT = duLieuGuiCongBHYT;
            _yeuCauTiepNhanDuLieuGuiCongBHYT = yeuCauTiepNhanDuLieuGuiCongBHYT;
            _cauHinhService = cauHinhService;
            _noiTruHoSoKhacRepository = noiTruHoSoKhacRepository;
            _khoaPhongChuyenKhoaRepository = khoaPhongChuyenKhoaRepository;
            _userRepository = userRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _nhanVienRepository = nhanVienRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository = yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository;
            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _noiTruChiDinhDuocPhamRepository = noiTruChiDinhDuocPhamRepository;
            _hopDongThauDuocPhamRepository = hopDongThauDuocPhamRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _yeuCauDichVuKyThuatTuongTrinhPTTTRepository = yeuCauDichVuKyThuatTuongTrinhPTTTRepository;
            _logger = logger;
        }


        #region Danh Sách Chờ Gởi Lên BHYT

        public List<LookupItemVo> GetThongTinGoiBHYTVersion(long yeuCauTiepNhan)
        {

            var yeuCauTiepNhanDuLieuGuiCongBHYTs = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan)
                .Select(cc => new LookupItemVo
                {
                    KeyId = cc.Version,
                    DisplayName = "Lần " + cc.Version + "-" + cc.CreatedOn.Value.ApplyFormatDate()
                }).ToList();

            return yeuCauTiepNhanDuLieuGuiCongBHYTs;
        }
        public async Task<GridDataSource> GetDanhSachGoiBaoHiemYteForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            BuildDefaultSortExpression(queryInfo);
            GoiBaoHiemYTeVo goiBaoHiemYTeVoInfo = null;
            DateTime tuNgay = DateTime.Now.Date;
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                goiBaoHiemYTeVoInfo = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.FromDate) || !string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.ToDate))
                {
                    goiBaoHiemYTeVoInfo.FromDate.TryParseExactCustom(out tuNgay);
                    if (string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        goiBaoHiemYTeVoInfo.ToDate.TryParseExactCustom(out denNgay);
                    }
                }
            }
            var queryNgoaiTru = BaseRepository.TableNoTracking
                .Where(o => o.DaGuiCongBHYT == null && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            o.CoBHYT == true && o.ThoiDiemTiepNhan >= tuNgay && o.ThoiDiemTiepNhan < denNgay);
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                queryNgoaiTru = queryNgoaiTru.ApplyLike(queryInfo.SearchTerms, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var ngoaiTruAll = queryNgoaiTru.Select(o => new GoiBaoHiemYTeDataVo
            {
                Id = o.Id,
                MaTN = o.MaYeuCauTiepNhan,
                MaBN = o.BenhNhan.MaBN,
                HoTen = o.HoTen,
                NamSinh = o.NamSinh,
                GioiTinh = o.GioiTinh,
                MucHuong = o.BHYTMucHuong,
                ThoiDiemTiepNhan = o.ThoiDiemTiepNhan,
                QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru,
                DiaChi = o.DiaChiDayDu,
                //update 15/04/2022: bỏ kiểm tra dv khám
                //YeuCauKhamBenhBHYTIds = o.YeuCauKhamBenhs
                //    .Where(k => k.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true).Select(k => k.Id).ToList()
            }).ToList();
            var ngoaiTruData = ngoaiTruAll.ToList();

            var ngoaiTruIds = ngoaiTruData.Where(o => o.QuyetToanTheoNoiTru == true).Select(o => o.Id).ToList();
            var queryNoiTru = BaseRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy && o.NoiTruBenhAn.ThoiDiemRaVien != null &&
                            (o.CoBHYT == true) && o.NoiTruBenhAn != null && ngoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()));

            var noiTruData = queryNoiTru.Select(o => new
            {
                o.Id,
                o.YeuCauTiepNhanNgoaiTruCanQuyetToanId
            }).ToList();

            //update 18/04/2022: kiểm tra có dv duoc duyệt
            var allYeuCauTiepNhanIds = ngoaiTruData.Select(o => o.Id).Concat(noiTruData.Select(o => o.Id)).ToList();

            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauDVKTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauDuocPhams= _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault()) && o.DonThuocThanhToan.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, YeuCauTiepNhanId = o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault() }).ToList();
            var yeuCauDichVuGiuongs = _yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();


            var returnData = new List<GoiBaoHiemYTeVo>();
            foreach (var ngoaiTru in ngoaiTruData)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == ngoaiTru.Id);
                if (ngoaiTru.QuyetToanTheoNoiTru != true || noiTru != null)
                {
                    var checkDuocChiTra = yeuCauKhams.Any(o=>o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDVKTs.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDuocPhams.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauVatTus.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            donThuocThanhToanChiTiets.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDichVuGiuongs.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id));

                    if(checkDuocChiTra)
                    {
                        returnData.Add(new GoiBaoHiemYTeVo
                        {
                            Id = ngoaiTru.Id,
                            MaTN = ngoaiTru.MaTN,
                            MaBN = ngoaiTru.MaBN,
                            HoTen = ngoaiTru.HoTen,
                            NamSinh = ngoaiTru.NamSinh?.ToString(),
                            MucHuong = "BHYT(" + ngoaiTru.MucHuong.GetValueOrDefault() + "%)",
                            Ngay = ngoaiTru.ThoiDiemTiepNhan,
                            ThoiGianTiepNhanStr = ngoaiTru.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                            GioiTinh = ngoaiTru.GioiTinh?.GetDescription(),
                            DiaChi = ngoaiTru.DiaChi
                        });
                    }
                }
            }

            return new GridDataSource { Data = returnData.OrderBy(o => o.Ngay).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = returnData.Count };
        }
        
        public async Task<GridDataSource> GetDanhSachGoiBaoHiemYteTotalPageForGridAsync(QueryInfo queryInfo)
        {
            GoiBaoHiemYTeVo goiBaoHiemYTeVoInfo = null;
            DateTime tuNgay = DateTime.Now.Date;
            DateTime denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                goiBaoHiemYTeVoInfo = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.FromDate) || !string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.ToDate))
                {
                    goiBaoHiemYTeVoInfo.FromDate.TryParseExactCustom(out tuNgay);
                    if (string.IsNullOrEmpty(goiBaoHiemYTeVoInfo.ToDate))
                    {
                        denNgay = DateTime.Now;
                    }
                    else
                    {
                        goiBaoHiemYTeVoInfo.ToDate.TryParseExactCustom(out denNgay);
                    }
                }
            }
            var queryNgoaiTru = BaseRepository.TableNoTracking
                .Where(o => o.DaGuiCongBHYT == null && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            o.CoBHYT == true && o.ThoiDiemTiepNhan >= tuNgay && o.ThoiDiemTiepNhan < denNgay);
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                queryNgoaiTru = queryNgoaiTru.ApplyLike(queryInfo.SearchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN);
            }

            var ngoaiTruAll = queryNgoaiTru.Select(o => new GoiBaoHiemYTeDataVo
            {
                Id = o.Id,
                MaTN = o.MaYeuCauTiepNhan,
                MaBN = o.BenhNhan.MaBN,
                HoTen = o.HoTen,
                NamSinh = o.NamSinh,
                GioiTinh = o.GioiTinh,
                MucHuong = o.BHYTMucHuong,
                ThoiDiemTiepNhan = o.ThoiDiemTiepNhan,
                QuyetToanTheoNoiTru = o.QuyetToanTheoNoiTru,
                DiaChi = o.DiaChiDayDu,
                //update 15/04/2022: bỏ kiểm tra dv khám
                //YeuCauKhamBenhBHYTIds = o.YeuCauKhamBenhs
                //    .Where(k => k.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && k.BaoHiemChiTra != null && k.BaoHiemChiTra == true).Select(k => k.Id).ToList()
            }).ToList();
            var ngoaiTruData = ngoaiTruAll.ToList();

            var ngoaiTruIds = ngoaiTruData.Where(o => o.QuyetToanTheoNoiTru == true).Select(o => o.Id).ToList();
            var queryNoiTru = BaseRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
                            o.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy && o.NoiTruBenhAn.ThoiDiemRaVien != null &&
                            (o.CoBHYT == true) && o.NoiTruBenhAn != null && ngoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()));

            var noiTruData = queryNoiTru.Select(o => new
            {
                o.Id,
                o.YeuCauTiepNhanNgoaiTruCanQuyetToanId
            }).ToList();

            //update 18/04/2022: kiểm tra có dv duoc duyệt
            var allYeuCauTiepNhanIds = ngoaiTruData.Select(o => o.Id).Concat(noiTruData.Select(o => o.Id)).ToList();

            var yeuCauKhams = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauDVKTs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauDuocPhams = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var yeuCauVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();
            var donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault()) && o.DonThuocThanhToan.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, YeuCauTiepNhanId = o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault() }).ToList();
            var yeuCauDichVuGiuongs = _yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository.TableNoTracking
                .Where(o => allYeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.BaoHiemChiTra == true)
                .Select(o => new { o.Id, o.YeuCauTiepNhanId }).ToList();

            var returnData = new List<GoiBaoHiemYTeVo>();
            foreach (var ngoaiTru in ngoaiTruData)
            {
                var noiTru = noiTruData.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == ngoaiTru.Id);
                if (ngoaiTru.QuyetToanTheoNoiTru != true || noiTru != null)
                {
                    var checkDuocChiTra = yeuCauKhams.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDVKTs.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDuocPhams.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauVatTus.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            donThuocThanhToanChiTiets.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id)) ||
                                            yeuCauDichVuGiuongs.Any(o => o.YeuCauTiepNhanId == ngoaiTru.Id || (noiTru != null && o.YeuCauTiepNhanId == noiTru.Id));

                    if (checkDuocChiTra)
                    {
                        returnData.Add(new GoiBaoHiemYTeVo
                        {
                            Id = ngoaiTru.Id,
                            MaTN = ngoaiTru.MaTN,
                            MaBN = ngoaiTru.MaBN,
                            HoTen = ngoaiTru.HoTen,
                            NamSinh = ngoaiTru.NamSinh?.ToString(),
                            MucHuong = "BHYT(" + ngoaiTru.MucHuong.GetValueOrDefault() + "%)",
                            Ngay = ngoaiTru.ThoiDiemTiepNhan,
                            ThoiGianTiepNhanStr = ngoaiTru.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                            GioiTinh = ngoaiTru.GioiTinh?.GetDescription(),
                            DiaChi = ngoaiTru.DiaChi
                        });
                    }
                }
            }

            return new GridDataSource { TotalRowCount = returnData.Count };
        }
        
        //public void CapNhatThongTinBHYT(long yeuCauTiepNhanId, HamGuiHoSoWatching hamGuiHoSoWatching)
        //{
        //    var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).FirstOrDefault();
        //    yeuCauTiepNhan.DaGuiCongBHYT = true;
        //    var versionLast = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhanId).Select(xx => xx.Version).LastOrDefault();
        //    var duLieuGuiCongBHYT = new Core.Domain.Entities.BHYT.DuLieuGuiCongBHYT()
        //    {
        //        DuLieuTongHop = hamGuiHoSoWatching.DataJson
        //    };
        //    duLieuGuiCongBHYT.YeuCauTiepNhanDuLieuGuiCongBHYTs.Add(new YeuCauTiepNhanDuLieuGuiCongBHYT()
        //    {
        //        YeuCauTiepNhanId = yeuCauTiepNhanId,
        //        DuLieuGuiCongBHYTId = duLieuGuiCongBHYT.Id,
        //        DuLieu = hamGuiHoSoWatching.DataJson,
        //        Version = versionLast + 1,
        //    });

        //    BaseRepository.Update(yeuCauTiepNhan);
        //    _duLieuGuiCongBHYT.Add(duLieuGuiCongBHYT);
        //}

        #endregion

        #region Danh Sách Lịch Sử BHYT

        public async Task<GridDataSource> GetDanhSachLichSuBHYTForGridAsync(QueryInfo queryInfo, bool isAllData = false)
        {
            var tuNgay = DateTime.Now.AddYears(-1);
            var denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);
                
                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.FromDate.TryParseExactCustom(out tuNgay);
                    queryString.ToDate.TryParseExactCustom(out denNgay);                    
                }
            }

            var yctns = BaseRepository.TableNoTracking.Where(o => o.DaGuiCongBHYT != null && o.DaGuiCongBHYT == true &&
             o.YeuCauTiepNhanDuLieuGuiCongBHYTs.Any(x=> x.CreatedOn > tuNgay && x.CreatedOn < denNgay));
            
            var query = yctns.Select(s => new GoiBaoHiemYTeVo
            {
                Id = s.Id,
                MaTN = s.MaYeuCauTiepNhan,
                MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                HoTen = s.HoTen,
                NamSinh = s.NamSinh.Value.ToString(),
                Ngay = s.ThoiDiemTiepNhan,
                MucHuong = "BHYT(" + s.BHYTMucHuong.GetValueOrDefault() + "%)",
                ThoiGianTiepNhanStr = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                GioiTinh = s.GioiTinh.GetDescription(),
                DiaChi = s.DiaChiDayDu
            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.TimKiem))
                {
                    query = query.ApplyLike(queryString.TimKiem.RemoveUniKeyAndToLower().Trim(),
                        g => g.MaBN.ToString().RemoveUniKeyAndToLower(),
                        g => g.HoTen.RemoveUniKeyAndToLower(),
                        g => g.DiaChi.RemoveUniKeyAndToLower(),
                        g => g.NamSinh.ToString().RemoveUniKeyAndToLower(),
                        g => g.MaTN.RemoveUniKeyAndToLower());

                }

                //if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                //{
                //    DateTime denNgay;
                //    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                //    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, System.Globalization.DateTimeStyles.None, out var tuNgay);
                //    if (string.IsNullOrEmpty(queryString.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryString.ToDate.TryParseExactCustom(out denNgay);
                //        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, System.Globalization.DateTimeStyles.None, out denNgay);
                //    }

                //    query = query.Where(p => p.Ngay >= tuNgay && p.Ngay <= denNgay.AddSeconds(59).AddMilliseconds(999));
                //}

            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = isAllData == true ? query.OrderBy(queryInfo.SortString).ToArrayAsync() :
                         query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDanhSachLichSuBHYTTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var tuNgay = DateTime.Now.AddYears(-1);
            var denNgay = DateTime.Now;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);

                if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                {
                    queryString.FromDate.TryParseExactCustom(out tuNgay);
                    queryString.ToDate.TryParseExactCustom(out denNgay);
                }
            }

            var yctns = BaseRepository.TableNoTracking.Where(o => o.DaGuiCongBHYT != null && o.DaGuiCongBHYT == true &&
             o.YeuCauTiepNhanDuLieuGuiCongBHYTs.Any(x => x.CreatedOn > tuNgay && x.CreatedOn < denNgay));
            
            var query = yctns.Select(s => new GoiBaoHiemYTeVo
            {
                Id = s.Id,
                MaTN = s.MaYeuCauTiepNhan,
                MaBN = s.BenhNhan != null ? s.BenhNhan.MaBN : "",
                HoTen = s.HoTen,
                NamSinh = s.NamSinh.Value.ToString(),
                Ngay = s.ThoiDiemTiepNhan,
                MucHuong = "BHYT(" + s.BHYTMucHuong.GetValueOrDefault() + "%)",
                ThoiGianTiepNhanStr = s.ThoiDiemTiepNhan.ApplyFormatDateTimeSACH(),
                GioiTinh = s.GioiTinh.GetDescription(),
                DiaChi = s.DiaChiDayDu
            });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<GoiBaoHiemYTeVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.TimKiem))
                {
                    query = query.ApplyLike(queryString.TimKiem.RemoveUniKeyAndToLower().Trim(),
                        g => g.MaBN.ToString().RemoveUniKeyAndToLower(),
                        g => g.HoTen.RemoveUniKeyAndToLower(),
                        g => g.DiaChi.RemoveUniKeyAndToLower(),
                        g => g.NamSinh.ToString().RemoveUniKeyAndToLower(),
                        g => g.MaTN.RemoveUniKeyAndToLower());

                }

                //if (!string.IsNullOrEmpty(queryString.FromDate) || !string.IsNullOrEmpty(queryString.ToDate))
                //{
                //    DateTime denNgay;
                //    queryString.FromDate.TryParseExactCustom(out var tuNgay);
                //    //DateTime.TryParseExact(queryString.FromDate, "dd/MM/yyyy hh:mm tt", null, System.Globalization.DateTimeStyles.None,out var tuNgay);
                //    if (string.IsNullOrEmpty(queryString.ToDate))
                //    {
                //        denNgay = DateTime.Now;
                //    }
                //    else
                //    {
                //        queryString.ToDate.TryParseExactCustom(out denNgay);
                //        //DateTime.TryParseExact(queryString.ToDate, "dd/MM/yyyy hh:mm tt", null, System.Globalization.DateTimeStyles.None, out denNgay);
                //    }

                //    query = query.Where(p => p.Ngay >= tuNgay && p.Ngay <= denNgay.AddSeconds(59));
                //}

            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public ThongTinBenhNhan GetThongTinChiTietBaoHiemYTe(GoiDanhSachThongTinBenhNhanCoBHYT danhSach)
        {
            var thongTinBenhNhanGoiBHYT = new ThongTinBenhNhan();
            if (danhSach.YeuCauTiepNhanIds.Count() > 0)
            {
                foreach (var yeuCauTiepNhan in danhSach.YeuCauTiepNhanIds)
                {

                    var duLieu = danhSach.Version != 0 ?
                        _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan && cc.Version == danhSach.Version).OrderByDescending(cc => cc.Version).FirstOrDefault() :
                    _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan).OrderByDescending(cc => cc.Version)
                                                                     .FirstOrDefault();
                    if (duLieu != null)
                    {
                        thongTinBenhNhanGoiBHYT = JsonConvert.DeserializeObject<ThongTinBenhNhan>(duLieu.DuLieu);
                        thongTinBenhNhanGoiBHYT.Version = duLieu.Version;
                    }
                }
            }


            return thongTinBenhNhanGoiBHYT;
        }

        #endregion


        #region CẬP NHẬT HÀM GỞI GIÁM ĐINH VÀ XUẤT XML VA LOAD THÔNG TIN BHYT

        public List<ThongTinBenhNhan> GetThongTinBenhNhanCoBHYT(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanIds)
        {
            var khoaPhongChuyenKhoas = _khoaPhongChuyenKhoaRepository.TableNoTracking.Include(o => o.Khoa).ToList();
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var danhSachYeuCauTiepNhanNgoaiTruIds = danhSachYeuCauTiepNhanIds.Id.Where(o => o != 0);

            var yctnNgoaiTruVaNoiTrus = BaseRepository.Table
                .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && ((danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.Id) && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.DaGuiCongBHYT != true) ||
                            (danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()))))
                         .Include(cc => cc.PhuongXa)
                        .Include(cc => cc.QuanHuyen)
                        .Include(cc => cc.TinhThanh)
                        .Include(cc => cc.NoiChuyen)
                        .Include(cc => cc.BenhNhan)
                        .Include(cc => cc.BHYTGiayMienCungChiTra)
                        //.Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruPhieuDieuTris).ThenInclude(cc => cc.NoiTruThamKhamChanDoanKemTheos)
                        //.Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.ChanDoanChinhRaVienICD)
                        //.Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruKhoaPhongDieuTris)
                        //.Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruEkipDieuTris).ThenInclude(cc => cc.BacSi)
                        .Include(cc => cc.KetQuaSinhHieus)
                        .Include(cc => cc.YeuCauNhapVien)
                        .ToList();

            //var yctnCoBHYTNgoaiTruVaNoiTrus = BaseRepository.Table
            //    .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && ((danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.Id) && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.DaGuiCongBHYT != true) ||
            //                (danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()))))
            //             .Include(cc => cc.PhuongXa)
            //            .Include(cc => cc.QuanHuyen)
            //            .Include(cc => cc.TinhThanh)
            //            .Include(cc => cc.NoiChuyen)
            //            .Include(cc => cc.BenhNhan)
            //            .Include(cc => cc.BHYTGiayMienCungChiTra)
            //            .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruPhieuDieuTris).ThenInclude(cc => cc.NoiTruThamKhamChanDoanKemTheos)
            //            .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.ChanDoanChinhRaVienICD)
            //            .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruKhoaPhongDieuTris)
            //            .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruEkipDieuTris).ThenInclude(cc => cc.BacSi)
            //            .Include(cc => cc.KetQuaSinhHieus)
            //            .Include(cc => cc.YeuCauNhapVien)
            //            .ToList();

            //foreach(var yctn in yctnCoBHYTNgoaiTruVaNoiTrus)
            //{
            //    //Explicit loading
            //    var yeuCauKhamBenhs = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauKhamBenhs);
            //    yeuCauKhamBenhs.Query()
            //        .Include(c => c.BenhVienChuyenVien)
            //        .Include(c => c.Icdchinh)
            //        .Include(c => c.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
            //        .Include(c => c.NhomGiaDichVuKhamBenhBenhVien)
            //        .Include(cc => cc.DichVuKhamBenhBenhVien).ThenInclude(cc => cc.DichVuKhamBenh)
            //        .Include(c => c.NoiThucHien).ThenInclude(c => c.KhoaPhong)
            //        .Include(c => c.BacSiThucHien).ThenInclude(c => c.User)
            //        .Load();

            //    var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDichVuKyThuats);
            //    yeuCauDichVuKyThuats.Query()
            //        .Include(c => c.NhomGiaDichVuKyThuatBenhVien)
            //        .Include(c => c.YeuCauVatTuBenhViens)
            //        .Include(c => c.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
            //        .Include(c => c.NhanVienThucHien)
            //        .Include(c => c.NhanVienChiDinh)
            //        .Include(c => c.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuXetNghiem)
            //        .Include(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(c => c.Icdchinh)
            //        .Include(cc => cc.YeuCauDichVuKyThuatTuongTrinhPTTT)
            //        .Load();

            //    var yeuCauDichVuGiuongBenhVienChiPhiBHYTs = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs);
            //    yeuCauDichVuGiuongBenhVienChiPhiBHYTs.Query()
            //        .Include(cc => cc.DichVuGiuongBenhVien).ThenInclude(cc => cc.DichVuGiuong)
            //        .Include(cc => cc.GiuongBenh)
            //        .Load();

            //    var yeuCauDuocPhamBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDuocPhamBenhViens);
            //    yeuCauDuocPhamBenhViens.Query()
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
            //        .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Include(cc => cc.DuocPhamBenhVien).ThenInclude(cc => cc.DuocPham)
            //        .Include(cc => cc.DuocPhamBenhVien)
            //        .Include(cc => cc.DonViTinh)
            //        .Include(cc => cc.DuongDung)
            //        .Include(cc => cc.NoiTruChiDinhDuocPham)
            //        .Include(cc => cc.NhanVienChiDinh)
            //        .Include(cc => cc.NoiChiDinh)
            //        .Include(cc => cc.XuatKhoDuocPhamChiTiet).ThenInclude(cc => cc.XuatKhoDuocPhamChiTietViTris).ThenInclude(cc => cc.NhapKhoDuocPhamChiTiet).ThenInclude(cc => cc.HopDongThauDuocPhams)
            //        .Load();

            //    var yeuCauVatTuBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauVatTuBenhViens);
            //    yeuCauVatTuBenhViens.Query()
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
            //        .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
            //        .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus)
            //        .Include(cc => cc.XuatKhoVatTuChiTiet).ThenInclude(cc => cc.XuatKhoVatTuChiTietViTris).ThenInclude(cc => cc.NhapKhoVatTuChiTiet).ThenInclude(cc => cc.HopDongThauVatTu)
            //        .Include(cc => cc.NhanVienChiDinh)
            //        .Include(cc => cc.NoiChiDinh)
            //        .Load();

            //    var donThuocThanhToans = BaseRepository.Context.Entry(yctn).Collection(o => o.DonThuocThanhToans);
            //    donThuocThanhToans.Query()
            //        .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuocPham).ThenInclude(o => o.DuocPhamBenhVien)
            //        .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuongDung)
            //        .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonViTinh)
            //        .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.YeuCauKhamBenhDonThuocChiTiet)
            //        .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.NoiTruDonThuocChiTiet)
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien).ThenInclude(c => c.User)
            //        .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
            //        .Load();
            //}

            var yeuCauTiepNhanIds = yctnNgoaiTruVaNoiTrus.Select(o => o.Id).ToList();
            var yeuCauTiepNhanNoiTruIds = yctnNgoaiTruVaNoiTrus.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.Id).ToList();

            var noiTruBenhAns = _noiTruBenhAnRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.ThoiDiemNhapVien,
                    o.ThoiDiemRaVien,
                    o.KetQuaDieuTri,
                    o.TinhTrangRaVien,
                    o.ThongTinRaVien,
                    o.ChanDoanChinhRaVienICDId,
                    o.DanhSachChanDoanKemTheoRaVienICDId,
                    o.LoaiBenhAn,
                    NoiTruEkipDieuTris = o.NoiTruEkipDieuTris.Select(ek => new 
                    {
                        ek.Id,
                        ek.TuNgay,
                        ek.DenNgay,
                        ek.BacSi.MaChungChiHanhNghe
                    }).ToList(),
                    NoiTruKhoaPhongDieuTris = o.NoiTruKhoaPhongDieuTris.Select(kp => new
                    {
                        kp.Id,
                        kp.ThoiDiemVaoKhoa,
                        kp.KhoaPhongChuyenDenId
                    }).ToList(),
                    NoiTruPhieuDieuTris = o.NoiTruPhieuDieuTris.Select(pdt => new
                    {
                        pdt.Id,
                        pdt.NgayDieuTri,
                        pdt.DienBien,
                        pdt.ChanDoanChinhICDId,
                        NoiTruThamKhamChanDoanKemTheos = pdt.NoiTruThamKhamChanDoanKemTheos.ToList()
                    }).ToList()
                })
                .ToList();

            var allYeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new
                {
                    o.Id,
                    o.MaDichVu,
                    o.TenDichVu,
                    o.DataKetQuaCanLamSang,
                    o.YeuCauTiepNhanId,
                    o.LoaiDichVuKyThuat,
                    o.DichVuKyThuatBenhVienId,
                    o.TrangThai,
                    o.BaoHiemChiTra,
                    o.ThoiDiemThucHien,
                    o.ThoiDiemDangKy,
                    o.ThoiDiemKetLuan,
                    o.ThoiDiemHoanThanh,
                    o.NoiTruPhieuDieuTriId,
                    o.NhomChiPhi,
                    o.SoLan,
                    o.DonGiaBaoHiem,
                    o.TiLeBaoHiemThanhToan,
                    o.MucHuongBaoHiem,
                    NoiThucHienKhoaPhongId = o.NoiThucHienId != null ? (long?)o.NoiThucHien.KhoaPhongId : (o.NoiChiDinhId != null ? (long?)o.NoiChiDinh.KhoaPhongId : null),
                    MaChungChiNhanVienThucHien = (o.NhanVienThucHienId != null && o.NhanVienThucHien.MaChungChiHanhNghe != null) ? o.NhanVienThucHien.MaChungChiHanhNghe : o.NhanVienChiDinh.MaChungChiHanhNghe,
                })
                .ToList();

            var allYeuCauKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new
                {
                    o.Id,
                    o.DichVuKhamBenhBenhVienId,
                    o.MaDichVu,
                    o.TenDichVu,
                    o.YeuCauTiepNhanId,
                    o.DonGiaBaoHiem,
                    o.DuocHuongBaoHiem,
                    o.BaoHiemChiTra,
                    o.MucHuongBaoHiem,
                    o.TiLeBaoHiemThanhToan,
                    o.TrangThai,
                    o.IcdchinhId,
                    o.GhiChuICDChinh,
                    o.ThoiDiemDangKy,
                    o.ThoiDiemHoanThanh,
                    o.ThoiDiemChiDinh,
                    o.KetQuaDieuTri,
                    o.TinhTrangRaVien,
                    MaChungChiBacSiThucHien = o.BacSiThucHienId != null ? o.BacSiThucHien.MaChungChiHanhNghe : null,
                    NoiThucHienKhoaPhongId = o.NoiThucHienId != null ? (long?)o.NoiThucHien.KhoaPhongId : null,
                    BenhVienChuyenVienMa = o.BenhVienChuyenVienId != null ? o.BenhVienChuyenVien.Ma : null,
                    YeuCauKhamBenhICDKhacs = o.YeuCauKhamBenhICDKhacs.Select(ICDKhac => new
                    {
                        ICDKhac.ICDId,
                        ICDKhac.GhiChu
                    }).ToList()
                })
                .ToList();

            var allYeuCauDichVuGiuongBenhVienChiPhiBHYT = _yeuCauDichVuGiuongBenhVienChiPhiBHYTRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.YeuCauTiepNhanId) && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.Id,
                    o.SoLuong,
                    o.KhoaPhongId,
                    MaChung = o.DichVuGiuongBenhVien.DichVuGiuongId != null ? o.DichVuGiuongBenhVien.DichVuGiuong.MaChung : null,
                    TenChung = o.DichVuGiuongBenhVien.DichVuGiuongId != null ? o.DichVuGiuongBenhVien.DichVuGiuong.TenChung : null,
                    MaGiuong = o.GiuongBenhId != null ? o.GiuongBenh.Ma : null,
                    o.NgayPhatSinh,
                    o.YeuCauTiepNhanId,
                    o.DonGiaBaoHiem,
                    o.DuocHuongBaoHiem,
                    o.BaoHiemChiTra,
                    o.MucHuongBaoHiem,
                    o.TiLeBaoHiemThanhToan
                })
                .ToList();
            ;

            var allYeuCauVatTuBenhViens = _yeuCauVatTuBenhVienRepository.TableNoTracking
                    .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && o.BaoHiemChiTra == true)
                    .Select(o => new
                    {
                        o.Id,
                        o.TrangThai,
                        o.BaoHiemChiTra,
                        o.SoLuong,
                        o.YeuCauTiepNhanId,
                        DichVuKyThuatBenhVienId = o.YeuCauDichVuKyThuatId != null ? (long?)o.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : null,
                        o.ThoiDiemChiDinh,
                        o.CreatedOn,
                        o.NoiTruPhieuDieuTriId,
                        o.YeuCauKhamBenhId,
                        o.XuatKhoVatTuChiTietId,
                        o.Ma,
                        o.Ten,
                        o.DonViTinh,
                        o.DonGiaBaoHiem,
                        o.TiLeBaoHiemThanhToan,
                        o.MucHuongBaoHiem,
                        KhoaChiDinhId = o.NoiChiDinh.KhoaPhongId,
                        o.NhanVienChiDinh.MaChungChiHanhNghe,
                        o.GhiChu
                    }).ToList();
            var xuatKhoVatTuChiTietIds = allYeuCauVatTuBenhViens.Where(o => o.XuatKhoVatTuChiTietId != null).Select(o => o.XuatKhoVatTuChiTietId.Value).Distinct().ToList();
            var allXuatKhoVatTuChiTietViTris = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => xuatKhoVatTuChiTietIds.Contains(o.XuatKhoVatTuChiTietId))
                .Select(o => new
                {
                    o.Id,
                    o.SoLuongXuat,
                    o.XuatKhoVatTuChiTietId,
                    o.NhapKhoVatTuChiTietId,
                    o.NhapKhoVatTuChiTiet.HopDongThauVatTuId,
                })
                .ToList();
            var hopDongThauVatTuIds = allXuatKhoVatTuChiTietViTris.Select(o => o.HopDongThauVatTuId).Distinct().ToList();
            var allHopDongThauVatTus = _hopDongThauVatTuRepository.TableNoTracking
                .Where(o => hopDongThauVatTuIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.SoQuyetDinh,
                    o.GoiThau,
                    o.NhomThau
                })
                .ToList();

            var allYeuCauDuocPhamBenhViens = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                    .Where(o => yeuCauTiepNhanIds.Contains(o.YeuCauTiepNhanId) && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.BaoHiemChiTra == true)
                    .Select(o => new
                    {
                        o.Id,
                        o.TrangThai,
                        o.BaoHiemChiTra,
                        o.SoLuong,
                        o.YeuCauTiepNhanId,
                        o.CreatedOn,
                        o.NoiTruPhieuDieuTriId,
                        o.YeuCauKhamBenhId,
                        o.NoiTruChiDinhDuocPhamId,
                        o.XuatKhoDuocPhamChiTietId,
                        o.MaHoatChat,
                        o.Ten,
                        TenDonViTinh = o.DonViTinh.Ten,
                        o.HamLuong,
                        MaDuongDung = o.DuongDung.Ma,
                        o.SoDangKy,
                        o.DonGiaBaoHiem,
                        o.TiLeBaoHiemThanhToan,
                        o.MucHuongBaoHiem,
                        KhoaChiDinhId = o.NoiChiDinh.KhoaPhongId,
                        o.NhanVienChiDinh.MaChungChiHanhNghe,
                        o.GhiChu
                    }).ToList();
            var noiTruChiDinhDuocPhamIds = allYeuCauDuocPhamBenhViens.Where(o => o.NoiTruChiDinhDuocPhamId != null).Select(o => o.NoiTruChiDinhDuocPhamId.Value).Distinct().ToList();
            var xuatKhoDuocPhamChiTietIds = allYeuCauDuocPhamBenhViens.Where(o => o.XuatKhoDuocPhamChiTietId != null).Select(o => o.XuatKhoDuocPhamChiTietId.Value).Distinct().ToList();
            var allNoiTruChiDinhDuocPhams = _noiTruChiDinhDuocPhamRepository.TableNoTracking
                .Where(o => noiTruChiDinhDuocPhamIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.SoLanTrenVien,
                    o.LieuDungTrenNgay
                })
                .ToList();

            var allXuatKhoDuocPhamChiTietViTris = _xuatKhoDuocPhamChiTietViTriRepository.TableNoTracking
                .Where(o => xuatKhoDuocPhamChiTietIds.Contains(o.XuatKhoDuocPhamChiTietId))
                .Select(o => new
                {
                    o.Id,
                    o.SoLuongXuat,
                    o.XuatKhoDuocPhamChiTietId,
                    o.NhapKhoDuocPhamChiTietId,
                    o.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId,
                })
                .ToList();

            var hopDongThauDuocPhamIds = allXuatKhoDuocPhamChiTietViTris.Select(o => o.HopDongThauDuocPhamId).Distinct().ToList();
            var allHopDongThauDuocPhams = _hopDongThauDuocPhamRepository.TableNoTracking
                .Where(o => hopDongThauDuocPhamIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.SoQuyetDinh,
                    o.GoiThau,
                    o.NhomThau
                })
                .ToList();


            var allDonThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                .Where(o => yeuCauTiepNhanIds.Contains(o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault()) && o.DonThuocThanhToan.TrangThai != TrangThaiDonThuocThanhToan.DaHuy && o.BaoHiemChiTra == true)
                .Select(o => new
                {
                    o.Id,
                    o.DonThuocThanhToan.TrangThai,
                    o.DonThuocThanhToan.YeuCauKhamBenhId,
                    o.DonThuocThanhToan.YeuCauTiepNhanId,
                    o.BaoHiemChiTra,
                    SoLanTrenVien = o.YeuCauKhamBenhDonThuocChiTietId != null ? o.YeuCauKhamBenhDonThuocChiTiet.SoLanTrenVien : null,
                    LieuDungTrenNgay = o.YeuCauKhamBenhDonThuocChiTietId != null ? o.YeuCauKhamBenhDonThuocChiTiet.LieuDungTrenNgay : null,
                    o.MaHoatChat,
                    o.Ten,
                    TenDonViTinh = o.DonViTinh.Ten,
                    o.HamLuong,
                    MaDuongDung = o.DuongDung.Ma,
                    o.SoDangKy,
                    o.DonGiaBaoHiem,
                    o.TiLeBaoHiemThanhToan,
                    o.MucHuongBaoHiem,
                    o.SoLuong,
                    o.SoQuyetDinhThau,
                    o.GoiThau,
                    o.NhomThau,
                    o.CreatedOn,
                    GhiChu = o.YeuCauKhamBenhDonThuocChiTietId != null ? o.YeuCauKhamBenhDonThuocChiTiet.GhiChu : (o.NoiTruDonThuocChiTietId != null ? o.NoiTruDonThuocChiTiet.GhiChu : o.GhiChu)
                }).ToList();

            var yeuCauDichVuKyThuatThuThuatPhauThuatIds = allYeuCauDichVuKyThuats.Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).Select(o => o.Id).Distinct().ToList();
            var yeuCauDichVuKyThuatTuongTrinhPTTTs = _yeuCauDichVuKyThuatTuongTrinhPTTTRepository.TableNoTracking
                .Where(o => yeuCauDichVuKyThuatThuThuatPhauThuatIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.TenPhuongPhapPTTT
                })
                .ToList();

            var yeuCauDichVuKyThuatIds = allYeuCauDichVuKyThuats.Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Select(o => o.Id).Distinct().ToList();
                        
            var phienXetNghiemChiTietData = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauDichVuKyThuatIds.Contains(o.YeuCauDichVuKyThuatId)).Include(o => o.KetQuaXetNghiemChiTiets).ToList();

            var bienBanHoiChanData = _noiTruHoSoKhacRepository.TableNoTracking.Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.YeuCauTiepNhanId) && o.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat).ToList();
            var allDichVuKyThuatBenhVienAnhXas = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Where(o => o.DichVuKyThuatId != null)
                .Select(o => new
                {
                    o.Id,
                    o.DichVuKyThuatId,
                    o.DichVuKyThuat.MaChung,
                    o.DichVuKyThuat.TenChung,
                    TenDichVuXetNghiem = o.DichVuXetNghiemId != null ? o.DichVuXetNghiem.Ten : null
                })
                .ToList();
            var allDichVuKhamBenhBenhVienAnhXas = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Where(o => o.DichVuKhamBenhId != null)
                .Select(o => new
                {
                    o.Id,
                    o.DichVuKhamBenhId,
                    o.DichVuKhamBenh.MaChung,
                    o.DichVuKhamBenh.TenChung
                })
                .ToList();
            var icdIds = new List<long>();
            foreach (var yckb in allYeuCauKhamBenhs)
            {
                foreach (var icdKhac in yckb.YeuCauKhamBenhICDKhacs)
                {
                    icdIds.Add(icdKhac.ICDId);
                }
                if (yckb.IcdchinhId != null)
                {
                    icdIds.Add(yckb.IcdchinhId.Value);
                }
            }
            foreach (var yctn in yctnNgoaiTruVaNoiTrus)
            {
                if (yctn.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    var noiTru = noiTruBenhAns.FirstOrDefault(o => o.Id == yctn.Id);
                    if (noiTru != null)
                    {
                        if (noiTru.ChanDoanChinhRaVienICDId != null)
                        {
                            icdIds.Add(noiTru.ChanDoanChinhRaVienICDId.Value);
                        }
                        if (!string.IsNullOrEmpty(noiTru.DanhSachChanDoanKemTheoRaVienICDId))
                        {
                            var icdKemTheoIds = noiTru.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                            foreach (var icdKemTheoId in icdKemTheoIds)
                            {
                                icdIds.Add(long.Parse(icdKemTheoId));
                            }
                        }
                        foreach (var noiTruPhieuDieuTri in noiTru.NoiTruPhieuDieuTris)
                        {
                            if (noiTruPhieuDieuTri.ChanDoanChinhICDId != null)
                            {
                                icdIds.Add(noiTruPhieuDieuTri.ChanDoanChinhICDId.Value);
                            }
                            foreach (var chanDoanKemTheo in noiTruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                            {
                                icdIds.Add(chanDoanKemTheo.ICDId);
                            }
                        }
                    }

                }
                if (yctn.YeuCauNhapVien != null && yctn.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                {
                    icdIds.Add(yctn.YeuCauNhapVien.ChanDoanNhapVienICDId.Value);
                }
            }

            icdIds = icdIds.Distinct().ToList();
            var icdData = _icdRepository.TableNoTracking.Where(o => icdIds.Contains(o.Id)).Select(o => new { o.Id, o.Ma, o.TenTiengViet }).ToList();

            var thongTinBenhNhanGoiBHYTs = new List<ThongTinBenhNhan>();
            foreach (var yeuCauTiepNhanId in danhSachYeuCauTiepNhanIds.Id)
            {
                var yctnNgoaiTru = yctnNgoaiTruVaNoiTrus.FirstOrDefault(o => o.Id == yeuCauTiepNhanId);
                var yctnNoiTru = yctnNgoaiTruVaNoiTrus.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == yeuCauTiepNhanId);

                string errorMessage = null;
                //KiemYeuCauTiepNhanGuiCongBHYT
                if (yctnNoiTru != null)
                {
                    var noiTruBenhAn = noiTruBenhAns.FirstOrDefault(o => o.Id == yctnNoiTru.Id);
                    if (noiTruBenhAn == null)
                        errorMessage = "Bệnh nhân chưa tạo bệnh án nội trú";
                    if (noiTruBenhAn.ThoiDiemRaVien == null)
                        errorMessage = "Bệnh nhân chưa kết thúc bệnh án nội trú";
                }
                if (yctnNgoaiTru == null)
                    errorMessage = "Yêu cầu tiếp nhận không tồn tại";
                if (yctnNgoaiTru.NamSinh == null)
                    errorMessage = "Không có năm sinh";
                if (yctnNgoaiTru.BHYTMucHuong == null)
                    errorMessage = "Không mức hưởng bảo hiểm y tế";

                if (errorMessage != null)
                {
                    thongTinBenhNhanGoiBHYTs.Add(new ThongTinBenhNhan
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhanId,
                        IsError = true,
                        ErrorMessage = errorMessage
                    });
                    continue;
                }

                var thangSinh = yctnNgoaiTru.ThangSinh == 0 ? 1 : yctnNgoaiTru.ThangSinh.Value;
                var ngaySinh = yctnNgoaiTru.NgaySinh == 0 ? 1 : yctnNgoaiTru.NgaySinh.Value;
                var ngaySinhBN = new DateTime(yctnNgoaiTru.NamSinh.Value, thangSinh, ngaySinh);


                var maBenh = string.Empty;
                //var tenBenhChinh = string.Empty;
                var maBenhKhac = string.Empty;
                //var tenBenhKhac = string.Empty;
                var tenBenh = string.Empty;

                var ngayRa = yctnNgoaiTru.ThoiDiemTiepNhan;
                var soNgayDieuTri = 1;
                var ketQuaDieuTri = Enums.EnumKetQuaDieuTri.Khoi;
                var tinhTrangRaVien = Enums.EnumTinhTrangRaVien.RaVien;
                var ngayThanhToan = yctnNgoaiTru.ThoiDiemTiepNhan;
                var maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.KhamBenh;
                var maKhoa = string.Empty;
                var tuoiBN = CalculateHelper.TinhTuoi(ngaySinhBN.Day, ngaySinhBN.Month, ngaySinhBN.Year);
                var canNang = yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                var maNoiChuyen = string.Empty;

                var yeuCauKhamBenhCoBHYTs = allYeuCauKhamBenhs
                    .Where(o => o.YeuCauTiepNhanId == yctnNgoaiTru.Id && o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList();
                var yeuCauKhamBenhChinh = yeuCauKhamBenhCoBHYTs.OrderByDescending(o => o.TiLeBaoHiemThanhToan).FirstOrDefault();

                if (yeuCauKhamBenhChinh != null && yctnNoiTru == null)
                {
                    var dsTenBenh = new List<string>();
                    var dsMaBenhKhac = new List<string>();
                    if (yeuCauKhamBenhChinh.IcdchinhId != null)
                    {
                        maBenh = icdData.FirstOrDefault(o => o.Id == yeuCauKhamBenhChinh.IcdchinhId)?.Ma;
                        dsTenBenh.Add(icdData.FirstOrDefault(o => o.Id == yeuCauKhamBenhChinh.IcdchinhId)?.TenTiengViet);
                    }
                    if (yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Any())
                    {
                        dsMaBenhKhac.AddRange(yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));
                        dsTenBenh.AddRange(yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.TenTiengViet));
                    }

                    foreach (var ycKham in yeuCauKhamBenhCoBHYTs)
                    {
                        if (ycKham.Id == yeuCauKhamBenhChinh.Id)
                            continue;
                        if (ycKham.IcdchinhId != null)
                        {
                            dsMaBenhKhac.Add(icdData.FirstOrDefault(o => o.Id == ycKham.IcdchinhId)?.Ma);
                            dsTenBenh.Add(icdData.FirstOrDefault(o => o.Id == ycKham.IcdchinhId)?.TenTiengViet);
                        }
                        if (ycKham.YeuCauKhamBenhICDKhacs.Any())
                        {
                            dsMaBenhKhac.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));
                            dsTenBenh.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.TenTiengViet));
                        }
                    }
                    maBenhKhac = string.Join(';', dsMaBenhKhac.Where(o => o != maBenh).Distinct());
                    tenBenh = string.Join(';', dsTenBenh.Distinct());

                    var yeuCauKhamBenhCuoi = yeuCauKhamBenhCoBHYTs.Where(o => o.ThoiDiemHoanThanh != null).OrderBy(o => o.ThoiDiemHoanThanh).LastOrDefault();

                    ngayRa = yeuCauKhamBenhCuoi?.ThoiDiemHoanThanh ?? yeuCauKhamBenhChinh.ThoiDiemChiDinh;

                    soNgayDieuTri = (int)(ngayRa - yctnNgoaiTru.ThoiDiemTiepNhan).TotalDays + 1;
                    ketQuaDieuTri = yeuCauKhamBenhChinh.KetQuaDieuTri ?? Enums.EnumKetQuaDieuTri.Khoi;
                    tinhTrangRaVien = yeuCauKhamBenhChinh.TinhTrangRaVien ?? Enums.EnumTinhTrangRaVien.RaVien;
                    ngayThanhToan = ngayRa;
                    maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.KhamBenh;
                    maKhoa = GetMaChuyenKhoa(yeuCauKhamBenhChinh.NoiThucHienKhoaPhongId, khoaPhongChuyenKhoas);// yeuCauKhamBenhChinh.NoiThucHien?.KhoaPhong.Ma,
                    canNang = yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                    maNoiChuyen = yeuCauKhamBenhChinh.BenhVienChuyenVienMa;
                    if (!string.IsNullOrEmpty(maNoiChuyen))
                    {
                        tinhTrangRaVien = EnumTinhTrangRaVien.ChuyenVien;
                    }
                }

                if (yctnNoiTru != null)
                {
                    var dsTenBenh = new List<string>();
                    var dsMaBenhKhac = new List<string>();
                    var noiTruBenhAn = noiTruBenhAns.First(o => o.Id == yctnNoiTru.Id);

                    if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
                    {
                        if (noiTruBenhAn.NoiTruPhieuDieuTris.Count() > 0)
                        {
                            var phieuDieuTri = noiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).First();

                            if (phieuDieuTri.ChanDoanChinhICDId != null)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == phieuDieuTri.ChanDoanChinhICDId);
                                if (icd != null)
                                {
                                    maBenh = icd.Ma;
                                    dsTenBenh.Add(icd.TenTiengViet);
                                }
                            }
                            foreach (var chanDoanKemTheo in phieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == chanDoanKemTheo.ICDId);
                                if (icd != null)
                                {
                                    if (!string.IsNullOrEmpty(icd.Ma))
                                    {
                                        dsMaBenhKhac.Add(icd.Ma);
                                    }
                                    if (!string.IsNullOrEmpty(icd.TenTiengViet))
                                    {
                                        dsTenBenh.Add(icd.TenTiengViet);
                                    }
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(maBenh))
                    {

                        if (noiTruBenhAn.ChanDoanChinhRaVienICDId != null)
                        {
                            var icdChanDoanRaVien = icdData.FirstOrDefault(o => o.Id == noiTruBenhAn.ChanDoanChinhRaVienICDId);
                            maBenh = icdChanDoanRaVien?.Ma;
                            dsTenBenh.Add(icdChanDoanRaVien?.TenTiengViet);
                        }
                        else if (yctnNoiTru.YeuCauNhapVien != null && yctnNoiTru.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                        {
                            var icdChanDoanNhapVien = icdData.FirstOrDefault(o => o.Id == yctnNoiTru.YeuCauNhapVien.ChanDoanNhapVienICDId);
                            if (icdChanDoanNhapVien != null)
                            {
                                maBenh = icdChanDoanNhapVien.Ma;
                                dsTenBenh.Add(icdChanDoanNhapVien.TenTiengViet);
                            }
                        }

                        if (!string.IsNullOrEmpty(noiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId))
                        {
                            var icdKemTheoIds = noiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                            for (int i = 0; i < icdKemTheoIds.Count(); i++)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == long.Parse(icdKemTheoIds[i]));
                                if (icd != null)
                                {
                                    if (!string.IsNullOrEmpty(icd.Ma))
                                    {
                                        dsMaBenhKhac.Add(icd.Ma);
                                    }
                                    if (!string.IsNullOrEmpty(icd.TenTiengViet))
                                    {
                                        dsTenBenh.Add(icd.TenTiengViet);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var ycKham in yeuCauKhamBenhCoBHYTs)
                    {
                        if (ycKham.IcdchinhId != null)
                        {
                            var icd = icdData.FirstOrDefault(o => o.Id == ycKham.IcdchinhId);
                            dsMaBenhKhac.Add(icd?.Ma);
                            dsTenBenh.Add(icd?.TenTiengViet);
                        }
                        if (ycKham.YeuCauKhamBenhICDKhacs.Any())
                        {
                            dsMaBenhKhac.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));
                            dsTenBenh.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.TenTiengViet));
                        }
                    }

                    maBenhKhac = string.Join(';', dsMaBenhKhac.Where(o => o != maBenh).Distinct());
                    tenBenh = string.Join(';', dsTenBenh.Distinct());

                    ngayRa = noiTruBenhAn.ThoiDiemRaVien.Value;
                    soNgayDieuTri = (int)(noiTruBenhAn.ThoiDiemRaVien.Value.Date - noiTruBenhAn.ThoiDiemNhapVien.Date).TotalDays + 1;
                    ketQuaDieuTri = noiTruBenhAn.KetQuaDieuTri ?? Enums.EnumKetQuaDieuTri.Khoi;
                    tinhTrangRaVien = noiTruBenhAn.TinhTrangRaVien ?? Enums.EnumTinhTrangRaVien.RaVien;
                    ngayThanhToan = noiTruBenhAn.ThoiDiemRaVien.Value;
                    maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.DieuTriNoiTru;
                    var khoaPhongDieuTriCuoiId = noiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(o => o.ThoiDiemVaoKhoa).LastOrDefault()?.KhoaPhongChuyenDenId;
                    maKhoa = GetMaChuyenKhoa(khoaPhongDieuTriCuoiId, khoaPhongChuyenKhoas);
                    canNang = yctnNoiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                    if (noiTruBenhAn.TinhTrangRaVien == EnumTinhTrangRaVien.ChuyenVien && noiTruBenhAn.ThongTinRaVien != null)
                    {
                        var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(noiTruBenhAn.ThongTinRaVien);
                        if (thongTinRaVien.BenhVienId != null)
                        {
                            var benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(o => o.Id == thongTinRaVien.BenhVienId);
                            if (benhVien != null)
                            {
                                maNoiChuyen = benhVien.Ma;
                            }
                        }
                    }
                }

                //=============================================================================== HỒ SƠ THÔNG TIN NGƯỜI BỆNH XML1 ==============================================================================================================
                var thongTinGoiBaoHiemYte = new ThongTinBenhNhan()
                {
                    STT = 1,
                    MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                    MaBenhNhan = yctnNgoaiTru.BenhNhan.MaBN,
                    HoTen = yctnNgoaiTru.HoTen,
                    NgaySinh = ngaySinhBN,
                    GioiTinh = yctnNgoaiTru.GioiTinh ?? Enums.LoaiGioiTinh.ChuaXacDinh,
                    DiaChi = yctnNgoaiTru.BHYTDiaChi,
                    MaThe = yctnNgoaiTru.BHYTMaSoThe,
                    MaCoSoKCBBanDau = yctnNgoaiTru.BHYTMaDKBD,
                    GiaTriTheTu = yctnNgoaiTru.BHYTNgayHieuLuc ?? yctnNgoaiTru.ThoiDiemTiepNhan,
                    GiaTriTheDen = yctnNgoaiTru.BHYTNgayHetHan,
                    MienCungChiTra = yctnNgoaiTru.BHYTNgayDuocMienCungChiTra,
                    TenBenh = tenBenh,
                    MaBenh = maBenh,
                    MaBenhKhac = maBenhKhac,
                    LyDoVaoVien = yctnNgoaiTru.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen,
                    MaNoiChuyen = maNoiChuyen,
                    MaTaiNan = null,
                    NgayVao = yctnNgoaiTru.ThoiDiemTiepNhan,
                    NgayRa = ngayRa,
                    SoNgayDieuTri = soNgayDieuTri,
                    KetQuaDieuTri = ketQuaDieuTri,
                    TinhTrangRaVien = tinhTrangRaVien,
                    NgayThanhToan = ngayThanhToan,
                    NamQuyetToan = DateTime.Now.Year,
                    ThangQuyetToan = DateTime.Now.Month,
                    MaLoaiKCB = maLoaiKCB,
                    MaKhoa = maKhoa,
                    MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan,
                    MaKhuVuc = yctnNgoaiTru.BHYTMaKhuVuc,
                    MaPhauThuatQuocTe = string.Empty,
                    CanNang = tuoiBN < 1 ? canNang : null,//yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang,
                    MucHuong = yctnNgoaiTru.BHYTMucHuong,
                };

                //============================================================================== HỒ SƠ THUỐC XML2 ===========================================================================================================================
                var maPhuongThucThanhToanXML2 = GetMaPhuongThucThanhToan(cauHinhBaoHiemYTe.MaPhuongThucThanhToanXML2);

                //var yeuCauDuocPhamBenhViens = yctnNgoaiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauDuocPhamBenhViens.AddRange(yctnNoiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                //}
                var yeuCauDuocPhamBenhViens = allYeuCauDuocPhamBenhViens.Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && o.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();



                foreach (var yeuCauDuocPham in yeuCauDuocPhamBenhViens.OrderBy(o => o.Id))
                {
                    var maBenhYeuCauDuocPham = "";
                    var ngayYLenh = yeuCauDuocPham.CreatedOn.Value;

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    if (yeuCauDuocPham.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = noiTruBenhAns.FirstOrDefault(o => o.Id == yctnNoiTru.Id)?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCauDuocPham.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                            }
                        }
                    }

                    if (yeuCauDuocPham.YeuCauKhamBenhId != null)
                    {
                        var yeuCauKhamBenh = allYeuCauKhamBenhs.FirstOrDefault(o => o.Id == yeuCauDuocPham.YeuCauKhamBenhId);
                        if (yeuCauKhamBenh != null && yeuCauKhamBenh.IcdchinhId != null)
                        {
                            maBenhYeuCauDuocPham = icdData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.IcdchinhId)?.Ma;
                            var maBenhKhacYeuCauDuocPham = string.Join(';', yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));

                            if (!string.IsNullOrEmpty(maBenhKhacYeuCauDuocPham))
                            {
                                maBenhYeuCauDuocPham = maBenhYeuCauDuocPham + ";" + maBenhKhacYeuCauDuocPham;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhYeuCauDuocPham))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhYeuCauDuocPham = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhYeuCauDuocPham = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    var lieuDung = yeuCauDuocPham.GhiChu;
                    if (yeuCauDuocPham.NoiTruChiDinhDuocPhamId != null)
                    {
                        var noiTruChiDinhDuocPham = allNoiTruChiDinhDuocPhams.FirstOrDefault(o => o.Id == yeuCauDuocPham.NoiTruChiDinhDuocPhamId);
                        lieuDung = GetLieuDungBHYT(noiTruChiDinhDuocPham?.SoLanTrenVien, noiTruChiDinhDuocPham?.LieuDungTrenNgay, yeuCauDuocPham.GhiChu);
                    }

                    var xuatKhoDuocPhamChiTietViTris = allXuatKhoDuocPhamChiTietViTris.Where(o => o.XuatKhoDuocPhamChiTietId == yeuCauDuocPham.XuatKhoDuocPhamChiTietId).ToList();
                    var groupXuatKhoTheoNhaThau = xuatKhoDuocPhamChiTietViTris.GroupBy(o => o.HopDongThauDuocPhamId);
                    //var groupXuatKhoTheoNhaThau = yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.GroupBy(o => o.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId);
                    double soLuongDaGhi = 0;
                    foreach (var xuatKhoTheoNhaThau in groupXuatKhoTheoNhaThau)
                    {
                        var hopDongThau = allHopDongThauDuocPhams.FirstOrDefault(o=>o.Id == xuatKhoTheoNhaThau.Key);
                        double soLuongTheoNhaThau = xuatKhoTheoNhaThau.Sum(o => o.SoLuongXuat).MathRoundNumber(2);
                        var soLuongGhi = (soLuongTheoNhaThau + soLuongDaGhi) <= yeuCauDuocPham.SoLuong ? soLuongTheoNhaThau : (yeuCauDuocPham.SoLuong - soLuongDaGhi).MathRoundNumber(2);

                        if (soLuongGhi > 0)
                        {
                            soLuongDaGhi += soLuongGhi;
                            var thongTinThuoc = new HoSoChiTietThuoc
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                MaThuoc = yeuCauDuocPham.MaHoatChat,
                                MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
                                TenThuoc = yeuCauDuocPham.Ten,
                                DonViTinh = yeuCauDuocPham.TenDonViTinh,
                                HamLuong = yeuCauDuocPham.HamLuong,
                                DuongDung = yeuCauDuocPham.MaDuongDung,
                                LieuDung = lieuDung,
                                SoDangKy = yeuCauDuocPham.SoDangKy,
                                ThongTinThau = hopDongThau?.SoQuyetDinh + ";" + hopDongThau?.GoiThau + ";" + hopDongThau?.NhomThau,
                                PhamVi = 1,
                                SoLuong = soLuongGhi,
                                DonGia = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault(),
                                TyLeThanhToan = yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault(),
                                TienNguonKhac = 0,
                                TienBenhNhanTuTra = 0,
                                TienNgoaiDinhSuat = 0,
                                MaKhoa = GetMaChuyenKhoa(yeuCauDuocPham.KhoaChiDinhId, khoaPhongChuyenKhoas),// yeuCauDuocPham.NoiChiDinh.KhoaPhong.Ma,
                                MaBacSi = yeuCauDuocPham.MaChungChiHanhNghe,
                                MaBenh = maBenhYeuCauDuocPham,
                                NgayYLenh = ngayYLenh,
                                MaPhuongThucThanhToan = maPhuongThucThanhToanXML2
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietThuoc.Add(thongTinThuoc);
                        }

                    }
                }

                //var donThuocThanhToanChiTiets = yctnNgoaiTru.DonThuocThanhToans.Where(o => o.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                //                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    donThuocThanhToanChiTiets.AddRange(yctnNoiTru.DonThuocThanhToans.Where(o => o.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                //                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true));
                //}
                var donThuocThanhToanChiTiets = allDonThuocThanhToanChiTiets.Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && o.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc && o.BaoHiemChiTra == true).ToList();

                foreach (var donThuocChiTiet in donThuocThanhToanChiTiets.OrderBy(o => o.Id))
                {
                    var yeuCauKhamBenh = allYeuCauKhamBenhs.FirstOrDefault(o => o.Id == donThuocChiTiet.YeuCauKhamBenhId);

                    var khoaPhongKeDonId = yeuCauKhamBenh?.NoiThucHienKhoaPhongId;
                    var maKhoaKeDon = maKhoa;
                    if (khoaPhongKeDonId != null)
                    {
                        maKhoaKeDon = GetMaChuyenKhoa(khoaPhongKeDonId, khoaPhongChuyenKhoas);
                    }
                    var maBacSiKeDon = yeuCauKhamBenh?.MaChungChiBacSiThucHien;

                    var maBenhTheoDon = "";

                    if (yeuCauKhamBenh != null && yeuCauKhamBenh.IcdchinhId != null)
                    {
                        maBenhTheoDon = icdData.FirstOrDefault(o => o.Id == yeuCauKhamBenh.IcdchinhId)?.Ma;
                        var maBenhKhacYeuCauDuocPham = string.Join(';', yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));

                        if (!string.IsNullOrEmpty(maBenhKhacYeuCauDuocPham))
                        {
                            maBenhTheoDon = maBenhTheoDon + ";" + maBenhKhacYeuCauDuocPham;
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhTheoDon))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoDon = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoDon = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    string lieuDung = GetLieuDungBHYT(donThuocChiTiet.SoLanTrenVien, donThuocChiTiet.LieuDungTrenNgay, donThuocChiTiet.GhiChu);
                    //if (donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet != null)
                    //{
                    //    lieuDung = GetLieuDungBHYT(donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.SoLanTrenVien, donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.LieuDungTrenNgay, donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.GhiChu);
                    //}
                    //else if (donThuocChiTiet.NoiTruDonThuocChiTiet != null)
                    //{
                    //    lieuDung = GetLieuDungBHYT(null, null, donThuocChiTiet.NoiTruDonThuocChiTiet.GhiChu);
                    //}
                    //else
                    //{
                    //    lieuDung = GetLieuDungBHYT(null, null, donThuocChiTiet.GhiChu);
                    //}

                    //MaKhoa = GetMaChuyenKhoa(yeuCauKhamBenh.NoiThucHien?.KhoaPhongId, khoaPhongChuyenKhoas),// yeuCauKhamBenh.NoiThucHien?.KhoaPhong.Ma,
                    //MaBacSi = yeuCauKhamBenh.BacSiThucHien?.MaChungChiHanhNghe,
                    //MaBenh = yeuCauKhamBenh.Icdchinh?.Ma,
                    var thongTinThuoc = new HoSoChiTietThuoc
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaThuoc = donThuocChiTiet.MaHoatChat,
                        MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
                        TenThuoc = donThuocChiTiet.Ten,
                        DonViTinh = donThuocChiTiet.TenDonViTinh,
                        HamLuong = donThuocChiTiet.HamLuong,
                        DuongDung = donThuocChiTiet.MaDuongDung,
                        LieuDung = lieuDung,
                        SoDangKy = donThuocChiTiet.SoDangKy,
                        ThongTinThau = donThuocChiTiet.SoQuyetDinhThau + ";" + donThuocChiTiet.GoiThau + ";" + donThuocChiTiet.NhomThau,
                        PhamVi = 1,
                        SoLuong = donThuocChiTiet.SoLuong,
                        DonGia = donThuocChiTiet.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = donThuocChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = donThuocChiTiet.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = maKhoaKeDon,
                        MaBacSi = maBacSiKeDon,
                        MaBenh = maBenhTheoDon,
                        NgayYLenh = donThuocChiTiet.CreatedOn.Value,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML2
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietThuoc.Add(thongTinThuoc);
                }

                //============================================================================== DỊCH VỤ KỸ THUẬT & VẬT TƯ (XML3) ====================================================================================================================
                var maPhuongThucThanhToanXML3 = GetMaPhuongThucThanhToan(cauHinhBaoHiemYTe.MaPhuongThucThanhToanXML3);

                //var yeuCauVatTuBenhViens = yctnNgoaiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauVatTuBenhViens.AddRange(yctnNoiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                //}
                var yeuCauVatTuBenhViens = allYeuCauVatTuBenhViens.Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && o.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();

                foreach (var yeuCauVatTu in yeuCauVatTuBenhViens.OrderBy(o => o.Id))
                {
                    var maDichVuKyThuat = allDichVuKyThuatBenhVienAnhXas.FirstOrDefault(o=>o.Id == yeuCauVatTu.DichVuKyThuatBenhVienId)?.MaChung;
                    var maBenhTheoVatTu = "";

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    var ngayYLenh = yeuCauVatTu.CreatedOn.Value;
                    var ngayKetQua = yeuCauVatTu.ThoiDiemChiDinh;
                    if (yeuCauVatTu.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = noiTruBenhAns.FirstOrDefault(o => o.Id == yctnNoiTru.Id)?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCauVatTu.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                                ngayKetQua = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                    {
                        maBenhTheoVatTu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                    }
                    else
                    {
                        maBenhTheoVatTu = thongTinGoiBaoHiemYte.MaBenh;
                    }

                    var xuatKhoVatTuChiTietViTris = allXuatKhoVatTuChiTietViTris.Where(o => o.XuatKhoVatTuChiTietId == yeuCauVatTu.XuatKhoVatTuChiTietId).ToList();
                    var groupXuatKhoTheoNhaThau = xuatKhoVatTuChiTietViTris.GroupBy(o => o.HopDongThauVatTuId);
                                        
                    double soLuongDaGhi = 0;
                    foreach (var xuatKhoTheoNhaThau in groupXuatKhoTheoNhaThau)
                    {
                        var hopDongThau = allHopDongThauVatTus.FirstOrDefault(o=>o.Id == xuatKhoTheoNhaThau.Key);

                        double soLuongTheoNhaThau = xuatKhoTheoNhaThau.Sum(o => o.SoLuongXuat).MathRoundNumber(2);
                        var soLuongGhi = (soLuongTheoNhaThau + soLuongDaGhi) <= yeuCauVatTu.SoLuong ? soLuongTheoNhaThau : (yeuCauVatTu.SoLuong - soLuongDaGhi).MathRoundNumber(2);
                        if (soLuongGhi > 0)
                        {
                            soLuongDaGhi += soLuongGhi;
                            var thongTinVatTu = new HoSoChiTietDVKT
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                MaDichVu = maDichVuKyThuat,
                                MaVatTu = yeuCauVatTu.Ma,
                                MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT,
                                TenVatTu = yeuCauVatTu.Ten,
                                DonViTinh = yeuCauVatTu.DonViTinh,
                                PhamVi = 1,
                                SoLuong = soLuongGhi,
                                DonGia = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault(),
                                ThongTinThau = hopDongThau?.SoQuyetDinh + ";" + hopDongThau?.GoiThau + ";" + hopDongThau?.NhomThau,
                                TyLeThanhToan = yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault(),
                                TienNguonKhac = 0,
                                TienBenhNhanTuTra = 0,
                                TienNgoaiDinhSuat = 0,
                                MaKhoa = GetMaChuyenKhoa(yeuCauVatTu.KhoaChiDinhId, khoaPhongChuyenKhoas),//yeuCauVatTu.NoiChiDinh.KhoaPhong.Ma,
                                MaBacSi = yeuCauVatTu.MaChungChiHanhNghe,
                                MaBenh = maBenhTheoVatTu,
                                NgayYLenh = ngayYLenh,
                                NgayKetQua = ngayKetQua,
                                MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinVatTu);
                        }
                    }
                }

                //var yeuCauDichVuKyThuats = yctnNgoaiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauDichVuKyThuats.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true));
                //}

                var yeuCauDichVuKyThuats = allYeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id))).ToList();

                foreach (var yeuCau in yeuCauDichVuKyThuats.OrderBy(o => o.Id))
                {
                    var maBenhTheoDichVu = "";
                    if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                    {
                        maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                    }
                    else
                    {
                        maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh;
                    }

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    var ngayYLenh = yeuCau.ThoiDiemThucHien ?? yeuCau.ThoiDiemDangKy;
                    var ngayKetQua = yeuCau.ThoiDiemKetLuan ?? yeuCau.ThoiDiemHoanThanh;
                    if (yeuCau.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = noiTruBenhAns.FirstOrDefault(o => o.Id == yctnNoiTru.Id)?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCau.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                                if (ngayKetQua < ngayYLenh)
                                {
                                    ngayKetQua = ngayYLenh;
                                }
                            }
                        }
                    }
                    var dichVuKyThuatBenhVien = allDichVuKyThuatBenhVienAnhXas.FirstOrDefault(o => o.Id == yeuCau.DichVuKyThuatBenhVienId);
                    var maDichVu = dichVuKyThuatBenhVien?.MaChung ?? yeuCau.MaDichVu;
                    var tenDichVu = dichVuKyThuatBenhVien?.TenChung ?? yeuCau.TenDichVu;
                    var thongTinKyThuat = new HoSoChiTietDVKT
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        MaNhom = yeuCau.NhomChiPhi,
                        TenDichVu = tenDichVu,
                        DonViTinh = "Lần",
                        PhamVi = 1,
                        SoLuong = yeuCau.SoLan,
                        DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = GetMaChuyenKhoa(yeuCau.NoiThucHienKhoaPhongId, khoaPhongChuyenKhoas),//yeuCau.NoiThucHien?.KhoaPhong.Ma ?? yeuCau.NoiChiDinh.KhoaPhong.Ma,
                        MaBacSi = yeuCau.MaChungChiNhanVienThucHien,
                        MaBenh = maBenhTheoDichVu,
                        NgayYLenh = ngayYLenh,
                        NgayKetQua = ngayKetQua,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                }

                //var yeuCauKhamBenhs = yctnNgoaiTru.YeuCauKhamBenhs.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauKhamBenhs.AddRange(yctnNoiTru.YeuCauKhamBenhs.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true));
                //}
                var yeuCauKhamBenhs = allYeuCauKhamBenhs.Where(o => o.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true && (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id))).ToList();
                foreach (var yeuCau in yeuCauKhamBenhs.OrderBy(o => o.Id))
                {
                    var maBenhTheoKhamBenh = "";
                    if (yeuCau.IcdchinhId != null)
                    {
                        maBenhTheoKhamBenh = icdData.FirstOrDefault(o => o.Id == yeuCau.IcdchinhId)?.Ma;
                        var maBenhKhacYeuCauKhamBenh = string.Join(';', yeuCau.YeuCauKhamBenhICDKhacs.Select(o => icdData.FirstOrDefault(icd => icd.Id == o.ICDId)?.Ma));

                        if (!string.IsNullOrEmpty(maBenhKhacYeuCauKhamBenh))
                        {
                            maBenhTheoKhamBenh = maBenhTheoKhamBenh + ";" + maBenhKhacYeuCauKhamBenh;
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhTheoKhamBenh))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoKhamBenh = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoKhamBenh = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    var dichVuKhamBenhBenhVien = allDichVuKhamBenhBenhVienAnhXas.FirstOrDefault(o => o.Id == yeuCau.DichVuKhamBenhBenhVienId);
                    var maDichVu = dichVuKhamBenhBenhVien?.MaChung ?? yeuCau.MaDichVu;
                    var tenDichVu = dichVuKhamBenhBenhVien?.TenChung ?? yeuCau.TenDichVu;
                    var thongTinKyThuat = new HoSoChiTietDVKT
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.KhamBenh,
                        TenDichVu = tenDichVu,
                        DonViTinh = "Lần",
                        PhamVi = 1,
                        SoLuong = 1,
                        DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = GetMaChuyenKhoa(yeuCau.NoiThucHienKhoaPhongId, khoaPhongChuyenKhoas),//yeuCau.NoiThucHien?.KhoaPhong.Ma ?? yeuCau.NoiChiDinh.KhoaPhong.Ma,
                        MaBacSi = yeuCau.MaChungChiBacSiThucHien,
                        MaBenh = maBenhTheoKhamBenh,
                        //NgayYLenh = yeuCau.ThoiDiemThucHien ?? yeuCau.ThoiDiemDangKy,
                        NgayYLenh = yeuCau.ThoiDiemDangKy,
                        NgayKetQua = yeuCau.ThoiDiemHoanThanh,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                }

                if (yctnNoiTru != null)
                {
                    var noiTruBenhAn = noiTruBenhAns.First(o => o.Id == yctnNoiTru.Id);
                    var yeuCauDichVuGiuongTheoNgays = allYeuCauDichVuGiuongBenhVienChiPhiBHYT.Where(o => o.BaoHiemChiTra == true);

                    foreach (var yeuCau in yeuCauDichVuGiuongTheoNgays.OrderBy(o => o.NgayPhatSinh))
                    {
                        var maBenhTheoDichVu = "";
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh;
                        }
                        var ekipDieuTri = noiTruBenhAn.NoiTruEkipDieuTris.Where(o => o.TuNgay.Date <= yeuCau.NgayPhatSinh && (o.DenNgay == null || yeuCau.NgayPhatSinh <= o.DenNgay)).FirstOrDefault();
                        if (ekipDieuTri == null)
                        {
                            ekipDieuTri = noiTruBenhAn.NoiTruEkipDieuTris.FirstOrDefault();
                        }
                        var maBacSiDieuTri = ekipDieuTri?.MaChungChiHanhNghe;
                        var ngayYLenh = yeuCau.NgayPhatSinh < noiTruBenhAn.ThoiDiemNhapVien ? noiTruBenhAn.ThoiDiemNhapVien : yeuCau.NgayPhatSinh;
                        DateTime? ngayKetQua = null;
                        if (yeuCau.NgayPhatSinh.Date == noiTruBenhAn.ThoiDiemRaVien.Value.Date)
                        {
                            ngayKetQua = noiTruBenhAn.ThoiDiemRaVien;
                        }
                        var thongTinKyThuat = new HoSoChiTietDVKT
                        {
                            MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                            STT = 1,
                            MaDichVu = yeuCau.MaChung,
                            MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru,
                            TenDichVu = yeuCau.TenChung,
                            DonViTinh = "",
                            PhamVi = 1,
                            SoLuong = yeuCau.SoLuong,
                            DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                            TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                            TienNguonKhac = 0,
                            TienBenhNhanTuTra = 0,
                            TienNgoaiDinhSuat = 0,
                            MaKhoa = GetMaChuyenKhoa(yeuCau.KhoaPhongId, khoaPhongChuyenKhoas),
                            MaGiuong = yeuCau.MaGiuong,
                            MaBacSi = maBacSiDieuTri,
                            MaBenh = maBenhTheoDichVu,
                            NgayYLenh = ngayYLenh,
                            NgayKetQua = ngayKetQua,
                            MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                        };
                        thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                    }
                }

                //============================================================================== DỊCH VỤ KỸ THUẬT CẬN LÂM S XML4 ==========================================================================================================================================================================================================================================================================================================
                //var yeuCauDichVuKyThuatCanLamSangs = yctnNgoaiTru.YeuCauDichVuKyThuats
                //    .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && o.ThoiDiemHoanThanh != null &&
                //                (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauDichVuKyThuatCanLamSangs.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && o.ThoiDiemHoanThanh != null &&
                //                (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)));
                //}

                var yeuCauDichVuKyThuatCanLamSangs = allYeuCauDichVuKyThuats
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && 
                                o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && o.ThoiDiemHoanThanh != null &&
                                (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                    .ToList();

                foreach (var yeuCau in yeuCauDichVuKyThuatCanLamSangs.OrderBy(o => o.Id))
                {
                    var ketLuan = string.Empty;
                    var tenChiSo = string.Empty;
                    var giaTri = string.Empty;
                    var moTa = string.Empty;

                    var dichVuKyThuatBenhVien = allDichVuKyThuatBenhVienAnhXas.FirstOrDefault(o => o.Id == yeuCau.DichVuKyThuatBenhVienId);

                    if ((yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) && !string.IsNullOrEmpty(yeuCau.DataKetQuaCanLamSang))
                    {
                        var data = JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(yeuCau.DataKetQuaCanLamSang);
                        if (data != null)
                        {
                            ketLuan = MaskHelper.RemoveHtmlFromString(data.KetLuan);
                        }
                    }
                    else if (yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                    {
                        tenChiSo = dichVuKyThuatBenhVien.TenDichVuXetNghiem;
                        var phienXNChiTiet = phienXetNghiemChiTietData.Where(o => o.YeuCauDichVuKyThuatId == yeuCau.Id).OrderBy(o => o.LanThucHien).LastOrDefault();
                        if (phienXNChiTiet != null)
                        {
                            moTa = phienXNChiTiet.GhiChu ?? string.Empty;
                            ketLuan = phienXNChiTiet.KetLuan ?? string.Empty;
                            giaTri = phienXNChiTiet.KetQuaXetNghiemChiTiets.Where(o => o.DichVuXetNghiemChaId == null).FirstOrDefault()?.GiaTriNhapTay ?? string.Empty;
                        }
                    }
                    var maDichVu = dichVuKyThuatBenhVien?.MaChung ?? yeuCau.MaDichVu;
                    var thongTinCanLamSang = new HoSoCanLamSang
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        TenChiSo = tenChiSo,
                        MoTa = moTa,
                        KetLuan = ketLuan,
                        GiaTri = giaTri,
                        NgayKQ = yeuCau.ThoiDiemHoanThanh.Value,
                        YeuCauDichVuKyThuatId = yeuCau.Id
                    };
                    thongTinGoiBaoHiemYte.HoSoCanLamSang.Add(thongTinCanLamSang);
                }
                //Hồ sơ chi tiết diễn biến bệnh (XML5)
                if (yctnNoiTru != null)
                {
                    var noiTruBenhAn = noiTruBenhAns.First(o => o.Id == yctnNoiTru.Id);
                    var phieuDieuTriTheoNgays = noiTruBenhAn.NoiTruPhieuDieuTris.GroupBy(o => o.NgayDieuTri.Date);
                    foreach (var phieuDieuTriTheoNgay in phieuDieuTriTheoNgays.OrderBy(o => o.Key))
                    {


                        DateTime? ngayPTTT = null;
                        var ppPhauThuat = string.Empty;
                        var hoiChan = string.Empty;
                        var dienBienData = phieuDieuTriTheoNgay.OrderBy(o => o.Id).LastOrDefault()?.DienBien;
                        var dienBien = string.Empty;
                        if (!string.IsNullOrEmpty(dienBienData))
                        {
                            var dienBiens = JsonConvert.DeserializeObject<List<DienBienPhieuDieuTriGoiBaoHiemYTe>>(dienBienData);
                            if (dienBiens != null)
                            {
                                dienBien = string.Join(';', dienBiens.Select(o => o.DienBien));
                            }
                        }

                        var bienBanHoiChan = bienBanHoiChanData.Where(o => o.ThoiDiemThucHien.Date == phieuDieuTriTheoNgay.Key && o.YeuCauTiepNhanId == yctnNoiTru.Id).OrderBy(o => o.ThoiDiemThucHien).LastOrDefault();
                        if (bienBanHoiChan != null && !string.IsNullOrEmpty(bienBanHoiChan.ThongTinHoSo))
                        {
                            var bienBanhoiChanVo = JsonConvert.DeserializeObject<BienBanHoiChanVo>(bienBanHoiChan.ThongTinHoSo);
                            hoiChan = bienBanhoiChanVo?.KetLuan ?? string.Empty;
                        }

                        //var dvPTTTNgoaiTrus = yctnNgoaiTru.YeuCauDichVuKyThuats
                        //    .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true
                        //                && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh.Value.Date == phieuDieuTriTheoNgay.Key && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList();
                        //var dvPTTTNoiTrus = yctnNoiTru.YeuCauDichVuKyThuats
                        //    .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true
                        //                && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh.Value.Date == phieuDieuTriTheoNgay.Key && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList();

                        var dvPTTTs = allYeuCauDichVuKyThuats
                            .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) &&
                                        o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true
                                        && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh.Value.Date == phieuDieuTriTheoNgay.Key && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat)
                            .OrderBy(o => o.ThoiDiemHoanThanh).ToList();
                        var dvPTTTIds = dvPTTTs.Select(pt => pt.Id).ToList();
                        if (dvPTTTs.Any())
                        {
                            //var dvPTTTs = dvPTTTNgoaiTrus.Concat(dvPTTTNoiTrus).OrderBy(o => o.ThoiDiemHoanThanh).ToList();

                            ngayPTTT = dvPTTTs.First().ThoiDiemHoanThanh;
                            var phuongPhapPTTTs = yeuCauDichVuKyThuatTuongTrinhPTTTs
                                .Where(o => dvPTTTIds.Contains(o.Id) && !string.IsNullOrEmpty(o.TenPhuongPhapPTTT))
                                .Select(o => o.TenPhuongPhapPTTT).ToList();
                            if (phuongPhapPTTTs.Any())
                            {
                                ppPhauThuat = string.Join(';', phuongPhapPTTTs);
                            }
                        }
                        DateTime ngayThepNhapVien = new DateTime(phieuDieuTriTheoNgay.Key.Year, phieuDieuTriTheoNgay.Key.Month, phieuDieuTriTheoNgay.Key.Day, noiTruBenhAn.ThoiDiemNhapVien.Hour, noiTruBenhAn.ThoiDiemNhapVien.Minute, noiTruBenhAn.ThoiDiemNhapVien.Second);
                        var ngayYLenh = ngayPTTT != null ? ngayPTTT.Value.AddMinutes(2) : ngayThepNhapVien.AddMinutes(2);

                        if (noiTruBenhAn.ThoiDiemNhapVien <= ngayYLenh && ngayYLenh <= noiTruBenhAn.ThoiDiemRaVien)
                        {
                            var hoSoChiTietDienBienBenh = new HoSoChiTietDienBienBenh
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                DienBien = dienBien,
                                NgayYLenh = ngayYLenh,
                                PhauThuat = ppPhauThuat,
                                HoiChuan = hoiChan
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietDienBienBenh.Add(hoSoChiTietDienBienBenh);
                        }
                    }
                }

                thongTinGoiBaoHiemYte.YeuCauTiepNhanId = yeuCauTiepNhanId;

                //them canh bao dv chua thuc hien
                var yeuCauDuocPhamBenhVienChuaThucHiens = allYeuCauDuocPhamBenhViens
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) &&
                                o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true)
                    .ToList();

                var donThuocThanhToanChiTietChuaThucHiens = allDonThuocThanhToanChiTiets
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && 
                                o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc && o.BaoHiemChiTra == true)
                    .ToList();

                var yeuCauVatTuBenhVienChuaThucHiens = allYeuCauVatTuBenhViens
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && 
                                o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true)
                    .ToList();

                var yeuCauDichVuKyThuatChuaThucHiens = allYeuCauDichVuKyThuats
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && 
                                o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true)
                    .ToList();

                var yeuCauKhamBenhChuaThucHiens = allYeuCauKhamBenhs
                    .Where(o => (o.YeuCauTiepNhanId == yctnNgoaiTru.Id || (yctnNoiTru != null && o.YeuCauTiepNhanId == yctnNoiTru.Id)) && 
                                o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true)
                    .ToList();

                //var yeuCauDuocPhamBenhVienChuaThucHiens = yctnNgoaiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //var donThuocThanhToanChiTietChuaThucHiens = yctnNgoaiTru.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                //                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true).ToList();
                //var yeuCauVatTuBenhVienChuaThucHiens = yctnNgoaiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //var yeuCauDichVuKyThuatChuaThucHiens = yctnNgoaiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true).ToList();
                //var yeuCauKhamBenhChuaThucHiens = yctnNgoaiTru.YeuCauKhamBenhs.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true).ToList();
                //if (yctnNoiTru != null)
                //{
                //    yeuCauDuocPhamBenhVienChuaThucHiens.AddRange(yctnNoiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                //    donThuocThanhToanChiTietChuaThucHiens.AddRange(yctnNoiTru.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                //                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true));
                //    yeuCauVatTuBenhVienChuaThucHiens.AddRange(yctnNoiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                //    yeuCauDichVuKyThuatChuaThucHiens.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true));
                //    yeuCauKhamBenhChuaThucHiens.AddRange(yctnNoiTru.YeuCauKhamBenhs.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true));
                //}
                var dsCanhBao = new List<string>();
                if (yeuCauDuocPhamBenhVienChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Chỉ định dược phẩm chưa thực hiện: {string.Join(';', yeuCauDuocPhamBenhVienChuaThucHiens.Select(o => o.Ten))}");
                }
                if (donThuocThanhToanChiTietChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Đơn thuốc chưa xuất: {string.Join(';', donThuocThanhToanChiTietChuaThucHiens.Select(o => o.Ten))}");
                }
                if (yeuCauVatTuBenhVienChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Chỉ định vật tư chưa thực hiện: {string.Join(';', yeuCauVatTuBenhVienChuaThucHiens.Select(o => o.Ten))}");
                }
                if (yeuCauDichVuKyThuatChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Dịch vụ kỹ thuật chưa thực hiện: {string.Join(';', yeuCauDichVuKyThuatChuaThucHiens.Select(o => o.TenDichVu))}");
                }
                if (yeuCauKhamBenhChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Dịch vụ khám chưa thực hiện: {string.Join(';', yeuCauKhamBenhChuaThucHiens.Select(o => o.TenDichVu))}");
                }
                if (dsCanhBao.Any())
                {
                    thongTinGoiBaoHiemYte.WarningMessage = string.Join('|', dsCanhBao);
                }

                thongTinBenhNhanGoiBHYTs.Add(thongTinGoiBaoHiemYte);
            }

            return thongTinBenhNhanGoiBHYTs;
        }

        public List<ThongTinBenhNhan> GetThongTinBenhNhanCoBHYTOld(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanIds)
        {
            var khoaPhongChuyenKhoas = _khoaPhongChuyenKhoaRepository.TableNoTracking.Include(o => o.Khoa).ToList();
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var danhSachYeuCauTiepNhanNgoaiTruIds = danhSachYeuCauTiepNhanIds.Id.Where(o => o != 0);

            var yctnCoBHYTNgoaiTruVaNoiTrus = BaseRepository.Table
                .Where(o => o.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy && ((danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.Id) && o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru && o.DaGuiCongBHYT != true) ||
                            (danhSachYeuCauTiepNhanNgoaiTruIds.Contains(o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault()))))
                         .Include(cc => cc.PhuongXa)
                        .Include(cc => cc.QuanHuyen)
                        .Include(cc => cc.TinhThanh)
                        .Include(cc => cc.NoiChuyen)
                        .Include(cc => cc.BenhNhan)
                        .Include(cc => cc.BHYTGiayMienCungChiTra)
                        .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruPhieuDieuTris).ThenInclude(cc => cc.NoiTruThamKhamChanDoanKemTheos)
                        .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.ChanDoanChinhRaVienICD)
                        .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruKhoaPhongDieuTris)
                        .Include(cc => cc.NoiTruBenhAn).ThenInclude(cc => cc.NoiTruEkipDieuTris).ThenInclude(cc => cc.BacSi)
                        //.Include(cc => cc.YeuCauKhamBenhs)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.BenhVienChuyenVien)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.Icdchinh)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.NhomGiaDichVuKhamBenhBenhVien)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(cc => cc.DichVuKhamBenhBenhVien).ThenInclude(cc => cc.DichVuKhamBenh)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.NoiThucHien).ThenInclude(c => c.KhoaPhong)
                        //.Include(cc => cc.YeuCauKhamBenhs).ThenInclude(c => c.BacSiThucHien).ThenInclude(c => c.User)
                        //.Include(cc => cc.YeuCauDichVuKyThuats)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.NhomGiaDichVuKyThuatBenhVien)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.YeuCauVatTuBenhViens)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.NhanVienThucHien)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.NhanVienChiDinh)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(c => c.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuXetNghiem)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(c => c.Icdchinh)
                        //.Include(cc => cc.YeuCauDichVuKyThuats).ThenInclude(cc => cc.YeuCauDichVuKyThuatTuongTrinhPTTT)
                        //.Include(cc => cc.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
                        //.Include(cc => cc.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(cc => cc.DichVuGiuongBenhVien).ThenInclude(cc => cc.DichVuGiuong)
                        //.Include(cc => cc.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(cc => cc.GiuongBenh)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.DuocPhamBenhVien).ThenInclude(cc => cc.DuocPham)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.DuocPhamBenhVien)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.DonViTinh)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.DuongDung)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.NoiTruChiDinhDuocPham)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.NhanVienChiDinh)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.NoiChiDinh)
                        //.Include(cc => cc.YeuCauDuocPhamBenhViens).ThenInclude(cc => cc.XuatKhoDuocPhamChiTiet).ThenInclude(cc => cc.XuatKhoDuocPhamChiTietViTris).ThenInclude(cc => cc.NhapKhoDuocPhamChiTiet).ThenInclude(cc => cc.HopDongThauDuocPhams)
                        //.Include(cc => cc.YeuCauVatTuBenhViens)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.XuatKhoVatTuChiTiet).ThenInclude(cc => cc.XuatKhoVatTuChiTietViTris).ThenInclude(cc => cc.NhapKhoVatTuChiTiet).ThenInclude(cc => cc.HopDongThauVatTu)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.NhanVienChiDinh)
                        //.Include(cc => cc.YeuCauVatTuBenhViens).ThenInclude(cc => cc.NoiChiDinh)
                        //.Include(cc => cc.DonThuocThanhToans)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuocPham).ThenInclude(o => o.DuocPhamBenhVien)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuongDung)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonViTinh)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.YeuCauKhamBenhDonThuocChiTiet)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.NoiTruDonThuocChiTiet)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien).ThenInclude(c => c.User)
                        //.Include(cc => cc.DonThuocThanhToans).ThenInclude(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                        .Include(cc => cc.KetQuaSinhHieus)
                        .Include(cc => cc.YeuCauNhapVien)
                        .ToList();

            foreach (var yctn in yctnCoBHYTNgoaiTruVaNoiTrus)
            {
                //Explicit loading
                var yeuCauKhamBenhs = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauKhamBenhs);
                yeuCauKhamBenhs.Query()
                    .Include(c => c.BenhVienChuyenVien)
                    .Include(c => c.Icdchinh)
                    .Include(c => c.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                    .Include(c => c.NhomGiaDichVuKhamBenhBenhVien)
                    .Include(cc => cc.DichVuKhamBenhBenhVien).ThenInclude(cc => cc.DichVuKhamBenh)
                    .Include(c => c.NoiThucHien).ThenInclude(c => c.KhoaPhong)
                    .Include(c => c.BacSiThucHien).ThenInclude(c => c.User)
                    .Load();

                var yeuCauDichVuKyThuats = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDichVuKyThuats);
                yeuCauDichVuKyThuats.Query()
                    .Include(c => c.NhomGiaDichVuKyThuatBenhVien)
                    .Include(c => c.YeuCauVatTuBenhViens)
                    .Include(c => c.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Include(c => c.NoiChiDinh).ThenInclude(c => c.KhoaPhong)
                    .Include(c => c.NhanVienThucHien)
                    .Include(c => c.NhanVienChiDinh)
                    .Include(c => c.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuXetNghiem)
                    .Include(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(c => c.Icdchinh)
                    .Include(cc => cc.YeuCauDichVuKyThuatTuongTrinhPTTT)
                    .Load();

                var yeuCauDichVuGiuongBenhVienChiPhiBHYTs = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs);
                yeuCauDichVuGiuongBenhVienChiPhiBHYTs.Query()
                    .Include(cc => cc.DichVuGiuongBenhVien).ThenInclude(cc => cc.DichVuGiuong)
                    .Include(cc => cc.GiuongBenh)
                    .Load();

                var yeuCauDuocPhamBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauDuocPhamBenhViens);
                yeuCauDuocPhamBenhViens.Query()
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
                    .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Include(cc => cc.DuocPhamBenhVien).ThenInclude(cc => cc.DuocPham)
                    .Include(cc => cc.DuocPhamBenhVien)
                    .Include(cc => cc.DonViTinh)
                    .Include(cc => cc.DuongDung)
                    .Include(cc => cc.NoiTruChiDinhDuocPham)
                    .Include(cc => cc.NhanVienChiDinh)
                    .Include(cc => cc.NoiChiDinh)
                    .Include(cc => cc.XuatKhoDuocPhamChiTiet).ThenInclude(cc => cc.XuatKhoDuocPhamChiTietViTris).ThenInclude(cc => cc.NhapKhoDuocPhamChiTiet).ThenInclude(cc => cc.HopDongThauDuocPhams)
                    .Load();

                var yeuCauVatTuBenhViens = BaseRepository.Context.Entry(yctn).Collection(o => o.YeuCauVatTuBenhViens);
                yeuCauVatTuBenhViens.Query()
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien)
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NhanVienThucHien)
                    .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Include(cc => cc.YeuCauDichVuKyThuat).ThenInclude(cc => cc.DichVuKyThuatBenhVien).ThenInclude(cc => cc.DichVuKyThuat)
                    .Include(cc => cc.VatTuBenhVien).ThenInclude(cc => cc.VatTus)
                    .Include(cc => cc.XuatKhoVatTuChiTiet).ThenInclude(cc => cc.XuatKhoVatTuChiTietViTris).ThenInclude(cc => cc.NhapKhoVatTuChiTiet).ThenInclude(cc => cc.HopDongThauVatTu)
                    .Include(cc => cc.NhanVienChiDinh)
                    .Include(cc => cc.NoiChiDinh)
                    .Load();

                var donThuocThanhToans = BaseRepository.Context.Entry(yctn).Collection(o => o.DonThuocThanhToans);
                donThuocThanhToans.Query()
                    .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuocPham).ThenInclude(o => o.DuocPhamBenhVien)
                    .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DuongDung)
                    .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.DonViTinh)
                    .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.YeuCauKhamBenhDonThuocChiTiet)
                    .Include(cc => cc.DonThuocThanhToanChiTiets).ThenInclude(o => o.NoiTruDonThuocChiTiet)
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.BacSiThucHien).ThenInclude(c => c.User)
                    .Include(cc => cc.YeuCauKhamBenh).ThenInclude(cc => cc.NoiThucHien).ThenInclude(cc => cc.KhoaPhong)
                    .Load();
            }


            var yeuCauDichVuKyThuatIds = yctnCoBHYTNgoaiTruVaNoiTrus.SelectMany(o => o.YeuCauDichVuKyThuats).Where(o => o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem).Select(o => o.Id).Distinct().ToList();
            var phienXetNghiemChiTietData = _phienXetNghiemChiTietRepository.TableNoTracking.Where(o => yeuCauDichVuKyThuatIds.Contains(o.YeuCauDichVuKyThuatId)).Include(o => o.KetQuaXetNghiemChiTiets).ToList();

            var yeuCauTiepNhanNoiTruIds = yctnCoBHYTNgoaiTruVaNoiTrus.Where(o => o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru).Select(o => o.Id).ToList();
            var bienBanHoiChanData = _noiTruHoSoKhacRepository.TableNoTracking.Where(o => yeuCauTiepNhanNoiTruIds.Contains(o.YeuCauTiepNhanId) && o.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat).ToList();

            var icdIds = new List<long>();
            foreach (var yctn in yctnCoBHYTNgoaiTruVaNoiTrus)
            {
                if (yctn.NoiTruBenhAn != null)
                {
                    var noiTru = yctn.NoiTruBenhAn;
                    if (noiTru.ChanDoanChinhRaVienICDId != null)
                    {
                        icdIds.Add(noiTru.ChanDoanChinhRaVienICDId.Value);
                    }
                    if (!string.IsNullOrEmpty(noiTru.DanhSachChanDoanKemTheoRaVienICDId))
                    {
                        var icdKemTheoIds = noiTru.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                        foreach (var icdKemTheoId in icdKemTheoIds)
                        {
                            icdIds.Add(long.Parse(icdKemTheoId));
                        }
                    }
                    foreach (var noiTruPhieuDieuTri in yctn.NoiTruBenhAn.NoiTruPhieuDieuTris)
                    {
                        if (noiTruPhieuDieuTri.ChanDoanChinhICDId != null)
                        {
                            icdIds.Add(noiTruPhieuDieuTri.ChanDoanChinhICDId.Value);
                        }
                        foreach (var chanDoanKemTheo in noiTruPhieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                        {
                            icdIds.Add(chanDoanKemTheo.ICDId);
                        }
                    }
                }
                if (yctn.YeuCauNhapVien != null && yctn.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                {
                    icdIds.Add(yctn.YeuCauNhapVien.ChanDoanNhapVienICDId.Value);
                }
            }

            icdIds = icdIds.Distinct().ToList();
            var icdData = _icdRepository.TableNoTracking.Where(o => icdIds.Contains(o.Id)).Select(o => new { o.Id, o.Ma, o.TenTiengViet }).ToList();

            var thongTinBenhNhanGoiBHYTs = new List<ThongTinBenhNhan>();
            foreach (var yeuCauTiepNhanId in danhSachYeuCauTiepNhanIds.Id)
            {
                var yctnNgoaiTru = yctnCoBHYTNgoaiTruVaNoiTrus.FirstOrDefault(o => o.Id == yeuCauTiepNhanId);
                var yctnNoiTru = yctnCoBHYTNgoaiTruVaNoiTrus.FirstOrDefault(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == yeuCauTiepNhanId);

                var errorMessage = KiemYeuCauTiepNhanGuiCongBHYT(yctnNgoaiTru, yctnNoiTru);
                if (errorMessage != null)
                {
                    thongTinBenhNhanGoiBHYTs.Add(new ThongTinBenhNhan
                    {
                        YeuCauTiepNhanId = yeuCauTiepNhanId,
                        IsError = true,
                        ErrorMessage = errorMessage
                    });
                    continue;
                }

                var thangSinh = yctnNgoaiTru.ThangSinh == 0 ? 1 : yctnNgoaiTru.ThangSinh.Value;
                var ngaySinh = yctnNgoaiTru.NgaySinh == 0 ? 1 : yctnNgoaiTru.NgaySinh.Value;
                var ngaySinhBN = new DateTime(yctnNgoaiTru.NamSinh.Value, thangSinh, ngaySinh);


                var maBenh = string.Empty;
                //var tenBenhChinh = string.Empty;
                var maBenhKhac = string.Empty;
                //var tenBenhKhac = string.Empty;
                var tenBenh = string.Empty;

                var ngayRa = yctnNgoaiTru.ThoiDiemTiepNhan;
                var soNgayDieuTri = 1;
                var ketQuaDieuTri = Enums.EnumKetQuaDieuTri.Khoi;
                var tinhTrangRaVien = Enums.EnumTinhTrangRaVien.RaVien;
                var ngayThanhToan = yctnNgoaiTru.ThoiDiemTiepNhan;
                var maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.KhamBenh;
                var maKhoa = string.Empty;
                var tuoiBN = CalculateHelper.TinhTuoi(ngaySinhBN.Day, ngaySinhBN.Month, ngaySinhBN.Year);
                var canNang = yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                var maNoiChuyen = string.Empty;

                var yeuCauKhamBenhCoBHYTs = yctnNgoaiTru.YeuCauKhamBenhs
                    .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == true && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList();
                var yeuCauKhamBenhChinh = yeuCauKhamBenhCoBHYTs.OrderByDescending(o => o.TiLeBaoHiemThanhToan).FirstOrDefault();
                //var yeuCauKhamBenhChinh = yctnNgoaiTru.YeuCauKhamBenhs
                //    .Where(o => o.IcdchinhId != null && o.DuocHuongBaoHiem && o.DuocHuongBaoHiem && o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).OrderBy(o => o.ThoiDiemHoanThanh).LastOrDefault();

                if (yeuCauKhamBenhChinh != null && yctnNoiTru == null)
                {
                    var dsTenBenh = new List<string>();
                    var dsMaBenhKhac = new List<string>();
                    if (yeuCauKhamBenhChinh.Icdchinh != null)
                    {
                        maBenh = yeuCauKhamBenhChinh.Icdchinh.Ma;
                        dsTenBenh.Add(yeuCauKhamBenhChinh.Icdchinh.TenTiengViet);
                    }
                    if (yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Any())
                    {
                        dsMaBenhKhac.AddRange(yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));
                        dsTenBenh.AddRange(yeuCauKhamBenhChinh.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.TenTiengViet));
                    }

                    foreach (var ycKham in yeuCauKhamBenhCoBHYTs)
                    {
                        if (ycKham.Id == yeuCauKhamBenhChinh.Id)
                            continue;
                        if (ycKham.Icdchinh != null)
                        {
                            dsMaBenhKhac.Add(ycKham.Icdchinh.Ma);
                            dsTenBenh.Add(ycKham.Icdchinh.TenTiengViet);
                        }
                        if (ycKham.YeuCauKhamBenhICDKhacs.Any())
                        {
                            dsMaBenhKhac.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));
                            dsTenBenh.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.TenTiengViet));
                        }
                    }
                    maBenhKhac = string.Join(';', dsMaBenhKhac.Where(o => o != maBenh).Distinct());
                    tenBenh = string.Join(';', dsTenBenh.Distinct());

                    var yeuCauKhamBenhCuoi = yeuCauKhamBenhCoBHYTs.Where(o => o.ThoiDiemHoanThanh != null).OrderBy(o => o.ThoiDiemHoanThanh).LastOrDefault();

                    ngayRa = yeuCauKhamBenhCuoi?.ThoiDiemHoanThanh ?? yeuCauKhamBenhChinh.ThoiDiemChiDinh;

                    soNgayDieuTri = (int)(ngayRa - yctnNgoaiTru.ThoiDiemTiepNhan).TotalDays + 1;
                    ketQuaDieuTri = yeuCauKhamBenhChinh.KetQuaDieuTri ?? Enums.EnumKetQuaDieuTri.Khoi;
                    tinhTrangRaVien = yeuCauKhamBenhChinh.TinhTrangRaVien ?? Enums.EnumTinhTrangRaVien.RaVien;
                    ngayThanhToan = ngayRa;
                    maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.KhamBenh;
                    maKhoa = GetMaChuyenKhoa(yeuCauKhamBenhChinh.NoiThucHien?.KhoaPhongId, khoaPhongChuyenKhoas);// yeuCauKhamBenhChinh.NoiThucHien?.KhoaPhong.Ma,
                    canNang = yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                    maNoiChuyen = yeuCauKhamBenhChinh.BenhVienChuyenVien?.Ma;
                    if (!string.IsNullOrEmpty(maNoiChuyen))
                    {
                        tinhTrangRaVien = EnumTinhTrangRaVien.ChuyenVien;
                    }
                }

                if (yctnNoiTru != null)
                {
                    var dsTenBenh = new List<string>();
                    var dsMaBenhKhac = new List<string>();

                    if (yctnNoiTru.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || yctnNoiTru.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
                    {
                        if (yctnNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.Count() > 0)
                        {
                            var phieuDieuTri = yctnNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).First();

                            if (phieuDieuTri.ChanDoanChinhICDId != null)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == phieuDieuTri.ChanDoanChinhICDId);
                                if (icd != null)
                                {
                                    maBenh = icd.Ma;
                                    dsTenBenh.Add(icd.TenTiengViet);
                                }
                            }
                            foreach (var chanDoanKemTheo in phieuDieuTri.NoiTruThamKhamChanDoanKemTheos)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == chanDoanKemTheo.ICDId);
                                if (icd != null)
                                {
                                    if (!string.IsNullOrEmpty(icd.Ma))
                                    {
                                        dsMaBenhKhac.Add(icd.Ma);
                                    }
                                    if (!string.IsNullOrEmpty(icd.TenTiengViet))
                                    {
                                        dsTenBenh.Add(icd.TenTiengViet);
                                    }
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(maBenh))
                    {
                        if (yctnNoiTru.NoiTruBenhAn.ChanDoanChinhRaVienICD != null)
                        {
                            maBenh = yctnNoiTru.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma;
                            dsTenBenh.Add(yctnNoiTru.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet);
                        }
                        else if (yctnNoiTru.YeuCauNhapVien != null && yctnNoiTru.YeuCauNhapVien.ChanDoanNhapVienICDId != null)
                        {
                            var icdChanDoanNhapVien = icdData.FirstOrDefault(o => o.Id == yctnNoiTru.YeuCauNhapVien.ChanDoanNhapVienICDId);
                            if (icdChanDoanNhapVien != null)
                            {
                                maBenh = icdChanDoanNhapVien.Ma;
                                dsTenBenh.Add(icdChanDoanNhapVien.TenTiengViet);
                            }
                        }

                        if (!string.IsNullOrEmpty(yctnNoiTru.NoiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId))
                        {
                            var icdKemTheoIds = yctnNoiTru.NoiTruBenhAn.DanhSachChanDoanKemTheoRaVienICDId.Split(Constants.ICDSeparator);
                            for (int i = 0; i < icdKemTheoIds.Count(); i++)
                            {
                                var icd = icdData.FirstOrDefault(o => o.Id == long.Parse(icdKemTheoIds[i]));
                                if (icd != null)
                                {
                                    if (!string.IsNullOrEmpty(icd.Ma))
                                    {
                                        dsMaBenhKhac.Add(icd.Ma);
                                    }
                                    if (!string.IsNullOrEmpty(icd.TenTiengViet))
                                    {
                                        dsTenBenh.Add(icd.TenTiengViet);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var ycKham in yeuCauKhamBenhCoBHYTs)
                    {
                        if (ycKham.Icdchinh != null)
                        {
                            dsMaBenhKhac.Add(ycKham.Icdchinh.Ma);
                            dsTenBenh.Add(ycKham.Icdchinh.TenTiengViet);
                        }
                        if (ycKham.YeuCauKhamBenhICDKhacs.Any())
                        {
                            dsMaBenhKhac.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));
                            dsTenBenh.AddRange(ycKham.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.TenTiengViet));
                        }
                    }

                    maBenhKhac = string.Join(';', dsMaBenhKhac.Where(o => o != maBenh).Distinct());
                    tenBenh = string.Join(';', dsTenBenh.Distinct());

                    ngayRa = yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien.Value;
                    soNgayDieuTri = (int)(yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien.Value.Date - yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien.Date).TotalDays + 1;
                    ketQuaDieuTri = yctnNoiTru.NoiTruBenhAn.KetQuaDieuTri ?? Enums.EnumKetQuaDieuTri.Khoi;
                    tinhTrangRaVien = yctnNoiTru.NoiTruBenhAn.TinhTrangRaVien ?? Enums.EnumTinhTrangRaVien.RaVien;
                    ngayThanhToan = yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien.Value;
                    maLoaiKCB = Enums.EnumMaHoaHinhThucKCB.DieuTriNoiTru;
                    var khoaPhongDieuTriCuoiId = yctnNoiTru.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.OrderBy(o => o.ThoiDiemVaoKhoa).LastOrDefault()?.KhoaPhongChuyenDenId;
                    maKhoa = GetMaChuyenKhoa(khoaPhongDieuTriCuoiId, khoaPhongChuyenKhoas);
                    canNang = yctnNoiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang;
                    if (yctnNoiTru.NoiTruBenhAn.TinhTrangRaVien == EnumTinhTrangRaVien.ChuyenVien && yctnNoiTru.NoiTruBenhAn.ThongTinRaVien != null)
                    {
                        var thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(yctnNoiTru.NoiTruBenhAn.ThongTinRaVien);
                        if (thongTinRaVien.BenhVienId != null)
                        {
                            var benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(o => o.Id == thongTinRaVien.BenhVienId);
                            if (benhVien != null)
                            {
                                maNoiChuyen = benhVien.Ma;
                            }
                        }
                    }
                }

                //=============================================================================== HỒ SƠ THÔNG TIN NGƯỜI BỆNH XML1 ==============================================================================================================
                var thongTinGoiBaoHiemYte = new ThongTinBenhNhan()
                {
                    STT = 1,
                    MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                    MaBenhNhan = yctnNgoaiTru.BenhNhan.MaBN,
                    HoTen = yctnNgoaiTru.HoTen,
                    NgaySinh = ngaySinhBN,
                    GioiTinh = yctnNgoaiTru.GioiTinh ?? Enums.LoaiGioiTinh.ChuaXacDinh,
                    DiaChi = yctnNgoaiTru.BHYTDiaChi,
                    MaThe = yctnNgoaiTru.BHYTMaSoThe,
                    MaCoSoKCBBanDau = yctnNgoaiTru.BHYTMaDKBD,
                    GiaTriTheTu = yctnNgoaiTru.BHYTNgayHieuLuc ?? yctnNgoaiTru.ThoiDiemTiepNhan,
                    GiaTriTheDen = yctnNgoaiTru.BHYTNgayHetHan,
                    MienCungChiTra = yctnNgoaiTru.BHYTNgayDuocMienCungChiTra,
                    TenBenh = tenBenh,
                    MaBenh = maBenh,
                    MaBenhKhac = maBenhKhac,
                    LyDoVaoVien = yctnNgoaiTru.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen,
                    MaNoiChuyen = maNoiChuyen,
                    MaTaiNan = null,
                    NgayVao = yctnNgoaiTru.ThoiDiemTiepNhan,
                    NgayRa = ngayRa,
                    SoNgayDieuTri = soNgayDieuTri,
                    KetQuaDieuTri = ketQuaDieuTri,
                    TinhTrangRaVien = tinhTrangRaVien,
                    NgayThanhToan = ngayThanhToan,
                    NamQuyetToan = DateTime.Now.Year,
                    ThangQuyetToan = DateTime.Now.Month,
                    MaLoaiKCB = maLoaiKCB,
                    MaKhoa = maKhoa,
                    MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan,
                    MaKhuVuc = yctnNgoaiTru.BHYTMaKhuVuc,
                    MaPhauThuatQuocTe = string.Empty,
                    CanNang = tuoiBN < 1 ? canNang : null,//yctnNgoaiTru.KetQuaSinhHieus.OrderBy(o => o.Id).LastOrDefault()?.CanNang,
                    MucHuong = yctnNgoaiTru.BHYTMucHuong,
                };

                //============================================================================== HỒ SƠ THUỐC XML2 ===========================================================================================================================
                var maPhuongThucThanhToanXML2 = GetMaPhuongThucThanhToan(cauHinhBaoHiemYTe.MaPhuongThucThanhToanXML2);

                var yeuCauDuocPhamBenhViens = yctnNgoaiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauDuocPhamBenhViens.AddRange(yctnNoiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                }
                foreach (var yeuCauDuocPham in yeuCauDuocPhamBenhViens.OrderBy(o => o.Id))
                {
                    var maBenhYeuCauDuocPham = "";
                    var ngayYLenh = yeuCauDuocPham.CreatedOn.Value;

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    if (yeuCauDuocPham.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = yctnNoiTru?.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCauDuocPham.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                            }
                        }
                    }

                    if (yeuCauDuocPham.YeuCauKhamBenh != null && yeuCauDuocPham.YeuCauKhamBenh.Icdchinh != null)
                    {
                        maBenhYeuCauDuocPham = yeuCauDuocPham.YeuCauKhamBenh.Icdchinh.Ma;
                        var maBenhKhacYeuCauDuocPham = string.Join(';', yeuCauDuocPham.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));

                        if (!string.IsNullOrEmpty(maBenhKhacYeuCauDuocPham))
                        {
                            maBenhYeuCauDuocPham = maBenhYeuCauDuocPham + ";" + maBenhKhacYeuCauDuocPham;
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhYeuCauDuocPham))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhYeuCauDuocPham = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhYeuCauDuocPham = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    var lieuDung = GetLieuDungBHYT(yeuCauDuocPham.NoiTruChiDinhDuocPham?.SoLanTrenVien, yeuCauDuocPham.NoiTruChiDinhDuocPham?.LieuDungTrenNgay, yeuCauDuocPham.GhiChu);
                    var groupXuatKhoTheoNhaThau = yeuCauDuocPham.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.GroupBy(o => o.NhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId);
                    double soLuongDaGhi = 0;
                    foreach (var xuatKhoTheoNhaThau in groupXuatKhoTheoNhaThau)
                    {
                        var hopDongThau = xuatKhoTheoNhaThau.First().NhapKhoDuocPhamChiTiet.HopDongThauDuocPhams;
                        double soLuongTheoNhaThau = xuatKhoTheoNhaThau.Sum(o => o.SoLuongXuat).MathRoundNumber(2);
                        var soLuongGhi = (soLuongTheoNhaThau + soLuongDaGhi) <= yeuCauDuocPham.SoLuong ? soLuongTheoNhaThau : (yeuCauDuocPham.SoLuong - soLuongDaGhi).MathRoundNumber(2);

                        if (soLuongGhi > 0)
                        {
                            soLuongDaGhi += soLuongGhi;
                            var thongTinThuoc = new HoSoChiTietThuoc
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                MaThuoc = yeuCauDuocPham.MaHoatChat,
                                MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
                                TenThuoc = yeuCauDuocPham.Ten,
                                DonViTinh = yeuCauDuocPham.DonViTinh.Ten,
                                HamLuong = yeuCauDuocPham.HamLuong,
                                DuongDung = yeuCauDuocPham.DuongDung.Ma,
                                LieuDung = lieuDung,
                                SoDangKy = yeuCauDuocPham.SoDangKy,
                                ThongTinThau = hopDongThau.SoQuyetDinh + ";" + hopDongThau.GoiThau + ";" + hopDongThau.NhomThau,
                                PhamVi = 1,
                                SoLuong = soLuongGhi,
                                DonGia = yeuCauDuocPham.DonGiaBaoHiem.GetValueOrDefault(),
                                TyLeThanhToan = yeuCauDuocPham.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yeuCauDuocPham.MucHuongBaoHiem.GetValueOrDefault(),
                                TienNguonKhac = 0,
                                TienBenhNhanTuTra = 0,
                                TienNgoaiDinhSuat = 0,
                                MaKhoa = GetMaChuyenKhoa(yeuCauDuocPham.NoiChiDinh?.KhoaPhongId, khoaPhongChuyenKhoas),// yeuCauDuocPham.NoiChiDinh.KhoaPhong.Ma,
                                MaBacSi = yeuCauDuocPham.NhanVienChiDinh.MaChungChiHanhNghe,
                                MaBenh = maBenhYeuCauDuocPham,
                                NgayYLenh = ngayYLenh,
                                MaPhuongThucThanhToan = maPhuongThucThanhToanXML2
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietThuoc.Add(thongTinThuoc);
                        }

                    }
                }

                var donThuocThanhToanChiTiets = yctnNgoaiTru.DonThuocThanhToans.Where(o => o.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    donThuocThanhToanChiTiets.AddRange(yctnNoiTru.DonThuocThanhToans.Where(o => o.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true));
                }

                foreach (var donThuocChiTiet in donThuocThanhToanChiTiets.OrderBy(o => o.Id))
                {
                    var khoaPhongKeDonId = donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh?.NoiThucHien?.KhoaPhongId;
                    var maKhoaKeDon = maKhoa;
                    if (khoaPhongKeDonId != null)
                    {
                        maKhoaKeDon = GetMaChuyenKhoa(khoaPhongKeDonId, khoaPhongChuyenKhoas);
                    }
                    var maBacSiKeDon = donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh?.BacSiThucHien?.MaChungChiHanhNghe;
                    //var maBenhTheoDon = donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh?.Icdchinh?.Ma ?? maBenh;

                    var maBenhTheoDon = "";
                    if (donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh != null && donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh.Icdchinh != null)
                    {
                        maBenhTheoDon = donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh.Icdchinh.Ma;
                        var maBenhKhacYeuCauDuocPham = string.Join(';', donThuocChiTiet.DonThuocThanhToan.YeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));

                        if (!string.IsNullOrEmpty(maBenhKhacYeuCauDuocPham))
                        {
                            maBenhTheoDon = maBenhTheoDon + ";" + maBenhKhacYeuCauDuocPham;
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhTheoDon))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoDon = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoDon = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    string lieuDung = string.Empty;
                    if (donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet != null)
                    {
                        lieuDung = GetLieuDungBHYT(donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.SoLanTrenVien, donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.LieuDungTrenNgay, donThuocChiTiet.YeuCauKhamBenhDonThuocChiTiet.GhiChu);
                    }
                    else if (donThuocChiTiet.NoiTruDonThuocChiTiet != null)
                    {
                        lieuDung = GetLieuDungBHYT(null, null, donThuocChiTiet.NoiTruDonThuocChiTiet.GhiChu);
                    }
                    else
                    {
                        lieuDung = GetLieuDungBHYT(null, null, donThuocChiTiet.GhiChu);
                    }

                    //MaKhoa = GetMaChuyenKhoa(yeuCauKhamBenh.NoiThucHien?.KhoaPhongId, khoaPhongChuyenKhoas),// yeuCauKhamBenh.NoiThucHien?.KhoaPhong.Ma,
                    //MaBacSi = yeuCauKhamBenh.BacSiThucHien?.MaChungChiHanhNghe,
                    //MaBenh = yeuCauKhamBenh.Icdchinh?.Ma,
                    var thongTinThuoc = new HoSoChiTietThuoc
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaThuoc = donThuocChiTiet.MaHoatChat,
                        MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.ThuocTrongDanhMucBHYT,
                        TenThuoc = donThuocChiTiet.Ten,
                        DonViTinh = donThuocChiTiet.DonViTinh.Ten,
                        HamLuong = donThuocChiTiet.HamLuong,
                        DuongDung = donThuocChiTiet.DuongDung.Ma,
                        LieuDung = lieuDung,
                        SoDangKy = donThuocChiTiet.SoDangKy,
                        ThongTinThau = donThuocChiTiet.SoQuyetDinhThau + ";" + donThuocChiTiet.GoiThau + ";" + donThuocChiTiet.NhomThau,
                        PhamVi = 1,
                        SoLuong = donThuocChiTiet.SoLuong,
                        DonGia = donThuocChiTiet.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = donThuocChiTiet.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = donThuocChiTiet.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = maKhoaKeDon,
                        MaBacSi = maBacSiKeDon,
                        MaBenh = maBenhTheoDon,
                        NgayYLenh = donThuocChiTiet.CreatedOn.Value,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML2
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietThuoc.Add(thongTinThuoc);
                }

                //============================================================================== DỊCH VỤ KỸ THUẬT & VẬT TƯ (XML3) ====================================================================================================================
                var maPhuongThucThanhToanXML3 = GetMaPhuongThucThanhToan(cauHinhBaoHiemYTe.MaPhuongThucThanhToanXML3);
                var yeuCauVatTuBenhViens = yctnNgoaiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauVatTuBenhViens.AddRange(yctnNoiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                }

                foreach (var yeuCauVatTu in yeuCauVatTuBenhViens.OrderBy(o => o.Id))
                {
                    var maDichVuKyThuat = yeuCauVatTu.YeuCauDichVuKyThuat?.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaChung;
                    var maBenhTheoVatTu = "";

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    var ngayYLenh = yeuCauVatTu.CreatedOn.Value;
                    var ngayKetQua = yeuCauVatTu.ThoiDiemChiDinh;
                    if (yeuCauVatTu.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = yctnNoiTru?.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCauVatTu.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                                ngayKetQua = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                    {
                        maBenhTheoVatTu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                    }
                    else
                    {
                        maBenhTheoVatTu = thongTinGoiBaoHiemYte.MaBenh;
                    }

                    var groupXuatKhoTheoNhaThau = yeuCauVatTu.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.GroupBy(o => o.NhapKhoVatTuChiTiet.HopDongThauVatTu);
                    double soLuongDaGhi = 0;
                    foreach (var xuatKhoTheoNhaThau in groupXuatKhoTheoNhaThau)
                    {
                        var hopDongThau = xuatKhoTheoNhaThau.First().NhapKhoVatTuChiTiet.HopDongThauVatTu;

                        double soLuongTheoNhaThau = xuatKhoTheoNhaThau.Sum(o => o.SoLuongXuat).MathRoundNumber(2);
                        var soLuongGhi = (soLuongTheoNhaThau + soLuongDaGhi) <= yeuCauVatTu.SoLuong ? soLuongTheoNhaThau : (yeuCauVatTu.SoLuong - soLuongDaGhi).MathRoundNumber(2);
                        if (soLuongGhi > 0)
                        {
                            soLuongDaGhi += soLuongGhi;
                            var thongTinVatTu = new HoSoChiTietDVKT
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                MaDichVu = maDichVuKyThuat,
                                MaVatTu = yeuCauVatTu.VatTuBenhVien.VatTus.Ma,
                                MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.VatTuYTeTrongDanhMucBHYT,
                                TenVatTu = yeuCauVatTu.VatTuBenhVien.VatTus.Ten,
                                DonViTinh = yeuCauVatTu.VatTuBenhVien.VatTus.DonViTinh,
                                PhamVi = 1,
                                SoLuong = soLuongGhi,
                                DonGia = yeuCauVatTu.DonGiaBaoHiem.GetValueOrDefault(),
                                ThongTinThau = hopDongThau.SoQuyetDinh + ";" + hopDongThau.GoiThau + ";" + hopDongThau.NhomThau,
                                TyLeThanhToan = yeuCauVatTu.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuong = yeuCauVatTu.MucHuongBaoHiem.GetValueOrDefault(),
                                TienNguonKhac = 0,
                                TienBenhNhanTuTra = 0,
                                TienNgoaiDinhSuat = 0,
                                MaKhoa = GetMaChuyenKhoa(yeuCauVatTu.NoiChiDinh?.KhoaPhongId, khoaPhongChuyenKhoas),//yeuCauVatTu.NoiChiDinh.KhoaPhong.Ma,
                                MaBacSi = yeuCauVatTu.NhanVienChiDinh.MaChungChiHanhNghe,
                                MaBenh = maBenhTheoVatTu,
                                NgayYLenh = ngayYLenh,
                                NgayKetQua = ngayKetQua,
                                MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinVatTu);
                        }
                    }
                }

                var yeuCauDichVuKyThuats = yctnNgoaiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauDichVuKyThuats.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true));
                }
                foreach (var yeuCau in yeuCauDichVuKyThuats.OrderBy(o => o.Id))
                {
                    var maBenhTheoDichVu = "";
                    if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                    {
                        maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                    }
                    else
                    {
                        maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh;
                    }

                    //update 04/08/2022: Thời gian y lệnh của DP, DVKT, truyền máu lấy theo ngày điều trị (tạm thời để 09:00)
                    var ngayYLenh = yeuCau.ThoiDiemThucHien ?? yeuCau.ThoiDiemDangKy;
                    var ngayKetQua = yeuCau.ThoiDiemKetLuan ?? yeuCau.ThoiDiemHoanThanh;
                    if (yeuCau.NoiTruPhieuDieuTriId != null)
                    {
                        var phieuDieuTri = yctnNoiTru?.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(o => o.Id == yeuCau.NoiTruPhieuDieuTriId);
                        if (phieuDieuTri != null)
                        {
                            if (ngayYLenh.Date != phieuDieuTri.NgayDieuTri.Date)
                            {
                                ngayYLenh = phieuDieuTri.NgayDieuTri.Date.AddHours(9);
                                if (ngayKetQua < ngayYLenh)
                                {
                                    ngayKetQua = ngayYLenh;
                                }
                            }
                        }
                    }

                    var maDichVu = yeuCau.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaChung ?? yeuCau.MaDichVu;
                    var tenDichVu = yeuCau.DichVuKyThuatBenhVien?.DichVuKyThuat?.TenChung ?? yeuCau.TenDichVu;
                    var thongTinKyThuat = new HoSoChiTietDVKT
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        MaNhom = yeuCau.NhomChiPhi,
                        TenDichVu = tenDichVu,
                        DonViTinh = "Lần",
                        PhamVi = 1,
                        SoLuong = yeuCau.SoLan,
                        DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = GetMaChuyenKhoa(yeuCau.NoiThucHien?.KhoaPhongId ?? yeuCau.NoiChiDinh.KhoaPhongId, khoaPhongChuyenKhoas),//yeuCau.NoiThucHien?.KhoaPhong.Ma ?? yeuCau.NoiChiDinh.KhoaPhong.Ma,
                        MaBacSi = yeuCau.NhanVienThucHien?.MaChungChiHanhNghe ?? yeuCau.NhanVienChiDinh.MaChungChiHanhNghe,
                        MaBenh = maBenhTheoDichVu,
                        NgayYLenh = ngayYLenh,
                        NgayKetQua = ngayKetQua,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                }

                var yeuCauKhamBenhs = yctnNgoaiTru.YeuCauKhamBenhs.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauKhamBenhs.AddRange(yctnNoiTru.YeuCauKhamBenhs.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true));
                }
                foreach (var yeuCau in yeuCauKhamBenhs.OrderBy(o => o.Id))
                {
                    var maBenhTheoKhamBenh = "";
                    if (yeuCau.Icdchinh != null)
                    {
                        maBenhTheoKhamBenh = yeuCau.Icdchinh.Ma;
                        var maBenhKhacYeuCauKhamBenh = string.Join(';', yeuCau.YeuCauKhamBenhICDKhacs.Select(o => o.ICD.Ma));

                        if (!string.IsNullOrEmpty(maBenhKhacYeuCauKhamBenh))
                        {
                            maBenhTheoKhamBenh = maBenhTheoKhamBenh + ";" + maBenhKhacYeuCauKhamBenh;
                        }
                    }
                    if (string.IsNullOrEmpty(maBenhTheoKhamBenh))
                    {
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoKhamBenh = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoKhamBenh = thongTinGoiBaoHiemYte.MaBenh;
                        }
                    }
                    var maDichVu = yeuCau.DichVuKhamBenhBenhVien?.DichVuKhamBenh?.MaChung ?? yeuCau.MaDichVu;
                    var tenDichVu = yeuCau.DichVuKhamBenhBenhVien?.DichVuKhamBenh?.TenChung ?? yeuCau.TenDichVu;
                    var thongTinKyThuat = new HoSoChiTietDVKT
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.KhamBenh,
                        TenDichVu = tenDichVu,
                        DonViTinh = "Lần",
                        PhamVi = 1,
                        SoLuong = 1,
                        DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                        TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                        MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                        TienNguonKhac = 0,
                        TienBenhNhanTuTra = 0,
                        TienNgoaiDinhSuat = 0,
                        MaKhoa = GetMaChuyenKhoa(yeuCau.NoiThucHien?.KhoaPhongId, khoaPhongChuyenKhoas),//yeuCau.NoiThucHien?.KhoaPhong.Ma ?? yeuCau.NoiChiDinh.KhoaPhong.Ma,
                        MaBacSi = yeuCau.BacSiThucHien?.MaChungChiHanhNghe,
                        MaBenh = maBenhTheoKhamBenh,
                        //NgayYLenh = yeuCau.ThoiDiemThucHien ?? yeuCau.ThoiDiemDangKy,
                        NgayYLenh = yeuCau.ThoiDiemDangKy,
                        NgayKetQua = yeuCau.ThoiDiemHoanThanh,
                        MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                    };
                    thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                }

                if (yctnNoiTru != null)
                {
                    var yeuCauDichVuGiuongTheoNgays = yctnNoiTru.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(o => o.BaoHiemChiTra == true);

                    foreach (var yeuCau in yeuCauDichVuGiuongTheoNgays.OrderBy(o => o.NgayPhatSinh))
                    {
                        var maBenhTheoDichVu = "";
                        if (!string.IsNullOrEmpty(thongTinGoiBaoHiemYte.MaBenhKhac))
                        {
                            maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh + ";" + thongTinGoiBaoHiemYte.MaBenhKhac;
                        }
                        else
                        {
                            maBenhTheoDichVu = thongTinGoiBaoHiemYte.MaBenh;
                        }
                        var ekipDieuTri = yctnNoiTru.NoiTruBenhAn.NoiTruEkipDieuTris.Where(o => o.TuNgay.Date <= yeuCau.NgayPhatSinh && (o.DenNgay == null || yeuCau.NgayPhatSinh <= o.DenNgay)).FirstOrDefault();
                        if (ekipDieuTri == null)
                        {
                            ekipDieuTri = yctnNoiTru.NoiTruBenhAn.NoiTruEkipDieuTris.FirstOrDefault();
                        }
                        var maBacSiDieuTri = ekipDieuTri?.BacSi?.MaChungChiHanhNghe;
                        var ngayYLenh = yeuCau.NgayPhatSinh < yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien ? yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien : yeuCau.NgayPhatSinh;
                        DateTime? ngayKetQua = null;
                        if (yeuCau.NgayPhatSinh.Date == yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien.Value.Date)
                        {
                            ngayKetQua = yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien;
                        }
                        var thongTinKyThuat = new HoSoChiTietDVKT
                        {
                            MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                            STT = 1,
                            MaDichVu = yeuCau.DichVuGiuongBenhVien.DichVuGiuong.MaChung,
                            MaNhom = Enums.EnumDanhMucNhomTheoChiPhi.GiuongDieuTriNoiTru,
                            TenDichVu = yeuCau.DichVuGiuongBenhVien.DichVuGiuong.TenChung,
                            DonViTinh = "",
                            PhamVi = 1,
                            SoLuong = yeuCau.SoLuong,
                            DonGia = yeuCau.DonGiaBaoHiem.GetValueOrDefault(),
                            TyLeThanhToan = yeuCau.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuong = yeuCau.MucHuongBaoHiem.GetValueOrDefault(),
                            TienNguonKhac = 0,
                            TienBenhNhanTuTra = 0,
                            TienNgoaiDinhSuat = 0,
                            MaKhoa = GetMaChuyenKhoa(yeuCau.KhoaPhongId, khoaPhongChuyenKhoas),
                            MaGiuong = yeuCau.GiuongBenh?.Ma,
                            MaBacSi = maBacSiDieuTri,
                            MaBenh = maBenhTheoDichVu,
                            NgayYLenh = ngayYLenh,
                            NgayKetQua = ngayKetQua,
                            MaPhuongThucThanhToan = maPhuongThucThanhToanXML3
                        };
                        thongTinGoiBaoHiemYte.HoSoChiTietDVKT.Add(thongTinKyThuat);
                    }
                }

                //============================================================================== DỊCH VỤ KỸ THUẬT CẬN LÂM S XML4 ==========================================================================================================================================================================================================================================================================================================
                var yeuCauDichVuKyThuatCanLamSangs = yctnNgoaiTru.YeuCauDichVuKyThuats
                    .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && o.ThoiDiemHoanThanh != null &&
                                (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauDichVuKyThuatCanLamSangs.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true && o.ThoiDiemHoanThanh != null &&
                                (o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)));
                }
                foreach (var yeuCau in yeuCauDichVuKyThuatCanLamSangs.OrderBy(o => o.Id))
                {
                    var ketLuan = string.Empty;
                    var tenChiSo = string.Empty;
                    var giaTri = string.Empty;
                    var moTa = string.Empty;
                    if ((yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang) && !string.IsNullOrEmpty(yeuCau.DataKetQuaCanLamSang))
                    {
                        var data = JsonConvert.DeserializeObject<ChiTietKetLuanCDHATDCNJSON>(yeuCau.DataKetQuaCanLamSang);
                        if (data != null)
                        {
                            ketLuan = MaskHelper.RemoveHtmlFromString(data.KetLuan);
                        }
                    }
                    else if (yeuCau.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                    {
                        if (yeuCau.DichVuKyThuatBenhVien?.DichVuXetNghiem != null)
                        {
                            tenChiSo = yeuCau.DichVuKyThuatBenhVien.DichVuXetNghiem.Ten;
                        }
                        var phienXNChiTiet = phienXetNghiemChiTietData.Where(o => o.YeuCauDichVuKyThuatId == yeuCau.Id).OrderBy(o => o.LanThucHien).LastOrDefault();
                        if (phienXNChiTiet != null)
                        {
                            moTa = phienXNChiTiet.GhiChu ?? string.Empty;
                            ketLuan = phienXNChiTiet.KetLuan ?? string.Empty;
                            giaTri = phienXNChiTiet.KetQuaXetNghiemChiTiets.Where(o => o.DichVuXetNghiemChaId == null).FirstOrDefault()?.GiaTriNhapTay ?? string.Empty;
                        }
                    }
                    var maDichVu = yeuCau.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaChung ?? yeuCau.MaDichVu;
                    var thongTinCanLamSang = new HoSoCanLamSang
                    {
                        MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                        STT = 1,
                        MaDichVu = maDichVu,
                        TenChiSo = tenChiSo,
                        MoTa = moTa,
                        KetLuan = ketLuan,
                        GiaTri = giaTri,
                        NgayKQ = yeuCau.ThoiDiemHoanThanh.Value,
                        YeuCauDichVuKyThuatId = yeuCau.Id
                    };
                    thongTinGoiBaoHiemYte.HoSoCanLamSang.Add(thongTinCanLamSang);
                }
                //Hồ sơ chi tiết diễn biến bệnh (XML5)
                if (yctnNoiTru != null)
                {
                    var phieuDieuTriTheoNgays = yctnNoiTru.NoiTruBenhAn.NoiTruPhieuDieuTris.GroupBy(o => o.NgayDieuTri.Date);
                    foreach (var phieuDieuTriTheoNgay in phieuDieuTriTheoNgays.OrderBy(o => o.Key))
                    {


                        DateTime? ngayPTTT = null;
                        var ppPhauThuat = string.Empty;
                        var hoiChan = string.Empty;
                        var dienBienData = phieuDieuTriTheoNgay.OrderBy(o => o.Id).LastOrDefault()?.DienBien;
                        var dienBien = string.Empty;
                        if (!string.IsNullOrEmpty(dienBienData))
                        {
                            var dienBiens = JsonConvert.DeserializeObject<List<DienBienPhieuDieuTriGoiBaoHiemYTe>>(dienBienData);
                            if (dienBiens != null)
                            {
                                dienBien = string.Join(';', dienBiens.Select(o => o.DienBien));
                            }
                        }

                        var bienBanHoiChan = bienBanHoiChanData.Where(o => o.ThoiDiemThucHien.Date == phieuDieuTriTheoNgay.Key && o.YeuCauTiepNhanId == yctnNoiTru.Id).OrderBy(o => o.ThoiDiemThucHien).LastOrDefault();
                        if (bienBanHoiChan != null && !string.IsNullOrEmpty(bienBanHoiChan.ThongTinHoSo))
                        {
                            var bienBanhoiChanVo = JsonConvert.DeserializeObject<BienBanHoiChanVo>(bienBanHoiChan.ThongTinHoSo);
                            hoiChan = bienBanhoiChanVo?.KetLuan ?? string.Empty;
                        }

                        var dvPTTTNgoaiTrus = yctnNgoaiTru.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true
                                        && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh.Value.Date == phieuDieuTriTheoNgay.Key && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList();
                        var dvPTTTNoiTrus = yctnNoiTru.YeuCauDichVuKyThuats
                            .Where(o => o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true
                                        && o.ThoiDiemHoanThanh != null && o.ThoiDiemHoanThanh.Value.Date == phieuDieuTriTheoNgay.Key && o.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList();
                        if (dvPTTTNgoaiTrus.Any() || dvPTTTNoiTrus.Any())
                        {
                            var dvPTTTs = dvPTTTNgoaiTrus.Concat(dvPTTTNoiTrus).OrderBy(o => o.ThoiDiemHoanThanh).ToList();

                            ngayPTTT = dvPTTTs.First().ThoiDiemHoanThanh;
                            var phuongPhapPTTTs = dvPTTTs
                                .Where(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT != null && !string.IsNullOrEmpty(o.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT))
                                .Select(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT.TenPhuongPhapPTTT).ToList();
                            if (phuongPhapPTTTs.Any())
                            {
                                ppPhauThuat = string.Join(';', phuongPhapPTTTs);
                            }
                        }
                        DateTime ngayThepNhapVien = new DateTime(phieuDieuTriTheoNgay.Key.Year, phieuDieuTriTheoNgay.Key.Month, phieuDieuTriTheoNgay.Key.Day, yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien.Hour, yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien.Minute, yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien.Second);
                        var ngayYLenh = ngayPTTT != null ? ngayPTTT.Value.AddMinutes(2) : ngayThepNhapVien.AddMinutes(2);

                        if (yctnNoiTru.NoiTruBenhAn.ThoiDiemNhapVien <= ngayYLenh && ngayYLenh <= yctnNoiTru.NoiTruBenhAn.ThoiDiemRaVien)
                        {
                            var hoSoChiTietDienBienBenh = new HoSoChiTietDienBienBenh
                            {
                                MaLienKet = yctnNgoaiTru.MaYeuCauTiepNhan,
                                STT = 1,
                                DienBien = dienBien,
                                NgayYLenh = ngayYLenh,
                                PhauThuat = ppPhauThuat,
                                HoiChuan = hoiChan
                            };
                            thongTinGoiBaoHiemYte.HoSoChiTietDienBienBenh.Add(hoSoChiTietDienBienBenh);
                        }
                    }
                }

                thongTinGoiBaoHiemYte.YeuCauTiepNhanId = yeuCauTiepNhanId;

                //them canh bao dv chua thuc hien
                var yeuCauDuocPhamBenhVienChuaThucHiens = yctnNgoaiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                var donThuocThanhToanChiTietChuaThucHiens = yctnNgoaiTru.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true).ToList();
                var yeuCauVatTuBenhVienChuaThucHiens = yctnNgoaiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true).ToList();
                var yeuCauDichVuKyThuatChuaThucHiens = yctnNgoaiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true).ToList();
                var yeuCauKhamBenhChuaThucHiens = yctnNgoaiTru.YeuCauKhamBenhs.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true).ToList();
                if (yctnNoiTru != null)
                {
                    yeuCauDuocPhamBenhVienChuaThucHiens.AddRange(yctnNoiTru.YeuCauDuocPhamBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                    donThuocThanhToanChiTietChuaThucHiens.AddRange(yctnNoiTru.DonThuocThanhToans.Where(o => o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc)
                                                                                .SelectMany(cc => cc.DonThuocThanhToanChiTiets).Where(p => p.BaoHiemChiTra == true));
                    yeuCauVatTuBenhVienChuaThucHiens.AddRange(yctnNoiTru.YeuCauVatTuBenhViens.Where(o => o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaThucHien && o.BaoHiemChiTra == true));
                    yeuCauDichVuKyThuatChuaThucHiens.AddRange(yctnNoiTru.YeuCauDichVuKyThuats.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && o.BaoHiemChiTra == true));
                    yeuCauKhamBenhChuaThucHiens.AddRange(yctnNoiTru.YeuCauKhamBenhs.Where(o => o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham && o.BaoHiemChiTra == true));
                }
                var dsCanhBao = new List<string>();
                if (yeuCauDuocPhamBenhVienChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Chỉ định dược phẩm chưa thực hiện: {string.Join(';', yeuCauDuocPhamBenhVienChuaThucHiens.Select(o => o.Ten))}");
                }
                if (donThuocThanhToanChiTietChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Đơn thuốc chưa xuất: {string.Join(';', donThuocThanhToanChiTietChuaThucHiens.Select(o => o.Ten))}");
                }
                if (yeuCauVatTuBenhVienChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Chỉ định vật tư chưa thực hiện: {string.Join(';', yeuCauVatTuBenhVienChuaThucHiens.Select(o => o.Ten))}");
                }
                if (yeuCauDichVuKyThuatChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Dịch vụ kỹ thuật chưa thực hiện: {string.Join(';', yeuCauDichVuKyThuatChuaThucHiens.Select(o => o.TenDichVu))}");
                }
                if (yeuCauKhamBenhChuaThucHiens.Any())
                {
                    dsCanhBao.Add($"Dịch vụ khám chưa thực hiện: {string.Join(';', yeuCauKhamBenhChuaThucHiens.Select(o => o.TenDichVu))}");
                }
                if (dsCanhBao.Any())
                {
                    thongTinGoiBaoHiemYte.WarningMessage = string.Join('|', dsCanhBao);
                }

                thongTinBenhNhanGoiBHYTs.Add(thongTinGoiBaoHiemYte);
            }

            return thongTinBenhNhanGoiBHYTs;
        }

        private string GetMaChuyenKhoa(long? khoaPhongId, List<KhoaPhongChuyenKhoa> khoaPhongChuyenKhoas)
        {
            if (khoaPhongId == null)
                return null;
            return khoaPhongChuyenKhoas.FirstOrDefault(o => o.KhoaPhongId == khoaPhongId)?.Khoa.Ma;
        }
        private string KiemYeuCauTiepNhanGuiCongBHYT(YeuCauTiepNhan yeuCauTiepNhan, YeuCauTiepNhan yeuCauTiepNhanNoiTru = null)
        {
            if (yeuCauTiepNhanNoiTru != null)
            {
                if (yeuCauTiepNhanNoiTru.NoiTruBenhAn == null)
                    return "Bệnh nhân chưa tạo bệnh án nội trú";
                if (yeuCauTiepNhanNoiTru.NoiTruBenhAn.ThoiDiemRaVien == null)
                    return "Bệnh nhân chưa kết thúc bệnh án nội trú";
            }
            if (yeuCauTiepNhan == null)
                return "Yêu cầu tiếp nhận không tồn tại";
            if (yeuCauTiepNhan.NamSinh == null)
                return "Không có năm sinh";
            if (yeuCauTiepNhan.BHYTMucHuong == null)
                return "Không mức hưởng bảo hiểm y tế";
            //update 15/04/2022: bỏ kiểm tra dv khám
            //var yeuCauKhamBenhChinh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => o.IcdchinhId != null && o.BaoHiemChiTra == true && o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).OrderBy(o => o.ThoiDiemHoanThanh).LastOrDefault();
            //if (yeuCauKhamBenhChinh == null || yeuCauKhamBenhChinh.ThoiDiemHoanThanh == null)
            //    return "Không có thông tin khám bệnh";

            return null;
        }

        public HamGuiHoSoWatchingVO addValueToXml(List<ThongTinBenhNhan> thongTinBenhNhans)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            string macskcb = cauHinhBaoHiemYTe.BenhVienTiepNhan;
            var result = new HamGuiHoSoWatchingVO();
            string tenFileXMLTongHop = string.Empty;
            XDocument myXml1 = new XDocument();
            var soLuongHoSo = thongTinBenhNhans.Count();

            var ThangQuyetToan = thongTinBenhNhans.FirstOrDefault().ThangQuyetToan < 10 ? "0" + thongTinBenhNhans.FirstOrDefault().ThangQuyetToan : thongTinBenhNhans.FirstOrDefault().ThangQuyetToan.ToString();
            var tenNam = thongTinBenhNhans.FirstOrDefault().NamQuyetToan + ThangQuyetToan;
            tenFileXMLTongHop = $"Resource\\TongHop_{macskcb}_" + tenNam + ".xml";

            //myXml1 = new XDocument(new XElement("GIAMDINHHS", new XAttribute("xmlnsxsd", "http://www.w3.org/2001/XMLSchema"),
            //    new XAttribute("xmlnsxsi", "http://www.w3.org/2001/XMLSchema-instance"),
            //                                new XElement("THONGTINDONVI", new XElement("MACSKCB", "01824")),
            //                                new XElement("THONGTINHOSO",
            //                                new XElement("NGAYLAP", "20191220"),
            //                                new XElement("SOLUONGHOSO", soLuongHoSo),
            //                                new XElement("DANHSACHHOSO")),
            //                                new XElement("CHUKYDONVI", null)
            //                                ));
            myXml1 = new XDocument(new XElement("GIAMDINHHS",
                                            new XElement("THONGTINDONVI", new XElement("MACSKCB", macskcb)),
                                            new XElement("THONGTINHOSO",
                                            new XElement("NGAYLAP", DateTime.Now.ToString(FormatNgayBHYT)),
                                            new XElement("SOLUONGHOSO", soLuongHoSo),
                                            new XElement("DANHSACHHOSO")),
                                            new XElement("CHUKYDONVI", null)
                                            ));

            result.NameFileDown = tenFileXMLTongHop;
            myXml1.Save(@tenFileXMLTongHop);

            int sttXML1 = 0;
            int sttXML2 = 0;
            int sttXML3 = 0;
            int sttXML4 = 0;
            int sttXML5 = 0;

            foreach (var thongtin in thongTinBenhNhans)
            {
                if (thongtin != null)
                {
                    try
                    {
                        TokenBHYT.ModifyNgayLapFileTongHop(@tenFileXMLTongHop);
                        sttXML1 = AddThongTinBenhNhanVoXmlTongHop(thongtin, "XML1", @tenFileXMLTongHop, sttXML1, false);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML1: " + ex.Message;
                        return result;
                    }
                }
                if (thongtin != null && thongtin.HoSoChiTietThuoc.Any())
                {
                    try
                    {
                        sttXML2 = AddHoSoChiTietThuocVoXmlTongHop(thongtin.HoSoChiTietThuoc, "XML2", @tenFileXMLTongHop, sttXML2, false);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML2: " + ex.Message;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoChiTietDVKT.Any())
                {
                    try
                    {
                        sttXML3 = AddHoSoChiTietDVKTVoXMLTongHop(thongtin.HoSoChiTietDVKT, "XML3", @tenFileXMLTongHop, sttXML3, false);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML3: " + ex.Message;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoCanLamSang.Any())
                {
                    try
                    {
                        sttXML4 = AddHoSoCanLamSangVoXMLTongHop(thongtin.HoSoCanLamSang, "XML4", @tenFileXMLTongHop, sttXML4, false);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML4: " + ex.Message;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoChiTietDienBienBenh.Any())
                {
                    try
                    {
                        sttXML5 = AddHoSoChiTietDienBienBenhVoXMLTongHop(thongtin.HoSoChiTietDienBienBenh, "XML5", @tenFileXMLTongHop, sttXML5, false);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML5: " + ex.Message;
                        return result;
                    }
                }
            }

            return result;
        }

        public async Task<HamGuiHoSoWatchingVO> GoiHoSoGiamDinhs(List<ThongTinBenhNhan> thongTinBenhNhans)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            string macskcb = cauHinhBaoHiemYTe.BenhVienTiepNhan;
            var ThangQuyetToan = thongTinBenhNhans.FirstOrDefault().ThangQuyetToan < 10 ? "0" + thongTinBenhNhans.FirstOrDefault().ThangQuyetToan : thongTinBenhNhans.FirstOrDefault().ThangQuyetToan.ToString();
            var tenNam = thongTinBenhNhans.FirstOrDefault().NamQuyetToan + ThangQuyetToan;

            var result = new HamGuiHoSoWatchingVO();
            result.XMLJson = "{\"";

            int sttXML1 = 0;
            int sttXML2 = 0;
            int sttXML3 = 0;
            int sttXML4 = 0;
            int sttXML5 = 0;
            int count = 0;

            var soLuongHoSo = thongTinBenhNhans.Count();
            string tenFileXMLTongHop = $"Resource\\TongHop_{macskcb}_" + tenNam + ".xml";
            //XDocument myXml1 = new XDocument(new XElement("GIAMDINHHS", new XAttribute("xmlnsxsd", "http://www.w3.org/2001/XMLSchema"), new XAttribute("xmlnsxsi", "http://www.w3.org/2001/XMLSchema-instance"),
            //                                   new XElement("THONGTINDONVI", new XElement("MACSKCB", "01824")),
            //                                   new XElement("THONGTINHOSO", new XElement("NGAYLAP", "20191220"),
            //                                   new XElement("SOLUONGHOSO", soLuongHoSo),
            //                                   new XElement("DANHSACHHOSO")),
            //                                   new XElement("CHUKYDONVI", null)
            //                                   ));
            XDocument myXml1 = new XDocument(new XElement("GIAMDINHHS",
                                               new XElement("THONGTINDONVI", new XElement("MACSKCB", macskcb)),
                                               new XElement("THONGTINHOSO", new XElement("NGAYLAP", DateTime.Now.ToString(FormatNgayBHYT)),
                                               new XElement("SOLUONGHOSO", soLuongHoSo),
                                               new XElement("DANHSACHHOSO")),
                                               new XElement("CHUKYDONVI", null)
                                               ));

            result.NameFileDown = tenFileXMLTongHop;
            myXml1.Save(@tenFileXMLTongHop);

            foreach (var thongtin in thongTinBenhNhans)
            {
                if (thongtin != null)
                {
                    result.XMLJson = result.XMLJson + "XML1\":";
                    try
                    {
                        TokenBHYT.ModifyNgayLapFileTongHop(@tenFileXMLTongHop);
                        sttXML1 = AddThongTinBenhNhanVoXmlTongHop(thongtin, "XML1", @tenFileXMLTongHop, sttXML1, true);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML1: " + ex.Message;
                        result.XMLJson = result.XMLJson + "}";
                        result.ErrorCheck = true;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoChiTietThuoc.Any())
                {

                    result.XMLJson = result.XMLJson + "XML2\":";
                    try
                    {
                        sttXML2 = AddHoSoChiTietThuocVoXmlTongHop(thongtin.HoSoChiTietThuoc, "XML2", @tenFileXMLTongHop, sttXML2, true);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML2: " + ex.Message;
                        result.XMLJson = result.XMLJson + "}";
                        result.ErrorCheck = true;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoChiTietDVKT.Any())
                {
                    result.XMLJson = result.XMLJson + "XML3\":";
                    try
                    {
                        sttXML3 = AddHoSoChiTietDVKTVoXMLTongHop(thongtin.HoSoChiTietDVKT, "XML3", @tenFileXMLTongHop, sttXML3, true);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML3: " + ex.Message;
                        result.XMLJson = result.XMLJson + "}";
                        result.ErrorCheck = true;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoCanLamSang.Any())
                {
                    result.XMLJson = result.XMLJson + "XML4\":";
                    try
                    {
                        sttXML4 = AddHoSoCanLamSangVoXMLTongHop(thongtin.HoSoCanLamSang, "XML4", @tenFileXMLTongHop, sttXML4, true);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML4: " + ex.Message;
                        result.XMLJson = result.XMLJson + "}";
                        result.ErrorCheck = true;
                        return result;
                    }
                }

                if (thongtin != null && thongtin.HoSoChiTietDienBienBenh.Any())
                {

                    result.XMLJson = result.XMLJson + "XML5\":";
                    try
                    {
                        sttXML5 = AddHoSoChiTietDienBienBenhVoXMLTongHop(thongtin.HoSoChiTietDienBienBenh, "XML5", @tenFileXMLTongHop, sttXML5, true);
                    }
                    catch (Exception ex)
                    {
                        result.XMLError = "XML5: " + ex.Message;
                        result.XMLJson = result.XMLJson + "}";
                        result.ErrorCheck = true;
                        return result;
                    }
                }
            }
            try
            {
                result.XMLJson = result.XMLJson + "}";
                var Bytearray = ConvertXmlToBase64Helper.XMLtoBytes(@tenFileXMLTongHop);

                result.ByteData = Bytearray;
                result.ErrorCheck = false;
                result.countFile = count;

                //========================== Bước 2: Gởi ByteData qua bên bảo hiểm y tế================================================
                if (cauHinhBaoHiemYTe.GoiCongBHYT)
                    GoiThongTinQuaCongBHYT(result, macskcb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result.XMLError = "Finally: " + ex.Message;
                //result.XMLJson = result.XMLJson + "}";
                result.ErrorCheck = true;
                return result;
            }

            return result;
        }

        private void GetToken()
        {
            var takeAPI = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/token/take");
            var requesttoken = new RestRequest(Method.POST);
            requesttoken.AddJsonBody(new { username = Constants.UserNameBHYT, password = Constants.PassBHYT });
            IRestResponse responsetoken = takeAPI.Execute(requesttoken);
            if (responsetoken.IsSuccessful)
            {
                var contentToken = JsonConvert.DeserializeObject<ThongTinTokenMoiVO>(responsetoken.Content);
                TokenBHYT.ModifyTokenBHYT(contentToken.APIKey.access_token, contentToken.APIKey.id_token);
            }
        }

        public void GoiThongTinQuaCongBHYT(HamGuiHoSoWatchingVO hamGuiHoSoWatchingVO, string macskcb)
        {
            if (hamGuiHoSoWatchingVO != null)
            {
                if (hamGuiHoSoWatchingVO.ErrorCheck == false)
                {
                    GetToken();
                    var token = TokenBHYT.GetTokenAPI().token;
                    var id_token = TokenBHYT.GetTokenAPI().id_token;

                    var takeAPI = new RestClient($"https://egw.baohiemxahoi.gov.vn/api/egw/guiHoSoGiamDinh4210?token=" + token + "&id_token=" + id_token + "&username=" + Constants.UserNameBHYT + "&password=" + Constants.PassBHYT + "&loaiHoSo=3&maTinh=01&maCSKCB=" + macskcb);

                    var requesttoken = new RestRequest(Method.POST);
                    //requesttoken.AddJsonBody(new { fileHS = hamGuiHoSoWatchingVO.ByteData });
                    requesttoken.AddJsonBody(hamGuiHoSoWatchingVO.ByteData);
                    IRestResponse responsetokenData = takeAPI.Execute(requesttoken);
                    _logger.LogTrace("GoiThongTinQuaCongBHYT", Encoding.UTF8.GetString(hamGuiHoSoWatchingVO.ByteData), responsetokenData.Content);
                    hamGuiHoSoWatchingVO.ErrorCheck = true;
                    if (responsetokenData.IsSuccessful)
                    {
                        
                        var contentHOSo = JsonConvert.DeserializeObject<KetQuaGuiCongBHYT>(responsetokenData.Content);

                        if (contentHOSo.maKetQua.Equals("201"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Định dạng XML không đúng";                          
                        }
                        else if (contentHOSo.maKetQua.Equals("200"))
                        {
                            hamGuiHoSoWatchingVO.ErrorCheck = false;
                        }
                        else if (contentHOSo.maKetQua.Equals("202"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Nội dung XML không đúng";
                        }
                        else if (contentHOSo.maKetQua.Equals("204"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "File XML không có nội dung";
                        }
                        else if (contentHOSo.maKetQua.Equals("205"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Lỗi sai định dạng tham số truyền vào";
                        }
                        else if (contentHOSo.maKetQua.Equals("401"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Lỗi xác thực";
                        }
                        else if (contentHOSo.maKetQua.Equals("408"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Request Timeout";
                        }
                        else if (contentHOSo.maKetQua.Equals("500"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Lỗi Sever BHYT";
                        }
                        else if (contentHOSo.maKetQua.Equals("10"))
                        {
                            hamGuiHoSoWatchingVO.APIError = "Lỗi lấy thông tin từ sever số thẻ";
                        }
                    }
                    else
                    {
                        hamGuiHoSoWatchingVO.APIError = "Lỗi gửi thông tin lên cổng BHYT";
                    }
                }
            }

        }
        //update LuuThongTinDaGoiVaoHeThong
        public int LuuLichSuDuLieuGuiCongBHYT(List<ThongTinBenhNhan> thongTinBenhNhans, bool coGuiCong)
        {
            if (thongTinBenhNhans != null)
            {
                foreach (var thongTinBenhNhan in thongTinBenhNhans)
                {
                    thongTinBenhNhan.WarningMessage = null;
                    thongTinBenhNhan.DataJson = null;
                }
                var duLieuGuiCongBHYT = new DuLieuGuiCongBHYT()
                {
                    DuLieuTongHop = JsonConvert.SerializeObject(thongTinBenhNhans),
                };
                int infoVersionLast = 0;
                foreach (var thongTinBenhNhan in thongTinBenhNhans)
                {
                    var yeuCauTiepNhan = BaseRepository.Table.Where(cc => cc.Id == thongTinBenhNhan.YeuCauTiepNhanId).FirstOrDefault();
                    yeuCauTiepNhan.DaGuiCongBHYT = true;
                    var versionLast = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
                                                .Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
                                                .OrderByDescending(cc => cc.Version).FirstOrDefault();
                    infoVersionLast = versionLast == null ? 1 : (versionLast.Version + 1);                    

                    duLieuGuiCongBHYT.YeuCauTiepNhanDuLieuGuiCongBHYTs.Add(
                        new Core.Domain.Entities.BHYT.YeuCauTiepNhanDuLieuGuiCongBHYT()
                        {
                            YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                            DuLieu = JsonConvert.SerializeObject(thongTinBenhNhan),
                            Version = infoVersionLast,
                            CoGuiCong = coGuiCong,
                            NgayVao = thongTinBenhNhan.NgayVao,
                            NgayRa = thongTinBenhNhan.NgayRa,
                            HinhThucKCB = thongTinBenhNhan.MaLoaiKCB
                        });
                }
                _duLieuGuiCongBHYT.Add(duLieuGuiCongBHYT);
                return infoVersionLast;
            }
            return 0;
        }

        //public int LuuThongTinDaGoiVaoHeThong(List<ThongTinBenhNhan> thongTinBenhNhans)
        //{
        //    if (thongTinBenhNhans != null)
        //    {
        //        foreach (var thongTinBenhNhan in thongTinBenhNhans)
        //        {
        //            var yeuCauTiepNhan = BaseRepository.Table.Where(cc => cc.Id == thongTinBenhNhan.YeuCauTiepNhanId).FirstOrDefault();
        //            yeuCauTiepNhan.DaGuiCongBHYT = true;
        //            var versionLast = _yeuCauTiepNhanDuLieuGuiCongBHYT.TableNoTracking
        //                                        .Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhan.Id)
        //                                        .OrderByDescending(cc => cc.Version).FirstOrDefault();
        //            int infoVersionLast = versionLast.Version + 1;
        //            var duLieuGuiCongBHYT = new DuLieuGuiCongBHYT()
        //            {
        //                DuLieuTongHop = JsonConvert.SerializeObject(thongTinBenhNhan),
        //            };

        //            duLieuGuiCongBHYT.YeuCauTiepNhanDuLieuGuiCongBHYTs.Add(
        //                new Core.Domain.Entities.BHYT.YeuCauTiepNhanDuLieuGuiCongBHYT()
        //                {
        //                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
        //                    DuLieu = JsonConvert.SerializeObject(thongTinBenhNhan),
        //                    Version = infoVersionLast
        //                });

        //            _duLieuGuiCongBHYT.Add(duLieuGuiCongBHYT);
        //            return infoVersionLast;
        //        }
        //    }
        //    return 0;
        //}

        public string KiemTraYeuCauTiepNhanGoiBHYT(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Include(c => c.YeuCauKhamBenhs).FirstOrDefault();
            if (yeuCauTiepNhan == null)
                return "Yêu cầu tiếp nhận không tồn tại";
            if (yeuCauTiepNhan.NamSinh == null)
                return "Không có năm sinh";
            if (yeuCauTiepNhan.BHYTMucHuong == null)
                return "Không mức hưởng bảo hiểm y tế";
            //update 15/04/2022: bỏ kiểm tra dv khám
            //var yeuCauKhamBenhChinh = yeuCauTiepNhan.YeuCauKhamBenhs.Where(o => o.IcdchinhId != null && o.BaoHiemChiTra == true && o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham).OrderBy(o => o.ThoiDiemHoanThanh).LastOrDefault();
            //if (yeuCauKhamBenhChinh == null || yeuCauKhamBenhChinh.ThoiDiemHoanThanh == null)
            //    return "Không có thông tin khám bệnh";
            return null;
        }

        private Enums.EnumMaPhuongThucThanhToan GetMaPhuongThucThanhToan(int maPhuongThucThanhToan)
        {
            switch (maPhuongThucThanhToan)
            {
                case 3:
                    return Enums.EnumMaPhuongThucThanhToan.DRG;
                case 2:
                    return Enums.EnumMaPhuongThucThanhToan.NgoaiDinhSuat;
                case 1:
                    return Enums.EnumMaPhuongThucThanhToan.DinhSuat;
                default:
                    return Enums.EnumMaPhuongThucThanhToan.PhiDichVu;
            }
        }

        #endregion      

        #region  FILL DATA FOR XML1,XML2,XML3,XML4,XML5

        public byte[] pathFileTongHop(string tenFile)
        {
            var byteArray = ConvertXmlToBase64Helper.XMLtoBytes(tenFile);
            return byteArray;
        }

        public int AddThongTinBenhNhanVoXmlTongHop(ThongTinBenhNhan thongtin, string nameFile, string @pathTongHop, int sttXML1, bool goiBHYT)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();
            sttXML1 = sttXML1 + 1;
            thongtin.STT = sttXML1;
            var fileTongHopXML1 = new XElement("TONG_HOP", new XElement("MA_LK", thongtin.MaLienKet), new XElement("STT", thongtin.STT),
                                                                       new XElement("MA_BN", thongtin.MaBenhNhan),
                                                                       new XElement("HO_TEN", thongtin.HoTen), // bỏ ![CDATA]
                                                                       new XElement("NGAY_SINH", ConvertNgayToXML(thongtin.NgaySinh)),
                                                                       new XElement("GIOI_TINH", thongtin.GioiTinh != 0 ? ((int)thongtin.GioiTinh).ToString() : 3.ToString()),
                                                                       //new XElement("DIA_CHI", thongtin.DiaChi), // bỏ ![CDATA]
                                                                       new XElement("DIA_CHI", new XCData(thongtin.DiaChi ?? "")),
                                                                       new XElement("MA_THE", thongtin.MaThe),
                                                                       new XElement("MA_DKBD", thongtin.MaCoSoKCBBanDau),
                                                                       new XElement("GT_THE_TU", ConvertNgayToXML(thongtin.GiaTriTheTu)),
                                                                       new XElement("GT_THE_DEN", ConvertNgayToXML(thongtin.GiaTriTheDen)),
                                                                       new XElement("MIEN_CUNG_CT", ConvertNgayToXML(thongtin.MienCungChiTra)),
                                                                       //new XElement("TEN_BENH", thongtin.TenBenh), // bỏ ![CDATA]
                                                                       new XElement("TEN_BENH", new XCData(thongtin.TenBenh ?? "")),
                                                                       new XElement("MA_BENH", thongtin.MaBenh),
                                                                       new XElement("MA_BENHKHAC", thongtin.MaBenhKhac ?? ""),
                                                                       new XElement("MA_LYDO_VVIEN", ((int)thongtin.LyDoVaoVien).ToString()),
                                                                       new XElement("MA_NOI_CHUYEN", thongtin.MaNoiChuyen == null ? "" : thongtin.MaNoiChuyen),
                                                                       new XElement("MA_TAI_NAN", thongtin.MaTaiNan == null ? "" : ((int)thongtin.MaTaiNan).ToString()),
                                                                       new XElement("NGAY_VAO", ConvertNgayGioXMl(thongtin.NgayVao)),
                                                                       new XElement("NGAY_RA", ConvertNgayGioXMl(thongtin.NgayRa)),
                                                                       new XElement("SO_NGAY_DTRI", thongtin.SoNgayDieuTri.ToString()),
                                                                       new XElement("KET_QUA_DTRI", (int)thongtin.KetQuaDieuTri),
                                                                       new XElement("TINH_TRANG_RV", (int)thongtin.TinhTrangRaVien),
                                                                       new XElement("NGAY_TTOAN", ConvertNgayGioXMl(thongtin.NgayThanhToan)),
                                                                       new XElement("T_THUOC", ConvertSoTienBHYT(thongtin.TienThuoc)), // format lại từ 0.00 sang 0
                                                                       new XElement("T_VTYT", ConvertSoTienBHYT(thongtin.TienVatTuYTe)), // format lại từ 0.00 sang 0
                                                                       new XElement("T_TONGCHI", ConvertSoTienBHYT(thongtin.TienTongChi)), // format lại từ 0.00 sang 0
                                                                       new XElement("T_BNTT", ConvertSoTienBHYT(thongtin.TienBenhNhanTuTra.GetValueOrDefault())), // format lại từ 0.00 sang 0
                                                                       new XElement("T_BNCCT", ConvertSoTienBHYT(thongtin.TienBenhNhanCungChiTra.GetValueOrDefault())), // format lại từ 0.00 sang 0
                                                                       new XElement("T_BHTT", ConvertSoTienBHYT(thongtin.TienBaoHiemThanhToan)), // format lại từ 0.00 sang 0
                                                                       new XElement("T_NGUONKHAC", ConvertSoTienBHYT(thongtin.TienNguonKhac)), // format lại từ 0.00 sang 0
                                                                       new XElement("T_NGOAIDS", ConvertSoTienBHYT(thongtin.TienNgoaiDinhSuat)), // format lại từ 0.00 sang 0
                                                                       new XElement("NAM_QT", thongtin.NamQuyetToan.ToString()),
                                                                       new XElement("THANG_QT", thongtin.ThangQuyetToan.ToString()),
                                                                       new XElement("MA_LOAI_KCB", ((int)thongtin.MaLoaiKCB).ToString()),
                                                                       new XElement("MA_KHOA", thongtin.MaKhoa),
                                                                       new XElement("MA_CSKCB", thongtin.MaCSKCB),
                                                                       new XElement("MA_KHUVUC", thongtin.MaKhuVuc ?? ""),
                                                                       new XElement("MA_PTTT_QT", thongtin.MaPhauThuatQuocTe ?? ""),
                                                                       new XElement("CAN_NANG", thongtin.CanNang.ToString() == null ? "" : thongtin.CanNang.ToString()));
            if (goiBHYT)
            {
                //Tạo file xml1 convert baseString64
                string createXml1 = "Resource\\TaoXML1.xml";

                XDocument myXml1 = new XDocument(fileTongHopXML1);
                myXml1.Save(createXml1);

                var formatXML1ToStringBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(createXml1));
                DeleteFile(createXml1);

                data.Descendants("DANHSACHHOSO").Last().Add(new XElement("HOSO", new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", formatXML1ToStringBase64))));
            }
            else
            {
                data.Descendants("DANHSACHHOSO").Last().Add(new XElement("HOSO", new XElement("FILEHOSO",
                   new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", ""))));
                data.Descendants("NOIDUNGFILE").Last().Add(fileTongHopXML1);
            }

            data.Save(@pathTongHop);

            return sttXML1;
        }

        #region ADD DATA THUỐC CỦA HỆ THỐNG VÀO XML

        public int AddHoSoChiTietThuocVoXmlTongHop(List<HoSoChiTietThuoc> hoSoChiTietThuocs, string nameFile, string @pathTongHop, int sttXML2, bool goiBHYT)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();

            if (goiBHYT)
            {
                string tenFileXML2 = "Resource\\XML2_01824_XML2.xml";
                XDocument myXml1 = new XDocument(new XElement("DSACH_CHI_TIET_THUOC", ""));
                myXml1.Save(@tenFileXML2);
                XDocument dataXML2 = XDocument.Load(@tenFileXML2);
                for (int i = 0; i < hoSoChiTietThuocs.Count; i++)
                {
                    sttXML2 = sttXML2 + 1;
                    hoSoChiTietThuocs[i].STT = sttXML2;
                    hoSoChiTietThuocs[i].MaLienKet = hoSoChiTietThuocs[i].MaLienKet;
                    hoSoChiTietThuocs[i].SoLuong = Math.Round(hoSoChiTietThuocs[i].SoLuong, 3);
                    hoSoChiTietThuocs[i].DonGia = Math.Round(hoSoChiTietThuocs[i].DonGia, 3);
                    dataXML2.Descendants("DSACH_CHI_TIET_THUOC").Last().Add(DataXMLThuoc(hoSoChiTietThuocs[i]));
                }

                dataXML2.Save(@tenFileXML2);

                var formatXML2ToStringBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(tenFileXML2));
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                  new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", formatXML2ToStringBase64)));
                data.Save(@pathTongHop);

                DeleteFile(tenFileXML2);
            }
            else
            {
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                  new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", new XElement("DSACH_CHI_TIET_THUOC", ""))));

                for (int i = 0; i < hoSoChiTietThuocs.Count; i++)
                {
                    sttXML2 = sttXML2 + 1;
                    hoSoChiTietThuocs[i].STT = sttXML2;
                    hoSoChiTietThuocs[i].MaLienKet = hoSoChiTietThuocs[i].MaLienKet;
                    hoSoChiTietThuocs[i].SoLuong = Math.Round(hoSoChiTietThuocs[i].SoLuong, 3);
                    hoSoChiTietThuocs[i].DonGia = Math.Round(hoSoChiTietThuocs[i].DonGia, 3);

                    data.Descendants("DSACH_CHI_TIET_THUOC").Last().Add(DataXMLThuoc(hoSoChiTietThuocs[i]));
                }
                data.Save(@pathTongHop);
            }
            return sttXML2;
        }
        private XElement DataXMLThuoc(HoSoChiTietThuoc hoSoChiTietThuocs)
        {
            return new XElement("CHI_TIET_THUOC",
                            new XElement("MA_LK", hoSoChiTietThuocs.MaLienKet),
                            new XElement("STT", hoSoChiTietThuocs.STT),
                            new XElement("MA_THUOC", hoSoChiTietThuocs.MaThuoc),
                            new XElement("MA_NHOM", ((int)hoSoChiTietThuocs.MaNhom).ToString()),
                            //new XElement("TEN_THUOC", "![CDATA[" + hoSoChiTietThuocs.TenThuoc + "]]"),
                            new XElement("TEN_THUOC", hoSoChiTietThuocs.TenThuoc),
                            new XElement("DON_VI_TINH", hoSoChiTietThuocs.DonViTinh),
                            //new XElement("HAM_LUONG", hoSoChiTietThuocs.HamLuong == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietThuocs.HamLuong + "]]"),
                            new XElement("HAM_LUONG", hoSoChiTietThuocs.HamLuong == null ? "" : hoSoChiTietThuocs.HamLuong),
                            new XElement("DUONG_DUNG", hoSoChiTietThuocs.DuongDung == null ? "" : hoSoChiTietThuocs.DuongDung),
                            //new XElement("LIEU_DUNG", hoSoChiTietThuocs.LieuDung == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietThuocs.LieuDung + "]]"),
                            new XElement("LIEU_DUNG", hoSoChiTietThuocs.LieuDung == null ? "" : hoSoChiTietThuocs.LieuDung),
                            new XElement("SO_DANG_KY", hoSoChiTietThuocs.SoDangKy == null ? "" : hoSoChiTietThuocs.SoDangKy),
                            new XElement("TT_THAU", hoSoChiTietThuocs.ThongTinThau == null ? "" : hoSoChiTietThuocs.ThongTinThau),
                            new XElement("PHAM_VI", hoSoChiTietThuocs.PhamVi),
                            new XElement("MUC_HUONG", hoSoChiTietThuocs.MucHuong.ToString()),
                            new XElement("T_NGUONKHAC", ConvertSoTienBHYT(hoSoChiTietThuocs.TienNguonKhac)),
                            new XElement("T_BNTT", ConvertSoTienBHYT(hoSoChiTietThuocs.TienBenhNhanTuTra.GetValueOrDefault())),
                            new XElement("T_BHTT", ConvertSoTienBHYT(hoSoChiTietThuocs.TienBaoHiemThanhToan)),
                            new XElement("T_BNCCT", ConvertSoTienBHYT(hoSoChiTietThuocs.TienBenhNhanCungChiTra.GetValueOrDefault())),
                            new XElement("T_NGOAIDS", ConvertSoTienBHYT(hoSoChiTietThuocs.TienNgoaiDinhSuat)),
                            new XElement("SO_LUONG", String.Format("{0:0.###}", hoSoChiTietThuocs.SoLuong)),
                            new XElement("DON_GIA", String.Format("{0:0.###}", hoSoChiTietThuocs.DonGia)),
                            new XElement("TYLE_TT", hoSoChiTietThuocs.TyLeThanhToan.ToString()),
                            new XElement("THANH_TIEN", ConvertSoTienBHYT(hoSoChiTietThuocs.ThanhTien)),
                            new XElement("MA_KHOA", hoSoChiTietThuocs.MaKhoa),
                            new XElement("MA_BAC_SI", hoSoChiTietThuocs.MaBacSi),
                            new XElement("MA_BENH", hoSoChiTietThuocs.MaBenh),
                            new XElement("NGAY_YL", ConvertNgayGioXMl(hoSoChiTietThuocs.NgayYLenh)),
                            new XElement("MA_PTTT", ((int)hoSoChiTietThuocs.MaPhuongThucThanhToan).ToString())
                            );
        }

        #endregion

        #region ADD DATA DỊCH VỤ KỸ THUẬT CỦA HỆ THỐNG VÀO XML

        public int AddHoSoChiTietDVKTVoXMLTongHop(List<HoSoChiTietDVKT> hoSoChiTietDVKTs, string nameFile, string @pathTongHop, int sttXML3, bool goiBHYT)
        {
            XDocument data = XDocument.Load(@pathTongHop);
            XNamespace root = data.Root.GetDefaultNamespace();

            if (goiBHYT)
            {
                string tenFileXML3 = "Resource\\XML2_01824_XML3.xml";
                XDocument myXml1 = new XDocument(new XElement("DSACH_CHI_TIET_DVKT", ""));
                myXml1.Save(tenFileXML3);
                XDocument dataXML3 = XDocument.Load(tenFileXML3);
                for (int i = 0; i < hoSoChiTietDVKTs.Count; i++)
                {
                    sttXML3 = sttXML3 + 1;
                    hoSoChiTietDVKTs[i].STT = sttXML3;
                    hoSoChiTietDVKTs[i].MaLienKet = hoSoChiTietDVKTs[i].MaLienKet;
                    hoSoChiTietDVKTs[i].SoLuong = (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].SoLuong), 3));
                    hoSoChiTietDVKTs[i].DonGia = (decimal)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].DonGia), 3));
                    dataXML3.Descendants("DSACH_CHI_TIET_DVKT").Last().Add(DataXMLDVKT(hoSoChiTietDVKTs[i]));
                }
                dataXML3.Save(tenFileXML3);
                var formatXML3ToStringBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(tenFileXML3));
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                   new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", formatXML3ToStringBase64)));

                DeleteFile(tenFileXML3);

            }
            else
            {
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                   new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", new XElement("DSACH_CHI_TIET_DVKT", ""))));

                for (int i = 0; i < hoSoChiTietDVKTs.Count; i++)
                {
                    sttXML3 = sttXML3 + 1;
                    hoSoChiTietDVKTs[i].STT = sttXML3;
                    hoSoChiTietDVKTs[i].MaLienKet = hoSoChiTietDVKTs[i].MaLienKet;
                    hoSoChiTietDVKTs[i].SoLuong = (double)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].SoLuong), 3));
                    hoSoChiTietDVKTs[i].DonGia = (decimal)(Math.Round(Convert.ToDecimal(hoSoChiTietDVKTs[i].DonGia), 3));
                    data.Descendants("DSACH_CHI_TIET_DVKT").Last().Add(DataXMLDVKT(hoSoChiTietDVKTs[i]));
                }
            }

            data.Save(@pathTongHop);
            return sttXML3;
        }
        private XElement DataXMLDVKT(HoSoChiTietDVKT hoSoChiTietDVKTs)
        {
            return new XElement("CHI_TIET_DVKT",
                    new XElement("MA_LK", hoSoChiTietDVKTs.MaLienKet),
                    new XElement("STT", hoSoChiTietDVKTs.STT),
                    new XElement("MA_DICH_VU", hoSoChiTietDVKTs.MaDichVu == null ? "" : hoSoChiTietDVKTs.MaDichVu),
                    new XElement("MA_VAT_TU", hoSoChiTietDVKTs.MaVatTu == null ? "" : hoSoChiTietDVKTs.MaVatTu),
                    new XElement("MA_NHOM", ((int)hoSoChiTietDVKTs.MaNhom).ToString()),
                    new XElement("GOI_VTYT", hoSoChiTietDVKTs.GoiVatTuYTe == null ? "" : hoSoChiTietDVKTs.GoiVatTuYTe),
                    new XElement("TEN_VAT_TU", hoSoChiTietDVKTs.TenVatTu == null ? "" : hoSoChiTietDVKTs.TenVatTu),
                    //new XElement("TEN_DICH_VU", hoSoChiTietDVKTs.TenDichVu == null ? "![CDATA[]]" : "![CDATA[" + hoSoChiTietDVKTs.TenDichVu + "]]"),
                    new XElement("TEN_DICH_VU", new XCData(hoSoChiTietDVKTs.TenDichVu ?? "")),
                    new XElement("DON_VI_TINH", hoSoChiTietDVKTs.DonViTinh == null ? "" : hoSoChiTietDVKTs.DonViTinh),
                    new XElement("PHAM_VI", hoSoChiTietDVKTs.PhamVi),
                    new XElement("SO_LUONG", String.Format("{0:0.###}", hoSoChiTietDVKTs.SoLuong)),
                    new XElement("DON_GIA", String.Format("{0:0.###}", hoSoChiTietDVKTs.DonGia)),
                    new XElement("TT_THAU", hoSoChiTietDVKTs.ThongTinThau == null ? "" : hoSoChiTietDVKTs.ThongTinThau),
                    new XElement("TYLE_TT", hoSoChiTietDVKTs.TyLeThanhToan.ToString()),
                    new XElement("THANH_TIEN", ConvertSoTienBHYT(hoSoChiTietDVKTs.ThanhTien)),
                    new XElement("T_TRANTT", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienThanhToanToiDa)),
                    new XElement("MUC_HUONG", hoSoChiTietDVKTs.MucHuong.ToString()),
                    new XElement("T_NGUONKHAC", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienNguonKhac)),
                    new XElement("T_BNTT", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienBenhNhanTuTra.GetValueOrDefault())),
                    new XElement("T_BHTT", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienBaoHiemThanhToan)),
                    new XElement("T_BNCCT", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienBenhNhanCungChiTra.GetValueOrDefault())),
                    new XElement("T_NGOAIDS", ConvertSoTienBHYT(hoSoChiTietDVKTs.TienNgoaiDinhSuat)),
                    new XElement("MA_KHOA", hoSoChiTietDVKTs.MaKhoa == null ? "" : hoSoChiTietDVKTs.MaKhoa),
                    new XElement("MA_GIUONG", hoSoChiTietDVKTs.MaGiuong == null ? "" : hoSoChiTietDVKTs.MaGiuong),
                    new XElement("MA_BAC_SI", hoSoChiTietDVKTs.MaBacSi == null ? "" : hoSoChiTietDVKTs.MaBacSi.ToString()),
                    new XElement("MA_BENH", hoSoChiTietDVKTs.MaBenh == null ? "" : hoSoChiTietDVKTs.MaBenh),
                    new XElement("NGAY_YL", ConvertNgayGioXMl(hoSoChiTietDVKTs.NgayYLenh)),
                    new XElement("NGAY_KQ", hoSoChiTietDVKTs.NgayKetQua == null ? "" : ConvertNgayGioXMl(hoSoChiTietDVKTs.NgayKetQua)),
                    new XElement("MA_PTTT", ((int)hoSoChiTietDVKTs.MaPhuongThucThanhToan).ToString()));
        }

        #endregion

        #region ADD DATA CẬN LÂM SÀNG CỦA HỆ THỐNG VÀO XML

        public int AddHoSoCanLamSangVoXMLTongHop(List<HoSoCanLamSang> hoSoCanLamSangs, string nameFile, string @pathTongHop, int sttXML4, bool goiBHYT)
        {
            XDocument data = XDocument.Load(@pathTongHop);

            if (goiBHYT)
            {
                //Tạo file xml4 convert baseString64
                string tenFileXML4 = "Resource\\XML2_01824_XML4.xml";
                XDocument myXml1 = new XDocument(new XElement("DSACH_CHI_TIET_CLS", ""));
                myXml1.Save(tenFileXML4);
                XDocument dataXml4 = XDocument.Load(tenFileXML4);

                for (int i = 0; i < hoSoCanLamSangs.Count; i++)
                {
                    sttXML4 = sttXML4 + 1;
                    hoSoCanLamSangs[i].STT = sttXML4;
                    hoSoCanLamSangs[i].MaLienKet = hoSoCanLamSangs[i].MaLienKet;
                    dataXml4.Descendants("DSACH_CHI_TIET_CLS").Last().Add(DataXMLCLS(hoSoCanLamSangs[i]));
                }

                dataXml4.Save(tenFileXML4);
                var formatXML4ToStringBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(tenFileXML4));
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                   new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", formatXML4ToStringBase64)));
                DeleteFile(tenFileXML4);
            }
            else
            {
                XNamespace root = data.Root.GetDefaultNamespace();
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                        new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", new XElement("DSACH_CHI_TIET_CLS", ""))));

                for (int i = 0; i < hoSoCanLamSangs.Count; i++)
                {
                    sttXML4 = sttXML4 + 1;
                    hoSoCanLamSangs[i].STT = sttXML4;
                    hoSoCanLamSangs[i].MaLienKet = hoSoCanLamSangs[i].MaLienKet;
                    data.Descendants("DSACH_CHI_TIET_CLS").Last().Add(DataXMLCLS(hoSoCanLamSangs[i]));
                }
            }

            data.Save(@pathTongHop);

            return sttXML4;
        }
        private XElement DataXMLCLS(HoSoCanLamSang hoSoCanLamSang)
        {
            return new XElement("CHI_TIET_CLS",
                                new XElement("MA_LK", hoSoCanLamSang.MaLienKet),
                                new XElement("STT", hoSoCanLamSang.STT),
                                new XElement("MA_DICH_VU", hoSoCanLamSang.MaDichVu),
                                new XElement("MA_CHI_SO", hoSoCanLamSang.MaChiSo == null ? "" : hoSoCanLamSang.MaChiSo),
                                //new XElement("TEN_CHI_SO", hoSoCanLamSang.TenChiSo == null ? "![CDATA[]]" : "![CDATA[" + hoSoCanLamSang.TenChiSo + "]]"),
                                new XElement("TEN_CHI_SO", hoSoCanLamSang.TenChiSo == null ? "" : hoSoCanLamSang.TenChiSo),
                                new XElement("GIA_TRI", new XCData(hoSoCanLamSang.GiaTri ?? "")),
                                new XElement("MA_MAY", hoSoCanLamSang.MaMay == null ? "" : hoSoCanLamSang.MaMay),
                                new XElement("MO_TA", new XCData(hoSoCanLamSang.MoTa ?? "")),
                                new XElement("KET_LUAN", new XCData(hoSoCanLamSang.KetLuan ?? "")),
                                new XElement("NGAY_KQ", ConvertNgayGioXMl(hoSoCanLamSang.NgayKQ)));
        }

        #endregion

        #region ADD DATA DIỄN BIẾN BỆNH CỦA HỆ THỐNG VÀO XML

        public int AddHoSoChiTietDienBienBenhVoXMLTongHop(List<HoSoChiTietDienBienBenh> hoSoChiTietDienBienBenh, string nameFile, string @pathTongHop, int sttXML5, bool goiBHYT)
        {
            XDocument data = XDocument.Load(@pathTongHop);

            if (goiBHYT)
            {
                string tenFileXML5 = "Resource\\XML2_01824_XML5.xml";
                XDocument myXml5 = new XDocument(new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH", ""));
                myXml5.Save(tenFileXML5);
                XDocument dataXML5 = XDocument.Load(tenFileXML5);

                for (int i = 0; i < hoSoChiTietDienBienBenh.Count; i++)
                {
                    sttXML5 = sttXML5 + 1;
                    hoSoChiTietDienBienBenh[i].STT = sttXML5;
                    hoSoChiTietDienBienBenh[i].MaLienKet = hoSoChiTietDienBienBenh[i].MaLienKet;
                    dataXML5.Descendants("DSACH_CHI_TIET_DIEN_BIEN_BENH").Last().Add(DataXMLHoSoChiTietDienBienBenh(hoSoChiTietDienBienBenh[i]));
                }

                dataXML5.Save(tenFileXML5);
                var formatXML5ToStringBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(tenFileXML5));
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                   new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", formatXML5ToStringBase64)));
                DeleteFile(tenFileXML5);
            }
            else
            {
                XNamespace root = data.Root.GetDefaultNamespace();
                data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                        new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", new XElement("DSACH_CHI_TIET_DIEN_BIEN_BENH", ""))));
                for (int i = 0; i < hoSoChiTietDienBienBenh.Count; i++)
                {
                    sttXML5 = sttXML5 + 1;
                    hoSoChiTietDienBienBenh[i].STT = sttXML5;
                    data.Descendants("DSACH_CHI_TIET_DIEN_BIEN_BENH").Last().Add(DataXMLHoSoChiTietDienBienBenh(hoSoChiTietDienBienBenh[i]));
                }
            }


            data.Save(@pathTongHop);

            return sttXML5;
        }

        private XElement DataXMLHoSoChiTietDienBienBenh(HoSoChiTietDienBienBenh hoSoChiTietDienBienBenh)
        {
            return new XElement("CHI_TIET_DIEN_BIEN_BENH",
                                    new XElement("MA_LK", hoSoChiTietDienBienBenh.MaLienKet),
                                    new XElement("STT", hoSoChiTietDienBienBenh.STT),
                                    new XElement("DIEN_BIEN", new XCData(hoSoChiTietDienBienBenh.DienBien ?? "")),
                                    new XElement("HOI_CHAN", new XCData(hoSoChiTietDienBienBenh.HoiChuan ?? "")),
                                    new XElement("PHAU_THUAT", new XCData(hoSoChiTietDienBienBenh.PhauThuat ?? "")),
                                    new XElement("NGAY_YL", ConvertNgayGioXMl(hoSoChiTietDienBienBenh.NgayYLenh)));
        }
        #endregion

        #region File Mã Hóa stringBase64 

        private void ReMoveDataXML1(string path)
        {
            XDocument data = XDocument.Load(path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement luuThongTinbenhNhan = data.Descendants("TONG_HOP").LastOrDefault();
            luuThongTinbenhNhan.Element("MA_LK").Remove();
            luuThongTinbenhNhan.Element("STT").Remove();
            luuThongTinbenhNhan.Element("MA_BN").Remove();
            luuThongTinbenhNhan.Element("HO_TEN").Remove();
            luuThongTinbenhNhan.Element("NGAY_SINH").Remove();
            luuThongTinbenhNhan.Element("GIOI_TINH").Remove();
            luuThongTinbenhNhan.Element("DIA_CHI").Remove();
            luuThongTinbenhNhan.Element("MA_THE").Remove();
            luuThongTinbenhNhan.Element("MA_DKBD").Remove();
            luuThongTinbenhNhan.Element("GT_THE_TU").Remove();
            luuThongTinbenhNhan.Element("GT_THE_DEN").Remove();
            luuThongTinbenhNhan.Element("MIEN_CUNG_CT").Remove();
            luuThongTinbenhNhan.Element("TEN_BENH").Remove();
            luuThongTinbenhNhan.Element("MA_BENH").Remove();
            luuThongTinbenhNhan.Element("MA_BENHKHAC").Remove();
            luuThongTinbenhNhan.Element("MA_LYDO_VVIEN").Remove();
            luuThongTinbenhNhan.Element("MA_NOI_CHUYEN").Remove();
            luuThongTinbenhNhan.Element("MA_TAI_NAN").Remove();
            luuThongTinbenhNhan.Element("NGAY_VAO").Remove();
            luuThongTinbenhNhan.Element("NGAY_RA").Remove();
            luuThongTinbenhNhan.Element("SO_NGAY_DTRI").Remove();
            luuThongTinbenhNhan.Element("KET_QUA_DTRI").Remove();
            luuThongTinbenhNhan.Element("TINH_TRANG_RV").Remove();
            luuThongTinbenhNhan.Element("NGAY_TTOAN").Remove();
            luuThongTinbenhNhan.Element("T_THUOC").Remove();
            luuThongTinbenhNhan.Element("T_VTYT").Remove();
            luuThongTinbenhNhan.Element("T_TONGCHI").Remove();
            luuThongTinbenhNhan.Element("T_BNTT").Remove();
            luuThongTinbenhNhan.Element("T_BNCCT").Remove();
            luuThongTinbenhNhan.Element("T_BHTT").Remove();
            luuThongTinbenhNhan.Element("T_NGUONKHAC").Remove();
            luuThongTinbenhNhan.Element("T_NGOAIDS").Remove();
            luuThongTinbenhNhan.Element("NAM_QT").Remove();
            luuThongTinbenhNhan.Element("THANG_QT").Remove();
            luuThongTinbenhNhan.Element("MA_LOAI_KCB").Remove();
            luuThongTinbenhNhan.Element("MA_KHOA").Remove();
            luuThongTinbenhNhan.Element("MA_CSKCB").Remove();
            luuThongTinbenhNhan.Element("MA_KHUVUC").Remove();
            luuThongTinbenhNhan.Element("MA_PTTT_QT").Remove();
            luuThongTinbenhNhan.Element("CAN_NANG").Remove();
            data.Save(path);
        }
        private void DeleteFile(string path)
        {
            System.IO.File.Delete(@path);
        }
        private static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        private void AddFileDown(string name, string duLieu, string path)
        {

            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            XElement luuThongTinbenhNhan = data.Descendants("DOWNLOAD").LastOrDefault();
            data.Descendants("DOWNLOAD").Last().Add(new XElement(name, duLieu));
            data.Save(@path);
        }

        private void AddFileHoSoToHoSoDauTienTongHopXML(string duLieu, string nameFile, string path)
        {
            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DANHSACHHOSO").Last().Add(new XElement("HOSO", new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", duLieu))));

            data.Save(@path);

        }
        private void AddFileHoSoToHoSoTongHopXML(string duLieu, string nameFile, string path)
        {
            XDocument data = XDocument.Load(@path);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("HOSO").Last().Add(new XElement("FILEHOSO",
                    new XElement("LOAIHOSO", nameFile), new XElement("NOIDUNGFILE", duLieu)));

            data.Save(@path);

        }
        private void AddHoSoToDanhSachHoSoTongHopXML()
        {
            var pathChung = @"Resource\\TongHop.xml";
            XDocument data = XDocument.Load(pathChung);
            XNamespace root = data.Root.GetDefaultNamespace();
            data.Descendants("DANHSACHHOSO").Last().Add(new XElement("HOSO", ""));
            data.Save(pathChung);

        }
        private void ReplaceCharacter(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            text = text.Replace("!", "<!");
            text = text.Replace("]]", "]]>");
            System.IO.File.WriteAllText(path, text);
        }
        private void ReplaceCharacterAdd(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            text = text.Replace("<!", "!");
            text = text.Replace("]]>", "]]");
            System.IO.File.WriteAllText(path, text);
        }
        private void DeleteValue(string path, string name)
        {
            XDocument doc = XDocument.Load(path);
            var q = from node in doc.Descendants(name)
                    select node;
            q.ToList().ForEach(x => x.Remove());
            doc.Save(path);

        }


        #region Convert XML ngày sinh , ngày ra , ngày vào , giá trị từ , giá trị đến

        private string ConvertNgayToXML(DateTime? dtTime)
        {
            if (!dtTime.HasValue)
                return "";
            return dtTime.Value.ToString(FormatNgayBHYT);
            //var thang = ngaySinh?.Month > 10 ? ngaySinh?.Month.ToString() : "0" + ngaySinh?.Month;
            //var ngay = ngaySinh?.Day > 10 ? ngaySinh?.Day.ToString() : "0" + ngaySinh?.Day;
            //return ngaySinh?.Year.ToString() + thang + ngay;
        }

        private string ConvertNgayGioXMl(DateTime? dtTime)
        {
            if (!dtTime.HasValue)
                return "";
            return dtTime.Value.ToString(FormatNgayGioBHYT);
            //var gioNgayRaVao = dtTime?.Hour > 10 ? dtTime?.Hour.ToString() : "0" + dtTime?.Hour;
            //var phutNgayRaVao = dtTime?.Minute > 10 ? dtTime?.Minute.ToString() : "0" + dtTime?.Minute;
            //var ngayNgayRaVao = dtTime?.Day > 10 ? dtTime?.Day.ToString() : "0" + dtTime?.Day;
            //var thangNgayRaVao = dtTime?.Month > 10 ? dtTime?.Month.ToString() : "0" + dtTime?.Month;

            //return dtTime?.Year.ToString() + thangNgayRaVao + ngayNgayRaVao + gioNgayRaVao + phutNgayRaVao;
        }

        private string ConvertSoTienBHYT(decimal? soTien)
        {
            if (soTien == null)
                return "";
            return soTien.Value.ToString("0.##");
        }

        private string GetLieuDungBHYT(double? soLanTrenVien, double? lieuDungTrenNgay, string cachDung)
        {
            if(soLanTrenVien != null && lieuDungTrenNgay != null)
            {
                return $"{soLanTrenVien.Value.MathRoundNumber(0)} viên/lần * {lieuDungTrenNgay.Value.MathRoundNumber(0)} lần/ngày";
            }
            return cachDung;
        }

        #endregion

        #endregion

        #endregion
    }

    public class DienBienPhieuDieuTriGoiBaoHiemYTe
    {
        public string DienBien { get; set; }
    }

    public class KetQuaGuiCongBHYT
    {
        public string maKetQua { get; set; }

    }
}
