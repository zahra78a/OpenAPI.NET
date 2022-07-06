﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Base interface for all the types that represent Open API Any.
    /// </summary>
    public interface IOpenApiAny : IOpenApiElement, IOpenApiExtension, ICloneable
    {
        /// <summary>
        /// Type of an <see cref="IOpenApiAny"/>.
        /// </summary>
        AnyType AnyType { get; }
    }
}
