using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien
{
    public class BaoCaoVienPhiThuTienGridVo  :GridItem
    {
        public  int STT { get;set;}
        public DateTime Ngay { get;set;}
        public string NgayString => Ngay.ApplyFormatDateTimeSACH();
        public string SoBLHD { get;set;}
        public string MaYTe { get;set;}
        public string TenBenhNhan { get;set;}
        public string SoBenhAn { get;set;}
        public decimal TamUng { get;set;}
        public decimal HoanUng { get;set;}
        public decimal ThuTien { get;set;}
        public decimal Hoan { get;set;}
        public decimal GoiDichVu { get;set;}
        public decimal CongNo { get;set;}
        public decimal Pos { get;set;}
        public decimal ChuyenKhoan { get;set;}
        public decimal Tienmat { get;set;}
        public string SoPhieuThu { get;set;}
        public decimal PosCapNhat { get;set;}
        public decimal ChuyenKhoanCapNhat { get;set;}
        public decimal TienmatCapNhat { get;set;}  
    }
}
