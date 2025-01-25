using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.Thuocs
{
    public class ThuocHoacHoatChat : BaseEntity
    {
        public int? STTHoatChat { get; set; }

        public int? STTThuoc { get; set; }

        public string Ma { get; set; }

        public string MaATC { get; set; }

        public string Ten { get; set; }

        public long? DuongDungId { get; set; }

        public bool? HoiChan { get; set; }

        public long TyLeBaoHiemThanhToan { get; set; }

        public bool? CoDieuKienThanhToan { get; set; }

        public string MoTa { get; set; } // cái này là mô tả của trường có điều kiện thanh toán

        public int? NhomChiPhi { get; set; } // cái này tạm thời chưa cần sử dụng

        public bool? BenhVienHang1 { get; set; }

        public bool? BenhVienHang2 { get; set; }

        public bool? BenhVienHang3 { get; set; }

        public bool? BenhVienHang4 { get; set; }

        public long NhomthuocId { get; set; }

        public virtual NhomThuoc NhomThuoc { get; set; }

        public virtual DuongDung DuongDung { get; set; }

        private ICollection<ADR> _adr1s;
        public virtual ICollection<ADR> ADR1s
        {
            get => _adr1s ?? (_adr1s = new List<ADR>());
            protected set => _adr1s = value;
        }
        private ICollection<ADR> _adr2s;
        public virtual ICollection<ADR> ADR2s
        {
            get => _adr2s ?? (_adr2s = new List<ADR>());
            protected set => _adr2s = value;
        }
    }
}
