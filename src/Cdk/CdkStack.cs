using Amazon.CDK;
using Constructs;

/**
 * This stack creates an S3 bucket, a DynamoDB table, a Lambda function, an API Gateway, and a CodePipeline.
 * The S3 bucket is configured as a static website and the CloudFront distribution is configured to use the S3 bucket as its origin.
 * The DynamoDB table is created with a partition key and a removal policy of DESTROY.
 * The Lambda function is configured with environment variables that contain the table name and is granted read and write permissions to the DynamoDB table.
 * The API Gateway is configured with a Lambda integration.
 * The CodePipeline builds and deploys the serverless project.
 * The pipeline has three stages: Source, Build, and Deploy.
 * The Source stage retrieves the source code from a GitHub repository.
 * The Build stage uses CodeBuild to build the source code.
 * The Deploy stage deploys the built artifacts to an S3 bucket.
 * The DynamoDB table is seeded with initial data using a Lambda function and a custom resource.
 */

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Define stack resources
            var bucket = new StorageStack(this);
            var table = new DatabaseStack(this);
            var lambdaFunction = new LambdaStack(this, table.Table);
            var api = new ApiGatewayStack(this, lambdaFunction.Function);
            new PipelineStack(this, bucket.Bucket);

            // Outputs for CloudFront, Lambda, and API Gateway
            new CfnOutput(this, "CloudFrontURL", new CfnOutputProps { Value = bucket.Distribution.DistributionDomainName });
            new CfnOutput(this, "LambdaFunctionName", new CfnOutputProps { Value = lambdaFunction.Function.FunctionName });
            new CfnOutput(this, "ApiGatewayURL", new CfnOutputProps { Value = api.Api.Url });
        }
    }
}
