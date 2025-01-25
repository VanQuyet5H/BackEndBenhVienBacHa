using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Infrastructure;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.BackgroundJobs
{
    //BVHD-3754
    [ScopedDependency(ServiceType = typeof(IXacNhanKhongDuocBHYTChiTraJob))]
    public class XacNhanKhongDuocBHYTChiTraJob : IXacNhanKhongDuocBHYTChiTraJob
    {
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _ycDvKtRepository;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _ycDpBvRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _ycDvGiuongBHYTRepository;
        private readonly IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        private readonly IRepository<DonThuocThanhToan> _dtThanhToanRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _dtThanhToanChiTietRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _ycvtBv;
        private readonly ILoggerManager _logger;

        public XacNhanKhongDuocBHYTChiTraJob(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<YeuCauDichVuKyThuat> ycDvKtRepository,
            IRepository<YeuCauDuocPhamBenhVien> ycDpBvRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> ycDvGiuongBHYTRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<DonThuocThanhToan> dtThanhToanRepository,
            IRepository<DonThuocThanhToanChiTiet> dtThanhToanChiTietRepository,
            IRepository<YeuCauVatTuBenhVien> ycvtBv,
            ILoggerManager logger)
        {
            _logger = logger;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _ycDvKtRepository = ycDvKtRepository;
            _ycDpBvRepository = ycDpBvRepository;
            _ycDvGiuongBHYTRepository = ycDvGiuongBHYTRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _dtThanhToanRepository = dtThanhToanRepository;
            _dtThanhToanChiTietRepository = dtThanhToanChiTietRepository;
            _ycvtBv = ycvtBv;
        }

        public void Run()
        {
            //get yctn noi tru co the BHYT het han
            var yctns = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(o => o.LoaiYeuCauTiepNhan == Core.Domain.Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru && o.CoBHYT == true && o.NoiTruBenhAn != null && o.NoiTruBenhAn.DaQuyetToan != true && o.YeuCauTiepNhanTheBHYTs.All(t => t.NgayHetHan != null && t.NgayHetHan < DateTime.Today))
                .Select(o => new YeuCauTiepNhanThongTinTheBHYTData
                {
                    Id = o.Id,
                    YeuCauTiepNhanNgoaiTruCanQuyetToanId = o.YeuCauTiepNhanNgoaiTruCanQuyetToanId,
                    ThongTinTheBHYTDatas = o.YeuCauTiepNhanTheBHYTs.Select(t => new ThongTinTheBHYTData 
                        {
                            Id = t.Id,
                            MaSoThe = t.MaSoThe,
                            NgayHieuLuc = t.NgayHieuLuc,
                            NgayHetHan = t.NgayHetHan,
                            DuocGiaHanThe = t.DuocGiaHanThe,
                        }).ToList()
                }).ToList();
            var yctnIds = yctns.Select(o => o.Id).Concat(yctns.Where(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null).Select(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToanId.GetValueOrDefault())).ToList();

            //check yeuCauKhamBenh
            var dichVus = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && o.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = o.ThoiDiemChiDinh }).ToList();
            var dichVuCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVus)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update yeuCauKhamBenh
            if (dichVuCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _yeuCauKhamBenhRepository.Table.Where(o => dichVuCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _yeuCauKhamBenhRepository.Context.SaveChanges();
            }

            //check ycDvKt
            var dichVuKts = _ycDvKtRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = (o.NoiTruPhieuDieuTri != null ? o.NoiTruPhieuDieuTri.NgayDieuTri : o.ThoiDiemChiDinh) }).ToList();
            var dichVuKtCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVuKts)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuKtCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update ycDvKt
            if (dichVuKtCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _ycDvKtRepository.Table.Where(o => dichVuKtCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _ycDvKtRepository.Context.SaveChanges();
            }

            //check yeuCauTruyenMau
            var dichVuTruyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && o.TrangThai != EnumTrangThaiYeuCauTruyenMau.DaHuy && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = (o.NoiTruPhieuDieuTri != null ? o.NoiTruPhieuDieuTri.NgayDieuTri : o.ThoiDiemChiDinh) }).ToList();
            var dichVuTruyenMauCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVuTruyenMaus)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuTruyenMauCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update yeuCauTruyenMau
            if (dichVuTruyenMauCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _yeuCauTruyenMauRepository.Table.Where(o => dichVuTruyenMauCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _yeuCauTruyenMauRepository.Context.SaveChanges();
            }

            //check yeuCauDpBv
            var dichVuDpBvs = _ycDpBvRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = (o.NoiTruPhieuDieuTri != null ? o.NoiTruPhieuDieuTri.NgayDieuTri : o.ThoiDiemChiDinh) }).ToList();
            var dichVuDpBvCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVuDpBvs)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuDpBvCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update yeuCauDpBv
            if (dichVuDpBvCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _ycDpBvRepository.Table.Where(o => dichVuDpBvCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _ycDpBvRepository.Context.SaveChanges();
            }

            //check yeuCauVt
            var dichVuVtBvs = _ycvtBv.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = (o.NoiTruPhieuDieuTri != null ? o.NoiTruPhieuDieuTri.NgayDieuTri : o.ThoiDiemChiDinh) }).ToList();
            var dichVuVtBvCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVuVtBvs)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuVtBvCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update yeuCauVt
            if (dichVuVtBvCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _ycvtBv.Table.Where(o => dichVuVtBvCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _ycvtBv.Context.SaveChanges();
            }

            //check yeuCauDvGiuong
            var dichVuGiuongs = _ycDvGiuongBHYTRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && yctnIds.Contains(o.YeuCauTiepNhanId))
                .Select(o => new { o.Id, o.YeuCauTiepNhanId, NgayPhatSinh = o.NgayPhatSinh }).ToList();
            var dichVuGiuongCanXacNhanIds = new List<long>();
            foreach (var dichVu in dichVuGiuongs)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dichVuGiuongCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update yeuCauDvGiuong
            if (dichVuGiuongCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _ycDvGiuongBHYTRepository.Table.Where(o => dichVuGiuongCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null && dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _ycDvGiuongBHYTRepository.Context.SaveChanges();
            }

            //check dtThanhToanChiTiet
            var dtThanhToanChiTiets = _dtThanhToanChiTietRepository.TableNoTracking
                .Where(o => o.DuocHuongBaoHiem && o.BaoHiemChiTra == null && yctnIds.Contains(o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault()))
                .Select(o => new { o.Id, YeuCauTiepNhanId = o.DonThuocThanhToan.YeuCauTiepNhanId.GetValueOrDefault(), NgayPhatSinh = o.CreatedOn.GetValueOrDefault() }).ToList();
            var dtThanhToanChiTietCanXacNhanIds = new List<long>();
            foreach (var dichVu in dtThanhToanChiTiets)
            {
                var yctn = yctns.FirstOrDefault(o => o.Id == dichVu.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.YeuCauTiepNhanId);
                if (yctn != null && yctn.ThongTinTheBHYTDatas.Any())
                {
                    if (yctn.ThongTinTheBHYTDatas.All(o => o.NgayHetHan != null && o.NgayHetHan.Value.AddDays(o.DuocGiaHanThe == true ? 15 : 0).Date < dichVu.NgayPhatSinh.Date))
                    {
                        dtThanhToanChiTietCanXacNhanIds.Add(dichVu.Id);
                    }
                }
            }
            //update dtThanhToanChiTiet
            if (dtThanhToanChiTietCanXacNhanIds.Any())
            {
                var dichVuCanXacNhans = _dtThanhToanChiTietRepository.Table.Include(o => o.DonThuocThanhToan).Where(o => dtThanhToanChiTietCanXacNhanIds.Contains(o.Id)).ToList();
                foreach (var dichVu in dichVuCanXacNhans)
                {
                    if (dichVu.BaoHiemChiTra == null)
                    {
                        dichVu.MucHuongBaoHiem = 0;
                        dichVu.BaoHiemChiTra = false;
                        dichVu.ThoiDiemDuyetBaoHiem = DateTime.Now;
                        dichVu.NhanVienDuyetBaoHiemId = (long)NhanVienHeThong.NhanVienDuyetBHYTTuDong;
                        if (dichVu.YeuCauTiepNhanTheBHYTId == null || string.IsNullOrEmpty(dichVu.MaSoTheBHYT))
                        {
                            var thongTinTheBHYTData = yctns.FirstOrDefault(o => o.Id == dichVu.DonThuocThanhToan.YeuCauTiepNhanId || o.YeuCauTiepNhanNgoaiTruCanQuyetToanId == dichVu.DonThuocThanhToan.YeuCauTiepNhanId)?.ThongTinTheBHYTDatas.OrderBy(o => o.NgayHetHan).LastOrDefault();
                            if (thongTinTheBHYTData != null)
                            {
                                dichVu.YeuCauTiepNhanTheBHYTId = thongTinTheBHYTData.Id;
                                dichVu.MaSoTheBHYT = thongTinTheBHYTData.MaSoThe;
                            }
                        }
                    }
                }
                _dtThanhToanChiTietRepository.Context.SaveChanges();
            }
        }
    }
    public class YeuCauTiepNhanThongTinTheBHYTData
    {
        public long Id { get; set; }
        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }
        public List<ThongTinTheBHYTData> ThongTinTheBHYTDatas { get; set; }
    }
    public class ThongTinTheBHYTData
    {
        public long Id { get; set; }
        public string MaSoThe { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public bool? DuocGiaHanThe { get; set; }
    }
}