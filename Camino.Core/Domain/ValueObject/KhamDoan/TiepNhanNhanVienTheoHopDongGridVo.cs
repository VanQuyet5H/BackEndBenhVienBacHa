using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class TiepNhanNhanVienTheoHopDongTimKiemNangCapVo
    {
        public long CongTyId { get; set; }
        public long? HopDongId { get; set; }
        public string SearchString { get; set; }
        public TiepNhanNhanVienTheoHopDongTrangThaiTimKiemNangCapVo TrangThai { get; set; }
        public TinhTrangDoChiSoSinhTonTimKiemNangCapVo TinhTrangDoChiSoSinhTon { get; set; }
        public bool IsLichSu { get; set; }
        public bool IsDangKham { get; set; }
        public bool IsDangKhamVaDaKham { get; set; }

    }

    public class TiepNhanNhanVienTheoHopDongTrangThaiTimKiemNangCapVo
    {
        public bool ChuaKham { get; set; }
        public bool DangKham { get; set; }
        public bool DaKham { get; set; }
        public bool HuyKham { get; set; }
        public bool DichVuPhatSinh { get; set; }
    }

    public class TinhTrangDoChiSoSinhTonTimKiemNangCapVo
    {
        public bool ChuaDo { get; set; }
        public bool DaDo { get; set; }
    }

    public class TiepNhanNhanVienTheoHopDongGridVo : GridItem
    {
        public TiepNhanNhanVienTheoHopDongGridVo()
        {
            YeuCauTiepNhanIds = new List<long>();
        }
        public List<long> YeuCauTiepNhanIds { get; set; }
        public int? Stt { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string DonViBoPhan { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiTimKiem { get; set; }
        public string Email { get; set; }
        public string ChungMinhThu { get; set; }
        public string SHC { get; set; }
        public string DanToc { get; set; }
        public string TinhThanh { get; set; }
        public string NhomKham { get; set; }
        public string GhiChu { get; set; }
        public Enums.TinhTrangDoChiSoSinhTon TinhTrangDoChiSoSinhTon { get; set; }
        public string TinhTrangSoDoCSST => TinhTrangDoChiSoSinhTon.GetDescription();
        public long? YeuCauTiepNhanId { get; set; }      
        public string MaYeuCauTiepNhan { get; set; }
        public Enums.EnumTrangThaiYeuCauTiepNhan? TrangThaiYeuCauTiepNhan { get; set; }

        public Enums.EnumTrangThaiYeuCauKhamBenh TinhTrang
        {
            get
            {
                if (TrangThaiYeuCauTiepNhan == null || TrangThaiYeuCauTiepNhan == 0)
                {
                    return Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                }
                else if (TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHoanTat)
                {
                    return Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
                }
                else if (TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                {
                    return Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;
                }

                return Enums.EnumTrangThaiYeuCauKhamBenh.DangKham;
            }
        }
        public string TenTinhTrang => TinhTrang.GetDescription();
        //BVHD-3385
        public bool CoDichVuPhatSinh { get; set; }

        //BVHD-3707: bỏ HideCheckbox để dùng chung chức năng quay lại chưa khám
        //public bool HideCheckbox => TinhTrang != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham;

        public long? HopDongKhamSucKhoeNhanVienId { get; set; }

        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan?.ApplyFormatDateTimeSACH();
    }

    public class TiepNhanDichVuChiDinhVo : GridItem
    {
        public Enums.NhomDichVuChiDinhKhamSucKhoe LoaiDichVu
        {
            get
            {
                switch (LoaiDichVuKyThuat)
                {
                    case Enums.LoaiDichVuKyThuat.XetNghiem:
                        return Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
                    case Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh:
                        return Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
                    case Enums.LoaiDichVuKyThuat.ThamDoChucNang:
                        return Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
                    case null:
                        return Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                    default:
                        return Enums.NhomDichVuChiDinhKhamSucKhoe.KH;
                }
            }
        }

        public string TenNhomDichVu => LoaiDichVuKyThuat == null ? Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription() : LoaiDichVuKyThuat.GetDescription(); //LoaiDichVu.GetDescription();
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? LoaiGiaId { get; set; }
        public string TenLoaiGia { get; set; }
        public int? SoLan { get; set; }
        public decimal? DonGiaBenhVien { get; set; }
        public decimal? DonGiaMoi { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaChuaUuDai { get; set; }
        public decimal ThanhTien => (decimal)((GoiKhamSucKhoeId == null && DonGiaMoi != null && SoLan != null) ? DonGiaMoi * SoLan : 0);
        public long? NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public int TinhTrang { get; set; }
        public string TenTinhTrang { get; set; }
        public bool IsDichVuBatBuoc { get; set; }
        public bool LaDichVuKham => LoaiDichVuKyThuat == null; // LoaiDichVu == Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }

        public string TenGoiKhamSucKhoe { get; set; }
        public bool LaDichVuVacxin { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTimeSACH();


        //BVHD-3618
        public bool? LaGoiChung { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKyThuatNhanVienId { get; set; }
        // BVHD -3668
        public int NhomId { get; set; }

        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
    }

    public class TiepNhanDichVuChiDinhQueryVo
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public int? NamSinh { get; set; }
        public int? Tuoi => NgayThangNamSinh != null ? (DateTime.Now.Year - NgayThangNamSinh.Value.Year) : (NamSinh == null ? null : (DateTime.Now.Year - NamSinh));
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public Enums.TinhTrangHonNhan? TinhTrangHonNhan { get; set; }
        public bool DaLapGiaDinh => TinhTrangHonNhan == Enums.TinhTrangHonNhan.CoGiaDinh;
        public bool CoMangThai { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
    }

    public class DichVuChiDinhKhamSucKhoeBenhNhanQueryVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long GoiKhamSucKhoeId { get; set; }
        public bool? LaDichVuThem { get; set; }
    }

    public class TienSuBenhNhanVien
    {
        public long LoaiTienSuId { get; set; }
        public string LoaiTienSu { get; set; }
        public bool? BenhNgheNghiep { get; set; }
        public string TenBenh { get; set; }
        public DateTime? PhatHienNam { get; set; }
    }

    public class DichVuGoiChungXoaChuaBatDauKhamVo
    {
        public long? GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKyThuatNhanVienId { get; set; }
    }
}
