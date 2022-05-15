

namespace app.Domain {
  public partial class DatabaseModel : DbContext {
    public virtual DbSet<User> User;
    public virtual DbSet<File> File;
    public virtual DbSet<Metadata> Metadata;
    public virtual DbSet<Permission> Permission;
    public virtual DbSet<Notification> Notification;

    public DatabaseModel() : base("name=maindb") { }
  }
}