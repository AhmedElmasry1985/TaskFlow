#!binbash
kubectl delete deployment -n taskflow  notification-depl tasksapi-depl usersapi-depl tasksapi-mssql-depl usersapi-mssql-depl rabbitmq-depl 
kubectl delete service -n taskflow usersapi-clusterip-svc tasksapi-clusterip-svc usersapi-mssql-clusterip-srv tasksapi-mssql-clusterip-srv rabbitmq-clusterip-srv
kubectl delete ingress  -n taskflow ingress-depl
kubectl delete pvc -n taskflow tasksapi-mssql-pvc usersapi-mssql-pvc
kubectl.exe delete namespaces taskflow
