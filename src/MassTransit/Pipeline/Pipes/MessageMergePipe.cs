// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Pipes
{
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageMergePipe<TConsumer, TMessage> :
        IPipe<ConsumeContext<TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _output;

        public MessageMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                ConsumerType = TypeMetadataCache<TConsumer>.ShortName,
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(ConsumeContext<TMessage> context)
        {
            var consumerConsumeContext = context as ConsumerConsumeContext<TConsumer, TMessage>;
            if (consumerConsumeContext == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<ConsumerConsumeContext<TConsumer, TMessage>>.ShortName}");

            return _output.Send(consumerConsumeContext);
        }
    }
}