﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using NuGet.Frameworks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiOperationTests
    {
        private static readonly OpenApiOperation _basicOperation = new OpenApiOperation();

        private static readonly OpenApiOperation _operationWithBody = new OpenApiOperation
        {
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new OpenApiExternalDocs
            {
                Description = "externalDocsDescription",
                Url = new Uri("http://external.com")
            },
            OperationId = "operationId1",
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1",
                },
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            },
            RequestBody = new OpenApiRequestBody
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Number).Minimum(5).Maximum(10).Build()
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Reference = new OpenApiReference
                    {
                        Id = "response1",
                        Type = ReferenceType.Response
                    }
                },
                ["400"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Number).Minimum(5).Maximum(10).Build()
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            }
        };

        private static readonly OpenApiOperation _advancedOperationWithTagsAndSecurity = new OpenApiOperation
        {
            Tags = new List<OpenApiTag>
            {
                new OpenApiTag
                {
                    Name = "tagName1",
                    Description = "tagDescription1",
                },
                new OpenApiTag
                {
                    Reference = new OpenApiReference
                    {
                        Id = "tagId1",
                        Type = ReferenceType.Tag
                    }
                }
            },
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new OpenApiExternalDocs
            {
                Description = "externalDocsDescription",
                Url = new Uri("http://external.com")
            },
            OperationId = "operationId1",
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1"
                },
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            },
            RequestBody = new OpenApiRequestBody
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Number).Minimum(5).Maximum(10).Build()
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Reference = new OpenApiReference
                    {
                        Id = "response1",
                        Type = ReferenceType.Response
                    }
                },
                ["400"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Number).Minimum(5).Maximum(10).Build()
                        }
                    }
                }
            },
            Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "securitySchemeId1",
                            Type = ReferenceType.SecurityScheme
                        }
                    }] = new List<string>(),
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "securitySchemeId2",
                            Type = ReferenceType.SecurityScheme
                        }
                    }] = new List<string>
                    {
                        "scopeName1",
                        "scopeName2"
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            }
        };

        private static readonly OpenApiOperation _operationWithFormData =
            new OpenApiOperation()
            {
                Summary = "Updates a pet in the store with form data",
                Description = "",
                OperationId = "updatePetWithForm",
                Parameters = new List<OpenApiParameter>()
                {
                    new OpenApiParameter()
                    {
                        Name = "petId",
                        In = ParameterLocation.Path,
                        Description = "ID of pet that needs to be updated",
                        Required = true,
                        Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                    }
                },
                RequestBody = new OpenApiRequestBody()
                {
                    Content =
                    {
                        ["application/x-www-form-urlencoded"] = new OpenApiMediaType()
                        {
                            Schema31 = new JsonSchemaBuilder()
                            .Properties(
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Updated name of the pet")),
                                ("status", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Updated status of the pet")))
                            .Required("name")
                            .Build()
                        },
                        ["multipart/form-data"] = new OpenApiMediaType()
                        {
                            Schema31 = new JsonSchemaBuilder()
                            .Properties(
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Updated name of the pet")),
                                ("status", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Updated status of the pet")))
                            .Required("name")
                            .Build()
                        }
                    }
                },
                Responses = new OpenApiResponses()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "Pet updated."
                    },
                    ["405"] = new OpenApiResponse()
                    {
                        Description = "Invalid input"
                    }
                }
            };

        private readonly ITestOutputHelper _output;

        public OpenApiOperationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicOperationAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""responses"": { }
}";

            // Act
            var actual = _basicOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOperationWithBodyAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""header""
    }
  ],
  ""requestBody"": {
    ""description"": ""description2"",
    ""content"": {
      ""application/json"": {
        ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
      }
    },
    ""required"": true
  },
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/components/responses/response1""
    },
    ""400"": {
      ""description"": null,
      ""content"": {
        ""application/json"": {
          ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
        }
      }
    }
  },
  ""servers"": [
    {
      ""url"": ""http://server.com"",
      ""description"": ""serverDescription""
    }
  ]
}";

            // Act
            var actual = _operationWithBody.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedOperationWithTagAndSecurityAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""tags"": [
    ""tagName1"",
    ""tagId1""
  ],
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""header""
    }
  ],
  ""requestBody"": {
    ""description"": ""description2"",
    ""content"": {
      ""application/json"": {
        ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
      }
    },
    ""required"": true
  },
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/components/responses/response1""
    },
    ""400"": {
      ""description"": null,
      ""content"": {
        ""application/json"": {
          ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
        }
      }
    }
  },
  ""security"": [
    {
      ""securitySchemeId1"": [ ],
      ""securitySchemeId2"": [
        ""scopeName1"",
        ""scopeName2""
      ]
    }
  ],
  ""servers"": [
    {
      ""url"": ""http://server.com"",
      ""description"": ""serverDescription""
    }
  ]
}";

            // Act
            var actual = _advancedOperationWithTagsAndSecurity.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicOperationAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""responses"": { }
}";

            // Act
            var actual = _basicOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOperationWithFormDataAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""Updates a pet in the store with form data"",
  ""description"": """",
  ""operationId"": ""updatePetWithForm"",
  ""parameters"": [
    {
      ""name"": ""petId"",
      ""in"": ""path"",
      ""description"": ""ID of pet that needs to be updated"",
      ""required"": true,
      ""schema"": {""type"":""string""}
    }
  ],
  ""requestBody"": {
    ""content"": {
      ""application/x-www-form-urlencoded"": {
        ""schema"": {""properties"":{""name"":{""type"":""string"",""description"":""Updated name of the pet""},""status"":{""type"":""string"",""description"":""Updated status of the pet""}},""required"":[""name""]}
      },
      ""multipart/form-data"": {
        ""schema"": {""properties"":{""name"":{""type"":""string"",""description"":""Updated name of the pet""},""status"":{""type"":""string"",""description"":""Updated status of the pet""}},""required"":[""name""]}
      }
    }
  },
  ""responses"": {
    ""200"": {
      ""description"": ""Pet updated.""
    },
    ""405"": {
      ""description"": ""Invalid input""
    }
  }
}";

            // Act
            var actual = _operationWithFormData.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOperationWithFormDataAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""Updates a pet in the store with form data"",
  ""description"": """",
  ""operationId"": ""updatePetWithForm"",
  ""consumes"": [
    ""application/x-www-form-urlencoded"",
    ""multipart/form-data""
  ],
  ""parameters"": [
    {
      ""in"": ""path"",
      ""name"": ""petId"",
      ""description"": ""ID of pet that needs to be updated"",
      ""required"": true,
      ""type"": ""string""
    },
    {
      ""in"": ""formData"",
      ""name"": ""name"",
      ""description"": ""Updated name of the pet"",
      ""required"": true,
      ""type"": ""string""
    },
    {
      ""in"": ""formData"",
      ""name"": ""status"",
      ""description"": ""Updated status of the pet"",
      ""type"": ""string""
    }
  ],
  ""responses"": {
    ""200"": {
      ""description"": ""Pet updated.""
    },
    ""405"": {
      ""description"": ""Invalid input""
    }
  }
}";

            // Act
            var actual = _operationWithFormData.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOperationWithBodyAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""parameters"": [
    {
      ""in"": ""path"",
      ""name"": ""parameter1""
    },
    {
      ""in"": ""header"",
      ""name"": ""parameter2""
    },
    {
      ""in"": ""body"",
      ""name"": ""body"",
      ""description"": ""description2"",
      ""required"": true,
      ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
    }
  ],
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/responses/response1""
    },
    ""400"": {
      ""description"": null,
      ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
    }
  },
  ""schemes"": [
    ""http""
  ]
}";

            // Act
            var actual = _operationWithBody.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedOperationWithTagAndSecurityAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""tags"": [
    ""tagName1"",
    ""tagId1""
  ],
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""parameters"": [
    {
      ""in"": ""path"",
      ""name"": ""parameter1""
    },
    {
      ""in"": ""header"",
      ""name"": ""parameter2""
    },
    {
      ""in"": ""body"",
      ""name"": ""body"",
      ""description"": ""description2"",
      ""required"": true,
      ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
    }
  ],
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/responses/response1""
    },
    ""400"": {
      ""description"": null,
      ""schema"": {""type"":""number"",""minimum"":5,""maximum"":10}
    }
  },
  ""schemes"": [
    ""http""
  ],
  ""security"": [
    {
      ""securitySchemeId1"": [ ],
      ""securitySchemeId2"": [
        ""scopeName1"",
        ""scopeName2""
      ]
    }
  ]
}";

            // Act
            var actual = _advancedOperationWithTagsAndSecurity.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOperationWithNullCollectionAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""responses"": { }
}";
            var operation = new OpenApiOperation
            {
                Parameters = null,
                Servers = null,
            };

            // Act
            var actual = operation.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void EnsureOpenApiOperationCopyConstructorCopiesResponsesObject()
        {
            // Arrange and act
            var operation = new OpenApiOperation(_operationWithBody);

            // Assert
            Assert.NotNull(operation.Responses);
            Assert.Equal(2, operation.Responses.Count);
        }

        [Fact]
        public void EnsureOpenApiOperationCopyConstructorCopiesNull()
        {
            // Arrange
            _basicOperation.Parameters = null;
            _basicOperation.Tags = null;
            _basicOperation.Responses = null;
            _basicOperation.Callbacks = null;
            _basicOperation.Security = null;
            _basicOperation.Servers = null;
            _basicOperation.Extensions = null;

            // Act
            var operation = new OpenApiOperation(_basicOperation);

            // Assert
            Assert.Null(operation.Tags);
            Assert.Null(operation.Summary);
            Assert.Null(operation.Description);
            Assert.Null(operation.ExternalDocs);
            Assert.Null(operation.OperationId);
            Assert.Null(operation.Parameters);
            Assert.Null(operation.RequestBody);
            Assert.Null(operation.Responses);
            Assert.Null(operation.Callbacks);
            Assert.Null(operation.Security);
            Assert.Null(operation.Servers);
            Assert.Null(operation.Extensions);
        }

        [Fact]
        public void EnsureOpenApiOperationCopyConstructor_SerializationResultsInSame()
        {
            var operations = new[]
            {
                _basicOperation,
                _operationWithBody,
                _operationWithFormData,
                _advancedOperationWithTagsAndSecurity
            };

            foreach (var operation in operations)
            {
                // Act
                var expected = operation.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
                var openApiOperation = new OpenApiOperation(operation);
                var actual = openApiOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

                // Assert
                actual.Should().Be(expected);
            }
        }
    }
}
