using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<InfoGiayChungNhanPhauThuatVo> GetThongTinGiayChungNhanPhauThuat(long yeuCauTiepNhan)
        {
            var newPT = new InfoGiayChungNhanPhauThuatVo();

            var cds = _icdRepository.TableNoTracking.Select(d => new {
                Ten = d.Ma + "-" + d.TenTiengViet,
                Id = d.Id
            }).ToList();



            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhan && p.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayChungNhanPhauThuat).FirstOrDefault();

            #region chẩn đoán
            var cd = string.Empty;
            if (noiTruHoSoKhac == null)
            {
                var chanDoanTheoNoiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking
               .Where(o => o.Id == yeuCauTiepNhan)
               .Select(s => new 
               {
                   Id = s.Id,
                   NoiTruPhieuDieuTriIdOrChanDoanKemTheoIds = (s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong || s.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh) ?
                                         s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).Select(d => d.Id.ToString()).FirstOrDefault() :
                                         s.DanhSachChanDoanKemTheoRaVienICDId,
                   ChanDoanICDChinhGhiChuOrDanhSachChanDoanKemTheoRaVienGhiChu = (s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo || s.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong || s.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh) ?
                                         s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri).Select(d => d.ChanDoanChinhGhiChu).FirstOrDefault() :
                                         s.ChanDoanChinhRaVienGhiChu,


                   LoaiBenhAn = s.LoaiBenhAn,
                   NoiTruPhieuDieuTriInfo = s.NoiTruPhieuDieuTris != null ? s.NoiTruPhieuDieuTris.OrderByDescending(c => c.NgayDieuTri)
                                           .Select(d => new NoiTruPhieuDieuTriInfoGridVo
                                           {
                                               NoiTruThamKhamChanDoanKemTheoICDIds = d.NoiTruThamKhamChanDoanKemTheos.Select(f => f.ICDId).ToList(),
                                               //NoiTruThamKhamChanDoanKemTheos = d.NoiTruThamKhamChanDoanKemTheos.Select(f => f.ICDId.ToString()).ToList().Join("|"),
                                               NoiTruBenhAnId = d.NoiTruBenhAnId,
                                               ChanDoanChinhICDId = d.ChanDoanChinhICDId,
                                               ChanDoanChinhGhiChu = d.ChanDoanChinhGhiChu,
                                               NoiTruPhieuDieuTriId = d.Id
                                           }).FirstOrDefault() : new NoiTruPhieuDieuTriInfoGridVo(),
                   ChanDoanChinhRaVienICDId = s.ChanDoanChinhRaVienICDId,
                   ChanDoanChinhRaVienGhiChu = s.ChanDoanChinhRaVienGhiChu,
                   //SoLuuTru = s.SoLuuTru,
                   
               }).FirstOrDefault();

                if (chanDoanTheoNoiTruBenhAn != null)
                {
                    var item = chanDoanTheoNoiTruBenhAn;
                    if (item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaMo ||
                             item.LoaiBenhAn == Enums.LoaiBenhAn.SanKhoaThuong ||
                             item.LoaiBenhAn == Enums.LoaiBenhAn.TreSoSinh)
                    {

                        if (item.NoiTruPhieuDieuTriInfo != null)
                        {
                            var chanDoanKemTheo = string.Empty;
                            // lấy thông tin bệnh an phiếu điều trị ngày cuối cùng
                            //var chanDoanKemTheos = item.NoiTruPhieuDieuTriInfo.NoiTruThamKhamChanDoanKemTheos;
                            //List<string> icdKemTheos = new List<string>();
                            //if (!string.IsNullOrEmpty(chanDoanKemTheos))
                            //{
                            //    var listKemTheos = chanDoanKemTheos.Split("|");
                            //    if (listKemTheos.Length != 0)
                            //    {

                            //        foreach (var icd in listKemTheos)
                            //        {
                            //            icdKemTheos.Add(cds.Where(d => d.Id == long.Parse(icd)).Select(d => d.Ten).FirstOrDefault());
                            //        }
                            //    }
                            //    chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", icdKemTheos);
                            //}
                            var listKemTheos = item.NoiTruPhieuDieuTriInfo.NoiTruThamKhamChanDoanKemTheoICDIds;
                            if (listKemTheos.Any())
                            {                                
                                List<string> icdKemTheos = new List<string>();
                                foreach (var icd in listKemTheos)
                                {
                                    icdKemTheos.Add(cds.Where(d => d.Id == icd).Select(d => d.Ten).FirstOrDefault());
                                }
                                chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", icdKemTheos);
                            }

                            var chanDoanChinhICDId = item.NoiTruPhieuDieuTriInfo.ChanDoanChinhICDId;
                            var chanDoanChinhICDGhiChu = item.NoiTruPhieuDieuTriInfo.ChanDoanChinhGhiChu;

                            if (chanDoanChinhICDId != null)
                            {
                                var chanDoan = chanDoanChinhICDId != null ?
                                              cds.Where(d => d.Id == (long)chanDoanChinhICDId).Select(d => d.Ten).FirstOrDefault() : "";

                                newPT.ChanDoan += (chanDoan != null ? chanDoan + "(" + chanDoanChinhICDGhiChu + ")" : chanDoanChinhICDGhiChu) + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "; " + chanDoanKemTheo : "");
                            }
                            else
                            {
                                newPT.ChanDoan = chanDoanKemTheo;
                            }
                        }


                    }
                    else
                    {
                        if (item.ChanDoanChinhRaVienICDId != null)
                        {
                            var chanDoan = cds.Where(d => d.Id == item.ChanDoanChinhRaVienICDId).Select(d => d.Ten).First();
                            if (!string.IsNullOrEmpty(item.ChanDoanChinhRaVienGhiChu))
                            {
                                newPT.ChanDoan = chanDoan + "(" + item.ChanDoanChinhRaVienGhiChu + ")";
                            }
                            else
                            {
                                newPT.ChanDoan = chanDoan;
                            }
                        }
                        else
                        {
                            newPT.ChanDoan = item.ChanDoanChinhRaVienGhiChu;

                            if (string.IsNullOrEmpty(newPT.ChanDoan))
                            {
                                var ycTiepNhan = await _yeuCauTiepNhanRepository.GetByIdAsync(yeuCauTiepNhan, s => s.Include(p => p.NoiTruHoSoKhacs).Include(d => d.NoiTruBenhAn).ThenInclude(g => g.NoiTruPhieuDieuTris).ThenInclude(h => h.NoiTruThamKhamChanDoanKemTheos).ThenInclude(j => j.ICD));

                                if (ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Any())
                                {
                                    var chanDoanKemTheo = string.Empty;
                                    // lấy thông tin bệnh an phiếu điều trị ngày cuối cùng
                                    var chanDoanKemTheos = ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(d => d.NoiTruThamKhamChanDoanKemTheos.Select(s => (s.ICD.Ma + "-" + s.ICD.TenTiengViet + "(" + s.GhiChu + ")"))).LastOrDefault();

                                    if (chanDoanKemTheos.Any())
                                    {
                                        chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", chanDoanKemTheos);
                                    }
                                    var chanDoanChinhICDId = ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(d => d.ChanDoanChinhICDId).LastOrDefault();
                                    var chanDoanChinhICDGhiChu = ycTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Select(d => d.ChanDoanChinhGhiChu).LastOrDefault();

                                    if (chanDoanChinhICDId != null)
                                    {
                                        var chanDoan = chanDoanChinhICDId != null ?
                                                      ChanDoan((long)chanDoanChinhICDId) : "";

                                        newPT.ChanDoan += chanDoan != null ? chanDoan + "(" + chanDoanChinhICDGhiChu + ")" : chanDoanChinhICDGhiChu;
                                        if (chanDoanKemTheo != null)
                                        {
                                            newPT.ChanDoan += "; " + chanDoanKemTheo;
                                        }
                                    }
                                    else
                                    {
                                        newPT.ChanDoan += chanDoanKemTheo;
                                    }
                                }
                            }
                        }
                    }
                }
                //newPT.SoLuuTru = chanDoanTheoNoiTruBenhAn.SoLuuTru;
                newPT.NoiTruHoSoKhacId = 0;
            }
            else
            {
                if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo))
                {
                    newPT = JsonConvert.DeserializeObject<InfoGiayChungNhanPhauThuatVo>(noiTruHoSoKhac.ThongTinHoSo);
                    if (newPT.TinhTrangRaVienId != 0 && newPT.TinhTrangRaVienId != null)
                    {
                        var tenTinhTrang = _tinhTrangRaVienHoSoKhacRepository.TableNoTracking
                            .Where(d => d.Id == newPT.TinhTrangRaVienId)
                            .Select(d => d.TenTinhTrangRaVien)
                            .FirstOrDefault(); ;
                        newPT.TinhTrangRaVienText = tenTinhTrang;
                    }
                    if (newPT.GiamDocChuyenMonId != 0 && newPT.GiamDocChuyenMonId != null)
                    {
                        newPT.GiamDocChuyenMon = _nhanVienRepository.TableNoTracking
                           .Where(d => d.Id == newPT.GiamDocChuyenMonId).Select(d => d.User.HoTen).FirstOrDefault();
                    }
                    if (newPT.TruongKhoaId != 0 && newPT.TruongKhoaId != null)
                    {
                        newPT.TruongKhoa = _nhanVienRepository.TableNoTracking
                            .Where(d => d.Id == newPT.TruongKhoaId).Select(d => d.User.HoTen).FirstOrDefault();
                    }
                    if (newPT.DichVuPTTTId != 0 && newPT.DichVuPTTTId != null)
                    {
                        newPT.DichVuPTTTText = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(d => d.Id == newPT.DichVuPTTTId).Select(d => d.TenDichVu).FirstOrDefault();
                    }
                }
                newPT.NoiTruHoSoKhacId = noiTruHoSoKhac.Id;
            }
            #endregion chan đoán
            return newPT;
        }
     
        public async Task<List<LookupItemVo>> GetListDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(DropDownListRequestModel model, long yctnId)
        {
            var lstYeuCauDichVuKyThuats = await _yeuCauTiepNhanRepository.TableNoTracking
                .Where(d => d.Id == yctnId).SelectMany(d => d.YeuCauDichVuKyThuats).ToListAsync();
            var lstIdDichVuPTTT = new List<long>();

            

            if (lstYeuCauDichVuKyThuats != null)
            {
                var dichVuDaTuongTrinhIds = lstYeuCauDichVuKyThuats.Select(g => g.Id).ToList();
                 lstIdDichVuPTTT = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(d => dichVuDaTuongTrinhIds.Contains(d.Id) &&
                               d.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                               d.YeuCauDichVuKyThuatTuongTrinhPTTT != null).Select(d => d.Id).ToList();
            }
            var result = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(d => lstIdDichVuPTTT.Contains(d.Id))
                .ApplyLike(model.Query, g => g.TenDichVu)
                .Take(model.Take)
                .ToListAsync();

            var listInfos = result.Select(item => new LookupItemVo()
            {
                DisplayName = item.TenDichVu,
                KeyId = item.Id,
            }).ToList();

            return listInfos;
        }
        public async Task<InfoYeuCauDichVuKyThuatTheoBenNhanVo> GetInfoDichVuKyThuatThuocPhauThuatThuThuatCuaBenhNhan(long ycdvktId)
        {

            var rrr = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(d => d.Id == ycdvktId).Select(item => item.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .FirstOrDefault();

            var result = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(d => d.Id == ycdvktId)
                .Select(item => new InfoYeuCauDichVuKyThuatTheoBenNhanVo
                {
                    KeyId = ycdvktId,
                    DisplayName = item.TenDichVu,
                    PhuongThucVoCamId = item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCamId,
                    PhuongThucVoCamText = item.YeuCauDichVuKyThuatTuongTrinhPTTT.PhuongPhapVoCam.Ten,
                    //PhauThuatVienId = item.NhanVienThucHienId,
                    //PhauThuatVienText = item.NhanVienThucHien.User.HoTen,
                    PhauThuatNgay = item.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemPhauThuat,
                    MaPhuongPhapPTTT = item.YeuCauDichVuKyThuatTuongTrinhPTTT.MaPhuongPhapPTTT,
                    YeuCauDichVuKyThuatTuongTrinhPTTTId = item.YeuCauDichVuKyThuatTuongTrinhPTTT.Id
                }).FirstOrDefault();

            var query = _phauThuatThuThuatEkipBacSiRepository.TableNoTracking
                 .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == result.YeuCauDichVuKyThuatTuongTrinhPTTTId && x.VaiTroBacSi != null)
                 .Select(x => new
                 {
                     Id = x.NhanVienId,
                     HoTen = x.NhanVien.User.HoTen,
                     VaiTro = x.VaiTroBacSi,
                 }).ToList();
            
            var bacSiChinh = query.Where(d => d.VaiTro == Enums.EnumVaiTroBacSi.PhauThuatVienChinh).FirstOrDefault();
            if (bacSiChinh != null)

            {
                result.PhauThuatVienId = bacSiChinh.Id;
                result.PhauThuatVienText = bacSiChinh.HoTen;
            }
            else
            {
                var queryDieuDuong = _phauThuatThuThuatEkipDieuDuongRepository.TableNoTracking
               .Where(x => x.YeuCauDichVuKyThuatTuongTrinhPTTTId == result.YeuCauDichVuKyThuatTuongTrinhPTTTId && x.VaiTroDieuDuong != null)
               .Select(x => new
               {
                   Id = x.NhanVienId,
                   HoTen = x.NhanVien.User.HoTen,
                   VaiTro = x.VaiTroDieuDuong,
               }).ToList();
                var dieuDuongChinh = queryDieuDuong.Where(d => d.VaiTro == Enums.EnumVaiTroDieuDuong.PhauThuatVienChinh).FirstOrDefault();

                if (dieuDuongChinh != null)

                {
                    result.PhauThuatVienId = dieuDuongChinh.Id;
                    result.PhauThuatVienText = dieuDuongChinh.HoTen;
                }
            }
            //if (!string.IsNullOrEmpty(result.MaPhuongPhapPTTT))
            //{
            //    var listStrings = result.MaPhuongPhapPTTT.Split("|");
            //    foreach (var item in listStrings)
            //    {
            //        //result.PhuongThucPhauThuats.Add(item.Replace("/n",""));
            //    }
            //}
            //if(phuongThucPhauThuats.Count() != 0)
            //{
            //    var tens = _dichVuKyThuatRepository.TableNoTracking.Where(d => phuongThucPhauThuats.Contains(d.MaChung))
            //        .Select(d => d.TenChung)
            //        .ToList();
            //    result.PhuongThucPhauThuats.AddRange(tens);
            //}

            return result;
        }
        public async Task<List<LookupItemVo>> GetListTinhTrangRaVienHoSoKhac(DropDownListRequestModel model)
        {
            var lstChucDanh = await _tinhTrangRaVienHoSoKhacRepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.TenTinhTrangRaVien)
                 .Take(model.Take)
                 .ToListAsync();

            var query = lstChucDanh.Select(item => new LookupItemVo()
            {
                DisplayName = item.TenTinhTrangRaVien,
                KeyId = item.Id,
            }).ToList();
            return query;
        }
        public bool KiemTraTinhTrangExit(string tinhTrang)
        {
            var result = _tinhTrangRaVienHoSoKhacRepository.TableNoTracking
                .Where(d => d.TenTinhTrangRaVien.ToLower().Trim() == tinhTrang.ToLower().Trim()).ToList();
            return result.Count() != 0 ? false : true;
        }
        public async Task<string> InGiayChungNhanPhauThuat(long yeuCauTiepNhanId)
        {
            

            var infoBn = _yeuCauTiepNhanRepository.TableNoTracking.Where(d => d.Id == yeuCauTiepNhanId)
                .Select(d => new { 
                    HoTen = d.HoTen, 
                    NgaySinh = d.NgaySinh,
                    ThangSinh = d.ThangSinh,
                    NamSinh = d.NamSinh,
                    DiaChi = d.DiaChiDayDu,
                    VaoVienNgay= d.NoiTruBenhAn.ThoiDiemNhapVien,
                    RaVienNgay = d.NoiTruBenhAn.ThoiDiemRaVien,
                    NhomMau = d.NhomMau,
                    YeuToRh = d.YeuToRh,
                    ThongTinRaVien = d.NoiTruBenhAn.ThongTinRaVien
                }).First();

            var ns = DateHelper.DOBFormat(infoBn.NgaySinh, infoBn.ThangSinh, infoBn.NamSinh);


            var infoSavejson = GetThongTinGiayChungNhanPhauThuat(yeuCauTiepNhanId);

            var data = new InfoGiayChungNhanPhauThuat();

            var thoiDiemRaVien = new DateTime();
            if (!string.IsNullOrEmpty(infoBn.ThongTinRaVien))
            {
                var jsonThongTinRaVien = JsonConvert.DeserializeObject<InfoThoiDiemRaVien>(infoBn.ThongTinRaVien);
                data.RaVienNgay = jsonThongTinRaVien.ThoiGianRaVien != null ? jsonThongTinRaVien.ThoiGianRaVien.Value.ApplyFormatDate() :"";
            }

            data.ChungNhan = infoBn.HoTen ;
            data.DiaChi = infoBn.DiaChi;
            data.VaoVienNgay = infoBn.VaoVienNgay.ApplyFormatDate();
            //data.RaVienNgay = infoBn.RaVienNgay?.ApplyFormatDate();

            data.SoLuuTru = !string.IsNullOrEmpty(infoSavejson.Result.SoLuuTru) ? infoSavejson.Result.SoLuuTru : "...............";
            if(!string.IsNullOrEmpty(infoSavejson?.Result?.ChanDoan))
            {
                data.ChanDoanBenhNhan = infoSavejson.Result.ChanDoan.Replace("\n","<br>");
            }
           

            data.NhomMau = "&nbsp;" + infoBn.NhomMau?.GetDescription();
            data.YeuToRh = "&nbsp;" + infoBn.YeuToRh?.GetDescription();

            if (!string.IsNullOrEmpty(infoSavejson.Result.PhauThuatNgayText))
            {
                DateTime.TryParseExact(infoSavejson.Result.PhauThuatNgayText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                data.PhauThuatNgay = tuNgay.ApplyFormatDate();
            }

            data.PhuongThucVoCam = "&nbsp;" + infoSavejson.Result.PhuongThucVoCamText;

            data.PhauThuatVien = infoSavejson.Result.PhauThuatVienText;

            data.TinhTrangLucRaVien = infoSavejson.Result.TinhTrangRaVienText;

            if (infoSavejson.Result.TruongKhoaId != null)
            {
                var hocham = _nhanVienRepository.TableNoTracking
                            .Where(d => d.Id == infoSavejson.Result.TruongKhoaId && d.HocHamHocViId != null).Select(d => d.HocHamHocVi.Ten).FirstOrDefault();
                data.TruongKhoa = !string.IsNullOrEmpty(hocham) ? hocham  + " " + infoSavejson.Result.TruongKhoa : infoSavejson.Result.TruongKhoa;

            }
            if (infoSavejson.Result.GiamDocChuyenMonId != null)
            {
                var hocham = _nhanVienRepository.TableNoTracking
                          .Where(d => d.Id == infoSavejson.Result.GiamDocChuyenMonId && d.HocHamHocViId != null).Select(d => d.HocHamHocVi.Ten).FirstOrDefault();
                data.GiamDocCM = !string.IsNullOrEmpty(hocham) ? hocham + " " + infoSavejson.Result.GiamDocChuyenMon: infoSavejson.Result.GiamDocChuyenMon;
            }

           
            if (!string.IsNullOrEmpty(infoSavejson.Result.PhuongThucPhauThuat))
            {
                data.PhuongThucPhauThuat = infoSavejson.Result.PhuongThucPhauThuat?.Replace("\n","<br>");
            }
          

            // data.KetQuaGPB = infoSavejson.Result.ketQuaGDP
            if (!string.IsNullOrEmpty(infoSavejson.Result.NgayThangNamText))
            {
                DateTime.TryParseExact(infoSavejson.Result.NgayThangNamText, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                data.Ngay = tuNgay.Day.ToString();
                data.Thang = tuNgay.Month.ToString();
                data.Nam = tuNgay.Year.ToString();
            }
            else
            {
                data.Ngay = "&nbsp;&nbsp;&nbsp;";
                data.Thang = "&nbsp;&nbsp;&nbsp;";
                data.Nam = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            

            if (!string.IsNullOrEmpty(infoBn.ThongTinRaVien))
            {
                var thongTinRaVien = JsonConvert.DeserializeObject<InfoThongTinRaVien>(infoBn.ThongTinRaVien);
                data.KetQuaGPB = thongTinRaVien.TenGiaPhauThuat;
            }



            data.NgayThangNamSinh = ns;


            var result1 = _templateRepository.TableNoTracking
                       .FirstOrDefault(x => x.Name.Equals("PhieuChungNhanPhauThuat1"));

            var content = string.Empty;
            var contentPhieu1 = string.Empty;
            var contentPhieu2 = string.Empty;
            contentPhieu1 = TemplateHelpper.FormatTemplateWithContentTemplate(result1.Body, data);

          

            var result2 = _templateRepository.TableNoTracking
                      .FirstOrDefault(x => x.Name.Equals("PhieuChungNhanPhauThuat2"));
            contentPhieu2 += TemplateHelpper.FormatTemplateWithContentTemplate(result2.Body, data);

            content+= "<div width='100%'>";
            content += contentPhieu1;
            content += "</div>";
            content += "<div class='pagebreak'> </div>";
            content += "<div width='100%'>";
            content += contentPhieu2;
            content += "</div>";
 
            return content;
        }
    }
}
