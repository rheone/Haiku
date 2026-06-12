using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class FollowMap : ClassMap<Follow>
{
    public FollowMap()
    {
        Table("Follows");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.Follower).Column("FollowerId").ForeignKey("FK_Follows_Follower").Not.Nullable();
        References(x => x.Followee).Column("FolloweeId").ForeignKey("FK_Follows_Followee").Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
