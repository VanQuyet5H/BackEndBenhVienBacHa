using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class DanhSachHuyThanhToanVo
    {
        public DanhSachHuyThanhToanVo()
        {
            DanhSachChiPhiKhamChuaBenhDaChons = new List<ChiPhiKhamChuaBenhVo>();
            DanhSachCongNoCus = new List<DanhSachCongNoCu>();
        } 

        public int YeuCauTiepNhanId { get; set; }
        public decimal? SoTienDaThanhToan { get; set; }
        public decimal? SoTienTraVaoLaiTaiKhoan { get; set; }

        public List<ChiPhiKhamChuaBenhVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
        public List<DanhSachCongNoCu> DanhSachCongNoCus { get; set; }
    }

    public class DanhSachCongNoCu
    {
        public long  CongTyCongNoId { get; set; }
        public string CongTyCongNoName { get; set; }
        public decimal SoTienCongNo { get; set; }
        public decimal? HoanLaiTien { get; set; }
    }

    public class KetQuaXacNhanHuyThanhToanVo
    {
        public int YeuCauTiepNhanId { get; set; }
        //1:..,2:...
        public int CaseValue { get; set; }
        public string Error { get; set; }
        public decimal SoTienDaThanhToan { get; set; }
        public List<ChiPhiKhamChuaBenhVo> DanhSachChiPhiKhamChuaBenhDaChons { get; set; }
        public List<DanhSachCongNoCu> DanhSachCongNoCus { get; set; }
    }
}
