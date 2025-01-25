using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChucVu
{
    public class ChucVuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.ChucVus.ChucVu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.ChucVus.ChucVu> builder)
        {
            builder.ToTable(MappingDefaults.ChucVuTable);
            //builder.Property(u => u.Ten).HasMaxLength(20);
            //builder.Property(u => u.TenVietTat).HasMaxLength(200);
            base.Configure(builder);
        }
    }
}
