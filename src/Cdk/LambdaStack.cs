using Constructs;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.DynamoDB;
using System.Collections.Generic;

/**
 * This stack creates a Lambda function that interacts with a DynamoDB table.
 * The Lambda function is configured with environment variables that contain the table name.
 * The Lambda function is granted read and write permissions to the DynamoDB table.
 */

namespace Cdk
{
    public class LambdaStack
    {
        public Function Function { get; }

        public LambdaStack(Construct scope, Table table)
        {
            Function = new Function(scope, "ServerlessProjFunction", new FunctionProps
            {
                Runtime = Runtime.NODEJS_18_X,
                Handler = "index.handler",
                Code = Code.FromAsset("lambda"),
                Environment = new Dictionary<string, string> { { "TABLE_NAME", table.TableName } }
            });

            table.GrantReadWriteData(Function);
        }
    }
}