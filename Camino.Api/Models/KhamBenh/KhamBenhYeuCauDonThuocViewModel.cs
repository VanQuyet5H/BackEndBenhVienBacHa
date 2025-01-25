using Camino.Api.Models.NhanVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.ChiSoXetNghiems;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauDonThuocViewModel:BaseViewModel
    {
        public KhamBenhYeuCauDonThuocViewModel()
        {
            KhamBenhYeuCauDonThuocChiTiets = new List<KhamBenhYeuCauDonThuocChiTietViewModel>();
        }
        //public long YeuCauTiepNhanId { get; set; }
        public int LoaiDonThuoc { get; set; }
        public int TrangThai { get; set; }
        public bool DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public int? PhuongThucThanhToan { get; set; }
        public long BacSiKeDonId { get; set; }
        public long NoiKeDonId { get; set; }
        public DateTime ThoiDiemKeDon { get; set; }
        public long? NoiCapThuocId { get; set; }
        public long? NhanVienCapThuocId { get; set; }
        public DateTime? ThoiDiemCapThuoc { get; set; }
        public string GhiChu { get; set; }

        public long YeuCauKhamBenhId { get; set; }
        public long? ToaThuocMauId { get; set; }


       // public virtual PhongBenhVien BacSiKeDon { get; set; }
        //public virtual NhanVienViewModel NhanVienCapThuoc { get; set; }
        //public virtual NhanVienViewModel NhanVienThanhToan { get; set; }
        //public virtual PhongBenhVien NoiCapThuoc { get; set; }
        //public virtual NhanVienViewModel NoiKeDon { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        //public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public KhamBenhYeuCauKhamBenhViewModel YeuCauKhamBenh { get; set; }
        public List<KhamBenhYeuCauDonThuocChiTietViewModel> KhamBenhYeuCauDonThuocChiTiets { get; set; }
    }

    public class KhamBenhYeuCauDonVTYTViewModel : BaseViewModel
    {
        public KhamBenhYeuCauDonVTYTViewModel()
        {
            KhamBenhYeuCauDonVTYTChiTiets = new List<KhamBenhYeuCauDonVTYTChiTietViewModel>();
        }
        public long YeuCauKhamBenhId { get; set; }
        public Enums.EnumTrangThaiDonVTYT TrangThai { get; set; }
        public long BacSiKeDonId { get; set; }
        public long NoiKeDonId { get; set; }
        public DateTime ThoiDiemKeDon { get; set; }
        public string GhiChu { get; set; }
        public KhamBenhYeuCauKhamBenhViewModel YeuCauKhamBenh { get; set; }
        public List<KhamBenhYeuCauDonVTYTChiTietViewModel> KhamBenhYeuCauDonVTYTChiTiets { get; set; }
    }
}
