using Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.PhieuInXetNghiem
{
    public class PhieuInXetNghiemVo 
    {
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
    }

    public class ThongTinBacSiVo
    {
        public long? YeuCauKhamBenhId { get; set; }

        public string BacSiChiDinh { get; set; }
        public long BacSiChiDinhId { get; set; }

        public string KhoaPhongChiDinh { get; set; }
        public bool FromLeTan { get; set; }
    }

    public class PhieuInXetNghiemModel
    {
        public string Html { get; set; }        
    }

    public class DuyetKetQuaXetNghiemPhieuInResultVo
    {
        public string DanhSach { get; set; }
        public string LogoUrl { get; set; }

        public string SoPhieu { get; set; }

        public string SoVaoVien { get; set; }

        public string MaYTe { get; set; }

        public string BarCodeImgBase64 { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string GioiTinhDisplay => ShowGioiTinhTxt(GioiTinh);

        private string ShowGioiTinhTxt(Enums.LoaiGioiTinh? gioiTinh)
        {
            if (gioiTinh == null || gioiTinh == Enums.LoaiGioiTinh.ChuaXacDinh)
            {
                return string.Empty;
            }

            return gioiTinh == Enums.LoaiGioiTinh.GioiTinhNam ? "Nam" : "Nữ";
        }

        public string DiaChi { get; set; }

        public string LoaiMau { get; set; }

        public string DoiTuong { get; set; }

        public string BsChiDinh { get; set; }

        public string ChanDoan { get; set; }

        public string GhiChu { get; set; }

        public string KhoaPhong { get; set; }

        public string TgLayMau { get; set; }

        public string NguoiLayMau { get; set; }

        public string TgNhanMau { get; set; }

        public string NguoiNhanMau { get; set; }

        public string NguoiThucHien { get; set; }

        public string KetQuaXetNghiem { get; set; }

        public string Gio { get; set; }

        public string Ngay { get; set; }

        public string Thang { get; set; }

        public string Nam { get; set; }

        public List<ListDataChildVo> ChiTietKetQuaXetNghiems { get; set; }
    }

    public class LichSuYeuCauChayLai
    {
        public string NguoiYeuCau { get; set; }
        public string NgayYeuCau { get; set; }
        public string LyDoYeuCau { get; set; }
        public string NguoiTuChoi { get; set; }
        public string NgayTuChoi { get; set; }
        public string LyDoTuChoi { get; set; }
        public bool? TrangThai { get; set; }
    }
}
