using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh.ViewModelCheckValidators
{
    public class ChiDinhDichVuKhamBenhViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhTruocId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string MaDichVuTT37 { get; set; }
        public string TenDichVu { get; set; }
        public decimal? Gia { get; set; }
        public int? SoLuong { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThai { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }

        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public long? NoiDangKyId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? BacSiThucHienId { get; set; }
        public string BacSiNoiDangKyId { get; set; }
        public long? NoiKetLuanId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public bool? TinhPhi { get; set; }
        public bool? LaThamVan { get; set; }
        public byte[] LastModified { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public ChiDinhDichVuTrungTuGoiViewModel DichVuKhamBenhTuGoi { get; set; }

        //BVHD-3575
        public long? PhieuDieuTriId { get; set; }
    }

    public class ChiDinhDichVuTrungTuGoiViewModel
    {
        public string Id { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public long? ChuongTrinhGoiDichVuId { get; set; }
        public long? ChuongTrinhGoiDichVuChiTietId { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
    }
}
