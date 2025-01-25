using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKSNK
{
    public class InPhieuDuyetLinhKSNK
    {
        public long YeuCauLinhVatTuId { get; set; }
        public string HostingName { get; set; }
        public bool HasHeader { get; set; }
    }

    public class ThongTinInPhieuLinhKSNKVo
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

    public class ThongTinInPhieuLinhKSNKChiTietVo
    {
        public long VatTuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string TenThuoc { get; set; }
        public string DVT { get; set; }
        public double SLYeuCau { get; set; }
        public double SLThucXuat { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string MaTN { get; set; }
    }
}
