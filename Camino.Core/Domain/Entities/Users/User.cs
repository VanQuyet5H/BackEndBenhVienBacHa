using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.HeThong;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;

namespace Camino.Core.Domain.Entities.Users
{
    public class User : BaseEntity
    {
        //Khi nào apply chức năng zip hình ảnh lại thì mở map ra
        [NotMapped]
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public string Email { get; set; }
        public string SoChungMinhThu { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public bool IsActive { get; set; }
        public string PassCode { get; set; }
        public DateTime? ExpiredCodeDate { get; set; }
        public bool? IsDefault { get; set; }
        public Enums.Region Region { get; set; }
        
        public virtual Entities.NhanViens.NhanVien NhanVien { get; set; }

        private ICollection<NhatKyHeThong> _nhatKyHeThongs;
        public virtual ICollection<NhatKyHeThong> NhatKyHeThongs
        {
            get => _nhatKyHeThongs ?? (_nhatKyHeThongs = new List<NhatKyHeThong>());
            protected set => _nhatKyHeThongs = value;
        }

        private ICollection<UserMessagingToken> _userMessagingTokens;
        public virtual ICollection<UserMessagingToken> UserMessagingTokens
        {
            get => _userMessagingTokens ?? (_userMessagingTokens = new List<UserMessagingToken>());
            protected set => _userMessagingTokens = value;
        }
        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiet;
        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiet ?? (_nhapKhoDuocPhamChiTiet = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiet = value;
        }
        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiet;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiet ?? (_nhapKhoVatTuChiTiet = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiet = value;
        }
        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;
        }

        private ICollection<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTiets;
        public virtual ICollection<YeuCauNhapKhoVatTuChiTiet> YeuCauNhapKhoVatTuChiTiets
        {
            get => _yeuCauNhapKhoVatTuChiTiets ?? (_yeuCauNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTiet>());
            protected set => _yeuCauNhapKhoVatTuChiTiets = value;
        }
    }
}
