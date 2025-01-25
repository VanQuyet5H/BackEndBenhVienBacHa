using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class GiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienString { get; set; }

        public string CMND { get; set; }
        public string CoQuanCapCMND { get; set; }
        public string SDT { get; set; }
        public long? BSGMHSId { get; set; }
        public string BSGMHSText { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }

    }
    public class InGiayCamKetGayMeGiamDauTrongDeSauPhauThuat
    {
        public string KhoaCreate { get; set; }

        public string HoTen { get; set; }
        public string MaTN { get; set; }
        public string MaNB { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string TenToiLa { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
      
        public string NguoiThan { get; set; }
        public string Khoa { get; set; }
        public string NgayThangNam { get; set; }
        public string NguoiDaiDien { get; set; }
        public string CMND { get; set; }
        public string CoQuanCap { get; set; }
        public string DanToc { get; set; }
        public string QuocTich { get; set; }
        public string NgheNghiep { get; set; }
        public string NoiLamViec { get; set; }
        public string KhiCanBaoTin { get; set; }
        public string NamSinhNguoiThan { get; set; }
        public string DiaChiNguoiThan { get; set; }
        public string CMNDNguoiThan { get; set; }
        public string DTLienLac { get; set; }
        public string BSGayMeHoiSuc { get; set; }
    }
    public class DataInPhieuDieuTriVaSerivcesGiayCamKetGayMeGiamDauTrongDeSauPhauThuatVo
    {
        public string MaSoTiepNhan { get; set; }

        public string Khoa { get; set; }


        public string HoTenNgBenh { get; set; }

        public int? NamSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }

      
        public string Cmnd { get; set; }

        public string GTNgBenh { get; set; }


        public string DiaChi { get; set; }


        public int Ngay { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public string CoQuanCap { get; set; }

        public string DanToc { get; set; }

        public string NgheNghiep { get; set; }

        public string NoiLamViec { get; set; }
        public string QuocTich { get; set; }
    }
}
