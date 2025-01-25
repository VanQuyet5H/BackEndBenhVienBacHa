using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;

namespace Camino.Api.Models.NhapKhoMau
{
    public class PhieuNhapKhoMauChiTietViewModel : BaseViewModel
    {
        public PhieuNhapKhoMauChiTietViewModel()
        {
            MaTuiMauDangNhaps = new List<string>();
            YeuCauTruyenMauIdDangChons = new List<long>();
            KetQuaXetNghiemKhacs = new List<KetQuaXetNghiemKhac>();
        }
        //public long? NhapKhoMauId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public string TenBenhNhanTruyenMau { get; set; }
        public LookupItemYeuCauTruyenMauVo ThongTinYeuCauTruyenMau { get; set; }
        public string MaTuiMau { get; set; }
        public long? MauVaChePhamId { get; set; }
        public string TenMauVaChePham { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public Enums.PhanLoaiMau? PhanLoaiMau { get; set; }
        public long? TheTich { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public string TenNhomMau =>  NhomMau != null ? "\""+ NhomMau.GetDescription() + "\"" +  (YeuToRh != null ? " Rh("+ TenYeuToRh + ")" : "") : null;
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public string TenYeuToRh => YeuToRh.GetDescription();
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? HanSuDung { get; set; }
        public DateTime? NgayLamXetNghiemHoaHop { get; set; }
        public long? NguoiLamXetNghiemHoaHopId { get; set; }
        public string NguoiLamXetNghiemHoaHop { get; set; }
        public DateTime? NgayNhap { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemSotRet { get; set; }
        public string TenKetQuaXetNghiemSotRet => KetQuaXetNghiemSotRet.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemGiangMai { get; set; }
        public string TenKetQuaXetNghiemGiangMai => KetQuaXetNghiemGiangMai.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHCV { get; set; }
        public string TenKetQuaXetNghiemHCV => KetQuaXetNghiemHCV.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHBV { get; set; }
        public string TenKetQuaXetNghiemHBV => KetQuaXetNghiemHBV.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHIV { get; set; }
        public string TenKetQuaXetNghiemHIV => KetQuaXetNghiemHIV.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaPhanUngCheoOngI { get; set; }
        public string TenKetQuaPhanUngCheoOngI => KetQuaPhanUngCheoOngI.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaPhanUngCheoOngII { get; set; }
        public string TenKetQuaPhanUngCheoOngII => KetQuaPhanUngCheoOngII.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemMoiTruongMuoi { get; set; }
        public string TenKetQuaXetNghiemMoiTruongMuoi => KetQuaXetNghiemMoiTruongMuoi.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiem37oCKhangGlubulin { get; set; }
        public string TenKetQuaXetNghiem37oCKhangGlubulin => KetQuaXetNghiem37oCKhangGlubulin.GetDescription();
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemNAT { get; set; }
        public string TenKetQuaXetNghiemNAT => KetQuaXetNghiemNAT.GetDescription();
        public string NguoiThucHien1 { get; set; }
        public string NguoiThucHien2 { get; set; }

        public List<KetQuaXetNghiemKhac> KetQuaXetNghiemKhacs { get; set; }

        public List<string> MaTuiMauDangNhaps { get; set; }
        public List<long> YeuCauTruyenMauIdDangChons { get; set; }

        public virtual PhieuNhapKhoMauViewModel NhapKhoMau { get; set; }
    }

    public class KetQuaXetNghiemKhac
    {
        public Enums.LoaiXetNghiemMauNhapThem? LoaiXetNghiem { get; set; }
        public string TenLoaiXetNghiem { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQua { get; set; }
        public string HienThiKetQuaKhac => LoaiXetNghiem.GetDescription() + " (" + KetQua.GetDescription() + ")";
    }
}
