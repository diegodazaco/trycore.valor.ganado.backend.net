# Trycore — API de Valor Ganado (EVM)

API REST desarrollada en **.NET 8 / C#** para la gestión de proyectos con cálculo automático de indicadores de **Earned Value Management (EVM)**. Los cálculos matemáticos son delegados a un microservicio auxiliar en **Python / FastAPI**.

---

## Tabla de contenidos

1. [Arquitectura](#arquitectura)
2. [Requisitos previos](#requisitos-previos)
3. [Estructura del repositorio](#estructura-del-repositorio)
4. [Despliegue con Docker Compose](#despliegue-con-docker-compose)
5. [Ejecución en local (desarrollo)](#ejecución-en-local-desarrollo)
6. [Variables de entorno](#variables-de-entorno)
7. [Endpoints de la API](#endpoints-de-la-api)
8. [Ejemplos de uso](#ejemplos-de-uso)
9. [Ejecución de pruebas](#ejecución-de-pruebas)

---

## Arquitectura

```
┌──────────────────────────────────────────────────┐
│                  Cliente / Frontend               │
└─────────────────────────┬────────────────────────┘
                           │ HTTP :5000
┌─────────────────────────▼────────────────────────┐
│          trycore-api  (.NET 8 / ASP.NET Core)    │
│  Controllers → Services → Repositories           │
│  Swagger UI:  /swagger-ui                        │
└──────────────┬──────────────────┬────────────────┘
               │ PostgreSQL       │ HTTP :8001
               ▼                  ▼
   ┌───────────────────┐  ┌──────────────────────┐
   │  postgres-trycore  │  │ evm-calculator-trycore│
   │  (PostgreSQL 17.5) │  │  (Python / FastAPI)   │
   └───────────────────┘  └──────────────────────┘
```

| Capa            | Proyecto                                  | Responsabilidad                                    |
|-----------------|-------------------------------------------|----------------------------------------------------|
| API             | `trycore.valor.ganado.api`                | Controladores REST, documentación Swagger          |
| Configuración   | `trycore.valor.ganado.configuration`      | Bootstrap de la aplicación (DI, middlewares)       |
| Aplicación      | `trycore.valor.ganado.application`        | DTOs, interfaces de servicios, lógica de negocio   |
| Dominio         | `trycore.valor.ganado.domain`             | Entidades y reglas del dominio                     |
| Infraestructura | `trycore.valor.ganado.infrastructure`     | EF Core, repositorios, cliente HTTP al microservicio|

---

## Requisitos previos

| Herramienta     | Versión mínima | Uso                                 |
|-----------------|---------------|--------------------------------------|
| Docker Desktop  | 24+           | Contenedores de todos los servicios  |
| Docker Compose  | v2            | Orquestación (incluido en Docker Desktop) |
| .NET SDK        | 8.0           | Solo para desarrollo local           |
| Git             | cualquiera    | Clonar el repositorio                |

---

## Estructura del repositorio

```
trycore.valor.ganado.backend.net/
├── ConfigFiles/
│   ├── docker-compose.yml       # Orquestación de los tres servicios
│   └── init.sql                 # (opcional) script SQL inicial
├── scripts/                     # Scripts de utilidad
├── services/
│   └── evm-calculator/          # Microservicio Python / FastAPI
│       ├── Dockerfile
│       ├── main.py
│       ├── calculator.py
│       └── models.py
├── src/
│   ├── trycore.valor.ganado.api/
│   │   ├── Controllers/
│   │   │   ├── ProjectsController.cs
│   │   │   └── ActivitiesController.cs
│   │   ├── Dockerfile           # Imagen Docker de la API .NET
│   │   └── Program.cs
│   ├── trycore.valor.ganado.application/
│   ├── trycore.valor.ganado.configuration/
│   │   └── BuilderApp.cs        # Bootstrap (DI + Swagger + migraciones)
│   ├── trycore.valor.ganado.domain/
│   └── trycore.valor.ganado.infrastructure/
│       ├── Migrations/          # Migraciones EF Core
│       └── Persistence/
└── tests/
    └── trycore.valor.ganado.test/
```

---

## Despliegue con Docker Compose

> Este es el método recomendado. Levanta PostgreSQL, el microservicio EVM y la API con un solo comando.

### 1. Clonar el repositorio

```bash
git clone https://github.com/diegodazaco/trycore.valor.ganado.backend.net.git
cd trycore.valor.ganado.backend.net
```

### 2. Construir y levantar los servicios

```bash
cd ConfigFiles
docker compose up --build -d
```

Docker Compose:
- Construye la imagen de la API .NET desde `src/trycore.valor.ganado.api/Dockerfile`.
- Construye la imagen del microservicio EVM desde `services/evm-calculator/Dockerfile`.
- Levanta PostgreSQL 17.5 con un volumen persistente.
- Espera a que PostgreSQL y el calculador estén listos (`healthcheck`) antes de iniciar la API.
- La API aplica las migraciones de EF Core automáticamente al arrancar.

### 3. Verificar que los servicios están corriendo

```bash
docker compose ps
```

Deberías ver tres servicios en estado `running`:

| Servicio                  | Puerto local |
|---------------------------|-------------|
| `postgres-trycore`        | 5432        |
| `evm-calculator-trycore`  | 8001        |
| `trycore-api`             | 5000        |

### 4. Acceder a la API

> Para explorar los endpoints desde el navegador se deben usar las siguientes rutas de documentación:

| Servicio          | URL en el navegador                    | Sufijo requerido |
|-------------------|----------------------------------------|------------------|
| API .NET (Swagger)| http://localhost:5000/swagger-ui       | `/swagger-ui`    |
| FastAPI (EVM)     | http://localhost:8001/docs             | `/docs`          |
| API base (REST)   | http://localhost:5000/api              | `/api`           |

> **Nota:** Al ingresar a `http://localhost:5000/` o `http://localhost:8001/` sin sufijo, ambos servicios redirigen automáticamente a su documentación correspondiente.

### 5. Detener los servicios

```bash
# Detiene sin eliminar volúmenes (datos persisten)
docker compose down

# Detiene y elimina volúmenes (base de datos se borra)
docker compose down -v
```

### 6. Ver logs

```bash
# Todos los servicios
docker compose logs -f

# Solo la API
docker compose logs -f api
```

---

## Ejecución en local (desarrollo)

Útil si se quiere depurar la API con Visual Studio o Rider manteniendo los servicios de soporte en Docker.

### 1. Levantar solo PostgreSQL y el calculador EVM

```bash
cd ConfigFiles
docker compose up postgres evm-calculator -d
```

### 2. Restaurar dependencias NuGet

```bash
# Desde la raíz del repositorio
dotnet restore
```

### 3. Ejecutar la API

```bash
dotnet run --project src/trycore.valor.ganado.api
```

La API abrirá automáticamente `https://localhost:7237/swagger-ui` en el navegador.

Las migraciones se aplican automáticamente al iniciar la aplicación.

### 4. Gestionar migraciones manualmente (opcional)

```bash
# Aplicar migraciones pendientes
dotnet ef database update \
  --project src/trycore.valor.ganado.infrastructure \
  --startup-project src/trycore.valor.ganado.api

# Crear una nueva migración
dotnet ef migrations add <NombreMigracion> \
  --project src/trycore.valor.ganado.infrastructure \
  --startup-project src/trycore.valor.ganado.api
```

---

## Variables de entorno

La API lee su configuración desde variables de entorno (sobreescriben `appsettings.json`).

| Variable                              | Descripción                                          | Valor por defecto (local)                          |
|---------------------------------------|------------------------------------------------------|----------------------------------------------------|
| `ASPNETCORE_ENVIRONMENT`              | Entorno de ejecución (`Development` / `Production`)  | `Development`                                      |
| `ASPNETCORE_URLS`                     | URL y puerto en que escucha la API                   | `https://localhost:7237;http://localhost:5234`      |
| `ConnectionStrings__DefaultConnection`| Cadena de conexión a PostgreSQL                      | `Host=localhost;Port=5432;Database=trycore_valor_ganado_db;Username=trycore_postgres;Password=MySecurePassword123!` |
| `EvmCalculator__BaseUrl`              | URL del microservicio de cálculo EVM                 | `http://localhost:8001`                            |

> En Docker Compose estos valores se inyectan directamente en la sección `environment` del servicio `api` dentro de `ConfigFiles/docker-compose.yml`.

---

## Endpoints de la API

### Proyectos — `GET /api/projects`

Retorna la lista de todos los proyectos sin indicadores EVM.

**Respuesta `200 OK`:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Proyecto Alpha",
    "description": "Descripción del proyecto",
    "cutoffDate": "2026-06-01T00:00:00Z"
  }
]
```

---

### Proyectos — `POST /api/projects`

Crea un nuevo proyecto.

**Cuerpo de la solicitud:**
```json
{
  "name": "Proyecto Alpha",
  "description": "Descripción del proyecto",
  "cutoffDate": "2026-06-01T00:00:00Z"
}
```

**Respuesta `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Proyecto Alpha",
  "description": "Descripción del proyecto",
  "cutoffDate": "2026-06-01T00:00:00Z"
}
```

---

### Proyectos — `GET /api/projects/{id}`

Retorna el detalle de un proyecto con todas sus actividades e indicadores EVM calculados en tiempo real.

**Respuesta `200 OK`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Proyecto Alpha",
  "description": "...",
  "cutoffDate": "2026-06-01T00:00:00Z",
  "activities": [
    {
      "id": "...",
      "name": "Actividad 1",
      "budgetAtCompletion": 10000,
      "plannedProgressPercentage": 50,
      "actualProgressPercentage": 40,
      "actualCost": 6000,
      "plannedValue": 5000,
      "earnedValue": 4000,
      "costVariance": -2000,
      "scheduleVariance": -1000,
      "cpi": 0.67,
      "spi": 0.80,
      "estimateAtCompletion": 14925.37,
      "varianceAtCompletion": -4925.37
    }
  ],
  "totalBudgetAtCompletion": 10000,
  "totalPlannedValue": 5000,
  "totalEarnedValue": 4000,
  "totalActualCost": 6000,
  "cpi": 0.67,
  "spi": 0.80
}
```

| Indicador | Significado                                         |
|-----------|-----------------------------------------------------|
| CPI > 1   | Bajo presupuesto (eficiencia de costo favorable)    |
| CPI < 1   | Sobrecosto                                          |
| SPI > 1   | Adelantado respecto al cronograma                   |
| SPI < 1   | Atrasado respecto al cronograma                     |

---

### Proyectos — `PUT /api/projects/{id}`

Actualiza los datos de un proyecto existente.

**Cuerpo de la solicitud:** igual que `POST /api/projects`.

---

### Proyectos — `DELETE /api/projects/{id}`

Elimina un proyecto y todas sus actividades (eliminación en cascada). Responde `204 No Content`.

---

### Actividades — `POST /api/projects/{projectId}/activities`

Agrega una actividad a un proyecto existente.

**Cuerpo de la solicitud:**
```json
{
  "name": "Actividad 1",
  "budgetAtCompletion": 10000,
  "plannedProgressPercentage": 50,
  "actualProgressPercentage": 40,
  "actualCost": 6000
}
```

| Campo                       | Descripción                                        |
|-----------------------------|----------------------------------------------------|
| `budgetAtCompletion`        | Presupuesto total planificado para la actividad    |
| `plannedProgressPercentage` | % de avance planificado a la fecha de corte (0–100)|
| `actualProgressPercentage`  | % de avance real completado (0–100)                |
| `actualCost`                | Costo real incurrido hasta la fecha de corte       |

---

### Actividades — `GET /api/activities/{id}`

Retorna una actividad por ID sin indicadores EVM.
Para ver los indicadores, consultar `GET /api/projects/{id}`.

---

### Actividades — `PUT /api/activities/{id}`

Actualiza una actividad existente. Los indicadores EVM se recalculan en la próxima consulta al proyecto.

---

### Actividades — `DELETE /api/activities/{id}`

Elimina una actividad. Responde `204 No Content`.

---

## Ejemplos de uso

### Flujo completo desde cero

```bash
# 1. Crear un proyecto
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Construcción Torre A",
    "description": "Proyecto de construcción civil",
    "cutoffDate": "2026-06-30T00:00:00Z"
  }'

# 2. Agregar una actividad (usar el id retornado en el paso anterior)
curl -X POST http://localhost:5000/api/projects/{projectId}/activities \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Cimentación",
    "budgetAtCompletion": 50000,
    "plannedProgressPercentage": 60,
    "actualProgressPercentage": 50,
    "actualCost": 32000
  }'

# 3. Consultar el proyecto con indicadores EVM calculados
curl http://localhost:5000/api/projects/{projectId}
```

---

## Ejecución de pruebas

Las pruebas son de integración y requieren Docker corriendo (PostgreSQL en `localhost:5432`).

```bash
# Asegurarse de que los servicios de soporte estén levantados
cd ConfigFiles && docker compose up postgres evm-calculator -d && cd ..

# Ejecutar todas las pruebas
dotnet test

# Con detalle de resultados
dotnet test --logger "console;verbosity=detailed"
```
