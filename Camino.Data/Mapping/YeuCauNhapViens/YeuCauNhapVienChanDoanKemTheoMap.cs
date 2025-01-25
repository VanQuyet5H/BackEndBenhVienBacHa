using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauNhapViens
{
    public class YeuCauNhapVienChanDoanKemTheoMap : CaminoEntityTypeConfiguration<YeuCauNhapVienChanDoanKemTheo>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapVienChanDoanKemTheo> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapVienChanDoanKemTheoTable);

            builder.HasOne(rf => rf.YeuCauNhapVien)
                .WithMany(r => r.YeuCauNhapVienChanDoanKemTheos)
                .HasForeignKey(rf => rf.YeuCauNhapVienId);

            builder.HasOne(rf => rf.ICD)
                .WithMany(r => r.YeuCauNhapVienChanDoanKemTheos)
                .HasForeignKey(rf => rf.ICDId);

            base.Configure(builder);
        }
    }
}
