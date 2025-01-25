using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKhamBenhBenhViens
{
    
    public class DichVuKhamBenhBenhVienMap : CaminoEntityTypeConfiguration<DichVuKhamBenhBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKhamBenhBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhBenhVienTable);
            builder.HasMany(rf => rf.DichVuKhamBenhBenhVienGiaBaoHiems)
               .WithOne(r => r.DichVuKhamBenhBenhVien)
               .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            builder.HasMany(rf => rf.DichVuKhamBenhBenhVienGiaBenhViens)
              .WithOne(r => r.DichVuKhamBenhBenhVien)
              .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.DichVuKhamBenh)
                .WithMany(r => r.DichVuKhamBenhBenhViens)
                .HasForeignKey(rf => rf.DichVuKhamBenhId);

            base.Configure(builder);
        }
    }
}
