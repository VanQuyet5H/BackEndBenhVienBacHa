using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo : GridItem
    {
        public BaoCaoDoanhThuKhamDoanTheoKhoaPhongGridVo()
        {
            DoanhThuTheoDichVus = new List<DoanhThuTheoDichVuVo>();
        }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string GioiTinh { get; set; }
        public long CongTyId { get; set; }
        public string TenCongTy { get; set; }

        #region Thông tin doanh thu theo nhóm dịch vụ
        public CauHinhBaoCao CauHinhBaoCao { get; set; }
        public decimal? KhamBenh => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh) 
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKhamBenh).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? XetNghiem => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.LaDichVuXetNghiem)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.LaDichVuXetNghiem).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? NoiSoi => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomNoiSoi)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomNoiSoi).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? NoiSoiTMH => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomNoiSoiTMH)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomNoiSoiTMH).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? SieuAm => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomSieuAm)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomSieuAm).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? XQuang => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat 
                                                               && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomXQuangThuong || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomXQuangSoHoa))
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem 
                                             && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomXQuangThuong || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomXQuangSoHoa)).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? CTScan => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomCTScanner)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomCTScanner).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? MRI => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomMRI)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem && a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomMRI).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? DienTimDienNao => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat 
                                                                       && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDienTim || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDienNao))
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem
                                             && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDienTim || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDienNao)).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? TDCNDoLoangXuong => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat 
                                                                         && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDoLoangXuong || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDoHoHap))
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat && !a.LaDichVuXetNghiem
                                             && (a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDoLoangXuong || a.NhomDichVuBenhVienId == CauHinhBaoCao.NhomDoHoHap)).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal? DVKhac => DoanhThuTheoDichVus.Any(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat 
                                                               && !a.LaDichVuXetNghiem
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomNoiSoi
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomNoiSoiTMH
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomSieuAm
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomXQuangThuong
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomXQuangSoHoa
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomCTScanner
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomMRI
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDienTim
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDienNao
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDoLoangXuong
                                                               && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDoHoHap)
            ? DoanhThuTheoDichVus.Where(a => a.DoanhThu != null && a.DoanhThu > 0 && a.NhomDichVu == Enums.EnumNhomGoiDichVu.DichVuKyThuat
                                             && !a.LaDichVuXetNghiem
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomNoiSoi
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomNoiSoiTMH
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomSieuAm
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomXQuangThuong
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomXQuangSoHoa
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomCTScanner
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomMRI
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDienTim
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDienNao
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDoLoangXuong
                                             && a.NhomDichVuBenhVienId != CauHinhBaoCao.NhomDoHoHap).Sum(a => a.DoanhThu ?? 0) : (decimal?)null;
        public decimal TongDoanhThuTheoNhomDichVu => DoanhThuTheoDichVus.Sum(a => a.DoanhThu ?? 0);
        #endregion

        public List<DoanhThuTheoDichVuVo> DoanhThuTheoDichVus { get; set; }

        public List<DoanhThuTheoKhoaVo> DoanhThuTheoKhoas
            => DoanhThuTheoDichVus.GroupBy(x => x.KhoaId).Select(item => new DoanhThuTheoKhoaVo()
            {
                KhoaId = item.Key,
                TenKhoa = item.First().TenKhoa,
                DoanhThu = item.Any(a => a.DoanhThu != null && a.DoanhThu != 0) ? item.Where(a => a.DoanhThu != null && a.DoanhThu != 0).Sum(a => a.DoanhThu) : (decimal?)null
            }).ToList();
        public decimal TongDoanhThuTheoKhoa => DoanhThuTheoKhoas.Sum(x => x.DoanhThu ?? 0);
    }

    public class DoanhThuTheoKhoaVo
    {
        public long KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public decimal? DoanhThu { get; set; }
    }

    public class DoanhThuTheoDichVuVo
    {
        public long YeucauTiepNhanId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal? DoanhThu { get; set; }
        public long KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public bool LaDichVuXetNghiem { get; set; }
    }
}
