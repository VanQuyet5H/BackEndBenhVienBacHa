using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Newtonsoft.Json;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Camino.Core.Domain.ValueObject.ToaThuocMau;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<string> GetTenNhanVien(long? nhanVienId)
        {
            if (nhanVienId != null && nhanVienId != 0)
            {
                return await _nhanVienRepository.TableNoTracking.Where(p => p.Id == nhanVienId).Select(p => p.User.HoTen).FirstOrDefaultAsync();
            }
            return string.Empty;
        }

        public async Task<ThongTinNhanVienDangNhap> ThongTinNhanVienDangNhapId(long id)
        {
            var nhanVienId = _userAgentHelper.GetCurrentUserId();
            var tenNhanVien = await _nhanVienRepository.TableNoTracking.Where(p => p.Id == nhanVienId).Select(p => p.User.HoTen).FirstAsync();
            var noitruBA = await _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == id).FirstOrDefaultAsync();

            var thongTinTongKetBAJSON = new PhuongPhapDieuTriJSON();

            if (!string.IsNullOrEmpty(noitruBA.ThongTinTongKetBenhAn))
            {
                thongTinTongKetBAJSON = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noitruBA.ThongTinTongKetBenhAn);
            }

            var thongTinRaVienJSON = new PhuongPhapDieuTriJSON();
            if (!string.IsNullOrEmpty(noitruBA.ThongTinRaVien))
            {
                thongTinRaVienJSON = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noitruBA.ThongTinRaVien);
            }
            var noiTruHoSoKhac = await _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == id && p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.GiayRaVien).FirstOrDefaultAsync();

            #region chẩn đoán
            var cd = string.Empty;
            if (noiTruHoSoKhac == null)
            {
                var ycTiepNhan = await _yeuCauTiepNhanRepository.GetByIdAsync(id, s => s.Include(p => p.NoiTruHoSoKhacs).Include(d => d.NoiTruBenhAn).ThenInclude(g => g.NoiTruPhieuDieuTris).ThenInclude(h => h.NoiTruThamKhamChanDoanKemTheos).ThenInclude(j => j.ICD));

                // kiểm tra loại bệnh án để lấy icd
                // Chẩn đoán link theo nội dung ICD ra viện, riêng Khoa Phụ Sản lấy theo ICD chính/phụ của ngày điều trị cuối cùng t
                var loaiThongTinBenhAn = GetThongTinLoaiBenhAn(id);
                if ((LoaiBenhAn)loaiThongTinBenhAn != LoaiBenhAn.SanKhoaMo && (LoaiBenhAn)loaiThongTinBenhAn != LoaiBenhAn.SanKhoaThuong)
                {
                    // lấy icd ra viện 
                    if (ycTiepNhan.NoiTruBenhAn != null)
                    {
                        var chanDoan = ycTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienICDId != null ?
                                              ChanDoan((long)ycTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienICDId) : "";
                        cd = !string.IsNullOrEmpty(chanDoan)   ? chanDoan + "(" + ycTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu + ")" : ycTiepNhan.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu;
                    }

                }
                else
                {
                    // icd chính phụ  last ngày điều trị
                    if (ycTiepNhan.NoiTruBenhAn != null)
                    {
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

                                cd += chanDoan != null ? chanDoan + "(" + chanDoanChinhICDGhiChu + ")" : chanDoanChinhICDGhiChu;
                                if (chanDoanKemTheo != null)
                                {
                                   cd += "; " + chanDoanKemTheo;
                                }
                            }
                            else
                            {
                               cd += chanDoanKemTheo;
                            }
                        }

                    }
                }
              
                /* return NoContent()*/
            }
            #endregion chan đoán
            if (noiTruHoSoKhac != null)
            {
                var phuongPhapDieuTriJSON = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTruHoSoKhac.ThongTinHoSo);
                return new ThongTinNhanVienDangNhap
                {
                    NhanVienDangNhapId = nhanVienId,
                    NhanVienDangNhap = tenNhanVien,
                    NgayThucHien = DateTime.Now,
                    PhuongPhapDieuTri = phuongPhapDieuTriJSON.PhuongPhapDieuTri,
                    ChanDoan = phuongPhapDieuTriJSON.ChanDoan  // phuongPhapDieuTriJSON.ChanDoan
                };
            }
            else
            {
                return new ThongTinNhanVienDangNhap
                {
                    NhanVienDangNhapId = nhanVienId,
                    NhanVienDangNhap = tenNhanVien,
                    NgayThucHien = DateTime.Now,
                    PhuongPhapDieuTri = thongTinTongKetBAJSON.PhuongPhapDieuTri,
                    ChanDoan = cd //thongTinRaVienJSON.GhiChuChuanDoanRaVien,
                };
            }
        }

        public string InGiayRaVien(long noiTruHoSoKhacId)
        {
            var content = string.Empty;
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayRaVienNoiTru")).First();
            var noiTruHoSoKhac = _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.Id == noiTruHoSoKhacId).FirstOrDefault();
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Include(p => p.NoiTruBenhAn).ThenInclude(p => p.KhoaPhongNhapVien)
                .Include(p => p.NoiTruBenhAn).ThenInclude(p=>p.NoiTruKhoaPhongDieuTris).ThenInclude(p=>p.KhoaPhongChuyenDen)
                .Include(z => z.YeuCauTiepNhanTheBHYTs)
                .Include(z => z.BenhNhan)
                .Include(p => p.DanToc).Include(p => p.NgheNghiep).Where(p => p.Id == noiTruHoSoKhac.YeuCauTiepNhanId).FirstOrDefault();
            PhuongPhapDieuTriJSON thongTinHoSo = null;
            if (!string.IsNullOrEmpty(noiTruHoSoKhac.ThongTinHoSo))
            {
                thongTinHoSo = JsonConvert.DeserializeObject<PhuongPhapDieuTriJSON>(noiTruHoSoKhac.ThongTinHoSo);
            }
            RaVien thongTinRaVien = null;
            if (!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien))
            {
                thongTinRaVien = JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien);
                if (thongTinRaVien.ThoiGianRaVien == null)
                {
                    throw new Exception(_localizationService.GetResource("GiayRaVien.ThoiGianRaVien.Required"));
                }
            }
            var tmp = "\n";
            var replace = "<br> &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;";
            string ghiChu =string.Empty;
          
            if (!string.IsNullOrEmpty(thongTinHoSo.GhiChu))
            {
                ghiChu = thongTinHoSo.GhiChu.Replace(tmp, replace);
            }
            var namSinh = string.Empty;
            if((yeuCauTiepNhan.NamSinh != null && yeuCauTiepNhan.NamSinh != 0) && (yeuCauTiepNhan.ThangSinh != null && yeuCauTiepNhan.ThangSinh != 0) && (yeuCauTiepNhan.NgaySinh != null && yeuCauTiepNhan.NgaySinh != 0))
            {
                namSinh =(yeuCauTiepNhan.NgaySinh < 10 ? "0"+ yeuCauTiepNhan.NgaySinh: yeuCauTiepNhan.NgaySinh.ToString()) + "/" + (yeuCauTiepNhan.ThangSinh < 10 ? "0" + yeuCauTiepNhan.ThangSinh : yeuCauTiepNhan.ThangSinh.ToString()) + "/" + yeuCauTiepNhan.NamSinh;
            }
            else if((yeuCauTiepNhan.NamSinh != null && yeuCauTiepNhan.NamSinh != 0))
            {
                namSinh = yeuCauTiepNhan.NamSinh.ToString();
            }

            // mã số BHYT
            // bệnh nhân ra viện  => kiểm tra BHYT bệnh nhân hiệu lực so với Thời gian ra viện => còn thì htBHYT  else thì empty
            // bệnh nhân chưa ra viện = > kiểm tra BHYT bệnh nhân hiệu lực so với thời gian hiện tại => còn thì htBHYT  else thì empty
            var kiemTraBenhNhanRaVienLayMaSoBH = string.Empty;
            // trường họp kết thúc BA
            if (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien != null)
            {
                if (yeuCauTiepNhan.BHYTNgayHetHan!=null && ((DateTime)yeuCauTiepNhan.BHYTNgayHetHan).Date >= ((DateTime)yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien).Date)
                {
                    kiemTraBenhNhanRaVienLayMaSoBH = yeuCauTiepNhan.BHYTMaSoThe;
                }
                else
                {
                    kiemTraBenhNhanRaVienLayMaSoBH = string.Empty;
                }
            }
            else
            {
                // trường hợp mở BA
                if (!string.IsNullOrEmpty(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien))
                {
                    var thongTinRaVienModel = JsonConvert.DeserializeObject<RaVien>(yeuCauTiepNhan.NoiTruBenhAn.ThongTinRaVien);
                    if (thongTinRaVienModel != null && thongTinRaVienModel.ThoiGianRaVien != null &&
                        yeuCauTiepNhan.BHYTNgayHetHan != null && ((DateTime)yeuCauTiepNhan.BHYTNgayHetHan).Date >= ((DateTime)thongTinRaVienModel.ThoiGianRaVien).Date)
                    {
                        kiemTraBenhNhanRaVienLayMaSoBH = yeuCauTiepNhan.BHYTMaSoThe;
                    }
                    else
                    {   // trường hợp BA k có thời gian ra viện
                        if (yeuCauTiepNhan.BHYTNgayHetHan != null && ((DateTime)yeuCauTiepNhan.BHYTNgayHetHan).Date >= DateTime.Now.Date)
                        {
                            kiemTraBenhNhanRaVienLayMaSoBH = yeuCauTiepNhan.BHYTMaSoThe;
                        }
                        else
                        {
                            kiemTraBenhNhanRaVienLayMaSoBH = string.Empty;
                        }
                    }
                }
                else
                {
                    if (yeuCauTiepNhan.BHYTNgayHetHan != null && ((DateTime)yeuCauTiepNhan.BHYTNgayHetHan).Date >= DateTime.Now.Date)
                    {
                        kiemTraBenhNhanRaVienLayMaSoBH = yeuCauTiepNhan.BHYTMaSoThe;
                    }
                    else
                    {
                        kiemTraBenhNhanRaVienLayMaSoBH = string.Empty;
                    }
                }

            }

            var data = new GiayRaVienVo
            {
                Khoa = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten,
                MaBN = yeuCauTiepNhan.BenhNhan.MaBN,
                SoBA = yeuCauTiepNhan.NoiTruBenhAn.SoBenhAn,
                HoTenBenhNhan = yeuCauTiepNhan.HoTen,
                GioiTinh = yeuCauTiepNhan.GioiTinh.GetDescription(),
                NamSinh = namSinh,
                DanToc = yeuCauTiepNhan.DanToc?.Ten,
                NgheNghiep = yeuCauTiepNhan.NgheNghiep?.Ten,
                BHYTMaSoThe = kiemTraBenhNhanRaVienLayMaSoBH,
                DiaChi = yeuCauTiepNhan.DiaChiDayDu,
                NgayThangNamGioVaoVien = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien.ConvertDatetimeToString(false),
                NgayThangNamGioRaVien = thongTinRaVien?.ThoiGianRaVien?.ConvertDatetimeToString(false),
                ChanDoan = thongTinHoSo?.ChanDoan,
                PPDieuTri = thongTinHoSo?.PhuongPhapDieuTri,
                GhiChu = ghiChu,
                Ngay = thongTinRaVien?.ThoiGianRaVien?.Day.ConvertDateToString(),
                Thang = thongTinRaVien?.ThoiGianRaVien?.Month.ConvertMonthToString(),
                Nam = thongTinRaVien?.ThoiGianRaVien?.Year.ConvertYearToString(),
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        public string ChanDoan(long idICD)
        {
            var cd = _icdRepository.TableNoTracking.Where(d => d.Id == idICD).Select(d => (d.Ma + "-" + d.TenTiengViet));
            return cd.FirstOrDefault();
        }
        public async Task<List<LookupItemVo>> GetGhiChuGiayRaVien(DropDownListRequestModel queryInfo)
        {

            //if(!string.IsNullOrEmpty(queryInfo.Query))
            //{
            //    queryInfo.Query = queryInfo.Query.Replace(" ", "\n");
            //    var lstValuess = await _inputStringStoredRepository.TableNoTracking
            //            .Where(p => p.Set == Enums.InputStringStoredKey.GhiChuGiayRaVien)
            //            .ApplyLike(queryInfo.Query, o => o.Value.Replace(" ", "\n"))
            //            .Take(queryInfo.Take).ToListAsync();

            //    var querys = lstValuess.Select(item => new LookupItemVo()
            //    {
            //        DisplayName = item.Value,
            //        KeyId = item.Id,
            //    });

            //    return querys.ToList();
            //}
            var test = queryInfo.Query !="" ? queryInfo.Query.Replace(" ", "\n"): queryInfo.Query;
            var lstValues = await _inputStringStoredRepository.TableNoTracking
                        .Where(p => p.Set == Enums.InputStringStoredKey.GhiChuGiayRaVien)
                        .ApplyLike(test, o => o.Value.Replace(" ", "\n"))
                        .Take(queryInfo.Take).ToListAsync();

            var query = lstValues.Select(item => new LookupItemVo()
            {
                DisplayName = item.Value,
                KeyId = item.Id,
            });

            return query.ToList();
        }
        public bool KiemTraGhiChuRaVienTonTai(string value)
        {
            var query = _inputStringStoredRepository.TableNoTracking.Where(d => d.Value == value).ToList() ;
            return query.Any() ? true : false;
        }
        public async Task<string> GetGhiChu(long id)
        {
            var listgc = _inputStringStoredRepository.TableNoTracking
                .Where(p => p.Set == Enums.InputStringStoredKey.GhiChuGiayRaVien && p.Id == id).Select(d => d.Value);
                
            return listgc.FirstOrDefault();
           
        }
        public async Task<List<NhanVienTemplateVos>> GetGiamDocChuyenMons(DropDownListRequestModel queryInfo)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.Users.User.HoTen),
                nameof(Core.Domain.Entities.Users.User.SoDienThoai),

            };
            var giamDocChuyenMon = 1;
            if (!string.IsNullOrEmpty(queryInfo.Query) && !queryInfo.Query.Contains(" ") || string.IsNullOrEmpty(queryInfo.Query))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var nhanViens = _nhanVienRepository.TableNoTracking
                    .Where(p => p.User.SoDienThoai != "0000000000" && p.NhanVienChucVus.Select(d=>d.ChucVuId).Contains(giamDocChuyenMon))
                    .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0
                    || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                    .Select(item => new NhanVienTemplateVos
                    {
                        DisplayName = item.User.HoTen,
                        KeyId = item.Id,
                    })
                    .ApplyLike(queryInfo.Query, o => o.DisplayName)
                    .Take(queryInfo.Take);

                    return await nhanViens.ToListAsync();
                }
                else
                {
                    var nhanViens = _nhanVienRepository.TableNoTracking
                    .Where(p => p.User.SoDienThoai != "0000000000" && p.NhanVienChucVus.Select(d => d.ChucVuId).Contains(giamDocChuyenMon))

                    .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                    .Select(item => new NhanVienTemplateVos
                    {
                        DisplayName = item.User.HoTen,
                        KeyId = item.Id,
                    })
                    .ApplyLike(queryInfo.Query, o => o.DisplayName)
                    .Take(queryInfo.Take);


                    return await nhanViens.ToListAsync();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var nhanVienIds = _nhanVienRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                               .Where(p => p.User.SoDienThoai != "0000000000" && p.NhanVienChucVus.Select(d => d.ChucVuId).Contains(giamDocChuyenMon))
                               .Select(p => p.Id).ToList();

                    var dictionary = nhanVienIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var nhanViens = _nhanVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                        .OrderByDescending(x => long.Parse(queryInfo.ParameterDependencies) == 0 || x.Id == long.Parse(queryInfo.ParameterDependencies)).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new NhanVienTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.User.HoTen,
                                            KeyId = item.Id,
                                            Ten = item.User.HoTen,
                                            SoDienThoai = item.User.SoDienThoai,
                                        });
                    return await nhanViens.ToListAsync();
                }
                else
                {
                    var nhanVienIds = _nhanVienRepository
                               .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                               .Where(p => p.User.SoDienThoai != "0000000000" && p.NhanVienChucVus.Select(d => d.ChucVuId).Contains(giamDocChuyenMon))
                               .Select(p => p.Id).ToList();

                    var dictionary = nhanVienIds.Select((id, index) => new
                    {
                        key = id,
                        rank = index,
                    }).ToDictionary(o => o.key, o => o.rank);

                    var nhanViens = _nhanVienRepository
                                        .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.Users.User), lstColumnNameSearch)
                                        .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                        .Take(queryInfo.Take)
                                        .Select(item => new NhanVienTemplateVos
                                        {
                                            Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                                            DisplayName = item.User.HoTen,
                                            KeyId = item.Id,
                                            Ten = item.User.HoTen,
                                            SoDienThoai = item.User.SoDienThoai
                                        });

                    return await nhanViens.ToListAsync();
                }
            }
        }
    }
}
