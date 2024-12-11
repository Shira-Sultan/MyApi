const uri = "/Users";

// const uri = "http://localhost:5099/Users";

let users = [];

function checkToken() {
    fetch(uri ,{
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization':`Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(response => response.json())
        .then(getUsers())
        .catch(error => 
        {
            sessionStorage.setItem("check",error)
            console.log(error);
            location.href = "./login.html";
        });     
}

function getUsers() {
    fetch(uri ,{
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization':`Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(response => response.json())
        .then(data => _displayUsers(data))
        .catch(error => console.error('Unable to get items.', error));   
}

function addUsers() {
    const addNameTextbox = document.getElementById('add-name');
    const addPasswordTextbox = document.getElementById('add-password');

    const item = { 
        name: addNameTextbox.value.trim(),
        password:addPasswordTextbox.value.trim()
    };

    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization':`Bearer ${localStorage.getItem("token")}`
            },
            body: JSON.stringify(item)
        })
        .then(response => response.json())
        .then(() => {
            getUsers();
            addNameTextbox.value = '';
            addPasswordTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteUsers(id) {
    fetch(`${uri}/${id}`,{
        method: 'DELETE',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization':`Bearer ${localStorage.getItem("token")}`
        },
    })
        .then(() => getUsers())
        .catch(error => console.error('Unable to delete item.', error));
}


function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'user' : 'users :)';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayUsers(data) {
    const tBody = document.getElementById('users');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {    
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteUsers(${item.id})`);

        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        let textNodeName = document.createTextNode(item.name);
        td1.appendChild(textNodeName);  

        let td2 = tr.insertCell(1);
        let textNodePassword = document.createTextNode(item.password);
        td2.appendChild(textNodePassword);

        let td4 = tr.insertCell(2);
        td4.appendChild(deleteButton);
    });
    users = data;
}

getUsers();