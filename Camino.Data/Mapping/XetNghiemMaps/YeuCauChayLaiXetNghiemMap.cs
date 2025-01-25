using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class YeuCauChayLaiXetNghiemMap : CaminoEntityTypeConfiguration<YeuCauChayLaiXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<YeuCauChayLaiXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauChayLaiXetNghiemTable);

            builder.HasOne(rf => rf.PhienXetNghiem)
                .WithMany(r => r.YeuCauChayLaiXetNghiems)
                .HasForeignKey(rf => rf.PhienXetNghiemId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.YeuCauChayLaiXetNghiems)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauChayLaiXetNghiemNhanVienYeuCaus)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauChayLaiXetNghiemNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
