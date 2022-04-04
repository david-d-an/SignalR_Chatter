"use strict";

var chatHubConnection = null;
function BuildChatHubConnection() {
    // Select WebSocket and SkipNegotiation for StickySession
    chatHubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`/chatHub${signalrpage.getQueryString()}`, {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withHubProtocol(
            new signalR.protocols.msgpack.MessagePackHubProtocol()
        )
        .build();

    chatHubConnection
    .start()
    .then(function () {
        // document.getElementById("sendButton1").disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

    chatHubConnection.on("ReceiveMessage", function (user, message, group) {
        var msg = message
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
        var encodedMsg = `${user} says ${msg}`;
        console.log({
            user: user,
            group: group,
            message: msg
        });

        if (group === $('#spaceInput1')[0].value)
            $('#messagesList1').append(encodedMsg + '\n');
        if (group === $('#spaceInput2')[0].value)
            $('#messagesList2').append(encodedMsg + '\n');
    });
}

function BuildChatGroup1() {
    // Attempt to add connection to WebSocket and enable "Send Message" button
    document.getElementById("subscribeButton1")
    .addEventListener("click", (event) => {
        // Subscribe space
        signalrpage.disableSpaceInput1();
        var space = document.getElementById("spaceInput1").value;
        chatHubConnection
            .invoke("SubscribeToSpace", space)
            .catch(function (err) {
                signalrpage.enableSpaceInput1();
                return console.error(err.toString());
            });
        event.preventDefault();
        console.log(`Subscribed to Space: ${space}`);
    });

    // Attempt to remove connection from WebSocket and disable "Send Message" button
    document.getElementById("unsubscribeButton1")
    .addEventListener("click", (event) => {
        // Subscribe space
        signalrpage.enableSpaceInput1();
        var space = document.getElementById("spaceInput1").value;
        chatHubConnection
            .invoke("UnsubscribeFromSpace", space)
            .catch(function (err) {
                signalrpage.disableSpaceInput1();
                return console.error(err.toString());
            });
        event.preventDefault();
        console.log(`Unsubscribed from Space: ${space}`);
    });
    
    // Build Message Send Action 1
    document.getElementById("sendButton1")
    .addEventListener("click", (event) => {
        console.log("Sending Message 1");
        // messagObject can carry more than one string.
        var messageObject = new class{}();

        messageObject.Space = document.getElementById("spaceInput1").value;
        messageObject.Message = document.getElementById("messageInput1").value;

        chatHubConnection
            .invoke("SendMessage", messageObject)
            .catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();
    });
};

function BuildChatGroup2() {
    // Attempt to add connection to WebSocket and enable "Send Message" button
    document.getElementById("subscribeButton2")
    .addEventListener("click", (event) => {
        // Subscribe space
        signalrpage.disableSpaceInput2();
        var space = document.getElementById("spaceInput2").value;
        chatHubConnection
            .invoke("SubscribeToSpace", space)
            .catch(function (err) {
                signalrpage.enableSpaceInput2();
                return console.error(err.toString());
            });
        event.preventDefault();
        console.log(`Subscribed to Space: ${space}`);
    });

    // Attempt to remove connection from WebSocket and disable "Send Message" button
    document.getElementById("unsubscribeButton2")
    .addEventListener("click", (event) => {
        // Subscribe space
        signalrpage.enableSpaceInput2();
        var space = document.getElementById("spaceInput2").value;
        chatHubConnection
            .invoke("UnsubscribeFromSpace", space)
            .catch(function (err) {
                signalrpage.disableSpaceInput2();
                return console.error(err.toString());
            });
        event.preventDefault();
        console.log(`Unsubscribed from Space: ${space}`);
    });
    
    // Build Message Send Action 2
    document.getElementById("sendButton2")
    .addEventListener("click", (event) => {
        console.log("Sending Message 2");
        // messagObject can carry more than one string.
        var messageObject = new class{}();
        messageObject.Space = document.getElementById("spaceInput2").value;
        messageObject.Message = document.getElementById("messageInput2").value;

        chatHubConnection
            .invoke("SendMessage", messageObject)
            .catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();
    });
}

BuildChatHubConnection();
BuildChatGroup1();
BuildChatGroup2();
