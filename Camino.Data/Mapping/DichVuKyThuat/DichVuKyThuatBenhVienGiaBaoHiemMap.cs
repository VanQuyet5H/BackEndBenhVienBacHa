using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienGiaBaoHiemMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienGiaBaoHiem>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienGiaBaoHiem> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienGiaBaoHiemTable);
            
                    builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                   .WithMany(r => r.DichVuKyThuatBenhVienGiaBaoHiems)
                   .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            base.Configure(builder);

        }
    }
}
