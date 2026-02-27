# MenuDigital API - Sistema de Gestión Gastronómica

![.NET](https://img.shields.io/badge/.NET-8.0-512bd4)
![EF Core](https://img.shields.io/badge/EF%20Core-Latest-blue)
![Docker](https://img.shields.io/badge/Docker-Container-2496ed)
![Architecture](https://img.shields.io/badge/Architecture-DDD%20/%20Clean%20Arch-green)
![Tests](https://img.shields.io/badge/Tests-xUnit-brightgreen)

**MenuDigital API** es una solución backend robusta diseñada para la digitalización de menús y la gestión de pedidos en tiempo real para restaurantes. Este proyecto demuestra la implementación de prácticas modernas de desarrollo de software, enfocándose en la escalabilidad, el mantenimiento y la separación de responsabilidades.

---

## 🚀 Características Principales

- **Gestión de Menú:** CRUD completo de platos (Dishes) y categorías.
- **Sistema de Pedidos:** Flujo completo desde la creación del pedido hasta la transición de estados (Pendiente, En Preparación, Entregado, etc.).
- **Validaciones Avanzadas:** Lógica de negocio protegida mediante validadores específicos para pedidos y precios.
- **Persistencia de Datos:** Uso de Entity Framework Core con migraciones automatizadas.
- **Sembrado de Datos (Seeding):** Base de datos pre-cargada para pruebas rápidas.
- **Frontend Incluido:** Interfaz administrativa y de cliente funcional para interactuar con la API.

---

## 🏗️ Arquitectura y Buenas Prácticas

El proyecto sigue los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**, organizado en las siguientes capas:

- **Domain:** Entidades principales y lógica central del negocio (Dish, Order, Category).
- **Application:** Interfaces, DTOs (Data Transfer Objects), Mappers y Casos de Uso (Servicios).
- **Infrastructure:** Implementación de persistencia (SQL Server), Contexto de BD y Migraciones.
- **API (Web):** Controladores RESTful y configuración de middlewares para el manejo global de excepciones.
- **UnitTests:** Suite de pruebas para asegurar la calidad del código en comandos y validaciones.

---

## 🛠️ Stack Tecnológico

- **Lenguaje:** C# / .NET 8
- **Base de Datos:** SQL Server
- **ORM:** Entity Framework Core (Code First)
- **Contenedores:** Docker & Docker Compose
- **Testing:** xUnit / Moq
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)

---

## ⚙️ Configuración y Ejecución

### Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Ejecución con Docker (Recomendado)
El proyecto incluye un archivo `compose.yml` que levanta la API y la base de datos automáticamente:

```bash
# Clonar el repositorio
git clone [https://github.com/tu-usuario/MenuDigitalAPI.git](https://github.com/tu-usuario/MenuDigitalAPI.git)
```

# Levantar los servicios
docker-compose up --build

## 🧪 Pruebas Unitarias
La calidad del software es una prioridad. Se han implementado pruebas para cubrir los flujos críticos de la aplicación:
```bash
# Ejecutar las pruebas
dotnet test
```
Enfoque de pruebas: Validadores de pedidos, lógica de transición de estados y comandos de platos.

## Roadmap / Próximas Mejoras
- [ ] Implementación de Autenticación con JWT (Identity).

- [ ] Integración de Swagger/OpenAPI mejorada para documentación de endpoints.

- [ ] Implementación de Patrón Repository para mayor abstracción.

- [ ] Notificaciones en tiempo real con SignalR.

## Contacto
Jonathan Miskinich - [LinkedIn](www.linkedin.com/in/jonathan-miskinich-devbackend) - [Email](jonathan.miskinich.jobs@gmail.com)