using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruBenhAnMap : CaminoEntityTypeConfiguration<NoiTruBenhAn>
    {
        public override void Configure(EntityTypeBuilder<NoiTruBenhAn> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruBenhAnTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithOne(r => r.NoiTruBenhAn).HasForeignKey<NoiTruBenhAn>(c => c.Id);
            
            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.NoiTruBenhAns)
                .HasForeignKey(rf => rf.BenhNhanId);

            builder.HasOne(rf => rf.NhanVienLuuTru)
                .WithMany(r => r.NhanVienLuuTruNoiTruBenhAns)
                .HasForeignKey(rf => rf.NhanVienLuuTruId);

            builder.HasOne(rf => rf.KhoaPhongNhapVien)
                .WithMany(r => r.NoiTruBenhAns)
                .HasForeignKey(rf => rf.KhoaPhongNhapVienId);

            builder.HasOne(rf => rf.NhanVienTaoBenhAn)
                .WithMany(r => r.NhanVienTaoBenhAnNoiTruBenhAns)
                .HasForeignKey(rf => rf.NhanVienTaoBenhAnId);

            builder.HasOne(rf => rf.ChuyenDenBenhVien)
                .WithMany(r => r.NoiTruBenhAns)
                .HasForeignKey(rf => rf.ChuyenDenBenhVienId);

            builder.HasOne(rf => rf.ChanDoanChinhRaVienICD)
                .WithMany(r => r.NoiTruBenhAns)
                .HasForeignKey(rf => rf.ChanDoanChinhRaVienICDId);

            base.Configure(builder);
        }
    }
}
