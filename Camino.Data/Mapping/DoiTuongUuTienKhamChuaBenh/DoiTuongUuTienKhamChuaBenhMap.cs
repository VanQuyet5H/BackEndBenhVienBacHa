using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DoiTuongUuTienKhamChuaBenh
{
    public class DoiTuongUuTienKhamChuaBenhMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs.DoiTuongUuTienKhamChuaBenh>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DoiTuongUuTienKhamChuaBenhs.DoiTuongUuTienKhamChuaBenh> builder)
        {
            builder.ToTable(MappingDefaults.DoiTuongUuTienKhamChuaBenhTable);

            base.Configure(builder);
        }
    }
}