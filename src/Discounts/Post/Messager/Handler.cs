namespace Iowa.Discounts.Post.Messager
{
    public class Handler
    {
        public Handler()
        {
            
        }
        public async Task Handle(Message message)
        {
            Console.WriteLine($"You have created a new Discount with id:{message.id}");
        }
    }
}