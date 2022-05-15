using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Permission {
    [Key]
    public int PermissionId { get; set; }

    [Required]
    public File File;

    [Required]
    public User User;

    public bool CanDownload = false;
    public bool CanUpload = false;
    public bool CanNotify = false;

    public Permission() { }
  }
}