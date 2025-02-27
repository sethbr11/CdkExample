using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.CustomResources;
using Constructs;
using System.Collections.Generic;

/**
 * This stack creates a DynamoDB table and seeds the table with initial data using a Lambda function and a custom resource.
 * The DynamoDB table is created with a partition key and a removal policy of DESTROY.
 * The Lambda function is configured with environment variables that contain the table name and is granted read and write permissions to the DynamoDB table.
 * The custom resource provider triggers the Lambda function to seed the DynamoDB table with initial data.
 */

namespace Cdk
{
    public class DatabaseStack : Construct
    {
        public Table Table { get; }

        public DatabaseStack(Construct scope) : base(scope, "DatabaseStack")
        {
            Table = new Table(scope, "ServerlessProjTable", new TableProps
            {
                PartitionKey = new Attribute { Name = "id", Type = AttributeType.STRING },
                TableName = "ServerlessProjTable",
                RemovalPolicy = RemovalPolicy.DESTROY // Deletes the table when the stack is deleted
            });

            // Create the Seed Lambda Function
            var seedFunction = new Function(this, "SeedFunction", new FunctionProps
            {
                Runtime = Runtime.NODEJS_18_X,
                Handler = "seed.handler",
                Code = Code.FromAsset("lambda"),
                Timeout = Duration.Seconds(30),
                Environment = new Dictionary<string, string>
                {
                    { "TABLE_NAME", Table.TableName }
                }
            });

            // Grant Seed Function Permissions to Write to the Table
            Table.GrantReadWriteData(seedFunction);

            // Custom Resource Provider to Trigger Seeding
            var provider = new Provider(this, "SeedProvider", new ProviderProps
            {
                OnEventHandler = seedFunction
            });

            new CustomResource(this, "SeedResource", new CustomResourceProps
            {
                ServiceToken = provider.ServiceToken
            });
        }
    }
}