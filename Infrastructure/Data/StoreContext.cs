using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// options will be sql server connection string
public class StoreContext(DbContextOptions options): DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}
