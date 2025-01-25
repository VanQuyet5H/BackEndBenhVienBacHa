using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Core.Domain.ValueObject.KetQuaCLS
{
    public class KetQuaCLSGridVo : GridItem
    {
        public KetQuaCLSGridVo()
        {
            KetQuaCLSGridChiTietVos = new List<KetQuaCLSGridChiTietVo>();
        }
        public string XemKetQua { get; set; }
        public List<string> XemKetQuaList { get; set; }

        public string NoiDung { get; set; }
        public string NoiDungRemoveDictrict => NoiDung.ToLower().RemoveDiacritics();
        public string BacSiKetLuan { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public string BacSiKetLuanRemoveDictrict => BacSiKetLuan != null ? BacSiKetLuan.ToLower().RemoveDiacritics() : "";
        public string FileDinhKem { get; set; }

        public long? LoaiKetQuaId { get; set; }
        public string LoaiKetQuaCLS { get; set; }
        public string Type { get; set; }
        public string TenFilePDF { get; set; }
        public string LoaiDuoiTapTin { get; set; }
        public string TenGuid { get; set; }
        public List<KetQuaItemCLS> TenGuidList { get; set; }
        public string Ma { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public int LoaiTapTin { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public bool IsDisable { get; set; }
        public List<string> lisTen { get; set; }
        public bool KiemTraListPDFImage { get; set; }
        public int LoaiDichVuKyThuat { get; set; }

        public string NguoiThucHien { get; set; }
        public string NgayThucHien { get; set; }
        public string NgayKetLuan { get; set; }
        public string ChuanDoan { get; set; }
        public long PhienXetNghiemId { get; set; }

        public bool GoiPhienXetNghiemLai { get; set; }
        public bool TrangThaiXetNghiemLai { get; set; }

        public int LanThucHien { get; set; }
        public bool IsCheck { get; set; }
        public string DataKetQuaCLS { get; set; }
        public List<KetQuaCLSGridChiTietVo> KetQuaCLSGridChiTietVos { get; set; }
        public List<KQXetNghiemChiTiet> KQXetNghiemChiTiets { get; set; }
    }

    public class KetQuaCLSGridChiTietVo
    {
        public KetQuaCLSGridChiTietVo()
        {
            Items = new List<KetQuaCLSGridChiTietVo>();
        }

        public long Id { get; set; }
        public string TenDichVu { get; set; }
        public string KetQuaCu { get; set; }
        public string KetQuaMoi { get; set; }
        public string CSBT { get; set; }
        public string DonVi { get; set; }
        public string MayXN { get; set; }
        public string NguoiDuyet { get; set; }
        public string NgayDuyet { get; set; }

        public bool IsRoot { get; set; }
        public bool IsParent { get; set; }
        public bool Expanded { get; set; } = true;
        public bool? IsBold { get; set; }

        public long DichVuXetNghiemChaId { get; set; }
        public bool IsCheck { get; set; }
        public List<KetQuaCLSGridChiTietVo> Items { get; set; }
    }


    public class KetQuaItemCLS
    {
        public int LoaiTapTin { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public string TenFile { get; set; }
    }
    public class KetQuaListCLSGridVo : GridItem
    {
        public string XemKetQua { get; set; }
        public string NgayThucHien { get; set; }
        public string NoiDung { get; set; }
        public string BacSiKetLuan { get; set; }
        public string FileDinhKem { get; set; }
    }
    public class ListKetQuaCLS
    {
        public string KetQuaCLS { get; set; }
        public bool IsCheck { get; set; }
    }

    public class KQXetNghiemChiTiet : GridItem
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
        public bool? DaGoiDuyet { get; set; }

        public string DonVi { get; set; }

        public bool Duyet { get; set; }
        public bool? IsParent { get; set; }


        public string KetQuaCu => GiaTriCu;
        public string KetQuaMoi => !string.IsNullOrEmpty(GiaTriDuyet) ? GiaTriDuyet : !string.IsNullOrEmpty(GiaTriNhapTay) ? GiaTriNhapTay : !string.IsNullOrEmpty(GiaTriTuMay) ? GiaTriTuMay : string.Empty;



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
}
