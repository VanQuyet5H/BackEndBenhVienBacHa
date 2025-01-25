using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuocPhamBenhVienMapping
{
    public class DuocPhamBenhVienMap : CaminoEntityTypeConfiguration<DuocPhamBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DuocPhamBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DuocPhamBenhVienTable);

                    builder.HasOne(rf => rf.DuocPham)
                        .WithOne(r => r.DuocPhamBenhVien).
                        HasForeignKey<DuocPhamBenhVien>(c => c.Id);

            builder.HasOne(rf => rf.DuocPhamBenhVienPhanNhom)
                .WithMany(r => r.DuocPhamBenhViens)
                .HasForeignKey(rf => rf.DuocPhamBenhVienPhanNhomId);

            builder.HasMany(rf => rf.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
               .WithOne(r => r.DuocPhamBenhVien)
               .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            base.Configure(builder);
        }
    }
}
