// ==UserScript==
// @name         Alt-Site Redirector
// @namespace    https://violentmonkey.github.io/
// @version      1.0
// @description  Redirect sites to their based counterparts
// @match        *://*/*
// @grant        GM_xmlhttpRequest
// @connect      localhost
// @run-at       document-start
// ==/UserScript==

(function() {
    'use strict';

    const serverUrl = 'https://redir.nloga.top';

    function checkAndRedirect() {
        const currentUrl = decodeURIComponent(window.location.href);

        if (/google\.com\/search|webhp/.test(currentUrl) || currentUrl.startsWith(serverUrl)) {
            document.documentElement.style.display = '';
            return;
        }

        document.documentElement.style.display = 'none';

        const queryUrl = `${serverUrl}/${currentUrl}`;

        GM_xmlhttpRequest({
            method: 'GET',
            url: queryUrl,
            onload: function(response) {
                if (response.status === 200 && response.finalUrl && response.finalUrl !== currentUrl) {
                    window.location.href = response.finalUrl;
                } else if (response.status === 200) {
                    try {
                        var data = JSON.parse(response.responseText);
                        if (data.redirectUrl) {
                            window.location.href = data.redirectUrl;
                        } else {
                            document.documentElement.style.display = '';
                        }
                    } catch (e) {
                        console.error('Failed to parse the redirection data:', e);
                        document.documentElement.style.display = '';
                    }
                } else {
                    document.documentElement.style.display = '';
                }
            },
            onerror: function(error) {
                console.error('Error checking site redirection:', error);
                document.documentElement.style.display = '';
            }
        });
    }

    if (!window.location.href.startsWith(serverUrl)) {
        checkAndRedirect();
    }
})();


