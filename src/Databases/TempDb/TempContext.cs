using Cassandra.Data.Linq;

namespace Iowa.Databases.TempDb;

public class TempContext(Cassandra.ISession session)
{
    private readonly Cassandra.ISession _session = session;
    public Table<Tables.UserIdBySubscriptionPlan.Table> UserIdBySubscriptionPlans => new(_session);
    public Table<Tables.SubscriptionByUserId.Table> SubscriptionByUserIds => new(_session);
}
