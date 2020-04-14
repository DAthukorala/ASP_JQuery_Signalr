using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrApp.Communications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalrApp.Communications
{
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

        //to change organization, call LeaveOrganizationGroup before changing organization and JoinOrganizationGroup after change
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

        private string GetGroupName() => $"{_organizationPrefix}test_org_id"; //$"{_organizationPrefix}{CommunicationsIdProvider.GetOrganizationId(Context.User)}";

    }
}