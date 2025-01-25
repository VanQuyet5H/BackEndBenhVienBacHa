using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.GoiDichVus
{
    public class GoiDichVu : BaseEntity
    {
        public Enums.EnumLoaiGoiDichVu LoaiGoiDichVu { get; set; }

        public string Ten { get; set; }

        //public long NhanVienTaoGoiId { get; set; }

        //public bool CoChietKhau { get; set; }

        //public long? ChiPhiGoiDichVu { get; set; }

        //public DateTime NgayBatDau { get; set; }

        //public DateTime? NgayKetThuc { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }

        public BoPhan? BoPhanId { get; set; }

        //public virtual NhanVien NhanVienTaoGoi { get; set; }

        private ICollection<GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhs { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKhamBenh> GoiDichVuChiTietDichVuKhamBenhs
        {
            get => _goiDichVuChiTietDichVuKhamBenhs ?? (_goiDichVuChiTietDichVuKhamBenhs = new List<GoiDichVuChiTietDichVuKhamBenh>());
            protected set => _goiDichVuChiTietDichVuKhamBenhs = value;
        }
        private ICollection<GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuats { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKyThuat> GoiDichVuChiTietDichVuKyThuats
        {
            get => _goiDichVuChiTietDichVuKyThuats ?? (_goiDichVuChiTietDichVuKyThuats = new List<GoiDichVuChiTietDichVuKyThuat>());
            protected set => _goiDichVuChiTietDichVuKyThuats = value;
        }

        //private ICollection<GoiDichVuChiTietDuocPham> _goiDichVuChiTietDuocPhams { get; set; }
        //public virtual ICollection<GoiDichVuChiTietDuocPham> GoiDichVuChiTietDuocPhams
        //{
        //    get => _goiDichVuChiTietDuocPhams ?? (_goiDichVuChiTietDuocPhams = new List<GoiDichVuChiTietDuocPham>());
        //    protected set => _goiDichVuChiTietDuocPhams = value;
        //}
        //private ICollection<GoiDichVuChiTietVatTu> _goiDichVuChiTietVatTus { get; set; }
        //public virtual ICollection<GoiDichVuChiTietVatTu> GoiDichVuChiTietVatTus
        //{
        //    get => _goiDichVuChiTietVatTus ?? (_goiDichVuChiTietVatTus = new List<GoiDichVuChiTietVatTu>());
        //    protected set => _goiDichVuChiTietVatTus = value;
        //}

        private ICollection<GoiDichVuChiTietDichVuGiuong> _goiDichVuChiTietDichVuGiuongs;
        public virtual ICollection<GoiDichVuChiTietDichVuGiuong> GoiDichVuChiTietDichVuGiuongs
        {
            get => _goiDichVuChiTietDichVuGiuongs ?? (_goiDichVuChiTietDichVuGiuongs = new List<GoiDichVuChiTietDichVuGiuong>());
            protected set => _goiDichVuChiTietDichVuGiuongs = value;
        }

        //private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        //public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        //{
        //    get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
        //    protected set => _yeuCauDichVuKyThuats = value;
        //}

        //private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens { get; set; }
        //public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        //{
        //    get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
        //    protected set => _yeuCauDuocPhamBenhViens = value;
        //}

        //private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs { get; set; }
        //public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        //{
        //    get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
        //    protected set => _yeuCauKhamBenhs = value;
        //}

        //private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        //public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        //{
        //    get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
        //    protected set => _yeuCauDichVuGiuongBenhViens = value;

        //}
        
        //private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVus;
        //public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVus
        //{
        //    get => _yeuCauGoiDichVus ?? (_yeuCauGoiDichVus = new List<YeuCauGoiDichVu>());
        //    protected set => _yeuCauGoiDichVus = value;

        //}

        //private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        //public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        //{
        //    get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
        //    protected set => _voucherChiTietMienGiams = value;
        //}

        private ICollection<ChuongTrinhGoiDichVu> _chuongTrinhGoiDichVus { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVu> ChuongTrinhGoiDichVus
        {
            get => _chuongTrinhGoiDichVus ?? (_chuongTrinhGoiDichVus = new List<ChuongTrinhGoiDichVu>());
            protected set => _chuongTrinhGoiDichVus = value;
        }

       
    }
}
