namespace mvmclean.backend.WebApp.Areas.Api.Models;

public class BasketModel
{
    public string SessionId { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalDuration { get; set; }

    public void UpdateTotal()
    {
        TotalAmount = Items.Sum(i => i.Price * i.Quantity);
        TotalDuration = Items.Sum(i => i.Duration * i.Quantity);
    }
}

public class BasketItem
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public int Quantity { get; set; }
}

public class AddToBasketRequest
{
    public string SessionId { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
}

public class RemoveFromBasketRequest
{
    public string SessionId { get; set; }
    public Guid ServiceId { get; set; }
}

public class UpdateQuantityRequest
{
    public string SessionId { get; set; }
    public Guid ServiceId { get; set; }
    public int Quantity { get; set; }
}
