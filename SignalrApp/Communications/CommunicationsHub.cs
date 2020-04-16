using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrApp.Communications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalrApp.Communications
{
    //user will always be part of an organization
    //so when the user logs in, we need to read the organization info from the user access token and add the user to that organization's group
    //when the user disconnects or change the organization, we need to remove the user from the current logged in organization and join him to the new organization
    //JoinOrganizationGroup and LeaveOrganizationGroup methods can be access through the jquery signalr client.
    //so when the user change organization->
    //1. call leave organization before user changes organization (so the claim still have the old organization name)
    //2. call join organization after user successfully changed the organization (so the claim will have the current organization info)
    public class CommunicationsHub : Hub
    {
        private const string _organizationPrefix = "Organization_";
        private const string _clientReceiveMessageMethodName = "receiveMessage";
        
        public override async Task OnConnected()
        {
            await JoinOrganizationGroup();
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await LeaveOrganizationGroup();
            await base.OnDisconnected(stopCalled);
        }

        public override async Task OnReconnected()
        {
            await JoinOrganizationGroup();
            await base.OnReconnected();
        }
                
        public async Task JoinOrganizationGroup()
        {
            var groupName = GetGroupName();
            await Groups.Add(Context.ConnectionId, groupName);
        }

        public async Task LeaveOrganizationGroup()
        {
            var groupName = GetGroupName();
            await Groups.Remove(Context.ConnectionId, groupName);
        }

        public void SendMessage(CommunicationMessage message)
        {
            IClientProxy proxy = null;
            switch (message.CommunicationType)
            {
                case RecipientType.Broadcast:
                    proxy = Clients.All;
                    break;
                case RecipientType.Organization:
                    proxy = Clients.Group(GetGroupName());
                    break;
                case RecipientType.User:
                    proxy = Clients.User(message.UserId);
                    break;
            }
            proxy?.Invoke(_clientReceiveMessageMethodName, message);
            if (message.IsPersist)
            {
                //use DI here
                var repository = new MessageRepository();
                repository.InsertMessage(message);
            }
        }

        private string GetGroupName() =>$"{_organizationPrefix}{CommunicationsIdProvider.GetOrganizationId(Context.User)}";// $"{_organizationPrefix}test_org_id"; 

    }
}