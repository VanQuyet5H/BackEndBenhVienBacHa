using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DuocPhamBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.YeuCauKhamBenh;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.DuocPhamBenhVien
{
    [ScopedDependency(ServiceType = typeof(IDuocPhamDaHetHanService))]
    public class DuocPhamDaHetHanService
        : MasterFileService<NhapKhoDuocPham>
            , IDuocPhamDaHetHanService
    {
        private readonly IRepository<Template> _templateRepository;

        public DuocPhamDaHetHanService
        (
            IRepository<NhapKhoDuocPham> repository,
            IRepository<Template> templateRepository
        )
            : base(repository)
        {
            _templateRepository = templateRepository;
        }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var duocPham = new List<DuocPhamDaHetHanGridVo>();
            List<DuocPhamDaHetHanGridVo> duocPhamQuery;
            DuocPhamDaHetHanGridVo duocPhamChi;

            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens)
                .ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
                .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri)
                .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
                .Select(s => new DuocPhamDaHetHanGridVo
                {
                    Id = s.Id,
                    DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                    HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                    DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                    Kho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                    NgayHetHanDisplay = s.HanSuDung.ApplyFormatDate(),
                    NgayHetHan = s.HanSuDung,
                    ViTri = s.KhoDuocPhamViTri.Ten,
                    ViTriId = s.KhoDuocPhamViTri.Id,
                    SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                    SoLo = s.Solo,
                    NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                    DuocPhamId = s.DuocPhamBenhVienId,
                    MaDuocPham = s.DuocPhamBenhViens.Ma,
                    DonGiaNhap = s.DonGiaNhap,
                    HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong
                }).Where(p => p.NgayHetHan < DateTime.Now.Date)
                .GroupBy(item => new
                {
                    item.DuocPhamId,
                    item.KhoId,
                    item.DonGiaNhap,
                    item.SoLo,
                    item.NgayHetHan,
                }).Select(s => new DuocPhamDaHetHanGridVo
                {
                    Id = s.First().Id,
                    DuocPham = s.First().DuocPham,
                    HoatChat = s.First().HoatChat,
                    DonViTinh = s.First().DonViTinh,
                    Kho = s.First().Kho,
                    NgayHetHanDisplay = s.First().NgayHetHanDisplay,
                    NgayHetHan = s.First().NgayHetHan,
                    ViTri = s.First().ViTri,
                    ViTriId = s.First().ViTriId,
                    SoLuongTon =s.Sum(d=>d.SoLuongTon),
                    SoLo = s.First().SoLo,
                    NhapKhoDuocPhamId = s.First().NhapKhoDuocPhamId,
                    DuocPhamId = s.First().DuocPhamId,
                    MaDuocPham = s.First().MaDuocPham,
                    DonGiaNhap = s.First().DonGiaNhap,
                    HamLuong = s.First().HamLuong
                });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuocPhamDaHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.KhoId != 0)
                {
                    query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
                }

                if (!string.IsNullOrEmpty(queryString.DuocPham))
                {
                    query = query.ApplyLike(queryString.DuocPham.TrimEnd().TrimStart(), g => g.DuocPham);
                }
            }

            duocPhamQuery = query.ToList();
            for (int i = 0; i < duocPhamQuery.Count; i++)
            {
                if (i == 0)
                {
                    double soLuongTon;
                    if (string.IsNullOrEmpty(duocPhamQuery.ToList()[i].ViTri))
                    {
                        soLuongTon = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamQuery.RemoveAll(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) &&
                            x.ViTriId.Equals(duocPhamQuery.ToList()[i].ViTriId)
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).ToList();
                        soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamQuery.Remove(item);
                        }
                    }

                    i = -1;
                }
            }

            var queryable = duocPham.AsQueryable();

            var countTask = queryInfo.LazyLoadPage == true ? 0 : queryable.Count();

            var queryTask = queryable.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArray();

            return new GridDataSource
            {
                Data = queryTask,
                TotalRowCount = countTask
            };
        }

        public GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var duocPham = new List<DuocPhamDaHetHanGridVo>();
            List<DuocPhamDaHetHanGridVo> duocPhamQuery;
            DuocPhamDaHetHanGridVo duocPhamChi;

            var query = BaseRepository.TableNoTracking
               .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens)
               .ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
               .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri)
               .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
               .Select(s => new DuocPhamDaHetHanGridVo
               {
                   Id = s.Id,
                   DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                   HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                   DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   Kho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                   NgayHetHanDisplay = s.HanSuDung.ApplyFormatDate(),
                   NgayHetHan = s.HanSuDung,
                   ViTri = s.KhoDuocPhamViTri.Ten,
                   ViTriId = s.KhoDuocPhamViTri.Id,
                   SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                   SoLo = s.Solo,
                   NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                   DuocPhamId = s.DuocPhamBenhVienId,
                   MaDuocPham = s.DuocPhamBenhViens.Ma,
                   DonGiaNhap = s.DonGiaNhap,
                   HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong
               }).Where(p => p.NgayHetHan < DateTime.Now.Date)
               .GroupBy(item => new
               {
                   item.DuocPhamId,
                   item.KhoId,
                   item.DonGiaNhap,
                   item.SoLo,
                   item.NgayHetHan,
               }).Select(s => new DuocPhamDaHetHanGridVo
               {
                   Id = s.First().Id,
                   DuocPham = s.First().DuocPham,
                   HoatChat = s.First().HoatChat,
                   DonViTinh = s.First().DonViTinh,
                   Kho = s.First().Kho,
                   NgayHetHanDisplay = s.First().NgayHetHanDisplay,
                   NgayHetHan = s.First().NgayHetHan,
                   ViTri = s.First().ViTri,
                   ViTriId = s.First().ViTriId,
                   SoLuongTon = s.Sum(d => d.SoLuongTon),
                   SoLo = s.First().SoLo,
                   NhapKhoDuocPhamId = s.First().NhapKhoDuocPhamId,
                   DuocPhamId = s.First().DuocPhamId,
                   MaDuocPham = s.First().MaDuocPham,
                   DonGiaNhap = s.First().DonGiaNhap,
                   HamLuong = s.First().HamLuong
               });

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DuocPhamDaHetHanSearchGridVoItem>(queryInfo.AdditionalSearchString);

                if (queryString.KhoId != 0)
                {
                    query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
                }

                if (!string.IsNullOrEmpty(queryString.DuocPham))
                {
                    query = query.ApplyLike(queryString.DuocPham.TrimEnd().TrimStart(), g => g.DuocPham);
                }
            }

            duocPhamQuery = query.ToList();
            for (int i = 0; i < duocPhamQuery.Count; i++)
            {
                if (i == 0)
                {
                    double soLuongTon;
                    if (string.IsNullOrEmpty(duocPhamQuery.ToList()[i].ViTri))
                    {
                        soLuongTon = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamQuery.RemoveAll(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) &&
                            x.ViTriId.Equals(duocPhamQuery.ToList()[i].ViTriId)
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).ToList();
                        soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamQuery.Remove(item);
                        }
                    }

                    i = -1;
                }
            }

            var queryable = duocPham.AsQueryable();

            var countTask = queryable.Count();

            return new GridDataSource
            {
                TotalRowCount = countTask
            };
        }

        public string GetHtml(string search)
        {
            var duocPham = new List<DuocPhamDaHetHanGridVo>();
            List<DuocPhamDaHetHanGridVo> duocPhamQuery;
            DuocPhamDaHetHanGridVo duocPhamChi;

            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("XemDanhMucDaHetHan"));
            var queryString = JsonConvert.DeserializeObject<DuocPhamDaHetHanSearchGridVoItem>(search);
            var query = BaseRepository.TableNoTracking
               .Include(x => x.NhapKhoDuocPhamChiTiets).ThenInclude(x => x.DuocPhamBenhViens)
               .ThenInclude(x => x.DinhMucDuocPhamTonKhos).ThenInclude(x => x.KhoDuocPham)
               .SelectMany(x => x.NhapKhoDuocPhamChiTiets).Include(x => x.KhoDuocPhamViTri)
               .Include(x => x.DuocPhamBenhViens).ThenInclude(x => x.DuocPham).ThenInclude(x => x.DonViTinh)
               .Select(s => new DuocPhamDaHetHanGridVo
               {
                   Id = s.Id,
                   DuocPham = s.DuocPhamBenhViens.DuocPham.Ten,
                   HoatChat = s.DuocPhamBenhViens.DuocPham.HoatChat,
                   DonViTinh = s.DuocPhamBenhViens.DuocPham.DonViTinh.Ten,
                   Kho = s.NhapKhoDuocPhams.KhoDuocPhams.Ten,
                   NgayHetHanDisplay = s.HanSuDung.ApplyFormatDate(),
                   NgayHetHan = s.HanSuDung,
                   ViTri = s.KhoDuocPhamViTri.Ten,
                   ViTriId = s.KhoDuocPhamViTri.Id,
                   SoLuongTon = s.SoLuongNhap - s.SoLuongDaXuat,
                   SoLo = s.Solo,
                   NhapKhoDuocPhamId = s.NhapKhoDuocPhamId,
                   DuocPhamId = s.DuocPhamBenhVienId,
                   MaDuocPham = s.DuocPhamBenhViens.Ma,
                   DonGiaNhap = s.DonGiaNhap,
                   HamLuong = s.DuocPhamBenhViens.DuocPham.HamLuong
               }).Where(p => p.NgayHetHan < DateTime.Now.Date)
               .GroupBy(item => new
               {
                   item.DuocPhamId,
                   item.KhoId,
                   item.DonGiaNhap,
                   item.SoLo,
                   item.NgayHetHan,
               }).Select(s => new DuocPhamDaHetHanGridVo
               {
                   Id = s.First().Id,
                   DuocPham = s.First().DuocPham,
                   HoatChat = s.First().HoatChat,
                   DonViTinh = s.First().DonViTinh,
                   Kho = s.First().Kho,
                   NgayHetHanDisplay = s.First().NgayHetHanDisplay,
                   NgayHetHan = s.First().NgayHetHan,
                   ViTri = s.First().ViTri,
                   ViTriId = s.First().ViTriId,
                   SoLuongTon = s.Sum(d => d.SoLuongTon),
                   SoLo = s.First().SoLo,
                   NhapKhoDuocPhamId = s.First().NhapKhoDuocPhamId,
                   DuocPhamId = s.First().DuocPhamId,
                   MaDuocPham = s.First().MaDuocPham,
                   DonGiaNhap = s.First().DonGiaNhap,
                   HamLuong = s.First().HamLuong
               });


            if (queryString.KhoId != 0 && queryString.KhoId != null)
            {
                query = query.Where(x => x.KhoId.Equals(queryString.KhoId));
            }

            if (!string.IsNullOrEmpty(queryString.DuocPham))
            {
                query = query.ApplyLike(queryString.DuocPham, g => g.DuocPham);
            }

            duocPhamQuery = query.ToList();
            for (int i = 0; i < duocPhamQuery.Count; i++)
            {
                if (i == 0)
                {
                    double soLuongTon;
                    if (string.IsNullOrEmpty(duocPhamQuery.ToList()[i].ViTri))
                    {
                        soLuongTon = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        duocPhamQuery.RemoveAll(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) && x.ViTri == null
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan));
                    }
                    else
                    {
                        var aa = duocPhamQuery.Where(x =>
                            x.Kho.Contains(duocPhamQuery.ToList()[i].Kho) &&
                            x.HoatChat.Contains(duocPhamQuery.ToList()[i].HoatChat) &&
                            x.ViTriId.Equals(duocPhamQuery.ToList()[i].ViTriId)
                            && x.DuocPham.Contains(duocPhamQuery.ToList()[i].DuocPham) &&
                            x.SoLuongTon.Equals(duocPhamQuery.ToList()[i].SoLuongTon) &&
                            x.NgayHetHan.Equals(duocPhamQuery.ToList()[i].NgayHetHan)).ToList();
                        soLuongTon = aa.Sum(x => x.SoLuongTon);
                        duocPhamChi = duocPhamQuery.ToList()[i];
                        duocPhamChi.SoLuongTon = soLuongTon;
                        duocPham.Add(duocPhamChi);
                        foreach (var item in aa)
                        {
                            duocPhamQuery.Remove(item);
                        }
                    }

                    i = -1;
                }
            }
            string finalResult = string.Empty;
            int stt = 1;
            for (int i = 0; i < duocPham.ToList().Count; i++)
            {
                finalResult =
                    finalResult + "<tr style='border: 1px solid #020000;text-align: center; '><td style=''border: 1px solid #020000;text-align: center;'>"
                                + stt
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].Kho
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].MaDuocPham + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].DuocPham + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].HamLuong + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].HoatChat + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].DonViTinh + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].SoLo + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].ViTri + "</td>"
                                  + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].DonGiaNhap + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].SoLuongTon + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].ThanhTien + "</td>"
                                + "<td style = 'border: 1px solid #020000;text-align: center;'>" +
                                duocPham.ToList()[i].NgayHetHanDisplay + "</td>"
                                + "</tr>";
                stt++;
            }

            var nowString = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

            var data = new DataVaLueHtml
            {
                TemplateDuocPham = finalResult,
                Now = nowString
            };

            if (result == null) return null;
            var content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            return content;
        }
    }
}
