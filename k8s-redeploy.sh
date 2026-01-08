#!/bin/bash

echo "🚀 Deploying TaskFlow to Kubernetes..."

# 1. Create namespace
echo "📦 Creating namespace..."
kubectl apply -f k8s/namespace.yaml

# 2. Check if nginx ingress controller exists
if ! kubectl get namespace ingress-nginx &> /dev/null; then
    echo "📥 Installing nginx ingress controller..."
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml
    
    echo "⏳ Waiting for ingress controller to be ready..."
    kubectl wait --namespace ingress-nginx \
      --for=condition=ready pod \
      --selector=app.kubernetes.io/component=controller \
      --timeout=180s
else
    echo "✅ Nginx ingress controller already installed"
fi

# 3. Deploy infrastructure (SQL + RabbitMQ)
echo "🗄️  Deploying infrastructure..."
kubectl apply -f k8s/usersapi-mssql-depl.yaml
kubectl apply -f k8s/tasksapi-mssql-depl.yaml
kubectl apply -f k8s/rabbitmq-depl.yaml

echo "⏳ Waiting for infrastructure to be ready..."
kubectl wait --for=condition=ready pod -l app=usersapi-mssql -n taskflow --timeout=120s
kubectl wait --for=condition=ready pod -l app=tasksapi-mssql -n taskflow --timeout=120s
kubectl wait --for=condition=ready pod -l app=rabbitmq -n taskflow --timeout=120s

# 4. Deploy APIs
echo "🌐 Deploying APIs..."
kubectl apply -f k8s/usersapi-depl.yaml
kubectl apply -f k8s/tasksapi-depl.yaml
kubectl apply -f k8s/notification-depl.yaml

echo "⏳ Waiting for APIs to be ready..."
kubectl wait --for=condition=ready pod -l app=usersapi -n taskflow --timeout=120s
kubectl wait --for=condition=ready pod -l app=tasksapi -n taskflow --timeout=120s
kubectl wait --for=condition=ready pod -l app=notification -n taskflow --timeout=120s

# 5. Deploy ingress
echo "🌍 Deploying ingress..."
kubectl apply -f k8s/ingress-depl.yaml

echo ""
echo "✅ Deployment complete!"
echo ""
echo "📊 Check status:"
echo "   kubectl get all -n taskflow"
echo ""
echo "📝 View logs:"
echo "   kubectl logs -f deployment/usersapi-depl -n taskflow"
echo "   kubectl logs -f deployment/tasksapi-depl -n taskflow"
echo "   kubectl logs -f deployment/notification-depl -n taskflow"
echo ""
echo "⚠️  Note: If services fail to start, check RabbitMQ/SQL Server readiness"
echo "   and restart pods manually: kubectl rollout restart deployment/<name> -n taskflow"