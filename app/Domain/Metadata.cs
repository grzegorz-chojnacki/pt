using System;
using System.ComponentModel.DataAnnotations;

namespace app.Domain {
  public partial class Metadata {
    [Key]
    public int Id { get; set; }

    [Required]
    public File File { get; set; }

    [Required]
    public DateTime Modified { get; set; }

    // Dublin Core
    public string Contributor { get; set; } = "";
    public string Coverage { get; set; } = "";
    public string Creator { get; set; } = "";
    public string Date { get; set; } = "";
    public string Description { get; set; } = "";
    public string Format { get; set; } = "";
    public string Identifier { get; set; } = "";
    public string Language { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string Relation { get; set; } = "";
    public string Rights { get; set; } = "";
    public string Source { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";

    public Metadata() { }
  }
}