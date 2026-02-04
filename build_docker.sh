#!/bin/bash
NAMESPACE="${1:-codebase_b1806_app}"
docker build -t "$NAMESPACE" .