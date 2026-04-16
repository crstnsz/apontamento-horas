# Instruções para Deploy

## Opção 1: Docker Compose (Recomendado para Desenvolvimento Local)

### Pré-requisitos
- Docker e Docker Compose instalados.
- Portas 3000, 8080 e 27017 livres no host.

### Passos para Deploy
1. **Navegar para o diretório devops**:
   ```
   cd devops
   ```

2. **Subir os serviços**:
   ```
   docker-compose up -d
   ```

3. **Verificar status**:
   ```
   docker-compose ps
   ```

4. **Ver logs (opcional)**:
   ```
   docker-compose logs -f
   ```

### Acesso na Intranet
- **Frontend**: `http://<ip-da-máquina>:3000`
- **Backend**: `http://<ip-da-máquina>:8080`
- **MongoDB**: Interno no container, acessível via `mongo:27017` dentro da rede Docker.

### Comandos Úteis
- Parar serviços: `docker-compose down`
- Reconstruir imagens: `docker-compose up --build -d`
- Limpar volumes: `docker-compose down -v`

---

## Opção 2: Kubernetes (Para Ambientes de Produção ou Orquestração)

### Pré-requisitos
- Cluster Kubernetes configurado e acessível via `kubectl`.
- Imagens Docker para backend e frontend construídas e disponíveis no registry (ex.: Docker Hub ou registry local).
  - Backend: `apontamento-api:latest`
  - Frontend: `apontamento-frontend:latest`
- Storage class configurada para PersistentVolume (para MongoDB).

### Passos para Deploy
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

### Acesso na Intranet
- **Frontend**: Acesse via o IP externo do LoadBalancer do `frontend-service` (porta 80).
- **Backend**: Acesse via o IP externo do LoadBalancer do `backend-service` (porta 80), ou internamente via `backend-service`.
- **MongoDB**: Acessível internamente via `mongo-service:27017`.

### Notas
- Ajuste as imagens Docker conforme necessário.
- Para produção, considere usar secrets para senhas em vez de ConfigMaps.
- Monitore os pods com `kubectl get pods` e logs com `kubectl logs`.
- Se o cluster não suportar LoadBalancer, altere para NodePort e exponha via ingress ou proxy reverso.