# Testes End-to-End (E2E)

Este diretório contém os testes de ponta a ponta para a API. Estes testes validam o fluxo completo da aplicação, conectando-se a uma instância real do banco de dados e outros serviços de infraestrutura.

## 🛠 Pré-requisitos

Como os testes rodam contra serviços reais, você precisa ter a infraestrutura rodando via Docker.

1. **Docker**: Certifique-se de que o Docker e o Docker Compose estão instalados e rodando.
2. **Infraestrutura**: Na raiz do repositório (onde está o `docker-compose.yml`), execute:

   ```bash
   docker-compose up -d
   ```

   Isso subirá:
   - **PostgreSQL**: Porta 5432
   - **Redis**: Porta 6379

## 🚀 Como Executar

Para rodar todos os testes E2E, execute o seguinte comando na raiz da solução ou dentro deste diretório:

```bash
dotnet test
```

## 🏗 Estrutura

- **WebApplicationFactory**: Utilizamos a `WebApplicationFactory` para levantar a API em memória, mas configurada para usar os serviços reais (Postgres e Redis) definidos no `appsettings.Development.json` ou variáveis de ambiente.
- **Banco de Dados**: A aplicação está configurada (em modo Development) para criar o banco e semear dados iniciais automaticamente ao iniciar.

## 📦 Tecnologias

- **xUnit**: Framework de testes.
- **FluentAssertions**: Para asserções mais legíveis.
- **Microsoft.AspNetCore.Mvc.Testing**: Para testes de integração com ASP.NET Core.
