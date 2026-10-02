using Countries.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Countries.Infrastructure.Data.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");

            builder.HasKey(city => city.Id);

            builder.Property(city => city.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(city => city.CountryId)
                .IsRequired();


            builder.HasOne(city => city.Country)
                .WithMany(country => country.Cities)
                .HasForeignKey(city => city.CountryId)
                //prevents deleting a country entity if any city entity still reference it
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(city => city.IsDeleted)
                .HasDefaultValue(false);

            // Added a global query filter to exclude soft-deleted cities and not deleted countries from queries
            builder.HasQueryFilter(city => !city.IsDeleted && !city.Country.IsDeleted);
        }
    }
}
