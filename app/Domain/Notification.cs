using System;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Notification {
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public File File { get; set; }

    [Required]
    public User User { get; set; }

    public Permission Permission { get; set; }
    public Metadata Metadata { get; set; }

    public Notification() { }
  }
}