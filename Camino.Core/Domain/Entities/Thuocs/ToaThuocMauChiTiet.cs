
namespace Camino.Core.Domain.Entities.Thuocs
{
    public class ToaThuocMauChiTiet:BaseEntity
    {
        public long ToaThuocMauId { get; set; }
        public long DuocPhamId { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        /// <summary>
        /// update 07/04/2020
        /// </summary>
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }
        public virtual DuocPham DuocPham { get; set; }
        public virtual ToaThuocMau ToaThuocMau { get; set; }
    }
}
