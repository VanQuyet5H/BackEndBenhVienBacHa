using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class ThongTinBacSiVo
    {
        //public long? YeuCauKhamBenhId { get; set; }

        public string BacSiChiDinh { get; set; }
        public long BacSiChiDinhId { get; set; }

        public string KhoaPhongChiDinh { get; set; }
        public bool FromLeTan { get; set; }

        public long PhienXetNghiemId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
    }

    public class PhieuInXetNghiemModel
    {
        public string Html { get; set; }
        //public bool FromLeTan { get; set; }

        //public ThongTinBacSiVo ThongTinBacSi { get; set; }
    }
    public class ThongTinBacSiTheoPhienVo
    {
        //public long? YeuCauKhamBenhId { get; set; }
        public ThongTinBacSiTheoPhienVo(){
            PhienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>();
        }
        public string BacSiChiDinh { get; set; }
        public long BacSiChiDinhId { get; set; }

        public string KhoaPhongChiDinh { get; set; }
        public bool FromLeTan { get; set; }
        public PhienXetNghiem PhienXetNghiem { get; set; }
        public List<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets { get; set; }
        public long PhienXetNghiemId { get; set; }
    }

    public class ThongTinPhienXNTheoBenhNhanVo :GridItem
    {
        public ThongTinPhienXNTheoBenhNhanVo()
        {
            PhienXetNghiemChiTietIds = new List<long>();
            YeuCauDichVuKyThuatIds = new List<long>();
            ThongTinBacSiTheoPhiens = new List<ThongTinBacSiTheoPhienVo>();
        }
       
        public List<long> PhienXetNghiemChiTietIds { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        public List<ThongTinBacSiTheoPhienVo> ThongTinBacSiTheoPhiens { get; set; }
    }

    public class ThongTinBSTheoTungPhienVo : GridItem
    {
        public ThongTinBSTheoTungPhienVo()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }

        public long YeuCauDichVuKyThuatId { get; set; }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        public long PhienXetNghiemId { get; set; }
        public List<ThongTinBacSiVo> ThongTinBacSiVo { get; set; }
    }

    public class ThongTinBenhNhanTungPhienVo : GridItem
    {
        public ThongTinBenhNhanTungPhienVo()
        {
            PhienXetNghiemChiTiets = new List<PhienXetNghiemChiTietVO>();
        }

        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string DiaChiDayDu { get; set; }
        public string DiaChi { get; set; }
        public string STT { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public bool? CoBHYT { get; set; }
        public int? BHYTMucHuong { get; set; }
        public string BHYTMaSoThe { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string BarCodeId { get; set; }
        public string GhiChu { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string SoPhieu { get; set; }

        public List<PhienXetNghiemChiTietVO> PhienXetNghiemChiTiets { get; set; }
    }
    public class PhienXetNghiemChiTietVO
    {
        public long Id { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public long? NhanVienChiDinhId { get; set; }
    }
    public class ThongTinNhomDichVuBenhVienVo
    {
        public string TenNhom { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
    }
}
