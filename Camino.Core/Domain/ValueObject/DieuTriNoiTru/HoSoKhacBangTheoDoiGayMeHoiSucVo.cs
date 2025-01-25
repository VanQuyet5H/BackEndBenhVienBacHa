using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class DanhSachBangTheoDoiGayMeHoiSucGridVo : GridItem
    {
        public DateTime NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien.ApplyFormatDate();
        public string LoaiMo { get; set; }
        public string NguoiMo { get; set; }
        public string ThongTinHoSo { get; set; }
    }

    public class HoSoKhacBangTheoDoiGayMeHoiSucVo
    {
        public string ChanDoan { get; set; }
        public string TienMe { get; set; }
        public string TacDung { get; set; }
        public string LoaiMo { get; set; }
        public string TuTheMo { get; set; }
        public string NguoiGayMe { get; set; }
        public string NguoiMo { get; set; }
        public string PhuongPhapVoCam { get; set; }
        public string NguoiThuCheo { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public int? Nang { get; set; }
        public int? Cao { get; set; }
        public string KetLuan { get; set; }
        public string MaSoThongTinVienPhi { get; set; }
    }

    public class HoSoKhacBangInTheoDoiGayMeHoiSucVo
    {
        public string MaSoTTVPhi { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string ChanDoan { get; set; }
        public int? Ngay { get; set; }
        public int? Thang { get; set; }
        public string Nam { get; set; } //20xx
        public string TienMe { get; set; }
        public string TacDung { get; set; }
        public int? Nang { get; set; }
        public int? Cao { get; set; }
        public string LoaiMo { get; set; }
        public string TuTheMo { get; set; }
        public string NguoiGayMe { get; set; }
        public string NguoiMo { get; set; }
        public string PhuongPhapVoCam { get; set; }
        public string NhomMau { get; set; }
        public string NguoiThuCheo { get; set; }
    }
}
