# Basic backend development principles

## Giriş

Bu proje, temel backend geliştirme prensiplerini, REST API oluşturma ve veritabanı (MSSQL) işlemlerini ölçmek amacıyla ASP.NET Core 8 ve Entity Framework Core kullanılarak geliştirilmiş bir projedir.

## Kullanılan Teknolojiler

- **Programlama Dili:** C#
- **Backend Çatısı:** ASP.NET Core 8
- **Veritabanı:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Paket Yöneticisi:** NuGet

## Kurulum ve Çalıştırma Adımları

Bu bölüm, projenin yerel makinenizde nasıl kurulacağını ve çalıştırılacağını açıklar.

### Ön Koşullar

Projenin doğru şekilde çalışabilmesi için aşağıdaki yazılımların sisteminizde yüklü olması gerekmektedir:

- **Git:** Proje kodlarını klonlamak için.
- **.NET 8 SDK:** C# ve ASP.NET Core 8 uygulamalarını derlemek ve çalıştırmak için.
- **Microsoft SQL Server:** Projenin kullanacağı veritabanı. SQL Server Management Studio (SSMS) veya Azure Data Studio gibi bir araç, veritabanını yönetmek için faydalı olacaktır.

### Kurulum Adımları

1.  **Depoyu Klonlayın:**

    ```bash
    git clone https://github.com/barisgevher/kayra-export-case1.git
    cd kayra-export-case1
    ```

2.  **NuGet Paketlerini Geri Yükleyin:**
    Proje dizininde (genellikle `.sln` dosyasının olduğu yerde) aşağıdaki komutu çalıştırın:

    ```bash
    dotnet restore
    ```

3.  **Veritabanı Yapılandırması:**

    - Projenizin ana klasöründe bulunan `appsettings.json` veya `appsettings.Development.json` dosyasını açın.
    - `ConnectionStrings` bölümündeki `DefaultConnection` değerini kendi MSSQL Server kurulumunuza göre güncelleyin.
      ```json
      {
        "ConnectionStrings": {
          "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=KayraCaseDb;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True"
          // Örneğin: Server=localhost\\SQLEXPRESS;Database=KayraCaseDb;Trusted_Connection=True;TrustServerCertificate=True
        }
        // Diğer ayarlar...
      }
      ```
    - `YOUR_SERVER_NAME`, `YOUR_DB_USER`, `YOUR_DB_PASSWORD` değerlerini kendi veritabanı sunucunuzun bilgileriyle değiştirin. `Trusted_Connection=True` kullanıyorsanız, kullanıcı adı ve şifreye gerek kalmayabilir.

4.  **Veritabanı Migrasyonlarını Uygulayın:**
    Entity Framework Core migrasyonlarını uygulayarak veritabanı şemasını oluşturun ve güncelleyin. Projenizin `DbContext` sınıfının olduğu proje dizininde (genellikle ana API projesi veya bir altyapı projesi olabilir) aşağıdaki komutları çalıştırın:
    ```bash
    dotnet ef database update
    ```
    Bu komut, `appsettings.json` dosyasındaki bağlantı dizesini kullanarak veritabanını oluşturacak veya mevcut şemayı güncelleyecektir.

### Uygulamayı Çalıştırma

Tüm bağımlılıklar yüklendikten ve veritabanı yapılandırıldıktan sonra uygulamayı çalıştırabilirsiniz:

- **Komut Satırı ile:**
  Projenin ana klasöründe (genellikle `.csproj` dosyasının olduğu yerde) aşağıdaki komutu çalıştırın:
  ```bash
  dotnet run
  ```
- **Visual Studio ile:**
  Proje çözümünü (`.sln` uzantılı dosya) Visual Studio'da açın ve "IIS Express" veya projenizin adını seçerek (örneğin "KayraExportCase") uygulamayı başlatın.

Uygulama başarıyla başlatıldığında, genellikle `http://localhost:5000` veya `http://localhost:7000` (HTTPS) gibi bir adresten erişilebilir olacaktır.

## API Uç Noktaları (Örnek)

Proje çalıştığında erişilebilecek bazı tipik API uç noktaları:

- `GET https://localhost:7282/api/Products`: Tüm öğeleri listeler.
- `POST https://localhost:7282/index.html#/Products/post_api_Products`: Yeni bir öğe oluşturur.
