using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.TemplateKhamTheoDoi
{
    public class TemplateKhamTheoDoiMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.PhauThuatThuThuats.TemplateKhamTheoDoi>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.PhauThuatThuThuats.TemplateKhamTheoDoi> builder)
        {
            builder.ToTable(MappingDefaults.TemplateKhamTheoDoi);
            base.Configure(builder);
        }
    }
}