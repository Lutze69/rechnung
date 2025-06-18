# ZugPferd-Rechner

## 1. Projektübersicht
Ein Programm, das bestehende PDF- oder Bild-Rechnungen als Vorlage nutzt und daraus digitale ZugPferd-Rechnungen (Light und Comfort) erzeugt bzw. einliest. Primärsprachig in Deutsch; englische Übersetzungen sind vorbereitet und können später aktiviert werden.

---

## 2. Funktionalitäten
- **Unterstützung beider Profile**  
  - **Light**: Erzeugt XML mit Verweis auf externes PDF.  
  - **Comfort**: Erstellt XML mit eingebettetem PDF (altes Layout bleibt erhalten).
- **PDF-/Bild-Einlesung**  
  - Automatische Texterkennung über Textlayer  
  - OCR-Fallback für Bilddateien (JPG, PNG, TIFF)  
- **Vorschau/Thumbnail**  
  - Zeigt eine Vorschau der geladenen PDF-Seite oder des Bildes, um große Dateien performant darzustellen  
- **Manuelle Korrektur**  
  - Markiert unsichere Felder (z. B. Rechnungsnummer, Datum, Beträge) farblich und ermöglicht Nutzerkorrektur  
- **Generierung**  
  - Baut ZugPferd-XML nach gewähltem Profil auf  
  - Kombiniert altes PDF-Layout (Kopf/Logo) mit neuen Rechnungsdaten zu einer finalen PDF-Datei (bei Comfort-Profil)  
- **Validierung**  
  - Prüft das erzeugte XML gegen das offizielle ZugPferd-XSD  
  - Wechselt automatisch zur OCR, wenn kein Textlayer vorhanden ist

---

## 3. Anforderungen
- **Betriebssystem**  
  - Windows 10 oder höher
- **Laufzeitumgebung & Frameworks**  
  - .NET 6 oder höher (WPF für die GUI)  
  - C# als Programmiersprache
- **Bibliotheken & Tools**  
  - **PDF-Verarbeitung**: iTextSharp  
  - **OCR**: Tesseract .NET (Sprachmodell Deutsch; Englisch optional)  
  - **XML**: System.Xml, ZugPferd-XSD (aktuelle Version)
- **Weitere Voraussetzungen**  
  - Ausreichend Arbeitsspeicher (mindestens 4 GB), um Vorschaubilder großer PDFs zu erzeugen  
  - Bei Bedarf Internetzugang, um fehlende OCR-Sprachmodelle (z. B. `eng.traineddata`) herunterzuladen

---

## 4. Installation
1. **.NET 6+ SDK installieren**  
   - Von https://dotnet.microsoft.com herunterladen und installieren  
2. **Programm-Paket herunterladen**  
   - ZIP-Datei entpacken oder MSI-Installer ausführen  
3. **Abhängigkeiten**  
   - Die benötigten Bibliotheken (iTextSharp, Tesseract) sind im NuGet-Paket eingebunden  
4. **Starten**  
   - Doppelklick auf `ZugPferdRechner.exe`  
   - Alternativ über die Kommandozeile:
     ```
     ZugPferdRechner.exe
     ```

---

## 5. Konfiguration & Optionen
- **Sprachumschalter (Deutsch / Englisch)**  
  - Standardmäßig auf Deutsch; Umschalter in der Titelleiste vorhanden, aber initial ausgegraut  
- **Profilwahl**  
  - Light- oder Comfort-Profil über Dropdown-Menü auswählbar  
- **Vorschaubild-Einstellungen**  
  - Thumbnail-Größe (Standard: 200×200 px) kann in `appsettings.json` angepasst werden:
    ```json
    {
      "Vorschaubild": {
        "Breite": 200,
        "Hoehe": 200
      }
    }
    ```
- **OCR-Sprachen**  
  - Standard: Deutsch (`deu.traineddata`)  
  - Englisch (optional): `eng.traineddata` (muss separat heruntergeladen werden)

---

## 6. Module & Workflow

### 6.1. GUI (WPF)
- **Hauptfenster-Aufbau**  
  - Menüleiste:  
    - **Datei** → „PDF/Bild laden“, „Beenden“  
  - Toolbar mit Buttons:  
    - **Load PDF/Bild**  
    - **Daten anzeigen**  
    - **Generieren**
  - Linke Seitenleiste: Formularfelder für  
    - Rechnungsnummer  
    - Rechnungsdatum  
    - Empfänger (Name, Adresse)  
    - Positionen (Artikel, Menge, Einzelpreis)  
    - Steuersatz (Dropdown 19 % / 7 %)  
  - Rechter Hauptbereich:  
    - Vorschaubild der aktuell geladenen PDF-Seite oder des Bildes  
  - Footer:  
    - Statusleiste mit Hinweisen (z. B. „Textlayer vorhanden“, „OCR-Modus aktiv“)
- **Sprachumschalter**  
  - Dropdown („DE | EN“) oben rechts, initial ausgegraut

### 6.2. PDF-Reader- & Vorschaubild-Modul
1. **Dateiauswahl**  
   - Nutzer wählt PDF- oder Bilddatei (JPG/PNG/TIFF) über Öffnen-Dialog  
   - Drag-&-Drop in das Vorschaubereich-Element möglich
2. **Vorschaubild erzeugen**  
   - Erste Seite der PDF als gerastertes Bild laden (iTextSharp)  
   - Thumbnail (z. B. 200×200 px) im Image-Control anzeigen  
3. **Textlayer-Prüfung**  
   - Prüft, ob PDF-Textlayer vorhanden ist:  
     - **Ja**: Direkte Text-Extraktion (siehe Modul 6.4)  
     - **Nein**: Automatischer Wechsel zum Bild-Reader/OCR-Modul (siehe Modul 6.3)

### 6.3. Bild-Reader/OCR-Modul
1. **Rasterbild-Erzeugung**  
   - PDF-Seite bei Bedarf in Bitmap umwandeln (iTextSharp)  
   - Oder bei reinen Bilddateien direkt laden (JPG/PNG)  
2. **OCR mit Tesseract .NET**  
   - Lädt Sprachmodell „Deutsch“ (optional „Englisch“)  
   - Erkennt Textblöcke, liefert Rohtext zurück  
3. **Markierung unsicherer Felder**  
   - OCR-Konfidenzwert < 80 % ⇒ Feld wird im Korrekturformular rot markiert  
   - Zu wenig erkannte Schlüsselwörter ⇒ Nutzerhinweis „Bitte manuell prüfen“

### 6.4. ETL-Modul (Extraktion – Transformation – Laden)
- **Extraktion**  
  - Identifikation von Schlüsselwörtern (z. B. „Rechnungsnummer“, „Rechnungsdatum“, „Gesamtbetrag“)  
  - Liefert erkannte Werte (Strings) ans Transformation-Modul
- **Transformation**  
  - String → C#-Datentyp (DateTime, decimal)  
  - Validierung der Pflichtfelder:  
    - Rechnungsnummer (nicht leer)  
    - Rechnungsdatum (gültiges Datum)  
    - Gesamtbetrag (Dezimalzahl ≥ 0)  
  - Internes Objekt `Rechnung` (C#-Klasse) befüllt:  
    ```csharp
    public class Rechnung {
      public string Rechnungsnummer { get; set; }
      public DateTime Datum { get; set; }
      public string Empfaenger { get; set; }
      public List<Position> Positionen { get; set; }
      public decimal Gesamtbetrag { get; set; }
      // … weitere Felder (Zahlungsziel, Bankverbindung …) …
    }

    public class Position {
      public string Artikel { get; set; }
      public int Menge { get; set; }
      public decimal Einzelpreis { get; set; }
      public decimal Gesamtpreis => Menge * Einzelpreis;
    }
    ```
- **Laden**  
  - Gibt `Rechnung`-Objekt ans Manuelle Korrektur-Modul weiter, wenn Fehler oder unsichere Felder vorliegen  
  - Andernfalls direkt ans Generierungs-Modul

### 6.5. Manuelles Korrektur-Modul
- **Anzeige erkannter Felder**  
  - DataGrid mit Spalten:  
    - **Feldname** – Name des Rechnungsfelds (z. B. „Rechnungsnummer“)  
    - **Erkannt (String)** – Vom Parser/OCR erkannter Wert  
    - **Korrektur (Editierbar)** – Eingabefeld, um Wert anzupassen  
    - **Status** – „OK“ (Grün) oder „Unsicher“ (Rot)
- **Korrektur**  
  - Nutzer klickt auf ein Feld, gibt manuelle Anpassung ein  
  - Ungültige Eingaben (z. B. ungültiges Datum) werden sofort rot unterlegt  
- **Speichern der Korrekturen**  
  - Klick auf „Änderungen übernehmen“ schreibt korrigierte Werte ins `Rechnung`-Objekt

### 6.6. Generierungs-Modul
- **Profilwahl**  
  - Dropdown-Menü: Light oder Comfort
- **XML-Builder**  
  - Bei Light:  
    - Erstellt XML nach ZugPferd-XSD  
    - Fügt `<ram:IncludedNote>` mit Pfad zum externen PDF ein  
  - Bei Comfort:  
    - Erzeugt XML wie bei Light  
    - Liest finale PDF-Datei ein, kodiert sie Base64 in `<ram:EmbeddedDocumentBinaryObject>`  
  - Validierung des XML gegen ZugPferd-XSD (Fehlerdialog mit Zeilen-/Elementangabe bei Verstoß)
- **PDF-Generator**  
  - Lädt Original-PDF-Seiten (iTextSharp) und legt altes Layout als Hintergrund  
  - Baut Rechnungstabelle über dem Kopfbereich ein:  
    - Spalten: Artikel | Menge | Einzelpreis | Gesamtpreis  
    - Summenzeile: Netto-Betrag, MwSt-Betrag, Brutto-Betrag  
    - Fußzeile: Zahlungsinformationen, Bankverbindung, Freitext  
  - Exportiert finale PDF in den Ausgabeordner (z. B. `C:\ZugPferd\Output\`)
- **ZugPferd-Paket (bei Comfort)**  
  - Erstellt ZIP-Datei `Rechnung_<Nr>_ZugPferd.zip` im Ausgabeordner, die XML und PDF enthält

### 6.7. Validierungs-/Fehlerbehandlungs-Modul
- **PDF-Validierung**  
  - Prüft Lesbarkeit (iTextSharp)  
  - Bei Problem: Statusanzeige „PDF unlesbar“ und automatischer Wechsel zur OCR  
- **OCR-Genauigkeit**  
  - Felder mit OCR-Konfidenz < 80 % markieren – rote Hinterlegung  
- **XML-Validierung**  
  - Nutzt `XmlReader` + `XmlSchemaSet`  
  - Bei Schema-Verstoß: Anzeige eines Dialogs mit  
    - Betroffenes Element  
    - Zeilennummer  
    - Kurzbeschreibung des Fehlers  
  - Nutzer kann Korrektur vornehmen und erneut validieren
- **Allgemeine Fehlermeldungen**  
  - **Pflichtfeld fehlt** → Pop-up „Pflichtfeld ‹Feldname› ist leer.“  
  - **Generierungsfehler** → Fehler in `error.log` schreiben und Popup informieren

---

## 7. Benutzeroberfläche & Bedienung
1. **Programm starten**  
   - Doppelklick auf `ZugPferdRechner.exe`  
   - Oder über Kommandozeile:
     ```
     ZugPferdRechner.exe
     ```
2. **PDF-/Bild laden**  
   - Klicke auf Button „PDF/Bild laden“ oder ziehe Datei in das Vorschaubereich-Fenster  
   - Vorschaubild wird erzeugt, Statusleiste zeigt „Textlayer vorhanden“ oder „OCR-Modus aktiv“
3. **Automatische Datenerkennung**  
   - Bei vorhandenem Textlayer: Parser extrahiert Daten und füllt Formularfelder  
   - Bei fehlendem Textlayer: OCR-Modul erkennt Text, markiert unklare Felder
4. **Manuelle Korrektur**  
   - Klicke auf Button „Daten anzeigen“  
   - DataGrid zeigt erkannte Felder; unsichere Felder rot markiert  
   - Korrigiere Werte in der Spalte „Korrektur“ und klicke „Änderungen übernehmen“
5. **Profilwahl**  
   - Wähle Light oder Comfort im Dropdown-Menü „ZugPferd-Profil“  
6. **Rechnung generieren**  
   - Klicke auf Button „Generieren“  
   - Bei erfolgreicher Erstellung erscheint Meldung:
     ```
     ZugPferd-Rechnung erfolgreich erstellt unter C:\ZugPferd\Output\
     ```
   - Ausgabeordner enthält:
     ```
     Output/
       Rechnung_<Nummer>.pdf        (finales PDF)
       Rechnung_<Nummer>.xml        (XML-Datei beim Light-Profil)
       Rechnung_<Nummer>_ZugPferd.zip (ZIP mit XML + PDF beim Comfort-Profil)
     ```
7. **Sprache umschalten (DE / EN)**  
   - Umschalter in der Titelleiste (initial ausgegraut)  
   - Später aktivierbar, um GUI auf Englisch umzustellen

---

## 8. Beispielablauf (Comfort-Profil)
1. Nutzer lädt eine alte PDF-Rechnung:  
   ```
   alteRechnung.pdf
   ```
2. Programm zeigt Vorschaubild, erkennt automatisch  
   - Rechnungsnummer: „INV-2025-001“  
   - Datum: „03.06.2025“  
   - Positionen und Beträge (OCR markiert unsichere Felder)
3. Einige Felder (z. B. Gesamtbetrag) sind unsicher (rot markiert)  
4. Nutzer korrigiert den Gesamtbetrag auf „1.250,00 €“  
5. Profil „Comfort“ auswählen  
6. Klick auf „Generieren“  
   - XML-Datei `INV-2025-001.xml` wird erstellt und validiert  
   - Finale PDF `INV-2025-001.pdf` wird erzeugt (altes Layout + neue Daten)  
   - ZIP-Datei `INV-2025-001_ZugPferd.zip` wird erstellt (enthält XML + PDF)
7. Ausgabe erscheint im Ordner `Output/`

---

## 9. Fehlerbehebung
- **Programm startet nicht**  
  - Prüfe, ob .NET 6+ installiert ist  
  - Fehlende DLL: Stelle sicher, dass `iTextSharp.dll` und `Tesseract.dll` im Programmverzeichnis liegen
- **Vorschaubild wird nicht angezeigt**  
  - PDF-Datei ist zu groß (> 50 MB): Passe Thumbnail-Größe in `appsettings.json` an  
  - Prüfe, ob iTextSharp korrekt referenziert wird
- **Keine Datenerkennung**  
  - Prüfe Statusleiste: „Textlayer vorhanden“?  
  - Wenn nicht, teste mit einer Bilddatei (JPG/PNG) und achte auf OCR-Konfidenz
- **XML-Validierungsfehler**  
  - Öffne die erzeugte XML-Datei in einem XML-Editor  
  - Vergleiche mit ZugPferd-XSD; Fehlermeldung im Dialog zeigt Element und Zeilennummer  
  - Korrigiere fehlerhafte Felder über das manuelle Korrektur-Modul und generiere erneut

---

## 10. Beispiele & Code-Schnipsel

### 10.1. appsettings.json
```json
{
  "Vorschaubild": {
    "Breite": 200,
    "Hoehe": 200
  },
  "OCR": {
    "Sprachen": [ "deu", "eng" ]
  }
}
```

### 10.2. C#-Klasse für Rechnung
```csharp
public class Rechnung {
    public string Rechnungsnummer { get; set; }
    public DateTime Datum { get; set; }
    public string Empfaenger { get; set; }
    public List<Position> Positionen { get; set; }
    public decimal Gesamtbetrag { get; set; }
    // Weitere Felder: Zahlungsziel, Bankverbindung, etc.  
}

public class Position {
    public string Artikel { get; set; }
    public int Menge { get; set; }
    public decimal Einzelpreis { get; set; }
    public decimal Gesamtpreis => Menge * Einzelpreis;
}
```

### 10.3. XML-Generierung (Auszug)
```csharp
// XML-Dokument einrichten
XmlDocument doc = new XmlDocument();
XmlElement root = doc.CreateElement("rsm:CrossIndustryInvoice", "urn:ferd:CrossIndustryDocument:invoice:1p0");
doc.AppendChild(root);

// Beispiel: Rechnungsnummer setzen
XmlElement exDoc = doc.CreateElement("ram:ExchangedDocument", root.NamespaceURI);
XmlElement id = doc.CreateElement("ram:ID", root.NamespaceURI);
id.InnerText = rechnung.Rechnungsnummer;
exDoc.AppendChild(id);
root.AppendChild(exDoc);

// ... Weitere Felder füllen nach ZugPferd-XSD ...

// Bei Comfort-Profil: PDF einbetten
byte[] pdfBytes = File.ReadAllBytes("EndgueltigeRechnung.pdf");
XmlElement embedded = doc.CreateElement("ram:IncludedSupplyChainTradeLineItem", root.NamespaceURI);
XmlElement binaryObject = doc.CreateElement("ram:EmbeddedDocumentBinaryObject", root.NamespaceURI);
binaryObject.SetAttribute("mimeCode", "application/pdf");
binaryObject.InnerText = Convert.ToBase64String(pdfBytes);
embedded.AppendChild(binaryObject);
root.AppendChild(embedded);

// XML speichern
doc.Save("Rechnung_INV-2025-001.xml");
```

### 10.4. PDF-Datei speichern mit Namensprüfung
```csharp
using System;
using System.IO;

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
// Hier erfolgt der eigentliche Speichervorgang des PDFs
```

---

## 11. Lizenz & Autor
- **Lizenz:** MIT (freie Nutzung und Anpassung)  
- **Autor:** Dein Name  
- **Kontakt:** deine.email@example.com
