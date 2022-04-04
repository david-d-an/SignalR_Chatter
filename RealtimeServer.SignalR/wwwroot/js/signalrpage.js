document.getElementById("subscribeButton1").disabled = false;
document.getElementById("sendButton1").disabled = true;
document.getElementById("unsubscribeButton1").disabled = true;

document.getElementById("subscribeButton2").disabled = false;
document.getElementById("sendButton2").disabled = true;
document.getElementById("unsubscribeButton2").disabled = true;

var signalrpage = {
    makeid: (length) => {
        var result = '';
        var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        var charactersLength = characters.length;
        for (var i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }
        return result;
    },

    getQueryString: () => {
        var userName = document.getElementById("userInput").value;
        // var space = document.getElementById("spaceInput").value;

        // console.log(`?username=${userName}&space=${space}`);
        return `?username=${userName}`;
    },

    enableSpaceInput1: () => {
        document.getElementById("spaceInput1").disabled = false;
        document.getElementById("subscribeButton1").disabled = false;
        document.getElementById("unsubscribeButton1").disabled = true;
        document.getElementById("sendButton1").disabled = true;
    },
    disableSpaceInput1: () => {
        document.getElementById("spaceInput1").disabled = true;
        document.getElementById("subscribeButton1").disabled = true;
        document.getElementById("unsubscribeButton1").disabled = false;
        document.getElementById("sendButton1").disabled = false;
    },
    enableSpaceInput2: () => {
        document.getElementById("spaceInput2").disabled = false;
        document.getElementById("subscribeButton2").disabled = false;
        document.getElementById("unsubscribeButton2").disabled = true;
        document.getElementById("sendButton2").disabled = true;
    },
    disableSpaceInput2: () => {
        document.getElementById("spaceInput2").disabled = true;
        document.getElementById("subscribeButton2").disabled = true;
        document.getElementById("unsubscribeButton2").disabled = false;
        document.getElementById("sendButton2").disabled = false;
    }
};

document.getElementById("userInput").value = signalrpage.makeid(5);


