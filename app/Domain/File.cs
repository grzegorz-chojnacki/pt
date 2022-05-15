using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class File {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Path { get; set; }

    [Required]
    public DateTime LastModified { get; set; }

    [Required]
    public List<Metadata> Metadata { get; set; } = new List<Metadata>();

    [Required]
    public HashSet<Permission> Permission { get; set; } = new HashSet<Permission>();

    public bool IsRemoved { get; set; } = false;


    public File() { }
  }
}