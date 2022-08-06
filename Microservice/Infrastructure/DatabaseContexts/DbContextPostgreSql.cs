using Microservice.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Microservice.Infrastructure.DatabaseContexts
{
    public class DbContextPostgreSql : DbContext
    {
        //Tables 
        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }

        //Migration schema name
        private readonly string _defaultSchema = "web";

        public DbContextPostgreSql(DbContextOptions<DbContextPostgreSql> options)  : base(options) 
        {
            
        }

        /*
            Configure NpgsqlDbContextOptions used during Startup.ConfigureServices()
        */
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(x => x.MigrationsHistoryTable("MigrationsTable"));
        }

        /* 
            Migration Script configurations:

                -GENERATE MIGRATION: dotnet ef migrations add PostegreSQL_init --output-dir ./Infrastructure/Migrations/PostgreSQL --context DbContextPostgreSql

                -APPLY MIGRATION:    dotnet ef database update PostegreSQL_init --context DbContextPostgreSql

                -UNAPPLY MIGRATION:  dotnet ef database update 0 --context DbContextPostgreSql
        */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(this._defaultSchema);
            //Get a Set of all DbContext model types. For example: [DbSet<User>, DBSet<Role>] become an HashSet<Type>={User,Role}  
            HashSet<Type> DbSetTypes = builder.Model.GetEntityTypes().Select(t => t.ClrType).ToHashSet();
            //Use all persistence classes that implement the IEntityTypeConfiguration<Type> having the Type T included in some DbSet<T>
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), T => this.FilterIEntityTypeConfigurationClass(T, DbSetTypes));
            base.OnModelCreating(builder);
        }

        protected bool FilterIEntityTypeConfigurationClass(Type type, HashSet<Type> DbSetTypes)
        {
            IEnumerable<Type> ImplementedInterfaces = type.GetInterfaces();
            foreach (Type implInterface in ImplementedInterfaces) { //Find an IEntityTypeConfiguration<T> interface with T included by DbSetTypes
                if (!implInterface.IsGenericType) { continue; }
                if (implInterface.GetGenericTypeDefinition() != typeof(IEntityTypeConfiguration<>)) { continue; }
                if (!DbSetTypes.Contains(implInterface.GenericTypeArguments[0])) { continue; }
                return true;
            }
            return false;
        }
    }

}
