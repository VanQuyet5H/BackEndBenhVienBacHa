using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacGiayChuyenTuyen(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayChuyenTuyen)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public string GetChanDoan(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                               .Include(p => p.NoiTruBenhAn)
                                                               .FirstOrDefault();

            if (yeuCauTiepNhan.NoiTruBenhAn != null && !string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien))
            {
                var model = JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien);
                return model.GhiChuChuanDoanRaVien;
            }

            return string.Empty;
        }

        public async Task<string> InGiayChuyenTuyen(long yeuCauTiepNhanId, bool isInFilePDF = true)
        {
            var today = DateTime.Now;

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "HoSoKhacGiayChuyenTuyenPDF" : "HoSoKhacGiayChuyenTuyen"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.DanToc)
                                                                     .Include(p => p.QuocTich)
                                                                     .Include(p => p.NgheNghiep)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayChuyenTuyen)
                                                               .FirstOrDefault();

            if(noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new
                {
                    //SoHoSo = thongTin.SoHoSo,
                    //SoChuyenTuyenSo = thongTin.SoChuyenTuyenSo,
                    //NguoiNhan = thongTin.NguoiNhan,
                    HoTenBenhNhan = yeuCauTiepNhan.HoTen,
                    GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                    Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "",
                    DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                    DanToc = yeuCauTiepNhan.DanToc?.Ten,
                    QuocTich = yeuCauTiepNhan.QuocTich?.Ten,
                    NgheNghiep = yeuCauTiepNhan.NgheNghiep?.Ten,
                    NoiLamViec = yeuCauTiepNhan.NoiLamViec,
                    //NgayHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Day,
                    //ThangHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Month,
                    //NamHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Year,
                    //NgayHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Day,
                    //ThangHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Month,
                    //NamHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Year,
                    //SoThe1 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(0, 2) : "",
                    //SoThe2 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(2, 1) : "",
                    //SoThe3 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(3, 2) : "",
                    //SoThe4 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(5, 2) : "",
                    //SoThe5 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(7) : "",

                    NgayHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Day.ToString() ?? "&emsp;&emsp;",
                    ThangHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Month.ToString() ?? "&emsp;&emsp;",
                    NamHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Year.ToString() ?? "&emsp;&emsp;",
                    NgayHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Day.ToString() ?? "&emsp;&emsp;",
                    ThangHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Month.ToString() ?? "&emsp;&emsp;",
                    NamHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Year.ToString() ?? "&emsp;&emsp;",
                    SoThe1 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(0, 2) : "&emsp;&emsp;",
                    SoThe2 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(2, 1) : "&emsp;&emsp;",
                    SoThe3 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(3, 2) : "&emsp;&emsp;",
                    SoThe4 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(5, 2) : "&emsp;&emsp;",
                    SoThe5 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(7) : "&emsp;&emsp;",
                    //DanhSachDuocKhamBenhDieuTri = danhSachDuocKhamBenhDieuTri,
                    //DauHieuLamSan = thongTin.DauHieuLamSan,
                    //KetQuaXetNghiemCLS = thongTin.KetQuaXetNghiemCLS,
                    //ChanDoan = thongTin.ChanDoan,
                    //PhuongPhapSuDungTrongDieuTri = thongTin.PhuongPhapSuDungTrongDieuTri,
                    //TinhTrangNguoiBenh = thongTin.TinhTrangNguoiBenh,
                    //HuongDieuTri = thongTin.HuongDieuTri,
                    //ChuyenTuyenHoi = thongTin.ChuyenTuyenHoi?.ApplyFormatDate(),
                    //PhuongTienVanChuyen = thongTin.PhuongTienVanChuyen,
                    //NguoiHoTong = thongTinNguoiHoTong,
                    Ngay = today.Day,
                    Thang = today.Month,
                    Nam = today.Year
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacGiayChuyenTuyenVo>(noiTruHoSoKhac.ThongTinHoSo);

            var thongTinNguoiHoTong =
                thongTin.HoTenNguoiHoTong +
                (!string.IsNullOrEmpty(thongTin.HoTenNguoiHoTong) && (!string.IsNullOrEmpty(thongTin.ChucDanhNguoiHoTong) || !string.IsNullOrEmpty(thongTin.TrinhDoChuyenMonNguoiHoTong)) ? " - " : "") +
                thongTin.ChucDanhNguoiHoTong +
                (!string.IsNullOrEmpty(thongTin.ChucDanhNguoiHoTong) && !string.IsNullOrEmpty(thongTin.TrinhDoChuyenMonNguoiHoTong) ? " - " : "") +
                thongTin.TrinhDoChuyenMonNguoiHoTong;

            var danhSachDuocKhamBenhDieuTri = string.Empty;

            if(isInFilePDF)
            {
                if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.DuocChuyenVien == true)
                {
                    danhSachDuocKhamBenhDieuTri =
                        "<tr>" +
                           "<td colspan='3' style='width: 37.5%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>+ Tại:&nbsp;</div>" +
                                   $"<div class='value'>{yeuCauTiepNhan.NoiChuyen?.Ten}({yeuCauTiepNhan.NoiChuyen?.TuyenChuyenMonKyThuat.GetDescription()})</div>" +
                               "</div>" +
                           "</td>" +
                           "<td style='width: 12.5%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>Tuyến:&nbsp;</div>" +
                                   "<div class='value remove-border-bottom'>" +
                                       "(<div style='display: inline-block; width: calc(100% - 12px); border-bottom: 1px dotted black;'>&emsp;&emsp;</div>)" +
                                   "</div>" +
                               "</div>" +
                           "</td>" +
                           "<td colspan='2' style='width: 25%;'>" +
                              "<div class='container'>" +
                                   "<div class='label'>Từ ngày:&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                               "</div>" +
                           "</td>" +
                           "<td colspan='2' style='width: 25%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>đến&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Day}&nbsp;/&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Month}&nbsp;/&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Year}&nbsp;</div>" +
                               "</div>" +
                           "</td>" +
                       "</tr>";

                    //2 hàng theo mẫu
                    danhSachDuocKhamBenhDieuTri +=
                        "<tr>" +
                           "<td colspan='3' style='width: 37.5%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>+ Tại:&nbsp;</div>" +
                                   $"<div class='value'>{yeuCauTiepNhan.NoiChuyen?.Ten}({yeuCauTiepNhan.NoiChuyen?.TuyenChuyenMonKyThuat.GetDescription()})</div>" +
                               "</div>" +
                           "</td>" +
                           "<td style='width: 12.5%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>Tuyến:&nbsp;</div>" +
                                   "<div class='value remove-border-bottom'>" +
                                       "(<div style='display: inline-block; width: calc(100% - 12px); border-bottom: 1px dotted black;'>&emsp;&emsp;</div>)" +
                                   "</div>" +
                               "</div>" +
                           "</td>" +
                           "<td colspan='2' style='width: 25%;'>" +
                              "<div class='container'>" +
                                   "<div class='label'>Từ ngày:&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                   "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                               "</div>" +
                           "</td>" +
                           "<td colspan='2' style='width: 25%;'>" +
                               "<div class='container'>" +
                                   "<div class='label'>đến&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Day}&nbsp;/&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Month}&nbsp;/&nbsp;</div>" +
                                   $"<div class='value text-center' style='width: fit-content;'>{yeuCauTiepNhan.ThoiDiemTiepNhan.Year}&nbsp;</div>" +
                               "</div>" +
                           "</td>" +
                       "</tr>";
                }
                else
                {
                    danhSachDuocKhamBenhDieuTri =
                        "<tr>" +
                            "<td colspan='3' style='width: 37.5%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>+ Tại:&nbsp;</div>" +
                                    "<div class='value'>&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                            "<td style='width: 12.5%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>Tuyến:&nbsp;</div>" +
                                    "<div class='value remove-border-bottom'>" +
                                        "(<div style='display: inline-block; width: calc(100% - 12px); border-bottom: 1px dotted black;'>&emsp;&emsp;</div>)" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                            "<td colspan='2' style='width: 25%;'>" +
                               "<div class='container'>" +
                                    "<div class='label'>Từ ngày:&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                            "<td colspan='2' style='width: 25%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>đến&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";

                    //2 hàng theo mẫu
                    danhSachDuocKhamBenhDieuTri +=
                        "<tr>" +
                            "<td colspan='3' style='width: 37.5%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>+ Tại:&nbsp;</div>" +
                                    "<div class='value'>&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                            "<td style='width: 12.5%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>Tuyến:&nbsp;</div>" +
                                    "<div class='value remove-border-bottom'>" +
                                        "(<div style='display: inline-block; width: calc(100% - 12px); border-bottom: 1px dotted black;'>&emsp;&emsp;</div>)" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                            "<td colspan='2' style='width: 25%;'>" +
                               "<div class='container'>" +
                                    "<div class='label'>Từ ngày:&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                            "<td colspan='2' style='width: 25%;'>" +
                                "<div class='container'>" +
                                    "<div class='label'>đến&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;/&nbsp;</div>" +
                                    "<div class='value text-center' style='width: fit-content;'>&emsp;&emsp;&nbsp;</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";
                }
            }
            else
            {
                if (yeuCauTiepNhan.CoBHYT == true && yeuCauTiepNhan.DuocChuyenVien == true)
                {
                    danhSachDuocKhamBenhDieuTri =
                        "<tr>" +
                            "<td colspan='2' style='font-size: 15px; width: 100%;'>" +
                                "<div style='width: 100%; display: flex;'>" +
                                    "<div style='width: 6%;'>+ Tại: </div>" +
                                    $"<div style='width: 25%; border-bottom: 1px dotted black;'>{yeuCauTiepNhan.NoiChuyen?.Ten}({yeuCauTiepNhan.NoiChuyen?.TuyenChuyenMonKyThuat.GetDescription()})&nbsp;</div>" +
                                    $"<div style='width: 10%; '>Tuyến(<div class='tuyen-kham-benh'>{"&nbsp;"}</div>)</div>" +
                                    "<div style='width: 8%; text-align: center;'> Từ ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                    "<div style='width: 9%; text-align: center;'> Đến ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Day} </div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Month} </div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Year} </div>" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";

                    //2 hàng theo mẫu
                    danhSachDuocKhamBenhDieuTri +=
                        "<tr>" +
                            "<td colspan='2' style='font-size: 15px; width: 100%;'>" +
                                "<div style='width: 100%; display: flex;'>" +
                                    "<div style='width: 6%;'>+ Tại: </div>" +
                                    $"<div style='width: 25%; border-bottom: 1px dotted black;'>{yeuCauTiepNhan.NoiChuyen?.Ten}({yeuCauTiepNhan.NoiChuyen?.TuyenChuyenMonKyThuat.GetDescription()})&nbsp;</div>" +
                                    $"<div style='width: 10%; '>Tuyến(<div class='tuyen-kham-benh'>{"&nbsp;"}</div>)</div>" +
                                    "<div style='width: 8%; text-align: center;'> Từ ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                    "<div style='width: 9%; text-align: center;'> Đến ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Day} </div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Month} </div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'> {yeuCauTiepNhan.ThoiDiemTiepNhan.Year} </div>" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";
                }
                else
                {
                    danhSachDuocKhamBenhDieuTri =
                        "<tr>" +
                            "<td colspan='2' style='font-size: 15px; width: 100%;'>" +
                                "<div style='width: 100%; display: flex;'>" +
                                    "<div style='width: 6%;'>+ Tại: </div>" +
                                    $"<div style='width: 25%; border-bottom: 1px dotted black;'>{""}&nbsp;</div>" +
                                    $"<div style='width: 10%; '>Tuyến(<div class='tuyen-kham-benh'>{"&nbsp;"}</div>)</div>" +
                                    "<div style='width: 8%; text-align: center;'> Từ ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                    "<div style='width: 9%; text-align: center;'> Đến ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";

                    //2 hàng theo mẫu
                    danhSachDuocKhamBenhDieuTri +=
                        "<tr>" +
                            "<td colspan='2' style='font-size: 15px; width: 100%;'>" +
                                "<div style='width: 100%; display: flex;'>" +
                                    "<div style='width: 6%;'>+ Tại: </div>" +
                                    $"<div style='width: 25%; border-bottom: 1px dotted black;'>{""}&nbsp;</div>" +
                                    $"<div style='width: 10%;'>Tuyến(<div class='tuyen-kham-benh'>{"&nbsp;"}</div>)</div>" +
                                    "<div style='width: 8%; text-align: center;'> Từ ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                    "<div style='width: 9%; text-align: center;'> Đến ngày: </div>" +
                                    "<div style='width: 21%; display: flex; border-bottom: 1px dotted black;'>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 30%; text-align: center;'>{""}</div>" +
                                        "<div style='width: 0.5%;'>/</div>" +
                                        $"<div style='width: 39%; text-align: center;'>{""}</div>" +
                                    "</div>" +
                                "</div>" +
                            "</td>" +
                        "</tr>";
                }
            }

            var data = new HoSoKhacGiayInChuyenTuyen
            {
                SoHoSo = thongTin.SoHoSo,
                SoChuyenTuyenSo = thongTin.SoChuyenTuyenSo,
                NguoiNhan = thongTin.NguoiNhan,
                HoTenBenhNhan = yeuCauTiepNhan.HoTen,
                GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "",
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                DanToc = yeuCauTiepNhan.DanToc?.Ten,
                QuocTich = yeuCauTiepNhan.QuocTich?.Ten,
                NgheNghiep = yeuCauTiepNhan.NgheNghiep?.Ten,
                NoiLamViec = yeuCauTiepNhan.NoiLamViec,
                NgayHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Day.ToString() ?? "&emsp;&emsp;",
                ThangHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Month.ToString() ?? "&emsp;&emsp;",
                NamHieuLucBHYT = yeuCauTiepNhan.BHYTNgayHieuLuc?.Year.ToString() ?? "&emsp;&emsp;",
                NgayHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Day.ToString() ?? "&emsp;&emsp;",
                ThangHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Month.ToString() ?? "&emsp;&emsp;",
                NamHetHanBHYT = yeuCauTiepNhan.BHYTNgayHetHan?.Year.ToString() ?? "&emsp;&emsp;",
                SoThe1 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(0, 2) : "&emsp;&emsp;",
                SoThe2 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(2, 1) : "&emsp;&emsp;",
                SoThe3 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(3, 2) : "&emsp;&emsp;",
                SoThe4 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(5, 2) : "&emsp;&emsp;",
                SoThe5 = !string.IsNullOrEmpty(yeuCauTiepNhan.BHYTMaSoThe) ? yeuCauTiepNhan.BHYTMaSoThe.Substring(7) : "&emsp;&emsp;",
                DanhSachDuocKhamBenhDieuTri = danhSachDuocKhamBenhDieuTri,
                DauHieuLamSan = thongTin.DauHieuLamSan,
                KetQuaXetNghiemCLS = thongTin.KetQuaXetNghiemCLS,
                ChanDoan = thongTin.ChanDoan,
                PhuongPhapSuDungTrongDieuTri = thongTin.PhuongPhapSuDungTrongDieuTri,
                TinhTrangNguoiBenh = thongTin.TinhTrangNguoiBenh,
                HuongDieuTri = thongTin.HuongDieuTri,
                ChuyenTuyenHoi = thongTin.ChuyenTuyenHoi?.ApplyFormatDate(),
                PhuongTienVanChuyen = thongTin.PhuongTienVanChuyen,
                NguoiHoTong = thongTinNguoiHoTong,
                LyDo1 = thongTin.LyDoChuyenTuyen != null && thongTin.LyDoChuyenTuyen == 0 ? "circle" : "",
                LyDo2 = thongTin.LyDoChuyenTuyen != null && thongTin.LyDoChuyenTuyen == 1 ? "circle" : "",
                Ngay = today.Day,
                Thang = today.Month,
                Nam = today.Year
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}
