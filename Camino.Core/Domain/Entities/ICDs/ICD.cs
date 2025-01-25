using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauNhapViens;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class ICD : BaseICDEntity
    {
        public long LoaiICDId { get; set; }

        //public int PhanLoaiTheoGioiTinhBenhNhan { get; set; }
        public string ChuanDoanLamSan { get; set; }
        public string ThongTinThamKhaoChoBenhNhan { get; set; }
        public string TenGoiKhac { get; set; }
        public bool? HieuLuc { get; set; }
        public string LoiDanCuaBacSi { get; set; }

        /// <summary>
        /// update Entity 27/10/2020
        /// </summary>
        /// 
        public bool? ManTinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public bool? BenhThuongGap { get; set; }
        public bool? BenhNam { get; set; }
        public bool? KhongBaoHiem { get; set; }
        public bool? NgoaiDinhSuat { get; set; }
        public long? KhoaId { get; set; }
        public string MoTa { get; set; }
        public string MaChiTiet { get; set; }

        /// end 

        public virtual LoaiICD LoaiICD { get; set; }
        public virtual Khoa Khoa { get; set; }


        private ICollection<ICDDoiTuongBenhNhanChiTiet> _iCDDoiTuongBenhNhanChiTiets;
        public virtual ICollection<ICDDoiTuongBenhNhanChiTiet> ICDDoiTuongBenhNhanChiTiets
        {
            get => _iCDDoiTuongBenhNhanChiTiets ?? (_iCDDoiTuongBenhNhanChiTiets = new List<ICDDoiTuongBenhNhanChiTiet>());
            protected set => _iCDDoiTuongBenhNhanChiTiets = value;
        }

        private ICollection<ChuanDoanLienKetICD> _chuanDoanLienKetICDs;
        public virtual ICollection<ChuanDoanLienKetICD> ChuanDoanLienKetICDs
        {
            get => _chuanDoanLienKetICDs ?? (_chuanDoanLienKetICDs = new List<ChuanDoanLienKetICD>());
            protected set => _chuanDoanLienKetICDs = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs { get; set; }
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }


        private ICollection<YeuCauKhamBenhICDKhac> _yeuCauKhamBenhICDKhacs;
        public virtual ICollection<YeuCauKhamBenhICDKhac> YeuCauKhamBenhICDKhacs
        {
            get => _yeuCauKhamBenhICDKhacs ?? (_yeuCauKhamBenhICDKhacs = new List<YeuCauKhamBenhICDKhac>());
            protected set => _yeuCauKhamBenhICDKhacs = value;
        }


        #region Update 12/2/2020

        //private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        //{
        //    get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
        //    protected set => _yeuCauTiepNhans = value;
        //}

        #endregion Update 12/2/2020

        private ICollection<ToaThuocMau> _toaThuocMaus;
        public virtual ICollection<ToaThuocMau> ToaThuocMaus
        {
            get => _toaThuocMaus ?? (_toaThuocMaus = new List<ToaThuocMau>());
            protected set => _toaThuocMaus = value;
        }

        //public virtual ICollection<ToaThuocMau> ToaThuocMau { get; set; }
        //public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhan { get; set; }
        //public virtual ICollection<YeuCauTiepNhanIcdkhac> YeuCauTiepNhanICDkhac { get; set; }


        private ICollection<YeuCauKhamBenhChanDoanPhanBiet> _yeuCauKhamBenhChanDoanPhanBiets;
        public virtual ICollection<YeuCauKhamBenhChanDoanPhanBiet> YeuCauKhamBenhChanDoanPhanBiets
        {
            get => _yeuCauKhamBenhChanDoanPhanBiets ?? (_yeuCauKhamBenhChanDoanPhanBiets = new List<YeuCauKhamBenhChanDoanPhanBiet>());
            protected set => _yeuCauKhamBenhChanDoanPhanBiets = value;
        }

        private ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats;
        public virtual ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> YeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats
        {
            get => _yeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats ?? (_yeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats = new List<YeuCauDichVuKyThuatTuongTrinhPTTT>());
            protected set => _yeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats = value;
        }

        private ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> _yeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats;
        public virtual ICollection<YeuCauDichVuKyThuatTuongTrinhPTTT> YeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats
        {
            get => _yeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats ?? (_yeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats = new List<YeuCauDichVuKyThuatTuongTrinhPTTT>());
            protected set => _yeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats = value;
        }

        private ICollection<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTris;
        public virtual ICollection<NoiTruKhoaPhongDieuTri> NoiTruKhoaPhongDieuTris
        {
            get => _noiTruKhoaPhongDieuTris ?? (_noiTruKhoaPhongDieuTris = new List<NoiTruKhoaPhongDieuTri>());
            protected set => _noiTruKhoaPhongDieuTris = value;
        }
        
        private ICollection<NoiTruPhieuDieuTri> _noiTruPhieuDieuTris;
        public virtual ICollection<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris
        {
            get => _noiTruPhieuDieuTris ?? (_noiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>());
            protected set => _noiTruPhieuDieuTris = value;
        }
        
        private ICollection<NoiTruThamKhamChanDoanKemTheo> _noiTruThamKhamChanDoanKemTheos;
        public virtual ICollection<NoiTruThamKhamChanDoanKemTheo> NoiTruThamKhamChanDoanKemTheos
        {
            get => _noiTruThamKhamChanDoanKemTheos ?? (_noiTruThamKhamChanDoanKemTheos = new List<NoiTruThamKhamChanDoanKemTheo>());
            protected set => _noiTruThamKhamChanDoanKemTheos = value;
        }
        
        private ICollection<YeuCauNhapVienChanDoanKemTheo> _yeuCauNhapVienChanDoanKemTheos;
        public virtual ICollection<YeuCauNhapVienChanDoanKemTheo> YeuCauNhapVienChanDoanKemTheos
        {
            get => _yeuCauNhapVienChanDoanKemTheos ?? (_yeuCauNhapVienChanDoanKemTheos = new List<YeuCauNhapVienChanDoanKemTheo>());
            protected set => _yeuCauNhapVienChanDoanKemTheos = value;
        }

        private ICollection<YeuCauNhapVien> _yeuCauNhapViens;
        public virtual ICollection<YeuCauNhapVien> YeuCauNhapViens
        {
            get => _yeuCauNhapViens ?? (_yeuCauNhapViens = new List<YeuCauNhapVien>());
            protected set => _yeuCauNhapViens = value;
        }

        private ICollection<NoiTruBenhAn> _noiTruBenhAns;
        public virtual ICollection<NoiTruBenhAn> NoiTruBenhAns
        {
            get => _noiTruBenhAns ?? (_noiTruBenhAns = new List<NoiTruBenhAn>());
            protected set => _noiTruBenhAns = value;
        }

        public ICollection<NhomICDLienKetICDTheoBenhVien> _nhomICDLienKetICDTheoBenhViens;
        public virtual ICollection<NhomICDLienKetICDTheoBenhVien> NhomICDLienKetICDTheoBenhViens
        {
            get => _nhomICDLienKetICDTheoBenhViens ?? (_nhomICDLienKetICDTheoBenhViens = new List<NhomICDLienKetICDTheoBenhVien>());
            protected set => _nhomICDLienKetICDTheoBenhViens = value;
        }
    }
}
