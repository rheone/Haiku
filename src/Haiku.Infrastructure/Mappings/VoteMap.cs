using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class VoteMap : ClassMap<Vote>
{
    public VoteMap()
    {
        Table("Votes");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.Poem).Column("HaikuId").ForeignKey("FK_Votes_Haikus").Not.Nullable();
        References(x => x.User).Column("UserId").ForeignKey("FK_Votes_Users").Not.Nullable();
        Map(x => x.Value).CustomType<sbyte>();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
