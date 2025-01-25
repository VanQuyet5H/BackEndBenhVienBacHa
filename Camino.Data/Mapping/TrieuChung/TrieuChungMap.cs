using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.TrieuChung
{
   public class TrieuChungMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.TrieuChungs.TrieuChung>
   {
       public override void Configure(EntityTypeBuilder<Core.Domain.Entities.TrieuChungs.TrieuChung> builder)
       {
           builder.ToTable(MappingDefaults.TrieuChungTable);
            builder.HasOne(rf => rf.TrieuChungCha)
                  .WithMany(r => r.TrieuChungs)
                  .HasForeignKey(rf => rf.TrieuChungChaId);
            base.Configure(builder);
       }
   }
   
}
