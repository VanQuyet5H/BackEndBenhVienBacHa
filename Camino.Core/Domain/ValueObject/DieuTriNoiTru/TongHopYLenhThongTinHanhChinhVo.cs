using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class TongHopYLenhThongTinHanhChinhVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string Tuoi => (NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value).ToString();
        public string TenGioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string SoBenhAn { get; set; }
        public string LoaiBenhAn { get; set; }
        public string Khoa { get; set; }
        public string BacSiDieuTri { get; set; }
        public string Phong { get; set; }
        public string Giuong { get; set; }
        public string DoiTuong { get; set; }
        public DateTime? NgayDieuTri { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class ChiTietYLenhQueryInfoVo {
        public long NoiTruBenhAnId { get; set; }
        public string NgayDieuTriStr { get; set; }

        public DateTime NgayDieuTri
        {
            get
            {
                var ngayDieuTri = new DateTime();
                if (!string.IsNullOrEmpty(NgayDieuTriStr))
                {
                    if (!string.IsNullOrEmpty(NgayDieuTriStr))
                    {
                        DateTime.TryParseExact(NgayDieuTriStr, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayDieuTri);
                    }
                }
                return ngayDieuTri.Date;
            }
        }
    }
}
