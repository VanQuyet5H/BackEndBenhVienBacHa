using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.NhapKhoQuaTangs
{
    public class NhapKhoQuaTangMap : CaminoEntityTypeConfiguration<NhapKhoQuaTang>
    {
        public override void Configure(EntityTypeBuilder<NhapKhoQuaTang> builder)
        {
            builder.ToTable(MappingDefaults.NhapKhoQuaTangTable);

            builder
                .HasOne(sc => sc.NguoiNhap)
                .WithMany(s => s.NhapKhoQuaTangNguoiNhaps)
                .HasForeignKey(sc => sc.NguoiNhapId);

            builder
                .HasOne(sc => sc.NguoiGiao)
                .WithMany(s => s.NhapKhoQuaTangNguoiGiaos)
                .HasForeignKey(sc => sc.NguoiGiaoId);


            base.Configure(builder);
        }
    }
}
