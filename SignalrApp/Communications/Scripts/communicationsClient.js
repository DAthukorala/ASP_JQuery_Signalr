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
                organizationId: "test org id",
                userId: "test user id",
                isPersist: false
            };
            theHub.server.sendMessage(dataToSend);
        });
    });

});