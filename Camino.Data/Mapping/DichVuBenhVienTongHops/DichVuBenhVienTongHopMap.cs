using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuBenhVienTongHops
{
    public class DichVuBenhVienTongHopMap : CaminoEntityTypeConfiguration<DichVuBenhVienTongHop>
    {
        public override void Configure(EntityTypeBuilder<DichVuBenhVienTongHop> builder)
        {
            builder.ToTable(MappingDefaults.DichVuBenhVienTongHopTable);

            base.Configure(builder);
        }
    }
}