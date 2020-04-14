$(function () {
    var theHub = $.connection.communicationsHub;

    theHub.client.receiveMessage = function (messageData) {
        console.log(messageData);
    };

    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            var dataToSend = {
                communicationType: 1,
                title: "test title",
                message: "test message",
                organizationId: "test_org_id",
                userId: "test_user_id",
                isPersist: false
            };
            theHub.server.sendMessage(dataToSend);
        });
    });

});