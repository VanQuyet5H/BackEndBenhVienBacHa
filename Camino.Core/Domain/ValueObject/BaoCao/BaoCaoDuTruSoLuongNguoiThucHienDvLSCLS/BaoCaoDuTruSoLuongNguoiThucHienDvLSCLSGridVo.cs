using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoDuTruSoLuongNguoiThucHienDvLSCLS
{
    public class BaoCaoDuTruSoLuongNguoiThucHienDvLSCLSGridVo:GridItem
    {
       public string TenDV { get; set; }
       public bool? GioiTinhNam { get; set; }
       public bool? GioiTinhNu { get; set; }
       public int SLNam {get; set; }
       public int SLNu { get; set; }
       public decimal Tong => SLNam + SLNu;
       public string NhomDichVu { get; set; }
       public string TenNhomDichVu => LoaiDichVuKyThuat == null ? Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription() : LoaiDichVuKyThuat.GetDescription(); //LoaiDichVu.GetDescription();
       public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
    }
    //public class ThongTinBenhNhan {
    //    public long? DichVuBenhVienId { get; set; }
    //    public string Ten { get; set; }
    //    public string TenNhomDichVu => LoaiDichVuKyThuat == null ? Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription() : LoaiDichVuKyThuat.GetDescription(); //LoaiDichVu.GetDescription();
    //    public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }

    //    public bool? GioiTinhNam { get; set; }
    //    public bool? GioiTinhNu { get; set; }
    //}
    public class TiepNhanDichVuChiDinhKhamDoanQueryVo
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public int? NamSinh { get; set; }
        public int? Tuoi => NgayThangNamSinh != null ? (DateTime.Now.Year - NgayThangNamSinh.Value.Year) : (NamSinh == null ? null : (DateTime.Now.Year - NamSinh));
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public Enums.TinhTrangHonNhan? TinhTrangHonNhan { get; set; }
        public bool DaLapGiaDinh => TinhTrangHonNhan == Enums.TinhTrangHonNhan.CoGiaDinh;
        public bool CoMangThai { get; set; }
        public long GoiKhamSucKhoeId { get; set; }
    }
}
