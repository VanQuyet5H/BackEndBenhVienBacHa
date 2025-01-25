using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class DichVuCanGhiNhanVTTHThuocVo
    {
        public string KeyId
        {
            get
            {
                string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
                return "{" + string.Format(templateKeyId, Id, NhomDichVu) + "}";
            }
        }

        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long Id { get; set; }
        public int NhomDichVu { get; set; }
    }

    public class DichVuGhiNhanVo{
        public long Id { get; set; }
        public int NhomId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
		public long? KhoId { get; set; }
    }

    public class VatTuThuocTieuHaoVo
    {
        public string KeyId
        {
            get
            {
                string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
                return "{" + string.Format(templateKeyId, Id, NhomDichVu) + "}";
            }
        }

        public string DisplayName { get; set; }
        public long Id { get; set; }
        public int NhomDichVu { get; set; }
        public string DonViTinh { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.MathRoundNumber(2).ToString();
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string NhaSanXuat { get; set; }
        public string DuongDung { get; set; }
    }

    public class ChiDinhGhiNhanVatTuThuocTieuHaoVo{
        public ChiDinhGhiNhanVatTuThuocTieuHaoVo()
        {
            NhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>();
            NhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string DichVuChiDinhId { get; set; }
        public long? KhoId { get; set; }
        public string DichVuGhiNhanId { get; set; }
        public bool IsCapNhatSoLuong { get; set; }
        public double? SoLuong { get; set; }
        public bool IsCapNhatTinhPhi { get; set; }
        public bool? TinhPhi { get; set; }
		public bool LaDuocPhamBHYT { get; set; }
        public List<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets { get; set; }
        public List<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets { get; set; }


        // dùng cho cập nhật
        public string YeuCauGhiNhanVTTHThuocId { get; set; }
        public double? SoLuongCapNhat { get; set; }
        public LoaiNoiChiDinh LoaiNoiChiDinh { get; set; }

        //Cập nhật 04/07/2022: bật cờ trường hợp thêm loại lĩnh bù -> xử lý xuất luôn
        public bool? LaLinhBu { get; set; }
    }

	public class GhiNhanVatTuTieuHaoThuocGridVo
	{
		public string Id
		{
			get
			{
				string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
				return "{" + string.Format(templateKeyId, YeuCauId, NhomYeuCauId) + "}";
			}
		}
        public string DichVuChiDinhId
        {
            get
            {
                string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
                return "{" + string.Format(templateKeyId, YeuCauDichVuChiDinhId, NhomChiDinhId) + "}";
            }
        }
        public string DichVuGhiNhanId
        {
            get
            {
                string templateKeyId = "\"Id\": {0}, \"NhomId\": {1}";
                return "{" + string.Format(templateKeyId, VatTuThuocBenhVienId, NhomYeuCauId) + "}";
            }
        }
        public long YeuCauId { get; set; }
		public string TenNhomYeuCau { get; set; }
		public int NhomYeuCauId { get; set; }
        public long? YeuCauDichVuChiDinhId { get; set; }
        public int NhomChiDinhId { get; set; }
        public bool LaDuocPham => (EnumNhomGoiDichVu) NhomYeuCauId == EnumNhomGoiDichVu.DuocPham;

		public string TenDichVu { get; set; }
		public long DichVuId { get; set; }

		public string MaDichVuYeuCau { get; set; }
		public string TenDichVuYeuCau { get; set; }

		public long KhoId { get; set; }
		public string TenKho { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public string DonViTinh { get; set; }
		public string TenDuongDung { get; set; }
		public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }

        public decimal? DonGia { get; set; }
		public decimal? ThanhTien { get; set; }


		public decimal? DonGiaBaoHiem { get; set; }
		public int? TiLeBaoHiemThanhToan { get; set; }
		public bool DuocHuongBaoHiem { get; set; }
		public bool? KhongTinhPhi { get; set; }
        public bool LaBHYT { get; set; }
		
		public string PhieuLinh { get; set; }
		public string PhieuXuat { get; set; }
		public int STT { get; set; }
        public EnumGiaiDoanPhauThuat? GiaiDoanPhauThuat { get; set; }
        public string GiaiDoanPhauThuatDisplay { get; set; }

        public bool TinhPhi { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ThoiGianChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh?.ApplyFormatDateTime();

        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }

        public long VatTuThuocBenhVienId { get; set; }
        public bool TinhTrang { get; set; }
        public bool IsKhoLe { get; set; }
        public bool IsKhoTong { get; set; }
        public bool IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }

    public class GhiNhanVatTuTieuHaoThuocGroupParentGridVo
    {
        public GhiNhanVatTuTieuHaoThuocGroupParentGridVo()
        {
            YeuCauGhiNhanVTTHThuocs = new List<GhiNhanVatTuTieuHaoThuocGridVo>();
            ThongTinGias = new List<GhiNhanVatTuTieuHaoThuocGroupGiaGridVo>();
        }

        public string Id => YeuCauGhiNhanVTTHThuocs.Select(x => x.Id).Join(";");
        public string DichVuChiDinhId => YeuCauGhiNhanVTTHThuocs.Select(x => x.DichVuChiDinhId).FirstOrDefault();
        public string DichVuGhiNhanId => YeuCauGhiNhanVTTHThuocs.Select(x => x.DichVuGhiNhanId).FirstOrDefault();

        public long? YeuCauDichVuChiDinhId { get; set; }
        public int NhomChiDinhId { get; set; }
        public int NhomYeuCauId { get; set; }
        public bool LaDuocPham => (EnumNhomGoiDichVu)NhomYeuCauId == EnumNhomGoiDichVu.DuocPham;

        public string TenDichVu { get; set; }
        public long DichVuId { get; set; }

        public string MaDichVuYeuCau { get; set; }
        public string TenDichVuYeuCau { get; set; }

        public long KhoId { get; set; }
        public string TenKho { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public string DonViTinh { get; set; }
        public string TenDuongDung { get; set; }
        public double? SoLuong { get; set; }
        //public string SoLuongDisplay { get; set; }

        //public decimal? DonGia { get; set; }
        public decimal ThanhTien { get; set; }


        public decimal? DonGiaBaoHiem => ThongTinGias.Select(a => a.DonGiaBaoHiem).Distinct().Count() > 1 ? null : ThongTinGias.Select(a => a.DonGiaBaoHiem).First();
        //public int? TiLeBaoHiemThanhToan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool LaBHYT { get; set; }

        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public EnumGiaiDoanPhauThuat? GiaiDoanPhauThuat { get; set; }
        public string GiaiDoanPhauThuatDisplay => GiaiDoanPhauThuat?.GetDescription();

        public bool TinhPhi { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? ThoiGianChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh?.ApplyFormatDateTime();

        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }

        public long VatTuThuocBenhVienId { get; set; }
        public bool TinhTrang { get; set; }
        public bool IsKhoLe { get; set; }
        public bool IsKhoTong { get; set; }
        public bool IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet { get; set; }

        public List<GhiNhanVatTuTieuHaoThuocGridVo> YeuCauGhiNhanVTTHThuocs { get; set; }
        public List<GhiNhanVatTuTieuHaoThuocGroupGiaGridVo> ThongTinGias { get; set; }

        //BVHD-3905
        public string TooltipTiLeBHYT { get; set; }
    }

    public class GhiNhanVatTuTieuHaoThuocGroupGiaGridVo
    {
        public bool IsTinhPhi { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public decimal? ThanhTien => IsTinhPhi == true ? DonGia * Convert.ToDecimal(SoLuong) : 0;
    }

    public class KhoSapXepUuTienLookupItemVo: LookupItemVo
    {
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
    }

    public class UpdateSoLuongItemGhiNhanVTTHThuocVo
    {
        public double SoLuong { get; set; }
        public double SoLuongBanDau { get; set; }
        public bool LaDuocPham { get; set; }
        public long VatTuThuocBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public long KhoId { get; set; }
    }

    public class VTTHThuocCanKiemTraTrungKhiThemVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public string DichVuChiDinhId { get; set; }
        public string DichVuGhiNhanId { get; set; }
    }
}
