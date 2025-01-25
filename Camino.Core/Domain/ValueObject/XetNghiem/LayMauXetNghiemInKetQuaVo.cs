using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{

    public class LayPhieuXetNghiemTheoYCKTVaNhomDVBVVo
    {
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? phieuDieuTriHienTaiId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
    }

    public class LayMauXetNghiemInKetQuaVo
    {
        public LayMauXetNghiemInKetQuaVo()
        {
            NhomDichVuBenhVienIds = new List<long>();
        }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? PhienXetNghiemId { get; set; }
        public List<long> NhomDichVuBenhVienIds { get; set; }
        public long? LoaiIn { get; set; }
        public List<DuyetKqXetNghiemChiTietModel> ListIn { get; set; }
    }
    public class DuyetKqXetNghiemChiTietModel : GridItem
    {
        public int LoaiKetQuaTuMay { get; set; } = 1;

        public long NhomDichVuBenhVienId { get; set; }

        public string Ten { get; set; }

        public long YeuCauDichVuKyThuatId { get; set; }

        public string GiaTriCu { get; set; }

        public string GiaTriTuMay { get; set; }

        public string GiaTriNhapTay { get; set; }

        public string GiaTriDuyet { get; set; }

        public bool? ToDamGiaTri { get; set; }

        public string Csbt { get; set; }

        public string DonVi { get; set; }

        public bool Duyet { get; set; }

        public DateTime? ThoiDiemGuiYeuCau { get; set; }

        public string ThoiDiemGuiYeuCauDisplay => ThoiDiemGuiYeuCau != null ? (ThoiDiemGuiYeuCau ?? DateTime.Now).ApplyFormatDateTime() : "";

        public DateTime? ThoiDiemNhanKetQua { get; set; }

        public string ThoiDiemNhanKetQuaDisplay => ThoiDiemNhanKetQua != null ? (ThoiDiemNhanKetQua ?? DateTime.Now).ApplyFormatDateTime() : "";

        public long? MayXetNghiemId { get; set; }

        public string TenMayXetNghiem { get; set; }

        public DateTime? ThoiDiemDuyetKetQua { get; set; }

        public string ThoiDiemDuyetKetQuaDisplay => ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : "";

        public string NguoiDuyet { get; set; }

        public string LoaiMau { get; set; }

        public long DichVuXetNghiemId { get; set; }

        public List<long> IdChilds { get; set; } = new List<long>();

        public int Level { get; set; }

        public List<string> DanhSachLoaiMau { get; set; }

        public List<string> DanhSachLoaiMauDaCoKetQua { get; set; }

        public List<string> DanhSachLoaiMauKhongDat { get; set; }

        public bool? YeuCauChayLai { get; set; }

        public bool? DaDuyet { get; set; }

        public string NguoiYeuCau { get; set; }

        public string NgayYeuCauDisplay { get; set; }

        public string LyDoYeuCau { get; set; }

        public string NguoiDuyetChayLai { get; set; }

        public string NgayDuyetDisplay { get; set; }

        public string Nhom { get; set; }

        public long NhomId { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }
    }
    public class ListOBJInXetNghiem
    {
        public string html { get; set; }
        public string GhiChu { get; set; }
        public string Ngay { get; set; }

        public string Thang { get; set; }

        public string Nam { get; set; }
        public string Gio { get; set; }
        public string NguoiThucHien { get; set; }

    }
}
