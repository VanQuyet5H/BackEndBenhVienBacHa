using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.LoaiGoiDichVu
{
    public class LoaiGoiDichVuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.LoaiGoiDichVus.LoaiGoiDichVu>

    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.LoaiGoiDichVus.LoaiGoiDichVu> builder)
        {
            builder.ToTable(MappingDefaults.LoaiGoiDichVuTable);

            base.Configure(builder);
        }
    }
}
