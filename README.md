# ProductApi (Clean Architecture, .NET 8)

Solución mínima con arquitectura limpia: Domain, Application, Infrastructure, Api y Tests. API RESTful para gestionar `Producto` con autenticación JWT, validaciones, servicio de dominio, repositorio en memoria thread-safe y Docker.

##Los puertos deben estar libres, o cambiar el puerto
## Ejecutar con Docker Modo Producción

- Construir imagen:
  ```bash
  docker build -t productapi:latest .
  ```
- Ejecutar contenedor:
  ```bash
  docker run -p 8080:8080 productapi:latest
  ```

## Ejecutar con Docker Modo Development (para visualizar el swagger)

- Construir imagen:
  ```bash
 docker build -t productapi:dev .
  ```
- Ejecutar contenedor:
  ```bash
  docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development productapi:dev
  ```
## deberia abrir la Url http://localhost:8080/swagger
  
- Swagger UI: http://localhost:8080/swagger
- Autenticación: POST `/api/auth/login` con cuerpo `{ "username": "admin", "password": "Password123!" }` y usar `Bearer <token>`.

## Collection Postman - solo seria reemplazar el token y correr 

https://.postman.co/workspace/Personal-Workspace~5a5e6972-ac8f-462e-b52a-a978c28aaeca/collection/8336714-66679b93-8662-46c4-a7f7-379115f141bc?action=share&creator=8336714

## Endpoints

- GET `/api/productos`
- GET `/api/productos/{id}`
- POST `/api/productos`
- PUT `/api/productos/{id}`
- DELETE `/api/productos/{id}`
- GET `/api/productos/filter?categoria={cat}&preciomin={min}`

## Respuestas solicitadas (breves)

1. ¿Ventajas de microservicios vs monolito?
   - Despliegue independiente por servicio.
   - Escalado selectivo de componentes críticos.
   - Aislamiento de fallos y límites claros de dominio.
   - Ciclos de entrega más cortos.

2. ¿CI/CD en este proyecto?
   - Pipeline con: build + tests + análisis estático + publicación de imagen Docker.
   - Versionado semántico y push a registry (e.g., GHCR/ACR).
   - Despliegue automático a dev/staging y aprobación manual a prod.

3. ¿Mantenibilidad y rendimiento a largo plazo?
   - Observabilidad (logs estructurados, métricas, tracing), pruebas y coverage.
   - Contratos estables (DTOs), validaciones y límites claros (Domain/Application).
   - Caching donde aplique, paginación, y profiling continuo.
   - Hardening (JWT rotation, secretos en vault) y benchmarks periódicos.

4. (Opcional) .NET MAUI
   - No he tenido la oportunidad todavía.

## Estructura

- `src/Domain`: Entidades de dominio.
- `src/Application`: DTOs, interfaces y servicios (reglas/validaciones).
- `src/Infrastructure`: Repositorios e implementación (in-memory concurrente).
- `src/Api`: ASP.NET Core Web API (JWT, Controllers, Swagger, middleware errores).
- `tests/Tests`: xUnit pruebas de servicio.

## Notas

- Repositorio en memoria usando `ConcurrentDictionary` para concurrencia.
- Validaciones: nombre no vacío, precio > 0, stock >= 0, categoría no vacía.
- Códigos HTTP: 200/201/204, 400 para validaciones, 404 cuando no existe, 500 genérico.
