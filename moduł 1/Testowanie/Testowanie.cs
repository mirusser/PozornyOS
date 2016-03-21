using System;
using Moduł_3_Zarządzanie_procesami_wyższy;
using OperationSystem_DiscSystem.Module_4.Source;

namespace moduł_1.Testowanie
{

    class Program
    {
        static void Main(string[] args)
        {
            
            Proces_Managment proces = new Proces_Managment();
            proces.XC("root");

            string name, name1, text;
            string co_wybieram;
            String fileName, newFileName, Text, positionTemp, znak;
            int position, positionA, howManyCharacters;
            PCB pcb;              
            do
            {
                Console.Write("Polecenie>");
                co_wybieram = Console.ReadLine();
                switch (co_wybieram)
                {
                    case ("?"):
                        Console.WriteLine("Lista poleceń:");
                        Console.WriteLine("\"end\" - Wyjście z programu");
                        Console.WriteLine("\"clear\" - Czyszczenie ekranu konsoli");
                        Console.WriteLine("\nZarządzanie procesami, moduł wyższy, struktura hierarchiczna\n");
                        Console.WriteLine("\"xc\" - Utworzenie procesu");
                        Console.WriteLine("\"xd\" - Usunięcie procesu");
                        Console.WriteLine("\"xh\" - Zatrzymanie procesu i powiadomienie programu nadzorczego");
                        Console.WriteLine("\"xn\" - Wyszukanie procesu");
                        Console.WriteLine("\"xr\" - Przeczytanie komunikatu wysłanego do procesu");
                        Console.WriteLine("\"xs\" - Wysłanie komunikatu");
                        Console.WriteLine("\"xy\" - Uruchomienie procesu");
                        Console.WriteLine("\"xz\" - Zatrzymanie procesu");
                        Console.WriteLine("\"xque\" - Obsługa błędów w procesach");
                        Console.WriteLine("\"display\" - Wyświetlenie wartości pól PCB procesu - wyświetla tylko niektóre");
                        Console.WriteLine("\"displayall\" - Wyświetlenie listy PCB");
                        Console.WriteLine("\nZarządzanie pamięcią fizyczną i pomocniczą z użyciem LRU ze stemplami czasowymi\n");
                        Console.WriteLine("\"pwtr\" - Wyświetl tablicę ramek");
                        Console.WriteLine("\"pwtsc\" - Wyswietl tablicę stronnic");
                        Console.WriteLine("\"pwts\" - Wyświetl tablicę stron wybranego procesu");
                        Console.WriteLine("\"pws\" - Wyświetl element stosu spod danego adresu");
                        Console.WriteLine("\"pzs\" - Zaisz element stosu pod adresem");
                        Console.WriteLine("\"pwd\" - Wyświetl element danych spod adresu");
                        Console.WriteLine("\"pzd\" - Zapisz element danych pod adresem");
                        Console.WriteLine("\"pwk\" - Wyświetl element kodu programu spod adresu");
                        Console.WriteLine("\"pzk\" - Zapisz kod programu od adresu");
                        Console.WriteLine("\"pwzm\" - Sprawdź zawartość semafora MEMORY");
                        Console.WriteLine("\nZarządzanie plikami i katalogami z użyciem techniki listowej\n");
                        Console.WriteLine("\"1\" - Utwórz plik");
                        Console.WriteLine("\"2\" - Utwórz plik z danymi");
                        Console.WriteLine("\"3\" - Dodaj dane do pliku");
                        Console.WriteLine("\"4\" - Wyświetl plik");
                        Console.WriteLine("\"5\" - Wyświetl listę plików");
                        Console.WriteLine("\"6\" - Usuń plik");
                        Console.WriteLine("\"7\" - Pokaż szczegółowo plik");
                        Console.WriteLine("\"8\" - Pokaż szczegółowe dane pliku");
                        Console.WriteLine("\"9\" - Pokaż pełną listę plików");
                        Console.WriteLine("\"10\" - Pokaż tablicę bloków");
                        Console.WriteLine("\"11\" - Pokaż wszystkie bloki");
                        Console.WriteLine("\"12\" - Pobierz znak z pliku");
                        Console.WriteLine("\"13\" - Zapisz znak do pliku");
                        Console.WriteLine("\"14\" - Pobierz ciąg znaków z pliku");
                        Console.WriteLine("\"15\" - Zapisanie ciągu znaków do pliku");
                        Console.WriteLine("\"16\" - Zapisz plik na dysk fizyczny");
                        Console.WriteLine("\"17\" - Wczytaj plik z dysku fizycznego");
                        break;
                    case ("xc"):
                        Console.Write("Czy proces ma mieć rodzica innego niż \"root\"? [T/N]: ");
                        name1 = Console.ReadLine();
                        if (name1 == "t" || name1 == "T" || name1 == "tak" || name1 == "Tak" || name1 == "TAK")
                        {
                            Console.Write("Podaj nazwę rodzica: ");
                            name1 = Console.ReadLine();
                            Console.Write("Podaj nazwę procesu: ");
                            name = Console.ReadLine();
                            proces.XC(name, name1);
                        }
                        else if (name1 == "n" || name1 == "N" || name1 == "nie" || name1 == "Nie" || name1 == "NIE")
                        {
                            Console.Write("Podaj nazwę procesu: ");
                            name = Console.ReadLine();
                            proces.XC(name, "root");
                        }
                        else
                        {
                            Console.WriteLine("Podano nieprawidłową odpowiedź");
                        }
                        break;
                    case ("xd"):
                        Console.Write("Podaj nazwę procesu: ");
                        name = Console.ReadLine();
                        proces.XD(name);
                        break;
                    case ("xh"):
                        Console.Write("Podaj nazwę procesu: ");
                        name = Console.ReadLine();
                        proces.XH(name);
                        break;
                    case ("xn"):
                        Console.Write("Podaj nazwę procesu: ");
                        name = Console.ReadLine();
                        pcb = proces.XN(name);
                        if (pcb != null)
                        {
                            Console.WriteLine("Znaleziono blok PCB o podanej nazwie: {0}", pcb.Name);
                        }
                        else
                        {
                            Console.WriteLine("Nie znaleziono bloku PCB o podanej nazwie: {0}", name);
                        }
                        break;
                    case ("xr"):
                        Console.Write("Podaj nazwę procesu, który ma odebrać komunikat: ");
                        name = Console.ReadLine();
                        proces.XR(name);
                        break;
                    case ("xs"):
                        Console.Write("Podaj nazwę procesu, który wysyła komunikat: ");
                        name = Console.ReadLine();
                        Console.Write("Podaj nazwę procesu, do którego ma pójść komunikat: ");
                        name1 = Console.ReadLine();
                        Console.Write("Podaj treść komunikatu: ");
                        text = Console.ReadLine();
                        proces.XS(name1, name, text);
                        break;
                    case ("xy"):
                        Console.Write("Podaj nazwę procesu: ");
                        name = Console.ReadLine();
                        proces.XY(name);
                        break;
                    case ("xz"):
                        Console.Write("Podaj nazwę procesu, który ma zostać zatrzymany:");
                        name = Console.ReadLine();
                        proces.XZ(name);
                        break;
                    case ("xque"):
                        proces.XQUE();
                        break;
                    case ("display"):
                        Console.Write("Podaj nazwę procesu, którego pola PCB mają być wyświetlona:");
                        name = Console.ReadLine();
                        proces.Display_PCB(name);
                        break;
                    case ("displayall"):
                        proces.Display_all();
                        break;
                    case ("clear"):
                        Console.Clear();
                        break;
                    case ("end"):
                        Console.WriteLine("Zamykanie systemu...");
                        break;
                    case ("pwtr"): //działa
                        proces.pamięć.wyświetl_zawartość_ramek();
                        break;
                    case ("pwtsc"): //działa
                        proces.pamięć.wyświetl_zawartość_stronnic();
                        break;
                    case ("pwts"): //działa
                        Console.WriteLine("Podaj nazwę procesu: ");
                        name = Console.ReadLine();
                        pcb = proces.XN(name);
                        if (pcb == null)
                        {
                            Console.WriteLine("Nie znaleziono takiego procesu.");
                        }
                        else
                        {
                            proces.pamięć.wyświetl_tablicę_stron(ref pcb);
                        }
                        break;
                    case ("pws"): //działa
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        try
                        {
                            Console.WriteLine("Element stosu: {0}",proces.pamięć.odczytaj_element_stosu(Convert.ToInt32(name)));
                        }
                        catch
                        {
                            Console.WriteLine("Nie wpisano wartości");
                        }
                        break;
                   case ("pzs"): //działa
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        Console.Write("Podaj dwuznakowy element: ");
                        name1 = Console.ReadLine();
                        try
                        {
                            if (proces.pamięć.zapisz_element_stosu(Convert.ToInt32(name), name1) == "0")
                            {
                                Console.WriteLine("Zapisano!");
                            }
                            else
                            {
                                Console.WriteLine("Wystąpił błąd przy zapisywaniu.");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Nie wpisano wartości");
                        }
                        break;
                    case ("pwd"): //działa
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        try
                        {
                            Console.WriteLine(proces.pamięć.odczytaj_z_pamięci_operacyjnej(Convert.ToInt32(name)));
                        }
                        catch
                        {
                            Console.WriteLine("Nie wpisano wartości");
                        }
                        break;
                    case ("pzd")://działa
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        Console.Write("Kod: ");
                        name1 = Console.ReadLine();
                        try
                        {
                            if (proces.pamięć.zapisz_do_pamięci_operacyjnej(Convert.ToInt32(name), name1) == "0")
                            {
                                Console.WriteLine("Zapisano!");
                            }
                            else
                            {
                                Console.WriteLine("Wystąpił błąd przy zapisywaniu.");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Nie wpisano wartości");
                        }
                        break;
                    case ("pwk")://działa
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        try
                        {
                            Console.WriteLine(proces.pamięć.odczytaj_komórkę_programu(Convert.ToInt32(name)));
                        }
                        catch
                        {
                            Console.WriteLine("Nie wpisano wartości");
                        }
                        break;
                    case ("pzk")://poprawić
                        Console.Write("Podaj adres: ");
                        name = Console.ReadLine();
                        Console.Write("Kod: ");
                        name1 = Console.ReadLine();
                        if (proces.pamięć.zapisz_program_do_pamięci(Convert.ToInt32(name), name1) == "0")
                        {
                            Console.WriteLine("Zapisano!");
                        }
                        else
                        {
                            Console.WriteLine("Wystąpił błąd przy zapisywaniu.");
                        }
                        ;
                        break;

                    case ("pwzm"): 
                        proces.pamięć.wyświetl_stan_semafora_memory();
                        break;
                    case "1":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Controller.addNewFile(fileName);
                        break;
                    case "2":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.WriteLine("Podaj tekst: ");
                        Text = Console.ReadLine();
                        Controller.addNewFile(fileName, Text);
                        break;
                    case "3":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.WriteLine("Podaj tekst: ");
                        Text = Console.ReadLine();
                        Controller.addTextToNewFile(fileName, Text);
                        break;
                    case "4":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.WriteLine(Controller.printFile(fileName));
                        break;
                    case "5":
                        Console.WriteLine(Controller.printFileNameList());
                        break;
                    case "6":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Controller.deleteFile(fileName);
                        break;
                    case "7":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.WriteLine(Controller.printAllFileData(fileName));
                        break;
                    case "8":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.WriteLine(Controller.printFileAttributes(fileName));
                        break;
                    case "9":
                        Console.WriteLine(Controller.printAllFiles());
                        break;
                    case "10":
                        Console.WriteLine(Controller.printBlocksTable());
                        break;
                    case "11":
                        Console.WriteLine(Controller.printAllBlocksData());
                        break;
                    case "12":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nPodaj pozycje znaku w pliku: ");
                        positionTemp = Console.ReadLine();
                        position = Int32.Parse(positionTemp);
                        Console.WriteLine(Controller.getCharFromFile(fileName, position));
                        break;
                    case "13":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nPodaj pozycje znaku w pliku: ");
                        positionTemp = Console.ReadLine();
                        position = Int32.Parse(positionTemp);
                        Console.Write("\nPodaj znak do zapisania: ");
                        znak = Console.ReadLine();
                        Controller.setCharToFile(fileName, position, Convert.ToChar(znak));
                        break;
                    case "14":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nPodaj początek ciągu: ");
                        positionTemp = Console.ReadLine();
                        Int32.TryParse(positionTemp, out positionA);
                        positionA = Int32.Parse(positionTemp);
                        Console.Write("\nPodaj ile znaków: ");
                        positionTemp = Console.ReadLine();
                        Int32.TryParse(positionTemp, out howManyCharacters);
                        Console.WriteLine(Controller.getCharArrayFromFile(fileName, positionA, howManyCharacters));
                        break;
                    case "15":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nPodaj początek ciągu: ");
                        positionTemp = Console.ReadLine();
                        Int32.TryParse(positionTemp, out positionA);
                        positionA = Int32.Parse(positionTemp);
                        Console.Write("\nPodaj znaki: ");
                        Text = Console.ReadLine();
                        Controller.setCharArrayToFile(fileName, positionA, Text);
                        break;
                    case "16":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nCzy plik ma posiadać taką samą nazwę? (Y/N): ");
                        positionTemp = Console.ReadLine();
                        if (positionTemp == "Y" || positionTemp == "y") Controller.writeToExternalFile(fileName);
                        else if (positionTemp == "N" || positionTemp == "n")
                        {
                            Console.Write("\nPodaj nową nazwę pliku: ");
                            newFileName = Console.ReadLine();
                            Controller.writeToExternalFile(fileName, newFileName);
                        }
                        break;
                    case "17":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        Console.Write("\nCzy plik ma posiadać taką samą nazwę? (Y/N): ");
                        positionTemp = Console.ReadLine();
                        if (positionTemp == "Y" || positionTemp == "y") Controller.readExternalFile(fileName);
                        else if (positionTemp == "N" || positionTemp == "n")
                        {
                            Console.Write("\nPodaj nową nazwę pliku: ");
                            newFileName = Console.ReadLine();
                            Controller.readExternalFile(fileName, newFileName);
                        }
                        break;
                    default:
                        Console.WriteLine("System nie odnalazł polecenia: \"{0}\"", co_wybieram);
                        Console.WriteLine("Wpisz \"?\" by uzyskać pomoc");
                        break;
                }
            }
            while (co_wybieram != "end");
            Console.WriteLine("Naciśnij dowolny klawisz by zamknąć...");
            Console.ReadKey();
    }     
    }
}
