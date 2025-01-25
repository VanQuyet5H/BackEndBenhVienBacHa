using Camino.Api.Models.KhamTheoDoiBoPhanKhac;
using System;
using System.Collections.Generic;

namespace Camino.Api.Models.KhamTheoDoi
{
    public class KhamTheoDoiViewModel : BaseViewModel
    {
        public long TheoDoiSauPhauThuatThuThuatId { get; set; }
        public DateTime ThoiDiemBatDauKham { get; set; }
        public DateTime? ThoiDiemHoanThanhKham { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string KhamToanThan { get; set; }
        public string ThongTinKhamTheoDoiTemplate { get; set; } //????
        public string ThongTinKhamTheoDoiData { get; set; } //????

        public List<KhamTheoDoiBoPhanKhacViewModel> KhamTheoDoiBoPhanKhacs { get; set; }

        //public virtual TheoDoiSauPhauThuatThuThuat TheoDoiSauPhauThuatThuThuat { get; set; }
        //public virtual PhongBenhVien NoiThucHien { get; set; }
        //public virtual NhanVien NhanVienThucHien { get; set; }

        //private ICollection<KhamTheoDoiBoPhanKhac> _khamTheoDoiBoPhanKhacs;
        //public virtual ICollection<KhamTheoDoiBoPhanKhac> KhamTheoDoiBoPhanKhacs
        //{
        //    get => _khamTheoDoiBoPhanKhacs ?? (_khamTheoDoiBoPhanKhacs = new List<KhamTheoDoiBoPhanKhac>());
        //    protected set => _khamTheoDoiBoPhanKhacs = value;
        //}
    }
}
