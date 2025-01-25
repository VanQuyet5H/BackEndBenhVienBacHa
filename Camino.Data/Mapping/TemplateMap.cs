using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping
{
    public class TemplateMap : CaminoEntityTypeConfiguration<Template>
    {
        public override void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.ToTable(MappingDefaults.TemplateTable);

         

            base.Configure(builder);
        }
    }
}
