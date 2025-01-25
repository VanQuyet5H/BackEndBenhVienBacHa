using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamDoanMapping
{
    public class NhanSuKhamSucKhoeTaiLieuDinhKemMap : CaminoEntityTypeConfiguration<NhanSuKhamSucKhoeTaiLieuDinhKem>
    {
        public override void Configure(EntityTypeBuilder<NhanSuKhamSucKhoeTaiLieuDinhKem> builder)
        {
            builder.ToTable(MappingDefaults.NhanSuKhamSucKhoeTaiLieuDinhKemTable);
            base.Configure(builder);
        }
    }
}
