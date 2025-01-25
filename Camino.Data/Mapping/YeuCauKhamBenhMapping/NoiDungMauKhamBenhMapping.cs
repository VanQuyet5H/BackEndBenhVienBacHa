using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class NoiDungMauKhamBenhMapping : CaminoEntityTypeConfiguration<NoiDungMauKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<NoiDungMauKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.NoiDungMauKhamBenhTable);

            builder.HasOne(x => x.BacSi)
                .WithMany(rf => rf.BacSiLuuNoiDungKhamBenhs)
                .HasForeignKey(x => x.BacSiId);
            
            base.Configure(builder);
        }
    }
}
