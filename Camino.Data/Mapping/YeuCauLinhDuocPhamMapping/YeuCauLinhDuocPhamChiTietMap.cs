using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.YeuCauLinhDuocPhamMapping
{
    public class YeuCauLinhDuocPhamChiTietMap : CaminoEntityTypeConfiguration<YeuCauLinhDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauLinhDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauLinhDuocPhamChiTietTable);

            builder.HasOne(rf => rf.YeuCauLinhDuocPham)
                .WithMany(r => r.YeuCauLinhDuocPhamChiTiets)
                .HasForeignKey(rf => rf.YeuCauLinhDuocPhamId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.YeuCauLinhDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.YeuCauDuocPhamBenhVien)
                .WithMany(r => r.YeuCauLinhDuocPhamChiTiets)
                .HasForeignKey(rf => rf.YeuCauDuocPhamBenhVienId);
        
            base.Configure(builder);
        }
    }
}
