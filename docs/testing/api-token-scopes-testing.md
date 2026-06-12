# API Token Scopes — Manual Test Checklist

## Setup
- Have one **user** account and one **admin** account ready
- Use `X-Api-Token: <plaintext>` header for API token requests
- Use `Authorization: Bearer <jwt>` for JWT requests

---

## Token Creation

### User role
- [ ] Create token with no `Scopes` → token stores all user scopes (`Reminders`, `TodoItems`, `UserStatistics`, `MoodRecords`)
- [ ] Create token with a valid subset of user scopes → only those scopes stored
- [ ] Create token with an admin scope (e.g. `Cache`) → returns 400 `ApiToken.InvalidScope`

### Admin role
- [ ] Create token with no `Scopes` → token stores all admin scopes (`Cache`, `Commands`, …)
- [ ] Create token with a valid subset of admin scopes → only those scopes stored
- [ ] Create token with a user scope (e.g. `Reminders`) → returns 400 `ApiToken.InvalidScope`

### Response
- [ ] `GET /api/api-tokens` response includes `scopes` array for each token

---

## Scope Enforcement — API Token

### User-scoped token (e.g. only `Reminders`)
- [ ] `GET /api/reminders` → 200
- [ ] `GET /api/todo-items` → 403
- [ ] `GET /api/mood-records` → 403
- [ ] `GET /api/user-statistics` → 403

### Admin-scoped token (e.g. only `Cache`)
- [ ] `POST /api/cache/prune` → 200
- [ ] `GET /api/commands` → 403

---

## JWT Auth — Scope Policies Bypassed

- [ ] JWT user: `GET /api/reminders` → 200 (scope handler bypassed)
- [ ] JWT user: `GET /api/todo-items` → 200
- [ ] JWT admin: `POST /api/cache/prune` → 200

---

## API Tokens Endpoint Always Blocked for API Token Auth

- [ ] Any request to `POST /api/api-tokens` with `X-Api-Token` → 401
- [ ] Any request to `GET /api/api-tokens` with `X-Api-Token` → 401
- [ ] Same endpoints with valid JWT → 200 / 204

---

## Backwards Compatibility

- [ ] Token created before scope changes (empty `Scopes` in DB) → all scope-protected endpoints return 403 (expected — token must be recreated)
