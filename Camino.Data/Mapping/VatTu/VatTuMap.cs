using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.VatTu
{
    public class VatTuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.VatTus.VatTu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.VatTus.VatTu> builder)
        {
            builder.ToTable(MappingDefaults.VatTuTable);

            builder.HasOne(rf => rf.NhomVatTu)
               .WithMany(r => r.VatTus)
               .HasForeignKey(rf => rf.NhomVatTuId);
            base.Configure(builder);
        }
    }
}
