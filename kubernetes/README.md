# Kubernetes - Configuração Inicial

Este diretório contém as configurações necessárias para executar a aplicação em um cluster Kubernetes usando [Kind](https://kind.sigs.k8s.io/).

## Pré-requisitos

- Docker instalado
- Kind instalado (`kind`)
- kubectl instalado
- Acesso a permissões de administrador para criar clusters

## Passos de Configuração

### 1. Criar o Cluster Kind

Execute o comando abaixo para criar um cluster Kubernetes local com a configuração definida:

```bash
kind create cluster --name dev --config kubernetes/kind-config.yaml
```

Este comando irá:
- Criar um cluster Kind chamado `dev`
- Configurar os nodes conforme especificado em `kind-config.yaml`
- Preparar o ambiente para deployment

### 2. Rotular os Nodes

Após a criação do cluster, é necessário rotular os nodes para direcionar os deployments corretos:

#### Marcar o node de Banco de Dados

```bash
kubectl label node dev-worker role=db
```

Este node será responsável pelo PostgreSQL e outras dependências de armazenamento.

#### Marcar o node de Aplicação

```bash
kubectl label node dev-worker2 role=app
```

Este node executará os serviços da aplicação .NET.

### 3. Aplicar os Manifests Kubernetes

Aplique todos os arquivos YAML de configuração:

```bash
kubectl apply -f kubernetes/infra/
kubectl apply -f kubernetes/backend/
```

Estes comandos irão:
- Criar os deployments de infraestrutura (PostgreSQL, Redis)
- Configurar os serviços necessários
- Iniciar os containers da aplicação

## Estrutura de Diretórios

```
kubernetes/
├── kind-config.yaml          # Configuração do cluster Kind
├── infra/                    # Serviços de infraestrutura
│   ├── postgres-deployment.yaml
│   ├── postgres-service.yaml
│   ├── redis-deployment.yaml
│   └── redis-service.yaml
└── backend/                  # Configuração da aplicação
```

## Detalhes do Redis

O Redis é um cache em memória de alta performance utilizado pela aplicação para melhorar a velocidade de acesso a dados.

### Configuração

- **Imagem**: `redis:latest`
- **Porta**: `6379` (padrão do Redis)
- **Recursos Solicitados**:
  - CPU: 100m
  - Memória: 128Mi
- **Limites de Recursos**:
  - CPU: 250m
  - Memória: 256Mi

### Funcionalidades Principais

- **In-Memory Data Store**: Armazenamento ultra-rápido de dados em memória
- **Cache Distribuído**: Reduz carga do banco de dados principal
- **Data Structures**: Suporta strings, lists, sets, sorted sets, hashes
- **Persistência Opcional**: Pode ser configurado com RDB ou AOF

### Performance

- Latência < 1ms para operações
- Throughput de até 100k+ operações por segundo
- Ideal para sessões, cache de consultas e dados temporários

### Acessar o Redis

Para conectar-se ao Redis dentro do cluster:

```bash
kubectl exec -it <redis-pod-name> -- redis-cli
```

Para portar o Redis localmente (se necessário):

```bash
kubectl port-forward svc/redis 6379:6379
```

## Verificar o Status dos Pods

Para verificar se todos os serviços estão rodando corretamente:

```bash
kubectl get pods -o wide
```

Para ver logs de um serviço específico:

```bash
kubectl logs -f deployment/<nome-do-deployment>
```

## Limpeza

Para remover o cluster Kind:

```bash
kind delete cluster --name dev
```

## Troubleshooting

### Pod não inicia
- Verifique os logs: `kubectl logs <pod-name>`
- Confirme se os labels dos nodes estão corretos: `kubectl get nodes --show-labels`

### Serviço não é acessível
- Verifique o serviço: `kubectl get svc`
- Teste a conectividade entre pods: `kubectl exec -it <pod> -- sh`

### Problemas de recursos
- Verifique a disponibilidade: `kubectl top nodes`
- Ajuste os limites em `resources.limits` nos arquivos YAML
