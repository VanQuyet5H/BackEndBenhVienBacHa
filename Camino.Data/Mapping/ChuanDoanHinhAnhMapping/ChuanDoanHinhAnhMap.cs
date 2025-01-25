using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuanDoanHinhAnhMapping
{
    public class ChuanDoanHinhAnhMap: CaminoEntityTypeConfiguration<ChuanDoanHinhAnh>
    {
        public override void Configure(EntityTypeBuilder<ChuanDoanHinhAnh> builder)
        {
            builder.ToTable(MappingDefaults.ChuanDoanHinhAnhTable);
            base.Configure(builder);
        }
    }
}
