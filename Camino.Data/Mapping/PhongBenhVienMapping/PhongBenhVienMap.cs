using Camino.Core.Domain.Entities.PhongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.PhongBenhVienMapping
{
    public class PhongBenhVienMap : CaminoEntityTypeConfiguration<PhongBenhVien>
    {
        public override void Configure(EntityTypeBuilder<PhongBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.PhongBenhVienTable);

                builder.HasOne(rf => rf.KhoaPhong)
               .WithMany(r => r.PhongBenhViens)
               .HasForeignKey(rf => rf.KhoaPhongId);

                builder.HasOne(rf => rf.HopDongKhamSucKhoe)
                    .WithMany(r => r.PhongBenhViens)
                    .HasForeignKey(rf => rf.HopDongKhamSucKhoeId);

            base.Configure(builder);
        }
    }
}
