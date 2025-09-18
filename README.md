# Product API

Bu proje, ASP.NET Core 8.0 ve Entity Framework Core kullanarak geliştirilmiş bir RESTful Product API'sidir. Temel CRUD işlemlerini destekler ve katmanlı mimari prensiplerini takip eder.

## 🚀 Teknolojiler

- **.NET 8.0** - Web API framework
- **ASP.NET Core** - Web application framework
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Veritabanı
- **Swagger/OpenAPI** - API dokümantasyonu

## 🏗️ Mimari

Proje katmanlı mimari (Layered Architecture) prensiplerini takip eder:

```
├── Controllers/         # API endpoints
├── Services/           # İş mantığı katmanı
├── Repositories/       # Veri erişim katmanı
├── Models/            # Entity modelleri
├── DTOs/              # Data Transfer Objects
├── Data/              # Database context
├── Middleware/        # Global middleware (Exception handling)
└── Migrations/        # EF Core migrations
```

## 📋 Özellikler

- ✅ **CRUD İşlemleri**: Product ekleme, listeleme, güncelleme, silme
- ✅ **Katmanlı Mimari**: Controller-Service-Repository pattern
- ✅ **Asenkron Programlama**: async/await kullanımı
- ✅ **Global Exception Handling**: Merkezi hata yönetimi
- ✅ **Dependency Injection**: SOLID prensiplerine uygun
- ✅ **Swagger Dokümantasyonu**: Otomatik API dokümantasyonu
- ✅ **Validation**: Model validation ve data annotations
- ✅ **CORS**: Cross-Origin Resource Sharing desteği

## 📊 API Endpoints

### Products

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/products` | Tüm ürünleri listele |
| GET | `/api/products/{id}` | ID'ye göre ürün detayı |
| POST | `/api/products` | Yeni ürün oluştur |
| PUT | `/api/products/{id}` | Ürün güncelle |
| DELETE | `/api/products/{id}` | Ürün sil |

### Model Yapısı

```json
{
  "id": 1,
  "name": "Ürün Adı",
  "description": "Ürün açıklaması",
  "price": 29.99,
  "stock": 100,
  "category": "Kategori",
  "createdAt": "2025-09-18T10:00:00Z",
  "updatedAt": "2025-09-18T10:00:00Z"
}
```

## 🛠️ Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd Case_1
```

### 2. Bağımlılıkları Yükleyin

```bash
dotnet restore
```

### 3. PostgreSQL Kurulumu

PostgreSQL'i yükleyin ve çalıştırın. Varsayılan ayarlar:
- Host: localhost
- Port: 5432
- Username: postgres
- Password: password

### 4. Veritabanı Bağlantısını Yapılandırın

`appsettings.json` veya `appsettings.Development.json` dosyasındaki bağlantı dizesini güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProductDb_Dev;Username=postgres;Password=your_password"
  }
}
```

### 5. EF Core Tools'u Yükleyin (İlk kez)

```bash
dotnet tool install --global dotnet-ef
```

### 6. Veritabanı Migration'larını Uygulayın

```bash
# Migration oluştur (gerekirse)
dotnet ef migrations add InitialCreate

# Migration'ları veritabanına uygula
dotnet ef database update
```

### 7. Uygulamayı Çalıştırın

```bash
dotnet run
```

Uygulama varsayılan olarak şu adreslerde çalışacaktır:
- HTTPS: `https://localhost:7046`
- HTTP: `http://localhost:5046`

### 8. Swagger UI'ya Erişin

Tarayıcınızda şu adresi açın:
- Development: `https://localhost:7046` (Swagger UI otomatik açılır)
- Swagger JSON: `https://localhost:7046/swagger/v1/swagger.json`

## 🧪 API Testleri

### Postman/Thunder Client ile Test

#### 1. Ürün Oluştur (POST)
```http
POST https://localhost:7046/api/products
Content-Type: application/json

{
  "name": "Test Ürünü",
  "description": "Bu bir test ürünüdür",
  "price": 49.99,
  "stock": 25,
  "category": "Test"
}
```

#### 2. Tüm Ürünleri Listele (GET)
```http
GET https://localhost:7046/api/products
```

#### 3. Ürün Detayı (GET)
```http
GET https://localhost:7046/api/products/1
```

#### 4. Ürün Güncelle (PUT)
```http
PUT https://localhost:7046/api/products/1
Content-Type: application/json

{
  "name": "Güncellenmiş Ürün",
  "price": 59.99,
  "stock": 30
}
```

#### 5. Ürün Sil (DELETE)
```http
DELETE https://localhost:7046/api/products/1
```

### cURL ile Test

```bash
# Ürün oluştur
curl -X POST "https://localhost:7046/api/products" \
     -H "Content-Type: application/json" \
     -d '{"name":"Test Product","description":"Test description","price":29.99,"stock":100,"category":"Electronics"}' \
     -k

# Ürünleri listele
curl -X GET "https://localhost:7046/api/products" -k

# Ürün detayı
curl -X GET "https://localhost:7046/api/products/1" -k
```

## 🐳 Docker ile Çalıştırma (Opsiyonel)

### PostgreSQL Docker Container

```bash
# PostgreSQL container'ı çalıştır
docker run --name postgres-db \
  -e POSTGRES_DB=ProductDb_Dev \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=password \
  -p 5432:5432 \
  -d postgres:15
```

## 📁 Proje Yapısı

```
Case_1/
├── Controllers/
│   └── ProductsController.cs      # API endpoints
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── DTOs/
│   ├── ProductDto.cs             # Response DTO
│   ├── CreateProductDto.cs       # Create request DTO
│   └── UpdateProductDto.cs       # Update request DTO
├── Middleware/
│   └── GlobalExceptionHandlingMiddleware.cs  # Exception handling
├── Migrations/                   # EF Core migrations
├── Models/
│   └── Product.cs               # Product entity
├── Repositories/
│   ├── IProductRepository.cs    # Repository interface
│   └── ProductRepository.cs     # Repository implementation
├── Services/
│   ├── IProductService.cs       # Service interface
│   └── ProductService.cs        # Service implementation
├── appsettings.json             # Configuration
├── appsettings.Development.json # Development configuration
├── Case_1.csproj               # Project file
├── Program.cs                  # Application entry point
└── README.md                   # Bu dosya
```

## 🔧 Geliştirme Notları

### SOLID Prensipleri

- **Single Responsibility**: Her sınıf tek bir sorumluluğa sahip
- **Dependency Injection**: Bağımlılıklar constructor injection ile yönetilir
- **Interface Segregation**: Repository ve Service katmanları interface'ler ile ayrılmış

### Error Handling

- Global exception middleware tüm hataları yakalar
- HTTP status kodları doğru şekilde döndürülür
- Validation hataları ModelState ile yönetilir

### Async/Await

- Tüm veritabanı işlemleri asenkron
- Performance optimizasyonu için non-blocking I/O

## 🚀 Production Deployment

### Environment Variables

Production ortamında şu environment variable'ları ayarlayın:

```bash
export ConnectionStrings__DefaultConnection="Host=prod-host;Database=ProductDb;Username=prod-user;Password=prod-password"
export ASPNETCORE_ENVIRONMENT=Production
```

### Health Checks (Gelecek Geliştirme)

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));
```

## 📈 Gelecek Geliştirmeler

- [ ] Unit testler eklenmesi
- [ ] Integration testler
- [ ] Logging (Serilog) entegrasyonu
- [ ] Caching (Redis) implementasyonu
- [ ] Rate limiting
- [ ] Authentication & Authorization (JWT)
- [ ] API versioning
- [ ] Health checks
- [ ] Docker containerization

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 📞 İletişim

Sorularınız için: [support@example.com](mailto:support@example.com)

---

**Not**: Bu API geliştirme amacıyla oluşturulmuştur ve production ortamında kullanılmadan önce güvenlik ve performans testlerinden geçirilmelidir.
