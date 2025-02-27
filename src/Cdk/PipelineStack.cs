using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.AWS.S3;
using Constructs;
using System.Collections.Generic;
using System;

/**
 * This stack creates a CodePipeline that builds and deploys a serverless project.
 * The pipeline has three stages: Source, Build, and Deploy.
 * The Source stage retrieves the source code from a GitHub repository.
 * The Build stage uses CodeBuild to build the source code.
 * The Deploy stage deploys the built artifacts to an S3 bucket.
 */

namespace Cdk
{
    public class PipelineStack
    {
        public Pipeline Pipeline { get; }

        public PipelineStack(Construct scope, Bucket bucket)
        {
            var sourceArtifact = new Artifact_();
            var buildArtifact = new Artifact_();

            Pipeline = new Pipeline(scope, "ServerlessPipeline", new PipelineProps
            {
                PipelineName = "ServerlessProjectDeploymentPipeline"
            });

            // Add stages to the pipeline
            AddSourceStage(sourceArtifact);
            AddBuildStage(scope, sourceArtifact, buildArtifact);
            AddDeployStage(bucket, buildArtifact);
        }

        private void AddSourceStage(Artifact_ sourceArtifact)
        {
            // Pipeline Source Stage: Pull from GitHub
            Pipeline.AddStage(new StageOptions
            {
                StageName = "Source",
                Actions = new IAction[]
                {
                    new GitHubSourceAction(new GitHubSourceActionProps
                    {
                        ActionName = "GitHub_Source",
                        Owner = "sethbr11",
                        Repo = "sample-site",
                        Branch = "main",
                        Output = sourceArtifact,
                        OauthToken = SecretValue.SecretsManager("github-token-sample-site")
                    })
                }
            });
        }

        private void AddBuildStage(Construct scope, Artifact_ sourceArtifact, Artifact_ buildArtifact)
        {
            // CodeBuild project: Defines the build environment
            var buildProject = new PipelineProject(scope, "BuildProject", new PipelineProjectProps
            {
                Environment = new BuildEnvironment
                {
                    BuildImage = LinuxBuildImage.STANDARD_5_0,
                    ComputeType = ComputeType.SMALL,
                    EnvironmentVariables = new Dictionary<string, IBuildEnvironmentVariable>
                    {
                        { "NODE_OPTIONS", new BuildEnvironmentVariable { Value = "--max-old-space-size=4096" } }
                    }
                },
                // Define the buildspec file
                BuildSpec = BuildSpec.FromObject(new Dictionary<string, object>
                {
                    ["version"] = "0.2",
                    ["phases"] = new Dictionary<string, object>
                    {
                        ["install"] = new Dictionary<string, object>
                        {
                            ["runtime-versions"] = new Dictionary<string, string> { ["nodejs"] = "20" }, // Set Node.js 20
                            ["commands"] = new[] { "npm install" }
                        },
                        ["build"] = new Dictionary<string, object>
                        {
                            ["commands"] = new[] { "mkdir -p dist", "npm run build" } // Ensure dist exists
                        }
                    },
                    ["artifacts"] = new Dictionary<string, object>
                    {
                        ["files"] = new[] { "**/*" },
                        ["base-directory"] = "dist"
                    }
                })
            });

            // Pipeline Build Stage: Takes code pulled from GitHub and builds it
            Pipeline.AddStage(new StageOptions
            {
                StageName = "Build",
                Actions = new IAction[]
                {
                    new CodeBuildAction(new CodeBuildActionProps
                    {
                        ActionName = "Build",
                        Project = buildProject,
                        Input = sourceArtifact,
                        Outputs = new[] { buildArtifact }
                    })
                }
            });
        }

        private void AddDeployStage(Bucket bucket, Artifact_ buildArtifact)
        {
            // Pipeline Deploy Stage: Deploys the built artifacts to an S3 bucket
            Pipeline.AddStage(new StageOptions
            {
                StageName = "Deploy",
                Actions = new IAction[]
                {
                    new S3DeployAction(new S3DeployActionProps
                    {
                        ActionName = "DeployToS3",
                        Bucket = bucket,
                        Input = buildArtifact,
                        CacheControl = new[] { CacheControl.MaxAge(Duration.Seconds(86400)) } // 1 day
                    }),
                }
            });
        }
    }
}