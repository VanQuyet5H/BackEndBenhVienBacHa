using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.VatTu
{
    public class VatTuTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName => Ten;

        public string Ma { get; set; }

        public string Ten { get; set; }
        public int? Rank { get; set; }
        public bool? SuDungTaiBenhVien { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string MaTaiBenhVien { get; set; }
        public Enums.LoaiSuDung? LoaiSuDungId { get; set; }
        public string LoaiSuDungText => LoaiSuDungId.GetDescription();
    }
}
