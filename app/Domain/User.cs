using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class User {
    [Key]
    public int Id { get; set; }

    [Required]
    public int Login { get; set; }

    [Required]
    public int Password { get; set; }

    public bool IsLocked { get; set; } = false;

    public bool IsAdmin { get; set; } = false;

    public List<string> IPs { get; set; } = new List<string>();

    public User() { }
  }
}