using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Constructs;

/**
 * This stack creates an S3 bucket and a CloudFront distribution.
 * The S3 bucket is configured as a static website and the CloudFront distribution
 * is configured to use the S3 bucket as its origin.
 */

namespace Cdk 
{
    public class StorageStack
    {
        public Bucket Bucket { get; }
        public Distribution Distribution { get; }

        public StorageStack(Construct scope)
        {
            Bucket = new Bucket(scope, "serverlessProjBucket", new BucketProps
            {
                BucketName = "2025-02-21-sab-serverless-proj-bucket",
                PublicReadAccess = true,
                AutoDeleteObjects = true, // Deletes the objects in the bucket when the stack is deleted
                WebsiteIndexDocument = "index.html",
                WebsiteErrorDocument = "error.html",
                RemovalPolicy = RemovalPolicy.DESTROY, // Deletes the bucket when the stack is deleted
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions { BlockPublicPolicy = false }),
            });

            Distribution = new Distribution(scope, "serverlessProjDist", new DistributionProps
            {
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = new S3StaticWebsiteOrigin(Bucket)
                }
            });
        }
    }
}