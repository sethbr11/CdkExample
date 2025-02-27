using Constructs;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using System.Collections.Generic;

/**
 * This stack creates an API Gateway REST API.
 * The API has a single resource, /items, which supports GET, POST, and PUT methods.
 * Each method is integrated with a Lambda function.
 */

namespace Cdk
{
    public class ApiGatewayStack
    {
        public RestApi Api { get; }

        public ApiGatewayStack(Construct scope, Function lambdaFunction)
        {
            Api = new RestApi(scope, "ServerlessProjApi");

            SetupApiGateway(Api, lambdaFunction);
        }

        private void SetupApiGateway(RestApi api, Function lambdaFunction)
        {
            var lambdaIntegration = new LambdaIntegration(lambdaFunction);

            // Create /items resource (only once)
            var items = api.Root.AddResource("items");

            // Add methods to /items resource with CORS support
            var options = new MethodOptions
            {
                MethodResponses = new[]
                {
                    new MethodResponse
                    {
                        StatusCode = "200",
                        ResponseParameters = new Dictionary<string, bool>
                        {
                            { "method.response.header.Access-Control-Allow-Origin", true },
                            { "method.response.header.Access-Control-Allow-Methods", true },
                            { "method.response.header.Access-Control-Allow-Headers", true }
                        }
                    }
                }
            };

            items.AddMethod("GET", lambdaIntegration, options);  // GET /items
            items.AddMethod("POST", lambdaIntegration, options); // POST /items
            items.AddMethod("PUT", lambdaIntegration, options);  // PUT /items

            // Enable CORS for the /items resource
            EnableCors(items);
        }

        private void EnableCors(Resource items)
        {
            // Setup CORS for the /items resource
            items.AddMethod("OPTIONS", new MockIntegration(new IntegrationOptions
            {
                IntegrationResponses = new[]
                {
                    new IntegrationResponse
                    {
                        StatusCode = "200",
                        ResponseParameters = new Dictionary<string, string>
                        {
                            { "method.response.header.Access-Control-Allow-Headers", "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'" },
                            { "method.response.header.Access-Control-Allow-Origin", "'*'" },
                            { "method.response.header.Access-Control-Allow-Methods", "'OPTIONS,GET,POST,PUT'" }
                        }
                    }
                },
                PassthroughBehavior = PassthroughBehavior.NEVER,
                RequestTemplates = new Dictionary<string, string>
                {
                    { "application/json", "{\"statusCode\": 200}" }
                }
            }), new MethodOptions
            {
                MethodResponses = new[]
                {
                    new MethodResponse
                    {
                        StatusCode = "200",
                        ResponseParameters = new Dictionary<string, bool>
                        {
                            { "method.response.header.Access-Control-Allow-Headers", true },
                            { "method.response.header.Access-Control-Allow-Origin", true },
                            { "method.response.header.Access-Control-Allow-Methods", true }
                        }
                    }
                }
            });
        }
    }
}