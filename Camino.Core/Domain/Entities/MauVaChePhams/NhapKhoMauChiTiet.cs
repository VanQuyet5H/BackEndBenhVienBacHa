using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class NhapKhoMauChiTiet : BaseEntity
    {
        public long NhapKhoMauId { get; set; }
        public long YeuCauTruyenMauId { get; set; }
        public string MaTuiMau { get; set; }
        public long MauVaChePhamId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public Enums.PhanLoaiMau PhanLoaiMau { get; set; }
        public long TheTich { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public DateTime NgaySanXuat { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime? NgayLamXetNghiemHoaHop { get; set; }
        public long? NguoiLamXetNghiemHoaHopId { get; set; }
        public string NguoiLamXetNghiemHoaHop { get; set; }        
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGiaBan { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public DateTime NgayNhap { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemSotRet { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemGiangMai { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHCV { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHBV { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemHIV { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaPhanUngCheoOngI { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaPhanUngCheoOngII { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemMoiTruongMuoi { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiem37oCKhangGlubulin { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaXetNghiemNAT { get; set; }
        public string NguoiThucHien1 { get; set; }
        public string NguoiThucHien2 { get; set; }

        // cập nhật cho phép tự nhập kết quả xét nghiệm nào cần
        public string KetQuaXetNghiemKhac { get; set; }

        public virtual NhapKhoMau NhapKhoMau { get; set; }
        public virtual YeuCauTruyenMau YeuCauTruyenMau { get; set; }
        public virtual MauVaChePham MauVaChePham { get; set; }       

        private ICollection<XuatKhoMauChiTiet> _xuatKhoMauChiTiets;
        public virtual ICollection<XuatKhoMauChiTiet> XuatKhoMauChiTiets
        {
            get => _xuatKhoMauChiTiets ?? (_xuatKhoMauChiTiets = new List<XuatKhoMauChiTiet>());
            protected set => _xuatKhoMauChiTiets = value;
        }
    }
}
