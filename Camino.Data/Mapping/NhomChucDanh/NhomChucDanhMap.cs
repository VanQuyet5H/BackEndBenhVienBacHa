using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhomChucDanh
{
    public class NhomChucDanhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh> builder)
        {
            builder.ToTable(MappingDefaults.NhomChucDanhTable);
            base.Configure(builder);
        }
    }
}
