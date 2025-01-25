using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.PhauThuatThuThuats
{
    public class TheoDoiSauPhauThuatThuThuat : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public EnumTrangThaiTheoDoiSauPhauThuatThuThuat TrangThai { get; set; }
        public DateTime? ThoiDiemBatDauTheoDoi { get; set; }
        public DateTime? ThoiDiemKetThucTheoDoi { get; set; }
        public long? BacSiPhuTrachTheoDoiId { get; set; }
        public long? DieuDuongPhuTrachTheoDoiId { get; set; }
        public string GhiChuTheoDoi { get; set; }
        public EnumTuVongPTTTTheoNgay? TuVongTrongPTTT { get; set; }
        public EnumThoiGianTuVongPTTTTheoNgay? KhoangThoiGianTuVong { get; set; }
        public DateTime? ThoiDiemTuVong { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhanVien BacSiPhuTrachTheoDoi { get; set; }
        public virtual NhanVien DieuDuongPhuTrachTheoDoi { get; set; }

        //private ICollection<PhauThuatThuThuatLuocDo> _phauThuatThuThuatLuocDos;
        //public virtual ICollection<PhauThuatThuThuatLuocDo> PhauThuatThuThuatLuocDos
        //{
        //    get => _phauThuatThuThuatLuocDos ?? (_phauThuatThuThuatLuocDos = new List<PhauThuatThuThuatLuocDo>());
        //    protected set => _phauThuatThuThuatLuocDos = value;
        //}

        private ICollection<KhamTheoDoi> _khamTheoDois;
        public virtual ICollection<KhamTheoDoi> KhamTheoDois
        {
            get => _khamTheoDois ?? (_khamTheoDois = new List<KhamTheoDoi>());
            protected set => _khamTheoDois = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }
    }
}