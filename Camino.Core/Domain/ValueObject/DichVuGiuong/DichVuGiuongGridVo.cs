using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DichVuGiuongThongTinGias;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;

namespace Camino.Core.Domain.ValueObject.DichVuGiuong
{
    public class DichVuGiuongGridVo : GridItem
    {
        public string Ma { get; set; }
        public long? DichVuGiuongId { get; set; }
        public string MaTT37 { get; set; }
        public string Ten { get; set; }
        public string TenNoiThucHien { get; set; }
        public long? KhoaId { get; set; }
        public string Khoa { get; set; }
        public Enums.HangBenhVien HangBenhVien { get; set; }
        public Enums.EnumLoaiGiuong LoaiGiuong { get; set; }
        public string LoaiGiuongDisplay { get; set; }
        public string HangBenhVienDisplay { get; set; }
        public string MoTa { get; set; }
        public long? DichVuGiuongThongTinGiaId { get; set; }
        public string HieuLucHienThi { get; set; }
        public List<DichVuGiuongThongTinGia> ListDichVuGiuongThongTinGia { get; set; }

        public bool AnhXa { get; set; }

        public string TLTT { get; set; }
        public string GiaBaoHiems { get; set; }
        public string GiaThuongBenhViens { get; set; }

        public ICollection<DichVuGiuongBenhVienGiaBaoHiem> DichVuGiuongBenhVienGiaBaoHiems { get; set; }
        public ICollection<DichVuGiuongBenhVienGiaBenhVien> DichVuGiuongBenhVienGiaBenhViens { get; set; }
    }

    public class JsonDichVuGiuong
    {
        public string SearchString { get; set; }
        public bool? AnhXa { get; set; }
        public bool? HieuLuc { get; set; }
    }

    public class DichVuGiuongBenhVienJSON
    {
        public long? DichVuGiuongBenhVienId { get; set; }    }

    
}
