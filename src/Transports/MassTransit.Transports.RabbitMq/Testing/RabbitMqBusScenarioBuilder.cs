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
namespace MassTransit.Testing
{
    using System;
    using ScenarioBuilders;


    /// <summary>
    /// A RabbitMQ bus scenario builder. Will consume off
    /// 'rabbitmq://localhost/mt_client', and use RabbitMQ routing. Timeout = 100 ms.
    /// </summary>
    public class RabbitMqBusScenarioBuilder :
        BusScenarioBuilderImpl
    {
        const string DefaultUri = "rabbitmq://localhost/mt_client";

        public RabbitMqBusScenarioBuilder()
            : base(new Uri(DefaultUri))
        {
            ConfigureEndpointFactory(x => { x.UseRabbitMq(); });

            ConfigureBus(x => { x.UseRabbitMq(); });
        }
    }
}