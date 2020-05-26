using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testprojekt
{
    class Program
    {
        // Es fehlt: Zurück sortieren des endgültigen Arrays (resort)
        // Zur Verfügung stellen als eigenständige Klasse der array mit Display-Elementen übergeben wird
        // Erweiterungen: Skalierung auf Totoaltime erfolgt im Moment relativ(prozentual). Interessant wäre auch eine Absolute (alle Bilder kriegen den Gleichen Wert aufaddiert)
        // Klasse image in eigene Klassendatei extrahieren und erweitern (z.B. Dateipfad)
     
        static void Main(string[] args)
        {

            Image[] images = new Image[10];
            images[0] = new Image(1,500);
            images[1] = new Image(2,1000);
            images[2] = new Image(3,2000);
            images[3] = new Image(4,2000);
            images[4] = new Image(5,2000);
            images[5] = new Image(6,200);
            images[6] = new Image(7,1000);
            images[7] = new Image(8,900);
            images[8] = new Image(9,510);
            images[9] = new Image(10,1000);

            CalcDisplayTime(60000, images);
            Console.ReadKey();
        }

        public static void CalcDisplayTime(int TimeInterval, params Image[] displays)
        {
            int TotalTime = 0;       // Vorgabe der Gesamtzeit
            double IsTotalTime = 0;     // Berechnete neue Gesamtsequenzdauer
            int IsTmpTotalTime = 0;     // Temporäre Gesamtsequenzdauer während Berechnung der Einzelzeiten
            double FitInTimeInterval = 0;    // Gibt an wie oft die Gesamtsequenz pro Minute durchlaufen wird
            int IsInTimeInterval = 0;       // Gibt an wie oft die Sequenz nach Berechnung pro Minute durchlaufen wird
            int i = 0;

            // Gesamtzeit für einen Durchlauf berechnen
            foreach (Image tmp in displays)
            {
                TotalTime += tmp.SollImageTime;
            }
            Console.WriteLine("Total time: {0}", TotalTime);
            // Berechnen des Prozentanteils jeden Bildes am Gesamtbild
            i = 0;
            foreach (Image tmp in displays)
            {
                i++;
                tmp.IstProzent = 100.0 / TotalTime * tmp.SollImageTime;
            }

            // Berechnen wie oft die Gesamtzeit in eine [TimeInterval] passt
            FitInTimeInterval = TimeInterval / TotalTime;
            IsInTimeInterval = Convert.ToInt16(Math.Round(FitInTimeInterval));
            Console.Write($"Total time({TotalTime}ms) fits {FitInTimeInterval} times in {TimeInterval}ms ({IsInTimeInterval} rounded)");

            // Neue Sollgesamtzeit berechnen
            IsTotalTime = TimeInterval / IsInTimeInterval;
            Console.WriteLine($"--> New total time = {IsTotalTime}ms");

            // Sollzeiten bestimmen (Einzelbild und Gesamt)
            i = 0;
            foreach (Image tmp in displays)
            {
                i++;
                tmp.IstImageTime = Convert.ToInt16(IsTotalTime / 100.0 * tmp.IstProzent);    // Neue Sollzeit
                tmp.SollProzent = 100.0 / IsTotalTime * tmp.IstImageTime;                    // Neuer Prozentwert bezogen auf Neue Gesamtzeit
                tmp.ProzentDelta = Math.Abs(tmp.IstProzent - tmp.SollProzent);                    // Delta Prozente
                IsTmpTotalTime += tmp.IstImageTime;
            }
            if ((IsTotalTime - IsTmpTotalTime) != 0)
                Console.Write("Compensate for inaccuracy: ");
            while ((IsTotalTime - IsTmpTotalTime) != 0)
            {
                Array.Sort(displays);
                displays[0].IstImageTime++;

                IsTmpTotalTime = 0;
                i = 0;
                foreach (Image tmp in displays)
                {
                    i++;
                    tmp.SollProzent = 100.0 / IsTotalTime * tmp.IstImageTime;                    // Neuer Prozentwert bezogen auf Neue Gesamtzeit
                    tmp.ProzentDelta = Math.Abs(tmp.IstProzent - tmp.SollProzent);             // Delta Prozente
                    IsTmpTotalTime += tmp.IstImageTime;
                }
                Console.Write(" +");
            }
            i = 0;
            Console.WriteLine("\r\n\nImage    | Duration(old)  | Duration(new)  | %Total(old) | %Total(new) |   msAdd  |   %add");
            foreach (Image tmp in displays)
            {
                i++;
                Console.WriteLine($"Image {tmp.Index, 2} -{tmp.SollImageTime,5}ms\\{IsTotalTime}ms -{tmp.IstImageTime,5}ms\\{TotalTime}ms -   {(tmp.SollProzent),05:f} %   -   {(tmp.IstProzent),05:f} %   -  {tmp.IstImageTime - tmp.SollImageTime,4}    -   {100.0/tmp.SollImageTime*tmp.IstImageTime-100, 02:F}");
            }
            return;
        }
    }

    public class Image : IComparable
    {
        public int Index;           // Index 
        public int SollImageTime;   // Vorgabe
        public int IstImageTime;    // Resultat
        public double SollProzent;  // Vorgabe
        public double IstProzent;   // Resultat
        public double ProzentDelta; // Abweichung des Anteils am Gesamt

        public int CompareTo(Object b)  // Zum soriteren mit Array.Sort(array)
        {
            Image y = (Image)b;

            if (this.ProzentDelta == y.ProzentDelta)
            {
                return 0;
            }
            if ((this.ProzentDelta % 1) < (y.ProzentDelta % 1))
            {
                return -1;
            }
            return 1;
        }

        public Image(int index,int time)  // Initialisiert mit Index und Anzeigedauer
        {
            Index = index;
            SollImageTime = time;
        }
    }
}
