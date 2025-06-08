namespace EtlSandbox.Domain.CustomerOrderFlats;

public sealed class CustomerOrderFlat
{
    public int RentalId { get; set; }
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime RentalDate { get; set; }
    public string? Category { get; set; }
}
