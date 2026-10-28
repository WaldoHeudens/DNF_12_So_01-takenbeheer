namespace Takenbeheer;

/// <summary>
/// Bevat de logica rond projecten en taken. De service werkt met een
/// TakenContext en is bewust los van console-uitvoer, zodat we ze kunnen
/// testen tegen een database in het geheugen.
/// </summary>
public class TakenService(TakenContext context)
{
    public int VoegProjectToe(string naam)
    {
        Project project = new() { Naam = naam };
        context.Projecten.Add(project);
        context.SaveChanges();
        return project.Id;
    }

    public int VoegTaakToe(int projectId, string omschrijving)
    {
        bool projectBestaat = context.Projecten.Any(project => project.Id == projectId);
        if (!projectBestaat)
        {
            throw new InvalidOperationException($"Project {projectId} bestaat niet.");
        }

        Taak taak = new() { ProjectId = projectId, Omschrijving = omschrijving };
        context.Taken.Add(taak);
        context.SaveChanges();
        return taak.Id;
    }

    public void VoltooiTaak(int taakId)
    {
        Taak taak = context.Taken.Find(taakId)
            ?? throw new InvalidOperationException($"Taak {taakId} bestaat niet.");

        taak.Voltooid = true;
        context.SaveChanges();
    }

    // Enkel de openstaande taken van een project, oudste eerst.
    public List<Taak> OpenTaken(int projectId) =>
        context.Taken
            .Where(taak => taak.ProjectId == projectId && !taak.Voltooid)
            .OrderBy(taak => taak.Id)
            .ToList();

    public int AantalOpen(int projectId) =>
        context.Taken.Count(taak => taak.ProjectId == projectId && !taak.Voltooid);
}
