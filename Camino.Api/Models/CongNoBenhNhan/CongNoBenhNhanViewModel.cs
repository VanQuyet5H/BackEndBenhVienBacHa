using System;

namespace Camino.Api.Models.CongNoBenhNhan
{
    public class CongNoBenhNhanViewModel : BaseViewModel
    {
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }
    }
}
