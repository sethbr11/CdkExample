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
- `lambda/index.js`: Main lambda function for interacting with the DynamoDB table.
- `lambda/seed.js`: Seeder function to fill the DynamoDB table with starter data.

## Useful commands

- `dotnet build src` compile this app
- `cdk ls` list all stacks in the app
- `cdk synth` emits the synthesized CloudFormation template
- `cdk deploy` deploy this stack to your default AWS account/region
- `cdk diff` compare deployed stack with current state
- `cdk docs` open CDK documentation

## Step-by-Step

1. Make sure to recreate the `.env` file with your own values. If you are not using a custom DNS, you may need to adjust some of the code in `StorageDistStack.cs`.
2. You will want to change the repository specified in `PipelineStack.cs` to be one that you control. If you want to use the sample site, clone/fork the repository so you are later able to change the API endpoint.
3. Run `cdk deploy` and confirm the structure of the stack.
4. If using a custom domain (which the code is set up for):
   - Go to [AWS Certificate Manager](https://aws.amazon.com/certificate-manager/) while the deploy is still running. Once you see the certificate associated with the deploy pop up, click on it to get the CNAME name and CNAME value. The CNAME name will have the full URL value, but you just need to use everything up to the domain name (example: if it gave the name `_e0aace964f4adac9a89e4d5dad37a9b8.subdomain.example.com.`, you just put in `_e0aace964f4adac9a89e4d5dad37a9b8.subdomain` as the name in your CNAME record). Put the CNAME value as the target in your DNS record. Lastly, disable proxy status. This will allow AWS to verify your domain.
   - Next, go to [CloudFront](https://aws.amazon.com/cloudfront/), find the distribution that is being created, and copy the distribution domain name to use as another value in a CNAME record, except this time the name will just be the subdomain you want to use (example: with the example above, you would put in the value `subdomain` as the name of the CNAME record and the value as the distribution domain name, which would be something like `sjgj1058pq7mw.cloudfront.net`, leaving out the `https://` and keeping the proxy on).
5. When the CDK is finished deploying, you will get four outputs: `CdkStack.CloudFrontURL`, `CdkStack.LambdaFunctionName`, `CdkStack.S3BucketURL`, and `CdkStack.ServerlessProjApiEndpoint########`. Take the API endpoint output and put it in your repository. Doing a git push will update your site in the S3 bucket, but it will take time before the CloudFront cache refreshes—default should be about an hour.

That's it! Your serverless application is now up and running!
