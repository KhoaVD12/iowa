# Resource Optimization Guide

This guide provides strategies for optimizing the Iowa application deployment on Kubernetes clusters with limited resources, specifically targeting **4CPU/4GB RAM worker nodes**.

## Resource Overview

### Current Allocation (Default values.yaml)

| Component | CPU Request | CPU Limit | Memory Request | Memory Limit | Storage |
|-----------|-------------|-----------|----------------|--------------|---------|
| Iowa App | 500m | 1000m | 512Mi | 1Gi | - |
| SQL Server | 500m | 1000m | 1Gi | 2Gi | 5Gi |
| Cassandra | 500m | 1000m | 1Gi | 1.5Gi | 5Gi |
| **TOTAL** | **1.5 CPU** | **3 CPU** | **2.5Gi** | **4.5Gi** | **10Gi** |

### Resource Strategy

The default configuration assumes:
- Not all pods will hit their CPU limits simultaneously
- Memory limits include headroom for peak usage
- Single replica per component for development/staging
- All pods can fit on one 4CPU/4GB node with careful tuning

## Optimization Levels

### Level 1: Minimal (Single Node, ~2.5GB RAM)

For development or extremely resource-constrained environments:

```yaml
# Iowa application
resources:
  requests:
    cpu: 250m
    memory: 256Mi
  limits:
    cpu: 500m
    memory: 512Mi

# SQL Server
mssql:
  resources:
    requests:
      cpu: 250m
      memory: 512Mi
    limits:
      cpu: 750m
      memory: 1Gi

# Cassandra
cassandra:
  resources:
    requests:
      cpu: 250m
      memory: 512Mi
    limits:
      cpu: 750m
      memory: 1Gi
```

**Total Request**: ~750m CPU, ~1.25Gi RAM
**Use Case**: Development, testing, proof-of-concept
**Limitations**: May experience slow performance under load

### Level 2: Balanced (Default, ~2.5GB RAM)

Current configuration - good balance between performance and resource usage:

```yaml
# Iowa application
resources:
  requests:
    cpu: 500m
    memory: 512Mi
  limits:
    cpu: 1000m
    memory: 1Gi

# SQL Server
mssql:
  resources:
    requests:
      cpu: 500m
      memory: 1Gi
    limits:
      cpu: 1000m
      memory: 2Gi

# Cassandra
cassandra:
  resources:
    requests:
      cpu: 500m
      memory: 1Gi
    limits:
      cpu: 1000m
      memory: 1.5Gi
```

**Total Request**: 1.5 CPU, ~2.5Gi RAM
**Use Case**: Staging, small production deployments
**Best For**: 4CPU/4GB nodes

### Level 3: Performance (Multi-Node, ~4GB+ RAM)

For production with higher availability requirements:

```yaml
# Iowa application - scale horizontally
replicaCount: 2
resources:
  requests:
    cpu: 750m
    memory: 768Mi
  limits:
    cpu: 1500m
    memory: 1.5Gi

# SQL Server
mssql:
  resources:
    requests:
      cpu: 1000m
      memory: 2Gi
    limits:
      cpu: 2000m
      memory: 4Gi

# Cassandra - multi-node cluster
cassandra:
  replicaCount: 3
  resources:
    requests:
      cpu: 750m
      memory: 2Gi
    limits:
      cpu: 1500m
      memory: 3Gi
```

**Total Request**: 4+ CPU, 8+ Gi RAM
**Use Case**: Production with HA
**Requirements**: Multiple nodes with 8CPU/16GB each

## Component-Specific Optimization

### Iowa Application

#### Disable Health Probes (Not Recommended)

If the application doesn't have health endpoints:

```yaml
livenessProbe: {}
readinessProbe: {}
```

#### Reduce Startup Time

```yaml
livenessProbe:
  initialDelaySeconds: 15  # Reduce from 30
  periodSeconds: 20         # Increase interval

readinessProbe:
  initialDelaySeconds: 5    # Reduce from 15
```

#### Enable Horizontal Pod Autoscaling

For production with multiple nodes:

```yaml
autoscaling:
  enabled: true
  minReplicas: 2
  maxReplicas: 5
  targetCPUUtilizationPercentage: 70
  targetMemoryUtilizationPercentage: 75
```

### SQL Server Optimization

#### Reduce Memory for Development

SQL Server is memory-intensive. For dev environments:

```yaml
mssql:
  resources:
    requests:
      cpu: 250m
      memory: 512Mi    # Minimum for SQL Server
    limits:
      cpu: 500m
      memory: 1Gi
```

#### Connection Pooling

Ensure the application uses connection pooling (already configured in Entity Framework):

```csharp
// In your connection string builder (already in code)
.WithTrustServerCertificate()  // Reduces SSL overhead
```

#### Disable SQL Server Features

For minimal installations, you can set environment variables:

```yaml
# In mssql subchart
env:
  - name: MSSQL_MEMORY_LIMIT_MB
    value: "1024"  # Limit SQL Server memory
```

### Cassandra Optimization

#### Single Node Configuration

For non-production (already default):

```yaml
cassandra:
  replicaCount: 1
  cluster:
    seedCount: 1
```

#### Reduce JVM Heap

Cassandra uses Java heap. Reduce for constrained environments:

```yaml
cassandra:
  jvm:
    maxHeapSize: 512M  # Default is calculated from memory
    newHeapSize: 128M
```

#### Disable Unnecessary Features

```yaml
cassandra:
  metrics:
    enabled: false  # Disable JMX metrics
```

## Storage Optimization

### Reduce PVC Sizes

For development:

```yaml
# SQL Server
mssql:
  persistence:
    size: 2Gi  # Reduce from 5Gi

# Cassandra
cassandra:
  persistence:
    size: 2Gi  # Reduce from 5Gi
```

### Use Faster Storage Classes

If available, use SSD-based storage:

```yaml
mssql:
  persistence:
    storageClass: fast-ssd

cassandra:
  persistence:
    storageClass: fast-ssd
```

### Dynamic Provisioning

Ensure your cluster has a default storage class:

```bash
# Check available storage classes
kubectl get storageclass

# Set default if needed
kubectl patch storageclass <name> -p '{"metadata": {"annotations":{"storageclass.kubernetes.io/is-default-class":"true"}}}'
```

## Network Optimization

### Use ClusterIP for Internal Services

Already configured - all databases use ClusterIP:

```yaml
# Don't expose databases externally
service:
  type: ClusterIP  # Not LoadBalancer or NodePort
```

### Optimize Ingress

#### Connection Limits

```yaml
ingress:
  annotations:
    nginx.ingress.kubernetes.io/proxy-body-size: "50m"
    nginx.ingress.kubernetes.io/proxy-connect-timeout: "600"
    nginx.ingress.kubernetes.io/proxy-send-timeout: "600"
    nginx.ingress.kubernetes.io/proxy-read-timeout: "600"
```

#### Enable Rate Limiting

```yaml
ingress:
  annotations:
    nginx.ingress.kubernetes.io/limit-rps: "100"
```

## Monitoring and Metrics

### Enable Resource Monitoring

```bash
# Install metrics-server (if not installed)
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml

# Check resource usage
kubectl top nodes
kubectl top pods -n iowa

# Watch resources
watch kubectl top pods -n iowa
```

### Set Resource Quotas

For multi-tenant clusters:

```yaml
apiVersion: v1
kind: ResourceQuota
metadata:
  name: iowa-quota
  namespace: iowa
spec:
  hard:
    requests.cpu: "4"
    requests.memory: 8Gi
    limits.cpu: "8"
    limits.memory: 16Gi
    persistentvolumeclaims: "5"
```

### Configure Pod Priority

For critical components:

```yaml
apiVersion: scheduling.k8s.io/v1
kind: PriorityClass
metadata:
  name: high-priority
value: 1000
globalDefault: false
---
# In values.yaml
priorityClassName: high-priority
```

## Scaling Strategies

### Vertical Pod Autoscaling (VPA)

For automatic resource adjustment:

```yaml
apiVersion: autoscaling.k8s.io/v1
kind: VerticalPodAutoscaler
metadata:
  name: iowa-vpa
  namespace: iowa
spec:
  targetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: iowa
  updatePolicy:
    updateMode: "Auto"
```

### Horizontal Pod Autoscaling (HPA)

Already configured in values.yaml:

```yaml
autoscaling:
  enabled: true
  minReplicas: 1
  maxReplicas: 3
  targetCPUUtilizationPercentage: 80
```

### Database Scaling

**SQL Server**: Vertical scaling only (increase resources)

**Cassandra**: Horizontal scaling:

```yaml
cassandra:
  replicaCount: 3  # Increase for HA
```

## Performance Tuning

### ASP.NET Core Settings

Configure in ConfigMap:

```json
{
  "Kestrel": {
    "Limits": {
      "MaxConcurrentConnections": 100,
      "MaxRequestBodySize": 52428800
    }
  }
}
```

### Database Connection Pooling

Already configured in code (see `Extensions.cs:86-89`):

```csharp
.EnableRetryOnFailure(
    maxRetryCount: 5,
    maxRetryDelay: TimeSpan.FromSeconds(30),
    errorNumbersToAdd: null)
```

### Cassandra Tuning

Optimize read/write timeouts:

```yaml
cassandra:
  readinessProbe:
    initialDelaySeconds: 60  # Cassandra takes time to start
    periodSeconds: 30
```

## Cost Optimization

### Use Spot/Preemptible Instances

For non-production:

```yaml
tolerations:
  - key: "node.kubernetes.io/spot"
    operator: "Equal"
    value: "true"
    effect: "NoSchedule"

nodeSelector:
  node.kubernetes.io/instance-type: spot
```

### Schedule Resources

Use CronJobs to scale down during off-hours:

```bash
# Scale down at night (11 PM)
kubectl create cronjob scale-down \
  --schedule="0 23 * * *" \
  --image=bitnami/kubectl \
  -- kubectl scale deployment iowa -n iowa --replicas=0

# Scale up in morning (7 AM)
kubectl create cronjob scale-up \
  --schedule="0 7 * * *" \
  --image=bitnami/kubectl \
  -- kubectl scale deployment iowa -n iowa --replicas=1
```

## Benchmarking

### Load Testing

```bash
# Install hey (HTTP load testing tool)
go install github.com/rakyll/hey@latest

# Test application
hey -n 1000 -c 10 https://iowa.example.com/

# Monitor during test
kubectl top pods -n iowa --watch
```

### Database Performance

```bash
# SQL Server - check query performance
kubectl exec -it -n iowa iowa-mssql-xxx -- /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'password' -Q "SELECT * FROM sys.dm_exec_requests"

# Cassandra - check metrics
kubectl exec -it -n iowa iowa-cassandra-0 -- nodetool status
```

## Troubleshooting Resource Issues

### OOMKilled Pods

```bash
# Check pod events
kubectl describe pod -n iowa <pod-name>

# If you see "OOMKilled", increase memory:
# In values.yaml
resources:
  limits:
    memory: 2Gi  # Increase
```

### CPU Throttling

```bash
# Check metrics
kubectl top pods -n iowa

# If CPU is constantly at limit, increase:
resources:
  limits:
    cpu: 1500m  # Increase
```

### Slow Startup

```bash
# Increase readiness probe delays
readinessProbe:
  initialDelaySeconds: 60  # Increase
  timeoutSeconds: 10       # Increase
```

## Recommended Configurations by Environment

### Development (Single Developer)

```yaml
replicaCount: 1
resources:
  requests: {cpu: 250m, memory: 256Mi}
  limits: {cpu: 500m, memory: 512Mi}
mssql:
  resources:
    requests: {cpu: 250m, memory: 512Mi}
    limits: {cpu: 500m, memory: 1Gi}
  persistence: {size: 2Gi}
cassandra:
  resources:
    requests: {cpu: 250m, memory: 512Mi}
    limits: {cpu: 500m, memory: 1Gi}
  persistence: {size: 2Gi}
```

### Staging (Team Testing)

Use default values.yaml (already optimized).

### Production (4CPU/4GB nodes)

```yaml
replicaCount: 2
autoscaling:
  enabled: true
  minReplicas: 2
  maxReplicas: 4
resources:
  requests: {cpu: 750m, memory: 768Mi}
  limits: {cpu: 1500m, memory: 1.5Gi}
mssql:
  resources:
    requests: {cpu: 750m, memory: 1.5Gi}
    limits: {cpu: 1500m, memory: 3Gi}
cassandra:
  replicaCount: 3
  resources:
    requests: {cpu: 750m, memory: 1.5Gi}
    limits: {cpu: 1500m, memory: 2Gi}
```

### Production (8CPU/16GB nodes)

Use Level 3 Performance configuration (see above).

## Summary

1. **Start with defaults** - The chart is already optimized for 4CPU/4GB nodes
2. **Monitor first** - Use `kubectl top` to see actual usage
3. **Adjust gradually** - Don't make large changes at once
4. **Test thoroughly** - Load test after each change
5. **Document changes** - Keep track of what works for your workload

For most deployments on 4CPU/4GB nodes, the **default configuration (Level 2)** provides the best balance of performance and resource efficiency.
