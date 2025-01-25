using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuCongKhaiVatTu
{
    public class PhieuCongKhaiVatTu
    {
        public PhieuCongKhaiVatTu()
        {
            DanhSachVatTus = new List<PhieuCongKhaiVatTuGridVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayVaoVien { get; set; }
        public DateTime NgayRaVien { get; set; }

        //BVHD-3876
        public List<PhieuCongKhaiVatTuGridVo> DanhSachVatTus { get; set; }
    }
    public class PhieuCongKhaiVatTuObject : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long NoiThucHienId { get; set; }
        public List<FileChuKyPhieuCongKhaiVatTunGridVo> ListFile { get; set; }
    }
    public class FileChuKyPhieuCongKhaiVatTunGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
    public class InPhieuCongKhaiVatTu
    {
        public string ChanDoan { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
        public List<Table> TableString { get; set; }
    }
    public class Table
    {
        public string Html { get; set; }
    }
}
