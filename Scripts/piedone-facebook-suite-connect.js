(function ($) {
    $.extend({
        facebookConnect: {
            initializeLogin: function (returnUrl, scope, saveSessionUrl, requestVerificationToken) {
                that = this;
                $("#facebook-connect-link").click(function (event) {
                    event.preventDefault();
                    that.login(returnUrl, scope, saveSessionUrl, requestVerificationToken);
                });
            },

            login: function (returnUrl, scope, saveSessionUrl, requestVerificationToken) {
                FB.login(function (response) {
                    if (response.status === 'connected') {
                        // The user is logged in and has authenticated the app

                        $.ajax({
                            type: "POST",
                            url: saveSessionUrl,
                            async: false,
                            data: {
                                    __RequestVerificationToken: requestVerificationToken,
                                    userId: response.authResponse.userID,
                                    accessToken: response.authResponse.accessToken,
                                    expiresIn: response.authResponse.expiresIn
                                  }
                        }).done(function (response) {
                            // Session saved
                        });

                        if (returnUrl == null) {
                            window.location.reload();
                        }
                        else {
                            window.location.href = decodeURIComponent((returnUrl + '').replace(/\+/g, '%20'));
                        }
                    } else if (response.status === 'not_authorized') {
                        // The user is logged in to Facebook, but has not authenticated the app
                    } else {
                        // The user isn't logged in to Facebook.
                    }
                }, { scope: scope });
            },

            initializeLogout: function (returnUrl, destroySessionUrl, requestVerificationToken) {
                that = this;
                $("#facebook-logout-link").click(function (event) {
                    event.preventDefault();
                    that.logout(returnUrl, destroySessionUrl, requestVerificationToken);
                });
            },

            logout: function (returnUrl, destroySessionUrl, requestVerificationToken) {
                FB.logout(function (response) {
                    $.ajax({
                        type: "POST",
                        url: destroySessionUrl,
                        async: false,
                        data: {
                            __RequestVerificationToken: requestVerificationToken
                        }
                    }).done(function (response) {
                        // Session destroyed
                    });
                    
                    if (returnUrl == null)
                        window.location.reload();
                    else
                        window.location.href = decodeURIComponent((returnUrl + '').replace(/\+/g, '%20'));
                });
            }
        }
    });
})(jQuery);