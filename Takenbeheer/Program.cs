using Microsoft.EntityFrameworkCore;
using Takenbeheer;

// De console-app gebruikt een SQLite-bestand. De context krijgt zijn opties
// via de constructor, precies zoals in de tests, maar dan met een andere
// provider-instelling.
DbContextOptions<TakenContext> opties = new DbContextOptionsBuilder<TakenContext>()
    .UseSqlite("Data Source=taken.db")
    .Options;

using TakenContext context = new(opties);
context.Database.EnsureCreated();

// Begin met een schone lei zodat de demo bij elke run hetzelfde toont.
context.Taken.RemoveRange(context.Taken);
context.Projecten.RemoveRange(context.Projecten);
context.SaveChanges();

TakenService service = new(context);

int project = service.VoegProjectToe("Cursus afwerken");
service.VoegTaakToe(project, "Hoofdstuk 12 schrijven");
int tweede = service.VoegTaakToe(project, "Oefeningen nakijken");
service.VoegTaakToe(project, "Feedback verwerken");

// Rond een taak af.
service.VoltooiTaak(tweede);

Console.WriteLine($"Openstaande taken ({service.AantalOpen(project)}):");
foreach (Taak taak in service.OpenTaken(project))
{
    Console.WriteLine($"  - {taak.Omschrijving}");
}
