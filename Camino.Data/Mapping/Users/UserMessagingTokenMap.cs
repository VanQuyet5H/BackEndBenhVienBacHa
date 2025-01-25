using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Users
{
    public class UserMessagingTokenMap : CaminoEntityTypeConfiguration<UserMessagingToken>
    {
        public override void Configure(EntityTypeBuilder<UserMessagingToken> builder)
        {
            builder.ToTable(MappingDefaults.UserMessagingTokenTable);

            builder.HasOne(rf => rf.User)
                .WithMany(r => r.UserMessagingTokens)
                .HasForeignKey(rf => rf.UserId);

            base.Configure(builder);
        }
    }
}
