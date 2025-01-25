using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.DonViMaus;
using Camino.Core.Domain.Entities.BenhNhans;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieu : BaseEntity
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string SoDienThoai { get; set; }
        public string DonVi { get; set; }
        public long? DonViMauId { get; set; }
        public long? NhanVienQuanLyId { get; set; }
        public bool? IsDisabled { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public virtual NhanViens.NhanVien NhanVienQuanLy { get; set; }
        public virtual DonViMau DonViMau { get; set; }


        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }
        
        private ICollection<NoiGioiThieuHopDong> _noiGioiThieuHopDongs;
        public virtual ICollection<NoiGioiThieuHopDong> NoiGioiThieuHopDongs
        {
            get => _noiGioiThieuHopDongs ?? (_noiGioiThieuHopDongs = new List<NoiGioiThieuHopDong>());
            protected set => _noiGioiThieuHopDongs = value;
        }
    }
}