using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NhomDichVuBenhVien
{
    public class DanhSachNhomDichVuThuongDung
    {
        public string Nhom { get; set; }
        public long GoiDichVuId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaBenhVien { get; set; }

        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string LoaiGia { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public double ThanhTien => SoLuong * (double)DonGia;
    }
}
