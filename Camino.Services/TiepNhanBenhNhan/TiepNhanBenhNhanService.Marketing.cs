using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.DanhMucMarketing;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Services.TiepNhanBenhNhan
{
    public partial class TiepNhanBenhNhanService
    {
        public async Task<List<DanhSachGoiChon>> GetMarketingForBenhNhan(long benhNhanId)
        {
            var benhNhan = await _benhNhanRepository.GetByIdAsync(benhNhanId, p => p.Include(o => o.YeuCauGoiDichVus).ThenInclude(o => o.ChuongTrinhGoiDichVu));

            var result = benhNhan.YeuCauGoiDichVus
                // cập nhật 19/05/2021 không hiển thị gói sơ sinh ở tiếp nhận
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.DaQuyetToan != true // cập nhật 10/06/2021: ko hiển thị gói đã quyết toán
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                    )
                .Select(o => new DanhSachGoiChon
                {
                    TenChuongTrinh = o.ChuongTrinhGoiDichVu?.Ten,
                    TenGoiDichVu = o.TenGoiDichVu,
                    ChuongTrinhGoiDichVuId = o.ChuongTrinhGoiDichVuId,
                    IsFromMarketing = o.BoPhanMarketingDangKy ?? false,
                    YeuCauGoiDichVuId = o.Id,
                    BenhNhanId = benhNhanId,
                    DaThanhToan = o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan,
                });

            return result.ToList();
        }

        public async Task<YeuCauTiepNhan> XoaGoiGiuLaiDichVu(DanhSachGoiChon model)
        {
            var entity = await GetByIdHaveInclude(model.YeuCauTiepNhanId ?? 0);

            var ycgdvDeleted = false;

            if (entity != null)
            {
                if (entity.YeuCauDichVuGiuongBenhViens.Any())
                {
                    foreach (var item in entity.YeuCauDichVuGiuongBenhViens)
                    {
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId)
                        {
                            item.YeuCauGoiDichVuId = null;
                            item.YeuCauGoiDichVu.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }
                if (entity.YeuCauDichVuKyThuats.Any())
                {
                    foreach (var item in entity.YeuCauDichVuKyThuats)
                    {
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId)
                        {
                            item.YeuCauGoiDichVuId = null;
                            item.YeuCauGoiDichVu.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }
                if (entity.YeuCauKhamBenhs.Any())
                {
                    foreach (var item in entity.YeuCauKhamBenhs)
                    {
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId)
                        {
                            item.YeuCauGoiDichVuId = null;
                            item.YeuCauGoiDichVu.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }

                //
                if (!ycgdvDeleted)
                {
                    var ycDelete = await _yeuCauGoiDichVuRepository.GetByIdAsync(model.YeuCauGoiDichVuId ?? 0);
                    await _yeuCauGoiDichVuRepository.DeleteAsync(ycDelete);
                }
            }

            return entity;

        }

        public async Task<YeuCauTiepNhan> XoaGoiKhongGiuLaiDichVu(DanhSachGoiChon model)
        {
            var entity = await GetByIdHaveInclude(model.YeuCauTiepNhanId ?? 0);

            var ycgdvDeleted = false;
            
            if (entity != null)
            {
                if (entity.YeuCauDichVuGiuongBenhViens.Any())
                {
                    foreach (var item in entity.YeuCauDichVuGiuongBenhViens)
                    {
                        // bổ sung trường hợp là dịch vụ khuyến mãi
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId || item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != model.YeuCauGoiDichVuId))
                        {
                            item.YeuCauGoiDichVu.WillDelete = true;
                            item.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }
                if (entity.YeuCauDichVuKyThuats.Any())
                {
                    foreach (var item in entity.YeuCauDichVuKyThuats)
                    {
                        // bổ sung trường hợp là dịch vụ khuyến mãi
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId || item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != model.YeuCauGoiDichVuId))
                        {
                            item.YeuCauGoiDichVu.WillDelete = true;
                            item.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }
                if (entity.YeuCauKhamBenhs.Any())
                {
                    foreach (var item in entity.YeuCauKhamBenhs)
                    {
                        // bổ sung trường hợp là dịch vụ khuyến mãi
                        if (item.YeuCauGoiDichVuId == model.YeuCauGoiDichVuId || item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != model.YeuCauGoiDichVuId))
                        {
                            item.YeuCauGoiDichVu.WillDelete = true;
                            item.WillDelete = true;

                            ycgdvDeleted = true;
                        }
                    }
                }

                //
                if (!ycgdvDeleted)
                {
                    var ycDelete = await _yeuCauGoiDichVuRepository.GetByIdAsync(model.YeuCauGoiDichVuId ?? 0);
                    await _yeuCauGoiDichVuRepository.DeleteAsync(ycDelete);
                }
            }

            return entity;

        }

        public async Task<GridDataSource> GetDataThongTinGoiForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new ThongTinGoiMarketingGridVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();

            //var lstIdString = new List<long>();
            long benhNhanId = 0;

            long chuongTrinhGoiDichVuIdPopup = 0;


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                //long.TryParse(queryInfo.AdditionalSearchString, out benhNhanId);

                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out chuongTrinhGoiDichVuIdPopup);
                }

                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }

            if (benhNhanId != 0)
            {
                if (chuongTrinhGoiDichVuIdPopup != 0)
                {
                    //&& p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId)
                    var queryOfChuongTrinh = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.Id == chuongTrinhGoiDichVuIdPopup  

                                // bệnh nhân đã đăng ký thì luôn hiện yêu cầu gói đến khi sử dụng hết hoặc quyết toán
                                //&& p.TuNgay.Date <= dateTimeNow
                                //&& (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                                //&& p.TamNgung != true
                                && p.GoiSoSinh != true// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
                                && p.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Any(y => y.DaQuyetToan != true && y.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien) // cập nhật 10/06/2021: ko hiển thị các gói đã quyết toán
                            )
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,

                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,
                        BenhNhanDaThanhToan = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Select(a => a.SoTienBenhNhanDaChi ?? 0).FirstOrDefault(),
                        DangDung = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauKhamBenhs.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                   //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                   && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                          //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                          && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0)))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                                                                 //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                 && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))

                    });
                    queryOfChuongTrinh = queryOfChuongTrinh.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                    lstIdOfBenhNhan.Add(chuongTrinhGoiDichVuIdPopup);

                    query = query.Concat(queryOfChuongTrinh);
                        
                    //&& p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId)
                    var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.Id != chuongTrinhGoiDichVuIdPopup

                                // bệnh nhân đã đăng ký thì luôn hiện yêu cầu gói đến khi sử dụng hết hoặc quyết toán
                                //&& p.TuNgay.Date <= dateTimeNow
                                //&& (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                                //&& p.TamNgung != true
                                && p.GoiSoSinh != true// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
                                && p.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Any(y => y.DaQuyetToan != true && y.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien) // cập nhật 10/06/2021: ko hiển thị các gói đã quyết toán
                            )
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiển thị tên chương trình
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,

                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,
                        BenhNhanDaThanhToan = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Select(a => a.SoTienBenhNhanDaChi ?? 0).FirstOrDefault(),
                        DangDung = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauKhamBenhs.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                                                                   //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                                                   && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                                                          //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                                                          && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0)))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                                                                                                                                 //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                                                                 && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))

                    });
                    queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                    lstIdOfBenhNhan.AddRange(await queryOfBenhNhan.Select(p => p.Id).ToListAsync());

                    query = query.Concat(queryOfBenhNhan);
                }
                else
                {
                    var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId)

                                // bệnh nhân đã đăng ký thì luôn hiện yêu cầu gói đến khi sử dụng hết hoặc quyết toán
                                //&& p.TuNgay.Date <= dateTimeNow
                                //&& (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                                //&& p.TamNgung != true
                                && p.GoiSoSinh != true// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
                                && p.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Any(y => y.DaQuyetToan != true && y.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien) // cập nhật 10/06/2021: ko hiển thị các gói đã quyết toán
                                )
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,

                        TongCong = s.GiaTruocChietKhau,
                        GiaGoi = s.GiaSauChietKhau,
                        BenhNhanDaThanhToan = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Select(a => a.SoTienBenhNhanDaChi ?? 0).FirstOrDefault(),
                        DangDung = s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauKhamBenhs.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                                            //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                            && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                                   //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                   && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0)))
                                   + s.YeuCauGoiDichVus.Where(x => x.BenhNhanId == benhNhanId).Sum(x => x.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                                                                                          //&& a.TrangThaiThanhToan != Enums.TrangThaiThanhToan.ChuaThanhToan
                                                                                                          && a.YeuCauGoiDichVuId == x.Id).Sum(a => a.DonGiaSauChietKhau ?? 0))

                    });
                    queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                    lstIdOfBenhNhan.AddRange(await queryOfBenhNhan.Select(p => p.Id).ToListAsync());

                    query = query.Concat(queryOfBenhNhan);
                }

            }

            ////cập nhật BVHD - 3246: chỉ hiện gói dịch vụ đã đăng ký
            //if (chuongTrinhGoiDichVuIdPopup != 0)
            //{
            //    var queryNormalChuongTrinh = _chuongTrinhGoiDichVuRepository.TableNoTracking
            //        .Where(p => p.Id == chuongTrinhGoiDichVuIdPopup 
            //                    && !lstIdOfBenhNhan.Contains(p.Id) 
            //                    && p.TuNgay.Date <= dateTimeNow
            //                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
            //                    && p.TamNgung != true
            //                    && p.GoiSoSinh != true)// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
            //        .Select(s => new ThongTinGoiMarketingGridVo
            //        {
            //            Id = s.Id,
            //            TenGoi = s.Ten,
            //            TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
            //            IsChecked = false,

            //            GiaSauChietKhau = s.GiaSauChietKhau,
            //            GiaTruocChietKhau = s.GiaTruocChietKhau,
            //        });
            //    queryNormalChuongTrinh = queryNormalChuongTrinh.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            //    query = query.Concat(queryNormalChuongTrinh);

            //    var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
            //        .Where(p => p.Id != chuongTrinhGoiDichVuIdPopup 
            //                    && !lstIdOfBenhNhan.Contains(p.Id) 
            //                    && p.TuNgay.Date <= dateTimeNow
            //                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
            //                    && p.TamNgung != true
            //                    && p.GoiSoSinh != true)// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
            //        .Select(s => new ThongTinGoiMarketingGridVo
            //        {
            //            Id = s.Id,
            //            TenGoi = s.Ten,
            //            TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
            //            IsChecked = false,

            //            GiaSauChietKhau = s.GiaSauChietKhau,
            //            GiaTruocChietKhau = s.GiaTruocChietKhau,
            //        });
            //    queryNormal = queryNormal.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            //    query = query.Concat(queryNormal);
            //}
            //else
            //{
            //    var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
            //        .Where(p => !lstIdOfBenhNhan.Contains(p.Id)
            //                    && p.TuNgay.Date <= dateTimeNow
            //                    && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
            //                    && p.TamNgung != true
            //                    && p.GoiSoSinh != true)// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
            //        .Select(s => new ThongTinGoiMarketingGridVo
            //        {
            //            Id = s.Id,
            //            TenGoi = s.Ten,
            //            TenDisplay = s.Ten, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
            //            IsChecked = false,

            //            GiaSauChietKhau = s.GiaSauChietKhau,
            //            GiaTruocChietKhau = s.GiaTruocChietKhau,
            //        });
            //    queryNormal = queryNormal.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            //    query = query.Concat(queryNormal);
            //}



            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query
                //.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                if (_yeuCauGoiDichVuRepository.TableNoTracking.Any(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id))
                {
                    item.TrangThai = _yeuCauGoiDichVuRepository.TableNoTracking.First(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.Id).TrangThaiThanhToan;
                    item.TrangThaiDisplay = item.TrangThai.GetDescription();
                }
                //
                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber() + "%";
                //
                //if (!item.IsChecked)
                //{
                //    if (lstIdString.Contains(item.Id))
                //    {
                //        item.IsChecked = true;
                //    }
                //}
                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataThongTinGoiForGridAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            
            long benhNhanId = 0;
            long yeuCauGoiDichVuIdPopup = 0;


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[1]) && queryInfo.AdditionalSearchString.Split("|")[1] != "")
                {
                    long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out yeuCauGoiDichVuIdPopup);
                }

                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            }


            var query = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(p => p.BenhNhanId == benhNhanId
                            && p.GoiSoSinh != true// cập nhật 19/05/2021: ko hiển thị gói sơ sinh ở tiếp nhận
                            && p.DaQuyetToan != true
                            && p.TrangThai == Enums.EnumTrangThaiYeuCauGoiDichVu.DangThucHien // cập nhật 10/06/2021: ko hiển thị các gói đã quyết toán

                            && p.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                )
                .ApplyLike(queryInfo.SearchTerms, g => g.TenChuongTrinh)
                .OrderByDescending(x => x.Id == yeuCauGoiDichVuIdPopup).ThenBy(x => x.Id)
                .Select(s => new ThongTinGoiMarketingGridVo
                {
                    Id = s.Id,
                    TenGoi = s.TenChuongTrinh,
                    TenDisplay = s.TenChuongTrinh, // + " - " + s.TenGoiDichVu, // chỉ hiện tên chương trình
                        IsChecked = true,

                    GiaSauChietKhau = s.GiaSauChietKhau,
                    GiaTruocChietKhau = s.GiaTruocChietKhau,

                    TongCong = s.GiaTruocChietKhau,
                    GiaGoi = s.GiaSauChietKhau,
                    BenhNhanDaThanhToan = s.SoTienBenhNhanDaChi ?? 0,
                    #region Cập nhật 15/12/2022
                    //DangDung = (s.YeuCauKhamBenhs.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    //               ? s.YeuCauKhamBenhs.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Sum(a => a.DonGiaSauChietKhau ?? 0) : 0)
                    //           + (s.YeuCauDichVuKyThuats.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    //               ? s.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0)) : 0)
                    //           + (s.YeuCauDichVuGiuongBenhViens.Any(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy)
                    //               ? s.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy).Sum(a => a.DonGiaSauChietKhau ?? 0) : 0),
                    #endregion
                    TrangThai = s.TrangThaiThanhToan,
                    TrangThaiDisplay = s.TrangThaiThanhToan.GetDescription()

                });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query
                .Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            #region Cập nhật 15/12/2022
            var lstChiPhiKham = new List<ThongTinChiPhiGoiDichVuVo>();
            var lstChiPhiKyThuat = new List<ThongTinChiPhiGoiDichVuVo>();
            var lstChiPhiGiuong = new List<ThongTinChiPhiGoiDichVuVo>();
            if (queryTask != null && queryTask.Result.Length > 0)
            {
                var lstYeuCauGoiId = queryTask.Result.Select(x => x.Id).ToList();
                lstChiPhiKham = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauGoiDichVuId != null
                                && lstYeuCauGoiId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.YeuCauGoiDichVuId.Value,
                        ChiPhi = x.DonGiaSauChietKhau ?? 0
                    })
                    .ToList();
                lstChiPhiKham = lstChiPhiKham
                    .GroupBy(x => new { x.YeuCauGoiId })
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.Key.YeuCauGoiId,
                        ChiPhi = x.Sum(a => a.ChiPhi)
                    })
                    .ToList();

                lstChiPhiKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauGoiDichVuId != null
                                && lstYeuCauGoiId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.YeuCauGoiDichVuId.Value,
                        ChiPhi = x.SoLan * (x.DonGiaSauChietKhau ?? 0)
                    }).ToList();
                lstChiPhiKyThuat = lstChiPhiKyThuat
                    .GroupBy(x => new { x.YeuCauGoiId })
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.Key.YeuCauGoiId,
                        ChiPhi = x.Sum(a => a.ChiPhi)
                    })
                    .ToList();

                lstChiPhiGiuong = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking
                    .Where(x => x.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy
                                && x.YeuCauGoiDichVuId != null
                                && lstYeuCauGoiId.Contains(x.YeuCauGoiDichVuId.Value))
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.YeuCauGoiDichVuId.Value,
                        ChiPhi = x.DonGiaSauChietKhau ?? 0
                    }).ToList();
                lstChiPhiGiuong = lstChiPhiGiuong
                    .GroupBy(x => new { x.YeuCauGoiId })
                    .Select(x => new ThongTinChiPhiGoiDichVuVo()
                    {
                        YeuCauGoiId = x.Key.YeuCauGoiId,
                        ChiPhi = x.Sum(a => a.ChiPhi)
                    })
                    .ToList();
            }
            #endregion

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber() + "%";

                #region Cập nhật 15/12/2022
                var chiPhiKham = lstChiPhiKham.Where(x => x.YeuCauGoiId == item.Id).Sum(x => x.ChiPhi ?? 0);
                var chiPhiKyThuat = lstChiPhiKyThuat.Where(x => x.YeuCauGoiId == item.Id).Sum(x => x.ChiPhi ?? 0);
                var chiPhiGiuong = lstChiPhiGiuong.Where(x => x.YeuCauGoiId == item.Id).Sum(x => x.ChiPhi ?? 0);
                item.DangDung = chiPhiKham + chiPhiKyThuat + chiPhiGiuong;
                #endregion

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalThongTinGoiPageForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var dateTimeNow = DateTime.Now.Date;

            var query = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new ThongTinGoiMarketingGridVo { }).AsQueryable();

            var lstIdOfBenhNhan = new List<long>();

            //var lstIdString = new List<long>();
            long benhNhanId = 0;


            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                long.TryParse(queryInfo.AdditionalSearchString, out benhNhanId);
            }

            if (benhNhanId != 0)
            {
                //var benhNhanId = JsonConvert.DeserializeObject<long>(queryInfo.AdditionalSearchString);
                var queryOfBenhNhan = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => p.YeuCauGoiDichVus.Any(x => x.BenhNhanId == benhNhanId) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        IsChecked = true,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,
                    });
                queryOfBenhNhan = queryOfBenhNhan.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

                lstIdOfBenhNhan = await queryOfBenhNhan.Select(p => p.Id).ToListAsync();

                query = query.Concat(queryOfBenhNhan);
            }

            var queryNormal = _chuongTrinhGoiDichVuRepository.TableNoTracking
                    .Where(p => !lstIdOfBenhNhan.Contains(p.Id) && p.TuNgay.Date <= dateTimeNow
                && (p.DenNgay == null || (p.DenNgay != null && p.DenNgay.Value.Date > dateTimeNow))
                && p.TamNgung != true)
                    .Select(s => new ThongTinGoiMarketingGridVo
                    {
                        Id = s.Id,
                        TenGoi = s.Ten,
                        IsChecked = false,

                        GiaSauChietKhau = s.GiaSauChietKhau,
                        GiaTruocChietKhau = s.GiaTruocChietKhau,
                    });
            queryNormal = queryNormal.ApplyLike(queryInfo.SearchTerms, g => g.TenGoi);

            query = query.Concat(queryNormal);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataDichVuGoiForGridAsync(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            dichVuGiuongDaChiDinhs = dichVuGiuongDaChiDinhs == null ? new List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>() : dichVuGiuongDaChiDinhs;

            //long childId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;

            var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();

            var lstModelChonTrongLanYCTNNay = new List<DanhSachDichVuChonTrongLanPopup>();

            long benhNhanId = 0;

            long childId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
                {
                    lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                }
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[3]) && queryInfo.AdditionalSearchString.Split("|")[3] != "")
                {
                    lstModelChonTrongLanYCTNNay = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[3]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out childId);
            }


            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var queryKhamBenh = _chuongTrinhGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGiaSauChietKhau,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKB,

                    DichVuId = s.DichVuKhamBenhBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,
                    

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,

                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                });

            var queryDVKT = _chuongTrinhGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGiaSauChietKhau,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKT,

                    DichVuId = s.DichVuKyThuatBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,

                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    IsNhomTiemChung = nhomTiemChungId == null ? false : s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                });

            var queryDVG = _chuongTrinhGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == childId)
               .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
               .Select(s => new DichVuGoiMarketingGridVo
               {
                   Id = s.Id,
                   Ma = s.DichVuGiuongBenhVien.Ma,
                   TenDichVu = s.DichVuGiuongBenhVien.Ten,
                   LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                   SoLuong = s.SoLan,
                   //SoLuongDisplay = s.SoLan.ApplyNumber(),
                   DonGia = s.DonGiaSauChietKhau,
                   //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                   ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                   //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                   GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                   GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                   TenNhomDichVu = tenNhomDVG,

                   DichVuId = s.DichVuGiuongBenhVienId,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                   ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                   TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,

                   //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.Any(p => p.BenhNhanId == benhNhanId) 
                   //? s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan : null,

                   NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
               });

            var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                //set trang thai thanh toan
                var yc = _yeuCauGoiDichVuRepository.TableNoTracking.FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (yc != null)
                {
                    item.TrangThaiThanhToan = yc.TrangThaiThanhToan;
                }
                //
                var dichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                        .Include(p => p.YeuCauDichVuKyThuats)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)
                        .Include(p => p.YeuCauKhamBenhs)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                        .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (dichVu != null)
                {

                    if (item.TenNhomDichVu == tenNhomDVKB)
                    {
                        if (dichVu.YeuCauKhamBenhs.Any())
                        {
                            item.SoLuongDaDung = dichVu.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == item.DichVuId)
                                ? dichVu.YeuCauKhamBenhs.Where(p => p.DichVuKhamBenhBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Count() : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else if (item.TenNhomDichVu == tenNhomDVKT)
                    {
                        if (dichVu.YeuCauDichVuKyThuats.Any())
                        {
                            item.SoLuongDaDung = dichVu.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == item.DichVuId)
                                ? dichVu.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else
                    {
                        //if (dichVu.YeuCauDichVuGiuongBenhViens.Any())
                        //{
                        //    item.SoLuongDaDung = dichVu.YeuCauDichVuGiuongBenhViens.Any(p => p.DichVuGiuongBenhVienId == item.DichVuId)
                        //        ? dichVu.YeuCauDichVuGiuongBenhViens.Where(p => p.DichVuGiuongBenhVienId == item.DichVuId).Count() : 0;
                        //    item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        //}
                        //else
                        //{
                        //    item.SoLuongDaDung = 0;
                        //    item.SoLuongDaDungDisplay = "0";
                        //}

                        // data get từ function tổng hợp rồi, nên ko cần check trạng thái
                        item.SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuBenhVienId == item.DichVuId
                                                                               && a.YeuCauGoiDichVuId == dichVu.Id
                                                                               && a.NhomGiaDichVuBenhVienId == item.NhomGiaDichVuBenhVienId).Sum(b => b.SoLuongDaSuDung);
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }
                }
                else
                {
                    item.SoLuongDaDung = 0;
                    item.SoLuongDaDungDisplay = "0";
                }

                //check active truoc khi set lst chọn
                if (item.SoLuongDaDung < item.SoLuong && item.NhomDichVu != Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && !item.IsNhomTiemChung)
                {
                    item.IsActive = true;
                }


                //
                var itemChon = lstModelChon.FirstOrDefault(p => p.TenNhomDichVu == item.TenNhomDichVu && p.DichVuId == item.DichVuId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (itemChon != null)
                {
                    item.SoLuongDungLanNay = itemChon.SoLuongDungLanNay;
                    //item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                }

                //
                if (lstModelChon.Any(p => p.TenNhomDichVu == item.TenNhomDichVu && p.DichVuId == item.DichVuId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId))
                {
                    item.IsChecked = true;
                }

                //lstModelChonTrongLanYCTNNay
                var itemChon2 = lstModelChonTrongLanYCTNNay
                    .Where(p => p.TenNhomDichVu == item.TenNhomDichVu && p.DichVuId == item.DichVuId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId).ToList();
                if (itemChon2.Any())
                {
                    foreach (var i in itemChon2)
                    {
                        item.SoLuongDaDung = item.SoLuongDaDung + i.SoLuongDungLanNay;
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }

                    item.SoLuongConLai = (item.SoLuong - item.SoLuongDaDung < 0) ? 0 : item.SoLuong - item.SoLuongDaDung;
                }
                else
                {
                    item.SoLuongConLai = (item.SoLuong - item.SoLuongDaDung < 0) ? 0 : item.SoLuong - item.SoLuongDaDung;

                }
                //

                item.SoLuongDisplay = item.SoLuong.ApplyNumber();
                item.DonGiaDisplay = item.DonGia.ApplyFormatMoneyVND();
                item.ThanhTienDisplay = (item.DonGia * item.SoLuong).ApplyFormatMoneyVND();

                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber();

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDichVuGoiForGridAsyncVer2(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            dichVuGiuongDaChiDinhs = dichVuGiuongDaChiDinhs == null ? new List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>() : dichVuGiuongDaChiDinhs;

            var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();
            var lstModelChonTrongLanYCTNNay = new List<DanhSachDichVuChonTrongLanPopup>();
            long benhNhanId = 0;
            long yeuCauGoiId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
                {
                    lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                }
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[3]) && queryInfo.AdditionalSearchString.Split("|")[3] != "")
                {
                    lstModelChonTrongLanYCTNNay = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[3]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out yeuCauGoiId);
            }


            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var queryKhamBenh = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == yeuCauGoiId)
                .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuKhamBenhBenhVien.Ma, x => x.DichVuKhamBenhBenhVien.Ten)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    YeuCauGoiDichVuId = yeuCauGoiId,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = s.SoLan,
                    DonGia = s.DonGiaSauChietKhau,
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKB,
                    DichVuId = s.DichVuKhamBenhBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                });

            var queryDVKT = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == yeuCauGoiId)
                .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    YeuCauGoiDichVuId = yeuCauGoiId,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = s.SoLan,
                    DonGia = s.DonGiaSauChietKhau,
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKT,
                    DichVuId = s.DichVuKyThuatBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    IsNhomTiemChung = nhomTiemChungId == null ? false : s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                });

            var queryDVG = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == yeuCauGoiId)
               .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuGiuongBenhVien.Ma, x => x.DichVuGiuongBenhVien.Ten)
               .Select(s => new DichVuGoiMarketingGridVo
               {
                   Id = s.Id,
                   YeuCauGoiDichVuId = yeuCauGoiId,
                   Ma = s.DichVuGiuongBenhVien.Ma,
                   TenDichVu = s.DichVuGiuongBenhVien.Ten,
                   LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                   SoLuong = s.SoLan,
                   DonGia = s.DonGiaSauChietKhau,
                   ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                   GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                   GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                   TenNhomDichVu = tenNhomDVG,
                   DichVuId = s.DichVuGiuongBenhVienId,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                   ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                   TenGoi = s.ChuongTrinhGoiDichVu.GoiDichVu.Ten,
                   NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
               });

            var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.YeuCauDichVuKyThuats)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)
                .Include(p => p.YeuCauKhamBenhs)
                .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.Id == yeuCauGoiId);
            foreach (var item in queryTask.Result)
            {
                //set trang thai thanh toan
                if (yeuCauGoiDichVu != null)
                {
                    item.TrangThaiThanhToan = yeuCauGoiDichVu.TrangThaiThanhToan;

                    if (item.TenNhomDichVu == tenNhomDVKB)
                    {
                        if (yeuCauGoiDichVu.YeuCauKhamBenhs.Any())
                        {
                            item.SoLuongDaDung = yeuCauGoiDichVu.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == item.DichVuId)
                                ? yeuCauGoiDichVu.YeuCauKhamBenhs.Count(p => p.DichVuKhamBenhBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else if (item.TenNhomDichVu == tenNhomDVKT)
                    {
                        if (yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any())
                        {
                            item.SoLuongDaDung = yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == item.DichVuId)
                                ? yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else
                    {
                        // data get từ function tổng hợp rồi, nên ko cần check trạng thái
                        item.SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuBenhVienId == item.DichVuId
                                                                               && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                               && a.NhomGiaDichVuBenhVienId == item.NhomGiaDichVuBenhVienId).Sum(b => b.SoLuongDaSuDung);
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }
                }
                else
                {
                    item.SoLuongDaDung = 0;
                    item.SoLuongDaDungDisplay = "0";
                }

                //check active truoc khi set lst chọn
                if (item.SoLuongDaDung < item.SoLuong && item.NhomDichVu != Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && !item.IsNhomTiemChung)
                {
                    item.IsActive = true;
                }

                
                var itemChon = lstModelChon.FirstOrDefault(p => p.TenNhomDichVu == item.TenNhomDichVu 
                                                                && p.DichVuId == item.DichVuId 
                                                                && p.YeuCauGoiDichVuId == item.YeuCauGoiDichVuId);
                if (itemChon != null)
                {
                    item.SoLuongDungLanNay = itemChon.SoLuongDungLanNay;
                }

                
                if (lstModelChon.Any(p => p.TenNhomDichVu == item.TenNhomDichVu 
                                          && p.DichVuId == item.DichVuId 
                                          && p.YeuCauGoiDichVuId == item.YeuCauGoiDichVuId))
                {
                    item.IsChecked = true;
                }
                
                var itemChon2 = lstModelChonTrongLanYCTNNay
                    .Where(p => p.TenNhomDichVu == item.TenNhomDichVu 
                                && p.DichVuId == item.DichVuId 
                                && p.YeuCauGoiDichVuId == item.YeuCauGoiDichVuId).ToList();
                if (itemChon2.Any())
                {
                    foreach (var i in itemChon2)
                    {
                        item.SoLuongDaDung = item.SoLuongDaDung + i.SoLuongDungLanNay;
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }

                    item.SoLuongConLai = (item.SoLuong - item.SoLuongDaDung < 0) ? 0 : item.SoLuong - item.SoLuongDaDung;
                }
                else
                {
                    item.SoLuongConLai = (item.SoLuong - item.SoLuongDaDung < 0) ? 0 : item.SoLuong - item.SoLuongDaDung;
                }

                item.SoLuongDisplay = item.SoLuong.ApplyNumber();
                item.DonGiaDisplay = item.DonGia.ApplyFormatMoneyVND();
                item.ThanhTienDisplay = (item.DonGia * item.SoLuong).ApplyFormatMoneyVND();

                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber();

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDichVuGoiPageForGridAsync(QueryInfo queryInfo)
        {
            var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();

            long benhNhanId = 0;

            long childId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
                {
                    lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out childId);
            }


            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var queryKhamBenh = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGia,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGia * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKB,

                    DichVuId = s.DichVuKhamBenhBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,

                });

            var queryDVKT = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGia,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGia * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKT,

                    DichVuId = s.DichVuKyThuatBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,
                });

            var queryDVG = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
               .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
               .Select(s => new DichVuGoiMarketingGridVo
               {
                   Id = s.Id,
                   Ma = s.DichVuGiuongBenhVien.Ma,
                   TenDichVu = s.DichVuGiuongBenhVien.Ten,
                   LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                   SoLuong = s.SoLan,
                   //SoLuongDisplay = s.SoLan.ApplyNumber(),
                   DonGia = s.DonGia,
                   //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                   ThanhTien = s.DonGia * s.SoLan,
                   //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                   GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                   GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                   TenNhomDichVu = tenNhomDVG,

                   DichVuId = s.DichVuGiuongBenhVienId,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                   ThuocGoi = s.ChuongTrinhGoiDichVu.Ten

                   //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.Any(p => p.BenhNhanId == benhNhanId) 
                   //? s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan : null,
               });

            var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataDichVuGoiForGridAsyncUpdateView(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            dichVuGiuongDaChiDinhs = dichVuGiuongDaChiDinhs == null ? new List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>() : dichVuGiuongDaChiDinhs;

            //long childId = !string.IsNullOrEmpty(queryInfo.SearchTerms) ? long.Parse(queryInfo.SearchTerms) : 0;

            var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();

            long benhNhanId = 0;

            long childId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
                {
                    lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out childId);
            }


            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var queryKhamBenh = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGiaSauChietKhau,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKB,

                    DichVuId = s.DichVuKhamBenhBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                });

            var queryDVKT = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
                .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = s.SoLan,
                    //SoLuongDisplay = s.SoLan.ApplyNumber(),
                    DonGia = s.DonGiaSauChietKhau,
                    //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKT,

                    DichVuId = s.DichVuKyThuatBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,

                    //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    IsNhomTiemChung = nhomTiemChungId == null ? false : s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                });

            var queryDVG = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
               .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
               .Select(s => new DichVuGoiMarketingGridVo
               {
                   Id = s.Id,
                   Ma = s.DichVuGiuongBenhVien.Ma,
                   TenDichVu = s.DichVuGiuongBenhVien.Ten,
                   LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                   SoLuong = s.SoLan,
                   //SoLuongDisplay = s.SoLan.ApplyNumber(),
                   DonGia = s.DonGiaSauChietKhau,
                   //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
                   ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                   //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
                   GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                   GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                   TenNhomDichVu = tenNhomDVG,

                   DichVuId = s.DichVuGiuongBenhVienId,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

                   ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,

                   //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.Any(p => p.BenhNhanId == benhNhanId) 
                   //? s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan : null,

                   NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
               });

            var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            foreach (var item in queryTask.Result)
            {
                //set trang thai thanh toan
                var yc = _yeuCauGoiDichVuRepository.TableNoTracking.FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (yc != null)
                {
                    item.TrangThaiThanhToan = yc.TrangThaiThanhToan;
                }
                //
                var dichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                        .Include(p => p.YeuCauDichVuKyThuats)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)
                        .Include(p => p.YeuCauKhamBenhs)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                        //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                        .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (dichVu != null)
                {

                    if (item.TenNhomDichVu == tenNhomDVKB)
                    {
                        if (dichVu.YeuCauKhamBenhs.Any())
                        {
                            item.SoLuongDaDung = dichVu.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == item.DichVuId)
                                ? dichVu.YeuCauKhamBenhs.Where(p => p.DichVuKhamBenhBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Count() : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else if (item.TenNhomDichVu == tenNhomDVKT)
                    {
                        if (dichVu.YeuCauDichVuKyThuats.Any())
                        {
                            item.SoLuongDaDung = dichVu.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == item.DichVuId)
                                ? dichVu.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else
                    {
                        //if (dichVu.YeuCauDichVuGiuongBenhViens.Any())
                        //{
                        //    item.SoLuongDaDung = dichVu.YeuCauDichVuGiuongBenhViens.Any(p => p.DichVuGiuongBenhVienId == item.DichVuId) 
                        //        ? dichVu.YeuCauDichVuGiuongBenhViens.Where(p => p.DichVuGiuongBenhVienId == item.DichVuId).Count() : 0;
                        //    item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        //}
                        //else
                        //{
                        //    item.SoLuongDaDung = 0;
                        //    item.SoLuongDaDungDisplay = "0";
                        //}

                        // data get từ function tổng hợp rồi, nên ko cần check trạng thái
                        item.SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuBenhVienId == item.DichVuId
                                                                               && a.YeuCauGoiDichVuId == dichVu.Id
                                                                               && a.NhomGiaDichVuBenhVienId == item.NhomGiaDichVuBenhVienId).Sum(b => b.SoLuongDaSuDung);
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }
                }
                else
                {
                    item.SoLuongDaDung = 0;
                    item.SoLuongDaDungDisplay = "0";
                }

                //check active truoc khi set lst chọn
                if (item.SoLuongDaDung < item.SoLuong && item.NhomDichVu != Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && !item.IsNhomTiemChung)
                {
                    item.IsActive = true;
                }


                //
                var itemChon = lstModelChon.FirstOrDefault(p => p.TenNhomDichVu == item.TenNhomDichVu && p.DichVuId == item.DichVuId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId);
                if (itemChon != null)
                {
                    item.SoLuongDungLanNay = itemChon.SoLuongDungLanNay;
                    //item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    item.SoLuongConLai = item.SoLuong - item.SoLuongDaDung - item.SoLuongDungLanNay;
                }
                else
                {
                    item.SoLuongConLai = item.SoLuong - item.SoLuongDaDung;

                }

                //
                if (lstModelChon.Any(p => p.TenNhomDichVu == item.TenNhomDichVu && p.DichVuId == item.DichVuId && p.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId))
                {
                    item.IsChecked = true;
                }


                item.SoLuongDisplay = item.SoLuong.ApplyNumber();
                item.DonGiaDisplay = item.DonGia.ApplyFormatMoneyVND();
                item.ThanhTienDisplay = (item.DonGia * item.SoLuong).ApplyFormatMoneyVND();

                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber();

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetDataDichVuGoiForGridAsyncUpdateViewVer2(QueryInfo queryInfo, List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);
            ReplaceDisplayValueSortExpression(queryInfo);

            // BVHD-3268: ko cho phép chỉ định dịch vụ tiêm chủng
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            dichVuGiuongDaChiDinhs = dichVuGiuongDaChiDinhs == null ? new List<ChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>() : dichVuGiuongDaChiDinhs;

            var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();

            long benhNhanId = 0;
            long yeuCauGoiId = 0;

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
                {
                    lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
                }
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
                long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out yeuCauGoiId);
            }


            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var queryKhamBenh = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.Id == yeuCauGoiId)
                .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuKhamBenhBenhVien.Ma, x=> x.DichVuKhamBenhBenhVien.Ten)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    YeuCauGoiDichVuId = yeuCauGoiId,
                    Ma = s.DichVuKhamBenhBenhVien.Ma,
                    TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
                    SoLuong = s.SoLan,
                    DonGia = s.DonGiaSauChietKhau,
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKB,
                    DichVuId = s.DichVuKhamBenhBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKhamBenh
                });

            var queryDVKT = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.Id == yeuCauGoiId)
                .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                .Select(s => new DichVuGoiMarketingGridVo
                {
                    Id = s.Id,
                    YeuCauGoiDichVuId = yeuCauGoiId,
                    Ma = s.DichVuKyThuatBenhVien.Ma,
                    TenDichVu = s.DichVuKyThuatBenhVien.Ten,
                    LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
                    NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
                    SoLuong = s.SoLan,
                    DonGia = s.DonGiaSauChietKhau,
                    ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                    GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                    GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                    TenNhomDichVu = tenNhomDVKT,
                    DichVuId = s.DichVuKyThuatBenhVienId,
                    ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                    ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                    NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    IsNhomTiemChung = nhomTiemChungId == null ? false : s.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                });

            var queryDVG = _yeuCauGoiDichVuRepository.TableNoTracking.Where(p => p.Id == yeuCauGoiId)
               .SelectMany(p => p.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs)
               .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.DichVuGiuongBenhVien.Ma, x => x.DichVuGiuongBenhVien.Ten)
               .Select(s => new DichVuGoiMarketingGridVo
               {
                   Id = s.Id,
                   YeuCauGoiDichVuId = yeuCauGoiId,
                   Ma = s.DichVuGiuongBenhVien.Ma,
                   TenDichVu = s.DichVuGiuongBenhVien.Ten,
                   LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
                   NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                   SoLuong = s.SoLan,
                   DonGia = s.DonGiaSauChietKhau,
                   ThanhTien = s.DonGiaSauChietKhau * s.SoLan,
                   GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                   GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                   TenNhomDichVu = tenNhomDVG,
                   DichVuId = s.DichVuGiuongBenhVienId,
                   ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,
                   ThuocGoi = s.ChuongTrinhGoiDichVu.Ten,
                   NhomDichVu = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh
               });

            var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //[pageable]="false"
            //var queryTask = query.Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            var stt = 1;
            var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.YeuCauDichVuKyThuats)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)
                .Include(p => p.YeuCauKhamBenhs)
                .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.Id == yeuCauGoiId);
            foreach (var item in queryTask.Result)
            {
                //set trang thai thanh toan
                if (yeuCauGoiDichVu != null)
                {
                    item.TrangThaiThanhToan = yeuCauGoiDichVu.TrangThaiThanhToan;

                    if (item.TenNhomDichVu == tenNhomDVKB)
                    {
                        if (yeuCauGoiDichVu.YeuCauKhamBenhs.Any())
                        {
                            item.SoLuongDaDung = yeuCauGoiDichVu.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == item.DichVuId)
                                ? yeuCauGoiDichVu.YeuCauKhamBenhs.Count(p => p.DichVuKhamBenhBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else if (item.TenNhomDichVu == tenNhomDVKT)
                    {
                        if (yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any())
                        {
                            item.SoLuongDaDung = yeuCauGoiDichVu.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == item.DichVuId)
                                ? yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == item.DichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan) : 0;
                            item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                        }
                        else
                        {
                            item.SoLuongDaDung = 0;
                            item.SoLuongDaDungDisplay = "0";
                        }
                    }
                    else
                    {
                        // data get từ function tổng hợp rồi, nên ko cần check trạng thái
                        item.SoLuongDaDung = dichVuGiuongDaChiDinhs.Where(a => a.DichVuBenhVienId == item.DichVuId
                                                                               && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                               && a.NhomGiaDichVuBenhVienId == item.NhomGiaDichVuBenhVienId).Sum(b => b.SoLuongDaSuDung);
                        item.SoLuongDaDungDisplay = item.SoLuongDaDung.ApplyNumber();
                    }
                }
                else
                {
                    item.SoLuongDaDung = 0;
                    item.SoLuongDaDungDisplay = "0";
                }

                //check active truoc khi set lst chọn
                if (item.SoLuongDaDung < item.SoLuong && item.NhomDichVu != Enums.EnumNhomGoiDichVu.DichVuGiuongBenh && !item.IsNhomTiemChung)
                {
                    item.IsActive = true;
                }

                var itemChon = lstModelChon.FirstOrDefault(p => p.TenNhomDichVu == item.TenNhomDichVu 
                                                                && p.DichVuId == item.DichVuId 
                                                                && p.YeuCauGoiDichVuId == item.YeuCauGoiDichVuId);
                if (itemChon != null)
                {
                    item.SoLuongDungLanNay = itemChon.SoLuongDungLanNay;

                    //BVHD-3770: đóng dòng code này lại vì dev cũ code bug
                    //item.SoLuongConLai = item.SoLuong - item.SoLuongDaDung - item.SoLuongDungLanNay;
                }
                else
                {
                    item.SoLuongConLai = item.SoLuong - item.SoLuongDaDung;
                }
                
                if (lstModelChon.Any(p => p.TenNhomDichVu == item.TenNhomDichVu 
                                          && p.DichVuId == item.DichVuId 
                                          && p.YeuCauGoiDichVuId == item.YeuCauGoiDichVuId))
                {
                    item.IsChecked = true;
                }


                item.SoLuongDisplay = item.SoLuong.ApplyNumber();
                item.DonGiaDisplay = item.DonGia.ApplyFormatMoneyVND();
                item.ThanhTienDisplay = (item.DonGia * item.SoLuong).ApplyFormatMoneyVND();

                item.GiaSauChietKhauDisplay = item.GiaSauChietKhau.ApplyFormatMoneyVND();
                item.GiaTruocChietKhauDisplay = item.GiaTruocChietKhau.ApplyFormatMoneyVND();
                item.TiLeChietKhauDisplay = item.TiLeChietKhau.ApplyNumber();

                item.STT = stt;
                stt++;
            }

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalDichVuGoiPageForGridAsyncUpdateView(QueryInfo queryInfo)
        {
            //[pageable]= "false"
            return new GridDataSource { TotalRowCount = 0 };

            //var lstModelChon = new List<DanhSachDichVuChonTrongLanPopup>();

            //long benhNhanId = 0;

            //long childId = 0;

            //if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            //{
            //    if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString.Split("|")[2]) && queryInfo.AdditionalSearchString.Split("|")[2] != "")
            //    {
            //        lstModelChon = JsonConvert.DeserializeObject<List<DanhSachDichVuChonTrongLanPopup>>(queryInfo.AdditionalSearchString.Split("|")[2]);
            //    }
            //    long.TryParse(queryInfo.AdditionalSearchString.Split("|")[0], out benhNhanId);
            //    long.TryParse(queryInfo.AdditionalSearchString.Split("|")[1], out childId);
            //}


            //var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            //var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            //var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            //var queryKhamBenh = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
            //    .SelectMany(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
            //    .Select(s => new DichVuGoiMarketingGridVo
            //    {
            //        Id = s.Id,
            //        Ma = s.DichVuKhamBenhBenhVien.Ma,
            //        TenDichVu = s.DichVuKhamBenhBenhVien.Ten,
            //        LoaiGiaDisplay = s.NhomGiaDichVuKhamBenhBenhVien.Ten,
            //        NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKhamBenhBenhVienId,
            //        SoLuong = s.SoLan,
            //        //SoLuongDisplay = s.SoLan.ApplyNumber(),
            //        DonGia = s.DonGia,
            //        //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
            //        ThanhTien = s.DonGia * s.SoLan,
            //        //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
            //        GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
            //        GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
            //        TenNhomDichVu = tenNhomDVKB,

            //        DichVuId = s.DichVuKhamBenhBenhVienId,
            //        ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

            //        //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,

            //    });

            //var queryDVKT = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
            //    .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
            //    .Select(s => new DichVuGoiMarketingGridVo
            //    {
            //        Id = s.Id,
            //        Ma = s.DichVuKyThuatBenhVien.Ma,
            //        TenDichVu = s.DichVuKyThuatBenhVien.Ten,
            //        LoaiGiaDisplay = s.NhomGiaDichVuKyThuatBenhVien.Ten,
            //        NhomGiaDichVuBenhVienId = s.NhomGiaDichVuKyThuatBenhVienId,
            //        SoLuong = s.SoLan,
            //        //SoLuongDisplay = s.SoLan.ApplyNumber(),
            //        DonGia = s.DonGia,
            //        //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
            //        ThanhTien = s.DonGia * s.SoLan,
            //        //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
            //        GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
            //        GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
            //        TenNhomDichVu = tenNhomDVKT,

            //        DichVuId = s.DichVuKyThuatBenhVienId,
            //        ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

            //        //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan,
            //    });

            //var queryDVG = _chuongTrinhGoiDichVuRepository.TableNoTracking.Where(p => p.Id == childId)
            //   .SelectMany(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
            //   .Select(s => new DichVuGoiMarketingGridVo
            //   {
            //       Id = s.Id,
            //       Ma = s.DichVuGiuongBenhVien.Ma,
            //       TenDichVu = s.DichVuGiuongBenhVien.Ten,
            //       LoaiGiaDisplay = s.NhomGiaDichVuGiuongBenhVien.Ten,
            //       NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
            //       SoLuong = s.SoLan,
            //       //SoLuongDisplay = s.SoLan.ApplyNumber(),
            //       DonGia = s.DonGia,
            //       //DonGiaDisplay = s.DonGia.ApplyFormatMoneyVND(),
            //       ThanhTien = s.DonGia * s.SoLan,
            //       //ThanhTienDisplay = (s.DonGia * s.SoLan).ApplyFormatMoneyVND(),
            //       GiaTruocChietKhau = s.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
            //       GiaSauChietKhau = s.ChuongTrinhGoiDichVu.GiaSauChietKhau,
            //       TenNhomDichVu = tenNhomDVG,

            //       DichVuId = s.DichVuGiuongBenhVienId,
            //       ChuongTrinhGoiDichVuId = s.ChuongTrinhGoiDichVuId,

            //       ThuocGoi = s.ChuongTrinhGoiDichVu.Ten

            //       //TrangThaiThanhToan = s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.Any(p => p.BenhNhanId == benhNhanId) 
            //       //? s.ChuongTrinhGoiDichVu.YeuCauGoiDichVus.First(p => p.BenhNhanId == benhNhanId).TrangThaiThanhToan : null,
            //   });

            //var query = queryDVG.Concat(queryKhamBenh).Concat(queryDVKT);
            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            //return new GridDataSource { TotalRowCount = countTask.Result };
        }



        public async Task<List<ChiDinhDichVuGridVo>> GetDataForGoiGridAsync(List<ThemDichVuKhamBenhVo> lstModel)
        {
            var result = new List<ChiDinhDichVuGridVo>();

            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            foreach (var model in lstModel)
            {
                var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                .FirstOrDefaultAsync(p => p.Id == model.YeuCauGoiDichVuId);

                if (yeuCauGoiDichVu != null)
                {
                    if (model.TenNhomDichVu == tenNhomDVG)
                    {
                        var dichVuKhamGiuongBenhVienGiaBaoHiem =
                                   await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                       .FirstOrDefaultAsync(p =>
                                           p.DichVuGiuongBenhVienId == model.MaDichVuId
                                           && p.TuNgay.Date <= DateTime.Now.Date
                                           && (p.DenNgay == null ||
                                               (p.DenNgay != null && p.DenNgay.GetValueOrDefault().Date >= DateTime.Now.Date)));

                        //var donGiaEntity = ctGoiDV.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == model.MaDichVuId)?
                        //        .DichVuGiuongBenhVien?.DichVuGiuongBenhVienGiaBenhViens?
                        //    .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                        //                         && (p.DenNgay == null ||
                        //                             (p.DenNgay != null && p.DenNgay.GetValueOrDefault().Date >=
                        //                              DateTime.Now.Date)));

                        var child = new ChiDinhDichVuGridVo
                        {
                            MaDichVuId = model.MaDichVuId ?? 0,
                            Ma = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                ? yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).DichVuGiuongBenhVien.Ma : "",
                            TenDichVu = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                ? yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).DichVuGiuongBenhVien.Ten : "",
                            LoaiGia = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                            && yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).NhomGiaDichVuGiuongBenhVien != null
                                ? yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).NhomGiaDichVuGiuongBenhVien.Ten : "",
                            LoaiGiaId = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                            && yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).NhomGiaDichVuGiuongBenhVien != null
                                ? yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).NhomGiaDichVuGiuongBenhVien.Id : 0,
                            SoLuong = model.SoLuong,

                            DonGiaDisplay = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                ? yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).DonGia.ApplyVietnameseFloatNumber() : "0",
                            DonGia = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(o => o.DichVuGiuongBenhVienId == model.MaDichVuId)
                                ? (double)yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.First(o => o.DichVuGiuongBenhVienId == model.MaDichVuId).DonGia : 0,

                            Nhom = Constants.NhomDichVu.DichVuGiuong,

                            //goi co chiet khau
                            IsGoiCoChietKhau = true,
                            GoiCoChietKhauId = yeuCauGoiDichVu.Id,
                            TenGoiChietKhau = yeuCauGoiDichVu.TenChuongTrinh,
                            IsDichVuTrongGoi = true,
                            GoiCoChietKhauIdTemp = yeuCauGoiDichVu.Id,
                            //update goi dv 10/21
                            //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                            DuocHuongBHYT = model.DuocHuongBHYT,
                        };

                        child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                        child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();
                        child.BHYTThanhToanDisplay = "0";

                        child.BHYTThanhToan = 0;
                        child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                        child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100;
                        child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                        child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan - child.SoTienMG;
                        child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                        child.IsHaveNoiThucHien = true;

                        //
                        child.GiaBHYT = (double)(dichVuKhamGiuongBenhVienGiaBaoHiem?.Gia ?? 0);
                        child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                        child.TiLeBaoHiemThanhToan = (dichVuKhamGiuongBenhVienGiaBaoHiem
                                                                   ?.TiLeBaoHiemThanhToan ?? 0);

                        if (child.GiaBHYT == 0)
                        {
                            child.DuocHuongBHYT = false;
                        }

                        result.Add(child);

                    }
                    else if (model.TenNhomDichVu == tenNhomDVKT)
                    {
                        var item = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == model.MaDichVuId);

                        var dichVuKyThuatBenhVienGiaBaoHiem =
                                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                            .FirstOrDefaultAsync(p =>
                                                p.DichVuKyThuatBenhVienId == model.MaDichVuId
                                                && p.TuNgay.Date <= DateTime.Now.Date
                                                && (p.DenNgay == null ||
                                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                        //var donGiaEntity = item.DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens
                        //    .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                        //                         && (p.DenNgay == null ||
                        //                             (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                        //                              DateTime.Now.Date)));

                        var child = new ChiDinhDichVuGridVo
                        {
                            MaDichVuId = item.DichVuKyThuatBenhVienId,
                            Ma = item.DichVuKyThuatBenhVien?.Ma ?? "",
                            TenDichVu = item.DichVuKyThuatBenhVien?.Ten ?? "",
                            LoaiGia = item.NhomGiaDichVuKyThuatBenhVien?.Ten ?? "",
                            LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                            SoLuong = model.SoLuong,

                            DonGiaDisplay = item.DonGia.ApplyVietnameseFloatNumber(),
                            DonGiaSauChietKhau = (double)item.DonGiaSauChietKhau,
                            DonGia = (double)item.DonGia,


                            Nhom = Constants.NhomDichVu.DichVuKyThuat,

                            //goi co chiet khau
                            IsGoiCoChietKhau = true,
                            GoiCoChietKhauId = yeuCauGoiDichVu.Id,
                            TenGoiChietKhau = yeuCauGoiDichVu.TenChuongTrinh,
                            IsDichVuTrongGoi = true,
                            GoiCoChietKhauIdTemp = yeuCauGoiDichVu.Id,
                            //update goi dv 10/21
                            //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                            DuocHuongBHYT = model.DuocHuongBHYT,
                        };
                        child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                        child.ThanhTienSauChietKhau = (child.DonGiaSauChietKhau ?? 0) * child.SoLuong ?? 0;
                        child.ThanhTienDisplay = child.DonGiaDisplay;
                        child.BHYTThanhToanDisplay = "0";

                        child.BHYTThanhToan = 0;
                        child.BHYTThanhToanChuaBaoGomMucHuong = 0;

                        //child.SoTienMG = (child.ThanhTien - (double)child.BHYTThanhToan) * child.TLMG / 100; 
                        //child.SoTienMGDisplay = child.SoTienMG.ApplyNumber();
                        child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                        child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                        child.IsHaveNoiThucHien = true;

                        //
                        child.GiaBHYT = (double)(dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0);
                        child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                        child.TiLeBaoHiemThanhToan = (dichVuKyThuatBenhVienGiaBaoHiem
                                                                   ?.TiLeBaoHiemThanhToan ?? 0);

                        if (child.GiaBHYT == 0)
                        {
                            child.DuocHuongBHYT = false;
                        }


                        result.Add(child);
                    }
                    else if (model.TenNhomDichVu == tenNhomDVKB)
                    {
                        var item = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId);

                        var dichVuKhamBenhBenhVienGiaBaoHiem =
                                    await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                        .FirstOrDefaultAsync(p =>
                                            p.DichVuKhamBenhBenhVienId == model.MaDichVuId
                                            && p.TuNgay.Date <= DateTime.Now.Date
                                            && (p.DenNgay == null ||
                                                (p.DenNgay != null && p.DenNgay.GetValueOrDefault().Date >= DateTime.Now.Date)));
                        //var donGiaEntity = ctGoiDV.ChuongTrinhGoiDichVuDichKhamBenhs
                        //    .FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == model.MaDichVuId)?
                        //    .DichVuKhamBenhBenhVien?.DichVuKhamBenhBenhVienGiaBenhViens?
                        //    .FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date
                        //                         && (p.DenNgay == null ||
                        //                             (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >=
                        //                              DateTime.Now.Date)));

                        var bhytThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                            (dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1 *
                                            (model.BHYTMucHuong ?? 100) / 100;
                        var bhytThanhToanChuaBaoGomMucHuong = (dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0) *
                                                              (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                   ?.TiLeBaoHiemThanhToan ?? 0) / 100 * 1;


                        var child = new ChiDinhDichVuGridVo
                        {
                            MaDichVuId = item.DichVuKhamBenhBenhVienId,
                            Ma = item.DichVuKhamBenhBenhVien?.Ma ?? "",
                            TenDichVu = item.DichVuKhamBenhBenhVien?.Ten ?? "",
                            LoaiGia = item.NhomGiaDichVuKhamBenhBenhVien?.Ten ?? "",
                            LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                            SoLuong = 1,
                            //DonGiaDisplay = (donGiaEntity != null ? (double)donGiaEntity.Gia : 0).ApplyNumber(),
                            //DonGia = donGiaEntity != null ? (double)donGiaEntity.Gia : 0,

                            DonGiaDisplay = item.DonGiaSauChietKhau.ApplyVietnameseFloatNumber(),
                            DonGiaSauChietKhau = (double)item.DonGiaSauChietKhau,
                            DonGia = (double)item.DonGia,

                            Nhom = Constants.NhomDichVu.DichVuKhamBenh,
                            //goi co chiet khau
                            IsGoiCoChietKhau = true,
                            GoiCoChietKhauId = yeuCauGoiDichVu.Id,
                            TenGoiChietKhau = yeuCauGoiDichVu.TenChuongTrinh,
                            IsDichVuTrongGoi = true,
                            GoiCoChietKhauIdTemp = yeuCauGoiDichVu.Id,
                            //update goi dv 10/21
                            //TongChiPhiGoi = goiDichVu.ChiPhiGoiDichVu ?? 0,
                            DuocHuongBHYT = model.DuocHuongBHYT,

                        };
                        child.ThanhTien = child.DonGia * child.SoLuong ?? 0;
                        child.ThanhTienSauChietKhau = (child.DonGiaSauChietKhau ?? 0) * child.SoLuong ?? 0;
                        child.ThanhTienDisplay = child.ThanhTien.ApplyNumber();

                        child.BHYTThanhToan = bhytThanhToan;
                        child.BHYTThanhToanDisplay = bhytThanhToan.ApplyVietnameseFloatNumber();
                        child.BHYTThanhToanChuaBaoGomMucHuong = bhytThanhToanChuaBaoGomMucHuong;

                        child.GiaBHYT = (double)(dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0);
                        child.GiaBHYTDislay = child.GiaBHYT.ApplyNumber();
                        child.TiLeBaoHiemThanhToan = (dichVuKhamBenhBenhVienGiaBaoHiem
                                                                   ?.TiLeBaoHiemThanhToan ?? 0);
                        child.BnThanhToan = child.ThanhTien - (double)child.BHYTThanhToan;
                        child.BnThanhToanDisplay = child.BnThanhToan.ApplyNumber();
                        child.IsHaveNoiThucHien = true;

                        if (child.GiaBHYT == 0)
                        {
                            child.DuocHuongBHYT = false;
                        }

                        result.Add(child);
                    }
                }
            }

            return result;
        }

        public async Task<bool> isValidateSoLuongTonTrongGoi(List<ChiDinhDichVuGridVo> lstDichVuThem, long benhNhanId)
        {
            var result = true;

            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            foreach (var i in lstDichVuThem)
            {
                if (i.IsGoiCoChietKhau)
                {
                    var yeuCauGoiDichVu = _yeuCauGoiDichVuRepository.TableNoTracking
                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                        .Include(p => p.YeuCauKhamBenhs)

                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                        .Include(p => p.YeuCauDichVuKyThuats)

                        .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)

                        .FirstOrDefault(p => p.Id == i.GoiCoChietKhauId);
                    if (yeuCauGoiDichVu != null)
                    {
                        if (i.Nhom == tenNhomDVKB)
                        {
                            var tongSoLuongGoi = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == i.MaDichVuId)?.SoLan ?? 0;

                            //var ycGoiDV = _yeuCauGoiDichVuRepository.TableNoTracking
                            //    .Include(p => p.YeuCauKhamBenhs)
                            //    .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == i.GoiCoChietKhauId);
                            //var soLuongDaDungDB = ycGoiDV != null
                            //        ? (ycGoiDV.YeuCauKhamBenhs.Any(p => p.DichVuKhamBenhBenhVienId == i.MaDichVuId) 
                            //        ? ycGoiDV.YeuCauKhamBenhs.Where(p => p.DichVuKhamBenhBenhVienId == i.MaDichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Count() : 0) : 0 ;
                            var soLuongDaDungDB = yeuCauGoiDichVu.YeuCauKhamBenhs.Count(p => p.DichVuKhamBenhBenhVienId == i.MaDichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);

                            var soLuongDaDungUI = lstDichVuThem.Where(p => p.IsGoiCoChietKhau
                                                                           && p.GoiCoChietKhauId == yeuCauGoiDichVu.Id 
                                                                           && p.Nhom == tenNhomDVKB 
                                                                           && p.MaDichVuId == i.MaDichVuId)
                                                    .Sum(p => p.SoLuong);

                            if (tongSoLuongGoi < soLuongDaDungDB + soLuongDaDungUI)
                            {
                                return false;
                            }

                        }
                        else if (i.Nhom == tenNhomDVKT)
                        {
                            var tongSoLuongGoi = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == i.MaDichVuId)?.SoLan ?? 0;

                            //var ycGoiDV = _yeuCauGoiDichVuRepository.TableNoTracking
                            //    .Include(p => p.YeuCauDichVuKyThuats)
                            //    .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == i.GoiCoChietKhauId);
                            //var soLuongDaDungDB = ycGoiDV != null
                            //        ? (ycGoiDV.YeuCauDichVuKyThuats.Any(p => p.DichVuKyThuatBenhVienId == i.MaDichVuId) 
                            //        ? ycGoiDV.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == i.MaDichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan) : 0) : 0;
                            var soLuongDaDungDB = yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(p => p.DichVuKyThuatBenhVienId == i.MaDichVuId && p.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Sum(p => p.SoLan);

                            var soLuongDaDungUI = lstDichVuThem.Where(p => p.IsGoiCoChietKhau 
                                                                           && p.GoiCoChietKhauId == yeuCauGoiDichVu.Id 
                                                                           && p.Nhom == tenNhomDVKT 
                                                                           && p.MaDichVuId == i.MaDichVuId)
                                                    .Sum(p => p.SoLuong);

                            if (tongSoLuongGoi < soLuongDaDungDB + soLuongDaDungUI)
                            {
                                return false;
                            }
                        }
                        else if (i.Nhom == tenNhomDVG) // tạm thời ko chỉ định dv giường ở ngoại trú
                        {
                            var tongSoLuongGoi = yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == i.MaDichVuId)?.SoLan ?? 0;

                            //var ycGoiDV = _yeuCauGoiDichVuRepository.TableNoTracking
                            //    .Include(p => p.YeuCauDichVuGiuongBenhViens)
                            //    .FirstOrDefault(p => p.BenhNhanId == benhNhanId && p.ChuongTrinhGoiDichVuId == i.GoiCoChietKhauId);
                            //var soLuongDaDungDB = ycGoiDV != null
                            //        ? (ycGoiDV.YeuCauDichVuGiuongBenhViens.Any(p => p.DichVuGiuongBenhVienId == i.MaDichVuId) 
                            //        ? ycGoiDV.YeuCauDichVuGiuongBenhViens.Where(p => p.DichVuGiuongBenhVienId == i.MaDichVuId).Count() : 0) : 0;
                            var soLuongDaDungDB = yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Count(p => p.DichVuGiuongBenhVienId == i.MaDichVuId && p.TrangThai != Enums.EnumTrangThaiGiuongBenh.DaHuy);

                            var soLuongDaDungUI = lstDichVuThem.Where(p => p.IsGoiCoChietKhau 
                                                                           && p.GoiCoChietKhauId == yeuCauGoiDichVu.Id 
                                                                           && p.Nhom == tenNhomDVG 
                                                                           && p.MaDichVuId == i.MaDichVuId)
                                                    .Sum(p => p.SoLuong);

                            if (tongSoLuongGoi < soLuongDaDungDB + soLuongDaDungUI)
                            {
                                return false;
                            }
                        }
                    }
                }
            }    

            return result;
        }

        public async Task<bool> CheckSoLuongTonTrongGoiLstDichVu(CheckDuSoLuongTonTrongGoiListDichVu model)
        {
            var result = true;

            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var lstDichVuThem = model.LstDichVuThem;

            #region Cập nhật 20/12/2022
            var lstBenhNhanId = lstDichVuThem.Where(x => x.BenhNhanId != null).Select(x => x.BenhNhanId).Distinct().ToList();
            var lstGoiDV = _yeuCauGoiDichVuRepository.TableNoTracking
                            .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                            .Include(p => p.YeuCauKhamBenhs)

                            .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                            .Include(p => p.YeuCauDichVuKyThuats)

                            .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                            .Include(p => p.YeuCauDichVuGiuongBenhViens)

                            .Where(p => lstBenhNhanId.Contains(p.BenhNhanId)).ToList();
            #endregion

            foreach (var dichVuThem in lstDichVuThem)
            {
                #region Cập nhật 20/12/2022
                //var lstGoiDVOfBenhNhan = _yeuCauGoiDichVuRepository.TableNoTracking
                //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                //.Include(p => p.YeuCauKhamBenhs)

                //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                //.Include(p => p.YeuCauDichVuKyThuats)

                //.Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                //.Include(p => p.YeuCauDichVuGiuongBenhViens)

                //.Where(p => p.BenhNhanId == dichVuThem.BenhNhanId).ToList();

                var lstGoiDVOfBenhNhan = lstGoiDV.Where(x => x.BenhNhanId == dichVuThem.BenhNhanId).ToList();
                #endregion

                //var soLuongCuaDichVuTrongDSDaThem = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.)
                if (dichVuThem.TenNhomDichVu == tenNhomDVKB)
                {
                    foreach (var yc in lstGoiDVOfBenhNhan)
                    {
                        //if (yc.ChuongTrinhGoiDichVuId != dichVuThem.ChuongTrinhGoiDichVuId) continue;
                        if (yc.Id != dichVuThem.YeuCauTiepNhanId) continue;

                        var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuThem.MaDichVuId);

                        if (ct != null)
                        {
                            var soLuongDaDungDB = yc.YeuCauKhamBenhs.Where(p => p.Id == dichVuThem.MaDichVuId).Count();
                            var tongSoLuongGoi = ct.SoLan;

                            if (tongSoLuongGoi >= soLuongDaDungDB + dichVuThem.SoLuong)
                            {
                                //return Ok
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
                else if (dichVuThem.TenNhomDichVu == tenNhomDVKT)
                {
                    foreach (var yc in lstGoiDVOfBenhNhan)
                    {
                        //if (yc.ChuongTrinhGoiDichVuId != dichVuThem.ChuongTrinhGoiDichVuId) continue;
                        if (yc.Id != dichVuThem.YeuCauTiepNhanId) continue;

                        var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuThem.MaDichVuId);

                        if (ct != null)
                        {
                            var soLuongDaDungDB = yc.YeuCauDichVuKyThuats.Where(p => p.Id == dichVuThem.MaDichVuId).Sum(p => p.SoLan);
                            var tongSoLuongGoi = ct.SoLan;

                            if (tongSoLuongGoi >= soLuongDaDungDB + dichVuThem.SoLuong)
                            {
                                //return Ok
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
                else if (dichVuThem.TenNhomDichVu == tenNhomDVG)
                {
                    foreach (var yc in lstGoiDVOfBenhNhan)
                    {
                        //if (yc.ChuongTrinhGoiDichVuId != dichVuThem.ChuongTrinhGoiDichVuId) continue;
                        if (yc.Id != dichVuThem.YeuCauTiepNhanId) continue;

                        var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuThem.MaDichVuId);

                        if (ct != null)
                        {
                            var soLuongDaDungDB = yc.YeuCauDichVuGiuongBenhViens.Where(p => p.Id == dichVuThem.MaDichVuId).Count();
                            var tongSoLuongGoi = ct.SoLan;

                            if (tongSoLuongGoi >= soLuongDaDungDB + dichVuThem.SoLuong)
                            {
                                //return Ok
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<DichVuTrongGoiKhiThem> CheckSoLuongTonTrongGoiForCreate(CheckDuSoLuongTonTrongGoi model)
        {
            var result = new DichVuTrongGoiKhiThem();

            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var dichVuThem = model.DichVuThem;
            var danhSachDaThem = model.DanhSachDichVuChonTrongLanPopup;

            var lstGoiDVOfBenhNhan = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauKhamBenhs)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(p => p.YeuCauDichVuKyThuats)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)

                .Where(p => p.BenhNhanId == dichVuThem.BenhNhanId).ToList();

            var ctOnYC = lstGoiDVOfBenhNhan.Select(p => p.ChuongTrinhGoiDichVuId).Distinct().ToList();

            if (dichVuThem.TenNhomDichVu == tenNhomDVKB)
            {
                foreach (var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauKhamBenhs.Count();
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuKhamBenhBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }
            else if (dichVuThem.TenNhomDichVu == tenNhomDVKT)
            {
                foreach (var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauKhamBenhs.Count();
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuKyThuatBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }
            else if (dichVuThem.TenNhomDichVu == tenNhomDVG)
            {
                foreach (var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauKhamBenhs.Count();
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuGiuongBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }

            //check ngoai yc da them
            var lstChuongTrinhDaThem = danhSachDaThem.Where(p => !ctOnYC.Contains(p.ChuongTrinhGoiDichVuId)).Select(p => p.ChuongTrinhGoiDichVuId).Distinct().ToList();
            foreach (var ctId in lstChuongTrinhDaThem)
            {
                if (dichVuThem.TenNhomDichVu == tenNhomDVKB)
                {
                    var ctyc = _chuongTrinhGoiDichVuRepository.TableNoTracking
                        .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                        .FirstOrDefault(p => p.Id == ctId);
                    var ct = ctyc != null ? ctyc.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuThem.MaDichVuId) : null;
                    if (ct != null)
                    {
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.ChuongTrinhGoiDichVuId == ctId).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = ctId;
                            result.IsFromMarketing = false;
                            result.TenChuongTrinh = ctyc.Ten;
                            result.TenDichVu = ct.DichVuKhamBenhBenhVien.Ten;
                            //result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
                else if (dichVuThem.TenNhomDichVu == tenNhomDVKT)
                {
                    var ctyc = _chuongTrinhGoiDichVuRepository.TableNoTracking
                        .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                        .FirstOrDefault(p => p.Id == ctId);

                    var ct = ctyc != null ? ctyc.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuThem.MaDichVuId) : null;

                    if (ct != null)
                    {
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.ChuongTrinhGoiDichVuId == ctId).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = ctId;
                            result.IsFromMarketing = false;
                            result.TenChuongTrinh = ctyc.Ten;
                            result.TenDichVu = ct.DichVuKyThuatBenhVien.Ten;
                            //result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
                else if (dichVuThem.TenNhomDichVu == tenNhomDVG)
                {
                    foreach (var yc in lstGoiDVOfBenhNhan)
                    {
                        var ctyc = _chuongTrinhGoiDichVuRepository.TableNoTracking
                       .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                       .FirstOrDefault(p => p.Id == ctId);

                        var ct = ctyc != null ? ctyc.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuThem.MaDichVuId) : null;

                        if (ct != null)
                        {
                            var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.ChuongTrinhGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                            var tongSoLuongGoi = ct.SoLan;

                            if (tongSoLuongGoi >= soLuongDaDungUI + dichVuThem.SoLuong)
                            {
                                //return Ok
                                result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                                result.ChuongTrinhGoiDichVuId = ctId;
                                result.IsFromMarketing = false;
                                result.TenChuongTrinh = ctyc.Ten;
                                result.TenDichVu = ct.DichVuGiuongBenhVien.Ten;
                                //result.YeuCauGoiDichVuId = yc.Id;

                                return result;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<DichVuTrongGoiKhiThem> CheckSoLuongTonTrongGoi(CheckDuSoLuongTonTrongGoi model)
        {
            var result = new DichVuTrongGoiKhiThem();

            var tenNhomDVKB = Constants.NhomDichVu.DichVuKhamBenh;
            var tenNhomDVKT = Constants.NhomDichVu.DichVuKyThuat;
            var tenNhomDVG = Constants.NhomDichVu.DichVuGiuong;

            var dichVuThem = model.DichVuThem;
            var danhSachDaThem = model.DanhSachDichVuChonTrongLanPopup;

            var lstGoiDVOfBenhNhan = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                .Include(p => p.YeuCauKhamBenhs)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                .Include(p => p.YeuCauDichVuKyThuats)

                .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                .Include(p => p.YeuCauDichVuGiuongBenhViens)

                .Where(p => p.BenhNhanId == dichVuThem.BenhNhanId).ToList();

            //var soLuongCuaDichVuTrongDSDaThem = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.)
            if (dichVuThem.TenNhomDichVu == tenNhomDVKB)
            {
                foreach(var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauKhamBenhs.Where(p => p.Id == dichVuThem.MaDichVuId).Count();
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuKhamBenhBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }
            else if (dichVuThem.TenNhomDichVu == tenNhomDVKT)
            {
                foreach (var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauDichVuKyThuats.Where(p => p.Id == dichVuThem.MaDichVuId).Sum(p => p.SoLan);
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuKyThuatBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }
            else if (dichVuThem.TenNhomDichVu == tenNhomDVG)
            {
                foreach (var yc in lstGoiDVOfBenhNhan)
                {
                    var ct = yc.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuThem.MaDichVuId);

                    if (ct != null)
                    {
                        var soLuongDaDungDB = yc.YeuCauDichVuGiuongBenhViens.Where(p => p.Id == dichVuThem.MaDichVuId).Count();
                        var soLuongDaDungUI = danhSachDaThem.Where(p => p.DichVuId == dichVuThem.MaDichVuId && p.YeuCauGoiDichVuId == yc.Id).Sum(p => p.SoLuongDungLanNay);
                        var tongSoLuongGoi = ct.SoLan;

                        if (tongSoLuongGoi >= soLuongDaDungDB + soLuongDaDungUI + dichVuThem.SoLuong)
                        {
                            //return Ok
                            result.BenhNhanId = dichVuThem.BenhNhanId ?? 0;
                            result.ChuongTrinhGoiDichVuId = yc.ChuongTrinhGoiDichVuId;
                            result.IsFromMarketing = yc.BoPhanMarketingDangKy ?? false;
                            result.TenChuongTrinh = yc.TenChuongTrinh;
                            result.TenDichVu = ct.DichVuGiuongBenhVien.Ten;
                            result.YeuCauGoiDichVuId = yc.Id;

                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<string> GetTenChuongTrinhGoiDichVu(long yeuCauGoiDichVuId, bool? laDichVuKhuyenMai = false)
        {
            if (yeuCauGoiDichVuId == 0) return null;

            #region code cũ
            //var entity = await _yeuCauGoiDichVuRepository.TableNoTracking
            //    .Include(p => p.ChuongTrinhGoiDichVu)
            //    .FirstOrDefaultAsync(p => p.Id == yeuCauGoiDichVuId);
            //return entity?.ChuongTrinhGoiDichVu.Ten + (entity != null && laDichVuKhuyenMai == true ? " (dịch vụ khuyến mãi)" : "");
            #endregion

            #region Cập nhật 19/12/2022 giảm thời gian load
            var tenChuongTrinh = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(p => p.Id == yeuCauGoiDichVuId)
                .Select(x => x.ChuongTrinhGoiDichVu.Ten)
                .FirstOrDefault();

            return tenChuongTrinh + (!string.IsNullOrEmpty(tenChuongTrinh) && laDichVuKhuyenMai == true ? " (dịch vụ khuyến mãi)" : "");
            #endregion
        }

        public async Task<decimal?> GetDonGiaDichVuGoi(long yeuCauGoiDichVuId, long dichVuId, bool khamBenh = false, bool kyThuat = false, bool giuongBenh = false)
        {
            if (yeuCauGoiDichVuId == 0) return null;

            #region code cũ
            //var entity = await _yeuCauGoiDichVuRepository.TableNoTracking
            //    .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
            //    .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
            //    .Include(p => p.ChuongTrinhGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
            //    .FirstOrDefaultAsync(p => p.Id == yeuCauGoiDichVuId);

            //if (entity != null)
            //{
            //    if (khamBenh)
            //    {
            //        return entity.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dichVuId)?.DonGiaSauChietKhau ?? 0;
            //    }
            //    else if (kyThuat)
            //    {
            //        return entity.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dichVuId)?.DonGiaSauChietKhau ?? 0;
            //    }
            //    else if (giuongBenh)
            //    {
            //        return entity.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dichVuId)?.DonGiaSauChietKhau ?? 0;
            //    }
            //}

            //return 0;
            #endregion

            #region cập nhật 19/12/2022 giảm thời gian load
            decimal donGiaSauChieuKhau = 0;
            var chuongTrinhGoiId = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.Id == yeuCauGoiDichVuId)
                    .Select(x => x.ChuongTrinhGoiDichVuId)
                    .FirstOrDefault();
            if (chuongTrinhGoiId != 0)
            {
                if (khamBenh)
                {
                    donGiaSauChieuKhau = _chuongTrinhGoiDichVuDichVuKhamBenhRepository.TableNoTracking
                                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiId 
                                                    && p.DichVuKhamBenhBenhVienId == dichVuId)
                                        .Select(x => x.DonGiaSauChietKhau).FirstOrDefault();
                }
                else if (kyThuat)
                {
                    donGiaSauChieuKhau = _chuongTrinhGoiDichVuDichVuKyThuatRepository.TableNoTracking
                                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiId
                                                    && p.DichVuKyThuatBenhVienId == dichVuId)
                                        .Select(x => x.DonGiaSauChietKhau).FirstOrDefault();
                }
                else if (giuongBenh)
                {
                    donGiaSauChieuKhau = _chuongTrinhGoiDichVuDichVuGiuongRepository.TableNoTracking
                                        .Where(p => p.ChuongTrinhGoiDichVuId == chuongTrinhGoiId
                                                    && p.DichVuGiuongBenhVienId == dichVuId)
                                        .Select(x => x.DonGiaSauChietKhau).FirstOrDefault();
                }
            }
            return donGiaSauChieuKhau;
            #endregion
        }

        public async Task<YeuCauTiepNhan> ThemDichVuFromGoiUpdateView(List<ThemDichVuKhamBenhVo> model, long yeuCauTiepNhanId, long benhNhanid, int? mucHuongBHYT)
        {
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var locationId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var lstCTGoiDVAdded = new List<YeuCauGoiDichVu>();

            #region Cập nhật 20/12/2022
            //var entity = await GetByIdHaveInclude(yeuCauTiepNhanId);
            var entity = await GetByIdHaveIncludeForAdddichVu(yeuCauTiepNhanId);
            #endregion

            if (entity != null)
            {
                #region Cập nhật 20/12/2022
                var lstChuongTrinhId = model.Where(x => x.ChuongTrinhGoiDichVuId != null).Select(x => x.ChuongTrinhGoiDichVuId).Distinct().ToList();

                var ctGoiDVs = _chuongTrinhGoiDichVuRepository.TableNoTracking

               .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
               .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

               .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
               .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

               .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
               .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

               .Where(p => lstChuongTrinhId.Contains(p.Id))
               .ToList();
                #endregion

                foreach (var dv in model)
                {
                    //get noi thuc hien neu co
                    long phongBenhVienId = 0;
                    long nhanVienId = 0;

                    if (!string.IsNullOrEmpty(dv.NoiThucHienId))
                    {
                        if (dv.NoiThucHienId.IndexOf(",") == -1)
                        {
                            phongBenhVienId = long.Parse(dv.NoiThucHienId);
                        }
                        else
                        {
                            var lstNoiThucHienId = dv.NoiThucHienId.Split(",");
                            phongBenhVienId = long.Parse(lstNoiThucHienId[0]);
                            nhanVienId = long.Parse(lstNoiThucHienId[1]);
                        }
                    }

                    #region Cập nhật 20/12/2022
                    //
                    //    var ctGoiDV = await _chuongTrinhGoiDichVuRepository.TableNoTracking

                    //.Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                    //.Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)

                    //.Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)
                    //.Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)

                    //.Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.DichVuGiuongBenhVien)
                    //.Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs).ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)

                    //.FirstOrDefaultAsync(p => p.Id == dv.ChuongTrinhGoiDichVuId);

                    var ctGoiDV = ctGoiDVs.FirstOrDefault(x => x.Id == dv.ChuongTrinhGoiDichVuId);
                    #endregion

                    var ycGoiDV = new YeuCauGoiDichVu();
                    long ycGoiDvId = 0;

                    ycGoiDV = _yeuCauGoiDichVuRepository.TableNoTracking
                        .FirstOrDefault(p => p.BenhNhanId == entity.BenhNhanId 
                                             && p.Id == dv.YeuCauGoiDichVuId);
                    if (ycGoiDV != null)
                    {
                        ycGoiDvId = ycGoiDV.Id;
                    }
                    else
                    {
                        if (lstCTGoiDVAdded.Any(p => p.ChuongTrinhGoiDichVuId == dv.ChuongTrinhGoiDichVuId))
                        {
                            ycGoiDV = lstCTGoiDVAdded.First(p => p.ChuongTrinhGoiDichVuId == dv.ChuongTrinhGoiDichVuId);
                        }
                        else
                        {
                            var ct = await _chuongTrinhGoiDichVuRepository.TableNoTracking
                            .Include(p => p.ChuongTrinhGoiDichVuDichKhamBenhs)
                            .Include(p => p.ChuongTrinhGoiDichVuDichVuKyThuats)
                            .Include(p => p.ChuongTrinhGoiDichVuDichVuGiuongs)
                            .FirstOrDefaultAsync(p => p.Id == dv.ChuongTrinhGoiDichVuId);

                            ycGoiDV = new YeuCauGoiDichVu();
                            ycGoiDV.BenhNhanId = entity.BenhNhanId ?? 0;
                            ycGoiDV.ChuongTrinhGoiDichVuId = dv.ChuongTrinhGoiDichVuId ?? 0;
                            ycGoiDV.MaChuongTrinh = ct.Ma;
                            ycGoiDV.TenChuongTrinh = ct.Ten;
                            ycGoiDV.GiaTruocChietKhau = ct.GiaTruocChietKhau;
                            ycGoiDV.GiaSauChietKhau = ct.GiaSauChietKhau;
                            ycGoiDV.TenGoiDichVu = ct.TenGoiDichVu;
                            ycGoiDV.MoTaGoiDichVu = ct.MoTaGoiDichVu;
                            ycGoiDV.GoiSoSinh = ct.GoiSoSinh;
                            ycGoiDV.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                            ycGoiDV.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                            ycGoiDV.ThoiDiemChiDinh = DateTime.Now;
                            ycGoiDV.BoPhanMarketingDangKy = false;
                            ycGoiDV.TrangThai = Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien;
                            ycGoiDV.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;

                            lstCTGoiDVAdded.Add(ycGoiDV);
                        }

                    }

                    //thêm dịch vụ
                    if (dv.TenNhomDichVu.Equals(Constants.NhomDichVu.DichVuKhamBenh))
                    {
                        var dichVuKhamBenhBenhVienGiaBaoHiem =
                                    await _dichVuKhamBenhBenhVienGiaBaoHiemRepository.TableNoTracking
                                        .FirstOrDefaultAsync(p =>
                                            p.DichVuKhamBenhBenhVienId == dv.MaDichVuId
                                            && p.TuNgay.Date <= DateTime.Now.Date
                                            && (p.DenNgay == null ||
                                                (p.DenNgay != null && p.DenNgay.GetValueOrDefault().Date >= DateTime.Now.Date)));


                        var dichVuKhamBenhBenhVien = _dichVuKhamBenhBenhVienRepository.TableNoTracking.Include(p => p.DichVuKhamBenh).FirstOrDefault(p => p.Id == dv.MaDichVuId);

                        var item = ctGoiDV.ChuongTrinhGoiDichVuDichKhamBenhs.FirstOrDefault(p => p.DichVuKhamBenhBenhVienId == dv.MaDichVuId);

                        var noiThucHien = GetNoiKhamGoiDVKB(dv.MaDichVuId ?? 0);

                        var yeuCauKhamBenh = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh
                        {
                            MaDichVu = dichVuKhamBenhBenhVien.Ma,
                            MaDichVuTT37 = dichVuKhamBenhBenhVien.DichVuKhamBenh?.MaTT37,
                            TenDichVu = dichVuKhamBenhBenhVien.Ten,

                            DuocHuongBaoHiem = dv.DuocHuongBHYT,
                            BaoHiemChiTra = null,

                            Gia = (decimal)item.DonGia,
                            DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                            DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                            //GiaBaoHiemThanhToan = item.BHYTThanhToan,

                            NoiChiDinhId = locationId,

                            NoiDangKyId = (phongBenhVienId != 0 || nhanVienId != 0) ? phongBenhVienId : noiThucHien?.NoiThucHienId,

                            //BacSiThucHienId = nhanVienId,
                            //BacSiDangKyId = (phongBenhVienId != 0 || nhanVienId != 0) ? (nhanVienId == 0 ? null : nhanVienId) : noiThucHien?.BacSiThucHienId,

                            NhanVienChiDinhId = currentUserId,
                            TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                            ThoiDiemChiDinh = DateTime.Now,
                            ThoiDiemDangKy = DateTime.Now,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                            DichVuKhamBenhBenhVienId = dichVuKhamBenhBenhVien.Id,

                            NhomGiaDichVuKhamBenhBenhVienId = dv.LoaiGiaId ?? 0,
                            //
                            DonGiaBaoHiem = dichVuKhamBenhBenhVienGiaBaoHiem?.Gia ?? 0,
                            MucHuongBaoHiem = dichVuKhamBenhBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,

                        };

                        if (phongBenhVienId != 0 || nhanVienId != 0)
                        {
                            if (nhanVienId != 0)
                            {
                                yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                            }
                        }
                        else
                        {
                            if (noiThucHien?.BacSiThucHienId != null && noiThucHien?.BacSiThucHienId != 0)
                            {
                                yeuCauKhamBenh.BacSiDangKyId = noiThucHien?.BacSiThucHienId;
                            }
                        }

                        //if (nhanVienId != 0)
                        //{
                        //    yeuCauKhamBenh.BacSiDangKyId = nhanVienId;
                        //}
                        //else
                        //{
                        //    yeuCauKhamBenh.BacSiDangKyId = null;
                        //}

                        //
                        if (ycGoiDvId == 0)
                        {
                            yeuCauKhamBenh.YeuCauGoiDichVu = ycGoiDV;
                        }
                        else if (ycGoiDvId != 0)
                        {
                            yeuCauKhamBenh.YeuCauGoiDichVuId = ycGoiDvId;
                        }
                        //
                        if (dichVuKhamBenhBenhVienGiaBaoHiem == null || dichVuKhamBenhBenhVienGiaBaoHiem?.Gia == 0)
                        {
                            yeuCauKhamBenh.DuocHuongBaoHiem = false;
                        }

                        //
                        entity.YeuCauKhamBenhs.Add(yeuCauKhamBenh);
                    }
                    else if (dv.TenNhomDichVu.Equals(Constants.NhomDichVu.DichVuKyThuat))
                    {
                        dv.DuocHuongBHYT = false; // cập nhật theo rule ở YCTN -> dịch vụ kỹ thuật ko đc hưởng BHYT
                        var item = ctGoiDV.ChuongTrinhGoiDichVuDichVuKyThuats.FirstOrDefault(p => p.DichVuKyThuatBenhVienId == dv.MaDichVuId);

                        var dichVuKyThuatBenhVienGiaBaoHiem =
                                        await _dichVuKyThuatBenhVienGiaBaoHiemRepository.TableNoTracking
                                            .FirstOrDefaultAsync(p =>
                                                p.DichVuKyThuatBenhVienId == dv.MaDichVuId
                                                && p.TuNgay.Date <= DateTime.Now.Date
                                                && (p.DenNgay == null ||
                                                    (p.DenNgay != null && p.DenNgay.GetValueOrDefault() >= DateTime.Now.Date)));

                        var noiThucHien = GetNoiKhamGoiDVKT(dv.MaDichVuId ?? 0);

                        var yeuCau = new YeuCauDichVuKyThuat
                        {
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            Ma4350DichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat?.Ma4350,
                            MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat?.MaGia,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,

                            DuocHuongBaoHiem = dv.DuocHuongBHYT,

                            BaoHiemChiTra = null,
                            Gia = (decimal)item.DonGia,
                            DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                            DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                            //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                            //TiLeUuDai = Convert.ToInt32(item.TLMG),
                            NoiChiDinhId = locationId,
                            
                            NoiThucHienId = phongBenhVienId != 0 ? phongBenhVienId : noiThucHien?.NoiThucHienId,

                            //NhanVienThucHienId = nhanVienId,
                            //BacSiThucHienId = nhanVienId,
                            NhanVienChiDinhId = currentUserId,
                            TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                            ThoiDiemChiDinh = DateTime.Now,
                            SoLan = dv.SoLuong ?? 0,
                            ThoiDiemDangKy = DateTime.Now,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,


                            DichVuKyThuatBenhVienId = item.DichVuKyThuatBenhVienId,
                            NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,

                            NhomChiPhi = item.DichVuKyThuatBenhVien.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                            NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                            LoaiDichVuKyThuat = GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId),
                            //
                            DonGiaBaoHiem = dichVuKyThuatBenhVienGiaBaoHiem?.Gia ?? 0,
                            MucHuongBaoHiem = dichVuKyThuatBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,
                        };

                        //
                        if (ycGoiDvId == 0)
                        {
                            yeuCau.YeuCauGoiDichVu = ycGoiDV;
                        }
                        else
                        {
                            yeuCau.YeuCauGoiDichVuId = ycGoiDvId;
                        }
                        //
                        if (dichVuKyThuatBenhVienGiaBaoHiem == null || dichVuKyThuatBenhVienGiaBaoHiem?.Gia == 0)
                        {
                            yeuCau.DuocHuongBaoHiem = false;
                        }
                        //

                        entity.YeuCauDichVuKyThuats.Add(yeuCau);
                    }
                    else if (dv.TenNhomDichVu.Equals(Constants.NhomDichVu.DichVuGiuong))
                    {
                        var item = ctGoiDV.ChuongTrinhGoiDichVuDichVuGiuongs.FirstOrDefault(p => p.DichVuGiuongBenhVienId == dv.MaDichVuId);

                        var dichVuKhamGiuongBenhVienGiaBaoHiem =
                                   await _dichVuGiuongBenhVienGiaBaoHiemRepository.TableNoTracking
                                       .FirstOrDefaultAsync(p =>
                                           p.DichVuGiuongBenhVienId == dv.MaDichVuId
                                           && p.TuNgay.Date <= DateTime.Now.Date
                                           && (p.DenNgay == null ||
                                               (p.DenNgay != null && p.DenNgay.GetValueOrDefault().Date >= DateTime.Now.Date)));

                        var noiThucHien = GetNoiKhamGoiDVG(dv.MaDichVuId ?? 0);

                        var yeuCau = new YeuCauDichVuGiuongBenhVien
                        {
                            Ma = item.DichVuGiuongBenhVien.Ma,
                            MaTT37 = item.DichVuGiuongBenhVien.DichVuGiuong?.MaTT37,
                            Ten = item.DichVuGiuongBenhVien.Ten,
                            DuocHuongBaoHiem = dv.DuocHuongBHYT,
                            BaoHiemChiTra = null,
                            Gia = (decimal)item.DonGia,
                            DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                            DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                            //GiaBaoHiemThanhToan = item.BHYTThanhToan,
                            NoiChiDinhId = locationId,

                            NoiThucHienId = phongBenhVienId != 0 ? phongBenhVienId : noiThucHien?.NoiThucHienId,

                            //nhanvien = nhanVienId,
                            //BacSiThucHienId = nhanVienId,
                            NhanVienChiDinhId = currentUserId,
                            TrangThai = Enums.EnumTrangThaiGiuongBenh.ChuaThucHien,
                            ThoiDiemChiDinh = DateTime.Now,
                            LoaiGiuong = item.DichVuGiuongBenhVien.LoaiGiuong,
                            MaGiuong = item.DichVuGiuongBenhVien.Ma,
                            TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                            NhomGiaDichVuGiuongBenhVienId = item.NhomGiaDichVuGiuongBenhVienId,
                            DichVuGiuongBenhVienId = item.DichVuGiuongBenhVienId,
                            //
                            DonGiaBaoHiem = dichVuKhamGiuongBenhVienGiaBaoHiem?.Gia ?? 0,
                            MucHuongBaoHiem = dichVuKhamGiuongBenhVienGiaBaoHiem?.TiLeBaoHiemThanhToan ?? 0,

                        };

                        //
                        if (ycGoiDvId == 0)
                        {
                            yeuCau.YeuCauGoiDichVu = ycGoiDV;
                        }
                        else
                        {
                            yeuCau.YeuCauGoiDichVuId = ycGoiDvId;
                        }
                        //
                        if (dichVuKhamGiuongBenhVienGiaBaoHiem == null || dichVuKhamGiuongBenhVienGiaBaoHiem?.Gia == 0)
                        {
                            yeuCau.DuocHuongBaoHiem = false;
                        }
                        //

                        entity.YeuCauDichVuGiuongBenhViens.Add(yeuCau);
                    }
                }
            }

            
            return entity;
        }

        private noiChiDinhDVModel GetNoiKhamGoiDVKB(long dvId)
        {
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            var tongSoNguoiKham = settings.GioiHanSoNguoiKham;

            #region Cập nhật 20/12/2022
            //var dvBenhVien = _dichVuKhamBenhBenhVienRepository
            //    .TableNoTracking.Include(p => p.DichVuKhamBenhBenhVienNoiThucHiens)
            //    .FirstOrDefault(p => p.Id == dvId);

            //if (dvBenhVien == null) return null;

            var dvBenhVien = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Where(x => x.Id == dvId)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (dvBenhVien == 0) return null;
            #endregion

            var lstKhoaPhongKhamId = new List<long>();
            var lstPhongBenhVienId = new List<long>();

            var noiThucHien = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuKhamBenhBenhVienId == dvId).ToList();

            var lstPhongKham = new List<Core.Domain.Entities.PhongBenhViens.PhongBenhVien>();

            if (noiThucHien.Any())
            {
                foreach (var item in noiThucHien)
                {
                    if (item.KhoaPhongId != null)
                    {
                        lstKhoaPhongKhamId.Add(item.KhoaPhongId ?? 0);
                    }
                    if (item.PhongBenhVienId != null)
                    {
                        lstPhongBenhVienId.Add(item.PhongBenhVienId ?? 0);
                    }
                }
            }
            else
            {
                lstPhongBenhVienId.AddRange(_phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id));
            }

            #region Cập nhật 20/12/2022
            if (lstKhoaPhongKhamId.Any())
            {
                //foreach (var khoaPhongKhamId in lstKhoaPhongKhamId)
                //{
                //    var lst = _phongBenhVienRepository.TableNoTracking
                //        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                //        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                //        .Where(p => p.IsDisabled != true && p.KhoaPhongId == khoaPhongKhamId)
                //        .Take(200)
                //        .ToList();
                //    lstPhongKham.AddRange(lst);
                //}

                var lst = _phongBenhVienRepository.TableNoTracking
                    .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                    .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                    .Where(p => p.IsDisabled != true && lstKhoaPhongKhamId.Contains(p.KhoaPhongId))
                    .Take(200)
                    .ToList();
                lstPhongKham.AddRange(lst);
            }
            if (lstPhongBenhVienId.Any())
            {
                //foreach (var phongBenhVienId in lstPhongBenhVienId)
                //{
                //    var lst = _phongBenhVienRepository.TableNoTracking
                //        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                //        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                //        .Where(p => p.IsDisabled != true && p.Id == phongBenhVienId)
                //        .Take(200)
                //        .ToList();
                //    lstPhongKham.AddRange(lst);
                //}

                var lst = _phongBenhVienRepository.TableNoTracking
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.User)
                        .Include(p => p.HoatDongNhanViens).ThenInclude(p => p.NhanVien).ThenInclude(p => p.ChucDanh)
                        .Where(p => p.IsDisabled != true && lstPhongBenhVienId.Contains(p.Id))
                        .Take(200)
                        .ToList();
                lstPhongKham.AddRange(lst);
            }
            #endregion

            var result = new List<PhongKhamTemplateVo>();
            foreach (var item in lstPhongKham)
            {
                if (item.HoatDongNhanViens.Any(p => p.NhanVien.ChucDanh != null && p.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi))
                {
                    foreach (var hoatDongNhanVien in item.HoatDongNhanViens)
                    {
                        var keyId = item.Id + "," + hoatDongNhanVien.NhanVienId;
                        if (hoatDongNhanVien.NhanVien.ChucDanh == null
                            || (hoatDongNhanVien.NhanVien.ChucDanh != null && hoatDongNhanVien.NhanVien.ChucDanh.NhomChucDanhId != (long)Enums.EnumNhomChucDanh.BacSi || result.Any(p => p.KeyId == keyId)))
                        {
                            continue;
                        }
                        var tong = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.NoiThucHienId != null && p.NoiThucHienId == item.Id) ||
                                                                                              (p.NoiThucHienId == null && p.NoiDangKyId != null && p.NoiDangKyId == item.Id)) &&
                                                                                              p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                        //Tram update: Tổng= tổng yêu cầu khám dc chỉ định hoặc thực hiện của BS này vào ngày hôm nay và có trạng thái khác Hủy
                        var tongYeuCauBacSiThucHien = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.BacSiThucHienId != null && p.BacSiThucHienId == hoatDongNhanVien.NhanVienId) ||
                                                                                                            (p.BacSiThucHienId == null && p.BacSiDangKyId != null && p.BacSiDangKyId == hoatDongNhanVien.NhanVienId)) &&
                                                                                                            p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                        var resultNeedAdd = new PhongKhamTemplateVo
                        {
                            BacSiId = hoatDongNhanVien.NhanVienId,
                            PhongKhamId = item.Id,
                            KeyId = keyId,
                            MaPhong = item.Ma,
                            TenBacSi = hoatDongNhanVien.NhanVien.User.HoTen,
                            Tong = tong.Count(),
                            DangKham = tong.Count(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham),
                            TongSoKhamGioiHan = tongSoNguoiKham,
                            IsWarning = tongYeuCauBacSiThucHien.Count() >= tongSoNguoiKham,
                            DisplayName = item.Ten,
                            TenPhong = item.Ten
                        };
                        result.Add(resultNeedAdd);
                    }
                }
                else
                {
                    //Tram update: Tổng= tổng yêu cầu khám dc chỉ định hoặc thực hiện tại phòng này vào ngày hôm nay và có trạng thái khác Hủy
                    var tong = _yeuCauKhamBenhRepository.TableNoTracking.Where(p => ((p.NoiThucHienId != null && p.NoiThucHienId == item.Id) ||
                                                                                     (p.NoiThucHienId == null && p.NoiDangKyId != null && p.NoiDangKyId == item.Id)) &&
                                                                                     p.ThoiDiemChiDinh.Date == DateTime.Now.Date && p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                    var keyId = item.Id + "," + 0;
                    var resultNeedAdd = new PhongKhamTemplateVo
                    {
                        BacSiId = 0,
                        PhongKhamId = item.Id,
                        KeyId = keyId,
                        MaPhong = item.Ma,
                        TenBacSi = "",
                        Tong = tong.Count(),
                        DangKham = tong.Count(p => p.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham),
                        TongSoKhamGioiHan = tongSoNguoiKham,
                        IsWarning = 0 >= tongSoNguoiKham,
                        DisplayName = item.Ma,
                        TenPhong = item.Ten
                    };
                    result.Add(resultNeedAdd);

                }

            }

            result = result.OrderBy(p => p.Tong).Distinct().ToList();

            return new noiChiDinhDVModel
            {
                BacSiThucHienId = (result.FirstOrDefault()?.BacSiId == 0 || result.FirstOrDefault()?.BacSiId == null) ? null : result.FirstOrDefault()?.BacSiId,
                NoiThucHienId = (result.FirstOrDefault()?.PhongKhamId == 0 || result.FirstOrDefault()?.PhongKhamId == null) ? null : result.FirstOrDefault()?.PhongKhamId
            };
        }

        private noiChiDinhDVModel GetNoiKhamGoiDVKT(long dvId)
        {
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var dvBenhVien = _dichVuKyThuatBenhVienRepository
                .TableNoTracking.Include(p => p.DichVuKyThuatBenhVienNoiThucHienUuTiens)
                .FirstOrDefault(p => p.Id == dvId);

            if (dvBenhVien == null) return null;

            var noiThucHienUuTien = dvBenhVien.DichVuKyThuatBenhVienNoiThucHienUuTiens?.FirstOrDefault(p => p.LoaiNoiThucHienUuTien == Enums.LoaiNoiThucHienUuTien.NguoiDung);

            if (noiThucHienUuTien != null)
            {
                return new noiChiDinhDVModel
                {
                    NoiThucHienId = noiThucHienUuTien.PhongBenhVienId
                };
            }
            else
            {
                var noiThucHien =  _dichVuKyThuatBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuKyThuatBenhVienId == dvId).ToList();
                if (noiThucHien.Any())
                {
                    foreach (var item in noiThucHien)
                    {
                        if (item.KhoaPhongId != null)
                        {
                            var phong = _phongBenhVienRepository.TableNoTracking.FirstOrDefault(p => p.IsDisabled != true && p.KhoaPhongId == item.KhoaPhongId);
                            if (phong != null)
                            {
                                return new noiChiDinhDVModel
                                {
                                    NoiThucHienId = phong?.Id ?? 0,
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                        if (item.PhongBenhVienId != null)
                        {
                            return new noiChiDinhDVModel
                            {
                                NoiThucHienId = item.PhongBenhVienId,
                            };
                        }
                    }
                }
                else
                {
                    var lstPhongId = _phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id);

                    if (lstPhongId.Any())
                    {
                        return new noiChiDinhDVModel
                        {
                            NoiThucHienId = lstPhongId.First(),
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;

        }

        private noiChiDinhDVModel GetNoiKhamGoiDVG(long dvId)
        {
            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();

            var dvBenhVien = _dichVuGiuongBenhVienRepository
                .TableNoTracking.Include(p => p.DichVuGiuongBenhVienNoiThucHiens)
                .FirstOrDefault(p => p.Id == dvId);

            if (dvBenhVien == null) return null;

            var noiThucHien = _dichVuGiuongBenhVienNoiThucHienRepository.TableNoTracking.Where(p => p.DichVuGiuongBenhVienId == dvId).ToList();
            if (noiThucHien.Any())
            {
                foreach (var item in noiThucHien)
                {
                    if (item.KhoaPhongId != null)
                    {
                        var phong = _phongBenhVienRepository.TableNoTracking.FirstOrDefault(p => p.IsDisabled != true && p.KhoaPhongId == item.KhoaPhongId);
                        if (phong != null)
                        {
                            return new noiChiDinhDVModel
                            {
                                NoiThucHienId = phong?.Id ?? 0,
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                    if (item.PhongBenhVienId != null)
                    {
                        return new noiChiDinhDVModel
                        {
                            NoiThucHienId = item.PhongBenhVienId,
                        };
                    }
                }
            }
            else
            {
                var lstPhongId = _phongBenhVienRepository.TableNoTracking.Where(p => p.IsDisabled != true).Select(p => p.Id);

                if (lstPhongId.Any())
                {
                    return new noiChiDinhDVModel
                    {
                        NoiThucHienId = lstPhongId.First(),
                    };
                }
                else
                {
                    return null;
                }
            }

            return null;

        }

    }
}
