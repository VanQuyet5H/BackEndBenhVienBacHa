using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial class YeuCauKhamBenhService
    {
        //grid -- tạm thời ko sử dụng
        #region grid

        public async Task<GridDataSource> GetDataForGridBenhNhanLamChiDinhAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                .OrderBy(x => x.LastTime)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh != null ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value : 0,
                    DiaChi = (s.YeuCauTiepNhan.DiaChi == null ? "" : s.YeuCauTiepNhan.DiaChi + " ") +
                             (s.YeuCauTiepNhan.PhuongXa == null ? "" : s.YeuCauTiepNhan.PhuongXa.Ten + " ") +
                             (s.YeuCauTiepNhan.QuanHuyen == null ? "" : s.YeuCauTiepNhan.QuanHuyen.Ten + " ") +
                             (s.YeuCauTiepNhan.TinhThanh == null ? "" : s.YeuCauTiepNhan.TinhThanh.Ten),
                    TinhTrang = s.YeuCauKhamBenh.TrangThai
                        .GetDescription(), //s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan.GetDescription(),

                    PhongBenhVienId = s.PhongBenhVienId,
                    TrangThai = s.TrangThai,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                    ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    //LoaiHangDoi = s.LoaiHangDoi,
                    SoThuTu = s.SoThuTu
                }).ApplyLike(queryInfo.SearchTerms, s => s.MaYeuCauTiepNhan, s => s.HoTen, s => s.DiaChi, s => s.MaBN.ToString())
                .Where(x => x.PhongBenhVienId == long.Parse(queryInfo.AdditionalSearchString)
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenhId != null
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            //&& x.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            //&& x.
                            //== Enums.EnumLoaiHangDoi.LamChiDinh
                            );
            //.OrderBy(x => x.SoThuTu);
            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGriBenhNhanLamChiDinhdAsync(QueryInfo queryInfo)
        {
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan)
                .OrderBy(x => x.LastTime)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhId,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = s.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh != null ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value : 0,
                    DiaChi = (s.YeuCauTiepNhan.DiaChi == null ? "" : s.YeuCauTiepNhan.DiaChi + " ") +
                             (s.YeuCauTiepNhan.PhuongXa == null ? "" : s.YeuCauTiepNhan.PhuongXa.Ten + " ") +
                             (s.YeuCauTiepNhan.QuanHuyen == null ? "" : s.YeuCauTiepNhan.QuanHuyen.Ten + " ") +
                             (s.YeuCauTiepNhan.TinhThanh == null ? "" : s.YeuCauTiepNhan.TinhThanh.Ten),
                    TinhTrang = s.YeuCauKhamBenh.TrangThai
                        .GetDescription(), //s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan.GetDescription(),

                    PhongBenhVienId = s.PhongBenhVienId,
                    TrangThai = s.TrangThai,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                    ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    //LoaiHangDoi = s.LoaiHangDoi,
                    SoThuTu = s.SoThuTu
                }).ApplyLike(queryInfo.SearchTerms, s => s.MaYeuCauTiepNhan, s => s.HoTen, s => s.DiaChi, s => s.MaBN.ToString())
                .Where(x => x.PhongBenhVienId == long.Parse(queryInfo.AdditionalSearchString)
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenhId != null
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            //&& x.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            //&& x.LoaiHangDoi == Enums.EnumLoaiHangDoi.LamChiDinh
                            );
            //.OrderBy(x => x.SoThuTu);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        //// get data

        #region get data

        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachBenhNhanChoKham(long phongKhamHienTai, string searchString = null, bool laKhamDoan = false)
        {
            //áp dụng hàng chờ mới cho màn hình khám bệnh: chỉ có hàng chờ khám và kết luận
            var laKhoaKhamNhieu = true;

            //var laKhoaKhamNhieu = false;
            //if (!laKhamDoan)
            //{
            //    laKhoaKhamNhieu = await KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(phongKhamHienTai);
            //}

            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                //.Include(x => x.YeuCauKhamBenh)
                .Where(x => x.PhongBenhVienId == phongKhamHienTai
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy

                            // cập nhật dùng chung cho phần khám sức khỏe
                            && ((!laKhamDoan
                                 && (!laKhoaKhamNhieu
                                     ? (x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                                     : (x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)))
                                || (laKhamDoan && x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham))
                            && ((!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))

                            // BVHD-3337: nếu ko tìm kiếm thì load theo ngày
                            // bổ sung: chỉ áp dụng với khám bệnh
                            && ((!laKhamDoan && (!string.IsNullOrEmpty(searchString) || x.CreatedOn.Value.Date == DateTime.Now.Date)) || laKhamDoan)
                            )
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    //TenNhomMau = s.YeuCauTiepNhan.NhomMau == null ? null : s.YeuCauTiepNhan.NhomMau.GetDescription(),
                    //Mach = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().NhipTim : null,
                    //HuyetApDisplay = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().HuyetApTamThu + "/" + s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().HuyetApTamTruong : null,
                    //NhipTho = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().NhipTho : null,
                    //CanNang = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().CanNang : null,
                    //NhietDo = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().ThanNhiet : null,
                    //ChieuCao = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().ChieuCao : null,
                    //BMI = s.YeuCauTiepNhan.KetQuaSinhHieus.Count != 0 && s.YeuCauTiepNhan.KetQuaSinhHieus.FirstOrDefault() != null ? s.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(a => a.Id).FirstOrDefault().Bmi : null,
                    //ProgessChiSoSinhTon = false,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "", //s.YeuCauTiepNhan.CoBHYT == true ? "bg-row-lightblue" : ""
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    // cập nhật dùng chung cho khám đoàn
                    //DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                    //TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,

                    //BVHD-3895
                    NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = s.YeuCauTiepNhan.ThangSinh
                })
                .ApplyLike(searchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN);
            //if (laKhamDoan)
            //{
            //    var lstYeuCauTiepNhanId = query.Select(x => x.YeuCauTiepNhanId).Distinct().ToList();
            //    var lstDichVuKhamTheoTiepNhan = await BaseRepository.TableNoTracking
            //        .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                    && lstYeuCauTiepNhanId.Contains(x.YeuCauTiepNhanId))
            //        .Select(item => new YeuCauDichVuKhamTheoTiepNhanVo()
            //        {
            //            YeuCauId = item.Id,
            //            TenDichVu = item.TenDichVu,
            //            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
            //            TrangThai = item.TrangThai
            //        })
            //        .ToListAsync();

            //    foreach (var benhNhan in query)
            //    {
            //        var lstDichVuKham =
            //            lstDichVuKhamTheoTiepNhan.Where(x => x.YeuCauTiepNhanId == benhNhan.YeuCauTiepNhanId);
            //        benhNhan.DichVuKhamDaThucHien = lstDichVuKham.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham).Count();
            //        benhNhan.TongDichVuKham = lstDichVuKham.Count();
            //    }
            //}

            return query.ToList();
        }
        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachChoKhamHangDoiChungAsync(long phongKhamHienTai, string searchString = null, bool laKhamDoan = false)
        {
            var dichVuKhamBenhBenhVienNoiThucHiens = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking.ToList();
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.ToList();
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId != phongKhamHienTai
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy

                            // cập nhật dùng chung cho phần khám sức khỏe
                            && x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                            && ((!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))

                            // BVHD-3337: nếu ko tìm kiếm thì load theo ngày
                            // bổ sung: chỉ áp dụng với khám bệnh
                            //&& ((!laKhamDoan && (!string.IsNullOrEmpty(searchString) || x.CreatedOn.Value.Date == DateTime.Now.Date)) || laKhamDoan)

                            && dichVuKhamBenhBenhVienNoiThucHiens
                                .Any(a => a.DichVuKhamBenhBenhVienId == x.YeuCauKhamBenh.DichVuKhamBenhBenhVienId &&
                                ((a.PhongBenhVienId != null && a.PhongBenhVienId == phongKhamHienTai)
                                          || (a.KhoaPhongId != null && phongBenhViens.Any(b => a.KhoaPhongId == b.KhoaPhongId && b.Id == phongKhamHienTai))))
                            )
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "",
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    // cập nhật dùng chung cho khám đoàn
                    //DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                    //TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,

                    //BVHD-3895
                    NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = s.YeuCauTiepNhan.ThangSinh
                })
                .ApplyLike(searchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN);

            //if (laKhamDoan)
            //{
            //    var lstYeuCauTiepNhanId = query.Select(x => x.YeuCauTiepNhanId).Distinct().ToList();
            //    var lstDichVuKhamTheoTiepNhan = await BaseRepository.TableNoTracking
            //        .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                    && lstYeuCauTiepNhanId.Contains(x.YeuCauTiepNhanId))
            //        .Select(item => new YeuCauDichVuKhamTheoTiepNhanVo()
            //        {
            //            YeuCauId = item.Id,
            //            TenDichVu = item.TenDichVu,
            //            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
            //            TrangThai = item.TrangThai
            //        })
            //        .ToListAsync();

            //    foreach (var benhNhan in query)
            //    {
            //        var lstDichVuKham =
            //            lstDichVuKhamTheoTiepNhan.Where(x => x.YeuCauTiepNhanId == benhNhan.YeuCauTiepNhanId);
            //        benhNhan.DichVuKhamDaThucHien = lstDichVuKham.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham).Count();
            //        benhNhan.TongDichVuKham = lstDichVuKham.Count();
            //    }
            //}
            return query.ToList();
        }

        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachChoKetLuanHangDoiChungAsync(long phongKhamHienTai, string searchString = null)
        {
            var dichVuKhamBenhBenhVienNoiThucHiens = _dichVuKhamBenhBenhVienNoiThucHienRepository.TableNoTracking.ToList();
            var phongBenhViens = _phongBenhVienRepository.TableNoTracking.ToList();
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId != phongKhamHienTai
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenh != null
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy

                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham

                            && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe

                            && dichVuKhamBenhBenhVienNoiThucHiens
                                .Any(a => a.DichVuKhamBenhBenhVienId == x.YeuCauKhamBenh.DichVuKhamBenhBenhVienId &&
                                ((a.PhongBenhVienId != null && a.PhongBenhVienId == phongKhamHienTai)
                                          || (a.KhoaPhongId != null && phongBenhViens.Any(b => a.KhoaPhongId == b.KhoaPhongId && b.Id == phongKhamHienTai))))
                            )
                .OrderByDescending(x => x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan).ThenBy(x => x.CreatedOn.Value.Date).ThenBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan ? "bg-row-lightblue" : "",
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    NoiDangKyId = s.PhongBenhVienId,

                    //BVHD-3895
                    NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = s.YeuCauTiepNhan.ThangSinh
                })
                .ApplyLike(searchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN);

            return query.ToList();
        }

        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachLamChiDinhHienTaiAsync(long phongKhamHienTaiId, string searchString = null)
        {
            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                //.Include(x => x.YeuCauTiepNhan)
                //.Include(x => x.YeuCauKhamBenh)
                .Where(x => x.PhongBenhVienId == phongKhamHienTaiId
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh

                            // BVHD-3337: nếu ko tìm kiếm thì load theo ngày
                            && (!string.IsNullOrEmpty(searchString) || x.CreatedOn.Value.Date == DateTime.Now.Date))
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh != null ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault() : 0,
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    //YeuCauDichVuKyThuats = s.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList(),
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    //HighLightClass = s.YeuCauTiepNhan.CoBHYT == true ? "bg-row-lightblue" : ""
                    NgayKhamBenh = s.CreatedOn.Value.Date,
                })
                .ApplyLike(searchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN);

            return query.ToList();
        }
        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachDoiKetLuanHienTaiAsync(long phongKhamHienTaiId, string searchString = null, bool laKhamDoan = false)
        {
            //áp dụng hàng chờ mới cho màn hình khám bệnh: chỉ có hàng chờ khám và kết luận
            // hàng đợi kết luận là gộp hàng đợi( kết luận cũ, chỉ định cũ, chờ khám (nhừng người bệnh đã lưu thông tin))
            var laKhoaKhamNhieu = true;

            //var laKhoaKhamNhieu = false;
            //if (!laKhamDoan)
            //{
            //    laKhoaKhamNhieu = await KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(phongKhamHienTaiId);
            //}

            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == phongKhamHienTaiId
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && ((!laKhamDoan && (!laKhoaKhamNhieu
                                                    ? x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                      : (x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                         || x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                                                         || x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangKham)))
                                || (laKhamDoan && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham))
                            && ((!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))

                            // BVHD-3337: nếu ko tìm kiếm thì load theo ngày
                            // bổ sung: chỉ áp dụng với khám bệnh
                            && ((!laKhamDoan && (!string.IsNullOrEmpty(searchString) || x.CreatedOn.Value.Date == DateTime.Now.Date)) || laKhamDoan)
                            );
            //.OrderBy(x => x.SoThuTu)

            if (laKhoaKhamNhieu && !laKhamDoan)
            {
                query = query.OrderByDescending(x => x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan).ThenBy(x => x.SoThuTu);
            }
            else
            {
                query = query.OrderBy(x => x.SoThuTu);
            }

            var result = query
            .Select(s => new BenhNhanChoKhamGridVo
            {
                Id = s.Id,
                MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = s.YeuCauTiepNhan.HoTen,
                MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                Tuoi = s.YeuCauTiepNhan.NamSinh != null ? DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.GetValueOrDefault() : 0,
                NamSinh = s.YeuCauTiepNhan.NamSinh,
                LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                //YeuCauDichVuKyThuats = s.YeuCauKhamBenh.YeuCauDichVuKyThuats.Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).ToList(),
                YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                //HighLightClass = s.YeuCauTiepNhan.CoBHYT == true ? "bg-row-lightblue" : ""
                NgayKhamBenh = s.CreatedOn.Value.Date,


                // cập nhật dùng chung cho khám đoàn
                //DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                //TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                YeuCauTiepNhanId = s.YeuCauTiepNhanId,

                HighLightClass = (laKhoaKhamNhieu && !laKhamDoan && s.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan) ? "bg-row-lightblue" : "",

                //BVHD-3895
                NgaySinh = s.YeuCauTiepNhan.NgaySinh,
                ThangSinh = s.YeuCauTiepNhan.ThangSinh
            })
            .ApplyLike(searchString, x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN);

            //if (laKhamDoan)
            //{
            //    var lstYeuCauTiepNhanId = query.Select(x => x.YeuCauTiepNhanId).Distinct().ToList();
            //    var lstDichVuKhamTheoTiepNhan = await BaseRepository.TableNoTracking
            //        .Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
            //                    && lstYeuCauTiepNhanId.Contains(x.YeuCauTiepNhanId))
            //        .Select(item => new YeuCauDichVuKhamTheoTiepNhanVo()
            //        {
            //            YeuCauId = item.Id,
            //            TenDichVu = item.TenDichVu,
            //            YeuCauTiepNhanId = item.YeuCauTiepNhanId,
            //            TrangThai = item.TrangThai
            //        })
            //        .ToListAsync();

            //    foreach (var benhNhan in query)
            //    {
            //        var lstDichVuKham =
            //            lstDichVuKhamTheoTiepNhan.Where(x => x.YeuCauTiepNhanId == benhNhan.YeuCauTiepNhanId);
            //        benhNhan.DichVuKhamDaThucHien = lstDichVuKham.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham).Count();
            //        benhNhan.TongDichVuKham = lstDichVuKham.Count();
            //    }
            //}
            return result.ToList();
        }

        public async Task<SoLuongYeuCauHienTaiVo> GetSoLuongYeuCauHienTai(long phongKhamId, bool laKhamDoan)
        {
            var slYeuCauHienTai = new SoLuongYeuCauHienTaiVo
            {
                ChuanBiKham = 0,
                DangLamChiDinh = 0,
                DangDoiKetLuan = 0
            };


            //áp dụng hàng chờ mới cho màn hình khám bệnh: chỉ có hàng chờ khám và kết luận
            var laKhoaKhamNhieu = true;

            //var laKhoaKhamNhieu = false;
            //if (!laKhamDoan)
            //{
            //    laKhoaKhamNhieu = await KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(phongKhamId);
            //}

            var dsYeuCauTiepNhanHienTai =
                await _phongBenhVienHangDoiRepository.TableNoTracking
                    .Include(x => x.YeuCauTiepNhan)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.DichVuKhamBenhBenhVien).ThenInclude(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(x => x.KhoaPhong)
                    .Where(x => x.PhongBenhVienId == phongKhamId
                                && x.YeuCauKhamBenh != null
                                //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy

                                // cập nhật dùng chung cho khám đoàn
                                && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                                && ((!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                                    || (laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))).ToListAsync();
            if (dsYeuCauTiepNhanHienTai != null)
            {
                if (!laKhoaKhamNhieu)
                {
                    slYeuCauHienTai.ChuanBiKham =
                        dsYeuCauTiepNhanHienTai.Count(x => x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham);
                    slYeuCauHienTai.DangDoiKetLuan =
                        dsYeuCauTiepNhanHienTai.Count(x => x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan);
                    slYeuCauHienTai.DangLamChiDinh =
                        dsYeuCauTiepNhanHienTai.Count(x => x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh);
                }
                else
                {
                    slYeuCauHienTai.ChuanBiKham =
                        dsYeuCauTiepNhanHienTai.Count(x => x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham);
                    slYeuCauHienTai.DangDoiKetLuan =
                        dsYeuCauTiepNhanHienTai.Count(x => x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                                                           || x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham
                                                           || x.YeuCauKhamBenh.TrangThai == EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh);
                }
            }
            return slYeuCauHienTai;
        }

        // hàm này hiện tại ko dùng
        public async Task<PhongBenhVienHangDoi> TimKiemBenhNhanTrongHangDoi(string searchString, long phongKhamId)
        {
            var query = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
                .Where(x =>
                             //(x.LoaiHangDoi == Enums.EnumLoaiHangDoi.ChuanBiKham || x.LoaiHangDoi == Enums.EnumLoaiHangDoi.LamChiDinh)
                             x.PhongBenhVienId == phongKhamId
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.TrangThai == Enums.EnumTrangThaiHangDoi.ChoKham
                            && (searchString == x.YeuCauTiepNhan.MaYeuCauTiepNhan ||
                            searchString.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan) ||
                            searchString == x.YeuCauTiepNhan.BenhNhanId.ToString()))
                .FirstOrDefaultAsync();
            return query;
        }

        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoPhongKham(long phongKhamId, long? hangDoiId, bool laKhamDoan = false, Enums.EnumTrangThaiHangDoi trangThai = Enums.EnumTrangThaiHangDoi.DangKham)
        {
            #region Code cũ
            //todo: có cập nhật bỏ await
            //var query =
            //     _phongBenhVienHangDoiRepository.Table
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NhanVienTiepNhan).ThenInclude(u => u.User)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.LyDoTiepNhan)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.Icdchinh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs).ThenInclude(y => y.ICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DuongDung)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DonViTinh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonVTYTs).ThenInclude(z => z.YeuCauKhamBenhDonVTYTChiTiets)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTrieuChungs).ThenInclude(z => z.TrieuChung)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChuanDoans).ThenInclude(z => z.ChuanDoan)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTruoc)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.ChanDoanSoBoICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.KhoaPhongNhapVien)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.BenhVienChuyenVien)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(z => z.ICD)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.User)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.ChucDanh).ThenInclude(u => u.NhomChucDanh)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.VanBangChuyenMon)
            //        .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.DichVuKhamBenhBenhVien)

            //        // bổ sung khám đoàn
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.HopDongKhamSucKhoeNhanVien).ThenInclude(t => t.HopDongKhamSucKhoe).ThenInclude(u => u.CongTyKhamSucKhoe)
            //        //  dịch vụ khuyến mãi
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)

            //         .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
            //        .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
            //        .Where(x => (hangDoiId == 0 || x.Id == hangDoiId)
            //                    && x.PhongBenhVienId == phongKhamId
            //                    && x.TrangThai == trangThai
            //                    && x.YeuCauKhamBenh != null
            //                    //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
            //                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
            //                    && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
            //                    ).FirstOrDefault();
            #endregion

            #region Cập nhật 28/03/2022
            //var hangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
            //    .FirstOrDefault(x => (hangDoiId == 0 || x.Id == hangDoiId)
            //                         && x.PhongBenhVienId == phongKhamId
            //                         && x.TrangThai == trangThai
            //                         && x.YeuCauKhamBenhId != null
            //                         //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
            //                         && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
            //                         && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
            //    );

            var hangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => (hangDoiId == 0 || x.Id == hangDoiId)
                                     && x.PhongBenhVienId == phongKhamId
                                     && x.TrangThai == trangThai
                                     && x.YeuCauKhamBenhId != null
                                     //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                                     && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                     && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
                )
                .Select(x => new
                {
                    Id = x.Id
                }).FirstOrDefault();

            if (hangDoi == null)
            {
                return null;
            }

            var query =
                 _phongBenhVienHangDoiRepository.Table.Where(x => x.Id == hangDoi.Id)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NhanVienTiepNhan).ThenInclude(u => u.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.LyDoTiepNhan)
                    //.Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.Icdchinh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs).ThenInclude(y => y.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DuongDung)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonThuocs).ThenInclude(z => z.YeuCauKhamBenhDonThuocChiTiets).ThenInclude(a => a.DonViTinh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhDonVTYTs).ThenInclude(z => z.YeuCauKhamBenhDonVTYTChiTiets)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTrieuChungs).ThenInclude(z => z.TrieuChung)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChuanDoans).ThenInclude(z => z.ChuanDoan)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhTruoc)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.ChanDoanSoBoICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.KhoaPhongNhapVien)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.BenhVienChuyenVien)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(z => z.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.User)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.ChucDanh).ThenInclude(u => u.NhomChucDanh)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.NhanVienHoTongChuyenVien).ThenInclude(u => u.VanBangChuyenMon)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.DichVuKhamBenhBenhVien)

                    // bổ sung khám đoàn
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.HopDongKhamSucKhoeNhanVien).ThenInclude(t => t.HopDongKhamSucKhoe).ThenInclude(u => u.CongTyKhamSucKhoe)
                    ////  dịch vụ khuyến mãi
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)

                    // .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    //.Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)

                    //BVHD-3960 - [PHÁT SINH TRIỂN KHAI] Thêm thông tin hình thức đến (27/07/2022)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.HinhThucDen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.NoiGioiThieu)
                    .FirstOrDefault();
            #endregion

            return query;
        }

        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhTiepTheoTheoPhongKham(long hangDoiId, bool laKhamDoan = false)
        {
            var query = new PhongBenhVienHangDoi();
            if (laKhamDoan)
            {
                query = _phongBenhVienHangDoiRepository.TableNoTracking
                        .Where(x => x.Id == hangDoiId)
                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                           .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)

                           // bổ sung khám đoàn
                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.HopDongKhamSucKhoeNhanVien).ThenInclude(t => t.HopDongKhamSucKhoe).ThenInclude(u => u.CongTyKhamSucKhoe)
                           .FirstOrDefault();
            }
            else
            {
                query = _phongBenhVienHangDoiRepository.TableNoTracking
                        .Where(x => x.Id == hangDoiId)
                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                           .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                           .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauDichVuKyThuats)
                           .FirstOrDefault();
            }

            return query;
        }

        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoPhongKhamLuuTabKhamBenh(long phongKhamId, long? hangDoiId, bool laKhamDoan = false, Enums.EnumTrangThaiHangDoi trangThai = Enums.EnumTrangThaiHangDoi.DangKham)
        {
            var hangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => (hangDoiId == 0 || x.Id == hangDoiId)
                                     && x.PhongBenhVienId == phongKhamId
                                     && x.TrangThai == trangThai
                                     && x.YeuCauKhamBenhId != null
                                     //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                                     && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                     && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe))
                )
                .Select(x => new
                {
                    Id = x.Id
                }).FirstOrDefault();

            if (hangDoi == null)
            {
                return null;
            }

            var query =
                 _phongBenhVienHangDoiRepository.Table.Where(x => x.Id == hangDoi.Id)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhICDKhacs)//.ThenInclude(y => y.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhKhamBoPhanKhacs)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhChanDoanPhanBiets)//.ThenInclude(z => z.ICD)
                    .Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhBoPhanTonThuongs)
                    //.Include(x => x.YeuCauKhamBenh).ThenInclude(y => y.YeuCauKhamBenhLichSuTrangThais)
                    .FirstOrDefault();

            return query;
        }

        public async Task<List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> GetTemplateCacDichVuKhamSucKhoeAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            //todo: có update bỏ await
            var lstDichVu = BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.Id != yeuCauKhamBenhId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.PhongBenhVienHangDois.Any())
                .ToList();
            return lstDichVu;
        }

        public async Task<IQueryable<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> GetTemplateCacDichVuKhamSucKhoeAsyncVer2(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            //todo: có update bỏ await
            var lstDichVu = BaseRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                            && x.Id != yeuCauKhamBenhId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                            && x.PhongBenhVienHangDois.Any()

                            //BVHD-3668
                            && (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan)
                            );
            return lstDichVu;
        }
        #endregion

        #region InPhieu
        private ThongTinChungCuaBenhNhan ThongTinBenhNhanHienTai(long yeuCauKhamBenhId)
        {
            var yeuCauKhamBenh = BaseRepository.GetById(yeuCauKhamBenhId,
                                       x => x.Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NguoiLienHeQuanHeNhanThan)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NgheNghiep)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.PhuongXa)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.QuanHuyen)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.TinhThanh)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.DanToc)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.QuocTich)
                                             .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.BenhNhan)
                                             .Include(yckb => yckb.BacSiThucHien).ThenInclude(bs => bs.User));

            var userCurrentId = _userAgentHelper.GetCurrentUserId();
            //var tenuserCurrent = _userRepository.TableNoTracking
            //                     .Where(u => u.Id == userCurrentId).Select(u => u.HoTen).FirstOrDefault();
            var tenuserCurrent = _userRepository.TableNoTracking
                                .Where(u => u.Id == userCurrentId).Select(u =>
                                (u.NhanVien.HocHamHocVi != null ? u.NhanVien.HocHamHocVi.Ma + " " : "")
                              //+ (u.NhanVien.ChucDanh != null ? u.NhanVien.ChucDanh.NhomChucDanh.Ma + "." : "")
                              + u.HoTen).FirstOrDefault();
            var tuoiThoiDiemHienTai = 0;
            if (yeuCauKhamBenh.YeuCauTiepNhan.NamSinh != null)
            {
                tuoiThoiDiemHienTai = DateTime.Now.Year - yeuCauKhamBenh.YeuCauTiepNhan.NamSinh.Value;
            }
            var dobConvert = DateHelper.ConvertDOBToTimeJson(yeuCauKhamBenh.YeuCauTiepNhan.NgaySinh, yeuCauKhamBenh.YeuCauTiepNhan.ThangSinh, yeuCauKhamBenh.YeuCauTiepNhan.NamSinh);
            var jsonConvertString = new NgayThangNamSinhVo();

            if (!string.IsNullOrEmpty(dobConvert) && tuoiThoiDiemHienTai < 6)
            {
                jsonConvertString = JsonConvert.DeserializeObject<NgayThangNamSinhVo>(dobConvert);
            }

            var tuoiBenhNhan = yeuCauKhamBenh.YeuCauTiepNhan.NamSinh != null ?
                            (tuoiThoiDiemHienTai < 6 ? jsonConvertString.Years + " Tuổi " + jsonConvertString.Months + " Tháng " + jsonConvertString.Days + " Ngày" : tuoiThoiDiemHienTai.ToString()) : tuoiThoiDiemHienTai.ToString();

            var result = new ThongTinChungCuaBenhNhan
            {
                MaTN = yeuCauKhamBenh.YeuCauTiepNhan.MaYeuCauTiepNhan,
                MaBN = yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.MaBN,
                HoTenBenhNhan = yeuCauKhamBenh.YeuCauTiepNhan.HoTen.ToUpper(),
                SinhNgay = DateHelper.DOBFormat(yeuCauKhamBenh.YeuCauTiepNhan.NgaySinh, yeuCauKhamBenh.YeuCauTiepNhan.ThangSinh, yeuCauKhamBenh.YeuCauTiepNhan.NamSinh),
                Tuoi = tuoiBenhNhan ?? "0",
                GioiTinh = yeuCauKhamBenh.YeuCauTiepNhan.GioiTinh.GetDescription(),
                NgheNghiep = yeuCauKhamBenh.YeuCauTiepNhan.NgheNghiep?.Ten,
                DanToc = yeuCauKhamBenh.YeuCauTiepNhan.DanToc?.Ten,
                SoNha = yeuCauKhamBenh.YeuCauTiepNhan.DiaChi,
                DiaChiDayDu = yeuCauKhamBenh.YeuCauTiepNhan.DiaChiDayDu,
                //ThonPho
                XaPhuong = yeuCauKhamBenh.YeuCauTiepNhan.PhuongXa?.Ten,
                Huyen = yeuCauKhamBenh.YeuCauTiepNhan.QuanHuyen?.Ten,
                TinhThanhPho = yeuCauKhamBenh.YeuCauTiepNhan.TinhThanh?.Ten,
                NoiLamViec = yeuCauKhamBenh.YeuCauTiepNhan.NoiLamViec,
                QuocTich = yeuCauKhamBenh.YeuCauTiepNhan.QuocTich?.QuocTich,
                DoiTuong = yeuCauKhamBenh.YeuCauTiepNhan.CoBHYT != true ? "Viện phí" : "BHYT (" + yeuCauKhamBenh.YeuCauTiepNhan.BHYTMucHuong.ToString() + "%)",
                BHYTNgayHetHan = (yeuCauKhamBenh.YeuCauTiepNhan.BHYTNgayHieuLuc != null || yeuCauKhamBenh.YeuCauTiepNhan.BHYTNgayHetHan != null) ? "từ " + (yeuCauKhamBenh.YeuCauTiepNhan.BHYTNgayHieuLuc?.ApplyFormatDate() ?? "") + " đến " + (yeuCauKhamBenh.YeuCauTiepNhan.BHYTNgayHetHan?.ApplyFormatDate() ?? "") : "",
                //BHYTMaSoThe = yeuCauKhamBenh.YeuCauTiepNhan.BHYTMaSoThe + (yeuCauKhamBenh.YeuCauTiepNhan.BHYTMaDKBD == null ? "" : " - " + yeuCauKhamBenh.YeuCauTiepNhan.BHYTMaDKBD),
                BHYTMaSoThe = yeuCauKhamBenh.YeuCauTiepNhan.BHYTMaSoThe,
                BHYTMaSoThe2 = yeuCauKhamBenh.YeuCauTiepNhan.BHYTMaSoThe,
                NguoiLienHeQuanHeThanNhan = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.Ten + (yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen != null ? " " + yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen : ""),
                NguoiLienHeQuanSoDienThoai = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeSoDienThoai.ApplyFormatPhone(),
                //NguoiLienHeDiaChiDayDu = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeDiaChiDayDu,
                SoDienThoai = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeSoDienThoai.ApplyFormatPhone(),
                ThoiDiemTiepNhan = yeuCauKhamBenh.YeuCauTiepNhan.ThoiDiemTiepNhan.ConvertDatetimeToString(),
                ThoiDiemTiepNhanFormat = yeuCauKhamBenh.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                ChanDoanNoiGioiThieu = yeuCauKhamBenh.ChanDoanCuaNoiGioiThieu,
                Ngay = DateTime.Today.Day.ConvertDateToString(),
                Thang = DateTime.Today.Month.ConvertMonthToString(),
                Nam = DateTime.Today.Year.ToString(),
                HoTenBacSi = tenuserCurrent,
                BenhSu = yeuCauKhamBenh.BenhSu?.Replace("\n", "<br>"),

                //Cập nhật 22/06/2022 -> yêu cầu phiếu khám và phiếu vào viện hiển thị số điện thoại từ 2 nguồn trong YCTN: SDT và SDT người liên hệ
                SoDienThoaiNguoiBenh = yeuCauKhamBenh.YeuCauTiepNhan.SoDienThoaiDisplay,
            };
            return result;
        }


        public List<string> InPhieuKhamBenh(PhieuKhamBenhVo phieuKhamBenhVo)
        {
            var contentPhieuKhamBenh = string.Empty;
            var contentPhieuKhamBenhVaoVien = string.Empty;
            var content = string.Empty;
            var contentList = new List<string>();
            var yeuCauKhamBenh = BaseRepository.GetById(phieuKhamBenhVo.YeuCauKhamBenhId,
                                     x => x.Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.KetQuaSinhHieus)
                                           .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.BenhNhan).ThenInclude(bn => bn.BenhNhanTienSuBenhs)
                                           .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.BenhNhan).ThenInclude(bn => bn.BenhNhanDiUngThuocs)
                                           .Include(yckb => yckb.YeuCauKhamBenhKhamBoPhanKhacs)
                                           .Include(yckb => yckb.YeuCauKhamBenhBoPhanTonThuongs)
                                           .Include(yckb => yckb.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(z => z.DonViTinh)
                                           .Include(yckb => yckb.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien).ThenInclude(z => z.DuocPham).ThenInclude(z => z.DuongDung)
                                           .Include(yckb => yckb.YeuCauVatTuBenhViens)
                                           .Include(yckb => yckb.KhoaPhongNhapVien)
                                           .Include(yckb => yckb.YeuCauDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)
                                           .Include(yckb => yckb.Icdchinh)
                                           .Include(yckb => yckb.YeuCauKhamBenhICDKhacs).ThenInclude(z => z.ICD)
                                           .Include(yckb => yckb.BacSiKetLuan).ThenInclude(z => z.HocHamHocVi)
                                           .Include(yckb => yckb.BacSiKetLuan).ThenInclude(z => z.User)
                                           );


            var thongTinChungBN = ThongTinBenhNhanHienTai(phieuKhamBenhVo.YeuCauKhamBenhId);
            var tienSuDiUngThuoc = string.Empty;
            var tienSuDiUngThucAn = string.Empty;
            var tienSuDiUngKhac = string.Empty;
            var ketQuaSinhHieu = yeuCauKhamBenh.YeuCauTiepNhan.KetQuaSinhHieus.OrderByDescending(kq => kq.Id).FirstOrDefault();
            var tienSuBenhBanThan = string.Empty;
            var tienSuBenhGiaDinh = string.Empty;
            foreach (var item in yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanTienSuBenhs)
            {
                if (item.LoaiTienSuBenh == Enums.EnumLoaiTienSuBenh.BanThan)
                {
                    if (!string.IsNullOrEmpty(item.TenBenh))
                    {
                        tienSuBenhBanThan += item.TenBenh + "; ";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.TenBenh))
                    {
                        tienSuBenhGiaDinh += item.TenBenh + "; ";
                    }
                }
            }

            foreach (var item in yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs)
            {
                if (item.LoaiDiUng == Enums.LoaiDiUng.Thuoc)
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngThuoc += item.TenDiUng + "; ";
                    }
                }
                else if (item.LoaiDiUng == Enums.LoaiDiUng.ThucAn)
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngThucAn += item.TenDiUng + "; ";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngKhac += item.TenDiUng + "; ";
                    }
                }
            }
            //var yeuCauDuocPhamBenhViens = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViensHuongThanGayNghien = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z =>
                                                                                                  z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                               && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                               && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                               && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                               && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                               && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.HuongThan
                                                                                                  || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy == LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViensTiem = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "2.10".Trim()
                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViensUong = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "1.01".Trim()
                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViensDat = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "4.04".Trim()
                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViensDungNgoai = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() == "3.05".Trim()
                                                                                     && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                     && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                || z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)).Select(z => z.Ten).Distinct().ToList();

            var yeuCauDuocPhamBenhViensKhac = yeuCauKhamBenh.YeuCauDuocPhamBenhViens.Where(z => z.DuongDung.Ma.Trim() != "2.10".Trim()
                                                                                             && z.DuongDung.Ma.Trim() != "1.01".Trim()
                                                                                             && z.DuongDung.Ma.Trim() != "4.04".Trim()
                                                                                             && z.DuongDung.Ma.Trim() != "3.05".Trim()
                                                                                             && z.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy
                                                                                             && (z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.HuongThan
                                                                                                && z.DuocPhamBenhVien.LoaiThuocTheoQuanLy != LoaiThuocTheoQuanLy.GayNghien)
                                                                                             ).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDuocPhamBenhViens = (yeuCauDuocPhamBenhViensHuongThanGayNghien
                                                .Concat(yeuCauDuocPhamBenhViensTiem)
                                                .Concat(yeuCauDuocPhamBenhViensUong)
                                                .Concat(yeuCauDuocPhamBenhViensDat)
                                                .Concat(yeuCauDuocPhamBenhViensDungNgoai)
                                                .Concat(yeuCauDuocPhamBenhViensKhac)).Distinct();

            var yeuCauVatTuBenhViens = yeuCauKhamBenh.YeuCauVatTuBenhViens.Where(z => z.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy).Select(z => z.Ten).Distinct().ToList();
            var yeuCauDichVuKyThuats = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                            && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                            && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p"
                                                                            #region Cập nhật 26/12/2022: bỏ trạng thái đã huỷ
                                                                            && z.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                            #endregion
                                                                            ).Select(z => z.TenDichVu).Distinct().ToList();
            var icdPhus = new List<string>();
            foreach (var item in yeuCauKhamBenh.YeuCauKhamBenhICDKhacs)
            {
                icdPhus.Add(item.ICD.Ma + " - " + item.ICD.TenTiengViet + (!string.IsNullOrEmpty(item.GhiChu) ? " (" + item.GhiChu?.Replace("\n", "<br>") + ")" : ""));
            }

            //var coNoiDungKhac = KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(yeuCauKhamBenh.DichVuKhamBenhBenhVienId).Result;
            var phongKham = string.Empty;
            if (phieuKhamBenhVo.CoKhamBenh)
            {
                if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Mắt".RemoveUniKeyAndToLower()))
                {
                    var templatePhieuKhamBenhKM = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienChuyenKhoaMat")).FirstOrDefault();
                    phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                    var templateDichVuMat = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                    var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                    var thongTinBenhMatChiTiet = new ThongTinBenhMatChiTiet();
                    if (!string.IsNullOrEmpty(templateDichVuMat))
                    {
                        thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuMat);
                        foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                        {
                            if (item.Id == "ThiLucKhongKinh")
                            {
                                thongTinBenhMatChiTiet.ThiLucKhongKinh = item.Value;
                            }
                            if (item.Id == "ThiLucKhongKinhMatPhai")
                            {
                                thongTinBenhMatChiTiet.MPKK = item.Value;
                            }
                            if (item.Id == "ThiLucKhongKinhMatTrai")
                            {
                                thongTinBenhMatChiTiet.MTKK = item.Value;
                            }
                            if (item.Id == "NhanAp")
                            {
                                thongTinBenhMatChiTiet.NhanAp = item.Value;
                            }
                            if (item.Id == "NhanApMatPhai")
                            {
                                thongTinBenhMatChiTiet.MPNA = item.Value;
                            }
                            if (item.Id == "NhanApMatTrai")
                            {
                                thongTinBenhMatChiTiet.MTNA = item.Value;
                            }
                            if (item.Id == "ThiLucCoKinh")
                            {
                                thongTinBenhMatChiTiet.ThiLucCoKinh = item.Value;
                            }
                            if (item.Id == "ThiLucCoKinhPhai")
                            {
                                thongTinBenhMatChiTiet.MPCK = item.Value;
                            }
                            if (item.Id == "ThiLucCoKinhTrai")
                            {
                                thongTinBenhMatChiTiet.MTCK = item.Value;
                            }
                            if (item.Id == "KhamMatNoiDung")
                            {
                                thongTinBenhMatChiTiet.KhamMatNoiDung = item.Value;
                            }
                        }
                    }

                    var data = new ThongTinBenhMat
                    {
                        MaBN = thongTinChungBN.MaBN,
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        QuocTich = thongTinChungBN?.QuocTich,
                        SoNha = thongTinChungBN?.SoNha,
                        XaPhuong = thongTinChungBN?.XaPhuong,
                        Huyen = thongTinChungBN?.Huyen,
                        TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                        NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                        SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                        ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>") ?? "&nbsp;",
                        ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                        LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                        TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                            ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                        //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                        NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                        KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                        TomTatCLS = yeuCauKhamBenh.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                        DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                        HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                        PhongKham = phongKham,
                        KhamMat = thongTinBenhMatChiTiet.KhamMatNoiDung?.Replace("\n", "<br>"),
                        MPCK = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MPCK) ? thongTinBenhMatChiTiet.MPCK?.Replace("\n", "<br>") : "&nbsp;",
                        MPKK = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MPKK) ? thongTinBenhMatChiTiet.MPKK?.Replace("\n", "<br>") : "&nbsp;",
                        MTCK = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MTCK) ? thongTinBenhMatChiTiet.MTCK?.Replace("\n", "<br>") : "&nbsp;",
                        MTKK = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MTKK) ? thongTinBenhMatChiTiet.MTKK?.Replace("\n", "<br>") : "&nbsp;",
                        MTNA = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MTNA) ? thongTinBenhMatChiTiet.MTNA?.Replace("\n", "<br>") : "&nbsp;",
                        MPNA = !string.IsNullOrEmpty(thongTinBenhMatChiTiet.MPNA) ? thongTinBenhMatChiTiet.MPNA?.Replace("\n", "<br>") : "&nbsp;",
                        HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                        QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                        ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                        //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                        //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                        NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                        NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                    };
                    contentPhieuKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhKM.Body, data);
                    if (contentPhieuKhamBenh != "")
                    {
                        contentPhieuKhamBenh = contentPhieuKhamBenh + "<div class=\"pagebreak\"> </div>";
                    }
                    contentList.Add(contentPhieuKhamBenh);
                }
                else if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Tai mũi họng".RemoveUniKeyAndToLower()))
                {
                    var templatePhieuKhamBenhTMH = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienTaiMuiHong")).FirstOrDefault();
                    phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                    var templateDichVuTMH = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                    var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                    var thongTinBenhTMHChiTiet = new ThongTinBenhNhanTaiMuiHongChiTiet();
                    if (!string.IsNullOrEmpty(templateDichVuTMH))
                    {
                        thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuTMH);
                        foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                        {
                            if (item.Id == "Tai")
                            {
                                thongTinBenhTMHChiTiet.Tai = item.Value;
                            }
                            if (item.Id == "Mui")
                            {
                                thongTinBenhTMHChiTiet.Mui = item.Value;
                            }
                            if (item.Id == "Hong")
                            {
                                thongTinBenhTMHChiTiet.Hong = item.Value;
                            }
                        }
                    }
                    var daXuLis = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                            && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                            && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p").Select(z => z.TenDichVu).Distinct().ToList();

                    var data = new ThongTinBenhTaiMuiHong
                    {
                        MaBN = thongTinChungBN.MaBN,
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        QuocTich = thongTinChungBN?.QuocTich,
                        SoNha = thongTinChungBN?.SoNha,
                        XaPhuong = thongTinChungBN?.XaPhuong,
                        Huyen = thongTinChungBN?.Huyen,
                        TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                        NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                        SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                        ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>") ?? "&nbsp;",
                        //ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu,
                        //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                        ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                        LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                        TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                            ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                        //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                        //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                        NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                        KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                        TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                        //DaXuLi = string.Join("; ", daXuLis),
                        DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                        HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                        PhongKham = phongKham,
                        Tai = thongTinBenhTMHChiTiet.Tai,
                        Mui = thongTinBenhTMHChiTiet.Mui,
                        Hong = thongTinBenhTMHChiTiet.Hong,
                        HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                        QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                        //ChanDoanVaoVien = yeuCauKhamBenh.Icdchinh?.Ma + "-" + yeuCauKhamBenh.Icdchinh?.TenTiengViet,
                        ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                        //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                        //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                        NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                        NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                    };
                    contentPhieuKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhTMH.Body, data);
                    if (contentPhieuKhamBenh != "")
                    {
                        contentPhieuKhamBenh = contentPhieuKhamBenh + "<div class=\"pagebreak\"> </div>";
                    }
                    contentList.Add(contentPhieuKhamBenh);

                }
                else if (yeuCauKhamBenh.TenDichVu.RemoveUniKeyAndToLower().Contains("Khám Răng hàm mặt".RemoveUniKeyAndToLower()))
                {
                    var templatePhieuKhamBenhRHM = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVienRangHamMat")).FirstOrDefault();
                    phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                    var templateDichVuTMH = yeuCauKhamBenh.ThongTinKhamTheoDichVuData;
                    var thongTinBenhNhan = new ThongTinBenhNhanTaiMuiHongList();
                    var thongTinBenhTMHChiTiet = new ThongTinBenhRHMChiTiet();
                    if (!string.IsNullOrEmpty(templateDichVuTMH))
                    {
                        thongTinBenhNhan = JsonConvert.DeserializeObject<ThongTinBenhNhanTaiMuiHongList>(templateDichVuTMH);
                        foreach (var item in thongTinBenhNhan.DataKhamTheoTemplate)
                        {
                            if (item.Id == "Rang")
                            {
                                thongTinBenhTMHChiTiet.Rang = item.Value;
                            }
                            if (item.Id == "Ham")
                            {
                                thongTinBenhTMHChiTiet.Ham = item.Value;
                            }
                            if (item.Id == "Mat")
                            {
                                thongTinBenhTMHChiTiet.Mat = item.Value;
                            }
                        }
                    }
                    var daXuLis = yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(z => z.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                             && !string.IsNullOrEmpty(z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat)
                                                                             && z.DichVuKyThuatBenhVien?.LoaiPhauThuatThuThuat.Substring(0, 1).ToLower() != "p").Select(z => z.TenDichVu).Distinct().ToList();

                    var data = new ThongTinBenhRangHamMat
                    {
                        MaBN = thongTinChungBN.MaBN,
                        BarCodeImgBase64 = BarcodeHelper.GenerateBarCode(thongTinChungBN?.MaBN, 210, 56),
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        QuocTich = thongTinChungBN?.QuocTich,
                        SoNha = thongTinChungBN?.SoNha,
                        XaPhuong = thongTinChungBN?.XaPhuong,
                        Huyen = thongTinChungBN?.Huyen,
                        TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                        NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),
                        SoDienThoaiQHTN = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                        ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu ?? "&nbsp;",
                        //ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu,
                        //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                        ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                        LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                        TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                            ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                        //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                        //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                        NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                        KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                        TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                        //DaXuLi = string.Join("; ", daXuLis),
                        DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                        HuongXuLi = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                        PhongKham = phongKham,
                        Rang = thongTinBenhTMHChiTiet.Rang,
                        Ham = thongTinBenhTMHChiTiet.Ham,
                        Mat = thongTinBenhTMHChiTiet.Mat,
                        HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                        QuaTrinhBenhLy = thongTinChungBN?.BenhSu?.Replace("\n", "<br>"),
                        //ChanDoanVaoVien = yeuCauKhamBenh.Icdchinh?.Ma + "-" + yeuCauKhamBenh.Icdchinh?.TenTiengViet,
                        ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),

                        //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                        //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                        NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                        NgayGioIn = DateTime.Now.ApplyFormatFullDateTime()
                    };
                    contentPhieuKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhRHM.Body, data);
                    if (contentPhieuKhamBenh != "")
                    {
                        contentPhieuKhamBenh = contentPhieuKhamBenh + "<div class=\"pagebreak\"> </div>";
                    }
                    contentList.Add(contentPhieuKhamBenh);
                }
                else
                {
                    var templatePhieuKhamBenh = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenh2")).FirstOrDefault();
                    var header = string.Empty;
                    //if (phieuKhamBenhVo.CoHeader)
                    //{
                    //    header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                    //           "<th>PHIẾU KHÁM BỆNH</th>" +
                    //      "</p>";
                    //}
                    phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                    var dataKhamBenh = string.Empty;
                    if (!string.IsNullOrEmpty(yeuCauKhamBenh.NoiDungKhamBenh))
                    {
                        dataKhamBenh += yeuCauKhamBenh.NoiDungKhamBenh + "<br>";
                    }
                    if (!string.IsNullOrEmpty(yeuCauKhamBenh.KhamToanThan))
                    {
                        dataKhamBenh += "Khám toàn thân: " + yeuCauKhamBenh.KhamToanThan + "<br>";
                    }
                    //var dataKhamBenh = coNoiDungKhac ? (!string.IsNullOrEmpty(yeuCauKhamBenh.NoiDungKhamBenh) ? yeuCauKhamBenh.NoiDungKhamBenh + "<br>" : "")
                    //                                        : (!string.IsNullOrEmpty(yeuCauKhamBenh.KhamToanThan) ? "Khám toàn thân: " + yeuCauKhamBenh.KhamToanThan + "<br>" : "");
                    if (!string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuData) && !string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
                    {
                        //Cập nhật 22/07/2022: chỉ in những trường có giá trị
                        var thongTinKhamTheoDichVuData =
                             JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(yeuCauKhamBenh
                                 .ThongTinKhamTheoDichVuData);
                        var thongTinBenhNhanKhamKhacTemplate =
                            JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(yeuCauKhamBenh
                                .ThongTinKhamTheoDichVuTemplate);
                        foreach (var item in thongTinBenhNhanKhamKhacTemplate.ComponentDynamics)
                        {
                            if (item.Type == 4 && item.groupItems != null && item.groupItems.Count > 0)
                            {
                                var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                if (itemData != null && !string.IsNullOrEmpty(itemData.Value))
                                {
                                    dataKhamBenh += item.Label + ": " + itemData?.Value + "<br>";
                                }
                                else
                                {
                                    itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o =>
                                        item.groupItems.Any(g => g.Id == o.Id));
                                    if (itemData != null)
                                    {
                                        dataKhamBenh += item.Label + ": ";
                                        var i = 0;
                                        foreach (var itemGroup in item.groupItems)
                                        {
                                            if (itemGroup.Type == 1)
                                            {
                                                dataKhamBenh += "<br>" + itemGroup.Label + ": ";
                                            }
                                            else
                                            {
                                                var itemDataGroup = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == itemGroup.Id);
                                                if (itemDataGroup != null && !string.IsNullOrEmpty(itemDataGroup.Value))
                                                {
                                                    dataKhamBenh += " +" + itemGroup.Label + ": " + itemDataGroup.Value;
                                                }
                                            }
                                            i++;
                                            if (i == item.groupItems.Count)
                                            {
                                                dataKhamBenh += "<br>";
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                                if (itemData != null && !string.IsNullOrEmpty(itemData.Value))
                                {
                                    dataKhamBenh += item.Label + ": " + itemData?.Value + "<br>";
                                }
                            }
                        }
                    }
                    var cacBoPhanKhac = string.Empty;
                    var lstCacBoPhanKhacs = yeuCauKhamBenh.YeuCauKhamBenhKhamBoPhanKhacs;

                    foreach (var item in lstCacBoPhanKhacs)
                    {
                        cacBoPhanKhac += "-Bộ phận: " + item.Ten + ", Mô tả: " + item.NoiDUng + "; <br>";
                    }

                    var lstHinhVeTonThuong = yeuCauKhamBenh.YeuCauKhamBenhBoPhanTonThuongs.ToList();
                    var imageStr = string.Empty;
                    var lstHinhVeTonThuongCount = lstHinhVeTonThuong.Count();
                    for (var i = 0; i < lstHinhVeTonThuongCount; i++)
                    {
                        if (i > 0)
                        {
                            imageStr = imageStr
                           + "<tr>"
                                   + "<td colspan='2' style='border-top: 1px dashed gray;'>"
                                       + "<img style='height: 300;width: 350px;align: top' src='" + lstHinhVeTonThuong[i].HinhAnh + "'/>"
                                   + "</td>"

                            + "</tr>"
                             + "<tr>"
                                   + "<td colspan='2' style='font-size:15px; vertical-align: top;border-top: 1px dashed gray; text-align:justify;'><b>"
                                       + lstHinhVeTonThuong[i].MoTa
                                     + "</b></td>"
                            + "</tr>"
                            ;
                        }
                        else
                        {
                            imageStr = imageStr
                           + "<tr>"
                                   + "<td colspan='2'>"
                                       + "<img style='height: 300px;width: 350px;vertical-align: top' src='" + lstHinhVeTonThuong[i].HinhAnh + "'/>"
                                   + "</td>"
                            + "</tr>"

                            + "<tr>"
                                   + "<td colspan='2' style='font-size:15px; vertical-align: top; text-align:justify;'><b>"
                                       + lstHinhVeTonThuong[i].MoTa
                                     + "</b></td>"
                            + "</tr>"
                            ;
                        }
                    }
                    var cacBoPhanKhacs = string.Empty;
                    if (!string.IsNullOrEmpty(cacBoPhanKhac))
                    {
                        cacBoPhanKhacs = "Các bộ phận khác: <br>" + cacBoPhanKhac;
                    }
                    var khamBenh = string.Empty;

                    khamBenh += "<br>" + dataKhamBenh?.Replace("\n", "<br>") + cacBoPhanKhacs + imageStr;


                    var data = new ThongTinBenhKhac
                    {
                        Header = header,
                        MaTN = thongTinChungBN.MaTN,
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        QuocTich = thongTinChungBN?.QuocTich,
                        SoNha = thongTinChungBN?.SoNha,
                        XaPhuong = thongTinChungBN?.XaPhuong,
                        Huyen = thongTinChungBN?.Huyen,
                        TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,

                        NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),

                        //Cập nhật 22/06/2022 -> yêu cầu phiếu khám và phiếu vào viện hiển thị số điện thoại từ 2 nguồn trong YCTN: SDT và SDT người liên hệ
                        //SoDienThoai = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                        SoDienThoai = thongTinChungBN?.SoDienThoaiNguoiBenhDisplay,

                        ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>") ?? "&nbsp;",
                        //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                        ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                        LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                        //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                        //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                        NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                        KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                        TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                        TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                            ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                            + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                        BenhSu = thongTinChungBN?.BenhSu,
                        //DaXuLi = tenYCDuocPhamBV,
                        DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                        HuongXuLy = yeuCauKhamBenh.CachGiaiQuyet?.Replace("\n", "<br>"),
                        PhongKham = phongKham,
                        HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                        Mach = ketQuaSinhHieu?.NhipTim == null ? null : ketQuaSinhHieu?.NhipTim.ToString(),
                        NhietDo = ketQuaSinhHieu?.ThanNhiet == null ? null : ketQuaSinhHieu?.ThanNhiet.ToString(),
                        HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                        NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                        CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                        ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                        BMI = ketQuaSinhHieu?.Bmi == null ? null : ((double?)Math.Round((ketQuaSinhHieu.Bmi.Value), 2)).ToString(),
                        SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                        TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Tiền sử bệnh bản thân: " + tienSuBenhBanThan : "")
                                + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Tiền sử bệnh gia đình: " + tienSuBenhGiaDinh : ""),
                        XetNghiemDaLam = string.Join("; ", yeuCauKhamBenh.YeuCauDichVuKyThuats.Where(p => p.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien).Select(z => z.TenDichVu).Distinct()),
                        ChanDoan = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),
                        Ngay = thongTinChungBN.Ngay,
                        Thang = thongTinChungBN.Thang,
                        Nam = thongTinChungBN.Nam,

                        //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                        //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                        NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),

                        KhamBenh = khamBenh,
                    };
                    contentPhieuKhamBenh = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenh.Body, data);
                    if (contentPhieuKhamBenh != "")
                    {
                        contentPhieuKhamBenh = contentPhieuKhamBenh + "<div class=\"pagebreak\"> </div>";
                    }
                    contentList.Add(contentPhieuKhamBenh);
                }
            }
            if (phieuKhamBenhVo.CoKhamBenhVaoVien)
            {
                var templatePhieuKhamBenhVaoVien = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenhVaoVien") && x.Version == 2).FirstOrDefault();
                var header = string.Empty;
                phongKham = _phongBenhVienRepository.TableNoTracking.Where(z => z.Id == yeuCauKhamBenh.NoiThucHienId).Select(v => v.Ten).FirstOrDefault();
                var cacBoPhanKhac = string.Empty;
                var lstCacBoPhanKhacs = yeuCauKhamBenh.YeuCauKhamBenhKhamBoPhanKhacs;
                foreach (var item in lstCacBoPhanKhacs)
                {
                    cacBoPhanKhac += "-Bộ phận: " + item.Ten + ", Mô tả: " + item.NoiDUng + "; <br>";
                }
                var cacBoPhan = string.Empty;
                if (!string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuData) && !string.IsNullOrEmpty(yeuCauKhamBenh.ThongTinKhamTheoDichVuTemplate))
                {
                    //Cập nhật 22/07/2022: chỉ in những trường có giá trị
                    var thongTinKhamTheoDichVuData =
                         JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(yeuCauKhamBenh
                             .ThongTinKhamTheoDichVuData);
                    var thongTinBenhNhanKhamKhacTemplate =
                        JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacTemplateList>(yeuCauKhamBenh
                            .ThongTinKhamTheoDichVuTemplate);
                    foreach (var item in thongTinBenhNhanKhamKhacTemplate.ComponentDynamics)
                    {
                        if (item.Type == 4 && item.groupItems != null && item.groupItems.Count > 0)
                        {
                            var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                            if (itemData != null && !string.IsNullOrEmpty(itemData.Value))
                            {
                                cacBoPhan += item.Label + ": " + itemData?.Value + "<br>";
                            }
                            else
                            {
                                itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o =>
                                    item.groupItems.Any(g => g.Id == o.Id));
                                if (itemData != null)
                                {
                                    cacBoPhan += item.Label + ": ";
                                    var i = 0;
                                    foreach (var itemGroup in item.groupItems)
                                    {
                                        if (itemGroup.Type == 1)
                                        {
                                            cacBoPhan += "<br>" + itemGroup.Label + ": ";
                                        }
                                        else
                                        {
                                            var itemDataGroup = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == itemGroup.Id);
                                            if (itemDataGroup != null && !string.IsNullOrEmpty(itemDataGroup.Value))
                                            {
                                                cacBoPhan += " +" + itemGroup.Label + ": " + itemDataGroup.Value;
                                            }
                                        }
                                        i++;
                                        if (i == item.groupItems.Count)
                                        {
                                            cacBoPhan += "<br>";
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            var itemData = thongTinKhamTheoDichVuData?.DataKhamTheoTemplate.FirstOrDefault(o => o.Id == item.Id);
                            if (itemData != null && !string.IsNullOrEmpty(itemData.Value))
                            {
                                cacBoPhan += item.Label + ": " + itemData?.Value + "<br>";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(cacBoPhanKhac))
                {
                    cacBoPhan += "<br>" + cacBoPhanKhac;
                }

                var data = new ThongTinBenhKhac
                {
                    Header = header,
                    MaTN = thongTinChungBN.MaTN,
                    MaBN = thongTinChungBN.MaBN,
                    HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                    SinhNgay = thongTinChungBN?.SinhNgay,
                    GioiTinh = thongTinChungBN?.GioiTinh,
                    NgheNghiep = thongTinChungBN?.NgheNghiep,
                    DanToc = thongTinChungBN?.DanToc,
                    QuocTich = thongTinChungBN?.QuocTich,
                    SoNha = thongTinChungBN?.SoNha,
                    XaPhuong = thongTinChungBN?.XaPhuong,
                    Huyen = thongTinChungBN?.Huyen,
                    TinhThanhPho = thongTinChungBN?.TinhThanhPho,
                    NoiLamViec = thongTinChungBN?.NoiLamViec,
                    DoiTuong = thongTinChungBN?.DoiTuong,
                    BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                    BHYTMaSoThe = thongTinChungBN?.BHYTMaSoThe2,
                    NguoiLienHeQuanHeThanNhan = thongTinChungBN?.NguoiLienHeQuanHeThanNhan + (!string.IsNullOrEmpty(thongTinChungBN.NguoiLienHeDiaChiDayDu) ? " (" + thongTinChungBN.NguoiLienHeDiaChiDayDu + ")" : ""),

                    //Cập nhật 22/06/2022 -> yêu cầu phiếu khám và phiếu vào viện hiển thị số điện thoại từ 2 nguồn trong YCTN: SDT và SDT người liên hệ
                    //SoDienThoai = thongTinChungBN?.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinChungBN?.SoDienThoaiNguoiBenhDisplay,

                    ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu?.Replace("\n", "<br>") ?? "&nbsp;",
                    //ChanDoanNoiGioiThieu = thongTinChungBN.ChanDoanNoiGioiThieu,
                    //ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                    ThoiDiemTiepNhan = yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString(),
                    //LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan,
                    LyDoVaoVien = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                    LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan?.Replace("\n", "<br>") ?? "&nbsp;",
                    //LyDoVaoKham = yeuCauKhamBenh.TrieuChungTiepNhan,
                    //ToanThan = yeuCauKhamBenh?.KhamToanThan,
                    //ToanThan = coNoiDungKhac ? yeuCauKhamBenh.NoiDungKhamBenh?.Replace("\n", "<br>") : yeuCauKhamBenh.KhamToanThan?.Replace("\n", "<br>"),
                    NoiDungKhamBenh = yeuCauKhamBenh.NoiDungKhamBenh,
                    KhamToanThan = yeuCauKhamBenh.KhamToanThan,
                    TomTatCLS = yeuCauKhamBenh?.TomTatKetQuaCLS?.Replace("\n", "<br>"),
                    QuaTrinhBenhLy = thongTinChungBN?.BenhSu,
                    //DaXuLi = thuocDaXuLy,
                    DaXuLi = string.Join("; ", yeuCauDuocPhamBenhViens) + string.Join("; ", yeuCauVatTuBenhViens) + string.Join("; ", yeuCauDichVuKyThuats),
                    Mach = ketQuaSinhHieu?.NhipTim == null ? null : ketQuaSinhHieu?.NhipTim.ToString(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet == null ? null : ketQuaSinhHieu?.ThanNhiet.ToString(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    BMI = ketQuaSinhHieu?.Bmi == null ? null : ((double?)Math.Round((ketQuaSinhHieu.Bmi.Value), 2)).ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "+ Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "+ Gia đình: " + tienSuBenhGiaDinh : ""),
                    ChanDoan = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),
                
                    //BVHD-3845 NgayThangNam khách muốn lấy => yeuCauKhamBenh.ThoiDiemChiDinh.ConvertDatetimeToString()
                    //Ngay = thongTinChungBN.Ngay,
                    //Thang = thongTinChungBN.Thang,
                    //Nam = thongTinChungBN.Nam,
                    //NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
                    NgayThangNam = yeuCauKhamBenh.ThoiDiemChiDinh.ApplyFormatNgayThangNam(),
                    Ngay = yeuCauKhamBenh.ThoiDiemChiDinh.Day.ToString(),
                    Thang = yeuCauKhamBenh.ThoiDiemChiDinh.Month.ToString(),
                    Nam = yeuCauKhamBenh.ThoiDiemChiDinh.Year.ToString(),
                    //BVHD-3845 The End 

                    CacBoPhan = cacBoPhan,
                    ChanDoanVaoVien = (yeuCauKhamBenh.Icdchinh != null ? yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh.TenTiengViet : "") + (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh) ? " (" + yeuCauKhamBenh.GhiChuICDChinh?.Replace("\n", "<br>") + ")" : "") + (icdPhus.Any() ? "; " + string.Join("; ", icdPhus) : ""),
                    HuongDieuTri = "Nhập viện - " + yeuCauKhamBenh.KhoaPhongNhapVien?.Ten,
                    HoTenBacSi = yeuCauKhamBenh.BacSiKetLuan != null ?
                                (yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi != null ? yeuCauKhamBenh.BacSiKetLuan.HocHamHocVi.Ma + " " : "") + yeuCauKhamBenh.BacSiKetLuan?.User.HoTen : "",
                    Khoa = phongKham
                };
                contentPhieuKhamBenhVaoVien = TemplateHelpper.FormatTemplateWithContentTemplate(templatePhieuKhamBenhVaoVien.Body, data);
                if (contentPhieuKhamBenhVaoVien != "")
                {
                    contentPhieuKhamBenhVaoVien = contentPhieuKhamBenhVaoVien + "<div class=\"pagebreak\"> </div>";
                }
                contentList.Add(contentPhieuKhamBenhVaoVien);
            }

            //if (contentPhieuKhamBenh != "" && contentPhieuKhamBenhVaoVien != "")
            //{
            //    contentPhieuKhamBenh = contentPhieuKhamBenh + "<div class=\"pagebreak\"> </div>";
            //}
            //content = contentPhieuKhamBenh + contentPhieuKhamBenhVaoVien;
            //contentList.Add(content);
            return contentList;

        }
        public string InGiayChuyenVien(long yeuCauKhamBenhId)
        {
            var content = string.Empty;
            var yeuCauKhamBenh = BaseRepository.GetById(yeuCauKhamBenhId,
                                     x => x.Include(yckb => yckb.NhanVienHoTongChuyenVien).ThenInclude(nv => nv.User)
                                           .Include(yckb => yckb.NhanVienHoTongChuyenVien).ThenInclude(nv => nv.ChucDanh).ThenInclude(cd => cd.NhomChucDanh)
                                           .Include(yckb => yckb.NhanVienHoTongChuyenVien).ThenInclude(nv => nv.VanBangChuyenMon)
                                           .Include(yckb => yckb.BacSiThucHien).ThenInclude(bs => bs.User)
                                           .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(bs => bs.BenhNhan).ThenInclude(bs => bs.BenhNhanTienSuBenhs)
                                           .Include(yckb => yckb.YeuCauVatTuBenhViens)
                                           .Include(yckb => yckb.YeuCauDuocPhamBenhViens)
                                           .Include(yckb => yckb.YeuCauDichVuKyThuats)
                                           .Include(yckb => yckb.BenhVienChuyenVien)
                                           .Include(yckb => yckb.Icdchinh));

            //Cập nhật 12/02/2022
            var sttChuyenVien = yeuCauKhamBenh.STTChuyenVien;
            var sttChuyenVienDisplay = string.Empty;
            if (sttChuyenVien == null)
            {
                sttChuyenVienDisplay = ResourceHelper.CreateSTTChuyenTuyenTheoNguoiBenh();
                yeuCauKhamBenh.STTChuyenVien = long.Parse(sttChuyenVienDisplay);
                BaseRepository.Context.SaveChanges();
            }
            else
            {
                sttChuyenVienDisplay = sttChuyenVien.Value.ToString("00");
            }

            var thongTinChungBN = ThongTinBenhNhanHienTai(yeuCauKhamBenhId);
            var templateGiayChuyenVien = yeuCauKhamBenh.YeuCauTiepNhan.CoBHYT == true ?
                _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayChuyenVienBHYT")).FirstOrDefault()
                : _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayChuyenVienKetLuan")).FirstOrDefault();
            var daKhamBenhDieuTri = string.Empty;

            var settings = _cauHinhService.LoadSetting<BaoHiemYTe>();
            //BvBacHa
            var benhVienLucTiepNhan = _benhVienRepository.TableNoTracking
                          .Where(p => p.Ma == settings.BenhVienTiepNhan).FirstOrDefault();

            if (yeuCauKhamBenh.YeuCauTiepNhan.CoBHYT == true && yeuCauKhamBenh.YeuCauTiepNhan.DuocChuyenVien == true)
            {
                daKhamBenhDieuTri = daKhamBenhDieuTri
                      + "<tr>"
                              + "<td colspan='3' style='font-size: 15px;' width='23%'>"
                                 + " + Tại: <b>" + yeuCauKhamBenh.YeuCauTiepNhan.NoiChuyen?.Ten
                                 + " </b>(<b>" + yeuCauKhamBenh.YeuCauTiepNhan.NoiChuyen?.TuyenChuyenMonKyThuat.GetDescription()
                                 + "</b>) Từ ngày ........./..../......."
                                 + " đến ngày<b> " + yeuCauKhamBenh.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate()
                              + "</b></td>"
                       + "</tr>";
            }
            if (yeuCauKhamBenh.CoChuyenVien == true)
            {
                daKhamBenhDieuTri = daKhamBenhDieuTri
                 + "<tr>"
                         + "<td colspan='3' style='font-size: 15px;' width='23%'>"
                            + " + Tại: <b>" + benhVienLucTiepNhan?.Ten
                            + " </b>(<b>" + benhVienLucTiepNhan?.TuyenChuyenMonKyThuat.GetDescription()
                            + "</b>) Từ ngày <b>" + yeuCauKhamBenh.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate()
                            + " </b>đến ngày<b> " + yeuCauKhamBenh.ThoiDiemChuyenVien?.ApplyFormatDate()
                         + "</b></td>"
                  + "</tr>";
            }
            var dauHieu = yeuCauKhamBenh.YeuCauTiepNhan.BenhNhan.BenhNhanTienSuBenhs.Select(s => new { DauHieu = s.LoaiTienSuBenh.GetDescription() + " - " + s.TenBenh }).Select(c => c.DauHieu).Distinct().ToList();
            if (templateGiayChuyenVien != null)
            {
                if (yeuCauKhamBenh.YeuCauTiepNhan.CoBHYT == true)
                {

                    var data = new ThongTinGiayChuyenVien
                    {
                        Nam = DateTime.Now.Year.ToString(),
                        STT = sttChuyenVienDisplay, //ResourceHelper.CreateSTTChuyenTuyenTheoNguoiBenh(yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId.Value), /*ResourceHelper.CreateSTTChuyenTuyen(),*/
                        BenhVienHienTai = "<b>BỆNH VIỆN ĐA KHOA QUỐC TẾ BẮC HÀ</b>",
                        MaTN = thongTinChungBN?.MaTN,
                        BenhVienChuyenDen = yeuCauKhamBenh.BenhVienChuyenVien?.Ten,
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        Tuoi = thongTinChungBN.Tuoi,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe2,
                        DauHieuLamSang = thongTinChungBN.BenhSu, // string.Join(", ", dauHieu),
                        KetQuaXetNghiemCLS = yeuCauKhamBenh.KetQuaXetNghiemCLS,
                        KetQuaCLS = yeuCauKhamBenh.KetQuaXetNghiemCLS,
                        ChanDoan = yeuCauKhamBenh.GhiChuICDChinh,
                        PhuongPhap = yeuCauKhamBenh.PhuongPhapTrongDieuTri,
                        DiaChi = thongTinChungBN.DiaChiDayDu,
                        QuocTich = thongTinChungBN.QuocTich,
                        DaKhamBenhDieuTri = daKhamBenhDieuTri,
                        TinhTrang = yeuCauKhamBenh.TinhTrangBenhNhanChuyenVien?.Replace("\n", "<br>"),
                        HuongDieuTri = yeuCauKhamBenh.LyDoChuyenVien,
                        ThoiGianChuyenTuyen = yeuCauKhamBenh.ThoiDiemChuyenVien?.ConvertDatetimeToString(),
                        PhuongTienVanChuyen = yeuCauKhamBenh.PhuongTienChuyenVien,
                        NguoiHoTong = yeuCauKhamBenh.NhanVienHoTongChuyenVien?.User.HoTen +
                                (yeuCauKhamBenh.NhanVienHoTongChuyenVien?.ChucDanh != null ? " - " + yeuCauKhamBenh.NhanVienHoTongChuyenVien?.ChucDanh?.NhomChucDanh.Ten : "")
                                                    + (yeuCauKhamBenh.NhanVienHoTongChuyenVien?.VanBangChuyenMon != null ? " - " + yeuCauKhamBenh.NhanVienHoTongChuyenVien?.VanBangChuyenMon?.Ten : ""),
                        NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam()
                    };
                    if (yeuCauKhamBenh.LoaiLyDoChuyenVien == LyDoChuyenTuyen.DuDieuKien)
                    {
                        templateGiayChuyenVien.Body =
                            templateGiayChuyenVien.Body.Replace("classBorderDuDieuKienChuyenTuyen", "border-label");
                    }
                    else if (yeuCauKhamBenh.LoaiLyDoChuyenVien == LyDoChuyenTuyen.TheoYeuCauBenhNhan)
                    {
                        templateGiayChuyenVien.Body =
                            templateGiayChuyenVien.Body.Replace("classBorderTheoYeuCauNguoiBenh", "border-label");
                    }
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayChuyenVien.Body, data);
                }
                else
                {
                    var data = new ThongTinGiayChuyenVien
                    {
                        STT = sttChuyenVienDisplay, // ResourceHelper.CreateSTTChuyenTuyenTheoNguoiBenh(yeuCauKhamBenh.YeuCauTiepNhan.BenhNhanId.Value), /*ResourceHelper.CreateSTTChuyenTuyen(),*/
                        MaTN = thongTinChungBN?.MaTN,
                        BenhVienChuyenDen = yeuCauKhamBenh.BenhVienChuyenVien?.Ten,
                        HoTenBenhNhan = thongTinChungBN?.HoTenBenhNhan,
                        SinhNgay = thongTinChungBN?.SinhNgay,
                        Tuoi = thongTinChungBN.Tuoi,
                        GioiTinh = thongTinChungBN?.GioiTinh,
                        NgheNghiep = thongTinChungBN?.NgheNghiep,
                        DanToc = thongTinChungBN?.DanToc,
                        NoiLamViec = thongTinChungBN?.NoiLamViec,
                        DoiTuong = thongTinChungBN?.DoiTuong,
                        BHYTNgayHetHan = thongTinChungBN?.BHYTNgayHetHan,
                        BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe2,
                        //DauHieuLamSang = yeuCauKhamBenh.YeuCauTiepNhan.TrieuChungTiepNhan,
                        DauHieuLamSang = thongTinChungBN.BenhSu, // string.Join(", ", dauHieu),
                        KetQuaCLS = yeuCauKhamBenh.KetQuaXetNghiemCLS,
                        ChanDoan = yeuCauKhamBenh.GhiChuICDChinh,
                        PhuongPhap = yeuCauKhamBenh.PhuongPhapTrongDieuTri/*phuongPhap*/,
                        DiaChi = thongTinChungBN.DiaChiDayDu,
                        QuocTich = thongTinChungBN.QuocTich,
                        DaKhamBenhDieuTri = daKhamBenhDieuTri,
                        TinhTrang = yeuCauKhamBenh.TinhTrangBenhNhanChuyenVien?.Replace("\n", "<br>"),
                        TenLoaiLyDoChuyenVien = yeuCauKhamBenh.LoaiLyDoChuyenVien.GetDescription(),
                        HuongDieuTri = yeuCauKhamBenh.LyDoChuyenVien,
                        ThoiGianChuyenTuyen = yeuCauKhamBenh.ThoiDiemChuyenVien?.ConvertDatetimeToString(),
                        PhuongTienVanChuyen = yeuCauKhamBenh.PhuongTienChuyenVien,
                        NguoiHoTong = yeuCauKhamBenh.NhanVienHoTongChuyenVien?.User.HoTen +
                                (yeuCauKhamBenh.NhanVienHoTongChuyenVien?.ChucDanh != null ? " - " + yeuCauKhamBenh.NhanVienHoTongChuyenVien?.ChucDanh?.NhomChucDanh.Ten : "")
                                                    + (yeuCauKhamBenh.NhanVienHoTongChuyenVien?.VanBangChuyenMon != null ? " - " + yeuCauKhamBenh.NhanVienHoTongChuyenVien?.VanBangChuyenMon?.Ten : ""),
                        ThoiDiemTiepNhan = thongTinChungBN?.ThoiDiemTiepNhan,
                        Ngay = thongTinChungBN?.Ngay,
                        Thang = thongTinChungBN?.Thang,
                        Nam = thongTinChungBN?.Nam,
                        BacSiThucHien = thongTinChungBN?.HoTenBacSi,
                    };
                    content = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayChuyenVien.Body, data);
                }
            }
            return content;
        }

        public ThongTinNgayNghiHuongBHYTTiepNhan GetThongTinNgayNghiHuongBHYT(long yeuCauKhamBenhId)
        {
            var thongTinNgayNghiHuongBHYT = BaseRepository.TableNoTracking.Where(x => x.Id == yeuCauKhamBenhId).FirstOrDefault();
            ThongTinNgayNghiHuongBHYTTiepNhan thongTin = new ThongTinNgayNghiHuongBHYTTiepNhan
            {
                ICDChinhNghiHuongBHYT = thongTinNgayNghiHuongBHYT.ICDChinhNghiHuongBHYT,
                TenICDChinhNghiHuongBHYT = thongTinNgayNghiHuongBHYT.TenICDChinhNghiHuongBHYT,
                PhuongPhapDieuTriNghiHuongBHYT = thongTinNgayNghiHuongBHYT.PhuongPhapDieuTriNghiHuongBHYT
            };

            return thongTin;
        }

        public void KTNgayGiayNghiHuongBHYT(ThongTinNgayNghiHuongBHYT thongTinNgayNghi)
        {
            var cauHinhNghiHuongBHYT = _cauHinhRepository.TableNoTracking
                .Where(x => x.Name.Contains("CauHinhKhamBenh.NghiHuongBHYT")).FirstOrDefault();
            var dataCauHinhNghiHuongBHYTs = JsonConvert.DeserializeObject<List<NghiHuongBHYTJson>>(cauHinhNghiHuongBHYT.Value);
            var soNgay = ((thongTinNgayNghi.DenNgay ?? DateTime.Now).Date - thongTinNgayNghi.ThoiDiemTiepNhan?.Date)?.Days;
            if (dataCauHinhNghiHuongBHYTs.Any(c => c.ICDId == thongTinNgayNghi.ICDChinhNghiHuongBHYT))
            {
                if (soNgay >= 180)
                {
                    throw new Exception("Số ngày nghỉ hưởng BHXH không được vượt quá 180 ngày");
                }
            }
            else
            {

                if (soNgay >= 30)
                {
                    throw new Exception("Số ngày nghỉ hưởng BHXH không được vượt quá 30 ngày");
                }
            }
        }

        public string XemGiayNghiHuongBHYTLien1(ThongTinNgayNghiHuongBHYT thongTinNgayNghi)
        {
            var thongTinChungBN = ThongTinBenhNhanHienTai(thongTinNgayNghi.YeuCauKhamBenhId);
            var contentLienSo1 = string.Empty;
            var templateGiayNghiHuongBHYTLienSo1 = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayNghiHuongBHYT")).First();

            var yeuCauKhamBenh = BaseRepository.GetById(thongTinNgayNghi.YeuCauKhamBenhId,
                                   x => x.Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NgheNghiep)
                                         .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NguoiLienHeQuanHeNhanThan));

            var thoiDiemTiepNhanFormat = string.Empty;
            var denNgayFormat = string.Empty;
            var soNgayNghi = string.Empty;

            if (thongTinNgayNghi.ThoiDiemTiepNhan != null && thongTinNgayNghi.DenNgay != null)
            {
                thoiDiemTiepNhanFormat = thongTinNgayNghi.ThoiDiemTiepNhan?.ApplyFormatDate();
                denNgayFormat = thongTinNgayNghi.DenNgay.Value.ApplyFormatDate();
                int result = DateTime.Compare(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null), DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null));
                if (result == 0)
                {
                    soNgayNghi = "1";
                }
                else if (result == -1)
                {
                    TimeSpan value = DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null).Subtract(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null));
                    var getday = value.Days + 1;
                    soNgayNghi = getday.ToString();
                }
                else
                {
                    denNgayFormat = thoiDiemTiepNhanFormat;
                    soNgayNghi = "1";
                }

                yeuCauKhamBenh.NghiHuongBHXHTuNgay = thongTinNgayNghi.ThoiDiemTiepNhan;
                yeuCauKhamBenh.NghiHuongBHXHDenNgay = thongTinNgayNghi.DenNgay;
                yeuCauKhamBenh.NghiHuongBHXHNgayIn = DateTime.Now;
                yeuCauKhamBenh.NghiHuongBHXHNguoiInId = _userAgentHelper.GetCurrentUserId();

                yeuCauKhamBenh.ICDChinhNghiHuongBHYT = thongTinNgayNghi.ICDChinhNghiHuongBHYT;
                yeuCauKhamBenh.TenICDChinhNghiHuongBHYT = thongTinNgayNghi.TenICDChinhNghiHuongBHYT;
                yeuCauKhamBenh.PhuongPhapDieuTriNghiHuongBHYT = thongTinNgayNghi.PhuongPhapDieuTriNghiHuongBHYT;

                BaseRepository.Update(yeuCauKhamBenh);
            }

            //todo: cần bổ sung phương pháp điều trị (Thạch)
            var hoTenCha = string.Empty;
            var hoTenMe = string.Empty;
            var quanHeThanNhanCha = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.TenVietTat;
            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("ChaDe"))
            {
                hoTenCha = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen;
            }

            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("MeDe"))
            {
                hoTenMe = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen;
            }

            var chanDoan = string.Empty;
            if (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh))
            {
                chanDoan = "<p style='margin: 0;'> CĐ: " + yeuCauKhamBenh.GhiChuICDChinh + "</p>" + "<p style='margin: 0;'>" + "" + "</p>";
            }
            //var header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
            //                 "<th>Liên số 1</th>" +
            //            "</p>";
            var dataLienSo1 = new ThongTinNgayNghiHuongBHYTDetail
            {
                //Header = header,
                //MaTN = thongTinChungBN.MaTN,
                //LienSo = "1",
                HoTenBenhNhan = thongTinChungBN.HoTenBenhNhan,
                SinhNgay = thongTinChungBN.SinhNgay,
                BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe2,
                GioiTinh = thongTinChungBN.GioiTinh,
                NoiLamViec = thongTinChungBN.NoiLamViec,
                ChanDoanPhuongPhap = chanDoan,
                SoNgayNghi = soNgayNghi + " NGÀY",
                ThoiDiemTiepNhan = thoiDiemTiepNhanFormat,
                DenNgay = denNgayFormat,
                HoTenCha = hoTenCha,
                //todo: confirm HoTenMe
                HoTenMe = hoTenMe,
                //HoTenBacSi = "Bs." + thongTinChungBN.HoTenBacSi,
                //Ngay = thongTinChungBN.Ngay,
                //Thang = thongTinChungBN.Thang,
                //Nam = thongTinChungBN.Nam,
                NgayThangNam = DateTime.Now.ApplyFormatNgayThangNam(),
            };
            contentLienSo1 = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayNghiHuongBHYTLienSo1.Body, dataLienSo1);
            //if (contentLienSo1 != "")
            //{
            //    contentLienSo1 = contentLienSo1 + "<div class=\"pagebreak\"> </div>";
            //}
            return contentLienSo1;
        }

        public string XemGiayNghiHuongBHYTLien2(ThongTinNgayNghiHuongBHYT thongTinNgayNghi)
        {
            var thongTinChungBN = ThongTinBenhNhanHienTai(thongTinNgayNghi.YeuCauKhamBenhId);
            var contentLienSo2 = string.Empty;
            var templateGiayNghiHuongBHYTLienSo2 = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("GiayNghiHuongBHYT")).First();

            var yeuCauKhamBenh = BaseRepository.GetById(thongTinNgayNghi.YeuCauKhamBenhId,
                                   x => x.Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NgheNghiep)
                                         .Include(yckb => yckb.YeuCauTiepNhan).ThenInclude(yctn => yctn.NguoiLienHeQuanHeNhanThan));

            var thoiDiemTiepNhanFormat = string.Empty;
            var denNgayFormat = string.Empty;
            var soNgayNghi = string.Empty;

            if (thongTinNgayNghi.ThoiDiemTiepNhan != null && thongTinNgayNghi.DenNgay != null)
            {
                thoiDiemTiepNhanFormat = thongTinNgayNghi.ThoiDiemTiepNhan?.ApplyFormatDate();
                denNgayFormat = thongTinNgayNghi.DenNgay.Value.ApplyFormatDate();
                int result = DateTime.Compare(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null), DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null));
                if (result == 0)
                {
                    soNgayNghi = "1";
                }
                else if (result == -1)
                {
                    TimeSpan value = DateTime.ParseExact(denNgayFormat, "dd/MM/yyyy", null).Subtract(DateTime.ParseExact(thoiDiemTiepNhanFormat, "dd/MM/yyyy", null));
                    var getday = value.Days + 1;
                    soNgayNghi = getday.ToString();
                }
                else
                {
                    denNgayFormat = thoiDiemTiepNhanFormat;
                    soNgayNghi = "1";
                }
            }
            //todo: cần bổ sung phương pháp điều trị (Thạch)
            var hoTenCha = string.Empty;
            var hoTenMe = string.Empty;
            var quanHeThanNhanCha = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThan?.TenVietTat;
            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("ChaDe"))
            {
                hoTenCha = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen;
            }

            if (!string.IsNullOrEmpty(quanHeThanNhanCha) && quanHeThanNhanCha.Contains("MeDe"))
            {
                hoTenMe = yeuCauKhamBenh.YeuCauTiepNhan.NguoiLienHeHoTen;
            }
            var chanDoan = string.Empty;
            if (!string.IsNullOrEmpty(yeuCauKhamBenh.GhiChuICDChinh))
            {
                chanDoan = "<p style='margin: 0;'> CĐ: " + yeuCauKhamBenh.GhiChuICDChinh + "</p>" + "<p style='margin: 0;'>" + "" + "</p>";
            }
            var header = "<p  style='background: #005dab;color:#fff; height:40px; font-size:30px;text-align:center'>" +
                      "<th>Liên số 2</th>" +
                 "</p>";
            var dataLienSo2 = new ThongTinNgayNghiHuongBHYTDetail
            {
                Header = header,
                MaTN = thongTinChungBN.MaTN,
                LienSo = "2",
                HoTenBenhNhan = thongTinChungBN.HoTenBenhNhan,
                SinhNgay = thongTinChungBN.SinhNgay,
                BHYTMaSoThe = thongTinChungBN.BHYTMaSoThe,
                GioiTinh = thongTinChungBN.GioiTinh,
                NoiLamViec = thongTinChungBN.NoiLamViec,
                ChanDoanPhuongPhap = chanDoan,
                SoNgayNghi = soNgayNghi,
                ThoiDiemTiepNhan = thoiDiemTiepNhanFormat,
                DenNgay = denNgayFormat,
                HoTenCha = hoTenCha,
                //todo: confirm HoTenMe
                HoTenMe = hoTenMe,
                HoTenBacSi = "Bs." + thongTinChungBN.HoTenBacSi,
                Ngay = thongTinChungBN.Ngay,
                Thang = thongTinChungBN.Thang,
                Nam = thongTinChungBN.Nam,
            };
            contentLienSo2 = TemplateHelpper.FormatTemplateWithContentTemplate(templateGiayNghiHuongBHYTLienSo2.Body, dataLienSo2);
            return contentLienSo2;
        }
        #endregion

        #region KiemTraNgayNghiHuongBHXH
        public async Task<bool> KiemTraNgayTiepNhan(DateTime? tuNgay, DateTime? denNgay, long yeuCauKhamBenhId)
        {
            var thoiDiemTiepNhan = await BaseRepository.TableNoTracking
                                    .Where(p => p.Id == yeuCauKhamBenhId)
                                    .Select(p => p.YeuCauTiepNhan.ThoiDiemTiepNhan)
                                    .FirstOrDefaultAsync();
            var myTime = DateTime.Today;
            if (tuNgay != null && denNgay != null)
            {
                if (thoiDiemTiepNhan.Date > tuNgay.Value.Date && tuNgay.Value.Date <= myTime)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> KiemTraDenNgay(DateTime? tuNgay, DateTime? denNgay, long yeuCauKhamBenhId)
        {
            var thoiDiemTiepNhan = await BaseRepository.TableNoTracking
                                    .Where(p => p.Id == yeuCauKhamBenhId)
                                    .Select(p => p.YeuCauTiepNhan.ThoiDiemTiepNhan)
                                    .FirstOrDefaultAsync();
            var myTime = DateTime.Today;
            if (tuNgay != null && denNgay != null)
            {
                if (tuNgay.Value.Date > denNgay.Value.Date)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Xử lý

        public async Task XuLyCapNhatBenhNhanVangAsync(long hangDoiId, long phongBenhVienId)
        {
            var hangDoiBenhNhanVang = await _phongBenhVienHangDoiRepository.Table
                .Where(x => x.PhongBenhVienId == phongBenhVienId
                            && x.Id == hangDoiId
                            && x.YeuCauKhamBenh != null
                            //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                .FirstOrDefaultAsync();
            if (hangDoiBenhNhanVang == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            hangDoiBenhNhanVang.TrangThai = EnumTrangThaiHangDoi.ChoKham;
            await _phongBenhVienHangDoiRepository.Context.SaveChangesAsync();


            //var lstHangDoiTheoPhongKham = await _phongBenhVienHangDoiRepository.Table
            //    .Include(x => x.YeuCauTiepNhan)
            //    .Include(x => x.YeuCauKhamBenh)
            //    .Where(x => x.PhongBenhVienId == phongBenhVienId
            //                && x.YeuCauKhamBenh != null
            //                //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
            //                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
            //                && x.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //    .OrderBy(x => x.SoThuTu).ToListAsync();

            //var hangDoiHienTai = lstHangDoiTheoPhongKham.FirstOrDefault(x => x.Id == hangDoiId);
            //if (hangDoiHienTai == null)
            //{
            //    //throw new ArgumentNullException(nameof(hangDoiHienTai));
            //    var currentUserLanguge = _userAgentHelper.GetUserLanguage();
            //    var mess = await _localeStringResourceRepository.TableNoTracking
            //        .Where(x => x.ResourceName == "ApiError.ConcurrencyError" && x.Language == (int)currentUserLanguge)
            //        .Select(x => x.ResourceValue).FirstOrDefaultAsync();
            //    throw new Exception(mess ?? "ApiError.ConcurrencyError");
            //}
            //// get list trạng thái yêu cầu khám bệnh theo hàng chờ của hàng đợi hiện tại
            //var lstTrangThaiYeuCauKhamTheoHangDoiDangKham = new List<Enums.EnumTrangThaiYeuCauKhamBenh>();
            //if (hangDoiHienTai.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham &&
            //    hangDoiHienTai.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
            //{
            //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(hangDoiHienTai.YeuCauKhamBenh.TrangThai);
            //}
            //else
            //{
            //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham);
            //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(Enums.EnumTrangThaiYeuCauKhamBenh.DangKham);
            //}

            //var lstHangDoiTheoLoaiHangDoiHienTai = lstHangDoiTheoPhongKham
            //    .Where(x => lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Any(item => item == x.YeuCauKhamBenh.TrangThai)).ToList();

            //// kiểm tra vị trí hiện tại của hàng đợi đang khám (a), nếu (a) nằm trong top 3 thì cần đẩy (a) xuống top 4
            //bool canSapXep = false;

            //if (lstHangDoiTheoLoaiHangDoiHienTai.Any())
            //{

            //    var viTriHangDoiHienTai = lstHangDoiTheoLoaiHangDoiHienTai.FindIndex(x => x.Id == hangDoiId);
            //    var lenHangDoiHienTai = lstHangDoiTheoLoaiHangDoiHienTai.Count();

            //    if (viTriHangDoiHienTai <= 2 && (viTriHangDoiHienTai != lenHangDoiHienTai - 1)) // vị trí trong top 3 và trong ds hàng chờ có hơn 3 item
            //    {
            //        canSapXep = true;
            //    }

            //    if (!canSapXep)
            //    {
            //        foreach (var item in lstHangDoiTheoLoaiHangDoiHienTai)
            //        {
            //            if (item.Id == hangDoiId)
            //            {
            //                item.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // xử lý tăng số thứ tự của các item hàng đợi từ vị trí thứ 4 trở về sau lên 1, sau đó chèn hàng đợi hiện tại vào vị trí thứ 4 (theo số thứ tự)
            //        var soThuTuHangDoiHienTaiMoi = 0;
            //        var flagTangSoThuTu = false;
            //        for (int i = lenHangDoiHienTai - 1; i >= 0; i--)
            //        {
            //            // gán giá trị mặc định cho lần đầu tiên lặp
            //            soThuTuHangDoiHienTaiMoi = soThuTuHangDoiHienTaiMoi == 0 ? lstHangDoiTheoLoaiHangDoiHienTai[i].SoThuTu : soThuTuHangDoiHienTaiMoi;

            //            // kiểm tra hàng đợi hiện tại đang khám
            //            if (lstHangDoiTheoLoaiHangDoiHienTai[i].Id == hangDoiId)
            //            {
            //                lstHangDoiTheoLoaiHangDoiHienTai[i].TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
            //                lstHangDoiTheoLoaiHangDoiHienTai[i].SoThuTu = flagTangSoThuTu ? soThuTuHangDoiHienTaiMoi : soThuTuHangDoiHienTaiMoi + 1;
            //                break;
            //            }

            //            if (lenHangDoiHienTai >= 5 && i >= 4)
            //            {
            //                if (i == 4)
            //                {
            //                    // gán sô thứ tự của hàng đợi vị trí thứ 5 hiện tại trong hàng đợi
            //                    // chuyển hàng đợi đang khám vào vị trí này
            //                    soThuTuHangDoiHienTaiMoi = lstHangDoiTheoLoaiHangDoiHienTai[i].SoThuTu;
            //                }
            //                lstHangDoiTheoLoaiHangDoiHienTai[i].SoThuTu += 1;
            //                flagTangSoThuTu = true;
            //            }
            //        }

            //    }

            //    await _phongBenhVienHangDoiRepository.UpdateAsync(lstHangDoiTheoLoaiHangDoiHienTai);
            //}

        }

        public async Task XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhan(long hangDoiHienTaiId, bool hoanThanhKham = false)
        {
            //Cập nhật 30/11/2022: bỏ bớt include dư thừa
            var hangDoiDangKham = new PhongBenhVienHangDoi();
            //var hangDoiDangKham =
            //     _phongBenhVienHangDoiRepository.Table
            //        .Include(y => y.YeuCauTiepNhan)
            //        //.Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauKhamBenhLichSuTrangThais)
            //        //.Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauDichVuGiuongBenhViens).ThenInclude(t => t.HoatDongGiuongBenhs)
            //        //.Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(v => v.BenhNhan)
            //        //.Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(v => v.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            //        .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(v => v.YeuCauKhamBenhs).ThenInclude(v => v.YeuCauNhapViens).ThenInclude(v => v.YeuCauTiepNhans)
            //        .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauNhapViens).ThenInclude(z => z.YeuCauNhapVienChanDoanKemTheos)
            //        .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauNhapViens).ThenInclude(z => z.YeuCauTiepNhans)
            //        //.Include(y => y.YeuCauKhamBenh)
            //        .First(x => x.Id == hangDoiHienTaiId);
            if (hoanThanhKham)
            {
                hangDoiDangKham =
                 _phongBenhVienHangDoiRepository.Table
                    .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauTiepNhan).ThenInclude(v => v.YeuCauKhamBenhs).ThenInclude(v => v.YeuCauNhapViens).ThenInclude(v => v.YeuCauTiepNhans)
                    .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauNhapViens).ThenInclude(z => z.YeuCauNhapVienChanDoanKemTheos)
                    .Include(y => y.YeuCauKhamBenh).ThenInclude(z => z.YeuCauNhapViens).ThenInclude(z => z.YeuCauTiepNhans)
                    .First(x => x.Id == hangDoiHienTaiId);
            }
            else
            {
                hangDoiDangKham =
                 _phongBenhVienHangDoiRepository.Table
                    .Include(y => y.YeuCauKhamBenh)
                    .First(x => x.Id == hangDoiHienTaiId);
            }

            if (hoanThanhKham) // trường hợp nhấn nút hoàn thành khám
            {
                // xử lý kiểm tra chỉ định dv khám tham vấn hoàn thành thì mới cho hoàn thành
                var dvKhamBenhThamVan = BaseRepository.TableNoTracking.Any(x =>
                    x.YeuCauKhamBenhTruocId == hangDoiDangKham.YeuCauKhamBenhId
                    && x.LaThamVan == true
                    && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                    && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham);
                if (dvKhamBenhThamVan)
                {
                    //var currentUserLanguge = _userAgentHelper.GetUserLanguage();// ?? Enums.LanguageType.VietNam;
                    //var mess = await _localeStringResourceRepository.TableNoTracking
                    //    .Where(x => x.ResourceName == "KhamBenh.HoanThanhKham.DichVuLaThamVanChuaHoanThanh" && x.Language == (int)currentUserLanguge)
                    //    .Select(x => x.ResourceValue).FirstOrDefaultAsync();
                    //throw new Exception(mess ?? "KhamBenh.HoanThanhKham.DichVuLaThamVanChuaHoanThanh");
                    throw new Exception(_localizationService.GetResource("KhamBenh.HoanThanhKham.DichVuLaThamVanChuaHoanThanh"));
                }

                //cập nhật: 30/11/2022: bỏ logic cũ - khám ngoại trú, KSK ko có dv giường
                //foreach (var giuong in hangDoiDangKham.YeuCauKhamBenh.YeuCauDichVuGiuongBenhViens.SelectMany(x => x.HoatDongGiuongBenhs))
                //{
                //    giuong.ThoiDiemKetThuc = DateTime.Now;
                //}

                hangDoiDangKham.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
                // cập nhật thời gian hoàn thành khám
                hangDoiDangKham.YeuCauKhamBenh.ThoiDiemHoanThanh = hangDoiDangKham.YeuCauKhamBenh.ThoiDiemHoanThanh ?? DateTime.Now;

                hangDoiDangKham.WillDelete = true;
                // lưu lịch sử
                var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                lichSuNew.TrangThaiYeuCauKhamBenh = hangDoiDangKham.YeuCauKhamBenh.TrangThai;
                lichSuNew.MoTa = "Hoàn thành yêu cầu khám: " + lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                hangDoiDangKham.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);

                var daLuu = await _yeuCauTiepNhanService.KiemTraVaLuuYeuCauNhapVienTuKhamNgoaiTruAsync(hangDoiDangKham.YeuCauKhamBenh);
                if (!daLuu)
                {
                    await _phongBenhVienHangDoiRepository.UpdateAsync(hangDoiDangKham);
                }

            }
            else // trường hợp lưu thông tin cho lần khám hiện tại, và khám yêu cầu khác đồng thời cập nhật lại ví trí hàng chờ
            {
                #region Code xử lý STT
                //// get list trạng thái yêu cầu khám bệnh theo hàng chờ của hàng đợi đang khám
                //var lstTrangThaiYeuCauKhamTheoHangDoiDangKham = new List<Enums.EnumTrangThaiYeuCauKhamBenh>();
                //if (hangDoiDangKham.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham &&
                //    hangDoiDangKham.YeuCauKhamBenh.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DangKham)
                //{
                //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(hangDoiDangKham.YeuCauKhamBenh.TrangThai);
                //}
                //else
                //{
                //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham);
                //    lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Add(Enums.EnumTrangThaiYeuCauKhamBenh.DangKham);
                //}

                //var laKhamDoan = hangDoiDangKham.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe;
                //var lstHangDoiTheoPhongKham = _phongBenhVienHangDoiRepository.Table
                //    //.Include(x => x.YeuCauTiepNhan)
                //    .Include(x => x.YeuCauKhamBenh)//.ThenInclude(y => y.YeuCauDichVuKyThuats)
                //    .Where(x => x.PhongBenhVienId == hangDoiDangKham.PhongBenhVienId
                //                && x.YeuCauKhamBenh != null
                //                //&& x.YeuCauTiepNhan.ThoiDiemTiepNhan.Date == DateTime.Now.Date -- sau này sẽ dùng lại
                //                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                //                && lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Contains(x.YeuCauKhamBenh.TrangThai)

                //                // bổ sung khám đoàn
                //                && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)))
                //    //&& lstTrangThaiYeuCauKhamTheoHangDoiDangKham.Any(item => item == x.YeuCauKhamBenh.TrangThai))
                //    .OrderBy(x => x.SoThuTu).ToList();

                //var viTriHangDoiDangKham = lstHangDoiTheoPhongKham.FindLastIndex(x => x.Id == hangDoiHienTaiId);
                //if (viTriHangDoiDangKham != 0 && lstHangDoiTheoPhongKham.Count() > 1)
                //{
                //    var sttDauTien = 0;
                //    foreach (var hangDoi in lstHangDoiTheoPhongKham)
                //    {
                //        sttDauTien = sttDauTien == 0 ? hangDoi.SoThuTu : sttDauTien;
                //        if (hangDoi.Id == hangDoiHienTaiId)
                //        {
                //            hangDoi.SoThuTu = sttDauTien;
                //            hangDoi.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                //            hangDoi.YeuCauKhamBenh.LastTime = DateTime.Now;

                //            //if ((hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham || hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
                //            if (hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                //                //&& hangDoi.YeuCauKhamBenh.YeuCauDichVuKyThuats.Any(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                //                //                                                        && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                //                //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                //                //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                //                //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                //                )
                //            {
                //                var coDVKTCLSThemMoi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                //                    .Any(x => x.YeuCauKhamBenhId == hangDoi.YeuCauKhamBenh.Id
                //                              && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                //                              && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh));
                //                if (coDVKTCLSThemMoi)
                //                {
                //                    var hangDoiLamChiDinh = _phongBenhVienHangDoiRepository.TableNoTracking
                //                        .Include(x => x.YeuCauKhamBenh)
                //                        .Where(x => x.YeuCauKhamBenh != null
                //                                    && x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                //                        .OrderByDescending(x => x.SoThuTu).FirstOrDefault();
                //                    if (hangDoiLamChiDinh != null)
                //                    {
                //                        hangDoi.SoThuTu = hangDoiLamChiDinh.SoThuTu + 1;
                //                    }
                //                    hangDoi.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh;

                //                    // lưu lịch sử
                //                    var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                //                    lichSuNew.TrangThaiYeuCauKhamBenh = hangDoi.YeuCauKhamBenh.TrangThai;
                //                    lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                //                    hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            hangDoi.SoThuTu += 1;
                //        }
                //    }
                //}
                //else if (viTriHangDoiDangKham == 0)
                //{
                //    foreach (var hangDoi in lstHangDoiTheoPhongKham)
                //    {
                //        if (hangDoi.Id == hangDoiHienTaiId)
                //        {
                //            hangDoi.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                //            hangDoi.YeuCauKhamBenh.LastTime = DateTime.Now;
                //            //if ((hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham || hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
                //            if (hangDoi.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan
                //                 //&& hangDoi.YeuCauKhamBenh.YeuCauDichVuKyThuats.Any(x => x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                //                 //                                                        && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                //                 //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                //                 //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                //                 //                                                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                //                 )
                //            {
                //                var coDVKTCLSThemMoi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                //                    .Any(x => x.YeuCauKhamBenhId == hangDoi.YeuCauKhamBenh.Id
                //                              && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                //                              && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                //                                  || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh));
                //                if (coDVKTCLSThemMoi)
                //                {
                //                    var hangDoiLamChiDinh = _phongBenhVienHangDoiRepository.TableNoTracking
                //                        .Include(x => x.YeuCauKhamBenh)
                //                        .Where(x => x.YeuCauKhamBenh != null
                //                                    && x.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh)
                //                        .OrderByDescending(x => x.SoThuTu).FirstOrDefault();
                //                    if (hangDoiLamChiDinh != null)
                //                    {
                //                        hangDoi.SoThuTu = hangDoiLamChiDinh.SoThuTu + 1;
                //                    }

                //                    hangDoi.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh;

                //                    // lưu lịch sử
                //                    var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                //                    lichSuNew.TrangThaiYeuCauKhamBenh = hangDoi.YeuCauKhamBenh.TrangThai;
                //                    lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                //                    hangDoi.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
                //                }
                //            }
                //            break;
                //        }
                //    }
                //}
                #endregion

                #region Cập nhật 25/03/2022
                hangDoiDangKham.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                hangDoiDangKham.YeuCauKhamBenh.LastTime = DateTime.Now;
                if (hangDoiDangKham.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangDoiKetLuan)
                {
                    var coDVKTCLSThemMoi = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Any(x => x.YeuCauKhamBenhId == hangDoiDangKham.YeuCauKhamBenh.Id
                                  && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                  && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat
                                      || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                      || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang
                                      || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh));
                    if (coDVKTCLSThemMoi)
                    {
                        hangDoiDangKham.YeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh;

                        // lưu lịch sử
                        var lichSuNew = new YeuCauKhamBenhLichSuTrangThai();
                        lichSuNew.TrangThaiYeuCauKhamBenh = hangDoiDangKham.YeuCauKhamBenh.TrangThai;
                        lichSuNew.MoTa = lichSuNew.TrangThaiYeuCauKhamBenh.GetDescription();
                        hangDoiDangKham.YeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(lichSuNew);
                    }
                }
                #endregion

                //_phongBenhVienHangDoiRepository.Update(lstHangDoiTheoPhongKham);
                _phongBenhVienHangDoiRepository.Context.SaveChanges();
            }
        }

        public async Task<bool> KiemTraCoBenhNhanKhacDangKhamTrongPhong(long? hangDoiDangKham, long phongBenhVienId, bool laKhamDoan = false)
        {
            var coBenhNhanKhacDangKham = await _phongBenhVienHangDoiRepository.TableNoTracking
                .AnyAsync(x => (hangDoiDangKham == null || x.Id != hangDoiDangKham)
                               && x.YeuCauKhamBenhId != null
                               && x.PhongBenhVienId == phongBenhVienId
                               && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham
                               && ((laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) || (!laKhamDoan && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan != Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)));
            return coBenhNhanKhacDangKham;
        }

        public async Task XuLyDataYeuCauKhamBenhChuyenKhamAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKham, bool coBaoHiem)
        {
            var dichVuKhamBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Where(x => x.Id == yeuCauKham.DichVuKhamBenhBenhVienId)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuKhamBenhBenhVien)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .FirstAsync();

            // kiểm tra giá bệnh viện
            var lstGiaBenhVien = dichVuKhamBenhVien.DichVuKhamBenhBenhVienGiaBenhViens
                .Where(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date)).ToList();
            var donGiaThuong = lstGiaBenhVien.FirstOrDefault(x => x.NhomGiaDichVuKhamBenhBenhVien.Ten == "Thường");
            if (donGiaThuong != null)
            {
                yeuCauKham.Gia = donGiaThuong.Gia;
                yeuCauKham.NhomGiaDichVuKhamBenhBenhVienId = donGiaThuong.NhomGiaDichVuKhamBenhBenhVienId;
            }
            else
            {
                var giaBenhVien = lstGiaBenhVien.First();
                yeuCauKham.Gia = giaBenhVien.Gia;
                yeuCauKham.NhomGiaDichVuKhamBenhBenhVienId = giaBenhVien.NhomGiaDichVuKhamBenhBenhVienId;
            }

            // kiểm tra giá bảo hiểm
            var lstGiaBaoHiem = dichVuKhamBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems
                .Where(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date)).ToList();
            if (lstGiaBaoHiem.Any())
            {
                var giaBH = lstGiaBaoHiem.First();

                yeuCauKham.DuocHuongBaoHiem = coBaoHiem;
                yeuCauKham.DonGiaBaoHiem = giaBH.Gia;
                yeuCauKham.TiLeBaoHiemThanhToan = giaBH.TiLeBaoHiemThanhToan;
            }

            // gán các thông tin khác
            yeuCauKham.MaDichVu = dichVuKhamBenhVien.Ma;
            yeuCauKham.TenDichVu = dichVuKhamBenhVien.Ten;


            yeuCauKham.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            yeuCauKham.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            yeuCauKham.ThoiDiemChiDinh = DateTime.Now;

            yeuCauKham.TrangThaiThanhToan = Enums.TrangThaiThanhToan.ChuaThanhToan;
            yeuCauKham.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
            yeuCauKham.BaoHiemChiTra = null;
            yeuCauKham.ThoiDiemDangKy = DateTime.Now;
        }

        public async Task KiemTraDataChuyenKhamAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId, long dichVuKhamBenhId)
        {
            var yeuCauKhamCanHuy = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauKhamBenhId);
            if (yeuCauKhamCanHuy == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if (yeuCauKhamCanHuy.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
            {
                throw new Exception(_localizationService.GetResource("ChuyenDichVuKham.YeuCauKhamBenh.CanThuNganHuyThanhToan"));
            }


            #region kiểm tra dịch vụ khám bệnh giá bệnh viện

            var dichVuKhamBenhVien = await _dichVuKhamBenhBenhVienRepository.TableNoTracking
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
                .FirstAsync(x => x.Id == dichVuKhamBenhId);
            if (!dichVuKhamBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.Any(x => x.TuNgay.Date <= DateTime.Now.Date && (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date)))
            {
                throw new Exception(_localizationService.GetResource("ChuyenDichVuKham.DichVuKhamBenhGiaBenhVien.NotExists"));
            }
            #endregion

            var isThucHienHoacThanhToan = false;

            #region kiểm tra yêu cầu dịch vụ khám bệnh và đơn thuốc, vật tư thanh toán
            if (yeuCauTiepNhan.YeuCauKhamBenhs.Any(x => (x.Id == yeuCauKhamBenhId || x.YeuCauKhamBenhTruocId == yeuCauKhamBenhId)
                                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                        && (x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                        && (x.DonThuocThanhToans.Any(a => a.TrangThai == Enums.TrangThaiDonThuocThanhToan.DaXuatThuoc || a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan)
                                                            || x.DonVTYTThanhToans.Any(a => a.TrangThai == Enums.TrangThaiDonVTYTThanhToan.DaXuatVTYT || a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan))))
            {
                isThucHienHoacThanhToan = true;
            }
            #endregion

            #region kiểm tra yêu cầu dịch vụ kỹ thuật
            if (yeuCauTiepNhan.YeuCauDichVuKyThuats.Any(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && (x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien
                                                            || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                            || x.YeuCauDichVuKyThuatTuongTrinhPTTT != null)))
            {
                isThucHienHoacThanhToan = true;
            }
            #endregion

            #region kiểm tra yêu cầu dược phẩm
            if (yeuCauTiepNhan.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                                             && (x.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien
                                                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                                 || x.YeuCauLinhDuocPhamId != null
                                                                 || x.XuatKhoDuocPhamChiTiet?.XuatKhoDuocPhamId != null)))
            {
                isThucHienHoacThanhToan = true;
            }
            #endregion

            #region kiểm tra yêu cầu vật tư bệnh viện
            if (yeuCauTiepNhan.YeuCauVatTuBenhViens.Any(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                                                             && (x.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien
                                                                 || x.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan
                                                                 || x.YeuCauLinhVatTuId != null
                                                                 || x.XuatKhoVatTuChiTiet?.XuatKhoVatTuId != null)))
            {
                isThucHienHoacThanhToan = true;
            }
            #endregion

            if (isThucHienHoacThanhToan)
            {
                throw new Exception(_localizationService.GetResource("ChuyenDichVuKham.YeuCauDichVu.DaThucHienHoacThanhToan"));
            }
        }
        #endregion

        #region  Khám đoàn khám bệnh tất cả phòng

        public async Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachHangDoiKhamDoanTatCaAsync(KhamDoanKhamBenhTatCaPhongTimKiemVo timKiemVo)
        {
            //todo: fix lại group theo người bệnh
            //var query = await _phongBenhVienHangDoiRepository.TableNoTracking
            //    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
            //    .Include(x => x.YeuCauKhamBenh)
            //    .Where(x => x.YeuCauKhamBenh != null
            //                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
            //                && ((timKiemVo.CongTyId == null && timKiemVo.HopDongId == null) 
            //                    || (timKiemVo.HopDongId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemVo.HopDongId) 
            //                    || (timKiemVo.CongTyId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == timKiemVo.CongTyId)))
            //    .OrderBy(x => x.SoThuTu)
            //    .Select(s => new BenhNhanChoKhamGridVo
            //    {
            //        Id = s.Id,
            //        SoThuTu = s.SoThuTu,
            //        MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
            //        HoTen = s.YeuCauTiepNhan.HoTen,
            //        MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
            //        TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
            //        Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
            //        LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
            //        YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

            //        HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "", 
            //        CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
            //        NgayKhamBenh = s.CreatedOn.Value.Date,

            //        // cập nhật dùng chung cho khám đoàn
            //        DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
            //        TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
            //    })
            //    .ApplyLike(timKiemVo.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN).ToListAsync();

            var query = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x =>
                            x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != timKiemVo.HopDongKhamNhanVienId
                            && x.YeuCauKhamBenh != null
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            //&& ((timKiemVo.CongTyId == null && timKiemVo.HopDongId == null)
                            //    || (timKiemVo.HopDongId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemVo.HopDongId)
                            //    || (timKiemVo.CongTyId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == timKiemVo.CongTyId)))
                            && timKiemVo.CongTyId != null
                            && timKiemVo.HopDongId != null
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemVo.HopDongId
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == timKiemVo.CongTyId)
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhId,
                    HopDongKhamSucKhoeNhanVienId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    NamSinh = s.YeuCauTiepNhan.NamSinh,
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "",
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    // cập nhật dùng chung cho khám đoàn
                    //DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                    //TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),

                    TrangThai = s.TrangThai,
                    TrangThaiYeuCauTiepNhan = s.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                    ThoiDiemTiepNhan = s.YeuCauTiepNhan.ThoiDiemTiepNhan,
                    LoaiHangDoi = Enums.EnumLoaiHangDoi.ChuanBiKham,
                    PhongBenhVienId = s.PhongBenhVienId,
                    GioiTinh = s.YeuCauTiepNhan.GioiTinh ?? Enums.LoaiGioiTinh.ChuaXacDinh
                })
                .ApplyLike(timKiemVo.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN).ToListAsync();

            var result = query
                .GroupBy(x => x.HopDongKhamSucKhoeNhanVienId)
                .Select(s => s.First())
                .ToList();
            //{
            //    Id = s.First().Id,
            //    BenhNhanId = s.Key,
            //    SoThuTu = s.First().SoThuTu,
            //    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
            //    HoTen = s.First().HoTen,
            //    MaBN = s.First().MaBN,
            //    TenGioiTinh = s.First().TenGioiTinh,
            //    Tuoi = s.First().Tuoi,
            //    LyDoKham = s.First().LyDoKham,
            //    YeuCauKhamBenhLastModified = s.First().YeuCauKhamBenhLastModified,

            //    HighLightClass = s.First().HighLightClass,
            //    CoBaoHiem = s.First().CoBaoHiem,
            //    NgayKhamBenh = s.First().NgayKhamBenh,

            //    // cập nhật dùng chung cho khám đoàn
            //    DichVuKhamDaThucHien = s.First().DichVuKhamDaThucHien,
            //    TongDichVuKham = s.First().TongDichVuKham
            //})

            return result;
        }


        public async Task<GridDataSource> GetDataForGridHangDoiKhamDoanTatCaAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemVo = new KhamDoanKhamBenhTatCaPhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemVo = JsonConvert.DeserializeObject<KhamDoanKhamBenhTatCaPhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenh != null
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                            && ((timKiemVo.CongTyId == null && timKiemVo.HopDongId == null)
                                || (timKiemVo.HopDongId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemVo.HopDongId)
                                || (timKiemVo.CongTyId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == timKiemVo.CongTyId)))
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    HopDongKhamSucKhoeNhanVienId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "",
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    // cập nhật dùng chung cho khám đoàn
                    DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                    TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                })
                .ApplyLike(timKiemVo.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN)
                .GroupBy(x => new { x.HopDongKhamSucKhoeNhanVienId })
                .Select(s => new BenhNhanChoKhamGridVo()
                {
                    Id = s.First().Id,
                    HopDongKhamSucKhoeNhanVienId = s.First().HopDongKhamSucKhoeNhanVienId,
                    SoThuTu = s.First().SoThuTu,
                    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                    HoTen = s.First().HoTen,
                    MaBN = s.First().MaBN,
                    TenGioiTinh = s.First().TenGioiTinh,
                    Tuoi = s.First().Tuoi,
                    LyDoKham = s.First().LyDoKham,
                    YeuCauKhamBenhLastModified = s.First().YeuCauKhamBenhLastModified,

                    HighLightClass = s.First().HighLightClass,
                    CoBaoHiem = s.First().CoBaoHiem,
                    NgayKhamBenh = s.First().NgayKhamBenh,

                    // cập nhật dùng chung cho khám đoàn
                    DichVuKhamDaThucHien = s.First().DichVuKhamDaThucHien,
                    TongDichVuKham = s.First().TongDichVuKham
                });


            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }
        public async Task<GridDataSource> GetTotalPageForGriHangDoiKhamDoanTatCaAsync(QueryInfo queryInfo)
        {
            var timKiemVo = new KhamDoanKhamBenhTatCaPhongTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemVo = JsonConvert.DeserializeObject<KhamDoanKhamBenhTatCaPhongTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var query = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenh != null
                            && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId != null
                            && ((timKiemVo.CongTyId == null && timKiemVo.HopDongId == null)
                                || (timKiemVo.HopDongId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoeId == timKiemVo.HopDongId)
                                || (timKiemVo.CongTyId != null && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVien.HopDongKhamSucKhoe.CongTyKhamSucKhoeId == timKiemVo.CongTyId)))
                .OrderBy(x => x.SoThuTu)
                .Select(s => new BenhNhanChoKhamGridVo
                {
                    Id = s.Id,
                    HopDongKhamSucKhoeNhanVienId = s.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId,
                    SoThuTu = s.SoThuTu,
                    MaYeuCauTiepNhan = s.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    MaBN = s.YeuCauTiepNhan.BenhNhan != null ? s.YeuCauTiepNhan.BenhNhan.MaBN : "",
                    TenGioiTinh = s.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = s.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh.Value),
                    LyDoKham = s.YeuCauTiepNhan.TrieuChungTiepNhan,
                    YeuCauKhamBenhLastModified = s.YeuCauKhamBenh.LastModified,

                    HighLightClass = s.YeuCauKhamBenh.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DangKham ? "bg-row-lightblue" : "",
                    CoBaoHiem = s.YeuCauTiepNhan.CoBHYT ?? false,
                    NgayKhamBenh = s.CreatedOn.Value.Date,

                    // cập nhật dùng chung cho khám đoàn
                    DichVuKhamDaThucHien = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham),
                    TongDichVuKham = s.YeuCauTiepNhan.YeuCauKhamBenhs.Count(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                })
                .ApplyLike(timKiemVo.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.MaBN)
                .GroupBy(x => new { x.HopDongKhamSucKhoeNhanVienId })
                .Select(s => new BenhNhanChoKhamGridVo()
                {
                    Id = s.First().Id,
                    HopDongKhamSucKhoeNhanVienId = s.First().HopDongKhamSucKhoeNhanVienId,
                    SoThuTu = s.First().SoThuTu,
                    MaYeuCauTiepNhan = s.First().MaYeuCauTiepNhan,
                    HoTen = s.First().HoTen,
                    MaBN = s.First().MaBN,
                    TenGioiTinh = s.First().TenGioiTinh,
                    Tuoi = s.First().Tuoi,
                    LyDoKham = s.First().LyDoKham,
                    YeuCauKhamBenhLastModified = s.First().YeuCauKhamBenhLastModified,

                    HighLightClass = s.First().HighLightClass,
                    CoBaoHiem = s.First().CoBaoHiem,
                    NgayKhamBenh = s.First().NgayKhamBenh,

                    // cập nhật dùng chung cho khám đoàn
                    DichVuKhamDaThucHien = s.First().DichVuKhamDaThucHien,
                    TongDichVuKham = s.First().TongDichVuKham
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(long hangDoiId)
        {
            var query =
                await _phongBenhVienHangDoiRepository.Table
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.DanToc)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NgheNghiep)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.PhuongXa)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.QuanHuyen)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.TinhThanh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanDiUngThuocs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.BenhNhan).ThenInclude(z => z.BenhNhanTienSuBenhs)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.KetQuaSinhHieus)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.NhanVienTiepNhan).ThenInclude(u => u.User)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.LyDoTiepNhan)
                    .Include(x => x.YeuCauKhamBenh)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.HopDongKhamSucKhoeNhanVien).ThenInclude(t => t.HopDongKhamSucKhoe).ThenInclude(u => u.CongTyKhamSucKhoe)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.YeuCauKhamBenhs).ThenInclude(z => z.PhongBenhVienHangDois)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.YeuCauDichVuKyThuats)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.YeuCauDuocPhamBenhViens)
                    .Include(x => x.YeuCauTiepNhan).ThenInclude(y => y.YeuCauVatTuBenhViens)

                    .Where(x => x.Id == hangDoiId
                                && x.YeuCauKhamBenh != null
                                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe).FirstOrDefaultAsync();
            return query;
        }

        public async Task<List<KetQuaMauDichVuKyThuatDataVo>> GetKetQuaMauDichVuKyThuatAsync(KhamDoanTatCaPhongKetQuaMauVo ketQuaMuaVo)
        {
            var lstDichVu = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == ketQuaMuaVo.YeuCauTiepNhanId
                            && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe
                            && x.YeuCauTiepNhan.HopDongKhamSucKhoeNhanVienId == ketQuaMuaVo.HopDongKhamSucKhoeNhanVienId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && x.NhanVienKetLuanId != null
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem))
                .Select(x => new KetQuaMauDichVuKyThuatDataVo()
                {
                    NoiDung = x.TenDichVu
                })
                .Distinct()
                .ToListAsync();
            return lstDichVu;
        }

        public async Task<KhamDoanTatCaPhongDichVuChuaThucHienVo> KiemTraDichVuKhamDoanChuaThucHienAsync(KhamDoanTatCaPhongKiemTraDichVuChuaThucHienVo kiemTraVo)
        {
            //var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(kiemTraVo.YeuCauTiepNhanId,
            //    x => x.Include(y => y.YeuCauDichVuKyThuats).Include(y => y.YeuCauKhamBenhs));

            var dichVuChuaThucHienVo = new KhamDoanTatCaPhongDichVuChuaThucHienVo()
            {
                //DichVuKyThuats = yeuCauTiepNhan.YeuCauDichVuKyThuats.Where(x => x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien).Select(x => x.TenDichVu).ToList(),
                DichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == kiemTraVo.YeuCauTiepNhanId
                                                                                           && x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                    .Select(x => x.TenDichVu).ToList(),
                DichVuKhamBenhs = BaseRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == kiemTraVo.YeuCauTiepNhanId
                                                                            && x.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham
                                                                            && !kiemTraVo.YeuCauKhamBenhDangChonThucHienIds.Contains(x.Id))
                    .Select(x => x.TenDichVu).ToList()
            };
            return dichVuChuaThucHienVo;
        }
        #endregion


        #region cập nhật quay lại chưa khám
        public async Task XuLyQuayLaiChuaKhamAsync(long hangDoiId)
        {
            var hangDoi = _phongBenhVienHangDoiRepository.Table
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhICDKhacs)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhKhamBoPhanKhacs)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhBoPhanTonThuongs)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhChanDoanPhanBiets)
                .Include(x => x.YeuCauKhamBenh).ThenInclude(x => x.YeuCauKhamBenhLichSuTrangThais)
                .Where(x => x.Id == hangDoiId
                            && x.YeuCauKhamBenhId != null)
                .FirstOrDefault();
            if (hangDoi == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }


            var yeuCauKhamHienTai = hangDoi.YeuCauKhamBenh;
            var yeucauTiepNhanHienTai = hangDoi.YeuCauTiepNhan;

            if (yeuCauKhamHienTai.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHuy"));
            }

            if (yeuCauKhamHienTai.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"));
            }

            // kiểm tra yêu cầu khám hiện tại có chỉ định, hoặc có lưu thông tin trong tab chẩn đoán hay chưa
            // nếu có thì ko cho quay lại chưa khám -> cảnh báo người dùng phải xóa data trước
            if (
                // tab chỉ định
                yeucauTiepNhanHienTai.YeuCauKhamBenhs.Any(x => x.YeuCauKhamBenhTruocId == yeuCauKhamHienTai.Id && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                || yeuCauKhamHienTai.YeuCauDichVuKyThuats.Any(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                || yeuCauKhamHienTai.YeuCauDuocPhamBenhViens.Any(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                || yeuCauKhamHienTai.YeuCauVatTuBenhViens.Any(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)

                // tab chẩn đoán
                || !string.IsNullOrEmpty(yeuCauKhamHienTai.TomTatKetQuaCLS)
                || yeuCauKhamHienTai.IcdchinhId != null
                || !string.IsNullOrEmpty(yeuCauKhamHienTai.GhiChuICDChinh)
                || yeuCauKhamHienTai.YeuCauKhamBenhICDKhacs.Any()
                || !string.IsNullOrEmpty(yeuCauKhamHienTai.CachGiaiQuyet)
                || yeuCauKhamHienTai.CoKeToa == true
                || yeuCauKhamHienTai.KhongKeToa == true
                || yeuCauKhamHienTai.CoTaiKham == true
                || yeuCauKhamHienTai.CoDieuTriNgoaiTru == true
                || yeuCauKhamHienTai.CoNhapVien == true
                || yeuCauKhamHienTai.CoChuyenVien == true
                || yeuCauKhamHienTai.CoTuVong == true
            )
            {
                throw new Exception(_localizationService.GetResource("Khambenh.QuayLaiChuaKham.ChuaXoaHetDuLieu"));
            }

            yeuCauKhamHienTai.BenhSu = null;
            yeuCauKhamHienTai.ChanDoanCuaNoiGioiThieu = null;
            yeuCauKhamHienTai.KhamToanThan = null;
            yeuCauKhamHienTai.ThongTinKhamTheoDichVuData = null;
            yeuCauKhamHienTai.ChanDoanSoBoICDId = null;
            yeuCauKhamHienTai.ChanDoanSoBoGhiChu = null;

            foreach (var icdKhac in yeuCauKhamHienTai.YeuCauKhamBenhKhamBoPhanKhacs)
            {
                icdKhac.WillDelete = true;
            }

            foreach (var icdKhac in yeuCauKhamHienTai.YeuCauKhamBenhBoPhanTonThuongs)
            {
                icdKhac.WillDelete = true;
            }

            foreach (var icdKhac in yeuCauKhamHienTai.YeuCauKhamBenhChanDoanPhanBiets)
            {
                icdKhac.WillDelete = true;
            }

            yeuCauKhamHienTai.TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham;

            yeuCauKhamHienTai.NoiThucHienId = null;
            yeuCauKhamHienTai.BacSiThucHienId = null;
            yeuCauKhamHienTai.ThoiDiemThucHien = null;
            yeuCauKhamHienTai.ThoiDiemHoanThanh = null;

            yeuCauKhamHienTai.BacSiKetLuanId = null;
            yeuCauKhamHienTai.NoiKetLuanId = null;

            // bổ sung lịch sử cho yckb -> sau này kiểm tra cho dễ
            var newLichSu = new YeuCauKhamBenhLichSuTrangThai()
            {
                TrangThaiYeuCauKhamBenh = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                MoTa = "Chuyển dịch vụ khám về chưa khám"
            };
            yeuCauKhamHienTai.YeuCauKhamBenhLichSuTrangThais.Add(newLichSu);


            hangDoi.TrangThai = EnumTrangThaiHangDoi.ChoKham;

            await _phongBenhVienHangDoiRepository.Context.SaveChangesAsync();
        }

        public async Task XuLyQuayLaiChuaKhamKhamDoanTheoPhongAsync(PhongBenhVienHangDoi hangDoi, string dataDefaultChuyenKhoaKham = null)
        {
            if (hangDoi.YeuCauKhamBenh == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }


            var yeuCauKhamHienTai = hangDoi.YeuCauKhamBenh;
            var yeucauTiepNhanHienTai = hangDoi.YeuCauTiepNhan;

            if (yeuCauKhamHienTai.TrangThai == EnumTrangThaiYeuCauKhamBenh.HuyKham)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHuy"));
            }

            if (yeuCauKhamHienTai.TrangThai == EnumTrangThaiYeuCauKhamBenh.DaKham)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHoanThanhKham"));
            }

            yeuCauKhamHienTai.TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham;
            // kiểm tra tất cả dv khám sức khỏe khác
            // nếu tất cả là chưa khám, thì xóa phân loại sức khỏe
            var coDataTabTuVanThuoc = false;
            if (yeucauTiepNhanHienTai.YeuCauKhamBenhs.Where(x => x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham).All(x => x.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham))
            {
                if (yeucauTiepNhanHienTai.TuVanThuocKhamSucKhoes.Any())
                {
                    coDataTabTuVanThuoc = true;
                }

                yeucauTiepNhanHienTai.KSKPhanLoaiTheLuc = null;
            }

            // kiểm tra yêu cầu khám hiện tại có chỉ định, hoặc có lưu thông tin trong tab chẩn đoán hay chưa
            // nếu có thì ko cho quay lại chưa khám -> cảnh báo người dùng phải xóa data trước
            if (
                // tab chỉ định
                yeucauTiepNhanHienTai.YeuCauKhamBenhs.Any(x => x.YeuCauKhamBenhTruocId == yeuCauKhamHienTai.Id && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                || yeuCauKhamHienTai.YeuCauDichVuKyThuats.Any(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                || yeuCauKhamHienTai.YeuCauDuocPhamBenhViens.Any(x => x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                || yeuCauKhamHienTai.YeuCauVatTuBenhViens.Any(x => x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy)
                || coDataTabTuVanThuoc
            )
            {
                if (coDataTabTuVanThuoc)
                {
                    throw new Exception(_localizationService.GetResource("KhambenhKhamSucKhoe.QuayLaiChuaKham.ChuaXoaHetDuLieu"));
                }
                else
                {
                    throw new Exception(_localizationService.GetResource("KhambenhKhamSucKhoe.QuayLaiChuaKham.ChuaXoaHetDuLieuTabChiDinh"));
                }
            }

            yeuCauKhamHienTai.BenhSu = null;
            yeuCauKhamHienTai.ChanDoanCuaNoiGioiThieu = null;
            yeuCauKhamHienTai.ThongTinKhamTheoDichVuData = dataDefaultChuyenKhoaKham;

            //yeuCauKhamHienTai.NoiThucHienId = null; ->  khám đoàn mặc định đã gián nơi thực hiện = nơi đăng ký
            yeuCauKhamHienTai.BacSiThucHienId = null;
            yeuCauKhamHienTai.ThoiDiemThucHien = null;

            yeuCauKhamHienTai.BacSiKetLuanId = null;
            yeuCauKhamHienTai.NoiKetLuanId = null;

            // bổ sung lịch sử cho yckb -> sau này kiểm tra cho dễ
            var newLichSu = new YeuCauKhamBenhLichSuTrangThai()
            {
                TrangThaiYeuCauKhamBenh = EnumTrangThaiYeuCauKhamBenh.ChuaKham,
                MoTa = "Chuyển dịch vụ khám về chưa khám"
            };
            yeuCauKhamHienTai.YeuCauKhamBenhLichSuTrangThais.Add(newLichSu);


            hangDoi.TrangThai = EnumTrangThaiHangDoi.ChoKham;

            await _phongBenhVienHangDoiRepository.Context.SaveChangesAsync();
        }
        #endregion

        #region BVHD-3574
        public async Task<bool> KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(long? phongId = null)
        {
            var phongHienTaiId = phongId ?? _userAgentHelper.GetCurrentNoiLLamViecId();
            var phognBenhVien = await _phongBenhVienRepository.TableNoTracking
                .FirstAsync(x => x.Id == phongHienTaiId);

            var lstCauHinhKhoaCoNhieuNguoiBenhId = await _cauHinhRepository.TableNoTracking
                .Where(x => !string.IsNullOrEmpty(x.Value)
                            && (x.Name.Contains("CauHinhKhamBenh.KhoaNoi")
                            || x.Name.Contains("CauHinhKhamBenh.KhoaNgoai")
                            || x.Name.Contains("CauHinhNoiTru.KhoaNhi")
                            || x.Name.Contains("CauHinhNoiTru.KhoaPhuSan")))
                .Select(x => long.Parse(x.Value))
                .Distinct()
                .ToListAsync();

            return lstCauHinhKhoaCoNhieuNguoiBenhId.Contains(phognBenhVien.KhoaPhongId);
        }

        public async Task<bool> KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(long dichVuBenhVienId)
        {
            //Cập nhật 01/12/2022: bỏ await
            //var lstCauHinhDichVuCoNhieuNguoiBenhId = _cauHinhRepository.TableNoTracking
            //    .Where(x => !string.IsNullOrEmpty(x.Value)
            //                && (x.Name.Contains("CauHinhKhamBenh.DichVuKhamNoi")
            //                    || x.Name.Contains("CauHinhKhamBenh.DichVuKhamNgoai")
            //                    || x.Name.Contains("CauHinhKhamBenh.DichVuKhamPhuSan")
            //                    || x.Name.Contains("CauHinhKhamBenh.DichVuKhamNhi")))
            //    .Select(x => x.Value)
            //    .Distinct()
            //    .ToList();
            //return lstCauHinhDichVuCoNhieuNguoiBenhId.Contains(dichVuBenhVienId);

            var strDichVuId = dichVuBenhVienId.ToString();
            var lstCauHinhDichVuCoNhieuNguoiBenhId = _cauHinhRepository.TableNoTracking
                .Where(x => x.Value.Equals(strDichVuId)
                            && (x.Name.Equals("CauHinhKhamBenh.DichVuKhamNoi")
                                || x.Name.Equals("CauHinhKhamBenh.DichVuKhamNgoai")
                                || x.Name.Equals("CauHinhKhamBenh.DichVuKhamPhuSan")
                                || x.Name.Equals("CauHinhKhamBenh.DichVuKhamNhi")))
                .Select(x => x.Value)
                .ToList();
            var results = lstCauHinhDichVuCoNhieuNguoiBenhId.Select(x => long.Parse(x)).Distinct().ToList();
            return results.Contains(dichVuBenhVienId);
        }

        #endregion

        #region BVHD-3797
        public async Task<List<string>> KiemTraCoDichVuChuaThucHienAsync(long yeuCauKhamBenhId)
        {
            var lstDichVuChuaHoanThanh = new List<string>();
            var dichVuKhams = BaseRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenhTruocId == yeuCauKhamBenhId 
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham 
                            && x.TrangThai != EnumTrangThaiYeuCauKhamBenh.DaKham)
                .Select(x => x.TenDichVu)
                //.Distinct()
                .ToList();

            var dichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                .Select(x => x.TenDichVu)
                //.Distinct()
                .ToList();

            lstDichVuChuaHoanThanh.AddRange(dichVuKhams);
            lstDichVuChuaHoanThanh.AddRange(dichVuKyThuats);

            return lstDichVuChuaHoanThanh.Distinct().ToList();
        }


        #endregion

        #region BVHD-3895
        public async Task<bool> KiemTraDichVuKhamHienThiTenVietTatAsync(long dichVuKhamBenhId)
        {
            var lstMaCauHinhDVKhamVietTat = EnumHelper.GetListEnum<Enums.KhamBenhDichVuKhamVietTat>().Select(item => item.GetDescription()).ToList();
            if (lstMaCauHinhDVKhamVietTat.Any())
            {
                var lstCauHinhNhomKham = _cauHinhRepository.TableNoTracking
                    .Where(x => lstMaCauHinhDVKhamVietTat.Contains(x.Name)
                                && x.DataType == Enums.DataType.Integer)
                    .ToList();
                var lstDVKhamVietTatId = lstCauHinhNhomKham
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => long.Parse(x.Value)).Distinct().ToList();
                return lstDVKhamVietTatId.Contains(dichVuKhamBenhId);
            }

            return false;
        }

        private List<long> GetichVuKhamIdHienThiTenVietTatAsync()
        {
            var lstDichVuId = new List<long>();
            var lstMaCauHinhDVKhamVietTat = EnumHelper.GetListEnum<Enums.KhamBenhDichVuKhamVietTat>().Select(item => item.GetDescription()).ToList();
            if (lstMaCauHinhDVKhamVietTat.Any())
            {
                var lstCauHinhNhomKham = _cauHinhRepository.TableNoTracking
                    .Where(x => lstMaCauHinhDVKhamVietTat.Contains(x.Name)
                                && x.DataType == Enums.DataType.Integer)
                    .ToList();
                lstDichVuId = lstCauHinhNhomKham
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => long.Parse(x.Value)).Distinct().ToList();
            }

            return lstDichVuId;
        }
        #endregion

        #region Cập nhật 01/12/2022
        public (bool,bool) KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(long nguoiBenhId)
        {
            var coGoi = false;
            var coDichVuKhuyenMai = false;
            if (nguoiBenhId != 0)
            {
                var lstChuongTrinhId = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy
                                && ((x.BenhNhanId == nguoiBenhId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == nguoiBenhId && x.GoiSoSinh == true)))
                    .Select(x => x.ChuongTrinhGoiDichVuId).ToList();
                if (lstChuongTrinhId.Any())
                {
                    coGoi = true;
                    lstChuongTrinhId = lstChuongTrinhId.Distinct().ToList();
                    coDichVuKhuyenMai = _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository.TableNoTracking
                        .Any(x => lstChuongTrinhId.Contains(x.ChuongTrinhGoiDichVuId));
                    if (coDichVuKhuyenMai)
                    {
                        return (coGoi, coDichVuKhuyenMai);
                    }
                    else
                    {
                        coDichVuKhuyenMai = _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository.TableNoTracking
                            .Any(x => lstChuongTrinhId.Contains(x.ChuongTrinhGoiDichVuId));
                        return (coGoi, coDichVuKhuyenMai);

                        // khám bệnh không hiển thị dv giường
                        //if (coDichVuKhuyenMai)
                        //{
                        //    return coDichVuKhuyenMai;
                        //}
                        //else
                        //{
                        //    coDichVuKhuyenMai = _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongRepository.TableNoTracking
                        //        .Any(x => lstChuongTrinhId.Contains(x.ChuongTrinhGoiDichVuId));
                        //    return coDichVuKhuyenMai;
                        //}
                    }
                }
            }
            return (coGoi, coDichVuKhuyenMai);
        }

        public YeuCauDichVuKyThuat GetDichVuKyThuatDieuTriNgoaiTruTheoYeuCauKhamBenhId(long yeuCauKhamBenhId)
        {
            var result = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauKhamBenhId == yeuCauKhamBenhId
                        && x.DieuTriNgoaiTru == true && x.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan)
                        .OrderByDescending(x => x.Id).FirstOrDefault();
            if(result == null)
            {
                result = new YeuCauDichVuKyThuat();
            }
            return result;
        }

        public List<LookupItemVo> GetListTenICD(List<long> ids)
        {
            var results = _iCDRepository.TableNoTracking
                .Where(x => ids.Contains(x.Id))
                .Select(x => new LookupItemVo()
                {
                    KeyId = x.Id,
                    DisplayName = x.Ma + " - " + x.TenTiengViet
                })
                .ToList();
            return results;
        }
        #endregion
    }
}
