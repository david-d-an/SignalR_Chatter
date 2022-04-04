# This dockerfile pulls the image of RealtimeClient.SignalR from ECR
# Please read RealtimeServer.SignalR/DockerInstructions.md to build an image and upload to ECR before using this file.

FROM 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:latest
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "RealtimeServer.SignalR.dll"]