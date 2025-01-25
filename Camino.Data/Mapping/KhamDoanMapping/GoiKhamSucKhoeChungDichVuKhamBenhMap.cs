using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class GoiKhamSucKhoeChungDichVuKhamBenhMap : CaminoEntityTypeConfiguration<GoiKhamSucKhoeChungDichVuKhamBenh>
    {
        public override void Configure(EntityTypeBuilder<GoiKhamSucKhoeChungDichVuKhamBenh> builder)
        {
            builder.ToTable(MappingDefaults.GoiKhamSucKhoeChungDichVuKhamBenhTable);

            builder.HasOne(x => x.GoiKhamSucKhoeChung)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuKhamBenhs)
                .HasForeignKey(x => x.GoiKhamSucKhoeChungId);

            builder.HasOne(x => x.DichVuKhamBenhBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuKhamBenhs)
                .HasForeignKey(x => x.DichVuKhamBenhBenhVienId);

            builder.HasOne(x => x.NhomGiaDichVuKhamBenhBenhVien)
                .WithMany(y => y.GoiKhamSucKhoeChungDichVuKhamBenhs)
                .HasForeignKey(x => x.NhomGiaDichVuKhamBenhBenhVienId);
            base.Configure(builder);
        }
    }
}
