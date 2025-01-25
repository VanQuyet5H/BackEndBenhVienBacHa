using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject
{
    public class ListDanhSachCanLamSang
    {
        public DanhSachCanLamSang[] DanhSachCanLamSangs { get; set; }
    }
    public class DanhSachCanLamSangVo
    {
        public DanhSachCanLamSangVo()
        {
            DanhSachCanLamSangs = new List<DanhSachCanLamSang>();
            KetQuaNhomXetNghiems = new List<KetQuaNhomXetNghiemVo>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public List<DanhSachCanLamSang> DanhSachCanLamSangs { get; set; }
        public List<KetQuaNhomXetNghiemVo> KetQuaNhomXetNghiems { get; set; }
    }


    public class DanhSachCanLamSang
    {
        public DanhSachCanLamSang()
        {
            GiayKetQuaLamSang = new List<GiayKetQuaLamSang>();
        }

        public long DichVuKyhuatId { get; set; }
        public string TenDichVu { get; set; }
        public string GhiChu { get; set; }
        public int LoaiYeuCauKyThuat { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public List<GiayKetQuaLamSang> GiayKetQuaLamSang { get; set; }
    }

    public class KetQuaNhomXetNghiemVo
    {
        public KetQuaNhomXetNghiemVo()
        {
            GiayKetQuaNhomCanLamSang = new List<GiayKetQuaLamSang>();
            NhomDanhSachXetNghiem = new List<NhomDanhSachXetNghiem>();
        }
        public long NhomDichVuKyThuatId { get; set; }
        public string TenNhomDichVuKyhuat { get; set; }
        public string KetLuan { get; set; }
        public bool CapNhatChuaThanhToan { get; set; }

        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }

        public List<GiayKetQuaLamSang> GiayKetQuaNhomCanLamSang { get; set; }
        public List<NhomDanhSachXetNghiem> NhomDanhSachXetNghiem { get; set; }

    }

    public class NhomDanhSachXetNghiem
    {
        public long DichVuId { get; set; }
        public string TenDichVu { get; set; }

        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }

        public bool CapNhatChuaThanhToan { get; set; }
    }

    public class GiayKetQuaLamSang
    {
        public long Id { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenGuid { get; set; }
        public List<string> TenGuidList { get; set; }

        public string DuongDan { get; set; }

        public long KichThuoc { get; set; }

        public int LoaiTapTin { get; set; }

        public string MoTa { get; set; }
    }

}