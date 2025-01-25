using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class InfoGiayChungNhanPhauThuatVo
    {
        public string SoLuuTru {get;set;}
        public string ChanDoan { get; set; }
        public string TinhTrangRaVienText { get; set; }
        public long TinhTrangRaVienId { get; set; }
        public DateTime? NgayThangNam { get; set; }
        public string NgayThangNamText { get; set; }
        public long? TruongKhoaId { get; set; }
        public string TruongKhoa { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public string GiamDocChuyenMon { get; set; }
        public long NoiTruHoSoKhacId { get; set; }

        public DateTime? PhauThuatNgay { get; set; }
        public string PhauThuatNgayText { get; set; }
        public long? PhuongThucVoCamId { get; set; }
        public string PhuongThucVoCamText { get; set; }
        public long? PhauThuatVienId { get; set; }
        public string PhauThuatVienText { get; set; }
        public string MaPhuongPhapPTTT { get; set; }
        public string PhuongThucPhauThuat { get; set; }
        public long DichVuPTTTId { get; set; }
        public string DichVuPTTTText { get; set; }

        public string KetQuaGPB { get; set; }

    }
    public class InfoYeuCauDichVuKyThuatTheoBenNhanVo
    {
        public InfoYeuCauDichVuKyThuatTheoBenNhanVo()
        {
            PhuongThucPhauThuats = new List<string>();
        }
        public long KeyId { get; set; }

        public string DisplayName { get; set; }
        public DateTime? PhauThuatNgay { get; set; }
        public long? PhuongThucVoCamId { get; set; }
        public string PhuongThucVoCamText { get; set; }
        public long? PhauThuatVienId { get; set; }
        public string PhauThuatVienText { get; set; }
        public string MaPhuongPhapPTTT { get; set; }
        public List<string> PhuongThucPhauThuats { get; set; }
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
    }
    public enum EnumTinhTrangRaVienHoSoKhac
    {
        [Description("Ra viện")]
        RaVien = 1,
        [Description("Chuyển viện")]
        ChuyenVien = 2,
        [Description("Trốn viện")]
        TronVien = 3,
        [Description("Xin ra viện")]
        XinRaVien = 4
    }
    public class InfoGiayChungNhanPhauThuat
    {
        public string ChungNhan { get; set; }
        public string DiaChi { get; set; }
        public string VaoVienNgay { get; set; }
        public string RaVienNgay { get; set; }
        public string SoLuuTru { get; set; }
        public string ChanDoanBenhNhan { get; set; }
        public string NhomMau { get; set; }
        public string YeuToRh { get; set; }

        public string PhauThuatNgay { get; set; }
        public string PhuongThucVoCam { get; set; }
        public string PhauThuatVien { get; set; }
        public string PhuongThucPhauThuat { get; set; }
        public string TinhTrangLucRaVien { get; set; }
        public string KetQuaGPB { get; set; }
        public string GiamDocCM { get; set; }
        public string TruongKhoa { get; set; }

        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NgayThangNamSinh { get; set; }

    }
    public class InfoThongTinRaVien
    {
        public string TenGiaPhauThuat { get; set; }
    }
    public class InfoThoiDiemRaVien { 
        public DateTime? ThoiGianRaVien { get; set; }
    }

}
