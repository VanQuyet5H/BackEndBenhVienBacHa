using Camino.Core.Domain.Entities.ChuyenKhoaChuyenNganh;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuyenKhoaChuyenNganhs
{
    public class ChuyenKhoaChuyenNganhMap : CaminoEntityTypeConfiguration<ChuyenKhoaChuyenNganh>
    {
        public override void Configure(EntityTypeBuilder<ChuyenKhoaChuyenNganh> builder)
        {
            builder.ToTable(MappingDefaults.ChuyenKhoaChuyenNganhTable);
            base.Configure(builder);
        }
    }
}
