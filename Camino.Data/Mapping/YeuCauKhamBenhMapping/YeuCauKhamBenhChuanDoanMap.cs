using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhChuanDoanMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhChuanDoan>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhChuanDoan> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhChuanDoanTable);

            builder.HasOne(m => m.YeuCauKhamBenh)
                .WithMany(u => u.YeuCauKhamBenhChuanDoans)
                .HasForeignKey(m => m.YeuCauKhamBenhId)
                .IsRequired();

            builder.HasOne(m => m.ChuanDoan)
                 .WithMany(u => u.YeuCauKhamBenhChuanDoans)
                 .HasForeignKey(m => m.ChuanDoanId);

            base.Configure(builder);
        }
    }
}