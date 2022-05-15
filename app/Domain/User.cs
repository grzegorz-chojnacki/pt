using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class User {
    [Key]
    public int UserId;

    [Required]
    public int Login;

    [Required]
    public int Password;

    public bool IsLocked = false;

    public bool IsAdmin = false;

    public HashSet<string> IPs = new HashSet<string>();

    public User() { }
  }
}