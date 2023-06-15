﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// The schema defining the type used for the request body.
        /// </summary>
        public JsonSchema Schema31 { get; set; }

        /// <summary>
        /// Example of the media type.
        /// The example object SHOULD be in the correct format as specified by the media type.
        /// </summary>
        public OpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// Each example object SHOULD match the media type and specified schema if present.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// A map between a property name and its encoding information.
        /// The key, being the property name, MUST exist in the schema as a property.
        /// The encoding object SHALL only apply to requestBody objects
        /// when the media type is multipart or application/x-www-form-urlencoded.
        /// </summary>
        public IDictionary<string, OpenApiEncoding> Encoding { get; set; } = new Dictionary<string, OpenApiEncoding>();

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiMediaType() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiMediaType"/> object
        /// </summary>
        public OpenApiMediaType(OpenApiMediaType mediaType)
        {
            Schema31 = JsonNodeCloneHelper.CloneJsonSchema(Schema31);
            Example = JsonNodeCloneHelper.Clone(mediaType?.Example);
            Examples = mediaType?.Examples != null ? new Dictionary<string, OpenApiExample>(mediaType.Examples) : null;
            Encoding = mediaType?.Encoding != null ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding) : null;
            Extensions = mediaType?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.1.
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (w, element) => element.SerializeAsV31(w));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (w, element) => element.SerializeAsV3(w));
        }        
        
        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));
            
            writer.WriteStartObject();

            // schema
            if(Schema31 != null)
            {
                writer.WritePropertyName(OpenApiConstants.Schema);
                writer.WriteRaw(JsonSerializer.Serialize(Schema31));
            }

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));
             
            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, callback);

            // encoding
            writer.WriteOptionalMap(OpenApiConstants.Encoding, Encoding, callback);

            // extensions
            writer.WriteExtensions(Extensions, version);
            
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Media type does not exist in V2.
        }
    }
}
