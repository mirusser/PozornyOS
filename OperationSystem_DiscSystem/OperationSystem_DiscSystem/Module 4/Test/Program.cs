using OperationSystem_DiscSystem.Module_4.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            String iterator, fileName, newFileName, Text, positionTemp, znak;
            int position, positionA, howManyCharacters;

            do
            {
                Console.WriteLine("\n\n-----MENU-----\n\n1. Utwórz plik. \n2. Utwórz plik z danymi. \n3. Dodaj dane do pliku. \n4. Wyświetl plik. \n5. Wyświetl listę plików. " 
                    + "\n6. Usuń plik. \n\n-----Opcje dodatkowe-----\n7. Pokaż szczegółowo plik. \n8. Pokaż szczegółowe dane pliku. \n9. Pokaż pełną listę plików. " 
                    + "\n10. Pokaż tablicę bloków. \n11. Pokaż wszystkie bloki. \n12. Pobierz znak z pliku. \n13. Zapisz znak do pliku. \n14. Pobierz ciąg znaków z pliku. \n15. Zapisanie ciągu znaków do pliku." 
                    + "\n16. Zapisz plik na dysk fizyczny. \n17. Wczytaj plik z dysku fizycznego. \n\n0. Wyjście");
                Console.Write("\n\nPodaj opcje: ");
                iterator = Console.ReadLine();

                switch (iterator)
                {
                    case "0":
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
                        Controller.addTextToExistingFile(fileName, Text);
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
                         BREAKPOINT:
                        Console.Write("\nPodaj pozycje znaku w pliku: ");
                        positionTemp = Console.ReadLine();
                        try
                        {
                            position = Int32.Parse(positionTemp);
                        }
                        catch(Exception arg)
                        {
                            Console.WriteLine("Podany zły znak. Należy podać liczbę.");
                            goto BREAKPOINT;
                        }
                        Console.WriteLine(Controller.getCharFromFile(fileName, position));
                        break;
                    case "13":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        BREAKPOINT1:
                        Console.Write("\nPodaj pozycje znaku w pliku: ");
                        positionTemp = Console.ReadLine();
                        try
                        {
                            position = Int32.Parse(positionTemp);
                        }catch(Exception arg)
                        {
                            Console.WriteLine("Podany zły znak. Należy podać liczbę.");
                            goto BREAKPOINT1;
                        }
                        Console.Write("\nPodaj znak do zapisania: ");
                        znak = Console.ReadLine();
                        Controller.setCharToFile(fileName, position, Convert.ToChar(znak));
                        break;
                    case "14":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        BREAKPOINT2:
                        Console.Write("\nPodaj początek ciągu: ");
                        positionTemp = Console.ReadLine();
                        try
                        {
                            positionA = Int32.Parse(positionTemp);
                        }catch(Exception arg)
                        {
                            Console.WriteLine("Podany zły znak. Należy podać liczbę.");
                            goto BREAKPOINT2;
                        }                      
                        BREAKPOINT3:
                        Console.Write("\nPodaj ile znaków: ");
                        positionTemp = Console.ReadLine();
                        try
                        {
                            Int32.TryParse(positionTemp, out howManyCharacters);
                        }catch(Exception arg)
                        {
                            Console.WriteLine("Podany zły znak. Należy podać liczbę.");
                            goto BREAKPOINT3;
                        }
                        Console.WriteLine(Controller.getCharArrayFromFile(fileName, positionA, howManyCharacters));
                        break;
                    case "15":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        BREAKPOINT4:
                        Console.Write("\nPodaj początek ciągu: ");
                        positionTemp = Console.ReadLine();
                        try
                        {
                            positionA = Int32.Parse(positionTemp);
                        }
                        catch(Exception arg)
                        {
                            Console.WriteLine("Podany zły znak. Należy podać liczbę.");
                            goto BREAKPOINT4;
                        }
                        Console.Write("\nPodaj znaki: ");
                        Text = Console.ReadLine();
                        Controller.setCharArrayToFile(fileName, positionA, Text); 
                        break;
                    case "16":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        BREAKPOINT5:
                        Console.Write("\nCzy plik ma posiadać taką samą nazwę? (Y/N): ");
                        positionTemp = Console.ReadLine();
                        if (positionTemp == "Y" || positionTemp == "y") Controller.writeToExternalFile(fileName);
                        else if(positionTemp == "N" || positionTemp == "n")
                        {
                            Console.Write("\nPodaj nową nazwę pliku: ");
                            newFileName = Console.ReadLine();
                            Controller.writeToExternalFile(fileName, newFileName);
                        }
                        else
                        {
                            goto BREAKPOINT5;
                        }
                        break;
                    case "17":
                        Console.Write("\nPodaj nazwe pliku: ");
                        fileName = Console.ReadLine();
                        BREAKPOINT6:
                        Console.Write("\nCzy plik ma posiadać taką samą nazwę? (Y/N): ");
                        positionTemp = Console.ReadLine();
                        if (positionTemp == "Y" || positionTemp == "y") Controller.readExternalFile(fileName);
                        else if (positionTemp == "N" || positionTemp == "n")
                        {
                            Console.Write("\nPodaj nową nazwę pliku: ");
                            newFileName = Console.ReadLine();
                            Controller.readExternalFile(fileName, newFileName);
                        }
                        else
                        {
                            Console.WriteLine("Błędna opcja. Proszę wybrać ponownie.");
                            goto BREAKPOINT6;
                        }
                        break;
                    default:
                        Console.WriteLine("Błąd");
                        break;
                }
            } while (iterator != "0");
        }
    }
}
