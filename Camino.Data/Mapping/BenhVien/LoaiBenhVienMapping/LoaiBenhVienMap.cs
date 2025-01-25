using Camino.Core.Domain.Entities.BenhVien.LoaiBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhVien.LoaiBenhVienMapping
{
    public class LoaiBenhVienMap : CaminoEntityTypeConfiguration<LoaiBenhVien>
    {
        public override void Configure(EntityTypeBuilder<LoaiBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.LoaiBenhVienTable);
            base.Configure(builder);
        }
    }
}
