window.ScrollBottomOfElement = (elemntName) => {
    let elment = document.getElementById(elemntName);
    elment.scrollTop = elment.scrollHeight;
    console.log("the scroll script was excuted");
};