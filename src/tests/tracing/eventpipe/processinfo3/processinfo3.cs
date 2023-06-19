// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Diagnostics;
using System.Reflection;
using Tracing.Tests.Common;
using ProcessInfo = Microsoft.Diagnostics.NETCore.Client.ProcessInfo;

namespace Tracing.Tests.ProcessInfoValidation
{
    public class ProcessInfoValidation
    {
        public static int Main()
        {
            Process currentProcess = Process.GetCurrentProcess();
            int pid = currentProcess.Id;
            Logger.logger.Log($"Test PID: {pid}");

            DiagnosticsClient client = new(pid);

            ProcessInfo info = client.GetProcessInfo();

            ProcessInfoValidator validator = new(currentProcess, Assembly.GetExecutingAssembly());
            validator.Validate(info);

            Logger.logger.Log($"\n{{\n\tprocessId: {info.ProcessId},\n\truntimeCookie: {info.RuntimeInstanceCookie},\n\tcommandLine: {info.CommandLine},\n\tOS: {info.OperatingSystem},\n\tArch: {info.ProcessArchitecture},\n\tManagedEntrypointAssemblyName: {info.ManagedEntrypointAssemblyName},\n\tClrProductVersion: {info.ClrProductVersionString},\n\tPortableRID: {info.PortableRuntimeIdentifier}\n}}");

            return 100;
        }
    }
}