using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs
{
    public class PhieuInLichSuKhamQueryInfo
    {
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiLichSuKhamChuaBenh? LoaiLichSuKhamChuaBenh { get; set; }
        public Enums.LoaiLichSuKhamChuaBenhChiTiet? LoaiLichSuKhamChuaBenhChiTiet { get; set; }
        public long? YeuCauDichVuId { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string Hosting { get; set; }
        public bool HienLichSuNoiTru { get; set; }
    }
}
