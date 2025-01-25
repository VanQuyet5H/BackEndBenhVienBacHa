using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class HoSoChiTietDVKT
    {
        public string MaLienKet { get; set; }
        public int? STT { get; set; }
        public string MaDichVu { get; set; }
        public string MaVatTu { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi MaNhom { get; set; }//old:public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string GoiVatTuYTe { get; set; }
        public string TenVatTu { get; set; }
        public string TenDichVu { get; set; }
        public string DonViTinh { get; set; }
        public int PhamVi { get; set; }//old:public int? PhamVi { get; set; }
        public double SoLuong { get; set; }//old:public double? SoLuong { get; set; }
        public decimal DonGia { get; set; }//old:public decimal? DonGia { get; set; }
        public string ThongTinThau { get; set; }
        public int TyLeThanhToan { get; set; }//old:public int? TyLeThanhToan { get; set; }
        public decimal ThanhTien => Math.Round((decimal)SoLuong * DonGia * TyLeThanhToan / 100, 2);//old:public double? ThanhTien { get; set; }
        public decimal? TienThanhToanToiDa { get; set; }//old:public double? MucThanhToanToiDa { get; set; }
        public int MucHuong { get; set; }//old:public int? MucHuong { get; set; }
        public decimal? TienNguonKhac { get; set; }//old:public double? TienNguonKhac { get; set; }
        public decimal? TienBenhNhanTuTra { get; set; }//old:public double? TienBenhNhanThanhToan { get; set; }
        public decimal TienBaoHiemThanhToan =>
            Math.Round((ThanhTien - TienBenhNhanTuTra.GetValueOrDefault() - TienNguonKhac.GetValueOrDefault()) * MucHuong / 100, 2);//old:public double? TienBaoHiemThanhToan { get; set; }
        public decimal? TienBenhNhanCungChiTra =>
            ThanhTien - TienBenhNhanTuTra.GetValueOrDefault() - TienNguonKhac.GetValueOrDefault() - TienBaoHiemThanhToan;//old:public double? TienBenhNhanCungChiTra { get; set; }
        public decimal? TienNgoaiDinhSuat { get; set; }//old:public double? TienNgoaiDanhSach { get; set; }
        public string MaKhoa { get; set; }
        public string MaGiuong { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public DateTime NgayYLenh { get; set; }//old:public string NgayYLenh { get; set; }
                                                //old:public DateTime? NgayYLenhTime { get; set; }
        public DateTime? NgayKetQua { get; set; }//old:public string NgayKetQua { get; set; }
                                                //old:public DateTime? NgayKetQuaTime { get; set; }
        public Enums.EnumMaPhuongThucThanhToan MaPhuongThucThanhToan { get; set; }//old:public Enums.EnumMaPhuongThucThanhToan? MaPhuongThucThanhToan { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }//new
        public long? YeuCauKhamBenhId { get; set; }//new
        public long? YeuCauVatTuBenhVienId { get; set; }//new
    }
}
    