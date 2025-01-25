using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class InHoSoKhamSucKhoeVo
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public string TenFile { get; set; }
        public string HostingName { get; set; }
        public Enums.LoaiHoSoKhamSucKhoe LoaiHoSoKhamSucKhoe { get; set; }
        public bool? NoFooter { get; set; }
    }

    public class HoSoKhamSucKhoeOInfoVo
    {
        public string Header { get; set; }
        public string LogoUrl { get; set; }
        public string DonViKham { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string MaNhanVien { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public string ViTriCongTac { get; set; }
        public string SoDoDichVuKham { get; set; }
    }

    public class DichVuKhamSucKhoeNhanVien
    {
        public int STT { get; set; }
        public string TenDichVu { get; set; }
        public string PhongThucHien { get; set; }
        public string Khoa { get; set; }
        public string Tang { get; set; }
        public string NoiThucHien => $"{PhongThucHien} - {Khoa} - {Tang}";
    }

    public class DichVuKhamSucKhoeNhanVienTheoSoDo
    {
        public DichVuKhamSucKhoeNhanVienTheoSoDo()
        {
            DichVuKhamSucKhoeNhanViens = new List<DichVuKhamSucKhoeNhanVien>();
        }
       public List<DichVuKhamSucKhoeNhanVien> DichVuKhamSucKhoeNhanViens { get; set; }
    }
}
