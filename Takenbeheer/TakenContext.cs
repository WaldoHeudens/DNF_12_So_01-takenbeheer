namespace Takenbeheer;

using Microsoft.EntityFrameworkCore;

// De context krijgt zijn opties via de constructor. Zo kiest Program.cs een
// SQLite-bestand, terwijl de tests een SQLite-database in het geheugen
// gebruiken. De context zelf hoeft daar niets van te weten.
public class TakenContext(DbContextOptions<TakenContext> options) : DbContext(options)
{
    public DbSet<Project> Projecten => Set<Project>();
    public DbSet<Taak> Taken => Set<Taak>();
}
