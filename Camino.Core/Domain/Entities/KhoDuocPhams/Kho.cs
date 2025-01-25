using Camino.Core.Domain.Entities.DinhMucDuocPhamTonKhos;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.KhoDuocPhamViTris;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;

namespace Camino.Core.Domain.Entities.KhoDuocPhams
{
    public class Kho : BaseEntity
    {
        public string Ten { get; set; }

        public Enums.EnumLoaiKhoDuocPham LoaiKho { get; set; }

        public long? KhoaPhongId { get; set; }

        public bool IsDefault { get; set; }

        public long? PhongBenhVienId { get; set; }

        public bool? LoaiDuocPham { get; set; }
        public bool? LoaiVatTu { get; set; }
        public bool? LaKhoKSNK { get; set; }

        public virtual PhongBenhVien PhongBenhVien { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }

        private ICollection<KhoViTri> _khoDuocPhamViTri;
        public virtual ICollection<KhoViTri> KhoDuocPhamViTris
        {
            get => _khoDuocPhamViTri ?? (_khoDuocPhamViTri = new List<KhoViTri>());
            protected set => _khoDuocPhamViTri = value;
        }

        private ICollection<NhapKhoDuocPham> _nhapKhoDuocPham;
        public virtual ICollection<NhapKhoDuocPham> NhapKhoDuocPhams
        {
            get => _nhapKhoDuocPham ?? (_nhapKhoDuocPham = new List<NhapKhoDuocPham>());
            protected set => _nhapKhoDuocPham = value;
        }

        private ICollection<DinhMucDuocPhamTonKho> _dinhMucDuocPhamTonKhos;
        public virtual ICollection<DinhMucDuocPhamTonKho> DinhMucDuocPhamTonKhos
        {
            get => _dinhMucDuocPhamTonKhos ?? (_dinhMucDuocPhamTonKhos = new List<DinhMucDuocPhamTonKho>());
            protected set => _dinhMucDuocPhamTonKhos = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhamXuats;
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhamXuats
        {
            get => _xuatKhoDuocPhamXuats ?? (_xuatKhoDuocPhamXuats = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhamXuats = value;
        }

        private ICollection<XuatKhoDuocPham> _xuatKhoDuocPhamNhaps;
        public virtual ICollection<XuatKhoDuocPham> XuatKhoDuocPhamNhaps
        {
            get => _xuatKhoDuocPhamNhaps ?? (_xuatKhoDuocPhamNhaps = new List<XuatKhoDuocPham>());
            protected set => _xuatKhoDuocPhamNhaps = value;
        }

        private ICollection<DinhMucVatTuTonKho> _dinhMucVatTuTonKhos;
        public virtual ICollection<DinhMucVatTuTonKho> DinhMucVatTuTonKhos
        {
            get => _dinhMucVatTuTonKhos ?? (_dinhMucVatTuTonKhos = new List<DinhMucVatTuTonKho>());
            protected set => _dinhMucVatTuTonKhos = value;
        }

        private ICollection<KhoNhanVienQuanLy> _khoNhanVienQuanLys;
        public virtual ICollection<KhoNhanVienQuanLy> KhoNhanVienQuanLys
        {
            get => _khoNhanVienQuanLys ?? (_khoNhanVienQuanLys = new List<KhoNhanVienQuanLy>());
            protected set => _khoNhanVienQuanLys = value;
        }

        private ICollection<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTus;
        public virtual ICollection<YeuCauNhapKhoVatTu> YeuCauNhapKhoVatTus
        {
            get => _yeuCauNhapKhoVatTus ?? (_yeuCauNhapKhoVatTus = new List<YeuCauNhapKhoVatTu>());
            protected set => _yeuCauNhapKhoVatTus = value;
        }

        private ICollection<NhapKhoVatTu> _nhapKhoVatTus;

        public virtual ICollection<NhapKhoVatTu> NhapKhoVatTus
        {
            get => _nhapKhoVatTus ?? (_nhapKhoVatTus = new List<NhapKhoVatTu>());
            protected set => _nhapKhoVatTus = value;
        }

        private ICollection<YeuCauLinhDuocPham> _yeuCauLinhDuocPhamKhoXuats;

        public virtual ICollection<YeuCauLinhDuocPham> YeuCauLinhDuocPhamKhoXuats
        {
            get => _yeuCauLinhDuocPhamKhoXuats ?? (_yeuCauLinhDuocPhamKhoXuats = new List<YeuCauLinhDuocPham>());
            protected set => _yeuCauLinhDuocPhamKhoXuats = value;
        }

        private ICollection<YeuCauLinhDuocPham> _yeuCauLinhDuocPhamKhoNhaps;

        public virtual ICollection<YeuCauLinhDuocPham> YeuCauLinhDuocPhamKhoNhaps
        {
            get => _yeuCauLinhDuocPhamKhoNhaps ?? (_yeuCauLinhDuocPhamKhoNhaps = new List<YeuCauLinhDuocPham>());
            protected set => _yeuCauLinhDuocPhamKhoNhaps = value;
        }

        private ICollection<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhams;

        public virtual ICollection<YeuCauNhapKhoDuocPham> YeuCauNhapKhoDuocPhams
        {
            get => _yeuCauNhapKhoDuocPhams ?? (_yeuCauNhapKhoDuocPhams = new List<YeuCauNhapKhoDuocPham>());
            protected set => _yeuCauNhapKhoDuocPhams = value;
        }

        private ICollection<YeuCauTraDuocPham> _yeuCauTraDuocPhamKhoXuats;

        public virtual ICollection<YeuCauTraDuocPham> YeuCauTraDuocPhamKhoXuats
        {
            get => _yeuCauTraDuocPhamKhoNhaps ?? (_yeuCauTraDuocPhamKhoNhaps = new List<YeuCauTraDuocPham>());
            protected set => _yeuCauTraDuocPhamKhoNhaps = value;
        }

        private ICollection<YeuCauTraDuocPham> _yeuCauTraDuocPhamKhoNhaps;

        public virtual ICollection<YeuCauTraDuocPham> YeuCauTraDuocPhamKhoNhaps
        {
            get => _yeuCauTraDuocPhamKhoNhaps ?? (_yeuCauTraDuocPhamKhoNhaps = new List<YeuCauTraDuocPham>());
            protected set => _yeuCauTraDuocPhamKhoNhaps = value;
        }

        private ICollection<YeuCauTraVatTu> _yeuCauTraVatTuKhoXuats;

        public virtual ICollection<YeuCauTraVatTu> YeuCauTraVatTuKhoXuats
        {
            get => _yeuCauTraVatTuKhoXuats ?? (_yeuCauTraVatTuKhoXuats = new List<YeuCauTraVatTu>());
            protected set => _yeuCauTraVatTuKhoXuats = value;
        }

        private ICollection<YeuCauTraVatTu> _yeuCauTraVatTuKhoNhaps;

        public virtual ICollection<YeuCauTraVatTu> YeuCauTraVatTuKhoNhaps
        {
            get => _yeuCauTraVatTuKhoNhaps ?? (_yeuCauTraVatTuKhoNhaps = new List<YeuCauTraVatTu>());
            protected set => _yeuCauTraVatTuKhoNhaps = value;
        }
        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }
        private ICollection<XuatKhoVatTu> _xuatKhoVatTuXuats;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTuXuats
        {
            get => _xuatKhoVatTuXuats ?? (_xuatKhoVatTuXuats = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTuXuats = value;
        }

        private ICollection<XuatKhoVatTu> _xuatKhoVatTuNhaps;
        public virtual ICollection<XuatKhoVatTu> XuatKhoVatTuNhaps
        {
            get => _xuatKhoVatTuNhaps ?? (_xuatKhoVatTuNhaps = new List<XuatKhoVatTu>());
            protected set => _xuatKhoVatTuNhaps = value;
        }

        private ICollection<YeuCauLinhVatTu> _yeuCauLinhVatTuKhoXuats;
        public virtual ICollection<YeuCauLinhVatTu> YeuCauLinhVatTuKhoXuats
        {
            get => _yeuCauLinhVatTuKhoXuats ?? (_yeuCauLinhVatTuKhoXuats = new List<YeuCauLinhVatTu>());
            protected set => _yeuCauLinhVatTuKhoXuats = value;
        }
        private ICollection<YeuCauLinhVatTu> _yeuCauLinhVatTuKhoNhaps;
        public virtual ICollection<YeuCauLinhVatTu> YeuCauLinhVatTuKhoNhaps
        {
            get => _yeuCauLinhVatTuKhoNhaps ?? (_yeuCauLinhVatTuKhoNhaps = new List<YeuCauLinhVatTu>());
            protected set => _yeuCauLinhVatTuKhoNhaps = value;
        }

        private ICollection<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamChiTiet> YeuCauTraDuocPhamChiTiets
        {
            get => _yeuCauTraDuocPhamChiTiets ?? (_yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTiet>());
            protected set => _yeuCauTraDuocPhamChiTiets = value;
        }

        private ICollection<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTiets;
        public virtual ICollection<YeuCauTraVatTuChiTiet> YeuCauTraVatTuChiTiets
        {
            get => _yeuCauTraVatTuChiTiets ?? (_yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuChiTiet>());
            protected set => _yeuCauTraVatTuChiTiets = value;

        }

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhams;
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhams
        {
            get => _duTruMuaDuocPhams ?? (_duTruMuaDuocPhams = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhams = value;

        }

        private ICollection<DuTruMuaVatTu> _duTruMuaVatTus;
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTus
        {
            get => _duTruMuaVatTus ?? (_duTruMuaVatTus = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTus = value;
        }
        
        private ICollection<YeuCauTraDuocPhamTuBenhNhan> _yeuCauTraDuocPhamTuBenhNhans;
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhan> YeuCauTraDuocPhamTuBenhNhans
        {
            get => _yeuCauTraDuocPhamTuBenhNhans ?? (_yeuCauTraDuocPhamTuBenhNhans = new List<YeuCauTraDuocPhamTuBenhNhan>());
            protected set => _yeuCauTraDuocPhamTuBenhNhans = value;
        }
        
        private ICollection<YeuCauTraVatTuTuBenhNhan> _yeuCauTraVatTuTuBenhNhans;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhan> YeuCauTraVatTuTuBenhNhans
        {
            get => _yeuCauTraVatTuTuBenhNhans ?? (_yeuCauTraVatTuTuBenhNhans = new List<YeuCauTraVatTuTuBenhNhan>());
            protected set => _yeuCauTraVatTuTuBenhNhans = value;
        }

        private ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> YeuCauTraDuocPhamTuBenhNhanChiTiets
        {
            get => _yeuCauTraDuocPhamTuBenhNhanChiTiets ?? (_yeuCauTraDuocPhamTuBenhNhanChiTiets = new List<YeuCauTraDuocPhamTuBenhNhanChiTiet>());
            protected set => _yeuCauTraDuocPhamTuBenhNhanChiTiets = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> YeuCauTraVatTuTuBenhNhanChiTiets
        {
            get => _yeuCauTraVatTuTuBenhNhanChiTiets ?? (_yeuCauTraVatTuTuBenhNhanChiTiets = new List<YeuCauTraVatTuTuBenhNhanChiTiet>());
            protected set => _yeuCauTraVatTuTuBenhNhanChiTiets = value;
        }

        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiets;
        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiets ?? (_nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiets = value;
        }

        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiets = value;
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

        private ICollection<YeuCauXuatKhoDuocPham> _yeuCauXuatKhoDuocPhams;
        public virtual ICollection<YeuCauXuatKhoDuocPham> YeuCauXuatKhoDuocPhams
        {
            get => _yeuCauXuatKhoDuocPhams ?? (_yeuCauXuatKhoDuocPhams = new List<YeuCauXuatKhoDuocPham>());
            protected set => _yeuCauXuatKhoDuocPhams = value;
        }
        private ICollection<YeuCauDieuChuyenDuocPham> _yeuCauDieuChuyenDuocPhamNhaps;
        public virtual ICollection<YeuCauDieuChuyenDuocPham> YeuCauDieuChuyenDuocPhamNhaps
        {
            get => _yeuCauDieuChuyenDuocPhamNhaps ?? (_yeuCauDieuChuyenDuocPhamNhaps = new List<YeuCauDieuChuyenDuocPham>());
            protected set => _yeuCauDieuChuyenDuocPhamNhaps = value;
        }

        private ICollection<YeuCauDieuChuyenDuocPham> _yeuCauDieuChuyenDuocPhamXuats;
        public virtual ICollection<YeuCauDieuChuyenDuocPham> YeuCauDieuChuyenDuocPhamXuats
        {
            get => _yeuCauDieuChuyenDuocPhamXuats ?? (_yeuCauDieuChuyenDuocPhamXuats = new List<YeuCauDieuChuyenDuocPham>());
            protected set => _yeuCauDieuChuyenDuocPhamXuats = value;
        }

        private ICollection<YeuCauXuatKhoVatTu> _yeuCauXuatKhoVatTus;
        public virtual ICollection<YeuCauXuatKhoVatTu> YeuCauXuatKhoVatTus
        {
            get => _yeuCauXuatKhoVatTus ?? (_yeuCauXuatKhoVatTus = new List<YeuCauXuatKhoVatTu>());
            protected set => _yeuCauXuatKhoVatTus = value;
        }
    }
}
