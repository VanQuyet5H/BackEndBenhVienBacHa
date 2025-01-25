using Camino.Core.Domain.Entities.PhongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.PhongBenhVienMapping
{
    public class PhongBenhVienNhomGiaDichVuKhamBenhMap : CaminoEntityTypeConfiguration<PhongBenhVienNhomGiaDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<PhongBenhVienNhomGiaDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.PhongBenhVienNhomGiaDichVuKhamBenhTable);

                builder.HasOne(rf => rf.PhongBenhVien)
                      .WithMany(r => r.PhongBenhVienNhomGiaDichVuKhamBenhs)
                      .HasForeignKey(rf => rf.PhongBenhVienId);

                builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                  .WithMany(r => r.PhongBenhVienNhomGiaDichVuKhamBenh)
                  .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
