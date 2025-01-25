using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class InPhieuChamSocVo
    {
        public long? BenhNhanId { get; set; }
        //public long? PhieuDieuTriId { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public bool? InTatCa { get; set; }

        public string NgayDieuTriStr { get; set; }

        public DateTime NgayDieuTri
        {
            get
            {
                var ngayDieuTri = new DateTime();
                if (!string.IsNullOrEmpty(NgayDieuTriStr))
                {
                    DateTime.TryParseExact(NgayDieuTriStr, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayDieuTri);
                }
                return ngayDieuTri.Date;
            }
        }

        public bool? KhongHienHeader { get; set; }
    }
    public class TongHopYLenhPhieuChamSocVo
    {
        public string Header { get; set; }
        public string Khoa { get; set; }
        public string SoVaoVien { get; set; }
        public string PhieuSo { get; set; }
        public string BenhNhanHoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string BenhNhanTuoi
        {
            get
            {
                var start = DateTime.Now;
                var ngaySinhDayDu = NgaySinh != null && ThangSinh != null && NgaySinh != 0 && ThangSinh != 0 ? new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value) : new DateTime(NamSinh.Value, 1, 1);
                var thangTuoi = ((start.Year * 12 + start.Month) - (ngaySinhDayDu.Year * 12 + ngaySinhDayDu.Month));
                //var chenhLechNgayTuoi = start.Day - ngaySinhDayDu.Day;
                if (thangTuoi > 72)// && chenhLechNgayTuoi >= 0) // 6 tuổi
                {
                    return (DateTime.Now.Year - NamSinh.Value).ToString();
                }
                else
                {
                    return thangTuoi + " tháng";
                }
            }
        }
        public string BenhNhanGioiTinh { get; set; }

        public string SoGiuong { get; set; }
        public string ChanDoan { get; set; }
        public string DanhSachYLenh { get; set; }
        public string Buong { get; set; }
    }

    public class ThongTinInChiTietYLenhVo
    {
        public ThongTinInChiTietYLenhVo()
        {
            DienBiens = new List<NoiTruPhieuDieuTriChiTietDienBien>();
            YLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>();
        }
        public DateTime NgayDieuTri { get; set; }
        public List<NoiTruPhieuDieuTriChiTietDienBien> DienBiens { get; set; }
        public List<NoiTruPhieuDieuTriChiTietYLenh> YLenhs { get; set; }
    }
}
