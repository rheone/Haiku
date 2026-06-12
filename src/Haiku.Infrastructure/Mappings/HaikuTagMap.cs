using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class HaikuTagMap : ClassMap<HaikuTag>
{
    public HaikuTagMap()
    {
        Table("HaikuTags");
        CompositeId()
            .KeyProperty(x => x.HaikuId)
            .KeyProperty(x => x.TagId);
        References(x => x.Poem).Column("HaikuId").ForeignKey("FK_HaikuTags_Haikus").Not.Nullable().Not.Insert().Not.Update();
        References(x => x.Tag).Column("TagId").ForeignKey("FK_HaikuTags_Tags").Not.Nullable().Not.Insert().Not.Update();
    }
}
