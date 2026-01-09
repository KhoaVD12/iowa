# Iowa Kubernetes Deployment - Complete Summary

This document provides a complete overview of the Iowa Helm chart deployment.

## Project Overview

**Application**: Iowa (ASP.NET Core 10.0)
**Databases**: SQL Server 2022, Cassandra
**Target Environment**: Kubernetes clusters with 4CPU/4GB RAM worker nodes
**Chart Location**: `/opt/project/vieteam/iowa/k8s/iowa`

## What Was Created

### Helm Chart Structure

```
k8s/
├── iowa/                              # Main Helm chart
│   ├── Chart.yaml                     # Chart metadata with Cassandra dependency
│   ├── values.yaml                    # Default values (optimized for 4CPU/4GB)
│   ├── .helmignore                    # Files to ignore in Helm package
│   ├── charts/                        # Subcharts directory
│   │   └── mssql/                     # Custom SQL Server subchart
│   │       ├── Chart.yaml             # SQL Server chart metadata
│   │       ├── values.yaml            # SQL Server configuration
│   │       └── templates/
│   │           ├── deployment.yaml    # SQL Server deployment
│   │           ├── service.yaml       # SQL Server service (ClusterIP)
│   │           ├── secret.yaml        # SQL Server SA password
│   │           ├── pvc.yaml           # SQL Server persistent volume claim (5Gi)
│   │           └── _helpers.tpl       # SQL Server template helpers
│   └── templates/                     # Iowa application templates
│       ├── deployment.yaml            # Iowa app deployment
│       ├── service.yaml               # Iowa app service (ClusterIP)
│       ├── ingress.yaml               # Iowa app ingress (nginx)
│       ├── configmap.yaml             # Database configuration (appsettings.Docker.json)
│       ├── serviceaccount.yaml        # Service account
│       ├── NOTES.txt                  # Post-install instructions
│       └── _helpers.tpl               # Template helpers
├── BUILD.md                           # Docker image build instructions
├── README.md                          # Main documentation
├── QUICKSTART.md                      # Quick start guide
├── RESOURCE_OPTIMIZATION.md           # Resource optimization guide
└── DEPLOYMENT_SUMMARY.md              # This file

```

## Component Details

### 1. Iowa Application

**Purpose**: Main ASP.NET Core 10.0 application
**Image**: `coolserver/iowa:latest` (update to your registry)
**Port**: 80 (HTTP)
**Replicas**: 1 (configurable)

**Resources (Default)**:
- CPU: 500m request, 1000m limit
- Memory: 512Mi request, 1Gi limit

**Configuration**:
- Environment: Docker (ASPNETCORE_ENVIRONMENT)
- Database connections via ConfigMap
- Health probes configured (customizable)

**Access**:
- Ingress: `iowa.example.com` (nginx, TLS optional)
- Service: ClusterIP (internal)

### 2. SQL Server 2022

**Purpose**: Relational database for IowaDb and IdentityDb
**Image**: `mcr.microsoft.com/mssql/server:2022-latest`
**Port**: 1433
**Edition**: Developer (change for production)

**Resources (Default)**:
- CPU: 500m request, 1000m limit
- Memory: 1Gi request, 2Gi limit
- Storage: 5Gi PVC

**Configuration**:
- SA Password: `SqlServer2022!` (CHANGE IN PRODUCTION!)
- Databases: `ssto-database` (IowaDb and IdentityDb)
- Hostname: `iowa-mssql`

**Access**:
- Service: ClusterIP (internal only)
- Port-forward: `kubectl port-forward svc/iowa-mssql 1433:1433`

**Features**:
- Persistent storage
- Automatic retry on connection failure (5 retries, 30s delay)
- Trust server certificate enabled

### 3. Cassandra

**Purpose**: NoSQL database for subscriptions data
**Chart**: Bitnami Cassandra 12.3.11
**Image**: `bitnami/cassandra:latest`
**Port**: 9042

**Resources (Default)**:
- CPU: 500m request, 1000m limit
- Memory: 1Gi request, 1.5Gi limit
- Storage: 5Gi PVC

**Configuration**:
- Cluster: TestCluster
- Datacenter: datacenter1
- Keyspace: subscriptions
- Replicas: 1 (single node for dev/staging)

**Access**:
- Service: ClusterIP (internal only)
- Port-forward: `kubectl port-forward svc/iowa-cassandra 9042:9042`

**Features**:
- Graceful failure handling (app continues if Cassandra unavailable)
- Init ConfigMap for keyspace creation
- Single node deployment (scalable to 3+ for production)

## Database Connection Configuration

All database connections are configured in ConfigMap (`iowa` ConfigMap in namespace):

**Location in Code**: `/opt/project/vieteam/iowa/src/appsettings.Docker.json`
**Location in Cluster**: ConfigMap `iowa` → mounted at `/app/appsettings.Docker.json`

**Connection Details**:

```yaml
CassandraDb:
  ContactPoint: iowa-cassandra
  Port: 9042
  Keyspace: subscriptions
  DataCenter: datacenter1

IowaDb:
  Host: iowa-mssql
  Port: 1433
  Database: ssto-database
  Username: sa
  Password: SqlServer2022!  # From values.yaml

IdentityDb:
  Host: iowa-mssql
  Port: 1433
  Database: ssto-database
  Username: sa
  Password: SqlServer2022!  # From values.yaml
```

## Resource Allocation Summary

### Default Configuration (4CPU/4GB Node)

| Resource | Request | Limit | Notes |
|----------|---------|-------|-------|
| **Total CPU** | 1.5 cores | 3 cores | Assumes not all hit limits simultaneously |
| **Total Memory** | 2.5Gi | 4.5Gi | Tight but functional on 4GB nodes |
| **Total Storage** | 10Gi | 10Gi | 5Gi per database |

### Optimizations Available

See `RESOURCE_OPTIMIZATION.md` for:
- **Level 1 (Minimal)**: 750m CPU, 1.25Gi RAM - for development
- **Level 2 (Balanced)**: 1.5 CPU, 2.5Gi RAM - default, for staging
- **Level 3 (Performance)**: 4+ CPU, 8+ Gi RAM - for production HA

## Security Configuration

### Default Passwords (MUST CHANGE IN PRODUCTION!)

```yaml
# SQL Server
mssql.config.saPassword: "SqlServer2022!"

# Cassandra
cassandra.dbUser.password: "cassandra"

# JWT Key (in ConfigMap)
jwt.key: "f1f02a950f8873d381d7f46119cd851dc9bf061be7c3e5a0cacff81700a2e4c8..."
```

### Security Features Enabled

- ✅ Non-root user for Iowa app (UID 1000)
- ✅ Read-only root filesystem (where possible)
- ✅ Drop all capabilities
- ✅ No privilege escalation
- ✅ Service account per deployment
- ✅ Network isolation (ClusterIP for databases)
- ⚠️ TLS/SSL for ingress (requires cert-manager)

### Security Checklist for Production

- [ ] Change all default passwords
- [ ] Use Kubernetes Secrets (not ConfigMaps) for passwords
- [ ] Enable cert-manager for TLS certificates
- [ ] Configure network policies
- [ ] Enable RBAC
- [ ] Use private container registry
- [ ] Scan images for vulnerabilities
- [ ] Enable audit logging
- [ ] Configure pod security policies

## Deployment Commands

### Quick Deployment

```bash
# Navigate to chart directory
cd /opt/project/vieteam/iowa/k8s/iowa

# Add Bitnami repo
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# Build dependencies (downloads Cassandra chart)
helm dependency build

# Install
helm install iowa . \
  --namespace iowa \
  --create-namespace \
  --set image.repository=your-registry/iowa \
  --set image.tag=v1.0.0
```

### Build Docker Image

```bash
cd /opt/project/vieteam/iowa
docker build -f src/Dockerfile -t your-registry/iowa:v1.0.0 .
docker push your-registry/iowa:v1.0.0
```

See `BUILD.md` for detailed build instructions.

## Verification Steps

```bash
# 1. Check all pods are running
kubectl get pods -n iowa

# Expected:
# iowa-xxxxxxxxxx-xxxxx        1/1     Running
# iowa-cassandra-0             1/1     Running
# iowa-mssql-xxxxxxxxxx-xxxxx  1/1     Running

# 2. Check services
kubectl get svc -n iowa

# Expected:
# iowa            ClusterIP   10.x.x.x    <none>        80/TCP     2m
# iowa-cassandra  ClusterIP   10.x.x.x    <none>        9042/TCP   2m
# iowa-mssql      ClusterIP   10.x.x.x    <none>        1433/TCP   2m

# 3. Check PVCs
kubectl get pvc -n iowa

# Expected:
# iowa-mssql             Bound    pvc-xxx   5Gi        RWO
# data-iowa-cassandra-0  Bound    pvc-xxx   5Gi        RWO

# 4. Check ingress
kubectl get ingress -n iowa

# 5. Check application logs
kubectl logs -n iowa -l app.kubernetes.io/name=iowa --tail=50
```

## Common Customizations

### Change Resource Limits

```yaml
# In values.yaml or via --set
resources:
  limits:
    cpu: 1500m
    memory: 2Gi
```

### Enable Horizontal Pod Autoscaling

```yaml
autoscaling:
  enabled: true
  minReplicas: 2
  maxReplicas: 5
  targetCPUUtilizationPercentage: 70
```

### Use Different Storage Class

```yaml
mssql:
  persistence:
    storageClass: fast-ssd

cassandra:
  persistence:
    storageClass: fast-ssd
```

### Disable Cassandra (if not needed)

```yaml
cassandra:
  enabled: false
```

### Change Domain

```yaml
ingress:
  hosts:
    - host: iowa.yourdomain.com
  tls:
    - secretName: iowa-tls
      hosts:
        - iowa.yourdomain.com
```

## Monitoring and Operations

### View Logs

```bash
# Iowa application
kubectl logs -n iowa -l app.kubernetes.io/name=iowa -f

# SQL Server
kubectl logs -n iowa -l app.kubernetes.io/name=mssql -f

# Cassandra
kubectl logs -n iowa -l app.kubernetes.io/name=cassandra -f
```

### Check Resource Usage

```bash
# Node resources
kubectl top nodes

# Pod resources
kubectl top pods -n iowa

# Watch resources
watch kubectl top pods -n iowa
```

### Access Databases

```bash
# SQL Server
kubectl port-forward -n iowa svc/iowa-mssql 1433:1433
sqlcmd -S localhost,1433 -U sa -P 'SqlServer2022!'

# Cassandra
kubectl exec -n iowa iowa-cassandra-0 -- cqlsh -u cassandra -p cassandra
```

### Scaling

```bash
# Scale Iowa application
kubectl scale deployment -n iowa iowa --replicas=3

# Scale Cassandra (requires manual intervention)
# Edit values.yaml and upgrade:
# cassandra.replicaCount: 3
helm upgrade iowa . -n iowa
```

## Upgrade Process

```bash
# 1. Update image tag in values.yaml or via --set
# 2. Upgrade Helm release
helm upgrade iowa . -n iowa \
  --set image.tag=v1.1.0

# 3. Monitor rollout
kubectl rollout status deployment -n iowa iowa

# 4. Rollback if needed
helm rollback iowa -n iowa
```

## Backup and Restore

### SQL Server Backup

```bash
# Backup
kubectl exec -n iowa iowa-mssql-xxx -- \
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'password' \
  -Q "BACKUP DATABASE [ssto-database] TO DISK = '/var/opt/mssql/backup.bak'"

# Copy backup out
kubectl cp -n iowa iowa-mssql-xxx:/var/opt/mssql/backup.bak ./backup.bak
```

### Cassandra Backup

```bash
# Snapshot
kubectl exec -n iowa iowa-cassandra-0 -- \
  nodetool snapshot subscriptions -t backup-$(date +%Y%m%d)

# List snapshots
kubectl exec -n iowa iowa-cassandra-0 -- \
  nodetool listsnapshots
```

## Documentation Index

1. **README.md** - Main documentation, architecture, configuration reference
2. **QUICKSTART.md** - Step-by-step deployment guide with troubleshooting
3. **BUILD.md** - Docker image build instructions and CI/CD examples
4. **RESOURCE_OPTIMIZATION.md** - Optimization strategies for different environments
5. **DEPLOYMENT_SUMMARY.md** - This file, complete overview and reference

## Support and Troubleshooting

### Pod Crashes or CrashLoopBackOff

```bash
kubectl describe pod -n iowa <pod-name>
kubectl logs -n iowa <pod-name> --previous
```

### Database Connection Issues

1. Check database pods are running
2. Verify ConfigMap has correct connection strings
3. Check network policies (if any)
4. Verify credentials

### Out of Memory or CPU

1. Check `kubectl top pods -n iowa`
2. Review logs for memory leaks
3. Increase resource limits in values.yaml
4. See RESOURCE_OPTIMIZATION.md

### Persistent Volume Issues

```bash
kubectl describe pvc -n iowa
kubectl get pv
kubectl get storageclass
```

## Next Steps

1. ✅ Build and push Docker image (see BUILD.md)
2. ✅ Update values.yaml with your configuration
3. ✅ Change default passwords
4. ✅ Deploy using QUICKSTART.md
5. ✅ Monitor resource usage
6. ✅ Set up backups
7. ✅ Configure monitoring (Prometheus/Grafana)
8. ✅ Set up CI/CD pipeline
9. ✅ Review security settings
10. ✅ Plan for production scaling

## Architecture Decisions

### Why Custom SQL Server Subchart?

- No official Bitnami SQL Server chart exists
- Microsoft only provides sample charts
- Custom subchart gives full control over configuration
- Easier to maintain and customize

### Why Single Node Cassandra?

- Optimized for resource-constrained environments
- Sufficient for development and staging
- Easy to scale to 3+ nodes for production
- Reduces resource usage on 4GB nodes

### Why ClusterIP for Databases?

- Security best practice (no external access)
- Reduces attack surface
- Use port-forwarding for administrative access
- Ingress only for application

### Why ConfigMap Instead of Secrets?

- Easier to demonstrate and debug
- **Must be changed to Secrets in production!**
- Allows easy visibility of configuration
- Can be easily migrated to external secret managers

## Production Readiness Checklist

- [ ] Image pushed to production registry
- [ ] All default passwords changed
- [ ] Secrets used instead of ConfigMaps
- [ ] TLS/SSL enabled with cert-manager
- [ ] Resource limits tested under load
- [ ] Database backups configured
- [ ] Monitoring and alerting set up
- [ ] RBAC configured
- [ ] Network policies implemented
- [ ] Disaster recovery plan documented
- [ ] High availability tested
- [ ] Load testing completed
- [ ] Security scan passed

## Contributing

To modify this chart:

1. Update appropriate values in `values.yaml`
2. Test changes: `helm template iowa . | kubectl apply -f - --dry-run=client`
3. Lint the chart: `helm lint .`
4. Package: `helm package .`
5. Document changes in README.md

## License

[Your License Here]

---

**Generated**: 2026-01-06
**Chart Version**: 0.1.0
**App Version**: 1.0.0
**Optimized For**: 4CPU/4GB worker nodes
