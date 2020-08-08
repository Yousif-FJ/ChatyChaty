"use strict";
//the method get called after the user input the token
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
    const MethodResponseList = ["TestResponse", "UpdateMessagesResponses", "InvalidJsonResponse"];

    //connect and listen to the methods
    for (let i = 0; i < MethodResponseList.length; i++) {

        connection.on(MethodResponseList[i], function (message) {
            var li = document.createElement("li");
            li.textContent = "Response from " + MethodResponseList[i] + " : " + message;
            document.getElementById("messagesList").appendChild(li);
        });
    }
    //list of methods
    const MethodList = ["SendTest", "RegisterSession"];
    //list of method description
    const MethodDesciptionList = ["Send a test message which the server will echo back to caller",
        "register client into connected devices, takes the last message Id as json"];

    //generate html for the list of method names
    //get tamplate html
    const tamplateForm = document.getElementById("tamplateForm");

    //cloning tamplate to create from for each action
    for (let i = 0; i < MethodList.length; i++) {
        //processing the form
        let newForm = tamplateForm.cloneNode(true);
        newForm.id = MethodList[i] + "Form";
        newForm.removeAttribute("hidden");
        let lable = newForm.childNodes[1];
        lable.innerHTML = MethodDesciptionList[i];
        let input = newForm.childNodes[3];
        input.id = MethodList[i] + "Input";
        let button = newForm.childNodes[5];

        //add event listener to the button
        button.addEventListener("click", function (event) {
            var message = input.value;
            connection.invoke(MethodList[i], message).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });
        document.getElementById("myTabContent").appendChild(newForm);

    }
}