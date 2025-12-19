namespace Iowa.SubscriptionByUserIds.Put.Messager;

public record Message(Guid Id, Guid OldUserId, string OldSubscriptionPlan, string OldCompanyName,
                               Guid NewUserId, string NewSubscriptionPlan, string NewCompanyName);