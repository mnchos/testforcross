using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main()
    { //Ввод файла,проверка файла
        Console.WriteLine("Введите путь к текстовому файлу:");
        string? filePath = Console.ReadLine();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Указанный файл не существует.");
            return;
        }
//время работы и обработка текста 
        DateTime startTime = DateTime.Now;
        string[] triplets = GetTopTriplets(filePath, 10);
        DateTime endTime = DateTime.Now;
        double executionTime = (endTime - startTime).TotalMilliseconds;
//вывод 
        Console.WriteLine("10 самых часто встречающихся триплетов:");
        foreach (string triplet in triplets)
        {
            Console.Write(triplet + ", ");
        }

        Console.WriteLine("\nВремя выполнения: " + executionTime + " миллисекунд.");
    }

    static string[] GetTopTriplets(string filePath, int topCount)
    {//Создаем потокобезопасную коллекцию наших триплетов, доступ к которой можно получать из нескольких потоков
        ConcurrentDictionary<string,int> triplets = new ConcurrentDictionary<string, int>();   
//Используем метод для парралельного выполнения цикла ForEach к коллекции строк.
// Каждая строка обрабатывается в отдельном потоке.
        Parallel.ForEach(File.ReadLines(filePath), line =>
        {
        for (int i = 0; i < line.Length - 2; i++)
            {
            string triplet = line.Substring(i, 3);

            // Игнорируем триплеты, содержащие пробелы
            if (!triplet.Contains(' '))
            {
                triplets.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
            }
         }
        });
//Возвращаем отсортированные триплеты
    return triplets.OrderByDescending(kv => kv.Value)
        .Select(kv => kv.Key)
        .Take(topCount)
        .ToArray();
    }
}
