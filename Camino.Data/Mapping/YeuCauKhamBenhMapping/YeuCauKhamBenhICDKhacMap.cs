using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhICDKhacMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhICDKhac>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhICDKhac> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhICDKhacTable);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauKhamBenhICDKhacs)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId)
               ;

            builder.HasOne(rf => rf.ICD)
               .WithMany(r => r.YeuCauKhamBenhICDKhacs)
               .HasForeignKey(rf => rf.ICDId);

            base.Configure(builder);
        }
    }
}
