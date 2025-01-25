using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuCongKhaiVatTu
{
    public class PhieuCongKhaiVatTuViewModel : BaseViewModel
    {
        public PhieuCongKhaiVatTuViewModel()
        {
            FileChuKy = new List<FileChuKyPhieuCongKhaiVatTuModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiVatTuModel> FileChuKy { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
    }
    public class FileChuKyPhieuCongKhaiVatTuModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichVatTu { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
}
