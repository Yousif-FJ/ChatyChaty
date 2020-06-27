"use strict";
function Authenticate() {
    //close existing connection
    if (connection != null) {
        connection.stop();
    }
    //define connection
    var connection = new signalR.HubConnectionBuilder().withUrl("/v1/chatHub",
        { accessTokenFactory: () => document.getElementById("Token").value })
        .build();

    //start connection, update container view and catch errors
    connection.start().then(function () {
        document.getElementById("main-controller-container").hidden = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    //list of response methods to listen to 
    let MethodList = ["TestResponse"];

    //connect and listen to the methods
    for (let i = 0; i < MethodList.length; i++) {

    connection.on(MethodList[i], function (message) {
        var li = document.createElement("li");
        li.textContent = "Response from "+ MethodList[i]+" : "+message;
        document.getElementById("messagesList").appendChild(li);
    });
    }

    //call method
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendTest", message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}