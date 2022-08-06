
using Microservice.Domain.Models;
using Microservice.Infrastructure.Seeds.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Infrastructure.Persistence.PostgreSQL
{
    public class UsersRolesTableConfiguration : IEntityTypeConfiguration<UsersRoles>
    {
        /*
            POSTGRESQL NAMING CONVENTIONS:

                SQL keywords: UPPER CASE
                identifiers (names of databases, tables, columns, etc): lower_case_with_underscores
        */
        private readonly string _tableName = "users_roles";

        public void Configure(EntityTypeBuilder<UsersRoles> builder)
        {
            //Table Name (schemaName is setted by the dbCtx)
            builder.ToTable(this._tableName);
            //Primary Key 
            builder.HasKey(row => new { row.UserName, row.RoleName });
            //Columns Settings
            builder.Property(row => row.UserName).IsRequired().HasMaxLength(30).HasColumnType("CHAR(30)").HasColumnName("username");
            builder.Property(row => row.RoleName).IsRequired().HasMaxLength(20).HasColumnType("CHAR(20)").HasColumnName("rolename");
            //Relationships Configurations
            builder.HasOne<User>(row => row.User).WithMany(user => user.UsersRoles).HasForeignKey(row => row.UserName).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<Role>(row => row.Role).WithMany(role => role.UsersRoles).HasForeignKey(row => row.RoleName).OnDelete(DeleteBehavior.Cascade);
            //Indeces
            builder.HasIndex(row => row.UserName);
            builder.HasIndex(row => row.RoleName);
            //Initial Data
            builder.HasData(UsersRolesSeeds.Seeds);
        }
    }
}
