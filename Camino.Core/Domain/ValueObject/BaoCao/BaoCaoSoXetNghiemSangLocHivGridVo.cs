using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoSoXetNghiemSangLocHivGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaBN { get; set; }
        public string MaBarcode { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string KhoaPhong { get; set; }
        public string DoiTuong { get; set; }
        public string NgayLayMau { get; set; }
        public string NgayXetNghiem { get; set; }
        public string KetQua { get; set; }
        public string KetQuaKhangDinh { get; set; }
        public string GhiChu { get; set; }
    }
    public class BaoCaoSoXetNghiemTheoDichVuVo : GridItem
    {
        public string TenDichVuKyThuat { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string Khoa { get; set; }
        public string Phong { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public List<KetQuaPhienXetNghiemChiTietVo> KetQuaPhienXetNghiemChiTietVos { get; set; }
        public string KetQua { get; set; }
    }
}