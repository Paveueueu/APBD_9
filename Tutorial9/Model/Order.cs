using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Tutorial9.Model;

public class Order
{
    public int IdOrder { get; set; }
    public int IdProduct { get; set; }
    [Range(0, Int32.MaxValue)]
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FulfilledAt { get; set; }
}