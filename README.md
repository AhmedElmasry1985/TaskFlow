# Task Flow Microservices App - Project Spec



## Project Overview

A microservices-based task management application implementing concepts from Les Jackson's .NET Microservices course. The project includes user authentication, task management, async notifications, and API gateway orchestration.

Timeline: ~15 days

Learning Focus: Practical implementation of microservices patterns, inter-service communication, CICD, and Kubernetes deployment.



## Architecture

### High-Level Design

\- **API Gateway:** Nginx for request routing and load balancing

\- **Service Communication:**

    - Synchronous: gRPC for real-time service-to-service calls

    - Asynchronous: RabbitMQ for event-driven notifications

\- **Data Persistence:** SQL Server with separate databases per service (Database-per-Service pattern)

\- **Container Orchestration:** Kubernetes for deployment and scaling



## Services Breakdown

1. ### UsersApi



**Purpose:** User authentication and authorization



**Responsibilities:**

\- User registration

\- User login with JWT token generation

\- Token validation and refresh

\- Publish events to RabbitMQ for notifications



**Tech:**

\- .NET 9 Web API

\- Entity Framework Core

\- SQL Server (dedicated database)

\- JWT authentication middleware

\- RabbitMQ client

\- gRPC server



**Endpoints:**

\- `POST /api/users/Register`

\- `POST /api/users/Login`

\- `GET /api/users/Me` (authenticated)



**Events Published:**

\- `UserRegistered`





#### 2\. TasksApi



**Purpose:** Core task management functionality



**Responsibilities:**

\- Create tasks

\- Complete tasks

\- Cancel tasks

\- Add notes to tasks

\- Publish events to RabbitMQ for notifications

\- Sync users via gRPC call to UsersApi



**Tech:**

\- .NET 9 Web API

\- Entity Framework Core

\- SQL Server (dedicated database)

\- JWT authentication middleware

\- RabbitMQ client

\- gRPC client



**Endpoints:**

\- `POST /api/tasks/CreateTask` - Create task

\- `GET /api/tasks/GetMine` - List user tasks

\- `GET /api/tasks/GetAssignedToMe` - Get task assigned to user

\- `POST /api/tasks/AddNote` - Add a note to a task

\- `POST /api/tasks/ChangeStatus` - Change a task status



**Events Published:**

\- `TaskCreated`

\- `TaskStatusChanged`

\- `NoteAdded`



### 3\. NotificationService



**Purpose:** Consume events and log notifications



**Responsibilities:**

\- Subscribe to RabbitMQ events

\- Log notification messages to console

\- (Future: Send SMS/Email notifications)



**Tech:**

\- .NET 9 Console App (BackgroundService/Worker)

\- RabbitMQ consumer

\- No database required



**Event Subscriptions:**

\- `UserRegistered`→ Log "User 'full name' with username 'username' registered."

\- `TaskCreated` → Log "Task 'task name' created by 'creator user' and assigned to 'assigned user'."

\- `TaskStatusChanged` → Log "Task 'task name' status changed from 'old status' to 'new status'."

\- `NoteAdded` → Log "Note 'note content' added to task 'task name' by 'creator user'."



#### 4\. API Gateway (Nginx)



**Purpose:** Single entry point for client applications



**Responsibilities:**

\- Route requests to appropriate services

\- Load balancing

\- SSL termination (optional)



**Routes:**

\- `/api/users/\*` → UsersApi

\- `/api/tasks/\*` → TasksApi





## Tech Stack



Runtime:		.NET 9

IDE:			JetBrains Rider (Windows 11)

API Framework:		ASP.NET Core Web API

ORM:			Entity Framework Core

Database:		SQL Server + PVC

Message Broker:		RabbitMQ

Sync Communication:	gRPC

API Gateway:		Nginx

Containerization:	Docker

Orchestration:		Kubernetes

Source Control:		Git/Github

CI/CD:			Github Actions


## Deployment

TaskFlow supports two deployment methods: **Kubernetes** (production-ready) and **Docker Compose** (local development).

---

### Option 1: Kubernetes Deployment (Recommended)

**Prerequisites:**
- Docker Desktop with Kubernetes enabled
- kubectl CLI installed
- 4GB+ RAM available

**Quick Start:**

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/TaskFlow.git
   cd TaskFlow
   ```

2. **Deploy to Kubernetes:**
   ```bash
   chmod +x k8s-redeploy.sh
   ./k8s-redeploy.sh
   ```

3. **Verify deployment:**
   ```bash
   kubectl get all -n taskflow
   ```

4. **Add host entry:**
   ```bash
   # Windows: C:\Windows\System32\drivers\etc\hosts
   # Linux/Mac: /etc/hosts
   127.0.0.1 taskflow.com
   ```

5. **Test the API:**
   ```bash
   curl http://taskflow.com/api/users/health
   ```

6. **Cleanup (remove all resources):**
   ```bash
   chmod +x k8s-cleanup.sh
   ./k8s-cleanup.sh
   ```

**Features:**
- ✅ Production-ready with health checks and resource limits
- ✅ Namespace isolation (`taskflow`)
- ✅ Nginx ingress for routing
- ✅ Persistent volumes for SQL Server databases
- ✅ Automatic pod restarts on failure

---

### Option 2: Docker Compose (Local Development)

**Prerequisites:**
- Docker Desktop installed
- 4GB+ RAM available

**Quick Start:**

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/TaskFlow.git
   cd TaskFlow/src
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **Verify services are running:**
   ```bash
   docker-compose ps
   ```

4. **Test the API:**
   ```bash
   curl http://localhost:8080/health
   ```

5. **View logs:**
   ```bash
   docker-compose logs -f usersapi
   docker-compose logs -f tasksapi
   docker-compose logs -f notificationservice
   ```

6. **Stop all services:**
   ```bash
   docker-compose down
   ```

7. **Stop and remove volumes (clean slate):**
   ```bash
   docker-compose down -v
   ```

**Features:**
- ✅ Simple setup for local development
- ✅ Hot reload support (if configured)
- ✅ Easy debugging with direct port access
- ✅ No Kubernetes overhead

**Service Ports (Docker Compose):**
- UsersApi: `http://localhost:8080`
- TasksApi: `http://localhost:9080`
- RabbitMQ Management: `http://localhost:15672` (guest/guest)
- SQL Server (UsersDb): `localhost:11433`
- SQL Server (TasksDb): `localhost:21433`

---

### Comparison: Kubernetes vs Docker Compose

| Feature | Kubernetes | Docker Compose |
|---------|-----------|----------------|
| **Use Case** | Production, staging | Local development |
| **Complexity** | Higher | Lower |
| **Scalability** | Auto-scaling, replicas | Manual scaling |
| **Health Checks** | Built-in liveness/readiness | Basic healthcheck |
| **Networking** | Service discovery, ingress | Port mapping |
| **Persistence** | PersistentVolumeClaims | Named volumes |
| **Setup Time** | ~3-5 minutes | ~1-2 minutes |
| **Resource Usage** | Higher (K8s overhead) | Lower |

---


### API Documentation

**Base URLs:**
- Kubernetes: `http://taskflow.com`
- Docker Compose: `http://localhost:8080` (UsersApi), `http://localhost:9080` (TasksApi)

**Register User:**
```bash
# Kubernetes
curl -X POST http://taskflow.com/api/users/Register \
  -H "Content-Type: application/json" \
  -d '{"username":"john","password":"Pass@123","fullName":"John Doe"}'

# Docker Compose
curl -X POST http://localhost:8080/api/users/Register \
  -H "Content-Type: application/json" \
  -d '{"username":"john","password":"Pass@123","fullName":"John Doe"}'
```

**Login:**
```bash
# Kubernetes
curl -X POST http://taskflow.com/api/users/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"john","password":"Pass@123"}'

# Docker Compose
curl -X POST http://localhost:8080/api/users/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"john","password":"Pass@123"}'
```

**Create Task:**
```bash
# Kubernetes
curl -X POST http://taskflow.com/api/tasks/CreateTask \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"title":"My Task","description":"Task description","assignedUserId":1}'

# Docker Compose
curl -X POST http://localhost:9080/api/tasks/CreateTask \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"title":"My Task","description":"Task description","assignedUserId":1}'
```

### Monitoring

**Kubernetes:**
```bash
# View all pods
kubectl get pods -n taskflow

# UsersApi logs
kubectl logs -f deployment/usersapi-depl -n taskflow

# TasksApi logs
kubectl logs -f deployment/tasksapi-depl -n taskflow

# Notification logs
kubectl logs -f deployment/notification-depl -n taskflow

# RabbitMQ logs
kubectl logs -f deployment/rabbitmq-depl -n taskflow
```

**Docker Compose:**
```bash
# View all containers
docker-compose ps

# UsersApi logs
docker-compose logs -f usersapi

# TasksApi logs
docker-compose logs -f tasksapi

# Notification logs
docker-compose logs -f notificationservice

# RabbitMQ logs
docker-compose logs -f rabbitmq

# All logs
docker-compose logs -f
```

### Troubleshooting

**Kubernetes:**

**Pods not starting:**
```bash
kubectl describe pod <pod-name> -n taskflow
kubectl get events -n taskflow --sort-by='.lastTimestamp'
```

**Database connection issues:**
```bash
kubectl logs deployment/usersapi-mssql-depl -n taskflow
kubectl logs deployment/tasksapi-mssql-depl -n taskflow
```

**RabbitMQ connection issues:**
```bash
kubectl logs deployment/rabbitmq-depl -n taskflow
```

**Restart a deployment:**
```bash
kubectl rollout restart deployment/usersapi-depl -n taskflow
```

---

**Docker Compose:**

**Container not starting:**
```bash
docker-compose ps
docker-compose logs <service-name>
```

**Database connection issues:**
```bash
docker-compose logs usersapi-mssql
docker-compose logs tasksapi-mssql
```

**RabbitMQ connection issues:**
```bash
docker-compose logs rabbitmq
```

**Restart a service:**
```bash
docker-compose restart usersapi
```

**Rebuild and restart:**
```bash
docker-compose up -d --build
```

---

## CI/CD Pipeline

The project uses **GitHub Actions** for continuous integration and deployment:

**Workflow:** `.github/workflows/ci-cd.yml`

**Triggers:**
- Push to `master` branch
- Pull requests to `master` branch

**Pipeline Steps:**
1. Checkout code
2. Build Docker images for all services
3. Push images to Docker Hub
4. (Optional) Deploy to Kubernetes cluster

**Docker Hub Images:**
- `ahmedelmasry1985/usersapi:latest`
- `ahmedelmasry1985/tasksapi:latest`
- `ahmedelmasry1985/notificationsrv:latest`

---

## Project Structure

```
TaskFlow/
├── .github/
│   └── workflows/
│       └── ci-cd.yml          # GitHub Actions pipeline
├── k8s/                       # Kubernetes manifests
│   ├── namespace.yaml
│   ├── usersapi-depl.yaml
│   ├── usersapi-mssql-depl.yaml
│   ├── tasksapi-depl.yaml
│   ├── tasksapi-mssql-depl.yaml
│   ├── notification-depl.yaml
│   ├── rabbitmq-depl.yaml
│   └── ingress-depl.yaml
├── src/                       # Source code
│   ├── Core/                  # Shared libraries
│   ├── Core.JwtAuth/          # JWT authentication
│   ├── UsersApi/              # User service
│   ├── TasksApi/              # Task service
│   ├── NotificationService/   # Notification worker
│   └── docker-compose.yml     # Docker Compose config
├── k8s-redeploy.sh           # K8s deployment script
├── k8s-cleanup.sh            # K8s cleanup script
└── README.md                 # This file
```

---

## Learning Outcomes

This project demonstrates:

✅ **Microservices Architecture** - Service decomposition and boundaries  
✅ **Inter-Service Communication** - gRPC (sync) and RabbitMQ (async)  
✅ **Database-per-Service Pattern** - Isolated data stores  
✅ **API Gateway Pattern** - Nginx ingress routing  
✅ **Event-Driven Architecture** - Pub/sub with message broker  
✅ **Containerization** - Docker multi-stage builds  
✅ **Orchestration** - Kubernetes deployments, services, ingress  
✅ **CI/CD** - Automated builds and deployments  
✅ **Authentication** - JWT tokens and middleware  
✅ **Health Checks** - Liveness and readiness probes  
✅ **Resource Management** - CPU/memory limits and requests  

---

## Future Enhancements

- [ ] Add Swagger/OpenAPI documentation
- [ ] Implement distributed tracing (Jaeger/Zipkin)
- [ ] Add centralized logging (ELK stack)
- [ ] Implement circuit breaker pattern (Polly)
- [ ] Add Redis caching layer
- [ ] Implement CQRS pattern
- [ ] Add integration tests
- [ ] Set up monitoring (Prometheus + Grafana)
- [ ] Implement rate limiting
- [ ] Add email/SMS notifications

---

## License

This project is for educational purposes based on Les Jackson's .NET Microservices course.
