using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienDinhMucDuocPhamVatTuMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienDinhMucDuocPhamVatTuTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.VatTuBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
                .HasForeignKey(rf => rf.VatTuBenhVienId);

            base.Configure(builder);
        }
    }
}