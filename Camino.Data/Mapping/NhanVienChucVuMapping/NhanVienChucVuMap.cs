using Camino.Core.Domain.Entities.NhanVienChucVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhanVienChucVuMapping
{
    public class NhanVienChucVuMap : CaminoEntityTypeConfiguration<NhanVienChucVu>
    {
        public override void Configure(EntityTypeBuilder<NhanVienChucVu> builder)
        {
            builder.ToTable(MappingDefaults.NhanVienChucVuTable);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.NhanVienChucVus)
                .HasForeignKey(rf => rf.NhanVienId);

            builder.HasOne(rf => rf.ChucVu)
                .WithMany(r => r.NhanVienChucVus)
                .HasForeignKey(rf => rf.ChucVuId);

            base.Configure(builder);
        }
    }
}
