using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Voucher;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Services.Voucher
{
    [ScopedDependency(ServiceType = typeof(IVoucherService))]
    public class VoucherService : MasterFileService<Core.Domain.Entities.Vouchers.Voucher>, IVoucherService
    {
        private IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        private IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatBenhVienGiaBenhVienRepository;
        private IRepository<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhVienRepository;
        private IRepository<VoucherChiTietMienGiam> _voucherChiTietMienGiamRepository;
        private IRepository<TheVoucher> _theVoucherRepository;
        private IRepository<TheVoucherYeuCauTiepNhan> _theVoucherYeuCauTiepNhanRepository;
        private IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private IRepository<NhomGiaDichVuKhamBenhBenhVien> _nhomGiaDichVuKhamBenhBenhVienRepository;
        private IRepository<NhomGiaDichVuKyThuatBenhVien> _nhomGiaDichVuKyThuatBenhVienRepository;
        private IRepository<Template> _templateRepository;

        public VoucherService(
            IRepository<Core.Domain.Entities.Vouchers.Voucher> repository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVienGiaBenhVien> dichVuKyThuatBenhVienGiaBenhVienRepository,
            IRepository<DichVuKhamBenhBenhVienGiaBenhVien> dichVuKhamBenhBenhVienGiaBenhVienRepository,
            IRepository<VoucherChiTietMienGiam> voucherChiTietMienGiamRepository,
            IRepository<TheVoucher> theVoucherRepository,
            IRepository<TheVoucherYeuCauTiepNhan> theVoucherYeuCauTiepNhanRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<NhomGiaDichVuKhamBenhBenhVien> nhomGiaDichVuKhamBenhBenhVienRepository,
            IRepository<NhomGiaDichVuKyThuatBenhVien> nhomGiaDichVuKyThuatBenhVienRepository,
            IRepository<Template> templateRepository
        ) : base(repository)
        {
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _dichVuKyThuatBenhVienGiaBenhVienRepository = dichVuKyThuatBenhVienGiaBenhVienRepository;
            _dichVuKhamBenhBenhVienGiaBenhVienRepository = dichVuKhamBenhBenhVienGiaBenhVienRepository;
            _voucherChiTietMienGiamRepository = voucherChiTietMienGiamRepository;
            _theVoucherRepository = theVoucherRepository;
            _theVoucherYeuCauTiepNhanRepository = theVoucherYeuCauTiepNhanRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _nhomGiaDichVuKhamBenhBenhVienRepository = nhomGiaDichVuKhamBenhBenhVienRepository;
            _nhomGiaDichVuKyThuatBenhVienRepository = nhomGiaDichVuKyThuatBenhVienRepository;
            _templateRepository = templateRepository;
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new VoucherSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<VoucherSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(p => new VoucherMarketingGridVo
            {
                Id = p.Id,
                Ma = p.Ma,
                Ten = p.Ten,
                SoLuongPhatHanh = p.SoLuongPhatHanh,
                TuNgay = p.TuNgay,
                DenNgay = p.DenNgay
            });
            //.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, p => p.Ma, p => p.Ten);
                }

                if (queryObject.RangeNgayApDung != null)
                {
                    //tuNgay <= p.TuNgay < p.DenNgay <= denNgay
                    if (queryObject.RangeNgayApDung.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNgayApDung.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.TuNgay.Date);
                    }

                    if (queryObject.RangeNgayApDung.endDate != null)
                    {
                        var denNgay = queryObject.RangeNgayApDung.endDate.GetValueOrDefault();
                        query = query.Where(p => p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value <= denNgay.Date));
                    }
                }
            }

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            int stt = 1;
            foreach (var item in queryTask.Result)
            {
                item.STT = stt++;
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new VoucherSearch();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<VoucherSearch>(queryInfo.AdditionalSearchString);
            }

            var query = BaseRepository.TableNoTracking.Select(p => new VoucherMarketingGridVo
            {
                Id = p.Id,
                Ma = p.Ma,
                Ten = p.Ten,
                SoLuongPhatHanh = p.SoLuongPhatHanh,
                TuNgay = p.TuNgay,
                DenNgay = p.DenNgay
            });
            //.ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            if (queryObject != null)
            {
                if (!string.IsNullOrEmpty(queryObject.SearchString))
                {
                    var searchTerms = queryObject.SearchString.Replace("\t", "").Trim();
                    query = query.ApplyLike(searchTerms, p => p.Ma, p => p.Ten);
                }

                if (queryObject.RangeNgayApDung != null)
                {
                    //tuNgay <= p.TuNgay < p.DenNgay <= denNgay
                    if (queryObject.RangeNgayApDung.startDate != null)
                    {
                        var tuNgay = queryObject.RangeNgayApDung.startDate.GetValueOrDefault();
                        query = query.Where(p => tuNgay.Date <= p.TuNgay.Date);
                    }

                    if (queryObject.RangeNgayApDung.endDate != null)
                    {
                        var denNgay = queryObject.RangeNgayApDung.endDate.GetValueOrDefault();
                        query = query.Where(p => p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value <= denNgay.Date));
                    }
                }
            }

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public bool CompareTuNgayDenNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            if (denNgay.HasValue && denNgay.HasValue)
            {
                return tuNgay.Value.Date > denNgay.Value.Date ? false : true;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<VoucherDichVuVo>> GetListDichVuChoVoucher(DropDownListRequestModel queryInfo)
        {
            var lstDichVu = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.HieuLuc != false)
                                                                                  .Select(p => new VoucherDichVuVo
                                                                                  {
                                                                                      KeyId = p.Id,
                                                                                      DisplayName = p.Ten,
                                                                                      Ma = p.Ma,
                                                                                      LoaiDichVu = EnumDichVuTongHop.KyThuat
                                                                                  })
                                                                                  .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                                                                                  .Union(_dichVuKhamBenhBenhVienRepository.TableNoTracking.Where(p => p.HieuLuc != false)
                                                                                                                                          .Select(p => new VoucherDichVuVo
                                                                                                                                          {
                                                                                                                                              KeyId = p.Id,
                                                                                                                                              DisplayName = p.Ten,
                                                                                                                                              Ma = p.Ma,
                                                                                                                                              LoaiDichVu = EnumDichVuTongHop.KhamBenh
                                                                                                                                          })
                                                                                                                                          .ApplyLike(queryInfo.Query, o => o.DisplayName, o => o.Ma)
                                                                                  )
                                                                                  .Take(queryInfo.Take)
                                                                                  .Distinct()
                                                                                  .ToListAsync();

            return lstDichVu;
        }

        public async Task<List<VoucherLoaiGiaVo>> GetListLoaiGiaChoDichVu(long dichVuId, EnumDichVuTongHop loaiDichVu)
        {
            switch (loaiDichVu)
            {
                case EnumDichVuTongHop.KyThuat:
                    return await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.Where(p => p.DichVuKyThuatBenhVienId == dichVuId)
                                                                                            .Select(p => new VoucherLoaiGiaVo
                                                                                            {
                                                                                                KeyId = p.NhomGiaDichVuKyThuatBenhVienId,
                                                                                                DisplayName = p.NhomGiaDichVuKyThuatBenhVien.Ten
                                                                                            })
                                                                                            .ToListAsync();
                case EnumDichVuTongHop.KhamBenh:
                    return await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.Where(p => p.DichVuKhamBenhBenhVienId == dichVuId)
                                                                                             .Select(p => new VoucherLoaiGiaVo
                                                                                             {
                                                                                                 KeyId = p.NhomGiaDichVuKhamBenhBenhVienId,
                                                                                                 DisplayName = p.NhomGiaDichVuKhamBenhBenhVien.Ten
                                                                                             })
                                                                                             .ToListAsync();
                default:
                    return new List<VoucherLoaiGiaVo>();
            }
        }

        public async Task<List<VoucherLoaiGiaVo>> GetListTatCaLoaiGiaChoDichVu(EnumDichVuTongHop loaiDichVu)
        {
            switch (loaiDichVu)
            {
                case EnumDichVuTongHop.KyThuat:
                    return await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking.Select(p => new VoucherLoaiGiaVo
                                                                                        {
                                                                                            KeyId = p.Id,
                                                                                            DisplayName = p.Ten
                                                                                        })
                                                                                        .ToListAsync();
                case EnumDichVuTongHop.KhamBenh:
                    return await _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking.Select(p => new VoucherLoaiGiaVo
                                                                                         {
                                                                                             KeyId = p.Id,
                                                                                             DisplayName = p.Ten
                                                                                         })
                                                                                         .ToListAsync();
                default:
                    return new List<VoucherLoaiGiaVo>();
            }
        }

        public async Task<decimal> GetDonGiaChoDichVu(long dichVuId, long loaiGiaId, EnumDichVuTongHop loaiDichVu)
        {
            switch (loaiDichVu)
            {
                case EnumDichVuTongHop.KyThuat:
                    return await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking.Where(p => p.DichVuKyThuatBenhVienId == dichVuId && p.NhomGiaDichVuKyThuatBenhVienId == loaiGiaId && p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || p.DenNgay.Value.Date >= DateTime.Now.Date))
                                                                                            .Select(p => p.Gia)
                                                                                            .FirstOrDefaultAsync();
                case EnumDichVuTongHop.KhamBenh:
                    return await _dichVuKhamBenhBenhVienGiaBenhVienRepository.TableNoTracking.Where(p => p.DichVuKhamBenhBenhVienId == dichVuId && p.NhomGiaDichVuKhamBenhBenhVienId == loaiGiaId && p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || p.DenNgay.Value.Date >= DateTime.Now.Date))
                                                                                             .Select(p => p.Gia)
                                                                                             .FirstOrDefaultAsync();
                default:
                    return 0;
            }
        }

        public async Task<GridDataSource> GetListDichVuForGridAsync(long voucherId)
        {
            var query = _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId &&
                                                                                     (p.DichVuKyThuatBenhVienId != null || p.DichVuKhamBenhBenhVienId != null))
                                                                         .Select(p => new DichVuVoucherMarketingGridVo
                                                                         {
                                                                             Id = p.Id,
                                                                             VoucherId = p.VoucherId,
                                                                             LoaiDichVuBenhVien = p.DichVuKyThuatBenhVienId != null ? EnumDichVuTongHop.KyThuat : EnumDichVuTongHop.KhamBenh,
                                                                             DichVuId = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVienId.GetValueOrDefault() : p.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                                                             MaDichVu = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVien.Ma : p.DichVuKhamBenhBenhVien.Ma,
                                                                             DichVuDisplay = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVien.Ten : p.DichVuKhamBenhBenhVien.Ten,
                                                                             LoaiGiaId = p.NhomGiaDichVuKyThuatBenhVienId != null ? p.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault() : p.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                                                             LoaiGiaDisplay = p.NhomGiaDichVuKyThuatBenhVienId != null ? p.NhomGiaDichVuKyThuatBenhVien.Ten : p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                                                             DonGia = p.DichVuKyThuatBenhVienId != null ?
                                                                                p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p2 => p2.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault() && p2.TuNgay.Date <= DateTime.Now.Date && (p2.DenNgay == null || p2.DenNgay.Value.Date >= DateTime.Now.Date)).Select(p2 => p2.Gia).FirstOrDefault() :
                                                                                p.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.Where(p2 => p2.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault() && p2.TuNgay.Date <= DateTime.Now.Date && (p2.DenNgay == null || p2.DenNgay.Value.Date >= DateTime.Now.Date)).Select(p2 => p2.Gia).FirstOrDefault(),
                                                                             LoaiChietKhau = p.LoaiChietKhau,
                                                                             SoTienChietKhau = p.SoTienChietKhau,
                                                                             TiLeChietKhau = p.TiLeChietKhau,
                                                                             GhiChu = p.GhiChu
                                                                         });

            var queryTask = await query.ToArrayAsync();
            var countTask = await query.CountAsync();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetPagesListDichVuForGridAsync(long voucherId)
        {
            var query = _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId &&
                                                                                     (p.DichVuKyThuatBenhVienId != null || p.DichVuKhamBenhBenhVienId != null))
                                                                         .Select(p => new DichVuVoucherMarketingGridVo
                                                                         {
                                                                             Id = p.Id,
                                                                             VoucherId = p.VoucherId,
                                                                             LoaiDichVuBenhVien = p.DichVuKyThuatBenhVienId != null ? EnumDichVuTongHop.KyThuat : EnumDichVuTongHop.KhamBenh,
                                                                             DichVuId = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVienId.GetValueOrDefault() : p.DichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                                                             MaDichVu = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVien.Ma : p.DichVuKhamBenhBenhVien.Ma,
                                                                             DichVuDisplay = p.DichVuKyThuatBenhVienId != null ? p.DichVuKyThuatBenhVien.Ten : p.DichVuKhamBenhBenhVien.Ten,
                                                                             LoaiGiaId = p.NhomGiaDichVuKyThuatBenhVienId != null ? p.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault() : p.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault(),
                                                                             LoaiGiaDisplay = p.NhomGiaDichVuKyThuatBenhVienId != null ? p.NhomGiaDichVuKyThuatBenhVien.Ten : p.NhomGiaDichVuKhamBenhBenhVien.Ten,
                                                                             DonGia = p.DichVuKyThuatBenhVienId != null ?
                                                                                p.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p2 => p2.NhomGiaDichVuKyThuatBenhVienId == p.NhomGiaDichVuKyThuatBenhVienId.GetValueOrDefault() && p2.TuNgay.Date <= DateTime.Now.Date && (p2.DenNgay == null || p2.DenNgay.Value.Date >= DateTime.Now.Date)).Select(p2 => p2.Gia).FirstOrDefault() :
                                                                                p.DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.Where(p2 => p2.NhomGiaDichVuKhamBenhBenhVienId == p.NhomGiaDichVuKhamBenhBenhVienId.GetValueOrDefault() && p2.TuNgay.Date <= DateTime.Now.Date && (p2.DenNgay == null || p2.DenNgay.Value.Date >= DateTime.Now.Date)).Select(p2 => p2.Gia).FirstOrDefault(),
                                                                             LoaiChietKhau = p.LoaiChietKhau,
                                                                             SoTienChietKhau = p.SoTienChietKhau,
                                                                             TiLeChietKhau = p.TiLeChietKhau,
                                                                             GhiChu = p.GhiChu
                                                                         });

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }

        public async Task<List<NhomDichVuBenhVienTreeViewVo>> GetListNhomDichVuChoVoucher(DropDownListRequestModel model)
        {
            var lstNhomDichVuCha = await _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.NhomDichVuBenhVienChaId == null).Select(p => p.Id).ToListAsync();

            var lstNhomDichVu = await _nhomDichVuBenhVienRepository.TableNoTracking
                .Select(item => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = item.Id,
                    DisplayName = item.Ten,
                    ParentId = item.NhomDichVuBenhVienChaId
                })
                .ToListAsync();

            lstNhomDichVu.Add(new NhomDichVuBenhVienTreeViewVo
            {
                
            });

            var query = lstNhomDichVu.Select(item => new NhomDichVuBenhVienTreeViewVo
            {
                KeyId = item.KeyId,
                DisplayName = item.DisplayName,
                ParentId = item.ParentId,
                Items = GetChildrenTree(lstNhomDichVu, item.KeyId, model.Query.RemoveVietnameseDiacritics(), item.DisplayName.RemoveVietnameseDiacritics())
            })
            .Where(x => x.ParentId == null &&
                        lstNhomDichVuCha.Any(p => p == x.KeyId) &&
                        (string.IsNullOrEmpty(model.Query) || (!string.IsNullOrEmpty(model.Query) && (x.Items.Any() || x.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(model.Query.RemoveVietnameseDiacritics().Trim().ToLower())))))
            .ToList();

            var idDichVuKhamBenh = _nhomDichVuBenhVienRepository.TableNoTracking.LastOrDefault().Id + 1;

            query.Add(new NhomDichVuBenhVienTreeViewVo
            {
                KeyId = idDichVuKhamBenh,
                DisplayName = "DỊCH VỤ KHÁM BỆNH",
                ParentId = null
            });

            query = query.Take(model.Take).ToList();

            return query;
        }

        private List<NhomDichVuBenhVienTreeViewVo> GetChildrenTree(List<NhomDichVuBenhVienTreeViewVo> comments, long Id, string queryString, string parentDisplay)
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id)
                .Select(c => new NhomDichVuBenhVienTreeViewVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString) ||
                            (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.RemoveVietnameseDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }

        public async Task<GridDataSource> GetListNhomDichVuForGridAsync(long voucherId)
        {
            var query = _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId && (p.NhomDichVuBenhVienId != null || p.NhomDichVuKhamBenh == true))
                                                                         .Select(p => new DichVuVoucherMarketingGridVo
                                                                         {
                                                                             Id = p.Id,
                                                                             VoucherId = p.VoucherId,
                                                                             NhomDichVuId = p.NhomDichVuBenhVienId != null ? p.NhomDichVuBenhVienId.GetValueOrDefault() : GetNhomDichVuBenhVienId(),
                                                                             NhomDichVuDisplay = p.NhomDichVuBenhVienId != null ? p.NhomDichVuBenhVien.Ten : "DỊCH VỤ KHÁM BỆNH",
                                                                             LoaiChietKhau = p.LoaiChietKhau,
                                                                             SoTienChietKhau = p.SoTienChietKhau,
                                                                             TiLeChietKhau = p.TiLeChietKhau,
                                                                             GhiChu = p.GhiChu
                                                                         });

            var queryTask = await query.ToArrayAsync();
            var countTask = await query.CountAsync();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetPagesListNhomDichVuForGridAsync(long voucherId)
        {
            var query = _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId && p.NhomDichVuBenhVienId != null)
                                                                         .Select(p => new DichVuVoucherMarketingGridVo
                                                                         {
                                                                             Id = p.Id,
                                                                             VoucherId = p.VoucherId,
                                                                             NhomDichVuId = p.NhomDichVuBenhVienId.GetValueOrDefault(),
                                                                             NhomDichVuDisplay = p.NhomDichVuBenhVien.Ten,
                                                                             LoaiChietKhau = p.LoaiChietKhau,
                                                                             SoTienChietKhau = p.SoTienChietKhau,
                                                                             TiLeChietKhau = p.TiLeChietKhau,
                                                                             GhiChu = p.GhiChu
                                                                         });

            return new GridDataSource { TotalRowCount = await query.CountAsync() };
        }

        public async Task<bool> KiemTraDichVuDaTonTaiTrongNhomDichVu(long voucherId, long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien)
        {
            var voucherChiTietMienGiams = await _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId)
                                                                                                 .Include(p => p.NhomDichVuBenhVien)
                                                                                                 .ToListAsync();

            if (loaiDichVuBenhVien == EnumDichVuTongHop.KyThuat)
            {
                if (voucherChiTietMienGiams.Any(p => p.NhomDichVuBenhVienId != null))
                {
                    var dichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuId)
                                                                                              .FirstOrDefaultAsync();

                    if (voucherChiTietMienGiams.Any(p => p.NhomDichVuBenhVienId != null &&
                                                         (p.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId || (p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && p.NhomDichVuBenhVien.NhomDichVuBenhVienChaId == dichVuKyThuat.NhomDichVuBenhVienId))))
                    {
                        return false;
                    }
                }
            }
            else if (loaiDichVuBenhVien == EnumDichVuTongHop.KhamBenh)
            {
                if (voucherChiTietMienGiams.Any(p => p.NhomDichVuKhamBenh == true))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> KiemTraNhomDichVuDaBaoGomDichVu(long voucherId, long nhomDichVuId)
        {
            var voucherChiTietMienGiams = await _voucherChiTietMienGiamRepository.TableNoTracking.Where(p => p.VoucherId == voucherId)
                                                                                                 .Include(p => p.DichVuKyThuatBenhVien)
                                                                                                 .ToListAsync();

            if(IsNhomDichVuKhamBenh(nhomDichVuId))
            {
                if (voucherChiTietMienGiams.Any(p => p.DichVuKhamBenhBenhVienId != null))
                {
                    return false;
                }
            }
            else
            {
                if (voucherChiTietMienGiams.Any(p => p.DichVuKyThuatBenhVienId != null))
                {
                    var nhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.Where(p => p.Id == nhomDichVuId)
                                                                                                 .FirstOrDefaultAsync();

                    if (voucherChiTietMienGiams.Any(p => p.DichVuKyThuatBenhVienId != null &&
                                                         (p.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomDichVuId || (nhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && p.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomDichVuBenhVien.NhomDichVuBenhVienChaId))))
                    {
                        return false;
                    }

                    //return true;
                }
            }

            return true;
        }

        public async Task<bool> KiemTraDichVuDaTonTaiTrongNhomDichVuTheoDanhSach(long dichVuId, EnumDichVuTongHop loaiDichVuBenhVien, List<VoucherChiTietMienGiam> lstVoucherChiTietMienGiam)
        {
            if(loaiDichVuBenhVien == EnumDichVuTongHop.KyThuat)
            {
                var dichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuId)
                                                                                          .Include(p => p.NhomDichVuBenhVien)
                                                                                          .FirstOrDefaultAsync();

                if (lstVoucherChiTietMienGiam.Any(p => p.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVienId || (dichVuKyThuat.NhomDichVuBenhVien.NhomDichVuBenhVienChaId != null && p.NhomDichVuBenhVienId == dichVuKyThuat.NhomDichVuBenhVien.NhomDichVuBenhVienChaId)))
                {
                    return false;
                }

                //return true;
            }
            else if(loaiDichVuBenhVien == EnumDichVuTongHop.KhamBenh)
            {
                if (lstVoucherChiTietMienGiam.Any(p => p.NhomDichVuBenhVienId != null && IsNhomDichVuKhamBenh(p.NhomDichVuBenhVienId.GetValueOrDefault())))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> KiemTraNhomDichVuDaBaoGomDichVuTheoDanhSach(long nhomDichVuId, List<VoucherChiTietMienGiam> lstVoucherChiTietMienGiam)
        {
            foreach(var item in lstVoucherChiTietMienGiam)
            {
                if (item.DichVuKyThuatBenhVienId != null)
                {
                    var dichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == item.DichVuKyThuatBenhVienId)
                                                                                              .Include(p => p.NhomDichVuBenhVien)
                                                                                              .FirstOrDefaultAsync();

                    if(dichVuKyThuat.NhomDichVuBenhVienId == nhomDichVuId || dichVuKyThuat.NhomDichVuBenhVien.NhomDichVuBenhVienChaId == nhomDichVuId)
                    {
                        return false;
                    }
                }
                else if (item.DichVuKhamBenhBenhVienId != null)
                {
                    if(IsNhomDichVuKhamBenh(nhomDichVuId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string GetBarcodeBasedOnMa(string ma)
        {
            return string.IsNullOrEmpty(ma) ? "" : BarcodeHelper.GenerateBarCode(ma, 300, 60);
        }

        public string GetHtmlVoucher(string hostingName, string ten, string ma, int soLuong, int maSoTu, int soLuongPhatHanh)
        {
            var result = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("Voucher"));
            var logoUrl = hostingName + "/assets/img/logo.png";
            var soChuSo = soLuongPhatHanh.ToString().Count();
            ten = Regex.Replace(ten, ".{50}", "$0<br/>");
            var content = "";

            for (int i = maSoTu; i < maSoTu + soLuong; i++)
            {
                if (content != "")
                {
                    content = content + "<div class=\"pagebreak\"> </div>";
                }

                var soChuSoHienTai = i.ToString().Count();
                var maVoucher = $"{ma}-{"".PadLeft(soChuSo - soChuSoHienTai, '0')}{i}";

                var data = new
                {
                    urlLogo = logoUrl,
                    urlBarcode = BarcodeHelper.GenerateBarCode(maVoucher, 300, 60),
                    tenVoucher = ten,
                    maVoucher = maVoucher,
                };

                content += TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, data);
            }

            return content;
        }

        public async Task<GridDataSource> GetListChiTietBenhNhanDaSuDungForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long voucherId);

            if (voucherId == 0)
            {
                return new GridDataSource { Data = new ChiTietBenhNhanDaSuDungGridVo[0], TotalRowCount = 0 };
            }

            var query = _theVoucherYeuCauTiepNhanRepository.TableNoTracking.Where(p => p.TheVoucher.VoucherId == voucherId)
                                                                           .Select(p => new ChiTietBenhNhanDaSuDungGridVo
                                                                           {
                                                                               MaTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                               MaBenhNhan = p.BenhNhan.MaBN,
                                                                               TenBenhNhan = p.BenhNhan.HoTen,
                                                                               DiaChi = p.BenhNhan.DiaChiDayDu,
                                                                               NgayDung = p.TheVoucher.CreatedOn.GetValueOrDefault(),
                                                                               MaVoucher = p.TheVoucher.Ma
                                                                           });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.MaTiepNhan, g => g.MaBenhNhan, g => g.TenBenhNhan, g => g.DiaChi, g => g.MaVoucher);

            var countTask = queryInfo.LazyLoadPage == true ? 0 : await query.CountAsync();
            var queryTask = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            return new GridDataSource { Data = queryTask, TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetPagesListChiTietBenhNhanDaSuDungForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long voucherId);

            if (voucherId == 0)
            {
                return new GridDataSource { Data = new ChiTietBenhNhanDaSuDungGridVo[0], TotalRowCount = 0 };
            }

            var query = _theVoucherYeuCauTiepNhanRepository.TableNoTracking.Where(p => p.TheVoucher.VoucherId == voucherId)
                                                                           .Select(p => new ChiTietBenhNhanDaSuDungGridVo
                                                                           {
                                                                               MaTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                                                               MaBenhNhan = p.BenhNhan.MaBN,
                                                                               TenBenhNhan = p.BenhNhan.HoTen,
                                                                               DiaChi = p.BenhNhan.DiaChiDayDu,
                                                                               NgayDung = p.TheVoucher.CreatedOn.GetValueOrDefault(),
                                                                               MaVoucher = p.TheVoucher.Ma
                                                                           });

            query = query.ApplyLike(queryInfo.SearchTerms, g => g.MaTiepNhan, g => g.MaBenhNhan, g => g.TenBenhNhan, g => g.DiaChi, g => g.MaVoucher);

            var countTask = await query.CountAsync();

            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<int> GetTongSoBenhNhanSuDungDichVu(long voucherId)
        {
            return await _theVoucherYeuCauTiepNhanRepository.TableNoTracking.Where(p => p.TheVoucher.VoucherId == voucherId)
                                                                            .CountAsync();
        }

        public async Task<bool> IsMaExists(string ma, long id)
        {
            if(id == 0)
            {
                return await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma == ma);
            }
            else
            {
                return await BaseRepository.TableNoTracking.AnyAsync(p => p.Ma == ma && p.Id != id);
            }
        }

        public async Task<SoLuongPhatHanhVoucher> GetSoLuongPhatHanhVoucher(long voucherId)
        {
            return await BaseRepository.TableNoTracking.Where(p => p.Id == voucherId)
                                                       .Select(p => new SoLuongPhatHanhVoucher
                                                       {
                                                           SoLuong = p.SoLuongPhatHanh
                                                       })
                                                       .FirstOrDefaultAsync();
        }

        public bool IsNhomDichVuKhamBenh(long nhomDichVuId)
        {
            return _nhomDichVuBenhVienRepository.TableNoTracking.LastOrDefault().Id + 1 == nhomDichVuId;
        }

        public long GetNhomDichVuBenhVienId()
        {
            return _nhomDichVuBenhVienRepository.TableNoTracking.LastOrDefault().Id + 1;
        }
    }
}