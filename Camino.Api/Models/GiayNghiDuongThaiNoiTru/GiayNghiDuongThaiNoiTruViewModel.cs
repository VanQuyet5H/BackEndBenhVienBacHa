using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayNghiDuongThaiNoiTru
{
    public class GiayChungNhanNghiDuongThaiViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? NghiTuNgay { get; set; }
        public DateTime? NghiDenNgay { get; set; }
        public DateTime? NgayThucHien { get; set; }
    }
    public class GiayChungNhanNghiDuongThaiQueryInfo
    {
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
    }
  }
