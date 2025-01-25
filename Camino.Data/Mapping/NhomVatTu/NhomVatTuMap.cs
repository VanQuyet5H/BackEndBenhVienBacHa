using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhomVatTu
{
    public class NhomVatTuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NhomVatTus.NhomVatTu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NhomVatTus.NhomVatTu> builder)
        {
            builder.ToTable(MappingDefaults.NhomVatTuTable);

            builder.HasOne(rf => rf.nhomVatTu)
               .WithMany(r => r.NhomVatTus)
               .HasForeignKey(rf => rf.NhomVatTuChaId);

            base.Configure(builder);
        }
    }
}
