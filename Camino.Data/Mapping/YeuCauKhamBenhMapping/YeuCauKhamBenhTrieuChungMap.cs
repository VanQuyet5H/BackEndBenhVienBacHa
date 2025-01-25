using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhTrieuChungMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhTrieuChung>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhTrieuChung> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhTrieuChungTable);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauKhamBenhTrieuChungs)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(m => m.TrieuChung)
                .WithMany(u => u.YeuCauKhamBenhTrieuChungs)
                .HasForeignKey(m => m.TrieuChungId);

            base.Configure(builder);
        }
    }
}
