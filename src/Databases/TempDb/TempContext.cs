using Cassandra.Data.Linq;

namespace Iowa.Databases.TempDb;

public class TempContext(Cassandra.ISession session)
{
    private readonly Cassandra.ISession _session = session;
    public Table<Tables.SubscriptionBySubscriptionPlan.Table> SubscriptionBySubscriptionPlans => new(_session);
    public Table<Tables.SubscriptionByUserId.Table> SubscriptionByUserIds => new(_session);
}
