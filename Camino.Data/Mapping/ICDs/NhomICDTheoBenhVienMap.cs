using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.ICDs
{
    public class NhomICDTheoBenhVienMap : CaminoEntityTypeConfiguration<NhomICDTheoBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomICDTheoBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomICDTheoBenhVienTable);

            builder.HasOne(rf => rf.ChuongICD)
                .WithMany(r => r.NhomICDTheoBenhViens)
                .HasForeignKey(rf => rf.ChuongICDId);

            base.Configure(builder);
        }
    }
}
