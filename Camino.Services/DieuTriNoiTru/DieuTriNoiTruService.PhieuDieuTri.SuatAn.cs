using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task<GridDataSource> GetDataForGridAsyncSuatAn(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId
                && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SuatAn)
                .Select(s => new PhieuDieuTriSuatAnGridVo
                {
                    Id = s.Id,
                    Ma = s.MaDichVu,
                    Ten = s.TenDichVu,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    BuaAn = s.BuaAn,
                    DonGia = s.Gia,
                    SoLan = s.SoLan,
                    DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVienId,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var searchTerms = queryInfo.SearchTerms.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.Ten
               );
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsyncSuatAn(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId
                && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SuatAn)
                .Select(s => new PhieuDieuTriSuatAnGridVo
                {
                    Id = s.Id,
                    Ma = s.MaDichVu,
                    Ten = s.TenDichVu,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    DonGia = s.Gia,
                    SoLan = s.SoLan,
                    DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVienId,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh

                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var searchTerms = queryInfo.SearchTerms.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms,
                     g => g.Ten
               );
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetSuatAn(DropDownListRequestModel model)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien.Ten),
            };
            var lst = new List<LookupItemVo>();
            var suatAnId = (long)LoaiDichVuKyThuat.SuatAn;
            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                lst = await _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(p => p.NhomDichVuBenhVienId == suatAnId)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .Take(model.Take).ToListAsync();
            }
            else
            {
                lst = await _dichVuKyThuatBenhVienRepository
                    .ApplyFulltext(model.Query, nameof(Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien)
                    , lstColumnNameSearch)
                    .Where(p => p.NhomDichVuBenhVienId == suatAnId)
                    .Select(item => new LookupItemVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                    })
                    .ToListAsync();
                //lst = await _mauVaChePhamRepository.TableNoTracking
                //    .Where(p => lstId.Contains(p.Id))
                //    .Take(model.Take)

                //     .OrderBy(lstId)
                //     .ToListAsync();
            }
            return lst;
        }

        public Task<List<LookupItemVo>> GetDoiTuongSuatAn(DropDownListRequestModel model)
        {
            var listEnum = EnumHelper.GetListEnum<Enums.DoiTuongSuDung>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();

            return Task.FromResult(result);
        }

        public async Task<YeuCauTiepNhan> ThemSuatAn(YeuCauTiepNhan yctn, long noiTruPhieuDieuTriId, Enums.DoiTuongSuDung DoiTuongId, int SoLan, long DichVuKyThuatBenhVienId, BuaAn? BuaAn)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentPhongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var ngayDieuTri = yctn.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == noiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;


            var dvkt = await GetDichVuKyThuatBenhVien(DichVuKyThuatBenhVienId);

            var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
            if (dvktGiaBV == null)
            {
                throw new Exception("Dịch vụ này đã hết hạn");
            }
            var dtudDVKTBV = yctn.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                                        o.DichVuKyThuatBenhVienId == DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);


            var yeuCauDVKT = new YeuCauDichVuKyThuat
            {
                NoiTruPhieuDieuTriId = noiTruPhieuDieuTriId,
                LoaiDichVuKyThuat = LoaiDichVuKyThuat.SuatAn,
                BuaAn = BuaAn,
                DichVuKyThuatBenhVienId = DichVuKyThuatBenhVienId,
                NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId,

                MaDichVu = dvkt.Ma,
                TenDichVu = dvkt.Ten,
                Gia = dvktGiaBV.Gia,
                SoLan = SoLan,
                TiLeUuDai = dtudDVKTBV?.TiLeUuDai,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                NhanVienChiDinhId = currentUserId,
                NoiChiDinhId = currentPhongLamViecId,
                ThoiDiemChiDinh = DateTime.Now,
                ThoiDiemDangKy = ngayDieuTri.Date == DateTime.Now.Date ? DateTime.Now : ngayDieuTri,
                NhomDichVuBenhVienId = dvkt.NhomDichVuBenhVienId,
                MaGiaDichVu = dvkt.DichVuKyThuat?.MaGia,
                TenGiaDichVu = dvkt.DichVuKyThuat?.TenGia,
                DoiTuongSuDung = DoiTuongId
            };

            yctn.YeuCauDichVuKyThuats.Add(yeuCauDVKT);

            return yctn;
        }

        public async Task<Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetDichVuKyThuatBenhVien(long id)
        {
            var dvkt = await _dichVuKyThuatBenhVienRepository.GetByIdAsync(id, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(o => o.DichVuKyThuat));

            return dvkt;
        }

        public async Task<string> InPhieuSuatAn(XacNhanInPhieuSuatAn xacNhanIn)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Include(x => x.BenhNhan)
                .Include(x => x.NoiTruBenhAn).ThenInclude(xx => xx.NoiTruPhieuDieuTris)
                .ThenInclude(xxx => xxx.YeuCauVatTuBenhViens)
                .Include(x => x.YeuCauTiepNhanTheBHYTs)
                 .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(c => c.GiuongBenh)
                .Include(x => x.NoiTruBenhAn).ThenInclude(xx => xx.NoiTruKhoaPhongDieuTris).ThenInclude(c => c.KhoaPhongChuyenDen)
                .Include(x => x.NoiTruBenhAn).ThenInclude(xx => xx.NoiTruPhieuDieuTris).ThenInclude(xxx => xxx.ChanDoanChinhICD)
                .Where(p => p.Id == xacNhanIn.YeuCauTiepNhanId).First();

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuInSuatAn"));

            var dienGiaiChanDoanSoBo = new List<string>();
            var chanDoanSoBos = new List<string>();

            long userId = _userAgentHelper.GetCurrentUserId();
            var tenNguoiChiDinh = _useRepository.TableNoTracking.Where(p => p.Id == userId).Select(p => p.HoTen).FirstOrDefault();

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;


            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NỘI DUNG</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>BỮA ĂN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>ĐƠN GIÁ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>BN PHẢI TRẢ</th>";
            htmlDanhSachDichVu += "</tr>";

            var i = 1;
            var content = string.Empty;

            var phieuSuatAns = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                                .Where(o => o.YeuCauTiepNhanId == xacNhanIn.YeuCauTiepNhanId
                                                  && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                  && o.NoiTruPhieuDieuTriId == xacNhanIn.PhieuDieuTriId && o.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SuatAn)
                                                 .Include(c => c.DichVuKyThuatBenhVien).ThenInclude(c => c.NhomDichVuBenhVien).ThenInclude(c => c.NhomDichVuBenhVienCha)
                                                 .Include(c => c.DichVuKyThuatBenhVien).ThenInclude(c => c.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(c => c.PhongBenhVien)
                                                 .Include(c => c.DichVuKyThuatBenhVien).ThenInclude(c => c.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(c => c.KhoaPhong)
                                                 .Include(c => c.NoiTruPhieuDieuTri).ThenInclude(c => c.ChanDoanChinhICD)
                                                 .ToList();
            if (xacNhanIn.DichVuDuocChon.Any())
            {
                phieuSuatAns = phieuSuatAns.Where(c => xacNhanIn.DichVuDuocChon.Contains(c.Id)).ToList();
            }

            if (!phieuSuatAns.Any())
            {
                return null;
            }
            var dsPhieuAns = new List<InDanhSachSuatAn>();
            if (yeuCauTiepNhan != null)
            {
                if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru)
                {
                    dienGiaiChanDoanSoBo.Add(yeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.YeuCauKhamBenhs.LastOrDefault()?.ChanDoanSoBoGhiChu);
                    if (yeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.YeuCauKhamBenhs.LastOrDefault()?.ChanDoanSoBoICD != null)
                    {
                        chanDoanSoBos.Add(yeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.YeuCauKhamBenhs.LastOrDefault()?.ChanDoanSoBoICD?.Ma + "-" + yeuCauTiepNhan.NoiTruBenhAn.KhoaPhongNhapVien.YeuCauKhamBenhs.LastOrDefault()?.ChanDoanSoBoICD?.TenTiengViet);
                    }

                }
                if (yeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru)
                {
                    dienGiaiChanDoanSoBo.Add(phieuSuatAns.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhGhiChu);
                    if (phieuSuatAns.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD != null)
                    {
                        chanDoanSoBos.Add(phieuSuatAns.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD?.Ma + "-" + phieuSuatAns.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet);
                    }
                }
            }

            foreach (var phieuSuatAn in phieuSuatAns)
            {
                var tenNoiThucHien = phieuSuatAn.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens.Where(a => a.KhoaPhongId != null).Select(a => a.KhoaPhong.Ma + " - " + a.KhoaPhong.Ten).FirstOrDefault();
                dsPhieuAns.Add(new InDanhSachSuatAn
                {
                    TenNhomDichVuBenhVien = phieuSuatAn.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                    TenNoiThucHien = tenNoiThucHien != null ? tenNoiThucHien :
                    phieuSuatAn.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienNoiThucHiens.Where(a => a.PhongBenhVien != null).Select(a => a.PhongBenhVien.Ma + " - " + a.PhongBenhVien.Ten).FirstOrDefault(),
                    Ten = phieuSuatAn.TenDichVu,
                    BuaAn = phieuSuatAn.BuaAn?.GetDescription(),
                    SoLuong = phieuSuatAn.SoLan,
                    DonGia = phieuSuatAn.Gia,
                });
            }


            var groupByNhoms = dsPhieuAns.GroupBy(c => c.Nhom);
            foreach (var groupByNhom in groupByNhoms)
            {
                //    htmlDanhSachDichVu += "<tr style=' border: 1px solid #020000; '>" +
                //                           "<td style='border: 1px solid #020000;' colspan='6'>" +
                //                           "<span style='float:left;'><b>" + groupByNhom.Key + "  </b></span></td>" +
                //                           "</tr>";
                foreach (var itemPA in groupByNhom)
                {
                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + itemPA.Ten + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + itemPA.BuaAn + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + itemPA.SoLuong + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: right;'>" + Convert.ToDouble(itemPA.DonGia).ApplyFormatMoneyToDouble() + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: right;'>" + Convert.ToDouble(itemPA.ThanhTien).ApplyFormatMoneyToDouble() + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: right;'>" + Convert.ToDouble(itemPA.ThanhTien).ApplyFormatMoneyToDouble() + "</td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                }
            }

            htmlDanhSachDichVu += "<tr style=' border: 1px solid #020000; '>" +
                                "<td style='border: 1px solid #020000;' colspan='6'></span>" +
                                "<span style='float:right;'><b>Tổng Cộng</b></span>" +
                                "</td>" +
                                $"<td style='border: 1px solid #020000;text-align: right;'><b>{Convert.ToDouble(dsPhieuAns.Sum(o => o.ThanhTien)).ApplyFormatMoneyToDouble()}</b> </td>" +
                                "</tr>";

            //var theBHYTs = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
            //            ? yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
            //                .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc) : null;

            var theBHYTs = yeuCauTiepNhan.CoBHYT == true ? yeuCauTiepNhan.BHYTMucHuong : (int?)null;
          

            var mucHuongThe = theBHYTs != null ? "<span style='font - size: 20px;'>BHYT(" + theBHYTs + " %) - QL " + yeuCauTiepNhan.BHYTMaSoThe?.Substring(2, 1) + "</span><br>" : string.Empty;
            var noiDK = _benhVienRepository.TableNoTracking.Where(a => a.Ma == yeuCauTiepNhan.BHYTMaDKBD).Select(a => a.Ten).FirstOrDefault();
            var ngayDieuTri = phieuSuatAns.Select(c => c.NoiTruPhieuDieuTri.NgayDieuTri).FirstOrDefault();

            var data = new
            {
                LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",

                MaYT = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                MaBA = yeuCauTiepNhan?.NoiTruBenhAn != null ? yeuCauTiepNhan?.NoiTruBenhAn.SoBenhAn : "",

                HoTen = yeuCauTiepNhan?.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan?.DiaChiDayDu,

                Ngay = ngayDieuTri.Day,
                Thang = ngayDieuTri.Month,
                Nam = ngayDieuTri.Year,

                DienThoai = yeuCauTiepNhan?.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)"
                                    + (yeuCauTiepNhan.BHYTMaSoThe == null ? null : " - QL" + yeuCauTiepNhan.BHYTMaSoThe.Substring(2, 1)),

                SoTheBHYT = theBHYTs != null ? yeuCauTiepNhan.BHYTMaSoThe + "-" + yeuCauTiepNhan.BHYTMaDKBD : string.Empty,

                HanThe = theBHYTs != null ?
                 " " + (yeuCauTiepNhan.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") +
                 " - " + (yeuCauTiepNhan.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : string.Empty,

                NoiDangKy = noiDK,

                NoiChiDinh = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(c => c.KhoaPhongChuyenDen != null).LastOrDefault().KhoaPhongChuyenDen.Ten,
                KhoaDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Where(c => c.KhoaPhongChuyenDen != null).LastOrDefault().KhoaPhongChuyenDen.Ten,
                PhongGiuong = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Where(c => c.GiuongBenh != null).LastOrDefault().GiuongBenh.Ten,

                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNguoiChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,

                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null).ToList().Join(";"),
                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null).ToList().Join(";"),
                BHYT = mucHuongThe
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

    }
}
