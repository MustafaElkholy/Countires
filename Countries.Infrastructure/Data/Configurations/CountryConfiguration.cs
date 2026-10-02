using Countries.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Countries.Infrastructure.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            builder.HasKey(country => country.Id);

            builder.Property(country => country.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(country => country.Code)
                .IsRequired()
                .HasMaxLength(2)
                .IsUnicode(false);

            builder.Property(country => country.IsDeleted)
                .HasDefaultValue(false);

            // Added a global query filter to exclude soft-deleted countries
            builder.HasQueryFilter(country => !country.IsDeleted);


        }
    }
}
