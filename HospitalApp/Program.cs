using HospitalClassesLibrary;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HospitalApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // ── Генерируем список пациентов ───────────────────────────────────
            var пациенты = new List<PatientInfo>();
            var rnd = new Random();
            string[] диагнозы = { "Грипп", "Пневмония", "COVID-19", "Диабет", "Мигрень" };

            for (int i = 0; i < 1000; i++)
                пациенты.Add(new PatientInfo
                {
                    Id = i,
                    Name = $"Пациент {i}",
                    Diagnosis = диагнозы[rnd.Next(диагнозы.Length)]
                });

            // ── Запись в поток и файл ─────────────────────────────────────────
            var сервис = new StreamService<PatientInfo>();
            using var поток = new MemoryStream();
            var прогресс = new Progress<string>(m => Console.WriteLine(m));

            Console.WriteLine("Записываю данные…");
            await сервис.WriteToStreamAsync(поток, пациенты, прогресс);

            поток.Position = 0;                       // «Перематываем» поток
            Console.WriteLine("Копирую в файл…");
            await сервис.CopyFromStreamAsync(поток, "пациенты.json", прогресс);

            // ── Краткая сводка ───────────────────────────────────────────────
            int total = пациенты.Count;               // 1000

            Console.WriteLine();
            foreach (var d in диагнозы)
            {
                int count = await сервис.GetStatisticsAsync(
                                "пациенты.json",
                                p => p.Diagnosis == d);

                Console.WriteLine($"{d,-10} {count} пациентов из {total}");
            }

            Console.WriteLine("\nНажмите Enter, чтобы выйти…");
            Console.ReadLine();
        }
    }
}
