using System.ComponentModel;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class CartItem
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public string RequestType { get; set; } = "BORROW";
    public DateTime? BorrowStartDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public DateTime? PickupDate { get; set; }
    public int BaseCredit { get; set; }
    public int MaxStock { get; set; }
    public string UnitName { get; set; } = "unit";
    public int? ProductVariantId { get; set; }
}

public class CartService
{
    private List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public event Action? OnChange;

    public void AddToCart(CartItem item)
    {
        var existing = _items.FirstOrDefault(i => i.Sku == item.Sku && i.RequestType == item.RequestType);
        if (existing != null)
        {
            if (existing.Quantity + item.Quantity <= existing.MaxStock)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                existing.Quantity = existing.MaxStock;
            }
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
        NotifyStateChanged();
    }

    public int GetTotalCredit()
    {
        return _items.Where(i => i.RequestType == "GIVEAWAY").Sum(i => i.Quantity * i.BaseCredit);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
