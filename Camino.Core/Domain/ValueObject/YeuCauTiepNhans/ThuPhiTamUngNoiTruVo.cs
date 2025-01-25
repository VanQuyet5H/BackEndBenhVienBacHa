using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThuPhiTamUngNoiTruVo
    {
        public ThuPhiTamUngNoiTruVo()
        {
            DanhSachKhamChuaBenhs = new List<ChiPhiKhamChuaBenhNoiTruVo>();
        }
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public DateTime NgayThuTamUng { get; set; }
        public string NoiDungThuTamUng { get; set; }

        //phần này bổ sung danh sách dịch vụ 
        public List<ChiPhiKhamChuaBenhNoiTruVo> DanhSachKhamChuaBenhs { get; set; }
    }
}
