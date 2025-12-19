namespace Iowa.SubscriptionByUserIds.Post.Messager;

public record Message(Guid Id, Guid UserId, string SubscriptionPlan, string CompanyName);