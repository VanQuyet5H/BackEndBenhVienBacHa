using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiThuoc
{
    public class PhieuCongKhaiThuoc
    {
        public PhieuCongKhaiThuoc()
        {
            DanhSachThuocs = new List<PhieuCongKhaiThuocGridVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public DateTime NgayRaVien { get; set; }

        //BVHD-3876
        public List<PhieuCongKhaiThuocGridVo> DanhSachThuocs { get; set; }
    }
    public class PhieuCongKhaiThuocObject : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiThuocnGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuCongKhaiThuocnGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InPhieuCongKhaiThuoc
    {
        public string ChanDoan { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public DateTime NgayRaVien { get; set; }
        public List<Table> TableString { get; set; }
    }
    public class Table
    {
        public string Html { get; set; }
    }
}
