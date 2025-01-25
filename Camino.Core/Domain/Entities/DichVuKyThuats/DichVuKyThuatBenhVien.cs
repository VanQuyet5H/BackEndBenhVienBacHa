using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatBenhVien : BaseEntity
    {
        public long? DichVuKyThuatId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string NoiBanHanh { get; set; }
        public int? SoMayThucHien { get; set; }
        public int? SoCanBoChuyenMon { get; set; }
        public int? ThoiGianThucHien { get; set; }
        public int? SoCaChophep { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public bool? CoUuDai { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public long? DichVuXetNghiemId { get; set; }
        public string LoaiPhauThuatThuThuat { get; set; }
        public EnumLoaiMauXetNghiem? LoaiMauXetNghiem { get; set; }

        // cập nhật [KẾT QUẢ KIỂM THỬ TỪ BỆNH VIỆN][CĐHA][YÊU CẦU] #13 Các kết quả có kèm hình ảnh trên giấy trả kết quả: Nội soi, đo loãng xương
        public bool? CoInKetQuaKemHinhAnh { get; set; }
        // cập nhật [KẾT QUẢ KIỂM THỬ TỪ BỆNH VIỆN][CĐHA][YÊU CẦU] #11 Mục Kỹ thuật tự động lấy theo tên của dịch vụ từ thời điểm đăng ký dịch vụ
        public string TenKyThuat { get; set; }
        public bool? DichVuCoKetQuaLau { get; set; }
        public bool? DichVuKhongKetQua { get; set; }
        public int? SoLanThucHienXetNghiem { get; set; }
        public virtual DichVuKyThuat DichVuKyThuat { get; set; }

        public virtual DichVuXetNghiem DichVuXetNghiem { get; set; }

        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        public virtual Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua.DichVukyThuatBenhVienMauKetQua DichVukyThuatBenhVienMauKetQua { get; set; }


        //BVHD-3822
        public long? ChuyenKhoaChuyenNganhId { get; set; }
        public virtual Camino.Core.Domain.Entities.ChuyenKhoaChuyenNganh.ChuyenKhoaChuyenNganh ChuyenKhoaChuyenNganh { get; set; }
        public int? SoPhimXquang { get; set; }


        //BVHD-3901
        public bool? DichVuChuyenGoi { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public long? KhoaPhongTinhDoanhThuId { get; set; }
        public virtual KhoaPhong KhoaPhongTinhDoanhThu { get; set; }

        private ICollection<DichVuKyThuatBenhVienGiaBaoHiem> _dichVuKyThuatBenhVienGiaBaoHiems { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienGiaBaoHiem> DichVuKyThuatBenhVienGiaBaoHiems
        {
            get => _dichVuKyThuatBenhVienGiaBaoHiems ?? (_dichVuKyThuatBenhVienGiaBaoHiems = new List<DichVuKyThuatBenhVienGiaBaoHiem>());
            protected set => _dichVuKyThuatBenhVienGiaBaoHiems = value;
        }

        private ICollection<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatVuBenhVienGiaBenhViens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienGiaBenhVien> DichVuKyThuatVuBenhVienGiaBenhViens
        {
            get => _dichVuKyThuatVuBenhVienGiaBenhViens ?? (_dichVuKyThuatVuBenhVienGiaBenhViens = new List<DichVuKyThuatBenhVienGiaBenhVien>());
            protected set => _dichVuKyThuatVuBenhVienGiaBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuats { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKyThuat> GoiDichVuChiTietDichVuKyThuats
        {
            get => _goiDichVuChiTietDichVuKyThuats ?? (_goiDichVuChiTietDichVuKyThuats = new List<GoiDichVuChiTietDichVuKyThuat>());
            protected set => _goiDichVuChiTietDichVuKyThuats = value;
        }


        private ICollection<DoiTuongUuDaiDichVuKyThuatBenhVien> _doiTuongUuDaiDichVuKyThuatBenhViens { get; set; }
        public virtual ICollection<DoiTuongUuDaiDichVuKyThuatBenhVien> DoiTuongUuDaiDichVuKyThuatBenhViens
        {
            get => _doiTuongUuDaiDichVuKyThuatBenhViens ?? (_doiTuongUuDaiDichVuKyThuatBenhViens = new List<DoiTuongUuDaiDichVuKyThuatBenhVien>());
            protected set => _doiTuongUuDaiDichVuKyThuatBenhViens = value;
        }

        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }

        private ICollection<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHiens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienNoiThucHien> DichVuKyThuatBenhVienNoiThucHiens
        {
            get => _dichVuKyThuatBenhVienNoiThucHiens ?? (_dichVuKyThuatBenhVienNoiThucHiens = new List<DichVuKyThuatBenhVienNoiThucHien>());
            protected set => _dichVuKyThuatBenhVienNoiThucHiens = value;
        }

        private ICollection<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTiens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienNoiThucHienUuTien> DichVuKyThuatBenhVienNoiThucHienUuTiens
        {
            get => _dichVuKyThuatBenhVienNoiThucHienUuTiens ?? (_dichVuKyThuatBenhVienNoiThucHienUuTiens = new List<DichVuKyThuatBenhVienNoiThucHienUuTien>());
            protected set => _dichVuKyThuatBenhVienNoiThucHienUuTiens = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuats { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> ChuongTrinhGoiDichVuDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuDichVuKyThuats ?? (_chuongTrinhGoiDichVuDichVuKyThuats = new List<ChuongTrinhGoiDichVuDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuDichVuKyThuats = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTiets;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets
        {
            get => _phienXetNghiemChiTiets ?? (_phienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTiets = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> GoiKhamSucKhoeDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeDichVuDichVuKyThuats ?? (_goiKhamSucKhoeDichVuDichVuKyThuats = new List<GoiKhamSucKhoeDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeDichVuDichVuKyThuats = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = value;
        }

        private ICollection<DichVuKyThuatBenhVienTiemChung> _dichVuKyThuatBenhVienTiemChungs;
        public virtual ICollection<DichVuKyThuatBenhVienTiemChung> DichVuKyThuatBenhVienTiemChungs
        {
            get => _dichVuKyThuatBenhVienTiemChungs ?? (_dichVuKyThuatBenhVienTiemChungs = new List<DichVuKyThuatBenhVienTiemChung>());
            protected set => _dichVuKyThuatBenhVienTiemChungs = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> _goiKhamSucKhoeChungDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> GoiKhamSucKhoeChungDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeChungDichVuDichVuKyThuats ?? (_goiKhamSucKhoeChungDichVuDichVuKyThuats = new List<GoiKhamSucKhoeChungDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeChungDichVuDichVuKyThuats = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> GoiKhamSucKhoeChungDichVuKyThuatNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKyThuatNhanViens ?? (_goiKhamSucKhoeChungDichVuKyThuatNhanViens = new List<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKyThuatNhanViens = value;
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

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = value;
        }
    }
}