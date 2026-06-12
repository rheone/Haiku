using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;
using Haiku.Domain.Enums;

namespace Haiku.Infrastructure.Mappings;

public class PoemMap : ClassMap<Poem>
{
    public PoemMap()
    {
        Table("Haikus");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.Author).Column("AuthorId").ForeignKey("FK_Haikus_Users").Not.Nullable();
        Map(x => x.Content).Length(500).Not.Nullable();
        Map(x => x.PoemType).Length(20).Not.Nullable().CustomType<PoemType>();
        Map(x => x.TotalSyllables);
        Map(x => x.IsDraft);
        Map(x => x.IsHidden);
        Map(x => x.CreatedAt).Not.Nullable();
        Map(x => x.DeletedAt).Nullable();
    }
}
