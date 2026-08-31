let productTypes;

window.addEventListener("DOMContentLoaded", async () => {
    await loadProducts();
    await loadProductTypes();
    document.getElementById("addProductForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        await addProduct();
    });
    document.getElementById("editProductForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        await editProduct();
    });
})

async function addProduct() {
    const productNameValue = document.getElementById("addNameInput").value;
    const productPriceValue = parseFloat(document.getElementById("addPriceInput").value.replace(",", "."));
    const productTypeValue = document.getElementById("addTypeSelect").value;

    const newProduct = {
        ProductName: productNameValue,
        ProductPrice: productPriceValue,
        ProductType: productTypeValue
    }

    const respons = await fetch("/admin/add/product", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(newProduct)
    });

    if (respons.ok) {
        await loadProducts();
    }
}

async function onOpenEditModal(productId) {
    const editProductForm = document.getElementById("editProductForm");

    editProductForm.onsubmit = async (e) => {
        e.preventDefault();
        await editProduct(productId);
    };
}

async function editProduct(productId) {
    const productNameValue = document.getElementById("editNameInput").value;
    const productPriceValue = parseFloat(document.getElementById("editPriceInput").value.replace(",", "."));
    const productTypeValue = document.getElementById("editTypeSelect").value;

    const updatedProduct = {
        ProductID: parseInt(productId),
        ProductName: productNameValue,
        ProductPrice: productPriceValue,
        ProductType: productTypeValue
    }

    const respons = await fetch("/admin/edit/product", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(updatedProduct)
    });

    if (respons.ok) {
        await loadProducts();
    } 
}

async function loadProductTypes() {
    const respons = await fetch("/admin/get/product/types")
    const result = await respons.json();
    productTypes = result;

    const addTypeSelect = document.getElementById("addTypeSelect")
    const editTypeSelect = document.getElementById("editTypeSelect")

    addTypeSelect.innerHTML = `<option></option>`
    editTypeSelect.innerHTML = `<option></option>`

    productTypes.forEach(type => {
        addTypeSelect.innerHTML += `<option>${type}</option>`
        editTypeSelect.innerHTML += `<option>${type}</option>`
    });
}

async function loadProducts() {
    const respons = await fetch("/admin/load/products")
    const result = await respons.json();
    let userTableContentHTML = "";

    result.forEach(product => {
        let v = "test_test"
        v.replace("_", " ");
        userTableContentHTML += `
            <tr>
				<td>${product.naam}</td>
				<td>${product.prijs.toLocaleString("nl-Be", {
            style: "currency",
            currency: "EUR"
        })}</td>
                <td>${product.producttype.replace("_", " ")}</td>
                <td>
                    <div class="d-flex gap-3 justify-content-end">
		                <button class="d-flex btn btn-danger gap-2" onclick="deleteProduct(${product.productId})"><span>Verwijder</span><i class="bi bi-trash"></i></button>
		                <button class="d-flex btn btn-warning gap-2" onclick="onOpenEditModal(${product.productId})" data-bs-toggle="modal" data-bs-target="#editProductModal"><span>Wijzigen</span><i class="bi bi-gear"></i></button>
	                </div>
                </td>
            </tr>
        `
    });

    document.getElementById("userTableContent").innerHTML = userTableContentHTML;
}

async function deleteProduct(productId) {
    const confirmRemoval = confirm("Weet je zeker dat je dit product wilt verwijderen?");
    if (!confirmRemoval) {
        return;
    }

    const respons = await fetch(`/admin/delete/product/${productId}`, {
        method: "DELETE"
    })

    if (respons.ok) {
        loadProducts();
    }
}
