# Docker Deployment to Elastic Beanstalk 

Elastic Beanstalk doesn't work with a multi-staged Dockerfile.
For example, the below syntax will break the deployment process because **AS base** is not expected by EB's docker engine.

```
 FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
```
<br>

A solution to this problem is to keep the Dockerfile as simple as possible.
I took an approach described below using Elastic Container Registry.

  1. Create EB environment. Do not upload anything yet.
  2. Add AmazonEC2ContainerRegistryReadOnly policy to the EC2 instance of 
     EB environment created during #1.
  3. Build a Docker image of your app and push it to ECR.
  4. Create a simple Dockerfile that pulls the image from ECR.
  5. Upload Dockerfile built during #5 to EB Environment.
     Since the image is already built, the app starts almost immediately.

Below is an example. The file is very simple and there is no build action.
It only pulls an image from ECR and exposes ports and an application entry point.

```
FROM 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:latest
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "RealtimeServer.SignalR.dll"]
```

# AWS Security Settings
In order to pull an image in ECR from an EB instnace, the EB instance must have the following access permission to ECR.

  1. BatchGetImage
  1. GetAuthorizationToken

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "ecr:BatchGetImage",
                "ecr:GetAuthorizationToken"
            ],
            "Resource": "*"
        }
    ]
}
```