"use strict";

function buildConnection() {
    // Select WebSocket and SkipNegotiation for StickySession
    var connectedUsersConnection = new signalR.HubConnectionBuilder()
        .withUrl(`/ConnectedUsersHub${signalrpage.getQueryString()}`, {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .build();

    connectedUsersConnection.on("ReceiveConnectedUser", function (_, message) {
        //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var displayMessage = `New Connection: ${message.Name} (${message.Type})`;
        var li = document.createElement("li");
        li.textContent = displayMessage;
        document.getElementById("connectionsList").appendChild(li);
    });

    connectedUsersConnection.on("ReceiveDisconnectedUser", function (user, message) {
        //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var displayMessage = `Disconnected Connection: ${user} (${message})`;
        var li = document.createElement("li");
        li.textContent = displayMessage;
        document.getElementById("connectionsList").appendChild(li);
    });

    connectedUsersConnection
        .start()
        .then(function () {
            var userName = document.getElementById("userInput").value;
            console.log(`User(${userName}) successfully connected`);
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}
buildConnection();

// Attempt to open WebSocket and enable/disable "Connect" button
// document.getElementById("subscribeButton1").addEventListener("click", (event) => {
//     // signalrpage.disableSpaceInput1();
//     // buildConnection();
// });

// document.getElementById("subscribeButton2").addEventListener("click", (event) => {
//     signalrpage.disableSpaceInput2();
//     // buildConnection();
// });
