using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.VatTuBenhViens;

namespace Camino.Core.Domain.Entities.Vouchers
{
    public class VoucherChiTietMienGiam : BaseEntity
    {
        public long VoucherId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public bool? NhomDichVuKhamBenh { get; set; }
        public Enums.LoaiChietKhau LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public string GhiChu { get; set; }

        public virtual Voucher Voucher { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
    }
}
