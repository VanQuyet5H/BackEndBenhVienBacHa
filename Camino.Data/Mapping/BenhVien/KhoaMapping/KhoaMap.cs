using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhVien.KhoaMapping
{
    public class KhoaMap : CaminoEntityTypeConfiguration<Khoa>
    {
        public override void Configure(EntityTypeBuilder<Khoa> builder)
        {
            builder.ToTable(MappingDefaults.KhoaTable);
            base.Configure(builder);
        }
    }
}
