using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;
using Haiku.Domain.Enums;

namespace Haiku.Infrastructure.Mappings;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Table("Users");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Email).Length(320).Not.Nullable().Unique();
        Map(x => x.Username).Length(50).Not.Nullable().Unique();
        Map(x => x.PasswordHash).Length(200).Not.Nullable();
        Map(x => x.DisplayName).Length(100);
        Map(x => x.Bio).Length(1000).Nullable();
        Map(x => x.ProfileImageUrl).Length(500).Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
        Map(x => x.EmailVerifiedAt).Nullable();
        Map(x => x.IsDisabled);
        Map(x => x.DeletedAt).Nullable();
        Map(x => x.DeletionChoice).Length(10).Nullable().CustomType<DeletionChoice>();
    }
}
