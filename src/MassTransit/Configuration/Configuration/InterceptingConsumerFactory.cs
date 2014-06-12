// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;


    public class InterceptingConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly ConsumerFactoryInterceptor<TConsumer> _interceptor;

        public InterceptingConsumerFactory(IConsumerFactory<TConsumer> consumerFactory,
            ConsumerFactoryInterceptor<TConsumer> interceptor)
        {
            if (consumerFactory == null)
                throw new ArgumentNullException("consumerFactory");
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            _consumerFactory = consumerFactory;
            _interceptor = interceptor;
        }

        Task IAsyncConsumerFactory<TConsumer>.GetConsumer<TMessage>(ConsumeContext<TMessage> consumeContext,
            ConsumerFactoryCallback<TConsumer, TMessage> callback)
        {
            return _consumerFactory.GetConsumer(consumeContext, (consumer, context) =>
                _interceptor(consumer, context, () => callback(consumer, context)));
        }
    }
}