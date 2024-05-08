// ==UserScript==
// @name         Alt-Site Redirector
// @namespace    https://violentmonkey.github.io/
// @version      1.1
// @description  Redirect sites to their based counterparts
// @match        *://*/*
// @grant        GM_xmlhttpRequest
// @connect      localhost
// @run-at       document-start
// ==/UserScript==

(function() {
    'use strict';

    const serverUrl = 'https://redir.nloga.top';

    function getDomain(url) {
        const a = document.createElement('a');
        a.href = url;
        return a.hostname;
    }

    function checkAndRedirect() {
        const currentUrl = window.location.href;
        const currentDomain = getDomain(currentUrl);

        if (/google\.com\/search|webhp/.test(currentUrl) || currentDomain === getDomain(serverUrl)) {
            document.documentElement.style.display = '';
            return;
        }

        document.documentElement.style.display = 'none';
        const queryUrl = `${serverUrl}/${encodeURIComponent(currentUrl)}`;

        GM_xmlhttpRequest({
            method: 'GET',
            url: queryUrl,
            onload: function(response) {
                if (response.status === 200) {
                    try {
                        var finalUrl = response.finalUrl || (JSON.parse(response.responseText).redirectUrl);

                        if (finalUrl && getDomain(finalUrl) !== currentDomain) {
                            window.location.href = finalUrl;
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

    checkAndRedirect();
})();
