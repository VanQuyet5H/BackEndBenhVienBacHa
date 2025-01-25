using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoSoLieuTinhThoiGianSuDungDVCuaKHGridVo: GridItem
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public bool LaCungNgay => TuNgay.Date == DenNgay.Date;

        public string HoTenKH { get; set; }
        public string MaBN { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemTN { get; set; }
        public string ThoiDiemTNStr => LaCungNgay ? ThoiDiemTN.ApplyFormatTime() : ThoiDiemTN.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemBSKham { get; set; }
        public string ThoiDiemBSKhamStr => LaCungNgay ? ThoiDiemBSKham?.ApplyFormatTime() : ThoiDiemBSKham?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemRaChiDinh { get; set; }
        public string ThoiDiemRaChiDinhStr => LaCungNgay ? ThoiDiemRaChiDinh?.ApplyFormatTime() : ThoiDiemRaChiDinh?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemLayMauXN { get; set; }
        public string ThoiDiemLayMauXNStr => LaCungNgay ? ThoiDiemLayMauXN?.ApplyFormatTime() : ThoiDiemLayMauXN?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemTraKetQuaXN { get; set; }
        public string ThoiDiemTraKQXNStr => LaCungNgay ? ThoiDiemTraKetQuaXN?.ApplyFormatTime() : ThoiDiemTraKetQuaXN?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemThucHienCLS { get; set; }
        public string ThoiDiemCDHAStr => LaCungNgay ? ThoiDiemThucHienCLS?.ApplyFormatTime() : ThoiDiemThucHienCLS?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemBacSiKetLuan { get; set; }
        public string ThoiDiemKLStr => LaCungNgay ? ThoiDiemBacSiKetLuan?.ApplyFormatTime() : ThoiDiemBacSiKetLuan?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemBacSiKeDonThuoc { get; set; }
        public string ThoiDiemKeDonStr => LaCungNgay ? ThoiDiemBacSiKeDonThuoc?.ApplyFormatTime() : ThoiDiemBacSiKeDonThuoc?.ApplyFormatDateTimeSACH();
    }
}
