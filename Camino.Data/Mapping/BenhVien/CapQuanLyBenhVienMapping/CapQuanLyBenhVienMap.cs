using Camino.Core.Domain.Entities.BenhVien.CapQuanLyBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhVien.CapQuanLyBenhVienMapping
{
    public class CapQuanLyBenhVienMap : CaminoEntityTypeConfiguration<CapQuanLyBenhVien>
    {
        public override void Configure(EntityTypeBuilder<CapQuanLyBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.CapQuanLyBenhVienTable);
            base.Configure(builder);
        }
    }
}
