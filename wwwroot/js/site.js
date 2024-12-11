const uriBooks = '/Books';
const uriUsers = '/Users';

// const uriBooks = 'http://localhost:5099/Books';
// const uriUsers = 'http://localhost:5099/Users';

let books = [];

//userName = document.getElementById('userName');
const checkToken = () => {
    fetch(uriBooks, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(response => response.json())
        .then(getItems())
        .catch(error => {
            sessionStorage.setItem("check", error)
            console.log(error);
            location.href = "./login.html"
        })
}

function getItems() {
    fetch(uriBooks, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(response => response.json())
        .then(data => _displayItems(data))
        // .catch(error => console.error('Unable to get items.', error));
        .catch(error => {
            console.log(error);
            location.href = "./login.html"
        });
}


function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const addPriceTextbox = document.getElementById('add-price');

    const item = {
        name: addNameTextbox.value.trim(),
        price: addPriceTextbox.value.trim()
    };

    fetch(uriBooks, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addPriceTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}


// function deleteItem(id) {
//     fetch(`${uriBooks}/${id}`, {
//         method: 'DELETE'
//     })
//         .then(() => getItems())
//         .catch(error => console.error('Unable to delete item.', error));
// }

function deleteItem(id) {
    fetch(`${uriBooks}/${id}`, {
        method: 'DELETE',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = books.find(item => item.id === id);

    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-price').value = item.price;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        name: document.getElementById('edit-name').value.trim(),
        price: document.getElementById('edit-price').value.trim()
    };

    fetch(`${uriBooks}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(item)
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'book on my website' : 'books on my website 📘📖';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('books');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(item.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodePrice = document.createTextNode(item.price);
        td2.appendChild(textNodePrice);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    books = data;
}

if (localStorage.getItem("token") == null) {
    console.log("login");
    sessionStorage.setItem("not", "not exist token")

    location.href = "./login.html"
}

function createLink() {
    if (localStorage.getItem("link") == "true") {
        let link = document.createElement("a");
        link.classList.add("book-link");
        link.href = "./userList.html";
        link.innerHTML = "users";
        
        console.log(sessionStorage.getItem("link"));
        document.body.appendChild(link);
    }
}

let hi = document.getElementById('hi');
let userName = localStorage.getItem("userName");
hi.innerText = `Hi ${userName}`;

getItems();
createLink();
