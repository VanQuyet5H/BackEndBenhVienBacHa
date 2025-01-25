using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DanTocs;
using Camino.Core.Domain.Entities.DonViHanhChinhs;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens;
using Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens;
using Camino.Core.Domain.Entities.NgheNghieps;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class HopDongKhamSucKhoeNhanVien : BaseEntity
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string HoTenKhac { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public string Email { get; set; }
        public string TenDonViHoacBoPhan { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public long GoiKhamSucKhoeId { get; set; }
        public bool DaLapGiaDinh { get; set; }
        public bool CoMangThai { get; set; }
        public string GhiChuTienSuBenh { get; set; }
        public string GhiChuDiUngThuoc { get; set; }
        public int? STTNhanVien { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DiaChiDayDu { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }
        public bool? HuyKham { get; set; }

        //update add properties in 15/03/2021.
        public DateTime? NgayCapChungMinhThu { get; set; }
        public string NoiCapChungMinhThu { get; set; }
        public DateTime? NgayBatDauLamViec { get; set; }
        public string NgheCongViecTruocDay { get; set; }
        public long? HoKhauTinhThanhId { get; set; }
        public long? HoKhauQuanHuyenId { get; set; }
        public long? HoKhauPhuongXaId { get; set; }
        public string HoKhauDiaChi { get; set; }

        public virtual HopDongKhamSucKhoe HopDongKhamSucKhoe { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NgheNghiep NgheNghiep { get; set; }
        public virtual QuocGia QuocTich { get; set; }

        public virtual DanToc DanToc { get; set; }
        public virtual DonViHanhChinh PhuongXa { get; set; }
        public virtual DonViHanhChinh QuanHuyen { get; set; }
        public virtual DonViHanhChinh TinhThanh { get; set; }


        public virtual DonViHanhChinh HoKhauPhuongXa { get; set; }
        public virtual DonViHanhChinh HoKhauQuanHuyen { get; set; }
        public virtual DonViHanhChinh HoKhauTinhThanh { get; set; }

        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans { get; set; }
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> _goiKhamSucKhoeChungDichVuKhamBenhNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien> GoiKhamSucKhoeChungDichVuKhamBenhNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens ?? (_goiKhamSucKhoeChungDichVuKhamBenhNhanViens = new List<GoiKhamSucKhoeChungDichVuKhamBenhNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhNhanViens = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> _goiKhamSucKhoeChungDichVuKyThuatNhanViens;

        public virtual ICollection<GoiKhamSucKhoeChungDichVuKyThuatNhanVien> GoiKhamSucKhoeChungDichVuKyThuatNhanViens
        {
            get => _goiKhamSucKhoeChungDichVuKyThuatNhanViens ?? (_goiKhamSucKhoeChungDichVuKyThuatNhanViens = new List<GoiKhamSucKhoeChungDichVuKyThuatNhanVien>());
            protected set => _goiKhamSucKhoeChungDichVuKyThuatNhanViens = value;
        }
    }
}
