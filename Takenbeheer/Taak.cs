namespace Takenbeheer;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// De "veel"-kant: een taak hoort bij een project via de foreign key ProjectId.
/// </summary>
public class Taak
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Omschrijving { get; set; } = "";

    public bool Voltooid { get; set; }

    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
