#!/bin/bash

echo "⚠️  WARNING: This will delete the entire TaskFlow namespace!"
read -p "Are you sure? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Cleanup cancelled."
    exit 0
fi

echo "🗑️  Deleting namespace 'taskflow'..."
kubectl delete namespace taskflow

echo "✅ Cleanup complete! All resources removed."