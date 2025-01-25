using System.Collections.Generic;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.NhomDichVuBenhVien
{
    public class NhomDichVuBenhVien : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool IsDefault { get; set; }

        public long? NhomDichVuBenhVienChaId { get; set; }

        private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        {
            get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
            protected set => _dichVuKyThuatBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }
        public virtual NhomDichVuBenhVien NhomDichVuBenhVienCha { get; set; }

        private ICollection<NhomDichVuBenhVien> _nhomDichVuBenhVienChas { get; set; }

        public virtual ICollection<NhomDichVuBenhVien> NhomDichVuBenhViens
        {
            get => _nhomDichVuBenhVienChas ?? (_nhomDichVuBenhVienChas = new List<NhomDichVuBenhVien>());
            protected set => _nhomDichVuBenhVienChas = value;
        }

        private ICollection<KetQuaNhomXetNghiem> _ketQuaNhomXetNghiems;
        public virtual ICollection<KetQuaNhomXetNghiem> KetQuaNhomXetNghiems
        {
            get => _ketQuaNhomXetNghiems ?? (_ketQuaNhomXetNghiems = new List<KetQuaNhomXetNghiem>());
            protected set => _ketQuaNhomXetNghiems = value;
        }

        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }

        private ICollection<MauMayXetNghiem> _mauMayXetNghiems { get; set; }
        public virtual ICollection<MauMayXetNghiem> MauMayXetNghiems
        {
            get => _mauMayXetNghiems ?? (_mauMayXetNghiems = new List<MauMayXetNghiem>());
            protected set => _mauMayXetNghiems = value;
        }

        private ICollection<MauXetNghiem> _mauXetNghiems;
        public virtual ICollection<MauXetNghiem> MauXetNghiems
        {
            get => _mauXetNghiems ?? (_mauXetNghiems = new List<MauXetNghiem>());
            protected set => _mauXetNghiems = value;
        }

        private ICollection<PhienXetNghiemChiTiet> _phienXetNghiemChiTiets;
        public virtual ICollection<PhienXetNghiemChiTiet> PhienXetNghiemChiTiets
        {
            get => _phienXetNghiemChiTiets ?? (_phienXetNghiemChiTiets = new List<PhienXetNghiemChiTiet>());
            protected set => _phienXetNghiemChiTiets = value;
        }

        private ICollection<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiems;
        public virtual ICollection<YeuCauChayLaiXetNghiem> YeuCauChayLaiXetNghiems
        {
            get => _yeuCauChayLaiXetNghiems ?? (_yeuCauChayLaiXetNghiems = new List<YeuCauChayLaiXetNghiem>());
            protected set => _yeuCauChayLaiXetNghiems = value;
        }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<DichVuXetNghiem> _dichVuXetNghiems;
        public virtual ICollection<DichVuXetNghiem> DichVuXetNghiems
        {
            get => _dichVuXetNghiems ?? (_dichVuXetNghiems = new List<DichVuXetNghiem>());
            protected set => _dichVuXetNghiems = value;
        }

        private ICollection<CauHinhNguoiDuyetTheoNhomDichVu> _cauHinhNguoiDuyetTheoNhomDichVus;
        public virtual ICollection<CauHinhNguoiDuyetTheoNhomDichVu> CauHinhNguoiDuyetTheoNhomDichVus
        {
            get => _cauHinhNguoiDuyetTheoNhomDichVus ?? (_cauHinhNguoiDuyetTheoNhomDichVus = new List<CauHinhNguoiDuyetTheoNhomDichVu>());
            protected set => _cauHinhNguoiDuyetTheoNhomDichVus = value;
        }
    }
}
