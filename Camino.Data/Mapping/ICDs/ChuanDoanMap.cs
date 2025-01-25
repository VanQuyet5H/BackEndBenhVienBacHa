using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class ChuanDoanMap : CaminoEntityTypeConfiguration<ChuanDoan>
    {
        public override void Configure(EntityTypeBuilder<ChuanDoan> builder)
        {
            builder.ToTable(MappingDefaults.ChuanDoanTable);

            builder.Property(x => x.Ma).HasMaxLength(20);
            builder.Property(x => x.TenTiengAnh).HasMaxLength(250);
            builder.Property(x => x.TenTiengViet).HasMaxLength(250);

            builder.HasOne(rf => rf.DanhMucChuanDoan)
                .WithMany(r => r.ChuanDoans)
                .HasForeignKey(rf => rf.DanhMucChuanDoanId);

            base.Configure(builder);
        }
    }
}
