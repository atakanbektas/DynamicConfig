using Config.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Not: Docker dışında local çalıştırıyorsan localhost kullan
        // Docker compose içinde ayrı container’da koşacaksan "mongodb://mongo:27017"
        var connectionString = "mongodb://localhost:27017";

        using var reader = new ConfigurationReader(
            applicationName: "SERVICE-A",
            connectionString: connectionString,
            refreshTimerIntervalInMs: 30000 // cache’i 3 sn’de bir tazele
        );

        // İzlemek istediğin key’leri burada tanımla
        // (Sadece GetValue<T> kullanılacak; hepsini string olarak çekeriz)
        string[] keys =
        {
            "Age",
            "IsBasketEnabled",
            "IsPossible",
            "SiteName"
        };

        while (true)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ---- ACTIVE CONFIGS (SERVICE-A) ----");

            foreach (var key in keys)
            {
                try
                {
                    var value = reader.GetValue<string>(key);
                    Console.WriteLine($"- {key} -> {value}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"! {key} okunamadı: {ex.Message}");
                }
            }

            Console.WriteLine(); // boş satır
            await Task.Delay(5000); // 5 sn
        }
    }
}


