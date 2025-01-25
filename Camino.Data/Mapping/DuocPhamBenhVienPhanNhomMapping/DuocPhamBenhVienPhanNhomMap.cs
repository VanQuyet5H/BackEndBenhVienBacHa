using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuocPhamBenhVienPhanNhomMapping
{
    public class DuocPhamBenhVienPhanNhomMap : CaminoEntityTypeConfiguration<DuocPhamBenhVienPhanNhom>
    {
        public override void Configure(EntityTypeBuilder<DuocPhamBenhVienPhanNhom> builder)
        {
            builder.ToTable(MappingDefaults.DuocPhamBenhVienPhanNhomTable);

            builder.HasOne(rf => rf.NhomCha)
                .WithMany(r => r.DuocPhamBenhVienPhanNhoms)
                .HasForeignKey(rf => rf.NhomChaId);

            base.Configure(builder);
        }
    }
}
