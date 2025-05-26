
function FormatDate(d) {
    var date = new Date(d);
   
    var day = date.getDate() > 9 ? date.getDate() : '0' + date.getDate();
    var mount = date.getMonth() + 1;
    var mount = mount > 9 ? mount : '0' + mount;
    var year = date.getFullYear();
    var hour = date.getHours() > 9 ? date.getHours() : '0' + date.getHours();
    var minute = date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes();
    var second = date.getSeconds() > 9 ? date.getSeconds() : '0' + date.getSeconds();

    var fd = day + "." + mount + "." + year + " " + hour + ":" + minute + ":" + second;
    return fd
}

var token = localStorage.getItem("token")
var userRoles = [];
if (token == null) {

    $(".NotLogined").show();
    $(".Logined").hide();

} else {
    $(".NotLogined").hide();
    $(".Logined").show();
    var payload = parseJwt(token);
    var username = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
    userRoles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    var userPhoto = payload["UserPhoto"];
    $("#ImgUserPhoto").attr("src", "https://localhost:7218/Files/UserPhotos/" + userPhoto);
    $("#UserName").html(username);
    
    var userId = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    var roleName = "";
    if (userRoles.includes("Admin")) {
        roleName = "Sistem Yönetici";
    } else {
        roleName="Normal Üye"
    }
    $("#divRole").html(roleName);

   //console.log(userRoles);
}



function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);

}
$("#Logout").click(function () {
    localStorage.removeItem("token");
    location.href = "/Login";
});


