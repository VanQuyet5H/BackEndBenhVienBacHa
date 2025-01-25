using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoBangKeChiTietTheoNguoiBenhTimKiemVo : QueryInfo
    {
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long? NguoiBenhId { get; set; }
        public string SearchString { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime? TuNgayFormat { get; set; }
        public DateTime? DenNgayFormat { get; set; }
    }

    public class BaoCaoBangKeChiTietTheoNguoiBenhGridVo : GridItem
    {
        public BaoCaoBangKeChiTietTheoNguoiBenhGridVo()
        {
            ChiTietGiaTonKhos = new List<ChiTietGiaTonKhoThuocVatTuVo>();
        }

        public string TenDichVuCovid { get; set; }
        public string STT { get; set; }
        public long YeucauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan?.ApplyFormatDate();
        public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        public string NoiGioiThieuDisplay { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string DiaChi { get; set; }

        public Enums.BangKeChiPhiTheoNhomDichVu Nhom
        {
            get
            {
                if (NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh 
                    || (NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat 
                        && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat 
                        && LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SuatAn
                        && !NoiDung.Contains(TenDichVuCovid)
                        && DaThucHien))
                {
                    return LaNoiTru == true ? Enums.BangKeChiPhiTheoNhomDichVu.CanLamSangNoiTru : Enums.BangKeChiPhiTheoNhomDichVu.CanLamSangNgoaiTru;
                }
                else if (NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                {
                    return Enums.BangKeChiPhiTheoNhomDichVu.Giuong;
                }
                else if (NhomDichVu == Enums.EnumNhomGoiDichVu.DuocPham)
                {
                    return Enums.BangKeChiPhiTheoNhomDichVu.Thuoc;
                }
                else if (NhomDichVu == Enums.EnumNhomGoiDichVu.VatTuTieuHao)
                {
                    return Enums.BangKeChiPhiTheoNhomDichVu.VatTu;
                }
                else if(NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                {
                    if (LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SuatAn)
                    {
                        return Enums.BangKeChiPhiTheoNhomDichVu.SuatAn;
                    }
                    else if (NoiDung.Contains(TenDichVuCovid))
                    {
                        return Enums.BangKeChiPhiTheoNhomDichVu.TestCovid;
                    }
                }
                return Enums.BangKeChiPhiTheoNhomDichVu.CanLamSangNgoaiTru;
            }
        }

        public string TenNhom => Nhom.GetDescription();
        public string NoiDung { get; set; }
        public string DonViTinh { get; set; }
        public double SoLuong { get; set; }
        public decimal? GiaNiemYet { get; set; }
        public decimal? GiaUuDai { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal ThanhTien => KhongTinhPhi ? 0 : ((ThanhTienThuocVatTu ?? Convert.ToDecimal(SoLuong * (double)(GiaNiemYet ?? 0))) - SoTienMienGiam.GetValueOrDefault());
        public decimal? NguoiBenhDaThanhToan { get; set; }
        public decimal? CongNoConPhaiThanhToan { get; set; }

        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public bool DaThucHien { get; set; }

        // thành tiền dùng rieng cho cho thuốc và vật tư
        public decimal? ThanhTienThuocVatTu { get; set; }
        public decimal TongCongChiPhi { get; set; }

        public string TenHinhThucDen { get; set; }
        public bool? LaGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaGioiThieu == true
            ? (TenHinhThucDen + ((!string.IsNullOrEmpty(TenHinhThucDen) && !string.IsNullOrEmpty(NoiGioiThieuDisplay)) ? "/ " : "") + NoiGioiThieuDisplay)
            : TenHinhThucDen;

        public bool? LaDataTheoDieuKienTimKiem { get; set; }
        public bool? LaNoiTru { get; set; }

        public bool KhongTinhPhi { get; set; }

        //BVHD-3767
        public long? XuatKhoChiTietId { get; set; }
        public List<ChiTietGiaTonKhoThuocVatTuVo> ChiTietGiaTonKhos { get; set; }

        //cập nhật ngày: 10/02/2022: đơn giá tồn kho ko nhân số lượng
        //public decimal? TongCongGiaTonKho => ChiTietGiaTonKhos.Any() ? ChiTietGiaTonKhos.Sum(x => x.GiaTonKho ?? 0) : (decimal?)null;
        public decimal? TongCongGiaTonKho => ChiTietGiaTonKhos.Any() ? ChiTietGiaTonKhos.Select(x => x.GiaTonKho ?? 0).Distinct().Sum() : (decimal?)null;

        public decimal? TongChiPhiGiaTonKho { get; set; }
    }

    public class ChiTietGiaTonKhoThuocVatTuVo
    {
        public double SoLuong { get; set; }
        public long? XuatKhoChiTietId { get; set; }
        public decimal? DonGiaTonKho { get; set; }

        //cập nhật ngày: 10/02/2022: đơn giá tồn kho ko nhân số lượng
        public decimal? GiaTonKho => DonGiaTonKho; // Convert.ToDecimal(SoLuong * (double)(DonGiaTonKho ?? 0));
        public bool LaDuocPham { get; set; }

        public long? NhapKhoChiTietId { get; set; }
    }
}
