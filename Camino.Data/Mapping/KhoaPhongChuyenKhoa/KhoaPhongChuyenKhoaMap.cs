using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoaPhongChuyenKhoa
{
    public class KhoaPhongChuyenKhoaMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.KhoaPhongChuyenKhoas.KhoaPhongChuyenKhoa> builder)
        {
            builder.ToTable(MappingDefaults.KhoaPhongChuyenKhoaTable);

            builder
                .HasOne(sc => sc.KhoaPhong)
                .WithMany(s => s.KhoaPhongChuyenKhoas)
                .HasForeignKey(sc => sc.KhoaPhongId);


            builder
                .HasOne(sc => sc.Khoa)
                .WithMany(s => s.KhoaPhongChuyenKhoas)
                .HasForeignKey(sc => sc.KhoaId);

            base.Configure(builder);
        }
    }
}
