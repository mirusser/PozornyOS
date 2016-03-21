using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moduł_3_Zarządzanie_procesami_wyższy;
using semafory_pamięci;
using struktury_pamięci;
using OperationSystem_DiscSystem.Module_4.Source;

namespace moduł_pamięci
{
    class Pamięć
    {
       
        #region dla innych modułów
        public bool XA(ref PCB pcb)
        {
            int ile_komórek_potrzeba = pcb.Auto_storage_size;

            int ile_ramek = ile_potrzeba_ramek(pcb.Auto_storage_size); //w ilu ramek w sumie program zajmie
            ile_ramek += ile_potrzeba_ramek(pcb.Auto_data_size);
            ile_ramek += ile_potrzeba_ramek(pcb.Auto_stack_size);
            int ile_należy_się_ramek = ile_ramek_przydzielić(ile_ramek); //ile ramek się programowi nalezy zgodnie z przydziałem proporcjonalnym
            int ile_należy_się_stronic = ile_ramek - ile_należy_się_ramek; //jezeli jest wymaganych więcej ramek niż proporcjonalnie może dostać proces, pozostałe ramki zostaną przesunięte do pamięci wirtualnej

            if (ile_ramek <= wolne_ramki.Count + wolne_stronice.Count) //jeżeli starczy ramek (uwzględniając przenoszenie do stronic) na proces to alokuję pamięć
            {
                przydziel_pamięć_programowi(ref pcb);

                return true;
            }
            else if (ile_ramek > tablica_ramek.Count + tablica_stronnic.Count)
            {
                Exception a = new Exception();
                throw a;
            }
            else
            {
                memory.P(ref pcb);

                return false;
            }
        }

        public bool XF(ref PCB pcb)
        {
            zwolnij_przydzieloną_pamięć_programowi(pcb.Auto_storage_addres, pcb.Auto_storage_size);
            int wartość_semafora_memory = memory.getWartość;
            if (wartość_semafora_memory < 0)
            {
                for (int i = wartość_semafora_memory; i < 0; i++)
                {
                    PCB bufor = memory.V();
                    XA(ref bufor);
                }
            }
            return true;
        }

        public string zapisz_element_stosu(int adres_logiczny, string na_stos) //zapisuje dwuznakowy element stosu pod konkretnym adresem, adres ostatniego elementu przechowywany jest w procesorze
        {
            //sprawdzam, czy element jest dwuznakowy
            if (na_stos.Length != 2)
            {
                Console.WriteLine("Element stosu, który próbujesz dodać nie składa się z dwóch znaków. Nie mozna go dodać na stos.");
                return "-2";
            }

            //konwertuję adres logiczny aby wiedzieć do stosu którego programu się odnieść
            int zbiór = adres_logiczny / 1000;
            int bufor = adres_logiczny % 1000;
            int strona = bufor / konfiguracja.długość_ramki;
            int element = bufor % konfiguracja.długość_ramki;

            //sprawdzam, czy adres jest prawidłowy
            try
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy");
                return "-1";
            }

            //sprawdzam, czy w danej ramce można coś zapisywać
            if (TablicaZbiorów[zbiór][strona].ochrona == true)
            {
                Console.WriteLine("Komórka znajdująca się pod podanym adresem nie może być zapisana");
                return "-3";
            }

            //teraz zapisuję
            if (TablicaZbiorów[zbiór][strona].poprawność == true) //jeżeli adres znajduje się w pamięci operacyjnej
            {
                int adres_fizyczny = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                pamięć_fizyczna[adres_fizyczny] = na_stos[0];
                pamięć_fizyczna[adres_fizyczny+1] = na_stos[1];
                return "0";
            }
            else //jeżeli nie
            {
                int adres_fizyczny = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                Controller.setCharArrayToFile("swap", adres_fizyczny + 1, na_stos);
                return "0";
            }
        }

        public string odczytaj_element_stosu(int adres_logiczny)
        {
            int zbiór = adres_logiczny / 1000; //odczytujemy który element na liscie tablic stron (tablic zbiorów)
            int bufor = adres_logiczny % 1000; //pozostała część adresu logicznego zawierająca informacje o nr strony i elementu
            int strona = bufor / konfiguracja.długość_ramki; //wydobycie z powyższego nr strony
            int element = bufor % konfiguracja.długość_ramki;

            try //sprawdzamy czy taki adres jest prawidłowy i czy istnieje, jeżeli nie istnieje to złapany zostanie wyjątek
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy.");
                return "-1";
            }

            //sprawdzam, czy w danej ramce można coś odczytywać tą funkcją
            if (TablicaZbiorów[zbiór][strona].ochrona == true)
            {
                Console.WriteLine("Komórka znajdująca się pod podanym adresem nie może być odczytana");
                return "-3";
            }

            /*Teraz sprawdzam, czy szukana strona wskazuje na pamięć fizyczną, jeżeli tak to ja odczytuję, a jak nie to z pamięci wirtualnej przenoszę do fizycznej lub zamieniam ramkę z wiertualnej na ramkę z fizycznej*/
            if (TablicaZbiorów[zbiór][strona].poprawność == true) //jeżeli jest w fizycznej
            {
                int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;

                TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                char[] odczytane = new char[2]; //2 bo element jest dwuelementowy

                for (int i = 0; i < 2; i++)
                {
                    odczytane[i] = pamięć_fizyczna[początek + i];
                }

                string zwracanie = new string(odczytane);

                return zwracanie;
            }
            else //jeżeli jest w wirtualnej
            {
                if (wolne_ramki.Count > 0) //jeżeli mamy jakies wolne ramki to stronę przenoszę do wolnej ramki
                {
                    int wolna_ramka = znajdź_wolną_ramkę(); //znajdujemy wolną ramkę, ramka od razu jest ustawiona jako uzyta
                    string zawartośc_stronicy = pobierz_stronicę(TablicaZbiorów[zbiór][strona].numer); //przepisujemy jej zawartość, oznaczamy ją jako nieużywaną i dodajemy do stosu wolnych stronic; od tego momentu stronica jest zbędna

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                    zapisz_ramkę(wolna_ramka, zawartośc_stronicy); //zapisujemy bufor w ramce

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy(); //skoro dokonujemy operacji na komórce musimy zaktualizować stempel czasowy jej strony
                    TablicaZbiorów[zbiór][strona].numer = wolna_ramka;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;

                    char[] odczytane = new char[2];

                    for (int i = 0; i < 2; i++)
                    {
                        odczytane[i] = pamięć_fizyczna[początek + i];
                    }

                    string zwracanie = new string(odczytane);

                    return zwracanie;
                }
                else //jeżeli nie mamy wolnych ramek to trzeba zamienić stronicę z ramką zawartościami
                {
                    int ramka_do_użycia = zastępowanie_stron(TablicaZbiorów[zbiór][strona].numer);
                    TablicaZbiorów[zbiór][strona].numer = ramka_do_użycia;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                    char[] odczytane = new char[2];

                    for (int i = 0; i < 2; i++)
                    {
                        odczytane[i] = pamięć_fizyczna[początek + i];
                    }

                    string zwracanie = new string(odczytane);

                    return zwracanie;
                }
            }
        }

        public string zapisz_do_pamięci_operacyjnej(int adres_logiczny, string dane)
        {
            //sprawdzam, czy element jest dwuznakowy
            if (dane.Length != 2)
            {
                Console.WriteLine("Dane, które próbujesz dodać nie składają się z dwóch znaków.");
                return "-2";
            }

            //konwertuję adres logiczny aby wiedzieć do stosu którego programu się odnieść
            int zbiór = adres_logiczny / 1000;
            int bufor = adres_logiczny % 1000;
            int strona = bufor / konfiguracja.długość_ramki;
            int element = bufor % konfiguracja.długość_ramki;

            //sprawdzam, czy adres jest prawidłowy
            try
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy");
                return "-1";
            }

            //sprawdzam, czy w danej ramce można coś zapisywać
            if (TablicaZbiorów[zbiór][strona].ochrona == true)
            {
                Console.WriteLine("Komórka znajdująca się pod podanym adresem nie może być zapisana.");
                return "-3";
            }
            
            //teraz zapisuję
            if (TablicaZbiorów[zbiór][strona].poprawność == true) //jeżeli adres znajduje się w pamięci operacyjnej
            {
                int adres_fizyczny = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                pamięć_fizyczna[adres_fizyczny] = dane[0];
                pamięć_fizyczna[adres_fizyczny+1] = dane[1];
                return "0";
            }
            else //jeżeli nie
            {
                int adres_fizyczny = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                Controller.setCharArrayToFile("swap", adres_fizyczny + 1, dane);
                return "0";
            }
        }

        public string odczytaj_z_pamięci_operacyjnej(int adres_logiczny)
        {
            int zbiór = adres_logiczny / 1000; //odczytujemy który element na liscie tablic stron (tablic zbiorów)
            int bufor = adres_logiczny % 1000; //pozostała część adresu logicznego zawierająca informacje o nr strony i elementu
            int strona = bufor / konfiguracja.długość_ramki; //wydobycie z powyższego nr strony
            int element = bufor % konfiguracja.długość_ramki;

            try //sprawdzamy czy taki adres jest prawidłowy i czy istnieje, jeżeli nie istnieje to złapany zostanie wyjątek
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy.");
                return "-1";
            }

            /*Teraz sprawdzam, czy szukana strona wskazuje na pamięć fizyczną, jeżeli tak to ja odczytuję, a jak nie to z pamięci wirtualnej przenoszę do fizycznej lub zamieniam ramkę z wiertualnej na ramkę z fizycznej*/
            if (TablicaZbiorów[zbiór][strona].poprawność == true) //jeżeli jest w fizycznej
            {
                TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;

                char[] odczytane = new char[2];

                for (int i = 0; i < 2; i++)
                {
                    odczytane[i] = pamięć_fizyczna[początek + i];
                }

                string zwracanie = new string(odczytane);

                return zwracanie;
            }
            else //jeżeli jest w wirtualnej
            {
                if (wolne_ramki.Count > 0) //jeżeli mamy jakies wolne ramki to stronę przenoszę do wolnej ramki
                {
                    int wolna_ramka = znajdź_wolną_ramkę(); //znajdujemy wolną ramkę, ramka od razu jest ustawiona jako uzyta
                    string zawartośc_stronicy = pobierz_stronicę(TablicaZbiorów[zbiór][strona].numer); //przepisujemy jej zawartość, oznaczamy ją jako nieużywaną i dodajemy do stosu wolnych stronic; od tego momentu stronica jest zbędna

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                    zapisz_ramkę(wolna_ramka, zawartośc_stronicy); //zapisujemy bufor w ramce

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy(); //skoro dokonujemy operacji na komórce musimy zaktualizować stempel czasowy jej strony
                    TablicaZbiorów[zbiór][strona].numer = wolna_ramka;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;

                    char[] odczytane = new char[2];

                    for (int i = 0; i < 2; i++)
                    {
                        odczytane[i] = pamięć_fizyczna[początek + i];
                    }

                    string zwracanie = new string(odczytane);

                    return zwracanie;
                }
                else //jeżeli nie mamy wolnych ramek to trzeba zamienić stronicę z ramką zawartościami
                {
                    int ramka_do_użycia = zastępowanie_stron(TablicaZbiorów[zbiór][strona].numer);
                    TablicaZbiorów[zbiór][strona].numer = ramka_do_użycia;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    int początek = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                    char[] odczytane = new char[2];

                    for (int i = 0; i < 2; i++)
                    {
                        odczytane[i] = pamięć_fizyczna[początek + i];
                    }

                    string zwracanie = new string(odczytane);

                    return zwracanie;
                }
            }
        }

        public string zapisz_program_do_pamięci(int adres_logiczny, string kod_programu) //poprawić!!!!!!!!!!!!!!!!!!!!!!
        {
            int zbiór = adres_logiczny / 1000; //odczytujemy który element na liscie tablic stron (tablic zbiorów)
            int bufor = adres_logiczny % 1000; //pozostała część adresu logicznego zawierająca informacje o nr strony i elementu
            int strona = bufor / konfiguracja.długość_ramki; //wydobycie z powyższego nr strony
            int element = bufor % konfiguracja.długość_ramki; //a tutaj nr elementu

            try //sprawdzamy czy taki adres jest prawidłowy i czy istnieje, jeżeli nie istnieje to złapany zostanie wyjątek
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy. Nie odnaleziono zadanego wpisu w tablicy stronic procesu.");
                return "-1";
            }

            if (TablicaZbiorów[zbiór][strona].ochrona == false) //jeżeli komórka jest przydzielona pamieci operacyjnej
            {
                Console.WriteLine("Odmowa dostępu. Komórka o podanym adresie przeznaczona jest dla pamięci operacyjnej a nie dla programu.");
                return "-2";
            }

            //sprawdzamy, czy istnieje adres końcowy - zabezpieczenie gdyby ktoś chciał zapisać kod programu zaczynając od dalszej komórki lub zbyt duży kod
            int ramki = ile_potrzeba_ramek(kod_programu.Length);
            try
            {
                int a = TablicaZbiorów[zbiór][strona + ramki - 1].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy. Nie odnaleziono zadanego wpisu w tablicy stronic procesu.");
                return "-1";
            }

            //sprawdzam również, czy dalsze komórki nie są chronione- jeżeli nie są, nie są przeznaczone na kod programu
            if (TablicaZbiorów[zbiór][strona + ramki - 1].ochrona == false)
            {
                Console.WriteLine("Kod Twojego programu zachodzi na pamięć zarezerwowaną dla innych struktur danych.");
                return "-3";
            }

            int długość_ostatniej;
            if (kod_programu.Length % konfiguracja.długość_ramki == 0)
            {
                długość_ostatniej = konfiguracja.długość_ramki;
            }
            else
            {
                długość_ostatniej = kod_programu.Length % konfiguracja.długość_ramki;
            }

            for (int i = 0; i < ramki; i++)
            {
                

                if (TablicaZbiorów[zbiór][i].poprawność == true) //jeżeli adres znajduje się w pamięci operacyjnej
                {
                    if (i == ramki-1)//ostatnia zapisaywana ramka/strona moze nie byc zapisana w całości
                    {

                        string substring = kod_programu.Substring(i * konfiguracja.długość_ramki, długość_ostatniej);
                        zapisz_ramkę(TablicaZbiorów[zbiór][i].numer, substring);
                    }
                    else
                    {
                        string substring = kod_programu.Substring(i * konfiguracja.długość_ramki, konfiguracja.długość_ramki);
                       zapisz_ramkę(TablicaZbiorów[zbiór][i].numer, substring);
                    }

                }
                else
                {
                    if (i == ramki-1)//ostatnia zapisaywana ramka/strona moze nie byc zapisana w całości
                    {
                        string substring = kod_programu.Substring(i * konfiguracja.długość_ramki, długość_ostatniej);
                        zapisz_stronnicę(TablicaZbiorów[zbiór][i].numer, substring);
                    }
                    else
                    {
                        string substring = kod_programu.Substring(i * konfiguracja.długość_ramki, konfiguracja.długość_ramki);
                        zapisz_stronnicę(TablicaZbiorów[zbiór][i].numer, substring);
                    }
                }
                TablicaZbiorów[zbiór][i].stempel_czasowy = getStempelCzasowy();
            }

            return "0";
        }

        public string odczytaj_komórkę_programu(int logiczny)
        {
            int zbiór = logiczny / 1000; //odczytujemy który element na liscie tablic stron (tablic zbiorów)
            int bufor = logiczny % 1000; //pozostała część adresu logicznego zawierająca informacje o nr strony i elementu
            int strona = bufor / konfiguracja.długość_ramki; //wydobycie z powyższego nr strony
            int element = bufor % konfiguracja.długość_ramki; //a tutaj nr elementu

            if (TablicaZbiorów[zbiór][strona].ochrona == false)
            {
                Console.WriteLine("Pod tym adresem nie znajduje się kod");
                return "-3";
            }

            try //sprawdzamy czy taki adres jest prawidłowy i czy istnieje, jeżeli nie istnieje to złapany zostanie wyjątek
            {
                int a = TablicaZbiorów[zbiór][strona].numer;
            }
            catch
            {
                Console.WriteLine("Podany adres jest nieprawidłowy.");
                return "-1";
            }

            /*Teraz sprawdzam, czy ramka do której należy komórka jest w pamięci fizycznek, jeżeli tak to ja odczytuję, a jak nie to z pamięci wirtualnej przenoszę do fizycznej lub zamieniam ramkę z wiertualnej na ramkę z fizycznej*/
            if (TablicaZbiorów[zbiór][strona].poprawność == true) //jeżeli jest w fizycznej
            {
                TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();
                int przetłumaczony = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                return pamięć_fizyczna[przetłumaczony].ToString();
            }
            else //jeżeli jest w wirtualnej
            {
                if (wolne_ramki.Count > 0) //jeżeli mamy jakies wolne ramki to stronę przenoszę do wolnej ramki
                {
                    int wolna_ramka = znajdź_wolną_ramkę(); //znajdujemy wolną ramkę, ramka od razu jest ustawiona jako uzyta
                    string zawartośc_stronicy = pobierz_stronicę(TablicaZbiorów[zbiór][strona].numer); //przepisujemy jej zawartość, oznaczamy ją jako nieużywaną i dodajemy do stosu wolnych stronic; od tego momentu stronica jest zbędna

                    zapisz_ramkę(wolna_ramka, zawartośc_stronicy); //zapisujemy bufor w ramce

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy(); //skoro dokonujemy operacji na komórce musimy zaktualizować stempel czasowy jej strony

                    int przetłumaczony = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                    TablicaZbiorów[zbiór][strona].poprawność = true;
                    return pamięć_fizyczna[przetłumaczony].ToString();
                }
                else //jeżeli nie mamy wolnych ramek to trzeba zamienić stronicę z ramką zawartościami
                {
                    int ramka_do_użycia = zastępowanie_stron(TablicaZbiorów[zbiór][strona].numer);
                    TablicaZbiorów[zbiór][strona].numer = ramka_do_użycia;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    int przetłumaczony = TablicaZbiorów[zbiór][strona].numer * konfiguracja.długość_ramki + element;
                    TablicaZbiorów[zbiór][strona].poprawność = true;

                    TablicaZbiorów[zbiór][strona].stempel_czasowy = getStempelCzasowy();

                    return pamięć_fizyczna[przetłumaczony].ToString();
                }
            }
        }

        #endregion

        #region do testowania

        public void wyświetl_zawartość_ramek()
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICARAMEK:");
            System.Console.WriteLine("Adres początkowy  Adres Końcowy   Zajęta? Zawartość", " ");
            for (int i = 0; i < konfiguracja.rozmiar / konfiguracja.długość_ramki; i++)
            {
                System.Console.Write("\t");
                System.Console.Write(tablica_ramek[i].adres_początkowy.ToString());
                System.Console.Write("\t\t");
                System.Console.Write(tablica_ramek[i].adres_końcowy);
                System.Console.Write("\t   ");
                System.Console.Write(tablica_ramek[i].zajęta);
                System.Console.Write("\t");


                for (int j = 0; j < konfiguracja.długość_ramki; j++)
                {
                    System.Console.Write(pamięć_fizyczna[i * konfiguracja.długość_ramki + j].ToString()); //bezposrednie odczytanie bez zmiany stempla czasowego
                }
                System.Console.Write("\n");
            }
            System.Console.WriteLine("------------------------------------");
        }

        public void wyświetl_zawartość_stronnic()
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICASTRONNIC:");
            System.Console.WriteLine("AdresPoczątkowy AdresKońcowy Zajęta?");
            for (int i = 0; i < konfiguracja.ile_stronic; i++)
            {
                Console.Write("\t{0}\t{1}\t{2}\t", tablica_stronnic[i].adres_początkowy, tablica_stronnic[i].adres_końcowy, tablica_stronnic[i].zajęta);
                System.Console.Write(odczytaj_stronicę(i));

                System.Console.Write("\n");
            }


            System.Console.WriteLine("------------------------------------");
        }

        public void wyświetl_tablicę_stron(ref PCB pcb)
        {
            int numerzbioru = pcb.Auto_storage_addres / 1000;

            wyświetl_tablicę_stron(TablicaZbiorów[numerzbioru]);
        }

        public void wyświetl_tablicę_stron(List<Strona> TablicaStron)
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICASTRON:");
            System.Console.WriteLine("NumerStrony NumerRamki Umieszczenie Ochrona Stempel");

            for (int i = 0; i < TablicaStron.Count; i++)
            {
                Console.WriteLine("\t{0}\t{1}\t\t{2}\t{3}\t\t{4}", i, TablicaStron[i].numer, TablicaStron[i].poprawność, TablicaStron[i].ochrona, TablicaStron[i].stempel_czasowy);
            }
            System.Console.WriteLine("------------------------------------");
        }

        public void wyświetl_stan_semafora_memory()
        {
            memory.wyświetl_stan_semafora_memory();
        }

        #endregion

        #region tymczasowe

        int getStempelCzasowy()
        {
            System.Threading.Thread.Sleep(1);
            return DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        }

        #endregion

        #region komórki pamięci, wszystkie listy i semafory

        Konfiguracja konfiguracja = new Konfiguracja();

        #region komórki pamięci
        char[] pamięć_fizyczna;
        #endregion

        #region semafory
        MEMORY memory = new MEMORY();

        #endregion

        #region ramki
        List<Ramka> tablica_ramek = new List<Ramka>();
        Stack<int> wolne_ramki = new Stack<int>();
        #endregion

        #region stronic
        Stack<int> wolne_stronice = new Stack<int>();
        List<Ramka> tablica_stronnic = new List<Ramka>();
        #endregion

        #endregion

        #region konstruktor i inicjalizacja list i stosów
        public Pamięć()
        {
            pamięć_fizyczna = new char[konfiguracja.rozmiar];

            for (int i = 0; i < konfiguracja.rozmiar; i++)
            {
                pamięć_fizyczna[i] = '-';
            }

            string bufor = "-";
            for (int i=0; i<konfiguracja.wirtualna-1; i++)
            {
                bufor += '-';
            }
            Controller.addNewFile("swap", bufor);

            inicjalizacja_listy_ramek();
            inicjalizacja_listy_stronic();
            inicjalizacja_wolnych_ramek();
            inicjalizacja_tablicy_zbiorów();
            inicjalizacja_wolnych_stronic();
        }

        void inicjalizacja_wolnych_stronic() //zapełnia stos wolnych stronic, aby odpowiednie funkcje mogły po prostu zdjąć by znać numer aktualnie wolnej stronicy
        {
            for (int i = konfiguracja.wirtualna / konfiguracja.długość_ramki - 1; i >= 0; i--)
            {
                wolne_stronice.Push(i);
            }
        }
        void inicjalizacja_listy_ramek() //inicjalizuje listę ramek aby zawierała odpowiednie wskazania na komórki pamięci
        {
            for (int i = 0; i < konfiguracja.rozmiar / konfiguracja.długość_ramki; i++)
            {
                Ramka ramka = new Ramka();
                ramka.adres_początkowy = i * konfiguracja.długość_ramki;
                ramka.adres_końcowy = i * konfiguracja.długość_ramki + konfiguracja.długość_ramki - 1;
                tablica_ramek.Add(ramka);
            }
        }
        void inicjalizacja_listy_stronic() //inicjalizuje listę stronic (ramek pamięci na dysku) aby zawierała odpowiednie wskazania na komórek na dysku
        {
            for (int i = 0; i < konfiguracja.wirtualna / konfiguracja.długość_ramki; i++)
            {
                Ramka stronica = new Ramka();
                stronica.adres_początkowy = i * konfiguracja.długość_ramki;
                stronica.adres_końcowy = i * konfiguracja.długość_ramki + konfiguracja.długość_ramki - 1;
                tablica_stronnic.Add(stronica);
            }
        }
        void inicjalizacja_wolnych_ramek() //zapełnia stos wolnych ramek, aby odpowiednie funkcje mogły bez problemu zdjąć by znać numer aktualnie wolnej ramki
        {
            for (int i = konfiguracja.rozmiar / konfiguracja.długość_ramki - 1; i >= 0; i--)
            {
                wolne_ramki.Push(i);
            }
        }
        void inicjalizacja_tablicy_zbiorów() //inicjalizuję listę tablic stronic (dla ułatwienia nazwałem ja "tablica zbiorów") aby algorytmy wyszukiwania nie trafiły na koniec listy i nie wyrzuciły błędu
        {
            int ile_zbiorów = konfiguracja.rozmiar / konfiguracja.długość_ramki + konfiguracja.wirtualna / konfiguracja.długość_ramki;

            for (int i = 0;  i < ile_zbiorów; i++)
            {
                List<Strona> TablicaStron = new List<Strona>();
                TablicaZbiorów.Add(TablicaStron);
            }

            for (int i = ile_zbiorów-1; i >= 0; i--)
            {
                wolne_zbiory.Push(i);
            }
        }
        #endregion

        #region operacje na ramkach

        string odczytaj_ramkę(int ramka) //odczytuje ramkę o zadanym numerze i zwraca jej zawartość w stringu
        {
            char[] bufor = new char[konfiguracja.długość_ramki]; //tworzę bufor na odczytane dane

            for (int i = 0; i < konfiguracja.długość_ramki; i++)
            {
                bufor[i] = pamięć_fizyczna[ramka * konfiguracja.długość_ramki + i]; //odczytuję całą ramkę
            }

            string zwracanie = new string(bufor); //tworzę stringa z tablicy charów (były problemy z funkcją ToString)
            return zwracanie; //zwracam stringa
        }

        void zapisz_ramkę(int numer_ramki, string znaki) //zapisuje znaki w konkretnej ramce (odpowiadającej dostarczonemu segmentowi); nie zmienia bitu używania na true; jest to zmieniane przy wyszukiwaniu wolnej ramki
        {
            int adres = numer_ramki * konfiguracja.długość_ramki;

            for (int i = 0; i < znaki.Length; i++)
            {
                pamięć_fizyczna[adres + i] = znaki[i];
            }
        }


        int znajdź_wolną_ramkę() //znajduje pierwszą wolną ramkę (pobiera nr ze stosu); zwraca jej numer i oznacza jako zajętą
        {
            int wolna_ramka = wolne_ramki.Pop();
            tablica_ramek[wolna_ramka].zajęta = true;
            return wolna_ramka;
        }
        #endregion

        #region operacje na stronicach

        int znajdź_wolną_stronnicę()
        {
            int znaleziona = wolne_stronice.Pop();
            tablica_stronnic[znaleziona].zajęta = true;
            return znaleziona;
        }

        void zapisz_stronnicę(int numer_stronicy, string wartość) //zapisuje konkretną stronicę, zakładamy, że jej numeru nie ma na stosie wolnych stronic
        {
            if (wartość.Length == 0)
            {
                Console.WriteLine("Zapisujesz u Krzycha pustego stringa!");
            }
            Controller.setCharArrayToFile("swap", tablica_stronnic[numer_stronicy].adres_początkowy + 1, wartość);
        }

        string odczytaj_stronicę(int numer) //odczytuje stronicę z pamięci wirtualnej, nie aksuje jej
        {
            string bufor = Controller.getCharArrayFromFile("swap", tablica_stronnic[numer].adres_początkowy + 1, konfiguracja.długość_ramki);
            return bufor;
        }

        string pobierz_stronicę(int numer) //zwraca stronicę z pamięci wirtualnej, kasuje ją z niej
        {
            wolne_stronice.Push(numer); //dodajemy nr stronicy do stosu wolnych stronic
            tablica_stronnic[numer].zajęta = false; //oznaczamy stronicę jako wolną
            return Controller.getCharArrayFromFile("swap", tablica_stronnic[numer].adres_początkowy + 1, konfiguracja.długość_ramki);
        }

        int zastępowanie_stron(int numer_stronicy) //podaję którą stronicę trzeba zastąpić, zwracany jest nr nowej ramki
        {
            int index1 = 0;
            int index2 = 0;
            int stempel = -1;

            for (int i = 0; i < TablicaZbiorów.Count; i++)
            {
                for (int j = 0; j < TablicaZbiorów[i].Count; j++)
                {
                    if (TablicaZbiorów[i][j].poprawność == true)
                    {
                        if (TablicaZbiorów[i][j].stempel_czasowy < stempel || stempel == -1)
                        {
                            index1 = i;
                            index2 = j;
                            stempel = TablicaZbiorów[i][j].stempel_czasowy;
                        }
                    }
                }
            }
            //najstarsza to TablicaZbiorów[i][j].numer
            int najstarsza = TablicaZbiorów[index1][index2].numer; 
            string ramka = odczytaj_ramkę(najstarsza); //zapisuję zawartość najstarszej
            TablicaZbiorów[index1][index2].numer = numer_stronicy; //od teraz strona wskazująca na ramkę wskazuje na stronicę
            TablicaZbiorów[index1][index2].poprawność = false; //oznaczam, że od teraz jest w pomocniczej
            string stronica = odczytaj_stronicę(numer_stronicy); //zanim zapiszę stronicę to muszę zachować zawartość stronicy którą chcę przenieść do ramki
            zapisz_stronnicę(numer_stronicy, ramka); //zapisuję zawartość tej starej ramki w stronnicy
            //nowa ramka jest gotowa do użycia
            zapisz_ramkę(najstarsza, stronica); //zapisuję zawartość stronicy w ramce
            //nie wiem, w której tablicy stronic jest nowa ramka, więc to będzie trzeba przypisać w funkcji
            return najstarsza;
            
        }

        #endregion

        #region przydział pamięci

        int ile_potrzeba_ramek(int ile) //zwraca liczbę wymaganych ramek/stronnic wymaganych do przechowania zadanej ilości znaków
        {
            if (ile <= 0)
            {
                return 0;
            }
            int ilość_ramek = ile / konfiguracja.długość_ramki;
            if (ile % konfiguracja.długość_ramki != 0)
            {
                ilość_ramek++;
            }

            return ilość_ramek;
        }

        List<List<Strona>> TablicaZbiorów = new List<List<Strona>>(); //Zbiór tablic stron każdego procesu
        Stack<int> wolne_zbiory = new Stack<int>();

        int ile_ramek_przydzielić(int ile_potrzebuje) //algorytm proporcjonalnego przydziału ramek
        {
            int ile_wolnych = wolne_ramki.Count;
            if (ile_potrzebuje <= ile_wolnych)
            {
                return ile_potrzebuje;
            }
            else
            {
                int ile_zajęte = konfiguracja.ile_ramek - ile_wolnych;
                int zwracanie = (int)Math.Ceiling(((double)ile_potrzebuje / ((double)ile_zajęte + (double)ile_potrzebuje) * (double)konfiguracja.ile_ramek));
                return zwracanie;
            }

        }

        int znajdź_i_zwolnij_najstarszą_ramkę() //wyszukuje najdawniej uzytą ramkę i przenosi ją do pamięci pomocniczej; zwraca numer zwolnionej juz ramki
        {
            int index1 = 0;
            int index2 = 0;
            int stempel = -1;

            for (int i = 0; i < TablicaZbiorów.Count; i++)
            {
                for (int j = 0; j < TablicaZbiorów[i].Count; j++)
                {
                    if (TablicaZbiorów[i][j].poprawność == true)
                    {
                        if (TablicaZbiorów[i][j].stempel_czasowy < stempel || stempel == -1)
                        {
                            index1 = i;
                            index2 = j;
                            stempel = TablicaZbiorów[i][j].stempel_czasowy;
                        }
                    }
                }
            }
            int wolna_stronica = znajdź_wolną_stronnicę();
            int zwolniona_ramka = TablicaZbiorów[index1][index2].numer;

            string zawartość_zwalnianej_ramki = odczytaj_ramkę(zwolniona_ramka);
            tablica_ramek[zwolniona_ramka].zajęta = false;

            zapisz_stronnicę(wolna_stronica, zawartość_zwalnianej_ramki);

            TablicaZbiorów[index1][index2].numer = wolna_stronica; //aktualizujemy odnośnik z numeru ramki na numer stronicy
            TablicaZbiorów[index1][index2].poprawność = false; //oznaczamy ramkę jako znajdującą się w pamięci pomocniczej

            return zwolniona_ramka;
        }

        void przydziel_pamięć_programowi(ref PCB pcb) //przydziela pamięć programowi, ramki/stronnice pozostają puste
        {
            int potrzeba_stron_na_kod = ile_potrzeba_ramek(pcb.Auto_storage_size);
            int potrzeba_stron_na_dane = ile_potrzeba_ramek(pcb.Auto_data_size);
            int potrzeba_stron_na_stos = ile_potrzeba_ramek(pcb.Auto_stack_size);
            int przyznano_ramek = ile_ramek_przydzielić(potrzeba_stron_na_kod + potrzeba_stron_na_dane + potrzeba_stron_na_stos);


            //przydzielam proporcjonalnie przydzielone dane kodowi, dane i stosowi

            int ramki_na_kod;
            int ramki_na_dane;
            int ramki_na_stos;

            if (przyznano_ramek == 0)
            {
                return;
            }

            if (przyznano_ramek > wolne_ramki.Count)
            {
                int ile_zwolnic = przyznano_ramek - wolne_ramki.Count;
                for (int i = 0; i < ile_zwolnic; i++)
                {
                    wolne_ramki.Push(znajdź_i_zwolnij_najstarszą_ramkę());
                }
            }

            int suma = potrzeba_stron_na_kod + potrzeba_stron_na_dane + potrzeba_stron_na_stos;
            ramki_na_kod = (int)Math.Ceiling((double)potrzeba_stron_na_kod / (double)suma * (double)przyznano_ramek);
            ramki_na_dane = (int)Math.Ceiling((double)potrzeba_stron_na_dane / (double)suma * (double)przyznano_ramek);
            ramki_na_stos = przyznano_ramek - ramki_na_kod;
            ramki_na_stos -= ramki_na_dane;
            


            int numerzbioru = wolne_zbiory.Pop();
            List<Strona> TablicaStron = new List<Strona>();

            if (potrzeba_stron_na_kod > 0)
            {
                pcb.Auto_storage_addres = numerzbioru * 1000;
            }
            for (int i = 0; i < ramki_na_kod; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_ramkę();
                strona.ochrona = true;
                strona.poprawność = true;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_ramkę(strona.numer, "KODKODKOD");
                TablicaStron.Add(strona);
            }
            for (int i = ramki_na_kod; i < potrzeba_stron_na_kod; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_stronnicę();
                strona.ochrona = true;
                strona.poprawność = false;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_stronnicę(strona.numer, "KODKODKOD");

                TablicaStron.Add(strona);
            }
            if (potrzeba_stron_na_dane > 0)
            {
                pcb.Auto_data_addres = numerzbioru * 1000 + potrzeba_stron_na_kod * konfiguracja.długość_ramki;
            }
            for (int i = 0; i < ramki_na_dane; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_ramkę();
                strona.ochrona = false;
                strona.poprawność = true;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_ramkę(strona.numer, "DANEDANE");
                TablicaStron.Add(strona);
            }
            for (int i = ramki_na_dane; i < potrzeba_stron_na_dane; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_stronnicę();
                strona.ochrona = false;
                strona.poprawność = false;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_stronnicę(strona.numer, "DANEDANE");

                TablicaStron.Add(strona);
            }
            if (potrzeba_stron_na_dane > 0)
            {
                pcb.Auto_stack_addres = numerzbioru * 1000 + potrzeba_stron_na_kod * konfiguracja.długość_ramki + potrzeba_stron_na_dane * konfiguracja.długość_ramki;
            }
            for (int i = 0; i < ramki_na_stos; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_ramkę();
                strona.ochrona = false;
                strona.poprawność = true;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_ramkę(strona.numer, "STOSSTOS");
                TablicaStron.Add(strona);
            }
            for (int i = ramki_na_stos; i < potrzeba_stron_na_stos; i++)
            {
                Strona strona = new Strona();
                strona.numer = znajdź_wolną_stronnicę();
                strona.ochrona = false;
                strona.poprawność = false;
                strona.stempel_czasowy = getStempelCzasowy();
                zapisz_stronnicę(strona.numer, "STOSSTOS");

                TablicaStron.Add(strona);
            }

            TablicaZbiorów[numerzbioru] = TablicaStron;
            
        }

        void zwolnij_przydzieloną_pamięć_programowi(int adres_początkowy, int ile)
        {
            int numerzbioru = adres_początkowy / 1000; //sprawdza w którym elemencie listy zbiorów )listy zawierającej tablice stronic każdego procesu) znajduje się żądany element
            if (adres_początkowy == -1) //gdyby nastąpiła próba zwolnienia pamięci procesu który nie został zaalokowany
            {
                return;
            }
            

            for (int i = 0; i < TablicaZbiorów[numerzbioru].Count(); i++)
            {
                if (TablicaZbiorów[numerzbioru][i].poprawność == true)
                {
                    //zwalniamy ramki
                    int numer_ramki = TablicaZbiorów[numerzbioru][i].numer;
                    tablica_ramek[numer_ramki].zajęta = false;
                    wolne_ramki.Push(numer_ramki);
                }
                else
                {
                    //zwalniamy stronice
                    int numer_stronicy = TablicaZbiorów[numerzbioru][i].numer;
                    tablica_stronnic[numer_stronicy].zajęta = false;
                    wolne_stronice.Push(numer_stronicy);
                }
            }

            wolne_zbiory.Push(numerzbioru);
        }
        #endregion
    }
}