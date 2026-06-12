using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class UserVerificationTokenMap : ClassMap<UserVerificationToken>
{
    public UserVerificationTokenMap()
    {
        Table("UserVerificationTokens");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.User).Column("UserId").ForeignKey("FK_UVT_Users").Not.Nullable();
        Map(x => x.Token).Length(128).Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
        Map(x => x.ExpiresAt).Not.Nullable();
        Map(x => x.UsedAt).Nullable();
    }
}
