using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class ThongTinBenhNhanPhauThuatQueryInfo
    {
        public long PhongBenhVienId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class ThongTinBenhNhanLookupItemVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string MaBA { get; set; }
    }

    public class BaoCaoThuocVaVatTuPhauThuatQueryInfoVo : QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long PhongBenhVienId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }

        //BVHD-3860
        public PhiBangKeThuocVo BangKeThuocPhi { get; set; }
    }

    public class DanhSachThuocVaVatTuPhauThuat : GridItem
    {
        public bool LaThuocVatTuBHYT { get; set; }
        public string Loai => LaThuocVatTuBHYT ? "BHYT" : "Viện phí";
        public string Nhom { get; set; }

        public string TenDichVu { get; set; }       
        public string HamLuongNoiSanXuat { get; set; }
        public string DonViTinh { get; set; }
        public bool KhongTinhPhi { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => KhongTinhPhi ? 0 : (decimal)SoLuong * DonGia;
        public EnumGiaiDoanPhauThuat? GiaiDoanPhauThuat { get; set; }
        public string TenGiaiDoanPhauThuat => GiaiDoanPhauThuat?.GetDescription();
    }

    public class PhiBangKeThuocVo
    {
        public bool? TinhPhi { get; set; }
        public bool? KhongTinhPhi { get; set; }
    }
}
