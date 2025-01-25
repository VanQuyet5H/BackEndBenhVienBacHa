using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DanhMucMarketing
{
    [ScopedDependency(ServiceType = typeof(IDanhMucMarketingService))]
    public class DanhMucMarketingService : MasterFileService<YeuCauGoiDichVu>, IDanhMucMarketingService
    {
        private readonly IRepository<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVuRepository;
        private readonly IRepository<BenhNhan> _benhNhanRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ICauHinhService _cauHinhService;

        private readonly IRepository<XuatKhoQuaTang> _xuatKhoQuaTangRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;

        private readonly IRepository<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuDichVuGiuongRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuatRepository;

        private readonly IRepository<NhapKhoQuaTangChiTiet> _nhapKhoQuaTangChiTietRepository;

        private readonly IRepository<XuatKhoQuaTangChiTiet> _xuatKhoQuaTangChiTietRepository;

        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<User> _userRepository;

        private readonly IRepository<YeuCauTiepNhan> _tiepNhanBenhNhanRepository;

        public DanhMucMarketingService(IRepository<YeuCauGoiDichVu> repository, IRepository<ChuongTrinhGoiDichVu> chuongTrinhGoiDichVuRepository
            , IRepository<BenhNhan> benhNhanRepository, IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository, ICauHinhService cauHinhService
            , IUserAgentHelper userAgentHelper, IRepository<XuatKhoQuaTang> xuatKhoQuaTangRepository, IRepository<NhapKhoQuaTangChiTiet> nhapKhoQuaTangChiTietRepository
            , IRepository<Template> templateRepository, IRepository<XuatKhoQuaTangChiTiet> xuatKhoQuaTangChiTietRepository,
            IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository,
            IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository,
            IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuGiuong> chuongTrinhGoiDichVuDichVuGiuongRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuDichVuKhamBenhRepository,
            IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuDichVuKyThuatRepository,
            IRepository<User> userRepository, IRepository<YeuCauTiepNhan> tiepNhanBenhNhanRepository) : base(repository)
        {
            _cauHinhService = cauHinhService;
            _chuongTrinhGoiDichVuRepository = chuongTrinhGoiDichVuRepository;
            _benhNhanRepository = benhNhanRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _userAgentHelper = userAgentHelper;
            _xuatKhoQuaTangRepository = xuatKhoQuaTangRepository;
            _templateRepository = templateRepository;
            _xuatKhoQuaTangChiTietRepository = xuatKhoQuaTangChiTietRepository;
            _nhapKhoQuaTangChiTietRepository = nhapKhoQuaTangChiTietRepository;
            _tiepNhanBenhNhanRepository = tiepNhanBenhNhanRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuDichVuGiuongRepository = chuongTrinhGoiDichVuDichVuGiuongRepository;
            _chuongTrinhGoiDichVuDichVuKhamBenhRepository = chuongTrinhGoiDichVuDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuDichVuKyThuatRepository = chuongTrinhGoiDichVuDichVuKyThuatRepository;
            _userRepository = userRepository;
        }


        public List<long> GetAllGoiTheoBenhNhan(long benhNhanId)
        {
            var yeuCauGoiDichVus = _benhNhanRepository.TableNoTracking.Include(c => c.YeuCauGoiDichVus)
                                            .Where(p => p.Id == benhNhanId).SelectMany(c => c.YeuCauGoiDichVus);
            return yeuCauGoiDichVus.Select(c => c.Id).ToList();
        }

        public string AllBangKeGoiDichVu(long yeuCauGoiDichVuId, string hostingName)
        {
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.GetById(yeuCauGoiDichVuId,
                x => x.Include(o => o.BenhNhan)
                    .Include(o => o.BenhNhanSoSinh)
                    .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                    .Include(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.NhanVienTuVan).ThenInclude(o => o.User)
                    .Include(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)

                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(c => c.DichVuKhamBenhBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(c => c.DichVuGiuongBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(c => c.DichVuKyThuatBenhVien).ThenInclude(c => c.NhomDichVuBenhVien)

                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhomDichVuBenhVien));


            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var nguoiLapPhieu = _userRepository.GetById(currentUserId).HoTen;

            var cauHinhPhieuThu = _cauHinhService.LoadSetting<CauHinhPhieuThu>();
            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("BangKeSuDungDVTrongGoi"));


            var dateItemChiPhis = string.Empty;
            int sttItem = 1;
            decimal tongGiaDichVu = 0M;

            if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any())
            {
                dateItemChiPhis += "<tr class='border'>" +
                                   "<td class='border' colspan='5'><strong>Khám bệnh</strong></td>" +
                                   "</tr>";

                foreach (var yeuCauKhamBenhGroup in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                            .GroupBy(o => new { o.DichVuKhamBenhBenhVienId, o.NhomGiaDichVuKhamBenhBenhVienId }))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border1'>" + sttItem + "</td>" +
                                       "<td class='border2'>" + yeuCauKhamBenhGroup.First().DichVuKhamBenhBenhVien.Ten + "</td>" +
                                       "<td class='border3' style='text-align: right'>" + Convert.ToDouble(yeuCauKhamBenhGroup.First().DonGiaSauChietKhau).ApplyFormatMoneyToDouble() + "</td>" +
                                       "<td class='border4' style='text-align: center;'>" + yeuCauKhamBenhGroup.Sum(o => o.SoLan) + "</td>" +
                                       "<td class='border5' style='text-align: right'>" + Convert.ToDouble(yeuCauKhamBenhGroup.First().DonGiaSauChietKhau * (decimal)yeuCauKhamBenhGroup.Sum(o => o.SoLan)).ApplyFormatMoneyToDouble() + "</td>" +
                                       "</tr>";
                    sttItem++;
                    tongGiaDichVu += yeuCauKhamBenhGroup.First().DonGiaSauChietKhau * (decimal)yeuCauKhamBenhGroup.Sum(o => o.SoLan);
                }
            }

            if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
            {
                dateItemChiPhis += "<tr class='border'>" +
                                   "<td class='border' colspan='5'><strong>Giường bệnh</strong></td>" +
                                   "</tr>";

                foreach (var yeuCauDichVuGiuongGroup in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.GroupBy(o => new { o.DichVuGiuongBenhVienId, o.NhomGiaDichVuGiuongBenhVienId }))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border1'>" + sttItem + "</td>" +
                                       "<td class='border2'>" + yeuCauDichVuGiuongGroup.First().DichVuGiuongBenhVien.Ten + "</td>" +
                                       "<td class='border3' style='text-align: right'>" + Convert.ToDouble(yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau).ApplyFormatMoneyToDouble() + "</td>" +
                                       "<td class='border4' style='text-align: center;'>" + yeuCauDichVuGiuongGroup.Sum(o => o.SoLan) + "</td>" +
                                       "<td class='border5' style='text-align: right'>" + Convert.ToDouble(yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLan)).ApplyFormatMoneyToDouble() + "</td>" +
                                       "</tr>";
                    sttItem++;
                    tongGiaDichVu += yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLan);
                }

            }

            if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any())
            {
                foreach (var nhomDichVuKyThuat in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                          .GroupBy(o => o.DichVuKyThuatBenhVien.NhomDichVuBenhVien))
                {
                    dateItemChiPhis += "<tr class='border'>" +
                                       "<td class='border' colspan='5'><strong>" + nhomDichVuKyThuat.First().DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten + "</strong></td>" +
                                       "</tr>";

                    foreach (var yeuCauDichVuKyThuatGroup in nhomDichVuKyThuat.GroupBy(o => new { o.DichVuKyThuatBenhVienId, o.NhomGiaDichVuKyThuatBenhVienId }))
                    {
                        dateItemChiPhis += "<tr class='border'>" +
                                           "<td class='border1'>" + sttItem + "</td>" +
                                           "<td class='border2'>" + yeuCauDichVuKyThuatGroup.First().DichVuKyThuatBenhVien.Ten + "</td>" +
                                           "<td class='border3' style='text-align: right'>" + Convert.ToDouble(yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau).ApplyFormatMoneyToDouble() + "</td>" +
                                           "<td class='border4' style='text-align: center;'>" + yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan) + "</td>" +
                                           "<td class='border5' style='text-align: right'>" + Convert.ToDouble(yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan)).ApplyFormatMoneyToDouble() + "</td>" +
                                           "</tr>";
                        sttItem++;
                        tongGiaDichVu += yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan);
                    }
                }

            }

            var benhNhan = yeuCauGoiDichVu.BenhNhanSoSinh ?? yeuCauGoiDichVu.BenhNhan;

            var data = new
            {
                TenBenhNhan = benhNhan.HoTen,
                NamSinh = benhNhan.NamSinh,
                GioiTinh = benhNhan.GioiTinh?.GetDescription(),
                DiaChi = benhNhan.DiaChiDayDu,
                TenGoi = yeuCauGoiDichVu.TenChuongTrinh,
                NgayDangKy = yeuCauGoiDichVu.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),
                NhanVienDangKy = yeuCauGoiDichVu.NhanVienChiDinh.User.HoTen,
                NoiDangKy = $"{yeuCauGoiDichVu.NoiChiDinh.KhoaPhong.Ten} - {yeuCauGoiDichVu.NoiChiDinh.Ten}",
                NhanVienTuVan = yeuCauGoiDichVu.NhanVienTuVan?.User.HoTen,
                DateItemChiPhis = dateItemChiPhis,
                NgayHienTai = DateTime.Now.ApplyFormatNgayThangNam(),

                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                TongGiaDichVu = Convert.ToDouble(tongGiaDichVu).ApplyFormatMoneyToDouble(),
                LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
                MaYTe = yeuCauGoiDichVu.BenhNhan.MaBN,
                BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(yeuCauGoiDichVu.BenhNhan.Id.ToString()),
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result?.Body, data);
            return content;
        }

        public virtual byte[] KetXuatGoiTheoBenhNhanExcel(long yeuCauGoiDichVuId)
        {
            using (var stream = new MemoryStream())
            {
                var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.GetById(yeuCauGoiDichVuId,
                x => x.Include(o => o.BenhNhan)
                    .Include(o => o.BenhNhanSoSinh)
                    .Include(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                    .Include(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
                    .Include(o => o.NhanVienTuVan).ThenInclude(o => o.User)
                    .Include(o => o.YeuCauKhamBenhs)
                    .Include(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)

                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(c => c.DichVuKhamBenhBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(c => c.DichVuGiuongBenhVien)
                    .Include(o => o.ChuongTrinhGoiDichVu).ThenInclude(c => c.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(c => c.DichVuKyThuatBenhVien).ThenInclude(c => c.NhomDichVuBenhVien)

                    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NhomDichVuBenhVien));


                var currentUserId = _userAgentHelper.GetCurrentUserId();
                var nguoiLapPhieu = _userRepository.GetById(currentUserId).HoTen;
                var dateItemChiPhis = string.Empty;
                var benhNhan = yeuCauGoiDichVu.BenhNhanSoSinh ?? yeuCauGoiDichVu.BenhNhan;
                var dateNow = DateTime.Now.ApplyFormatNgayThangNam();

                using (var xlPackage = new ExcelPackage(stream))
                {

                    var worksheet = xlPackage.Workbook.Worksheets.Add(yeuCauGoiDichVu.TenGoiDichVu);

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    //SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 25;
                    worksheet.Column(2).Width = 35;
                    worksheet.Column(3).Width = 25;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 25;

                    worksheet.DefaultColWidth = 7;

                    using (var range = worksheet.Cells["A2:E2"])
                    {
                        range.Worksheet.Cells["A2:E2"].Merge = true;
                        range.Worksheet.Cells["A2:E2"].Value = "BẢNG KÊ SỬ DỤNG DỊCH VỤ TRONG GÓI";
                        range.Worksheet.Cells["A2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A2:E2"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:E2"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:E2"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A3:B3"])
                    {
                        range.Worksheet.Cells["A3:B3"].Merge = true;
                        range.Worksheet.Cells["A3:B3"].Value = "Tên bệnh nhân: " + benhNhan.HoTen;
                        range.Worksheet.Cells["A3:B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A3:B3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A3:B3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A3:B3"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["C3:D3"])
                    {
                        range.Worksheet.Cells["D3"].Merge = true;
                        range.Worksheet.Cells["D3"].Value = "Năm sinh:" + benhNhan.NamSinh;
                        range.Worksheet.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["D3"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["D3:E3"])
                    {
                        range.Worksheet.Cells["E3"].Merge = true;
                        range.Worksheet.Cells["E3"].Value = "Giới tính: " + benhNhan.GioiTinh.GetDescription();
                        range.Worksheet.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["E3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E3"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["E3"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A4:E4"])
                    {
                        range.Worksheet.Cells["A4:E4"].Merge = true;
                        range.Worksheet.Cells["A4:E4"].Value = "Địa chỉ: " + benhNhan.DiaChiDayDu;
                        range.Worksheet.Cells["A4:E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A4:E4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A4:E4"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A4:E4"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A5:E5"])
                    {
                        range.Worksheet.Cells["A5:E5"].Merge = true;
                        range.Worksheet.Cells["A5:E5"].Value = "Gói đăng ký: " + yeuCauGoiDichVu.TenChuongTrinh;
                        range.Worksheet.Cells["A5:E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A5:E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A5:E5"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A5:E5"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A6:B6"])
                    {
                        range.Worksheet.Cells["A6:B6"].Merge = true;
                        range.Worksheet.Cells["A6:B6"].Value = "Ngày đăng ký: " + yeuCauGoiDichVu.ThoiDiemChiDinh.ApplyFormatNgayThangNam();
                        range.Worksheet.Cells["A6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A6:B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A6:B6"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A6:B6"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["C6:D6"])
                    {
                        range.Worksheet.Cells["C6:D6"].Merge = true;
                        range.Worksheet.Cells["C6:D6"].Value = "Nhân viên đăng ký: " + yeuCauGoiDichVu.NhanVienChiDinh.User.HoTen;
                        range.Worksheet.Cells["C6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C6:D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C6:D6"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["C6:D6"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["A7:B7"])
                    {
                        range.Worksheet.Cells["A7:B7"].Merge = true;
                        range.Worksheet.Cells["A7:B7"].Value = "Nhân viên tư vấn (chỉ định): " + yeuCauGoiDichVu.NhanVienTuVan?.User.HoTen;
                        range.Worksheet.Cells["A7:B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A7:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A7:B7"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A7:B7"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["C7:D7"])
                    {
                        range.Worksheet.Cells["C7:D7"].Merge = true;
                        range.Worksheet.Cells["C7:D7"].Value = "Nơi đăng ký: " + $"{yeuCauGoiDichVu.NoiChiDinh.KhoaPhong.Ten} - {yeuCauGoiDichVu.NoiChiDinh.Ten}";
                        range.Worksheet.Cells["C7:D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["C7:D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C7:D7"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["C7:D7"].Style.Font.Color.SetColor(Color.Black);

                    }

                    using (var range = worksheet.Cells["A8:E8"])
                    {
                        range.Worksheet.Cells["A8:E8"].Merge = true;
                        range.Worksheet.Cells["A8:E8"].Value = "Ghi chú: " + string.Empty;
                        range.Worksheet.Cells["A8:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A8:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:E8"].Style.Font.SetFromFont(new Font("Times New Roman", 13));
                        range.Worksheet.Cells["A8:E8"].Style.Font.Color.SetColor(Color.Black);
                    }


                    using (var range = worksheet.Cells["A9:E9"])
                    {
                        range.Worksheet.Cells["A9:E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A9:E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        range.Worksheet.Cells["A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A9"].Value = "STT";
                        range.Worksheet.Cells["A9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A9"].Style.Font.Bold = true;


                        range.Worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B9"].Value = "TÊN DỊCH VỤ";
                        range.Worksheet.Cells["B9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B9"].Style.Font.Bold = true;


                        range.Worksheet.Cells["C9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C9"].Value = "ĐƠN GIÁ";
                        range.Worksheet.Cells["C9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["C9"].Style.Font.Bold = true;

                        range.Worksheet.Cells["D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D9"].Value = "SỐ LƯỢNG";
                        range.Worksheet.Cells["D9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D9"].Style.Font.Bold = true;

                        range.Worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E9"].Value = "TỔNG TIỀN";
                        range.Worksheet.Cells["E9"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E9"].Style.Font.Bold = true;
                    }


                    var index = 10;
                    var STT = 1;
                    var tongCong = 0M;

                    if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any())
                    {
                        using (var range = worksheet.Cells["A" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        using (var range = worksheet.Cells["A" + index + ":B" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "KHÁM BỆNH";
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        index++;

                        foreach (var yeuCauKhamBenhGroup in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs
                                    .GroupBy(o => new { o.DichVuKhamBenhBenhVienId, o.NhomGiaDichVuKhamBenhBenhVienId }))
                        {
                            using (var range = worksheet.Cells["A" + index + ":E" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }

                            using (var range = worksheet.Cells["A" + index])
                            {
                                range.Worksheet.Cells["A" + index].Value = STT;
                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = yeuCauKhamBenhGroup.First().DichVuKhamBenhBenhVien.Ten;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = yeuCauKhamBenhGroup.First().DonGiaSauChietKhau;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = yeuCauKhamBenhGroup.Sum(c => c.SoLan);
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            }

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = yeuCauKhamBenhGroup.First().DonGiaSauChietKhau * yeuCauKhamBenhGroup.Sum(c => c.SoLan);
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            }
                            tongCong += yeuCauKhamBenhGroup.First().DonGiaSauChietKhau * yeuCauKhamBenhGroup.Sum(c => c.SoLan);
                            STT++;
                            index++;
                        }

                    }

                    if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any())
                    {
                        using (var range = worksheet.Cells["A" + index + ":E" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                        }
                        worksheet.Row(index).Height = 20.5;
                        using (var range = worksheet.Cells["A" + index + ":B" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":B" + index].Value = "GIƯỜNG";
                            range.Worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }

                        index++;

                        foreach (var yeuCauDichVuGiuongGroup in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs
                                   .GroupBy(o => new { o.DichVuGiuongBenhVienId, o.NhomGiaDichVuGiuongBenhVienId }))
                        {
                            using (var range = worksheet.Cells["A" + index + ":E" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }

                            using (var range = worksheet.Cells["A" + index])
                            {
                                range.Worksheet.Cells["A" + index].Value = STT;
                                range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["B" + index])
                            {
                                range.Worksheet.Cells["B" + index].Value = yeuCauDichVuGiuongGroup.First().DichVuGiuongBenhVien.Ten;
                                range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }

                            using (var range = worksheet.Cells["C" + index])
                            {
                                range.Worksheet.Cells["C" + index].Value = yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau;
                                range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                            }

                            using (var range = worksheet.Cells["D" + index])
                            {
                                range.Worksheet.Cells["D" + index].Value = yeuCauDichVuGiuongGroup.Sum(c => c.SoLan);
                                range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            }

                            using (var range = worksheet.Cells["E" + index])
                            {
                                range.Worksheet.Cells["E" + index].Value = yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLan);
                                range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                            }
                            tongCong += yeuCauDichVuGiuongGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuGiuongGroup.Sum(o => o.SoLan);
                            STT++;
                            index++;
                        }
                    }

                    if (yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any())
                    {
                        foreach (var nhomDichVuKyThuat in yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                                                  .GroupBy(o => o.DichVuKyThuatBenhVien.NhomDichVuBenhVien))
                        {

                            using (var range = worksheet.Cells["A" + index + ":E" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Bold = true;
                            }
                            worksheet.Row(index).Height = 20.5;
                            using (var range = worksheet.Cells["A" + index + ":B" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":B" + index].Merge = true;
                                range.Worksheet.Cells["A" + index + ":B" + index].Value = nhomDichVuKyThuat.First().DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten;
                                range.Worksheet.Cells["A" + index + ":B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            }

                            index++;

                            foreach (var yeuCauDichVuKyThuatGroup in nhomDichVuKyThuat.GroupBy(o => new { o.DichVuKyThuatBenhVienId, o.NhomGiaDichVuKyThuatBenhVienId }))
                            {
                                using (var range = worksheet.Cells["A" + index + ":E" + index])
                                {
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    range.Worksheet.Cells["A" + index + ":E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                }

                                using (var range = worksheet.Cells["A" + index])
                                {
                                    range.Worksheet.Cells["A" + index].Value = STT;
                                    range.Worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["B" + index])
                                {
                                    range.Worksheet.Cells["B" + index].Value = yeuCauDichVuKyThuatGroup.First().DichVuKyThuatBenhVien.Ten;
                                    range.Worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                }

                                using (var range = worksheet.Cells["C" + index])
                                {
                                    range.Worksheet.Cells["C" + index].Value = yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau;
                                    range.Worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    range.Worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
                                }

                                using (var range = worksheet.Cells["D" + index])
                                {
                                    range.Worksheet.Cells["D" + index].Value = yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan);
                                    range.Worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                }

                                using (var range = worksheet.Cells["E" + index])
                                {
                                    range.Worksheet.Cells["E" + index].Value = yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan);
                                    range.Worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

                                }

                                tongCong += yeuCauDichVuKyThuatGroup.First().DonGiaSauChietKhau * (decimal)yeuCauDichVuKyThuatGroup.Sum(o => o.SoLan);
                                STT++;
                                index++;
                            }

                        }
                    }

                    using (var range = worksheet.Cells["A" + index + ":E" + index])
                    {
                        range.Worksheet.Cells["A" + index + ":D" + index].Merge = true;
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["A" + index + ":D" + index].Value = "TỔNG CỘNG";
                        range.Worksheet.Cells["A" + index + ":D" + index].Style.Font.Bold = true;

                        range.Worksheet.Cells["E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Worksheet.Cells["E" + index].Value = tongCong;
                        range.Worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";
                        range.Worksheet.Cells["E" + index].Style.Font.Bold = true;
                    }

                    index += 2;
                    using (var range = worksheet.Cells["A" + index + ":E" + index])
                    {
                        range.Worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Value = "XÁC NHẬN CỦA NGƯỜI BỆNH";
                        range.Worksheet.Cells["B" + index].Style.Font.Bold = true;


                        range.Worksheet.Cells["D" + index + ":E" + index].Merge = true;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index + ":E" + index].Value = dateNow;

                        index++;
                        range.Worksheet.Cells["B" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["B" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["B" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B" + index].Value = "(Ký, ghi rõ họ tên)";

                        range.Worksheet.Cells["D" + index + ":E" + index].Merge = true;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.Bold = true;
                        range.Worksheet.Cells["D" + index + ":E" + index].Value = "NGƯỜI LẬP BẢNG KÊ";

                        index++;


                        range.Worksheet.Cells["D" + index + ":E" + index].Merge = true;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["D" + index + ":E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D" + index + ":E" + index].Value = "(Ký, ghi rõ họ tên)";
                    }


                    xlPackage.Save();
                }

                return stream.ToArray();

            }
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }

            var queryObject = new DanhSachMarketingSearchSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DanhSachMarketingSearchSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DangChoNhanTien == false && queryObject.DangChoThanhToan == false && queryObject.DaThanhToan == false)
            {
                queryObject.DangChoNhanTien = true;
                queryObject.DangChoThanhToan = true;
                queryObject.DaThanhToan = true;
            }

            var queryDangChoNhanTien = getData(Enums.TrangThaiThanhToan.ChuaThanhToan, queryInfo, queryObject, true);
            var queryDangChoThanhToan = getData(Enums.TrangThaiThanhToan.ChuaThanhToan, queryInfo, queryObject);
            var queryDaThanhToan = getData(Enums.TrangThaiThanhToan.DaThanhToan, queryInfo, queryObject);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachMarketingGridVo()).AsQueryable();
            var isHaveQuery = false;

            var lstIdChon = new List<long>();

            if (queryObject.DangChoNhanTien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoNhanTien);
                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    query = queryDangChoNhanTien;
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }
            if (queryObject.DangChoThanhToan)
            {
                if (isHaveQuery)
                {
                    var lstTemp = lstIdChon;
                    query = query.Concat(queryDangChoThanhToan.Where(p => !lstTemp.Contains(p.Id)));

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    var lstTemp = lstIdChon;

                    query = queryDangChoThanhToan.Where(p => !lstTemp.Contains(p.Id));
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }
            if (queryObject.DaThanhToan)
            {
                if (isHaveQuery)
                {
                    var lstTemp = lstIdChon;

                    query = query.Concat(queryDaThanhToan.Where(p => !lstTemp.Contains(p.Id)));

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    var lstTemp = lstIdChon;

                    query = queryDaThanhToan.Where(p => !lstTemp.Contains(p.Id));
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }

            //query = query.Distinct();
            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayTao desc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }
            else
            {
                query = query.OrderBy("TenBenhNhan asc");
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                //
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        private IQueryable<DanhSachMarketingGridVo> getData(Enums.TrangThaiThanhToan trangThaiThanhToan, QueryInfo queryInfo, DanhSachMarketingSearchSearch queryObject, bool choNhanTien = false)
        {
            var result = _benhNhanRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachMarketingGridVo()).AsQueryable();
            if (queryObject != null)
            {
                if (queryObject.TuNgay != null && queryObject.DenNgay == null)
                {
                    var tuNgay = queryObject.TuNgay.GetValueOrDefault();

                    //result = result.Where(p => p.yeu.Any(o => tuNgay <= o.NgayTao));
                    result = _benhNhanRepository.TableNoTracking
                        .Where(p => p.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == trangThaiThanhToan && (!choNhanTien || o.BoPhanMarketingDaNhanTien != true) && tuNgay <= o.CreatedOn))
                        .Select(s => new DanhSachMarketingGridVo
                        {
                            Id = s.Id,
                            ChungMinhThu = s.SoChungMinhThu,
                            DiaChi = s.DiaChiDayDu,
                            DienThoai = s.SoDienThoai,
                            DienThoaiDisplay = s.SoDienThoaiDisplay,
                            GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                            MaBenhNhan = s.MaBN,
                            NamSinh = s.NamSinh + "",
                            TenBenhNhan = s.HoTen,
                            NgayTaoDisplay = (s.CreatedOn ?? DateTime.Now).ApplyFormatDateTime(),
                            NgayTao = s.CreatedOn,
                            EnableDeleteButton = s.YeuCauGoiDichVus.All(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && x.BoPhanMarketingDaNhanTien != true),
                        });
                }
                else if (queryObject.DenNgay != null && queryObject.TuNgay == null)
                {
                    var denNgay = queryObject.DenNgay.GetValueOrDefault();
                    //result = result.Where(p => denNgay >= p.NgayTao);
                    result = _benhNhanRepository.TableNoTracking
                        .Where(p => p.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == trangThaiThanhToan && (!choNhanTien || o.BoPhanMarketingDaNhanTien != true) && denNgay >= o.CreatedOn))
                        .Select(s => new DanhSachMarketingGridVo
                        {
                            Id = s.Id,
                            ChungMinhThu = s.SoChungMinhThu,
                            DiaChi = s.DiaChiDayDu,
                            DienThoai = s.SoDienThoai,
                            DienThoaiDisplay = s.SoDienThoaiDisplay,
                            GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                            MaBenhNhan = s.MaBN,
                            NamSinh = s.NamSinh + "",
                            TenBenhNhan = s.HoTen,
                            NgayTaoDisplay = (s.CreatedOn ?? DateTime.Now).ApplyFormatDateTime(),
                            NgayTao = s.CreatedOn,
                            EnableDeleteButton = s.YeuCauGoiDichVus.All(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && x.BoPhanMarketingDaNhanTien != true)
                        });
                }
                else if (queryObject.DenNgay != null && queryObject.TuNgay != null)
                {
                    var tuNgay = queryObject.TuNgay.GetValueOrDefault();
                    var denNgay = queryObject.DenNgay.GetValueOrDefault();

                    result = _benhNhanRepository.TableNoTracking
                        .Where(p => p.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == trangThaiThanhToan && (!choNhanTien || o.BoPhanMarketingDaNhanTien != true) && tuNgay <= o.CreatedOn && denNgay >= o.CreatedOn))
                        .Select(s => new DanhSachMarketingGridVo
                        {
                            Id = s.Id,
                            ChungMinhThu = s.SoChungMinhThu,
                            DiaChi = s.DiaChiDayDu,
                            DienThoai = s.SoDienThoai,
                            DienThoaiDisplay = s.SoDienThoaiDisplay,
                            GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                            MaBenhNhan = s.MaBN,
                            NamSinh = s.NamSinh + "",
                            TenBenhNhan = s.HoTen,
                            NgayTaoDisplay = (s.CreatedOn ?? DateTime.Now).ApplyFormatDateTime(),
                            NgayTao = s.CreatedOn,
                            EnableDeleteButton = s.YeuCauGoiDichVus.All(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && x.BoPhanMarketingDaNhanTien != true)
                        });
                }
                else
                {
                    result = _benhNhanRepository.TableNoTracking
                        .Where(p => p.YeuCauGoiDichVus.Any(o => o.TrangThaiThanhToan == trangThaiThanhToan && (!choNhanTien || o.BoPhanMarketingDaNhanTien != true)))
                        .Select(s => new DanhSachMarketingGridVo
                        {
                            Id = s.Id,
                            ChungMinhThu = s.SoChungMinhThu,
                            DiaChi = s.DiaChiDayDu,
                            DienThoai = s.SoDienThoai,
                            DienThoaiDisplay = s.SoDienThoaiDisplay,
                            GioiTinh = s.GioiTinh != null ? s.GioiTinh.GetDescription() : "",
                            MaBenhNhan = s.MaBN,
                            NamSinh = s.NamSinh + "",
                            TenBenhNhan = s.HoTen,
                            NgayTaoDisplay = (s.CreatedOn ?? DateTime.Now).ApplyFormatDateTime(),
                            NgayTao = s.CreatedOn,
                            EnableDeleteButton = s.YeuCauGoiDichVus.All(x => x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan && x.BoPhanMarketingDaNhanTien != true)
                        });
                }
            }



            result = result.ApplyLike(queryInfo.SearchTerms, g => g.ChungMinhThu
                , g => g.DiaChi, g => g.DienThoai, g => g.DienThoaiDisplay, g => g.MaBenhNhan, g => g.NamSinh, g => g.TenBenhNhan);

            return result;
        }

        public async Task<GridDataSource> GetTotalForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new DanhSachMarketingSearchSearch();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<DanhSachMarketingSearchSearch>(queryInfo.AdditionalSearchString);
            }

            if (queryObject != null && queryObject.DangChoNhanTien == false && queryObject.DangChoThanhToan == false && queryObject.DaThanhToan == false)
            {
                queryObject.DangChoNhanTien = true;
                queryObject.DangChoThanhToan = true;
                queryObject.DaThanhToan = true;
            }

            var queryDangChoNhanTien = getData(Enums.TrangThaiThanhToan.ChuaThanhToan, queryInfo, queryObject, true);
            var queryDangChoThanhToan = getData(Enums.TrangThaiThanhToan.ChuaThanhToan, queryInfo, queryObject);
            var queryDaThanhToan = getData(Enums.TrangThaiThanhToan.DaThanhToan, queryInfo, queryObject);

            var query = _benhNhanRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new DanhSachMarketingGridVo()).AsQueryable();
            var isHaveQuery = false;
            var lstIdChon = new List<long>();
            if (queryObject.DangChoNhanTien)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDangChoNhanTien);
                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    query = queryDangChoNhanTien;
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }
            if (queryObject.DangChoThanhToan)
            {
                if (isHaveQuery)
                {
                    var lstTemp = lstIdChon;
                    query = query.Concat(queryDangChoThanhToan.Where(p => !lstTemp.Contains(p.Id)));

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    var lstTemp = lstIdChon;

                    query = queryDangChoThanhToan.Where(p => !lstTemp.Contains(p.Id));
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }
            if (queryObject.DaThanhToan)
            {
                if (isHaveQuery)
                {
                    var lstTemp = lstIdChon;

                    query = query.Concat(queryDaThanhToan.Where(p => !lstTemp.Contains(p.Id)));

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
                else
                {
                    var lstTemp = lstIdChon;

                    query = queryDaThanhToan.Where(p => !lstTemp.Contains(p.Id));
                    isHaveQuery = true;

                    lstIdChon = query.Select(p => p.Id).ToList();
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? idExcel, bool forExportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;

                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }

            long benhNhanId = 0;
            if (idExcel != null && idExcel != 0)
            {
                benhNhanId = idExcel ?? 0;
            }
            else
            {
                benhNhanId = long.Parse(queryInfo.AdditionalSearchString);
            }

            //long benhNhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var query = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.BenhNhanId == benhNhanId)
               .Select(s => new DanhSachMarketingChildGridVo
               {
                   Id = s.Id,
                   ChuongTrinhGoiMarketing = s.TenChuongTrinh,
                   TongTienTTDisplay = s.GiaSauChietKhau.ApplyFormatMoneyVND(),
                   TongTienTT = s.GiaSauChietKhau + "",
                   TrangThaiSuDung = s.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien ? "Chưa sử dụng" : s.TrangThai.GetDescription(),
                   TrangThaiTT = (s.BoPhanMarketingDaNhanTien != true && s.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan) ? "Đang chờ nhận tiền" : s.TrangThaiThanhToan.GetDescription(),
                   TrangThai = s.TrangThai,
                   TrangThaiThanhToan = s.TrangThaiThanhToan,
                   benhNhanId = benhNhanId,
                   chuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                   NgayDangKy = s.CreatedOn,
                   BoPhanMarketingDaNhanTien = s.BoPhanMarketingDaNhanTien,
               });

            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.ChuongTrinhGoiMarketing
            //    , g => g.TongTienTT);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                //
                var yc = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuQuaTangs)
                    .Include(p => p.XuatKhoQuaTangs).ThenInclude(p => p.XuatKhoQuaTangChiTiet)
                    .FirstOrDefault(p => p.Id == item.Id);
                var tongSoQua = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuQuaTangs.Count();
                var tongSoQuaDaXuat = yc.XuatKhoQuaTangs.SelectMany(p => p.XuatKhoQuaTangChiTiet).Select(p => p.QuaTangId).Distinct().Count();

                item.TrangThaiNhanQua = tongSoQuaDaXuat + "/" + tongSoQua;
                item.TongSoQua = tongSoQua;
                item.TongSoQuaDaXuat = tongSoQuaDaXuat;

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalForGridChildAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long benhNhanId = !string.IsNullOrEmpty(queryInfo.AdditionalSearchString) ? long.Parse(queryInfo.AdditionalSearchString) : 0;

            var query = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.BenhNhanId == benhNhanId)
               .Select(s => new DanhSachMarketingChildGridVo
               {
                   Id = s.Id,
                   ChuongTrinhGoiMarketing = s.TenChuongTrinh,
                   TongTienTTDisplay = s.GiaSauChietKhau.ApplyFormatMoneyVND(),
                   TongTienTT = s.GiaSauChietKhau + "",
                   TrangThaiSuDung = s.TrangThai.GetDescription(),
                   TrangThaiTT = s.TrangThaiThanhToan.GetDescription(),
                   TrangThai = s.TrangThai,
                   TrangThaiThanhToan = s.TrangThaiThanhToan
               });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataThongTinGoiDaHoanThanhForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var lstIdString = new List<long>();

            long benhNhanId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    lstIdString = JsonConvert.DeserializeObject<List<long>>(queryInfo.AdditionalSearchString.Split("|")[1]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => lstIdString.Contains(p.Id))
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten + " - " + s.TenGoiDichVu,
                        IsChecked = false,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,
                    });
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                if (_yeuCauGoiDichVuRepository.TableNoTracking.Any(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id))
                {
                    item.TrangThai = _yeuCauGoiDichVuRepository.TableNoTracking.First(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id).TrangThaiThanhToan;
                    item.TrangThaiDisplay = item.TrangThai.GetDescription();
                }
                //
                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber() + "%";
                //
                if (!item.IsChecked)
                {
                    if (lstIdString.Contains(item.Id))
                    {
                        item.IsChecked = true;
                    }
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalThongTinGoiDaHoanThanhPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var lstIdString = new List<long>();

            long benhNhanId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    lstIdString = JsonConvert.DeserializeObject<List<long>>(queryInfo.AdditionalSearchString.Split("|")[1]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => lstIdString.Contains(p.Id))
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        IsChecked = false,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau
                    });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataDichVuGoiForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            long chuongTrinhGoiDichVuId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;
            var query = _chuongTrinhGoiDichVuDichVuKhamBenhRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kb => new CacDichVuTrongGoiMarketingGridVo
                        {
                            Id = kb.Id,
                            Ma = kb.DichVuKhamBenhBenhVien.Ma,
                            Ten = kb.DichVuKhamBenhBenhVien.Ten,
                            NhomId = 1,
                            SoLan = kb.SoLan,
                            LoaiGia = kb.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            DonGiaTruocCK = kb.DonGiaTruocChietKhau,
                            DonGiaSauCK = kb.DonGiaSauChietKhau,
                        }).Union(
                         _chuongTrinhGoiDichVuDichVuKyThuatRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kt => new CacDichVuTrongGoiMarketingGridVo
                        {
                            Id = kt.Id,
                            Ma = kt.DichVuKyThuatBenhVien.Ma,
                            Ten = kt.DichVuKyThuatBenhVien.Ten,
                            NhomId = 2,
                            SoLan = kt.SoLan,
                            LoaiGia = kt.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGiaTruocCK = kt.DonGiaTruocChietKhau,
                            DonGiaSauCK = kt.DonGiaSauChietKhau,
                        }
                        )).Union(
                                _chuongTrinhGoiDichVuDichVuGiuongRepository.TableNoTracking
                                .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                                .Select(g => new CacDichVuTrongGoiMarketingGridVo
                                {
                                    Id = g.Id,
                                    Ma = g.DichVuGiuongBenhVien.Ma,
                                    Ten = g.DichVuGiuongBenhVien.Ten,
                                    NhomId = 3,
                                    SoLan = g.SoLan,
                                    LoaiGia = g.NhomGiaDichVuGiuongBenhVien.Ten,
                                    DonGiaTruocCK = g.DonGiaTruocChietKhau,
                                    DonGiaSauCK = g.DonGiaSauChietKhau,
                                }
                            ));
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDichVuGoiPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long chuongTrinhGoiDichVuId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;
            var query = _chuongTrinhGoiDichVuDichVuKhamBenhRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kb => new CacDichVuTrongGoiMarketingGridVo
                        {
                            Id = kb.Id,
                            Ma = kb.DichVuKhamBenhBenhVien.Ma,
                            Ten = kb.DichVuKhamBenhBenhVien.Ten,
                            NhomId = 1,
                            SoLan = kb.SoLan,
                            LoaiGia = kb.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            DonGiaTruocCK = kb.DonGiaTruocChietKhau,
                            DonGiaSauCK = kb.DonGiaSauChietKhau,
                        }).Union(
                         _chuongTrinhGoiDichVuDichVuKyThuatRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kt => new CacDichVuTrongGoiMarketingGridVo
                        {
                            Id = kt.Id,
                            Ma = kt.DichVuKyThuatBenhVien.Ma,
                            Ten = kt.DichVuKyThuatBenhVien.Ten,
                            NhomId = 2,
                            SoLan = kt.SoLan,
                            LoaiGia = kt.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGiaTruocCK = kt.DonGiaTruocChietKhau,
                            DonGiaSauCK = kt.DonGiaSauChietKhau,
                        }
                        )).Union(
                                _chuongTrinhGoiDichVuDichVuGiuongRepository.TableNoTracking
                                .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                                .Select(g => new CacDichVuTrongGoiMarketingGridVo
                                {
                                    Id = g.Id,
                                    Ma = g.DichVuGiuongBenhVien.Ma,
                                    Ten = g.DichVuGiuongBenhVien.Ten,
                                    NhomId = 3,
                                    SoLan = g.SoLan,
                                    LoaiGia = g.NhomGiaDichVuGiuongBenhVien.Ten,
                                    DonGiaTruocCK = g.DonGiaTruocChietKhau,
                                    DonGiaSauCK = g.DonGiaSauChietKhau,
                                }
                            ));
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<ICollection<LookupItemTemplateVo>> GetListGoiMarketing(DropDownListRequestModel queryInfo)
        {
            var dateTimeNow = DateTime.Now.Date;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new LookupItemTemplateVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();

            var lstIdString = new List<long>();
            long benhNhanId = 0;

            if (benhNhanId != 0)
            {
                var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId))
                   .Select(s => new LookupItemTemplateVo
                   {
                       KeyId = s.Id,
                       DisplayName = s.Ten + " - " + s.TenGoiDichVu,
                       Ma = s.Ma
                   });

                queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.Query, g => g.DisplayName);
                lstIdOfBenhNhan = await queryOfBenhNhan.Select(p => p.KeyId).ToListAsync();

                query = query.Concat(queryOfBenhNhan);
            }

            var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.BenhNhan == null && p.TamNgung != true && !lstIdOfBenhNhan.Contains(p.Id) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new LookupItemTemplateVo
                    {
                        KeyId = s.Id,
                        DisplayName = s.Ten + " - " + s.TenGoiDichVu,
                        Ma = s.Ma
                    });

            queryNormal = queryNormal.ApplyLike(queryInfo.Query, g => g.DisplayName);

            query = query.Concat(queryNormal);

            return query.ToList();
        }

        public List<ThongTinGoiMarketingGridVo> AddThongTinGoiMarketing(ChonGoiMarketing ChonGoiMarketing)
        {
            var dateTimeNow = DateTime.Now.Date;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentName = _userRepository.TableNoTracking.FirstOrDefault(c => c.Id == currentUserId).HoTen;
            var thongTinGoiMarketing = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == ChonGoiMarketing.GoiMarketingId && p.TuNgay.Date <= dateTimeNow
              && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
              && p.TamNgung != true)
                  .Select(s => new ThongTinGoiMarketingGridVo
                  {
                      Id = s.Id,
                      TenDisplay = s.Ten + " - " + s.TenGoiDichVu,
                      GoiSoSinh = s.GoiSoSinh,
                      GiaSauChietKhau = s.GiaSauChietKhau,
                      GiaTruocChietKhau = s.GiaTruocChietKhau,
                      GiaGoi = s.GiaSauChietKhau,
                      NgayDangKy = DateTime.Now,
                      NguoiDangKy = currentName,
                      IsChecked = true,
                  }).FirstOrDefault();

            var dataTheoSoLuongs = new List<ThongTinGoiMarketingGridVo>();
            if (ChonGoiMarketing.SoLuong > 0)
            {
                for (int i = 0; i < ChonGoiMarketing.SoLuong; i++)
                {
                    dataTheoSoLuongs.Add(thongTinGoiMarketing);
                }
            }
            return dataTheoSoLuongs;
        }

        public async Task<GridDataSource> GetThongGoiMRTBenhNhan(long benhNhanId)
        {
            var dateTimeNow = DateTime.Now.Date;
            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new ThongTinGoiMarketingGridVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();
            var lstIdString = new List<long>();
            if (benhNhanId != 0)
            {
                var goiBenhNhans = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.BenhNhanId == benhNhanId).Select(c => c.ChuongTrinhGoiDichVu)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten + " - " + s.TenGoiDichVu,
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,                        
                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,

                        IsHaveGift = s.ChuongTrinhGoiDichVuQuaTangs.Any(),
                        CoCacDichVuKhac = s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(),


                        DangDung = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauKhamBenhs.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                        && a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                        && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                   && a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                   && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0)))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                                                                                          && a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                          && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))

                    });
                query = query.Concat(goiBenhNhans);
            }

            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            var listYeuCauGoiDichVuIds = new List<long>();
            foreach (var item in queryTask.Result)
            {
                if (_yeuCauGoiDichVuRepository.TableNoTracking.Any(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id))
                {
                    var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking.Include(c => c.NhanVienChiDinh).ThenInclude(c => c.User)
                                                                    .First(p => !listYeuCauGoiDichVuIds.Contains(p.Id) &&
                                                                           p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id);

                    item.BenhNhanDaThanhToan = yeuCauGoiDichVu.SoTienBenhNhanDaChi ?? 0M;

                    item.TrangThai = yeuCauGoiDichVu.TrangThaiThanhToan;
                    item.TrangThaiDisplay = item.TrangThai.GetDescription();

                    item.NgayDangKy = yeuCauGoiDichVu.ThoiDiemChiDinh;
                    item.NguoiDangKy = yeuCauGoiDichVu.NhanVienChiDinh.User.HoTen;
                    item.TrangThaiGoi = yeuCauGoiDichVu.TrangThai;
                    item.GoiSoSinh = yeuCauGoiDichVu.GoiSoSinh;

                    listYeuCauGoiDichVuIds.Add(yeuCauGoiDichVu.Id);
                }
                //
                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber() + "%";
                //
                if (!item.IsChecked)
                {
                    if (lstIdString.Contains(item.Id))
                    {
                        item.IsChecked = true;
                    }
                }
                item.STT = stt;
                stt++;

            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }


        public async Task<GridDataSource> GetDataThongTinGoiForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new ThongTinGoiMarketingGridVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();

            var lstIdString = new List<long>();
            long benhNhanId = 0;


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    lstIdString = JsonConvert.DeserializeObject<List<long>>(queryInfo.AdditionalSearchString.Split("|")[1]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }

            if (benhNhanId != 0)
            {
                //var benhNhanId = JsonConvert.DeserializeObject<long>(queryInfo.AdditionalSearchString);
                var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId))
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten + " - " + s.TenGoiDichVu,
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,

                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,
                        BenhNhanDaThanhToan = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Select(a => a.SoTienBenhNhanDaChi ?? 0).FirstOrDefault(),

                        IsHaveGift = s.ChuongTrinhGoiDichVuQuaTangs.Any(),
                        CoCacDichVuKhac = s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()
                    });
                queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                lstIdOfBenhNhan = await queryOfBenhNhan.Select(p => p.Id).ToListAsync();

                query = query.Concat(queryOfBenhNhan);
            }

            var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => !lstIdOfBenhNhan.Contains(p.Id) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten + " - " + s.TenGoiDichVu,
                        IsChecked = false,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,

                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,
                        BenhNhanDaThanhToan = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Select(a => a.SoTienBenhNhanDaChi ?? 0).FirstOrDefault(),
                        IsHaveGift = s.ChuongTrinhGoiDichVuQuaTangs.Any(),
                        CoCacDichVuKhac = s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any() || s.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any()
                    });
            queryNormal = queryNormal.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            query = query.Concat(queryNormal);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                if (_yeuCauGoiDichVuRepository.TableNoTracking.Any(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id))
                {
                    item.TrangThai = _yeuCauGoiDichVuRepository.TableNoTracking.First(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id).TrangThaiThanhToan;
                    item.TrangThaiDisplay = item.TrangThai.GetDescription();
                }
                //
                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber() + "%";
                //
                if (!item.IsChecked)
                {
                    if (lstIdString.Contains(item.Id))
                    {
                        item.IsChecked = true;
                    }
                }
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<int> GetSoLuongDichVuTrongGoiDichVu(long benhNhanId, long ycgdvId, long dichVuId, string tenNhom, long yeuCauDichVuId)
        {
            var ycgdv = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.YeuCauDichVuKyThuats)
                .Include(p => p.YeuCauKhamBenhs)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)

                .FirstOrDefaultAsync(p => p.Id == ycgdvId);

            if (ycgdv == null)
            {
                return 0;
            }

            if (tenNhom == Constants.NhomDichVu.DichVuKhamBenh)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDungKhacYeuCauNay = ycgdv.YeuCauKhamBenhs.Where(p => p.Id != yeuCauDichVuId && p.DichVuKhamBenhBenhVienId == dichVuId).Count();
                return soLan - soLuongDaDungKhacYeuCauNay;
            }
            else if (tenNhom == Constants.NhomDichVu.DichVuKyThuat)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDungKhacYeuCauNay = ycgdv.YeuCauDichVuKyThuats.Where(p => p.Id != yeuCauDichVuId && p.DichVuKyThuatBenhVienId == dichVuId).Sum(p => p.SoLan);
                return soLan - soLuongDaDungKhacYeuCauNay;
            }
            else if (tenNhom == Constants.NhomDichVu.DichVuGiuong)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDungKhacYeuCauNay = ycgdv.YeuCauDichVuGiuongBenhViens.Where(p => p.Id != yeuCauDichVuId && p.DichVuGiuongBenhVienId == dichVuId).Count();
                return soLan - soLuongDaDungKhacYeuCauNay;
            }

            return 0;
        }

        public async Task<int> GetSoLuongConLaiOfYeuCauGoiDichVu(long benhNhanId, long ycgdvId, long dichVuId, string tenNhom)
        {
            var ycgdv = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.YeuCauDichVuKyThuats)
                .Include(p => p.YeuCauKhamBenhs)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)

                .FirstOrDefaultAsync(p => p.Id == ycgdvId);

            if (ycgdv == null)
            {
                return 0;
            }

            if (tenNhom == Constants.NhomDichVu.DichVuKhamBenh)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDung = ycgdv.YeuCauKhamBenhs.Where(p => p.DichVuKhamBenhBenhVienId == dichVuId).Count();

                return soLan - soLuongDaDung;
            }
            else if (tenNhom == Constants.NhomDichVu.DichVuKyThuat)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDung = ycgdv.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == dichVuId).Sum(p => p.SoLan);

                return soLan - soLuongDaDung;
            }
            else if (tenNhom == Constants.NhomDichVu.DichVuGiuong)
            {
                var soLan = ycgdv.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuId)?.SoLan ?? 0;
                var soLuongDaDung = ycgdv.YeuCauDichVuGiuongBenhViens.Where(p => p.DichVuGiuongBenhVienId == dichVuId).Count();

                return soLan - soLuongDaDung;
            }

            return 0;
        }

        public async Task<GridDataSource> GetTotalThongTinGoiPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new ThongTinGoiMarketingGridVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();

            var lstIdString = new List<long>();
            long benhNhanId = 0;


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    lstIdString = JsonConvert.DeserializeObject<List<long>>(queryInfo.AdditionalSearchString.Split("|")[1]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }

            if (benhNhanId != 0)
            {
                //var benhNhanId = JsonConvert.DeserializeObject<long>(queryInfo.AdditionalSearchString);
                var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,
                    });
                queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                lstIdOfBenhNhan = await queryOfBenhNhan.Select(p => p.Id).ToListAsync();

                query = query.Concat(queryOfBenhNhan);
            }

            var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => !lstIdOfBenhNhan.Contains(p.Id) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        IsChecked = false,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,
                    });
            queryNormal = queryNormal.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            query = query.Concat(queryNormal);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<ChuongTrinhGoiDichVu>> GetListChuongTrinhGoiCurrently()
        {
            var dateTimeNow = DateTime.Now.Date;
            var result = await _chuongTrinhGoiDichVuRepository.TableNoTracking

                .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                .Where(p => p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true).ToListAsync();

            return result;
        }

        public async Task<bool> IsExistsYeuCauGoiDichVuOfBenhNhan(long benhNhanId)
        {
            var dateTimeNow = DateTime.Now.Date;
            var yeuCauGoiDichVu = await BaseRepository.TableNoTracking.FirstOrDefaultAsync(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVu.TuNgay.Date <= dateTimeNow
                && (p.ChuongTrinhGoiDichVu.DenNgay == null || (p.ChuongTrinhGoiDichVu.DenNgay != null && p.ChuongTrinhGoiDichVu.DenNgay.Value.Date > dateTimeNow))
                && p.ChuongTrinhGoiDichVu.TamNgung != true);

            if (yeuCauGoiDichVu != null)
            {
                return true;
            }

            return false;

        }

        public async Task<GridDataSource> GetDataQuaTangGoiForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long childId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;

            long.TryParse(queryInfo.AdditionalSearchString, out long benhNhanId);

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .Include(p => p.ChuongTrinhGoiDichVuQuaTangs).ThenInclude(p => p.QuaTang).ThenInclude(p => p.NhapKhoQuaTangChiTiets)
                .SelectMany(p => p.ChuongTrinhGoiDichVuQuaTangs)
                .Include(p => p.QuaTang).ThenInclude(p => p.NhapKhoQuaTangChiTiets)
                .Select(s => new QuaTangGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ten = s.QuaTang.Ten,
                    QuaTangId = s.QuaTangId,
                    //YeuCauGoiDichVuId = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus
                    SoLuong = s.SoLuong,
                    SoLuongTon = s.QuaTang.NhapKhoQuaTangChiTiets.Any() ?
                        s.QuaTang.NhapKhoQuaTangChiTiets.Sum(p => p.SoLuongNhap) - s.QuaTang.NhapKhoQuaTangChiTiets.Sum(p => p.SoLuongDaXuat) : 0,
                    GhiChu = s.GhiChu,
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                //
                var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Include(p => p.XuatKhoQuaTangs).ThenInclude(p => p.XuatKhoQuaTangChiTiet)
                    .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == childId);

                item.SoPhieuXuat = yeuCauGoiDichVu?.XuatKhoQuaTangs.FirstOrDefault(p => p.XuatKhoQuaTangChiTiet.Any(x => x.QuaTangId == item.QuaTangId))?.SoPhieu;

                item.SoLuongDisplay = item.SoLuong.ApplyNumber();
                item.SoLuongTonDisplay = item.SoLuongTon.ApplyNumber();

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalQuaTangGoiPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long childId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuQuaTangs)
                .Select(s => new QuaTangGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ten = s.QuaTang.Ten,
                    SoLuong = s.SoLuong,
                    GhiChu = s.GhiChu,
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataCacDichVuTrongGoiForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var chuongTrinhGoiDichVuId = long.Parse(queryObj[0]);
            var isUpdate = bool.Parse(queryObj[1]);
            var query = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kb => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                        {
                            Id = kb.Id,
                            Ma = kb.DichVuKhamBenhBenhVien.Ma,
                            Ten = kb.DichVuKhamBenhBenhVien.Ten,
                            NhomId = 1,
                            SoLan = kb.SoLan,
                            LoaiGia = kb.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            DonGia = kb.DonGia,
                            DonGiaKhuyenMai = kb.DonGiaKhuyenMai,
                            GhiChu = kb.GhiChu,
                            SoNgaySuDung = kb.SoNgaySuDung,
                            HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(kb.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(kb.SoNgaySuDung, kb.CreatedOn),
                        }).Union(
                         _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kt => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                        {
                            Id = kt.Id,
                            Ma = kt.DichVuKyThuatBenhVien.Ma,
                            Ten = kt.DichVuKyThuatBenhVien.Ten,
                            NhomId = 2,
                            SoLan = kt.SoLan,
                            LoaiGia = kt.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGia = kt.DonGia,
                            DonGiaKhuyenMai = kt.DonGiaKhuyenMai,
                            GhiChu = kt.GhiChu,
                            SoNgaySuDung = kt.SoNgaySuDung,
                            HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(kt.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(kt.SoNgaySuDung, kt.CreatedOn),
                        }
                        )).Union(
                                _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository.TableNoTracking
                                .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                                .Select(g => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                                {
                                    Id = g.Id,
                                    Ma = g.DichVuGiuongBenhVien.Ma,
                                    Ten = g.DichVuGiuongBenhVien.Ten,
                                    NhomId = 3,
                                    SoLan = g.SoLan,
                                    LoaiGia = g.NhomGiaDichVuGiuongBenhVien.Ten,
                                    DonGia = g.DonGia,
                                    DonGiaKhuyenMai = g.DonGiaKhuyenMai,
                                    GhiChu = g.GhiChu,
                                    SoNgaySuDung = g.SoNgaySuDung,
                                    HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(g.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(g.SoNgaySuDung, g.CreatedOn),
                                }
                            ));
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalCacDichVuTrongGoiPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var chuongTrinhGoiDichVuId = long.Parse(queryObj[0]);
            var isUpdate = bool.Parse(queryObj[1]);
            var query = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kb => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                        {
                            Id = kb.Id,
                            Ma = kb.DichVuKhamBenhBenhVien.Ma,
                            Ten = kb.DichVuKhamBenhBenhVien.Ten,
                            NhomId = 1,
                            SoLan = kb.SoLan,
                            LoaiGia = kb.NhomGiaDichVuKhamBenhBenhVien.Ten,
                            DonGia = kb.DonGia,
                            DonGiaKhuyenMai = kb.DonGiaKhuyenMai,
                            GhiChu = kb.GhiChu,
                            SoNgaySuDung = kb.SoNgaySuDung,
                            HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(kb.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(kb.SoNgaySuDung, kb.CreatedOn),
                        }).Union(
                         _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking
                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                        .Select(kt => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                        {
                            Id = kt.Id,
                            Ma = kt.DichVuKyThuatBenhVien.Ma,
                            Ten = kt.DichVuKyThuatBenhVien.Ten,
                            NhomId = 2,
                            SoLan = kt.SoLan,
                            LoaiGia = kt.NhomGiaDichVuKyThuatBenhVien.Ten,
                            DonGia = kt.DonGia,
                            DonGiaKhuyenMai = kt.DonGiaKhuyenMai,
                            GhiChu = kt.GhiChu,
                            SoNgaySuDung = kt.SoNgaySuDung,
                            HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(kt.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(kt.SoNgaySuDung, kt.CreatedOn),
                        }
                        )).Union(
                                _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository.TableNoTracking
                                .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId)
                                .Select(g => new CacDichVuKhuyenMaiTrongGoiMarketingGridVo
                                {
                                    Id = g.Id,
                                    Ma = g.DichVuGiuongBenhVien.Ma,
                                    Ten = g.DichVuGiuongBenhVien.Ten,
                                    NhomId = 3,
                                    SoLan = g.SoLan,
                                    LoaiGia = g.NhomGiaDichVuGiuongBenhVien.Ten,
                                    DonGia = g.DonGia,
                                    DonGiaKhuyenMai = g.DonGiaKhuyenMai,
                                    GhiChu = g.GhiChu,
                                    SoNgaySuDung = g.SoNgaySuDung,
                                    HanSuDung = !isUpdate ? DateHelper.CalculateHanSuDung(g.SoNgaySuDung, null) : DateHelper.CalculateHanSuDung(g.SoNgaySuDung, g.CreatedOn),
                                }
                            ));
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<bool> CheckDichVuValidate(List<long> lstDaChon)
        {
            foreach (var id in lstDaChon)
            {
                var dateTimeNow = DateTime.Now.Date;
                var item = await _chuongTrinhGoiDichVuRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == id && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true);
                if (item == null)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<ChuongTrinhGoiDichVu> GetChuongTrinhGoiDichVu(long id)
        {
            var result = await _chuongTrinhGoiDichVuRepository.TableNoTracking
                .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                .FirstOrDefaultAsync(p => p.Id == id);

            return result;
        }

        public async Task<List<QuaTangDaXuat>> XuatQuaTang(long benhNhanId, long chuongTrinhGoiDichVuId)
        {
            var result = new List<QuaTangDaXuat>();

            var lstNhapKhoNeedUpdate = new List<DanhSachNhapKhoCanCapNhatSoLuong>();
            //var lstXuatKhoChiTiet = new List<XuatKhoQuaTangChiTiet>();

            var benhNhan = await _benhNhanRepository.GetByIdAsync(benhNhanId
                , p => p.Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.ChuongTrinhGoiDichVu)
                        .ThenInclude(o => o.ChuongTrinhGoiDichVuQuaTangs).ThenInclude(o => o.QuaTang).ThenInclude(o => o.NhapKhoQuaTangChiTiets)

                        .Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.ChuongTrinhGoiDichVu)
                        .ThenInclude(o => o.ChuongTrinhGoiDichVuQuaTangs).ThenInclude(o => o.QuaTang).ThenInclude(o => o.XuatKhoQuaTangChiTiet));

            var ct = benhNhan.YeuCauGoiDichVus.First(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId).ChuongTrinhGoiDichVu;
            var ycGoiId = benhNhan.YeuCauGoiDichVus.First(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId).Id;
            var dateTimeNow = DateTime.Now;

            var ctQuaTang = ct.ChuongTrinhGoiDichVuQuaTangs.ToList();

            var xuatKho = new XuatKhoQuaTang
            {
                NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
                BenhNhanId = benhNhanId,
                YeuCauGoiDichVuId = ycGoiId,
                NgayXuat = dateTimeNow,
            };

            foreach (var i in ctQuaTang)
            {
                //Check da xuat
                if (CheckQuaTangDaXuat(ycGoiId, i.QuaTangId))
                {
                    continue;
                }
                //Check hieu luc
                //if (i.QuaTang.HieuLuc != true)
                //{
                //    return false;
                //}
                //check sl ton
                if (i.QuaTang.NhapKhoQuaTangChiTiets.Sum(p => p.SoLuongNhap) - i.QuaTang.NhapKhoQuaTangChiTiets.Sum(p => p.SoLuongDaXuat) < i.SoLuong)
                {
                    continue;
                }

                var quaXuat = new QuaTangDaXuat
                {
                    QuaTangId = i.QuaTangId,
                    SoLuongDaXuat = i.SoLuong,
                };



                var soLuongDaXuat = 0;

                var lstNhapKho = i.QuaTang.NhapKhoQuaTangChiTiets.Where(p => p.SoLuongNhap - p.SoLuongDaXuat > 0).ToList();
                foreach (var nhapKho in lstNhapKho)
                {
                    if (i.SoLuong - soLuongDaXuat <= 0)
                    {
                        break;
                    }

                    var slTon = nhapKho.SoLuongNhap - nhapKho.SoLuongDaXuat;

                    var xkct = new XuatKhoQuaTangChiTiet();

                    if (slTon >= i.SoLuong - soLuongDaXuat)
                    {
                        xkct.SoLuongXuat = i.SoLuong - soLuongDaXuat;
                        soLuongDaXuat = i.SoLuong;
                    }
                    else
                    {
                        xkct.SoLuongXuat = slTon;
                        soLuongDaXuat = soLuongDaXuat + slTon;
                    }

                    //create new xuatkho
                    xkct.QuaTangId = i.QuaTangId;
                    xkct.NhapKhoQuaTangChiTietId = nhapKho.Id;

                    xuatKho.XuatKhoQuaTangChiTiet.Add(xkct);

                    //
                    var nk = new DanhSachNhapKhoCanCapNhatSoLuong
                    {
                        NhapKhoChiTietId = nhapKho.Id,
                        SoLuongXuat = xkct.SoLuongXuat,
                    };
                    lstNhapKhoNeedUpdate.Add(nk);
                }

                //lstXuatKhoChiTiet.Add(xuatKho);
                result.Add(quaXuat);
            }

            if (!xuatKho.XuatKhoQuaTangChiTiet.Any())
            {
                return null;
            }

            _xuatKhoQuaTangRepository.Add(xuatKho);

            result.First().XuatKhoId = xuatKho.Id;
            result.First().YeuCauGoiId = ycGoiId;

            UpdateCheckXuatTatCaQua(benhNhanId, chuongTrinhGoiDichVuId);

            UpdateNhapKhoQuaTang(lstNhapKhoNeedUpdate);

            return result;
        }


        public async Task<string> InPhieuXuat(long id, List<QuaTangDaXuat> quaDaXuat)
        {
            var content = string.Empty;
            var hearder = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuXuatQua"));

            var xuatKhoId = quaDaXuat.First().XuatKhoId;

            var data = await _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(x => x.Id == id)
                .Select(item => new ThongTinInXuatQuaVo()
                {
                    TenNguoiNhanHang = item.BenhNhan.HoTen,
                    BoPhan = "Marketing",
                    LyDoXuatKho = "Quà tặng kèm theo gói",
                    SoPhieuXuat = item.XuatKhoQuaTangs.First(p => p.Id == xuatKhoId).SoPhieu,
                    //XuatTaiKho = item.KhoVatTuXuat.Ten,
                    //DiaDiem = "",

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                }).FirstAsync();

            hearder = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                              "<th>PHIẾU XUẤT</th>" +
                         "</p>";

            var query = await _xuatKhoQuaTangChiTietRepository.TableNoTracking.Where(x => x.XuatKhoQuaTangId == xuatKhoId)
                .Select(s => new ThongTinInXuatQuaChiTietVo
                {
                    Ten = s.QuaTang.Ten,
                    DVT = s.QuaTang.DonViTinh,
                    SL = quaDaXuat.First(o => o.QuaTangId == s.QuaTangId).SoLuongDaXuat,
                    SLDisplay = quaDaXuat.First(o => o.QuaTangId == s.QuaTangId).SoLuongDaXuat.ApplyNumber(),
                })
                .ToListAsync();

            data.SoLuongTong = query.Sum(p => p.SL).ApplyNumber();
            //data.SoLuongYeuCauTong = query.Sum(p => p.SLYC).ApplyNumber();

            //var totalTenNhom = query.Select(p => p.TenNhom).Distinct().ToList();

            var info = string.Empty;

            var STT = 1;
            var queryNhom = query.ToList();
            foreach (var item in queryNhom)
            {
                info = info
                                       + "<tr style='border: 1px solid #020000;text-align: center; '>"
                                       + "<td style=''border: 1px solid #020000;text-align: center;'>" + STT
                                       + "<td style = 'border: 1px solid #020000;text-align: left;'>" + item.Ten
                                       + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DVT
                                       + "<td style = 'border: 1px solid #020000;text-align: right;'>" + item.SLDisplay
                                       + "</tr>";
                STT++;
            }

            data.Header = hearder;
            data.DanhSachThuoc = info;
            ;

            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        private bool CheckQuaTangDaXuat(long YeuCauGoiDichVuId, long QuaTangId)
        {
            var xuatKhoQuaTang = _xuatKhoQuaTangRepository.TableNoTracking
                .FirstOrDefault(p => p.YeuCauGoiDichVuId == YeuCauGoiDichVuId && p.XuatKhoQuaTangChiTiet.Any(x => x.QuaTangId == QuaTangId));

            return xuatKhoQuaTang != null;
        }

        private void UpdateNhapKhoQuaTang(List<DanhSachNhapKhoCanCapNhatSoLuong> lstDanhSach)
        {
            foreach (var item in lstDanhSach)
            {
                var nkct = _nhapKhoQuaTangChiTietRepository.GetById(item.NhapKhoChiTietId);
                nkct.SoLuongDaXuat = nkct.SoLuongDaXuat + item.SoLuongXuat;
                _nhapKhoQuaTangChiTietRepository.Update(nkct);
            }
        }

        private void UpdateCheckXuatTatCaQua(long benhNhanId, long chuongTrinhGoiDichVuId)
        {
            var ycGoiDV = _yeuCauGoiDichVuRepository.Table
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuQuaTangs)
                .Include(p => p.XuatKhoQuaTangs)
                .First(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == chuongTrinhGoiDichVuId);

            var lstQuaTang = ycGoiDV.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuQuaTangs.Select(p => p.QuaTangId).ToList();
            var daXuatHet = true;

            foreach (var qua in lstQuaTang)
            {
                if (!ycGoiDV.XuatKhoQuaTangs.Any(p => p.XuatKhoQuaTangChiTiet.Any(x => x.QuaTangId == qua)))
                {
                    daXuatHet = false;
                }
            }

            if (daXuatHet)
            {
                ycGoiDV.DaTangQua = true;
                _yeuCauGoiDichVuRepository.Update(ycGoiDV);
            }
        }

        public async Task<YeuCauTiepNhan> GetYCTNDangThucHienOfBenhNhan(long benhNhanId)
        {
            var yctn = await _tiepNhanBenhNhanRepository.TableNoTracking.FirstOrDefaultAsync(p => (p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
             || p.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.DangKyGoiMarketing) && p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
             && p.BenhNhanId == benhNhanId);

            return yctn;
        }
    }
}
