$(function () {

    var theHub = $.connection.communicationsHub;

    $.connection.hub.start().done(function () {
        initActtions();
    });

    theHub.client.receiveMessage = function (received) {
        $("#messageTitle").text(received.Title);
        $("#messageContent").text(received.Message);
    };

    function userToUser() {
        var message = {
            communicationType: 2,
            title: "user to user title",
            message: "user to user message",
            userId: "1"
        };
        theHub.server.sendMessage(message);
    }

    function toAllUsersInOrganization() {
        var message = {
            communicationType: 1,
            title: "to all users in organization title",
            message: "to all users in organization message"
        };
        theHub.server.sendMessage(message);
    }

    function toAllUsersInAllOrganizations() {
        var message = {
            communicationType: 0,
            title: "to all users in all organizations title",
            message: "to all users in all organizations message"
        };
        theHub.server.sendMessage(message);
    }

    function withSave() {
        var message = {
            communicationType: 2,
            title: "user to user with save title",
            message: "user to user with save message",
            userId: "2",
            isPersist: 1
        };
        theHub.server.sendMessage(message);
    }

    function withoutSave() {
        var message = {
            communicationType: 2,
            title: "user to user without save title",
            message: "user to user without save message",
            userId: "2",
            isPersist: 0
        };
        theHub.server.sendMessage(message);
    }

    //1. call leave organization before user changes organization (so the claim still have the old organization name)
    //2. call join organization after user successfully changed the organization (so the claim will have the current organization info)
    function changeOrganization() {
        //first disconnect from the current organization group
        theHub.server.LeaveOrganizationGroup().then(function () {
            //perform organization change operation and chain the join organization call afterwards once is resolved
            //i am creating a dummy promise here to mimic the behavior
            Promise.resolve('Organization_Changed').then(function () {
                theHub.server.JoinOrganizationGroup();
            });
        });
    }

    function initActtions() {
        $('#userToUser').click(function () {
            userToUser();
        });
        $('#toAllUsersInOrganization').click(function () {
            toAllUsersInOrganization();
        });
        $('#toAllUsersInAllOrganizations').click(function () {
            toAllUsersInAllOrganizations();
        });
        $('#withSave').click(function () {
            withSave();
        });
        $('#withoutSave').click(function () {
            withoutSave();
        });
    }

});