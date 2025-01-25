using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.XetNghiems
{
    public class PhienXetNghiem : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int BarCodeNumber { get; private set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string BarCodeId { get; private set; }

        public long BenhNhanId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public int MaSo { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public long? PhongThucHienId { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string KetLuan { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
        public string GhiChu { get; set; }
        public bool? ChoKetQua { get; set; }
        public bool? DaTraKetQua { get; set; }
        public DateTime? ThoiDiemTraKetQua { get; set; }

        public virtual BenhNhan BenhNhan { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual PhongBenhVien PhongThucHien { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual NhanVien NhanVienKetLuan { get; set; }

        private ICollection<MauXetNghiem> _mauXetNghiems;
        public virtual ICollection<MauXetNghiem> MauXetNghiems
        {
            get => _mauXetNghiems ?? (_mauXetNghiems = new List<MauXetNghiem>());
            protected set => _mauXetNghiems = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTiets;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets
        {
            get => _phienXetNghiemChiTiets ?? (_phienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTiets = value;
        }

        private ICollection<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiems;
        public virtual ICollection<YeuCauChayLaiXetNghiem> YeuCauChayLaiXetNghiems
        {
            get => _yeuCauChayLaiXetNghiems ?? (_yeuCauChayLaiXetNghiems = new List<YeuCauChayLaiXetNghiem>());
            protected set => _yeuCauChayLaiXetNghiems = value;
        }
    }
}
