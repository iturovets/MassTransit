// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class PublishSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly IServiceBusHost[] _hosts;
        readonly ILog _log = Logger.Get<ServiceBusSendEndpointProvider>();
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public PublishSendEndpointProvider(IMessageSerializer serializer, Uri sourceAddress, IServiceBusHost[] hosts)
        {
            _hosts = hosts;
            _sourceAddress = sourceAddress;
            _serializer = serializer;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            IServiceBusHost host =
                _hosts.FirstOrDefault(x => x.Settings.ServiceUri.Host.Equals(address.Host, StringComparison.OrdinalIgnoreCase));
            if (host == null)
                throw new EndpointNotFoundException("The endpoint address specified an unknown host: " + address);

            TopicDescription topicDescription = await (await host.RootNamespaceManager).CreateTopicSafeAsync(address.GetTopicDescription());

            MessagingFactory messagingFactory = await host.MessagingFactory;

            MessageSender messageSender = await messagingFactory.CreateMessageSenderAsync(topicDescription.Path);

            var sendTransport = new ServiceBusSendTransport(messageSender);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress);
        }
    }
}