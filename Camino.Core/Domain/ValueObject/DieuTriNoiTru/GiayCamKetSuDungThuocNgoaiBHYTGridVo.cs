using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class GiayCamKetSuDungThuocNgoaiBHYTVo
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienString { get; set; }



        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }

    }
    public class InGiayCamKetSuDungThuocNgoaiBHYT
    {
        public string KhoaCreate { get; set; }

        public string HoTen { get; set; }
        public string MaTN { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string TenToiLa { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string ChanDoan { get; set; }
        public string NguoiThan { get; set; }
        public string Khoa { get; set; }
        public string NgayThangNam { get; set; }
        public string NguoiDaiDien { get; set; }
    }
}
