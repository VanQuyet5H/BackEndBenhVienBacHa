using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        #region Grid
        public async Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNhanVienTheoHopDongTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNhanVienTheoHopDongTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            if (timKiemNangCaoObj.HopDongId == null || timKiemNangCaoObj.HopDongId == 0)
            {
                return new GridDataSource
                {
                    Data = new List<GridItem>(),
                    TotalRowCount = 0
                };
            }
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId //&& (timKiemNangCaoObj.DaTaoYCTN == null||x.YeuCauTiepNhans.Any())
                //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaNhanVien, x => x.HoTen, x => x.TenDonViHoacBoPhan, x => x.NamSinh.ToString(), x => x.SoDienThoai,
                //    x => x.SoChungMinhThu, x => x.NhomDoiTuongKhamSucKhoe) //, x => x.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.MaYeuCauTiepNhan).FirstOrDefault())
                // chuyển applylike -> search thủ công
                    && (x.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                        || x.YeuCauTiepNhans.Any(a => a.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics))))
                .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                {
                    Stt = item.STTNhanVien,
                    Id = item.Id,
                    MaNhanVien = item.MaNhanVien,
                    TenNhanVien = item.HoTen,
                    DonViBoPhan = item.TenDonViHoacBoPhan,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    NamSinh = item.NamSinh,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    SoDienThoaiTimKiem = item.SoDienThoai,
                    Email = item.Email,
                    ChungMinhThu = item.SoChungMinhThu,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                    TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                    NhomKham = item.NhomDoiTuongKhamSucKhoe,
                    //YeuCauTiepNhanId= item.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault(),
                    //MaYeuCauTiepNhan = item.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.MaYeuCauTiepNhan).FirstOrDefault(),
                    //TrangThaiYeuCauTiepNhan = item.HuyKham == true ? Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy : item.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.TrangThaiYeuCauTiepNhan).FirstOrDefault(),
                    //TinhTrangDoChiSoSinhTon = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.OrderByDescending(a => a.Id).FirstOrDefault().KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    TrangThaiYeuCauTiepNhan = (item.HuyKham == true || (item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.All(a => a.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHuy)))
                        ? Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                        : (!item.YeuCauTiepNhans.Any() ? (EnumTrangThaiYeuCauTiepNhan?)null
                            : (item.YeuCauTiepNhans.Any(a => a.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                                ? EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                : EnumTrangThaiYeuCauTiepNhan.DaHoanTat)),
                    TinhTrangDoChiSoSinhTon = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.Where(a => a.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy).All(a => a.KetQuaSinhHieus.Any()) ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    CoDichVuPhatSinh = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.Any(a => a.YeuCauDichVuKyThuats.Any(b => b.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && b.GoiKhamSucKhoeId == null))
                });
            //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaNhanVien, x => x.TenNhanVien, x => x.DonViBoPhan,
            //                                                    x => x.GioiTinh, x => x.NamSinh.ToString(), x => x.SoDienThoai, x => x.SoDienThoaiTimKiem,
            //                                                    x => x.ChungMinhThu, x => x.DanToc, x => x.TinhThanh, x => x.NhomKham);

            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham || timKiemNangCaoObj.TrangThai.DaKham || timKiemNangCaoObj.TrangThai.HuyKham || timKiemNangCaoObj.TrangThai.DichVuPhatSinh))
            {
                //query = query.Where(x =>
                //    (timKiemNangCaoObj.TrangThai.ChuaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                //    || (timKiemNangCaoObj.TrangThai.DangKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                //    || (timKiemNangCaoObj.TrangThai.DaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                //    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                //    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));

                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && (x.TrangThaiYeuCauTiepNhan == null || x.TrangThaiYeuCauTiepNhan == 0))
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    || (timKiemNangCaoObj.TrangThai.DaKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));
            }

            // kiểm tra tình trạng đo chỉ số sinh tôn
            if (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                    || (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.DaDo));
            }

            // kiểm tra nếu là danh sách lịch sử
            if (timKiemNangCaoObj.IsLichSu)
            {
                query = query.Where(x =>
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            // kiểm tra nếu là danh sách đang thực hiện
            else if (timKiemNangCaoObj.IsDangKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            }
            else if (timKiemNangCaoObj.IsDangKhamVaDaKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                         || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            else
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == null
                                         || x.TrangThaiYeuCauTiepNhan == 0
                                         || x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThaiYeuCauTiepNhan";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            //await Task.WhenAll(countTask, queryTask);

            if (queryTask.Length > 0)
            {
                var lstHopDongKhamSucKhoeNhanVienId = queryTask.Select(x => x.Id).Distinct().ToList();
                var lstHopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhans)
                    .Where(x => lstHopDongKhamSucKhoeNhanVienId.Contains(x.Id))
                    .ToList();
                foreach (var hopDongNhanVien in queryTask)
                {
                    var yeuCauTiepNhan = lstHopDongKhamSucKhoeNhanVien.First(x => x.Id == hopDongNhanVien.Id).YeuCauTiepNhans.OrderByDescending(a => a.Id).FirstOrDefault();
                    if (yeuCauTiepNhan != null)
                    {
                        hopDongNhanVien.MaYeuCauTiepNhan = yeuCauTiepNhan.MaYeuCauTiepNhan;
                        hopDongNhanVien.YeuCauTiepNhanId = yeuCauTiepNhan.Id;
                    }
                }
            }

            return new GridDataSource
            {
                Data = queryTask,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalNhanVienTheoHopDongForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNhanVienTheoHopDongTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNhanVienTheoHopDongTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            if (timKiemNangCaoObj.HopDongId == null || timKiemNangCaoObj.HopDongId == 0)
            {
                return new GridDataSource
                {
                    Data = new List<GridItem>(),
                    TotalRowCount = 0
                };
            }
            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId // && (timKiemNangCaoObj.DaTaoYCTN == null || x.YeuCauTiepNhans.Any())
                //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaNhanVien, x => x.HoTen, x => x.TenDonViHoacBoPhan, x => x.NamSinh.ToString(), x => x.SoDienThoai,
                //    x => x.SoChungMinhThu, x => x.NhomDoiTuongKhamSucKhoe) // x => x.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.MaYeuCauTiepNhan).FirstOrDefault())
                && (x.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                    || x.YeuCauTiepNhans.Any(a => a.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics))))
                .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                {
                    Id = item.Id,
                    MaNhanVien = item.MaNhanVien,
                    TenNhanVien = item.HoTen,
                    DonViBoPhan = item.TenDonViHoacBoPhan,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    NamSinh = item.NamSinh,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    SoDienThoaiTimKiem = item.SoDienThoai,
                    Email = item.Email,
                    ChungMinhThu = item.SoChungMinhThu,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                    TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                    NhomKham = item.NhomDoiTuongKhamSucKhoe,
                    //YeuCauTiepNhanId = item.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefault(),
                    //TrangThaiYeuCauTiepNhan = item.HuyKham == true ? Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy : item.YeuCauTiepNhans.OrderByDescending(a => a.Id).Select(a => a.TrangThaiYeuCauTiepNhan).FirstOrDefault(),
                    //TinhTrangDoChiSoSinhTon = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.OrderByDescending(a => a.Id).FirstOrDefault().KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    TrangThaiYeuCauTiepNhan = (item.HuyKham == true || (item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.All(a => a.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DaHuy)))
                        ? Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                        : (!item.YeuCauTiepNhans.Any() ? (EnumTrangThaiYeuCauTiepNhan?)null
                            : (item.YeuCauTiepNhans.Any(a => a.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                                ? EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                : EnumTrangThaiYeuCauTiepNhan.DaHoanTat)),
                    TinhTrangDoChiSoSinhTon = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.Where(a => a.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy).All(a => a.KetQuaSinhHieus.Any()) ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    CoDichVuPhatSinh = item.YeuCauTiepNhans.Any() && item.YeuCauTiepNhans.Any(a => a.YeuCauDichVuKyThuats.Any(b => b.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && b.GoiKhamSucKhoeId == null))
                });
            //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaNhanVien, x => x.TenNhanVien, x => x.DonViBoPhan,
            //                                                    x => x.GioiTinh, x => x.NamSinh.ToString(), x => x.SoDienThoai, x => x.SoDienThoaiTimKiem,
            //                                                    x => x.ChungMinhThu, x => x.DanToc, x => x.TinhThanh, x => x.NhomKham);

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham || timKiemNangCaoObj.TrangThai.DaKham || timKiemNangCaoObj.TrangThai.HuyKham || timKiemNangCaoObj.TrangThai.DichVuPhatSinh))
            {
                //query = query.Where(x =>
                //    (timKiemNangCaoObj.TrangThai.ChuaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                //    || (timKiemNangCaoObj.TrangThai.DangKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                //    || (timKiemNangCaoObj.TrangThai.DaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                //    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                //    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));

                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && (x.TrangThaiYeuCauTiepNhan == null || x.TrangThaiYeuCauTiepNhan == 0))
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    || (timKiemNangCaoObj.TrangThai.DaKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));
            }

            // kiểm tra tình trạng đo chỉ số sinh tôn
            if (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                    || (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.DaDo));
            }

            // kiểm tra nếu là danh sách lịch sử
            if (timKiemNangCaoObj.IsLichSu)
            {
                query = query.Where(x =>
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            // kiểm tra nếu là danh sách đang thực hiện
            else if (timKiemNangCaoObj.IsDangKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            }
            else if (timKiemNangCaoObj.IsDangKhamVaDaKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                         || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            else
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == null
                                         || x.TrangThaiYeuCauTiepNhan == 0
                                         || x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsyncVer2(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNhanVienTheoHopDongTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNhanVienTheoHopDongTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            if (timKiemNangCaoObj.HopDongId == null || timKiemNangCaoObj.HopDongId == 0)
            {
                return new GridDataSource
                {
                    Data = new List<GridItem>(),
                    TotalRowCount = 0
                };
            }

            #region Cập nhật 08/04/2022
            var lstTiepNhanCoDichVuPhatSinhId = new List<long>();
            if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh)
            {
                lstTiepNhanCoDichVuPhatSinhId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null
                                && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.GoiKhamSucKhoeId == null)
                    .Select(x => x.YeuCauTiepNhanId)
                    .Distinct().ToList();
            }
            #endregion

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                            && !x.YeuCauTiepNhans.Any()
                            && (x.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)))
                .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                {
                    Stt = item.STTNhanVien,
                    Id = item.Id,
                    MaNhanVien = item.MaNhanVien,
                    TenNhanVien = item.HoTen,
                    DonViBoPhan = item.TenDonViHoacBoPhan,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    NamSinh = item.NamSinh,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    SoDienThoaiTimKiem = item.SoDienThoai,
                    Email = item.Email,
                    ChungMinhThu = item.SoChungMinhThu,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                    TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                    NhomKham = item.NhomDoiTuongKhamSucKhoe,
                    TrangThaiYeuCauTiepNhan = (EnumTrangThaiYeuCauTiepNhan?)null,
                    TinhTrangDoChiSoSinhTon = Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    CoDichVuPhatSinh = false
                }).Union(
                    BaseRepository.TableNoTracking
                        .Where(x => x.HopDongKhamSucKhoeNhanVien != null
                                    && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                                    && (x.HopDongKhamSucKhoeNhanVien.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)))
                    .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                    {
                        Stt = item.HopDongKhamSucKhoeNhanVien != null ? item.HopDongKhamSucKhoeNhanVien.STTNhanVien : null,
                        Id = item.HopDongKhamSucKhoeNhanVienId.Value,
                        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                        YeuCauTiepNhanId = item.Id,
                        HopDongKhamSucKhoeNhanVienId = item.HopDongKhamSucKhoeNhanVienId,
                        MaNhanVien = item.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                        TenNhanVien = item.HoTen,
                        DonViBoPhan = item.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan,
                        GioiTinh = item.GioiTinh.GetDescription(),
                        NamSinh = item.NamSinh,
                        SoDienThoai = item.SoDienThoaiDisplay,
                        SoDienThoaiTimKiem = item.SoDienThoai,
                        Email = item.Email,
                        ChungMinhThu = item.SoChungMinhThu,
                        DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                        TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                        NhomKham = item.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                        TrangThaiYeuCauTiepNhan = item.TrangThaiYeuCauTiepNhan,
                        //TinhTrangDoChiSoSinhTon = item.KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                        TinhTrangDoChiSoSinhTon = (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo)) 
                            ? (item.KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo) 
                            : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                        //CoDichVuPhatSinh = (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh) ? item.YeuCauDichVuKyThuats.Any(b => b.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && b.GoiKhamSucKhoeId == null) : false,
                        CoDichVuPhatSinh = (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh) ? lstTiepNhanCoDichVuPhatSinhId.Contains(item.Id) : false,
                        ThoiDiemTiepNhan = item.ThoiDiemTiepNhan
                    })
                //.GroupBy(x => x.HopDongKhamSucKhoeNhanVienId)
                //.Select(item => item.OrderByDescending(a => a.Id).FirstOrDefault())
                );
            //.ApplyLike(timKiemNangCaoObj.SearchString, x => x.MaNhanVien, x => x.TenNhanVien, x => x.DonViBoPhan,
            //                                                    x => x.GioiTinh, x => x.NamSinh.ToString(), x => x.SoDienThoai, x => x.SoDienThoaiTimKiem,
            //                                                    x => x.ChungMinhThu, x => x.DanToc, x => x.TinhThanh, x => x.NhomKham);

            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham || timKiemNangCaoObj.TrangThai.DaKham || timKiemNangCaoObj.TrangThai.HuyKham || timKiemNangCaoObj.TrangThai.DichVuPhatSinh))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && (x.TrangThaiYeuCauTiepNhan == null || x.TrangThaiYeuCauTiepNhan == 0))
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    || (timKiemNangCaoObj.TrangThai.DaKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));
            }

            // kiểm tra tình trạng đo chỉ số sinh tôn
            if (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                    || (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.DaDo));
            }

            // kiểm tra nếu là danh sách lịch sử
            if (timKiemNangCaoObj.IsLichSu)
            {
                query = query.Where(x =>
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            // kiểm tra nếu là danh sách đang thực hiện
            else if (timKiemNangCaoObj.IsDangKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            }
            else if (timKiemNangCaoObj.IsDangKhamVaDaKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                         || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            else
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == null
                                         || x.TrangThaiYeuCauTiepNhan == 0
                                         || x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThaiYeuCauTiepNhan";
            }

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = await query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            return new GridDataSource
            {
                Data = queryTask,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalNhanVienTheoHopDongForGridAsyncVer2(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new TiepNhanNhanVienTheoHopDongTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNhanVienTheoHopDongTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            if (timKiemNangCaoObj.HopDongId == null || timKiemNangCaoObj.HopDongId == 0)
            {
                return new GridDataSource
                {
                    Data = new List<GridItem>(),
                    TotalRowCount = 0
                };
            }

            #region Cập nhật 08/04/2022
            var lstTiepNhanCoDichVuPhatSinhId = new List<long>();
            if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh)
            {
                lstTiepNhanCoDichVuPhatSinhId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null
                                && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.GoiKhamSucKhoeId == null)
                    .Select(x => x.YeuCauTiepNhanId)
                    .Distinct().ToList();
            }
            #endregion

            var query = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                            && !x.YeuCauTiepNhans.Any()
                            && (x.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                || x.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)))
                .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                {
                    Id = item.Id,
                    MaNhanVien = item.MaNhanVien,
                    TenNhanVien = item.HoTen,
                    DonViBoPhan = item.TenDonViHoacBoPhan,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    NamSinh = item.NamSinh,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    SoDienThoaiTimKiem = item.SoDienThoai,
                    Email = item.Email,
                    ChungMinhThu = item.SoChungMinhThu,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                    TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                    NhomKham = item.NhomDoiTuongKhamSucKhoe,
                    TrangThaiYeuCauTiepNhan = (EnumTrangThaiYeuCauTiepNhan?)null,
                    TinhTrangDoChiSoSinhTon = Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    CoDichVuPhatSinh = false
                }).Union(
                    BaseRepository.TableNoTracking
                        .Where(x => x.HopDongKhamSucKhoeNhanVien != null
                                    && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                                    && (x.HopDongKhamSucKhoeNhanVien.MaNhanVien.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HoTen.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.SoDienThoai.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.SoChungMinhThu.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)
                                        || x.MaYeuCauTiepNhan.Trim().ToLower().Contains(searchStringRemoveVietnameseDiacritics)))
                    .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                    {
                        Id = item.Id,
                        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                        YeuCauTiepNhanId = item.Id,
                        HopDongKhamSucKhoeNhanVienId = item.HopDongKhamSucKhoeNhanVienId,
                        MaNhanVien = item.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                        TenNhanVien = item.HoTen,
                        DonViBoPhan = item.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan,
                        GioiTinh = item.GioiTinh.GetDescription(),
                        NamSinh = item.NamSinh,
                        SoDienThoai = item.SoDienThoaiDisplay,
                        SoDienThoaiTimKiem = item.SoDienThoai,
                        Email = item.Email,
                        ChungMinhThu = item.SoChungMinhThu,
                        DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                        TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                        NhomKham = item.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                        TrangThaiYeuCauTiepNhan = item.TrangThaiYeuCauTiepNhan,
                        //TinhTrangDoChiSoSinhTon = item.KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                        TinhTrangDoChiSoSinhTon = (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
                            ? (item.KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                            : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                        //CoDichVuPhatSinh = item.YeuCauDichVuKyThuats.Any(b => b.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && b.GoiKhamSucKhoeId == null)
                        //CoDichVuPhatSinh = (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh) ? item.YeuCauDichVuKyThuats.Any(b => b.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && b.GoiKhamSucKhoeId == null) : false,
                        CoDichVuPhatSinh = (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh) ? lstTiepNhanCoDichVuPhatSinhId.Contains(item.Id) : false,
                    })
                //.GroupBy(x => x.HopDongKhamSucKhoeNhanVienId)
                //.Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                //{
                //    Id = item.Last().Id,
                //    MaYeuCauTiepNhan = item.Last().MaYeuCauTiepNhan,
                //    YeuCauTiepNhanId = item.Last().Id,
                //    HopDongKhamSucKhoeNhanVienId = item.Key,
                //    MaNhanVien = item.Last().MaNhanVien,
                //    TenNhanVien = item.Last().TenNhanVien,
                //    DonViBoPhan = item.Last().DonViBoPhan,
                //    GioiTinh = item.Last().GioiTinh,
                //    NamSinh = item.Last().NamSinh,
                //    SoDienThoai = item.Last().SoDienThoai,
                //    SoDienThoaiTimKiem = item.Last().SoDienThoaiTimKiem,
                //    Email = item.Last().Email,
                //    ChungMinhThu = item.Last().ChungMinhThu,
                //    DanToc = item.Last().DanToc,
                //    TinhThanh = item.Last().TinhThanh,
                //    NhomKham = item.Last().NhomKham,
                //    TrangThaiYeuCauTiepNhan = item.Last().TrangThaiYeuCauTiepNhan,
                //    TinhTrangDoChiSoSinhTon = item.Last().TinhTrangDoChiSoSinhTon,
                //    CoDichVuPhatSinh = item.Last().CoDichVuPhatSinh
                //})
                );

            // kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham || timKiemNangCaoObj.TrangThai.DaKham || timKiemNangCaoObj.TrangThai.HuyKham || timKiemNangCaoObj.TrangThai.DichVuPhatSinh))
            {
                //query = query.Where(x =>
                //    (timKiemNangCaoObj.TrangThai.ChuaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                //    || (timKiemNangCaoObj.TrangThai.DangKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                //    || (timKiemNangCaoObj.TrangThai.DaKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                //    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TinhTrang == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                //    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));

                query = query.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && (x.TrangThaiYeuCauTiepNhan == null || x.TrangThaiYeuCauTiepNhan == 0))
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    || (timKiemNangCaoObj.TrangThai.DaKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));
            }

            // kiểm tra tình trạng đo chỉ số sinh tôn
            if (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
            {
                query = query.Where(x =>
                    (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                    || (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.DaDo));
            }

            // kiểm tra nếu là danh sách lịch sử
            if (timKiemNangCaoObj.IsLichSu)
            {
                query = query.Where(x =>
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            // kiểm tra nếu là danh sách đang thực hiện
            else if (timKiemNangCaoObj.IsDangKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            }
            else if (timKiemNangCaoObj.IsDangKhamVaDaKham)
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                         || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            else
            {
                query = query.Where(x => x.TrangThaiYeuCauTiepNhan == null
                                         || x.TrangThaiYeuCauTiepNhan == 0
                                         || x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataNhanVienTheoHopDongForGridAsyncVer3(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new TiepNhanNhanVienTheoHopDongTimKiemNangCapVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<TiepNhanNhanVienTheoHopDongTimKiemNangCapVo>(queryInfo.AdditionalSearchString);
            }
            var searchStringRemoveVietnameseDiacritics = (timKiemNangCaoObj.SearchString ?? "").Trim().ToLower().RemoveVietnameseDiacritics();

            if (timKiemNangCaoObj.HopDongId == null || timKiemNangCaoObj.HopDongId == 0)
            {
                return new GridDataSource
                {
                    Data = new List<GridItem>(),
                    TotalRowCount = 0
                };
            }

            #region Cập nhật 08/04/2022
            var lstTiepNhanCoDichVuPhatSinhId = new List<long>();
            if (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh)
            {
                lstTiepNhanCoDichVuPhatSinhId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien != null
                                && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.GoiKhamSucKhoeId == null)
                    .Select(x => x.YeuCauTiepNhanId)
                    .Distinct().ToList();
            }
            #endregion

            var hopDongKhamSucKhoeNhanViens = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId)
                .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                {
                    YeuCauTiepNhanIds = item.YeuCauTiepNhans.Select(o=>o.Id).ToList(),
                    Stt = item.STTNhanVien,
                    Id = item.Id,
                    MaNhanVien = item.MaNhanVien,
                    TenNhanVien = item.HoTen,
                    DonViBoPhan = item.TenDonViHoacBoPhan,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    NamSinh = item.NamSinh,
                    SoDienThoai = item.SoDienThoaiDisplay,
                    SoDienThoaiTimKiem = item.SoDienThoai,
                    Email = item.Email,
                    ChungMinhThu = item.SoChungMinhThu,
                    DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                    TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                    NhomKham = item.NhomDoiTuongKhamSucKhoe,
                    TrangThaiYeuCauTiepNhan = (EnumTrangThaiYeuCauTiepNhan?)null,
                    TinhTrangDoChiSoSinhTon = Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                    CoDichVuPhatSinh = false
                }).ToList();

            var yeuCauTiepNhans = BaseRepository.TableNoTracking
                        .Where(x => x.HopDongKhamSucKhoeNhanVienId != null
                                    && x.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemNangCaoObj.HopDongId)
                    .Select(item => new TiepNhanNhanVienTheoHopDongGridVo()
                    {
                        Stt = item.HopDongKhamSucKhoeNhanVien != null ? item.HopDongKhamSucKhoeNhanVien.STTNhanVien : null,
                        Id = item.HopDongKhamSucKhoeNhanVienId.Value,
                        MaYeuCauTiepNhan = item.MaYeuCauTiepNhan,
                        YeuCauTiepNhanId = item.Id,
                        HopDongKhamSucKhoeNhanVienId = item.HopDongKhamSucKhoeNhanVienId,
                        MaNhanVien = item.HopDongKhamSucKhoeNhanVien.MaNhanVien,
                        TenNhanVien = item.HoTen,
                        DonViBoPhan = item.HopDongKhamSucKhoeNhanVien.TenDonViHoacBoPhan,
                        GioiTinh = item.GioiTinh.GetDescription(),
                        NamSinh = item.NamSinh,
                        SoDienThoai = item.SoDienThoaiDisplay,
                        SoDienThoaiTimKiem = item.SoDienThoai,
                        Email = item.Email,
                        ChungMinhThu = item.SoChungMinhThu,
                        DanToc = item.DanToc != null ? item.DanToc.Ten : null,
                        TinhThanh = item.TinhThanh != null ? item.TinhThanh.Ten : null,
                        NhomKham = item.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe,
                        TrangThaiYeuCauTiepNhan = item.TrangThaiYeuCauTiepNhan,
                        TinhTrangDoChiSoSinhTon = (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
                            ? (item.KetQuaSinhHieus.Any() ? Enums.TinhTrangDoChiSoSinhTon.DaDo : Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                            : Enums.TinhTrangDoChiSoSinhTon.ChuaDo,
                        CoDichVuPhatSinh = (timKiemNangCaoObj.TrangThai != null && timKiemNangCaoObj.TrangThai.DichVuPhatSinh) ? lstTiepNhanCoDichVuPhatSinhId.Contains(item.Id) : false,
                        ThoiDiemTiepNhan = item.ThoiDiemTiepNhan
                    }).ToList();
            hopDongKhamSucKhoeNhanViens = hopDongKhamSucKhoeNhanViens.Where(o => !o.YeuCauTiepNhanIds.Any()).ToList();
            if (!string.IsNullOrEmpty(searchStringRemoveVietnameseDiacritics))
            {
                hopDongKhamSucKhoeNhanViens = hopDongKhamSucKhoeNhanViens
                .Where(x => (x.MaNhanVien != null && x.MaNhanVien.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.DonViBoPhan != null && x.DonViBoPhan.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.TenNhanVien != null && x.TenNhanVien.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.NamSinh != null && x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.SoDienThoai != null && x.SoDienThoai.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.ChungMinhThu != null && x.ChungMinhThu.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.NhomKham != null && x.NhomKham.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics)))
                .ToList();

                yeuCauTiepNhans = yeuCauTiepNhans
                .Where(x => (x.MaNhanVien != null && x.MaNhanVien.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.MaYeuCauTiepNhan != null && x.MaYeuCauTiepNhan.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.DonViBoPhan != null && x.DonViBoPhan.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.TenNhanVien != null && x.TenNhanVien.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.NamSinh != null && x.NamSinh.ToString().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.SoDienThoai != null && x.SoDienThoai.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.ChungMinhThu != null && x.ChungMinhThu.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics))
                                || (x.NhomKham != null && x.NhomKham.Trim().ToLower().RemoveVietnameseDiacritics().Contains(searchStringRemoveVietnameseDiacritics)))
                .ToList();
            }

            var returnData = hopDongKhamSucKhoeNhanViens.Concat(yeuCauTiepNhans);

            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TrangThai != null && (timKiemNangCaoObj.TrangThai.ChuaKham || timKiemNangCaoObj.TrangThai.DangKham || timKiemNangCaoObj.TrangThai.DaKham || timKiemNangCaoObj.TrangThai.HuyKham || timKiemNangCaoObj.TrangThai.DichVuPhatSinh))
            {
                returnData = returnData.Where(x =>
                    (timKiemNangCaoObj.TrangThai.ChuaKham && (x.TrangThaiYeuCauTiepNhan == null || x.TrangThaiYeuCauTiepNhan == 0))
                    || (timKiemNangCaoObj.TrangThai.DangKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                    || (timKiemNangCaoObj.TrangThai.DaKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                    || (timKiemNangCaoObj.TrangThai.HuyKham && x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                    || (timKiemNangCaoObj.TrangThai.DichVuPhatSinh && x.CoDichVuPhatSinh));
            }

            // kiểm tra tình trạng đo chỉ số sinh tôn
            if (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon != null && (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo || timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo))
            {
                returnData = returnData.Where(x =>
                    (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.ChuaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.ChuaDo)
                    || (timKiemNangCaoObj.TinhTrangDoChiSoSinhTon.DaDo && x.TinhTrangDoChiSoSinhTon == Enums.TinhTrangDoChiSoSinhTon.DaDo));
            }

            // kiểm tra nếu là danh sách lịch sử
            if (timKiemNangCaoObj.IsLichSu)
            {
                returnData = returnData.Where(x =>
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy ||
                    x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            // kiểm tra nếu là danh sách đang thực hiện
            else if (timKiemNangCaoObj.IsDangKham)
            {
                returnData = returnData.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien);
            }
            else if (timKiemNangCaoObj.IsDangKhamVaDaKham)
            {
                returnData = returnData.Where(x => x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                         || x.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat);
            }
            else
            {
                returnData = returnData.Where(x => x.TrangThaiYeuCauTiepNhan == null
                                         || x.TrangThaiYeuCauTiepNhan == 0
                                         || x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy);
            }

            if (queryInfo.Sort.Count == 1 && string.IsNullOrEmpty(queryInfo.Sort[0].Dir))
            {
                queryInfo.Sort[0].Dir = "asc";
                queryInfo.Sort[0].Field = "TrangThaiYeuCauTiepNhan";
            }
            var totalRowCount = returnData.Count();
            var data = returnData.AsQueryable().OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();
                        
            return new GridDataSource
            {
                Data = data,
                TotalRowCount = totalRowCount
            };
        }

        public async Task<ICollection<TiepNhanDichVuChiDinhVo>> GetDanhSachDichVuTheoGoiKhamCuaBenhNhanAsync(TiepNhanDichVuChiDinhQueryVo hopDongQueryInfo)
        {
            var hopDongKhamNhanVien =
                await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking.FirstOrDefaultAsync(x =>
                    x.Id == hopDongQueryInfo.HopDongKhamSucKhoeNhanVienId);
            if (hopDongKhamNhanVien == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var query = await _goiKhamSucKhoeDichVuKhamBenhRepository.TableNoTracking
                .Where(x => x.GoiKhamSucKhoeId == hopDongKhamNhanVien.GoiKhamSucKhoeId
                            && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                            && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                            && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                            && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                .Select(item => new TiepNhanDichVuChiDinhVo()
                {
                    //LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh,
                    //TenNhomDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription(),
                    TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    Ten = item.DichVuKhamBenhBenhVien.Ten,
                    Ma = item.DichVuKhamBenhBenhVien.Ma,
                    DonGiaBenhVien = item.DonGiaBenhVien,
                    DonGiaMoi = item.DonGiaBenhVien,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                    TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                    TenNoiThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                    TinhTrang = (int)Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                    TenTinhTrang = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham.GetDescription(),
                    SoLan = 1, // dịch vụ khám mặc định là 1 lần,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                    IsDichVuBatBuoc = item.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa
                                      || item.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                      || item.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat
                                      || item.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                      || item.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat,
                }).Union(_goiKhamSucKhoeDichVuDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.GoiKhamSucKhoeId == hopDongKhamNhanVien.GoiKhamSucKhoeId
                                && ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                                && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                                && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                    .Select(item => new TiepNhanDichVuChiDinhVo()
                    {
                        LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),//GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId),
                        //LoaiDichVu = item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ma == "XN" ? Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem : (item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ma == "CĐHA" ? Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh : Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang),
                        //TenNhomDichVu = item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha != null ? item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten : item.DichVuKyThuatBenhVien.NhomDichVuBenhVien.Ten,
                        TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        Ten = item.DichVuKyThuatBenhVien.Ten,
                        Ma = item.DichVuKyThuatBenhVien.Ma,
                        DonGiaBenhVien = item.DonGiaBenhVien,
                        DonGiaMoi = item.DonGiaBenhVien,
                        DonGiaUuDai = item.DonGiaUuDai,
                        DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                        GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                        LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId,
                        TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                        NoiThucHienId = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                        TenNoiThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                        TinhTrang = (int)Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        TenTinhTrang = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien.GetDescription(),
                        SoLan = item.SoLan
                    }))

                //BVHD-3618
                .Union(
                    _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.TableNoTracking
                        .Where(x => x.HopDongKhamSucKhoeNhanVienId == hopDongQueryInfo.HopDongKhamSucKhoeNhanVienId)
                    .Select(item => new TiepNhanDichVuChiDinhVo()
                    {
                        TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        Ten = item.DichVuKhamBenhBenhVien.Ten,
                        Ma = item.DichVuKhamBenhBenhVien.Ma,
                        DonGiaBenhVien = item.GoiKhamSucKhoeDichVuKhamBenh.DonGiaBenhVien,
                        DonGiaMoi = item.GoiKhamSucKhoeDichVuKhamBenh.DonGiaBenhVien,
                        DonGiaUuDai = item.GoiKhamSucKhoeDichVuKhamBenh.DonGiaUuDai,
                        DonGiaChuaUuDai = item.GoiKhamSucKhoeDichVuKhamBenh.DonGiaChuaUuDai,
                        GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                        LoaiGiaId = item.GoiKhamSucKhoeDichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId,
                        TenLoaiGia = item.GoiKhamSucKhoeDichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVien.Ten,
                        NoiThucHienId = item.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                        TenNoiThucHien = item.GoiKhamSucKhoeDichVuKhamBenh.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                        TinhTrang = (int)Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                        TenTinhTrang = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham.GetDescription(),
                        SoLan = 1, // dịch vụ khám mặc định là 1 lần,
                        ChuyenKhoaKhamSucKhoe = item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe,
                        IsDichVuBatBuoc = item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa
                                      || item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                      || item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat
                                      || item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                      || item.GoiKhamSucKhoeDichVuKhamBenh.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat,
                        LaGoiChung = true,
                        GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId = item.Id
                    }))
                .Union(
                    _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.TableNoTracking
                        .Where(x => x.HopDongKhamSucKhoeNhanVienId == hopDongQueryInfo.HopDongKhamSucKhoeNhanVienId)
                    .Select(item => new TiepNhanDichVuChiDinhVo()
                    {
                        LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                        TenGoiKhamSucKhoe = item.GoiKhamSucKhoe.Ten,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        Ten = item.DichVuKyThuatBenhVien.Ten,
                        Ma = item.DichVuKyThuatBenhVien.Ma,
                        DonGiaBenhVien = item.GoiKhamSucKhoeDichVuDichVuKyThuat.DonGiaBenhVien,
                        DonGiaMoi = item.GoiKhamSucKhoeDichVuDichVuKyThuat.DonGiaBenhVien,
                        DonGiaUuDai = item.GoiKhamSucKhoeDichVuDichVuKyThuat.DonGiaUuDai,
                        DonGiaChuaUuDai = item.GoiKhamSucKhoeDichVuDichVuKyThuat.DonGiaChuaUuDai,
                        GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                        LoaiGiaId = item.GoiKhamSucKhoeDichVuDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId,
                        TenLoaiGia = item.GoiKhamSucKhoeDichVuDichVuKyThuat.NhomGiaDichVuKyThuatBenhVien.Ten,
                        NoiThucHienId = item.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVienId).FirstOrDefault(),
                        TenNoiThucHien = item.GoiKhamSucKhoeDichVuDichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                        TinhTrang = (int)Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                        TenTinhTrang = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien.GetDescription(),
                        SoLan = item.GoiKhamSucKhoeDichVuDichVuKyThuat.SoLan,
                        LaGoiChung = true,
                        GoiKhamSucKhoeChungDichVuKyThuatNhanVienId = item.Id
                    })
                )
                .OrderBy(x => x.TenNhomDichVu)
                .ToListAsync();
            return query;
        }

        public async Task<GridDataSource> GetDataDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var timKiemNangCaoObj = new DichVuChiDinhKhamSucKhoeBenhNhanQueryVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuChiDinhKhamSucKhoeBenhNhanQueryVo>(queryInfo.AdditionalSearchString);
            }

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.YeuCauTiepNhanId
                            //&& ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId != timKiemNangCaoObj.GoiKhamSucKhoeId) 
                            //    || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId == timKiemNangCaoObj.GoiKhamSucKhoeId))

                            // update tách riêng biệt grid chỉ định ngoài gói và trong gói
                            && ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId == null)
                                || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId != null))
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(item => new TiepNhanDichVuChiDinhVo()
                {
                    Id = item.Id,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    Ten = item.DichVuKhamBenhBenhVien.Ten,
                    Ma = item.DichVuKhamBenhBenhVien.Ma,
                    DonGiaBenhVien = item.Gia,
                    DonGiaMoi = item.Gia,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                    TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NoiThucHienId = item.NoiDangKyId,// item.NoiThucHienId,
                    TenNoiThucHien = item.NoiDangKy.Ten, // item.NoiThucHien.Ten,
                    TinhTrang = (int)item.TrangThai,
                    TenTinhTrang = item.TrangThai.GetDescription(),
                    SoLan = 1, // dịch vụ khám mặc định là 1 lần
                    IsDichVuBatBuoc = item.DichVuKhamBenhBenhVien.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa
                                      || item.DichVuKhamBenhBenhVien.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa
                                      || item.DichVuKhamBenhBenhVien.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.Mat
                                      || item.DichVuKhamBenhBenhVien.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong
                                      || item.DichVuKhamBenhBenhVien.ChuyenKhoaKhamSucKhoe == Enums.ChuyenKhoaKhamSucKhoe.RangHamMat,
                    ChuyenKhoaKhamSucKhoe = item.ChuyenKhoaKhamSucKhoe,
                    //TenGoiKhamSucKhoe = item.GoiKhamSucKhoe != null ? item.GoiKhamSucKhoe.Ten : null,
                    TenGoiKhamSucKhoe = item.GoiKhamSucKhoe != null ? item.GoiKhamSucKhoe.Ten : (item.GoiKhamSucKhoeDichVuPhatSinhId != null ? item.GoiKhamSucKhoeDichVuPhatSinh.Ten : null),
                    ThoiDiemChiDinh = item.ThoiDiemChiDinh,
                    // cập nhật 3668 
                    NhomId = (int)EnumNhomGoiDichVu.DichVuKhamBenh,

                    //BVHD-3668
                    GoiKhamSucKhoeDichVuPhatSinhId = item.GoiKhamSucKhoeDichVuPhatSinhId,
                    LaGoiChung = item.GoiKhamSucKhoe.GoiChung == true,
                    GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId =
                        item.GoiKhamSucKhoe.GoiChung == true ?
                            _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.TableNoTracking
                                .Where(x => x.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId.Value 
                                            && x.DichVuKhamBenhBenhVienId == item.DichVuKhamBenhBenhVienId
                                            && x.HopDongKhamSucKhoeNhanVienId == item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId)
                                .Select(x => x.Id).FirstOrDefault() : (long?)null
                }).Union(
                    _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.YeuCauTiepNhanId
                            //&& ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId != timKiemNangCaoObj.GoiKhamSucKhoeId)
                            //    || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId == timKiemNangCaoObj.GoiKhamSucKhoeId))

                            // update tách riêng biệt grid chỉ định ngoài gói và trong gói
                            && ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId == null)
                                || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId != null))
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                        .Select(item => new TiepNhanDichVuChiDinhVo()
                        {
                            Id = item.Id,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            Ten = item.DichVuKyThuatBenhVien.Ten,
                            Ma = item.DichVuKyThuatBenhVien.Ma,
                            DonGiaBenhVien = item.Gia,
                            DonGiaMoi = item.Gia,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            NoiThucHienId = item.NoiThucHienId,
                            TenNoiThucHien = item.NoiThucHien.Ten,
                            TinhTrang = (int)item.TrangThai,
                            TenTinhTrang = item.TrangThai.GetDescription(),
                            SoLan = item.SoLan,
                            //TenGoiKhamSucKhoe = item.GoiKhamSucKhoe != null ? item.GoiKhamSucKhoe.Ten : null,
                            TenGoiKhamSucKhoe = item.GoiKhamSucKhoe != null ? item.GoiKhamSucKhoe.Ten : (item.GoiKhamSucKhoeDichVuPhatSinhId != null ? item.GoiKhamSucKhoeDichVuPhatSinh.Ten : null),
                            ThoiDiemChiDinh = item.ThoiDiemChiDinh,
                            LaDichVuVacxin = nhomTiemChungId != null && item.NhomDichVuBenhVienId == nhomTiemChungId && item.YeuCauDichVuKyThuatKhamSangLocTiemChung != null,
                            NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,

                            //BVHD-3668
                            GoiKhamSucKhoeDichVuPhatSinhId = item.GoiKhamSucKhoeDichVuPhatSinhId,
                            LaGoiChung = item.GoiKhamSucKhoe.GoiChung == true,
                            GoiKhamSucKhoeChungDichVuKyThuatNhanVienId =
                                item.GoiKhamSucKhoe.GoiChung == true ?
                                _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.TableNoTracking
                                    .Where(x => x.GoiKhamSucKhoeId == item.GoiKhamSucKhoeId.Value 
                                                && x.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId
                                                && x.HopDongKhamSucKhoeNhanVienId == item.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId)
                                    .Select(x => x.Id).FirstOrDefault() : (long?)null
                        }));

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                // bỏ phân trang
                //.Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalDichVuChiDinhKhamSucKhoeNhanVienForGridAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new DichVuChiDinhKhamSucKhoeBenhNhanQueryVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuChiDinhKhamSucKhoeBenhNhanQueryVo>(queryInfo.AdditionalSearchString);
            }

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();
            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.YeuCauTiepNhanId
                            //&& ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId != timKiemNangCaoObj.GoiKhamSucKhoeId)
                            //    || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId == timKiemNangCaoObj.GoiKhamSucKhoeId))

                            // update tách riêng biệt grid chỉ định ngoài gói và trong gói
                            && ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId == null)
                                || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId != null))
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .Select(item => new TiepNhanDichVuChiDinhVo()
                {
                    Id = item.Id,
                    DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                    Ten = item.DichVuKhamBenhBenhVien.Ten,
                    Ma = item.DichVuKhamBenhBenhVien.Ma,
                    DonGiaBenhVien = item.Gia,
                    DonGiaMoi = item.Gia,
                    DonGiaUuDai = item.DonGiaUuDai,
                    DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                    GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                    LoaiGiaId = item.NhomGiaDichVuKhamBenhBenhVienId,
                    TenLoaiGia = item.NhomGiaDichVuKhamBenhBenhVien.Ten,
                    NoiThucHienId = item.NoiThucHienId,
                    TenNoiThucHien = item.NoiThucHien.Ten,
                    TinhTrang = (int)item.TrangThai,
                    TenTinhTrang = item.TrangThai.GetDescription(),
                    SoLan = 1 // dịch vụ khám mặc định là 1 lần
                }).Union(
                    _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.YeuCauTiepNhanId
                            //&& ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId != timKiemNangCaoObj.GoiKhamSucKhoeId)
                            //    || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId == timKiemNangCaoObj.GoiKhamSucKhoeId))

                            // update tách riêng biệt grid chỉ định ngoài gói và trong gói
                            && ((timKiemNangCaoObj.LaDichVuThem == true && x.GoiKhamSucKhoeId == null)
                                || (timKiemNangCaoObj.LaDichVuThem != true && x.GoiKhamSucKhoeId != null))
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                        .Select(item => new TiepNhanDichVuChiDinhVo()
                        {
                            Id = item.Id,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            Ten = item.DichVuKyThuatBenhVien.Ten,
                            Ma = item.DichVuKyThuatBenhVien.Ma,
                            DonGiaBenhVien = item.Gia,
                            DonGiaMoi = item.Gia,
                            DonGiaUuDai = item.DonGiaUuDai,
                            DonGiaChuaUuDai = item.DonGiaChuaUuDai,
                            GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,
                            LoaiGiaId = item.NhomGiaDichVuKyThuatBenhVienId,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            NoiThucHienId = item.NoiThucHienId,
                            TenNoiThucHien = item.NoiThucHien.Ten,
                            TinhTrang = (int)item.TrangThai,
                            TenTinhTrang = item.TrangThai.GetDescription(),
                            SoLan = item.SoLan
                        }));

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Get data

        public async Task<int> GetSoLuongConLaiDichVuTrongGoiKhamSucKhoeAsync(long goiKhamSucKhoeId, long dichVuBenhVienId, Enums.EnumNhomGoiDichVu nhomDichVu, long? yeuCauTiepNhanId = null, bool laGoiChung = true)
        {
            var soLuongConLai = 0;
            yeuCauTiepNhanId = yeuCauTiepNhanId ?? 0;
            //var goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
            //    .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(y => y.DichVuKhamBenhBenhVien).ThenInclude(z => z.YeuCauKhamBenhs)
            //    .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien).ThenInclude(z => z.YeuCauDichVuKyThuats)
            //    .Where(x => x.Id == goiKhamSucKhoeId).First();
            var goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
                .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs)
                .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats)
                .Where(x => x.Id == goiKhamSucKhoeId).First();
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenhs)
                .Where(x => x.Id == yeuCauTiepNhanId).FirstOrDefault();



            if (laGoiChung && goiKhamSucKhoe.GoiChung != true)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.GoiKhamSucKhoe.KhongPhaiGoiChung"));
            }
            if (nhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                var goiKhamSucKhoeDichVuKhamBenh = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.First(x => x.DichVuKhamBenhBenhVienId == dichVuBenhVienId);
                var soLuongDaDung = 0;
                if (yeuCauTiepNhan != null)
                {
                    soLuongDaDung = yeuCauTiepNhan.YeuCauKhamBenhs
                                        .Count(x => x.DichVuKhamBenhBenhVienId == dichVuBenhVienId
                                                    && x.GoiKhamSucKhoeId == goiKhamSucKhoeId
                                                    && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);
                }
                soLuongConLai = 1 - soLuongDaDung;
            }
            else if (nhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
            {
                var goiKhamSucKhoeDichVuDichVuKyThuat = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.First(x => x.DichVuKyThuatBenhVienId == dichVuBenhVienId);

                var soLuongDaDung = 0;
                if (yeuCauTiepNhan != null)
                {
                    soLuongDaDung = yeuCauTiepNhan.YeuCauDichVuKyThuats
                    .Where(x => x.DichVuKyThuatBenhVienId == dichVuBenhVienId
                                && x.GoiKhamSucKhoeId == goiKhamSucKhoeId
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Sum(x => x.SoLan);
                }

                soLuongConLai = goiKhamSucKhoeDichVuDichVuKyThuat.SoLan - soLuongDaDung;
            }
            return soLuongConLai;
        }

        public async Task<List<LookupItemDichVuMultiselectTemplateVo>> GetDonGiaTheoDichVuKhamSucKhoeAsync(List<LookupItemDichVuKhamBenhBVHoacDVKTBenhVienTemplateVo> lstDichVu, string goiChungStringQuery)
        {
            // hiện tại dịch vụ hệ thống chỉ cho chỉ định dv kỹ thuật, dịch vụ khám chỉ có trong gói chung
            var lstDichVuKemDonGia = new List<LookupItemDichVuMultiselectTemplateVo>();

            var lstDichVuKhamId = lstDichVu.Where(x => x.Loai == 1).Select(x => x.KeyId).Distinct().ToList();
            var lstDichVuKyThuatId = lstDichVu.Where(x => x.Loai == 2).Select(x => x.KeyId).Distinct().ToList();

            var lstDonGiaDichVuKhamBenhGoiChung = new List<LookupItemDichVuMultiselectTemplateVo>();
            var lstDonGiaDichVuKyThuatBenhVien = new List<LookupItemDichVuMultiselectTemplateVo>();
            var lstDonGiaDichVuKyThuatGoiChung = new List<LookupItemDichVuMultiselectTemplateVo>();


            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var laGoiDichVuPhatSinh = false;

            // trường hợp là dịch vụ hệ thống
            if (string.IsNullOrEmpty(goiChungStringQuery))
            {
                lstDonGiaDichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienGiaBenhVienRepository.TableNoTracking
                    .Where(x => lstDichVuKyThuatId.Contains(x.DichVuKyThuatBenhVienId))
                    .GroupBy(x => new { x.DichVuKyThuatBenhVienId })
                    .Select(item => new LookupItemDichVuMultiselectTemplateVo()
                    {
                        DichVuBenhVienId = item.Key.DichVuKyThuatBenhVienId,
                        DonGia = item.Any(x => x.TuNgay.Date <= DateTime.Now.Date
                                                 && (x.DenNgay == null || DateTime.Now.Date <= x.DenNgay.Value.Date))
                                ? item.Where(x => x.TuNgay.Date <= DateTime.Now.Date
                                                 && (x.DenNgay == null || DateTime.Now.Date <= x.DenNgay.Value.Date))
                                    .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId).ThenBy(x => x.TuNgay).Select(x => x.Gia).FirstOrDefault()
                                : (decimal?)null
                    }).ToListAsync();
            }
            // trường hợp là dv trong gói chung
            else
            {
                var hopDongQueryInfo = JsonConvert.DeserializeObject<TiepNhanDichVuChiDinhQueryVo>(goiChungStringQuery);
                var hopDongKhamSucKhoeNhanVien = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .FirstOrDefault(x => x.Id == hopDongQueryInfo.HopDongKhamSucKhoeNhanVienId);
                var goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
                    .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs)
                    .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats)
                    .FirstOrDefault(x => x.Id == hopDongQueryInfo.GoiKhamSucKhoeId);

                if (hopDongKhamSucKhoeNhanVien != null && goiKhamSucKhoe != null)
                {
                    lstDonGiaDichVuKhamBenhGoiChung = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs
                    .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                                && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                                && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                    .Select(item => new LookupItemDichVuMultiselectTemplateVo()
                    {
                        DichVuBenhVienId = item.DichVuKhamBenhBenhVienId,
                        //GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,

                        //BVHD-3668
                        GoiKhamSucKhoeId = goiKhamSucKhoe.GoiDichVuPhatSinh != true ? item.GoiKhamSucKhoeId : (long?)null,
                        GoiKhamSucKhoeDichVuPhatSinhId = goiKhamSucKhoe.GoiDichVuPhatSinh == true ? item.GoiKhamSucKhoeId : (long?)null,

                        DonGia = item.DonGiaUuDai
                    })
                    .ToList();

                    lstDonGiaDichVuKyThuatGoiChung = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats
                        .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                    && ((!x.CoMangThai && !x.KhongMangThai) || hopDongQueryInfo.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongQueryInfo.CoMangThai) || (x.KhongMangThai && !hopDongQueryInfo.CoMangThai))
                                    && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongQueryInfo.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongQueryInfo.DaLapGiaDinh))
                                    && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (hopDongQueryInfo.Tuoi != null && ((x.SoTuoiTu == null || hopDongQueryInfo.Tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || hopDongQueryInfo.Tuoi <= x.SoTuoiDen)))))
                        .Select(item => new LookupItemDichVuMultiselectTemplateVo()
                        {
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            //GoiKhamSucKhoeId = item.GoiKhamSucKhoeId,

                            //BVHD-3668
                            GoiKhamSucKhoeId = goiKhamSucKhoe.GoiDichVuPhatSinh != true ? item.GoiKhamSucKhoeId : (long?)null,
                            GoiKhamSucKhoeDichVuPhatSinhId = goiKhamSucKhoe.GoiDichVuPhatSinh == true ? item.GoiKhamSucKhoeId : (long?)null,

                            DonGia = item.DonGiaUuDai
                        })
                    .ToList();


                    //BVHD-3668
                    laGoiDichVuPhatSinh = goiKhamSucKhoe.GoiDichVuPhatSinh == true;
                }
            }

            foreach (var dichVuLookup in lstDichVu)
            {
                var donGia = new LookupItemDichVuMultiselectTemplateVo();
                if (dichVuLookup.Loai == 1) // dv khám
                {
                    donGia = lstDonGiaDichVuKhamBenhGoiChung.FirstOrDefault(x => x.DichVuBenhVienId == dichVuLookup.KeyId);
                }
                else
                {
                    donGia = lstDonGiaDichVuKyThuatGoiChung.FirstOrDefault(x => x.DichVuBenhVienId == dichVuLookup.KeyId);
                    if (donGia == null)
                    {
                        donGia = lstDonGiaDichVuKyThuatBenhVien.FirstOrDefault(x => x.DichVuBenhVienId == dichVuLookup.KeyId);
                    }
                }

                var item = new LookupItemDichVuMultiselectTemplateVo()
                {
                    KeyId = JsonConvert.SerializeObject(new KeyIdStringDichVuKhamSucKhoeVo()
                    {
                        DichVuId = dichVuLookup.KeyId,
                        GoiKhamSucKhoeId = donGia?.GoiKhamSucKhoeId,
                        NhomDichVu = (Enums.EnumNhomGoiDichVu)dichVuLookup.Loai,

                        //BVHD-3668
                        DonGia = laGoiDichVuPhatSinh ? donGia?.DonGia : (decimal?)null,
                        GoiKhamSucKhoeDichVuPhatSinhId = donGia?.GoiKhamSucKhoeDichVuPhatSinhId
                    }),
                    DisplayName = dichVuLookup.DisplayName,
                    Ten = dichVuLookup.Ten,
                    Ma = dichVuLookup.Ma,
                    DonGia = donGia?.DonGia
                };
                lstDichVuKemDonGia.Add(item);
            }
            return lstDichVuKemDonGia;
        }

        public async Task KiemTraSoLuongConLaiNhieuDichVuTrongGoiKhamSucKhoeAsync(long goiKhamSucKhoeId, List<string> dichVuThems, List<TiepNhanDichVuChiDinhVo> dichVuGois, long? yeuCauTiepNhanId = null)
        {
            yeuCauTiepNhanId = yeuCauTiepNhanId ?? 0;
            var goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
                .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(y => y.DichVuKhamBenhBenhVien)//.ThenInclude(z => z.YeuCauKhamBenhs)
                .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)//.ThenInclude(z => z.YeuCauDichVuKyThuats)
                .Where(x => x.Id == goiKhamSucKhoeId).First();

            if (goiKhamSucKhoe.GoiChung != true && goiKhamSucKhoe.GoiDichVuPhatSinh != true)
            {
                throw new Exception(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.GoiKhamSucKhoe.KhongPhaiGoiChung"));
            }

            var lstYeuCauKhamTrongGoi = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && (x.GoiKhamSucKhoeId == goiKhamSucKhoeId || x.GoiKhamSucKhoeDichVuPhatSinhId == goiKhamSucKhoeId))
                .Select(x => new { x.Id, x.DichVuKhamBenhBenhVienId, x.DichVuKhamBenhBenhVien.Ten })
                .ToList();

            var lstYeuCauKyThuatTrongGoi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (x.GoiKhamSucKhoeId == goiKhamSucKhoeId || x.GoiKhamSucKhoeDichVuPhatSinhId == goiKhamSucKhoeId))
                .Select(x => new { x.Id, x.DichVuKyThuatBenhVienId, x.SoLan, x.DichVuKyThuatBenhVien.Ten })
                .ToList();

            foreach (var dichVu in dichVuThems)
            {
                var dichVuObj = JsonConvert.DeserializeObject<KeyIdStringDichVuKhamSucKhoeVo>(dichVu);

                if (dichVuObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    var goiKhamSucKhoeDichVuKhamBenh = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.First(x => x.DichVuKhamBenhBenhVienId == dichVuObj.DichVuId);
                    //var soLuongDaDung = goiKhamSucKhoeDichVuKhamBenh.DichVuKhamBenhBenhVien.YeuCauKhamBenhs
                    //    .Count(x => x.DichVuKhamBenhBenhVienId == dichVuObj.DichVuId
                    //                && x.GoiKhamSucKhoeId == goiKhamSucKhoeId
                    //                && x.YeuCauTiepNhanId == yeuCauTiepNhanId
                    //                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham);

                    var soLuongDaDung = lstYeuCauKhamTrongGoi.Count(x => x.DichVuKhamBenhBenhVienId == dichVuObj.DichVuId);
                    var soLanChuaLuu = dichVuGois.Count(x => x.LoaiDichVu == NhomDichVuChiDinhKhamSucKhoe.KhamBenh
                                                             && x.DichVuBenhVienId == dichVuObj.DichVuId
                                                             && x.GoiKhamSucKhoeId == goiKhamSucKhoeId);
                    var soLuongConLai = 1 - soLuongDaDung - soLanChuaLuu;
                    if (soLuongConLai <= 0)
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), goiKhamSucKhoeDichVuKhamBenh.DichVuKhamBenhBenhVien.Ten, soLuongConLai));
                    }
                }
                else if (dichVuObj.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    var goiKhamSucKhoeDichVuDichVuKyThuat = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.First(x => x.DichVuKyThuatBenhVienId == dichVuObj.DichVuId);
                    //var soLuongDaDung = goiKhamSucKhoeDichVuDichVuKyThuat.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats
                    //    .Where(x => x.DichVuKyThuatBenhVienId == dichVuObj.DichVuId
                    //                && x.GoiKhamSucKhoeId == goiKhamSucKhoeId
                    //                && x.YeuCauTiepNhanId == yeuCauTiepNhanId
                    //                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    //    .Sum(x => x.SoLan);

                    var soLuongDaDung = lstYeuCauKyThuatTrongGoi.Where(x => x.DichVuKyThuatBenhVienId == dichVuObj.DichVuId).Sum(x => x.SoLan);
                    var soLanChuaLuu = dichVuGois.Count(x => x.LoaiDichVu != NhomDichVuChiDinhKhamSucKhoe.KhamBenh
                                                             && x.DichVuBenhVienId == dichVuObj.DichVuId
                                                             && x.GoiKhamSucKhoeId == goiKhamSucKhoeId);
                    var soLuongConLai = goiKhamSucKhoeDichVuDichVuKyThuat.SoLan - soLuongDaDung - soLanChuaLuu;
                    if (soLuongConLai <= 0)
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), goiKhamSucKhoeDichVuDichVuKyThuat.DichVuKyThuatBenhVien.Ten, soLuongConLai));
                    }
                }
            }
        }

        public async Task KiemTraTrungDichVuKhamTrongGoiKhamSucKhoeAsync(List<string> dichVuThems, List<TiepNhanDichVuChiDinhVo> dichVuGois, List<TiepNhanDichVuChiDinhVo> dichVuNgoaiGois, long? yeuCauTiepNhanId = null)
        {
            var lstDichVu = new List<long>();
            foreach (var dichVu in dichVuThems)
            {
                var dichVuObj = JsonConvert.DeserializeObject<KeyIdStringDichVuKhamSucKhoeVo>(dichVu);
                if (dichVuObj.NhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    lstDichVu.Add(dichVuObj.DichVuId);

                    var dvChiDinhTrenGrid = dichVuGois.Where(x => x.DichVuBenhVienId != null).FirstOrDefault(x => x.LoaiDichVu == NhomDichVuChiDinhKhamSucKhoe.KhamBenh && x.DichVuBenhVienId == dichVuObj.DichVuId);
                    var dvChiDinhNgoaiGoiTrenGrid = dichVuNgoaiGois.Where(x => x.DichVuBenhVienId != null).FirstOrDefault(x => x.LoaiDichVu == NhomDichVuChiDinhKhamSucKhoe.KhamBenh && x.DichVuBenhVienId == dichVuObj.DichVuId);
                    if (dvChiDinhTrenGrid != null || dvChiDinhNgoaiGoiTrenGrid != null)
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKhamBenh.IsExists"), dvChiDinhTrenGrid?.Ten ?? dvChiDinhNgoaiGoiTrenGrid?.Ten ?? ""));
                    }
                }
            }

            // check thêm trường hợp nhiều user cùng khám cho 1 người bệnh => dv user này thêm mà user khác chưa load lại page thì sẽ ko hiện
            if (yeuCauTiepNhanId != null && yeuCauTiepNhanId != 0 && lstDichVu.Any())
            {
                var dvGoiIds = dichVuGois
                    .Where(x => x.DichVuBenhVienId != null && x.LoaiDichVu == NhomDichVuChiDinhKhamSucKhoe.KhamBenh)
                    .Select(x => x.DichVuBenhVienId).ToList();

                //BVHD-3668
                dvGoiIds.AddRange(dichVuNgoaiGois
                    .Where(x => x.DichVuBenhVienId != null && x.LoaiDichVu == NhomDichVuChiDinhKhamSucKhoe.KhamBenh)
                    .Select(x => x.DichVuBenhVienId).ToList());

                var yeuCauKhamBenhDaChiDinhs = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                && !dvGoiIds.Contains(x.DichVuKhamBenhBenhVienId))
                    .Select(x => new { x.DichVuKhamBenhBenhVienId, x.TenDichVu })
                    .Distinct()
                    .ToList();
                foreach (var dichVuKham in yeuCauKhamBenhDaChiDinhs)
                {
                    if (lstDichVu.Any(x => x == dichVuKham.DichVuKhamBenhBenhVienId))
                    {
                        throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKhamBenh.IsExists"), dichVuKham.TenDichVu));
                    }
                }
            }
        }

        public async Task<YeuCauTiepNhan> GetYeucauTiepNhanDungDeLuuDichVuAsync(long? yeuCauTiepNhanId, long? yeuCauTiepNhanKSKId, long? benhNhanId = null)
        {
            yeuCauTiepNhanId = yeuCauTiepNhanId == 0 ? null : yeuCauTiepNhanId;
            yeuCauTiepNhanKSKId = yeuCauTiepNhanKSKId == 0 ? null : yeuCauTiepNhanKSKId;
            benhNhanId = benhNhanId == 0 ? null : benhNhanId;

            var yctnNgoaiTruTuKSK = BaseRepository.Table
                .Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets)
                .FirstOrDefault(x => (benhNhanId == null || x.BenhNhanId == benhNhanId)
                                     && (yeuCauTiepNhanId == null || x.Id == yeuCauTiepNhanId)
                                     && (yeuCauTiepNhanKSKId == null || x.YeuCauTiepNhanKhamSucKhoeId == yeuCauTiepNhanKSKId)
                                        
                                     && x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien
                                     && x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                     && x.YeuCauTiepNhanKhamSucKhoeId != null);

            //if ((yeuCauTiepNhanId != null || yeuCauTiepNhanKSKId != null) && yctnNgoaiTruTuKSK == null)
            //{
            //    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            //}

            var maYCTNLeTan = BaseRepository.TableNoTracking
                .Where(x => (benhNhanId == null || x.BenhNhanId == benhNhanId)
                            && (yeuCauTiepNhanId == null || x.Id == yeuCauTiepNhanId)
                            && (yeuCauTiepNhanKSKId == null || x.YeuCauTiepNhanKhamSucKhoeId == yeuCauTiepNhanKSKId)

                            && x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien
                            && x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                            && x.YeuCauTiepNhanKhamSucKhoeId == null)
                .Select(x => x.MaYeuCauTiepNhan).FirstOrDefault();
            //var yctnNgoaiTruTuKSK = yctns.FirstOrDefault(x => (yeuCauTiepNhanId == null || x.Id == yeuCauTiepNhanId)
            //                                                  && (yeuCauTiepNhanKSKId == null || x.YeuCauTiepNhanKhamSucKhoeId == yeuCauTiepNhanKSKId)

            //                                                  //dùng cho trường hợp đã bắt đầu khám
            //                                                  && x.YeuCauTiepNhanKhamSucKhoeId != null);
            if (benhNhanId != null && !string.IsNullOrEmpty(maYCTNLeTan) && yctnNgoaiTruTuKSK == null)
            {
                throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.YeuCauTiepNhanLeTan.IsExists"), maYCTNLeTan));
            }

            return yctnNgoaiTruTuKSK;
        }
        #endregion

        #region Xử lý data

        public async Task XuLyLuuThongTinHopDongKhamNhanVienAsync(YeuCauTiepNhan yeuCauKhamSucKhoe, List<TiepNhanDichVuChiDinhVo> dichVus, bool coQuyenCapNhatHanhChinh = true,
            List<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhViens = null, List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhViens = null, List<TemplateDichVuKhamSucKhoe> templateDichVuKhamSucKhoes = null, Core.Domain.Entities.ICDs.ICD icdKhamSucKhoe = null)
        {
            var now = DateTime.Now;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            // bổ sung ngày 06/12/2021: nhằm mục đích lưu dịch vụ ngoại trú
            YeuCauTiepNhan yeuCauTiepNhanNgoaiTru = null;
            var coThemDichVuVaoTiepNhanKSK = false;
            var coThemDichVuVaoTiepNhaNgoaiTru = false;
            var lanDauKiemTraYCTNNgoaiTru = true;

            if (yeuCauKhamSucKhoe.Id == 0)
            {
                yeuCauKhamSucKhoe.MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan();
                yeuCauKhamSucKhoe.LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe;
                yeuCauKhamSucKhoe.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien;
                yeuCauKhamSucKhoe.ThoiDiemTiepNhan = now;
                yeuCauKhamSucKhoe.ThoiDiemCapNhatTrangThai = now;
                yeuCauKhamSucKhoe.NoiTiepNhanId = phongHienTaiId;
                yeuCauKhamSucKhoe.NhanVienTiepNhanId = currentUserId;
            }

            if (yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan == null)
            {
                yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan = new BenhNhan();
                yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.YeuCauTiepNhans.Add(yeuCauKhamSucKhoe);
            }
            else
            {
                yeuCauKhamSucKhoe.BenhNhanId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhanId;
            }
            
            if (coQuyenCapNhatHanhChinh)
            {
                yeuCauKhamSucKhoe.HoTen = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.HoTen = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.HoTen;
                yeuCauKhamSucKhoe.NgaySinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.NgaySinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NgaySinh;
                yeuCauKhamSucKhoe.ThangSinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.ThangSinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.ThangSinh;
                yeuCauKhamSucKhoe.NamSinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.NamSinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NamSinh;
                yeuCauKhamSucKhoe.SoChungMinhThu = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.SoChungMinhThu = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.SoChungMinhThu;
                yeuCauKhamSucKhoe.GioiTinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.GioiTinh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.GioiTinh;
                yeuCauKhamSucKhoe.NgheNghiepId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.NgheNghiepId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NgheNghiepId;
                yeuCauKhamSucKhoe.QuocTichId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.QuocTichId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.QuocTichId;
                yeuCauKhamSucKhoe.DanTocId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.DanTocId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.DanTocId;
                yeuCauKhamSucKhoe.DiaChi = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.DiaChi = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.DiaChi;
                yeuCauKhamSucKhoe.PhuongXaId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.PhuongXaId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.PhuongXaId;
                yeuCauKhamSucKhoe.QuanHuyenId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.QuanHuyenId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.QuanHuyenId;
                yeuCauKhamSucKhoe.TinhThanhId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.TinhThanhId = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.TinhThanhId;
                yeuCauKhamSucKhoe.NhomMau = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.NhomMau = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NhomMau;
                yeuCauKhamSucKhoe.YeuToRh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.YeuToRh = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.YeuToRh;
                yeuCauKhamSucKhoe.Email = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.Email = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.Email;
                yeuCauKhamSucKhoe.SoDienThoai = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.SoDienThoai = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.SoDienThoai;
            }

            #region Xử lý thêm dịch vụ

            if (yeuCauKhamSucKhoe.Id == 0)
            {
                var goiKhamChiDinhThemIds = dichVus.Where(x => x.GoiKhamSucKhoeId != null
                                                            && x.GoiKhamSucKhoeId.Value != yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeId)
                                                .Select(x => x.GoiKhamSucKhoeId.Value).Distinct().ToList();
                if (goiKhamChiDinhThemIds.Any())
                {
                    var kiemTraGoiKhamChung = _goiKhamSucKhoeRepository.TableNoTracking
                                                .Where(x => goiKhamChiDinhThemIds.Contains(x.Id)).All(x => x.GoiChung == true);
                    if (!kiemTraGoiKhamChung)
                    {
                        throw new Exception(_localizationService.GetResource("ChiDinhDichVuKhamSucKhoeNhanVien.GoiKhamSucKhoe.KhongPhaiGoiChung"));
                    }
                }

                var lstChuyenKhoaKSKChinh = EnumHelper.GetListEnum<Enums.ChuyenKhoaKhamSucKhoe>().Select(item => (Enums.ChuyenKhoaKhamSucKhoe)item).ToList();
                
                foreach (var dichVu in dichVus)
                {
                    // thêm dịch vụ khám
                    if (dichVu.LaDichVuKham) //Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh)
                    {
                        //yeuCauKhamSucKhoe.YeuCauKhamBenhs.Add(await XuLyThemYeuCauKhamBenhAsync(dichVu));

                        var dichVuKhamBenhThemMoi = await XuLyThemYeuCauKhamBenhAsync(dichVu, dichVuKhamBenhBenhViens,templateDichVuKhamSucKhoes, icdKhamSucKhoe);
                        if (dichVuKhamBenhThemMoi.ChuyenKhoaKhamSucKhoe != null
                            && lstChuyenKhoaKSKChinh.Contains(dichVuKhamBenhThemMoi.ChuyenKhoaKhamSucKhoe.Value))
                        {
                            coThemDichVuVaoTiepNhanKSK = true;
                            yeuCauKhamSucKhoe.YeuCauKhamBenhs.Add(dichVuKhamBenhThemMoi);
                        }
                        else
                        {
                            coThemDichVuVaoTiepNhaNgoaiTru = true;
                            if (yeuCauTiepNhanNgoaiTru == null 
                                && (yeuCauKhamSucKhoe.Id != 0 || (yeuCauKhamSucKhoe.BenhNhanId != null && yeuCauKhamSucKhoe.BenhNhanId != 0))
                                && lanDauKiemTraYCTNNgoaiTru)
                            {
                                yeuCauTiepNhanNgoaiTru = await GetYeucauTiepNhanDungDeLuuDichVuAsync(null, yeuCauKhamSucKhoe.Id, yeuCauKhamSucKhoe.BenhNhanId);
                                lanDauKiemTraYCTNNgoaiTru = false;
                            }
                            XuLyThemDichVuKhamNgoaiChuyenKhoaChinh(yeuCauKhamSucKhoe, dichVuKhamBenhThemMoi, true, yeuCauTiepNhanNgoaiTru);
                        }
                    }
                    // thêm dịch vụ kỹ thuật
                    else
                    {
                        coThemDichVuVaoTiepNhanKSK = true;
                        yeuCauKhamSucKhoe.YeuCauDichVuKyThuats.Add(await XuLyThemYeuCauDichVuKyThuatAsync(dichVu, dichVuKyThuatBenhViens));
                    }
                }
            }

            #endregion

            #region Xử lý lưu tiền sử bệnh người bệnh

            if (yeuCauKhamSucKhoe.Id == 0 && !string.IsNullOrEmpty(yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh) && yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh != "[]")
            {
                var tienSuBenhs = JsonConvert.DeserializeObject<List<TienSuBenhNhanVien>>(yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.GhiChuTienSuBenh);
                if (tienSuBenhs != null)
                {
                    foreach (var tienSuBenh in tienSuBenhs)
                    {
                        var newTienSuBenh = new BenhNhanTienSuBenh()
                        {
                            TenBenh = tienSuBenh.TenBenh,
                            LoaiTienSuBenh = (Enums.EnumLoaiTienSuBenh)tienSuBenh.LoaiTienSuId
                        };
                        yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.BenhNhan.BenhNhanTienSuBenhs.Add(newTienSuBenh);
                    }
                }
            }
            #endregion

            if (yeuCauKhamSucKhoe.Id == 0)
            {
                var lstDichVuKham = yeuCauKhamSucKhoe.YeuCauKhamBenhs
                    .Select(x => new { x.TenDichVu, x.DichVuKhamBenhBenhVienId })
                    .GroupBy(x => x.DichVuKhamBenhBenhVienId)
                    .Select(x => new { TenDichVu = x.First().TenDichVu, SoLan = x.Count() })
                    .ToList();
                var lstDichVuKhamThemTrung = lstDichVuKham.Where(x => x.SoLan > 1).ToList();
                if (lstDichVuKhamThemTrung.Any())
                {
                    throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKhamBenh.IsExists"), string.Join(", ", lstDichVuKhamThemTrung.Select(x => x.TenDichVu).ToList())));
                }

                await PrepareDichVuAndAddAsync(yeuCauKhamSucKhoe);

                // trường hợp chưa bắt đầu khám, kiểm tra nếu có thêm dv ngoại trú
                if (coThemDichVuVaoTiepNhaNgoaiTru && yeuCauTiepNhanNgoaiTru != null)
                {
                    await PrepareDichVuAndAddAsync(yeuCauTiepNhanNgoaiTru);
                }
            }
            else
            {
                await PrepareForAddDichVuAndUpdateAsync(yeuCauKhamSucKhoe);
            }

            #region Xử lý nhóm đối tượng khám sức khỏe
            if (!string.IsNullOrEmpty(yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe))
            {
                var isExists = await _inputStringStoredRepository
                    .TableNoTracking
                    .AnyAsync(p => p.Value.Trim().ToLower() == yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe.Trim().ToLower()
                                   && p.Set == Enums.InputStringStoredKey.NhomDoiTuongKhamSucKhoe);
                if (!isExists)
                {
                    var inputStringStored = new Core.Domain.Entities.InputStringStoreds.InputStringStored
                    {
                        Set = Enums.InputStringStoredKey.NhomDoiTuongKhamSucKhoe,
                        Value = yeuCauKhamSucKhoe.HopDongKhamSucKhoeNhanVien.NhomDoiTuongKhamSucKhoe
                    };
                    await _inputStringStoredRepository.AddAsync(inputStringStored);
                }
            }
            #endregion
        }
        public List<DichVuKhamBenhBenhVien> GetDichVuKhamBenhBenhViens()
        {
            return _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenh)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens).ToList();
        }
        public List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> GetDichVuKyThuatBenhViens()
        {
            return _dichVuKyThuatBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKyThuat)
                .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens)
                .ToList();
        }
        public List<TemplateDichVuKhamSucKhoe> GetTemplateDichVuKhamSucKhoes()
        {
            return _templateDichVuKhamSucKhoeRepository.TableNoTracking
                .ToList();
        }
        public Core.Domain.Entities.ICDs.ICD GetIcdKhamSucKhoe()
        {
            var cauHinhIcdKhamSucKhoe = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.IcdKhamSucKhoe");
            long.TryParse(cauHinhIcdKhamSucKhoe?.Value, out long icdKhamSucKhoeId);
            var icdKhamSucKhoe = _icdRepository.TableNoTracking.FirstOrDefault(x => x.Id == icdKhamSucKhoeId);
            return icdKhamSucKhoe;
        }
        private async Task<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> XuLyThemYeuCauKhamBenhAsync(TiepNhanDichVuChiDinhVo dichVu, List<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhViens = null, List<TemplateDichVuKhamSucKhoe> templateDichVuKhamSucKhoes = null, Core.Domain.Entities.ICDs.ICD icdKhamSucKhoe = null)
        {
            var now = DateTime.Now;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            DichVuKhamBenhBenhVien dichVuKhamBenhChiDinh;
            if (dichVuKhamBenhBenhViens != null)
            {
                dichVuKhamBenhChiDinh = dichVuKhamBenhBenhViens.First(o => o.Id == dichVu.DichVuBenhVienId);
            }
            else
            {
                dichVuKhamBenhChiDinh = await _dichVuKhamBenhBenhVienRepository.Table
                .Include(x => x.DichVuKhamBenh)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
                .Where(x => x.Id == dichVu.DichVuBenhVienId).FirstAsync();
            }
            
            var giaBaoHiem = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBaoHiems
                .FirstOrDefault(x => x.TuNgay.Date <= now.Date
                                     && (x.DenNgay == null || x.DenNgay.Value.Date >= now.Date));

            TemplateDichVuKhamSucKhoe templateDichVuKhamSucKhoe;
            if(templateDichVuKhamSucKhoes != null)
            {
                templateDichVuKhamSucKhoe = templateDichVuKhamSucKhoes.FirstOrDefault(x => x.ChuyenKhoaKhamSucKhoe == dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe);
            }
            else
            {
                templateDichVuKhamSucKhoe = _templateDichVuKhamSucKhoeRepository.TableNoTracking.FirstOrDefault(x => x.ChuyenKhoaKhamSucKhoe == dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe);
            }            

            //todo: cân nhắc lưu default
            var thongTinKhamData = GetDataDefaultDichVuKhamSucKhoe(dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe); //string.Empty;
            //switch (dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe)
            //{
            //    case Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TuanHoan\",\"Value\":\"Bình thường\"},{\"Id\":\"TuanHoanPhanLoai\",\"Value\":1},{\"Id\":\"HoHap\",\"Value\":\"Bình thường\"},{\"Id\":\"HoHapPhanLoai\",\"Value\":1},{\"Id\":\"TieuHoa\",\"Value\":\"Bình thường\"},{\"Id\":\"TieuHoaPhanLoai\",\"Value\":1},{\"Id\":\"ThanTietLieu\",\"Value\":\"Bình thường\"},{\"Id\":\"ThanTietLieuPhanLoai\",\"Value\":1},{\"Id\":\"NoiTiet\",\"Value\":\"Bình thường\"},{\"Id\":\"NoiTietPhanLoai\",\"Value\":1},{\"Id\":\"CoXuongKhop\",\"Value\":\"Bình thường\"},{\"Id\":\"CoXuongKhopPhanLoai\",\"Value\":1},{\"Id\":\"ThanKinh\",\"Value\":\"Bình thường\"},{\"Id\":\"ThanKinhPhanLoai\",\"Value\":1},{\"Id\":\"TamThan\",\"Value\":\"Bình thường\"},{\"Id\":\"TamThanPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"NgoaiKhoa\",\"Value\":\"Bình thường\"},{\"Id\":\"NgoaiKhoaPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"SanPhuKhoa\",\"Value\":\"Bình thường\"},{\"Id\":\"SanPhuKhoaPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.Mat:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"Không\"},{\"Id\":\"MatPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.RangHamMat:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"HamTren\",\"Value\":\"Bình thường\"},{\"Id\":\"HamDuoi\",\"Value\":\"Bình thường\"},{\"Id\":\"CacBenhRangHamMat\",\"Value\":\"Không\"},{\"Id\":\"RangHamMatPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TaiPhaiNoiThuong\",\"Value\":\"5 mét\"},{\"Id\":\"TaiPhaiNoiTham\",\"Value\":\"0.5 mét\"},{\"Id\":\"TaiTraiNoiThuong\",\"Value\":\"5 mét\"},{\"Id\":\"TaiTraiNoiTham\",\"Value\":\"0.5 mét\"},{\"Id\":\"CacBenhTaiMuiHong\",\"Value\":\"Không\"},{\"Id\":\"TaiMuiHongPhanLoai\",\"Value\":1}]}";
            //        break;
            //    case Enums.ChuyenKhoaKhamSucKhoe.DaLieu:
            //        thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"DaLieu\",\"Value\":\"Bình thường\"},{\"Id\":\"DaLieuPhanLoai\",\"Value\":1}]}";
            //        break;
            //}
            //var goiKhamSucKhoe = await _goiKhamSucKhoeRepository.Table.FirstOrDefaultAsync(x => x.Id == dichVu.GoiKhamSucKhoeId);

            //BVHD-3598
            if(icdKhamSucKhoe == null)
            {
                var cauHinhIcdKhamSucKhoe = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.IcdKhamSucKhoe");
                long.TryParse(cauHinhIcdKhamSucKhoe?.Value, out long icdKhamSucKhoeId);
                icdKhamSucKhoe = _icdRepository.TableNoTracking.FirstOrDefault(x => x.Id == icdKhamSucKhoeId);
            }

            var entity = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh()
            {
                DichVuKhamBenhBenhVienId = dichVu.DichVuBenhVienId.Value,
                //DichVuKhamBenhBenhVien = dichVuKhamBenhChiDinh,
                NhomGiaDichVuKhamBenhBenhVienId = dichVu.LoaiGiaId.Value,
                MaDichVu = dichVu.Ma,
                MaDichVuTT37 = dichVuKhamBenhChiDinh.DichVuKhamBenh?.MaTT37,
                TenDichVu = dichVu.Ten,
                Gia = dichVu.DonGiaMoi.Value,
                DuocHuongBaoHiem = false,
                KhongTinhPhi = false,

                NhanVienChiDinhId = currentUserId,
                NoiChiDinhId = phongHienTaiId,
                ThoiDiemChiDinh = now,

                NoiDangKyId = dichVu.NoiThucHienId,
                ThoiDiemDangKy = now,
                BacSiDangKyId = currentUserId,

                NoiThucHienId = dichVu.NoiThucHienId,

                GoiKhamSucKhoeId = dichVu.GoiKhamSucKhoeId,
                //GoiKhamSucKhoe = goiKhamSucKhoe,
                ChuyenKhoaKhamSucKhoe = dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe,
                DonGiaUuDai = dichVu.DonGiaUuDai,
                DonGiaChuaUuDai = dichVu.DonGiaChuaUuDai,
                TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                BaoHiemChiTra = null,

                ThongTinKhamTheoDichVuTemplate = templateDichVuKhamSucKhoe?.ComponentDynamics,
                ThongTinKhamTheoDichVuData = thongTinKhamData,

                //BVHD-3598
                ChanDoanSoBoICDId = icdKhamSucKhoe?.Id,
                ChanDoanSoBoGhiChu = icdKhamSucKhoe?.TenTiengViet,

                //BVHD-3668
                GoiKhamSucKhoeDichVuPhatSinhId = dichVu.GoiKhamSucKhoeDichVuPhatSinhId,
                GiaBenhVienTaiThoiDiemChiDinh =
                    dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBenhViens
                        .Any(x => x.NhomGiaDichVuKhamBenhBenhVienId == dichVu.LoaiGiaId.Value
                                  && x.TuNgay.Date <= now.Date
                                  && (x.DenNgay == null || x.DenNgay.Value.Date >= now.Date))
                        ? dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBenhViens.First(x => x.NhomGiaDichVuKhamBenhBenhVienId == dichVu.LoaiGiaId.Value
                                                                                            && x.TuNgay.Date <= now.Date
                                                                                            && (x.DenNgay == null || x.DenNgay.Value.Date >= now.Date)).Gia
                        : (decimal?)null,
            };

            if (giaBaoHiem != null)
            {
                entity.DonGiaBaoHiem = giaBaoHiem.Gia;
                entity.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;
            }

            return entity;
        }
        private async Task<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat> XuLyThemYeuCauDichVuKyThuatAsync(TiepNhanDichVuChiDinhVo dichVu, List<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhViens = null)
        {
            var now = DateTime.Now;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien dichKyThuatChiDinh;
            if(dichVuKyThuatBenhViens != null)
            {
                dichKyThuatChiDinh = dichVuKyThuatBenhViens.First(o => o.Id == dichVu.DichVuBenhVienId);
            }
            else
            {
                dichKyThuatChiDinh = await _dichVuKyThuatBenhVienRepository.Table
                .Include(x => x.DichVuKyThuat)
                .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Where(x => x.Id == dichVu.DichVuBenhVienId).FirstAsync();
            }
            
            //var goiKhamSucKhoe = await _goiKhamSucKhoeRepository.Table.FirstOrDefaultAsync(x => x.Id == dichVu.GoiKhamSucKhoeId);

            var entity = new Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat()
            {
                DichVuKyThuatBenhVienId = dichVu.DichVuBenhVienId.Value,
                //DichVuKyThuatBenhVien = dichKyThuatChiDinh,
                NhomGiaDichVuKyThuatBenhVienId = dichVu.LoaiGiaId.Value,
                MaDichVu = dichVu.Ma,
                Ma4350DichVu = dichKyThuatChiDinh.DichVuKyThuat?.Ma4350,
                MaGiaDichVu = dichKyThuatChiDinh.DichVuKyThuat?.MaGia,
                TenDichVu = dichVu.Ten,
                TenTiengAnhDichVu = dichKyThuatChiDinh.DichVuKyThuat?.TenTiengAnh,
                TenGiaDichVu = dichKyThuatChiDinh.DichVuKyThuat?.TenGia,
                Gia = dichVu.DonGiaMoi.Value,
                SoLan = dichVu.SoLan.Value,
                DuocHuongBaoHiem = false,

                LoaiDichVuKyThuat = dichVu.LoaiDichVuKyThuat.Value,//GetLoaiDichVuKyThuat(dichKyThuatChiDinh.NhomDichVuBenhVienId),
                NhomChiPhi = dichKyThuatChiDinh.DichVuKyThuat?.NhomChiPhi ?? Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                NhomDichVuBenhVienId = dichKyThuatChiDinh.NhomDichVuBenhVienId,

                NhanVienChiDinhId = currentUserId,
                NoiChiDinhId = phongHienTaiId,
                ThoiDiemChiDinh = now,

                ThoiDiemDangKy = now,
                NoiThucHienId = dichVu.NoiThucHienId,

                GoiKhamSucKhoeId = dichVu.GoiKhamSucKhoeId,
                //GoiKhamSucKhoe = goiKhamSucKhoe,
                DonGiaUuDai = dichVu.DonGiaUuDai,
                DonGiaChuaUuDai = dichVu.DonGiaChuaUuDai,
                TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan,
                TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                BaoHiemChiTra = null,
                GiaBenhVienTaiThoiDiemChiDinh =
                    dichKyThuatChiDinh.DichVuKyThuatVuBenhVienGiaBenhViens
                        .Any(x => x.NhomGiaDichVuKyThuatBenhVienId == dichVu.LoaiGiaId.Value
                                  && x.TuNgay.Date <= now.Date
                                  && (x.DenNgay == null || x.DenNgay.Value.Date >= now.Date))
                      ? dichKyThuatChiDinh.DichVuKyThuatVuBenhVienGiaBenhViens.First(x => x.NhomGiaDichVuKyThuatBenhVienId == dichVu.LoaiGiaId.Value
                                                                                          && x.TuNgay.Date <= now.Date
                                                                                          && (x.DenNgay == null || x.DenNgay.Value.Date >= now.Date)).Gia
                      : (decimal?)null,

                //BVHD-3668
                GoiKhamSucKhoeDichVuPhatSinhId = dichVu.GoiKhamSucKhoeDichVuPhatSinhId
            };

            return entity;
        }

        public async Task XuLyXoaDichVuKhamSucKhoeChiDinhAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauDichVuId, bool laDichVuKham)
        {
            if (laDichVuKham)
            {
                var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauDichVuId);
                if (yeuCauKhamBenh == null)
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                if (yeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && yeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                {
                    throw new Exception(_localizationService.GetResource("ChiDinh.DichVuKham.DaThucHien"));
                }

                yeuCauKhamBenh.WillDelete = true;
            }
            else
            {
                var yeuCauDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats.FirstOrDefault(x => x.Id == yeuCauDichVuId);
                if (yeuCauDichVuKyThuat == null)
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                if (yeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && yeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                {
                    throw new Exception(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThucHien"));
                }

                yeuCauDichVuKyThuat.WillDelete = true;
            }

            await PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhan);
        }

        public async Task XuLyThemDichVuKhamSucKhoeChiDinhAsync(YeuCauTiepNhan yeuCauTiepNhan, TiepNhanDichVuChiDinhVo dichVu)
        {
            // thêm dịch vụ khám
            if (dichVu.LaDichVuKham) // Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh)
            {
                yeuCauTiepNhan.YeuCauKhamBenhs.Add(await XuLyThemYeuCauKhamBenhAsync(dichVu));
            }
            // thêm dịch vụ kỹ thuật
            else
            {
                yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(await XuLyThemYeuCauDichVuKyThuatAsync(dichVu));
            }

            await PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
        }

        public async Task<List<TiepNhanDichVuChiDinhVo>> XuLyThemDichVuKhamSucKhoeChiDinhAsyncMultiselect(List<string> dichVuThems, Enums.HinhThucKhamBenh hinhThucKham, long hopDongKhamSucKhoeId, YeuCauTiepNhan yeuCauTiepNhan = null, long? HopDongKhamSucKhoeNhanVienId = null)
        {
            // bổ sung ngày 06/12/2021: nhằm mục đích lưu dịch vụ ngoại trú
            YeuCauTiepNhan yeuCauTiepNhanNgoaiTru = null;
            var coThemDichVuVaoTiepNhanKSK = false;
            var coThemDichVuVaoTiepNhaNgoaiTru = false;
            var lanDauKiemTraYCTNNgoaiTru = true;

            var lstDichVu = new List<TiepNhanDichVuChiDinhVo>();

            //BVHD-3618
            var lstDichVuKhamGoiChungMoiThem = new List<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>();
            var lstDichVuKyThuatGoiChungMoiThem = new List<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>();
            var lstIndexDichVuKhamGoiChungMoiThem = new List<int>();
            var lstIndexDichVuKyThuatGoiChungMoiThem = new List<int>();

            GoiKhamSucKhoe goiKhamSucKhoe = null;
            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var cauHinhKhamBenhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKhamBenh.NhomGiaThuong");
            long.TryParse(cauHinhKhamBenhNhomGiaThuongBenhVien?.Value, out long nhomGiaKhamBenhThuongId);

            var phongNoiVienMacDinh = _phongBenhVienRepository.TableNoTracking
                .Where(x => x.IsDisabled != true)
                .First();
            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var lstChuyenKhoaKSKChinh = EnumHelper.GetListEnum<Enums.ChuyenKhoaKhamSucKhoe>().Select(item => (Enums.ChuyenKhoaKhamSucKhoe)item).ToList();
            //BVHD-3618
            HopDongKhamSucKhoeNhanVien hopDongNhanVien = null;
            bool laThemDichVuGoiChungVaoBangTam = false;
            if (yeuCauTiepNhan == null && HopDongKhamSucKhoeNhanVienId != null)
            {
                hopDongNhanVien = _hopDongKhamSucKhoeNhanVienRepository.Table
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
                    .FirstOrDefault(x => x.Id == HopDongKhamSucKhoeNhanVienId);
            }

            foreach (var dichVu in dichVuThems)
            {
                var dichVuObj = JsonConvert.DeserializeObject<KeyIdStringDichVuKhamSucKhoeVo>(dichVu);
                if (dichVuObj.GoiKhamSucKhoeId != null && goiKhamSucKhoe?.Id != dichVuObj.GoiKhamSucKhoeId
                    || (dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId != null && goiKhamSucKhoe?.Id != dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId))
                {
                    goiKhamSucKhoe = _goiKhamSucKhoeRepository.TableNoTracking
                        .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(y => y.DichVuKhamBenhBenhVien)
                        .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(y => y.NhomGiaDichVuKhamBenhBenhVien)
                        .Include(x => x.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(y => y.GoiKhamSucKhoeNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                        .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                        .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(y => y.NhomGiaDichVuKyThuatBenhVien)
                        .Include(x => x.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(y => y.GoiKhamSucKhoeNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                        .First(x => x.Id == dichVuObj.GoiKhamSucKhoeId || x.Id == dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId);
                }

                var dichVuMoi = new TiepNhanDichVuChiDinhVo()
                {
                    DichVuBenhVienId = dichVuObj.DichVuId,
                    LoaiGiaId = null,
                    Ma = null,
                    Ten = null,
                    DonGiaMoi = null,
                    NoiThucHienId = null,
                    GoiKhamSucKhoeId = dichVuObj.GoiKhamSucKhoeId,
                    DonGiaUuDai = null,
                    DonGiaChuaUuDai = null,
                    SoLan = 1
                };

                if (dichVuObj.NhomDichVu == EnumNhomGoiDichVu.DichVuKhamBenh)
                {
                    dichVuMoi.TinhTrang = (int)Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                    dichVuMoi.TenTinhTrang = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham.GetDescription();

                    //BVHD-3668
                    dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId = dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId;
                    var laChuyenKhoaKSKChinh = false;

                    // đối với dv khám, chỉ cho phép chỉ định từ gói khám sức khỏe <- cũ
                    // BVHD-3688: cho phép chỉ định dịch vụ khám từ trong gói và cả ngoài gói, dv khám ko thuộc 7 nhóm khám sức khỏe thì sẽ xử lý tạo YCTN ngoại trú
                    if (dichVuMoi.GoiKhamSucKhoeId != null)
                    {
                        var dichVuKham = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.First(x => x.DichVuKhamBenhBenhVienId == dichVuMoi.DichVuBenhVienId);

                        dichVuMoi.Ma = dichVuKham.DichVuKhamBenhBenhVien.Ma;
                        dichVuMoi.Ten = dichVuKham.DichVuKhamBenhBenhVien.Ten;
                        dichVuMoi.LoaiGiaId = dichVuKham.NhomGiaDichVuKhamBenhBenhVienId;
                        dichVuMoi.TenLoaiGia = dichVuKham.NhomGiaDichVuKhamBenhBenhVien.Ten;
                        dichVuMoi.DonGiaBenhVien = dichVuMoi.DonGiaMoi = dichVuKham.DonGiaBenhVien;
                        dichVuMoi.DonGiaUuDai = dichVuKham.DonGiaUuDai;
                        dichVuMoi.DonGiaChuaUuDai = dichVuKham.DonGiaChuaUuDai;

                        var noiThucHienChon = dichVuKham.GoiKhamSucKhoeNoiThucHiens.Select(a => a).First();
                        dichVuMoi.NoiThucHienId = noiThucHienChon.PhongBenhVienId;
                        dichVuMoi.TenNoiThucHien = noiThucHienChon.PhongBenhVien.Ten;

                        dichVuMoi.ChuyenKhoaKhamSucKhoe = dichVuKham.ChuyenKhoaKhamSucKhoe;
                        dichVuMoi.TenGoiKhamSucKhoe = dichVuKham.GoiKhamSucKhoe.Ten;

                        //if (yeuCauTiepNhan == null)
                        {
                            
                            dichVuMoi.LaGoiChung = true;

                            var goiKhamSucKhoeChungDichVuKhamBenhNhanVien = new GoiKhamSucKhoeChungDichVuKhamBenhNhanVien()
                            {
                                GoiKhamSucKhoeId = dichVuKham.GoiKhamSucKhoeId,
                                GoiKhamSucKhoeDichVuKhamBenhId = dichVuKham.Id,
                                DichVuKhamBenhBenhVienId = dichVuKham.DichVuKhamBenhBenhVienId
                            };

                            if (yeuCauTiepNhan == null)
                            {
                                laThemDichVuGoiChungVaoBangTam = true;
                                hopDongNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Add(goiKhamSucKhoeChungDichVuKhamBenhNhanVien);
                                lstDichVuKhamGoiChungMoiThem.Add(goiKhamSucKhoeChungDichVuKhamBenhNhanVien);
                                lstIndexDichVuKhamGoiChungMoiThem.Add(lstDichVu.Count);
                            }
                            else
                            {
                                yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Add(goiKhamSucKhoeChungDichVuKhamBenhNhanVien);
                            }
                        }

                        if (dichVuKham.ChuyenKhoaKhamSucKhoe != null && lstChuyenKhoaKSKChinh.Contains(dichVuKham.ChuyenKhoaKhamSucKhoe.Value))
                        {
                            laChuyenKhoaKSKChinh = true;
                        }
                    }
                    else
                    {
                        var dichVuKhamBenhChiDinh = _dichVuKhamBenhBenhVienRepository.TableNoTracking
                            .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                            .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                            .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuKhamBenhBenhVien)
                            .FirstOrDefault(x => x.Id == dichVuObj.DichVuId);

                        if (dichVuKhamBenhChiDinh == null)
                        {
                            throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                        }

                        if (!dichVuKhamBenhChiDinh.HieuLuc)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKhamBenh.HetHieuLuc"), dichVuKhamBenhChiDinh.Ten));
                        }

                        var dvKhamGiaBV = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBenhViens
                            .Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderByDescending(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomGiaKhamBenhThuongId)
                            .ThenBy(x => x.CreatedOn)
                            .FirstOrDefault();
                        if (dvKhamGiaBV == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKhamBenh.KhongCoGiaHieuLuc"), dichVuKhamBenhChiDinh.Ten));
                        }

                        if (dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe != null
                            && lstChuyenKhoaKSKChinh.Contains(dichVuKhamBenhChiDinh.ChuyenKhoaKhamSucKhoe.Value))
                        {
                            laChuyenKhoaKSKChinh = true;
                        }

                        dichVuMoi.Ma = dichVuKhamBenhChiDinh.Ma;
                        dichVuMoi.Ten = dichVuKhamBenhChiDinh.Ten;
                        dichVuMoi.LoaiGiaId = dvKhamGiaBV.NhomGiaDichVuKhamBenhBenhVienId;
                        dichVuMoi.DonGiaBenhVien = dichVuMoi.DonGiaMoi = dichVuObj.DonGia ?? dvKhamGiaBV.Gia;
                        dichVuMoi.TenLoaiGia = dvKhamGiaBV.NhomGiaDichVuKhamBenhBenhVien.Ten;
                        dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId = dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId;

                        #region Xử lý nơi thực hiện
                        if (dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId != null && dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId != 0)
                        {
                            var dichVuKhamPhatSinh = goiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs.First(x => x.DichVuKhamBenhBenhVienId == dichVuMoi.DichVuBenhVienId);
                            var noiThucHienTheoGoiKham = dichVuKhamPhatSinh.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVien).FirstOrDefault();
                            dichVuMoi.NoiThucHienId = noiThucHienTheoGoiKham == null ? phongNoiVienMacDinh.Id : noiThucHienTheoGoiKham.Id;
                            dichVuMoi.TenNoiThucHien = noiThucHienTheoGoiKham == null ? phongNoiVienMacDinh.Ten : noiThucHienTheoGoiKham.Ten;
                            dichVuMoi.TenGoiKhamSucKhoe = dichVuKhamPhatSinh.GoiKhamSucKhoe.Ten;
                        }
                        else
                        {
                            if (hinhThucKham == HinhThucKhamBenh.KhamDoanNgoaiVien)
                            {
                                var queryInfo = new DropDownListRequestModel()
                                {
                                    ParameterDependencies = "{DichVuId: " + dichVuMoi.DichVuBenhVienId + ", HopDongKhamSucKhoeId: " + hopDongKhamSucKhoeId + "}",
                                    Take = 1
                                };
                                var lstPhongThucHien = await GetKhoaPhongGoiKham(queryInfo);

                                var noiThucHienChon = lstPhongThucHien.Select(x => x).First();
                                dichVuMoi.NoiThucHienId = noiThucHienChon.KeyId;
                                dichVuMoi.TenNoiThucHien = noiThucHienChon.Ten;
                            }
                            else
                            {
                                if (dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienNoiThucHiens.Any())
                                {
                                    var phongThucHiens = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienNoiThucHiens
                                        .Where(x => x.PhongBenhVienId != null && x.PhongBenhVienId != 0)
                                        .Select(x => new LookupItemVo
                                        {
                                            KeyId = x.PhongBenhVien.Id,
                                            DisplayName = x.PhongBenhVien.Ten
                                        }).Distinct()
                                        .ToList();
                                    var phongThucHienTheoKhoas = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienNoiThucHiens
                                        .Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong)
                                        .SelectMany(x => x.PhongBenhViens)
                                        .Select(x => new LookupItemVo
                                        {
                                            KeyId = x.Id,
                                            DisplayName = x.Ten
                                        }).Distinct()
                                        .Distinct().ToList();

                                    phongThucHiens.AddRange(phongThucHienTheoKhoas);

                                    var noiThucHien = phongThucHiens.OrderBy(x => x.KeyId).First();
                                    dichVuMoi.NoiThucHienId = noiThucHien.KeyId;
                                    dichVuMoi.TenNoiThucHien = noiThucHien.DisplayName;
                                }
                                else
                                {
                                    dichVuMoi.NoiThucHienId = phongNoiVienMacDinh.Id;
                                    dichVuMoi.TenNoiThucHien = phongNoiVienMacDinh.Ten;
                                }
                            }
                        }
                        #endregion
                    }

                    //if (yeuCauTiepNhan != null)
                    //{
                    //    yeuCauTiepNhan.YeuCauKhamBenhs.Add(await XuLyThemYeuCauKhamBenhAsync(dichVuMoi));
                    //}

                    if (yeuCauTiepNhan != null)
                    {
                        var dichVuKhamBenhThemMoi = await XuLyThemYeuCauKhamBenhAsync(dichVuMoi);

                        if (laChuyenKhoaKSKChinh)
                        {
                            coThemDichVuVaoTiepNhanKSK = true;
                            yeuCauTiepNhan.YeuCauKhamBenhs.Add(dichVuKhamBenhThemMoi);
                        }
                        else
                        {
                            coThemDichVuVaoTiepNhaNgoaiTru = true;
                            if (yeuCauTiepNhanNgoaiTru == null 
                                && (yeuCauTiepNhan.Id != 0 || (yeuCauTiepNhan.BenhNhanId != null && yeuCauTiepNhan.BenhNhanId != 0)) 
                                && lanDauKiemTraYCTNNgoaiTru)
                            {
                                yeuCauTiepNhanNgoaiTru = await GetYeucauTiepNhanDungDeLuuDichVuAsync(null, yeuCauTiepNhan.Id, yeuCauTiepNhan.BenhNhanId);
                                lanDauKiemTraYCTNNgoaiTru = false;
                            }
                            XuLyThemDichVuKhamNgoaiChuyenKhoaChinh(yeuCauTiepNhan, dichVuKhamBenhThemMoi, false, yeuCauTiepNhanNgoaiTru);
                        }
                    }

                    lstDichVu.Add(dichVuMoi);
                }
                else if (dichVuObj.NhomDichVu == EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    dichVuMoi.TinhTrang = (int)Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                    dichVuMoi.TenTinhTrang = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien.GetDescription();

                    // đối với dv kỹ thuật, cho phép chỉ định từ trong và ngoài gói khám sức khỏe
                    if (dichVuMoi.GoiKhamSucKhoeId != null)
                    {
                        var dichVuKyThuat = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.First(x => x.DichVuKyThuatBenhVienId == dichVuMoi.DichVuBenhVienId);

                        dichVuMoi.Ma = dichVuKyThuat.DichVuKyThuatBenhVien.Ma;
                        dichVuMoi.Ten = dichVuKyThuat.DichVuKyThuatBenhVien.Ten;
                        dichVuMoi.LoaiGiaId = dichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId;
                        dichVuMoi.TenLoaiGia = dichVuKyThuat.NhomGiaDichVuKyThuatBenhVien.Ten;
                        dichVuMoi.DonGiaBenhVien = dichVuMoi.DonGiaMoi = dichVuKyThuat.DonGiaBenhVien;
                        dichVuMoi.DonGiaUuDai = dichVuKyThuat.DonGiaUuDai;
                        dichVuMoi.DonGiaChuaUuDai = dichVuKyThuat.DonGiaChuaUuDai;

                        var noiThucHienChon = dichVuKyThuat.GoiKhamSucKhoeNoiThucHiens.Select(a => a).FirstOrDefault();
                        dichVuMoi.NoiThucHienId = noiThucHienChon.PhongBenhVienId;
                        dichVuMoi.TenNoiThucHien = noiThucHienChon.PhongBenhVien.Ten;

                        dichVuMoi.ChuyenKhoaKhamSucKhoe = null;
                        dichVuMoi.TenGoiKhamSucKhoe = dichVuKyThuat.GoiKhamSucKhoe.Ten;
                        dichVuMoi.LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuKyThuat.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien);

                        // phần này chỉ dùng để xử lý cho gói khám sức khỏe là gói chung
                        //if (yeuCauTiepNhan == null)
                        {
                            dichVuMoi.LaGoiChung = true;

                            var goiKhamSucKhoeChungDichVuKyThuatNhanVien = new GoiKhamSucKhoeChungDichVuKyThuatNhanVien()
                            {
                                GoiKhamSucKhoeId = dichVuKyThuat.GoiKhamSucKhoeId,
                                GoiKhamSucKhoeDichVuDichVuKyThuatId = dichVuKyThuat.Id,
                                DichVuKyThuatBenhVienId = dichVuKyThuat.DichVuKyThuatBenhVienId
                            };
                            if (yeuCauTiepNhan == null)
                            {
                                laThemDichVuGoiChungVaoBangTam = true;
                                hopDongNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Add(goiKhamSucKhoeChungDichVuKyThuatNhanVien);
                                lstDichVuKyThuatGoiChungMoiThem.Add(goiKhamSucKhoeChungDichVuKyThuatNhanVien);
                                lstIndexDichVuKyThuatGoiChungMoiThem.Add(lstDichVu.Count);
                            }
                            else
                            {
                                yeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Add(goiKhamSucKhoeChungDichVuKyThuatNhanVien);
                            }
                            
                        }
                    }
                    else
                    {
                        var dichVuKyThuat = _dichVuKyThuatBenhVienRepository.TableNoTracking
                            .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(x => x.PhongBenhVien)
                            .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(x => x.PhongBenhVien)
                            .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong).ThenInclude(x => x.PhongBenhViens)
                            .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuKyThuatBenhVien)
                            .FirstOrDefault(x => x.Id == dichVuObj.DichVuId);
                        if (dichVuKyThuat == null)
                        {
                            throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                        }

                        if (!dichVuKyThuat.HieuLuc)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKyThuat.HetHieuLuc"), dichVuKyThuat.Ten));
                        }

                        var dvktGiaBV = dichVuKyThuat.DichVuKyThuatVuBenhVienGiaBenhViens
                            .Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                            .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                            .ThenBy(x => x.CreatedOn)
                            .FirstOrDefault();
                        if (dvktGiaBV == null)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.DichVuKyThuat.KhongCoGiaHieuLuc"), dichVuKyThuat.Ten));
                        }

                        dichVuMoi.Ma = dichVuKyThuat.Ma;
                        dichVuMoi.Ten = dichVuKyThuat.Ten;
                        dichVuMoi.LoaiGiaId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId;
                        dichVuMoi.DonGiaBenhVien = dichVuMoi.DonGiaMoi = dichVuObj.DonGia ?? dvktGiaBV.Gia; //BVHD-3668
                        dichVuMoi.TenLoaiGia = dvktGiaBV.NhomGiaDichVuKyThuatBenhVien.Ten;
                        dichVuMoi.LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuKyThuat.NhomDichVuBenhVienId, lstNhomDichVuBenhVien);

                        //BVHD-3668
                        dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId = dichVuObj.GoiKhamSucKhoeDichVuPhatSinhId;

                        #region Xử lý nơi thực hiện
                        if (dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId != null && dichVuMoi.GoiKhamSucKhoeDichVuPhatSinhId != 0)
                        {
                            var dichVuKyThuatPhatSinh = goiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats.First(x => x.DichVuKyThuatBenhVienId == dichVuMoi.DichVuBenhVienId);
                            var noiThucHienTheoGoiKham = dichVuKyThuatPhatSinh.GoiKhamSucKhoeNoiThucHiens.Select(a => a.PhongBenhVien).FirstOrDefault();
                            dichVuMoi.NoiThucHienId = noiThucHienTheoGoiKham == null ? phongNoiVienMacDinh.Id : noiThucHienTheoGoiKham.Id;
                            dichVuMoi.TenNoiThucHien = noiThucHienTheoGoiKham == null ? phongNoiVienMacDinh.Ten : noiThucHienTheoGoiKham.Ten;
                            dichVuMoi.TenGoiKhamSucKhoe = dichVuKyThuatPhatSinh.GoiKhamSucKhoe.Ten;
                        }
                        else
                        {
                            if (hinhThucKham == HinhThucKhamBenh.KhamDoanNgoaiVien)
                            {
                                var queryInfo = new DropDownListRequestModel()
                                {
                                    ParameterDependencies = "{DichVuId: " + dichVuMoi.DichVuBenhVienId + ", HopDongKhamSucKhoeId: " + hopDongKhamSucKhoeId + "}",
                                    Take = 1
                                };
                                var lstPhongThucHien = await GetKhoaPhongGoiKham(queryInfo);

                                var noiThucHienChon = lstPhongThucHien.Select(x => x).First();
                                dichVuMoi.NoiThucHienId = noiThucHienChon.KeyId;
                                dichVuMoi.TenNoiThucHien = noiThucHienChon.Ten;
                            }
                            else
                            {
                                var noiThucHienUuTiens = dichVuKyThuat.DichVuKyThuatBenhVienNoiThucHienUuTiens
                                    .OrderByDescending(x => x.LoaiNoiThucHienUuTien == LoaiNoiThucHienUuTien.NguoiDung)
                                    .Select(x => new LookupItemVo
                                    {
                                        KeyId = x.PhongBenhVienId,
                                        DisplayName = x.PhongBenhVien.Ten
                                    })
                                    .ToList();

                                if (!noiThucHienUuTiens.Any() && !dichVuKyThuat.DichVuKyThuatBenhVienNoiThucHiens.Any())
                                {
                                    dichVuMoi.NoiThucHienId = phongNoiVienMacDinh.Id;
                                    dichVuMoi.TenNoiThucHien = phongNoiVienMacDinh.Ten;
                                }
                                else
                                {
                                    var phongThucHiens = dichVuKyThuat.DichVuKyThuatBenhVienNoiThucHiens
                                        .Where(x => x.PhongBenhVienId != null && x.PhongBenhVienId != 0)
                                        .Select(x => new LookupItemVo
                                        {
                                            KeyId = x.PhongBenhVien.Id,
                                            DisplayName = x.PhongBenhVien.Ten
                                        }).Distinct()
                                        .ToList();
                                    var phongThucHienTheoKhoas = dichVuKyThuat.DichVuKyThuatBenhVienNoiThucHiens
                                        .Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhong)
                                        .SelectMany(x => x.PhongBenhViens)
                                        .Select(x => new LookupItemVo
                                        {
                                            KeyId = x.Id,
                                            DisplayName = x.Ten
                                        }).Distinct()
                                        .ToList();

                                    phongThucHiens.AddRange(phongThucHienTheoKhoas);

                                    var noiThucHienUuTien = noiThucHienUuTiens.FirstOrDefault();
                                    if (noiThucHienUuTien != null && phongThucHiens.Select(x => x.KeyId).Contains(noiThucHienUuTien.KeyId))
                                    {
                                        dichVuMoi.NoiThucHienId = noiThucHienUuTien.KeyId;
                                        dichVuMoi.TenNoiThucHien = noiThucHienUuTien.DisplayName;
                                    }
                                    else
                                    {
                                        if (phongThucHiens.Any())
                                        {
                                            var phongThucHien = phongThucHiens.OrderBy(x => x.KeyId).First();
                                            dichVuMoi.NoiThucHienId = phongThucHien.KeyId;
                                            dichVuMoi.TenNoiThucHien = phongThucHien.DisplayName;
                                        }
                                        else
                                        {
                                            dichVuMoi.NoiThucHienId = phongNoiVienMacDinh.Id;
                                            dichVuMoi.TenNoiThucHien = phongNoiVienMacDinh.Ten;
                                        }
                                    }
                                }



                            }
                        }
                        #endregion

                    }

                    if (yeuCauTiepNhan != null)
                    {
                        coThemDichVuVaoTiepNhanKSK = true;
                        yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(await XuLyThemYeuCauDichVuKyThuatAsync(dichVuMoi));
                    }
                    lstDichVu.Add(dichVuMoi);
                }
            }

            if (yeuCauTiepNhan != null)
            {
                if (coThemDichVuVaoTiepNhanKSK 
                    // trường hợp đã add YCTN ngoại trú vào YCTN KSK luôn
                    || (coThemDichVuVaoTiepNhaNgoaiTru && yeuCauTiepNhanNgoaiTru == null))
                {
                    await PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);
                }
                if (coThemDichVuVaoTiepNhaNgoaiTru && yeuCauTiepNhanNgoaiTru != null)
                {
                    await PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanNgoaiTru);
                }
            }
            else if (laThemDichVuGoiChungVaoBangTam)
            {
                _hopDongKhamSucKhoeNhanVienRepository.Context.SaveChanges();

                foreach (var dichVuKham in lstDichVuKhamGoiChungMoiThem.Select((value, i) => (value, i)))
                {
                    var indexDichVuThemTam = lstIndexDichVuKhamGoiChungMoiThem.Skip(dichVuKham.Item2).Take(1).First();
                    var dichVuThemTam = lstDichVu.ElementAt(indexDichVuThemTam);
                    dichVuThemTam.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId = dichVuKham.Item1.Id;
                }

                foreach (var dichVuKyThuat in lstDichVuKyThuatGoiChungMoiThem.Select((value, i) => (value, i)))
                {
                    var indexDichVuThemTam = lstIndexDichVuKyThuatGoiChungMoiThem.Skip(dichVuKyThuat.Item2).Take(1).First();
                    var dichVuThemTam = lstDichVu.ElementAt(indexDichVuThemTam);
                    dichVuThemTam.GoiKhamSucKhoeChungDichVuKyThuatNhanVienId = dichVuKyThuat.Item1.Id;
                }
            }

            return lstDichVu;
        }

        public async Task<string> GetTenNhomGiaTheoLoaiDichVuAsync(Enums.NhomDichVuChiDinhKhamSucKhoe nhomDichVu, long NhomGiaId)
        {
            var tenNhom = string.Empty;
            // thêm dịch vụ khám
            if (nhomDichVu == Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh)
            {
                tenNhom = await _nhomGiaDichVuKhamBenhBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == NhomGiaId)
                    .Select(x => x.Ten)
                    .FirstOrDefaultAsync();
            }
            // thêm dịch vụ kỹ thuật
            else
            {
                tenNhom = await _nhomGiaDichVuKyThuatBenhVienRepository.TableNoTracking
                    .Where(x => x.Id == NhomGiaId)
                    .Select(x => x.Ten)
                    .FirstOrDefaultAsync();
            }

            return tenNhom;
        }

        public string GetDataDefaultDichVuKhamSucKhoe(Enums.ChuyenKhoaKhamSucKhoe? chuyenKhoaKhamSucKhoe)
        {
            var thongTinKhamData = string.Empty;
            switch (chuyenKhoaKhamSucKhoe)
            {
                case Enums.ChuyenKhoaKhamSucKhoe.NoiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TuanHoan\",\"Value\":\"Bình thường\"},{\"Id\":\"TuanHoanPhanLoai\",\"Value\":1},{\"Id\":\"HoHap\",\"Value\":\"Bình thường\"},{\"Id\":\"HoHapPhanLoai\",\"Value\":1},{\"Id\":\"TieuHoa\",\"Value\":\"Bình thường\"},{\"Id\":\"TieuHoaPhanLoai\",\"Value\":1},{\"Id\":\"ThanTietLieu\",\"Value\":\"Bình thường\"},{\"Id\":\"ThanTietLieuPhanLoai\",\"Value\":1},{\"Id\":\"NoiTiet\",\"Value\":\"Bình thường\"},{\"Id\":\"NoiTietPhanLoai\",\"Value\":1},{\"Id\":\"CoXuongKhop\",\"Value\":\"Bình thường\"},{\"Id\":\"CoXuongKhopPhanLoai\",\"Value\":1},{\"Id\":\"ThanKinh\",\"Value\":\"Bình thường\"},{\"Id\":\"ThanKinhPhanLoai\",\"Value\":1},{\"Id\":\"TamThan\",\"Value\":\"Bình thường\"},{\"Id\":\"TamThanPhanLoai\",\"Value\":1}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.NgoaiKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"NgoaiKhoa\",\"Value\":\"Bình thường\"},{\"Id\":\"NgoaiKhoaPhanLoai\",\"Value\":1}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.SanPhuKhoa:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"SanPhuKhoa\",\"Value\":\"Bình thường\"},{\"Id\":\"SanPhuKhoaPhanLoai\",\"Value\":1}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.Mat:
                    //thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"Bình thường\"},{\"Id\":\"MatPhanLoai\",\"Value\":1}]}";
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"KhongKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"KhongKinhMatTrai\",\"Value\":\"10/10\"},{\"Id\":\"CacBenhVeMat\",\"Value\":\"2 Mắt: Bình thường\"},{\"Id\":\"MatPhanLoai\",\"Value\":1}]}"; //{\"Id\":\"CoKinhMatPhai\",\"Value\":\"10/10\"},{\"Id\":\"CoKinhMatTrai\",\"Value\":\"10/10\"},
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.RangHamMat:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"HamTren\",\"Value\":\"Bình thường\"},{\"Id\":\"HamDuoi\",\"Value\":\"Bình thường\"},{\"Id\":\"CacBenhRangHamMat\",\"Value\":\"Bình thường\"},{\"Id\":\"RangHamMatPhanLoai\",\"Value\":1}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.TaiMuiHong:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"TaiPhaiNoiThuong\",\"Value\":\"5 mét\"},{\"Id\":\"TaiPhaiNoiTham\",\"Value\":\"0.5 mét\"},{\"Id\":\"TaiTraiNoiThuong\",\"Value\":\"5 mét\"},{\"Id\":\"TaiTraiNoiTham\",\"Value\":\"0.5 mét\"},{\"Id\":\"CacBenhTaiMuiHong\",\"Value\":\"Bình thường\"},{\"Id\":\"TaiMuiHongPhanLoai\",\"Value\":1}]}";
                    break;
                case Enums.ChuyenKhoaKhamSucKhoe.DaLieu:
                    thongTinKhamData = "{\"DataKhamTheoTemplate\": [{\"Id\":\"DaLieu\",\"Value\":\"Bình thường\"},{\"Id\":\"DaLieuPhanLoai\",\"Value\":1}]}";
                    break;
                default:
                    thongTinKhamData = null;
                    break;
            }

            return thongTinKhamData;
        }

        public async Task XuLyXoaDichVuGoiChungChuaBatDauKhamAsync(DichVuGoiChungXoaChuaBatDauKhamVo xoaDichVuVo)
        {
            if (xoaDichVuVo.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId != null)
            {
                var chiTiet = await _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.GetByIdAsync(xoaDichVuVo.GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId.Value);
                chiTiet.WillDelete = true;
                await _goiKhamSucKhoeChungDichVuKhamBenhNhanVienRepository.Context.SaveChangesAsync();
            }
            else if (xoaDichVuVo.GoiKhamSucKhoeChungDichVuKyThuatNhanVienId != null)
            {
                var chiTiet = await _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.GetByIdAsync(xoaDichVuVo.GoiKhamSucKhoeChungDichVuKyThuatNhanVienId.Value);
                chiTiet.WillDelete = true;
                await _goiKhamSucKhoeChungDichVuKyThuatNhanVienRepository.Context.SaveChangesAsync();
            }
        }

        public async Task XuLyQuayLaiChuaKhamNhieuNhanVienAsync(List<long> hopDongKSKNhanVienIds)
        {
            //update 27/12/2022: bỏ xóa trực tiếp
            var lstTiepNhanKhamSucKhoeNhanVien = BaseRepository.Table
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.TuVanThuocKhamSucKhoes)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)                
                .Where(x => x.HopDongKhamSucKhoeNhanVienId != null
                            && hopDongKSKNhanVienIds.Contains(x.HopDongKhamSucKhoeNhanVienId.Value)
                            && x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                .ToList();

            foreach (var tiepNhan in lstTiepNhanKhamSucKhoeNhanVien)
            {
                tiepNhan.TrangThaiYeuCauTiepNhan = EnumTrangThaiYeuCauTiepNhan.DaHuy;
                tiepNhan.HopDongKhamSucKhoeNhanVienId = null;
                foreach (var yeuCauKhamBenh in tiepNhan.YeuCauKhamBenhs)
                {
                    yeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.HuyKham;
                    yeuCauKhamBenh.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    foreach(var phongBenhVienHangDoi in yeuCauKhamBenh.PhongBenhVienHangDois)
                    {
                        phongBenhVienHangDoi.WillDelete = true;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in tiepNhan.YeuCauDichVuKyThuats)
                {
                    yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                    yeuCauDichVuKyThuat.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    foreach (var phongBenhVienHangDoi in yeuCauDichVuKyThuat.PhongBenhVienHangDois)
                    {
                        phongBenhVienHangDoi.WillDelete = true;
                    }
                }

                foreach (var goiChungKhamChiTiet in tiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                {
                    goiChungKhamChiTiet.WillDelete = true;
                }

                foreach (var goiChungKyThuatChiTiet in tiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
                {
                    goiChungKyThuatChiTiet.WillDelete = true;
                }
            }

            BaseRepository.Context.SaveChanges();
        }

        public async Task XuLyQuayLaiChuaKhamNhieuNhanVienAsyncOld(List<long> hopDongKSKNhanVienIds)
        {
            var lstTiepNhanKhamSucKhoeNhanVien = BaseRepository.Table
                .Include(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.TuVanThuocKhamSucKhoes)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                .Include(x => x.HopDongKhamSucKhoeNhanVien).ThenInclude(x => x.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
                .Include(x => x.YeuCauTiepNhanNgoaiTruKhamSucKhoes).ThenInclude(x => x.YeuCauKhamBenhs).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauTiepNhanNgoaiTruKhamSucKhoes).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauTiepNhanNgoaiTruKhamSucKhoes).ThenInclude(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauTiepNhanNgoaiTruKhamSucKhoes).ThenInclude(x => x.YeuCauVatTuBenhViens)
                .Where(x => x.HopDongKhamSucKhoeNhanVienId != null
                            && hopDongKSKNhanVienIds.Contains(x.HopDongKhamSucKhoeNhanVienId.Value)
                            && x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien)
                .ToList();

            foreach (var tiepNhan in lstTiepNhanKhamSucKhoeNhanVien)
            {
                tiepNhan.WillDelete = true;
                foreach (var tiepNhanNgoaiTruKSK in tiepNhan.YeuCauTiepNhanNgoaiTruKhamSucKhoes)
                {
                    tiepNhanNgoaiTruKSK.WillDelete = true;
                }

                foreach (var goiChungKhamChiTiet in tiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens)
                {
                    goiChungKhamChiTiet.WillDelete = true;
                }

                foreach (var goiChungKyThuatChiTiet in tiepNhan.HopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens)
                {
                    goiChungKyThuatChiTiet.WillDelete = true;
                }
            }

            BaseRepository.Context.SaveChanges();
        }
        #endregion




        #region Kiểm tra validator

        public async Task<bool> KiemTraTrungMaNhanVienTheoHopDongAsync(long hopDongId, long hopDongNhanVienId, string maNhanVien)
        {
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return false;
            }

            var kiemTra = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .AnyAsync(x => x.HopDongKhamSucKhoeId == hopDongId
                               && x.Id != hopDongNhanVienId
                               && !string.IsNullOrEmpty(x.MaNhanVien)
                               && x.MaNhanVien.ToLower().Trim() == maNhanVien.ToLower().Trim());
            return kiemTra;
        }

        public async Task KiemTraNguoiBenhCanQuayLaiChuaKhamKSK(List<long> hopDongKSKNhanVienIds)
        {
            var yctnNgoaiTruTuKSKTenNguoiBenhs = BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanKhamSucKhoeId != null
                            && x.YeuCauTiepNhanKhamSucKhoe.HopDongKhamSucKhoeNhanVienId != null
                            && hopDongKSKNhanVienIds.Contains(x.YeuCauTiepNhanKhamSucKhoe.HopDongKhamSucKhoeNhanVienId.Value)
                            && x.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                            && x.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .Select(x => x.HoTen)
                .Distinct()
                .ToList();
            if (yctnNgoaiTruTuKSKTenNguoiBenhs.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("TiepNhanKhamDoan.QuayLaiChuaKhamNhieuNhanVien.CoYCTNNgoaiTru"), yctnNgoaiTruTuKSKTenNguoiBenhs.Join(", ")));
            }
            
            var nguoiBenhCoPhatSinhThanhToan = _taiKhoanBenhNhanThuRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                            && hopDongKSKNhanVienIds.Contains(x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId.Value))
                .Select(x => x.YeuCauTiepNhan.HoTen)
                .Distinct()
                .ToList();
            if (nguoiBenhCoPhatSinhThanhToan.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("TiepNhanKhamDoan.QuayLaiChuaKhamNhieuNhanVien.CoPhatSinhThanhToan"), nguoiBenhCoPhatSinhThanhToan.Join(", ")));
            }

            var nguoiBenhcoDichVuDaThucHienHoacThanhToans =
                _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                && hopDongKSKNhanVienIds.Contains(x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId.Value)
                                && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                && (x.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                || x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan))
                .Select(x => x.YeuCauTiepNhan.HoTen)
                    .Union(
                        _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                                        && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                                        && hopDongKSKNhanVienIds.Contains(x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId.Value)
                                        && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && (x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                            || x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                            || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan))
                            .Select(x => x.YeuCauTiepNhan.HoTen)
                        )
                .Distinct()
                .ToList();
            if (nguoiBenhcoDichVuDaThucHienHoacThanhToans.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("TiepNhanKhamDoan.QuayLaiChuaKhamNhieuNhanVien.CoDichVuThucHienHoacThanhToan"), nguoiBenhcoDichVuDaThucHienHoacThanhToans.Join(", ")));
            }

            var nguoiBenhChuaBatDauKhams = _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => !x.YeuCauTiepNhans.Any()
                            && hopDongKSKNhanVienIds.Contains(x.Id))
                .Select(x => x.HoTen)
                .Distinct()
                .ToList();
            if (nguoiBenhChuaBatDauKhams.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("TiepNhanKhamDoan.QuayLaiChuaKhamNhieuNhanVien.ChuaBatDauKham"), nguoiBenhChuaBatDauKhams.Join(", ")));
            }
        }
        #endregion

        #region Xử lý html, pdf
        public async Task<string> XuLyInBangHuongDanKhamSucKhoeAsync(InHoSoKhamSucKhoeVo hoSoIn)
        {
            var content = string.Empty;

            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BangHuongDanKhamSucKhoe"));
            var data = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                .Where(x => x.Id == hoSoIn.HopDongKhamSucKhoeNhanVienId)
                .Select(item => new HoSoKhamSucKhoeOInfoVo()
                {
                    Header = "",
                    LogoUrl = hoSoIn.HostingName + "/assets/img/logo-bacha-full.png",
                    DonViKham = item.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HoTen = item.HoTen,
                    NamSinh = item.NamSinh,
                    GioiTinh = item.GioiTinh.GetDescription(),
                    MaNhanVien = item.MaNhanVien,
                    ChucVu = "",
                    GhiChu = "",
                    ViTriCongTac = item.TenDonViHoacBoPhan,
                    SoDoDichVuKham = ""
                }).FirstAsync();

            var lstDichVuKhamSucKhoe = new List<DichVuKhamSucKhoeNhanVien>();

            var yeuCauTiepNhanKhamSucKhoe = await BaseRepository.TableNoTracking
                //.Include(x => x.YeuCauKhamBenhs).ThenInclude(y => y.NoiThucHien).ThenInclude(z => z.KhoaPhong)
                //.Include(x => x.YeuCauDichVuKyThuats).ThenInclude(y => y.NoiThucHien).ThenInclude(z => z.KhoaPhong)
                .Where(x => x.HopDongKhamSucKhoeNhanVienId == hoSoIn.HopDongKhamSucKhoeNhanVienId)
                .FirstOrDefaultAsync();
            if (yeuCauTiepNhanKhamSucKhoe != null)
            {
                //lstDichVuKhamSucKhoe.AddRange(yeuCauTiepNhanKhamSucKhoe.YeuCauKhamBenhs
                //    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).Select(item => new DichVuKhamSucKhoeNhanVien()
                //    {
                //        TenDichVu = item.TenDichVu,
                //        PhongThucHien = item.NoiThucHien.Ten,
                //        Khoa = item.NoiThucHien.KhoaPhong.Ten,
                //        Tang = item.NoiThucHien.Tang
                //    }));
                //lstDichVuKhamSucKhoe.AddRange(yeuCauTiepNhanKhamSucKhoe.YeuCauDichVuKyThuats
                //    .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).Select(item => new DichVuKhamSucKhoeNhanVien()
                //    {
                //        TenDichVu = item.TenDichVu,
                //        PhongThucHien = item.NoiThucHien.Ten,
                //        Khoa = item.NoiThucHien.KhoaPhong.Ten,
                //        Tang = item.NoiThucHien.Tang
                //    }));
                var lstYeuCauKham = _yeuCauKhamBenhRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanKhamSucKhoe.Id
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    .Select(item => new DichVuKhamSucKhoeNhanVien()
                    {
                        TenDichVu = item.TenDichVu,
                        PhongThucHien = item.NoiThucHien.Ten,
                        Khoa = item.NoiThucHien.KhoaPhong.Ten,
                        Tang = item.NoiThucHien.Tang
                    }).ToList();
                var lstYeuCauKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanKhamSucKhoe.Id
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .Select(item => new DichVuKhamSucKhoeNhanVien()
                    {
                        TenDichVu = item.TenDichVu,
                        PhongThucHien = item.NoiThucHien.Ten,
                        Khoa = item.NoiThucHien.KhoaPhong.Ten,
                        Tang = item.NoiThucHien.Tang
                    }).ToList();
                lstDichVuKhamSucKhoe.AddRange(lstYeuCauKham);
                lstDichVuKhamSucKhoe.AddRange(lstYeuCauKyThuat);
            }
            else
            {
                var hopDongKhamSucKhoeNhanVien = await _hopDongKhamSucKhoeNhanVienRepository.TableNoTracking
                    .Include(x => x.GoiKhamSucKhoe).ThenInclude(y => y.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.DichVuKhamBenhBenhVien)
                    .Include(x => x.GoiKhamSucKhoe).ThenInclude(y => y.GoiKhamSucKhoeDichVuKhamBenhs).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien).ThenInclude(o => o.KhoaPhong)
                    .Include(x => x.GoiKhamSucKhoe).ThenInclude(y => y.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.DichVuKyThuatBenhVien)
                    .Include(x => x.GoiKhamSucKhoe).ThenInclude(y => y.GoiKhamSucKhoeDichVuDichVuKyThuats).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien).ThenInclude(o => o.KhoaPhong)

                    //BVHD-3618
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens).ThenInclude(y => y.GoiKhamSucKhoeDichVuKhamBenh).ThenInclude(t => t.DichVuKhamBenhBenhVien)
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens).ThenInclude(y => y.GoiKhamSucKhoeDichVuKhamBenh).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien).ThenInclude(o => o.KhoaPhong)
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKyThuatNhanViens).ThenInclude(y => y.GoiKhamSucKhoeDichVuDichVuKyThuat).ThenInclude(t => t.DichVuKyThuatBenhVien)
                    .Include(x => x.GoiKhamSucKhoeChungDichVuKyThuatNhanViens).ThenInclude(y => y.GoiKhamSucKhoeDichVuDichVuKyThuat).ThenInclude(t => t.GoiKhamSucKhoeNoiThucHiens).ThenInclude(u => u.PhongBenhVien).ThenInclude(o => o.KhoaPhong)
                    .Where(x => x.Id == hoSoIn.HopDongKhamSucKhoeNhanVienId).FirstOrDefaultAsync();

                if (hopDongKhamSucKhoeNhanVien != null && hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe != null)
                {
                    var tuoi = hopDongKhamSucKhoeNhanVien.NamSinh != null ? DateTime.Now.Year - hopDongKhamSucKhoeNhanVien.NamSinh.Value : (int?)null;

                    // todo: cân nhắc dùng lại hàm get dịch vụ trong gói ở màn hình chi tiết
                    lstDichVuKhamSucKhoe.AddRange(hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuKhamBenhs
                        .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                    && ((!x.CoMangThai && !x.KhongMangThai) || hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongKhamSucKhoeNhanVien.CoMangThai) || (x.KhongMangThai && !hopDongKhamSucKhoeNhanVien.CoMangThai))
                                    && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongKhamSucKhoeNhanVien.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongKhamSucKhoeNhanVien.DaLapGiaDinh))
                                    && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
                        .Select(item => new DichVuKhamSucKhoeNhanVien()
                        {
                            TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                            PhongThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                            Khoa = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                            Tang = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Tang).FirstOrDefault(),
                        }));
                    lstDichVuKhamSucKhoe.AddRange(hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoe.GoiKhamSucKhoeDichVuDichVuKyThuats
                        .Where(x => ((!x.GioiTinhNam && !x.GioiTinhNu) || (x.GioiTinhNam && hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam) || (x.GioiTinhNu && hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu))
                                    && ((!x.CoMangThai && !x.KhongMangThai) || hopDongKhamSucKhoeNhanVien.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam || (x.CoMangThai && hopDongKhamSucKhoeNhanVien.CoMangThai) || (x.KhongMangThai && !hopDongKhamSucKhoeNhanVien.CoMangThai))
                                    && ((!x.DaLapGiaDinh && !x.ChuaLapGiaDinh) || (x.ChuaLapGiaDinh && !hopDongKhamSucKhoeNhanVien.DaLapGiaDinh) || (x.DaLapGiaDinh && hopDongKhamSucKhoeNhanVien.DaLapGiaDinh))
                                    && ((x.SoTuoiTu == null && x.SoTuoiDen == null) || (tuoi != null && ((x.SoTuoiTu == null || tuoi >= x.SoTuoiTu) && (x.SoTuoiDen == null || tuoi <= x.SoTuoiDen)))))
                        .Select(item => new DichVuKhamSucKhoeNhanVien()
                        {
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            PhongThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                            Khoa = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                            Tang = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Tang).FirstOrDefault()
                        }));

                    #region BVHD-3618
                    if (hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Any())
                    {
                        lstDichVuKhamSucKhoe.AddRange(hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens.Select(x => x.GoiKhamSucKhoeDichVuKhamBenh)
                            .Select(item => new DichVuKhamSucKhoeNhanVien()
                            {
                                TenDichVu = item.DichVuKhamBenhBenhVien.Ten,
                                PhongThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                                Khoa = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                                Tang = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Tang).FirstOrDefault(),
                            }));
                    }

                    if (hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Any())
                    {
                        lstDichVuKhamSucKhoe.AddRange(hopDongKhamSucKhoeNhanVien.GoiKhamSucKhoeChungDichVuKyThuatNhanViens.Select(x => x.GoiKhamSucKhoeDichVuDichVuKyThuat)
                            .Select(item => new DichVuKhamSucKhoeNhanVien()
                            {
                                TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                                PhongThucHien = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Ten).FirstOrDefault(),
                                Khoa = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.KhoaPhong.Ten).FirstOrDefault(),
                                Tang = item.GoiKhamSucKhoeNoiThucHiens.Select(x => x.PhongBenhVien.Tang).FirstOrDefault(),
                            }));
                    }
                    #endregion
                }
            }

            // set số thứ tự cho dịch vụ
            if (lstDichVuKhamSucKhoe.Any())
            {
                lstDichVuKhamSucKhoe = lstDichVuKhamSucKhoe.OrderBy(x => x.TenDichVu).ToList(); //todo: cần cập nhật lại

                var sttDichVu = 1;
                foreach (var dichVu in lstDichVuKhamSucKhoe)
                {
                    dichVu.STT = sttDichVu++;
                }
            }


            //todo: cập  nhật lại số thứ tự
            var soDichVuTrong1Hang = 3;
            var lisDichVuTheoSoDo = new List<DichVuKhamSucKhoeNhanVienTheoSoDo>();
            var count = lstDichVuKhamSucKhoe.Count % soDichVuTrong1Hang == 0 ? (lstDichVuKhamSucKhoe.Count / soDichVuTrong1Hang) : (lstDichVuKhamSucKhoe.Count / soDichVuTrong1Hang) + 1;
            for (int i = 0; i < count; i++)
            {
                var nhomDichVu = new DichVuKhamSucKhoeNhanVienTheoSoDo()
                {
                    DichVuKhamSucKhoeNhanViens = lstDichVuKhamSucKhoe.Skip(i * soDichVuTrong1Hang).Take(soDichVuTrong1Hang).ToList()
                };
                lisDichVuTheoSoDo.Add(nhomDichVu);
            }

            var soDoKham = "<table id='soDoDichVuKham' width='100%' style='text-align:center;'>";
            var isRight = true;
            var iconLeft = "/assets/img/logo-arrow-left.png";
            var iconRight = "/assets/img/logo-arrow-right.png";
            var iconDown = "/assets/img/logo-arrow-down.png";
            var icon = hoSoIn.HostingName + iconRight;
            var isKetThuc = false;
            var isAddRowKetThuc = false;
            var colKetThuc = "<td>"
                             + "<p class='boder-cell margin-center' style='height:38px; width:38px; font-size:13px; display:table;vertical-align:middle; background-color:red; color:white; border-radius:50%; margin:0 auto;'>Kết thúc</p>"
                             + "</td>";
            // kiểm tra tổng dịch vụ mà bé hơn hoặc bằng số dịch vụ trong 1 hàng, thì sẽ xử lý đặc biệt và kết thúc dòng luôn
            var tongSoDichVu = lstDichVuKhamSucKhoe.Count;
            foreach (var dichVu in lisDichVuTheoSoDo.Select((value, index) => new { value, index }))
            {
                var tr = string.Empty;

                if (!isRight)
                {
                    dichVu.value.DichVuKhamSucKhoeNhanViens.Reverse();
                }

                tr += "<tr>";
                if (dichVu.index == 0)
                {
                    tr += "<td style='width:10%'> "
                            + "<p class='boder-cell margin-center' style='height: 38px; width: 38px; font-size:13px; display: table;vertical-align: middle; background-color: #005dab; color: white; border-radius: 50%; margin:0 auto;'>Bắt đầu</p>"
                        + "</td>"
                            + "<td style='width:5%'>"
                        + "<img style ='width:20px;' src='" + icon + "'>"
                        + "</td>";

                    var soCotConLai = 5;
                    foreach (var dichVuTheoNhom in dichVu.value.DichVuKhamSucKhoeNhanViens.Select((value, index) => new { value, index }))
                    {
                        if (tongSoDichVu >= soDichVuTrong1Hang)
                        {
                            soCotConLai = 0;
                            tr += "<td class='boder-cell' style='width: 25%;'>" +
                                  "(" + dichVuTheoNhom.value.STT + ")<br>" +
                                  dichVuTheoNhom.value.TenDichVu + "<br>" +
                                  dichVuTheoNhom.value.NoiThucHien +
                                  "</td>";
                            if (dichVuTheoNhom.index != dichVu.value.DichVuKhamSucKhoeNhanViens.Count - 1)
                            {
                                tr += "<td style='width:5%'>"
                                      + "<img style ='width:20px;' src ='" + icon + "' >"
                                      + "</td>";
                            }
                        }
                        else
                        {
                            tr += "<td class='boder-cell' style='width: 25%;'>" +
                                  "(" + dichVuTheoNhom.value.STT + ")<br>" +
                                  dichVuTheoNhom.value.TenDichVu + "<br>" +
                                  dichVuTheoNhom.value.NoiThucHien +
                                  "</td>";
                            soCotConLai--;
                            // trường hợp dấu mũi tên trước dịch vụ cuối cùng
                            if (dichVuTheoNhom.index != dichVu.value.DichVuKhamSucKhoeNhanViens.Count - 1)
                            {
                                tr += "<td style='width:5%'>"
                                      + "<img style ='width:20px;' src ='" + icon + "' >"
                                      + "</td>";
                                soCotConLai--;
                            }

                            // trường hợp mũi tên sau dịch vụ cuối cùng và icon kết thúc
                            else
                            {
                                tr += "<td style='width:5%'>"
                                      + "<img style ='width:20px;' src ='" + icon + "' >"
                                      + "</td>";
                                tr += colKetThuc;

                                soCotConLai -= 2;

                                for (int i = 0; i < soCotConLai; i++)
                                {
                                    tr += "<td></td>";
                                }
                            }
                        }
                    }

                    if (tongSoDichVu == soDichVuTrong1Hang)
                    {
                        //soDoKham += "<tr><td></td>"
                        //            + "<td></td>"
                        //            + "<td></td>"
                        //            + "<td></td>"
                        //            + "<td></td>"
                        //            + colKetThuc
                        //            + "<td>"
                        //            + "<img src ='" + hoSoIn.HostingName + iconDown + "' >"
                        //            + "</td></tr>";
                        isAddRowKetThuc = true;
                        isKetThuc = true;
                    }
                }
                else
                {
                    var trTemp = string.Empty;
                    foreach (var dichVuTheoNhom in dichVu.value.DichVuKhamSucKhoeNhanViens.Select((value, index) => new { value, index }))
                    {
                        var tdTemp = "<td class='boder-cell'>" +
                                  "(" + dichVuTheoNhom.value.STT + ")<br>" +
                                  dichVuTheoNhom.value.TenDichVu + "<br>" +
                                  dichVuTheoNhom.value.NoiThucHien +
                                  "</td>";
                        var iconTemp = string.Empty;

                        var countDichVuTrongHangHienTai = dichVu.value.DichVuKhamSucKhoeNhanViens.Count;

                        //trường hợp dòng cuối cùng hướng từ phải sang trái -> show thêm icon mũi tên
                        var showLastIconInLastRow = !isRight && dichVuTheoNhom.index == 0 && dichVuTheoNhom.value.STT == lisDichVuTheoSoDo.SelectMany(x => x.DichVuKhamSucKhoeNhanViens).Count();

                        if (countDichVuTrongHangHienTai < soDichVuTrong1Hang
                            || (countDichVuTrongHangHienTai == soDichVuTrong1Hang && (isRight && dichVuTheoNhom.index < 2) || (!isRight && (showLastIconInLastRow || dichVuTheoNhom.index > 0))))
                        {
                            iconTemp = "<td>"
                                      + "<img src ='" + icon + "' >"
                                      + "</td>";
                        }

                        if (isRight)
                        {
                            trTemp += tdTemp + iconTemp;
                        }
                        else
                        {
                            trTemp += iconTemp + tdTemp;
                        }
                    }

                    var sttTemp = dichVu.value.DichVuKhamSucKhoeNhanViens.Select(x => x.STT).OrderByDescending(x => x).First();
                    if (sttTemp >= lstDichVuKhamSucKhoe.Count) // trường hợp là dịch vụ cuối cùng
                    {
                        isKetThuc = true;
                        if (sttTemp % soDichVuTrong1Hang == 0 && isRight) // full right
                        {
                            isAddRowKetThuc = true;
                            tr += "<td></td>"
                                  + "<td></td>"
                                  + trTemp;
                        }
                        else
                        {
                            if (sttTemp % soDichVuTrong1Hang == 0) // full left
                            {
                                tr = colKetThuc
                                      //+ "<td>"
                                      //+ "<img src ='" + icon + "' >"
                                      //+ "</td>"
                                      + trTemp;
                            }
                            else
                            {
                                var slCotTrong = 7 - (sttTemp % 3) * 2; // + (!isRight ? 1 : 0);
                                var isFirstAddButtonKetThuc = true;
                                var countRight = (sttTemp % 3) * 2;
                                for (int i = 0; i < slCotTrong; i++)
                                {
                                    if (isRight)
                                    {
                                        countRight++;
                                        if (isFirstAddButtonKetThuc)
                                        {
                                            isFirstAddButtonKetThuc = false;
                                            trTemp += colKetThuc;
                                        }
                                        else
                                        {
                                            if (countRight <= 5)
                                            {
                                                trTemp += "<td></td>";
                                            }
                                            else
                                            {
                                                trTemp = "<td></td>" + trTemp;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (isFirstAddButtonKetThuc)
                                        {
                                            isFirstAddButtonKetThuc = false;
                                            trTemp = colKetThuc + trTemp;
                                        }
                                        else
                                        {
                                            trTemp = "<td></td>" + trTemp;
                                        }
                                    }
                                }
                                tr += trTemp;
                            }
                        }
                    }
                    else
                    {
                        tr = "<td></td>"
                              + "<td></td>"
                              + trTemp;
                    }

                }

                tr += "</tr>";
                soDoKham += tr;

                if (tongSoDichVu < soDichVuTrong1Hang)
                {
                    break;
                }

                if (isAddRowKetThuc)
                {
                    soDoKham += "<tr><td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td>"
                                + "<img src ='" + hoSoIn.HostingName + iconDown + "' >"
                                + "</td></tr>";
                    soDoKham += "<tr><td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + "<td></td>"
                                + colKetThuc
                                + "</tr>";
                }

                if (isKetThuc)
                {
                    break;
                }

                #region Kiểm tra xuống hàng
                icon = hoSoIn.HostingName + iconDown;
                if (isRight) // dòng trên là 
                {
                    if (!isKetThuc)
                    {
                        soDoKham += "<tr><td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td>"
                                    + "<img src ='" + icon + "' >"
                                    + "</td></tr>";
                    }

                    icon = hoSoIn.HostingName + iconLeft;
                    isRight = false;
                }
                else
                {
                    if (!isKetThuc)
                    {
                        soDoKham += "<tr><td></td>"
                                    + "<td></td>"
                                    + "<td>"
                                    + "<img src ='" + icon + "' >"
                                    + "</td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "<td></td>"
                                    + "</tr>";
                    }

                    icon = hoSoIn.HostingName + iconRight;
                    isRight = true;
                }
                #endregion
            }
            soDoKham += "</table>";

            data.SoDoDichVuKham = soDoKham;
            content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }


        #endregion
        #region in dịch vụ  chỉ định ngoài gói khám đoàn 
        public string InDichVuChiDinhPhatSinh(string hosting, long yeuCauTiepNhanId, List<DichVuChiDinhInGridVo> vos)

        {
            long userId = _userAgentHelper.GetCurrentUserId();
            var nguoiChiDinh = _userRepository.GetById(userId);
            var content = "";
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                        .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                          .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)

                        .Include(p => p.NguoiLienHeQuanHeNhanThan)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBenhViens)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuongBenhVienGiaBaoHiems)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)?.ThenInclude(p => p.DichVuGiuong)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.DichVuGiuongBenhVien)//?.ThenInclude(p => p.Khoa)?.ThenInclude(p => p.PhongBenhViens)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhomGiaDichVuGiuongBenhVien)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NoiThucHien).ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.YeuCauDichVuGiuongBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)


                        .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                        .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)

                        //.Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPhamBenhVienGiaBaoHiems)
                        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.DuocPhamBenhVien)?.ThenInclude(p => p.DuocPham)
                        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NoiCapThuoc).ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.YeuCauDuocPhamBenhViens)?.ThenInclude(p => p.NhanVienCapThuoc)?.ThenInclude(p => p.User)

                        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.VatTuBenhVien)?.ThenInclude(p => p.VatTus)
                        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiChiDinh)
                        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NoiCapVatTu).ThenInclude(p => p.KhoaPhong)
                        .Include(p => p.YeuCauVatTuBenhViens)?.ThenInclude(p => p.NhanVienCapVatTu)?.ThenInclude(p => p.User)

                        .Include(p => p.BenhNhan)
                        .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                        .Include(cc => cc.PhuongXa)
                        .Include(cc => cc.QuanHuyen)
                        .Include(cc => cc.TinhThanh)
                        .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.ToList());

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.ToList());

            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1
            content = InChiDinhNgoaiGoiInChung(hosting, yeuCauTiepNhanId, vos, content); // in chung theo nhóm và người chỉ định
            return content;
        }
        private string InChiDinhNgoaiGoiInChung(string hosting, long yeuCauTiepNhanId, List<DichVuChiDinhInGridVo> vos, string content)
        {
            //KieuInChung => in 1. In Theo dịch vụ chỉ định (cùng người chỉ định dịch vụ) 2. In theo số thứ tự (cùng người chỉ định dịch vụ)
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
                      .Include(p => p.NguoiLienHeQuanHeNhanThan)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                      .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                      .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                      .Include(p => p.BenhNhan)
                      .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                      .Include(cc => cc.PhuongXa)
                      .Include(cc => cc.QuanHuyen)
                      .Include(cc => cc.TinhThanh)
                      .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
                      .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();

            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận


            // in chỉ định khám bệnh và dịch vụ kỹ thuật inChungChiDinh = 1

            var listInDichVuKyThuat = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
            var listTheoNguoiChiDinh = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
            var listDichVuKys = vos.Where(x => x.NhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat);
            foreach (var itx in listDichVuKys)
            {
                foreach (var ycdvkt in listDVKT.Where(o => o.DichVuKyThuatBenhVien != null))
                {
                    if (itx.DichVuChiDinhId == ycdvkt.Id)
                    {
                        var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                        objNguoiChidinh.dichVuChiDinhId = ycdvkt.Id; // 
                        objNguoiChidinh.nhomChiDinhId = (int)EnumNhomGoiDichVu.DichVuKyThuat;
                        objNguoiChidinh.NhanVienChiDinhId = ycdvkt.NhanVienChiDinhId;
                        objNguoiChidinh.ThoiDiemChiDinh = new DateTime(ycdvkt.ThoiDiemChiDinh.Year, ycdvkt.ThoiDiemChiDinh.Month, ycdvkt.ThoiDiemChiDinh.Day, 0, 0, 0);
                        listTheoNguoiChiDinh.Add(objNguoiChidinh);
                    }

                }
            }
            #region yêu cầu khám
            var lstDVKB = vos.Where(x => x.NhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
            foreach (var itx in lstDVKB)
            {
                var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.DichVuChiDinhId
                   && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                    ).OrderBy(x => x.CreatedOn);

                if (lstYeuCauKhamBenhChiDinh != null)
                {
                    foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                    {
                        if (itx.DichVuChiDinhId == yckb.Id)
                        {
                            var objNguoiChidinh = new ListDichVuChiDinhTheoNguoiChiDinh();
                            objNguoiChidinh.dichVuChiDinhId = yckb.Id; // 
                            objNguoiChidinh.nhomChiDinhId = (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh; //
                            objNguoiChidinh.NhanVienChiDinhId = yckb.NhanVienChiDinhId;
                            objNguoiChidinh.ThoiDiemChiDinh = new DateTime(yckb.ThoiDiemChiDinh.Year, yckb.ThoiDiemChiDinh.Month, yckb.ThoiDiemChiDinh.Day, 0, 0, 0);
                            listTheoNguoiChiDinh.Add(objNguoiChidinh);
                        }
                    }
                }
            }
            #endregion yêu cầu khám

            /// in theo nhóm dịch vụ và Người chỉ định
            var listInChiDinhTheoNguoiChiDinh = listTheoNguoiChiDinh.GroupBy(s => new { s.NhanVienChiDinhId, s.ThoiDiemChiDinh }).OrderBy(d => d.Key.ThoiDiemChiDinh).ToList();
            foreach (var itemListDichVuChiDinhTheoNguoiChiDinh in listInChiDinhTheoNguoiChiDinh)
            {
                var listCanIn = new List<ListDichVuChiDinhTheoNguoiChiDinh>();
                listCanIn.AddRange(itemListDichVuChiDinhTheoNguoiChiDinh);
                content = AddChiDinhKhamBenhTheoNguoiChiDinhVaNhoms(hosting, yeuCauTiepNhanId, vos, content, listCanIn);
            }
            return content;
        }
        private string AddChiDinhKhamBenhTheoNguoiChiDinhVaNhoms(string hosting, long yeuCauTiepNhanId, List<DichVuChiDinhInGridVo> vos, string content, List<ListDichVuChiDinhTheoNguoiChiDinh> listCanIn)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)//?.ThenInclude(p => p.Khoa)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)?.ThenInclude(p => p.KhoaPhong)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.HocHamHocVi)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh).ThenInclude(p => p.ChanDoanSoBoICD)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri).ThenInclude(p => p.ChanDoanChinhICD)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauTiepNhan)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauKhamBenh)?.ThenInclude(p => p.ChanDoanSoBoICD)
                                  .Include(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiTruPhieuDieuTri)?.ThenInclude(p => p.ChanDoanChinhICD)
                                  .Include(p => p.NguoiLienHeQuanHeNhanThan)
                                  .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.ChanDoanSoBoICD)
                                  .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy).ThenInclude(p => p.KhoaPhong)
                                  .Include(p => p.YeuCauKhamBenhs)?.ThenInclude(p => p.NhanVienChiDinh)?.ThenInclude(p => p.User)
                                  .Include(p => p.BenhNhan)
                                  .Include(p => p.NoiTiepNhan).ThenInclude(p => p.KhoaPhong)
                                  .Include(cc => cc.PhuongXa)
                                  .Include(cc => cc.QuanHuyen)
                                  .Include(cc => cc.TinhThanh)
                                  .Include(cc => cc.NoiTruBenhAn).ThenInclude(pp => pp.NoiTruPhieuDieuTris)
                                  .Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefault();
            #region entity dichVuKhams , dichVuKyThuats
            var listYeuCauKhamBenhIds = vos.Where(d => d.NhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Select(d => d.DichVuChiDinhId).ToList();
            List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> listDVK = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            listDVK.AddRange(yeuCauTiepNhan.YeuCauKhamBenhs.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && listYeuCauKhamBenhIds.Contains(s.Id)).ToList()); // tất cả dịch vụ dịch vụ khám theo yêu cầu tiếp nhận

            var listYeuCauDichVuKyThuatIds = vos.Where(d => d.NhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).Select(d => d.DichVuChiDinhId).ToList();
            List<YeuCauDichVuKyThuat> listDVKT = new List<YeuCauDichVuKyThuat>();

            listDVKT.AddRange(yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(s => s.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && listYeuCauDichVuKyThuatIds.Contains(s.Id)).ToList()); // tất cả dịch vụ dịch vụ kỹ thuật theo yêu cầu tiếp nhận
            #endregion entity dichVuKhams , dichVuKyThuats

            decimal tongtien = 0;
            var phongBenhVienId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var maPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ma;
            var tenPhong = _phongBenhVienRepository.TableNoTracking.Where(x => x.Id == phongBenhVienId).First().Ten;

            string tenNguoiChiDinh = "";
            content += "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";

            var tamp = "<table id='showHeader' style='width: 100%; height: 40px; background: #005dab;color:#fff;'> <tr><th rowspan = '3' style = 'font-size: 20px;'>PHIẾU CHỈ ĐỊNH CẬN LÂM SÀNG</th></tr></table>";
            var tmp = "<table id=\"showHeader\" style=\"display:none;\"></table>";

            string ngay = "";
            string thang = "";
            string nam = "";
            var isHave = false;
            var htmlDanhSachDichVu = string.Empty;
            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>THÀNH TIỀN (VNĐ)</th>";
            htmlDanhSachDichVu += "</tr>";
            var i = 1;
            int indexDVKT = 1;
            var listInDichVuKyThuats = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDichVuKyThuat>();
            var listInDichVuKhams = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();

            var listCanInlaDichVuKhamBenhHayDichVuKyThuat = listCanIn.Select(d => d.nhomChiDinhId).First();

           

            if (listCanInlaDichVuKhamBenhHayDichVuKyThuat == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh)
            {
                decimal tongCong = 0;
                int soLuong = 0;
                #region update 3668
                //DỊCH VỤ KHÁM BỆNH



                var lstDVKB = listCanIn.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                     && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                      ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb);
                            }
                        }
                    }
                }


                // BVHD-3939 // == 1 
               
                var thanhTienDv = listInDichVuKhamBenh
                    .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                    .Sum();
                CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                tongCong += thanhTienDv.GetValueOrDefault();

                foreach (var yckb in listInDichVuKhamBenh)
                {
                    ngay = yckb.ThoiDiemDangKy.Day.ToString();
                    thang = yckb.ThoiDiemDangKy.Month.ToString();
                    nam = yckb.ThoiDiemDangKy.Year.ToString();
                    if (indexDVKB == 1)
                    {
                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                        htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: center;'><b>{thanhTienFormat}</b></td>";
                        htmlDanhSachDichVu += " </tr>";
                    }

                    var maHocHamVi = string.Empty;
                    var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                    if (maHocHamViId != null)
                    {
                        maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                    }

                    tenNguoiChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);


                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + 1 + "</td>"; // so lan kham
                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                    htmlDanhSachDichVu += " </tr>";
                    i++;
                    indexDVKB++;
                    soLuong++;
                }

                #endregion update 3668
                #region dịch vụ kỹ thuật
                var lstDVKT = listCanIn.Where(x => x.nhomChiDinhId == (int)EnumNhomGoiDichVu.DichVuKyThuat).ToList();

                foreach (var itx in listCanIn)
                {
                    foreach (var ycdvkt in yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.GoiKhamSucKhoeId == null))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            listInDichVuKyThuats.Add(ycdvkt);
                        }
                    }
                }

                List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinh = new List<ListDichVuChiDinhCLSPTTT>();
                foreach (var ycdvkt in listInDichVuKyThuats)
                {
                    var lstDichVuKT = new ListDichVuChiDinhCLSPTTT();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.NhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.DichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                var lstdvkt = yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null);
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDVKT = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDVKT = string.Format(culDVKT, "{0:n2}", thanhTienDVKT);
                        tongCong += thanhTienDVKT.GetValueOrDefault() ;

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Any())
                            {
                                //var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormatDVKT}</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);

                                if (maHocHamViId.Any(d => d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }

                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).First() : "");

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDVKT = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDVKT = string.Format(culDVKT, "{0:n2}", thanhTienDVKT);
                        tongCong += thanhTienDVKT.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First() != null)
                            {
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d => d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }

                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).First() : "");

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormatDVKT}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    #endregion
                }
                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939
            }
            else
            {
                decimal tongCong = 0;
                int soLuong = 0;
                #region dịch vụ kỹ thuật
                var lstDVKT = listCanIn.Where(x => x.nhomChiDinhId == (int)EnumNhomGoiDichVu.DichVuKyThuat).ToList();

                foreach (var itx in listCanIn)
                {
                    foreach (var ycdvkt in yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null && o.GoiKhamSucKhoeId == null))
                    {
                        if (itx.dichVuChiDinhId == ycdvkt.Id)
                        {
                            listInDichVuKyThuats.Add(ycdvkt);
                        }
                    }
                }

                List<ListDichVuChiDinhCLSPTTT> lstDichVuChidinh = new List<ListDichVuChiDinhCLSPTTT>();
                foreach (var ycdvkt in listInDichVuKyThuats)
                {
                    var lstDichVuKT = new ListDichVuChiDinhCLSPTTT();
                    var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).First().Ten;
                    lstDichVuKT.TenNhom = nhomDichVu;
                    lstDichVuKT.NhomChiDinhId = ycdvkt.NhomDichVuBenhVien.Id;
                    lstDichVuKT.DichVuChiDinhId = ycdvkt.Id;
                    lstDichVuChidinh.Add(lstDichVuKT);
                }
                var lstdvkt = yeuCauTiepNhan?.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null);
                foreach (var dv in lstDichVuChidinh.GroupBy(x => x.TenNhom).ToList())
                {

                    if (dv.Count() > 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDvKt = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDvkt = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDvkt = string.Format(culDvkt, "{0:n2}", thanhTienDvKt);
                        tongCong += thanhTienDvKt.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Any())
                            {
                                //var nhomDichVu = _nhomDichVuBenhVienRepository.TableNoTracking.Where(x => x.Id == ycdvkt.NhomDichVuBenhVien.Id).Select(q => q.Ten).FirstOrDefault();

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                if (indexDVKT == 1)
                                {
                                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                    htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                    htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormatDvkt}</b></td>";
                                    htmlDanhSachDichVu += " </tr>";
                                }
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);

                                if (maHocHamViId.Any(d => d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }

                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).First() : "");

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                    if (dv.Count() == 1)
                    {
                        // BVHD-3939 // == 1 
                        var listDichVuIds = dv.Select(d => d.DichVuChiDinhId).ToList();
                        var thanhTienDvKT = lstdvkt.Where(d => listDichVuIds.Contains(d.Id))
                            .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * d.SoLan) : (d.Gia * d.SoLan)))
                            .Sum();
                        CultureInfo culDVKT = CultureInfo.GetCultureInfo("vi-VN");
                        var thanhTienFormatDVKT = string.Format(culDVKT, "{0:n2}", thanhTienDvKT);
                        tongCong += thanhTienDvKT.GetValueOrDefault();

                        foreach (var ycdvktIn in dv)
                        {
                            if (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First() != null)
                            {
                                var maHocHamVi = string.Empty;
                                var maHocHamViId = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.HocHamHocViId);
                                if (maHocHamViId.Any(d => d != null))
                                {
                                    maHocHamVi = MaHocHamHocVi((long)maHocHamViId.First());
                                }

                                tenNguoiChiDinh = returnStringTen(maHocHamVi, "", lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).Any() ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(s => s.NhanVienChiDinh?.User.HoTen).First() : "");

                                ngay = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Day.ToString()).First();
                                thang = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Month.ToString()).First();
                                nam = lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).Select(d => d.ThoiDiemDangKy.Year.ToString()).First();

                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b> " + ycdvktIn.TenNhom.ToUpper() + "</b></td>";
                                htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;'><b>{thanhTienFormatDVKT}</b></td>";
                                htmlDanhSachDichVu += " </tr>";
                                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().DichVuKyThuatBenhVien.Ten + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien != null ? lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().NoiThucHien?.Ten : "") + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan + "</td>";
                                htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                                htmlDanhSachDichVu += " </tr>";
                                i++;
                                indexDVKT++;
                                ycdvktIn.TenNhom = "";
                                soLuong += lstdvkt.Where(p => p.Id == ycdvktIn.DichVuChiDinhId).First().SoLan;
                            }
                        }
                        indexDVKT = 1;
                    }
                }
                #endregion

                #region update 3668
                //DỊCH VỤ KHÁM BỆNH
                var lstDVKB = listCanIn.Where(x => x.nhomChiDinhId == (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh);
                int indexDVKB = 1;
                var listInDichVuKhamBenh = new List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>();
               
                foreach (var itx in lstDVKB)
                {
                    var lstYeuCauKhamBenhChiDinh = listDVK.Where(s => s.Id == itx.dichVuChiDinhId
                     && s.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                      ).OrderBy(x => x.CreatedOn); // to do nam ho;

                    if (lstYeuCauKhamBenhChiDinh != null)
                    {
                        foreach (var yckb in lstYeuCauKhamBenhChiDinh)
                        {
                            if (itx.dichVuChiDinhId == yckb.Id)
                            {
                                listInDichVuKhamBenh.Add(yckb);
                            }
                        }
                    }
                }
                if (listInDichVuKhamBenh.ToList().Count() != 0)
                {
                    var thanhTienDv = listInDichVuKhamBenh
                .Select(d => (d.YeuCauGoiDichVuId != null ? (d.DonGiaSauChietKhau * 1) : (d.Gia * 1)))
                .First();
                    CultureInfo culDVK = CultureInfo.GetCultureInfo("vi-VN");
                    var thanhTienFormat = string.Format(culDVK, "{0:n2}", thanhTienDv);
                    tongCong += thanhTienDv.GetValueOrDefault();

                    foreach (var yckb in listInDichVuKhamBenh)
                    {
                        ngay = yckb.ThoiDiemDangKy.Day.ToString();
                        thang = yckb.ThoiDiemDangKy.Month.ToString();
                        nam = yckb.ThoiDiemDangKy.Year.ToString();
                        if (indexDVKB == 1)
                        {
                            htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                            htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='4'><b>DỊCH VỤ KHÁM BỆNH</b></td>";
                            htmlDanhSachDichVu += $"<td style='border: 1px solid #020000;text-align: right;'><b>{thanhTienFormat}</b></td>";
                            htmlDanhSachDichVu += " </tr>";
                        }

                        var maHocHamVi = string.Empty;
                        var maHocHamViId = yckb.NhanVienChiDinh?.HocHamHocViId;
                        if (maHocHamViId != null)
                        {
                            maHocHamVi = MaHocHamHocVi((long)maHocHamViId);
                        }

                        tenNguoiChiDinh = returnStringTen(maHocHamVi, "", yckb.NhanVienChiDinh?.User?.HoTen);

                        htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.TenDichVu + "</td>";
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null ? yckb.NoiDangKy?.Ten : "") + "</td>";

                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + "1" + "</td>"; // mặc định 1 dòng yeu cau khám = 1
                        htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'></td>";
                        htmlDanhSachDichVu += " </tr>";
                        i++;
                        indexDVKB++;
                        soLuong++;
                    }
                }
                 

                #endregion update 3668

                // BVHD-3939- page -total
                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
                htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: left;' colspan='3'><b>TỔNG CỘNG</b> </th>";
                // BVHD-3939 - số lượng
                htmlDanhSachDichVu += $" <th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'><b>{soLuong}</b></th>";
                htmlDanhSachDichVu += $"<th style='border: 1px solid #020000; border-collapse: collapse;text-align: right;'><b>{tongCong.ApplyFormatMoneyVND()}</b></th>";

                htmlDanhSachDichVu += " </tr>";
                // end BVHD-3939
            }


            // chẩn đoán sơ 
            var chanDoanSoBo = string.Empty;
            // diễn giải
            var dienGiai = string.Empty;
            var valueCD = _cauHinhRepository.TableNoTracking.Where(d => d.Name == "CauHinhKhamSucKhoe.IcdKhamSucKhoe").Select(d => d.Value).ToList();
            if (valueCD.Any())
            {
                var query = _iCDRepository.TableNoTracking.Where(d => d.Id == long.Parse(valueCD.First()))
                    .Select(d => new
                    {
                        cd = d.Ma + "-" + d.TenTiengViet,
                        dg = d.TenTiengViet
                    }).ToList();
                chanDoanSoBo = query.Any() ? query.First().cd : "";
                dienGiai = query.Any() ? query.First().dg : "";
            }

          

            var data = new
            {
                LogoUrl = hosting + "/assets/img/logo-bacha-full.png",
                BarCodeImgBase64 = !string.IsNullOrEmpty(yeuCauTiepNhan?.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(yeuCauTiepNhan?.MaYeuCauTiepNhan) : "",
                MaTN = yeuCauTiepNhan?.MaYeuCauTiepNhan,
                MaBN = yeuCauTiepNhan?.BenhNhan != null ? yeuCauTiepNhan?.BenhNhan.MaBN : "",
                HoTen = yeuCauTiepNhan?.HoTen ?? "",
                GioiTinhString = yeuCauTiepNhan?.GioiTinh.GetDescription(),
                NamSinh = yeuCauTiepNhan?.NamSinh ?? null,
                DiaChi = yeuCauTiepNhan?.DiaChiDayDu,
                Ngay = ngay,
                Thang = thang,
                Nam = nam,
                DienThoai = yeuCauTiepNhan?.SoDienThoai,

                DoiTuong = yeuCauTiepNhan.CoBHYT == true ? "BHYT (" + yeuCauTiepNhan.BHYTMucHuong + "%)" : "Viện phí",

                SoTheBHYT =  yeuCauTiepNhan?.BHYTMaSoThe,
                
                HanThe = yeuCauTiepNhan.BHYTNgayHieuLuc != null && yeuCauTiepNhan.BHYTNgayHetHan != null ? "từ ngày: " + yeuCauTiepNhan.BHYTNgayHieuLuc.GetValueOrDefault().ToString("dd/MM/yyyy")
                                                                                                          +" đến ngày: " + yeuCauTiepNhan.BHYTNgayHetHan.GetValueOrDefault().ToString("dd/MM/yyyy") :"",

                NoiYeuCau = tenPhong,
                ChuanDoanSoBo = chanDoanSoBo,
                DienGiai = dienGiai,
                DanhSachDichVu = htmlDanhSachDichVu,
                NguoiChiDinh = tenNguoiChiDinh,
                NguoiGiamHo = yeuCauTiepNhan.NguoiLienHeHoTen,
                TenQuanHeThanNhan = yeuCauTiepNhan?.NguoiLienHeQuanHeNhanThan?.Ten,
                PhieuThu = "DichVuKyThuat",
                TongCong = tongtien.ApplyFormatMoneyVND()
            };

            var result3 = _templateRepository.TableNoTracking.FirstOrDefault(x => x.Name.Equals("PhieuChiDinh"));
            content += TemplateHelpper.FormatTemplateWithContentTemplate(result3.Body, data) + "<div class=\"pagebreak\"> </div>";
            if (string.IsNullOrEmpty(data.TenQuanHeThanNhan))
            {
                var tampKB = "<tr id='NguoiGiamHo' style='display:none'>";
                var tmpKB = "<tr id=\"NguoiGiamHo\">";
                var test = content.IndexOf(tmp);
                content = content.Replace(tmpKB, tampKB);
            }

            htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
            htmlDanhSachDichVu += "</tr>";
            i = 1;
            return content;

        }
        #endregion
        #region format tên người chỉ định 
        private string returnStringTen(string maHocHamHocVi, string maNhomChucDanh, string ten)
        {
            var stringTen = string.Empty;
            //chỗ này show theo format: Mã học vị học hàm + dấu cách + Tên bác sĩ
            if (!string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = maHocHamHocVi + " " + ten;
            }
            if (string.IsNullOrEmpty(maHocHamHocVi))
            {
                stringTen = ten;
            }
            return stringTen;
        }
        private string MaHocHamHocVi(long id)
        {
            var maHocHamVi = string.Empty;
            maHocHamVi = _hocViHocHamRepository.TableNoTracking.Where(d => d.Id == id).Select(d => d.Ma).FirstOrDefault();
            return maHocHamVi;
        }
        #endregion

        #region BVHD-3668
        private void XuLyThemDichVuKhamNgoaiChuyenKhoaChinh(YeuCauTiepNhan tiepNhanKSK, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKham, bool laBatDauKham = false, YeuCauTiepNhan yeuCauTiepNhanNgoaiTruDaThem = null)
        {
            //if (tiepNhanNgoaiTruCoHieuLuc != null && tiepNhanNgoaiTruCoHieuLuc.Id != 0)
            //{
            //    throw new Exception(string.Format(_localizationService.GetResource("KhamDoanTiepNhan.TiepNhanNgoaiTru.ConHieuLuc"), tiepNhanNgoaiTruCoHieuLuc.MaYeuCauTiepNhan));
            //}

            if (tiepNhanKSK.Id != 0 || laBatDauKham)
            {
                if (yeuCauTiepNhanNgoaiTruDaThem == null)
                {
                    var tiepNhanNgoaiTruCoHieuLuc = tiepNhanKSK.YeuCauTiepNhanNgoaiTruKhamSucKhoes
                       .FirstOrDefault(x => x.TrangThaiYeuCauTiepNhan == EnumTrangThaiYeuCauTiepNhan.DangThucHien);

                    var now = DateTime.Now;
                    var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    var currentUserId = _userAgentHelper.GetCurrentUserId();

                    if (tiepNhanNgoaiTruCoHieuLuc == null)
                    {
                        tiepNhanNgoaiTruCoHieuLuc = new YeuCauTiepNhan();
                        tiepNhanNgoaiTruCoHieuLuc.MaYeuCauTiepNhan = ResourceHelper.CreateMaYeuCauTiepNhan();
                        tiepNhanNgoaiTruCoHieuLuc.LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru;
                        tiepNhanNgoaiTruCoHieuLuc.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien;
                        tiepNhanNgoaiTruCoHieuLuc.ThoiDiemTiepNhan = now;
                        tiepNhanNgoaiTruCoHieuLuc.ThoiDiemCapNhatTrangThai = now;
                        tiepNhanNgoaiTruCoHieuLuc.NoiTiepNhanId = phongHienTaiId;
                        tiepNhanNgoaiTruCoHieuLuc.NhanVienTiepNhanId = currentUserId;

                        tiepNhanNgoaiTruCoHieuLuc.BenhNhanId = tiepNhanKSK.HopDongKhamSucKhoeNhanVien.BenhNhanId;
                        if (tiepNhanNgoaiTruCoHieuLuc.BenhNhanId == null)
                        {
                            tiepNhanNgoaiTruCoHieuLuc.BenhNhan = tiepNhanKSK.HopDongKhamSucKhoeNhanVien.BenhNhan;
                        }

                        tiepNhanNgoaiTruCoHieuLuc.HoTen = tiepNhanKSK.HoTen;
                        tiepNhanNgoaiTruCoHieuLuc.NgaySinh = tiepNhanKSK.NgaySinh;
                        tiepNhanNgoaiTruCoHieuLuc.ThangSinh = tiepNhanKSK.ThangSinh;
                        tiepNhanNgoaiTruCoHieuLuc.NamSinh = tiepNhanKSK.NamSinh;
                        tiepNhanNgoaiTruCoHieuLuc.SoChungMinhThu = tiepNhanKSK.SoChungMinhThu;
                        tiepNhanNgoaiTruCoHieuLuc.GioiTinh = tiepNhanKSK.GioiTinh;
                        tiepNhanNgoaiTruCoHieuLuc.NgheNghiepId = tiepNhanKSK.NgheNghiepId;
                        tiepNhanNgoaiTruCoHieuLuc.QuocTichId = tiepNhanKSK.QuocTichId;
                        tiepNhanNgoaiTruCoHieuLuc.DanTocId = tiepNhanKSK.DanTocId;
                        tiepNhanNgoaiTruCoHieuLuc.DiaChi = tiepNhanKSK.DiaChi;
                        tiepNhanNgoaiTruCoHieuLuc.PhuongXaId = tiepNhanKSK.PhuongXaId;
                        tiepNhanNgoaiTruCoHieuLuc.QuanHuyenId = tiepNhanKSK.QuanHuyenId;
                        tiepNhanNgoaiTruCoHieuLuc.TinhThanhId = tiepNhanKSK.TinhThanhId;
                        tiepNhanNgoaiTruCoHieuLuc.NhomMau = tiepNhanKSK.NhomMau;
                        tiepNhanNgoaiTruCoHieuLuc.YeuToRh = tiepNhanKSK.YeuToRh;
                        tiepNhanNgoaiTruCoHieuLuc.Email = tiepNhanKSK.Email;
                        tiepNhanNgoaiTruCoHieuLuc.SoDienThoai = tiepNhanKSK.SoDienThoai;

                        var cauHinhLyDoTiepNhanLaKham = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.LyDoTiepNhanLaKhamBenh");
                        long.TryParse(cauHinhLyDoTiepNhanLaKham?.Value, out long lyDoTiepNhanKhamBenhId);
                        tiepNhanNgoaiTruCoHieuLuc.LyDoTiepNhanId = lyDoTiepNhanKhamBenhId == 0 ? (long?)null : lyDoTiepNhanKhamBenhId;

                        tiepNhanKSK.YeuCauTiepNhanNgoaiTruKhamSucKhoes.Add(tiepNhanNgoaiTruCoHieuLuc);
                    }
                    if (tiepNhanNgoaiTruCoHieuLuc?.CoBHYT == true && yeuCauKham.DonGiaBaoHiem.GetValueOrDefault() > 0)
                    {
                        yeuCauKham.DuocHuongBaoHiem = true;
                    }
                    tiepNhanNgoaiTruCoHieuLuc?.YeuCauKhamBenhs.Add(yeuCauKham);
                }
                else
                {
                    if (yeuCauTiepNhanNgoaiTruDaThem?.CoBHYT == true && yeuCauKham.DonGiaBaoHiem.GetValueOrDefault() > 0)
                    {
                        yeuCauKham.DuocHuongBaoHiem = true;
                    }
                    yeuCauTiepNhanNgoaiTruDaThem?.YeuCauKhamBenhs.Add(yeuCauKham);
                }
            }
        }


        #endregion
    }
}
