using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.Grid;
namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class ADRGridVo : GridItem
    {
        public string TenThuocHoacHoatChat1 { get; set; }
        public string TenThuocHoacHoatChat2 { get; set; }
        public string Ma1ThuocHoacHoatChat { get; set; }
        public string MaTenHoatChat1 { get; set; }
        public string MaTenHoatChat2 { get; set; }
        public string Ma2ThuocHoacHoatChat { get; set; }
        public long ThuocHoacHoatChat1Id { get; set; }
        public long ThuocHoacHoatChat2Id { get; set; }
        public Enums.MucDoChuYKhiChiDinh MucDoChuYKhiChiDinh { get; set; }
        public string MucDoChuYKhiChiDinhDisplay { get; set; }
        public Enums.MucDoTuongTac MucDoTuongTac { get; set; }
        public string MucDoTuongTacDisplay { get; set; }
        public bool? DuocDongHoc { get; set; }
        public string DuocDongHocDisplay { get; set; }
        public bool? DuocLucHoc { get; set; }
        public string DuocLucHocDisplay { get; set; }
        public bool? ThuocThucAn { get; set; }
        public string ThuocThucAnDisplay { get; set; }
        public bool? QuyTac { get; set; }
        public string QuyTacDisplay { get; set; }
        public string TuongTacHauQua { get; set; }
        public string CachXuLy { get; set; }
        public string GhiChu { get; set; }

        public virtual ThuocHoacHoatChat ThuocHoacHoatChat { get; set; }

    }
    public class HoatChatTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }
}
