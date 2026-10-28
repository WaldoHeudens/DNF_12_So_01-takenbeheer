namespace Takenbeheer;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// De "een"-kant: een project met meerdere taken.
/// </summary>
public class Project
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Naam { get; set; } = "";

    public List<Taak> Taken { get; set; } = [];
}
