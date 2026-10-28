namespace Takenbeheer.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TakenServiceTests
{
    // Maakt een verse context op een SQLite-database in het geheugen. De
    // in-memory database leeft zolang de SqliteConnection open blijft; de
    // context bezit die connection niet en sluit ze dus niet bij Dispose. De
    // isolatie komt doordat elke test zijn eigen connection (en dus eigen
    // lege database) maakt, zonder bestand op schijf en zonder migrations.
    private static TakenContext MaakContext()
    {
        SqliteConnection verbinding = new("DataSource=:memory:");
        verbinding.Open();

        DbContextOptions<TakenContext> opties = new DbContextOptionsBuilder<TakenContext>()
            .UseSqlite(verbinding)
            .Options;

        TakenContext context = new(opties);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void VoegProjectToe_kent_een_id_toe()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);

        int id = service.VoegProjectToe("Website");

        Assert.True(id > 0);
        Assert.Equal(1, context.Projecten.Count());
    }

    [Fact]
    public void VoegTaakToe_koppelt_de_taak_aan_het_project()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);
        int project = service.VoegProjectToe("Website");

        int taak = service.VoegTaakToe(project, "Homepage ontwerpen");

        Taak? bewaard = context.Taken.Find(taak);
        Assert.NotNull(bewaard);
        Assert.Equal(project, bewaard!.ProjectId);
        Assert.False(bewaard.Voltooid);
    }

    [Fact]
    public void VoegTaakToe_op_onbestaand_project_werpt()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);

        Assert.Throws<InvalidOperationException>(
            () => service.VoegTaakToe(999, "Zweeftaak"));
    }

    [Fact]
    public void VoltooiTaak_haalt_de_taak_uit_de_openstaande_lijst()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);
        int project = service.VoegProjectToe("Website");
        service.VoegTaakToe(project, "Taak A");
        int taakB = service.VoegTaakToe(project, "Taak B");

        service.VoltooiTaak(taakB);

        List<Taak> open = service.OpenTaken(project);
        Assert.Single(open);
        Assert.Equal("Taak A", open[0].Omschrijving);
    }

    [Fact]
    public void AantalOpen_telt_enkel_de_openstaande_taken()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);
        int project = service.VoegProjectToe("Website");
        service.VoegTaakToe(project, "Taak A");
        int taakB = service.VoegTaakToe(project, "Taak B");
        service.VoegTaakToe(project, "Taak C");

        service.VoltooiTaak(taakB);

        Assert.Equal(2, service.AantalOpen(project));
    }

    [Fact]
    public void OpenTaken_telt_enkel_taken_van_het_gevraagde_project()
    {
        using TakenContext context = MaakContext();
        TakenService service = new(context);
        int project1 = service.VoegProjectToe("Website");
        int project2 = service.VoegProjectToe("Mobiele app");
        service.VoegTaakToe(project1, "Alleen voor website");
        service.VoegTaakToe(project2, "Alleen voor app");

        List<Taak> open1 = service.OpenTaken(project1);
        Assert.Single(open1);
        Assert.Equal("Alleen voor website", open1[0].Omschrijving);
    }
}
