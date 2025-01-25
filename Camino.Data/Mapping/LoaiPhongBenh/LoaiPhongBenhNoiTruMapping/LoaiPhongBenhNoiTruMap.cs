using Camino.Core.Domain.Entities.LoaiPhongBenh.LoaiPhongBenhNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.LoaiPhongBenh.LoaiPhongBenhNoiTruMapping
{
    public class LoaiPhongBenhNoiTruMap : CaminoEntityTypeConfiguration<LoaiPhongBenhNoiTru>
    {
        public override void Configure(EntityTypeBuilder<LoaiPhongBenhNoiTru> builder)
        {
            builder.ToTable(MappingDefaults.LoaiPhongBenhNoiTruTable);
            base.Configure(builder);
        }
    }
}
