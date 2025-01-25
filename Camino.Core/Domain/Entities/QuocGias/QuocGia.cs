using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DanTocs;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NhaSanXuatTheoQuocGias;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.QuocGias
{
    public class QuocGia : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string QuocTich { get; set; }

        public string MaDienThoaiQuocTe { get; set; }

        public string ThuDo { get; set; }

        public Enums.EnumChauLuc ChauLuc { get; set; }

        public bool? IsDisabled { get; set; }



        private ICollection<BenhNhan> _quocTichBenhNhans;
        public virtual ICollection<BenhNhan> QuocTichBenhNhans
        {
            get => _quocTichBenhNhans ?? (_quocTichBenhNhans = new List<BenhNhan>());
            protected set => _quocTichBenhNhans = value;
        }

        private ICollection<NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia> _nhaSanXuatTheoQuocGias;
        public virtual ICollection<NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia> NhaSanXuatTheoQuocGias
        {
            get => _nhaSanXuatTheoQuocGias ?? (_nhaSanXuatTheoQuocGias = new List<NhaSanXuatTheoQuocGia>());
            protected set => _nhaSanXuatTheoQuocGias = value;
        }

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020
        #region 8/7/2020
        private ICollection<DanToc> _danTocs;

        public virtual ICollection<DanToc> DanTocs
        {
            get => _danTocs ?? (_danTocs = new List<DanToc>());
            protected set => _danTocs = value;
        }
        #endregion

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeNhanViens
        {
            get => _hopDongKhamSucKhoeNhanViens ?? (_hopDongKhamSucKhoeNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeNhanViens = value;
        }
    }
}
