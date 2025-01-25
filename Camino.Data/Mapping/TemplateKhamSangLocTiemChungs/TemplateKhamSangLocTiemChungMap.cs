using Camino.Core.Domain.Entities.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.TemplateKhamSangLocTiemChungs
{
    public class TemplateKhamSangLocTiemChungMap : CaminoEntityTypeConfiguration<TemplateKhamSangLocTiemChung>
    {
        public override void Configure(EntityTypeBuilder<TemplateKhamSangLocTiemChung> builder)
        {
            builder.ToTable(MappingDefaults.TemplateKhamSangLocTiemChungTable);

            base.Configure(builder);
        }
    }
}