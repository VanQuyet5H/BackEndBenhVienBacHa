using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.BenhVien
{
    public class BenhVien : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string DiaChi { get; set; }
        public long LoaiBenhVienId { get; set; }
        public Enums.HangBenhVien HangBenhVien { get; set; }
        public Enums.TuyenChuyenMonKyThuat TuyenChuyenMonKyThuat { get; set; }
        public string SoDienThoaiLanhDao { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }

        public long DonViHanhChinhId { get; set; }
        public bool? HieuLuc { get; set; }

        public virtual LoaiBenhViens.LoaiBenhVien LoaiBenhVien { get; set; }
        //public virtual CapQuanLyBenhViens.CapQuanLyBenhVien CapQuanLyBenhVien { get; set; }
        public virtual DonViHanhChinhs.DonViHanhChinh DonViHanhChinh { get; set; }

        //private ICollection<BenhNhan> _BHYTNoiDangKyhBenhNhans;
        //public virtual ICollection<BenhNhan> BHYTNoiDangKyBenhNhans
        //{
        //    get => _BHYTNoiDangKyhBenhNhans ?? (_BHYTNoiDangKyhBenhNhans = new List<BenhNhan>());
        //    protected set => _BHYTNoiDangKyhBenhNhans = value;
        //}

        #region Update 12/2/2020

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        //{
        //    get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhans = value;
        //}

        #endregion Update 12/2/2020

        #region update v2 tiep nhan

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion update v2 tiep nhan

        /// <summary>
        /// Update 02/06/2020
        /// </summary>
        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<NoiTruBenhAn> _noiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NoiTruBenhAns
        {
            get => _noiTruBenhAns ?? (_noiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _noiTruBenhAns = value;
        }
    }
}
