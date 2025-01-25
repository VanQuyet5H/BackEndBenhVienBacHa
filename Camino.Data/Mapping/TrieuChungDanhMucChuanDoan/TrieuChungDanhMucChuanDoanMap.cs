using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.TrieuChungDanhMucChuanDoan
{
    public class TrieuChungDanhMucChuanDoanMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.TrieuChungDanhMucChuanDoans.TrieuChungDanhMucChuanDoan>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.TrieuChungDanhMucChuanDoans.TrieuChungDanhMucChuanDoan> builder)
        {
            builder.ToTable(MappingDefaults.TrieuChungDanhMucChuanDoanTable);
            builder
                .HasOne(sc => sc.TrieuChung)
                .WithMany(s => s.TrieuChungDanhMucChuanDoans)
                .HasForeignKey(sc => sc.TrieuChungId);


            builder
                .HasOne(sc => sc.DanhMucChuanDoan)
                .WithMany(s => s.TrieuChungDanhMucChuanDoans)
                .HasForeignKey(sc => sc.DanhMucChuanDoanId);
            base.Configure(builder);
        }
    }
}
