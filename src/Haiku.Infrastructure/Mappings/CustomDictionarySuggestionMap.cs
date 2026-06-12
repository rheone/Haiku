using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;
using Haiku.Domain.Enums;

namespace Haiku.Infrastructure.Mappings;

public class CustomDictionarySuggestionMap : ClassMap<CustomDictionarySuggestion>
{
    public CustomDictionarySuggestionMap()
    {
        Table("CustomDictionarySuggestions");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Word).Length(200).Not.Nullable();
        Map(x => x.SuggestedSyllableCount);
        References(x => x.SuggestedBy).Column("SuggestedByUserId").ForeignKey("FK_CDS_SuggestedBy").Not.Nullable();
        Map(x => x.Justification).Length(200).Nullable();
        References(x => x.ReviewedBy).Column("ReviewedByUserId").ForeignKey("FK_CDS_ReviewedBy").Nullable();
        Map(x => x.ReviewedAt).Nullable();
        Map(x => x.Status).Length(20).Not.Nullable().CustomType<DictionarySuggestionStatus>();
    }
}
