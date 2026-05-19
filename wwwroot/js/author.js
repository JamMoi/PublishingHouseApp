// Odottaa, että HTML-dokumentti on ladattu kokonaan ennen koodin suorittamista
document.addEventListener("DOMContentLoaded", function () {
    // Hakee hakukentän HTML-sivulta id:n "authorSearch" perusteella
    const authorSearchInput = document.getElementById("authorSearch");
    // Hakee HTML-elementin, johon hakutulostaulukko päivitetään
    const authorsTableContainer = document.getElementById("authorsTableContainer");   

    if (!authorSearchInput) return;     // Lopettaa koodin suorittamisen, jos hakukenttää ei löydy sivulta

    // Luo muuttujan viiveajastimelle
    let debounceTimer;

    // Kuuntelee hakukentän sisältöön tehtäviä muutoksia
    authorSearchInput.addEventListener("input", function () {
        clearTimeout(debounceTimer);        // Peruuttaa aiemmin käynnistetyn ajastimen, jos käyttäjä jatkaa kirjoittamista

        // Käynnistää uuden ajastimen, joka suorittaa haun 300 millisekunnin viiveellä
        debounceTimer = setTimeout(async () => {
            const query = authorSearchInput.value; // Tallentaa hakukentän tämänhetkisen arvon query-muuttujaan

            // Lähettää palvelimelle hakupyynnön ja liittää hakusanan URL-osoitteeseen turvallisesti
            const response = await fetch(`/Authors/LiveSearch?query=${encodeURIComponent(query)}`);

            // Muuntaa palvelimen palauttaman vastauksen tekstimuotoiseksi HTML-sisällöksi
            const html = await response.text();

            // Päivittää sivulla olevan taulukkoalueen uudella HTML-sisällöllä
            authorsTableContainer.innerHTML = html;

            // Määrittää 300 millisekunnin viiveen ennen haun suorittamista
        }, 300);
    });
});