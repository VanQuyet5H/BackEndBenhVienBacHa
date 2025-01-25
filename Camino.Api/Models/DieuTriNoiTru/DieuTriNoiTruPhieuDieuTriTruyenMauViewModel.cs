
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriTruyenMauViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? MauVaChePhamId { get; set; }
        public long TheTich { get; set; }
        public EnumNhomMau? NhomMau { get; set; }
        public EnumYeuToRh? YeuToRh { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
    }
}
