using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Domain.CustomerOrderFlats.Entities;

public sealed class CustomerOrderFlat : IEntity
{
    public int Id => RentalId;

    public int RentalId { get; set; }

    public string? CustomerName { get; set; }

    public decimal Amount { get; set; }

    public DateTime RentalDate { get; set; }

    public string? Category { get; set; }

    public int UniqId { get; set; }

    public bool IsDeleted { get; set; }

}