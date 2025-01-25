using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs;
using Camino.Core.Domain.Entities.GoiDichVus;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.LoaiGoiDichVus;

namespace Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus
{
    public class ChuongTrinhGoiDichVu : BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long GoiDichVuId { get; set; }
        public long? CongTyBaoHiemTuNhanId { get; set; }
        public string TenGoiDichVu { get; set; }
        public string MoTaGoiDichVu { get; set; }
        public decimal GiaTruocChietKhau { get; set; }
        public decimal GiaSauChietKhau { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool? GoiSoSinh { get; set; }
        public bool? TamNgung { get; set; }
        public long? BenhNhanId { get; set; }
        public long? LoaiGoiDichVuId { get; set; }

        public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual LoaiGoiDichVu LoaiGoiDichVu { get; set; }

        public virtual CongTyBaoHiemTuNhan CongTyBaoHiemTuNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }

        private ICollection<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuDichVuGiuongs;
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuGiuong> ChuongTrinhGoiDichVuDichVuGiuongs
        {
            get => _chuongTrinhGoiDichVuDichVuGiuongs ?? (_chuongTrinhGoiDichVuDichVuGiuongs = new List<ChuongTrinhGoiDichVuDichVuGiuong>());
            protected set => _chuongTrinhGoiDichVuDichVuGiuongs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichKhamBenhs;
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> ChuongTrinhGoiDichVuDichKhamBenhs
        {
            get => _chuongTrinhGoiDichVuDichKhamBenhs ?? (_chuongTrinhGoiDichVuDichKhamBenhs = new List<ChuongTrinhGoiDichVuDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuDichKhamBenhs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuats;
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> ChuongTrinhGoiDichVuDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuDichVuKyThuats ?? (_chuongTrinhGoiDichVuDichVuKyThuats = new List<ChuongTrinhGoiDichVuDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuDichVuKyThuats = value;
        }

        private ICollection<ChuongTrinhGoiDichVuQuaTang> _chuongTrinhGoiDichVuQuaTangs;
        public virtual ICollection<ChuongTrinhGoiDichVuQuaTang> ChuongTrinhGoiDichVuQuaTangs
        {
            get => _chuongTrinhGoiDichVuQuaTangs ?? (_chuongTrinhGoiDichVuQuaTangs = new List<ChuongTrinhGoiDichVuQuaTang>());
            protected set => _chuongTrinhGoiDichVuQuaTangs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = value;
        }

        private ICollection<YeuCauGoiDichVu> _yeuCauGoiDichVus;
        public virtual ICollection<YeuCauGoiDichVu> YeuCauGoiDichVus
        {
            get => _yeuCauGoiDichVus ?? (_yeuCauGoiDichVus = new List<YeuCauGoiDichVu>());
            protected set => _yeuCauGoiDichVus = value;

        }
    }
}
