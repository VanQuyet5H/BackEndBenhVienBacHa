using Camino.Core.Domain.Entities.NhomDichVuBenhVien;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhomDichVuBenhVienMapping
{
    public class NhomDichVuBenhVienMap : CaminoEntityTypeConfiguration<NhomDichVuBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomDichVuBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomDichVuBenhVienTable);
            base.Configure(builder);
        }
    }
}
