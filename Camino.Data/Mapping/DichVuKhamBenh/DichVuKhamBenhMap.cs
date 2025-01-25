using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.DichVuKhamBenh
{
    public class DichVuKhamBenhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DichVuKhamBenhs.DichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhTable);

            builder.HasOne(rf => rf.Khoa)
                .WithMany(r => r.DichVuKhamBenhs)
                .HasForeignKey(rf => rf.KhoaId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
