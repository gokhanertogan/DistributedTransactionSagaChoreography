using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Persistence.EntityConfigurations;

public class OrderOutboxConfiguration : IEntityTypeConfiguration<OrderOutbox>
{
    public void Configure(EntityTypeBuilder<OrderOutbox> builder)
    {
        builder.HasKey(p => p.IdempotentToken);
    }
}
