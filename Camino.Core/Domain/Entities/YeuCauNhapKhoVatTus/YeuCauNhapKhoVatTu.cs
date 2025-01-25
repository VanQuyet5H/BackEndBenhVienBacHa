using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus
{
    public class YeuCauNhapKhoVatTu : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long KhoId { get; set; }
        public string SoChungTu { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public DateTime NgayNhap { get; set; }
        public long NguoiNhapId { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public bool? DaXuatExcel { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual NhanVien NguoiGiao { get; set; }
        public virtual NhanVien NguoiNhap { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTus;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTus
        {
            get => _nhapKhoVatTus ?? (_nhapKhoVatTus = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTus = value;
        }

        private ICollection<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTiets;

        public virtual ICollection<YeuCauNhapKhoVatTuChiTiet> YeuCauNhapKhoVatTuChiTiets
        {
            get => _yeuCauNhapKhoVatTuChiTiets ?? (_yeuCauNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTiet>());
            protected set => _yeuCauNhapKhoVatTuChiTiets = value;
        }
    }
}
