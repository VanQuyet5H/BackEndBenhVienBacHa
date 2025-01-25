using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Camino.Core.Domain.ValueObject
{
    public class NoiDungKetQuaVo
    {
        public long Id { get; set; }
        public string ThongTinHanhChinhMaTiepNhan { get; set; }
        public string ThongTinHanhChinhMaBenhNhan { get; set; }
        public string ThongTinHanhChinhHoTen { get; set; }
        public int? ThongTinHanhChinhNgaySinh { get; set; }
        public int? ThongTinHanhChinhThangSinh { get; set; }
        public int? ThongTinHanhChinhNamSinh { get; set; }
        public int ThongTinHanhChinhTuoi => ThongTinHanhChinhNamSinh == null ? 0 : DateTime.Now.Year - ThongTinHanhChinhNamSinh.Value;
        public string ThongTinHanhChinhTenGioiTinh { get; set; }
        public string ThongTinHanhChinhDiaChi { get; set; }
        public string ThongTinHanhChinhDoiTuong { get; set; }
        public string ThongTinHanhChinhBacSiChiDinh { get; set; }
        public string ThongTinHanhChinhNgayChiDinh { get; set; }
        public string ThongTinHanhChinhNoiChiDinh { get; set; }
        public string ThongTinHanhChinhSoBenhAn { get; set; }
        public string ThongTinHanhChinhChanDoan { get; set; }
        public string ThongTinHanhChinhChiDinh { get; set; }

        public string DataKetQuaCanLamSang { get; set; }
        public ChiTietKetQuaCDHATDCNVo ChiTietKetQuaObj { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public long? MayTraKetQuaId { get; set; }
        public string FileChuKy { get; set; }
    }

    public class ChiTietKetQuaCDHATDCNVo
    {
        public ChiTietKetQuaCDHATDCNVo()
        {
            HinhAnhDinhKems = new List<HinhAnhDinhKemKetQuaVo>();
        }
        public long? NguoiLuuId { get; set; }
        public string NguoiLuuTen { get; set; }
        public DateTime? ThoiDiemLuu { get; set; }

        public string TenKetQuaLabel => "KẾT QUẢ";
        public string TenKetQua { get; set; }
        public string KyThuat { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }
        public bool InKemAnh { get; set; }
        public List<HinhAnhDinhKemKetQuaVo> HinhAnhDinhKems { get; set; }
    }

    public class HinhAnhDinhKemKetQuaVo
    {
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        public bool InKemKetQua { get; set; }
    }
}
