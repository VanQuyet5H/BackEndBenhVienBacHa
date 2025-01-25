using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhomDichVuBenhVien;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.XetNghiems
{
    public class MauXetNghiem : BaseEntity
    {
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public Enums.EnumLoaiMauXetNghiem LoaiMauXetNghiem { get; set; }
        public int SoLuongMau { get; set; }
        public string BarCodeId { get; set; }
        public int? BarCodeNumber { get; set; }
        public DateTime? ThoiDiemLayMau { get; set; }
        public long? PhongLayMauId { get; set; }
        public long? NhanVienLayMauId { get; set; }

        public long? PhieuGoiMauXetNghiemId { get; set; }
        public bool? DatChatLuong { get; set; }
        public long? NhanVienXetKhongDatId { get; set; }
        public DateTime? ThoiDiemXetKhongDat { get; set; }
        public string LyDoKhongDat { get; set; }
        public string GhiChu { get; set; }

        public DateTime? ThoiDiemNhanMau { get; set; }
        public long? PhongNhanMauId { get; set; }
        public long? NhanVienNhanMauId { get; set; }

        public virtual PhienXetNghiem PhienXetNghiem { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        public virtual PhongBenhVien PhongLayMau { get; set; }
        public virtual NhanVien NhanVienLayMau { get; set; }
        public virtual PhieuGoiMauXetNghiem PhieuGoiMauXetNghiem { get; set; }
        public virtual NhanVien NhanVienXetKhongDat { get; set; }
        public virtual PhongBenhVien PhongNhanMau { get; set; }
        public virtual NhanVien NhanVienNhanMau { get; set; }
    }
}
