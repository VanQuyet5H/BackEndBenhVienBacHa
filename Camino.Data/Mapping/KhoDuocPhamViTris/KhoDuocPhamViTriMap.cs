using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoDuocPhamViTris
{
    public class KhoDuocPhamViTriMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.KhoDuocPhamViTris.KhoViTri> builder)
        {
            builder.ToTable(MappingDefaults.KhoDuocPhamViTriTable);

            builder.HasOne(rf => rf.KhoDuocPham)
                .WithMany(r => r.KhoDuocPhamViTris)
                .HasForeignKey(rf => rf.KhoId);
            base.Configure(builder);
        }
    }
}
