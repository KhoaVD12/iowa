namespace Iowa.SubscriptionByUserIds.Put.Messager;

public record Message(Guid Id, Guid UserId, string SubscriptionPlan, string CompanyName);