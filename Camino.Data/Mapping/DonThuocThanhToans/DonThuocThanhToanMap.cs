using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DonThuocThanhToans
{
    public class DonThuocThanhToanMap : CaminoEntityTypeConfiguration<DonThuocThanhToan>
    {
        public override void Configure(EntityTypeBuilder<DonThuocThanhToan> builder)
        {
            builder.ToTable(MappingDefaults.DonThuocThanhToanTable);

            builder.HasOne(rf => rf.YeuCauKhamBenhDonThuoc)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.YeuCauKhamBenhDonThuocId);
            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.BenhNhanId);
            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.DonThuocThanhToanBiHuys)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);
            builder.HasOne(rf => rf.NoiCapThuoc)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.NoiCapThuocId);
            builder.HasOne(rf => rf.NhanVienCapThuoc)
                .WithMany(r => r.DonThuocThanhToans)
                .HasForeignKey(rf => rf.NhanVienCapThuocId);
            //update 15/06/2021
            builder.HasOne(rf => rf.NoiTruDonThuoc)
               .WithMany(r => r.DonThuocThanhToans)
               .HasForeignKey(rf => rf.NoiTruDonThuocId);

            base.Configure(builder);
        }
    }
}
