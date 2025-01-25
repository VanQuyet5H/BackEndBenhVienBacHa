using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class ThongTinBHYTViewModel
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
        public List<dsLichSuKCB2018> dsLichSuKCB2018 { get; set; }
        public List<dsLichSuKT2018> dsLichSuKT2018 { get; set; }
    }
    public class dsLichSuKT2018
    {
        public string userKT { get; set; }
        public string thoiGianKT { get; set; }
        public string thongBao { get; set; }
        public string maLoi { get; set; }
    }
    public class userConfirmViewModel
    {
        public string userName { get; set; }
        public string pass { get; set; }
    }
    public class dsLichSuKCB2018
    {
        public string maHoSo { get; set; }
        public string maCSKCB{ get; set; }
        public string ngayVao{ get; set; }
        public string ngayRa{ get; set; }
        public string tenBenh{ get; set; }
        public string tinhTrang{ get; set; }
        public string kqDieuTri{ get; set; }
        public string lyDoVV{ get; set; }
        public string TEMP1{ get; set; }
        public string TEMP2{ get; set; }
        public string TEMP3 { get; set; }
        public string TEMP4{ get; set; }
        public string TEMP5{ get; set; }
    }
    public class ThongTinTokenMoiViewModel
    {
        public string maKetQua { get; set; }
        public APIKey APIKey { get; set; }
        
    }
    public class ThongTinHoSoMoiViewModel
    {
        public string maKetQua { get; set; }

    }
    public class APIKey
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
    }
    public class ThongTinBenhNhanModel
    {
        public string MaThe { get; set; }
        public string TenBenhNhan { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int? NamSinh { get; set; }
        
    }
    public class ChiTietLichSuKhamBenhModel
    {
        public string maKetQua { get; set; }
        public hoSoKCB hoSoKCB { get; set; }

    }
    public class hoSoKCB
    {
        public xml1 xml1 { get; set; }
    }
    public class xml1
    {
        public int Id { get; set; }
        public string MaLk { get; set; }
        public string Stt { get; set; }
        public string MaBn { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaThe { get; set; }
        public string MaDkbd { get; set; }
        public string TenBenh { get; set; }
        public string MaBenh { get; set; }
        public string MaBenhkhac { get; set; }
        public string MaLydoVvien { get; set; }
        public string MaNoiChuyen { get; set; }
        public string MaTaiNan { get; set; }
        public string SoNgayDtri { get; set; }
        public string KetQuaDtri { get; set; }
        public string TinhTrangRv { get; set; }

        public string NgayTtoan { get; set; }
        public string MucHuong { get; set; }
        public string TThuoc { get; set; }
        public string TVtyt { get; set; }
        public string TTongchi { get; set; }
        public string TBntt { get; set; }
        public string TBncct { get; set; }
        public string TBhtt { get; set; }
        public string TNguonkhac { get; set; }
        public string TNgoaids { get; set; }
        public string NamQt { get; set; }
        public string ThangQt { get; set; }
        public string MaLoaiKcb { get; set; }
        public string MaKhoa { get; set; }
        public string MaCskcb { get; set; }
        public string MaKhuvuc { get; set; }
        public string MaPtttQt { get; set; }
        public string CanNang { get; set; }
        public int CosokcbId { get; set; }
        public string TinhthanhId { get; set; }
        public string Trangthai { get; set; }
        public string HosoId { get; set; }
        public string Mieuta { get; set; }
        public string Status { get; set; }

        public string NgaySinh { get; set; }
        public string NgayRa { get; set; }
        public string NgayVao { get; set; }
        public string GtTheTu { get; set; }
        public string GtTheDen { get; set; }
        public string Xuattoan { get; set; }
        public string Ngaythanhtoan { get; set; }
        public string Trongmau { get; set; }
        public string LoaiBn { get; set; }
        public string KXetnghiem { get; set; }
        public string KCdhatdcn { get; set; }
        public string KThuoc { get; set; }
        public string KMau { get; set; }
        public string KPttt { get; set; }
        public string KVtyt { get; set; }

        public string HDvkt { get; set; }
        public string HThuoc { get; set; }
        public string HVtyt { get; set; }
        public string Ngaygui { get; set; }
        public string Userid { get; set; }
        public string Tinhtranggui { get; set; }
        public string Ngaynhap { get; set; }
        public string Sohosoid { get; set; }
        public string Sauthangluongcoso { get; set; }
        public string Tienkham { get; set; }
        public string Tiengiuong { get; set; }
        public string Tienvanchuyen { get; set; }
        public string NgaySinhTemp { get; set; }
        public string MaGd { get; set; }
        public string SoPhieu { get; set; }

        public string MaBacsy { get; set; }
        public string TenChame { get; set; }
        public string Du5namlt { get; set; }
        public string Ngaynhan { get; set; }
        public string KyQT { get; set; }
        public string Loidulieu { get; set; }
        public string MaLoi { get; set; }
        public string Loaihoso { get; set; }
        public string QTThe { get; set; }

    }
}