using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mvmclean.backend.Domain.Aggregates.Invoice;

namespace mvmclean.backend.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : EntityConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);

        builder.OwnsMany(o => o.LineItems, li =>
        {
            li.Property<int>("Id"); 
            li.OwnsOne(i=>i.UnitPrice);
            li.Property(x => x.Quantity);
            li.Property(x => x.Description);
        });

        builder.OwnsOne(i => i.PaymentTerms);

        builder.OwnsOne(i => i.DiscountAmount);
        builder.OwnsOne(i => i.Subtotal);
        builder.OwnsOne(i => i.TotalAmount);


       

        

    }
}