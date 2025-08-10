CREATE
DATABASE SakilaFlat;

CREATE TABLE CustomerOrderFlats
(
    Id           Int64,
    RentalId     Int64,
    CustomerName Nullable(String),
    Amount       Decimal(10, 2),
    RentalDate   String,
    Category     Nullable(String),
    IsDeleted    UInt8
) ENGINE = MergeTree()
ORDER BY (Id);

