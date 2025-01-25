using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.KhoaPhong;
using Camino.Api.Models.NhanVien;
using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using System;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class YeuCauKhamBenhViewModel : BaseViewModel
    {
        public string MaYeuCauKhamBenh { get; set; }
        public long BenhNhanId { get; set; }
        public long LyDoKhamBenhId { get; set; }
        public bool? DuocChuyenVien { get; set; }

        public string LoiDanCuaBacSi { get; set; }

        public long? GiayChuyenVienId { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }
        public bool? LaTaiKham { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public string ThoiGianTiepNhan { get; set; }
        public string GhiChuTrieuChungLamSang { get; set; }
        //public long? DichVuKhamBenhId { get; set; }
        //public long? KhoaPhongId { get; set; }
        public long? BacSiChiDinhId { get; set; }
        //public GiayChuyenVienViewModel GiayChuyenVien { get; set; }
        public PhongBenhVienViewModel PhongKham { get; set; }
        public BenhNhanTiepNhanBenhNhanViewModel BenhNhan { get; set; }
        public DichVuKhamBenhViewModel DichVuKhamBenh { get; set; }
        public KhoaPhongViewModel KhoaPhong { get; set; }
        public NhanVienViewModel BacSiChiDinh { get; set; }
    }
}
