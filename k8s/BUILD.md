# Docker Image Build Instructions

This document provides instructions for building the Iowa application Docker image.

## Prerequisites

- Docker installed and running
- Access to the Iowa source code
- Docker registry access (if pushing to a registry)

## Building the Iowa Application Image

The Iowa application uses a multi-stage Dockerfile located at `src/Dockerfile`.

### Build Command

From the project root directory (`/opt/project/vieteam/iowa`):

```bash
# Build the image
docker build -f src/Dockerfile -t coolserver/iowa:latest .

# Or with a specific version tag
docker build -f src/Dockerfile -t coolserver/iowa:v1.0.0 .
```

### Build Process

The Dockerfile uses a multi-stage build:

1. **Build Stage** (SDK image):
   - Uses `mcr.microsoft.com/dotnet/sdk:10.0`
   - Restores NuGet packages
   - Compiles the application
   - Publishes to `/app/src/out`
   - Copies necessary Excel files for database seeding

2. **Runtime Stage** (ASP.NET image):
   - Uses `mcr.microsoft.com/dotnet/aspnet:10.0`
   - Copies only the published output
   - Minimal runtime dependencies

### Tagging Strategy

Recommended tagging strategy:

```bash
# Development
docker build -f src/Dockerfile -t coolserver/iowa:dev .

# Staging
docker build -f src/Dockerfile -t coolserver/iowa:staging .

# Production with version
docker build -f src/Dockerfile -t coolserver/iowa:v1.0.0 .
docker tag coolserver/iowa:v1.0.0 coolserver/iowa:latest
```

## Pushing to Docker Registry

### Docker Hub

```bash
# Login to Docker Hub
docker login

# Tag the image with your username
docker tag coolserver/iowa:latest yourusername/iowa:latest

# Push to Docker Hub
docker push yourusername/iowa:latest
```

### Private Registry

```bash
# Login to your private registry
docker login registry.example.com

# Tag the image
docker tag coolserver/iowa:latest registry.example.com/iowa:latest

# Push to registry
docker push registry.example.com/iowa:latest
```

### Update Helm Chart

After pushing to a registry, update the image repository in `k8s/iowa/values.yaml`:

```yaml
image:
  repository: yourusername/iowa  # or registry.example.com/iowa
  tag: latest
  pullPolicy: IfNotPresent
```

## Building for Multiple Architectures

To build for multiple architectures (amd64, arm64):

```bash
# Create and use a new builder
docker buildx create --name multiarch --use

# Build for multiple platforms
docker buildx build \
  -f src/Dockerfile \
  --platform linux/amd64,linux/arm64 \
  -t coolserver/iowa:latest \
  --push \
  .
```

## Troubleshooting

### Build fails with "unable to find package"

Ensure you're running the build command from the project root directory where `iowa.sln` is located.

### Build fails with "COPY failed"

Verify that the Excel files exist at:
- `src/Databases/App/Tables/Package/Packages.xlsx`
- `src/Databases/App/Tables/Provider/Providers.xlsx`

### Image size is too large

The multi-stage build should keep the image size reasonable. If needed, you can:
1. Review what files are being copied
2. Add unnecessary files to `.dockerignore`
3. Use a smaller base image (not recommended for .NET applications)

## Image Verification

After building, verify the image:

```bash
# Check image size
docker images coolserver/iowa:latest

# Run the container locally
docker run -p 8080:80 -e ASPNETCORE_ENVIRONMENT=Development coolserver/iowa:latest

# Inspect the image
docker inspect coolserver/iowa:latest
```

## Automated Builds

### GitHub Actions Example

Create `.github/workflows/docker-build.yml`:

```yaml
name: Build and Push Docker Image

on:
  push:
    branches: [ master ]
    tags: [ 'v*' ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Login to Docker Hub
        uses: docker/login-action@v2
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: .
          file: ./src/Dockerfile
          push: true
          tags: coolserver/iowa:latest
```

## Related Documentation

- [README.md](README.md) - Main documentation
- [QUICKSTART.md](QUICKSTART.md) - Quick deployment guide
- [RESOURCE_OPTIMIZATION.md](RESOURCE_OPTIMIZATION.md) - Resource optimization guide
