using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public string GetPhieuHoSoKhacJson(long yeuCauTiepNhanId, long? hoSoKhacId)
        {
            var que = _noiTruHoSoKhacRepository.TableNoTracking.Where(d => d.YeuCauTiepNhanId == yeuCauTiepNhanId && d.Id == hoSoKhacId).Select(d => d.ThongTinHoSo);
            return que.FirstOrDefault();
        }
        public HoSoChamSocDieuDuongHoSinhVo GetDataTheoYCTN(long yeuCauTiepNhanId)
        {
            var ngayHienTai = new DateTime();
            ngayHienTai = DateTime.Now;
            var query = BaseRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId).Select(s => new HoSoChamSocDieuDuongHoSinhVo
            {
                HoVaTen = s.HoTen,
                Tuoi = s.NamSinh != null ? (DateTime.Now.Year - s.NamSinh): null,
                GioiTinh = s.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? true : false,
                DiaChi = s.DiaChiDayDu,
                NgheNghiep = s.NgheNghiep.Ten,
                DienThoai = s.SoDienThoai,
                HoTenNguoiNha = s.NguoiLienHeQuanHeNhanThanId != null ? s.NguoiLienHeQuanHeNhanThan.Ten + " " +   s.NguoiLienHeHoTen: s.NguoiLienHeHoTen,
                DienThoaiLienLacNguoiNha = s.NguoiLienHeSoDienThoai, 
                NgayVaoVien = s.CreatedOn,
                ChanDoankhiVaoVien = s.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(d=>d.ChanDoanChinhGhiChu).FirstOrDefault(),
                LyDoVaoVienTheoYeuCauTiepNhan = s.LyDoVaoVien.GetDescription(),

                NoiTruPhieuDieuTris = s.NoiTruBenhAn.NoiTruPhieuDieuTris.ToList(),



                LoaiBenhAn = s.NoiTruBenhAn.LoaiBenhAn,
                ThongTinBenhAn = s.NoiTruBenhAn.ThongTinBenhAn,
                YeuCauTiepNhanNgoaiTruId = s.YeuCauTiepNhanNgoaiTruCanQuyetToanId
                //KhaiThacBenhSus = s.YeuCauKhamBenhs.Select(d=>d.BenhSu).ToList(),
                //TienSuBenhNhans = s.BenhNhan.BenhNhanTienSuBenhs.Select(d=>d.TenBenh).ToList(),
                //BenhNhanDiUngThuocs = s.BenhNhan.BenhNhanDiUngThuocs.Select(g=>g.TenDiUng).ToList(),

                //ChanDoanVaoViens = s.YeuCauKhamBenhs.Select(d=>d.GhiChuICDChinh).ToList(),
                //LyDoVaoViens = s.YeuCauKhamBenhs.Select(d=>d.TrieuChungTiepNhan).ToList()
            }).FirstOrDefault();

            if(query.YeuCauTiepNhanNgoaiTruId != null )
            {
                var queryNgoaiTru = BaseRepository.TableNoTracking.Where(s => s.Id == query.YeuCauTiepNhanNgoaiTruId).Select(s => new HoSoChamSocDieuDuongHoSinhVo
                {
                    KhaiThacBenhSus = s.YeuCauKhamBenhs.Select(d => d.BenhSu).ToList(),
                    TienSuBenhNhans = s.BenhNhan.BenhNhanTienSuBenhs.Select(d => d.TenBenh).ToList(),
                    BenhNhanDiUngThuocs = s.BenhNhan.BenhNhanDiUngThuocs.Select(g => g.TenDiUng).ToList(),

                    ChanDoanVaoViens = s.YeuCauKhamBenhs.Select(d => d.GhiChuICDChinh).ToList(),
                    LyDoVaoViens = s.YeuCauKhamBenhs.Select(d => d.TrieuChungTiepNhan).ToList()
                }).FirstOrDefault();
                if(queryNgoaiTru != null)
                {
                    //. lý do vào viện lấy lý do vào viện tab khám bệnh (TrieuChungTiepNhan yêu cầu khám) 
                    query.LyDoVaoVien = queryNgoaiTru.LyDoVaoViens.Where(d => d != null).ToList().Join(", ");


                    // lấy bệnh sử tab khám bệnh (bệnh sử yêu cầu khám) 
                    query.KhaiThacBenhSu = queryNgoaiTru.KhaiThacBenhSus.Where(d => d != null).ToList().Join(", ");

                    // tiền sử lấy bệnh nhân dị ứng thuốc theo yêu cầu tiêp nhận
                    query.TienSuDiUngCo = queryNgoaiTru.BenhNhanDiUngThuocs.ToList().Count() != 0 ? true : false;
                    query.TienSuDiUngneuCo = queryNgoaiTru.BenhNhanDiUngThuocs.ToList().Join("\n");


                    // lấy chẩn đoán ghi chú chính tap  kết luận (khám bệnh)
                    query.ChanDoankhiVaoVien = queryNgoaiTru.ChanDoanVaoViens.Where(d => d != null).Join(", ");
                }
            }


            return query;
        }
        public async Task<string> InHoSoDieuDtriChamSocHoSinh(XacNhanInHoSoChamSocDieuDuongHoSinh xacNhanInHoSoChamSocDieuDuongHoSinh)
        {
            var thongtinIn = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == xacNhanInHoSoChamSocDieuDuongHoSinh.NoiTruHoSoKhacId && x.YeuCauTiepNhanId == xacNhanInHoSoChamSocDieuDuongHoSinh.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInHoSoChamSocDieuDuongHoSinh.LoaiHoSoDieuTriNoiTru).Select(x => x.ThongTinHoSo).FirstOrDefault();
            var queryString = JsonConvert.DeserializeObject<HoSoChamSocDieuDuongHoSinhVo>(thongtinIn);
            var content = string.Empty;

            var result = _templateRepository.TableNoTracking
               .FirstOrDefault(x => x.Name.Equals("HoSoChamSocDieuDuong"));

            // replace khoảng \n text area 
            string tmp = "\n";
            string replace = "<br>";
            string chanDoan = "";

            if(!string.IsNullOrEmpty(queryString.TienSuDiUngneuCo))
            {
                queryString.TienSuDiUngneuCo = queryString.TienSuDiUngneuCo.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.NguoiBenhCoKhuyetTat))
            {
                queryString.NguoiBenhCoKhuyetTat = queryString.NguoiBenhCoKhuyetTat.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.TinhTrangHienTaiCuaNB))
            {
                queryString.TinhTrangHienTaiCuaNB = queryString.TinhTrangHienTaiCuaNB.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.KeHoachChamSocVaTheoDoi))
            {
                queryString.KeHoachChamSocVaTheoDoi = queryString.KeHoachChamSocVaTheoDoi.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.DanhGiaTinhTrangNBKhiRaVien))
            {
                queryString.DanhGiaTinhTrangNBKhiRaVien = queryString.DanhGiaTinhTrangNBKhiRaVien.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.HuongDanNBNhungDieuCanThiet))
            {
                queryString.HuongDanNBNhungDieuCanThiet = queryString.HuongDanNBNhungDieuCanThiet.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.HenDenKham))
            {
                queryString.HenDenKham = queryString.HenDenKham.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.ChanDoankhiVaoVien))
            {
                queryString.ChanDoankhiVaoVien = queryString.ChanDoankhiVaoVien.Replace(tmp, replace);
            }

            if (!string.IsNullOrEmpty(queryString.LyDoVaoVien))
            {
                queryString.LyDoVaoVien = queryString.LyDoVaoVien.Replace(tmp, replace);
            }
            if (!string.IsNullOrEmpty(queryString.KhaiThacBenhSu))
            {
                queryString.KhaiThacBenhSu = queryString.KhaiThacBenhSu.Replace(tmp, replace);
            }
            if (!string.IsNullOrEmpty(queryString.TienSuDiUngneuCo))
            {
                queryString.TienSuDiUngneuCo = queryString.TienSuDiUngneuCo.Replace(tmp, replace);
            }







            var dataInfo = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == xacNhanInHoSoChamSocDieuDuongHoSinh.YeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == xacNhanInHoSoChamSocDieuDuongHoSinh.LoaiHoSoDieuTriNoiTru)
                                                                             .Select(x => new
                                                                             {
                                                                                 HoVaTenNB = queryString.HoVaTen,
                                                                                 Tuoi = queryString.Tuoi,
                                                                                 GioiTinh = queryString.GioiTinhText,
                                                                                 DiaChi = queryString.DiaChi,
                                                                                 NgheNghiep = queryString.NgheNghiep,
                                                                                 DienThoai = queryString.DienThoai,
                                                                                 HoVaTenNguoiNhaKhiCanBaoTin = queryString.HoTenNguoiNha,
                                                                                 DienThoaiLienLac = queryString.DienThoaiLienLacNguoiNha,
                                                                                 NgayVaoVien = queryString.NgayVaoVien != null ? queryString.NgayVaoVien.Value.ApplyFormatDate() : "",
                                                                                 ChanDoanKhiVaoVien = queryString.ChanDoankhiVaoVien,
                                                                                 LyDoVaoVien = queryString.LyDoVaoVien,
                                                                                 KhaiThacBenhSu = queryString.KhaiThacBenhSu,
                                                                                 TienSu = queryString.TienSu,
                                                                                 Co = queryString.TienSuDiUngCo == true ? "X" : "",
                                                                                 Khong = queryString.TienSuDiUngCo == false ? "X" : "",
                                                                                 NeuCoKeTen = queryString.TienSuDiUngneuCo,
                                                                                 CoHutThuoc = queryString.CoHutThuoc == true ? "X" : "",
                                                                                 CoNghienRuouBia = queryString.CoNghienRuouBia == true ? "X" : "",
                                                                                 NguoiBenhCoKhuyetTat = queryString.NguoiBenhCoKhuyetTat,
                                                                                 TinhTaoTiepXuc = queryString.TinhTrangTinhTao == true ? "X" : "",
                                                                                 Me = queryString.TinhTrangMe == true ? "X" : "",
                                                                                 LoMo = queryString.TinhTrangLoMo == true ? "X" : "",
                                                                                 TinhTrangHienTaiCuaNguoiBenh = queryString.TinhTrangHienTaiCuaNB,
                                                                                 KeHoachChamSocVaTheoDoi = queryString.KeHoachChamSocVaTheoDoi,
                                                                                 BSChoVeNha = queryString.BSChoVe == true ? "X" : "",
                                                                                 ChuyenVien = queryString.ChuyenVien == true ? "X" : "",
                                                                                 NangXinVe = queryString.NangXinVe == true ? "X" : "",
                                                                                 DanhGiaTinhTrangNBKhiRaVien = queryString.DanhGiaTinhTrangNBKhiRaVien,
                                                                                 HuongDanNBNhungDieuCanThiet = queryString.HuongDanNBNhungDieuCanThiet,
                                                                                 GiayRaVien = queryString.GiayRaVien == true ? "X" : "",
                                                                                 DonThuoc = queryString.DonThuoc == true ? "X" : "",
                                                                                 BienLaiThanhToanVienPhi = queryString.BienLaiThanhToanVienPhi == true ? "X" : "",
                                                                                 GiayChungNhanPhauThuat = queryString.GiayChungNhanPhauThuat == true ? "X" : "",
                                                                                 GiayChungSinh = queryString.GiayChungSinh == true ? "X" : "",
                                                                                 HenDenKhamLai = queryString.HenDenKham,
                                                                                 NgayHienTai = DateTime.Now.Day,
                                                                                 ThangHienTai = DateTime.Now.Month,
                                                                                 NamHienTai = DateTime.Now.Year,
                                                                                 DieuDuongTruongKhoa = queryString.HoSinhTruong,
                                                                                 DieuDuongChamSocNguoiBenh = queryString.HoSinhChamSocNguoiBenh,
                                                                                 MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                                 NgaySinh = x.YeuCauTiepNhan.NgaySinh,
                                                                                 ThangSinh = x.YeuCauTiepNhan.ThangSinh,
                                                                                 NamSinh = x.YeuCauTiepNhan.NamSinh
                                                                             }).FirstOrDefault();


            var data = new InHoSoChamSocDieuDuongHoSinhVo();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(dataInfo.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(dataInfo.MaYeuCauTiepNhan.ToString()) : "";
            data.MaTN = dataInfo.MaYeuCauTiepNhan;

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            
            data.KhoaPhongDangIn = tenKhoa;

            data.HoTen = !string.IsNullOrEmpty(dataInfo.HoVaTenNB) ? "<div class='container'><div class='label'>1. Họ và tên NB:&nbsp;</div><div class='values'><b>" + dataInfo.HoVaTenNB + "</b></div></div>"
                : "<div class='container'><div class='label'>1. Họ và tên NB:&nbsp;</div><div class='value'><b>" + dataInfo.HoVaTenNB + "</b></div></div>";

            var ns = DateHelper.DOBFormat(dataInfo.NgaySinh, dataInfo.ThangSinh, dataInfo.NamSinh);

            data.NgayThangNam = !string.IsNullOrEmpty(ns) ? "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='values'><b>" + ns + "</b></div></div>"
                : "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='value'><b>" + ns + "</b></div></div>";

            data.GioiTinh = !string.IsNullOrEmpty(dataInfo.GioiTinh) ? "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='values'><b>" + dataInfo.GioiTinh + "</b></div></div>"
                : "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='value'>" + dataInfo.GioiTinh + "</div></div>";

            data.DiaChi = !string.IsNullOrEmpty(dataInfo.DiaChi) ? "<div class='container'><div class='label'>2. Địa chỉ:&nbsp;</div><div class='values'>" + dataInfo.DiaChi + "</div></div>"
                 : "<div class='container'><div class='label'>2. Địa chỉ:&nbsp;</div><div class='value'>" + dataInfo.DiaChi + "</div></div>";

            data.NgheNghiep = !string.IsNullOrEmpty(dataInfo.NgheNghiep) ? "<div class='container'><div class='label'>3. Nghề nghiệp:&nbsp;</div><div class='values'>" + dataInfo.NgheNghiep + "</div></div>"
                : "<div class='container'><div class='label'>3. Nghề nghiệp:&nbsp;</div><div class='value'>" + dataInfo.NgheNghiep + "</div></div>";

            data.DienThoai = !string.IsNullOrEmpty(dataInfo.DienThoai) ? "<div class='container'><div class='label'>Điện thoại:&nbsp;</div><div class='values'>" + dataInfo.DienThoai + "</div></div>"
                : "<div class='container'><div class='label'>Điện thoại:&nbsp;</div><div class='value'>" + dataInfo.DienThoai + "</div></div>";

            data.HoVaTenNguoiNhaKhiCanBaoTin = !string.IsNullOrEmpty(dataInfo.HoVaTenNguoiNhaKhiCanBaoTin) ? "<div class='container'><div class='label'>4 .Họ và tên người nhà khi cần báo tin:&nbsp;</div><div class='values'>" + dataInfo.HoVaTenNguoiNhaKhiCanBaoTin + "</div></div>"
              : "<div class='container'><div class='label'>4 .Họ và tên người nhà khi cần báo tin:</div><div class='value'>" + dataInfo.HoVaTenNguoiNhaKhiCanBaoTin + "</div></div>";

            data.DienThoaiLienLac = !string.IsNullOrEmpty(dataInfo.NgayVaoVien) ? "<div class='container'><div class='label'>Điện thoại liên lạc:&nbsp;</div><div class='values'>" + dataInfo.DienThoaiLienLac + "</div></div>"
              : "<div class='container'><div class='label'>Điện thoại liên lạc:</div><div class='value'>" + dataInfo.DienThoaiLienLac + "</div></div>";



            data.NgayVaoVien = !string.IsNullOrEmpty(dataInfo.NgayVaoVien) ? "<div class='container'><div class='label'>5. Ngày vào viện:&nbsp;</div><div class='values'>" + dataInfo.NgayVaoVien + "</div></div>"
              : "<div class='container'><div class='label'>5. Ngày vào viện:</div><div class='value'>" + dataInfo.NgayVaoVien + "</div></div>";

            data.ChanDoanKhiVaoVien = !string.IsNullOrEmpty(dataInfo.ChanDoanKhiVaoVien) ? "<div class='container'>"+ 
                                                                                            "<div class='label'>6. </div>" +
                                                                                            $"<div class='values'> <div style='word-break: break-word;'>Chẩn đoán khi vào viện: {dataInfo.ChanDoanKhiVaoVien}</div></div>"+
                                                                                            "</div>"
                : "<div class='container'><div class='label'>6. Chẩn đoán khi vào viện:</div><div class='value'>" + dataInfo.ChanDoanKhiVaoVien + "</div></div>";

            data.LyDoVaoVien = !string.IsNullOrEmpty(dataInfo.LyDoVaoVien) ? "<div class='container'>" +
                                                                                            "<div class='label'>7. </div>" +
                                                                                            $"<div class='values'> <div style='word-break: break-word;'>Lý do vào viện: {dataInfo.LyDoVaoVien}</div></div>" +
                                                                                            "</div>" 
                : "<div class='container'><div class='label'>7. Lý do vào viện:</div><div class='value'>" + dataInfo.LyDoVaoVien + "</div></div>";


            data.KhaiThacBenhSu = !string.IsNullOrEmpty(dataInfo.KhaiThacBenhSu) ? "<div class='container'>" +
                                                                                            "<div class='label'>8. </div>" +
                                                                                            $"<div class='values'> <div style='word-break: break-word;'>Khai thác bệnh sử: {dataInfo.KhaiThacBenhSu}</div></div>" +
                                                                                            "</div>"  
                : "<div class='container'><div class='label'>8. Khai thác bệnh sử:</div><div class='value'>" + dataInfo.KhaiThacBenhSu + "</div></div>" +
                  "<tr>" +
                  "<td colspan='5' style='padding-left: 15px;'>" +
                  "<div class='container'>" +
                  "<div class='value' style='height: 30px;'>" + dataInfo.KhaiThacBenhSu + "</div>" +
                  "</div>" +
                  "</td>" +
                  "</tr>" +
                  "<tr>" +
                  "<td colspan='5' style='padding-left: 15px;'>" +
                  "<div class='container'>" +
                  "<div class='value' style='height: 30px;'>" + dataInfo.KhaiThacBenhSu + "</div>" +
                  "</div>" +
                  "</td>" +
                  "</tr>";



            data.TienSu = !string.IsNullOrEmpty(dataInfo.TienSu) ? "<div class='container'>" +
                                                                     "<div class='label'></div>" +
                                                                     $"<div class='values'> <div style='word-break: break-word;'>+ Tiền sử: {dataInfo.TienSu?.Replace("\n","<br>")}</div></div>" +
                                                                     "</div>"  
                : "<div class='container'><div class='label'>+ Tiền sử:</div><div class='value'>" + dataInfo.TienSu + "</div></div>" +
                 "<tr>" +
                  "<td colspan='5' style='padding-left: 15px;'>" +
                  "<div class='container'>" +
                  "<div class='value' style='height: 30px;'>" + dataInfo.TienSu + "</div>" +
                  "</div>" +
                  "</td>" +
                  "</tr>";

            data.Co = dataInfo.Co;
            data.Khong = dataInfo.Khong;


            data.NeuCoKeTen = !string.IsNullOrEmpty(dataInfo.NeuCoKeTen) ? "<div class='container'>" +
                                                                             "<div class='label'></div>" +
                                                                             $"<div class='values'> <div style='word-break: break-word;'>&nbsp;&nbsp;&nbsp;Nếu có kể tên: {dataInfo.NeuCoKeTen?.Replace("\n", "<br>")}</div></div>" +
                                                                             "</div>"  
              : "<div class='container'><div class='label'>Nếu có kể tên:</div><div class='value'>" + dataInfo.NeuCoKeTen + "</div></div>";


            data.CoHutThuoc = dataInfo.CoHutThuoc;
            data.CoNghienRuouBia = dataInfo.CoNghienRuouBia;


            data.NguoiBenhCoKhuyetTat = !string.IsNullOrEmpty(dataInfo.NguoiBenhCoKhuyetTat) ? "<div class='container'>" +
                                                                                                 "<div class='label'></div>" +
                                                                                                 $"<div class='values'> <div style='word-break: break-word;'>+ Người bệnh có khuyết tật gì không? {dataInfo.NguoiBenhCoKhuyetTat?.Replace("\n", "<br>")}</div></div>" +
                                                                                                 "</div>" 
              : "<div class='container'><div class='label'>+ Người bệnh có khuyết tật gì không?</div><div class='value'>" + dataInfo.NguoiBenhCoKhuyetTat + "</div></div>";


            data.TinhTaoTiepXuc = dataInfo.TinhTaoTiepXuc;
            data.Me = dataInfo.Me;
            data.LoMo = dataInfo.LoMo;

            data.TinhTrangHienTaiCuaNguoiBenh = !string.IsNullOrEmpty(dataInfo.TinhTrangHienTaiCuaNguoiBenh) ? "<div class='container'>" +
                                                                                                                 "<div class='label'></div>" +
                                                                                                                 $"<div class='values'> <div style='word-break: break-word;'>{dataInfo.TinhTrangHienTaiCuaNguoiBenh?.Replace("\n", "<br>")}</div></div>" +
                                                                                                                 "</div>" 
                : "<div class='container'><div class='value'>" + dataInfo.TinhTrangHienTaiCuaNguoiBenh + "</div></div>";

            data.KeHoachChamSocVaTheoDoi = !string.IsNullOrEmpty(dataInfo.KeHoachChamSocVaTheoDoi) ? "<div class='container'>" +
                                                                                                                 "<div class='label'> </div>" +
                                                                                                                 $"<div class='values'> <div style='word-break: break-word;'> {dataInfo.KeHoachChamSocVaTheoDoi?.Replace("\n", "<br>")}</div></div>" +
                                                                                                                 "</div>"
                : "<div class='container'><div class='value'>" + dataInfo.KeHoachChamSocVaTheoDoi + "</div></div>";

            data.BSChoVeNha = dataInfo.BSChoVeNha;
            data.ChuyenVien = dataInfo.ChuyenVien;
            data.NangXinVe = dataInfo.NangXinVe;

            data.DanhGiaTinhTrangNBKhiRaVien = !string.IsNullOrEmpty(dataInfo.DanhGiaTinhTrangNBKhiRaVien) ? "<div class='container'>" +
                                                                                                                 "<div class='label'> </div>" +
                                                                                                                 $"<div class='values'> <div style='word-break: break-word;'> {dataInfo.DanhGiaTinhTrangNBKhiRaVien?.Replace("\n", "<br>")}</div></div>" +
                                                                                                                 "</div>" 
                : "<div class='container'><div class='value'>" + dataInfo.DanhGiaTinhTrangNBKhiRaVien + "</div></div>";

            data.DanhGiaTinhTrangNBKhiRaVien = !string.IsNullOrEmpty(dataInfo.DanhGiaTinhTrangNBKhiRaVien) ? "<div class='container'>" +
                                                                                                                 "<div class='label'> </div>" +
                                                                                                                 $"<div class='values'> <div style='word-break: break-word;'> {dataInfo.DanhGiaTinhTrangNBKhiRaVien?.Replace("\n", "<br>")}</div></div>" +
                                                                                                                 "</div>"
                : "<div class='container'><div class='value'>" + dataInfo.DanhGiaTinhTrangNBKhiRaVien + "</div></div>";


            data.HuongDanNBNhungDieuCanThiet = !string.IsNullOrEmpty(dataInfo.HuongDanNBNhungDieuCanThiet) ? "<div class='container'>" +
                                                                                                                 "<div class='label'> </div>" +
                                                                                                                 $"<div class='values'> <div style='word-break: break-word;'> {dataInfo.HuongDanNBNhungDieuCanThiet?.Replace("\n", "<br>")}</div></div>" +
                                                                                                                 "</div>" 
                : "<div class='container'><div class='value'>" + dataInfo.HuongDanNBNhungDieuCanThiet + "</div></div>";

            data.GiayRaVien = dataInfo.GiayRaVien;

            data.DonThuoc = dataInfo.DonThuoc;

            data.BienLaiThanhToanVienPhi = dataInfo.BienLaiThanhToanVienPhi;

            data.GiayChungNhanPhauThuat = dataInfo.GiayChungNhanPhauThuat;

            data.GiayChungSinh = dataInfo.GiayChungSinh;

            data.HenDenKhamLai = !string.IsNullOrEmpty(dataInfo.HenDenKhamLai) ? "<div class='container'>" +
                                                                                        "<div class='label'> </div>" +
                                                                                        $"<div class='values'> <div style='word-break: break-word;'> {dataInfo.HenDenKhamLai?.Replace("\n", "<br>")}</div></div>" +
                                                                                        "</div>"  
                : "<div class='container'><div class='value'>" + dataInfo.HenDenKhamLai + "</div></div>";


            data.NgayHienTai = (queryString.NgayThucHien?.Day > 9 ? queryString.NgayThucHien?.Day + "" : "0" + queryString.NgayThucHien?.Day);

            data.ThangHienTai = (queryString.NgayThucHien?.Month > 9 ? queryString.NgayThucHien?.Month + "" : "0" + queryString.NgayThucHien?.Month);
            data.NamHienTai = queryString.NgayThucHien?.Year +"";

            data.DieuDuongTruongKhoa = dataInfo.DieuDuongTruongKhoa;
            data.DieuDuongChamSocNguoiBenh = dataInfo.DieuDuongChamSocNguoiBenh;




            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
