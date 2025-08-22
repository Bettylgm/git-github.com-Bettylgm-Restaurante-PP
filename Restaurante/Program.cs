using System;
using System.Threading.Tasks;


namespace Program;

public class Program
{
    public static async Task Main(string[] args)
    {
        var clientes = new[]
        {
            new Cliente { Id = "A" },
            new Cliente { Id = "B" }
        };

        foreach (var cliente in clientes)
        {
            cliente.Llegadaalrestaurante();
        }

        var chefs = new[]
        {
            new Chef { Id = 1 },
            new Chef { Id = 2 }
        };

        var chefTasks = new Task[chefs.Length];
        for (int i = 0; i < chefs.Length; i++)
        {
            chefTasks[i] = chefs[i].PrepararPlatilloAsync();
        }

        await Task.WhenAll(chefTasks);
    }
}
    

public class Cliente
{
    public string Id { get; set; } = string.Empty;
    public void Llegadaalrestaurante()
    {
        Console.WriteLine($"[Cliente {Id}] ha llegado al restaurante.");
        SimulacionInicia.RecordStartTime();
        Ordenar();
    }
    public void Ordenar()
    {
        foreach (var platillo in new[] { "pizza", "pasta", "calzone" })
        {
            SimulacionInicia.OrderQue.Enqueue($"{Id}-{platillo}");
        }
    }
}

public class Chef
{
    public int Id { get; set; }

    public async Task PrepararPlatilloAsync()
    {
        while (!SimulacionInicia.OrderQue.IsEmpty)
        {
            if (SimulacionInicia.OrderQue.TryDequeue(out var platillo))
            {
                var parts = platillo.Split('-');
                var ClienteId = parts[0];
                var platilloNombre = parts[1];

                Console.WriteLine($"[Chef {Id}] está preparando {platilloNombre} para el cliente {ClienteId}.");
                await Task.Delay(SimulacionInicia.Menu[platilloNombre] * 1000); 
                Console.WriteLine($"[Chef {Id}] ha terminado de preparar {platilloNombre} para el cliente {ClienteId}.");

                if (platilloNombre == "Tacos")
                {
                    Console.WriteLine($"[Chef {Id}] ha servido {platilloNombre} al cliente {ClienteId}.");
                }
            }
        }
    }
}

public static class SimulacionInicia
{
    public static ConcurrentQueue<string> OrderQue { get; } = new ConcurrentQueue<string>();
    public static Dictionary<string, int> Menu { get; } = new Dictionary<string, int>
    {
        { "pizza", 2 },
        { "pasta", 3 },
        { "calzone", 4 }
    };

    private static DateTime? startTime;
    public static void RecordStartTime()
    {
        startTime = DateTime.Now;
    }
}
