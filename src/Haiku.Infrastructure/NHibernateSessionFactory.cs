using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Haiku.Infrastructure;

public class NHibernateSessionFactory
{
    private readonly ISessionFactory _sessionFactory;

    public NHibernateSessionFactory(string connectionString)
    {
        _sessionFactory = Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(connectionString)
                .ShowSql())
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateSessionFactory>())
            .BuildSessionFactory();
    }

    public ISession OpenSession() => _sessionFactory.OpenSession();
}
