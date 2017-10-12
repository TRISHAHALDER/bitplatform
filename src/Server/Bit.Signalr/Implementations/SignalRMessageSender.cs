﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core.Contracts;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Bit.Signalr.Implementations
{
    public class SignalRMessageSender : IMessageSender
    {
        public virtual IMessageContentFormatter Formatter { get; set; }

        private IConnectionManager _connectionManager;

        public virtual Microsoft.AspNet.SignalR.IDependencyResolver SignalrDependencyResolver
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(SignalrDependencyResolver));

                _connectionManager = value.Resolve<IConnectionManager>();
            }
        }

        public virtual async Task SendMessageToUsersAsync<T>(string messageKey, T messageArgs, string[] userIds)
            where T : class
        {
            if (userIds == null)
                throw new ArgumentNullException(nameof(userIds));

            if (messageKey == null)
                throw new ArgumentNullException(nameof(messageKey));

            IHubContext hubContext = _connectionManager.GetHubContext<MessagesHub>();

            string objectArgsAsString = messageArgs == null ? string.Empty : Formatter.Serialize(messageArgs);

            userIds.ToList()
                .ForEach(async u =>
                {
                    await hubContext.Clients.Group(u).OnMessageReceived(messageKey, objectArgsAsString);
                });
        }

        public virtual void SendMessageToUsers<T>(string messageKey, T messageArgs, string[] userIds)
            where T : class
        {
            if (userIds == null)
                throw new ArgumentNullException(nameof(userIds));

            if (messageKey == null)
                throw new ArgumentNullException(nameof(messageKey));

            IHubContext hubContext = _connectionManager.GetHubContext<MessagesHub>();

            string objectArgsAsString = messageArgs == null ? string.Empty : Formatter.Serialize(messageArgs);

            userIds.ToList()
                .ForEach(u =>
                {
                    hubContext.Clients.Group(u).OnMessageReceived(messageKey, objectArgsAsString);
                });
        }

        public async Task SendMessageToAllAsync<T>(string messageKey, T messageArgs)
            where T : class
        {
            if (messageKey == null)
                throw new ArgumentNullException(nameof(messageKey));

            IHubContext hubContext = _connectionManager.GetHubContext<MessagesHub>();

            string objectArgsAsString = messageArgs == null ? string.Empty : Formatter.Serialize(messageArgs);

            await hubContext.Clients.All.OnMessageReceived(messageKey, objectArgsAsString);
        }

        public void SendMessageToAll<T>(string messageKey, T messageArgs)
            where T : class
        {
            if (messageKey == null)
                throw new ArgumentNullException(nameof(messageKey));

            IHubContext hubContext = _connectionManager.GetHubContext<MessagesHub>();

            string objectArgsAsString = messageArgs == null ? string.Empty : Formatter.Serialize(messageArgs);

            hubContext.Clients.All.OnMessageReceived(messageKey, objectArgsAsString);
        }

        public async Task SendMessageToGroupsAsync<T>(string messageKey, T messageArgs, string[] groupNames) where T : class
        {
            await SendMessageToUsersAsync(messageKey, messageArgs, groupNames);
        }

        public void SendMessageToGroups<T>(string messageKey, T messageArgs, string[] groupNames) where T : class
        {
            SendMessageToUsers(messageKey, messageArgs, groupNames);
        }
    }
}