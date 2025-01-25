using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class YeuCauNhanSuKhamSucKhoe : BaseEntity
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public long NhanVienGuiYeuCauId { get; set; }
        public DateTime NgayGuiYeuCau { get; set; }
        public bool? DuocKHTHDuyet { get; set; }
        public DateTime? NgayKHTHDuyet { get; set; }
        public long? NhanVienKHTHDuyetId { get; set; }
        public string LyDoKHTHKhongDuyet { get; set; }
        public bool? DuocNhanSuDuyet { get; set; }
        public DateTime? NgayNhanSuDuyet { get; set; }
        public long? NhanVienNhanSuDuyetId { get; set; }
        public string LyDoNhanSuKhongDuyet { get; set; }
        public bool? DuocGiamDocDuyet { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public long? GiamDocId { get; set; }
        public string LyDoGiamDocKhongDuyet { get; set; }

        public virtual HopDongKhamSucKhoe HopDongKhamSucKhoe { get; set; }
        public virtual NhanVien NhanVienGuiYeuCau { get; set; }
        public virtual NhanVien NhanVienKHTHDuyet { get; set; }
        public virtual NhanVien NhanVienNhanSuDuyet { get; set; }
        public virtual NhanVien GiamDoc { get; set; }

        private ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> _yeuCauNhanSuKhamSucKhoeChiTiets;
        public virtual ICollection<YeuCauNhanSuKhamSucKhoeChiTiet> YeuCauNhanSuKhamSucKhoeChiTiets
        {
            get => _yeuCauNhanSuKhamSucKhoeChiTiets ?? (_yeuCauNhanSuKhamSucKhoeChiTiets = new List<YeuCauNhanSuKhamSucKhoeChiTiet>());
            protected set => _yeuCauNhanSuKhamSucKhoeChiTiets = value;
        }
    }
}
