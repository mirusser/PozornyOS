using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace struktury_pamięci
{
    public class Konfiguracja
    {
        public int rozmiar { get; }
        public int długość_ramki { get; }
        public int wirtualna { get; }
        public int ile_ramek { get; }
        public int ile_stronic { get; }

        public Konfiguracja()
        {
            rozmiar = 256;
            długość_ramki = 16;
            wirtualna = 128;
            ile_ramek = rozmiar / długość_ramki;
            ile_stronic = wirtualna / długość_ramki;
        }
    }

    public class Ramka
    {
        public int adres_początkowy { get; set; }
        public int adres_końcowy { get; set; }
        public Boolean zajęta { get; set; }

        public Ramka()
        {
            zajęta = false;
        }
    }

    public class Strona
    {
        public int numer { get; set; } //na jaką ramkę/stronę wskazuje
        public bool poprawność { get; set; } // 1-fizyczna 0 -wirtualna
        public bool ochrona { get; set; } //1-tylko czytanie 0-czytanie i zapis
        public int stempel_czasowy { get; set; }
    }

}
