namespace Iowa.Packages.Post.Messager;

public record Message(Guid PackageId, string SubscriptionPlan, string CompanyName);
