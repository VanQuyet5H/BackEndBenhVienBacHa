using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class BenhNhanTienSuBenhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.BenhNhans.BenhNhanTienSuBenh> builder)
        {
            builder.ToTable(MappingDefaults.BenhNhanTienSuBenhTable);
                builder.HasOne(m => m.BenhNhan)
                    .WithMany(u => u.BenhNhanTienSuBenhs)
                    .HasForeignKey(m => m.BenhNhanId);

            base.Configure(builder);
        }

    }
}
