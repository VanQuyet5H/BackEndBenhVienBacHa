using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class BangHuongDanKhamSucKhoeInfoVo
    {
        public long Id { get; set; }
        public bool HasHeader { get; set; }
        public string HostingName { get; set; }
    }

    public class BangHuongDanKhamSucKhoeDetailVo
    {
        public string Header { get; set; }
        public string LogoUrl { get; set; }
        public string DonViKham { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string NamSinh { get; set; }
        public string MaNhanVien { get; set; }
        public string ChucVu { get; set; }
        public string GhiChu { get; set; }
        public string ViTriCongTac { get; set; }
        public string SoDoDichVuKham { get; set; }
    }

    public class DichVuKhamTrongSoDoVo
    {
        public string TenDichVu { get; set; }
        public string NoiThucHien { get; set; }
    }
}
