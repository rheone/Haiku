using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class LoveMap : ClassMap<Love>
{
    public LoveMap()
    {
        Table("Loves");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.Poem).Column("HaikuId").ForeignKey("FK_Loves_Haikus").Not.Nullable();
        References(x => x.User).Column("UserId").ForeignKey("FK_Loves_Users").Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
