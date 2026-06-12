using FluentNHibernate.Mapping;
using Haiku.Domain.Entities;

namespace Haiku.Infrastructure.Mappings;

public class ModerationActionMap : ClassMap<ModerationAction>
{
    public ModerationActionMap()
    {
        Table("ModerationActions");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.ActionType).Length(50).Not.Nullable();
        Map(x => x.TargetType).Length(20).Not.Nullable();
        Map(x => x.TargetId).Not.Nullable();
        References(x => x.ActionedBy).Column("ActionedByUserId").ForeignKey("FK_MA_Users").Not.Nullable();
        Map(x => x.Reason).Length(500).Not.Nullable();
        Map(x => x.CreatedAt).Not.Nullable();
    }
}
