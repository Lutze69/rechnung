using System;
using System.IO;

class PdfNameCheck
{
    static void Main(string[] args)
    {
        Console.Write("Bitte geben Sie den gewünschten Dateinamen (mit .pdf) ein: ");
        string fileName = Console.ReadLine();

        while (File.Exists(fileName))
        {
            Console.WriteLine($"Die Datei '{fileName}' existiert bereits.");
            Console.Write("Möchten Sie einen anderen Namen eingeben? (j/n): ");
            string reply = Console.ReadLine();
            if (!reply.Equals("j", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Vorgang abgebrochen.");
                return;
            }
            Console.Write("Neuer Dateiname: ");
            fileName = Console.ReadLine();
        }

        Console.WriteLine($"PDF wird unter '{fileName}' gespeichert.");
        // TODO: Speichervorgang des PDFs implementieren
    }
}
