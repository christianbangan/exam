function myFunction(id) {
    var x = document.getElementById("changeButton" + id);
    console.log(x);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

