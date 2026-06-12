using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class TagDailyCountMap : ClassMap<TagDailyCount>
{
    public TagDailyCountMap()
    {
        Table("TagDailyCounts");
        CompositeId()
            .KeyProperty(x => x.TagId)
            .KeyProperty(x => x.Date);
        Map(x => x.Count);
        References(x => x.Tag).Column("TagId").ForeignKey("FK_TDC_Tags").Not.Nullable().Not.Insert().Not.Update();
    }
}
