using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanKhamBenhTatCaPhongTimKiemVo
    {
        public long? CongTyId { get; set; }
        public long? HopDongId { get; set; }
        public string SearchString { get; set; }
        public long? HopDongKhamNhanVienId { get; set; }
    }

    public class KhamDoanTatCaPhongKetQuaMauVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
    }

    public class KetQuaMauDichVuKyThuatDataVo
    {
        public string NoiDung { get; set; }
    }

    public class KhamDoanTatCaPhongDichVuChuaThucHienVo
    {
        public KhamDoanTatCaPhongDichVuChuaThucHienVo()
        {
            DichVuKyThuats = new List<string>();
            DichVuKhamBenhs = new List<string>();
        }
        public List<string> DichVuKyThuats { get; set; }
        public List<string> DichVuKhamBenhs { get; set; }
    }

    public class KhamDoanTatCaPhongKiemTraDichVuChuaThucHienVo
    {
        public KhamDoanTatCaPhongKiemTraDichVuChuaThucHienVo()
        {
            YeuCauKhamBenhDangChonThucHienIds = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<long> YeuCauKhamBenhDangChonThucHienIds { get; set; }
    }
}
