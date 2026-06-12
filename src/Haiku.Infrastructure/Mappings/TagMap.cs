using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class TagMap : ClassMap<Tag>
{
    public TagMap()
    {
        Table("Tags");
        Id(x => x.Id).GeneratedBy.Identity();
        Map(x => x.Name).Length(100).Not.Nullable().Unique();
    }
}
