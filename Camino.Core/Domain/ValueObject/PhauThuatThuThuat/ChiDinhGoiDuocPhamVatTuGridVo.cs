using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ChiDinhGoiDuocPhamVatTuGridVo : GridItem
    {
        public long KhoId { get; set; }
        public string KhoDisplay { get; set; }
        public long DuocPhamVatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public bool LaDuocPhamVatTuBHYT { get; set; }
        public bool LaDuocPham { get; set; }
        public string DonViTinh { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongKe { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool? CheckBox { get; set; }
        public EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public EnumGiaiDoanPhauThuat GiaiDoanPhauThuat { get; set; }

        public string DichVuGhiNhanId
        {
            get
            {
                var result = string.Empty;
                var dichVuGhiNhanVo = new DichVuGhiNhanVo()
                {
                    Id = Id,
                    KhoId = KhoId,
                    LaDuocPhamBHYT = LaDuocPhamVatTuBHYT,
                    NhomId = LaDuocPham ? 4 : 3
                };
                result = JsonConvert.SerializeObject(dichVuGhiNhanVo);
                return result;
            }
        }

        public int NhomId => SoLuongKe <= SoLuongTon ? 1 : 0;
        public string Nhom => SoLuongKe <= SoLuongTon ? "Đủ tồn trong tủ trực" : "Không đủ tồn trong tủ trực";
    }
}