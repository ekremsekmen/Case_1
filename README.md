# 🚀 Case_1_2 - Product API with Authentication

Modern .NET 8.0 Web API projesi - JWT Authentication, Redis Cache ve CQRS pattern ile geliştirilmiştir.

**Backend Developer 1. ve 2. Aşama gereksinimlerini karşılar:**
- ✅ Product CRUD API
- ✅ JWT Authentication  
- ✅ Redis Cache
- ✅ Onion Architecture + CQRS
- ✅ PostgreSQL Database

---

## 🛠️ Kurulum ve Çalıştırma

### 📋 Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) 
- [Redis](https://redis.io/download)

### 1️⃣ Projeyi İndirin

```bash
git clone <your-repo-url>
cd Case_1_2
```

### 2️⃣ Bağımlılıkları Yükleyin

```bash
dotnet restore
```

### 3️⃣ PostgreSQL Kurulumu

**Windows:**
1. PostgreSQL'i [buradan](https://www.postgresql.org/download/windows/) indirin ve kurun
2. pgAdmin ile `ProductDb_Dev` database'ini oluşturun

**macOS:**
```bash
brew install postgresql
brew services start postgresql
createdb ProductDb_Dev
```

**Linux (Ubuntu):**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo -u postgres createdb ProductDb_Dev
```

### 4️⃣ Redis Kurulumu

**Windows:**
- [Redis for Windows](https://github.com/microsoftarchive/redis/releases) indirin ve çalıştırın

**macOS:**
```bash
brew install redis
brew services start redis
```

**Linux (Ubuntu):**
```bash
sudo apt install redis-server
sudo systemctl start redis-server
```

### 5️⃣ Database Migration

```bash
# EF Core tools kurulumu (ilk kez)
dotnet tool install --global dotnet-ef

# Database'i oluşturun
dotnet ef database update
```

### 6️⃣ Uygulamayı Çalıştırın

```bash
dotnet run
```

**✅ Uygulama çalışacaktır:**
- **API**: http://localhost:5125
- **Swagger UI**: http://localhost:5125 (otomatik açılır)

---

## 🧪 Hızlı Test

### 1. Kullanıcı Kaydı
```bash
curl -X POST "http://localhost:5125/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### 2. Login (Token Alın)
```bash
curl -X POST "http://localhost:5125/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "testuser",
    "password": "Test123!"
  }'
```

### 3. Ürün Oluşturun (Token ile)
```bash
curl -X POST "http://localhost:5125/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "name": "Test Product",
    "description": "Test açıklaması",
    "price": 29.99,
    "stock": 100,
    "category": "Electronics"
  }'
```

### 4. Ürünleri Listeleyin
```bash
curl -X GET "http://localhost:5125/api/products"
```

---

## 📋 API Endpoints

| Method | Endpoint | Açıklama | Auth |
|--------|----------|----------|------|
| POST | `/api/auth/register` | Kullanıcı kaydı | ❌ |
| POST | `/api/auth/login` | Login (JWT token) | ❌ |
| GET | `/api/auth/profile` | Kullanıcı profili | ✅ |
| GET | `/api/products` | Ürün listesi (Cache'li) | ❌ |
| GET | `/api/products/{id}` | Ürün detayı | ❌ |
| POST | `/api/products` | Ürün oluştur | ✅ |
| PUT | `/api/products/{id}` | Ürün güncelle | ✅ |
| DELETE | `/api/products/{id}` | Ürün sil | ✅ |

---

## ⚙️ Konfigürasyon

`appsettings.Development.json` dosyasındaki ayarları kontrol edin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProductDb_Dev;Username=postgres;Password=password",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "SecretKey": "DevSuperSecretKeyThatShouldBeAtLeast256BitsLongForHmacSha256Algorithm",
    "Issuer": "Case1API",
    "Audience": "Case1APIUsers",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

**Not:** PostgreSQL şifrenizi değiştirdiyseniz `DefaultConnection` string'ini güncelleyin.

---

## 🔧 Sorun Giderme

### PostgreSQL Bağlantı Hatası
```bash
# PostgreSQL çalışıyor mu kontrol edin
sudo systemctl status postgresql  # Linux
brew services list | grep postgres  # macOS
```

### Redis Bağlantı Hatası
```bash
# Redis çalışıyor mu kontrol edin
redis-cli ping  # PONG döner ise çalışıyor
```

### Migration Hatası
```bash
# Migration'ları sıfırlayın
dotnet ef database drop
dotnet ef database update
```

### Port Çakışması
`Properties/launchSettings.json` dosyasından port'u değiştirebilirsiniz.

---

## 🏗️ Proje Yapısı

```
Case_1_2/
├── 📁 API/                           # API Katmanı
│   ├── Controllers/                  # REST API endpoints
│   │   ├── AuthController.cs         # Authentication endpoints
│   │   └── ProductsController.cs     # Product CRUD endpoints
│   └── Middleware/                   # API-specific middleware (empty)
├── 🎯 Core/                          # İç Katmanlar
│   ├── Domain/                       # Domain Entities
│   │   └── Entities/
│   │       ├── User.cs               # Kullanıcı entity
│   │       ├── Product.cs            # Ürün entity
│   │       └── RefreshToken.cs       # JWT refresh token
│   └── Application/                  # Application Logic
│       ├── Commands/                 # CQRS Commands (Write)
│       │   ├── Auth/                 # Auth commands
│       │   └── Products/             # Product commands
│       ├── Queries/                  # CQRS Queries (Read)
│       │   ├── Auth/                 # Auth queries
│       │   └── Products/             # Product queries
│       ├── Handlers/                 # Command/Query Handlers
│       │   ├── Auth/                 # Auth handlers
│       │   └── Products/             # Product handlers
│       └── DTOs/                     # Data Transfer Objects
│           └── Auth/                 # Auth DTOs
├── 🔧 Infrastructure/                # Dış Katman
│   ├── Data/
│   │   └── ApplicationDbContext.cs  # EF Core DbContext
│   ├── Repositories/                # Repository Pattern
│   │   ├── IProductRepository.cs
│   │   └── ProductRepository.cs
│   └── Services/                    # Business Services
│       ├── ProductService.cs        # Product business logic
│       ├── JwtService.cs            # JWT token service
│       └── CacheService.cs          # Redis cache service
├── ⚙️ Middleware/                    # Global HTTP Middleware
│   └── GlobalExceptionHandlingMiddleware.cs
├── 📊 Migrations/                   # EF Core Migrations
├── 📁 Properties/                   # Launch Settings
│   └── launchSettings.json
├── 📋 DTOs/                         # Legacy DTOs (backward compatibility)
├── 📁 Data/                         # Legacy data folder (empty)
├── 📦 Models/                       # Legacy models folder (empty)
├── 📄 Program.cs                    # Application Entry Point
├── 📄 appsettings.json             # Production Configuration
├── 📄 appsettings.Development.json # Development Configuration
├── 📄 Case_1_2.csproj              # Project Configuration
├── 📄 Case_1_2.sln                 # Solution File
├── 📄 Case_1_2.http                # HTTP Test Requests
└── 📄 README.md                    # Documentation
```

---

## 📚 Kullanılan Teknolojiler

- **.NET 8.0** - Web API framework
- **PostgreSQL** - Database
- **Redis** - Cache
- **Entity Framework Core** - ORM
- **JWT** - Authentication
- **MediatR** - CQRS pattern
- **Serilog** - Logging
- **Swagger** - API documentation

---

## 🎯 Özellikler

- ✅ **CRUD Operations** - Complete product management
- ✅ **JWT Authentication** - Secure login system
- ✅ **Redis Caching** - High performance data access
- ✅ **CQRS Pattern** - Command/Query separation
- ✅ **Onion Architecture** - Clean code structure
- ✅ **Global Exception Handling** - Centralized error management
- ✅ **Swagger Documentation** - Interactive API docs
- ✅ **Structured Logging** - Comprehensive logging

---

## 📞 Destek

Herhangi bir sorun yaşarsanız:
1. Önce **Sorun Giderme** bölümünü kontrol edin
2. GitHub Issues'dan yeni bir issue açın
3. Log dosyalarını (`logs/` klasöründe) kontrol edin

**👨‍💻 Geliştirici:** [Ekrem Sekmen](mailto:ekremsekmenq@gmail.com)
