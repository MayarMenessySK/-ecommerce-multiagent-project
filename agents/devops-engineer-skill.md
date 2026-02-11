# DevOps Engineer Agent - Skill Definition

## Agent Identity
**Role**: DevOps Engineer  
**Responsibility**: CI/CD pipelines, containerization, Kubernetes deployment, infrastructure as code  
**Tech Stack**: Docker, Docker Compose, Kubernetes, GitHub Actions, PostgreSQL, Nginx

---

## 🎯 BEFORE YOU START: Create GitHub Issue

```bash
gh issue create \
  --title "[DevOps] Setup CI/CD and Deployment" \
  --body "## Agent: DevOps Engineer

## Tasks
- [ ] Create Dockerfiles
- [ ] Create docker-compose.yml
- [ ] Setup GitHub Actions
- [ ] Configure Kubernetes manifests
- [ ] Setup environment variables
- [ ] Create deployment scripts

## Deliverables
- Dockerfile for each service
- docker-compose.yml
- .github/workflows/ci-cd.yml
- kubernetes/ manifests
- Deployment docs

## Dependencies
- Depends on: #5 (Tests must pass)

## Acceptance Criteria
- [ ] Docker builds successfully
- [ ] docker-compose works locally
- [ ] GitHub Actions runs tests
- [ ] K8s deployment configured" \
  --label "agent-task,devops,in-progress"
```

📖 **See GITHUB-WORKFLOW.md for details**

---

## Core Competencies

### 1. Containerization
- **Docker**: Multi-stage builds, optimization
- **Docker Compose**: Local development environment
- **Container Registry**: GitHub Container Registry (GHCR)
- **Image Optimization**: Layer caching, minimal base images
- **Security**: Non-root users, vulnerability scanning

### 2. Continuous Integration/Continuous Deployment
- **GitHub Actions**: Workflow automation
- **Build Pipeline**: Automated builds, tests, deployments
- **Deployment Strategies**: Rolling updates, blue-green deployment
- **Environment Management**: Dev, Staging, Production
- **Secrets Management**: GitHub Secrets, Kubernetes Secrets

### 3. Kubernetes Orchestration
- **Deployments**: Application deployment configurations
- **Services**: Load balancing, service discovery
- **Ingress**: External access, SSL/TLS termination
- **ConfigMaps & Secrets**: Configuration management
- **Persistent Volumes**: Database data persistence
- **Horizontal Pod Autoscaling**: Auto-scaling based on load

### 4. Monitoring & Logging
- **Application Logs**: Centralized logging
- **Metrics**: Performance metrics, health checks
- **Alerts**: Automated alerting
- **Dashboards**: Visualization (optional: Grafana)

## Technology Stack

### Core Tools
- **Docker**: Containerization platform
- **Docker Compose**: Local orchestration
- **Kubernetes**: Production orchestration
- **GitHub Actions**: CI/CD automation
- **Nginx**: Reverse proxy, load balancer
- **PostgreSQL**: Database (containerized)

### Infrastructure
- **Container Registry**: GitHub Container Registry
- **Kubernetes Cluster**: Cloud provider (AWS EKS, Azure AKS, GKE) or self-hosted
- **DNS**: Domain configuration
- **SSL/TLS**: Let's Encrypt certificates

## Project Structure

```
ecommerce-deployment/
├── docker/
│   ├── backend.Dockerfile
│   ├── frontend.Dockerfile
│   └── nginx.conf
├── docker-compose.yml
├── docker-compose.prod.yml
│
├── kubernetes/
│   ├── namespace.yaml
│   ├── configmaps/
│   │   ├── backend-config.yaml
│   │   └── frontend-config.yaml
│   ├── secrets/
│   │   ├── database-secret.yaml
│   │   ├── jwt-secret.yaml
│   │   └── stripe-secret.yaml
│   ├── deployments/
│   │   ├── backend-deployment.yaml
│   │   ├── frontend-deployment.yaml
│   │   ├── database-deployment.yaml
│   │   └── nginx-deployment.yaml
│   ├── services/
│   │   ├── backend-service.yaml
│   │   ├── frontend-service.yaml
│   │   ├── database-service.yaml
│   │   └── nginx-service.yaml
│   ├── ingress/
│   │   └── ingress.yaml
│   ├── volumes/
│   │   └── database-pvc.yaml
│   └── hpa/
│       ├── backend-hpa.yaml
│       └── frontend-hpa.yaml
│
├── .github/
│   └── workflows/
│       ├── ci.yml
│       ├── cd-dev.yml
│       ├── cd-staging.yml
│       └── cd-production.yml
│
└── scripts/
    ├── deploy.sh
    ├── rollback.sh
    └── backup-db.sh
```

## Docker Configuration

### Backend Dockerfile (Multi-Stage Build)
```dockerfile
# docker/backend.Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY src/ECommerce.API/*.csproj ./src/ECommerce.API/
COPY src/ECommerce.Core/*.csproj ./src/ECommerce.Core/
COPY src/ECommerce.Data/*.csproj ./src/ECommerce.Data/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY src/ ./src/

# Build and publish
WORKDIR /app/src/ECommerce.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1000 appuser && \
    adduser --system --uid 1000 --gid 1000 appuser

# Copy published app
COPY --from=build /app/publish ./

# Change ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
```

### Frontend Dockerfile (Multi-Stage Build)
```dockerfile
# docker/frontend.Dockerfile
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source
COPY . .

# Build
RUN npm run build

# Production image with Nginx
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Remove default Nginx static assets
RUN rm -rf ./*

# Copy built app
COPY --from=build /app/dist .

# Copy Nginx configuration
COPY docker/nginx.conf /etc/nginx/nginx.conf

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /usr/share/nginx/html && \
    chown -R appuser:appuser /var/cache/nginx && \
    chown -R appuser:appuser /var/log/nginx && \
    touch /var/run/nginx.pid && \
    chown -R appuser:appuser /var/run/nginx.pid

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080 || exit 1

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
```

### Nginx Configuration
```nginx
# docker/nginx.conf
user appuser;
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 1024;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';

    access_log /var/log/nginx/access.log main;

    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    client_max_body_size 20M;

    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml text/javascript 
               application/x-javascript application/xml+rss 
               application/json application/javascript;

    server {
        listen 8080;
        server_name _;
        root /usr/share/nginx/html;
        index index.html;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Referrer-Policy "no-referrer-when-downgrade" always;

        # SPA routing
        location / {
            try_files $uri $uri/ /index.html;
        }

        # Static assets caching
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }

        # Health check endpoint
        location /health {
            access_log off;
            return 200 "healthy\n";
            add_header Content-Type text/plain;
        }
    }
}
```

## Docker Compose (Local Development)

```yaml
# docker-compose.yml
version: '3.8'

services:
  database:
    image: postgres:16-alpine
    container_name: ecommerce-db
    environment:
      POSTGRES_DB: ecommerce
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  backend:
    build:
      context: ./backend
      dockerfile: ../docker/backend.Dockerfile
    container_name: ecommerce-backend
    environment:
      ConnectionStrings__DefaultConnection: "Host=database;Database=ecommerce;Username=postgres;Password=postgres123"
      Jwt__SecretKey: "your-dev-secret-key-min-256-bits-long"
      Jwt__Issuer: "ECommerceAPI"
      Jwt__Audience: "ECommerceClient"
      Stripe__SecretKey: "sk_test_your_key"
      ASPNETCORE_ENVIRONMENT: Development
    ports:
      - "5000:8080"
    depends_on:
      database:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  frontend:
    build:
      context: ./frontend
      dockerfile: ../docker/frontend.Dockerfile
    container_name: ecommerce-frontend
    environment:
      VITE_API_URL: http://localhost:5000/api
    ports:
      - "3000:8080"
    depends_on:
      - backend
    healthcheck:
      test: ["CMD", "wget", "--spider", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres_data:
    driver: local
```

## Kubernetes Deployments

### Namespace
```yaml
# kubernetes/namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: ecommerce
  labels:
    name: ecommerce
    environment: production
```

### Database Deployment
```yaml
# kubernetes/deployments/database-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: postgres
  namespace: ecommerce
spec:
  replicas: 1
  selector:
    matchLabels:
      app: postgres
  template:
    metadata:
      labels:
        app: postgres
    spec:
      containers:
      - name: postgres
        image: postgres:16-alpine
        ports:
        - containerPort: 5432
        env:
        - name: POSTGRES_DB
          value: ecommerce
        - name: POSTGRES_USER
          valueFrom:
            secretKeyRef:
              name: database-secret
              key: username
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: database-secret
              key: password
        volumeMounts:
        - name: postgres-storage
          mountPath: /var/lib/postgresql/data
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 5
          periodSeconds: 5
      volumes:
      - name: postgres-storage
        persistentVolumeClaim:
          claimName: postgres-pvc
```

### Backend Deployment
```yaml
# kubernetes/deployments/backend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: backend
  namespace: ecommerce
spec:
  replicas: 3
  selector:
    matchLabels:
      app: backend
  template:
    metadata:
      labels:
        app: backend
    spec:
      containers:
      - name: backend
        image: ghcr.io/your-org/ecommerce-backend:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: database-secret
              key: connection-string
        - name: Jwt__SecretKey
          valueFrom:
            secretKeyRef:
              name: jwt-secret
              key: secret-key
        - name: Jwt__Issuer
          valueFrom:
            configMapKeyRef:
              name: backend-config
              key: jwt-issuer
        - name: Jwt__Audience
          valueFrom:
            configMapKeyRef:
              name: backend-config
              key: jwt-audience
        - name: Stripe__SecretKey
          valueFrom:
            secretKeyRef:
              name: stripe-secret
              key: secret-key
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
```

### Frontend Deployment
```yaml
# kubernetes/deployments/frontend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: frontend
  namespace: ecommerce
spec:
  replicas: 2
  selector:
    matchLabels:
      app: frontend
  template:
    metadata:
      labels:
        app: frontend
    spec:
      containers:
      - name: frontend
        image: ghcr.io/your-org/ecommerce-frontend:latest
        ports:
        - containerPort: 8080
        env:
        - name: VITE_API_URL
          valueFrom:
            configMapKeyRef:
              name: frontend-config
              key: api-url
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "256Mi"
            cpu: "200m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
```

### Services
```yaml
# kubernetes/services/backend-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: backend-service
  namespace: ecommerce
spec:
  selector:
    app: backend
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: ClusterIP

---
# kubernetes/services/frontend-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: frontend-service
  namespace: ecommerce
spec:
  selector:
    app: frontend
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: ClusterIP

---
# kubernetes/services/database-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: postgres-service
  namespace: ecommerce
spec:
  selector:
    app: postgres
  ports:
  - protocol: TCP
    port: 5432
    targetPort: 5432
  type: ClusterIP
```

### Ingress
```yaml
# kubernetes/ingress/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ecommerce-ingress
  namespace: ecommerce
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    nginx.ingress.kubernetes.io/proxy-body-size: "20m"
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - ecommerce.example.com
    - api.ecommerce.example.com
    secretName: ecommerce-tls
  rules:
  - host: ecommerce.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: frontend-service
            port:
              number: 80
  - host: api.ecommerce.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: backend-service
            port:
              number: 80
```

### Horizontal Pod Autoscaler
```yaml
# kubernetes/hpa/backend-hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: backend-hpa
  namespace: ecommerce
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: backend
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

## GitHub Actions CI/CD

### CI Workflow
```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  backend-build:
    name: Backend Build & Test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend

      - name: Build
        run: dotnet build --no-restore -c Release
        working-directory: ./backend

      - name: Run Unit Tests
        run: dotnet test tests/backend/Unit/ --no-build -c Release --logger trx

      - name: Run Integration Tests
        run: dotnet test tests/backend/Integration/ --no-build -c Release --logger trx

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: '**/*.trx'

  frontend-build:
    name: Frontend Build & Test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: './frontend/package-lock.json'

      - name: Install dependencies
        run: npm ci
        working-directory: ./frontend

      - name: Lint
        run: npm run lint
        working-directory: ./frontend

      - name: Run Unit Tests
        run: npm run test:unit
        working-directory: ./frontend

      - name: Build
        run: npm run build
        working-directory: ./frontend

      - name: Run E2E Tests
        run: |
          npx playwright install --with-deps
          npm run test:e2e
        working-directory: ./frontend

  docker-build:
    name: Docker Build
    runs-on: ubuntu-latest
    needs: [backend-build, frontend-build]
    steps:
      - uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Build Backend Image
        uses: docker/build-push-action@v4
        with:
          context: ./backend
          file: ./docker/backend.Dockerfile
          push: false
          tags: ecommerce-backend:test
          cache-from: type=gha
          cache-to: type=gha,mode=max

      - name: Build Frontend Image
        uses: docker/build-push-action@v4
        with:
          context: ./frontend
          file: ./docker/frontend.Dockerfile
          push: false
          tags: ecommerce-frontend:test
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

### CD Production Workflow
```yaml
# .github/workflows/cd-production.yml
name: CD - Production

on:
  push:
    tags:
      - 'v*.*.*'

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-push:
    name: Build and Push Images
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    steps:
      - uses: actions/checkout@v3

      - name: Log in to Container Registry
        uses: docker/login-action@v2
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata (tags, labels) - Backend
        id: meta-backend
        uses: docker/metadata-action@v4
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-backend
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=semver,pattern={{major}}
            type=sha

      - name: Build and push Backend
        uses: docker/build-push-action@v4
        with:
          context: ./backend
          file: ./docker/backend.Dockerfile
          push: true
          tags: ${{ steps.meta-backend.outputs.tags }}
          labels: ${{ steps.meta-backend.outputs.labels }}

      - name: Extract metadata - Frontend
        id: meta-frontend
        uses: docker/metadata-action@v4
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-frontend
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=sha

      - name: Build and push Frontend
        uses: docker/build-push-action@v4
        with:
          context: ./frontend
          file: ./docker/frontend.Dockerfile
          push: true
          tags: ${{ steps.meta-frontend.outputs.tags }}
          labels: ${{ steps.meta-frontend.outputs.labels }}

  deploy-production:
    name: Deploy to Production
    runs-on: ubuntu-latest
    needs: build-and-push
    environment:
      name: production
      url: https://ecommerce.example.com
    steps:
      - uses: actions/checkout@v3

      - name: Set up kubectl
        uses: azure/setup-kubectl@v3

      - name: Configure Kubernetes
        run: |
          echo "${{ secrets.KUBE_CONFIG }}" | base64 -d > kubeconfig
          export KUBECONFIG=kubeconfig

      - name: Update image tags
        run: |
          kubectl set image deployment/backend backend=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-backend:${{ github.ref_name }} -n ecommerce
          kubectl set image deployment/frontend frontend=${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-frontend:${{ github.ref_name }} -n ecommerce

      - name: Wait for rollout
        run: |
          kubectl rollout status deployment/backend -n ecommerce --timeout=5m
          kubectl rollout status deployment/frontend -n ecommerce --timeout=5m

      - name: Verify deployment
        run: |
          kubectl get pods -n ecommerce
          kubectl get services -n ecommerce
```

## Deployment Scripts

### Deploy Script
```bash
#!/bin/bash
# scripts/deploy.sh

set -e

ENVIRONMENT=$1
VERSION=$2

if [ -z "$ENVIRONMENT" ] || [ -z "$VERSION" ]; then
    echo "Usage: ./deploy.sh <environment> <version>"
    exit 1
fi

echo "Deploying version $VERSION to $ENVIRONMENT..."

# Apply Kubernetes manifests
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/configmaps/ -n ecommerce
kubectl apply -f kubernetes/secrets/ -n ecommerce
kubectl apply -f kubernetes/volumes/ -n ecommerce
kubectl apply -f kubernetes/deployments/ -n ecommerce
kubectl apply -f kubernetes/services/ -n ecommerce
kubectl apply -f kubernetes/ingress/ -n ecommerce
kubectl apply -f kubernetes/hpa/ -n ecommerce

# Update image versions
kubectl set image deployment/backend backend=ghcr.io/your-org/ecommerce-backend:$VERSION -n ecommerce
kubectl set image deployment/frontend frontend=ghcr.io/your-org/ecommerce-frontend:$VERSION -n ecommerce

# Wait for rollout
kubectl rollout status deployment/backend -n ecommerce
kubectl rollout status deployment/frontend -n ecommerce

echo "Deployment complete!"
```

### Rollback Script
```bash
#!/bin/bash
# scripts/rollback.sh

set -e

DEPLOYMENT=$1

if [ -z "$DEPLOYMENT" ]; then
    echo "Usage: ./rollback.sh <deployment-name>"
    exit 1
fi

echo "Rolling back deployment: $DEPLOYMENT"

kubectl rollout undo deployment/$DEPLOYMENT -n ecommerce
kubectl rollout status deployment/$DEPLOYMENT -n ecommerce

echo "Rollback complete!"
```

## Monitoring & Logging

### Health Check Endpoints
Backend (ASP.NET Core):
```csharp
// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});
```

## Git Workflow
- **Branch Naming**: `devops/[feature-name]`
- **Infrastructure as Code**: All configs in Git
- **Secrets**: Never commit secrets, use GitHub Secrets

## Environment Variables

### Development
- `ASPNETCORE_ENVIRONMENT=Development`
- `POSTGRES_PASSWORD=postgres123`

### Production
- `ASPNETCORE_ENVIRONMENT=Production`
- Secrets from Kubernetes Secrets

## Deliverables
- [ ] Dockerfiles for all services
- [ ] Docker Compose for local development
- [ ] Kubernetes manifests (all resources)
- [ ] GitHub Actions CI/CD pipelines
- [ ] Deployment scripts
- [ ] Rollback procedures
- [ ] Monitoring setup
- [ ] SSL/TLS certificates
- [ ] Database backup procedures
- [ ] Infrastructure documentation

## Success Criteria
- All services containerized
- Local development with Docker Compose working
- CI/CD pipeline fully automated
- Kubernetes cluster deployed
- Zero-downtime deployments
- Auto-scaling configured
- Monitoring and logging operational
- Backup/restore procedures tested
- SSL/TLS configured
- Documentation complete
