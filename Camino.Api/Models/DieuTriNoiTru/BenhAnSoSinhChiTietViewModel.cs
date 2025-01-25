using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class BenhAnSoSinhChiTietViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public string MaBenhAnMe { get; set; }
        public string TenBanDau { get; set; }
        public string TenKhaiSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public int? GioSinh { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }

        public string SoDienThoai { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public long? NgheNghiepId { get; set; }

        public long? QuocTichId { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string DiaChi { get; set; }
        public long? DanTocId { get; set; }
        public string Email { get; set; }
        public string NoiLamViec { get; set; }

        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }

        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }
        public string NguoiLienHeDiaChi { get; set; }

        public long? ResultKhoaNhapVienId { get; set; }
        public string ResultTenKhoaNhapVien { get; set; }
        public long? ResultYeuCauTiepNhanId { get; set; }
        public long? ResultBenhNhanId { get; set; }
        public bool? ResultKhongCanChiDinhGiuong { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
        public long? KhoaChuyenBenhAnSoSinhVeId { get; set; }

        public DateTime? LucDeSoSinh { get; set; }
    }

    public class TaoBenhAnSoSinhKhacKhoaViewModel
    {
        public BenhAnSoSinhChiTietViewModel BenhAnSoSinhChiTietViewModel { get; set; }
        public DacDiemTreSoSinh DacDiemTreSoSinh { get; set; }
    }
}
