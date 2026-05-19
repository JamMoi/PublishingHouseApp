// Odottaa, että HTML-dokumentti on ladattu kokonaan ennen koodin suorittamista
document.addEventListener("DOMContentLoaded", function () {
    // Hakee kirjahakukentän HTML-sivulta id:n "booksSearch" perusteella
    const bookSearchInput = document.getElementById("booksSearch");

    // Hakee hakupainikkeen HTML-sivulta id:n "bookSearchButton" perusteella
    const bookSearchButton = document.getElementById("bookSearchButton");

    // Hakee HTML-elementin, johon kirjahakutulokset päivitetään
    const booksContainer = document.getElementById("booksContainer");

    // Lopettaa koodin suorittamisen, jos hakukenttää TAI tulosaluetta ei löydy sivulta
    if (!bookSearchInput || !booksContainer) return; 

    // Luo muuttujan viiveajastimelle
    let debounceTimer;

    // Määrittää asynkronisen funktion kirjojen hakemista varten
    async function searchBooks() {

        // Tallentaa hakukentän tämänhetkisen arvon query-muuttujaan
        const query = bookSearchInput.value;

        // Lähettää palvelimelle hakupyynnön ja liittää hakusanan URL-osoitteeseen turvallisesti
        const response = await fetch(`/Books/LiveSearch?query=${encodeURIComponent(query)}`);

        // Muuntaa palvelimen palauttaman vastauksen tekstimuotoiseksi HTML-sisällöksi
        const html = await response.text();

        // Päivittää sivulla olevan kirja-alueen uudella HTML-sisällöllä
        booksContainer.innerHTML = html;
    }
    // Kuuntelee näppäimistön painalluksia hakukentässä
        bookSearchInput.addEventListener("keyup", function () {

        // Peruuttaa aiemmin käynnistetyn ajastimen, jos käyttäjä jatkaa kirjoittamista
            clearTimeout(debounceTimer);

        // Käynnistää uuden ajastimen, joka suorittaa haun 300 millisekunnin viiveellä
        debounceTimer = setTimeout(searchBooks, 300);
    });

    if (bookSearchButton) {    // Tarkistaa, löytyykö sivulta hakupainike

        // Kuuntelee hakupainikkeen klikkausta ja suorittaa kirjahakutoiminnon
        bookSearchButton.addEventListener("click", searchBooks);
    }
});