// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Tracing.Tests.Common
{
    public sealed class ProcessInfoValidator
    {
        private readonly Assembly _assembly;
        private readonly Process _process;

        public ProcessInfoValidator(Process process, Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(process);
            ArgumentNullException.ThrowIfNull(assembly);

            _assembly = assembly;
            _process = process;
        }

        public void ValidateProcessId(uint value)
        {
            Utils.Assert(_process.Id == (int)value, $"Process ID must match. Expected: {pid}, Received: {value}");
        }

        public void ValidateRuntimeInstanceCookie(Guid value)
        {
            Utils.Assert(Guid.Empty != value, $"RuntimeInstanceCookie must not be empty.");
        }

        public void ValidateCommandLine(string value)
        {

        }
    }
}