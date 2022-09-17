using Microservice.Domain.Models;
using Microservice.Infrastructure.Seeds.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Infrastructure.Persistence.PostgreSQL
{
    public class UsersTableConfiguration : IEntityTypeConfiguration<User>
    {
        /*
            POSTGRESQL NAMING CONVENTIONS:

                SQL keywords: UPPER CASE
                identifiers (names of databases, tables, columns, etc): lower_case_with_underscores
        */
        private readonly string _tableName = "users";

        public void Configure(EntityTypeBuilder<User> builder)
        {
            //Table Name (schemaName is setted by the dbCtx)
            builder.ToTable(this._tableName);
            //Primary Key 
            builder.HasKey(user => user.UserName);
            //Columns Settings
            builder.Property(user => user.UserName).IsRequired().HasMaxLength(30).HasColumnType("CHAR(30)").HasColumnName("username");
            builder.Property(user => user.Name).IsRequired().HasMaxLength(50).HasColumnType("CHAR(50)").HasColumnName("name");
            builder.Property(user => user.Surname).IsRequired().HasMaxLength(50).HasColumnType("CHAR(50)").HasColumnName("surname");
            builder.Property(user => user.Email).IsRequired().HasMaxLength(80).HasColumnType("CHAR(80)").HasColumnName("email");
            builder.Property(user => user.Password).IsRequired().HasMaxLength(256).HasColumnType("CHAR(256)").HasColumnName("password");
            //Indeces
            builder.HasIndex(x => x.UserName);
            builder.HasIndex(x => x.Email).IsUnique(true);
            //Initial Data
            builder.HasData(UsersSeeds.Seeds);

        }
    }
}
