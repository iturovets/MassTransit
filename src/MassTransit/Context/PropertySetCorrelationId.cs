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
namespace MassTransit.Context
{
    using System;
    using System.Reflection;
    using Internals.Reflection;


    public class PropertySetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class
    {
        readonly ReadOnlyProperty<T, Guid> _property;

        public PropertySetCorrelationId(PropertyInfo propertyInfo)
        {
            _property = new ReadOnlyProperty<T, Guid>(propertyInfo);
        }

        void ISetCorrelationId<T>.SetCorrelationId(SendContext<T> context)
        {
            var correlationId = _property.Get(context.Message);
            if (correlationId != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}