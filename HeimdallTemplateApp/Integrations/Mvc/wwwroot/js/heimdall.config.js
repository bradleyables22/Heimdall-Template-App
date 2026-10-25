(function () {
    "use strict";

    if (!window.Heimdall)
        return;

    // Application-wide Heimdall defaults belong here. Keep page-specific
    // behavior visible in the rendered Heimdall attributes.

    // Heimdall.config.debug = true;
    // Heimdall.config.requestTimeoutMs = 10_000;
    // Heimdall.config.requestSync = "queue-latest";
    // Heimdall.config.clientInfo = true;
    // Heimdall.config.clientInfoMaxAgeMs = 60_000;
    // Heimdall.config.visibleRootMargin = "200px 0px";
    // Heimdall.config.visibleThreshold = 0.1;
    // Heimdall.config.ssePauseWhenHidden = true;

    // Only disable antiforgery when the server is configured the same way
    // and the application uses a security model that does not rely on
    // ambient cookies.
    // Heimdall.config.antiforgery = false;

    // Resolve short-lived credentials at request time when the application
    // uses bearer authentication. Filter by origin and request kind before
    // attaching application headers.
    // Heimdall.config.requestHeaders = async context => {
    //     if (new URL(context.url).origin !== window.location.origin)
    //         return {};
    //
    //     return {};
    // };

})();
