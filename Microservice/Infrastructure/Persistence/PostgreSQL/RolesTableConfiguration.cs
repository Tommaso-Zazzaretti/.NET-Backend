using Microservice.Domain.Models;
using Microservice.Infrastructure.Seeds.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Infrastructure.Persistence.PostgreSQL
{
    public class RolesTableConfiguration : IEntityTypeConfiguration<Role>
    {
        /*
            POSTGRESQL NAMING CONVENTIONS:

                SQL keywords: UPPER CASE
                identifiers (names of databases, tables, columns, etc): lower_case_with_underscores
        */
        private readonly string _tableName = "roles";

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            //Table Name (schemaName is setted by the dbCtx)
            builder.ToTable(this._tableName);
            //Primary Key 
            builder.HasKey(role => role.RoleName);
            //Columns Settings
            builder.Property(role => role.RoleName).IsRequired().HasMaxLength(20).HasColumnType("CHAR(20)").HasColumnName("rolename");
            //Indeces
            builder.HasIndex(role => role.RoleName);
            //Initial Data
            builder.HasData(RolesSeeds.Seeds);
        }
    }
}
