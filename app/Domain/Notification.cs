using System;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Notification {
    [Key]
    public int NotificationId { get; set; }

    [Required]
    public DateTime Date;

    [Required]
    public File File;

    [Required]
    public User User;

    public Permission Permission;
    public Metadata Metadata;

    public Notification() { }
  }
}