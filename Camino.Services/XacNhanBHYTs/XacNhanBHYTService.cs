using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.YeuCauKhamBenh;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.XacNhanBHYTs
{
    [ScopedDependency(ServiceType = typeof(IXacNhanBHYTService))]
    public class XacNhanBHYTService : MasterFileService<YeuCauTiepNhan>, IXacNhanBHYTService
    {
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<GiayChuyenVien> _giayChuyenVienRepository;
        private readonly IRepository<GiayMienCungChiTra> _giayMienCungChiTraRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Camino.Core.Domain.Entities.Users.User> _userRepository;
        private readonly IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        private readonly IRepository<DuyetBaoHiem> _duyetBaoHiemRepository;

        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;
        public XacNhanBHYTService
            (
                IRepository<Template> templateRepository,
                IRepository<YeuCauTiepNhan> repository,
                IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
                IRepository<Camino.Core.Domain.Entities.Users.User> userRepository,
                IRepository<GiayChuyenVien> giayChuyenVienRepository,
                IRepository<GiayMienCungChiTra> giayMienCungChiTraRepository,
                IRepository<DonThuocThanhToan> donThuocThanhToanRepository,
                IRepository<DuyetBaoHiem> duyetBaoHiemRepository,
                IUserAgentHelper userAgentHelper,
                IYeuCauKhamBenhService yeuCauKhamBenhService
            ) : base(repository)
        {
            _userRepository = userRepository;
            _userAgentHelper = userAgentHelper;
            _templateRepository = templateRepository;
            _benhVienRepository = benhVienRepository;
            _giayChuyenVienRepository = giayChuyenVienRepository;
            _giayMienCungChiTraRepository = giayMienCungChiTraRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _duyetBaoHiemRepository = duyetBaoHiemRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
        }

        public string GetNoiDKBD(string maDk)
        {
            var noiDk = _benhVienRepository.TableNoTracking.Where(p => p.Ma == maDk)
                .Select(p => p.Ten).FirstOrDefault();

            return noiDk;
        }

        public string GetGiayChuyenVien(long? id)
        {
            var giayChuyenVien = _giayChuyenVienRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.Ten).FirstOrDefault();

            return giayChuyenVien;
        }

        public string GetGiayMienCungChiTra(long? id)
        {
            var giayMienCungChiTra = _giayMienCungChiTraRepository.TableNoTracking.Where(p => p.Id == id)
                .Select(p => p.Ten).FirstOrDefault();

            return giayMienCungChiTra;
        }


        public async Task<string> PhieuLinhThuocBenhNhanXacNhanBHYT(long baoHiemYteId, string hostingName)
        {
            var result = await _templateRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Name.Equals("PhieuLinhThuocBHYT"));
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentUserName = await _userRepository.TableNoTracking.Where(p => p.Id == currentUserId).Select(p => p.HoTen).FirstOrDefaultAsync();

            var keToaThuoc = string.Empty;
            int idex = 1;

            var baoHiemYtes = await _duyetBaoHiemRepository.TableNoTracking.Where(c => c.Id == baoHiemYteId).Include(o => o.DuyetBaoHiemChiTiets)
                                                                                                            .ThenInclude(o => o.DonThuocThanhToanChiTiet)
                                                                                                            .ThenInclude(o => o.DonViTinh)
                                                                                                            .Include(o => o.DuyetBaoHiemChiTiets)
                                                                                                            .ThenInclude(o => o.DonThuocThanhToanChiTiet).ThenInclude(d => d.DuocPham).ThenInclude(d => d.DuocPhamBenhVien)
                                                                                                            .FirstOrDefaultAsync();
            var yeuCauTiepNhanId = baoHiemYtes.YeuCauTiepNhanId;

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId)
                                                                                           .Include(o => o.TinhThanh)
                                                                                           .Include(o => o.QuanHuyen)
                                                                                           .Include(o => o.PhuongXa)
                                                                                           .Include(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                                                                                           .Include(o => o.BenhNhan)
                                                                                           .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                                                                                           .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.ChanDoanSoBoICD).FirstOrDefaultAsync();

            var danhSachDonThuocBHYTBenhNhans = baoHiemYtes.DuyetBaoHiemChiTiets.Select(c => c.DonThuocThanhToanChiTiet);

            foreach (var danhSachDonThuocBHYTBenhNhan in danhSachDonThuocBHYTBenhNhans)
            {
                if (danhSachDonThuocBHYTBenhNhan == null) continue;
                keToaThuoc = keToaThuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        idex++
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        _yeuCauKhamBenhService.FormatTenDuocPham(danhSachDonThuocBHYTBenhNhan.Ten, danhSachDonThuocBHYTBenhNhan.HoatChat, danhSachDonThuocBHYTBenhNhan.HamLuong, danhSachDonThuocBHYTBenhNhan.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId)
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        danhSachDonThuocBHYTBenhNhan.HamLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: center;padding: 5px;'>" +
                                        danhSachDonThuocBHYTBenhNhan?.DonViTinh.Ten
                                        + "<td style = 'border: 1px solid #020000;text-align: right;padding: 5px;'>" +
                                        _yeuCauKhamBenhService.FormatSoLuong(danhSachDonThuocBHYTBenhNhan.SoLuong, danhSachDonThuocBHYTBenhNhan.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy);
            }

            var department = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiThucHien != null);
            var diagnose = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.ChanDoanSoBoICD != null);

            var departmentName = department?.NoiThucHien?.KhoaPhong?.Ten;
            var departmentCode = department?.NoiThucHien?.KhoaPhong?.Ma;

            var chanDoanVaMaBenh = diagnose?.ChanDoanSoBoGhiChu + " - mã bệnh :" + diagnose?.ChanDoanSoBoICD?.Ma;

            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = yeuCauTiepNhan.HoTen.ToUpper(),
                NguoiThuTien = currentUserName,
                KhoaPhongKham = departmentName != null ? departmentName + " - " + departmentCode : null,

                GioKham = department?.ThoiDiemThucHien?.Hour + " h " + department?.ThoiDiemThucHien?.Minute,
                NgayKham = department?.ThoiDiemThucHien?.ApplyFormatDate(),
               
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan?.NgaySinh, yeuCauTiepNhan?.ThangSinh, yeuCauTiepNhan?.NamSinh),
                ChanDoanVaMaBenh = chanDoanVaMaBenh,

                SoTheBhyt = yeuCauTiepNhan.BHYTMaSoThe,

                TuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ApplyFormatDate(),
                DenNgay = yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ApplyFormatDate(),

                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                CongKhoan = idex - 1,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                       DateTime.Now.Year,

                NguoiPhatThuoc = yeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                keToaThuoc = keToaThuoc,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }
        public async Task<string> PhieuLinhThuocBenhNhanBHYTTheoYCTN(long yeuCauTiepNhanId, string hostingName)
        {
            var result = await _templateRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Name.Equals("PhieuLinhThuocBHYT"));
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentUserName = await _userRepository.TableNoTracking.Where(p => p.Id == currentUserId).Select(p => p.HoTen).FirstOrDefaultAsync();

            var keToaThuoc = string.Empty;
            int idex = 1;

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(c => c.Id == yeuCauTiepNhanId)
                                                                                           .Include(o => o.TinhThanh)
                                                                                           .Include(o => o.QuanHuyen)
                                                                                           .Include(o => o.PhuongXa)
                                                                                           .Include(o => o.NhanVienTiepNhan).ThenInclude(o => o.User)
                                                                                           .Include(o => o.BenhNhan)
                                                                                           .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
                                                                                           .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.ChanDoanSoBoICD).FirstOrDefaultAsync();

            var danhSachDonThuocBHYTBenhNhans = await _donThuocThanhToanRepository.TableNoTracking.Include(x => x.YeuCauTiepNhan)
                                                                   .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT)
                                                                       .SelectMany(x => x.DonThuocThanhToanChiTiets).Include(o => o.DonViTinh).Include(d=>d.DuocPham).ThenInclude(d=>d.DuocPhamBenhVien).ToListAsync();

            foreach (var danhSachDonThuocBHYTBenhNhan in danhSachDonThuocBHYTBenhNhans)
            {
                if (danhSachDonThuocBHYTBenhNhan == null) continue;
                keToaThuoc = keToaThuoc + "<tr style='border: 1px solid #020000;'>"
                                        + "<td style='border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        idex++
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        _yeuCauKhamBenhService.FormatTenDuocPham(danhSachDonThuocBHYTBenhNhan.Ten,danhSachDonThuocBHYTBenhNhan.HoatChat,danhSachDonThuocBHYTBenhNhan.HamLuong, danhSachDonThuocBHYTBenhNhan.DuocPham.DuocPhamBenhVien.DuocPhamBenhVienPhanNhomId)
                                        + "<td style = 'border: 1px solid #020000;text-align: left;padding: 5px;'>" +
                                        danhSachDonThuocBHYTBenhNhan.HamLuong
                                        + "<td style = 'border: 1px solid #020000;text-align: center;padding: 5px;'>" +
                                        _yeuCauKhamBenhService.FormatSoLuong(danhSachDonThuocBHYTBenhNhan.SoLuong,danhSachDonThuocBHYTBenhNhan.DuocPham.DuocPhamBenhVien.LoaiThuocTheoQuanLy)
                                        + "<td style = 'border: 1px solid #020000;text-align: right;padding: 5px;'>" +
                                         danhSachDonThuocBHYTBenhNhan?.DonViTinh.Ten;
            }

            var department = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.NoiThucHien != null);
            var diagnose = yeuCauTiepNhan.YeuCauKhamBenhs.LastOrDefault(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && p.ChanDoanSoBoICD != null);

            var departmentName = department?.NoiThucHien?.KhoaPhong?.Ten;
            var departmentCode = department?.NoiThucHien?.KhoaPhong?.Ma;

            var chanDoanVaMaBenh = diagnose?.ChanDoanSoBoGhiChu + " - mã bệnh :" + diagnose?.ChanDoanSoBoICD?.Ma;

            var data = new
            {
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                NguoiNopTien = yeuCauTiepNhan.HoTen.ToUpper(),
                NguoiThuTien = currentUserName,
                KhoaPhongKham = departmentName != null ? departmentName + " - " + departmentCode : null,
                MaYTe = yeuCauTiepNhan.BenhNhan.MaBN,
                BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(yeuCauTiepNhan.BenhNhan.Id.ToString()),
                GioKham = department?.ThoiDiemThucHien?.Hour + " h " + department?.ThoiDiemThucHien?.Minute,
                NgayKham = department?.ThoiDiemThucHien?.ApplyFormatDate(),
                
                NamSinh = DateHelper.DOBFormat(yeuCauTiepNhan?.NgaySinh, yeuCauTiepNhan?.ThangSinh, yeuCauTiepNhan?.NamSinh),

                ChanDoanVaMaBenh = chanDoanVaMaBenh,

                SoTheBhyt = yeuCauTiepNhan.BHYTMaSoThe,

                TuNgay = yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ApplyFormatDate(),
                DenNgay = yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ApplyFormatDate(),

                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                CongKhoan = idex - 1,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
                                       DateTime.Now.Year,

                NguoiPhatThuoc = yeuCauTiepNhan.NhanVienTiepNhan?.User?.HoTen,
                keToaThuoc = keToaThuoc,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }


        public async Task<GiayMienCungChiTra> GetDocumentChoGiayMienCungChiTra(long id)
        {
            var giayMienCungChiTra = await _giayMienCungChiTraRepository.TableNoTracking
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            return giayMienCungChiTra;
        }

        public async Task<GiayChuyenVien> GetDocumentChoGiayChuyenVien(long id)
        {
            var giayChuyenVien =
                await _giayChuyenVienRepository.TableNoTracking.Where(p => p.Id == id).FirstOrDefaultAsync();
            return giayChuyenVien;
        }
    }
}
