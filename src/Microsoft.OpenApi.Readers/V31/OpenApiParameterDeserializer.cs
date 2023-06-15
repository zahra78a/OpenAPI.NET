﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
            new FixedFieldMap<OpenApiParameter>
            {
                {
                    "name", (o, n) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
                },
                {
                    "in", (o, n) =>
                    {
                        var inString = n.GetScalarValue();

                        if ( Enum.GetValues(typeof(ParameterLocation)).Cast<ParameterLocation>()
                            .Select( e => e.GetDisplayName() )
                            .Contains(inString) )
                        {
                            o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>();
                        }
                        else
                        {
                            o.In = null;
                        }
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "required", (o, n) =>
                    {
                        o.Required = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "deprecated", (o, n) =>
                    {
                        o.Deprecated = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "allowEmptyValue", (o, n) =>
                    {
                        o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "allowReserved", (o, n) =>
                    {
                        o.AllowReserved = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "style", (o, n) =>
                    {
                        o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>();
                    }
                },
                {
                    "explode", (o, n) =>
                    {
                        o.Explode = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "schema", (o, n) =>
                    {
                        o.Schema31 = LoadSchema(n);
                    }
                },
                {
                    "content", (o, n) =>
                    {
                        o.Content = n.CreateMap(LoadMediaType);
                    }
                },
                {
                    "examples", (o, n) =>
                    {
                        o.Examples = n.CreateMap(LoadExample);
                    }
                },
                {
                    "example", (o, n) =>
                    {
                        o.Example = n.CreateAny();
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new PatternFieldMap<OpenApiParameter>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new AnyFieldMap<OpenApiParameter>
        {
            {
                OpenApiConstants.Example,
                new AnyFieldMapParameter<OpenApiParameter>(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema31)
            }
        };

        private static readonly AnyMapFieldMap<OpenApiParameter, OpenApiExample> _parameterAnyMapOpenApiExampleFields =
            new AnyMapFieldMap<OpenApiParameter, OpenApiExample>
        {
            {
                OpenApiConstants.Examples,
                new AnyMapFieldMapParameter<OpenApiParameter, OpenApiExample>(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => e.Value = v,
                    m => m.Schema31)
            }
        };

        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var description = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Description);
                var summary = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Summary);

                return mapNode.GetReferencedObject<OpenApiParameter>(ReferenceType.Parameter, pointer, summary, description);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields);
            ProcessAnyFields(mapNode, parameter, _parameterAnyFields);
            ProcessAnyMapFields(mapNode, parameter, _parameterAnyMapOpenApiExampleFields);

            return parameter;
        }
    }
}
