using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.BenhNhans
{
    [ScopedDependency(ServiceType = typeof(ITaiKhoanBenhNhanService))]
    public class TaiKhoanBenhNhanService : MasterFileService<TaiKhoanBenhNhan>, ITaiKhoanBenhNhanService
    {
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        public TaiKhoanBenhNhanService(IRepository<TaiKhoanBenhNhan> repository, IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository) : base(repository)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
        }
        //SoDuTaiKhoan -> SoTienDaTamUng - update thu ngan 02/03/2021
        //public async Task<decimal> SoDuTaiKhoan(long benhNhanId)
        //{
        //    if (benhNhanId == 0)
        //        return 0;
        //    var tk = BaseRepository.GetByIdFirstOrDefault(benhNhanId);
        //    var result = tk?.SoDuTaiKhoan ?? 0;
        //    return result.SoTienTuongDuong(0) ? 0 : result;
        //}

        public async Task<decimal> GetSoTienDaTamUngAsync(long yeuCauTiepNhanId)
        {
            var ycTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.TaiKhoanBenhNhanThus).Include(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToan).ThenInclude(o => o.TaiKhoanBenhNhanThus));

            var thuTamUng = ycTiepNhan.TaiKhoanBenhNhanThus
                .Concat(ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan?.TaiKhoanBenhNhanThus ?? new List<TaiKhoanBenhNhanThu>())
                .Where(o => o.DaHuy != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTamUng && o.ThuTienGoiDichVu != true && o.PhieuHoanUngId == null)
                .ToList();

            var soTienDaTamUng = thuTamUng.Select(o => o.TienMat.GetValueOrDefault(0) + o.ChuyenKhoan.GetValueOrDefault(0) + o.POS.GetValueOrDefault(0)).DefaultIfEmpty(0).Sum();
            return soTienDaTamUng;
        }
        /// <summary>
        /// Số tiền ứng lượng còn lại dành cho ngoại trú
        /// </summary>
        /// <param name="yeuCauTiepNhanId"></param>
        /// <returns></returns>
        public async Task<decimal> GetSoTienUocLuongConLai(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId,
                o => o.Include(x => x.BenhNhan).ThenInclude(x => x.TaiKhoanBenhNhan)
                .Include(x => x.YeuCauKhamBenhs)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Include(x => x.YeuCauDuocPhamBenhViens)
                .Include(x => x.YeuCauVatTuBenhViens)
                .Include(x => x.DonThuocThanhToans).ThenInclude(x => x.DonThuocThanhToanChiTiets));

            var soDuTk = await GetSoTienDaTamUngAsync(yeuCauTiepNhanId);
            soDuTk -= GetSoTienCanThanhToanNgoaiTru(yeuCauTiepNhan);
            
            //return (soDuTk.SoTienTuongDuong(0) || soDuTk < 0) ? 0 : soDuTk;

            //Cập nhật 27/07/2022: Update hiển thị số dư tài khoản cho phép âm
            return soDuTk;
        }

        public decimal GetSoTienCanThanhToanNgoaiTru(YeuCauTiepNhan yeuCauTiepNhan)
        {
            decimal soTienCanThanhToan = 0;
            soTienCanThanhToan += yeuCauTiepNhan.YeuCauKhamBenhs
                .Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham && o.KhongTinhPhi != true && o.YeuCauGoiDichVuId == null && o.GoiKhamSucKhoeId == null && o.CreatedOn != null)
                .Select(o => o.Gia - (o.BaoHiemChiTra != true ? 0 : (o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100)) - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault() - o.SoTienBenhNhanDaChi.GetValueOrDefault())
                .Sum(o => o < 0 ? 0 : o);
            soTienCanThanhToan += yeuCauTiepNhan.YeuCauDichVuKyThuats
                .Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && o.KhongTinhPhi != true && o.YeuCauGoiDichVuId == null && o.GoiKhamSucKhoeId == null && o.CreatedOn != null)
                .Select(o => o.Gia * o.SoLan - (o.BaoHiemChiTra != true ? 0 : (o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100)) * o.SoLan - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault() - o.SoTienBenhNhanDaChi.GetValueOrDefault())
                .Sum(o => o < 0 ? 0 : o);
            soTienCanThanhToan += yeuCauTiepNhan.YeuCauDuocPhamBenhViens
                .Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && o.KhongTinhPhi != true && o.YeuCauGoiDichVuId == null && o.CreatedOn != null)
                .Select(o => o.DonGiaBan * (decimal)o.SoLuong - (o.BaoHiemChiTra != true ? 0 : (o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100)) * (decimal)o.SoLuong - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault() - o.SoTienBenhNhanDaChi.GetValueOrDefault())
                .Sum(o => o < 0 ? 0 : o);
            soTienCanThanhToan += yeuCauTiepNhan.YeuCauVatTuBenhViens
                .Where(o => (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && o.KhongTinhPhi != true && o.YeuCauGoiDichVuId == null && o.CreatedOn != null)
                .Select(o => o.DonGiaBan * (decimal)o.SoLuong - (o.BaoHiemChiTra != true ? 0 : (o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100)) * (decimal)o.SoLuong - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault() - o.SoTienBenhNhanDaChi.GetValueOrDefault())
                .Sum(o => o < 0 ? 0 : o);
            soTienCanThanhToan += yeuCauTiepNhan.DonThuocThanhToans
                .Where(o => (o.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT) && (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan) && o.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy && o.CreatedOn != null)
                .SelectMany(o => o.DonThuocThanhToanChiTiets).Where(o => o.WillDelete == false)
                .Select(o => o.DonGiaBan * (decimal)o.SoLuong - (o.BaoHiemChiTra != true ? 0 : (o.DonGiaBaoHiem.GetValueOrDefault() * o.TiLeBaoHiemThanhToan.GetValueOrDefault() / 100 * o.MucHuongBaoHiem.GetValueOrDefault() / 100)) * (decimal)o.SoLuong - o.SoTienBaoHiemTuNhanChiTra.GetValueOrDefault() - o.SoTienMienGiam.GetValueOrDefault() - o.SoTienBenhNhanDaChi.GetValueOrDefault())
                .Sum(o => o < 0 ? 0 : o);
            return soTienCanThanhToan;
        }

        public async Task<bool> KiemTraConPhieuThuCongNo(long yeuCauTiepNhanId)
        {
            var ycTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauTiepNhanId,
                x => x.Include(o => o.TaiKhoanBenhNhanThus).Include(o => o.YeuCauTiepNhanNgoaiTruCanQuyetToan).ThenInclude(o => o.TaiKhoanBenhNhanThus));

            var phieuThuCongNos = ycTiepNhan.TaiKhoanBenhNhanThus
                .Concat(ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan?.TaiKhoanBenhNhanThus ?? new List<TaiKhoanBenhNhanThu>())
                .Where(o => o.DaHuy != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuTheoChiPhi && o.ThuTienGoiDichVu != true && o.CongNo.GetValueOrDefault() != 0)
                .ToList();

            var phieuTraNos = ycTiepNhan.TaiKhoanBenhNhanThus
                .Concat(ycTiepNhan.YeuCauTiepNhanNgoaiTruCanQuyetToan?.TaiKhoanBenhNhanThus ?? new List<TaiKhoanBenhNhanThu>())
                .Where(o => o.DaHuy != true && o.LoaiThuTienBenhNhan == Enums.LoaiThuTienBenhNhan.ThuNo)
                .ToList();
            var coPhieuThuCoCongNo = false;
            foreach (var phieuThu in phieuThuCongNos)
            {
                var soTienTraNo = phieuTraNos.Where(o => o.ThuNoPhieuThuId == phieuThu.Id).Select(o => o.TienMat.GetValueOrDefault() + o.ChuyenKhoan.GetValueOrDefault() + o.POS.GetValueOrDefault()).DefaultIfEmpty().Sum();
                if (!phieuThu.CongNo.GetValueOrDefault().AlmostEqual(soTienTraNo) && phieuThu.CongNo.GetValueOrDefault() > soTienTraNo)
                {
                    coPhieuThuCoCongNo = true;
                    break;
                }
            }

            return coPhieuThuCoCongNo;
        }
    }
}
