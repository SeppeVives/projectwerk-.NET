let setIntervalID;

window.addEventListener("DOMContentLoaded", () => {
    setIntervalID = setInterval(updateStatus, 1000)
})

// Functie om live de status van een bestelling voor een gebruiker weer te geven.
function updateStatus() {
    fetch("/api/visitor/orders/statuses").then(respons => {
        if (respons.status == 404) {
            throw new Error("Geen bestellingstatus gevonden voor huidige bezoeker.")
        }
        return respons.json();
    }).then(statuses => {
        const statusSpans = document.querySelectorAll(".status-span");
        statusSpans.forEach((element, index) => {
            element.innerText = statuses[index];
        })
        const statusCards = document.querySelectorAll(".card");
        statusCards.forEach((element, index) => {
            changeStatusColor(element, statuses[index]);
        })
    }).catch(error => {
        console.error(error)
        clearInterval(setIntervalID);
    });
}

// Functie om het uiterlijk van de statuskaart te veranderen afhankelijk van de status.
function changeStatusColor(element, statusText) {
    // Verwijder eerst oude text-kleuren
    element.classList.remove('border-success', 'border-warning', 'border-danger', 'border');
    // Pas dit aan naar de exacte namen van jouw statussen (hoofdlettergevoelig!)
    if (statusText === "afgeleverd") {
        element.classList.add('border-success'); // Groen
    }
    else if (statusText === "klaar") {
        element.classList.add('border-warning'); // Oranje
    }
    else if (statusText === "besteld") {
        element.classList.add('border-danger'); // Rood
    }
}
