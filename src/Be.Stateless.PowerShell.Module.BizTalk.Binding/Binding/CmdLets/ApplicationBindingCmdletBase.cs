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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Extensions;
using Be.Stateless.BizTalk.Install.Command;

namespace Be.Stateless.BizTalk.Binding.CmdLets
{
    public abstract class ApplicationBindingCmdletBase : Cmdlet
    {
        protected internal ApplicationBindingCmdletBase(bool isApplicationBindingInstanceRequired)
        {
            _isApplicationBindingInstanceRequired = isApplicationBindingInstanceRequired;
        }

        #region Base Class Member Overrides

        protected override void BeginProcessing()
        {
            _bindingGenerationCommand = CreateApplicationBindingGenerationCommand();
            ConfigureCommand();
        }

        protected override void ProcessRecord()
        {
            _bindingGenerationCommand.Execute(Console.WriteLine);
        }

        #endregion

        [Parameter]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        public Type ApplicationSettingsOverrideType { get; set; }

        [Parameter]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        public string[] AssemblyProbingPaths { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        public string Environment { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        public string InputAssemblyFilePath { get; set; }

        protected bool IsApplicationBindingInstanceDefined => _bindingGenerationCommand.ApplicationBinding != null;

        private void ConfigureCommand()
        {
            var applicationBinding = CreateApplicationBindingsInstance();

            if (_isApplicationBindingInstanceRequired && applicationBinding == null)
                throw new InvalidOperationException($"An instance of '{typeof(IApplicationBinding)}' is required.");

            _bindingGenerationCommand.ApplicationBinding = (IVisitable<IApplicationBindingVisitor>) applicationBinding;
            _bindingGenerationCommand.AssemblyProbingPaths = AssemblyProbingPaths;
            _bindingGenerationCommand.ApplicationSettingsOverrideType = ApplicationSettingsOverrideType;
            _bindingGenerationCommand.TargetEnvironment = Environment;
        }

        protected virtual IApplicationBinding CreateApplicationBindingsInstance()
        {
            // Wil not fail here if no instance found - Will fail in configure command.
            return Assembly.LoadFrom(InputAssemblyFilePath).CreateApplicationBindingsInstances().SingleOrDefault();
        }

        protected abstract ApplicationBindingGenerationCommand CreateApplicationBindingGenerationCommand();

        private readonly bool _isApplicationBindingInstanceRequired;
        private ApplicationBindingGenerationCommand _bindingGenerationCommand;
    }
}
