var webSearchPendingHighlight = null;

if (isPostMessageEnabled()) {
    addMessageListener(searchMessageHandler);
}

/**
* @param {Object} event
**/
function searchMessageHandler(event) {
    var message = getMessage(event.data);

    switch (message.messageType) {
        case "activated":
            $('input#txtSearch').focus();
            break;

        case "loaded":
            if (message.messageData != null
                && message.messageData.indexOf(webSearchPendingHighlight) != -1) {

                // If the content loaded is our pending search navigate, apply the search highlights
                highlightContentFrame();
            }
            webSearchPendingHighlight = null;
            break;

        case "quicksearch":
        case "searchHighlightComplete":

            // Pass to parent frame handler
            parent.postMessage(message.messageType + "|" + message.messageData, "*");
            break;

    }
}

$(function () {

    $('input#txtSearch').keyup(function (event) {
        if (event.keyCode == 13) {
            $('input#search').click();
        }
    });

    $('input#search').on('click', function () {
        btnSearch_onclick();
    });
});