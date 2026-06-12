using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class PasswordResetTokenMap : ClassMap<PasswordResetToken>
{
    public PasswordResetTokenMap()
    {
        Table("PasswordResetTokens");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.User).Column("UserId").ForeignKey("FK_PRT_Users").Not.Nullable();
        Map(x => x.Token).Length(128).Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
        Map(x => x.ExpiresAt).Not.Nullable();
        Map(x => x.UsedAt).Nullable();
    }
}
