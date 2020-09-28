"use strict";
//the method get called after the user input the token
function Start() {
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
    const MethodResponseList = ["TestResponse", "UpdateMessagesResponses", "RegisterSessionErrorResponse", "SendMessageErrorResponse"];

    //connect and listen to the methods
    for (let i = 0; i < MethodResponseList.length; i++) {
        connection.on(MethodResponseList[i], function (message) {
            var li = document.createElement("li");
            li.textContent = "Response from " + MethodResponseList[i] + " : " + message;
            document.getElementById("messagesList").appendChild(li);
        });

        document.getElementById("listeningMethodList").innerHTML += MethodResponseList[i] + " ";
    }
    //list of methods
    const MethodList = ["SendTest", "RegisterSession","SendMessage"];
    //list of method description
    const MethodDesciptionList = [
        `Send a test string which the server will echo back to caller`,
        `Register client into connected devices, takes the last message Id as json\n
         sample requst:\n
         1`,
        `Send a message to user with the chatId everything is same with normal API with one difference,\n 
         you don't get a response unless an error happens (like json format error or invalid resource Id),\n
         then a json response (with type ResponseBase) is sent to the corresponding response methods\n
         sample requst:\n
         {"chatId": 0, "body": "string"}`
    ];

    //generate html for the list of method names
    //get tamplate html
    if (document.getElementById(MethodList[0] + "Form")==null) {

        const tamplateForm = document.getElementById("tamplateForm");
        //loop for each method
        for (let i = 0; i < MethodList.length; i++) {
            //cloning tamplate to create from for each action
            let newForm = tamplateForm.cloneNode(true);
            let title = newForm.childNodes[1];
            let lable = newForm.childNodes[3];
            let input = newForm.childNodes[5];
            let button = newForm.childNodes[7];

            //set id
            newForm.id = MethodList[i] + "Form";
            input.id = MethodList[i] + "Input";
            //make visible 
            newForm.removeAttribute("hidden");
            //set text
            title.innerHTML = MethodList[i];
            lable.innerHTML = MethodDesciptionList[i];

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
}