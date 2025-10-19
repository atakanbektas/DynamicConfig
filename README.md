# 🧩 AppDynamicConfig

### Özet

AppDynamicConfig; uygulamaların **çalışırken konfigürasyonlarını merkezi bir yerden yönetmesini** sağlayan küçük bir örnek sistemdir.  
Bu yapı, **deployment veya restart gerektirmeden** konfigürasyon güncellemelerini mümkün kılar.

Başlıca bileşenler:

- **Config.Api** – CRUD ve sorgu uçları (MongoDB)
- **Admin.Mvc** – Config kayıtlarını yönetmek için basit bir UI (Bootstrap)
- **Config.Client** – Uygulama içinden ayar okumak için .NET kütüphanesi (ConfigurationReader)
- **ServiceA** – Client kütüphanesini kullanan örnek console servisi

Her şey **docker-compose** ile tek komutla ayağa kalkar.

---

### 🧠 Not

Client kütüphanesi bu örnekte **doğrudan MongoDB’ye** bağlanır.  
İstenirse bu yapı kolayca **API üzerinden veri çekecek şekilde** dönüştürülebilir.

---

## ⚙️ Hızlı Başlangıç (Docker)

### Ön koşullar

- **Docker Desktop**
- **.NET 8 SDK** *(opsiyonel, yerel geliştirme için)*

---

### 🚀 Çalıştırma

```bash
docker compose up --build -d
```

---

### Varsayılan Adresler

| Servis | URL |
|--------|-----|
| **API Swagger** | http://localhost:5080/swagger |
| **Admin UI** | http://localhost:5070 |
| **MongoDB** | localhost:27017 *(Docker içinden hostname: `mongo`)* |

---

## 🧱 Mimari

```
Bu proje Onion Architecture (Soğan Mimarisi) üzerine kurulmuştur.
Amaç; katmanlar arasındaki bağımlılıkları dışa doğru yönlendirmek, domain katmanını merkeze alarak bağımlılıkları tersine çevirmektir.
MediatR kütüphanesi CQRS yapısını desteklemek için kullanılmıştır.
```

---

## 🧰 Yapı

```
AppDynamicConfig/
├─ ConfigService/
│  ├─ Config.Api/
│  ├─ Config.Application/
│  ├─ Config.Domain/
│  ├─ Config.Infrastructure/
│  └─ Config.Persistence/
├─ Samples/
│  ├─ Admin.Mvc/
│  └─ Config.Client/
├─ ServiceA/
└─ docker-compose.yml
```

---

## 🧩 Config.Client Kullanımı

```csharp
using Config.Client;

using var reader = new ConfigurationReader(
    applicationName: "SERVICE-A",
    connectionString: "mongodb://localhost:27017",
    refreshTimerIntervalInMs: 5000,
    logger: Console.WriteLine);

var siteName = reader.GetValue<string>("SiteName");
var isEnabled = reader.GetValue<bool>("IsBasketEnabled");
var maxCount = reader.GetValue<int>("MaxItemCount");
```

**Nasıl Çalışır:**

- İlk açılışta MongoDB’den aktif kayıtları çeker.
- Her X milisaniyede bir otomatik yeniler.
- `GetValue<T>()` her zaman **cache üzerinden okur**.
- Mongo geçici olarak down olsa bile **okuma işlemleri kesilmez**.
- Tip dönüşümleri:
  - `string` → ham değer
  - `int` → `int.Parse(...)`
  - `double` → `double.Parse(...)`
  - `bool` → `"1"` veya `true/false`

---

## 🧾 API Uçları

| Method | Endpoint                    | Açıklama                 |
|-------:|-----------------------------|--------------------------|
| GET    | `/api/configs?app=SERVICE-A`| Config listesi           |
| POST   | `/api/configs`              | Yeni config ekle/güncelle|
| DELETE | `/api/configs/{app}/{name}` | Config sil               |

**POST örneği:**
```json
{
  "applicationName": "SERVICE-A",
  "name": "SiteName",
  "type": "string",
  "value": "soty.io",
  "isActive": true
}

```

---

## 🧪 Unit Test

Örnek test projesi:  
`tests/Config.Client.Tests/ConfigurationReaderTests.cs`

```bash
dotnet test
```

---

## 👨‍💻 Geliştirici Notu

> Bu proje, **dinamik konfigürasyon yönetimi** kavramını örnekleyen basit ama genişletilebilir bir yapıdır.  
> Amaç, mikroservis mimarisinde konfigürasyonun **canlı ve merkezi** biçimde yönetilebileceğini göstermektir.
