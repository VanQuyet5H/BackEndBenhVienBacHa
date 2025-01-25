using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BHYT
{
    public class ThongTinBHYTVO
    {
        public string maKetQua { get; set; }
        public string ghiChu { get; set; }
        public string maThe { get; set; }
        public string hoTen { get; set; }
        public string ngaySinh { get; set; }
        public string gioiTinh { get; set; }
        public string diaChi { get; set; }
        public string maDKBD { get; set; }
        public string cqBHXH { get; set; }
        public string gtTheTu { get; set; }
        public string gtTheDen { get; set; }
        public string maKV { get; set; }
        public string ngayDu5Nam { get; set; }
        public string maSoBHXH { get; set; }
        public string maTheCu { get; set; }
        public string maTheMoi { get; set; }
        public string gtTheTuMoi { get; set; }
        public string gtTheDenMoi { get; set; }
        public List<dsLichSuKCB2018VO> dsLichSuKCB2018 { get; set; }
        public List<dsLichSuKT2018VO> dsLichSuKT2018 { get; set; }

        public bool isConnectSuccessfully { get; set; } = true;
    }
    
    public class dsLichSuKT2018VO
    {
        public string userKT { get; set; }
        public string thoiGianKT { get; set; }
        public string thongBao { get; set; }
        public string maLoi { get; set; }

        public string tenCSKCB { get; set; }
        public string maCSKCB { get; set; }

        public string thoiGianKTDisplay { get; set; }
        public DateTime? thoiGianKTDateTime { get; set; }
    }
    public class dsLichSuKCB2018VO
    {
        public string maHoSo { get; set; }
        public string maCSKCB { get; set; }
        public string ngayVao { get; set; }
        public string ngayVaoDisplay { get; set; }
        public string ngayRa { get; set; }
        public string ngayRaDisplay { get; set; }
        public string tenBenh { get; set; }
        public string tinhTrang { get; set; }
        public string tinhTrangDisplay { get; set; }
        public string kqDieuTri { get; set; }
        public string kqDieuTriDisplay { get; set; }
        public string lyDoVV { get; set; }
        public string lyDoVVDisplay { get; set; }
        public string TEMP1 { get; set; }
        public string TEMP2 { get; set; }
        public string TEMP3 { get; set; }
        public string TEMP4 { get; set; }
        public string TEMP5 { get; set; }
        public string coSoKCB { get; set; }

        public DateTime? ngayVaoDateTime { get; set; }
        public DateTime? ngayRaDateTime { get; set; }
    }
    public class ThongTinTokenMoiVO
    {
        public string maKetQua { get; set; }
        public APIKeyVO APIKey { get; set; }

    }
    public class APIKeyVO
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
    }
}
