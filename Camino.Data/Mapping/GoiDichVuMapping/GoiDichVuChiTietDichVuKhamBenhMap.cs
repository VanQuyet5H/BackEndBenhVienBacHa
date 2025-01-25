using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GoiDichVuMapping
{
    public class GoiDichVuChiTietDichVuKhamBenhMap : CaminoEntityTypeConfiguration<GoiDichVuChiTietDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<GoiDichVuChiTietDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.GoiDichVuChiTietDichVuKhamBenhTable);

            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                      .WithMany(r => r.GoiDichVuChiTietDichVuKhamBenhs)
                      .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.GoiDichVuChiTietDichVuKhamBenhs)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.GoiDichVu)
                      .WithMany(r => r.GoiDichVuChiTietDichVuKhamBenhs)
                      .HasForeignKey(rf => rf.GoiDichVuId);


            base.Configure(builder);
        }
    }
}
