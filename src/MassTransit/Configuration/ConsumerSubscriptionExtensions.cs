﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Configuration;
    using Logging;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Pipeline.Sinks;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;
    using Util;

    public static class ConsumerSubscriptionExtensions
    {
        static readonly ILog _log = Logger.Get(typeof (ConsumerSubscriptionExtensions));

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] IConsumerFactory<TConsumer> consumerFactory, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", typeof (TConsumer));

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this SubscriptionBusServiceConfigurator configurator, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default consumer factory)", typeof (TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] Func<TConsumer> consumerFactory, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof (TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator Consumer(
            [NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] Type consumerType,
            [NotNull] Func<Type, object> consumerFactory, IMessageRetryPolicy retryPolicy = null)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType.ToShortTypeName());

            var consumerConfigurator = (SubscriptionBuilderConfigurator)Activator.CreateInstance(
                typeof(UntypedConsumerSubscriptionConfigurator<>).MakeGenericType(consumerType), consumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator as ConsumerSubscriptionConfigurator;
        }

        public static UnsubscribeAction SubscribeConsumer<TConsumer>([NotNull] this IServiceBus bus, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default consumer factory)", typeof (TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }

        public static UnsubscribeAction SubscribeConsumer<TConsumer>([NotNull] this IServiceBus bus,
                                                                     [NotNull] Func<TConsumer> consumerFactory, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof (TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }

        public static UnsubscribeAction SubscribeConsumer<TConsumer>([NotNull] this IServiceBus bus,
                                                                     [NotNull] IConsumerFactory<TConsumer>
                                                                         consumerFactory, IMessageRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", typeof (TConsumer));

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }

        public static UnsubscribeAction SubscribeConsumer([NotNull] this IServiceBus bus, [NotNull] Type consumerType,
                                                          [NotNull] Func<Type, object> objectFactory, IMessageRetryPolicy retryPolicy = null)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType);


            object factory = Activator.CreateInstance(typeof (ObjectConsumerFactory<>).MakeGenericType(consumerType), objectFactory);

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector(consumerType);

            return bus.Configure(x => connector.FastInvoke<ConsumerConnector, UnsubscribeAction>("Connect", x, factory));
        }
    }
}