using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.KhoNhanVienQuanLyMapping
{
    public class KhoNhanVienQuanLyMap : CaminoEntityTypeConfiguration<KhoNhanVienQuanLy>
    {
        public override void Configure(EntityTypeBuilder<KhoNhanVienQuanLy> builder)
        {
            builder.ToTable(MappingDefaults.KhoNhanVienQuanLyTable);

            builder.HasOne(rf => rf.Kho)
                .WithMany(r => r.KhoNhanVienQuanLys)
                .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.KhoNhanVienQuanLys)
                .HasForeignKey(rf => rf.NhanVienId);

            base.Configure(builder);
        }
    }
}
