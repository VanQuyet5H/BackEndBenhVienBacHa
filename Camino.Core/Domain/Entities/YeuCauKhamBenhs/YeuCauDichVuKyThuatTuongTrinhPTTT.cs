using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.PhuongPhapVoCams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuKyThuatTuongTrinhPTTT : BaseEntity
    {
        public long? ICDTruocPhauThuatId { get; set; }

        public long? ICDSauPhauThuatId { get; set; }

        public string GhiChuICDTruocPhauThuat { get; set; }

        public string GhiChuICDSauPhauThuat { get; set; }

        public string MaPhuongPhapPTTT { get; set; }

        public string TenPhuongPhapPTTT { get; set; }

        public Enums.LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }

        public long? PhuongPhapVoCamId { get; set; }

        public Enums.EnumTinhHinhPhauThuatThuThuat? TinhHinhPTTT { get; set; }

        public Enums.EnumTaiBienPTTT? TaiBienPTTT { get; set; }

        public Enums.EnumTuVongPTTTTheoNgay? TuVongTrongPTTT { get; set; }

        //public Enums.EnumTuVongPTTTTheoNgay? TuVongTrongPTTT { get; set; }

        public Enums.EnumThoiGianTuVongPTTTTheoNgay? KhoangThoiGianTuVong { get; set; }

        public DateTime? ThoiDiemPhauThuat { get; set; }
        
        public DateTime? ThoiGianBatDauGayMe { get; set; }
        
        public DateTime? ThoiDiemKetThucPhauThuat { get; set; }

        public string TrinhTuPhauThuat { get; set; }

        public long? NhanVienTuongTrinhId { get; set; }
        public DateTime? ThoiDiemKetThucTuongTrinh { get; set; }
        public bool? KhongThucHien { get; set; }
        public string LyDoKhongThucHien { get; set; }

        //BVHD-3877
        public string GhiChuCaPTTT { get; set; }

        public virtual NhanVien NhanVienTuongTrinh { get; set; }

        public virtual ICD ICDTruocPhauThuat { get; set; }

        public virtual ICD ICDSauPhauThuat { get; set; }

        public virtual PhuongPhapVoCam PhuongPhapVoCam { get; set; }

        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }

        private ICollection<YeuCauDichVuKyThuatLuocDoPhauThuat> _yeuCauDichVuKyThuatLuocDoPhauThuats;

        public virtual ICollection<YeuCauDichVuKyThuatLuocDoPhauThuat> YeuCauDichVuKyThuatLuocDoPhauThuats
        {
            get => _yeuCauDichVuKyThuatLuocDoPhauThuats ?? (_yeuCauDichVuKyThuatLuocDoPhauThuats = new List<YeuCauDichVuKyThuatLuocDoPhauThuat>());
            protected set => _yeuCauDichVuKyThuatLuocDoPhauThuats = value;
        }

        private ICollection<PhauThuatThuThuatEkipBacSi> _phauThuatThuThuatEkipBacSis;
        public virtual ICollection<PhauThuatThuThuatEkipBacSi> PhauThuatThuThuatEkipBacSis
        {
            get => _phauThuatThuThuatEkipBacSis ?? (_phauThuatThuThuatEkipBacSis = new List<PhauThuatThuThuatEkipBacSi>());
            protected set => _phauThuatThuThuatEkipBacSis = value;
        }

        private ICollection<PhauThuatThuThuatEkipDieuDuong> _phauThuatThuThuatEkipDieuDuongs;
        public virtual ICollection<PhauThuatThuThuatEkipDieuDuong> PhauThuatThuThuatEkipDieuDuongs
        {
            get => _phauThuatThuThuatEkipDieuDuongs ?? (_phauThuatThuThuatEkipDieuDuongs = new List<PhauThuatThuThuatEkipDieuDuong>());
            protected set => _phauThuatThuThuatEkipDieuDuongs = value;
        }
    }
}
