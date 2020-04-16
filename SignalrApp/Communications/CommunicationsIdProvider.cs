using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SignalrApp.Communications
{
    public class CommunicationsIdProvider : IUserIdProvider
    {
        //replace these key values with the values that are used in your authentication token
        private const string _claimTypeUserId = "LoggedInUserId";
        private const string _claimTypeOrganizationId = "CurrentOrganizationId";
                
        public string GetUserId(IRequest request)
        {
            var claim = GetUserId(request.User);
            return claim;
        }

        public static string GetUserId(IPrincipal user)
        {
            var userId = GetClaimValue(user, _claimTypeUserId);
            return userId;
        }

        /// <summary>
        /// Get the organization id to be used in the hub to maintain user's communication group
        /// </summary>
        /// <param name="user">access token</param>
        /// <returns></returns>
        public static string GetOrganizationId(IPrincipal user)
        {
            var organizationId = GetClaimValue(user, _claimTypeOrganizationId);
            return organizationId;
        }

        private static string GetClaimValue(IPrincipal user, string typeName)
        {
            var userClaim = user as ClaimsPrincipal;
            return userClaim?.Claims.FirstOrDefault(c => c.Type.Equals(typeName, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}