using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using static Camino.Core.Domain.ValueObject.TonKhos.VatTuDaHetHan;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.TonKhos
{
    public partial class TonKhoService
    {
        public async Task<GridDataSource> GetDanhSachVatTuDaHetHanForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            var queryString = JsonConvert.DeserializeObject<VatTuDaHetHanSearchJson>(queryInfo.AdditionalSearchString);

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Include(p => p.NhapKhoVatTu).ThenInclude(nk => nk.Kho)
                .Include(p=>p.KhoViTri)
                .Include(p => p.VatTuBenhVien).ThenInclude(vtbv => vtbv.VatTus)
                .Where(x => (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK
                         || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe 
                         || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) 
                         && (queryString.KhoId == 0 || queryString.KhoId == null || x.NhapKhoVatTu.KhoId == queryString.KhoId) 
                         && x.HanSuDung.Date < DateTime.Now.Date);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Trim();
                query = query.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten, g => g.VatTuBenhVien.VatTus.DonViTinh, g => g.NhapKhoVatTu.Kho.Ten, g => g.KhoViTri.Ten
               );
            }

            var result = query.Select(s => new VatTuDaHetHanGridVo()
               {
                   Id = s.Id,
                   VatTuBenhVienId = s.VatTuBenhVienId,
                   TenKho = s.NhapKhoVatTu.Kho.Ten,
                   TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                   DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                   ViTri = s.KhoViTri.Ten,
                   NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                   NgayHetHan = s.HanSuDung,
                   SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                   KhoId = s.NhapKhoVatTu.KhoId,
                   DonGiaNhap = s.DonGiaNhap,
                   SoLo = s.Solo,
                   MaVatTu = s.VatTuBenhVien.Ma

               })
               .GroupBy(x => new { x.VatTuBenhVienId, x.NgayHetHan, x.KhoId })
               .Select(item => new VatTuDaHetHanGridVo()
               {
                   VatTuBenhVienId = item.First().VatTuBenhVienId,
                   TenKho = item.First().TenKho,
                   TenVatTu = item.First().TenVatTu,
                   DonViTinh = item.First().DonViTinh,
                   ViTri = item.First().ViTri,
                   NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                   NgayHetHan = item.First().NgayHetHan,
                   SoLuongTon = item.Sum(p => p.SoLuongTon),
                   DonGiaNhap = item.First().DonGiaNhap,
                   SoLo = item.First().SoLo,
                   MaVatTu = item.First().MaVatTu
               })
                .OrderBy(x => x.NgayHetHan).ThenBy(x => x.TenVatTu).Where(p=>p.SoLuongTon > 0).Distinct();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : result.Count();
            var queryTask = result.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }
        public async Task<GridDataSource> GetDanhSachVatTuDaHetHanTotalPageForGridAsync(QueryInfo queryInfo)
        {
            if (string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                return null;
            }
            var queryString = JsonConvert.DeserializeObject<VatTuDaHetHanSearchJson>(queryInfo.AdditionalSearchString);
            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking
                .Include(p => p.NhapKhoVatTu).ThenInclude(nk => nk.Kho)
                .Include(p => p.KhoViTri)
                .Include(p => p.VatTuBenhVien).ThenInclude(vtbv => vtbv.VatTus)
                .Where(x => (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK
                         || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                         || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)
                         && (queryString.KhoId == 0 || queryString.KhoId == null || x.NhapKhoVatTu.KhoId == queryString.KhoId)
                         && x.HanSuDung.Date < DateTime.Now.Date);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Trim();
                query = query.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten, g => g.VatTuBenhVien.VatTus.DonViTinh, g => g.NhapKhoVatTu.Kho.Ten, g => g.KhoViTri.Ten
               );
            }

            var result = query.Select(s => new VatTuDaHetHanGridVo()
            {
                Id = s.Id,
                VatTuBenhVienId = s.VatTuBenhVienId,
                TenKho = s.NhapKhoVatTu.Kho.Ten,
                TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                ViTri = s.KhoViTri.Ten,
                NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                NgayHetHan = s.HanSuDung,
                SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                KhoId = s.NhapKhoVatTu.KhoId,
                DonGiaNhap = s.DonGiaNhap,
                SoLo = s.Solo,
                MaVatTu = s.VatTuBenhVien.Ma

            })
              .GroupBy(x => new { x.VatTuBenhVienId, x.NgayHetHan, x.KhoId })
              .Select(item => new VatTuDaHetHanGridVo()
              {
                  VatTuBenhVienId = item.First().VatTuBenhVienId,
                  TenKho = item.First().TenKho,
                  TenVatTu = item.First().TenVatTu,
                  DonViTinh = item.First().DonViTinh,
                  ViTri = item.First().ViTri,
                  NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                  NgayHetHan = item.First().NgayHetHan,
                  SoLuongTon = item.Sum(p => p.SoLuongTon),
                  DonGiaNhap = item.First().DonGiaNhap,
                  SoLo = item.First().SoLo,
                  MaVatTu = item.First().MaVatTu
              })
               .OrderBy(x => x.NgayHetHan).ThenBy(x => x.TenVatTu).Where(p => p.SoLuongTon > 0).Distinct();

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<List<LookupItemVo>> GetKhoVatTu(LookupQueryInfo queryInfo)
        {
            var lst = new List<LookupItemVo>
            {
                //new LookupItemVo { DisplayName = "Tất cả", KeyId = 0 }
            };
            var khoVatTus = lst.Union(BaseRepository.TableNoTracking
                   .Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK
                          || p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                          || p.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.LoaiVatTu == true)
                          .Select(item => new LookupItemVo
                          {
                              DisplayName = item.Ten,
                              KeyId = Convert.ToInt32(item.Id),
                          })
                          .ApplyLike(queryInfo.Query, g => g.DisplayName)
                          .Take(queryInfo.Take))
                          .ToList();
            return khoVatTus;
        }

        public async Task<List<LookupItemVo>> GetKhoVatTusWithoutTatCa(LookupQueryInfo queryInfo)
        {
            //return await BaseRepository.TableNoTracking.Where(p => (p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 ||
            //                                                       p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe ||
            //                                                       p.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) && p.LoaiVatTu == true)
            //                                           .Select(item => new LookupItemVo
            //                                           {
            //                                               DisplayName = item.Ten,
            //                                               KeyId = Convert.ToInt32(item.Id),
            //                                           })
            //                                           .ApplyLike(queryInfo.Query, g => g.DisplayName)
            //                                           .Take(queryInfo.Take)
            //                                           .ToListAsync();
            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            var result = _khoNhanVienQuanLyRepository.TableNoTracking
                         .Where(p => p.NhanVienId == userCurrentId 
                                  && p.Kho.LoaiVatTu == true 
                                  && (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK ||
                                                                   p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe ||
                                                                   p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                         .Select(s => new LookupItemVo
                         {
                             KeyId = s.KhoId,
                             DisplayName = s.Kho.Ten
                         })
                         .ApplyLike(queryInfo.Query, o => o.DisplayName)
                         .Take(queryInfo.Take);
            return await result.ToListAsync();
        }

        public async Task<LookupItemVo> GetFirstKhoVatTu()
        {
            return await BaseRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || p.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK ||
                                                                   p.LoaiKho == EnumLoaiKhoDuocPham.KhoLe ||
                                                                   p.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)
                                                       .Select(item => new LookupItemVo
                                                       {
                                                           DisplayName = item.Ten,
                                                           KeyId = Convert.ToInt32(item.Id),
                                                       })
                                                       .FirstOrDefaultAsync();
        }

        public string XemVatTuDaHetHan(InVatTuDaHetHan inVatTuDaHetHan)
        {
            var templateVatTuDaHetHan = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoVatTuDaHetHan")).First();
            var queryString = JsonConvert.DeserializeObject<VatTuDaHetHanSearchJson>(inVatTuDaHetHan.SearchString);

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking
                 .Include(p => p.NhapKhoVatTu).ThenInclude(nk => nk.Kho)
                 .Include(p => p.KhoViTri)
                 .Include(p => p.VatTuBenhVien).ThenInclude(vtbv => vtbv.VatTus)
                 .Where(x => (x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoHanhChinh || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoKSNK
                          || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe
                          || x.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)
                          && (queryString.KhoId == 0 || queryString.KhoId == null || x.NhapKhoVatTu.KhoId == queryString.KhoId)
                          && x.HanSuDung.Date < DateTime.Now.Date);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Trim();
                query = query.ApplyLike(searchTerms,
                    g => g.VatTuBenhVien.VatTus.Ten, g => g.VatTuBenhVien.VatTus.DonViTinh, g => g.NhapKhoVatTu.Kho.Ten, g => g.KhoViTri.Ten
               );
            }

            var result = query.Select(s => new VatTuDaHetHanGridVo()
            {
                Id = s.Id,
                VatTuBenhVienId = s.VatTuBenhVienId,
                TenKho = s.NhapKhoVatTu.Kho.Ten,
                TenVatTu = s.VatTuBenhVien.VatTus.Ten,
                DonViTinh = s.VatTuBenhVien.VatTus.DonViTinh,
                ViTri = s.KhoViTri.Ten,
                NgayHetHanHienThi = s.HanSuDung.ApplyFormatDate(),
                NgayHetHan = s.HanSuDung,
                SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                KhoId = s.NhapKhoVatTu.KhoId,
                DonGiaNhap = s.DonGiaNhap,
                SoLo = s.Solo,
                MaVatTu = s.VatTuBenhVien.Ma

            })
             .GroupBy(x => new { x.VatTuBenhVienId, x.NgayHetHan, x.KhoId })
             .Select(item => new VatTuDaHetHanGridVo()
             {
                 VatTuBenhVienId = item.First().VatTuBenhVienId,
                 TenKho = item.First().TenKho,
                 TenVatTu = item.First().TenVatTu,
                 DonViTinh = item.First().DonViTinh,
                 ViTri = item.First().ViTri,
                 NgayHetHanHienThi = item.First().NgayHetHanHienThi,
                 NgayHetHan = item.First().NgayHetHan,
                 SoLuongTon = item.Sum(p => p.SoLuongTon),
                 DonGiaNhap = item.First().DonGiaNhap,
                 SoLo = item.First().SoLo,
                 MaVatTu = item.First().MaVatTu
             })
              .OrderBy(x => x.NgayHetHan).ThenBy(x => x.TenVatTu).Where(p => p.SoLuongTon > 0).Distinct();

            var content = string.Empty;
            var dataBodyContent = string.Empty;
            var header = string.Empty;

            if (inVatTuDaHetHan.Header)
            {
                header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                            "<th>DANH MỤC VẬT TƯ ĐÃ HẾT HẠN</th>" +
                       "</p>";
            }

            var lstVatTu = result.OrderByDescending(p => p.NgayHetHan).ToList();
            foreach (var item in lstVatTu)
            {
                dataBodyContent = dataBodyContent + "<tr style='border: 1px solid #020000;text-align: center; '><td style=''border: 1px solid #020000;text-align: center;'>" + item.TenKho
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.MaVatTu
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TenVatTu
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonViTinh
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + (item.SoLo != "NULL" ? item.SoLo : "")
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.ViTri
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonGiaNhap.ApplyFormatTien()
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuongTon.ApplyNumber()
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + Convert.ToDecimal(item.ThanhTien).ApplyFormatTien()
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.NgayHetHanHienThi + "</tr>";
            }

            var data = new VatTuDaHetHanData
            {
                Header = header,
                TemplateVatTu = dataBodyContent,
                Ngay = DateTime.Now.Day.ConvertDateToString(),
                Thang = DateTime.Now.Month.ConvertMonthToString(),
                Nam = DateTime.Now.Year.ConvertYearToString(),
            };
            content = TemplateHelpper.FormatTemplateWithContentTemplate(templateVatTuDaHetHan.Body, data);
            return content;
        }

    }
}
