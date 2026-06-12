using NHibernate;

namespace Haiku.Infrastructure;

public class ScopedSession : IDisposable
{
    public ISession Session { get; }

    public ScopedSession(NHibernateSessionFactory sessionFactory)
    {
        Session = sessionFactory.OpenSession();
    }

    public void Dispose()
    {
        Session?.Dispose();
    }
}
