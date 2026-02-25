# Menu Digital Restaurante

Sistema para gestión de menú digital en restaurante. Consta de API backend, frontend estático y base de datos PostgreSQL orquestadas con Docker Compose.

---

## 🧩 Tecnologías usadas

- C# / .NET  
- ASP.NET Core Web API  
- Entity Framework Core (Code‑First)  
- PostgreSQL  
- Docker + Docker Compose  
- Frontend estático servido por Nginx  

---

## 📁 Estructura

```
.
├── Application/
├── Domain/
├── Infrastructure/
├── MenuDigitalRestaurante/      ← proyecto API
├── frontend/                     ← archivos estáticos del front
├── Dockerfile                    ← para la API
├── docker-compose.yml
└── README.md
```

---

## 🚀 Cómo levantar el sistema

1. Clonar el repositorio  
   ```bash
   git clone https://github.com/MiskinichJonathanJ/MenuDigitalAPI.git
   cd MenuDigitalAPI
   ```

2. Levantar con Docker Compose  
   ```bash
   docker-compose up --build
   ```

   Esto crea tres servicios:
   - **db**: PostgreSQL  
   - **api**: backend .NET  
   - **frontend**: cliente estático servido por Nginx  

---

## 🌐 URLs de acceso

| Componente | URL local                      |
|------------|--------------------------------|
| API        | http://localhost:5000          |
| Frontend   | http://localhost:3000          |
| Base datos (externa) | localhost:5434            |

---

## 🔌 Conexiones de base de datos

### En el contenedor de la API (interno Docker)
```text
Host = db  
Port = 5432  
Database = testDb  
Username = postgres  
Password = Admin#1234
```

### Desde tu máquina local (para herramientas como DBeaver, psql, etc.)
```text
Host = localhost  
Port = 5434  
Database = testDb  
Username = postgres  
Password = Admin#1234
```

---

## ⚙ Variables de entorno para la API

```text
ConnectionStrings__MenuDigitalConnection = "Host=db;Port=5432;Database=testDb;Username=postgres;Password=Admin#1234"
ASPNETCORE_ENVIRONMENT = Development
ASPNETCORE_URLS = http://+:80
```

---

## 🛑 Detener el sistema

- Para detener contenedores sin borrar datos:
  ```bash
  docker-compose down
  ```
- Para detener y borrar volúmenes (incluye borrar datos de DB):
  ```bash
  docker-compose down -v
  ```

---

## 🔍 Endpoints disponibles (ejemplos)

- `GET /api/v1/dishes` — listar platos (con filtros y orden)  
- `POST /api/v1/dishes` — crear nuevo plato  
- `PUT /api/v1/dishes/{id}` — actualizar plato  

---

## 🧠 Consejos y notas

- Frontend estático es servido por Nginx dentro del contenedor `frontend`.  
- API usa `dotnet watch` para recargar automáticamente en desarrollo.  
- Datos de la base de datos se persisten en el volumen `db_data`.  
- Si cambias modelos, asegurate de generar migraciones con EF Core y aplicarlas.  
- Si algo falla, correr `docker ps` para revisar qué contenedor no arrancó.  
