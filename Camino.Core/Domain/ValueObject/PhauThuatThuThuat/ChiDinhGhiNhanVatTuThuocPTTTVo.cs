using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class ChiDinhGhiNhanVatTuThuocPTTTVo
    {
        public ChiDinhGhiNhanVatTuThuocPTTTVo()
        {
            NhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
            NhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long DichVuChiDinhId { get; set; }
        public long? KhoId { get; set; }
        public string DichVuGhiNhanId { get; set; }
        public double? SoLuong { get; set; }
        public bool? TinhPhi { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public List<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets { get; set; }
        public List<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets { get; set; }
        public EnumGiaiDoanPhauThuat GiaiDoanPhauThuat { get; set; }
        public LoaiNoiChiDinh LoaiNoiChiDinh { get; set; }
        // dùng cho cập nhật
        public string YeuCauGhiNhanVTTHThuocId { get; set; }
        public int? SoLuongCapNhat { get; set; }
        public bool IsCapNhatTinhPhi { get; set; }
        public bool IsCapNhatSoLuong { get; set; }
    }

    public class ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTVo
    {
        public ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTVo()
        {
            ChiDinhGhiNhanVatTuThuocPTTTChiTiets = new List<ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTChiTietVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTChiTietVo> ChiDinhGhiNhanVatTuThuocPTTTChiTiets { get; set; }
    }

    public class ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTChiTietVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long KhoId { get; set; }
        public string DichVuGhiNhanId { get; set; }
        public double SoLuong { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public EnumGiaiDoanPhauThuat GiaiDoanPhauThuat { get; set; }
    }
}
