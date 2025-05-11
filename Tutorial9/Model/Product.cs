using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class Product
{
    public int IdProduct { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Description { get; set; }
    [Range(0, Int32.MaxValue)] // TODO
    public int Price { get; set; }
}