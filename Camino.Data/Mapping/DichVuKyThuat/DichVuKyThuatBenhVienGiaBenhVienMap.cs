using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienGiaBenhVienMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienGiaBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienGiaBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienGiaBenhVienTable);

                builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
               .WithMany(r => r.DichVuKyThuatVuBenhVienGiaBenhViens)
               .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);


                builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
               .WithMany(r => r.DichVuKyThuatVuBenhVienGiaBenhViens)
               .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            base.Configure(builder);

        }
    }
}
