using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class BookmarkMap : ClassMap<Bookmark>
{
    public BookmarkMap()
    {
        Table("Bookmarks");
        Id(x => x.Id).GeneratedBy.GuidComb();
        References(x => x.User).Column("UserId").ForeignKey("FK_Bookmarks_Users").Not.Nullable();
        References(x => x.Poem).Column("HaikuId").ForeignKey("FK_Bookmarks_Haikus").Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
