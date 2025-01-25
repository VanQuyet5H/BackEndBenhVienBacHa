using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class CongTyKhamSucKhoe : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Enums.EnumLoaiCongTy LoaiCongTy { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MaSoThue { get; set; }
        public string SoTaiKhoanNganHang { get; set; }
        public string NguoiDaiDien { get; set; }
        public string NguoiLienHe { get; set; }
        public bool CoHoatDong { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }

        private ICollection<HopDongKhamSucKhoe> _hopDongKhamSucKhoes;
        public virtual ICollection<HopDongKhamSucKhoe> HopDongKhamSucKhoes
        {
            get => _hopDongKhamSucKhoes ?? (_hopDongKhamSucKhoes = new List<HopDongKhamSucKhoe>());
            protected set => _hopDongKhamSucKhoes = value;
        }
    }
}
