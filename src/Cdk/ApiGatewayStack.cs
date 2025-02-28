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
        // Need to use RestApiBase instead of RestApi for v1
        public RestApiBase Api { get; }

        public ApiGatewayStack(Construct scope, Function lambdaFunction)
        {
            Api = new RestApi(scope, "ServerlessProjApi", new RestApiProps
            {
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,           // Allow all origins
                    AllowMethods = Cors.ALL_METHODS,           // Allow all methods (GET, POST, PUT, etc.)
                    AllowHeaders = new[] { "Content-Type", "X-Amz-Date", "Authorization", "X-Api-Key" } // CORS headers
                }
            });

            SetupApiGateway(Api, lambdaFunction);
        }

        private void SetupApiGateway(RestApiBase api, Function lambdaFunction)
        {
            var lambdaIntegration = new LambdaIntegration(lambdaFunction, new LambdaIntegrationOptions
            {
                IntegrationResponses = new[]
                {
                    new IntegrationResponse
                    {
                        StatusCode = "200",
                        ResponseParameters = new Dictionary<string, string>
                        {
                            { "method.response.header.Access-Control-Allow-Origin", "'*'" },
                            { "method.response.header.Access-Control-Allow-Headers", "'Content-Type,X-Amz-Date,Authorization,X-Api-Key'" },
                            { "method.response.header.Access-Control-Allow-Methods", "'GET,POST,PUT'" }
                        }
                    }
                }
            });

            var options = new MethodOptions
            {
                AuthorizationType = AuthorizationType.NONE,
                MethodResponses = new[] {
                    new MethodResponse
                    {
                        StatusCode = "200",
                        ResponseParameters = new Dictionary<string, bool>
                        {
                            { "method.response.header.Access-Control-Allow-Origin", true },
                            { "method.response.header.Access-Control-Allow-Headers", true },
                            { "method.response.header.Access-Control-Allow-Methods", true }
                        }
                    }
                }
            };

            // Create /items resource
            var items = api.Root.AddResource("items");

            // Add methods for /items resource (GET, POST, PUT)
            items.AddMethod("GET", lambdaIntegration, options);  // GET /items
            items.AddMethod("POST", lambdaIntegration, options); // POST /items
            items.AddMethod("PUT", lambdaIntegration, options);  // PUT /items
        }
    }
}