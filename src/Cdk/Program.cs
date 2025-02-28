using Amazon.CDK;
using System.IO;
using DotNetEnv;

namespace Cdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env file
            Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            var app = new App();

            new CdkStack(app, "CdkStack", new StackProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("AWS_ACCOUNT_ID"),
                    Region = System.Environment.GetEnvironmentVariable("AWS_REGION")
                }
            });

            app.Synth();
        }
    }
}
