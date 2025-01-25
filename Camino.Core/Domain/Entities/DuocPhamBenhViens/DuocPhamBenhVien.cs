using Camino.Core.Domain.Entities.DinhMucDuocPhamTonKhos;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Core.Domain.Entities.DuocPhamBenhViens
{
    public class DuocPhamBenhVien : BaseEntity
    {
        // public bool BaoHiemChiTra { get; set; }
        // public int TiLeBaoHiemThanhToan { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Ma { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public bool HieuLuc { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public virtual DuocPhamBenhVienPhanNhom DuocPhamBenhVienPhanNhom { get; set; }
        public virtual DuocPham DuocPham { get; set; }
        public LoaiDieuKienBaoQuanDuocPham? LoaiDieuKienBaoQuanDuocPham { get; set; }
        public string ThongTinDieuKienBaoQuanDuocPham { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }

        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiets;
        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiets ?? (_nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiets = value;

        }
        private ICollection<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTiets;
        public virtual ICollection<XuatKhoDuocPhamChiTiet> XuatKhoDuocPhamChiTiets
        {
            get => _xuatKhoDuocPhamChiTiets ?? (_xuatKhoDuocPhamChiTiets = new List<XuatKhoDuocPhamChiTiet>());
            protected set => _xuatKhoDuocPhamChiTiets = value;
        }
        private ICollection<DinhMucDuocPhamTonKho> _dinhMucDuocPhamTonKhos;
        public virtual ICollection<DinhMucDuocPhamTonKho> DinhMucDuocPhamTonKhos
        {
            get => _dinhMucDuocPhamTonKhos ?? (_dinhMucDuocPhamTonKhos = new List<DinhMucDuocPhamTonKho>());
            protected set => _dinhMucDuocPhamTonKhos = value;
        }


        //private ICollection<GoiDichVuChiTietDuocPham> _goiDichVuChiTietDuocPhams;
        //public virtual ICollection<GoiDichVuChiTietDuocPham> GoiDichVuChiTietDuocPhams
        //{
        //    get => _goiDichVuChiTietDuocPhams ?? (_goiDichVuChiTietDuocPhams = new List<GoiDichVuChiTietDuocPham>());
        //    protected set => _goiDichVuChiTietDuocPhams = value;
        //}

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        //private ICollection<DuocPhamBenhVienGiaBaoHiem> _duocPhamBenhVienGiaBaoHiems;
        //public virtual ICollection<DuocPhamBenhVienGiaBaoHiem> DuocPhamBenhVienGiaBaoHiems
        //{
        //    get => _duocPhamBenhVienGiaBaoHiems ?? (_duocPhamBenhVienGiaBaoHiems = new List<DuocPhamBenhVienGiaBaoHiem>());
        //    protected set => _duocPhamBenhVienGiaBaoHiems = value;
        //}

        //private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        //public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        //{
        //    get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
        //    protected set => _voucherChiTietMienGiams = value;
        //}

        private ICollection<YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauLinhDuocPhamChiTiet> YeuCauLinhDuocPhamChiTiets
        {
            get => _yeuCauLinhDuocPhamChiTiets ?? (_yeuCauLinhDuocPhamChiTiets = new List<YeuCauLinhDuocPhamChiTiet>());
            protected set => _yeuCauLinhDuocPhamChiTiets = value;
        }

        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamChiTiet> YeuCauTraDuocPhamChiTiets
        {
            get => _yeuCauTraDuocPhamChiTiets ?? (_yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTiet>());
            protected set => _yeuCauTraDuocPhamChiTiets = value;
        }
        
        private ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> YeuCauTraDuocPhamTuBenhNhanChiTiets
        {
            get => _yeuCauTraDuocPhamTuBenhNhanChiTiets ?? (_yeuCauTraDuocPhamTuBenhNhanChiTiets = new List<YeuCauTraDuocPhamTuBenhNhanChiTiet>());
            protected set => _yeuCauTraDuocPhamTuBenhNhanChiTiets = value;
        }

        private ICollection<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhams;
        public virtual ICollection<NoiTruChiDinhDuocPham> NoiTruChiDinhDuocPhams
        {
            get => _noiTruChiDinhDuocPhams ?? (_noiTruChiDinhDuocPhams = new List<NoiTruChiDinhDuocPham>());
            protected set => _noiTruChiDinhDuocPhams = value;
        }

        private ICollection<YeuCauXuatKhoDuocPhamChiTiet> _yeuCauXuatKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauXuatKhoDuocPhamChiTiet> YeuCauXuatKhoDuocPhamChiTiets
        {
            get => _yeuCauXuatKhoDuocPhamChiTiets ?? (_yeuCauXuatKhoDuocPhamChiTiets = new List<YeuCauXuatKhoDuocPhamChiTiet>());
            protected set => _yeuCauXuatKhoDuocPhamChiTiets = value;
        }
        private ICollection<YeuCauDieuChuyenDuocPhamChiTiet> _yeuCauDieuChuyenDuocPhamChiTiets;
        public virtual ICollection<YeuCauDieuChuyenDuocPhamChiTiet> YeuCauDieuChuyenDuocPhamChiTiets
        {
            get => _yeuCauDieuChuyenDuocPhamChiTiets ?? (_yeuCauDieuChuyenDuocPhamChiTiets = new List<YeuCauDieuChuyenDuocPhamChiTiet>());
            protected set => _yeuCauDieuChuyenDuocPhamChiTiets = value;
        }

        private ICollection<DichVuKyThuatBenhVienTiemChung> _dichVuKyThuatBenhVienTiemChungs;
        public virtual ICollection<DichVuKyThuatBenhVienTiemChung> DichVuKyThuatBenhVienTiemChungs
        {
            get => _dichVuKyThuatBenhVienTiemChungs ?? (_dichVuKyThuatBenhVienTiemChungs = new List<DichVuKyThuatBenhVienTiemChung>());
            protected set => _dichVuKyThuatBenhVienTiemChungs = value;
        }

        private ICollection<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu> _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus;
        public virtual ICollection<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu> DichVuKyThuatBenhVienDinhMucDuocPhamVatTus
        {
            get => _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus ?? (_dichVuKyThuatBenhVienDinhMucDuocPhamVatTus = new List<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu>());
            protected set => _dichVuKyThuatBenhVienDinhMucDuocPhamVatTus = value;
        }

        private ICollection<DuocPhamBenhVienMayXetNghiem> _duocPhamBenhVienMayXetNghiems;
        public virtual ICollection<DuocPhamBenhVienMayXetNghiem> DuocPhamBenhVienMayXetNghiems
        {
            get => _duocPhamBenhVienMayXetNghiems ?? (_duocPhamBenhVienMayXetNghiems = new List<DuocPhamBenhVienMayXetNghiem>());
            protected set => _duocPhamBenhVienMayXetNghiems = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDuocPham> _noiGioiThieuHopDongChiTietHoaHongDuocPhams;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDuocPham> NoiGioiThieuHopDongChiTietHoaHongDuocPhams
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDuocPhams ?? (_noiGioiThieuHopDongChiTietHoaHongDuocPhams = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDuocPhams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDuocPham> _noiGioiThieuHopDongChiTietHeSoDuocPhams;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDuocPham> NoiGioiThieuHopDongChiTietHeSoDuocPhams
        {
            get => _noiGioiThieuHopDongChiTietHeSoDuocPhams ?? (_noiGioiThieuHopDongChiTietHeSoDuocPhams = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDuocPhams = value;
        }
    }
}
