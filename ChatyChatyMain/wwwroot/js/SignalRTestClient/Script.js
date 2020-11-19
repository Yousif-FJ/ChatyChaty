"use strict";
//the method get called after the user input the token and click connect and authenticate
function Start() {
    document.getElementById("authenticate-and-connect-button").disabled = true; 

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
    const MethodResponseList = ["UpdateMessagesResponses"];

    //connect and listen to the methods
    for (let i = 0; i < MethodResponseList.length; i++) {
        connection.on(MethodResponseList[i], function (message) {
            var li = document.createElement("li");
            li.textContent = "Response from " + MethodResponseList[i] + " : " + JSON.stringify(message);
            document.getElementById("messagesList").appendChild(li);
        });

        document.getElementById("listeningMethodList").innerHTML += MethodResponseList[i] + " ";
    }
}