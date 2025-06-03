using System;
using System.IO;
using System.Xml;

namespace ZugPferdLite
{
    public class Rechnung
    {
        public string Rechnungsnummer { get; set; } = "";
        public DateTime Datum { get; set; } = DateTime.Now;
        public string Empfaenger { get; set; } = "";
        public decimal Gesamtbetrag { get; set; }
    }

    public static class InvoiceGenerator
    {
        public static string GenerateXml(Rechnung r)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("Invoice");
            doc.AppendChild(root);

            void AddElement(string name, string value)
            {
                var e = doc.CreateElement(name);
                e.InnerText = value;
                root.AppendChild(e);
            }

            AddElement("Number", r.Rechnungsnummer);
            AddElement("Date", r.Datum.ToString("yyyy-MM-dd"));
            AddElement("Recipient", r.Empfaenger);
            AddElement("Amount", r.Gesamtbetrag.ToString("F2"));

            using var sw = new StringWriter();
            doc.Save(sw);
            return sw.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var rechnung = new Rechnung
            {
                Rechnungsnummer = "INV-0001",
                Datum = DateTime.Today,
                Empfaenger = "Musterkunde GmbH",
                Gesamtbetrag = 1250.00m
            };

            var xml = InvoiceGenerator.GenerateXml(rechnung);
            File.WriteAllText("Rechnung_INV-0001.xml", xml);
            Console.WriteLine("XML-Rechnung erstellt: Rechnung_INV-0001.xml");
        }
    }
}
