using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuXetNghiem
{
    public class DichVuXetNghiemMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.DichVuXetNghiemTable);

            builder.HasOne(rf => rf.DichVuXetNghiemCha)
                .WithMany(r => r.DichVuXetNghiems)
                .HasForeignKey(rf => rf.DichVuXetNghiemChaId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.DichVuXetNghiems)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.HDPP)
             .WithMany(r => r.DichVuXetNghiems)
             .HasForeignKey(rf => rf.HdppId);

            base.Configure(builder);
        }
    }
}
