// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Hosting.Tests
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class SiteExtensionReferenceAttribute : Attribute
    {
        public SiteExtensionReferenceAttribute(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;
        }

        public string Name { get; }
        public string FilePath { get; }
    }
}
