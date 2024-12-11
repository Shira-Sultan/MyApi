const uri = '/login';
// const uri = 'http://localhost:5099/login';

const dom = {
    name: document.getElementById("name"),
    password: document.getElementById("password"),
    submitBtn: document.getElementById("submit")
}

dom.submitBtn.onclick = (event) => {
    event.preventDefault();

    console.log(1);

    if (!dom.name.value || !dom.password.value) {
        alert("Please fill in all fields.");
        return;
    }

    const user = { name: dom.name.value, password: dom.password.value }

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },

        body: JSON.stringify(user)
    })

        .then((response) => {
            if (!response.ok) {
                if (response.status === 401) {
                    alert("The username or password you entered is incorrect");
                }
                throw new Error('Server error: ' + response.status);
            }
            return response.json();
        })
        .then((res) => {
            console.log(3)
            if (res.status == 401)
                alert("The username or password you entered is incorrect")
            if (!res.token || !res.id)
                throw new Error('Invalid response from server');
            else {
                console.log("success");
                if (dom.name.value == "Shira" && dom.password.value == "shira2396"){
                    console.log("Admin insert");
                    localStorage.setItem("link", true);
                }
                else
                    localStorage.setItem("link", false);

                localStorage.setItem("token", res.token);
                localStorage.setItem('userId', res.id);
                localStorage.setItem('userName', dom.name.value);
                location.href = "../index.html";

                console.log(4);
            }
        })
        .catch(error => console.error('Unable to add item.', error));
    console.log(5);
}