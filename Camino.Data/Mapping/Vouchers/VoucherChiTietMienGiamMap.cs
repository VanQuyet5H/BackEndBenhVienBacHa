using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Vouchers
{
    public class VoucherChiTietMienGiamMap : CaminoEntityTypeConfiguration<VoucherChiTietMienGiam>
    {
        public override void Configure(EntityTypeBuilder<VoucherChiTietMienGiam> builder)
        {
            builder.ToTable(MappingDefaults.VoucherChiTietMienGiamTable);
            builder.HasOne(rf => rf.Voucher)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.VoucherId);
            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);
            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.VoucherChiTietMienGiams)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);


            //builder.HasOne(rf => rf.DuocPhamBenhVien)
            //    .WithMany(r => r.VoucherChiTietMienGiams)
            //    .HasForeignKey(rf => rf.DuocPhamBenhVienId);
            //builder.HasOne(rf => rf.VatTuBenhVien)
            //    .WithMany(r => r.VoucherChiTietMienGiams)
            //    .HasForeignKey(rf => rf.VatTuBenhVienId);
            //builder.HasOne(rf => rf.DichVuGiuongBenhVien)
            //    .WithMany(r => r.VoucherChiTietMienGiams)
            //    .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);
            //builder.HasOne(rf => rf.GoiDichVu)
            //    .WithMany(r => r.VoucherChiTietMienGiams)
            //    .HasForeignKey(rf => rf.GoiDichVuId);

            base.Configure(builder);
        }
    }
}
