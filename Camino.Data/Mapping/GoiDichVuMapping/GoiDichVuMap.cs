using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GoiDichVuMapping
{
    public class GoiDichVuMap : CaminoEntityTypeConfiguration<GoiDichVu>
    {
        public override void Configure(EntityTypeBuilder<GoiDichVu> builder)
        {
            builder.ToTable(MappingDefaults.GoiDichVuTable);

            //update goi dv 10/21
            //builder.HasOne(rf => rf.NhanVienTaoGoi)
            //          .WithMany(r => r.GoiDichVus)
            //          .HasForeignKey(rf => rf.NhanVienTaoGoiId)
            //          .IsRequired();

            base.Configure(builder);
        }
    }
}
