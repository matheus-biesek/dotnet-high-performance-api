# Kubernetes - Configuração Inicial (Kind)

Este guia explica como configurar um cluster Kubernetes local usando Kind, com a aplicação .NET, PostgreSQL e Redis.

## Pré-requisitos

- Docker instalado
- Kind instalado (`kind`)
- kubectl instalado

---

## 1. Configuração do Cluster Kubernetes

### 1.1 Visão geral da topologia

O cluster será criado a partir do arquivo `kubernetes/kind-config.yaml` com:

- **1 control-plane** (gerenciador do cluster)
- **2 workers** (executam os pods)

Nomes dos nodes esperados após criação:
- `dev-control-plane`
- `dev-worker` (node de banco de dados)
- `dev-worker2` (node de aplicação)

### 1.2 Configuração de Nodes e Pods

**Mapeamento de serviços/pods para nodes:**

| Serviço | Deployment | Réplicas | Node | Label |
|---------|-----------|----------|------|-------|
| PostgreSQL | postgres | 1 | `dev-worker` | `role=db` |
| Redis | redis | 1 | `dev-worker` | `role=db` |
| Backend (.NET) | backend | 2 | `dev-worker2` | `role=app` |

Cada serviço será agendado (scheduled) no node correto usando `nodeSelector` e labels.

---

## 2. Passo a Passo

### 2.1 Criar o Cluster Kind

Execute o comando para criar o cluster com os nodes:

```bash
kind create cluster --name dev --config kubernetes/kind-config.yaml
```

Verifique os nodes criados:

```bash
kubectl get nodes -o wide
```

---

## 2.2 Adicione labels para o igress

```
kubectl label node dev-control-plane ingress-ready=true
kubectl label node dev-worker ingress-ready=true
kubectl label node dev-worker2 ingress-ready=true
kubectl label node dev-worker3 ingress-ready=true
```

## 2.3 Instale o Ingess-nginx no Cluster

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.0/deploy/static/provider/kind/deploy.yaml
``` 

## 2.4 Ajusta sua maquina linux para responder um host corretamente

```
sudo nano /etc/hosts
```

### 2.4 Adicione

```
127.0.0.1 backend.local
```

---

## 3. Construir a Imagem Docker

### 3.1 Build da Imagem

A partir da raiz do repositório, execute:

```bash
docker build -f src/DotNetHighPerformanceApi.Api/Dockerfile -t dotnethighperfapi:latest .
```

Verifique que a imagem foi criada:

```bash
docker images | grep dotnethighperfapi
```

---

## 4. Carregar a Imagem para o Cluster Kind

Como estamos usando Kind localmente, precisamos disponibilizar a imagem ao cluster:

```bash
kind load docker-image dotnethighperfapi:latest --name dev
```

Isso copia a imagem Docker do host para dentro do cluster Kind, tornando-a disponível para os pods.

> **Nota:** Se você usar um registry remoto, pule este passo e use `docker push` + atualize o YAML do deployment com a URL da imagem.

---

## 5. Aplicar Deployments e Services

### 5.1 Estrutura de Arquivos

```
kubernetes/
├── kind-config.yaml              # Configuração do cluster
├── infra/                        # Serviços de infraestrutura
│   ├── postgres-deployment.yaml
│   ├── postgres-service.yaml
│   ├── redis-deployment.yaml
│   └── redis-service.yaml
└── backend/                      # Aplicação
    ├── asp-deployment.yaml
    └── asp-service.yaml
```

### 5.2 Aplicar os Manifests

Execute os comandos para criar todos os serviços:

**Infraestrutura (PostgreSQL + Redis):**

```bash
kubectl apply -f kubernetes/infra/
```

**Backend (Aplicação .NET):**

```bash
kubectl apply -f kubernetes/backend/
```

---

## 6. Verificar Status

### 6.1 Listar Pods e Nodes

Ver todos os pods com o node onde estão rodando:

```bash
kubectl get pods -o wide
```

Ver os nodes com seus labels:

```bash
kubectl get nodes --show-labels
```

### 6.2 Verificar Deployments

Ver detalhes dos deployments (réplicas desejadas/atuais):

```bash
kubectl get deploy -o wide
kubectl describe deploy backend
```

### 6.3 Ver Logs

Logs de um deployment específico:

```bash
kubectl logs -f deployment/backend
```

Logs de um pod específico:

```bash
kubectl logs <pod-name>
```

---

## 7. Acessar Serviços

### 7.1 PostgreSQL

Conectar ao PostgreSQL:

```bash
kubectl exec -it $(kubectl get pods -l app=postgres -o jsonpath='{.items[0].metadata.name}') -- psql -U postgres -d HighPerformanceApiDb
```

### 7.2 Redis

Acessar Redis CLI:

```bash
kubectl exec -it $(kubectl get pods -l app=redis -o jsonpath='{.items[0].metadata.name}') -- redis-cli
```

### 7.3 API Backend

Testar a API via port-forward:

```bash
# Redirecionar porta local 5000 para o serviço backend
kubectl port-forward svc/backend 5000:80
```

Em outro terminal:

```bash
# Testar health check
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

---

## 8.1 Reiniciando os pods do Deployment
```
kubectl rollout restart deployment/backend
```

## 8.2 Limpeza

Remover o cluster Kind:

```bash
kind delete cluster --name dev
```

---

## 9. Troubleshooting

| Problema | Solução |
|----------|---------|
| Pod não inicia | `kubectl logs <pod-name>` ou `kubectl describe pod <pod-name>` |
| Image not found | Execute `kind load docker-image <image>:<tag> --name dev` |
| Serviço não é acessível | Verifique labels: `kubectl get nodes --show-labels` e `kubectl get pods --show-labels` |
| Recurso insuficiente | `kubectl top nodes` para ver uso de CPU/memória |
| Pod agendado no node errado | Verifique `nodeSelector` no YAML e labels do node |

---

## Referências Rápidas

| Comando | Descrição |
|---------|-----------|
| `kubectl get nodes` | Lista nodes do cluster |
| `kubectl get pods -A` | Lista todos os pods em todos os namespaces |
| `kubectl get svc` | Lista serviços |
| `kubectl describe pod <name>` | Detalhes completos de um pod |
| `kubectl delete pod <name>` | Remove um pod (será recriado) |
| `kubectl apply -f <file>` | Aplica um manifest YAML |
| `kubectl delete -f <file>` | Remove recursos de um manifest |


---

## Adicione labels para o igress

kubectl label node dev-control-plane ingress-ready=true
kubectl label node dev-worker ingress-ready=true
kubectl label node dev-worker2 ingress-ready=true
kubectl label node dev-worker3 ingress-ready=true


## Instale o Ingess-nginx no Cluster

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.0/deploy/static/provider/kind/deploy.yaml

``` 

## Ajusta sua maquina linux para responder um host corretamente

```
sudo nano /etc/hosts
```

### adicione

```
127.0.0.1 backend.local
```