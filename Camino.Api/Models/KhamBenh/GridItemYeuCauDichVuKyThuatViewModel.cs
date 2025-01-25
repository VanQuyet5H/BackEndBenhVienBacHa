using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class GridItemYeuCauDichVuKyThuatViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public int? SoLan { get; set; }
        public long? NguoiThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public bool IsUpdateLoaiGia { get; set; }
        public bool IsUpdateSoLan { get; set; }
        public bool IsUpdateNguoiThucHien { get; set; }
        public bool IsUpdateNoiThucHien { get; set; }
        public string BenhPhamXetNghiem { get; set; }
        public bool IsUpdateBenhPhamXetNghiem { get; set; }

        public byte[] YeuCauKhamBenhLastModified { get; set; }
        public bool IsKhamBenhDangKham { get; set; }

        public long? PhieuDieuTri { get; set; }
        public DateTime? GioBatDau { get; set; }
        public bool IsUpdateGioThucHien { get; set; }

        public bool? DuocHuongBaoHiem { get; set; }
        public bool IsUpdateDuocHuongBaoHiem { get; set; }

        public bool? LaDichVuTrongGoi { get; set; }
        public bool IsSwapDichVuGoi { get; set; }

        public bool IsKhamDoanTatCa { get; set; }

        public bool? IsDichVuKham { get; set; }

        //BVHD-3654
        public bool? TinhPhi { get; set; }
        public bool IsUpdateTinhPhi { get; set; }

        //BVHD-3825
        public bool? LaDichVuKhuyenMai { get; set; }
        public bool IsSwapDichVuKhuyenMai { get; set; }
    }
}
