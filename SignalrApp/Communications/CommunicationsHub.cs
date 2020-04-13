using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrApp.Communications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalrApp.Communications
{
    public class CommunicationsHub : Hub
    {
        private Dictionary<string, ConnectionDetails> _connections { get; set; } = new Dictionary<string, ConnectionDetails>();
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
            var connectionInfo = HandleExistingConnection();
            var groupName = GetGroupName(connectionInfo.OrganizationId);
            await Groups.Add(connectionInfo.ConnectionId, groupName);
        }

        public async Task LeaveOrganizationGroup()
        {
            var connectionInfo = GetConnectionInfo(Context.ConnectionId);
            if (connectionInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(connectionInfo.OrganizationId))
                {
                    var groupName = GetGroupName(connectionInfo.OrganizationId);
                    await Groups.Remove(connectionInfo.ConnectionId, groupName);
                }
            }
            _connections.Remove(Context.ConnectionId);
        }

        public async Task SendMessage(CommunicationMessage communication)
        {
            var connectionInfo = GetConnectionInfo(Context.ConnectionId);
            if (connectionInfo != null)
            {
                IClientProxy proxy = null;
                switch (communication.CommunicationType)
                {
                    case RecipientType.Broadcast:
                        proxy = Clients.All;
                        break;
                    case RecipientType.Organization:
                        proxy = Clients.Group(GetGroupName(communication.OrganizationId));
                        break;
                    case RecipientType.User:
                        proxy = Clients.User(communication.UserId);
                        break;
                }
                await proxy?.Invoke(_clientReceiveMessageMethodName, communication);
            }
        }

        //following methods can be added to a separate class.
        //but just added them here to avoid any mismatches in DIs in the existing system

        private ConnectionDetails HandleExistingConnection()
        {
            var userId = CommunicationsIdProvider.GetUserId(Context.User);
            var organizationId = CommunicationsIdProvider.GetOrganizationId(Context.User);
            var connectionId = Context.ConnectionId;

            var connectionInfo = GetConnectionInfo(connectionId);
            if (connectionInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(connectionInfo.OrganizationId))
                {
                    var groupName = GetGroupName(connectionInfo.OrganizationId);
                    Groups.Remove(connectionInfo.ConnectionId, groupName);
                }
                connectionInfo.OrganizationId = organizationId;
            }
            else
            {
                connectionInfo = new ConnectionDetails
                {
                    ConnectionId = connectionId,
                    UserId = userId,
                    OrganizationId = organizationId
                };
                _connections[connectionId] = connectionInfo;
            }
            return connectionInfo;
        }

        private ConnectionDetails GetConnectionInfo(string connectionId)
        {
            if (_connections.TryGetValue(connectionId, out ConnectionDetails value))
            {
                return value;
            }
            return null;
        }

        private string GetGroupName(string organizationId) => $"{_organizationPrefix}{organizationId}";

    }
}