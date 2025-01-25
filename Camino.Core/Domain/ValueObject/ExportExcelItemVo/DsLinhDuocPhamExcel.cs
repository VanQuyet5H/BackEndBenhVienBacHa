using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DsLinhDuocPhamExcel 
    {
        [Width(30)]
        public string MaPL { get; set; }
        [Width(30)]
        public string Loai { get; set; }
        [Width(30)]
        public string NguoiYeuCau { get; set; }
        [Width(30)]
        public string LinhTuKho { get; set; }
        [Width(30)]
        public string LinhNgayYeuCauHienThiTuKho { get; set; }
        [Width(30)]
        public string LinhVeKho { get; set; }
        [Width(30)]
        public string TinhTrang { get; set; }
        [Width(30)]
        public string Nguoiduyet { get; set; }

        [Width(30)]
        public string NgayDuyetString { get; set; }
        [Width(30)]
        public string NgayYeuCauString {get; set; }
    }
}
