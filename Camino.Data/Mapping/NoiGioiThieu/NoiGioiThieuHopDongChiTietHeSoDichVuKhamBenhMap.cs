using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhMap : CaminoEntityTypeConfiguration<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhTable);

            builder.HasOne(rf => rf.NoiGioiThieuHopDong)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs)
                .HasForeignKey(rf => rf.NoiGioiThieuHopDongId);
            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);
            builder.HasOne(rf => rf.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(r => r.NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs)
                .HasForeignKey(rf => rf.NhomGiaDichVuKhamBenhBenhVienId);

            base.Configure(builder);
        }
    }
}
