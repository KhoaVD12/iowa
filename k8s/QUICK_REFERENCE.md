# Iowa Helm Chart - Quick Reference

Essential commands and information for the Iowa Kubernetes deployment.

## Installation

```bash
# Build Docker image
cd /opt/project/vieteam/iowa
docker build -f src/Dockerfile -t your-registry/iowa:latest .
docker push your-registry/iowa:latest

# Install Helm chart
cd k8s/iowa
helm repo add bitnami https://charts.bitnami.com/bitnami
helm dependency build
helm install iowa . --namespace iowa --create-namespace \
  --set image.repository=your-registry/iowa \
  --set image.tag=latest
```

## Key Services

| Service | Type | Port | Access |
|---------|------|------|--------|
| Iowa App | ClusterIP | 80 | Via Ingress: `iowa.example.com` |
| SQL Server | ClusterIP | 1433 | Port-forward: `kubectl port-forward svc/iowa-mssql 1433:1433` |
| Cassandra | ClusterIP | 9042 | Port-forward: `kubectl port-forward svc/iowa-cassandra 9042:9042` |

## Database Configuration

**File**: `/opt/project/vieteam/iowa/src/appsettings.Docker.json`

**SQL Server**:
- Host: `iowa-mssql`
- Port: `1433`
- Database: `ssto-database`
- User: `sa`
- Password: `SqlServer2022!` (CHANGE IN PRODUCTION!)

**Cassandra**:
- Host: `iowa-cassandra`
- Port: `9042`
- Keyspace: `subscriptions`
- DataCenter: `datacenter1`

## Default Resource Allocation

| Component | CPU Request | CPU Limit | Memory Request | Memory Limit | Storage |
|-----------|-------------|-----------|----------------|--------------|---------|
| Iowa | 500m | 1000m | 512Mi | 1Gi | - |
| SQL Server | 500m | 1000m | 1Gi | 2Gi | 5Gi |
| Cassandra | 500m | 1000m | 1Gi | 1.5Gi | 5Gi |

**Total**: 1.5 CPU, 2.5Gi RAM, 10Gi storage

## Essential Commands

### Check Status
```bash
kubectl get all -n iowa
kubectl get pods -n iowa
kubectl get svc -n iowa
kubectl get pvc -n iowa
kubectl get ingress -n iowa
```

### View Logs
```bash
# Iowa application
kubectl logs -n iowa -l app.kubernetes.io/name=iowa -f

# SQL Server
kubectl logs -n iowa -l app.kubernetes.io/name=mssql -f

# Cassandra
kubectl logs -n iowa -l app.kubernetes.io/name=cassandra -f
```

### Resource Monitoring
```bash
kubectl top nodes
kubectl top pods -n iowa
watch kubectl top pods -n iowa
```

### Debugging
```bash
# Describe pod
kubectl describe pod -n iowa <pod-name>

# Get events
kubectl get events -n iowa --sort-by='.lastTimestamp'

# Shell into pod
kubectl exec -it -n iowa <pod-name> -- /bin/bash

# Check ConfigMap
kubectl get configmap -n iowa iowa -o yaml
```

### Scaling
```bash
# Scale Iowa app
kubectl scale deployment -n iowa iowa --replicas=3

# Restart deployment
kubectl rollout restart deployment -n iowa iowa

# Check rollout status
kubectl rollout status deployment -n iowa iowa
```

### Upgrade & Rollback
```bash
# Upgrade
helm upgrade iowa . -n iowa --set image.tag=v1.1.0

# Rollback
helm rollback iowa -n iowa

# History
helm history iowa -n iowa
```

### Database Access
```bash
# SQL Server
kubectl port-forward -n iowa svc/iowa-mssql 1433:1433
sqlcmd -S localhost,1433 -U sa -P 'SqlServer2022!'

# Cassandra
kubectl exec -it -n iowa iowa-cassandra-0 -- cqlsh -u cassandra -p cassandra
```

### Cleanup
```bash
# Uninstall (keeps PVCs)
helm uninstall iowa -n iowa

# Delete PVCs (CAUTION: Deletes data!)
kubectl delete pvc -n iowa --all

# Delete namespace
kubectl delete namespace iowa
```

## Important Files

| File | Location | Purpose |
|------|----------|---------|
| Docker build | `/opt/project/vieteam/iowa/src/Dockerfile` | Application image |
| DB config | `/opt/project/vieteam/iowa/src/appsettings.Docker.json` | Database connections |
| DB code | `/opt/project/vieteam/iowa/src/Databases/Extensions.cs` | Connection setup |
| Main Chart | `k8s/iowa/Chart.yaml` | Chart metadata |
| Values | `k8s/iowa/values.yaml` | Configuration |
| SQL Chart | `k8s/iowa/charts/mssql/` | SQL Server subchart |

## Security Warnings

⚠️ **CHANGE THESE IN PRODUCTION:**
- SQL Server password: `SqlServer2022!`
- Cassandra password: `cassandra`
- JWT key in ConfigMap
- Machine auth secret key

## Quick Troubleshooting

| Issue | Command | Action |
|-------|---------|--------|
| Pod won't start | `kubectl describe pod -n iowa <pod>` | Check events and resource limits |
| OOMKilled | `kubectl describe pod -n iowa <pod>` | Increase memory limits |
| DB connection failed | `kubectl logs -n iowa <pod>` | Verify DB pods are running |
| No ingress access | `kubectl get ingress -n iowa` | Check ingress controller and DNS |
| Storage issues | `kubectl describe pvc -n iowa` | Verify storage class exists |

## Documentation

- **QUICKSTART.md** - Step-by-step deployment
- **README.md** - Complete documentation
- **BUILD.md** - Docker image builds
- **RESOURCE_OPTIMIZATION.md** - Performance tuning
- **DEPLOYMENT_SUMMARY.md** - Complete overview

## Support Contacts

- Application logs: `kubectl logs -n iowa -l app.kubernetes.io/name=iowa`
- Database logs: `kubectl logs -n iowa -l app.kubernetes.io/name=mssql`
- Cluster events: `kubectl get events -n iowa`

---

**Quick Start**: See QUICKSTART.md
**Full Docs**: See README.md
**Chart Version**: 0.1.0
