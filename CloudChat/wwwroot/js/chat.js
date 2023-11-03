var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var username = "";

document.getElementById("sendButton").disabled = true;

function appendMessage(message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = message;
}

connection.on("ReceiveMessage", appendMessage);

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;

    var words = message.split(' ');

    if (!words || words.length < 2) {
        appendMessage("Unknown command, please type SEND then a message or room and then a room nunmber");
        return;
    }

    if (words[0].toLocaleLowerCase() == "room") {
        console.log(words);

        connection
            .invoke("ChangeRoom", username, +words[1])
            .catch(function (err) {
                return console.error(err.toString());
            });
    } else if (words[0].toLocaleLowerCase() == "send") {
        message = words.slice(1).join(' ');

        connection
            .invoke("SendMessage", username, message)
            .catch(function (err) {
                return console.error(err.toString());
            });
    } else {
        appendMessage("Unknown command, please type SEND then a message or room and then a room nunmber");
    }

    event.preventDefault();
});
document.getElementById("loginButton").addEventListener("click", function (event) {
    username = document.getElementById("username").value;

    document.getElementById("login-page").style.display = "none";
    document.getElementById("chat-page").style.display = "block";


    event.preventDefault();
});