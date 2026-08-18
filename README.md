# Black Wallet Challenge - Microservicio de Flujo de Caja

Solución al desafío técnico backend enfocado en la gestión de ingresos y gastos con precisión financiera, desarrollado por **Agustín Santillán**.

---

## Estructura de la Solución (N-Tier Architecture)

La solución está organizada en dos proyectos independientes dentro de un único `.sln`:

```
CashFlow-master/
├── CashFlow.Api/                  # Capa de presentación — Thin Controllers + DI
├── CashFlow.Core/                 # Dominio — Entidades, DTOs, Enums, Interfaces
├── CashFlow.Infrastructure/       # Persistencia — EF Core, Repositorios, Migraciones
├── CashFlow.services/             # Lógica de negocio — Servicios de aplicación
├── Legacy.UsersApi/               # Bonus Track — API legacy en .NET Framework 4.8
├── Black Wallet Challenge - Agustin.postman_collection.json
└── CashFlow.sln
```

### Proyecto 1: Microservicio Core (.NET 8)

| Capa | Responsabilidad |
|------|-----------------|
| `CashFlow.Api` | Controladores HTTP, registro de dependencias, configuración de Swagger |
| `CashFlow.Core` | Entidades del dominio (`Transaction`), DTOs, Enums, interfaces abstractas |
| `CashFlow.Infrastructure` | `CashFlowDbContext`, `TransactionRepository`, migraciones EF Core |
| `CashFlow.services` | `TransactionService` — orquesta validaciones y delegación al repositorio |

### Proyecto 2: Legacy Users API (.NET Framework 4.8)

API independiente que simula un orquestador legacy con ASP.NET Web API 2. Expone autenticación y consulta de usuarios con persistencia en memoria, sin dependencias de base de datos.

---

## Requisitos Previos

| Herramienta | Versión mínima |
|-------------|---------------|
| .NET 8 SDK | 8.0 |
| .NET Framework Developer Pack | 4.8 |
| Microsoft SQL Server | 2017 (o SQL Server Express) |
| Visual Studio | 2022 (recomendado) |

---

## Inicialización de la Base de Datos

### Paso 1 — Configurar la cadena de conexión

Editar `CashFlow.Api/appsettings.json` y reemplazar el valor de `DefaultConnection` para apuntar a la instancia local de SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CashFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Si se usa una instancia nombrada distinta (e.g., `.\MSSQLSERVER`), ajustar el valor de `Server` en consecuencia.

### Paso 2 — Aplicar la migración

**Opción A — Package Manager Console (Visual Studio):**

```powershell
Update-Database -Project CashFlow.Infrastructure -StartupProject CashFlow.Api
```

**Opción B — CLI de .NET (terminal):**

```bash
dotnet ef database update --project CashFlow.Infrastructure --startup-project CashFlow.Api
```

Esto ejecuta la migración `20260815220617_InitialCreate` y crea la base de datos `CashFlowDb` con la tabla `Transactions` configurada con `decimal(18,2)` para los montos.

---

## Ejecución de los Proyectos

### Microservicio Core (.NET 8)

**Desde Visual Studio:**  
Seleccionar `CashFlow.Api` como proyecto de inicio y presionar `F5` o `Ctrl+F5`.

**Desde la CLI:**

```bash
cd CashFlow.Api
dotnet run
```

El microservicio quedará disponible en:

| Protocolo | URL |
|-----------|-----|
| HTTP | `http://localhost:5180` |
| HTTPS | `https://localhost:7126` |
| Swagger UI | `https://localhost:7126/swagger` |

### Legacy Users API (.NET Framework 4.8)

La API legacy requiere IIS Express incluido en Visual Studio (no es compatible con `dotnet run` por tratarse de .NET Framework).

**Desde Visual Studio:**  
Click derecho sobre `Legacy.UsersApi` → `Set as Startup Project` → `F5`.

Para ejecutar ambos proyectos en simultáneo:
1. Click derecho en la solución → `Set Startup Projects...`
2. Seleccionar `Multiple startup projects`
3. Establecer `Start` para `CashFlow.Api` y `Legacy.UsersApi`
4. Presionar `F5`

---

## Endpoints de la API

### Microservicio Core — `CashFlow.Api`

Base URL: `https://localhost:7126/api/transactions`

| Método | Ruta | Descripción | Código de éxito |
|--------|------|-------------|-----------------|
| `POST` | `/income` | Registrar un ingreso | `201 Created` |
| `POST` | `/expense` | Registrar un gasto (validación de saldo atómica) | `201 Created` |
| `GET` | `/balance` | Consultar saldo actual (ingresos, gastos, neto) | `200 OK` |
| `GET` | `/history` | Listar todas las transacciones ordenadas por fecha | `200 OK` |

#### Body para `POST /income` y `POST /expense`

```json
{
  "description": "Pago de cliente ABC",
  "amount": 15000.00,
  "date": "2026-08-15T10:00:00Z"
}
```

> El campo `date` es opcional. Si se omite, se registra la fecha/hora actual del servidor.

#### Respuesta de `GET /balance`

```json
{
  "totalIncome": 50000.00,
  "totalExpense": 12000.00,
  "netBalance": 38000.00
}
```

#### Validaciones de negocio

- `amount` debe ser mayor a cero; de lo contrario: `400 Bad Request`.
- Al registrar un gasto: si el saldo neto es menor al monto solicitado: `400 Bad Request` con `{ "error": "Saldo insuficiente para registrar este gasto." }`.

### Legacy Users API — `Legacy.UsersApi`

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/login` | Autenticar usuario (devuelve token simulado) |
| `GET` | `/api/users/{id}` | Obtener usuario por ID |

#### Credenciales de prueba (en memoria)

| Email | Password |
|-------|----------|
| `admin@blackwallet.com` | `1234` |
| `operador@blackwallet.com` | `1234` |

#### Body para `POST /api/login`

```json
{
  "email": "admin@blackwallet.com",
  "password": "1234"
}
```

---

## Colección Postman

En la raíz del repositorio se incluye:

```
Black Wallet Challenge - Agustin.postman_collection.json
```

Contiene ejemplos pre-configurados con flujos exitosos (`201 Created`, `200 OK`) y casos de error de negocio (`400 Bad Request` por saldo insuficiente).

**Importar en Postman:** `File → Import → seleccionar el archivo .json`.

---

## Justificación de Arquitectura y Decisiones Técnicas

### ORM — Entity Framework Core 8

Se eligió **EF Core 8** sobre alternativas como Dapper o ADO.NET puro por las siguientes razones:

- **Code-First con migraciones controladas:** permite versionar el esquema de base de datos junto al código, crucial en entornos CI/CD. La migración `InitialCreate` es reproducible en cualquier entorno con un solo comando.
- **Configuración Fluent API (`IEntityTypeConfiguration`):** el uso de `TransactionConfiguration` mantiene el `DbContext` limpio y centraliza las restricciones del esquema (longitudes, tipos de columna, valores por defecto) sin contaminar las entidades del dominio con atributos de persistencia.
- **LINQ type-safe:** las consultas sobre `Transactions` (filtros por `TransactionType`, sumas de `Amount`) se verifican en tiempo de compilación, reduciendo errores de mapeo.
- **Testabilidad:** la abstracción detrás de `ITransactionRepository` permite reemplazar EF Core por un repositorio en memoria en pruebas unitarias sin cambios en la capa de servicios.

### Precisión Financiera

Se utiliza `decimal` en C# con `.HasColumnType("decimal(18,2)")` via Fluent API en SQL Server. Esto evita los errores de redondeo por punto flotante que ocurrirían con `float` o `double`, crítico en cualquier sistema que maneje dinero.

El tipo `TransactionType` (enum) se persiste como `string` en la base de datos (`.HasConversion<string>()`) para mejorar la legibilidad de los datos y evitar dependencias de orden en el enum.

### Concurrencia y Consistencia (Race Conditions)

El método `AddExpenseAtomicAsync` en `TransactionRepository` envuelve la operación completa en una transacción de base de datos con nivel de aislamiento `RepeatableRead`:

```csharp
using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
```

Esto garantiza que el cálculo del saldo y la inserción del gasto sean **atómicos**: si dos solicitudes de gasto concurrentes llegan simultáneamente con saldo justo, sólo una de ellas tendrá éxito. La otra recibirá el rollback y lanzará la excepción de saldo insuficiente.

### Patrón Repositorio

La interfaz `ITransactionRepository` en `CashFlow.Core` desacopla la capa de servicios de EF Core. Beneficios concretos:

- La lógica de negocio en `TransactionService` no importa ningún namespace de `Microsoft.EntityFrameworkCore`.
- Las consultas a la base de datos están centralizadas y son fácilmente auditables.
- Facilita la implementación de caché distribuida en el futuro sin modificar la capa de servicios.

### Manejo del Tiempo

Las transacciones de tipo gasto usan `DateTime.UtcNow` para garantizar consistencia temporal independientemente de la zona horaria del servidor host — relevante en despliegues cloud multi-región.
