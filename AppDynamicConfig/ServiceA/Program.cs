using Config.Client;

class Program
{
    static async Task Main(string[] args)
    {

        var connectionString = "mongodb://localhost:27017";

        using var reader = new ConfigurationReader(
            applicationName: "SERVICE-A",
            connectionString: connectionString,
            refreshTimerIntervalInMs: 5000
        );


        string[] keys =
        {
            "MaxItemCount",
            "IsBasketEnabled",
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
            await Task.Delay(10000); // 10 sn
        }
    }
}


