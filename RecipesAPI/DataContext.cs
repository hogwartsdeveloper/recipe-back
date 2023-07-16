global using Microsoft.EntityFrameworkCore;

namespace RecipesAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=recipedb;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .Ignore(r => r.Ingredients);
            modelBuilder.Entity<Ingredient>().HasNoKey();
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    }
}

