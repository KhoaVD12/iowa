namespace Iowa.SubscriptionByUserIds.Delete.Messager;

public record Message(string SubscriptionPlan, string CompanyName, Guid UserId);
