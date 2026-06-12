using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class CustomDictionaryWordMap : ClassMap<CustomDictionaryWord>
{
    public CustomDictionaryWordMap()
    {
        Table("CustomDictionaryWords");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Word).Length(200).Not.Nullable().Unique();
        Map(x => x.SyllableCount);
        References(x => x.AddedBy).Column("AddedByUserId").ForeignKey("FK_CDW_Users").Not.Nullable();
        Map(x => x.AddedAt).Not.Nullable();
        Map(x => x.IsApproved);
    }
}
