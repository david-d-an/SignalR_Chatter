# AWS Account ID
account=$1

# Image Version
version=$2

# Dockerfile name
dockerfile='Dockerfile'
# Dockerfile name for Apple Silicon
if [[ !(-z "$3") && ($3 == '--M1' || $3 == '--m1') ]]
then
  dockerfile+='_AppleSilicon'
fi

# Name of multi-architecture builder instance
buildername='multiarch'

# Architecture types supported by the resulting image
# linux/amd64 must be included.
# linux/amd64 container may run on ARM64 machines with warnings.
# linux/arm64 container doesn't run on x64 machines.
target_architecture='linux/amd64,linux/arm64'

# Login to AWS ECR
aws ecr get-login-password --region us-east-2 --profile vnext | \
docker login --username AWS --password-stdin $account.dkr.ecr.us-east-2.amazonaws.com

# Create multi-architecture builder instance and use it to build the image.
# BuildKit create an image for each architecture and tag them to the same version.
# Docker Pull automatically finds an image with the best matching architecture 
# for the host machine bound to the same tag.
# Visit https://www.docker.com/blog/multi-arch-images/ for details.
if docker buildx ls | grep -q -E $buildername'.*docker-container'
then
  docker buildx use $buildername
else
  docker buildx create --name $buildername --use
fi

# Build and push
docker buildx build --platform $target_architecture -f RealtimeServer/Docker/$dockerfile \
      -t $account.dkr.ecr.us-east-2.amazonaws.com/simple-realtime-server:$version --push .
docker buildx build --platform $target_architecture -f RealtimeServer/Docker/$dockerfile \
      -t $account.dkr.ecr.us-east-2.amazonaws.com/simple-realtime-server:latest --push .

# Come back to default builder instance
docker buildx use default

