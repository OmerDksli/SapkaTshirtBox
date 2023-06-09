// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
alert("ajax çalıştı");
function userToList() {
    alert("ajax çalıştı");
}
//function AFail(jqXHR, textStatus, errorThrown) {
//    alert(textStatus);
//    alert(errorThrown);
//}
function filtering(gelen) {
    alert(gelen);
    var userId = document.getElementById("user").value;

    $.post("@Url.Action("filterUser","Users")/" + userId, userToList, AFail "json");
}