using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Permission {
    [Key]
    public int Id { get; set; }

    [Required]
    public File File { get; set; }

    [Required]
    public User User { get; set; }

    public bool CanDownload { get; set; } = false;
    public bool CanUpload { get; set; } = false;
    public bool CanNotify { get; set; } = false;

    public Permission() { }
  }
}