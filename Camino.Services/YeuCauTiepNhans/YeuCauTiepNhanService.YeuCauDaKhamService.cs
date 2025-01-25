using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial class YeuCauTiepNhanService
    {
        public async Task<GridDataSource> GetDataForGridAsyncDanhSachDaKham(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .OrderByDescending(s => s.ThoiDiemTiepNhan)
                //TODO: TrangThaiYeuCauTiepNhan need update
                //.Where(o=>o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .Select(s => new YeuCauTiepNhanGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.Id,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    DiaChi = s.DiaChiDayDu,
                    //TODO: TrangThaiYeuCauTiepNhan need update
                    //TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                    //TrangThaiYeuCauTiepNhanDisplay = s.TrangThaiYeuCauTiepNhan.GetDescription(),
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTime(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    DoiTuong = SetSoPhanTramHuongBHYT(s.BHYTMaSoThe)
                });
            //var tuNgayTemp = Convert.ToDateTime(queryString.ThoiDiemTiepNhanTu);
            //var denNgayTemp = string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen)
            //    ? DateTime.Now
            //    : Convert.ToDateTime(queryString.ThoiDiemTiepNhanDen);
            //query = query.Where(p => p.ThoiDiemTiepNhan == tuNgayTemp);
            //if (!String.IsNullOrEmpty(queryString.DiaChi))
            //{
            //    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.TrimEnd().TrimStart()));
            //}
            //if ((queryString.KhoaPhongId) != null)
            //{
            //    query = query.Where(p => p.KhoaPhongId == queryString.KhoaPhongId);
            //}
            //if (queryString.TrangThaiYeuCauTiepNhan != null)
            //{
            //    query = query.Where(p => p.TrangThaiYeuCauTiepNhan == queryString.TrangThaiYeuCauTiepNhan);
            //}
            //if ((queryString.DoiTuongBHYT) != null)
            //{
            //    var querySearch = BaseRepository.TableNoTracking.Where(p => p.Id == queryString.DoiTuongBHYT)
            //       .Select(p => p.BHYTMaSoThe).FirstOrDefault();
            //    query = query.Where(p => p.DoiTuong.Contains(SetSoPhanTramHuongBHYT(querySearch.ToString())));
            //}
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var queryStrings = JsonConvert.DeserializeObject<YeuCauTiepNhanSearch>(queryInfo.SearchTerms);
                query = query.Where(p => p.NoiTiepNhanId == queryStrings.PhongKhamId);
                //query = query.ApplyLike(queryStrings.SearchChing,
                //            g => g.HoTen, g => g.NamSinh.ToString(), g => g.DiaChi, g => g.TrieuChungTiepNhan).Where(g=>g.NoiTiepNhanId == queryStrings.PhongKhamId);
                //query = query.Where(p => EF.Functions.Like(p.HoTen, $"%{queryInfo.SearchTerms}%"));
            }
            query = query.Where(o => Convert.ToDateTime(o.ThoiDiemTiepNhan).Date == (DateTime.Now).Date);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }


        public string SetSoPhanTramHuongBHYT(string maThe)
        {
            var maTheArray = maThe.Substring(2, 1);
            var soPhanTramHuongBHYT = 0;
            if (maTheArray == "1" || maTheArray == "2")
            {
                soPhanTramHuongBHYT = 100;
            }
            else if (maTheArray == "3")
            {
                soPhanTramHuongBHYT = 95;
            }
            else if (maTheArray == "4")
            {
                soPhanTramHuongBHYT = 80;
            }
            else
            {
                soPhanTramHuongBHYT = 100;
            }
            var result = "BHYT (" + soPhanTramHuongBHYT.ToString() + "%)";
            return result;
        }
        public async Task<GridDataSource> GetTotalPageForGridAsyncDanhSachDaKham(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            //todo: need improve
            var query = BaseRepository.TableNoTracking
                .OrderByDescending(s => s.ThoiDiemTiepNhan)
                //TODO: TrangThaiYeuCauTiepNhan need update
                //.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .Select(s => new YeuCauTiepNhanGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.MaYeuCauTiepNhan,
                    BenhNhanId = s.Id,
                    HoTen = s.HoTen,
                    NamSinh = s.NamSinh,
                    DiaChi = s.DiaChiDayDu,
                    //TODO: TrangThaiYeuCauTiepNhan need update
                    //TrangThaiYeuCauTiepNhan = s.TrangThaiYeuCauTiepNhan,
                    //TrangThaiYeuCauTiepNhanDisplay = s.TrangThaiYeuCauTiepNhan.GetDescription(),
                    ThoiDiemTiepNhanDisplay = s.ThoiDiemTiepNhan.ApplyFormatDateTime(),
                    ThoiDiemTiepNhan = s.ThoiDiemTiepNhan,
                    TrieuChungTiepNhan = s.TrieuChungTiepNhan,
                    DoiTuong = SetSoPhanTramHuongBHYT(s.BHYTMaSoThe)
                });
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var queryString = JsonConvert.DeserializeObject<YeuCauTiepNhanGridVo>(queryInfo.AdditionalSearchString);
                if (!String.IsNullOrEmpty(queryString.HoTen))
                {
                    query = query.Where(p => p.HoTen.Contains(queryString.HoTen.TrimEnd().TrimStart()));
                }
                if (!String.IsNullOrEmpty(queryString.NamSinh.ToString()))
                {
                    query = query.Where(p => p.NamSinh.ToString().Contains(queryString.NamSinh.ToString().TrimEnd().TrimStart()));
                }
                if (!string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanTu))
                {

                    var tuNgayTemp = Convert.ToDateTime(queryString.ThoiDiemTiepNhanTu);
                    //var denNgayTemp = string.IsNullOrEmpty(queryString.ThoiDiemTiepNhanDen)
                    //    ? DateTime.Now
                    //    : Convert.ToDateTime(queryString.ThoiDiemTiepNhanDen);
                    query = query.Where(p => p.ThoiDiemTiepNhan == tuNgayTemp);
                }
                if (!String.IsNullOrEmpty(queryString.DiaChi))
                {
                    query = query.Where(p => p.DiaChi.Contains(queryString.DiaChi.TrimEnd().TrimStart()));
                }
                if ((queryString.KhoaPhongId) != null)
                {
                    query = query.Where(p => p.KhoaPhongId == queryString.KhoaPhongId);
                }
                //TODO: TrangThaiYeuCauTiepNhan need update
                //if (queryString.TrangThaiYeuCauTiepNhan != null)
                //{
                //    query = query.Where(p => p.TrangThaiYeuCauTiepNhan == queryString.TrangThaiYeuCauTiepNhan);
                //}
                if ((queryString.DoiTuongBHYT) != null)
                {
                    var querySearch = BaseRepository.TableNoTracking.Where(p => p.Id == queryString.DoiTuongBHYT)
                       .Select(p => p.BHYTMaSoThe).FirstOrDefault();
                    query = query.Where(p => p.DoiTuong.Contains(SetSoPhanTramHuongBHYT(querySearch.ToString())));
                }
            }
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                query = query.ApplyLike(queryInfo.SearchTerms,
                            g => g.HoTen, g => g.NamSinh.ToString(), g => g.DiaChi, g => g.TrieuChungTiepNhan);
                //query = query.Where(p => EF.Functions.Like(p.HoTen, $"%{queryInfo.SearchTerms}%"));
            }
            query = query.Where(o => Convert.ToDateTime(o.ThoiDiemTiepNhan).Date == (DateTime.Now).Date);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<List<LookupItemVo>> GetDoiTuongs(DropDownListRequestModel queryInfo)
        {
            var result = BaseRepository.TableNoTracking
                .Select(item => new LookupItemVo
                {
                    DisplayName = SetSoPhanTramHuongBHYT(item.BHYTMaSoThe),
                    KeyId = item.Id,
                });
            result = result.GroupBy(p => new
            {
                p.DisplayName
            }).Select(g => g.First());
            await Task.WhenAll(result.ToListAsync());
            return result.ToList();
        }
        public async Task<List<LookupItemVo>> GetLyDos(DropDownListRequestModel queryInfo)
        {
            var result = BaseRepository.TableNoTracking

               //.Include(o => o.TrieuChung)
               .Select(item => new LookupItemVo
               {
                   DisplayName = item.TrieuChungTiepNhan,
                   KeyId = item.Id,
               });
            result = result.GroupBy(p => new
            {
                p.DisplayName
            }).Select(g => g.First());
            await Task.WhenAll(result.ApplyLike(queryInfo.Query, o => o.DisplayName).ToListAsync());
            return result.ToList();
        }
        public List<LookupItemVo> GetTinhTrangKhamBenhs()
        {
            var result = BaseRepository.TableNoTracking
                //TODO: TrangThaiYeuCauTiepNhan need update
              //.Where(p => p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
              //|| p.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
              .Select(item => new LookupItemVo
              {
                  DisplayName = item.TrangThaiYeuCauTiepNhan.GetDescription(),
                  KeyId = (long)item.TrangThaiYeuCauTiepNhan,
              });
            result = result.GroupBy(p => new
            {
                p.DisplayName
            }).Select(g => g.First());
            return result.ToList();
        }

        public List<ThoiDiemDuKien> ThoiDiemKhamDuKien(long Id)
        {
            //TiepNhan
            var ListcountPatientChuaKham = BaseRepository.Table.AsNoTracking()
               .OrderByDescending(o => o.Id)
               //.Where(o => o.TrangThaiYeuCauTiepNhan.GetDescription() == "Chưa khám")
                //TODO: TrangThaiYeuCauTiepNhan need update
               //.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
               .Select(o => new ThoiDiemDuKien
               {
                   Id = o.Id,
                   ThoiDiemTiepNhans = o.ThoiDiemTiepNhan
               }).ToList();
            var selectIdChuaKham = ListcountPatientChuaKham.Where(o => o.Id == Id).Select(o => o.Id).FirstOrDefault();

            if (ListcountPatientChuaKham != null && selectIdChuaKham == Id)
            {
                for (int i = 0; i < ListcountPatientChuaKham.Count(); i++)
                {
                    ListcountPatientChuaKham[i].ThoiDiemDuKiens = ListcountPatientChuaKham[i].ThoiDiemTiepNhans.AddMinutes(8 * (i + 1)); //=>8 phut/benh nhan
                }
                return ListcountPatientChuaKham;
            }
            //KhamDuKien
            var ListcountPatientDangKham = BaseRepository.Table.AsNoTracking()
              .OrderByDescending(o => o.Id)
              //.Where(o => o.TrangThaiYeuCauTiepNhan.GetDescription() == "Đang khám")
                //TODO: TrangThaiYeuCauTiepNhan need update
              //.Where(o => o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
              .Select(o => new ThoiDiemDuKien
              {
                  Id = o.Id,
                  ThoiDiemTiepNhans = o.ThoiDiemTiepNhan
              }).ToList();
            var selectIdDangKham = ListcountPatientDangKham.Where(o => o.Id == Id).Select(o => o.Id).FirstOrDefault();

            if (ListcountPatientDangKham != null && selectIdDangKham == Id)
            {
                for (int i = 0; i < ListcountPatientDangKham.Count(); i++)
                {
                    ListcountPatientDangKham[i].ThoiDiemDuKiens = ListcountPatientDangKham[i].ThoiDiemTiepNhans.AddMinutes(8 * (i + 1));
                }
                return ListcountPatientDangKham;
            }
            return null;
        }
    }
}
