using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ThongTinThuePhongVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long? ThuePhongId { get; set; }
        public long? CauHinhThuePhongId { get; set; }
        public bool CoThuePhong { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
        public DateTime ThoiDiemKetThuc { get; set; }
    }

    //Cập nhật 06/06/2022: kiểm tra thuê phòng theo thời điểm tiếp nhận ngoại trú, nếu không có thì lấy thời điểm tạo bệnh án
    public class LookupThoiDiemTiepNhanThuePhongVo
    {
        public bool LaNoiTru { get; set; }
        public bool CoTiepNhanNgoaiTru { get; set; }
        public DateTime? ThoiDiemTaoBenhAn { get; set; }
        public DateTime? ThoiDiemTiepNhanCanQuyetToan { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
    }
}
