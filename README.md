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
  - Respostas:
    - `200 OK` — body (OrderDto):

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

    - `404 Not Found` — se o pedido não existir

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
  - Respostas:
    - `200 OK` — `ProductDto`:

```json
{ "id": 1, "name": "Produto A", "price": 19.99 }
```
    - `404 Not Found` — se não existir

Caching: esses endpoints usam cache distribuído (Redis) via `ICacheService` com TTL configurado no handler.

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

Se quiser, eu posso:
- Gerar exemplos mais completos (headers, respostas de erro),
- Atualizar `OrdersController` para versão `v1`,
- Adicionar uma seção com contratos OpenAPI/Swagger (se desejar exportar exemplos automáticos).
