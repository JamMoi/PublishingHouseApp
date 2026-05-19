using Microsoft.EntityFrameworkCore;

namespace PublishingHouseApp.Models;

public class PaginatedList<T> : List<T>
{
    // Nykyinen sivu (esim. 1, 2, 3...)
    public int PageIndex { get; private set; }

    // Kuinka monta sivua yhteensä
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        // Jos sivu on virheellinen (esim. 0 tai -1), korjataan se ykköseksi
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        // Tallennetaan nykyinen sivu
        PageIndex = pageIndex;

        // Lasketaan sivujen kokonaismäärä
        // esim: 25 itemiä / 10 per sivu = 3 sivua
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        // Lisätään haetut itemit tähän listaan
        AddRange(items);
    }

    // Onko edellinen sivu olemassa
    public bool HasPreviousPage => PageIndex > 1;

    // Onko seuraava sivu olemassa
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize)
    {
        // Lasketaan montako riviä yhteensä tietokannassa
        var count = await source.CountAsync();

        // Haetaan vain kyseisen sivun data:
        // Skip = ohitetaan edelliset sivut
        // Take = otetaan vain pageSize määrä
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Palautetaan valmis paginoitu lista
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}