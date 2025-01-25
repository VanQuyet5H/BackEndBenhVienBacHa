using Camino.Core.Domain.Entities.TemplateKhamBenhTheoDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.TemplateKhamBenhTheoKhoaMapping
{
    public class TemplateKhamBenhTheoDichVuMap : CaminoEntityTypeConfiguration<TemplateKhamBenhTheoDichVu>
    {
        public override void Configure(EntityTypeBuilder<TemplateKhamBenhTheoDichVu> builder)
        {
            builder.ToTable(MappingDefaults.TemplateKhamBenhTheoDichVuTable);

            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.TemplateKhamBenhTheoDichVus)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
