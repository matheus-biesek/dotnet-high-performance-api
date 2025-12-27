# API Controllers

Este documento descreve os controllers presentes na aplicação, os dados que recebem e os dados que retornam, com exemplos JSON e rotas.

**Controllers**
- **OrdersController (v1)**: `/v{version}/orders` (ex.: `/v1/orders`)
- **ProductsController (v1)**: `/v{version}/products` (ex.: `/v1/products`)
- **SlowProductsController (v1)**: `/v{version}/slow/products` (ex.: `/v1/slow/products`) — controller demonstrativo com más práticas.

**Modelos principais (DTOs)**
- `CreateOrderDto`: { "productIds": [int, ...] }
- `ProductDto`: { "id": int, "name": string, "price": decimal }
- `OrderDto`: { "id": int, "createdAt": string (ISO 8601), "total": decimal, "products": [ProductDto, ...] }

---

**OrdersController (v1)**
- Route base: `/v{version}/orders` (ex.: `/v1/orders`)

Endpoints:

- Criar pedido
  - Método: POST
  - URL: `/v1/orders`
  - Body (JSON):

```json
{
  "productIds": [1, 2, 3]
}
```

  - Respostas:
    - `201 Created` — corpo: id do pedido criado (ex.: `123`)
    - `400/422` — validação (se houver)

- Obter pedido por id
  - Método: GET
  - URL: `/v1/orders/{id}` (ex.: `/v1/orders/123`)
  - Headers:
    - `If-None-Match` (opcional): ETag da resposta anterior para validação de cache
  - Respostas:
    - `200 OK` — body (OrderDto) + header `ETag`:

```json
{
  "id": 123,
  "createdAt": "2025-12-27T12:34:56Z",
  "total": 59.97,
  "products": [
    { "id": 1, "name": "Produto A", "price": 19.99 },
    { "id": 2, "name": "Produto B", "price": 39.98 }
  ]
}
```

    - `304 Not Modified` — se `If-None-Match` coincidir com o `ETag` atual (cache intacto, sem corpo)
    - `404 Not Found` — se o pedido não existir

  - Exemplo com ETag:

```bash
# Primeira requisição (obtém ETag)
curl -i http://localhost:5000/v1/orders/123
# Resposta: 200 OK
# Headers: ETag: W/"hash_base64_nao_expoe_tabela"

# Segunda requisição com If-None-Match
curl -i -H 'If-None-Match: W/"hash_base64_nao_expoe_tabela"' http://localhost:5000/v1/orders/123
# Resposta: 304 Not Modified (sem corpo, apenas headers)
```

Caching: O endpoint usa cache distribuído (Redis) via `ICacheService` com TTL de 10 minutos. ETags são gerados baseado no `id` e `version` da entidade.

- Atualizar pedido (Optimistic Locking)
  - Método: PUT
  - URL: `/v1/orders/{id}` (ex.: `/v1/orders/123`)
  - Headers:
    - `If-Match` (obrigatório): ETag da versão atual para validação de conflito
  - Body (JSON):

```json
{
  "version": 1
}
```

  - Respostas:
    - `200 OK` — corpo: `OrderDto` atualizado com novo `ETag` no header
    - `412 Precondition Failed` — se `If-Match` não coincidir com a versão atual (conflito de concorrência)
    - `404 Not Found` — se o pedido não existir
    - `400 Bad Request` — se `If-Match` header estiver ausente

  - Exemplo com If-Match:

```bash
# 1. Obter pedido com ETag
curl -i http://localhost:5000/v1/orders/123
# Resposta: 200 OK
# Headers: ETag: W/"hash_v1"
# Body: { "id": 123, "version": 1, ... }

# 2. Atualizar com If-Match (usando ETag como validação)
curl -i -X PUT \
  -H 'If-Match: W/"hash_v1"' \
  -H "Content-Type: application/json" \
  -d '{"version": 1}' \
  http://localhost:5000/v1/orders/123
# Resposta: 200 OK (sucesso)
# Headers: ETag: W/"hash_v2" (nova versão)

# 3. Tentar atualizar novamente com ETag antigo (conflito)
curl -i -X PUT \
  -H 'If-Match: W/"hash_v1"' \
  -H "Content-Type: application/json" \
  -d '{"version": 1}' \
  http://localhost:5000/v1/orders/123
# Resposta: 412 Precondition Failed (conflito detectado)
```

---

**ProductsController (v1)**
- Route base: `/v{version}/products` (ex.: `/v1/products`)

Endpoints:

- Listar produtos
  - Método: GET
  - URL: `/v1/products`
  - Resposta (`200 OK`): array de `ProductDto`:

```json
[
  { "id": 1, "name": "Produto A", "price": 19.99 },
  { "id": 2, "name": "Produto B", "price": 39.98 }
]
```

- Obter produto por id
  - Método: GET
  - URL: `/v1/products/{id}` (ex.: `/v1/products/1`)
  - Headers:
    - `If-None-Match` (opcional): ETag da resposta anterior para validação de cache
  - Respostas:
    - `200 OK` — `ProductDto` + header `ETag`:

```json
{ "id": 1, "name": "Produto A", "price": 19.99 }
```

    - `304 Not Modified` — se `If-None-Match` coincidir com o `ETag` atual (cache intacto)
    - `404 Not Found` — se não existir

  - Exemplo com ETag:

```bash
# Primeira requisição (obtém ETag)
curl -i http://localhost:5000/v1/products/1
# Resposta: 200 OK
# Headers: ETag: W/"hash_base64_nao_expoe_tabela"

# Segunda requisição com If-None-Match
curl -i -H 'If-None-Match: W/"hash_base64_nao_expoe_tabela"' http://localhost:5000/v1/products/1
# Resposta: 304 Not Modified (sem corpo, apenas headers)
```

Caching: esses endpoints usam cache distribuído (Redis) via `ICacheService` com TTL de 5 minutos. ETags são gerados baseado no `id` e `version` da entidade, permitindo validação de cache sem expor informações internas.

- Atualizar produto (Optimistic Locking)
  - Método: PUT
  - URL: `/v1/products/{id}` (ex.: `/v1/products/1`)
  - Headers:
    - `If-Match` (obrigatório): ETag da versão atual para validação de conflito
  - Body (JSON):

```json
{
  "name": "Produto A Atualizado",
  "price": 24.99,
  "version": 1
}
```

  - Respostas:
    - `200 OK` — corpo: `ProductDto` atualizado com novo `ETag` no header
    - `412 Precondition Failed` — se `If-Match` não coincidir com a versão atual (conflito de concorrência)
    - `404 Not Found` — se o produto não existir
    - `400 Bad Request` — se `If-Match` header estiver ausente

  - Exemplo com If-Match:

```bash
# 1. Obter produto com ETag
curl -i http://localhost:5000/v1/products/1
# Resposta: 200 OK
# Headers: ETag: W/"hash_v1"
# Body: { "id": 1, "name": "Produto A", "price": 19.99, ... }

# 2. Atualizar com If-Match (usando ETag como validação)
curl -i -X PUT \
  -H 'If-Match: W/"hash_v1"' \
  -H "Content-Type: application/json" \
  -d '{"name":"Produto A Atualizado", "price": 24.99, "version": 1}' \
  http://localhost:5000/v1/products/1
# Resposta: 200 OK (sucesso)
# Headers: ETag: W/"hash_v2" (nova versão)

# 3. Tentar atualizar novamente com ETag antigo (conflito)
curl -i -X PUT \
  -H 'If-Match: W/"hash_v1"' \
  -H "Content-Type: application/json" \
  -d '{"name":"Produto Novo", "price": 29.99, "version": 1}' \
  http://localhost:5000/v1/products/1
# Resposta: 412 Precondition Failed (conflito detectado)
```

---

**SlowProductsController (v1)**
- Route base: `/v{version}/slow/products` (ex.: `/v1/slow/products`)
- Este controller é um exemplo didático de más práticas (sincronia, N+1, sem cache, sem paginação). Evite usá-lo em produção.

Endpoints (idem ao `ProductsController`):

- `GET /v1/slow/products` — retorna lista de `ProductDto` (implementação síncrona e sem cache).
- `GET /v1/slow/products/{id}` — retorna `ProductDto` ou `404`.

---

Observações finais
- Todos os controllers estão versionados com API Versioning (`v1`).
- **Suporte a ETag (If-None-Match)**: Os endpoints `GET` retornam um header `ETag` baseado no hash SHA256 do `id` + `version`. Clientes podem usar `If-None-Match` para validar se a entidade foi modificada:
  - Se ETag for igual: retorna `304 Not Modified` (sem corpo).
  - Se ETag for diferente ou não existir: retorna `200 OK` com novo corpo e novo `ETag`.
  - ETags são formatados como `W/"base64_hash"` (weak ETag) e não expõem nomes de tabelas ou informações internas.
- **Otimistic Locking (If-Match)**: Os endpoints `PUT` implementam optimistic locking usando a coluna `Version`:
  - Cliente obtém a versão atual via `GET` (no header `ETag`).
  - Cliente envia a versão esperada no body do `PUT` (campo `version`).
  - Se a versão não coincidir: retorna `412 Precondition Failed` (conflito de concorrência detectado).
  - Se coincidir: atualiza o recurso e incrementa `Version`, retorna `200 OK` com novo `ETag`.
  - Este padrão previne o problema do "lost update" em cenários de concorrência.
- **Database Indexes**: Coluna `version` nas tabelas `products` e `orders` possui índices para otimizar consultas.
- Exemplo de chamada cURL para criar um pedido:

```bash
curl -X POST http://localhost:5000/v1/orders \
  -H "Content-Type: application/json" \
  -d '{"productIds":[1,2]}'
```

Exemplo para obter produtos (versão v1):

```bash
curl http://localhost:5000/v1/products
```
