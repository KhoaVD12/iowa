# Quick Start Guide

Get the Iowa application running on Kubernetes in minutes.

## Prerequisites Checklist

- [ ] Kubernetes cluster (1.19+) with at least 4CPU/4GB RAM per node
- [ ] `kubectl` configured to access your cluster
- [ ] Helm 3.8+ installed
- [ ] Docker installed (for building the image)
- [ ] Ingress controller (nginx) installed in your cluster
- [ ] Storage class configured for persistent volumes

## Step-by-Step Deployment

### Step 1: Build the Docker Image

```bash
# Navigate to the project root
cd /opt/project/vieteam/iowa

# Build the image
docker build -f src/Dockerfile -t iowa-app:v1.0.0 .

# Tag for your registry (replace with your registry)
docker tag iowa-app:v1.0.0 your-registry.com/iowa:v1.0.0

# Push to registry
docker push your-registry.com/iowa:v1.0.0
```

**Don't have a registry?** You can use:
- Docker Hub: `docker.io/yourusername/iowa:v1.0.0`
- GitHub Container Registry: `ghcr.io/yourusername/iowa:v1.0.0`
- Cloud provider registries (ECR, GCR, ACR)

### Step 2: Prepare Helm Chart

```bash
# Navigate to the helm chart directory
cd k8s/iowa

# Add Bitnami repository for Cassandra
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# Download chart dependencies
helm dependency build
```

You should see:
```
Saving 1 charts
Downloading cassandra from repo oci://registry-1.docker.io/bitnamicharts
Deleting outdated charts
```

### Step 3: Customize Configuration

Create a `custom-values.yaml` file:

```yaml
# Image configuration
image:
  repository: your-registry.com/iowa  # YOUR REGISTRY HERE
  tag: v1.0.0
  pullPolicy: IfNotPresent

# Ingress configuration
ingress:
  enabled: true
  className: nginx
  hosts:
    - host: iowa.example.com  # YOUR DOMAIN HERE
      paths:
        - path: /
          pathType: Prefix
  tls:
    - secretName: iowa-tls
      hosts:
        - iowa.example.com  # YOUR DOMAIN HERE

# Security: Change these passwords!
config:
  iowaDb:
    password: "YourSecurePassword123!"
  identityDb:
    password: "YourSecurePassword123!"

mssql:
  config:
    saPassword: "YourSecurePassword123!"

cassandra:
  dbUser:
    password: "YourSecurePassword123!"
```

**For testing without ingress**, disable it:

```yaml
ingress:
  enabled: false
```

### Step 4: Install the Chart

```bash
# Create namespace
kubectl create namespace iowa

# Install with custom values
helm install iowa . \
  --namespace iowa \
  --values custom-values.yaml
```

Or install with inline values:

```bash
helm install iowa . \
  --namespace iowa \
  --set image.repository=your-registry.com/iowa \
  --set image.tag=v1.0.0 \
  --set ingress.hosts[0].host=iowa.example.com \
  --set config.iowaDb.password=SecurePass123! \
  --set mssql.config.saPassword=SecurePass123!
```

### Step 5: Monitor Deployment

```bash
# Watch pod creation
kubectl get pods -n iowa -w

# Check all resources
kubectl get all -n iowa

# View deployment status
helm status iowa -n iowa
```

Expected output:
```
NAME                         READY   STATUS    RESTARTS   AGE
iowa-5f7b8c9d6-xxxxx        1/1     Running   0          2m
iowa-cassandra-0            1/1     Running   0          2m
iowa-mssql-xxxxxxxxx-xxxxx  1/1     Running   0          2m
```

### Step 6: Verify Databases

```bash
# Check PVCs are bound
kubectl get pvc -n iowa

# Expected output:
# NAME                   STATUS   VOLUME    CAPACITY   ACCESS MODES
# iowa-mssql             Bound    pvc-xxx   5Gi        RWO
# data-iowa-cassandra-0  Bound    pvc-xxx   5Gi        RWO

# Check SQL Server logs
kubectl logs -n iowa -l app.kubernetes.io/name=mssql

# Check Cassandra logs
kubectl logs -n iowa -l app.kubernetes.io/name=cassandra
```

### Step 7: Access the Application

#### Option A: Via Ingress (Production)

```bash
# Get ingress details
kubectl get ingress -n iowa

# Access via browser
https://iowa.example.com
```

Make sure your DNS is configured to point to your ingress controller's external IP.

#### Option B: Via Port Forward (Testing)

```bash
# Port forward to the Iowa service
kubectl port-forward -n iowa svc/iowa 8080:80

# Access via browser
http://localhost:8080
```

### Step 8: Test Database Connectivity

```bash
# Get Iowa pod name
POD_NAME=$(kubectl get pods -n iowa -l app.kubernetes.io/name=iowa -o jsonpath='{.items[0].metadata.name}')

# Check application logs for database connections
kubectl logs -n iowa $POD_NAME | grep -i "connected\|database"

# You should see:
# ✅ Cassandra connected
# And SQL Server EF migrations
```

## Verification Steps

### Check Application Health

```bash
# If health endpoint is available
kubectl port-forward -n iowa svc/iowa 8080:80
curl http://localhost:8080/health
```

### Check SQL Server

```bash
# Port forward to SQL Server
kubectl port-forward -n iowa svc/iowa-mssql 1433:1433

# Connect using SQL client (requires sqlcmd or Azure Data Studio)
sqlcmd -S localhost,1433 -U sa -P 'YourSecurePassword123!'
```

### Check Cassandra

```bash
# Port forward to Cassandra
kubectl port-forward -n iowa svc/iowa-cassandra 9042:9042

# Connect using cqlsh (if available)
kubectl exec -n iowa iowa-cassandra-0 -- cqlsh -u cassandra -p YourSecurePassword123!
```

## Common Issues and Solutions

### Issue: Pods stuck in "Pending"

**Cause**: Insufficient resources or PVC not binding

**Solution**:
```bash
# Check node resources
kubectl describe nodes | grep -A 5 "Allocated resources"

# Check PVC events
kubectl describe pvc -n iowa

# Check if storage class exists
kubectl get storageclass
```

### Issue: Image pull errors

**Cause**: Wrong image name or missing credentials

**Solution**:
```bash
# Check pod events
kubectl describe pod -n iowa <pod-name>

# Create image pull secret if needed
kubectl create secret docker-registry regcred \
  --docker-server=your-registry.com \
  --docker-username=your-username \
  --docker-password=your-password \
  -n iowa

# Update values.yaml
imagePullSecrets:
  - name: regcred
```

### Issue: Database connection failed

**Cause**: Databases not ready or wrong credentials

**Solution**:
```bash
# Check database pod logs
kubectl logs -n iowa -l app.kubernetes.io/name=mssql
kubectl logs -n iowa -l app.kubernetes.io/name=cassandra

# Verify ConfigMap
kubectl get configmap -n iowa iowa -o yaml

# Restart Iowa pods after databases are ready
kubectl rollout restart deployment -n iowa iowa
```

### Issue: Ingress not working

**Cause**: Ingress controller not installed or DNS not configured

**Solution**:
```bash
# Check if ingress controller is running
kubectl get pods -n ingress-nginx

# Get ingress external IP
kubectl get svc -n ingress-nginx

# Test without ingress first
kubectl port-forward -n iowa svc/iowa 8080:80
```

## Quick Commands Reference

```bash
# View all resources
kubectl get all -n iowa

# View logs (follow)
kubectl logs -n iowa -f -l app.kubernetes.io/name=iowa

# Restart deployment
kubectl rollout restart deployment -n iowa iowa

# Scale up/down
kubectl scale deployment -n iowa iowa --replicas=2

# Get pod shell
kubectl exec -it -n iowa <pod-name> -- /bin/bash

# Delete everything
helm uninstall iowa -n iowa
kubectl delete namespace iowa
```

## Next Steps

1. **Configure monitoring**: Set up Prometheus and Grafana
2. **Enable backups**: Configure database backup solutions
3. **Review security**: Update passwords, enable TLS
4. **Optimize resources**: See [RESOURCE_OPTIMIZATION.md](RESOURCE_OPTIMIZATION.md)
5. **Set up CI/CD**: Automate builds and deployments

## Need Help?

- Check logs: `kubectl logs -n iowa <pod-name>`
- Describe pod: `kubectl describe pod -n iowa <pod-name>`
- Review events: `kubectl get events -n iowa --sort-by='.lastTimestamp'`
- See [README.md](README.md) for detailed documentation
- See [RESOURCE_OPTIMIZATION.md](RESOURCE_OPTIMIZATION.md) for performance tuning

## Clean Up (if needed)

To completely remove the deployment:

```bash
# Uninstall Helm release
helm uninstall iowa -n iowa

# Delete persistent volumes (CAUTION: This deletes data!)
kubectl delete pvc -n iowa --all

# Delete namespace
kubectl delete namespace iowa
```

**Warning**: Deleting PVCs will permanently delete all database data!
