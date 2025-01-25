using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.NoiTruDonThuocMapping
{
    public class NoiTruDonThuocMap : CaminoEntityTypeConfiguration<NoiTruDonThuoc>

    {
        public override void Configure(EntityTypeBuilder<NoiTruDonThuoc> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruDonThuocTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.NoiTruDonThuocs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);


            builder.HasOne(rf => rf.ToaThuocMau)
                .WithMany(r => r.NoiTruDonThuocs)
                .HasForeignKey(rf => rf.ToaThuocMauId);

            builder.HasOne(rf => rf.NoiKeDon)
             .WithMany(r => r.NoiTruDonThuocNoiKeDons)
             .HasForeignKey(rf => rf.NoiKeDonId);

            builder.HasOne(rf => rf.BacSiKeDon)
                   .WithMany(r => r.NoiTruDonThuocBacSiKeDons)
                   .HasForeignKey(rf => rf.BacSiKeDonId);

            base.Configure(builder);
        }
    }
}
