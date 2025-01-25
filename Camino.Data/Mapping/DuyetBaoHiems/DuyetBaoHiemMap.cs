using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuyetBaoHiems
{
    public class DuyetBaoHiemMap : CaminoEntityTypeConfiguration<DuyetBaoHiem>
    {
        public override void Configure(EntityTypeBuilder<DuyetBaoHiem> builder)
        {
            builder.ToTable(MappingDefaults.DuyetBaoHiemTable);

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.DuyetBaoHiems)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);
            builder.HasOne(rf => rf.NoiDuyetBaoHiem)
                .WithMany(r => r.DuyetBaoHiems)
                .HasForeignKey(rf => rf.NoiDuyetBaoHiemId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.DuyetBaoHiems)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            base.Configure(builder);
        }
    }
}
