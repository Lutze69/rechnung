using System;
using ZugPferdLite;
using Xunit;

namespace ZugPferdLite.Tests
{
    public class InvoiceGeneratorTests
    {
        [Fact]
        public void GenerateXml_CreatesRootInvoiceElement()
        {
            var r = new Rechnung
            {
                Rechnungsnummer = "INV-0001",
                Datum = new DateTime(2025, 6, 3),
                Empfaenger = "Test Kunde",
                Gesamtbetrag = 100m
            };

            var xml = InvoiceGenerator.GenerateXml(r);

            Assert.Contains("<Invoice>", xml);
            Assert.Contains("<Number>INV-0001</Number>", xml);
        }
    }
}
