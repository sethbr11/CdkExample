# CDK Serverless App in C#

This project demonstrates a CDK app with multiple stacks that together form a serverless application. The main stack (`CdkStack`) contains the following resources:

- An S3 bucket configured as a static website with a CloudFront distribution.
- A DynamoDB table to hold app data.
- A Lambda function with environment variables containing the table name, granted read and write permissions to the DynamoDB table.
- An API Gateway with a Lambda integration.
- A CodePipeline that builds and deploys the serverless project with three stages: Source, Build, and Deploy.

The `cdk.json` file tells the CDK Toolkit how to execute the app.

## Project Structure

- `src/Cdk/CdkStack.cs`: Main stack that defines all resources.
- `src/Cdk/StorageStack.cs`: Stack for the S3 bucket and CloudFront distribution.
- `src/Cdk/DatabaseStack.cs`: Stack for the DynamoDB table.
- `src/Cdk/LambdaStack.cs`: Stack for the Lambda function.
- `src/Cdk/ApiGatewayStack.cs`: Stack for the API Gateway.
- `src/Cdk/PipelineStack.cs`: Stack for the CodePipeline.

## Useful commands

- `dotnet build src` compile this app
- `cdk ls` list all stacks in the app
- `cdk synth` emits the synthesized CloudFormation template
- `cdk deploy` deploy this stack to your default AWS account/region
- `cdk diff` compare deployed stack with current state
- `cdk docs` open CDK documentation

Enjoy!
