using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.VatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Core.Domain.Entities.VatTuBenhViens
{
    public class VatTuBenhVien : BaseEntity
    {
        //public bool BaoHiemChiTra { get; set; }
        //public int TiLeBaoHiemThanhToan { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Ma { get; set; }
        public string MaVatTuBenhVien { get; set; }

        public bool HieuLuc { get; set; }

        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }

        public virtual VatTu VatTus { get; set; }
        //private ICollection<GoiDichVuChiTietVatTu> _goiDichVuChiTietVatTus { get; set; }
        //public virtual ICollection<GoiDichVuChiTietVatTu> GoiDichVuChiTietVatTus
        //{
        //    get => _goiDichVuChiTietVatTus ?? (_goiDichVuChiTietVatTus = new List<GoiDichVuChiTietVatTu>());
        //    protected set => _goiDichVuChiTietVatTus = value;
        //}
        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens { get; set; }
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        //private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        //public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        //{
        //    get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
        //    protected set => _voucherChiTietMienGiams = value;
        //}

        private ICollection<DinhMucVatTuTonKho> _dinhMucVatTuTonKhos;
        public virtual ICollection<DinhMucVatTuTonKho> DinhMucVatTuTonKhos
        {
            get => _dinhMucVatTuTonKhos ?? (_dinhMucVatTuTonKhos = new List<DinhMucVatTuTonKho>());
            protected set => _dinhMucVatTuTonKhos = value;
        }

        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiets = value;

        }

        //private ICollection<VatTuBenhVienGiaBaoHiem> _vatTuBenhVienGiaBaoHiems;
        //public virtual ICollection<VatTuBenhVienGiaBaoHiem> VatTuBenhVienGiaBaoHiems
        //{
        //    get => _vatTuBenhVienGiaBaoHiems ?? (_vatTuBenhVienGiaBaoHiems = new List<VatTuBenhVienGiaBaoHiem>());
        //    protected set => _vatTuBenhVienGiaBaoHiems = value;

        //}

        private ICollection<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTiets;

        public virtual ICollection<XuatKhoVatTuChiTiet> XuatKhoVatTuChiTiets
        {
            get => _xuatKhoVatTuChiTiets ?? (_xuatKhoVatTuChiTiets = new List<XuatKhoVatTuChiTiet>());
            protected set => _xuatKhoVatTuChiTiets = value;
        }

        private ICollection<YeuCauLinhVatTuChiTiet> _yeuCauLinhVatTuChiTiets;

        public virtual ICollection<YeuCauLinhVatTuChiTiet> YeuCauLinhVatTuChiTiets
        {
            get => _yeuCauLinhVatTuChiTiets ?? (_yeuCauLinhVatTuChiTiets = new List<YeuCauLinhVatTuChiTiet>());
            protected set => _yeuCauLinhVatTuChiTiets = value;
        }

        private ICollection<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTiets;

        public virtual ICollection<YeuCauNhapKhoVatTuChiTiet> YeuCauNhapKhoVatTuChiTiets
        {
            get => _yeuCauNhapKhoVatTuChiTiets ?? (_yeuCauNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTiet>());
            protected set => _yeuCauNhapKhoVatTuChiTiets = value;
        }

        private ICollection<YeuCauKhamBenhDonVTYTChiTiet> _yeuCauKhamBenhDonVTYTChiTiets;
        public virtual ICollection<YeuCauKhamBenhDonVTYTChiTiet> YeuCauKhamBenhDonVTYTChiTiets
        {
            get => _yeuCauKhamBenhDonVTYTChiTiets ?? (_yeuCauKhamBenhDonVTYTChiTiets = new List<YeuCauKhamBenhDonVTYTChiTiet>());
            protected set => _yeuCauKhamBenhDonVTYTChiTiets = value;
        }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }

        private ICollection<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTiets;
        public virtual ICollection<YeuCauTraVatTuChiTiet> YeuCauTraVatTuChiTiets
        {
            get => _yeuCauTraVatTuChiTiets ?? (_yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuChiTiet>());
            protected set => _yeuCauTraVatTuChiTiets = value;

        }

        private ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> YeuCauTraVatTuTuBenhNhanChiTiets
        {
            get => _yeuCauTraVatTuTuBenhNhanChiTiets ?? (_yeuCauTraVatTuTuBenhNhanChiTiets = new List<YeuCauTraVatTuTuBenhNhanChiTiet>());
            protected set => _yeuCauTraVatTuTuBenhNhanChiTiets = value;
        }

        private ICollection<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTiets;
        public virtual ICollection<YeuCauXuatKhoVatTuChiTiet> YeuCauXuatKhoVatTuChiTiets
        {
            get => _yeuCauXuatKhoVatTuChiTiets ?? (_yeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoVatTuChiTiet>());
            protected set => _yeuCauXuatKhoVatTuChiTiets = value;
        }

        private ICollection<DonVTYTThanhToanChiTietTheoPhieuThu> _donVTYTThanhToanChiTietTheoPhieuThus;
        public virtual ICollection<DonVTYTThanhToanChiTietTheoPhieuThu> DonVTYTThanhToanChiTietTheoPhieuThus
        {
            get => _donVTYTThanhToanChiTietTheoPhieuThus ?? (_donVTYTThanhToanChiTietTheoPhieuThus = new List<DonVTYTThanhToanChiTietTheoPhieuThu>());
            protected set => _donVTYTThanhToanChiTietTheoPhieuThus = value;
        }

        private ICollection<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu> _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus;
        public virtual ICollection<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu> DichVuKyThuatBenhVienDinhMucDuocPhamVatTus
        {
            get => _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus ?? (_dichVuKyThuatBenhVienDinhMucDuocPhamVatTus = new List<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu>());
            protected set => _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongVatTu> _noiGioiThieuHopDongChiTietHoaHongVatTus;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongVatTu> NoiGioiThieuHopDongChiTietHoaHongVatTus
        {
            get => _noiGioiThieuHopDongChiTietHoaHongVatTus ?? (_noiGioiThieuHopDongChiTietHoaHongVatTus = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongVatTus = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoVatTu> _noiGioiThieuHopDongChiTietHeSoVatTus;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoVatTu> NoiGioiThieuHopDongChiTietHeSoVatTus
        {
            get => _noiGioiThieuHopDongChiTietHeSoVatTus ?? (_noiGioiThieuHopDongChiTietHeSoVatTus = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>());
            protected set => _noiGioiThieuHopDongChiTietHeSoVatTus = value;
        }
    }
}
