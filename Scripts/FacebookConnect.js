(function ($) {
    $.extend({
        facebookConnect: {
            initializeLogin: function (returnUrl, scope) {
                that = this;
                $("#facebook-connect-link").click(function (event) {
                    event.preventDefault();
                    that.login(returnUrl, scope);
                });
            },

            login: function (returnUrl, scope) {
                FB.login(function (response) {
                    if (response.authResponse) {
                        // user is logged in and granted some permissions.
                        if (returnUrl == null) {
                            window.location.reload();
                        }
                        else {
                            window.location.href = returnUrl;
                        }
                    } else {
                        //alert('User cancelled login or did not fully authorize.');
                    }
                }, { scope: scope });
            },

            initializeLogout: function (returnUrl) {
                that = this;
                $("#facebook-logout-link").click(function (event) {
                    event.preventDefault();
                    that.logout(returnUrl);
                });
            },

            logout: function (returnUrl) {
                FB.logout(function (response) {
                    if (returnUrl == null)
                        window.location.reload();
                    else
                        window.location.href = returnUrl;
                });
            }
        }
    });
})(jQuery);