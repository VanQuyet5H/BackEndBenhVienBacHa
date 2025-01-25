using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoThuVienPhiChuaHoanTimKiemVo
    {
        public string NhomThuVienPhi { get; set; }
        public long? KhoaPhongId { get; set; }
        public Enums.NhomThuVienPhiChuaHoan? NhomThuVienPhiEnum { get; set; }

        public string SearchString { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
    }

    public class LookupItemNhomThuVienPhiVo
    {
        public string KeyId => JsonConvert.SerializeObject(new KeyIdObjectNhomThuVienPhiVo()
        {
            Value = Value,
            LaKhoaNhapVien = LaKhoaNhapVien
        });
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }

        public long Value { get; set; }
        public bool LaKhoaNhapVien { get; set; }
    }

    public class KeyIdObjectNhomThuVienPhiVo
    {
        public long Value { get; set; }
        public bool LaKhoaNhapVien { get; set; }
    }

    public class BaoCaoThuVienPhiChuaHoanGridVo : GridItem
    {
        public BaoCaoThuVienPhiChuaHoanGridVo()
        {
            YeuCauGoiDichVuIds = new List<long>();
        }

        public int? STT { get; set; }

        public string TenNhomThuVienPhiChuaHoan
        {
            get
            {
                if (LaGoi)
                {
                    return Enums.NhomThuVienPhiChuaHoan.GoiDichVu.GetDescription();
                }
                else if (LaNgoaiTru)
                {
                    return Enums.NhomThuVienPhiChuaHoan.NgoaiTru.GetDescription();
                }
                else
                {
                    return TenKhoaNhapVien;
                }
            }
        }
        public long YeucauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string BienLai { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuDisplay => NgayThu.ApplyFormatDate();

        public bool LaGoi { get; set; }
        public bool LaNoiTru { get; set; }
        public string TenKhoaNhapVien { get; set; }
        public bool LaNgoaiTru => !LaGoi && !LaNoiTru;
        public long? PhieuHoanUngId { get; set; }

        public decimal? SoTienTamUngTamTinh { get; set; }
        public decimal? SoTienDaHoanTamTinh { get; set; }
        public decimal? SoTienTamUng => SoTienTamUngTamTinh.GetValueOrDefault() > 0 ? SoTienTamUngTamTinh : (decimal?)null;
        public decimal? SoTienDaHoan => (PhieuHoanUngId != null && !LaGoi) 
            // trường hợp có phiếu hoàn hoàn ứng
            ? (SoTienTamUngTamTinh.GetValueOrDefault() > 0 ? SoTienTamUngTamTinh : (decimal?)null) 

            // trường hợp trong gói
            : (SoTienDaHoanTamTinh.GetValueOrDefault() > 0 ? SoTienDaHoanTamTinh : (decimal?)null);

        public List<long> YeuCauGoiDichVuIds { get; set; }
    }

    public class BaoCaoThuVienPhiChuaHoanTongCongVo
    {
        public decimal TongTienTamUng { get; set; }
        public decimal TongTienDaHoan { get; set; }
    }

    public class ChiTietHoanPhiTheoGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public decimal SoTienHoanPhi { get; set; }
        public long TaiKhoanBenhNhanThuId { get; set; }
        public decimal TongTienPhieuThu { get; set; }
    }

    public class ChiTietTongTienHoanPhiTheoGoiVo
    {
        public long YeuCauGoiDichVuId { get; set; }
        public decimal TongTienHoanPhi { get; set; }
    }

    public class ChiPhiHoanPhiTheoPhieuThuVo
    {
        public ChiPhiHoanPhiTheoPhieuThuVo()
        {
            YeuCauGoiDichVuIds = new List<long>();
        }
        public long TaiKhoanBenhNhanThuId { get; set; }
        public decimal TongTienPhieuThu { get; set; }
        public decimal TongTienDaHoan { get; set; }

        public List<long> YeuCauGoiDichVuIds { get; set; }
    }
}
