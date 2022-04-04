# Docker instructions
This document provides instructions to build a docker image for **RealtimeServer.SignalR** application.
When all steps in this document are completed, use a docker file in the root project folder for deployment.

## Build Image 
There are 3 steps to build a docker image for **RealtimeServer.SignalR**.
1. Go to the root project directory. (Provided that the current directory is this document's directory.)
2. Execute *docker build* command. 
3. Execute *docker run* command to run the app off of port 8081.

See below for an actual script.

```sh
> cd ..

> docker build -f RealtimeServer.SignalR/Dockerfile -t virbela.vnext.realtimeserver.signalr .

> docker run -d -p 8081:80 --name virbela.vnext.realtimeserver.signalr virbela.vnext.realtimeserver.signalr
```


# Upload to AWS ECR
Once docker has been successfully built, it's time to upload the image to ECR for deployment.
The current target repository is **virbela.vnext.realtimeserver.signalr**.

1. After the build completes, tag your image as *latest* so you can replace the latest image in the repository.
2. Tag your image one more time with a version number, so you can push the versioned image to the repository.
3. Connect to ECR.
4. Push the image with *latest* tag to ECR repository.
5. Push the image tagged with a version number to ECR repository.

See below for an actual script.

<pre>
> docker tag virbela.vnext.realtimeserver.signalr:latest 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:latest

> docker tag virbela.vnext.realtimeserver.signalr:latest 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:<i>{versionnumber}</i>

> aws ecr get-login-password --region us-east-2 | docker login --username AWS --password-stdin 656601024875.dkr.ecr.us-east-2.amazonaws.com

> docker push 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:latest

> docker push 656601024875.dkr.ecr.us-east-2.amazonaws.com/virbela.vnext.realtimeserver.signalr:<i>{versionnumber}</i>
</pre>

