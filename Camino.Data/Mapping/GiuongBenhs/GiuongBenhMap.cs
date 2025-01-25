using Camino.Core.Domain.Entities.GiuongBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GiuongBenhs
{
    public class GiuongBenhMap : CaminoEntityTypeConfiguration<GiuongBenh>
    {
        public override void Configure(EntityTypeBuilder<GiuongBenh> builder)
        {
            builder.ToTable(MappingDefaults.GiuongBenhTable);

            builder.HasOne(m => m.PhongBenhVien)
                .WithMany(u => u.GiuongBenhs)
                .HasForeignKey(m => m.PhongBenhVienId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}