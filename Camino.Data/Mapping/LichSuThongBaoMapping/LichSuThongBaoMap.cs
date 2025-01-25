using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Messages;


namespace Camino.Data.Mapping.LichSuThongBaoMapping
{
    public class LichSuThongBaoMap : CaminoEntityTypeConfiguration<LichSuThongBao>
    {
        public override void Configure(EntityTypeBuilder<LichSuThongBao> builder)
        {
            builder.ToTable(MappingDefaults.LichSuThongBaoTable);
            builder.Property(u => u.GoiDen).HasMaxLength(20);
            base.Configure(builder);
        } 
    }
}
