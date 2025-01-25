using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;


namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ma4350 { get; set; }
        public long DichVuKyThuatId { get; set; }
        public string Ten { get; set; }
        public string TenKhoa { get; set; }
        public string NgayBatDauHienThi { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string ThongTu { get; set; }
        public string NghiDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public long? SoMayTT { get; set; }
        public long? SoMayCBCM { get; set; }
        public long? ThoiGianThucHien { get; set; }
        public long? SoCaCP { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public string HieuLucHienThi { get; set; }
        //public string MaGia { get; set; }
        //public string TenGia { get; set; }
        //public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        //public string NhomChiPhiDisplay { get; set; }
        //public string NhomDichVuKyThuatDisplay { get; set; }
        //public Enums.LoaiPhauThuatThuThuat LoaiPhauThuatThuThuat { get; set; }
        //public string LoaiPhauThuatThuThuatDisplay { get; set; }
        public string TenNoiThucHien { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public string LoaiPhauThuatThuThuat { get; set; }
        public Enums.EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }
        public string LoaiMauXetNghiemText => LoaiMauXetNghiem.GetDescription();
        public bool? DichVuCoKetQuaLau { get; set; }

        public ICollection<DichVuKyThuatBenhVienGiaBaoHiem> DichVuKyThuatBenhVienGiaBaoHiems { get; set; }
        public ICollection<DichVuKyThuatBenhVienGiaBenhVien> DichVuKyThuatBenhVienGiaBenhViens { get; set; }

        public IEnumerable<Camino.Core.Domain.Entities.KhoaPhongs.KhoaPhong> KhoaPhongs { get; set; }
        public IEnumerable<Camino.Core.Domain.Entities.KhoaPhongs.KhoaPhong> KhoaPhongThucHiens { get; set; }
        public IEnumerable<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> PhongBenhViens { get; set; }

        public IEnumerable<Camino.Core.Domain.Entities.PhongBenhViens.PhongBenhVien> KhoaPhongThucHienUuTiens { get; set; }

        public string TenNoiThucHienUuTien { get; set; }

        public string Khoas { get; set; }
        public string GiaThuongBenhVien { get; set; }
        public string GiaBHYT { get; set; }
        public string TiLeBaoHiemThanhToan { get; set; }
        public bool AnhXa { get; set; }
        public string TenKyThuat { get; set; }
        public string ChuyenKhoaChuyenNganh { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int? SoPhimXquang { get; set; }
    }

    public class DichVuKyThuatBenhVienChildrenGridVo : GridItem
    {

        public decimal Gia { get; set; }
        public string GiaDisplay { get; set; }
        public string GiaHienThi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public string TuNgayDisplay { get; set; }
        public string DenNgayDisplay { get; set; }
        public int TiLeThanhToan { get; set; }
    }
    public class DichVuKyThuatBenhVienChildrenGiaBenhVienGridVo : GridItem
    {
        public string LoaiGia { get; set; }
        public decimal Gia { get; set; }
        public string GiaDisplay { get; set; }
        public string GiaHienThi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public string TuNgayDisplay { get; set; }
        public string DenNgayDisplay { get; set; }
    }

    public class DichVuKyThuatBenhVienJSON
    {
        public long DichVuKyThuatBenhVienId { get; set; }
    }
    public class JsonDichVuKyThuatBenhVien
    {
        public long? NhomDichVuBenhVienId { get; set; }
        public bool? AnhXa { get; set; }
        public bool? HieuLuc { get; set; }
        public string SearchString { get; set; }
    }

}