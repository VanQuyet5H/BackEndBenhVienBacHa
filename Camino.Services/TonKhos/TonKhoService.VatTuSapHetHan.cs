using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject.TonKhos;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.TonKhos
{
    public partial class TonKhoService
    {
        public async Task<GridDataSource> GetDanhSachVatTuSapHetHanForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            DateTime dayHetHan = DateTime.Now;
            //DateTime dayHetHan = DateTime.Now.AddDays(settings.VatTuSapHetHanNgayHetHan);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuSapHetHanSearchGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuSapHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                     (queryString.KhoId == 0 || p.NhapKhoVatTu.KhoId == queryString.KhoId))
                                                                         .Select(p => new VatTuSapHetHanGridVo
                                                                         {
                                                                             Id = p.VatTuBenhVien.VatTus.Id,
                                                                             VatTuBenhVienId = p.VatTuBenhVienId,
                                                                             TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                             DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                             TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                             NgayHetHanHienThi = p.HanSuDung.ApplyFormatDate(),
                                                                             NgayHetHan = p.HanSuDung,
                                                                             ViTri = p.KhoViTri.Ten,
                                                                             VitriId = p.KhoViTriId,
                                                                             KhoId = p.NhapKhoVatTu.KhoId,
                                                                             SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                             SoNgayTruocKhiHetHan = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId && p2.SoNgayTruocKhiHetHan != null) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.VatTuSapHetHanNgayHetHan,
                                                                             SoLo = p.Solo,
                                                                             NhapKhoVatTuId = p.NhapKhoVatTuId,
                                                                             MaVatTu = p.VatTuBenhVien.Ma,
                                                                             DonGiaNhap = p.DonGiaNhap,
                                                                         }).Where(p => p.NgayHetHan.Date >= DateTime.Now.Date && p.NgayHetHan.Date <= dayHetHan.AddDays(p.SoNgayTruocKhiHetHan).Date && p.SoLuongTon > 0)
                                                                         .GroupBy(x => new {
                                                                             x.VatTuBenhVienId,
                                                                             x.NgayHetHan,
                                                                             x.KhoId,
                                                                             x.DonGiaNhap,
                                                                             x.SoLo,
                                                                             x.SoNgayTruocKhiHetHan,
                                                                         })
                                                                        .Select(item => new VatTuSapHetHanGridVo()
                                                                        {
                                                                            Id = item.First().Id,
                                                                            VatTuBenhVienId = item.First().VatTuBenhVienId,
                                                                            TenKho = item.First().TenKho,
                                                                            TenVatTu = item.First().TenVatTu,
                                                                            DonViTinh = item.First().DonViTinh,
                                                                            ViTri = item.First().ViTri,
                                                                            VitriId = item.First().VitriId,
                                                                            NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                                                                            NgayHetHan = item.First().NgayHetHan,
                                                                            SoLuongTon = item.Sum(p => p.SoLuongTon),
                                                                            SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                                                                            KhoId = item.First().KhoId,
                                                                            SoLo = item.First().SoLo,
                                                                            MaVatTu = item.First().MaVatTu,
                                                                            DonGiaNhap = item.First().DonGiaNhap,
                                                                            NhapKhoVatTuId = item.First().NhapKhoVatTuId
                                                                        });

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh, g => g.TenKho, g => g.ViTri);
            }

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalVatTuSapHetHanPagesForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            DateTime dayHetHan = DateTime.Now;
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuSapHetHanSearchGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuSapHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                 (queryString.KhoId == 0 || p.NhapKhoVatTu.KhoId == queryString.KhoId))
                                                                     .Select(p => new VatTuSapHetHanGridVo
                                                                     {
                                                                         Id = p.VatTuBenhVien.VatTus.Id,
                                                                         VatTuBenhVienId = p.VatTuBenhVienId,
                                                                         TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                         DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                         TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                         NgayHetHanHienThi = p.HanSuDung.ApplyFormatDate(),
                                                                         NgayHetHan = p.HanSuDung,
                                                                         ViTri = p.KhoViTri.Ten,
                                                                         VitriId = p.KhoViTriId,
                                                                         KhoId = p.NhapKhoVatTu.KhoId,
                                                                         SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                         SoNgayTruocKhiHetHan = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId && p2.SoNgayTruocKhiHetHan != null) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.VatTuSapHetHanNgayHetHan,
                                                                         SoLo = p.Solo,
                                                                         NhapKhoVatTuId = p.NhapKhoVatTuId,
                                                                         MaVatTu = p.VatTuBenhVien.Ma,
                                                                         DonGiaNhap = p.DonGiaNhap,
                                                                     }).Where(p => p.NgayHetHan.Date >= DateTime.Now.Date && p.NgayHetHan.Date <= dayHetHan.AddDays(p.SoNgayTruocKhiHetHan).Date && p.SoLuongTon > 0)
                                                                     .GroupBy(x => new {
                                                                         x.VatTuBenhVienId,
                                                                         x.NgayHetHan,
                                                                         x.KhoId,
                                                                         x.DonGiaNhap,
                                                                         x.SoLo,
                                                                         x.SoNgayTruocKhiHetHan,
                                                                     })
                                                                    .Select(item => new VatTuSapHetHanGridVo()
                                                                    {   Id = item.First().Id,
                                                                        VatTuBenhVienId = item.First().VatTuBenhVienId,
                                                                        TenKho = item.First().TenKho,
                                                                        TenVatTu = item.First().TenVatTu,
                                                                        DonViTinh = item.First().DonViTinh,
                                                                        ViTri = item.First().ViTri,
                                                                        VitriId = item.First().VitriId,
                                                                        NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                                                                        NgayHetHan = item.First().NgayHetHan,
                                                                        SoLuongTon = item.Sum(p => p.SoLuongTon),
                                                                        SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                                                                        KhoId = item.First().KhoId,
                                                                        SoLo = item.First().SoLo,
                                                                        MaVatTu = item.First().MaVatTu,
                                                                        DonGiaNhap = item.First().DonGiaNhap,
                                                                        NhapKhoVatTuId = item.First().NhapKhoVatTuId
                                                                    });

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh, g => g.TenKho, g => g.ViTri);
            }

            var countTask = query.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public string GetVatTuSapHetHanHTML(string searchString)
        {
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoVatTuSapHetHan")).FirstOrDefault();
            var settings = _cauhinhService.LoadSetting<CauHinhBaoCao>();
            DateTime dayHetHan = DateTime.Now;

            var queryString = new VatTuSapHetHanSearchGridVoItem();
            if (!string.IsNullOrEmpty(searchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuSapHetHanSearchGridVoItem>(searchString);
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  (queryString.KhoId == 0 || p.NhapKhoVatTu.KhoId == queryString.KhoId))
                                                                      .Select(p => new VatTuSapHetHanGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          VatTuBenhVienId = p.VatTuBenhVienId,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                          NgayHetHanHienThi = p.HanSuDung.ApplyFormatDate(),
                                                                          NgayHetHan = p.HanSuDung,
                                                                          ViTri = p.KhoViTri.Ten,
                                                                          VitriId = p.KhoViTriId,
                                                                          KhoId = p.NhapKhoVatTu.KhoId,
                                                                          SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                          SoNgayTruocKhiHetHan = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId && p2.SoNgayTruocKhiHetHan != null) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).SoNgayTruocKhiHetHan.GetValueOrDefault() : settings.VatTuSapHetHanNgayHetHan,
                                                                          SoLo = p.Solo,
                                                                          NhapKhoVatTuId = p.NhapKhoVatTuId,
                                                                          MaVatTu = p.VatTuBenhVien.Ma,
                                                                          DonGiaNhap = p.DonGiaNhap,
                                                                      }).Where(p => p.NgayHetHan.Date >= DateTime.Now.Date && p.NgayHetHan.Date <= dayHetHan.AddDays(p.SoNgayTruocKhiHetHan).Date && p.SoLuongTon > 0)
                                                                      .GroupBy(x => new {
                                                                          x.VatTuBenhVienId,
                                                                          x.NgayHetHan,
                                                                          x.KhoId,
                                                                          x.DonGiaNhap,
                                                                          x.SoLo,
                                                                          x.SoNgayTruocKhiHetHan,
                                                                      })
                                                                     .Select(item => new VatTuSapHetHanGridVo()
                                                                     {
                                                                         Id = item.First().Id,
                                                                         VatTuBenhVienId = item.First().VatTuBenhVienId,
                                                                         TenKho = item.First().TenKho,
                                                                         TenVatTu = item.First().TenVatTu,
                                                                         DonViTinh = item.First().DonViTinh,
                                                                         ViTri = item.First().ViTri,
                                                                         VitriId = item.First().VitriId,
                                                                         NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                                                                         NgayHetHan = item.First().NgayHetHan,
                                                                         SoLuongTon = item.Sum(p => p.SoLuongTon),
                                                                         SoNgayTruocKhiHetHan = item.First().SoNgayTruocKhiHetHan,
                                                                         KhoId = item.First().KhoId,
                                                                         SoLo = item.First().SoLo,
                                                                         MaVatTu = item.First().MaVatTu,
                                                                         DonGiaNhap = item.First().DonGiaNhap,
                                                                         NhapKhoVatTuId = item.First().NhapKhoVatTuId
                                                                     });

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh, g => g.TenKho, g => g.ViTri);
            }

            string finalresult = String.Empty;
            //var lstVatTu = query.OrderByDescending(p => p.NgayHetHan).ToList();
            var lstVatTu = !string.IsNullOrEmpty(searchString) ? query.OrderBy(queryString.SortString).ToList() : query.ToList();
            foreach (var item in lstVatTu)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center; '><td style=''border: 1px solid #020000;text-align: center;'>" + item.TenKho
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaVatTu
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TenVatTu
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonViTinh
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLo !="NULL" ? item.SoLo :"")
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.ViTri
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonGiaNhap.ApplyFormatTien()
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuongTonDisplay
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + Convert.ToDecimal(item.ThanhTien).ApplyFormatTien()
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.NgayHetHanHienThi + "</tr>";

            }
            string ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new DataVaLueHTML
            {
                TemplateVatTu = finalresult,
                Ngay = ngayThangHientai
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
