var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var username = "";

document.getElementById("sendButton").disabled = true;

function appendMessage(message, textColor = "#000") {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.style.color = textColor;
    li.textContent = message;
}

connection.on("ReceiveMessage", function (message) { appendMessage(message) });

connection.on("ReceiveError", function (message) { appendMessage(message, "#FF0000") });

connection.on("BulkReceiveMessages", function (messages) {
    messages.forEach(message => {
        if (message) {
            appendMessage(message);
        }
    })
});

connection
    .start()
    .then(function () {
        document.getElementById("sendButton").disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

document.getElementById("commands-form").addEventListener("submit", function (event) {
    event.preventDefault();

    var message = document.getElementById("messageInput").value;

    var words = message.split(' ');

    if (!words || words.length < 2) {
        appendMessage("Unknown command, please type SEND then a message or room and then a room nunmber");
        return;
    }

    if (words[0].toLocaleLowerCase() == "room") {
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

    document.getElementById("messageInput").value = "";
});

document.getElementById("login-form").addEventListener("submit", function (event) {
    event.preventDefault();

    username = document.getElementById("username").value;

    connection
        .invoke("ActivateUser", username)
        .catch(function (err) {
            return console.error(err.toString());
        });

    document.getElementById("login-page").style.display = "none";
    document.getElementById("chat-page").style.display = "block";
    document.getElementById("messageInput").focus();
});

document.getElementById("username").focus();
