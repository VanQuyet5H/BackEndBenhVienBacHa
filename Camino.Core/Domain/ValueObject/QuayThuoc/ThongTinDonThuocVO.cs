using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
    public class ThongTinDonThuocVO
    {
        public ThongTinDonThuocVO(){

            DanhSachDonThuocs = new List<ThongTinDuocPhamQuayThuocVo>();
            DanhSachCongNos = new List<CongNoVo>();
        }
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
        public decimal? SoTienCongNo { get; set; }
        //Danh sách don thuoc
        public List<ThongTinDuocPhamQuayThuocVo> DanhSachDonThuocs { get; set; }

        //Danh sách công nợ 
        public List<CongNoVo> DanhSachCongNos { get; set; }

        //BVHD-3951
        public string GhiChu { get; set; }
    }

    public class CongNoVo
    {
        public long CongTyCongNoId { get; set; }
        public decimal SoTienCongNo { get; set; }
    }

    public class XemTruocBangKeThuoc
    {
        public string KhachVangLai { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }

        public long YeuCauTiepNhanId { get; set; }
        public string Hosting { get; set; }
        public List<ThongTinDuocPhamQuayThuocVo> DanhSachDonThuocs { get; set; }
    }
}
