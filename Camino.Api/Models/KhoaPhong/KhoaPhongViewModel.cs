using System.Collections.Generic;
using Camino.Api.Models.KhoaPhongChuyenKhoa;
using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhoaPhong
{
    public class KhoaPhongViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public Enums.EnumLoaiKhoaPhong LoaiKhoaPhong { get; set; }

        public bool? IsDisabled { get; set; }

        public string MoTa { get; set; }

        public string TenLoaiKhoaPhong { get; set; }

        public bool? CoKhamNgoaiTru { get; set; }
        public bool? CoKhamNoiTru { get; set; }

        public long? SoTienThuTamUng { get; set; }

        public string KieuKhamDisplay { get; set; }
        public PhongBenhVienViewModel PhongKham { get; set; }

        public int? SoGiuongKeHoach { get; set; }

        public List<long> KhoaIds { get; set; }
        public List<KhoaPhongChuyenKhoaViewModel> KhoaPhongChuyenKhoas { get; set; }
    }   
}
