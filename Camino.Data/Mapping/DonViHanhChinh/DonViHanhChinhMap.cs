using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.DonViHanhChinh
{
    public class DonViHanhChinhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> builder)
        {
            builder.ToTable(MappingDefaults.DonViHanhChinhTable);
            //builder.Property(u => u.Ten).HasMaxLength(20);
            //builder.Property(u => u.TenVietTat).HasMaxLength(200);

            builder.HasOne(rf => rf.TrucThuocDonViHanhChinh)
               .WithMany(r => r.TrucThuocDonViHanhChinhs)
               .HasForeignKey(rf => rf.TrucThuocDonViHanhChinhId);
            base.Configure(builder);
        }
    }
}
