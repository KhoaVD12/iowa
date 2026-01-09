# Iowa Helm Chart

Production-ready Helm chart for deploying the Iowa application on Kubernetes with SQL Server and Cassandra databases.

## Overview

This Helm chart deploys:
- **Iowa Application**: ASP.NET Core 10.0 web application
- **SQL Server 2022**: Used for IowaDb and IdentityDb (custom subchart)
- **Cassandra**: Used for subscription data (Bitnami chart)

All components are optimized for Kubernetes clusters with worker nodes having **4CPU/4GB RAM**.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Ingress (NGINX)                      │
│                     iowa.example.com                        │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
                    ┌────────────────┐
                    │  Iowa Service  │
                    │   (ClusterIP)  │
                    └────────┬───────┘
                             │
                             ▼
                    ┌────────────────┐
                    │  Iowa App Pod  │
                    │  (ASP.NET 10)  │
                    └────┬───────┬───┘
                         │       │
          ┌──────────────┘       └──────────────┐
          ▼                                     ▼
┌──────────────────┐                  ┌──────────────────┐
│  SQL Server      │                  │  Cassandra       │
│  (IowaDb +       │                  │  (subscriptions) │
│   IdentityDb)    │                  │                  │
│  ClusterIP:1433  │                  │  ClusterIP:9042  │
└──────┬───────────┘                  └────────┬─────────┘
       │                                       │
       ▼                                       ▼
┌──────────────┐                        ┌──────────────┐
│  PVC (5GB)   │                        │  PVC (5GB)   │
└──────────────┘                        └──────────────┘
```

## Prerequisites

- Kubernetes 1.19+
- Helm 3.8+
- StorageClass configured for PersistentVolumeClaims
- Ingress controller (nginx recommended)
- cert-manager (optional, for TLS)

## Quick Start

See [QUICKSTART.md](QUICKSTART.md) for step-by-step deployment instructions.

## Installation

### 1. Build and Push Docker Image

```bash
# Build the Iowa application image
cd /opt/project/vieteam/iowa
docker build -f src/Dockerfile -t your-registry/iowa:latest .
docker push your-registry/iowa:latest
```

See [BUILD.md](BUILD.md) for detailed build instructions.

### 2. Update Configuration

Edit `values.yaml`:

```yaml
# Update image repository
image:
  repository: your-registry/iowa
  tag: latest

# Update ingress hostname
ingress:
  hosts:
    - host: iowa.yourdomain.com
  tls:
    - secretName: iowa-tls
      hosts:
        - iowa.yourdomain.com
```

**IMPORTANT**: Change default passwords in production!

### 3. Install the Chart

```bash
cd k8s/iowa

# Add Bitnami repository (for Cassandra)
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# Install dependencies
helm dependency build

# Install the chart
helm install iowa . \
  --namespace iowa \
  --create-namespace \
  --set config.iowaDb.password=YourSecurePassword \
  --set config.identityDb.password=YourSecurePassword \
  --set mssql.config.saPassword=YourSecurePassword
```

### 4. Verify Deployment

```bash
# Check pod status
kubectl get pods -n iowa

# Check services
kubectl get svc -n iowa

# Check persistent volumes
kubectl get pvc -n iowa

# View logs
kubectl logs -n iowa -l app.kubernetes.io/name=iowa
```

## Configuration

### Iowa Application

| Parameter | Description | Default |
|-----------|-------------|---------|
| `replicaCount` | Number of replicas | `1` |
| `image.repository` | Image repository | `coolserver/iowa` |
| `image.tag` | Image tag | `latest` |
| `resources.requests.cpu` | CPU request | `500m` |
| `resources.requests.memory` | Memory request | `512Mi` |
| `resources.limits.cpu` | CPU limit | `1000m` |
| `resources.limits.memory` | Memory limit | `1Gi` |
| `ingress.enabled` | Enable ingress | `true` |
| `ingress.className` | Ingress class | `nginx` |

### SQL Server (mssql subchart)

| Parameter | Description | Default |
|-----------|-------------|---------|
| `mssql.enabled` | Enable SQL Server | `true` |
| `mssql.config.saPassword` | SA password | `SqlServer2022!` |
| `mssql.persistence.size` | Storage size | `5Gi` |
| `mssql.resources.requests.cpu` | CPU request | `500m` |
| `mssql.resources.requests.memory` | Memory request | `1Gi` |
| `mssql.resources.limits.cpu` | CPU limit | `1000m` |
| `mssql.resources.limits.memory` | Memory limit | `2Gi` |

### Cassandra (Bitnami chart)

| Parameter | Description | Default |
|-----------|-------------|---------|
| `cassandra.enabled` | Enable Cassandra | `true` |
| `cassandra.replicaCount` | Number of replicas | `1` |
| `cassandra.persistence.size` | Storage size | `5Gi` |
| `cassandra.resources.requests.cpu` | CPU request | `500m` |
| `cassandra.resources.requests.memory` | Memory request | `1Gi` |
| `cassandra.resources.limits.cpu` | CPU limit | `1000m` |
| `cassandra.resources.limits.memory` | Memory limit | `1.5Gi` |

For complete configuration options, see [values.yaml](iowa/values.yaml).

## Database Configuration

### Connection Strings

Database connections are configured via ConfigMap and injected into the application:

**SQL Server (IowaDb & IdentityDb):**
- Host: `iowa-mssql`
- Port: `1433`
- Configured in: `/opt/project/vieteam/iowa/src/appsettings.Docker.json`

**Cassandra (subscriptions):**
- ContactPoint: `iowa-cassandra`
- Port: `9042`
- Keyspace: `subscriptions`
- Configured in: `/opt/project/vieteam/iowa/src/appsettings.Docker.json`

### Database Access

All databases are configured with `ClusterIP` service type (internal access only):

```bash
# Port-forward to access SQL Server
kubectl port-forward -n iowa svc/iowa-mssql 1433:1433

# Port-forward to access Cassandra
kubectl port-forward -n iowa svc/iowa-cassandra 9042:9042
```

## Resource Optimization

This chart is optimized for worker nodes with **4CPU/4GB RAM**. See [RESOURCE_OPTIMIZATION.md](RESOURCE_OPTIMIZATION.md) for:
- Resource allocation strategy
- Scaling recommendations
- Performance tuning tips
- Cost optimization

## Upgrading

```bash
# Update the chart
helm upgrade iowa . -n iowa

# With new values
helm upgrade iowa . -n iowa -f custom-values.yaml
```

## Uninstalling

```bash
# Uninstall the release
helm uninstall iowa -n iowa

# Delete PVCs (optional - be careful!)
kubectl delete pvc -n iowa -l app.kubernetes.io/instance=iowa
```

## Troubleshooting

### Pods not starting

```bash
# Check pod events
kubectl describe pod -n iowa <pod-name>

# Check logs
kubectl logs -n iowa <pod-name>
```

### Database connection issues

```bash
# Verify database services are running
kubectl get pods -n iowa -l app.kubernetes.io/name=mssql
kubectl get pods -n iowa -l app.kubernetes.io/name=cassandra

# Check database logs
kubectl logs -n iowa <database-pod-name>
```

### Insufficient resources

```bash
# Check node resources
kubectl top nodes

# Check pod resources
kubectl top pods -n iowa

# Describe nodes to see resource allocation
kubectl describe nodes
```

See [RESOURCE_OPTIMIZATION.md](RESOURCE_OPTIMIZATION.md) for optimization strategies.

## Security Considerations

1. **Change Default Passwords**: Update all default passwords in `values.yaml`
2. **Use Secrets**: Consider using Kubernetes Secrets or external secret managers
3. **Network Policies**: Implement network policies to restrict traffic
4. **RBAC**: Configure appropriate RBAC permissions
5. **TLS**: Enable TLS for ingress using cert-manager
6. **Security Context**: Review and adjust security contexts as needed

## Production Recommendations

1. **Monitoring**: Deploy Prometheus and Grafana for monitoring
2. **Logging**: Use ELK stack or similar for centralized logging
3. **Backups**: Implement regular database backups
4. **High Availability**: Consider multi-replica deployments for production
5. **Resource Limits**: Fine-tune resource limits based on actual usage
6. **Health Checks**: Configure liveness and readiness probes

## Support

For issues and questions:
- Review [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
- Check application logs
- Verify resource allocation
- Review database connectivity

## License

[Your License Here]

## References

- [Bitnami Cassandra Helm Chart](https://github.com/bitnami/charts/tree/main/bitnami/cassandra)
- [Microsoft SQL Server Documentation](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-containers-deploy-helm-charts-kubernetes)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Helm Documentation](https://helm.sh/docs/)
