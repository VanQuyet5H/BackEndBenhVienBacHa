using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiDuongThai;
using Camino.Core.Domain.ValueObject.GiayChungNhanNghiViecHuongBHXH;
using Camino.Core.Domain.ValueObject.GiayChungSinhMangThaiHo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Helpers;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.GoiBaoHiemYTe
{
    [ScopedDependency(ServiceType = typeof(IGoiBaoHiemYTeService))]
    public partial class GoiBaoHiemYTeService : MasterFileService<YeuCauTiepNhan>, IGoiBaoHiemYTeService
    {
        #region Giấy Ra Viện

        public List<GiayRaVienVo> GetThongTinGiayRaVien(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var khoaPhongChuyenKhoas = _khoaPhongChuyenKhoaRepository.TableNoTracking.Include(o => o.Khoa).ToList();
            var nhanViens = _nhanVienRepository.TableNoTracking.Include(o => o.User).ToList();
            var returnData = new List<GiayRaVienVo>();
            var yctnNoiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault()).ToList();
            var queryGiayRaVienVos = BaseRepository.TableNoTracking
                        .Where(d => yctnNoiTruIds.Contains(d.Id) && d.NoiTruBenhAn != null && d.CoBHYT == true)
                        .Select(yctn => new Camino.Core.Domain.ValueObject.ExcelChungTu.GiayRaVienVoData()
                        {
                            SoLuuTruGiayRaVien = yctn.NoiTruBenhAn.SoLuuTru,
                            MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan,
                            GiayRaVienTheBHYTDatas = yctn.YeuCauTiepNhanTheBHYTs.Select(t => new GiayRaVienTheBHYTData { NgayHieuLuc = t.NgayHieuLuc, MaSoThe = t.MaSoThe, BHYTDiaChi = t.DiaChi }).ToList(),
                            //MaThe = yctn.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                            //        ? yctn.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                            //            .OrderByDescending(a => a.MaSoThe).ThenBy(a => a.NgayHieuLuc)
                            //            .Select(a => a.MaSoThe).FirstOrDefault() : string.Empty,

                            MaYTe = yctn.BenhNhan.MaBN,
                            HoTen = yctn.HoTen,
                            //DiaChi = yctn.BHYTDiaChi,

                            NgaySinh = yctn.NgaySinh,
                            ThangSinh = yctn.ThangSinh,
                            NamSinh = yctn.NamSinh,

                            MaDanToc = yctn.DanToc != null ? yctn.DanToc.Ma : "",
                            LoaiGioiTinh = yctn.GioiTinh,
                            NgheNghiep = yctn.NgheNghiep != null ? yctn.NgheNghiep.Ten : "",

                            GiayRaVienJson = yctn.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(c => c.ThongTinHoSo).FirstOrDefault(),

                            NgayVao = yctn.NoiTruBenhAn.ThoiDiemNhapVien,
                            NgayRa = yctn.NoiTruBenhAn.ThoiDiemRaVien,

                            NguoiLienHeQuanHeNhanThanId = yctn.NguoiLienHeQuanHeNhanThanId,
                            NguoiLienHeHoTen = yctn.NguoiLienHeHoTen,

                            KhoaPhongNhapVienId = yctn.NoiTruBenhAn.KhoaPhongNhapVienId,
                            GiayRaVienKhoaPhongDieuTriDatas = yctn.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Select(k => new GiayRaVienKhoaPhongDieuTriData { KhoaPhongId = k.KhoaPhongChuyenDenId, ThoiDiemVaoKhoa = k.ThoiDiemVaoKhoa }).ToList(),

                            ThongTinBenhAn = yctn.NoiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || yctn.NoiTruBenhAn.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong ?
                                             yctn.NoiTruBenhAn.ThongTinBenhAn : string.Empty,
                            NgayCapCT = yctn.NoiTruHoSoKhacs.Where(c => c.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayRaVien).Select(x => x.ThoiDiemThucHien).FirstOrDefault()
                        }).ToList();

            foreach (var noiTru in queryGiayRaVienVos.Where(c => !string.IsNullOrEmpty(c.GiayRaVienJson)))
            {
                var theBHYTData = noiTru.GiayRaVienTheBHYTDatas.OrderBy(o => o.NgayHieuLuc).LastOrDefault();
                var maThe = theBHYTData?.MaSoThe ?? string.Empty;
                var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe, 10);
                var laTreEmDuoi7 = CalculateHelper.TinhTuoi(noiTru.NgaySinh, noiTru.ThangSinh, noiTru.NamSinh) < 7;
                var hoTenCha = laTreEmDuoi7 && noiTru.NguoiLienHeQuanHeNhanThanId == cauHinhBaoHiemYTe.QuanHeNhanThanChaDeId ? noiTru.NguoiLienHeHoTen : string.Empty;
                var hoTenMe = laTreEmDuoi7 && noiTru.NguoiLienHeQuanHeNhanThanId == cauHinhBaoHiemYTe.QuanHeNhanThanMeDeId ? noiTru.NguoiLienHeHoTen : string.Empty;


                if (!string.IsNullOrEmpty(noiTru.GiayRaVienJson))
                {
                    var giayRaVienJson = JsonConvert.DeserializeObject<GiayRaVien>(noiTru.GiayRaVienJson);
                    var thongTinBenhAn = !string.IsNullOrEmpty(noiTru.ThongTinBenhAn) ? JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTru.ThongTinBenhAn) : null;

                    var truongKhoa = nhanViens.Where(x => x.Id == giayRaVienJson.TruongKhoaId).FirstOrDefault();
                    var giamDoc = nhanViens.Where(x => x.Id == giayRaVienJson.GiamDocChuyenMonId).FirstOrDefault();

                    var khoaPhongRaVienId = noiTru.GiayRaVienKhoaPhongDieuTriDatas.OrderBy(o => o.ThoiDiemVaoKhoa).LastOrDefault()?.KhoaPhongId;

                    var itemData = new GiayRaVienVo()
                    {
                        SoLuuTruGiayRaVien = noiTru.SoLuuTruGiayRaVien,
                        MaCSKCB = noiTru.MaCSKCB,
                        MaThe = maThe,
                        MaSoBHXH = maSoBHXH,
                        HoTen = noiTru.HoTen,
                        DiaChi = theBHYTData?.BHYTDiaChi,

                        NgaySinh = DateHelper.DOBFormat(noiTru.NgaySinh, noiTru.ThangSinh, noiTru.NamSinh),
                        DanToc = GetDanTocBHYT(noiTru.MaDanToc),
                        GioiTinh = GetGioiTinhBHYT(noiTru.LoaiGioiTinh),
                        NgheNghiep = noiTru.NgheNghiep,

                        NgayRa = noiTru.NgayRa?.ApplyFormatDateTimeSACH(),
                        NgayVao = noiTru.NgayVao.ApplyFormatDateTimeSACH(),

                        HoTenCha = hoTenCha,
                        HoTenMe = hoTenMe,

                        SoSeRi = noiTru.MaYTe,
                        ChanDoan = giayRaVienJson.ChanDoan,
                        PhuongPhapDieuTri = giayRaVienJson.PhuongPhapDieuTri,
                        GhiChu = giayRaVienJson.GhiChu,

                        NguoiDaiDien = giamDoc != null ? giamDoc.User.HoTen : string.Empty,
                        MaTruongKhoa = truongKhoa != null ? truongKhoa.MaChungChiHanhNghe : string.Empty,

                        NgayCapCT = noiTru.NgayCapCT?.ToString("dd/MM/yyyy"),
                        NgayTaoChungTuDateTime = noiTru.NgayCapCT,
                        TenTruongKhoa = truongKhoa != null ? truongKhoa.User.HoTen : string.Empty,

                        TuoiThai = thongTinBenhAn?.TuoiThai,
                        MaKhoa = GetMaChuyenKhoa(khoaPhongRaVienId, khoaPhongChuyenKhoas),
                        TEKT = laTreEmDuoi7 ? "1" : string.Empty
                    };

                    returnData.Add(itemData);
                }
            }

            return returnData.OrderBy(o=>o.NgayTaoChungTuDateTime).ToList();
        }

        public virtual byte[] ExportGiayRaVien(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var datas = GetThongTinGiayRaVien(excelChungTuQueryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GiayRaVienVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("File excel mẫu giấy ra viện");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 20;
                    worksheet.Column(25).Width = 20;
                    worksheet.Column(26).Width = 20;


                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:Z1"])
                    {
                        range.Worksheet.Cells["A1:Z1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:Z1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:Z1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:Z1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_CT";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "MA_CSKCB";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "SO_SERI";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "MA_KHOA";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "MA_SOBHXH";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "MA_THE";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "HO_TEN";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "DIA_CHI";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "DAN_TOC";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "GIOI_TINH";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "NGHE_NGHIEP";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "NGAY_VAO";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "NGAY_RA";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "TUOI_THAI";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "CHAN_DOAN";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R1:R1"].Merge = true;
                        range.Worksheet.Cells["R1:R1"].Value = "PP_DIEUTRI";
                        range.Worksheet.Cells["R1:R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R1:R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S1:S1"].Merge = true;
                        range.Worksheet.Cells["S1:S1"].Value = "GHI_CHU";
                        range.Worksheet.Cells["S1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S1:S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T1:T1"].Merge = true;
                        range.Worksheet.Cells["T1:T1"].Value = "NGUOI_DAI_DIEN";
                        range.Worksheet.Cells["T1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T1:T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U1:U1"].Merge = true;
                        range.Worksheet.Cells["U1:U1"].Value = "MA_TRUONGKHOA";
                        range.Worksheet.Cells["U1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U1:U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V1:V1"].Merge = true;
                        range.Worksheet.Cells["V1:V1"].Value = "NGAY_CT";
                        range.Worksheet.Cells["V1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V1:V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W1:W1"].Merge = true;
                        range.Worksheet.Cells["W1:W1"].Value = "TEN_TRUONGKHOA";
                        range.Worksheet.Cells["W1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["X1:X1"].Merge = true;
                        range.Worksheet.Cells["X1:X1"].Value = "HO_TEN_CHA";
                        range.Worksheet.Cells["X1:X1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["X1:X1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Y1:Y1"].Merge = true;
                        range.Worksheet.Cells["Y1:Y1"].Value = "HO_TEN_ME";
                        range.Worksheet.Cells["Y1:Y1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Y1:Y1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["Z1:Z1"].Merge = true;
                        range.Worksheet.Cells["Z1:Z1"].Value = "TEKT";
                        range.Worksheet.Cells["Z1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Z1:Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<GiayRaVienVo>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":Z" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":Z" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":Z" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":Z" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.SoLuuTruGiayRaVien;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaCSKCB;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.SoSeRi;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.MaKhoa;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.MaSoBHXH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.MaThe;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.HoTen;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.DiaChi;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.NgaySinh;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.DanToc;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.GioiTinh;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.NgheNghiep;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.NgayVao;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.NgayRa;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.TuoiThai;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.ChanDoan;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.PhuongPhapDieuTri;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.GhiChu;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.NguoiDaiDien;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.MaTruongKhoa;

                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.NgayCapCT;


                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.TenTruongKhoa;

                                worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["X" + index].Value = item.HoTenCha;

                                worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Y" + index].Value = item.HoTenMe;

                                worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Z" + index].Value = item.TEKT;


                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        #endregion

        #region Tóm Tắt Bệnh Án

        public List<GiayTomTatBenhAn> GetThongGiayTomTatBenhAn(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var yctnNoiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault()).ToList();
            var returnData = new List<GiayTomTatBenhAn>();

            var noiTruHoSoKhacs = _noiTruHoSoKhacRepository.TableNoTracking
                          .Where(c => c.LoaiHoSoDieuTriNoiTru == Core.Domain.Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn && yctnNoiTruIds.Contains(c.YeuCauTiepNhanId) && c.YeuCauTiepNhan.NoiTruBenhAn != null)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.BenhNhan)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn).ThenInclude(c => c.ChanDoanChinhRaVienICD)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauNhapVien).ThenInclude(c => c.ChanDoanNhapVienICD)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauTiepNhanTheBHYTs)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.DanToc)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NgheNghiep).ToList();

            if (noiTruHoSoKhacs.Any())
            {
                foreach (var noiTruHoSoKhac in noiTruHoSoKhacs)
                {
                    if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo) && noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                    {

                        var tomTatHoSoBenhAn = JsonConvert.DeserializeObject<Camino.Core.Domain.ValueObject.DieuTriNoiTru.TomTatHoSoBenhAnVo>(noiTruHoSoKhac.ThongTinHoSo);

                        var giayTomTatBenhAnData = new GiayTomTatBenhAn();

                        giayTomTatBenhAnData.MaCT = string.Empty;
                        giayTomTatBenhAnData.MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan;
                        giayTomTatBenhAnData.SoSeRi = noiTruHoSoKhac.YeuCauTiepNhan.BenhNhan.MaBN;

                        var theBHYTData = noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.OrderBy(o => o.NgayHieuLuc).LastOrDefault();
                        var maThe = theBHYTData?.MaSoThe ?? string.Empty;
                        var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe , 10);

                        giayTomTatBenhAnData.MaThe = maThe;
                        giayTomTatBenhAnData.MaSoBHXH = maSoBHXH;

                        giayTomTatBenhAnData.HoTen = noiTruHoSoKhac.YeuCauTiepNhan.HoTen;
                        giayTomTatBenhAnData.NgaySinh = DateHelper.DOBFormat(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh);
                        //giayTomTatBenhAnData.GioiTinh = (int)noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh;
                        giayTomTatBenhAnData.GioiTinh = GetGioiTinhBHYT(noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh);
                        giayTomTatBenhAnData.DiaChi = theBHYTData?.DiaChi;
                        //giayTomTatBenhAnData.DanToc = int.Parse(noiTruHoSoKhac.YeuCauTiepNhan?.DanToc?.Ma);
                        giayTomTatBenhAnData.DanToc = GetDanTocBHYT(noiTruHoSoKhac.YeuCauTiepNhan.DanToc?.Ma);
                        giayTomTatBenhAnData.NgheNghiep = noiTruHoSoKhac.YeuCauTiepNhan.NgheNghiep?.Ten;

                        var laTreEmDuoi7 = CalculateHelper.TinhTuoi(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh) < 7;
                        var hoTenCha = laTreEmDuoi7 && noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId == cauHinhBaoHiemYTe.QuanHeNhanThanChaDeId ? noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeHoTen : string.Empty;
                        var hoTenMe = laTreEmDuoi7 && noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId == cauHinhBaoHiemYTe.QuanHeNhanThanMeDeId ? noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeHoTen : string.Empty;


                        giayTomTatBenhAnData.HoTenCha = hoTenCha;
                        giayTomTatBenhAnData.HoTenMe = hoTenMe;
                        giayTomTatBenhAnData.NguoiGiamHo = noiTruHoSoKhac.YeuCauTiepNhan.NguoiLienHeHoTen;
                        giayTomTatBenhAnData.TenDonVi = noiTruHoSoKhac.YeuCauTiepNhan.NoiLamViec;

                        giayTomTatBenhAnData.NgayVao = noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.ToString("yyyyMMddHHmm");
                        giayTomTatBenhAnData.NgayRa = noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null ? noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien?.ToString("yyyyMMddHHmm") : "";

                        giayTomTatBenhAnData.ChanDoanLucVaoVien = noiTruHoSoKhac.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD != null ?
                            MaskHelper.ICDDisplay(noiTruHoSoKhac.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.Ma,
                                                  noiTruHoSoKhac.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet,
                                                   noiTruHoSoKhac.YeuCauTiepNhan.YeuCauNhapVien.ChanDoanNhapVienGhiChu, Enums.KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu) : string.Empty;

                        giayTomTatBenhAnData.ChanDoanLucRaVien = noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienICD != null ?
                         MaskHelper.ICDDisplay(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma,
                                               noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet,
                                              noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu, Enums.KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu) : string.Empty;

                        giayTomTatBenhAnData.QuaTrinhBenhLyVaDienBienLamSang = tomTatHoSoBenhAn.DienBienLamSang;
                        giayTomTatBenhAnData.TomTatKetQuaXetNghiemCLS = tomTatHoSoBenhAn.KqXnCls;
                        giayTomTatBenhAnData.PhuongPhapDieuTri = tomTatHoSoBenhAn.PpDieuTri;
                        giayTomTatBenhAnData.NgaySinhCon = string.Empty;

                        if (!string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn))
                        {
                            var tongKetBenhAn = JsonConvert.DeserializeObject<ThongTinTreSoSinhVo>(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn);
                            if (tongKetBenhAn.DacDiemTreSoSinhs != null && tongKetBenhAn.DacDiemTreSoSinhs.Count > 0)
                            {
                                var lucDe = tongKetBenhAn.DacDiemTreSoSinhs.Select(c => c.DeLuc).FirstOrDefault()?.ToString("yyyy/MM/dd ");
                                giayTomTatBenhAnData.NgaySinhCon = lucDe;
                                giayTomTatBenhAnData.NgayChetCon = tongKetBenhAn.DacDiemTreSoSinhs.Any(c => c.TinhTrangId == Enums.EnumTrangThaiSong.Chet) ?
                                      tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.TinhTrangId == Enums.EnumTrangThaiSong.Chet).Select(d => d.DeLuc).FirstOrDefault()?.ToString("yyyy/MM/dd") : string.Empty;

                                giayTomTatBenhAnData.SoConChet = tongKetBenhAn.DacDiemTreSoSinhs.Any(c => c.TinhTrangId == Enums.EnumTrangThaiSong.Chet) ?
                                     tongKetBenhAn.DacDiemTreSoSinhs.Where(c => c.TinhTrangId == Enums.EnumTrangThaiSong.Chet).Count().ToString() : string.Empty;
                            }
                        }

                        //giayTomTatBenhAnData.TinhTrangRaVien = (int)noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.TinhTrangRaVien;
                        giayTomTatBenhAnData.TinhTrangRaVien = GetTinhTrangRaVienBHYT(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.TinhTrangRaVien);
                        giayTomTatBenhAnData.GhiChu = tomTatHoSoBenhAn.GhiChu;
                        giayTomTatBenhAnData.NguoiDaiDien = tomTatHoSoBenhAn.GiamDoc;
                        giayTomTatBenhAnData.NgayCapChungTu = noiTruHoSoKhac.ThoiDiemThucHien.ToString("yyyyMMdd");
                        giayTomTatBenhAnData.NgayTaoChungTuDateTime = noiTruHoSoKhac.ThoiDiemThucHien;
                        giayTomTatBenhAnData.TEKT = laTreEmDuoi7 ? "1" : "0";

                        returnData.Add(giayTomTatBenhAnData);
                    }
                }
            }

            return returnData.OrderBy(o=>o.NgayTaoChungTuDateTime).ToList();

        }

        public virtual byte[] ExportGiayTomTatBenhAn(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var datas = GetThongGiayTomTatBenhAn(excelChungTuQueryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GiayTomTatBenhAn>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("File excel mẫu tóm tắt bệnh án");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 20;
                    worksheet.Column(25).Width = 20;
                    worksheet.Column(26).Width = 20;

                    worksheet.Column(27).Width = 20;
                    worksheet.Column(28).Width = 20;
                    worksheet.Column(29).Width = 20;
                    worksheet.Column(30).Width = 20;
                    worksheet.Column(31).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:AE1"])
                    {
                        range.Worksheet.Cells["A1:AE1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:AE1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:AE1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:AE1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:AE1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:AE1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_CT";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "MA_CSKCB";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "SO_SERI";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "MA_BHXH";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "MA_THE";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "HO_TEN";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "GIOI_TINH";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "DAN_TOC";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "DIA_CHI";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "NGHE_NGHIEP";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "HO_TEN_CHA";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "HO_TEN_ME";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "NGUOI_GIAM_HO";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "TEN_DONVI";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "NGAY_VAO";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R1:R1"].Merge = true;
                        range.Worksheet.Cells["R1:R1"].Value = "NGAY_RA";
                        range.Worksheet.Cells["R1:R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R1:R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S1:S1"].Merge = true;
                        range.Worksheet.Cells["S1:S1"].Value = "CHAN_DOAN_VAO";
                        range.Worksheet.Cells["S1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S1:S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T1:T1"].Merge = true;
                        range.Worksheet.Cells["T1:T1"].Value = "CHAN_DOAN_RA";
                        range.Worksheet.Cells["T1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T1:T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U1:U1"].Merge = true;
                        range.Worksheet.Cells["U1:U1"].Value = "QT_BENHLY";
                        range.Worksheet.Cells["U1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U1:U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V1:V1"].Merge = true;
                        range.Worksheet.Cells["V1:V1"].Value = "TOMTAT_KQ";
                        range.Worksheet.Cells["V1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V1:V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W1:W1"].Merge = true;
                        range.Worksheet.Cells["W1:W1"].Value = "PP_DIEUTRI";
                        range.Worksheet.Cells["W1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["X1:X1"].Merge = true;
                        range.Worksheet.Cells["X1:X1"].Value = "NGAY_SINHCON";
                        range.Worksheet.Cells["X1:X1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["X1:X1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Y1:Y1"].Merge = true;
                        range.Worksheet.Cells["Y1:Y1"].Value = "NGAY_CHETCON";
                        range.Worksheet.Cells["Y1:Y1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Y1:Y1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Z1:Z1"].Merge = true;
                        range.Worksheet.Cells["Z1:Z1"].Value = "SO_CONCHET";
                        range.Worksheet.Cells["Z1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Z1:Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AA1:AA1"].Merge = true;
                        range.Worksheet.Cells["AA1:AA1"].Value = "TT_RAVIEN";
                        range.Worksheet.Cells["AA1:AA1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AA1:AA1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AB1:AB1"].Merge = true;
                        range.Worksheet.Cells["AB1:AB1"].Value = "GHI_CHU";
                        range.Worksheet.Cells["AB1:AB1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AB1:AB1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AC1:AC1"].Merge = true;
                        range.Worksheet.Cells["AC1:AC1"].Value = "NGUOI_DAI_DIEN";
                        range.Worksheet.Cells["AC1:AC1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AC1:AC1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AD1:AD1"].Merge = true;
                        range.Worksheet.Cells["AD1:AD1"].Value = "NGAY_CT";
                        range.Worksheet.Cells["AD1:AD1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AD1:AD1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AE1:AE1"].Merge = true;
                        range.Worksheet.Cells["AE1:AE1"].Value = "TEKT";
                        range.Worksheet.Cells["AE1:AE1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AE1:AE1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                    }



                    var manager = new PropertyManager<GiayTomTatBenhAn>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            //using (var range = worksheet.Cells["A" + index + ":AE1" + index])
                            //{
                            //    range.Worksheet.Cells["A" + index + ":AE1" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            //    range.Worksheet.Cells["A" + index + ":AE1" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            //    range.Worksheet.Cells["A" + index + ":AE1" + index].Style.Font.Color.SetColor(Color.Black);
                            //    range.Worksheet.Cells["A" + index + ":AE1" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //}
                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.MaCT;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.MaCSKCB;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.SoSeRi;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.MaSoBHXH;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.MaThe;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.HoTen;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.NgaySinh;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Value = item.GioiTinh;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Value = item.DanToc;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Value = item.DiaChi;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Value = item.NgheNghiep;

                            worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["M" + index].Value = item.HoTenCha;

                            worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["N" + index].Value = item.HoTenMe;

                            worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["O" + index].Value = item.NguoiGiamHo;

                            worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["P" + index].Value = item.TenDonVi;

                            worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Q" + index].Value = item.NgayVao;

                            worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["R" + index].Value = item.NgayRa;

                            worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["S" + index].Value = item.ChanDoanLucVaoVien;

                            worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["T" + index].Value = item.ChanDoanLucRaVien;

                            worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["U" + index].Value = item.QuaTrinhBenhLyVaDienBienLamSang;

                            worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["V" + index].Value = item.TomTatKetQuaXetNghiemCLS;

                            worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["W" + index].Value = item.PhuongPhapDieuTri;

                            worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["X" + index].Value = item.NgaySinhCon;

                            worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Y" + index].Value = item.NgayChetCon;

                            worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["Z" + index].Value = item.SoConChet;


                            worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AA" + index].Value = item.TinhTrangRaVien;

                            worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AB" + index].Value = item.GhiChu;

                            worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AC" + index].Value = item.NguoiDaiDien;

                            worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AD" + index].Value = item.NgayCapChungTu;

                            worksheet.Cells["AE" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["AE" + index].Value = item.TEKT;

                            index++;
                            stt++;
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }


        #endregion

        #region Giấy Nghỉ Hưởng BHXH

        public List<GiayNghiHuongBHXH> GetThongGiayNghiHuongBHXH(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var yctnNgoaiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId == null && o.YeuCauTiepNhanNgoaiTruId != null).Select(o => o.YeuCauTiepNhanNgoaiTruId.GetValueOrDefault()).ToList();
            var yctnNoiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault()).ToList();

            var yeuCauTiepNhanNgoaiTrus = BaseRepository.TableNoTracking.Where(d => yctnNgoaiTruIds.Contains(d.Id) && d.CoBHYT == true)
                .Include(c => c.YeuCauKhamBenhs).ThenInclude(c => c.BacSiThucHien).ThenInclude(x => x.User).Include(c => c.BenhNhan);

            var returnData = new List<GiayNghiHuongBHXH>();

            var noiTruHoSoKhacs = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(c => c.LoaiHoSoDieuTriNoiTru == Core.Domain.Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanNghiViecHuongBHXH && yctnNoiTruIds.Contains(c.YeuCauTiepNhanId) && c.YeuCauTiepNhan.NoiTruBenhAn != null)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn).ThenInclude(c => c.ChanDoanChinhRaVienICD)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauNhapVien).ThenInclude(c => c.ChanDoanNhapVienICD)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.BenhNhan)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauTiepNhanTheBHYTs)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.DanToc)
                           .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NgheNghiep).ToList();

            foreach (var yeuCauTiepNhan in yeuCauTiepNhanNgoaiTrus)
            {
                var giayNghiHuongBHXHData = new GiayNghiHuongBHXH();

                if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                    yeuCauTiepNhan.YeuCauKhamBenhs.Any(yckb => yckb.BaoHiemChiTra == true && yckb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && yckb.NghiHuongBHXHTuNgay != null && yckb.NghiHuongBHXHDenNgay != null))
                {
                    var ycKham = yeuCauTiepNhan.YeuCauKhamBenhs
                        .Where(yckb =>
                            yckb.BaoHiemChiTra == true && yckb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                            yckb.NghiHuongBHXHTuNgay != null && yckb.NghiHuongBHXHDenNgay != null)
                        .OrderBy(o => o.Id).Last();

                    var maThe = yeuCauTiepNhan.BHYTMaSoThe;                
                    var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe, 10);

                    giayNghiHuongBHXHData.MaCT = string.Empty;
                    giayNghiHuongBHXHData.MaBV = yeuCauTiepNhan.BHYTMaDKBD;
                    //giayNghiHuongBHXHData.SeRi = ;
                    giayNghiHuongBHXHData.MaThe = maThe;
                    giayNghiHuongBHXHData.MaBS = ycKham.BacSiThucHien?.MaChungChiHanhNghe;
                    giayNghiHuongBHXHData.MaSoBHXH = maSoBHXH;
                    giayNghiHuongBHXHData.HoTen = yeuCauTiepNhan.HoTen;
                    giayNghiHuongBHXHData.NgaySinh = DateHelper.DOBFormat(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);
                    giayNghiHuongBHXHData.GioiTinh = GetGioiTinhBHYT(yeuCauTiepNhan.GioiTinh);
                    giayNghiHuongBHXHData.PhuongPhapDieuTri = ycKham.GhiChuICDChinh;
                    giayNghiHuongBHXHData.MaDonVi = string.Empty;
                    giayNghiHuongBHXHData.TenDonVi = yeuCauTiepNhan.NoiLamViec;
                    giayNghiHuongBHXHData.TuNgay = ycKham.NghiHuongBHXHTuNgay.Value.ToString("dd/MM/yyyy");
                    giayNghiHuongBHXHData.DenNgay = ycKham.NghiHuongBHXHDenNgay.Value.ToString("dd/MM/yyyy");
                    giayNghiHuongBHXHData.SoNgay = (int)(ycKham.NghiHuongBHXHDenNgay.Value.Date - ycKham.NghiHuongBHXHTuNgay.Value.Date).TotalDays + 1;

                    giayNghiHuongBHXHData.HoTenCha = string.Empty;
                    giayNghiHuongBHXHData.HoTenMe = string.Empty;
                    giayNghiHuongBHXHData.NgayCT = ycKham.NghiHuongBHXHNgayIn?.ToString("dd/MM/yyyy");
                    giayNghiHuongBHXHData.NgayTaoChungTuDateTime = ycKham.NghiHuongBHXHNgayIn;
                    giayNghiHuongBHXHData.NguoiDaiDien = string.Empty;
                    giayNghiHuongBHXHData.TenBS = ycKham.BacSiThucHien?.User.HoTen;
                    giayNghiHuongBHXHData.MauSo = string.Empty;
                    giayNghiHuongBHXHData.SOKCB = yeuCauTiepNhan.BenhNhan.MaBN;
                    giayNghiHuongBHXHData.PhuongPhapDieuTri = ycKham.PhuongPhapDieuTriNghiHuongBHYT;

                    returnData.Add(giayNghiHuongBHXHData);
                }
            }

            if (noiTruHoSoKhacs.Any())
            {
                foreach (var noiTruHoSoKhac in noiTruHoSoKhacs)
                {
                    if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo) && noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                    {
                        //var benhVien = new Core.Domain.Entities.BenhVien.BenhVien();
                        //if (!String.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.BHYTMaDKBD))
                        //{
                        //    benhVien = _benhVienRepository.TableNoTracking
                        //                .FirstOrDefault(p => p.Ma.Equals(noiTruHoSoKhac.YeuCauTiepNhan.BHYTMaDKBD));
                        //}

                        var giayChungNhanNghiViecHuongBHXH = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiViecHuongBHXH>(noiTruHoSoKhac.ThongTinHoSo);

                        var giayNghiHuongBHXHData = new GiayNghiHuongBHXH();

                        var theBHYTData = noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.OrderBy(o => o.NgayHieuLuc).LastOrDefault();
                        var maThe = theBHYTData?.MaSoThe ?? string.Empty;                       
                        var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe, 10);


                        giayNghiHuongBHXHData.MaCT = string.Empty; //ToDo
                        //ToDo:
                        giayNghiHuongBHXHData.SOKCB = noiTruHoSoKhac.YeuCauTiepNhan.BenhNhan.MaBN;

                        giayNghiHuongBHXHData.MaBV = theBHYTData?.MaDKBD;
                        giayNghiHuongBHXHData.SeRi = giayChungNhanNghiViecHuongBHXH.SoSeri;
                        giayNghiHuongBHXHData.MaThe = maThe;
                        giayNghiHuongBHXHData.MaBS = giayChungNhanNghiViecHuongBHXH.MaHanhNgheKBCB;
                        giayNghiHuongBHXHData.MaSoBHXH = maSoBHXH;
                        giayNghiHuongBHXHData.HoTen = noiTruHoSoKhac.YeuCauTiepNhan.HoTen;
                        giayNghiHuongBHXHData.NgaySinh = DateHelper.DOBFormat(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh);
                        giayNghiHuongBHXHData.GioiTinh = GetGioiTinhBHYT(noiTruHoSoKhac.YeuCauTiepNhan.GioiTinh);
                        giayNghiHuongBHXHData.PhuongPhapDieuTri = giayChungNhanNghiViecHuongBHXH.ChanDoanVaPhuongPhapDieuTri;
                        giayNghiHuongBHXHData.MaDonVi = string.Empty;
                        giayNghiHuongBHXHData.TenDonVi = noiTruHoSoKhac.YeuCauTiepNhan.NoiLamViec;

                        var tuNgayDisplay = string.Empty;
                        var denNgayDisplay = string.Empty;
                        if (!string.IsNullOrEmpty(giayChungNhanNghiViecHuongBHXH.NghiTuNgayDisplay) && !string.IsNullOrEmpty(giayChungNhanNghiViecHuongBHXH.NghiDenNgayDisplay))
                        {

                            DateTime.TryParseExact(giayChungNhanNghiViecHuongBHXH.NghiTuNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                            //tuNgay = new DateTime(tuNgayTemp.Year, tuNgayTemp.Month, tuNgayTemp.Day, 0, 0, 0);
                            tuNgayDisplay = tuNgay.ToString("dd/MM/yyyy");

                            DateTime.TryParseExact(giayChungNhanNghiViecHuongBHXH.NghiDenNgayDisplay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime denNgay);
                            //denNgay = new DateTime(denNgayTemp.Year, denNgayTemp.Month, denNgayTemp.Day, 23, 59, 59);
                            denNgayDisplay = denNgay.ToString("dd/MM/yyyy");
                        }

                        giayNghiHuongBHXHData.TuNgay = tuNgayDisplay;
                        giayNghiHuongBHXHData.DenNgay = denNgayDisplay;
                        giayNghiHuongBHXHData.SoNgay = giayChungNhanNghiViecHuongBHXH.SoNgayNghi;

                        giayNghiHuongBHXHData.HoTenCha = giayChungNhanNghiViecHuongBHXH.HoTenCha;
                        giayNghiHuongBHXHData.HoTenMe = giayChungNhanNghiViecHuongBHXH.HoTenMe;
                        giayNghiHuongBHXHData.NgayCT = noiTruHoSoKhac.ThoiDiemThucHien.ToString("dd/MM/yyyy");
                        giayNghiHuongBHXHData.NgayTaoChungTuDateTime = noiTruHoSoKhac.ThoiDiemThucHien;
                        giayNghiHuongBHXHData.NguoiDaiDien = giayChungNhanNghiViecHuongBHXH.ThuTruongDonVi;
                        giayNghiHuongBHXHData.TenBS = giayChungNhanNghiViecHuongBHXH.NguoiHanhNgheKBCB;
                        giayNghiHuongBHXHData.MauSo = giayChungNhanNghiViecHuongBHXH.MauSo;

                        returnData.Add(giayNghiHuongBHXHData);
                    }
                }
            }

            return returnData.OrderBy(o=>o.NgayTaoChungTuDateTime).ToList();

        }

        public virtual byte[] ExportGiayNghiHuongBHXH(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var datas = GetThongGiayNghiHuongBHXH(excelChungTuQueryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GiayNghiHuongBHXH>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("File excel mẫu nghỉ hưởng BHXH");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:W1"])
                    {
                        range.Worksheet.Cells["A1:W1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:W1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:W1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:W1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_CT";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "SO_KCB";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "MA_BV";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "MA_BS";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "MA_SOBHXH";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "MA_THE";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "HO_TEN";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "GIOI_TINH";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "PP_DIEUTRI";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "MA_DVI";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "TEN_DVI";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "TU_NGAY";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "DEN_NGAY";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "SO_NGAY";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "HO_TEN_CHA";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R1:R1"].Merge = true;
                        range.Worksheet.Cells["R1:R1"].Value = "HO_TEN_ME";
                        range.Worksheet.Cells["R1:R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R1:R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S1:S1"].Merge = true;
                        range.Worksheet.Cells["S1:S1"].Value = "NGAY_CT";
                        range.Worksheet.Cells["S1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S1:S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T1:T1"].Merge = true;
                        range.Worksheet.Cells["T1:T1"].Value = "NGUOI_DAI_DIEN";
                        range.Worksheet.Cells["T1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T1:T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U1:U1"].Merge = true;
                        range.Worksheet.Cells["U1:U1"].Value = "TEN_BSY";
                        range.Worksheet.Cells["U1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U1:U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V1:V1"].Merge = true;
                        range.Worksheet.Cells["V1:V1"].Value = "SERI";
                        range.Worksheet.Cells["V1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V1:V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W1:W1"].Merge = true;
                        range.Worksheet.Cells["W1:W1"].Value = "MAU_SO";
                        range.Worksheet.Cells["W1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<GiayNghiHuongBHXH>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":W" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":W" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":W" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":W" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaCT;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.SOKCB;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaBV;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.MaBS;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.MaSoBHXH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.MaThe;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.HoTen;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.NgaySinh;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.GioiTinh;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.PhuongPhapDieuTri;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.MaDonVi;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.TenDonVi;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.TuNgay;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.DenNgay;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.SoNgay;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.HoTenCha;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.HoTenMe;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.NgayCT;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.NguoiDaiDien;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.TenBS;

                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.SeRi;

                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.MauSo;

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }


        #endregion

        #region Giấy Chứng Sinh

        public List<GiayChungSinhVo> GetThongGiayChungSinh(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var benhVien = _benhVienRepository.TableNoTracking.FirstOrDefault(p => p.Ma == cauHinhBaoHiemYTe.BenhVienTiepNhan);
            var nhanViens = _nhanVienRepository.TableNoTracking.Include(o => o.User).ToList();
            var yctnNoiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault()).ToList();
            var returnData = new List<GiayChungSinhVo>();

            var noiTruHoSoKhacs = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(c => c.LoaiHoSoDieuTriNoiTru == Core.Domain.Enums.LoaiHoSoDieuTriNoiTru.GiayChungSinh && yctnNoiTruIds.Contains(c.YeuCauTiepNhanId) && c.YeuCauTiepNhan.NoiTruBenhAn != null)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NoiTruBenhAn).ThenInclude(c => c.ChanDoanChinhRaVienICD)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauNhapVien).ThenInclude(c => c.ChanDoanNhapVienICD)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.BenhNhan)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauTiepNhanTheBHYTs)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.DanToc)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NgheNghiep).ToList();

            if (noiTruHoSoKhacs.Any())
            {
                foreach (var noiTruHoSoKhac in noiTruHoSoKhacs)
                {
                    if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo) && noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                    {

                        var thongTinGiayChungSinh = JsonConvert.DeserializeObject<Core.Domain.ValueObject.DieuTriNoiTru.GiayChungSinhNewJSONVo>(noiTruHoSoKhac.ThongTinHoSo);
                        var giayChungSinhVo = new GiayChungSinhVo();

                        giayChungSinhVo.MaCT = string.Empty;
                        giayChungSinhVo.MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan;

                        var theBHYTData = noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.OrderBy(o => o.NgayHieuLuc).LastOrDefault();
                        var maThe = theBHYTData?.MaSoThe ?? string.Empty;
                        var maThes = maThe.ToArray();
                        var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe, 10);

                        giayChungSinhVo.MaThe = maThe;

                        giayChungSinhVo.SoSeRi = noiTruHoSoKhac.YeuCauTiepNhan.BenhNhan.MaBN;
                        giayChungSinhVo.MaSoBHXHMe = maSoBHXH;
                        giayChungSinhVo.HoTenMe = noiTruHoSoKhac.YeuCauTiepNhan.HoTen;
                        giayChungSinhVo.NgaySinh = DateHelper.DOBFormat(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh);

                        giayChungSinhVo.DiaChi = theBHYTData?.DiaChi;
                        giayChungSinhVo.CMND = thongTinGiayChungSinh.CMND;
                        giayChungSinhVo.NgayCapCMND = thongTinGiayChungSinh.NgayCap?.ToString("dd/MM/yyyy");
                        giayChungSinhVo.NoiCapCMND = thongTinGiayChungSinh.NoiCap;
                        giayChungSinhVo.DanToc = GetDanTocBHYT(noiTruHoSoKhac.YeuCauTiepNhan.DanToc?.Ma);
                        giayChungSinhVo.HoTenCha = thongTinGiayChungSinh.HoVaTenCha;

                        giayChungSinhVo.NgaySinhCon = thongTinGiayChungSinh.ThoiGianDe?.ToString("dd/MM/yyyy HH:mm");
                        giayChungSinhVo.NoiSinhCon = benhVien?.Ten;
                        giayChungSinhVo.TenCon = thongTinGiayChungSinh.DuDinhDatTenCon;
                        if (!string.IsNullOrEmpty(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn))
                        {
                            var thongtinDacDiemTreSoSinhs = JsonConvert.DeserializeObject<DieuTriNoiTruTongKetBenhAnKhoaSanGrid>(noiTruHoSoKhac.YeuCauTiepNhan.NoiTruBenhAn.ThongTinTongKetBenhAn);

                            giayChungSinhVo.SoCon = thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs != null ? thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.Count() : 1;

                            giayChungSinhVo.TinhTrangCon = thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs != null ? thongtinDacDiemTreSoSinhs.DacDiemTreSoSinhs.LastOrDefault()?.TinhTrang : string.Empty;
                        }


                        giayChungSinhVo.GioiTinhCon =
                            thongTinGiayChungSinh.GioiTinh == LoaiGioiTinh.GioiTinhNam.GetDescription()
                                ? 1
                                : (thongTinGiayChungSinh.GioiTinh == LoaiGioiTinh.GioiTinhNu.GetDescription() ? 2 : 3);

                        giayChungSinhVo.CanNangCon = thongTinGiayChungSinh.CanNang;
                        giayChungSinhVo.NguoiDoDe = thongTinGiayChungSinh.NhanVienDoDeId != null ? nhanViens.FirstOrDefault(x => x.Id == thongTinGiayChungSinh.NhanVienDoDeId.Value)?.User.HoTen : string.Empty;

                        giayChungSinhVo.NguoiGhiPhieu = thongTinGiayChungSinh.NhanVienGhiPhieuId != null ? nhanViens.FirstOrDefault(x => x.Id == thongTinGiayChungSinh.NhanVienGhiPhieuId.Value)?.User.HoTen : string.Empty;


                        giayChungSinhVo.GhiChu = thongTinGiayChungSinh.GhiChu;
                        giayChungSinhVo.So = thongTinGiayChungSinh.So;
                        giayChungSinhVo.QuyenSo = thongTinGiayChungSinh.QuyenSo;
                        giayChungSinhVo.NguoiDaiDien = thongTinGiayChungSinh.GiamDocChuyenMonId != null ? nhanViens.FirstOrDefault(x => x.Id == thongTinGiayChungSinh.GiamDocChuyenMonId)?.User.HoTen : string.Empty;
                        giayChungSinhVo.NgayTaoChungTu = thongTinGiayChungSinh.NgayCapGiayChungSinh?.ApplyFormatDate();
                        giayChungSinhVo.NgayTaoChungTuDateTime = thongTinGiayChungSinh.NgayCapGiayChungSinh;
                        returnData.Add(giayChungSinhVo);
                    }
                }
            }

            return returnData.OrderBy(o=>o.NgayTaoChungTuDateTime).ToList();
        }

        public virtual byte[] ExportGiayChungSinh(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var datas = GetThongGiayChungSinh(excelChungTuQueryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GiayChungSinhVo>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("File excel mẫu giấy chứng sinh");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 15;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 40;
                    worksheet.Column(15).Width = 50;
                    worksheet.Column(16).Width = 50;
                    worksheet.Column(17).Width = 25;
                    worksheet.Column(18).Width = 20;
                    worksheet.Column(19).Width = 20;
                    worksheet.Column(20).Width = 20;
                    worksheet.Column(21).Width = 20;
                    worksheet.Column(22).Width = 20;
                    worksheet.Column(23).Width = 20;
                    worksheet.Column(24).Width = 20;
                    worksheet.Column(25).Width = 20;
                    worksheet.Column(26).Width = 20;

                    worksheet.Column(27).Width = 20;
                    worksheet.Column(28).Width = 20;
                    worksheet.Column(29).Width = 20;
                    worksheet.Column(30).Width = 20;
                    worksheet.Column(31).Width = 20;

                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:AD1"])
                    {
                        range.Worksheet.Cells["A1:AD1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:AD1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:AD1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:AD1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:AD1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:AD1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_CT";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "MA_CSKCB";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "MA_THE";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "SO_SERI";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "MA_SOBHXH_ME";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "HO_TEN_ME";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "DIA_CHI";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "CMND";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "NGAY_CAP_CMND";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "NOI_CAP_CMND";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "DAN_TOC";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "HO_TEN_CHA";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "NGAY_SINHCON";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "NOI_SINH_CON";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "TEN_CON";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["R1:R1"].Merge = true;
                        range.Worksheet.Cells["R1:R1"].Value = "SO_CON";
                        range.Worksheet.Cells["R1:R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["R1:R1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["S1:S1"].Merge = true;
                        range.Worksheet.Cells["S1:S1"].Value = "GIOI_TINH_CON";
                        range.Worksheet.Cells["S1:S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["S1:S1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["T1:T1"].Merge = true;
                        range.Worksheet.Cells["T1:T1"].Value = "CAN_NANG_CON";
                        range.Worksheet.Cells["T1:T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["T1:T1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["U1:U1"].Merge = true;
                        range.Worksheet.Cells["U1:U1"].Value = "TINH_TRANG_CON";
                        range.Worksheet.Cells["U1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["U1:U1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["V1:V1"].Merge = true;
                        range.Worksheet.Cells["V1:V1"].Value = "GHI_CHU";
                        range.Worksheet.Cells["V1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["V1:V1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["W1:W1"].Merge = true;
                        range.Worksheet.Cells["W1:W1"].Value = "NGUOI_DO_DE";
                        range.Worksheet.Cells["W1:W1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["W1:W1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["X1:X1"].Merge = true;
                        range.Worksheet.Cells["X1:X1"].Value = "NGUOI_GHI_PHIEU";
                        range.Worksheet.Cells["X1:X1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["X1:X1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Y1:Y1"].Merge = true;
                        range.Worksheet.Cells["Y1:Y1"].Value = "NGUOI_DAI_DIEN";
                        range.Worksheet.Cells["Y1:Y1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Y1:Y1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Z1:Z1"].Merge = true;
                        range.Worksheet.Cells["Z1:Z1"].Value = "NGAY_CT";
                        range.Worksheet.Cells["Z1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Z1:Z1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AA1:AA1"].Merge = true;
                        range.Worksheet.Cells["AA1:AA1"].Value = "SINHCON_PHAUTHUAT";
                        range.Worksheet.Cells["AA1:AA1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AA1:AA1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AB1:AB1"].Merge = true;
                        range.Worksheet.Cells["AB1:AB1"].Value = "SINHCON_DUOI32TUAN";
                        range.Worksheet.Cells["AB1:AB1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AB1:AB1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AC1:AC1"].Merge = true;
                        range.Worksheet.Cells["AC1:AC1"].Value = "SO";
                        range.Worksheet.Cells["AC1:AC1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AC1:AC1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["AD1:AD1"].Merge = true;
                        range.Worksheet.Cells["AD1:AD1"].Value = "QUYEN_SO";
                        range.Worksheet.Cells["AD1:AD1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["AD1:AD1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    }


                    var manager = new PropertyManager<GiayChungSinhVo>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":AD" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaCT;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.MaCSKCB;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaThe;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.SoSeRi;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.MaSoBHXHMe;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.HoTenMe;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.NgaySinh;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.DiaChi;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.CMND;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.NgayCapCMND;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.NoiCapCMND;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.DanToc;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.HoTenCha;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.NgaySinhCon;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.NoiSinhCon;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.TenCon;

                                worksheet.Cells["R" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["R" + index].Value = item.SoCon;

                                worksheet.Cells["S" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["S" + index].Value = item.GioiTinhCon;

                                worksheet.Cells["T" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["T" + index].Value = item.CanNangCon;

                                worksheet.Cells["U" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["U" + index].Value = item.TinhTrangCon;

                                worksheet.Cells["V" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["V" + index].Value = item.GhiChu;

                                worksheet.Cells["W" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["W" + index].Value = item.NguoiDoDe;

                                worksheet.Cells["X" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["X" + index].Value = item.NguoiGhiPhieu;

                                worksheet.Cells["Y" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Y" + index].Value = item.NguoiDaiDien;

                                worksheet.Cells["Z" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Z" + index].Value = item.NgayTaoChungTu;

                                worksheet.Cells["AA" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AA" + index].Value = item.SinhConPhauThuat;

                                worksheet.Cells["AB" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AB" + index].Value = item.SinhConDuoi32Tuan;

                                worksheet.Cells["AC" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AC" + index].Value = item.So;

                                worksheet.Cells["AD" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["AD" + index].Value = item.QuyenSo;

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }


        #endregion

        #region Chứng Nhận Nghỉ Dưỡng Thai

        public List<GiayChungNhanNghiDuongThai> GetThongGiayNghiDuongThai(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var cauHinhBaoHiemYTe = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var returnData = new List<GiayChungNhanNghiDuongThai>();

            var yctnNgoaiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId == null && o.YeuCauTiepNhanNgoaiTruId != null).Select(o => o.YeuCauTiepNhanNgoaiTruId.GetValueOrDefault()).ToList();
            var yctnNoiTruIds = excelChungTuQueryInfo.ThongTinYeuCauTiepNhans.Where(o => o.YeuCauTiepNhanNoiTruId != null).Select(o => o.YeuCauTiepNhanNoiTruId.GetValueOrDefault()).ToList();

            var yeuCauTiepNhanNgoaiTrus = BaseRepository.TableNoTracking.Where(d => yctnNgoaiTruIds.Contains(d.Id) && d.CoBHYT == true)
                .Include(yctnNN => yctnNN.BenhNhan)
                .Include(c => c.YeuCauKhamBenhs).ThenInclude(c => c.BacSiThucHien).ThenInclude(x => x.User);

            var noiTruHoSoKhacs = _noiTruHoSoKhacRepository.TableNoTracking
                .Where(c => c.LoaiHoSoDieuTriNoiTru == Core.Domain.Enums.LoaiHoSoDieuTriNoiTru.GiayNghiDuongThai && yctnNoiTruIds.Contains(c.YeuCauTiepNhanId) && c.YeuCauTiepNhan.NoiTruBenhAn != null)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.BenhNhan)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.YeuCauTiepNhanTheBHYTs)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.DanToc)
                .Include(yctnNN => yctnNN.YeuCauTiepNhan).ThenInclude(nt => nt.NgheNghiep).ToList();


            foreach (var yeuCauTiepNhan in yeuCauTiepNhanNgoaiTrus)
            {
                var giayNghiHuongBHXHData = new GiayChungNhanNghiDuongThai();

                if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru &&
                    yeuCauTiepNhan.YeuCauKhamBenhs.Any(yckb => yckb.BaoHiemChiTra == true && yckb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null))
                {
                    var ycKham = yeuCauTiepNhan.YeuCauKhamBenhs
                        .Where(yckb =>
                            yckb.BaoHiemChiTra == true && yckb.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham &&
                            yckb.DuongThaiTuNgay != null && yckb.DuongThaiDenNgay != null)
                        .OrderBy(o => o.Id).Last();
                    var tuDauTienKhamDuongThai = ycKham.DuongThaiTuNgay;
                    var denDauTienKhamDuongThai = ycKham.DuongThaiDenNgay;

                    giayNghiHuongBHXHData.TuNgay = tuDauTienKhamDuongThai?.ToString("yyyyMMdd");
                    giayNghiHuongBHXHData.DenNgay = denDauTienKhamDuongThai?.ToString("yyyyMMdd");

                    giayNghiHuongBHXHData.ChanDoan = ycKham.GhiChuICDChinh;
                    giayNghiHuongBHXHData.TenBS = ycKham.BacSiThucHien?.User.HoTen;
                    giayNghiHuongBHXHData.MaBS = ycKham.BacSiThucHien?.MaChungChiHanhNghe;


                    giayNghiHuongBHXHData.MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan;
                    giayNghiHuongBHXHData.SoSeRi = string.Empty;
                    giayNghiHuongBHXHData.MaCT = string.Empty;

                    giayNghiHuongBHXHData.MaThe = yeuCauTiepNhan.BHYTMaSoThe;                  
                    giayNghiHuongBHXHData.MaSoBHXH = GetLastBHXHLayTheoMaThe(yeuCauTiepNhan.BHYTMaSoThe, 10); 

                    giayNghiHuongBHXHData.HoTen = yeuCauTiepNhan.HoTen;
                    giayNghiHuongBHXHData.NgaySinh = DateHelper.DOBFormatYYYYMMDD(yeuCauTiepNhan.NgaySinh, yeuCauTiepNhan.ThangSinh, yeuCauTiepNhan.NamSinh);


                    giayNghiHuongBHXHData.TenDonVi = yeuCauTiepNhan.NoiLamViec;

                    giayNghiHuongBHXHData.NguoiDaiDien = string.Empty;
                    giayNghiHuongBHXHData.NgayCT = ycKham.DuongThaiNgayIn?.ToString("yyyyMMdd");
                    giayNghiHuongBHXHData.NgayTaoChungTuDateTime = ycKham.DuongThaiNgayIn;
                    giayNghiHuongBHXHData.SoKCB = yeuCauTiepNhan.BenhNhan.MaBN;

                    returnData.Add(giayNghiHuongBHXHData);
                }
            }

            foreach (var noiTruHoSoKhac in noiTruHoSoKhacs)
            {
                if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo) && noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any())
                {
                    var giayNghiHuongBHXHData = new GiayChungNhanNghiDuongThai();
                    var thongTinPhieuDuongThais = JsonConvert.DeserializeObject<InPhieuGiayChungNhanNghiDuongThai>(noiTruHoSoKhac.ThongTinHoSo);

                    giayNghiHuongBHXHData.TuNgay = thongTinPhieuDuongThais.NghiTuNgay.ToString("yyyyMMdd");
                    giayNghiHuongBHXHData.DenNgay = thongTinPhieuDuongThais.NghiDenNgay.ToString("yyyyMMdd");

                    giayNghiHuongBHXHData.ChanDoan = thongTinPhieuDuongThais.ChanDoan;
                    giayNghiHuongBHXHData.TenBS = thongTinPhieuDuongThais.NguoiHanhNgheKBCB;
                    giayNghiHuongBHXHData.MaBS = thongTinPhieuDuongThais.MaNguoiHanhNgheKBCB;

                    giayNghiHuongBHXHData.MaCSKCB = cauHinhBaoHiemYTe.BenhVienTiepNhan;
                    giayNghiHuongBHXHData.SoSeRi = thongTinPhieuDuongThais.SoSeri;
                    giayNghiHuongBHXHData.MaCT = string.Empty;

                    var theBHYTData = noiTruHoSoKhac.YeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.OrderBy(o => o.NgayHieuLuc).LastOrDefault();
                    var maThe = theBHYTData?.MaSoThe ?? string.Empty;                  
                    var maSoBHXH = GetLastBHXHLayTheoMaThe(maThe, 10);

                    giayNghiHuongBHXHData.MaThe = maThe;
                    giayNghiHuongBHXHData.MaSoBHXH = maSoBHXH;

                    giayNghiHuongBHXHData.HoTen = noiTruHoSoKhac.YeuCauTiepNhan.HoTen;
                    giayNghiHuongBHXHData.NgaySinh = DateHelper.DOBFormatYYYYMMDD(noiTruHoSoKhac.YeuCauTiepNhan.NgaySinh, noiTruHoSoKhac.YeuCauTiepNhan.ThangSinh, noiTruHoSoKhac.YeuCauTiepNhan.NamSinh);
                    giayNghiHuongBHXHData.TenDonVi = noiTruHoSoKhac.YeuCauTiepNhan.NoiLamViec;
                    giayNghiHuongBHXHData.NguoiDaiDien = string.Empty;
                    giayNghiHuongBHXHData.NgayCT = noiTruHoSoKhac.ThoiDiemThucHien.ToString("yyyyMMdd");
                    giayNghiHuongBHXHData.NgayTaoChungTuDateTime = noiTruHoSoKhac.ThoiDiemThucHien;
                    giayNghiHuongBHXHData.SoKCB = noiTruHoSoKhac.YeuCauTiepNhan.BenhNhan.MaBN;

                    returnData.Add(giayNghiHuongBHXHData);
                }
            }

            return returnData.OrderBy(o=>o.NgayTaoChungTuDateTime).ToList();
        }

        public virtual byte[] ExportGiayNghiDuongThai(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            var datas = GetThongGiayNghiDuongThai(excelChungTuQueryInfo);

            int ind = 1;
            var requestProperties = new[]
            {
                new PropertyByName<GiayChungNhanNghiDuongThai>("STT", p => ind++)
            };

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("File excel chứng nhận nghỉ dưỡng thai");
                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 20;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 30;
                    worksheet.Column(8).Width = 30;
                    worksheet.Column(9).Width = 30;
                    worksheet.Column(10).Width = 35;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 40;
                    worksheet.Column(13).Width = 30;
                    worksheet.Column(14).Width = 30;
                    worksheet.Column(15).Width = 40;
                    worksheet.Column(16).Width = 30;
                    worksheet.Column(17).Width = 30;
                    worksheet.DefaultColWidth = 7;
                    worksheet.Row(8).Height = 24;


                    using (var range = worksheet.Cells["A1:Q1"])
                    {
                        range.Worksheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:Q1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:Q1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:Q1"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A1:A1"].Merge = true;
                        range.Worksheet.Cells["A1:A1"].Value = "STT";
                        range.Worksheet.Cells["A1:A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["A1:A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["B1:B1"].Merge = true;
                        range.Worksheet.Cells["B1:B1"].Value = "MA_CSKCB";
                        range.Worksheet.Cells["B1:B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["B1:B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["C1:C1"].Merge = true;
                        range.Worksheet.Cells["C1:C1"].Value = "SO_SERI";
                        range.Worksheet.Cells["C1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["C1:C1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["D1:D1"].Merge = true;
                        range.Worksheet.Cells["D1:D1"].Value = "MA_CT";
                        range.Worksheet.Cells["D1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["D1:D1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["E1:E1"].Merge = true;
                        range.Worksheet.Cells["E1:E1"].Value = "SO_KCB";
                        range.Worksheet.Cells["E1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["E1:E1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["F1:F1"].Merge = true;
                        range.Worksheet.Cells["F1:F1"].Value = "MA_BHXH";
                        range.Worksheet.Cells["F1:F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["F1:F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["G1:G1"].Merge = true;
                        range.Worksheet.Cells["G1:G1"].Value = "MA_THE";
                        range.Worksheet.Cells["G1:G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["G1:G1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                        range.Worksheet.Cells["H1:H1"].Merge = true;
                        range.Worksheet.Cells["H1:H1"].Value = "HO_TEN";
                        range.Worksheet.Cells["H1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["H1:H1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["I1:I1"].Merge = true;
                        range.Worksheet.Cells["I1:I1"].Value = "NGAY_SINH";
                        range.Worksheet.Cells["I1:I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["I1:I1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["J1:J1"].Merge = true;
                        range.Worksheet.Cells["J1:J1"].Value = "TEN_DVI";
                        range.Worksheet.Cells["J1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["J1:J1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["K1:K1"].Merge = true;
                        range.Worksheet.Cells["K1:K1"].Value = "CHAN_DOAN";
                        range.Worksheet.Cells["K1:K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["K1:K1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["L1:L1"].Merge = true;
                        range.Worksheet.Cells["L1:L1"].Value = "TU_NGAY";
                        range.Worksheet.Cells["L1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["L1:L1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["M1:M1"].Merge = true;
                        range.Worksheet.Cells["M1:M1"].Value = "DEN_NGAY";
                        range.Worksheet.Cells["M1:M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["M1:M1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["N1:N1"].Merge = true;
                        range.Worksheet.Cells["N1:N1"].Value = "NGUOI_DAI_DIEN";
                        range.Worksheet.Cells["N1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["N1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["O1:O1"].Merge = true;
                        range.Worksheet.Cells["O1:O1"].Value = "TEN_BS";
                        range.Worksheet.Cells["O1:O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["O1:O1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["P1:P1"].Merge = true;
                        range.Worksheet.Cells["P1:P1"].Value = "MA_BS";
                        range.Worksheet.Cells["P1:P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["P1:P1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["Q1:Q1"].Merge = true;
                        range.Worksheet.Cells["Q1:Q1"].Value = "NGAY_CT";
                        range.Worksheet.Cells["Q1:Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        range.Worksheet.Cells["Q1:Q1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    }

                    var manager = new PropertyManager<GiayChungNhanNghiDuongThai>(requestProperties);
                    int index = 2;

                    var stt = 1;
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            using (var range = worksheet.Cells["A" + index + ":Q" + index])
                            {
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Font.Color.SetColor(Color.Black);
                                range.Worksheet.Cells["A" + index + ":Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                                worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["A" + index].Value = stt;

                                worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["B" + index].Value = item.MaCSKCB;

                                worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["C" + index].Value = item.SoSeRi;

                                worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["D" + index].Value = item.MaCT;

                                worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["E" + index].Value = item.SoKCB;

                                worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["F" + index].Value = item.MaSoBHXH;

                                worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["G" + index].Value = item.MaThe;

                                worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["H" + index].Value = item.HoTen;

                                worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["I" + index].Value = item.NgaySinh;

                                worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["J" + index].Value = item.TenDonVi;

                                worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["K" + index].Value = item.ChanDoan;

                                worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["L" + index].Value = item.TuNgay;

                                worksheet.Cells["M" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["M" + index].Value = item.DenNgay;

                                worksheet.Cells["N" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["N" + index].Value = item.NguoiDaiDien;

                                worksheet.Cells["O" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["O" + index].Value = item.TenBS;

                                worksheet.Cells["P" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["P" + index].Value = item.MaBS;

                                worksheet.Cells["Q" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells["Q" + index].Value = item.NgayCT;

                                index++;
                                stt++;
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        #endregion

        private int GetGioiTinhBHYT(LoaiGioiTinh? loaiGioiTinh)
        {
            return loaiGioiTinh == null ? 3 : (int)loaiGioiTinh;
        }

        private int GetDanTocBHYT(string maDanToc)
        {
            var danToc = 1;
            int.TryParse(maDanToc, out danToc);
            return danToc;
        }

        private int GetTinhTrangRaVienBHYT(Enums.EnumTinhTrangRaVien? tinhTrangRaVien)
        {
            return tinhTrangRaVien == null ? 1 : (int)tinhTrangRaVien;
        }

        private string GetLastBHXHLayTheoMaThe(string source, int numberOfChars)
        {
            if (numberOfChars >= source.Length)
                return source;
            return source.Substring(source.Length - numberOfChars);
        }
    }
}
