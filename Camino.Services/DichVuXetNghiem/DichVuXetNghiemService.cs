using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuXetNghiem;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DichVuXetNghiem
{
    [ScopedDependency(ServiceType = typeof(IDichVuXetNghiemService))]

    public class DichVuXetNghiemService : MasterFileService<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>, IDichVuXetNghiemService
    {
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;
        private readonly IRepository<MauMayXetNghiem> _mauMayXetNghiemRepository;
        private readonly IRepository<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;

        public DichVuXetNghiemService(
            IRepository<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> repository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository,
            IRepository<MauMayXetNghiem> mauMayXetNghiemRepository,
            IRepository<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository
           ) : base(repository)
        {
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _mauMayXetNghiemRepository = mauMayXetNghiemRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
        }

        public List<DichVuXetNghiemGridVo> GetDataTreeView(QueryInfo queryInfo)
        {
            var list = new List<DichVuXetNghiemGridVo>();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<DichVuXetNghiemGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    queryString.SearchString = queryString.SearchString.Trim();
                }
                var getTenNhom = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Id == queryString.NhomDichVuBenhVienId)
                                    .Select(p => p.Ten).FirstOrDefault();
                if (queryString.Loai == EnumLoaiChiSoXetNghiem.NhomXetNghiem)
                {
                    list = _dichVuKyThuatBenhVienRepository.TableNoTracking
                     .Where(p => p.NhomDichVuBenhVienId == queryString.NhomDichVuBenhVienId
                             && (
                                    queryString.SearchString == null
                                    || p.NhomDichVuBenhVien.Ten.Contains(queryString.SearchString.RemoveVietnameseDiacritics().ToLower().Trim())
                                    || p.Ten.ToLower().Contains(queryString.SearchString.ToLower().Trim())
                                    || p.DichVuXetNghiem.Ten.Contains(queryString.SearchString.ToLower().Trim())
                                    || p.DichVuXetNghiem.DichVuXetNghiems.Any(i => i.Ten.ToLower().Contains(queryString.SearchString.ToLower().Trim())
                                                                                || i.DichVuXetNghiems.Any(x => x.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))
                                                                             )
                                    ))
                     .OrderBy(p => p.DichVuXetNghiem.SoThuTu ?? 0).ThenBy(p => p.Id)
                     .Select(s => new DichVuXetNghiemGridVo
                     {
                         Id = s.Id,
                         IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                         Ma = s.Ma,
                         Ten = s.Ten,
                         TenCha = s.NhomDichVuBenhVien.Ten,
                         NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                         DichVuXetNghiemId = s.DichVuXetNghiemId,
                         CapDichVu = 1,
                         CoChiSoXetNghiem = s.DichVuXetNghiemId != null ? true : false,
                         Loai = EnumLoaiChiSoXetNghiem.DVKTBenhVien,
                         LoaiMauXetNghiem = s.LoaiMauXetNghiem,
                         TenDichVuKyThuat = s.Ten,
                         ChiSoCha = s.Ten,
                         DichVuKyThuatBenhVienId = s.Id,
                         SoThuTu = s.DichVuXetNghiem.SoThuTu,
                         DonVi = s.DichVuXetNghiem.DonVi,
                         MaKetNoi = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MaKetNoi).FirstOrDefault(),
                         TenMauMay = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MauMayXetNghiem.Ten).FirstOrDefault(),
                         NamMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMax : "",
                         NamMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMin : "",
                         NuMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMin : "",
                         NuMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMax : "",
                         TreEmMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMin : "",
                         TreEmMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMax : "",
                         NguyHiemMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMin : "",
                         NguyHiemMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMax : "",
                         TreEm6Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Max : "",
                         TreEm6Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Min : "",
                         TreEm612Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Min : "",
                         TreEm612Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Max : "",
                         TreEm1218Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Min : "",
                         TreEm1218Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Max : "",
                     }).ToList();
                    var listChild = BaseRepository.TableNoTracking
                            .Where(p => p.HieuLuc == true).ToList();
                    var listXetNghiem = list.Where(p => p.DichVuXetNghiemId != null).ToList();
                    foreach (var item in listXetNghiem)
                    {
                        if (listChild.Any(p => p.DichVuXetNghiemChaId == item.DichVuXetNghiemId))
                        {
                            item.HasChildren = true;
                        }
                    }
                }
                else if (queryString.Loai == EnumLoaiChiSoXetNghiem.DVKTBenhVien)
                {
                    list = BaseRepository.TableNoTracking
                       .Where(p => p.HieuLuc == true && p.CapDichVu == 2
                                && p.DichVuXetNghiemChaId == queryString.DichVuXetNghiemChaId
                                && (
                                    queryString.SearchString == null
                                    || p.NhomDichVuBenhVien.Ten.Contains(queryString.SearchString.RemoveVietnameseDiacritics().ToLower().Trim())
                                    || p.Ten.ToLower().Contains(queryString.SearchString.ToLower().Trim())
                                    || p.DichVuXetNghiems.Any(i => i.Ten.ToLower().Contains(queryString.SearchString.ToLower().Trim()))
                                    || p.DichVuXetNghiemCha.Ten.ToLower().Trim().Contains(queryString.SearchString.ToLower().Trim())
                                    ))
                       .OrderBy(p => p.SoThuTu).ThenBy(p => p.Id)
                       .Select(s => new DichVuXetNghiemGridVo
                       {
                           Id = s.Id,
                           IdCap = (s.NhomDichVuBenhVienId + ";dvxn;" + (long)EnumLoaiChiSoXetNghiem.DVXetNghiem).ToString() + ";" + s.Id.ToString(),
                           CapDichVu = 2,
                           Ma = s.Ma,
                           Ten = s.Ten,
                           Loai = EnumLoaiChiSoXetNghiem.DVXetNghiem,
                           DichVuXetNghiemChaId = s.DichVuXetNghiemChaId,
                           LoaiMauXetNghiem = s.DichVuXetNghiemCha.DichVuKyThuatBenhViens.First().LoaiMauXetNghiem,
                           NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                           TenDichVuKyThuat = s.DichVuXetNghiemCha.Ten,
                           ChiSoCha = s.DichVuXetNghiemCha.Ten,
                           TenCha = s.NhomDichVuBenhVien.Ten,
                           SoThuTu = s.SoThuTu,
                           DonVi = s.DonVi,
                           MaKetNoi = s.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MaKetNoi).FirstOrDefault(),
                           TenMauMay = s.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MauMayXetNghiem.Ten).FirstOrDefault(),
                           NamMax = s.NamMax,
                           NamMin = s.NamMin,
                           NuMin = s.NuMin,
                           NuMax = s.NuMax,
                           TreEmMin = s.TreEmMin,
                           TreEmMax = s.TreEmMax,
                           NguyHiemMin = s.NguyHiemMin,
                           NguyHiemMax = s.NguyHiemMax,
                           TreEm6Max = s.TreEm6Max,
                           TreEm6Min = s.TreEm6Min,
                           TreEm612Min = s.TreEm612Min,
                           TreEm612Max = s.TreEm612Max,
                           TreEm1218Min = s.TreEm1218Min,
                           TreEm1218Max = s.TreEm1218Max,
                       }).ToList();
                    var listChild = BaseRepository.TableNoTracking
                             .Where(p => p.HieuLuc == true && p.CapDichVu == 3).ToList();
                    foreach (var item in list)
                    {
                        if (listChild.Any(p => p.DichVuXetNghiemChaId == item.Id))
                        {
                            item.HasChildren = true;
                        }
                        //item.TenCha = getTenNhom;
                    }

                }
                else
                {
                    var dichVuXetNghiemCha = BaseRepository.TableNoTracking
                          .Where(p => p.Id == queryString.DichVuXetNghiemId
                                   && p.HieuLuc
                                   && p.DichVuXetNghiemChaId == null
                                   && p.NhomDichVuBenhVienId == queryString.NhomDichVuBenhVienId).FirstOrDefault();

                    list = BaseRepository.TableNoTracking
                      .Where(p => p.HieuLuc == true && p.CapDichVu == 3 && p.DichVuXetNghiemChaId == queryString.DichVuXetNghiemChaId)
                      .OrderBy(p => p.SoThuTu).ThenBy(p => p.Id)
                      .Select(s => new DichVuXetNghiemGridVo
                      {
                          Id = s.Id,
                          IdCap = (s.NhomDichVuBenhVienId + ";dvxn;" + (long)EnumLoaiChiSoXetNghiem.DVXetNghiem).ToString() + ";" + s.Id.ToString(),
                          CapDichVu = 3,
                          Ma = s.Ma,
                          Ten = s.Ten,
                          Loai = EnumLoaiChiSoXetNghiem.DVXetNghiem,
                          DichVuXetNghiemChaId = s.DichVuXetNghiemChaId,
                          NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                          TenCha = s.NhomDichVuBenhVien.Ten,
                          TenDichVuKyThuat = dichVuXetNghiemCha != null ? dichVuXetNghiemCha.Ten : "",
                          DichVuXetNghiemId = dichVuXetNghiemCha != null ? dichVuXetNghiemCha.Id : 0,
                          ChiSoCha = s.DichVuXetNghiemCha.Ten,
                          SoThuTu = s.SoThuTu,
                          DonVi = s.DonVi,
                          MaKetNoi = s.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MaKetNoi).FirstOrDefault(),
                          TenMauMay = s.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MauMayXetNghiem.Ten).FirstOrDefault(),
                          NamMax = s.NamMax,
                          NamMin = s.NamMin,
                          NuMin = s.NuMin,
                          NuMax = s.NuMax,
                          TreEmMin = s.TreEmMin,
                          TreEmMax = s.TreEmMax,
                          NguyHiemMin = s.NguyHiemMin,
                          NguyHiemMax = s.NguyHiemMax,
                          TreEm6Max = s.TreEm6Max,
                          TreEm6Min = s.TreEm6Min,
                          TreEm612Min = s.TreEm612Min,
                          TreEm612Max = s.TreEm612Max,
                          TreEm1218Min = s.TreEm1218Min,
                          TreEm1218Max = s.TreEm1218Max,
                      }).ToList();

                    //foreach (var item in list)
                    //{
                    //    item.TenCha = getTenNhom;
                    //}
                }
            }
            else
            {
                var nhomQueryCon = _nhomDichVuBenhVienRepository.TableNoTracking
                      .Where(p => p.NhomDichVuBenhVienChaId == 2 && (queryInfo.SearchTerms == null || p.Ten.ToLower().Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().Trim().ToLower()) ||
                                                                    (p.DichVuKyThuatBenhViens.Any(o => o.Ten.ToLower().Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().Trim().ToLower()) || o.DichVuXetNghiem.DichVuXetNghiems.Any(i => i.Ten.ToLower().Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().Trim().ToLower()))))))
                      .OrderBy(p => p.Id)
                      .Select(s => new DichVuXetNghiemGridVo
                      {
                          Id = s.Id,
                          IdCap = (s.Id + ";nhom;" + (long)EnumLoaiChiSoXetNghiem.NhomXetNghiem).ToString(),
                          Ma = s.Ma,
                          Ten = s.Ten,
                          TenCha = s.Ten,
                          CapDichVu = 10,
                          Loai = EnumLoaiChiSoXetNghiem.NhomXetNghiem,
                          HasChildren = s.DichVuKyThuatBenhViens.Any()
                      }).ToList();

                var nhomQueryCha = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(p => p.NhomDichVuBenhVien.Ma == "XN"
                            && (queryInfo.SearchTerms == null
                                || p.NhomDichVuBenhVien.Ten.Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim())
                                    || p.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim())
                                    || p.DichVuXetNghiem.Ten.Contains(queryInfo.SearchTerms.ToLower().Trim())
                                    || p.DichVuXetNghiem.DichVuXetNghiems.Any(i => i.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim())
                                                                                || i.DichVuXetNghiems.Any(x => x.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))
                                                                             )
                                ))
                    .OrderBy(p => p.Id)
                    .Select(s => new DichVuXetNghiemGridVo
                    {
                        Id = s.Id,
                        IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                        Ma = s.Ma,
                        Ten = s.Ten,
                        TenCha = s.NhomDichVuBenhVien.Ten,
                        NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                        DichVuXetNghiemId = s.DichVuXetNghiemId,
                        CapDichVu = 1,
                        CoChiSoXetNghiem = s.DichVuXetNghiemId != null ? true : false,
                        Loai = EnumLoaiChiSoXetNghiem.DVKTBenhVien,
                        LoaiMauXetNghiem = s.LoaiMauXetNghiem,
                        TenDichVuKyThuat = s.Ten,
                        ChiSoCha = s.Ten,
                        DichVuKyThuatBenhVienId = s.Id,
                        SoThuTu = s.DichVuXetNghiem.SoThuTu,
                        DonVi = s.DichVuXetNghiem.DonVi,
                        MaKetNoi = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MaKetNoi).FirstOrDefault(),
                        TenMauMay = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MauMayXetNghiem.Ten).FirstOrDefault(),
                        NamMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMax : "",
                        NamMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMin : "",
                        NuMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMin : "",
                        NuMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMax : "",
                        TreEmMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMin : "",
                        TreEmMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMax : "",
                        NguyHiemMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMin : "",
                        NguyHiemMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMax : "",
                        TreEm6Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Max : "",
                        TreEm6Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Min : "",
                        TreEm612Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Min : "",
                        TreEm612Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Max : "",
                        TreEm1218Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Min : "",
                        TreEm1218Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Max : "",
                    }).ToList();
                var listChild = BaseRepository.TableNoTracking
                                .Where(p => p.HieuLuc == true).ToList();
                foreach (var item in nhomQueryCha.Where(p => p.DichVuXetNghiemId != null))
                {
                    if (listChild.Any(p => p.DichVuXetNghiemChaId == item.DichVuXetNghiemId))
                    {
                        item.HasChildren = true;
                    }
                }
                var nhomQuery = nhomQueryCon.Union(nhomQueryCha);
                if (nhomQuery.Any())
                {
                    list.AddRange(nhomQuery.ToList());
                }
            }
            return list;
        }

        public List<DichVuXetNghiemGridVo> SearchDichVuXetNghiem(QueryInfo queryInfo)
        {
            var list = new List<DichVuXetNghiemGridVo>();

            var nhomQueryCon = _nhomDichVuBenhVienRepository.TableNoTracking
                .Where(p => p.NhomDichVuBenhVienChaId == 2
                        && (queryInfo.SearchTerms == null
                            || p.Ten.ToLower().Contains(queryInfo.SearchTerms.Trim().ToLower())
                            || p.DichVuKyThuatBenhViens.Any(o => o.Ten.ToLower().Contains(queryInfo.SearchTerms.Trim().ToLower())
                                                            || o.DichVuXetNghiem.Ten.Contains(queryInfo.SearchTerms.Trim().ToLower())
                                                            || o.DichVuXetNghiem.DichVuXetNghiems.Any(i => i.Ten.ToLower().Trim().Contains(queryInfo.SearchTerms.ToLower().Trim())
                                                                                                        || i.DichVuXetNghiems.Any(x => x.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))
                                )
                            )
                            ))
                .OrderBy(p => p.Id)
                .Select(s => new DichVuXetNghiemGridVo
                {
                    Id = s.Id,
                    IdCap = (s.Id + ";nhom;" + (long)EnumLoaiChiSoXetNghiem.NhomXetNghiem).ToString(),
                    Ma = s.Ma,
                    Ten = s.Ten,
                    CapDichVu = 10,
                    Loai = EnumLoaiChiSoXetNghiem.NhomXetNghiem,
                    HasChildren = s.DichVuKyThuatBenhViens.Any(),
                })
                ;
            var nhomQueryCha = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(p => p.NhomDichVuBenhVien.Ma == "XN"
                            && (queryInfo.SearchTerms == null
                                || p.NhomDichVuBenhVien.Ten.Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim())
                                    || p.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim())
                                    || p.DichVuXetNghiem.Ten.Contains(queryInfo.SearchTerms.ToLower().Trim())
                                    || p.DichVuXetNghiem.DichVuXetNghiems.Any(i => i.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim())
                                                                                || i.DichVuXetNghiems.Any(x => x.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))
                                                                             )
                                ))
                    .OrderBy(p => p.Id)
                    .Select(s => new DichVuXetNghiemGridVo
                    {
                        Id = s.Id,
                        IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                        Ma = s.Ma,
                        Ten = s.Ten,
                        TenCha = s.NhomDichVuBenhVien.Ten,
                        NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                        DichVuXetNghiemId = s.DichVuXetNghiemId,
                        CapDichVu = 1,
                        CoChiSoXetNghiem = s.DichVuXetNghiemId != null ? true : false,
                        Loai = EnumLoaiChiSoXetNghiem.DVKTBenhVien,
                        LoaiMauXetNghiem = s.LoaiMauXetNghiem,
                        TenDichVuKyThuat = s.Ten,
                        ChiSoCha = s.Ten,
                        DichVuKyThuatBenhVienId = s.Id,
                        SoThuTu = s.DichVuXetNghiem.SoThuTu,
                        DonVi = s.DichVuXetNghiem.DonVi,
                        MaKetNoi = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MaKetNoi).FirstOrDefault(),
                        TenMauMay = s.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Where(p => p.HieuLuc == true).Select(p => p.MauMayXetNghiem.Ten).FirstOrDefault(),
                        NamMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMax : "",
                        NamMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NamMin : "",
                        NuMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMin : "",
                        NuMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NuMax : "",
                        TreEmMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMin : "",
                        TreEmMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEmMax : "",
                        NguyHiemMin = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMin : "",
                        NguyHiemMax = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.NguyHiemMax : "",
                        TreEm6Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Max : "",
                        TreEm6Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm6Min : "",
                        TreEm612Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Min : "",
                        TreEm612Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm612Max : "",
                        TreEm1218Min = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Min : "",
                        TreEm1218Max = s.DichVuXetNghiem != null ? s.DichVuXetNghiem.TreEm1218Max : "",
                    });
            var listChild = BaseRepository.TableNoTracking
                            .Where(p => p.HieuLuc == true).ToList();
            var listXetNghiem = nhomQueryCha.Where(p => p.DichVuXetNghiemId != null).ToList();
            foreach (var item in listXetNghiem)
            {
                if (listChild.Any(p => p.DichVuXetNghiemChaId == item.DichVuXetNghiemId))
                {
                    item.HasChildren = true;
                }
            }
            var nhomQuery = nhomQueryCon.Concat(nhomQueryCha);
            if (nhomQuery.Any())
            {
                list.AddRange(nhomQuery.ToList());
            }
            return list;
        }

        public List<LookupItemVo> GetLoaiMau(DropDownListRequestModel queryInfo)
        {
            var enums = Enum.GetValues(typeof(EnumLoaiMauXetNghiem)).Cast<Enum>();
            var result = enums.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(p => p.DisplayName.RemoveVietnameseDiacritics().ToLower()
                    .Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower()));
            return result.ToList();
        }

        public async Task XoaDichVuXetNghiems(long dichVuXetNghiemId)
        {
            var query = BaseRepository.Table
                    .Include(p => p.DichVuXetNghiems).ThenInclude(p => p.DichVuXetNghiemKetNoiChiSos)
                    .Include(p => p.DichVuXetNghiemKetNoiChiSos).Where(p => p.Id == dichVuXetNghiemId).ToList();
            foreach (var item in query)
            {
                item.WillDelete = true;
                item.DichVuXetNghiems.All(p => p.WillDelete = true);
                item.DichVuXetNghiems.SelectMany(p => p.DichVuXetNghiemKetNoiChiSos).All(p => p.WillDelete = true);
            }
            await BaseRepository.DeleteByIdAsync(dichVuXetNghiemId);
        }

        public Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien DichVuKyThuatBenhVienEntity(long dichVuXetNghiemId, long nhomDichVuBenhVienId)
        {
            var baseEntity = BaseRepository.TableNoTracking.Where(p => p.Id == dichVuXetNghiemId && p.NhomDichVuBenhVienId == nhomDichVuBenhVienId && p.HieuLuc == true && p.DichVuXetNghiemChaId == null).First();

            var dichVuKyThuatBenhVien = _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(p => p.DichVuXetNghiem).ThenInclude(p => p.DichVuXetNghiems)
                .Include(p => p.NhomDichVuBenhVien)
                .Where(p => p.DichVuXetNghiemId == baseEntity.Id).FirstOrDefault();
            return dichVuKyThuatBenhVien;
        }

        public List<MauMayXetNghiemGridVo> MauMayXetNghiemGridVo(long dichVuXetNghiemId)
        {
            var mauMayXetNghiem = _dichVuXetNghiemKetNoiChiSoRepository
                                  .TableNoTracking.Where(p => p.DichVuXetNghiemId == dichVuXetNghiemId && p.HieuLuc == true)
                                  .Select(s => new MauMayXetNghiemGridVo
                                  {
                                      Id = s.Id,
                                      MauMayXetNghiemId = s.MauMayXetNghiemId,
                                      TenKetNoi = s.TenKetNoi,
                                      MaChiSo = s.MaChiSo,
                                      TenMauMayXN = s.MauMayXetNghiem.Ten,
                                      NotSendOrder = s.NotSendOrder
                                  });
            return mauMayXetNghiem.ToList();
        }

        public async Task<List<MauMayXetNghiemLookup>> MauMayXetNghiemLookUp(DropDownListRequestModel queryInfo)
        {
            var nhomDVXNIds = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.NhomDichVuBenhVienChaId == 2).Select(p => p.Id).ToList();
            var lstColumnNameSearch = new List<string>
            {
                nameof(MauMayXetNghiem.Ten),
                nameof(MauMayXetNghiem.Ma)
            };
            if (string.IsNullOrEmpty(queryInfo.Query) || !queryInfo.Query.Contains(" "))
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var mauMayXetNghiemId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);
                    var mauMayXetNghiems = await _mauMayXetNghiemRepository.TableNoTracking
                        .Where(p => p.NhomDichVuBenhVienId != null && nhomDVXNIds.Contains(p.NhomDichVuBenhVienId.Value))
                        .OrderByDescending(x => mauMayXetNghiemId == 0 || x.Id == mauMayXetNghiemId).ThenBy(x => x.Id)
                        .Select(item => new MauMayXetNghiemLookup
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            Ten = item.Ten,
                            Ma = item.Ma,
                        })
                        .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten)
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return mauMayXetNghiems;
                }
                else
                {
                    var mauMayXetNghiems = await _mauMayXetNghiemRepository.TableNoTracking
                        .Where(p => p.NhomDichVuBenhVienId != null && nhomDVXNIds.Contains(p.NhomDichVuBenhVienId.Value))
                        .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                        .Select(item => new MauMayXetNghiemLookup
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            Ten = item.Ten,
                            Ma = item.Ma,
                        })
                        .ApplyLike(queryInfo.Query, o => o.Ma, o => o.Ten)
                        .Take(queryInfo.Take)
                        .ToListAsync();
                    return mauMayXetNghiems;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
                {
                    var lstId = _mauMayXetNghiemRepository
                                   .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.MauMayXetNghiems.MauMayXetNghiem), lstColumnNameSearch).Select(p => p.Id).ToList();

                    var dictionary = lstId.Select((p, i) => new
                    {
                        key = p,
                        rank = i
                    }).ToDictionary(o => o.key, o => o.rank);
                    var mauMayXetNghiemId = CommonHelper.GetIdFromRequestDropDownList(queryInfo);

                    var mauMayXetNghiems = await _mauMayXetNghiemRepository
                         .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.MauMayXetNghiems.MauMayXetNghiem), lstColumnNameSearch)
                                            .Where(p => p.NhomDichVuBenhVienId != null && nhomDVXNIds.Contains(p.NhomDichVuBenhVienId.Value))
                                            .OrderByDescending(x => mauMayXetNghiemId == 0 || x.Id == mauMayXetNghiemId).ThenBy(x => x.Id)
                                            .Take(queryInfo.Take)
                            .Select(item => new MauMayXetNghiemLookup
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id,
                                Ten = item.Ten,
                                Ma = item.Ma,
                                Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                            })
                            .Take(queryInfo.Take)
                            .ToListAsync();
                    return mauMayXetNghiems;
                }
                else
                {
                    var lstId = _mauMayXetNghiemRepository
                                   .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.MauMayXetNghiems.MauMayXetNghiem), lstColumnNameSearch).Select(p => p.Id).ToList();

                    var dictionary = lstId.Select((p, i) => new
                    {
                        key = p,
                        rank = i
                    }).ToDictionary(o => o.key, o => o.rank);
                    var mauMayXetNghiems = await _mauMayXetNghiemRepository
                         .ApplyFulltext(queryInfo.Query, nameof(Core.Domain.Entities.MauMayXetNghiems.MauMayXetNghiem), lstColumnNameSearch)
                                            .Where(p => p.NhomDichVuBenhVienId != null && nhomDVXNIds.Contains(p.NhomDichVuBenhVienId.Value))
                                            .OrderByDescending(x => queryInfo.Id == 0 || x.Id == queryInfo.Id).ThenBy(x => x.Id)
                                            .Take(queryInfo.Take)
                            .Select(item => new MauMayXetNghiemLookup
                            {
                                DisplayName = item.Ten,
                                KeyId = item.Id,
                                Ten = item.Ten,
                                Ma = item.Ma,
                                Rank = dictionary.Any(a => a.Key == item.Id) ? dictionary[item.Id] : dictionary.Count,
                            })
                            .Take(queryInfo.Take)
                            .ToListAsync();
                    return mauMayXetNghiems;
                }

            }
        }

        public async Task<List<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>> DichVuXetNghiems(long dichVuXNChaId, long dichVuXNConId, long? nhomDichVuBenhVienId)
        {
            var dichVuXetNghiems = BaseRepository.TableNoTracking
                .Include(p => p.DichVuKyThuatBenhViens)
                .Include(p => p.DichVuXetNghiemKetNoiChiSos).Where(p => p.Id == dichVuXNConId || p.Id == dichVuXNChaId).ToList();
            return dichVuXetNghiems;
        }

        public async Task<List<DichVuXetNghiemGridVo>> NhomDichVuXetNghiems()
        {
            var query = _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.NhomDichVuBenhVienChaId == 2)
                        .Select(s => new DichVuXetNghiemGridVo
                        {
                            Id = s.Id,
                            Ma = s.Ma,
                            Ten = s.Ten,
                            Loai = EnumLoaiChiSoXetNghiem.NhomXetNghiem
                        });
            return await query.OrderBy(p => p.Id).ToListAsync();
        }

        public byte[] ExportExportDataTreeViews(List<DichVuXetNghiemGridVo> nhomDichVuBenhViens)
        {

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DANH SÁCH DỊCH VỤ XÉT NGHIỆM");

                    // set row
                    worksheet.Row(9).Height = 24.5;
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 10;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 10;
                    worksheet.Column(4).Width = 10;
                    worksheet.Column(5).Width = 10;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 10;
                    worksheet.Column(9).Width = 10;
                    worksheet.Column(10).Width = 10;
                    worksheet.Column(11).Width = 10;
                    worksheet.Column(12).Width = 10;
                    worksheet.Column(13).Width = 10;
                    worksheet.Column(14).Width = 10;
                    worksheet.Column(15).Width = 10;
                    worksheet.Column(16).Width = 10;
                    worksheet.Column(17).Width = 10;
                    worksheet.Column(18).Width = 10;
                    worksheet.Column(19).Width = 10;
                    worksheet.Column(20).Width = 10;
                    worksheet.Column(21).Width = 10;
                    worksheet.Column(22).Width = 10;
                    worksheet.DefaultColWidth = 7;
                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A1"])
                    {
                        range.Worksheet.Cells["A1"].Value = "Id";
                        range.Worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["B1"])
                    {
                        range.Worksheet.Cells["B1"].Value = "Tên";
                        range.Worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["B1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["B1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["C1"])
                    {
                        range.Worksheet.Cells["C1"].Value = "Level";
                        range.Worksheet.Cells["C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["C1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["C1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["D1"])
                    {
                        range.Worksheet.Cells["D1"].Value = "STT";
                        range.Worksheet.Cells["D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["D1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["D1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["E1"])
                    {
                        range.Worksheet.Cells["E1"].Value = "Mã LIS";
                        range.Worksheet.Cells["E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["E1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["E1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["F1"])
                    {
                        range.Worksheet.Cells["F1"].Value = "Đơn vị";
                        range.Worksheet.Cells["F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["F1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["F1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["G1"])
                    {
                        range.Worksheet.Cells["G1"].Value = "Loại mẫu";
                        range.Worksheet.Cells["G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["G1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["G1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["H1"])
                    {
                        range.Worksheet.Cells["H1"].Value = "Tên mẫu máy xét nghiệm";
                        range.Worksheet.Cells["H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["H1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["H1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["I1"])
                    {
                        range.Worksheet.Cells["I1"].Value = "Nam cao";
                        range.Worksheet.Cells["I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["I1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["I1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["I1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["J1"])
                    {
                        range.Worksheet.Cells["J1"].Value = "Nam thấp";
                        range.Worksheet.Cells["J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["J1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["J1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["K1"])
                    {
                        range.Worksheet.Cells["K1"].Value = "Nữ cao";
                        range.Worksheet.Cells["K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["K1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["K1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["K1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["L1"])
                    {
                        range.Worksheet.Cells["L1"].Value = "Nữ thấp";
                        range.Worksheet.Cells["L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["L1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["L1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["M1"])
                    {
                        range.Worksheet.Cells["M1"].Value = "Trẻ em cao";
                        range.Worksheet.Cells["M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["M1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["M1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["M1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["N1"])
                    {
                        range.Worksheet.Cells["N1"].Value = "Trẻ em thấp";
                        range.Worksheet.Cells["N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["N1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }
                    using (var range = worksheet.Cells["O1"])
                    {
                        range.Worksheet.Cells["O1"].Value = "Trẻ em(12 - 18) cao";
                        range.Worksheet.Cells["O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["O1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["O1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["O1"].Style.Font.Color.SetColor(Color.Black);
                    }

                    using (var range = worksheet.Cells["P1"])
                    {
                        range.Worksheet.Cells["P1"].Value = "Trẻ em(12 - 18) thấp";
                        range.Worksheet.Cells["P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["P1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["P1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }
                    using (var range = worksheet.Cells["Q1"])
                    {
                        range.Worksheet.Cells["Q1"].Value = "Trẻ em(6 - 12) cao";
                        range.Worksheet.Cells["Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["Q1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["Q1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["Q1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["R1"])
                    {
                        range.Worksheet.Cells["R1"].Value = "Trẻ em(6 - 12) thấp";
                        range.Worksheet.Cells["R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["R1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["R1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }
                    using (var range = worksheet.Cells["S1"])
                    {
                        range.Worksheet.Cells["S1"].Value = "Trẻ em(< 6) cao";
                        range.Worksheet.Cells["S1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["S1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["S1"].Style.Font.SetFromFont(new Font("Arial", 12));
                        range.Worksheet.Cells["S1"].Style.Font.Color.SetColor(Color.Black);
                    }
                    using (var range = worksheet.Cells["T1"])
                    {
                        range.Worksheet.Cells["T1"].Value = "Trẻ em (< 6) thấp";
                        range.Worksheet.Cells["T1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["T1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["T1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }
                    using (var range = worksheet.Cells["U1"])
                    {
                        range.Worksheet.Cells["U1"].Value = "Giá trị nguy hiểm cao";
                        range.Worksheet.Cells["U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["U1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }
                    using (var range = worksheet.Cells["V1"])
                    {
                        range.Worksheet.Cells["V1"].Value = "Giá trị nguy hiểm thấp";
                        range.Worksheet.Cells["V1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["V1"].Style.Font.SetFromFont(new Font("Arial", 12));
                    }

                    int indexData = 2; // bắt đầu đổ data từ dòng 2
                    foreach (var nhomDichVuBenhVien in nhomDichVuBenhViens)
                    {
                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        worksheet.Cells["A" + indexData].Value = nhomDichVuBenhVien.Id;
                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.SetFromFont(new Font("Arial", 14));
                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.Bold = true;
                        worksheet.Cells["B" + indexData].Value = nhomDichVuBenhVien.Ten;
                        worksheet.Cells["C" + indexData].Value = "Level 1";
                        worksheet.Cells["D" + indexData + ":V" + indexData].Value = "";

                        indexData++;
                        var level2 = GetDataTreeView(new QueryInfo
                        {
                            AdditionalSearchString = "{\"Id\":" + nhomDichVuBenhVien.Id + ",\"IdCap\":null,\"HasChildren\":false,\"DichVuKyThuatBenhVienId\":1848,\"Ma\":null,\"Ten\":null,\"TenCha\":null,\"DichVuXetNghiemChaId\":" + nhomDichVuBenhVien.Id + ",\"DichVuXetNghiemId\":null,\"NhomDichVuBenhVienId\":" + nhomDichVuBenhVien.Id + ",\"CapDichVu\":" + 10 + ",\"MoTa\":null,\"SearchString\":null,\"SearchTermString\":null,\"Loai\":" + 1 + ",\"CoChiSoXetNghiem\":null}",
                            SearchString = "PEFkdmFuY2VkUXVlcnlQYXJhbWV0ZXJzPjxTZWFyY2hUZXJtcz48L1NlYXJjaFRlcm1zPjwvQWR2YW5jZWRRdWVyeVBhcmFtZXRlcnM+",
                            Take = 9999,
                            Skip = 0
                        });
                        if (level2.Any())
                        {
                            foreach (var item in level2)
                            {
                                worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.SetFromFont(new Font("Arial", 10));
                                worksheet.Cells["A" + indexData].Value = item.Id;
                                worksheet.Cells["B" + indexData].Value = "    " + item.Ten;
                                worksheet.Cells["C" + indexData].Value = item.CapDichVu == 10 ? "Level 1" : (item.CapDichVu == 1 ? "Level 2" : (item.CapDichVu == 2 ? "Level 3" : "Level 4"));
                                worksheet.Cells["D" + indexData].Value = item.SoThuTu;
                                worksheet.Cells["E" + indexData].Value = item.MaKetNoi;
                                worksheet.Cells["F" + indexData].Value = item.DonVi;
                                worksheet.Cells["G" + indexData].Value = item.TenLoaiMauXetNghiem;
                                worksheet.Cells["H" + indexData].Value = item.TenMauMay;
                                worksheet.Cells["I" + indexData].Value = item.NamMax;
                                worksheet.Cells["J" + indexData].Value = item.NamMin;
                                worksheet.Cells["K" + indexData].Value = item.NuMax;
                                worksheet.Cells["L" + indexData].Value = item.NuMin;
                                worksheet.Cells["M" + indexData].Value = item.TreEmMax;
                                worksheet.Cells["N" + indexData].Value = item.TreEmMin;
                                worksheet.Cells["O" + indexData].Value = item.TreEm1218Max;
                                worksheet.Cells["P" + indexData].Value = item.TreEm1218Min;
                                worksheet.Cells["Q" + indexData].Value = item.TreEm612Max;
                                worksheet.Cells["R" + indexData].Value = item.TreEm612Min;
                                worksheet.Cells["S" + indexData].Value = item.TreEm6Max;
                                worksheet.Cells["T" + indexData].Value = item.TreEm6Min;
                                worksheet.Cells["U" + indexData].Value = item.NguyHiemMax;
                                worksheet.Cells["V" + indexData].Value = item.NguyHiemMin;
                                indexData++;
                                if (item.HasChildren)
                                {
                                    var level3 = GetDataTreeView(new QueryInfo
                                    {
                                        AdditionalSearchString = "{\"Id\":" + item.Id + ",\"IdCap\":null,\"HasChildren\":false,\"DichVuKyThuatBenhVienId\":1848,\"Ma\":null,\"Ten\":null,\"TenCha\":null,\"DichVuXetNghiemChaId\":" + (item.DichVuXetNghiemChaId == null ? item.DichVuXetNghiemId : item.DichVuXetNghiemChaId) + ",\"DichVuXetNghiemId\":null,\"NhomDichVuBenhVienId\":" + nhomDichVuBenhVien.Id + ",\"CapDichVu\":" + 2 + ",\"MoTa\":null,\"SearchString\":null,\"SearchTermString\":null,\"Loai\":" + 2 + ",\"CoChiSoXetNghiem\":null}",
                                        SearchString = "PEFkdmFuY2VkUXVlcnlQYXJhbWV0ZXJzPjxTZWFyY2hUZXJtcz48L1NlYXJjaFRlcm1zPjwvQWR2YW5jZWRRdWVyeVBhcmFtZXRlcnM+",
                                        Take = 9999,
                                        Skip = 0
                                    });
                                    if (level3.Any())
                                    {
                                        foreach (var dataLv3 in level3)
                                        {
                                            worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.SetFromFont(new Font("Arial", 10));
                                            worksheet.Cells["A" + indexData].Value = dataLv3.Id;
                                            worksheet.Cells["B" + indexData].Value = "       " + dataLv3.Ten;
                                            worksheet.Cells["C" + indexData].Value = dataLv3.CapDichVu == 10 ? "Level 1" : (dataLv3.CapDichVu == 1 ? "Level 2" : (dataLv3.CapDichVu == 2 ? "Level 3" : "Level 4"));
                                            worksheet.Cells["D" + indexData].Value = dataLv3.SoThuTu;
                                            worksheet.Cells["E" + indexData].Value = dataLv3.MaKetNoi;
                                            worksheet.Cells["F" + indexData].Value = dataLv3.DonVi;
                                            worksheet.Cells["G" + indexData].Value = dataLv3.TenLoaiMauXetNghiem;
                                            worksheet.Cells["H" + indexData].Value = dataLv3.TenMauMay;
                                            worksheet.Cells["I" + indexData].Value = dataLv3.NamMax;
                                            worksheet.Cells["J" + indexData].Value = dataLv3.NamMin;
                                            worksheet.Cells["K" + indexData].Value = dataLv3.NuMax;
                                            worksheet.Cells["L" + indexData].Value = dataLv3.NuMin;
                                            worksheet.Cells["M" + indexData].Value = dataLv3.TreEmMax;
                                            worksheet.Cells["N" + indexData].Value = dataLv3.TreEmMin;
                                            worksheet.Cells["O" + indexData].Value = dataLv3.TreEm1218Max;
                                            worksheet.Cells["P" + indexData].Value = dataLv3.TreEm1218Min;
                                            worksheet.Cells["Q" + indexData].Value = dataLv3.TreEm612Max;
                                            worksheet.Cells["R" + indexData].Value = dataLv3.TreEm612Min;
                                            worksheet.Cells["S" + indexData].Value = dataLv3.TreEm6Max;
                                            worksheet.Cells["T" + indexData].Value = dataLv3.TreEm6Min;
                                            worksheet.Cells["U" + indexData].Value = dataLv3.NguyHiemMax;
                                            worksheet.Cells["V" + indexData].Value = dataLv3.NguyHiemMin;
                                            indexData++;

                                            if (dataLv3.HasChildren)
                                            {
                                                var level4 = GetDataTreeView(new QueryInfo
                                                {
                                                    AdditionalSearchString = "{\"Id\":" + dataLv3.Id + ",\"IdCap\":null,\"HasChildren\":false,\"DichVuKyThuatBenhVienId\":1848,\"Ma\":null,\"Ten\":null,\"TenCha\":null,\"DichVuXetNghiemChaId\":" + (dataLv3.DichVuXetNghiemChaId == null ? dataLv3.DichVuXetNghiemId : dataLv3.DichVuXetNghiemChaId) + ",\"DichVuXetNghiemId\":null,\"NhomDichVuBenhVienId\":" + nhomDichVuBenhVien.Id + ",\"CapDichVu\":" + 3 + ",\"MoTa\":null,\"SearchString\":null,\"SearchTermString\":null,\"Loai\":" + 3 + ",\"CoChiSoXetNghiem\":null}",
                                                    SearchString = "PEFkdmFuY2VkUXVlcnlQYXJhbWV0ZXJzPjxTZWFyY2hUZXJtcz48L1NlYXJjaFRlcm1zPjwvQWR2YW5jZWRRdWVyeVBhcmFtZXRlcnM+",
                                                    Take = 9999,
                                                    Skip = 0
                                                });
                                                if (level4.Any())
                                                {
                                                    foreach (var dataLv4 in level4)
                                                    {
                                                        worksheet.Cells["A" + indexData + ":V" + indexData].Style.Font.SetFromFont(new Font("Arial", 10));
                                                        worksheet.Cells["A" + indexData].Value = dataLv4.Id;
                                                        worksheet.Cells["B" + indexData].Value = "          " + dataLv4.Ten;
                                                        worksheet.Cells["C" + indexData].Value = dataLv4.CapDichVu == 10 ? "Level 1" : (dataLv4.CapDichVu == 1 ? "Level 2" : (dataLv4.CapDichVu == 2 ? "Level 3" : "Level 4"));
                                                        worksheet.Cells["D" + indexData].Value = dataLv4.SoThuTu;
                                                        worksheet.Cells["E" + indexData].Value = dataLv4.MaKetNoi;
                                                        worksheet.Cells["F" + indexData].Value = dataLv4.DonVi;
                                                        worksheet.Cells["G" + indexData].Value = dataLv4.TenLoaiMauXetNghiem;
                                                        worksheet.Cells["H" + indexData].Value = dataLv4.TenMauMay;
                                                        worksheet.Cells["I" + indexData].Value = dataLv4.NamMax;
                                                        worksheet.Cells["J" + indexData].Value = dataLv4.NamMin;
                                                        worksheet.Cells["K" + indexData].Value = dataLv4.NuMax;
                                                        worksheet.Cells["L" + indexData].Value = dataLv4.NuMin;
                                                        worksheet.Cells["M" + indexData].Value = dataLv4.TreEmMax;
                                                        worksheet.Cells["N" + indexData].Value = dataLv4.TreEmMin;
                                                        worksheet.Cells["O" + indexData].Value = dataLv4.TreEm1218Max;
                                                        worksheet.Cells["P" + indexData].Value = dataLv4.TreEm1218Min;
                                                        worksheet.Cells["Q" + indexData].Value = dataLv4.TreEm612Max;
                                                        worksheet.Cells["R" + indexData].Value = dataLv4.TreEm612Min;
                                                        worksheet.Cells["S" + indexData].Value = dataLv4.TreEm6Max;
                                                        worksheet.Cells["T" + indexData].Value = dataLv4.TreEm6Min;
                                                        worksheet.Cells["U" + indexData].Value = dataLv4.NguyHiemMax;
                                                        worksheet.Cells["V" + indexData].Value = dataLv4.NguyHiemMin;
                                                        indexData++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
