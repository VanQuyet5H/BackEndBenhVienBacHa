using Camino.Core.Domain;

namespace Camino.Api.Models.Thuoc
{
    public class ADRViewModel : BaseViewModel
    {
        public long ThuocHoacHoatChat1Id { get; set; }
        public string Ma1ThuocHoacHoatChat { get; set; }
        public string Ma2ThuocHoacHoatChat { get; set; }
        public string TenThuocHoacHoatChat1 { get; set; }
        public string TenThuocHoacHoatChat2 { get; set; }
        public long ThuocHoacHoatChat2Id { get; set; }
        public string MaTenHoatChat1 { get; set; }
        public string MaTenHoatChat2 { get; set; }
        //public string Ma2 { get; set; }
        public Enums.MucDoChuYKhiChiDinh MucDoChuYKhiChiDinh { get; set; }
        public string MucDoChuYKhiChiDinhDisplay { get; set; }
        public Enums.MucDoTuongTac MucDoTuongTac { get; set; }
        public string MucDoTuongTacDisplay { get; set; }
        public bool? DuocDongHoc { get; set; }
        public bool? DuocLucHoc { get; set; }
        public bool? ThuocThucAn { get; set; }
        public bool? QuyTac { get; set; }
        public string TuongTacHauQua { get; set; }
        public string CachXuLy { get; set; }
        public string GhiChu { get; set; }
        public string DuocDongHocDisplay { get; set; }
        public string DuocLucHocDisplay { get; set; }
        public string ThuocThucAnDisplay { get; set; }
        public string QuyTacDisplay { get; set; }
        
    }
}
