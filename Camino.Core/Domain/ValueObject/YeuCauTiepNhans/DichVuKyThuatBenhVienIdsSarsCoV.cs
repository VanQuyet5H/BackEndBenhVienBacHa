using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class DichVuKyThuatBenhVienIdsSarsCoV
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public string TenDichVu { get; set; }
    }
    public class DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPham
    {
        public List<long> Ids { get; set; }
        public EnumLoaiMauXetNghiem LoaiMauXetNghiem { get; set; }
        public string LoaiMauXetNghiemText { get; set; }
    }
    public class DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem
    {
        public long PhienXetNghiemId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }

    }
    public class DichVuKyThuatBenhVienThuocXNGridVo
    {
        public DichVuKyThuatBenhVienThuocXNGridVo()
        {
            XetNghiemKhongThuocNhomSarsCov = new List<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem>();
            XetNghiemThuocNhomSarsCov = new List<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem>();
        }
        public List<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem> XetNghiemKhongThuocNhomSarsCov { get; set; }
        public List<DichVuKyThuatBenhVienIdsSarsCoVLoaiBenhPhamTheoPhienXetNghiem> XetNghiemThuocNhomSarsCov { get; set; }

    }
    public class InfoSarsCoVTheoYeuCauTiepNhan
    {
        public string BieuHienLamSang  { get; set; }
        public EnumLoaiMauXetNghiem LoaiMauXetNghiem { get; set; }
        public string DichTeSarsCoV2 { get; set; }
        public string LoaiMauXetNghiemText { get; set; }
    }
}
