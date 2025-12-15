namespace Iowa.SubscriptionBySubscriptionPlan.Post.Messager;

public record Message
(Guid SubscriptionPlanId, Guid UserId, string SubscriptionPlan);