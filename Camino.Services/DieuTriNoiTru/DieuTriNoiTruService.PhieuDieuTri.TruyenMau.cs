using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {

        public async Task<GridDataSource> GetDataForGridAsyncTruyenMau(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryObj = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(queryObj[0]);
            var phieuDieuTriId = long.Parse(queryObj[1]);

            var query = _yeuCauTruyenMauRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.NoiTruPhieuDieuTriId == phieuDieuTriId && o.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy)
                .Select(s => new PhieuDieuTriTruyenMauGridVo
                {
                    Id = s.Id,
                    Ma = s.MauVaChePham.Ma,
                    Ten = s.MauVaChePham.Ten,
                    TheTich = s.TheTich,
                    NhomMau = s.NhomMau,
                    YeuToRh = s.YeuToRh,
                    DonGiaBan = s.DonGiaBan,
                    DonGiaBaoHiem = s.DonGiaBaoHiem,
                    TrangThai = s.TrangThai,
                    ThoiGianBatDauTruyen = s.ThoiGianBatDauTruyen,
                    MauVaChePhamId = s.MauVaChePhamId,
                    ThoiDiemChiDinh = s.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = s.NhanVienChiDinh.User.HoTen,
                    DaNhapKhoMauChiTiet = s.NhapKhoMauChiTiets.Any(),
                    ThoiGianDienBien = s.ThoiGianDienBien
                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncTruyenMau(QueryInfo queryInfo)
        {
            return null;
        }

        public void ApDungThoiGianDienBienTruyenMau(List<long> yeuCauTruyenMauIds, DateTime? thoiGianDienBien)
        {
            var yeuCauTruyenMaus = _yeuCauTruyenMauRepository.Table.Where(o => yeuCauTruyenMauIds.Contains(o.Id)).ToList();
            if (yeuCauTruyenMaus.Any())
            {
                foreach (var yeuCauTruyenMau in yeuCauTruyenMaus)
                {
                    yeuCauTruyenMau.ThoiGianDienBien = thoiGianDienBien;
                }
                _yeuCauTruyenMauRepository.Context.SaveChanges();
            }
        }

        public async Task<List<MauVaChePhamTemplateVo>> GetMauVaChePham(DropDownListRequestModel model)
        {
            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.MauVaChePhams.MauVaChePham.Ma),
                nameof(Core.Domain.Entities.MauVaChePhams.MauVaChePham.Ten),
            };
            var lstMauVaChePhams = new List<MauVaChePhamTemplateVo>();
            var mauVaChePhamId = CommonHelper.GetIdFromRequestDropDownList(model);

            if (string.IsNullOrEmpty(model.Query) || !model.Query.Contains(" "))
            {
                lstMauVaChePhams = await _mauVaChePhamRepository.TableNoTracking
                    .Select(item => new MauVaChePhamTemplateVo
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        Ten = item.Ten,
                        Ma = item.Ma,
                        TheTich = item.TheTich,
                        DonGia = item.GiaTriToiDa
                    })
                    .ApplyLike(model.Query, x => x.Ma, x => x.Ten)
                    //.Take(model.Take)
                    .ToListAsync();
            }
            else
            {
                var lstMauVaChePhamId = await _mauVaChePhamRepository
                    .ApplyFulltext(model.Query, nameof(Core.Domain.Entities.MauVaChePhams.MauVaChePham), lstColumnNameSearch)
                    .Select(x => x.Id)
                    .ToListAsync();
                lstMauVaChePhams = await _mauVaChePhamRepository.TableNoTracking
                    .OrderByDescending(x => x.Id == mauVaChePhamId)
                    .ThenBy(p => lstMauVaChePhamId.IndexOf(p.Id) != -1 ? lstMauVaChePhamId.IndexOf(p.Id) : model.Take + 1)
                    //.Take(model.Take)
                     .Select(item => new MauVaChePhamTemplateVo
                     {
                         DisplayName = item.Ten,
                         KeyId = item.Id,
                         Ten = item.Ten,
                         Ma = item.Ma,
                         TheTich = item.TheTich,
                         DonGia = item.GiaTriToiDa
                     }).ToListAsync();
            }
            return lstMauVaChePhams;
        }

        public async Task<List<NhomMauTemplateVo>> GetNhomMauRH(DropDownListRequestModel model)
        {
            var listEnumNhomMau = EnumHelper.GetListEnum<EnumNhomMau>();
            var listEnumRH = EnumHelper.GetListEnum<EnumYeuToRh>();
            var listNhomMauRHs = new List<NhomMauTemplateVo>();
            foreach (var item in listEnumNhomMau)
            {
                foreach (var ytem in listEnumRH)
                {
                    var nhomMauRH = new NhomMauTemplateVo
                    {
                        KeyId = item + ";" + ytem,
                        DisplayName = item.GetDescription() + ytem.GetDescription()
                    };
                    listNhomMauRHs.Add(nhomMauRH);
                }
            }
            if (!string.IsNullOrEmpty(model.Query))
            {
                listNhomMauRHs = listNhomMauRHs.Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }
            return listNhomMauRHs;
        }

        public async Task ThemYeuCauTruyenMau(PhieuDieuTriTruyenMauVo truyenMauVo, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var noiTruPhieuDieuTri = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(p => p.Id == truyenMauVo.Id).First();
            var nhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            var noiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var mauVaChePham = _mauVaChePhamRepository.GetById(truyenMauVo.MauVaChePhamId);

            var ycTruyenMau = new YeuCauTruyenMau
            {
                YeuCauTiepNhanId = truyenMauVo.YeuCauTiepNhanId,
                MauVaChePhamId = truyenMauVo.MauVaChePhamId,
                MaDichVu = mauVaChePham.Ma,
                TenDichVu = mauVaChePham.Ten,
                TheTich = mauVaChePham.TheTich,
                NhomMau = truyenMauVo.NhomMau,
                YeuToRh = truyenMauVo.YeuToRh,
                //update BVHD-3320
                DuocHuongBaoHiem = false,
                DonGiaNhap = mauVaChePham.GiaTriToiDa,
                DonGiaBan = mauVaChePham.GiaTriToiDa,
                DonGiaBaoHiem = 0,
                //----------------
                TrangThai = EnumTrangThaiYeuCauTruyenMau.ChuaThucHien,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                PhanLoaiMau = PhanLoaiMau.ChePhamHongCau,// confirm
                NhanVienChiDinhId = nhanVienChiDinhId,
                NoiChiDinhId = noiChiDinhId,
                ThoiDiemChiDinh = DateTime.Now,
                ThoiGianBatDauTruyen = truyenMauVo.ThoiGianBatDauTruyen
            };
            //noiTruPhieuDieuTri.YeuCauTruyenMaus.Add(ycTruyenMau);

            //Cập nhật 01/04/2022
            ycTruyenMau.NoiTruPhieuDieuTriId = noiTruPhieuDieuTri.Id;
            yeuCauTiepNhan.YeuCauTruyenMaus.Add(ycTruyenMau);
        }

        public async Task CapNhatYeuCauTruyenMau(PhieuDieuTriTruyenMauVo truyenMauVo, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var ycTruyenMau = yeuCauTiepNhan.YeuCauTruyenMaus.FirstOrDefault(p => p.Id == truyenMauVo.Id);

            if (ycTruyenMau == null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.YeuCauTruyenMau.NotExists"));
            }

            //if (ycTruyenMau.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan)
            //{
            //    throw new Exception(_localizationService.GetResource("PhieuDieuTri.YeuCauTruyenMau.DaThanhToan"));
            //}
            ycTruyenMau.NhomMau = truyenMauVo.NhomMau;
            ycTruyenMau.YeuToRh = truyenMauVo.YeuToRh;
            ycTruyenMau.TheTich = truyenMauVo.TheTich;
            ycTruyenMau.ThoiGianBatDauTruyen = truyenMauVo.ThoiGianBatDauTruyen;
        }

        public async Task XoaYeuCauTruyenMau(long ycTruyenMauId, YeuCauTiepNhan yeuCauTiepNhan)
        {
            var ycTruyenMau = yeuCauTiepNhan.YeuCauTruyenMaus.FirstOrDefault(p => p.Id == ycTruyenMauId);
            if (ycTruyenMau == null)
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.YeuCauTruyenMau.NotExists"));
            }
            if (ycTruyenMau.NhapKhoMauChiTiets.Any())
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.DonGiaNhap.Exists"));
            }
            if (ycTruyenMau.NhapKhoMauChiTiets.Any(z => z.NhapKhoMau?.DuocKeToanDuyet == true))
            {
                throw new Exception(_localizationService.GetResource("PhieuDieuTri.YeuCauTruyenMau.NotDeleted"));
            }

            //if (ycTruyenMau.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan)
            //{
            //    throw new Exception(_localizationService.GetResource("PhieuDieuTri.YeuCauTruyenMau.DaThanhToan"));
            //}
            ycTruyenMau.TrangThai = EnumTrangThaiYeuCauTruyenMau.DaHuy;

            await XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu.TruyenMau, ycTruyenMauId);
        }

        public async Task<string> InPhieuTruyenMau(XacNhanInPhieuTruyenMau xacNhanIn)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking.Include(x => x.NoiTruBenhAn).ThenInclude(xx => xx.NoiTruPhieuDieuTris).ThenInclude(xxx => xxx.YeuCauTruyenMaus)
                .Include(x => x.NoiTruBenhAn).ThenInclude(xx => xx.NoiTruPhieuDieuTris).ThenInclude(xxx => xxx.ChanDoanChinhICD).Where(p => p.Id == xacNhanIn.YeuCauTiepNhanId).First();

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuInChiTruyenMau"));

            var dienGiaiChanDoanSoBo = new List<string>();
            var chanDoanSoBos = new List<string>();

            long userId = _userAgentHelper.GetCurrentUserId();
            var tenNguoiChiDinh = _useRepository.TableNoTracking
                           .Where(u => u.Id == userId).Select(u =>
                           (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                         + u.HoTen).FirstOrDefault();

            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;


            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NHÓM MÁU</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "</tr>";

            var i = 1;
            var content = string.Empty;
            var truyenMauTheoDieuTris = yeuCauTiepNhan.NoiTruBenhAn.NoiTruPhieuDieuTris.Where(c => c.Id == xacNhanIn.PhieuDieuTriId)
                                                      .SelectMany(c => c.YeuCauTruyenMaus).Where(tm => tm.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy).ToList();
            if (!truyenMauTheoDieuTris.Any())
            {
                return null;
            }
            var dsTruyenMaus = new List<InDanhSachTruyenMau>();
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
                    dienGiaiChanDoanSoBo.Add(truyenMauTheoDieuTris.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhGhiChu);
                    if (truyenMauTheoDieuTris.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD != null)
                    {
                        chanDoanSoBos.Add(truyenMauTheoDieuTris.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD?.Ma + "-" + truyenMauTheoDieuTris.LastOrDefault().NoiTruPhieuDieuTri.ChanDoanChinhICD?.TenTiengViet);
                    }

                }
            }

            foreach (var truyenMau in truyenMauTheoDieuTris.GroupBy(c => new { TenDichVu = c.TenDichVu, NhomMau = c.NhomMau }))
            {
                dsTruyenMaus.Add(new InDanhSachTruyenMau
                {
                    YeuToRh = truyenMau.FirstOrDefault().YeuToRh,
                    Ten = truyenMau.FirstOrDefault().TenDichVu,
                    SoLuong = (double)truyenMau.Count(),
                    NhomMau = truyenMau.FirstOrDefault().NhomMau.GetDescription(),
                });
            }



            foreach (var truyenMau in dsTruyenMaus)
            {
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + truyenMau.Ten + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + truyenMau.NhomMau + truyenMau.YeuToRh.GetDescription() + "</td>";
                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + truyenMau.SoLuong + "</td>";
                htmlDanhSachDichVu += " </tr>";
                i++;
            }

            var data = new
            {
                LogoUrl = xacNhanIn.Hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                HoTen = yeuCauTiepNhan?.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                DienThoai = yeuCauTiepNhan?.SoDienThoai,
                DoiTuong = yeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauTiepNhan.BHYTMucHuong.ToString() + "%)"
                                    + (yeuCauTiepNhan.BHYTMaSoThe == null ? null : " - QL" + yeuCauTiepNhan.BHYTMaSoThe.Substring(2, 1)),
                SoTheBHYT = yeuCauTiepNhan.BHYTMaSoThe,
                HanThe = (yeuCauTiepNhan?.BHYTNgayHieuLuc != null || yeuCauTiepNhan?.BHYTNgayHetHan != null) ? "từ ngày: " + (yeuCauTiepNhan?.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (yeuCauTiepNhan?.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",

                NoiYeuCau = tenPhong,
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNguoiChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                ChuanDoanSoBo = chanDoanSoBos.Where(s => s != null).ToList().Join(";"),
                DienGiai = dienGiaiChanDoanSoBo.Where(s => s != null).ToList().Join(";")
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }

    }
}
