let modalChangeRoles;
let databaseRoles;

window.addEventListener("DOMContentLoaded", async () => {
    document.getElementById("addUserBtn").addEventListener("click", loadQrCode);
    const addUserModal = document.getElementById("addUserModal");
    addUserModal.addEventListener("hidden.bs.modal", () => {
        loadUsers();
    })

    await getRoles();
    await loadUsers();
})

async function getRoles() {
    const responseRoles = await fetch("/admin/get/roles")
    databaseRoles = await responseRoles.json();
}

async function loadQrCode() {
    const respons = await fetch("/admin/add/user")
    const result = await respons.json();

    document.getElementById("qrCodeLink").href = result.url;
    document.getElementById("qrCodeImage").src = result.imageSrc;
}

async function loadUsers() {
    const respons = await fetch("/admin/load/users")
    const result = await respons.json();
    let userTableContentHTML = "";

    result.forEach(user => {
        const nameDisplay = (user.naam === null) ? "/" : user.naam;
        const activatedDisplay = (user.geactiveerd == false) ? "Nee" : "Ja";
        let rolesBadgesHTML = ""
        let dropdownRolesHTML = "";

        databaseRoles.forEach(role => {
            dropdownRolesHTML += `<li><button class="btn btn-primary dropdown-item" onclick='addRoleUser(${user.id},"${role.naam}")'>${role.naam}</button></li>`
        })
        user.roles.forEach((role, index) => {
            rolesBadgesHTML += `<span class="badge text-bg-success">
                <div class="d-flex gap-2 align-items-center">
                    <span>${role.naam}</span>
                    <button class="button btn-close btn-outline-dark" onclick='deleteRoleUser(${user.id}, "${role.naam}")'></button>
                </div>
            </span>`
        });

        if (rolesBadgesHTML == "") {
            rolesBadgesHTML = "/";
        }

        userTableContentHTML += `
            <tr>
				<td>${nameDisplay}</td>
				<td>${activatedDisplay}</td>
                <td>
                    <div class="d-flex gap-1 flex-wrap justify-content-center">${rolesBadgesHTML}</div>
                </td>
                <td>
                    <div class="d-flex gap-3 justify-content-end">
                        <button class="d-flex btn btn-danger gap-2" onclick="removeUser(${user.id})"><span>Verwijder</span><i class="bi bi-trash"></i></button>
                        <div class="dropdown">
					        <button class="btn btn-warning dropdown-toggle" data-bs-toggle="dropdown">Rollen</button>
					        <ul class="dropdown-menu gap-3" id="dropdownRoles">
                            ${dropdownRolesHTML}
					        </ul>
				        </div>
                    </div>
                </td>
			</tr>
        `
    });

    document.getElementById("userTableContent").innerHTML = userTableContentHTML;
}

async function removeUser(userId) {

    if (!confirm("Weet je zeker dat je deze gebruiker wilt verwijderen?")) {
        return;
    }
    const respons = await fetch(`/admin/remove/user?id=${userId}`, {
        method: 'DELETE'
    })
    loadUsers();
}

async function addRoleUser(id, role) {
    const postBody = {
        UserId: id,
        NewRole: role
    };
    const respons = await fetch("/admin/add/user/role", {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(postBody)
    });
    loadUsers();
}

async function deleteRoleUser(id, role) {
    const respons = await fetch(`/admin/delete/user/role/?userId=${id}&roleName=${role}`, {
        method: 'DELETE'
    });
    loadUsers();
}
