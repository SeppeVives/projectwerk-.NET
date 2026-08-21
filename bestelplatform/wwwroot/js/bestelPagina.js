// Functie om bij het bestellen de integer in de input te verhogen.
function changeProductAmount(uniqueID, change) {
    const amountInput = document.getElementById(`amount-${uniqueID}`);
    let currentAmount = parseInt(amountInput.value)

    currentAmount += change;

    if (currentAmount < 0) {
        currentAmount = 0;
    }

    amountInput.value = currentAmount;
}