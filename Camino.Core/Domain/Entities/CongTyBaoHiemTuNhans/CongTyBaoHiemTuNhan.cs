using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;

namespace Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans
{
    public class CongTyBaoHiemTuNhan : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public string Email { get; set; }
        public string MaSoThue { get; set; }
        public string DonVi { get; set; }
        public Enums.EnumHinhThucBaoHiem HinhThucBaoHiem { get; set; }
        public Enums.EnumPhamViBaoHiem PhamViBaoHiem { get; set; }
        public string GhiChu { get; set; }

        //public virtual ICollection<BenhNhan> BenhNhan { get; set; }
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhan { get; set; }

        #region Update 12/2/2020

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        //{
        //    get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhans = value;
        //}

        #endregion Update 12/2/2020

        //update 22/02/2020 - thach
        public ICollection<YeuCauTiepNhanCongTyBaoHiemTuNhan> _yeuCauTiepNhanCongTyBaoHiemTuNhans;
        public virtual ICollection<YeuCauTiepNhanCongTyBaoHiemTuNhan> YeuCauTiepNhanCongTyBaoHiemTuNhans
        {
            get => _yeuCauTiepNhanCongTyBaoHiemTuNhans ?? (_yeuCauTiepNhanCongTyBaoHiemTuNhans = new List<YeuCauTiepNhanCongTyBaoHiemTuNhan>());
            protected set => _yeuCauTiepNhanCongTyBaoHiemTuNhans = value;
        }

        public ICollection<BenhNhanCongTyBaoHiemTuNhan> _benhNhanCongTyBaoHiemTuNhans;
        public virtual ICollection<BenhNhanCongTyBaoHiemTuNhan> BenhNhanCongTyBaoHiemTuNhans
        {
            get => _benhNhanCongTyBaoHiemTuNhans ?? (_benhNhanCongTyBaoHiemTuNhans = new List<BenhNhanCongTyBaoHiemTuNhan>());
            protected set => _benhNhanCongTyBaoHiemTuNhans = value;
        }

        private ICollection<CongTyBaoHiemTuNhanCongNo> _baoHiemTuNhanCongNos;
        public virtual ICollection<CongTyBaoHiemTuNhanCongNo> CongTyBaoHiemTuNhanCongNos
        {
            get => _baoHiemTuNhanCongNos ?? (_baoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNo>());
            protected set => _baoHiemTuNhanCongNos = value;
        }

        private ICollection<GachNo> _gachNos;
        public virtual ICollection<GachNo> GachNos
        {
            get => _gachNos ?? (_gachNos = new List<GachNo>());
            protected set => _gachNos = value;
        }

        private ICollection<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVus;
        public virtual ICollection<ChuongTrinhGoiDichVu> ChuongTrinhGoiDichVus
        {
            get => _chuongTrinhGoiDichVus ?? (_chuongTrinhGoiDichVus = new List<ChuongTrinhGoiDichVu>());
            protected set => _chuongTrinhGoiDichVus = value;
        }
    }
}
