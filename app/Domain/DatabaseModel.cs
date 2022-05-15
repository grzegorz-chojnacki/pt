using System.Data.Entity;

namespace app.Domain {
  public partial class DatabaseModel : DbContext {
    public DbSet<User> User;
    public DbSet<File> File;
    public DbSet<Metadata> Metadata;
    public DbSet<Permission> Permission;
    public DbSet<Notification> Notification;

    public DatabaseModel() : base("name=maindb") { }
  }
}