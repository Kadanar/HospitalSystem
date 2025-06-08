using HospitalClassesLibrary;
using System.Text;

namespace HospitalApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Главный метод начал работу в потоке: {Thread.CurrentThread.ManagedThreadId}");

            var пациенты = new List<PatientInfo>();
            var рандом = new Random();
            string[] диагнозы = { "Грипп", "Пневмония", "COVID-19", "Диабет", "Мигрень" };

            for (int i = 0; i < 1000; i++)
            {
                пациенты.Add(new PatientInfo
                {
                    Id = i,
                    Name = $"Пациент {i}",
                    Diagnosis = диагнозы[рандом.Next(диагнозы.Length)]
                });
            }

            var сервис = new StreamService<PatientInfo>();
            using var поток = new MemoryStream();
            var прогресс = new Progress<string>(сообщение => Console.WriteLine(сообщение));

            Console.WriteLine("Запуск записи в поток и копирования в файл...");

            var задачаЗаписи = сервис.WriteToStreamAsync(поток, пациенты, прогресс);
            await Task.Delay(100);
            var задачаКопирования = сервис.CopyFromStreamAsync(поток, "пациенты.json", прогресс);

            await Task.WhenAll(задачаЗаписи, задачаКопирования);

            Func<PatientInfo, bool> фильтр = п => п.Diagnosis == "COVID-19";
            int количество = await сервис.GetStatisticsAsync("пациенты.json", фильтр);

            Console.WriteLine($"Статистика: {количество} пациентов с диагнозом 'COVID-19'");
        }
    }
}
