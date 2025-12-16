namespace Iowa.SubscriptionByUserIds.Post.Messager;

public record Message(Guid UserId, string SubscriptionPlan, string CompanyName);