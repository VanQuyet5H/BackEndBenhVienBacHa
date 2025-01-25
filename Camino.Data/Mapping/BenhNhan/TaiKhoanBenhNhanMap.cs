using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class TaiKhoanBenhNhanMap : CaminoEntityTypeConfiguration<TaiKhoanBenhNhan>
    {
        public override void Configure(EntityTypeBuilder<TaiKhoanBenhNhan> builder)
        {
            builder.ToTable(MappingDefaults.TaiKhoanBenhNhanTable);

            builder.HasOne(rf => rf.BenhNhan)
                .WithOne(r => r.TaiKhoanBenhNhan).
                HasForeignKey<TaiKhoanBenhNhan>(c => c.Id);

            base.Configure(builder);
        }
    }
}
