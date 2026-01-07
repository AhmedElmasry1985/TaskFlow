#!/bin/bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml
kubectl apply -f k8s/ingress-depl.yaml
kubectl apply -f k8s/rabbitmq-depl.yaml
kubectl apply -f k8s/usersapi-mssql-depl.yaml
kubectl apply -f k8s/tasksapi-mssql-depl.yaml
kubectl apply -f k8s/usersapi-depl.yaml
kubectl apply -f k8s/tasksapi-depl.yaml
kubectl apply -f k8s/notification-depl.yaml
echo "⚠️ commands may not sync the platforms at its startup, which means you may need to restart it manually!"