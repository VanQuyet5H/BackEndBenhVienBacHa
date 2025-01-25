using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Api.Models.DichVuGiuong;

namespace Camino.Api.Models.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienViewModel : BaseViewModel
    {
        public DichVuGiuongBenhVienViewModel()
        {
            //KhoaPhongIds = new List<long>();
            //NoiThucHienIds = new List<string>();
        }
        public long? DichVuGiuongId { get; set; }
        public string DichVuGiuongModelText { get; set; }

        public string Ma { get; set; }
        public string Ten { get; set; }

        public List<long> KhoaPhongIds { get; set; }
        public List<string> NoiThucHienIds { get; set; }
        public Enums.EnumLoaiGiuong? LoaiGiuong { get; set; }
        public string LoaiGiuongText { get; set; }
        public string MoTa { get; set; }

        public bool HieuLuc { get; set; }
        public bool? AnhXa { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        public DichVuGiuongViewModel DichVuGiuong { get; set; }
        public List<DichVuGiuongBenhVienGiaBaoHiemViewModel> DichVuGiuongBenhVienGiaBaoHiems { get; set; }
        public List<DichVuGiuongBenhVienGiaBenhVienViewModel> DichVuGiuongBenhVienGiaBenhViens { get; set; }
    }
}
