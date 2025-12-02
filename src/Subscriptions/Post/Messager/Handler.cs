namespace Iowa.Subscriptions.Post.Messager;

public class Handler
{
    public Handler()
    {
        
    }
    public async Task Handle(Message message)
    {
        Console.WriteLine($"You have created a new Subscriptions with id:{message.id}");
    }
}
