// ==UserScript==
// @name         Alt-Site Redirector
// @namespace    https://violentmonkey.github.io/
// @version      1.0
// @description  Redirect sites to their based counterparts
// @match        *://*/*
// @exclude      *://*.google.com/*
// @grant        GM_xmlhttpRequest
// @connect      localhost
// @run-at       document-start
// ==/UserScript==

(function() {
    'use strict';

    //hide while checking
    document.documentElement.style.display = 'none';

    const serverUrl = 'https://redir.nloga.top'; //link to instance

    function checkAndRedirect() {
        const currentUrl = window.location.href;

        //exclude google because it doesnt like it for some reason
        if (/google\.com\/search|webhp/.test(currentUrl)) {
            document.documentElement.style.display = '';
            return;
        }

        const queryUrl = `${serverUrl}/${encodeURIComponent(currentUrl)}`;

        GM_xmlhttpRequest({
            method: 'GET',
            url: queryUrl,
            onload: function(response) {
                if (response.status === 200 && response.finalUrl && response.finalUrl !== currentUrl) {
                    //redirect if link new
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
