using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
    public class YeuCauLinhChiTietDuocPhamVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public bool isBHYT { get; set; }
    }

    public class InPhieuDuyetLinhDuocPham
    {
        public long YeuCauLinhDuocPhamId { get; set; }
        public string HostingName { get; set; }
        public bool HasHeader { get; set; }
    }

    public class ThongTinInPhieuLinhDuocPhamVo
    {
        public string HeaderPhieuLinhThuoc { get; set; }
        public string TenNguoiNhanHang { get; set; }
        public string BoPhan { get; set; }
        public string LyDoXuatKho { get; set; }
        public string XuatTaiKho { get; set; }
        public string DiaDiem { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DanhSachThuoc { get; set; }

    }

    public class ThongTinInPhieuLinhDuocPhamChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string TenThuoc { get; set; }
        public string DVT { get; set; }
        public double SLYeuCau { get; set; }
        public double SLThucXuat { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public bool LaDuocPhamHayVatTu { get; set; }
        //BVHD-3675
        public string MaTN { get; set; }
    }
}
