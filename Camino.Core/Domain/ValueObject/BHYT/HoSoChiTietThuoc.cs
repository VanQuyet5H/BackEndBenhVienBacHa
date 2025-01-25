using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class HoSoChiTietThuoc
    {
        public string MaLienKet { get; set; }
        public int? STT { get; set; }
        public string MaThuoc { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi MaNhom { get; set; }//old:public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }
        public string LieuDung { get; set; }
        public string SoDangKy { get; set; }
        public string ThongTinThau { get; set; }
        public int PhamVi { get; set; }//old:public int? PhamVi { get; set; }
        public double SoLuong { get; set; }//old:public double? SoLuong { get; set; }
        public decimal DonGia { get; set; }//old:public decimal? DonGia { get; set; }
        public int TyLeThanhToan { get; set; }//old:public int? TyLeThanhToan { get; set; }
        public decimal ThanhTien => Math.Round((decimal) SoLuong * DonGia * TyLeThanhToan / 100, 2);//old:public double? ThanhTien { get; set; }
        public int MucHuong { get; set; }
        public decimal? TienNguonKhac { get; set; }
        public decimal? TienBenhNhanTuTra { get; set; }
        public decimal TienBaoHiemThanhToan =>
            Math.Round((ThanhTien - TienBenhNhanTuTra.GetValueOrDefault() - TienNguonKhac.GetValueOrDefault()) * MucHuong / 100, 2); //old:public decimal? TienBaoHiemTuTra { get; set; }
        public decimal? TienBenhNhanCungChiTra =>
            ThanhTien - TienBenhNhanTuTra.GetValueOrDefault() - TienNguonKhac.GetValueOrDefault() - TienBaoHiemThanhToan;  //old:public decimal? TienBenhNhanTuChiTra { get; set; }
        public decimal? TienNgoaiDinhSuat { get; set; }//old:public decimal? TienNgoaiDanhSach { get; set; }

        public string MaKhoa { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public DateTime NgayYLenh { get; set; }//old:public string NgayYLenh { get; set; }
                                               //old:public DateTime? NgayYLenhTime { get; set; }

        public Enums.EnumMaPhuongThucThanhToan MaPhuongThucThanhToan { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }//new
        public long? YeuCauDuocPhamBenhVienId { get; set; }//new
    }
}
