using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.DichVuKhamBenhBenhViens
{

    public class DichVuKhamBenhBenhVienGiaBenhVienMap : CaminoEntityTypeConfiguration<DichVuKhamBenhBenhVienGiaBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKhamBenhBenhVienGiaBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhBenhVienGiaBenhVienTable);
            //builder.HasMany(rf => rf.Di)
            //   .WithMany(r => r.DichVuGiuongBenhViens)
            //   .HasForeignKey(rf => rf.DichVuGiuongId);

                   builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                  .WithMany(r => r.DichVuKhamBenhBenhVienGiaBenhViens)
                  .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

                   builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                  .WithMany(r => r.DichVuKhamBenhBenhVienGiaBenhViens)
                  .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);


            base.Configure(builder);
        }
    }
}
