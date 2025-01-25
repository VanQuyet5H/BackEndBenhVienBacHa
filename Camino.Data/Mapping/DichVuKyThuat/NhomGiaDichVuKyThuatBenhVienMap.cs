using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class NhomGiaDichVuKyThuatBenhVienMap : CaminoEntityTypeConfiguration<NhomGiaDichVuKyThuatBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomGiaDichVuKyThuatBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomGiaDichVuKyThuatBenhVienTable);
            base.Configure(builder);

        }
    }
}
