using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Constructs;
using Amazon.CDK.AWS.CertificateManager;

/**
 * This stack creates an S3 bucket and a CloudFront distribution.
 * The S3 bucket is configured as a static website and the CloudFront distribution is configured to use the S3 bucket as its origin.
 * The CloudFront distribution is configured with a custom domain name and a certificate.
 * The custom domain name is created using a Route 53 hosted zone and the certificate is created using the ACM certificate provider.
 */

namespace Cdk 
{
    public class StorageDistStack
    {
        public Bucket Bucket { get; }
        public Distribution Distribution { get; }

        public StorageDistStack(Construct scope)
        {
            var domainName = System.Environment.GetEnvironmentVariable("DOMAIN_NAME");
            var recordName = System.Environment.GetEnvironmentVariable("RECORD_NAME");
            var subdomainName = recordName + "." + domainName;

            // Create the S3 bucket
            Bucket = new Bucket(scope, "serverlessProjBucket", new BucketProps
            {
                BucketName = "2025-02-21-sab-serverless-proj-bucket",
                PublicReadAccess = true,
                AutoDeleteObjects = true, // Deletes the objects in the bucket when the stack is deleted
                WebsiteIndexDocument = "index.html",
                WebsiteErrorDocument = "index.html",
                RemovalPolicy = RemovalPolicy.DESTROY, // Deletes the bucket when the stack is deleted
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions { BlockPublicPolicy = false }),
            });

            // Create the certificate using Cloudflare DNS validation
            var certificate = new Certificate(scope, "SiteCertificate", new CertificateProps
            {
                DomainName = subdomainName,
                Validation = CertificateValidation.FromDns() // This works with Cloudflare DNS as well
            });

            // After CDK deploy, manually add the DNS record provided by ACM into Cloudflare
            Distribution = new Distribution(scope, "serverlessProjDist", new DistributionProps
            {
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = new S3StaticWebsiteOrigin(Bucket)
                },
                DomainNames = new[] { subdomainName },
                Certificate = certificate
            });
        }
    }
}