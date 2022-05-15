using System;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Metadata {
    [Key]
    public int MetadataId { get; set; }

    [Required]
    public File file;

    [Required]
    public DateTime Modified;

    // Dublin Core
    public string Contributor = "";
    public string Coverage = "";
    public string Creator = "";
    public string Date = "";
    public string Description = "";
    public string Format = "";
    public string Identifier = "";
    public string Language = "";
    public string Publisher = "";
    public string Relation = "";
    public string Rights = "";
    public string Source = "";
    public string Subject = "";
    public string Title = "";
    public string Type = "";

    public Metadata() { }
  }
}