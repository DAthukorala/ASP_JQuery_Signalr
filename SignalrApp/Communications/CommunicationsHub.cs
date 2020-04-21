using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrApp.Communications.Models;
using System.Threading.Tasks;

namespace SignalrApp.Communications
{
    //user will always be part of an organization
    //so when the user logs in, we need to read the organization info from the user access token and add the user to that organization's group
    //when the user disconnects or change the organization, we need to remove the user from the current logged in organization and join him to the new organization
    //JoinOrganizationGroup and LeaveOrganizationGroup methods can be access through the jquery signalr client.
    public class CommunicationsHub : Hub
    {
        private const string _organizationPrefix = "Organization_";
        private const string _clientReceiveMessageMethodName = "receiveMessage";
        private readonly static ConnectionMapping _userConnectionMap = new ConnectionMapping();

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
            var (groupName, id) = GetConnectionInfo();
            _userConnectionMap.Add(id, Context.ConnectionId);
            await Groups.Add(Context.ConnectionId, groupName);
        }

        public async Task LeaveOrganizationGroup()
        {
            var (groupName, id) = GetConnectionInfo();
            _userConnectionMap.Remove(id, Context.ConnectionId);
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
                    proxy = Clients.Client(GetConnectionid(message.UserId));
                    break;
            }
            proxy?.Invoke(_clientReceiveMessageMethodName, message);

            if (message.IsPersist)
            {
                var repository = new MessageRepository();
                repository.InsertMessage(message);
            }
        }

        private (string groupName, string id) GetConnectionInfo() => (GetGroupName(), GetConnectionid());

        private string GetGroupName() => $"{_organizationPrefix}{CommunicationsIdProvider.GetOrganizationId(Context.User)}";

        private string GetConnectionid() => $"{CommunicationsIdProvider.GetOrganizationId(Context.User)}_{CommunicationsIdProvider.GetUserId(Context.User)}";

        private string GetConnectionid(string userId) => _userConnectionMap.GetConnections($"{CommunicationsIdProvider.GetOrganizationId(Context.User)}_{userId}");

        //please use below code to replace above for demo purposes
        //private string GetGroupName() => $"{_organizationPrefix}{GetRandomOrgId()}";
        //private string GetConnectionid() => $"{GetRandomOrgId()}_{GetRandomuserId()}";
        //private string GetConnectionid(string userId) => _userConnectionMap.GetConnections($"{GetRandomOrgId()}_user_{userId}");
        //private string GetRandomuserId()
        //{
        //    Random random = new Random();
        //    return $"user_{random.Next(0, 4)}";
        //}
        //private string GetRandomOrgId()
        //{
        //    Random random = new Random();
        //    return $"organization_{random.Next(0, 2)}";
        //}

    }
}