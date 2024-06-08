/*
function flipString(str) {
    return str.split('').reverse().join('');
}


function getFormattedDateTime() {
    var now = new Date();

    var day = now.getDate();
    var month = now.getMonth() + 1; // Months are zero-based
    var year = now.getFullYear();

    var hours = now.getHours();
    var minutes = now.getMinutes();
    var seconds = now.getSeconds();

    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // The hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;

    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;

    var formattedDate = day + '/' + month + '/' + year;
    var formattedTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;

    return formattedDate + ' ' + formattedTime;
}

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();

connection.on("ReceiveMessage", function (user, message) {
    var now = new Date();
    var currentTime = now.toLocaleTimeString()

    var currUser = document.getElementById("chatContent(" + user + ")")
    if (currUser) {
        var currUserContent = document.createElement("div");
        currUserContent.innerHTML = `
                        <div class="justify-content-end d-flex w-auto">
                            <div class="mt-1 bg-green border-2 p-2">
                                <p>${message}</p>
                                    <p><small>${getFormattedDateTime()}</small></p>
                            </div>
                        </div>
                    `
        currUser.appendChild(currUserContent);
    }
    var otherUser = document.getElementById("chatContent(" + flipString(user) + ")");
    if (otherUser) {
        var otherUserContent = document.createElement("div");
        otherUserContent.innerHTML = `
                        <div class="justify-content-start d-flex w-auto">
                            <div class="mt-1 bg-light border-2 p-2">
                                <p>${message}</p>
                                <p><small>${getFormattedDateTime()}</small></p>
                            </div>
                        </div>
                    `
        otherUser.appendChild(otherUserContent);
    }
});

/*
document.getElementById("sendButton").disabled = true;
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});*/

/*
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});*/

function showChat(currStudId, otherStudId) {
    const allChats = document.querySelectorAll('div[id^="chat("]');
    allChats.forEach(function (div) {
        div.classList.add("hidden"); // Hide all chat divs initially
        div.classList.remove("bg-green");
    });
    var chatToShow = document.getElementById("chat(" + currStudId + "," + otherStudId + ")");
    if (chatToShow) {
        chatToShow.classList.remove("hidden"); // Show the chat div corresponding to the selected student ID
        chatToShow.classList.add("bg-green");
    }
}
function sendMessage(currStudId, otherStudId) {
    var user = currStudId + "," + otherStudId;
    var message = document.getElementById("inputMsg(" + otherStudId + ")").value;
    var currentTime = getFormattedDateTime();

    alert(user + message + currentTime);
    $.ajax({
        type: 'POST',
        url: '@Url.Action("Test", "Chat")',
        dataType: 'json',
        data: {

            currStudId: currStudId,
            otherStudId: otherStudId,
            message: message,
            currentTime: currentTime
        },
        success: function (response) {
            alert('success!');
            message.value = "";
            connection.invoke("SendMessage", user, message).catch(function (err) {
                return console.error(err.toString());
            });

        },
        error: function (xhr, status, error) {
            alert('dont leave blank');
        }
    });
    //event.preventDefault();
}


function test() {
    $.ajax({
        type: 'POST',
        url: '@Url.Action("Test", "Chat")',
        dataType: 'json',
        success: function (response) {
            alert('success!');
        },
        error: function (xhr, status, error) {
            alert('dont leave blank');
        }
    });
}
