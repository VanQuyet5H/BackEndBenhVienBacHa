using Camino.Core.Domain.Entities.DichVuKhamBenhs;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.KhamDoans;
using static Camino.Core.Domain.Enums;

using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVien : BaseEntity
    {
        public long? DichVuKhamBenhId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public bool? CoUuDai { get; set; }
        public ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public virtual DichVuKhamBenh DichVuKhamBenh { get; set; }

        private ICollection<DichVuKhamBenhBenhVienGiaBaoHiem> _dichVuKhamBenhBenhVienGiaBaoHiems { get; set; }
        public virtual ICollection<DichVuKhamBenhBenhVienGiaBaoHiem> DichVuKhamBenhBenhVienGiaBaoHiems
        {
            get => _dichVuKhamBenhBenhVienGiaBaoHiems ?? (_dichVuKhamBenhBenhVienGiaBaoHiems = new List<DichVuKhamBenhBenhVienGiaBaoHiem>());
            protected set => _dichVuKhamBenhBenhVienGiaBaoHiems = value;
        }

        private ICollection<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhViens { get; set; }
        public virtual ICollection<DichVuKhamBenhBenhVienGiaBenhVien> DichVuKhamBenhBenhVienGiaBenhViens
        {
            get => _dichVuKhamBenhBenhVienGiaBenhViens ?? (_dichVuKhamBenhBenhVienGiaBenhViens = new List<DichVuKhamBenhBenhVienGiaBenhVien>());
            protected set => _dichVuKhamBenhBenhVienGiaBenhViens = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs { get; set; }
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhs { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKhamBenh> GoiDichVuChiTietDichVuKhamBenhs
        {
            get => _goiDichVuChiTietDichVuKhamBenhs ?? (_goiDichVuChiTietDichVuKhamBenhs = new List<GoiDichVuChiTietDichVuKhamBenh>());
            protected set => _goiDichVuChiTietDichVuKhamBenhs = value;
        }

        private ICollection<DoiTuongUuDaiDichVuKhamBenhBenhVien> _doiTuongUuDaiDichVuKhamBenhBenhVien { get; set; }
        public virtual ICollection<DoiTuongUuDaiDichVuKhamBenhBenhVien> DoiTuongUuDaiDichVuKhamBenhBenhViens
        {
            get => _doiTuongUuDaiDichVuKhamBenhBenhVien ?? (_doiTuongUuDaiDichVuKhamBenhBenhVien = new List<DoiTuongUuDaiDichVuKhamBenhBenhVien>());
            protected set => _doiTuongUuDaiDichVuKhamBenhBenhVien = value;
        }

        private ICollection<TemplateKhamBenhTheoDichVu> _templateKhamBenhTheoDichVus { get; set; }
        public virtual ICollection<TemplateKhamBenhTheoDichVu> TemplateKhamBenhTheoDichVus
        {
            get => _templateKhamBenhTheoDichVus ?? (_templateKhamBenhTheoDichVus = new List<TemplateKhamBenhTheoDichVu>());
            protected set => _templateKhamBenhTheoDichVus = value;
        }

        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }

        private ICollection<DichVuKhamBenhBenhVienNoiThucHien> _dichVuKhamBenhBenhVienNoiThucHiens { get; set; }
        public virtual ICollection<DichVuKhamBenhBenhVienNoiThucHien> DichVuKhamBenhBenhVienNoiThucHiens
        {
            get => _dichVuKhamBenhBenhVienNoiThucHiens ?? (_dichVuKhamBenhBenhVienNoiThucHiens = new List<DichVuKhamBenhBenhVienNoiThucHien>());
            protected set => _dichVuKhamBenhBenhVienNoiThucHiens = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichKhamBenhs { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> ChuongTrinhGoiDichVuDichKhamBenhs
        {
            get => _chuongTrinhGoiDichVuDichKhamBenhs ?? (_chuongTrinhGoiDichVuDichKhamBenhs = new List<ChuongTrinhGoiDichVuDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuDichKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeDichVuKhamBenh> GoiKhamSucKhoeDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeDichVuKhamBenhs ?? (_goiKhamSucKhoeDichVuKhamBenhs = new List<GoiKhamSucKhoeDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeDichVuKhamBenhs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> _goiKhamSucKhoeChungDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> GoiKhamSucKhoeChungDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhs ?? (_goiKhamSucKhoeChungDichVuKhamBenhs = new List<GoiKhamSucKhoeChungDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> GoiKhamSucKhoeChungDichVuKhamBenhNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens ?? (_goiKhamSucKhoeChungDichVuKhamBenhNhanViens = new List<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = value;
        }
    }
}
