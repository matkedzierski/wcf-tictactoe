using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace KolkoKrzyzyk_UslugaWCF
{
    // UWAGA: możesz użyć polecenia „Zmień nazwę” w menu „Refaktoryzuj”, aby zmienić nazwę klasy „Service1” w kodzie i pliku konfiguracji.
    public class KolkoKrzyzykServer : IGraKolkoKrzyzyk
    {
        const int W = 3;
        const int K = 3;
        static int[,] ArrayPlansza = new int[W, K];
        static List<int> ListaRuchow = new List<int>();
        const int ZnakKolko = 1;
        const int ZnakKrzyzyk = 2;
        static Random rnd = new Random();

        public KolkoKrzyzykServer()
        {
        }

        public void Start()
        {
            ArrayPlansza = new int[W, K];

            ListaRuchow = new List<int>();
            for (int i=0; i<9; i++)
            {
                ListaRuchow.Add(i);
            }
        }

        public bool WykonajRuch(int wiersz, int kolumna, out int wierszServer, out int kolumnaServer)
        {
            wierszServer = -1; kolumnaServer = -1;

            // ruch klienta
            if (ArrayPlansza[wiersz, kolumna] == 0) ArrayPlansza[wiersz, kolumna] = ZnakKolko;
            else return false;
            ListaRuchow.Remove(wiersz * K + kolumna);

            // sprawdzanie remisu
            if (ListaRuchow.Count == 0) return true;

            // ruch serwera
            int x = rnd.Next(0, ListaRuchow.Count - 1);
            int indeks = ListaRuchow[x];
            kolumnaServer = indeks % K;
            wierszServer = (indeks - kolumnaServer) / K;
            ArrayPlansza[wierszServer, kolumnaServer] = ZnakKrzyzyk;
            ListaRuchow.Remove(wierszServer * K + kolumnaServer);



            return true;
        }

        public int SparwdzWygrana(int wiersz, int kolumna)
        {
            int znak = ArrayPlansza[wiersz, kolumna];
            if (
               ArrayPlansza[0, kolumna] == znak && ArrayPlansza[1, kolumna] == znak && ArrayPlansza[2, kolumna] == znak // poziomo
                ) return 1;

            
            if (
               ArrayPlansza[wiersz, 0] == znak && ArrayPlansza[wiersz, 1] == znak && ArrayPlansza[wiersz, 2] == znak // pionowo
                ) return 2;
            

            if (
                ArrayPlansza[0, 0]==znak && ArrayPlansza[1, 1]==znak && ArrayPlansza[2, 2]==znak    // skos \
                ) return 4;
            
            
            if (
                ArrayPlansza[0, 2]==znak && ArrayPlansza[1, 1]==znak && ArrayPlansza[2, 0]==znak    // skos /
                ) return 8;

            return 0;
        }

    }
}
