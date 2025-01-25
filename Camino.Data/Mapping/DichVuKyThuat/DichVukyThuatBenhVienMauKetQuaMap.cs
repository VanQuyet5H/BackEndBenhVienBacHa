using Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVukyThuatBenhVienMauKetQuaMap : CaminoEntityTypeConfiguration<DichVukyThuatBenhVienMauKetQua>
    {
        public override void Configure(EntityTypeBuilder<DichVukyThuatBenhVienMauKetQua> builder)
        {
            builder.ToTable(MappingDefaults.DichVukyThuatBenhVienMauKetQuaTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                   .WithOne(r => r.DichVukyThuatBenhVienMauKetQua)
                   .HasForeignKey<DichVukyThuatBenhVienMauKetQua>(c => c.Id);

            builder.HasOne(rf => rf.NhanVienThucHien)
                   .WithMany(r => r.DichVukyThuatBenhVienMauKetQuas)
                   .HasForeignKey(rf => rf.NhanVienThucHienId);

            base.Configure(builder);
        }
    }
}
