using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class PhieuKhamGayMeTruocMoViewModel
    {
        public PhieuKhamGayMeTruocMoViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public double? Nang { get; set; }

        public double? Cao { get; set; }

        public string Asa { get; set; }

        public string Mallampati { get; set; }

        public string KcCamGiap { get; set; }

        public string HaMieng { get; set; }

        public int? RangGia { get; set; }

        public int? GioMoTruocBuaAn { get; set; }

        public bool? CapCuu { get; set; }

        public bool? DaDayDay { get; set; }

        public string ChanDoan { get; set; }

        public string HuongDieuTri { get; set; }

        public string TienSuNoiKhoa { get; set; }

        public string TienSuNgoaiKhoa { get; set; }

        public string TienSuGayMe { get; set; }

        public string DiUng { get; set; }

        public bool? ThuocLa { get; set; }

        public bool? Ruou { get; set; }

        public bool? MaTuy { get; set; }

        public string BenhLayNhiem { get; set; }

        public string ThuocDangDieuTri { get; set; }

        public string KhamLamSang { get; set; }

        public string TuanHoan { get; set; }

        public string M { get; set; }

        public string Ha { get; set; }

        public string HoHap { get; set; }

        public string ThanKinh { get; set; }

        public string CotSong { get; set; }

        public string XnBatThuong { get; set; }

        public string YcBoSung { get; set; }

        public string DkCachThuocVc { get; set; }

        public string DkGiamDauSauMo { get; set; }

        public string YlenhTruocMo { get; set; }

        public string ThuocTienMe { get; set; }

        public DateTime? NgayKham { get; set; }

        public DateTime? NgayHenKham { get; set; }

        public string DdThucHien { get; set; }

        public string BsGmHs { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
    }
}
