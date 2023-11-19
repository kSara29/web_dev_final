$(document).ready(function () {
    $("#changePasswordBtn").click(function () {
        $("#changePasswordForm").css("display", "block")
        $("#changePassword").val($("#changePasswordForm").is(":visible"));
        $("#changePasswordBtn").css("display", "none");
    });

    const el = document.querySelector("#SendChanges");
    let userid = el.dataset.userid;
    
    $("#submitPasswordChange").click(function () {
        var formData = new FormData($('form')[0]);

        $.ajax({
            url: "/User/Edit",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                window.location.href = "/User/Profile?userId="+userid
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});