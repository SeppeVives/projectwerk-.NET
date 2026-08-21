// Functie om te controleren of een gebruiker minstens 1 product heeft besteld vooraleer de form dit verzendt.
function validateOrder() {
    const amountInputs = document.querySelectorAll('input[name$=".Amount"]');
    let productOrdered = false;
    for (const amountInput of amountInputs) {
        if (parseInt(amountInput.value) > 0) {
            productOrdered = true;
            break;
        }
    }

    if (!productOrdered) {
        const bericht = "Voeg minstens één product toe aan je bestelling.";
        const alertContainer = document.getElementById("alert-container");
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger fade show';
        alertDiv.role = 'alert';
        alertDiv.innerHTML = `
        <span>${bericht}</span>`;

        alertContainer.appendChild(alertDiv);
        window.scrollTo({ top: 0, behavior: 'smooth' });

        const bsAlert = new bootstrap.Alert(alertDiv);
        setTimeout(() => {
            bsAlert.close();
        }, 1000);

        return false;
    }
    else
        return true;
}