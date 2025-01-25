using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TonKhos;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;

namespace Camino.Services.TonKhos
{
    public partial class TonKhoService
    {
        #region Tồn kho vật tư (cảnh báo)
        public async Task<GridDataSource> GetDanhSachVatTuTonKhoCanhBaoForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuTonKhoCanhBaoSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoCanhBaoSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new VatTuTonKhoCanhBaoGridVo[0], TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId)
                .Select(p => new
                {
                    Id = p.VatTuBenhVienId,
                    SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                    VatTuBenhVienId = p.VatTuBenhVienId
                }).ToList();
            
            var allDataGroup = allDataNhap.GroupBy(o => o.VatTuBenhVienId);
            var vatTuBenhVienIds = allDataGroup.Select(o => o.Key).ToList();
            var dinhMucVatTuTonKhos = _dinhMucVatTuTonKhoRepository.TableNoTracking
                .Where(o => o.KhoId == queryString.KhoId).ToList();

            var vatTus = _vatTuRepository.TableNoTracking
                .Where(o => vatTuBenhVienIds.Contains(o.Id))
                .Select(o => new
                {
                    o.Id,
                    o.Ten,
                    o.DonViTinh
                })
                .ToList();

            var dataReturn = new List<VatTuTonKhoCanhBaoGridVo>();
            foreach (var group in allDataGroup)
            {
                var vatTuTonKhoCanhBaoGridVo = new VatTuTonKhoCanhBaoGridVo
                {
                    Id = group.Key,
                    SoLuongTon = group.Sum(o => o.SoLuongTon).MathRoundNumber(2),
                    VatTuBenhVienId = group.Key,
                    KhoId = queryString.KhoId,
                };
                var vatTu = vatTus.First(o=>o.Id == group.Key);
                vatTuTonKhoCanhBaoGridVo.TenVatTu = vatTu.Ten;
                vatTuTonKhoCanhBaoGridVo.DonViTinh = vatTu.DonViTinh;

                var dinhMucVatTuTonKho = dinhMucVatTuTonKhos.Where(o => o.VatTuBenhVienId == group.Key).FirstOrDefault();
                if (dinhMucVatTuTonKho!=null)
                {
                    vatTuTonKhoCanhBaoGridVo.TonToiThieu = dinhMucVatTuTonKho.TonToiThieu;
                    vatTuTonKhoCanhBaoGridVo.TonToiDa = dinhMucVatTuTonKho.TonToiDa;
                }
                dataReturn.Add(vatTuTonKhoCanhBaoGridVo);
            }
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                dataReturn = dataReturn.Where(o=>o.TenVatTu.ToLower().RemoveVietnameseDiacritics().Contains(queryString.SearchString.ToLower().RemoveVietnameseDiacritics())).ToList();
            }
            if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            {
                dataReturn = dataReturn.Where(p => p.CanhBao.Contains(queryString.CanhBao)).ToList();
            }

            return new GridDataSource { Data = dataReturn.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = dataReturn.Count() };

            //var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
            //                                                                      p.NhapKhoVatTu.KhoId == queryString.KhoId)
            //                                                          .Select(p => new VatTuTonKhoCanhBaoGridVo
            //                                                          {
            //                                                              Id = p.VatTuBenhVien.VatTus.Id,
            //                                                              TenVatTu = p.VatTuBenhVien.VatTus.Ten,
            //                                                              DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
            //                                                              SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
            //                                                              TonToiThieu = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiThieu : null,
            //                                                              TonToiDa = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiDa : null,
            //                                                              VatTuBenhVienId = p.VatTuBenhVienId,
            //                                                              KhoId = p.NhapKhoVatTu.KhoId,
            //                                                              TenKho = p.NhapKhoVatTu.Kho.Ten
            //                                                          });

            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            //if (!string.IsNullOrEmpty(queryString.SearchString))
            //{
            //    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //    query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            //}

            //query = query.GroupBy(p => new { p.VatTuBenhVienId, p.KhoId })
            //             .Select(item => new VatTuTonKhoCanhBaoGridVo
            //             {
            //                 Id = item.First().Id,
            //                 TenVatTu = item.First().TenVatTu,
            //                 DonViTinh = item.First().DonViTinh,
            //                 SoLuongTon = item.Sum(p => p.SoLuongTon),
            //                 TonToiThieu = item.First().TonToiThieu,
            //                 TonToiDa = item.First().TonToiDa,
            //                 VatTuBenhVienId = item.First().VatTuBenhVienId,
            //                 KhoId = item.First().KhoId,
            //                 TenKho = item.First().TenKho
            //             })
            //             .Where(p => p.CanhBao != string.Empty);

            //if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            //{
            //    query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
            //}

            //var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            //return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalVatTuTonKhoCanhBaoPagesForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuTonKhoCanhBaoSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoCanhBaoSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { TotalRowCount = 0 };
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.KhoId == queryString.KhoId)
                                                                      .Select(p => new VatTuTonKhoCanhBaoGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                          TonToiThieu = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiThieu : null,
                                                                          TonToiDa = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiDa : null,
                                                                          VatTuBenhVienId = p.VatTuBenhVienId,
                                                                          KhoId = p.NhapKhoVatTu.KhoId,
                                                                          TenKho = p.NhapKhoVatTu.Kho.Ten
                                                                      });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            query = query.GroupBy(p => new { p.VatTuBenhVienId, p.KhoId })
                         .Select(item => new VatTuTonKhoCanhBaoGridVo
                         {
                             Id = item.First().Id,
                             TenVatTu = item.First().TenVatTu,
                             DonViTinh = item.First().DonViTinh,
                             SoLuongTon = item.Sum(p => p.SoLuongTon),
                             TonToiThieu = item.First().TonToiThieu,
                             TonToiDa = item.First().TonToiDa,
                             VatTuBenhVienId = item.First().VatTuBenhVienId,
                             KhoId = item.First().KhoId,
                             TenKho = item.First().TenKho
                         })
                         .Where(p => p.CanhBao != string.Empty);

            if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            {
                query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
            }

            var countTask = query.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public string GetVatTuTonKhoCanhBaoHTML(string search)
        {
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoVatTuDangCanhBao")).FirstOrDefault();

            var queryString = new VatTuTonKhoCanhBaoSearchGridVoItem();
            if (!string.IsNullOrEmpty(search))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoCanhBaoSearchGridVoItem>(search);
            }

            var gridData = GetDanhSachVatTuTonKhoCanhBaoForGridAsync(new QueryInfo { AdditionalSearchString = search }, true).Result;

            var lstVatTu = gridData.Data.Select(p => (VatTuTonKhoCanhBaoGridVo)p).ToList();

            var tenKho = queryString.KhoId == 0 ? "Tất cả" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();





            //var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
            //                                                                      p.NhapKhoVatTu.KhoId == queryString.KhoId)
            //                                                          .Select(p => new VatTuTonKhoCanhBaoGridVo
            //                                                          {
            //                                                              Id = p.VatTuBenhVien.VatTus.Id,
            //                                                              TenVatTu = p.VatTuBenhVien.VatTus.Ten,
            //                                                              DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
            //                                                              SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
            //                                                              TonToiThieu = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiThieu : null,
            //                                                              TonToiDa = p.VatTuBenhVien.DinhMucVatTuTonKhos.Any(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId) ? p.VatTuBenhVien.DinhMucVatTuTonKhos.First(p2 => p2.KhoId == p.NhapKhoVatTu.KhoId).TonToiDa : null,
            //                                                              VatTuBenhVienId = p.VatTuBenhVienId,
            //                                                              KhoId = p.NhapKhoVatTu.KhoId,
            //                                                              TenKho = p.NhapKhoVatTu.Kho.Ten,
            //                                                          });

            //if (!string.IsNullOrEmpty(queryString.SearchString))
            //{
            //    var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            //    query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            //}

            //query = query.GroupBy(p => new { p.VatTuBenhVienId, p.KhoId })
            //             .Select(item => new VatTuTonKhoCanhBaoGridVo
            //             {
            //                 Id = item.First().Id,
            //                 TenVatTu = item.First().TenVatTu,
            //                 DonViTinh = item.First().DonViTinh,
            //                 SoLuongTon = item.Sum(p => p.SoLuongTon),
            //                 TonToiThieu = item.First().TonToiThieu,
            //                 TonToiDa = item.First().TonToiDa,
            //                 VatTuBenhVienId = item.First().VatTuBenhVienId,
            //                 KhoId = item.First().KhoId,
            //                 TenKho = item.First().TenKho
            //             })
            //             .Where(p => p.CanhBao != string.Empty);

            //if (!string.IsNullOrEmpty(queryString.CanhBao) && !queryString.CanhBao.Contains("Tất cả"))
            //{
            //    query = query.Where(p => p.CanhBao.Contains(queryString.CanhBao));
            //}

            string finalresult = String.Empty;
            //var lstVatTu = query.OrderBy(queryString.SortString).ToList();

            foreach (var item in lstVatTu)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TenVatTu + "</td>"
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonViTinh + "</td>"
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.SoLuongTonDisplay + "</td>"
                                          + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.CanhBao + "</td>"
                                          + "</tr>";
            }

            string ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new VatTuTonKhoCanhBaoHTML
            {
                TemplateVatTuTonKhoCanhBao = finalresult,
                TenKho = tenKho,
                CanhBao = queryString.CanhBao,
                Ngay = ngayThangHientai
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion

        #region Tồn kho vật tư (tổng hợp)
        public async Task<GridDataSource> GetDanhSachVatTuTonKhoTongHopForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuTonKhoTongHopSearchGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoTongHopSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new VatTuTonKhoTongHopGridVo[0], TotalRowCount = 0 };
            }

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.KhoId == queryString.KhoId)
                                                                      .Select(p => new VatTuTonKhoTongHopGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                          KhoId = p.NhapKhoVatTu.KhoId,
                                                                          TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                          GiaTriSoLuongTon = p.SoLuongNhap * (double)p.DonGiaNhap - p.SoLuongDaXuat * (double)p.DonGiaNhap
                                                                      });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            query = query.GroupBy(p => new { p.Id, p.KhoId })
                         .Select(p => new VatTuTonKhoTongHopGridVo
                         {
                             Id = p.First().Id,
                             TenVatTu = p.First().TenVatTu,
                             DonViTinh = p.First().DonViTinh,
                             SoLuongTon = p.Sum(p2 => p2.SoLuongTon),
                             KhoId = p.First().KhoId,
                             TenKho = p.First().TenKho,
                             GiaTriSoLuongTon = p.Sum(p2 => p2.GiaTriSoLuongTon)
                         });

            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();

            var returnData = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            returnData.ForEach(o => o.SoLuongTon = o.SoLuongTon.MathRoundNumber(2));

            return new GridDataSource { Data = returnData.ToArray(), TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetTotalVatTuTonKhoTongHopPagesForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryString = new VatTuTonKhoTongHopSearchGridVoItem();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoTongHopSearchGridVoItem>(queryInfo.AdditionalSearchString);
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { TotalRowCount = 0 };
            }

            var tenKho = queryString.KhoId == 0 ? "" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.KhoId == queryString.KhoId)
                                                                      .Select(p => new VatTuTonKhoTongHopGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                          KhoId = p.NhapKhoVatTu.KhoId,
                                                                          TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                          GiaTriSoLuongTon = p.SoLuongNhap * (double)p.DonGiaNhap - p.SoLuongDaXuat * (double)p.DonGiaNhap
                                                                      });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            query = query.GroupBy(p => new { p.Id, p.KhoId })
                         .Select(p => new VatTuTonKhoTongHopGridVo
                         {
                             Id = p.First().Id,
                             TenVatTu = p.First().TenVatTu,
                             DonViTinh = p.First().DonViTinh,
                             SoLuongTon = p.Sum(p2 => p2.SoLuongTon),
                             KhoId = p.First().KhoId,
                             TenKho = p.First().TenKho,
                             GiaTriSoLuongTon = p.Sum(p2 => p2.GiaTriSoLuongTon)
                         });

            var countTask = query.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public string GetVatTuTonKhoTongHopHTML(string search)
        {
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoVatTuTonKhoTongHop")).FirstOrDefault();

            var queryString = new VatTuTonKhoTongHopSearchGridVoItem();
            if (!string.IsNullOrEmpty(search))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoTongHopSearchGridVoItem>(search);
            }

            var tenKho = queryString.KhoId == 0 ? "Tất cả" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.KhoId == queryString.KhoId)
                                                                      .Select(p => new VatTuTonKhoTongHopGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          SoLuongTon = p.SoLuongNhap - p.SoLuongDaXuat,
                                                                          KhoId = p.NhapKhoVatTu.KhoId,
                                                                          TenKho = p.NhapKhoVatTu.Kho.Ten,
                                                                          GiaTriSoLuongTon = p.SoLuongNhap * (double)p.DonGiaNhap - p.SoLuongDaXuat * (double)p.DonGiaNhap
                                                                      });

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            query = query.GroupBy(p => new { p.Id, p.KhoId })
                         .Select(p => new VatTuTonKhoTongHopGridVo
                         {
                             Id = p.First().Id,
                             TenVatTu = p.First().TenVatTu,
                             DonViTinh = p.First().DonViTinh,
                             SoLuongTon = p.Sum(p2 => p2.SoLuongTon),
                             KhoId = p.First().KhoId,
                             TenKho = p.First().TenKho,
                             GiaTriSoLuongTon = p.Sum(p2 => p2.GiaTriSoLuongTon)
                         });

            string finalresult = String.Empty;
            var lstVatTu = query.OrderBy(queryString.SortString).ToList();
            var i = 1;
            double totalGiaTriSoLuongTon = 0;
            foreach (var item in lstVatTu)
            {
                finalresult = finalresult + "<tr style='border: 1px solid #020000;'>"
                    + "<td style='padding:5px;border: 1px solid #020000;'>" + i + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.TenVatTu + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;'>" + item.DonViTinh + "</td>"
                                          + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + item.SoLuongTonDisplay + "</td>"
                              + "<td style = 'padding:5px;border: 1px solid #020000;text-align: right;'>" + item.GiaTriSoLuongTonFormat + "</td>"
                                          + "</tr>";
                totalGiaTriSoLuongTon += item.GiaTriSoLuongTon;
                i++;

            }
            string ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new VatTuTonKhoTongHopHTML
            {
                TemplateVatTuTonKhoTongHop = finalresult,
                TenKho = tenKho,
                Ngay = ngayThangHientai,
                TotalGiaTriSoLuongTon = ((decimal)totalGiaTriSoLuongTon).ApplyFormatMoneyVND()
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion

        #region Tồn kho vật tư (nhập xuất)
        public async Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var queryString = new VatTuTonKhoNhapXuatSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoNhapXuatSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }
            var searchTerms = string.Empty;
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new VatTuTonKhoNhapXuatGridVo[0], TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId && o.NgayNhap <= endDate)
                    .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo()
                    {
                        Id = o.Id,
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        Ma = o.VatTuBenhVien.Ma,
                        Ten = o.VatTuBenhVien.VatTus.Ten,
                        DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                        SoLo = o.Solo,
                        DonGiaNhap = o.DonGiaNhap,
                        VAT = o.VAT,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaVatTuBHYT,
                        //Nhom = o.VatTuBenhVien.VatTus != null ? o.VatTuBenhVien.VatTus.NhomVatTu != null ? o.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                        NhomId = o.VatTuBenhVien.VatTus.NhomVatTuId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ApplyLike(searchTerms, g => g.Ten, g => g.Ma).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryString.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= endDate)))
                .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = o.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    Ten = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    DVT = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SoLo = o.NhapKhoVatTuChiTiet.Solo,
                    DonGiaNhap = o.NhapKhoVatTuChiTiet.DonGiaNhap,
                    VAT = o.NhapKhoVatTuChiTiet.VAT,

                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    //Nhom = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                    NhomId = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTuId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ApplyLike(searchTerms, g => g.Ten, g => g.Ma).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();
            var allDataGroup = allDataNhapXuat.GroupBy(o => o.VatTuBenhVienId);
            var nhomVatTus = _nhomVatTuRepository.TableNoTracking.Select(o => new { o.Id, o.Ten }).ToList();
            var dataReturn = new List<VatTuTonKhoNhapXuatGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < startDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= startDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var tonCuoi = tonDau + nhapTrongKy - xuatTrongKy;
                dataReturn.Add(new VatTuTonKhoNhapXuatGridVo
                {
                    Id = xuatNhapDuocPham.Key,
                    KhoId = queryString.KhoId,
                    TenVatTu = xuatNhapDuocPham.First().Ten,
                    Ma = xuatNhapDuocPham.First().Ma,
                    DonViTinh = xuatNhapDuocPham.First().DVT,
                    TenNhomVatTu = nhomVatTus.FirstOrDefault(o=>o.Id == xuatNhapDuocPham.First().NhomId)?.Ten,
                    NhomVatTuId = xuatNhapDuocPham.First().NhomId,
                    TonDauKy = tonDau,
                    NhapTrongKy = nhapTrongKy,
                    XuatTrongKy = xuatTrongKy,
                    TonCuoiKy = tonCuoi,                    
                });
            }

            var countTask = queryInfo.LazyLoadPage == true ? 0 : dataReturn.Count();
            var queryTask = dataReturn.AsQueryable().OrderBy(queryInfo.SortString).ThenBy(o=>o.Ma).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };

            /*
            var tenKho = queryString.KhoId == 0 ? "" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == queryString.KhoId &&
                                                                                  (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.NgayNhap.Date <= endDate.Date)
                                                                      .Select(p => new VatTuTonKhoNhapXuatGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          KhoId = queryString.KhoId,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh,
                                                                          NhomVatTuId = p.VatTuBenhVien.VatTus.NhomVatTuId,
                                                                          TenNhomVatTu = p.VatTuBenhVien.VatTus.NhomVatTu.Ten,
                                                                          Ma = p.VatTuBenhVien.VatTus.Ma,
                                                                      })
                                                                      .Distinct();

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryTask.Count();

            //Gán lại do select ra null
            foreach (var item in queryTask)
            {
                var nhapKho = _nhapKhoVatTuChiTietRepository.TableNoTracking;
                var xuatKho = _xuatKhoVatTuChiTietRepository.TableNoTracking;

                item.TonDauKy = nhapKho.Any(p => p.NgayNhap.Date <= startDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                nhapKho.Where(p => p.NgayNhap.Date <= startDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                   (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                       .Sum(p => p.SoLuongNhap) -
                                       (
                                            xuatKho.Any(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= startDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                            (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                            (
                                                xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= startDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                                   (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                   .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                   .Any() ?
                                                xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= startDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                                   (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                       .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                       .Sum(p => p.SoLuongXuat) :
                                                0
                                            ) :
                                            0
                                       ) :
                                0;

                item.NhapTrongKy = nhapKho.Any(p => p.NgayNhap.Date >= startDate.Date && p.NgayNhap.Date <= endDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                    (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                   nhapKho.Where(p => p.NgayNhap.Date >= startDate.Date && p.NgayNhap.Date <= endDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                      (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                          .Sum(p => p.SoLuongNhap) :
                                   0;

                item.XuatTrongKy = xuatKho.Any(p => p.NgayXuat != null && p.NgayXuat.Value.Date >= startDate.Date && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                    (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                        (
                                            xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date >= startDate.Date && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                               (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                   .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                   .Any() ?
                                            xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date >= startDate && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                               (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                   .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                   .Sum(p => p.SoLuongXuat) :
                                            0
                                        ) :
                                    0;

                item.TonCuoiKy = nhapKho.Any(p => p.NgayNhap.Date <= endDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                  (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                 nhapKho.Where(p => p.NgayNhap.Date <= endDate.Date && p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                   (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                        .Sum(p => p.SoLuongNhap) -
                                        (
                                            xuatKho.Any(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                            (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc)) ?
                                            (
                                                xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                                   (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                   .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                   .Any() ?
                                                xuatKho.Where(p => p.NgayXuat != null && p.NgayXuat.Value.Date <= endDate.Date && p.XuatKhoVatTu.KhoXuatId == queryString.KhoId && p.VatTuBenhVienId == item.Id &&
                                                                   (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                       .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                       .Sum(p => p.SoLuongXuat) :
                                                0
                                            ) :
                                            0
                                       ) :
                                0;
            }
            
            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
            */
        }

        public async Task<GridDataSource> GetTotalVatTuTonKhoNhapXuatPagesForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var queryString = new VatTuTonKhoNhapXuatSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoNhapXuatSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }
            var searchTerms = string.Empty;
            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                searchTerms = queryString.SearchString.Replace("\t", "").Trim();
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId && o.NgayNhap <= endDate)
                .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo()
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.VatTuBenhVienId,
                    Ma = o.VatTuBenhVien.Ma,
                    Ten = o.VatTuBenhVien.VatTus.Ten,
                    DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                    SoLo = o.Solo,
                    DonGiaNhap = o.DonGiaNhap,
                    VAT = o.VAT,
                    NgayNhapXuat = o.NgayNhap,
                    LaVatTuBHYT = o.LaVatTuBHYT,                    
                    SLNhap = o.SoLuongNhap,
                    SLXuat = 0
                }).ApplyLike(searchTerms, g => g.Ten, g => g.Ma).ToList();

            var countTask = allDataNhap.GroupBy(o => o.VatTuBenhVienId).Count();

            return new GridDataSource { TotalRowCount = countTask };
            /*
            var tenKho = queryString.KhoId == 0 ? "" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var query = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == queryString.KhoId &&
                                                                                  (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NhapKhoVatTu.NgayNhap.Date <= endDate.Date)
                                                                      .Select(p => new VatTuTonKhoNhapXuatGridVo
                                                                      {
                                                                          Id = p.VatTuBenhVien.VatTus.Id,
                                                                          TenVatTu = p.VatTuBenhVien.VatTus.Ten,
                                                                          DonViTinh = p.VatTuBenhVien.VatTus.DonViTinh
                                                                      })
                                                                      .Distinct();

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenVatTu, g => g.DonViTinh);

            if (!string.IsNullOrEmpty(queryString.SearchString))
            {
                //var searchTerms = queryString.SearchString.Replace("\t", "").Trim();
                query = query.ApplyLike(searchTerms, g => g.TenVatTu, g => g.DonViTinh);
            }

            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take);
            var countTask = queryTask.Count();

            return new GridDataSource { TotalRowCount = countTask };
            */
        }

        public async Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatChiTietForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var queryString = new VatTuTonKhoNhapXuatSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoNhapXuatSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { Data = new VatTuTonKhoNhapXuatDetailGridVo[0], TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId && o.NgayNhap <= endDate && o.VatTuBenhVienId == queryString.VatTuId)
                .Select(o => new VatTuTonKhoNhapXuatDetailGridVo()
                {
                    Id = o.Id,
                    Ngay = o.NgayNhap,
                    NgayDisplay = o.NgayNhap.ApplyFormatDateTime(),
                    SoPhieu = o.NhapKhoVatTu.SoPhieu,
                    Nhap = o.SoLuongNhap,
                    Xuat = 0
                }).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryString.KhoId && o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryString.VatTuId
                            && ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= endDate)))
                .Select(o => new VatTuTonKhoNhapXuatDetailGridVo
                {
                    Id = o.Id,
                    Ngay = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    NgayDisplay = (o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat).ApplyFormatDateTime(),
                    isNhapKho = true,
                    SoPhieu = o.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu,
                    Nhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    Xuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).OrderBy(o => o.Ngay).ToList();

            var tonDauKy = allDataNhapXuat.Where(o => o.Ngay < startDate)
                .Select(o => o.Nhap - o.Xuat).DefaultIfEmpty(0).Sum();
            var allDataNhapXuatTuNgay = allDataNhapXuat.Where(o => o.Ngay >= startDate).ToList();
            for (int i = 0; i < allDataNhapXuatTuNgay.Count; i++)
            {
                allDataNhapXuatTuNgay[i].STT = i + 1;
                if (i == 0)
                {
                    allDataNhapXuatTuNgay[i].Ton = tonDauKy + allDataNhapXuatTuNgay[i].Nhap - allDataNhapXuatTuNgay[i].Xuat;
                }
                else
                {
                    allDataNhapXuatTuNgay[i].Ton = allDataNhapXuatTuNgay[i - 1].Ton + allDataNhapXuatTuNgay[i].Nhap - allDataNhapXuatTuNgay[i].Xuat;
                }
            }

            return new GridDataSource { Data = allDataNhapXuatTuNgay.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = allDataNhapXuatTuNgay.Count };

            /*
            var queryNhapKho = _nhapKhoVatTuRepository.TableNoTracking.Where(p => p.KhoId == queryString.KhoId && p.NgayNhap.Date >= startDate.Date && p.NgayNhap.Date <= endDate.Date &&
                                                                                  (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                                      .SelectMany(p => p.NhapKhoVatTuChiTiets)
                                                                      .Where(p => p.VatTuBenhVien.VatTus.Id == queryString.VatTuId)
                                                                      .Select(p => new VatTuTonKhoNhapXuatDetailGridVo
                                                                      {
                                                                          Id = p.Id,
                                                                          Ngay = p.NgayNhap,
                                                                          NgayDisplay = p.NgayNhap.ApplyFormatDateTime(),
                                                                          SoPhieu = p.NhapKhoVatTu.SoPhieu,
                                                                          isNhapKho = true,
                                                                          Nhap = p.SoLuongNhap,
                                                                          Xuat = 0,
                                                                      }).Distinct();

            var queryXuatKho = _xuatKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.XuatKhoVatTu.KhoXuatId == queryString.KhoId &&
                                                                                         (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                         p.NgayXuat != null && p.NgayXuat.Value.Date >= startDate.Date && p.NgayXuat.Value.Date <= endDate.Date && p.VatTuBenhVien.VatTus.Id == queryString.VatTuId)
                                                                             .Select(p => new VatTuTonKhoNhapXuatDetailGridVo
                                                                             {
                                                                                 Id = p.Id,
                                                                                 Ngay = p.NgayXuat,
                                                                                 NgayDisplay = p.NgayXuat != null ? p.NgayXuat.Value.ApplyFormatDateTime() : string.Empty,
                                                                                 SoPhieu = p.XuatKhoVatTu.SoPhieu,
                                                                                 isNhapKho = false,
                                                                                 Nhap = 0,
                                                                                 Xuat = p.XuatKhoVatTuChiTietViTris.Sum(p2 => p2.SoLuongXuat),
                                                                             }).Distinct();

            var queryFormat = queryNhapKho.Concat(queryXuatKho);

            var queryTask = queryFormat.Skip(queryInfo.Skip).Take(queryInfo.Take);
            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryTask.Count();

            if (!string.IsNullOrEmpty(queryInfo.SortString))
            {
                queryTask = queryFormat.OrderBy(queryInfo.SortString);
            }

            var result = queryTask.ToArray();

            var idx = 1;

            foreach (var item in result)
            {
                item.STT = idx++;
            }

            //Tính tổng sl tồn từ đầu đến trước startDate
            var slTonKho = (double)0;

            slTonKho += _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.NhapKhoVatTu.KhoId == queryString.KhoId && p.VatTuBenhVienId == queryString.VatTuId &&
                                                                                  (p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.NhapKhoVatTu.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.NgayNhap.Date < startDate.Date)
                                                                      .Sum(p => p.SoLuongNhap);

            slTonKho -= _xuatKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.XuatKhoVatTu.KhoXuatId == queryString.KhoId &&
                                                                                  (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                  p.VatTuBenhVienId == queryString.VatTuId && p.NgayXuat != null && p.NgayXuat.Value.Date < startDate.Date)
                                                                      .SelectMany(p => p.XuatKhoVatTuChiTietViTris)
                                                                      .Sum(p => p.SoLuongXuat);

            //Chỉnh sửa sl tồn theo luỹ kế nhập & xuất
            var resultOrderedByDate = result.OrderBy(p => p.Ngay).ToList();
            var listDictionary = new Dictionary<int?, double>();

            foreach (var item in resultOrderedByDate)
            {
                slTonKho = item.isNhapKho ? slTonKho + item.Nhap : slTonKho - item.Xuat;
                listDictionary.Add(item.STT, slTonKho);
            }

            foreach (var item in result)
            {
                item.Ton = listDictionary.Where(p => p.Key == item.STT).FirstOrDefault().Value;
            }

            return new GridDataSource { Data = result, TotalRowCount = countTask };
            */
        }

        public async Task<GridDataSource> GetDanhSachVatTuTonKhoNhapXuatChiTietPagesForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            var queryString = new VatTuTonKhoNhapXuatSearchGridVoItem();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoNhapXuatSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
            }

            if (queryString.KhoId == 0)
            {
                return new GridDataSource { TotalRowCount = 0 };
            }

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId && o.NgayNhap >= startDate && o.NgayNhap <= endDate && o.VatTuBenhVienId == queryString.VatTuId)
                .Select(o => new VatTuTonKhoNhapXuatDetailGridVo()
                {
                    Id = o.Id
                }).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryString.KhoId && o.NhapKhoVatTuChiTiet.VatTuBenhVienId == queryString.VatTuId
                            && ((o.NgayXuat != null && o.NgayXuat >= startDate && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat >= startDate && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= endDate)))
                .Select(o => new VatTuTonKhoNhapXuatDetailGridVo
                {
                    Id = o.Id
                }).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();

            return new GridDataSource { TotalRowCount = allDataNhapXuat.Count };
            /*
            var queryNhapKho = _nhapKhoVatTuRepository.TableNoTracking.Where(p => p.KhoId == queryString.KhoId && p.NgayNhap.Date >= startDate.Date && p.NgayNhap.Date <= endDate.Date &&
                                                                                  (p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.Kho.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc))
                                                                      .SelectMany(p => p.NhapKhoVatTuChiTiets)
                                                                      .Where(p => p.VatTuBenhVien.VatTus.Id == queryString.VatTuId)
                                                                      .Select(p => new VatTuTonKhoNhapXuatDetailGridVo
                                                                      {
                                                                          NgayDisplay = p.NgayNhap.ApplyFormatDateTime(),
                                                                          SoPhieu = p.NhapKhoVatTu.SoPhieu,
                                                                          Nhap = p.SoLuongNhap,
                                                                          Xuat = 0,
                                                                      }).Distinct();

            var queryXuatKho = _xuatKhoVatTuChiTietRepository.TableNoTracking.Where(p => p.XuatKhoVatTu.KhoXuatId == queryString.KhoId &&
                                                                                         (p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoTongVTYTCap2 || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.KhoLe || p.XuatKhoVatTu.KhoVatTuXuat.LoaiKho == EnumLoaiKhoDuocPham.NhaThuoc) &&
                                                                                         p.NgayXuat != null && p.NgayXuat.Value.Date <= endDate.Date && p.NgayXuat.Value.Date >= startDate.Date && p.VatTuBenhVien.VatTus.Id == queryString.VatTuId)
                                                                             .Select(p => new VatTuTonKhoNhapXuatDetailGridVo
                                                                             {
                                                                                 NgayDisplay = p.NgayXuat != null ? p.NgayXuat.Value.ApplyFormatDateTime() : string.Empty,
                                                                                 SoPhieu = p.XuatKhoVatTu.SoPhieu,
                                                                                 Nhap = 0,
                                                                                 Xuat = p.XuatKhoVatTuChiTietViTris.Sum(p2 => p2.SoLuongXuat)
                                                                             }).Distinct();

            var queryFormat = queryNhapKho.Concat(queryXuatKho);

            var queryTask = queryFormat.Skip(queryInfo.Skip).Take(queryInfo.Take);
            var countTask = queryTask.Count();

            return new GridDataSource { TotalRowCount = countTask };
            */
        }

        public string GetVatTuTonKhoNhapXuatHTML(string search)
        {
            var result = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("BaoCaoVatTuTonKhoNhapXuat")).FirstOrDefault();

            var startDate = new DateTime(1970,01,01);
            var endDate = new DateTime();
            endDate = DateTime.Now;
            var searchString = string.Empty;
            var sortString = string.Empty;

            var queryString = new VatTuTonKhoNhapXuatSearchGridVoItem();

            if (!string.IsNullOrEmpty(search))
            {
                queryString = JsonConvert.DeserializeObject<VatTuTonKhoNhapXuatSearchGridVoItem>(search);

                if (queryString.RangeDate != null)
                {
                    startDate = queryString.RangeDate.TuNgay ?? startDate;
                    endDate = (queryString.RangeDate.DenNgay ?? endDate).Date.AddDays(1).AddMilliseconds(-1);
                }
                searchString = queryString.SearchString;
                sortString = queryString.SortString;
            }

            var tenKho = queryString.KhoId == 0 ? "" : _khoDuocPhamRepository.TableNoTracking.Where(p => p.Id == queryString.KhoId).Select(p => p.Ten).FirstOrDefault();

            var allDataNhap = _nhapKhoVatTuChiTietRepository.TableNoTracking.Where(o => o.NhapKhoVatTu.KhoId == queryString.KhoId && o.NgayNhap <= endDate)
                    .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo()
                    {
                        Id = o.Id,
                        VatTuBenhVienId = o.VatTuBenhVienId,
                        Ma = o.VatTuBenhVien.Ma,
                        Ten = o.VatTuBenhVien.VatTus.Ten,
                        DVT = o.VatTuBenhVien.VatTus.DonViTinh,
                        SoLo = o.Solo,
                        DonGiaNhap = o.DonGiaNhap,
                        VAT = o.VAT,
                        NgayNhapXuat = o.NgayNhap,
                        LaVatTuBHYT = o.LaVatTuBHYT,
                        Nhom = o.VatTuBenhVien.VatTus != null ? o.VatTuBenhVien.VatTus.NhomVatTu != null ? o.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                        NhomId = o.VatTuBenhVien.VatTus.NhomVatTuId,
                        SLNhap = o.SoLuongNhap,
                        SLXuat = 0
                    }).ApplyLike(searchString, g => g.Ten, g => g.Ma).ToList();

            var allDataXuat = _xuatKhoVatTuChiTietViTriRepository.TableNoTracking
                .Where(o => o.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null &&
                            o.XuatKhoVatTuChiTiet.XuatKhoVatTu.KhoXuatId == queryString.KhoId
                            && ((o.NgayXuat != null && o.NgayXuat <= endDate) ||
                                (o.NgayXuat == null && o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat <= endDate)))
                .Select(o => new BaoCaoChiTietXuatNhapTonVTGridVo
                {
                    Id = o.Id,
                    VatTuBenhVienId = o.NhapKhoVatTuChiTiet.VatTuBenhVienId,
                    Ma = o.NhapKhoVatTuChiTiet.VatTuBenhVien.Ma,
                    Ten = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.Ten,
                    DVT = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.DonViTinh,
                    SoLo = o.NhapKhoVatTuChiTiet.Solo,
                    DonGiaNhap = o.NhapKhoVatTuChiTiet.DonGiaNhap,
                    VAT = o.NhapKhoVatTuChiTiet.VAT,

                    NgayNhapXuat = o.NgayXuat != null
                        ? o.NgayXuat.Value
                        : o.XuatKhoVatTuChiTiet.XuatKhoVatTu.NgayXuat,
                    LaVatTuBHYT = o.NhapKhoVatTuChiTiet.LaVatTuBHYT,
                    Nhom = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu != null ? o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTu.Ten : "" : "",
                    NhomId = o.NhapKhoVatTuChiTiet.VatTuBenhVien.VatTus.NhomVatTuId,
                    SLNhap = o.SoLuongXuat < 0 ? o.SoLuongXuat * (-1) : 0,
                    SLXuat = o.SoLuongXuat > 0 ? o.SoLuongXuat : 0,
                }).ApplyLike(searchString, g => g.Ten, g => g.Ma).ToList();

            var allDataNhapXuat = allDataNhap.Concat(allDataXuat).ToList();
            var allDataGroup = allDataNhapXuat.GroupBy(o => o.VatTuBenhVienId);

            var dataReturn = new List<VatTuTonKhoNhapXuatGridVo>();
            foreach (var xuatNhapDuocPham in allDataGroup)
            {
                var tonDau = xuatNhapDuocPham.Where(o => o.NgayNhapXuat < startDate)
                    .Select(o => o.SLNhap.GetValueOrDefault() - o.SLXuat.GetValueOrDefault()).DefaultIfEmpty(0).Sum();
                var allDataNhapXuatTuNgay = xuatNhapDuocPham.Where(o => o.NgayNhapXuat >= startDate).ToList();
                var nhapTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLNhap.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var xuatTrongKy = allDataNhapXuatTuNgay.Select(o => o.SLXuat.GetValueOrDefault()).DefaultIfEmpty().Sum();
                var tonCuoi = tonDau + nhapTrongKy - xuatTrongKy;
                dataReturn.Add(new VatTuTonKhoNhapXuatGridVo
                {
                    Id = xuatNhapDuocPham.Key,
                    KhoId = queryString.KhoId,
                    TenVatTu = xuatNhapDuocPham.First().Ten,
                    Ma = xuatNhapDuocPham.First().Ma,
                    DonViTinh = xuatNhapDuocPham.First().DVT,
                    TenNhomVatTu = xuatNhapDuocPham.First().Nhom,
                    NhomVatTuId = xuatNhapDuocPham.First().NhomId,
                    TonDauKy = tonDau,
                    NhapTrongKy = nhapTrongKy,
                    XuatTrongKy = xuatTrongKy,
                    TonCuoiKy = tonCuoi
                });
            }
            var vatTuPhanNhoms = dataReturn.Select(z => new { z.NhomVatTuId, z.TenNhomVatTu }).Distinct().OrderBy(x => x.TenNhomVatTu).ToList();
            var finalresult = string.Empty;
            var STT = 1;
            foreach (var nhom in vatTuPhanNhoms)
            {
                var coHeaderGroup = true;
                foreach (var item in dataReturn)
                {
                    if (nhom.NhomVatTuId == item.NhomVatTuId)
                    {
                        if (coHeaderGroup)
                        {
                            var headerBHYT = "<tr style='border: 1px solid #020000;text-align: center; '>"
                                          + "<td style='border: 1px solid #020000;text-align: left;' colspan='8'><b>" + nhom.TenNhomVatTu
                                          + "</b></tr>";
                            finalresult += headerBHYT;
                            coHeaderGroup = false;
                        }
                        finalresult = finalresult + "<tr style='border: 1px solid #020000;text-align: center;'>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + STT + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.Ma + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TenVatTu + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.DonViTinh + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TonDauKyDisplay + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.NhapTrongKyDisplay + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.XuatTrongKyDisplay + "</td>"
                                            + "<td style = 'border: 1px solid #020000;text-align: center;'>" + item.TonCuoiKyDisplay + "</td>"
                                            + "</tr>";
                        STT++;
                    }
                }
            }

            string ngayThangHientai = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new VatTuTonKhoNhapXuatHTML
            {
                TemplateVatTuTonKhoNhapXuat = finalresult,
                TenKho = tenKho,
                StartDate = startDate.ApplyFormatDate(),
                EndDate = endDate.ApplyFormatDate(),
                Ngay = ngayThangHientai
            };
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
        #endregion

        public async Task<ChiTietVatTuTonKhoNhapXuat> GetVatTuAndKhoName(ChiTietVatTuTonKhoNhapXuat model)
        {
            model.TenVatTu = (await _vatTuRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.VatTuId))?.Ten;
            model.KhoDisplay = (await _khoDuocPhamRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == model.KhoId))?.Ten;

            return model;
        }

        public async Task<GridDataSource> GetChiTietTonKhoCuaVatTu(QueryInfo queryInfo)
        {
            var vatTuId = _vatTuRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoVatTuChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                vatTuId = queryString.VatTuId;
            }
            //var tonDauKy = _nhapKhoVatTuRepository.TableNoTracking
            //    .Where(x => x.KhoId == khoId)
            //    .Sum(o => o.NhapKhoVatTuChiTiets.Where(c => c.VatTuBenhVienId == vatTuId)
            //                                        .Select(c => c.SoLuongNhap - c.XuatKhoVatTuChiTietViTris.Where(p => p.NgayXuat != null).Select(p => p.SoLuongXuat).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum());

            var queryNhapKho = _nhapKhoVatTuRepository.TableNoTracking
                .Where(x => x.KhoId == khoId)
                .SelectMany(p => p.NhapKhoVatTuChiTiets)
                .Where(x => x.VatTuBenhVien.VatTus.Id == vatTuId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.NgayNhap,
                    NgayDisplay = s.NgayNhap.ApplyFormatDateTime(),
                    MaChungTu = s.NhapKhoVatTu.SoPhieu,
                    Nhap = s.SoLuongNhap,
                    Xuat = 0,
                    SoLo = s.Solo,
                    HanSuDung = s.HanSuDung,
                    Loai = 1,
                    SoLuong = s.SoLuongNhap,
                    SoLuongDaXuat = s.SoLuongDaXuat,
                    MaVach = s.MaVach,
                    MaRef = s.MaRef,
                    VAT = s.VAT,
                    TiLeBHYTThanhToan = s.TiLeBHYTThanhToan,
                    LaVatTuBHYT = s.LaVatTuBHYT,
                    DonGiaNhap = s.DonGiaNhap,
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung.GetDescription()
                });

            var queryXuatKho = _xuatKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTu.KhoXuatId == khoId
                        && x.NgayXuat != null && x.VatTuBenhVien.VatTus.Id == vatTuId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.NgayXuat.Value,
                    NgayDisplay = s.NgayXuat != null ? s.NgayXuat.Value.ApplyFormatDateTime() : string.Empty,
                    MaChungTu = s.XuatKhoVatTu.SoPhieu,
                    Nhap = 0,
                    Xuat = s.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat),
                    Loai = 2,
                    SoLuong = s.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat),
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung.GetDescription()
                });

            var queryDangBook = _xuatKhoVatTuChiTietRepository.TableNoTracking
                .Where(x => x.XuatKhoVatTuId == null && x.XuatKhoVatTuChiTietViTris.Any(o => o.SoLuongXuat > 0 && o.NhapKhoVatTuChiTiet.NhapKhoVatTu.KhoId == khoId)
                        && x.VatTuBenhVien.VatTus.Id == vatTuId)
                .Select(s => new NhapXuatTonKhoCapNhatDetailGridVo
                {
                    Id = s.Id,
                    NgayNhapXuat = s.CreatedOn.Value,
                    NgayDisplay = s.CreatedOn != null ? s.CreatedOn.Value.ApplyFormatDateTime() : string.Empty,
                    MaChungTu = "Đang book",
                    Nhap = 0,
                    Xuat = s.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat),
                    Loai = 2,
                    SoLuong = s.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat),
                    LoaiSuDung = s.VatTuBenhVien.LoaiSuDung.GetDescription(),
                    HighLightClass = "bg-row-lightpink",
                    ThongTinBooking = s.XuatKhoVatTuChiTietViTris.Any(o => o.DonVTYTThanhToanChiTiets.Any()) ? "Mã YCTN có đơn VTYT: " + string.Join(", ", s.XuatKhoVatTuChiTietViTris.SelectMany(o => o.DonVTYTThanhToanChiTiets.Select(i => i.DonVTYTThanhToan.YeuCauTiepNhan.MaYeuCauTiepNhan)).Distinct()) :
                    (s.XuatKhoVatTuChiTietViTris.Any(o => o.YeuCauXuatKhoVatTuChiTiets.Any()) ? "Yêu cầu xuất vật tư: " + string.Join(", ", s.XuatKhoVatTuChiTietViTris.SelectMany(o => o.YeuCauXuatKhoVatTuChiTiets.Select(i => i.YeuCauXuatKhoVatTu.SoChungTu)).Distinct()) :                    
                    (s.XuatKhoVatTuChiTietViTris.Any(o => o.YeuCauTraVatTuChiTiets.Any()) ? "Yều cầu trả vật tư: " + string.Join(", ", s.XuatKhoVatTuChiTietViTris.SelectMany(o => o.YeuCauTraVatTuChiTiets.Select(i => i.YeuCauTraVatTu.SoPhieu)).Distinct()) :
                    (s.XuatKhoVatTuChiTietViTris.Any(o => o.XuatKhoVatTuChiTiet.YeuCauVatTuBenhViens.Any()) ? "Mã YCTN có vật tư: " + string.Join(", ", s.XuatKhoVatTuChiTietViTris.SelectMany(o => o.XuatKhoVatTuChiTiet.YeuCauVatTuBenhViens.Select(i => i.YeuCauTiepNhan.MaYeuCauTiepNhan)).Distinct()) : "")))
                });
            var queryFormat = queryNhapKho.Concat(queryXuatKho).Concat(queryDangBook).OrderByDescending(o => o.NgayNhapXuat);

            //var index = 1;
            //var result = await queryFormat.ToListAsync();

            //var slTon = tonDauKy;
            //foreach (var item in result)
            //{
            //    item.STT = index;
            //    item.Ton = slTon + item.Nhap - item.Xuat;
            //    slTon = item.Ton;
            //    index++;
            //}

            var query = queryFormat.AsQueryable();
            var countTask = queryInfo.LazyLoadPage == true ? 0 : query.Count();


            var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take);

            //if (!string.IsNullOrEmpty(queryInfo.SortString))
            //{
            //    queryTask = queryTask.OrderBy(queryInfo.SortString);
            //}
            return new GridDataSource { Data = queryTask.ToArray(), TotalRowCount = countTask };
        }

        public double GetTongTonKhoCuaVatTu(QueryInfo queryInfo)
        {
            var vatTuId = _vatTuRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;
            var khoId = BaseRepository.TableNoTracking.FirstOrDefault()?.Id ?? 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<NhatXuatTonKhoVatTuChiTietGridVoItem>(queryInfo.AdditionalSearchString);
                khoId = queryString.KhoId == 0 ? khoId : queryString.KhoId;
                vatTuId = queryString.VatTuId;
            }

            var tongTon = _nhapKhoVatTuRepository.TableNoTracking
                .Where(x => x.KhoId == khoId)
                .SelectMany(p => p.NhapKhoVatTuChiTiets)
                .Where(x => x.VatTuBenhVien.VatTus.Id == vatTuId)
                .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat);

            return tongTon;
        }
        public void UpdateChiTietTonKhoCuaVatTu(CapNhatTonKhoVatTuItem capNhatTonKhoItem, out List<dynamic> errors)
        {
            errors = new List<dynamic>();
            if (capNhatTonKhoItem.CapNhatTonKhoItemDetails.Any())
            {
                foreach (var item in capNhatTonKhoItem.CapNhatTonKhoItemDetails)
                {
                    if (item.Loai == 1)
                    {
                        var nhapKhoVatTuChiTiet = _nhapKhoVatTuChiTietRepository.GetById(item.Id);
                        if (nhapKhoVatTuChiTiet != null)
                        {
                            if (string.IsNullOrEmpty(item.SoLo))
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].SoLo", Message = "Số lô yêu cầu nhập" });
                            }
                            if (item.HanSuDung == null)
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].HanSuDung", Message = "Hạn sử dụng yêu cầu nhập" });
                            }
                            if (item.DonGiaNhap == null)
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].DonGiaNhap", Message = "Giá nhập yêu cầu nhập" });
                            }

                            if (item.LaVatTuBHYT && item.TiLeBHYTThanhToan == null)
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].TiLeBHYTThanhToan", Message = "TL bảo hiểm thanh toán yêu cầu nhập" });
                            }
                            if (!item.LaVatTuBHYT && item.VAT == null)
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].VAT", Message = "VAT yêu cầu nhập" });
                            }
                            if (item.SoLuong < nhapKhoVatTuChiTiet.SoLuongDaXuat)
                            {
                                nhapKhoVatTuChiTiet.SoLuongNhap = item.SoLuong;
                                nhapKhoVatTuChiTiet.HanSuDung = item.HanSuDung.GetValueOrDefault();
                                nhapKhoVatTuChiTiet.Solo = item.SoLo;
                                nhapKhoVatTuChiTiet.MaVach = item.MaVach;
                                nhapKhoVatTuChiTiet.MaRef = item.MaRef;
                                nhapKhoVatTuChiTiet.DonGiaNhap = item.DonGiaNhap.GetValueOrDefault();
                                if (nhapKhoVatTuChiTiet.LaVatTuBHYT)
                                {
                                    nhapKhoVatTuChiTiet.TiLeBHYTThanhToan = item.TiLeBHYTThanhToan.GetValueOrDefault();
                                    nhapKhoVatTuChiTiet.VAT = 0;
                                }
                                else
                                {
                                    nhapKhoVatTuChiTiet.TiLeBHYTThanhToan = null;
                                    nhapKhoVatTuChiTiet.VAT = item.VAT.GetValueOrDefault();
                                }
                                _nhapKhoVatTuChiTietRepository.Update(nhapKhoVatTuChiTiet);
                            }
                            else
                            {
                                errors.Add(new { Field = "CapNhatTonKhoItemDetails[" + item.Id + "].SoLuong", Message = "Số lượng không được nhỏ hơn số lượng đã xuất" });
                            }
                        }
                    }
                    //                    else
                    //                    {
                    //                        var xuatKhoVatTuChiTiet = _xuatKhoVatTuChiTietRepository.GetById(item.Id);
                    //                        if (xuatKhoVatTuChiTiet != null)
                    //                        {
                    //                            var soLuongXuat =
                    //                                xuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Sum(p => p.SoLuongXuat);
                    //                            if (item.SoLuong > soLuongXuat)
                    //                            {
                    //                                
                    //                            }
                    //                            else
                    //                            {
                    //                                if (item.SoLuong < soLuongXuat)
                    //                                {
                    //
                    //                                }
                    //                            }
                    //                            xuatKhoVatTuChiTiet. = item.SoLuong;
                    //                        }
                    //                    }
                }
            }
            _nhapKhoVatTuChiTietRepository.Context.SaveChanges();
        }

        public async Task CapNhatChiTietVatTu(CapNhatTonKhoVatTuVo capNhatTonKhoVatTuVo)
        {
            foreach (var nkct in capNhatTonKhoVatTuVo.NhapKhoVatTuChiTiets.Where(z => z.Loai == 1))
            {
                var nhapKhoVatTuChiTiet = _nhapKhoVatTuChiTietRepository.GetById(nkct.Id);
                nhapKhoVatTuChiTiet.VatTuBenhVienId = capNhatTonKhoVatTuVo.VatTuId.GetValueOrDefault();
                nhapKhoVatTuChiTiet.Solo = nkct.SoLo;
                nhapKhoVatTuChiTiet.HanSuDung = nkct.HanSuDung.GetValueOrDefault();
                nhapKhoVatTuChiTiet.MaRef = nkct.MaRef;
                nhapKhoVatTuChiTiet.MaVach = nkct.MaVach;
                nhapKhoVatTuChiTiet.DonGiaNhap = nkct.DonGiaNhap.GetValueOrDefault();
                nhapKhoVatTuChiTiet.VAT = nkct.VAT.GetValueOrDefault();
                nhapKhoVatTuChiTiet.TiLeBHYTThanhToan = nkct.TiLeBHYTThanhToan.GetValueOrDefault();
                nhapKhoVatTuChiTiet.SoLuongNhap = nkct.SoLuong.GetValueOrDefault();
                await _nhapKhoVatTuChiTietRepository.UpdateAsync(nhapKhoVatTuChiTiet);
            }
        }

        public async Task<bool> KiemTraSoLuongHopLe(double? soLuong, double? soLuongXuat)
        {
            if (soLuong == null || soLuongXuat == null)
            {
                return true;
            }
            if (soLuong.Value < soLuongXuat.Value)
            {
                return false;
            }
            return true;
        }


        public async Task<List<VatTuLookupVo>> GetVatTuBenhVien(DropDownListRequestModel queryInfo)
        {

            var lstColumnNameSearch = new List<string>
            {
                nameof(Core.Domain.Entities.VatTus.VatTu.Ma),
                nameof(Core.Domain.Entities.VatTus.VatTu.Ten)

            };

            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                var result = _vatTuBenhVienRepository.TableNoTracking
                   .Where(p => p.HieuLuc)
                   .Select(s => new VatTuLookupVo
                   {
                       KeyId = s.Id,
                       DisplayName = s.VatTus.Ten,
                       Ten = s.VatTus.Ten,
                       Ma = s.VatTus.Ma,
                   })
                   .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ten, o => o.Ma)
                   .Take(queryInfo.Take)
                   ;
                return await result.ToListAsync();
            }
            else
            {
                var lstDuocPhamId = await _vatTuRepository
                  .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                  //.Where(dv => dv.DuocPhamBenhVien.HieuLuc == true)
                  .Select(s => s.Id).ToListAsync();

                var dct = lstDuocPhamId.Select((p, i) => new
                {
                    key = p,
                    rank = i
                }).ToDictionary(o => o.key, o => o.rank);

                var lstDichVuKyThuatBenhVien = _vatTuBenhVienRepository
                                    .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.VatTus.VatTu), lstColumnNameSearch)
                 //.Where(dv => dv.HieuLuc == true)
                 .Select(s => new VatTuLookupVo
                 {
                     KeyId = s.Id,
                     DisplayName = s.VatTus.Ten,
                     Ten = s.VatTus.Ten,
                     Ma = s.VatTus.Ma,
                 })
                  .OrderBy(p => dct.Any(a => a.Key == p.KeyId) ? dct[p.KeyId] : dct.Count)
                  .Take(queryInfo.Take);
                return await lstDichVuKyThuatBenhVien.ToListAsync();
            }

        }
    }
}
