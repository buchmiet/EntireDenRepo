using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace denWebServicesNET80.Models;

public class UsersAndClientsDbContext : DbContext
{
    public UsersAndClientsDbContext(DbContextOptions<UsersAndClientsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMaxClientsAssociation>()
            .HasKey(u => u.UserId);
        modelBuilder.Entity<UserClientNames>()
            .HasIndex(u => new { u.UserId, u.ClientName })
            .IsUnique();
    }

    public DbSet<UserMaxClientsAssociation> UserMaxClientsAssociations { get; set; }
    public DbSet<UserClientNames> UserClientNamess { get; set; }
    public DbSet<LoggedInClient> LoggedInClients { get; set; }
    public DbSet<SensitiveInformation> SensitiveInformations { get; set; }

}

public class SensitiveInformation
{
    [Key]
    public int SensitiveInformationId { get; set; } //PK
    [ForeignKey("UserMaxClientsAssociation")]
    public int UserId { get; set; } // FK to UserMaxClientsAssociation
    public string DBHost { get; set; }
    public string DBPort { get; set; }
    public string DBUserName { get; set; }
    public string DBPassword { get; set; }
    public string DBname { get; set; }
}


public class LoggedInClient
{
    [Key]
    public int LoggedInClientId { get; set; } //PK
    [ForeignKey("UserClientNames")]
    public int UserClientNamesId { get; set; }  // FK to  UserClientNames
    public string Handshake { get; set; }
    public bool IsConnected { get; set; }
    public DateTime TimeCreated { get; set; }
    public string ConnectionId { get; set; }
   
}


public class UserMaxClientsAssociation
{
    [Key]
    public int UserId { get; set; } //PK
    public string UserName { get; set; }
    public int MaxClients { get; set; }
}

public class UserClientNames
{
    [Key]
    public int UserClientNamesId { get; set; } //PK
    [ForeignKey("UserMaxClientsAssociation")]
    public int UserId { get; set; } // FK to UserMaxClientsAssociation
    public string ClientName { get; set; } // UserId-ClientName combination should be unique
      
}