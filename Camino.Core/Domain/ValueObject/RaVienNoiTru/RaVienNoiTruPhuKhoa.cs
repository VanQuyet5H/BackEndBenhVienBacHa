using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.RaVienNoiTru
{
    public class RaVienNoiTruPhuKhoa : CommonRaVien
    {
        public long YeuCauTiepNhanId { get; set; }

        public long? ChuanDoanNoiChuyenDenId { get; set; }
        public string TenChuanDoanNoiChuyenDen { get; set; }
        public string GhiChuChuanDoanNoiChuyenDen { get; set; }

        public long? ChuanDoanKKBCapCuuId { get; set; }
        public string TenChuanDoanKKBCapCuu { get; set; }
        public string GhiChuChuanDoanKKBCapCuu { get; set; }

        public long? NoiChuanDoanKhiVaoKhoaDieuTriId { get; set; }
        public string TenNoiChuanDoanKhiVaoKhoaDieuTri { get; set; }
        public string GhiChuNoiChuanDoanKhiVaoKhoaDieuTri { get; set; }

        public bool? TrieuChung { get; set; }
        public bool? DoPhauThuat { get; set; }
        public bool? DoGayMe { get; set; }
        public bool? DoNhiemKhau { get; set; }
        public bool? Khac { get; set; }

        public int? TongSoNgayDieuTriSauPT { get; set; }
        public int? TongSoLanPT { get; set; }

        public long? ChuanDoanRaVienId { get; set; }
        public string TenChuanDoanRaVien { get; set; }
        public string GhiChuChuanDoanRaVien { get; set; }
        public string NguyenNhan { get; set; }

        public List<ThongTinChuanDoanKemTheo> ChuanDoanKemTheos { get; set; }


        public long? ChuanDoanTruocPhauThuatId { get; set; }
        public string TenChuanDoanTruocPhauThuat { get; set; }
        public string GhiChuChuanDoanTruocPhauThuat { get; set; }

        public long? ChuanDoanSauPhauThuatId { get; set; }
        public string TenChuanDoanSauPhauThuat { get; set; }
        public string GhiChuChuanDoanSauPhauThuat { get; set; }
    } 
}
