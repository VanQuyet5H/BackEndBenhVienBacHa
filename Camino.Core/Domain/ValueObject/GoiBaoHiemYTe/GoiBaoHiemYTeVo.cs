using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GoiBaoHiemYTe
{
    public class GoiBaoHiemYTeVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }       
        public string DiaChi { get; set; }
        public string ThoiGianTiepNhanStr { get; set; }       
        public string MucHuong { get; set; }
        public DateTime Ngay { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool TrangThai { get; set; }
        public string TimKiem { get; set; }
        public string SearchString { get; set; }


    }
    public class GoiBaoHiemYTeDataVo : GridItem
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public bool? QuyetToanTheoNoiTru { get; set; }
        public int? MucHuong { get; set; }
        public List<long> YeuCauKhamBenhBHYTIds { get; set; }

    }
}
