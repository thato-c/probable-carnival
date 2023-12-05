// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function togglePassword(element) {
    var passwordInput = $(element).closest('.input-group').find('input');
    var strengthMessage = $('#passwordStrengthMessage');

    if (passwordInput.attr('type') === 'password') {
        passwordInput.attr('type', 'text');
    } else {
        passwordInput.attr('type', 'password');
    }
}

$(document).ready(function () {
    var strengthMessage = $('#passwordStrengthMessage');

    $('input[type="password"]').on('input', function () {
        var password = $(this).val();
        var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

        if (regex.test(password)) {
            strengthMessage.text('Password strength: Strong');
            strengthMessage.removeClass('text-danger').addClass('text-success');
        } else {
            strengthMessage.text('Password must meet the recommended strength.');
            strengthMessage.removeClass('text-success').addClass('text-danger');
        }
    });
});