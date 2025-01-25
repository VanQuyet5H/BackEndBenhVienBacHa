using System;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ThongTinHoSoGetInfo : GridItem
    {
        public string ThongTinHoSo { get; set; }

        public DateTime NgayHoiChan { get; set; }
        public int LoaiHoSoDieuTriNoiTruId { get; set; }
        public string LoaiHoSoDieuTriNoiTruTen { get; set; }
    }

    public class ThongTinFileDinhKem : GridItem
    {
        public string TenGuid { get; set; }

        public string Uid { get; set; }

        public string DuongDanTmp { get; set; }
    }

    public class DuocPhamTheoKhoThuocTuTrucTheoBenhNhanTemplateVo :GridItem
    {
        public long KeyId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }
        public string SoLoSX { get; set; }

        public string NuocSX { get; set; }
        public string HamLuong { get; set; }

    }
    public class KetQuaTemplateVo
    {
        public long KeyId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }
        public string SoLoSX { get; set; }

        public string NuocSX { get; set; }
        public string HamLuong { get; set; }

    }
    public enum KetQuaPhanUngThuocNoiTruHoSoKhac
    {
        [Description("Âm tính")]
        AmTinh = 1,
        [Description("Dương tính")]
        DuongTinh = 2,
        [Description("Nghi ngờ")]
        NghiNgo = 3
    }
    public class PhanUngThuocVaServicesHttpParams
    {
        public long YeuCauTiepNhanId { get; set; }

        public string HostingName { get; set; }

        public bool? Header { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
    }
}
