#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl.Binding;

namespace Be.Stateless.BizTalk.Extensions
{
    internal static class AssemblyExtensions
    {
        public static IEnumerable<IApplicationBinding> CreateApplicationBindingsInstances(this Assembly assembly)
        {
            var serviceTypes = assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => typeof(IApplicationBinding).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes) yield return Activator.CreateInstance(serviceType) as IApplicationBinding;
        }
    }
}
