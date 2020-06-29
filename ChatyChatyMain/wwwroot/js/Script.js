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
    const MethodResponseList = ["TestResponse"];

    //connect and listen to the methods
    for (let i = 0; i < MethodResponseList.length; i++) {

        connection.on(MethodResponseList[i], function (message) {
            var li = document.createElement("li");
            li.textContent = "Response from " + MethodResponseList[i] + " : " + message;
            document.getElementById("messagesList").appendChild(li);
        });
    }
    //list of methods
    const MethodList = ["SendTest"];
    //list of method description
    const MethodDesciptionList = ["Send a test message which the server will echo back to caller"];

    //generate html for the list of methods 
    //get tamplate html
    const tamplateForm = document.getElementById("tamplateForm");
    const tamplateLi = document.getElementById("tamplateLi");

    for (let i = 0; i < MethodList.length; i++) {
        //processing the form
        //clone existing html to reuse it 
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

        //processing the list item
        let newLi = tamplateLi.cloneNode(true);
        newLi.removeAttribute("hidden");
        newLi.id = MethodList[i] + "Li";
        newLi.childNodes[1].setAttribute("href", "#" + newForm.id);
        newLi.childNodes[1].innerHTML = MethodList[i];
        document.getElementById("myTab").appendChild(newLi);
    }
}