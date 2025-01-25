
using Camino.Core.Domain.Entities.BHYT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BHYT
{
    public class YeuCauTiepNhanDuLieuGuiCongBHYTMap : CaminoEntityTypeConfiguration<YeuCauTiepNhanDuLieuGuiCongBHYT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhanDuLieuGuiCongBHYT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanDuLieuGuiCongBHYTTable);

            builder.HasOne(m => m.YeuCauTiepNhan)
                 .WithMany(u => u.YeuCauTiepNhanDuLieuGuiCongBHYTs)
                 .HasForeignKey(m => m.YeuCauTiepNhanId);

            builder.HasOne(m => m.DuLieuGuiCongBHYT)
                .WithMany(u => u.YeuCauTiepNhanDuLieuGuiCongBHYTs)
                .HasForeignKey(m => m.DuLieuGuiCongBHYTId);

            base.Configure(builder);
        }
    }
}