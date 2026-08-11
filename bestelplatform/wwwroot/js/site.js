// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.addEventListener("DOMContentLoaded", () => {
    
})

function changeProductAmount(uniqueID, change) {
    const amountInput = document.getElementById(`amount-${uniqueID}`);
    let currentAmount = parseInt(amountInput.value)

    currentAmount += change;

    if (currentAmount < 0) {
        currentAmount = 0;
    }

    amountInput.value = currentAmount;
}
