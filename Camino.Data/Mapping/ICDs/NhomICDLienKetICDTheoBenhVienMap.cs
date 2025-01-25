using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ICDs
{
    public class NhomICDLienKetICDTheoBenhVienMap : CaminoEntityTypeConfiguration<NhomICDLienKetICDTheoBenhVien>
    {
        public override void Configure(EntityTypeBuilder<NhomICDLienKetICDTheoBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.NhomICDLienKetICDTheoBenhVienTable);

            builder.HasOne(rf => rf.NhomICDTheoBenhVien)
                .WithMany(r => r.NhomICDLienKetICDTheoBenhViens)
                .HasForeignKey(rf => rf.NhomICDTheoBenhVienId);

            builder.HasOne(rf => rf.ICD)
                .WithMany(r => r.NhomICDLienKetICDTheoBenhViens)
                .HasForeignKey(rf => rf.ICDId);

            base.Configure(builder);
        }
    }
}
