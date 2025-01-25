using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuocPhamBenhVienMapping
{
    public class DuocPhamBenhVienMayXetNghiemMap : CaminoEntityTypeConfiguration<DuocPhamBenhVienMayXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<DuocPhamBenhVienMayXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.DuocPhamBenhVienMayXetNghiemTable);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.DuocPhamBenhVienMayXetNghiems)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.MayXetNghiem)
                .WithMany(r => r.DuocPhamBenhVienMayXetNghiems)
                .HasForeignKey(rf => rf.MayXetNghiemId);

            base.Configure(builder);
        }
    }
}
