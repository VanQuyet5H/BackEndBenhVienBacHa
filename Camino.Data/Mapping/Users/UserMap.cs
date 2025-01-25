using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Users
{
    public class UserMap : CaminoEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(MappingDefaults.UserTable);

            builder.Property(u => u.Email).HasMaxLength(100);
            builder.Property(u => u.PassCode).HasMaxLength(50);

            base.Configure(builder);
        }
    }
}
