using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinChiTietKhamChuaBenhCoBHYTVo : GridItem
    {
        public string NoiDung { get; set; }

        public string DonViTinh { get; set; }

        public decimal? SoLuong { get; set; }    

        public decimal? DonGiaBH { get; set; }
        public decimal? DonGiaBV => DonGiaBH;

        public decimal? TiLeThanhToanTheoDV { get; set; }

        public decimal? ThanhTienBV => ThanhTienBH;

        public decimal? TiLeThanhToanBHYT { get; set; }

        public decimal? ThanhTienBH => SoLuong * DonGiaBH;

        public decimal? QuyBHYT { get; set; }

        public decimal? NguoiBenhCungChiTra => ThanhTienBH.GetValueOrDefault() - QuyBHYT.GetValueOrDefault();

        public decimal? Khac => 0;

        public decimal? NguoiBenhTuTra => 0;

        public bool? BaoHiemChiTra { get; set; }

        public bool? DuocHuongBaoHiem { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }

        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }

        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }

        public Enums.EnumYeuCauDuocPhamBenhVien TrangThaiDuocPham { get; set; }

        public Enums.EnumTrangThaiGiuongBenh TrangThaiGiuongBenh { get; set; }

        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }

        public NhomChiPhiKhamChuaBenh Nhom { get; set; }
    }

    public class ThongTinChiTietKhamChuaBenhVo : GridItem
    {
        public string NoiDung { get; set; }

        public string DonViTinh { get; set; }

        public decimal? SoLuong { get; set; }

        public decimal? DonGiaBV { get; set; }

        public decimal? DonGiaBH { get; set; }

        public decimal? TiLeThanhToanTheoDV { get; set; }

        public decimal? ThanhTienBV { get; set; }

        public decimal? TiLeThanhToanBHYT { get; set; }

        public decimal? ThanhTienBH => SoLuong * DonGiaBH;

        public decimal? QuyBHYT { get; set; }

        public decimal? NguoiBenhCungChiTra => ThanhTienBH.GetValueOrDefault() - QuyBHYT.GetValueOrDefault();

        public decimal? Khac => 0;

        public decimal? NguoiBenhTuTra => ThanhTienBV.GetValueOrDefault() - ThanhTienBH.GetValueOrDefault();

        public bool? BaoHiemChiTra { get; set; }

        public bool? DuocHuongBaoHiem { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
     
        public NhomChiPhiKhamChuaBenh Nhom { get; set; }
    }



    public class BangKeKhamBenhCoBHYTVo : GridItem
    {
        public string NoiDung { get; set; }

        public string DonViTinh { get; set; }

        public decimal? SoLuong { get; set; }

        public decimal? DonGiaBH { get; set; }

        //BVHD-3792: Cột đơn giá BV cập nhật về giá BV
        public decimal? DonGiaBV => DonGiaTheoBV;
        public decimal? DonGiaTheoBV { get; set; }

        public decimal? TiLeThanhToanTheoDV { get; set; }

        public decimal? ThanhTienBV => SoLuong.GetValueOrDefault() * DonGiaBV.GetValueOrDefault();

        public decimal? MucHuongBaoHiem { get; set; }

        public decimal? ThanhTienBH => SoLuong * DonGiaBH;

        public decimal DonGiaBHYTThanhToan => DonGiaBH.GetValueOrDefault() * TiLeThanhToanTheoDV.GetValueOrDefault() / 100 * MucHuongBaoHiem.GetValueOrDefault() / 100;

        public decimal QuyBHYT => SoLuong.GetValueOrDefault() * DonGiaBHYTThanhToan;

        public decimal? NguoiBenhCungChiTra => ThanhTienBH.GetValueOrDefault() - QuyBHYT;

        public decimal? Khac => 0;

        public decimal? NguoiBenhTuTra => 0;

        public bool? BaoHiemChiTra { get; set; }

        public bool? DuocHuongBaoHiem { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }


        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }

        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }

        public Enums.EnumYeuCauDuocPhamBenhVien TrangThaiDuocPham { get; set; }

        public Enums.EnumTrangThaiGiuongBenh TrangThaiGiuongBenh { get; set; }

        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }

        public NhomChiPhiBangKe Nhom { get; set; }
        public int? SoGoiVatTu { get; set; }
    }

    public class BangKeKhamBenhBenhVienVo : GridItem
    {
        public string NoiDung { get; set; }

        public string DonViTinh { get; set; }

        public decimal? SoLuong { get; set; }

        public decimal? DonGiaBV { get; set; }

        public decimal? DonGiaBH { get; set; }

        public decimal? TiLeThanhToanTheoDV { get; set; }

        public decimal? ThanhTienBV => SoLuong * DonGiaBV;

        public decimal? MucHuongBaoHiem { get; set; }

        public decimal? ThanhTienBH => SoLuong * DonGiaBH;

        public decimal DonGiaBHYTThanhToan => DonGiaBH.GetValueOrDefault() * TiLeThanhToanTheoDV.GetValueOrDefault() / 100 * MucHuongBaoHiem.GetValueOrDefault() / 100;

        public decimal QuyBHYT => SoLuong.GetValueOrDefault() * DonGiaBHYTThanhToan;

        public decimal? NguoiBenhCungChiTra => MucHuongBaoHiem.GetValueOrDefault() > 0? (ThanhTienBH.GetValueOrDefault() - QuyBHYT) : 0;

        public decimal? Khac { get; set; }

        public decimal? NguoiBenhTuTra => (MucHuongBaoHiem.GetValueOrDefault() > 0 ? (ThanhTienBV.GetValueOrDefault() - ThanhTienBH.GetValueOrDefault()) : ThanhTienBV.GetValueOrDefault()) - Khac.GetValueOrDefault();

        public bool? BaoHiemChiTra { get; set; }

        public bool DuocHuongBaoHiem { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }

        public NhomChiPhiBangKe Nhom { get; set; }
        public int? SoGoiVatTu { get; set; }
    }
}
