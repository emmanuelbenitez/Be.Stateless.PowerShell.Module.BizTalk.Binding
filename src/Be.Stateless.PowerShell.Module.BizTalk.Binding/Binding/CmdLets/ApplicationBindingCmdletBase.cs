#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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
using System.Management.Automation;
using System.Reflection;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Install.Command;

namespace Be.Stateless.BizTalk.Binding.CmdLets
{
    public abstract class ApplicationBindingCmdletBase : Cmdlet
    {
        protected internal ApplicationBindingCmdletBase(bool isApplicationBindingResolverRequired)
        {
            _isApplicationBindingResolverRequired = isApplicationBindingResolverRequired;
        }

        #region Base Class Member Overrides

        protected override void BeginProcessing()
        {
            _bindingGenerationCommand = CreateApplicationBindingGenerationCommand();
            ConfigureCommand();
        }

        protected override void ProcessRecord()
        {
            _bindingGenerationCommand.Execute(message => WriteInformation(message, new string[0]));
        }

        #endregion

        [Parameter]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "CmdLet parameter")]
        public Type ApplicationSettingsOverrideType { get; set; }

        [Parameter]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "CmdLet parameter")]
        public string[] AssemblyProbingPaths { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "CmdLet parameter")]
        public string Environment { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "CmdLet parameter")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "CmdLet parameter")]
        public string InputAssemblyFilePath { get; set; }

        protected bool IsApplicationBindingResolverDefined => _bindingGenerationCommand.ApplicationBindingResolver != null;

        private void ConfigureCommand()
        {
            var applicationBindingResolver = CreateApplicationBindingResolver();

            if (_isApplicationBindingResolverRequired && applicationBindingResolver == null)
                throw new InvalidOperationException($"An instance of '{typeof(IApplicationBindingResolver)}' is required.");

            _bindingGenerationCommand.ApplicationBindingResolver = CreateApplicationBindingResolver();
            _bindingGenerationCommand.AssemblyProbingPaths = AssemblyProbingPaths;
            _bindingGenerationCommand.ApplicationSettingOverrideType = ApplicationSettingsOverrideType;
            _bindingGenerationCommand.TargetEnvironment = Environment;
        }

        protected virtual IApplicationBindingResolver CreateApplicationBindingResolver()
        {
            return new ApplicationBindingAssemblyResolver(Assembly.LoadFrom(InputAssemblyFilePath));
        }

        protected abstract ApplicationBindingGenerationCommand CreateApplicationBindingGenerationCommand();

        private readonly bool _isApplicationBindingResolverRequired;
        private ApplicationBindingGenerationCommand _bindingGenerationCommand;
    }
}
