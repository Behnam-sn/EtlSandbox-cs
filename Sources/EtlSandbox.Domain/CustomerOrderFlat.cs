namespace EtlSandbox.Domain;

public sealed class CustomerOrderFlat
{
    public int RentalId { get; set; }
    public string CustomerName { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime RentalDate { get; set; }
    public string Category { get; set; } = null!;
}
