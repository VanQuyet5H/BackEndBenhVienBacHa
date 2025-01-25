using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DichVuKyThuatViewModel : BaseViewModel
    {
        public string TenChung { get; set; }
        public string TenTiengAnh { get; set; }
        public string MaChung { get; set; }
        public string Ma4350 { get; set; }
        public string MaGia { get; set; }
        public string TenGia { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }
        public string TenNhomChiPhi => NhomChiPhi.GetDescription();
        public long? NhomDichVuKyThuatId { get; set; }
        public string TenNhomDichVuKyThuat { get; set; }
        public Enums.LoaiPhauThuatThuThuat? LoaiPhauThuatThuThuat { get; set; }
        public string TenLoaiPhauThuatThuThuat => LoaiPhauThuatThuThuat.GetDescription();
        public bool? NgoaiQuyDinhBHYT { get; set; }
        public string MoTa { get; set; }

        public List<DichVuKyThuatThongTinGiaViewModel> DichVuKyThuatThongTinGias { get; set; }
    }
}
