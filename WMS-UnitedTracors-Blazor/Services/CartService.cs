using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class CartItem
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public string RequestType { get; set; } = "BORROW";
    public int BaseCredit { get; set; }
    public int MaxStock { get; set; }
    public string UnitName { get; set; } = "unit";
    public int? ProductVariantId { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
}

public class CartService
{
    private List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public event Action? OnChange;
    public event Action? OnShowCart;
    public event Action<string>? OnShowToast;

    public void ShowCart() => OnShowCart?.Invoke();
    public void ShowToast(string message) => OnShowToast?.Invoke(message);

    public string? EventName { get; set; }
    public DateTime? EventStartDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public DateTime? EventDate { get => EventStartDate; set => EventStartDate = value; }
    public DateTime? BorrowStartDate { get; set; }
    public DateTime? BorrowEndDate { get; set; }
    public DateTime? GiveawayPickupDate { get; set; }
    public string? Notes { get; set; }
    public string? DocumentationLink { get; set; }

    public void AddToCart(CartItem item)
    {
        var existing = _items.FirstOrDefault(i => i.Sku == item.Sku && i.RequestType == item.RequestType && i.ProductVariantId == item.ProductVariantId);
        if (existing != null)
        {
            if (existing.Quantity + item.Quantity <= existing.MaxStock)
                existing.Quantity += item.Quantity;
            else
                existing.Quantity = existing.MaxStock;
        }
        else
        {
            _items.Add(item);
        }
        NotifyStateChanged();
    }

    public void RemoveItem(CartItem item)
    {
        _items.Remove(item);
        NotifyStateChanged();
    }

    public void ClearCart()
    {
        _items.Clear();
        EventName = null;
        EventStartDate = null;
        EventEndDate = null;
        BorrowStartDate = null;
        BorrowEndDate = null;
        GiveawayPickupDate = null;
        Notes = null;
        DocumentationLink = null;
        NotifyStateChanged();
    }

    public int GetTotalCredit()
    {
        return _items.Where(i => i.RequestType == "GIVEAWAY").Sum(i => i.Quantity * i.BaseCredit);
    }

    public void NotifyChange() => OnChange?.Invoke();

    private void NotifyStateChanged() => OnChange?.Invoke();
}
