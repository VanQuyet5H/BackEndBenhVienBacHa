using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.Entities.XetNghiems;

namespace Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay
{
    public class BaoCaoLuuketQuaXeNghiemTrongNgayGridVo : GridItem
    {
        public int STT { get;set; }
        public string Sid { get; set; }
        public string HoVaTen { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDisplay => NamSinh != null ? NamSinh.Value.ToString() : "";
        public Enums.LoaiGioiTinh?  GioiTinh { get; set; }
        public string LoaiGioiTinhDisplay => GioiTinh != null ? GioiTinh.Value.GetDescription() : "";
        public long NoiChiDinhId { get; set; }

        public string NoiChiDinh => NoiChiDinhs != null
            ? string.Join(';', NoiChiDinhs.Where(o => !string.IsNullOrEmpty(o)).Distinct())
            : string.Empty;
        public IEnumerable<string> NoiChiDinhs { get; set; }
        public bool? BHYT { get; set; }
        public string BHYTDisplay => BHYT != null ? BHYT == true ? "X" : "" : "";
        public bool KhamSucKhoe { get; set; }
        public string KhamSucKhoeDisplay => KhamSucKhoe != null ? KhamSucKhoe == true ? "X" : "" : "";
        public string HoTenBacSi => HoTenBacSis != null
            ? string.Join(';', HoTenBacSis.Where(o => !string.IsNullOrEmpty(o)).Distinct())
            : string.Empty;
        public IEnumerable<string> HoTenBacSis { get; set; }
        public string ChanDoan => ChanDoans != null
            ? string.Join(';', ChanDoans.Where(o => !string.IsNullOrEmpty(o)).Distinct())
            : string.Empty;
        public IEnumerable<string> ChanDoans { get; set; }
        public string KetQua { get; set; }
    }

    public class KetQuaPhienXetNghiemChiTiet
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public string YeuCauDichVuKyThuatTen { get; set; }
        public int LanThucHien { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string NhomDichVuBenhVienTen { get; set; }
        public IEnumerable<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets { get; set; }
    }
}
