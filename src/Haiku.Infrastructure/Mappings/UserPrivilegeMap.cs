using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class UserPrivilegeMap : ClassMap<UserPrivilege>
{
    public UserPrivilegeMap()
    {
        Table("UserPrivileges");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.User).Column("UserId").ForeignKey("FK_UP_Users").Not.Nullable();
        Map(x => x.Privilege).Length(50).Not.Nullable();
        References(x => x.GrantedBy).Column("GrantedByUserId").ForeignKey("FK_UP_GrantedBy").Not.Nullable();
        Map(x => x.GrantedAt).Not.Nullable();
    }
}
