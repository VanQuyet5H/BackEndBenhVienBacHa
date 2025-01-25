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
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuXetNghiem;
using Camino.Core.Domain.ValueObject.TuDienKyThuat;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.ExportImport.Help;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DichVuXetNghiem
{
    [ScopedDependency(ServiceType = typeof(ITuDienDichVuKyThuatService))]
    public class TuDienDichVuKyThuatService : MasterFileService<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat>, ITuDienDichVuKyThuatService
    {
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUserAgentHelper _userAgentHelper;

        public TuDienDichVuKyThuatService(
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> repository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository,
            IRepository<User> userRepository,
            IUserAgentHelper userAgentHelper
           ) : base(repository)
        {
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _userRepository = userRepository;
            _userAgentHelper = userAgentHelper;
        }

        public List<TuDienKyThuatGridVo> GetDataTreeView(QueryInfo queryInfo)
        {
            var currentId = _userAgentHelper.GetCurrentUserId();
            var username = _userRepository.TableNoTracking.Where(c => c.Id == currentId).Select(c => c.HoTen).FirstOrDefault();

            var list = new List<TuDienKyThuatGridVo>();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<TuDienKyThuatGridVo>(queryInfo.AdditionalSearchString);
                if (!string.IsNullOrEmpty(queryString.SearchString))
                {
                    queryString.SearchString = queryString.SearchString.Trim();
                }

                var nhomQueryCon = _nhomDichVuBenhVienRepository.TableNoTracking
                     .Where(p => (p.NhomDichVuBenhVienChaId == queryString.NhomDichVuBenhVienId)
                        && (queryString.SearchString == null || p.Ten.ToLower().Contains(queryString.SearchString.RemoveVietnameseDiacritics().Trim().ToLower()) ||
                                                                   (p.DichVuKyThuatBenhViens.Any(o => o.Ten.ToLower().Contains(queryString.SearchString.RemoveVietnameseDiacritics().Trim().ToLower())
                                                                   ))))
                     .OrderBy(p => p.Id)
                     .Select(s => new TuDienKyThuatGridVo
                     {
                         Id = s.Id,
                         IdCap = (s.Id + ";nhom;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString(),
                         Ma = s.Ma,
                         Ten = s.Ten,
                         TenCha = s.Ten,
                         CapDichVu = 1,
                         NhomDichVuBenhVienId = s.Id,
                         TenNhomDichVuBenhVien = s.Ten,
                         HasChildren = s.DichVuKyThuatBenhViens.Any(),
                     }).ToList();

                var nhomQueryCha = _dichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(p => p.NhomDichVuBenhVienId == queryString.NhomDichVuBenhVienId && (queryString.SearchString == null
                                || p.NhomDichVuBenhVien.Ten.Contains(queryString.SearchString.RemoveVietnameseDiacritics().ToLower().Trim())
                                    || p.Ten.ToLower().Contains(queryString.SearchString.ToLower().Trim())))
                                    .OrderBy(p => p.Id)
                    .Select(s => new TuDienKyThuatGridVo
                    {
                        Id = s.Id,
                        IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                        Ma = s.Ma,
                        Ten = s.Ten,
                        TenCha = s.NhomDichVuBenhVien.Ten,
                        NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                        TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                        CapDichVu = 2,

                        DichVuKyThuatId = s.DichVuKyThuat.Id,
                        TenDichVuKyThuat = s.DichVuKyThuat.TenChung,

                        ChiSoCha = s.Ten,
                        DichVuKyThuatBenhVienId = s.Id,

                        UserLoginId = currentId,
                        TenUserLogin = username,

                        TenKetQuaMau = s.DichVukyThuatBenhVienMauKetQua.TenKetQuaMau,
                        MaSo = s.DichVukyThuatBenhVienMauKetQua.MaSo,
                        KetLuan = s.DichVukyThuatBenhVienMauKetQua.KetLuan,
                        KetQua = s.DichVukyThuatBenhVienMauKetQua.KetQua,
                    }).ToList();

                var listChild = _dichVuKyThuatBenhVienRepository.TableNoTracking.ToList();
                foreach (var item in nhomQueryCha.Where(p => p.NhomDichVuBenhVienId != null))
                {
                    if (listChild.Any(p => p.NhomDichVuBenhVien != null && p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId == item.NhomDichVuBenhVienId))
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
            else
            {
                var nhomQueryCon = _nhomDichVuBenhVienRepository.TableNoTracking
                      .Where(p => (p.Ma == "CĐHA" || p.Ma == "TDCN")
                         && (queryInfo.SearchTerms == null || p.Ten.ToLower().Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().Trim().ToLower()) ||
                                                                    (p.DichVuKyThuatBenhViens.Any(o => o.Ten.ToLower().Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().Trim().ToLower())
                                                                    ))))
                      .OrderBy(p => p.Id)
                      .Select(s => new TuDienKyThuatGridVo
                      {
                          Id = s.Id,
                          IdCap = (s.Id + ";nhom;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString(),
                          Ma = s.Ma,
                          Ten = s.Ten,
                          TenCha = s.NhomDichVuBenhVienCha.Ten,
                          CapDichVu = 1,
                          NhomDichVuBenhVienId = s.Id,
                          TenNhomDichVuBenhVien = s.NhomDichVuBenhVienCha.Ten,
                          HasChildren = s.DichVuKyThuatBenhViens.Any(),
                      }).ToList();

                var nhomQueryCha = _dichVuKyThuatBenhVienRepository.TableNoTracking
                  .Where(p => (p.NhomDichVuBenhVien.Ma == "CDHA" || p.NhomDichVuBenhVien.Ma == "TDCN") &&
                        p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && (queryInfo.SearchTerms == null
                              || p.NhomDichVuBenhVien.Ten.Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim())
                                  || p.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim())))
                                  .OrderBy(p => p.Id)
                   .Select(s => new TuDienKyThuatGridVo
                   {
                       Id = s.Id,
                       IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                       Ma = s.Ma,
                       Ten = s.Ten,
                       TenCha = s.NhomDichVuBenhVien.Ten,
                       NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                       TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                       TenDichVuKyThuat = s.DichVuKyThuat.TenChung,
                       CapDichVu = 2
                   }).ToList();

                var listChild = _dichVuKyThuatBenhVienRepository.TableNoTracking.ToList();
                foreach (var item in nhomQueryCha.Where(p => p.NhomDichVuBenhVienId != null))
                {
                    if (listChild.Any(p => p.NhomDichVuBenhVien != null && p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId == item.NhomDichVuBenhVienId))
                    {
                        item.HasChildren = true;
                    }
                }

                var nhomQuery = nhomQueryCon.Union(nhomQueryCha);
                if (nhomQuery.Any())
                {
                    list.AddRange(nhomQuery.ToList());
                }

                return list;
            }

            return list;
        }

        public List<TuDienKyThuatGridVo> SearchDichVuKyThuatBenhVien(QueryInfo queryInfo)
        {
            var currentId = _userAgentHelper.GetCurrentUserId();
            var username = _userRepository.TableNoTracking.Where(c => c.Id == currentId).Select(c => c.HoTen).FirstOrDefault();

            var list = new List<TuDienKyThuatGridVo>();
            var nhomDichVuBenhIds = new List<long?>();

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                nhomDichVuBenhIds = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => (queryInfo.SearchTerms == null
                         || p.NhomDichVuBenhVien.Ten.Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim())
                         || p.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))).Select(c => c.NhomDichVuBenhVien.NhomDichVuBenhVienChaId).Where(c => c.Value == 3 || c.Value == 4).ToList();
            }

            if (!nhomDichVuBenhIds.Any())
            {
                nhomDichVuBenhIds.Add(3);
                nhomDichVuBenhIds.Add(4);
            }

            var nhomQueryCon = _nhomDichVuBenhVienRepository.TableNoTracking
                .Where(p => nhomDichVuBenhIds.Contains(p.Id))
                .OrderBy(p => p.Id)
                .Select(s => new TuDienKyThuatGridVo
                {
                    Id = s.Id,
                    IdCap = (s.Id + ";nhom;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString(),
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenCha = s.Ten,
                    CapDichVu = 1,
                    NhomDichVuBenhVienId = s.Id,
                    TenNhomDichVuBenhVien = s.NhomDichVuBenhVienCha.Ten,
                    HasChildren = s.DichVuKyThuatBenhViens.Any(),

                    UserLoginId = currentId,
                    TenUserLogin = username,
                }).ToList();

            var nhomQueryCha = _dichVuKyThuatBenhVienRepository.TableNoTracking
                 .Where(p => (p.NhomDichVuBenhVien.Id == 3 || p.NhomDichVuBenhVien.Id == 4) && p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && (queryInfo.SearchTerms == null
                             || p.NhomDichVuBenhVien.Ten.Contains(queryInfo.SearchTerms.RemoveVietnameseDiacritics().ToLower().Trim())
                             || p.Ten.ToLower().Contains(queryInfo.SearchTerms.ToLower().Trim()))).OrderBy(p => p.Id)
                  .Select(s => new TuDienKyThuatGridVo
                  {
                      Id = s.Id,
                      IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                      Ma = s.Ma,
                      Ten = s.Ten,
                      TenCha = s.NhomDichVuBenhVien.Ten,
                      NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                      TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                      CapDichVu = 2,

                      UserLoginId = currentId,
                      TenUserLogin = username,

                      TenKetQuaMau = s.DichVukyThuatBenhVienMauKetQua.TenKetQuaMau,
                      MaSo = s.DichVukyThuatBenhVienMauKetQua.MaSo,
                      KetLuan = s.DichVukyThuatBenhVienMauKetQua.KetLuan,
                      KetQua = s.DichVukyThuatBenhVienMauKetQua.KetQua,
                  }).ToList();

            var listChild = _dichVuKyThuatBenhVienRepository.TableNoTracking.ToList();
            foreach (var item in nhomQueryCha.Where(p => p.NhomDichVuBenhVienId != null))
            {
                if (listChild.Any(p => p.NhomDichVuBenhVien != null && p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId == item.NhomDichVuBenhVienId))
                {
                    item.HasChildren = true;
                }
            }

            var nhomQuery = nhomQueryCon.Union(nhomQueryCha);
            if (nhomQuery.Any())
            {
                list.AddRange(nhomQuery.ToList());
            }

            return list;

        }


        public TuDienKyThuatGridVo GetDichVuKyThuats(long dichVuKyThuatBenhVienId)
        {
            var currentId = _userAgentHelper.GetCurrentUserId();
            var username = _userRepository.TableNoTracking.Where(c => c.Id == currentId).Select(c => c.HoTen).FirstOrDefault();

            return _dichVuKyThuatBenhVienRepository.TableNoTracking
                     .Where(p => p.Id == dichVuKyThuatBenhVienId).OrderBy(p => p.Id)
                      .Select(s => new TuDienKyThuatGridVo
                      {
                          Id = s.Id,
                          DichVuKyThuatBenhVienId = s.Id,

                          IdCap = (s.NhomDichVuBenhVienId + ";dvkt;" + (long)EnumLoaiChiSoXetNghiem.DVKTBenhVien).ToString() + ";" + s.Id.ToString(),
                          Ma = s.Ma,
                          Ten = s.Ten,
                          TenCha = s.NhomDichVuBenhVien.Ten,
                          NhomDichVuBenhVienId = s.NhomDichVuBenhVienId,
                          TenNhomDichVuBenhVien = s.NhomDichVuBenhVien.Ten,
                          CapDichVu = 2,

                          UserLoginId = currentId,
                          TenUserLogin = username,

                          TenDichVuKyThuat = s.DichVuKyThuat.TenChung,
                          TenKetQuaMau = s.DichVukyThuatBenhVienMauKetQua.TenKetQuaMau,
                          MaSo = s.DichVukyThuatBenhVienMauKetQua.MaSo,
                          KetLuan = s.DichVukyThuatBenhVienMauKetQua.KetLuan,
                          KetQua = s.DichVukyThuatBenhVienMauKetQua.KetQua,
                      }).FirstOrDefault();
        }


        public void LuuDichVukyThuatBenhVienMauKetQua(TuDienKyThuatGridVo tuDienKyThuatGridVo)
        {
            var dichVuKyThuatBenhVien = _dichVuKyThuatBenhVienRepository.Table.Where(c => c.Id == tuDienKyThuatGridVo.DichVuKyThuatBenhVienId)
                                                                        .Include(c => c.DichVukyThuatBenhVienMauKetQua).FirstOrDefault();

            if (dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua != null)
            {
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.NhanVienThucHienId = tuDienKyThuatGridVo.UserLoginId;
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.TenKetQuaMau = tuDienKyThuatGridVo.TenKetQuaMau;
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.MaSo = tuDienKyThuatGridVo.MaSo;
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.KetLuan = tuDienKyThuatGridVo.KetLuan;
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua.KetQua = tuDienKyThuatGridVo.KetQua;

            }
            else
            {
                dichVuKyThuatBenhVien.DichVukyThuatBenhVienMauKetQua = new Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua.DichVukyThuatBenhVienMauKetQua
                {
                    NhanVienThucHienId = tuDienKyThuatGridVo.UserLoginId,
                    TenKetQuaMau = tuDienKyThuatGridVo.TenKetQuaMau,
                    MaSo = tuDienKyThuatGridVo.MaSo,
                    KetLuan = tuDienKyThuatGridVo.KetLuan,
                    KetQua = tuDienKyThuatGridVo.KetQua,
                };
            }
            _dichVuKyThuatBenhVienRepository.Update(dichVuKyThuatBenhVien);
        }

    }
}
