# Instruções para Deploy no Kubernetes

## Pré-requisitos
- Cluster Kubernetes configurado e acessível via `kubectl`.
- Imagens Docker para backend e frontend construídas e disponíveis no registry (ex.: Docker Hub ou registry local).
  - Backend: `apontamento-api:latest`
  - Frontend: `apontamento-frontend:latest`
- Storage class configurada para PersistentVolume (para MongoDB).

## Passos para Deploy
1. **Aplicar MongoDB**:
   ```
   kubectl apply -f mongo.yaml
   ```

2. **Aplicar Backend**:
   ```
   kubectl apply -f backend.yaml
   ```

3. **Aplicar Frontend**:
   ```
   kubectl apply -f frontend.yaml
   ```

## Acesso na Intranet
- **Frontend**: Acesse via o IP externo do LoadBalancer do `frontend-service` (porta 80).
- **Backend**: Acesse via o IP externo do LoadBalancer do `backend-service` (porta 80), ou internamente via `backend-service`.
- **MongoDB**: Acessível internamente via `mongo-service:27017`.

## Notas
- Ajuste as imagens Docker conforme necessário.
- Para produção, considere usar secrets para senhas em vez de ConfigMaps.
- Monitore os pods com `kubectl get pods` e logs com `kubectl logs`.
- Se o cluster não suportar LoadBalancer, altere para NodePort e exponha via ingress ou proxy reverso.