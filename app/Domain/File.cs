using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class File {
    [Key]
    public int Id;

    [Required]
    public string Path;

    [Required]
    public DateTime LastModified;

    [Required]
    public List<Metadata> Metadata = new List<Metadata>();

    [Required]
    public HashSet<Permission> Permission = new HashSet<Permission>();

    public bool isRemoved = false;


    public File() { }
  }
}