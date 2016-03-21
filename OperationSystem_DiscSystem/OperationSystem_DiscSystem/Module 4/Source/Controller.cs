using OperationSystem_DiscSystem.Module_4.ExceptionNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source
{
    static class Controller
    {
        /* Tworzenie poszczególnych systemów */
        private static FolderSystem.Folder folder = new FolderSystem.Folder();
        public static FileBlocksSystem.FileBlocks1 Disc = new FileBlocksSystem.FileBlocks1();
        private static FreeBlocksSystem.FreeBlocks freeBlocks = new FreeBlocksSystem.FreeBlocks();
        private static Reader reader = new Reader();
        private static Printer printer = new Printer();

        /* Zmienne dodatkowe*/
        private const int FileNameLength = 16;
        private const int blockAmount = 32;
        
        /* Gettery i Settery */
        public static int getBlockAmount()
        {
            return blockAmount;
        }

        /*======================Metody dostępowe======================*/
            
        /* Tworzenie pliku */
        public static void addNewFile(String fileName, String Text)
        {
            if (checkFileNameLength(fileName) && isEnoughFreeBlocks(Text) && checkFileNameExist(fileName))
            {
                byte nextFreeBlockID = Convert.ToByte(freeBlocks.getNextFreeBlock());
                folder.createNewFile(fileName, nextFreeBlockID);
                Disc.resetBlockData(nextFreeBlockID);
                Disc.addTextToExistingFile(fileName, Text, nextFreeBlockID);
            }
        }
        /* Metoda tworząca nowy plik bez tekstu*/
        public static void addNewFile(String fileName)
        {
            if (checkFileNameLength(fileName) && isEnoughFreeBlocks(" ") && checkFileNameExist(fileName))
            {
                byte nextFreeBlockID = Convert.ToByte(freeBlocks.getNextFreeBlock());
                Disc.resetBlockData(nextFreeBlockID);
                folder.createNewFile(fileName, nextFreeBlockID);
            }
        }
        /* Metoda dodająca tekst do nowego pliku */
        public static void addTextToNewFile(String fileName, String Text)
        {
            if (checkIfFileExist(fileName) && isEnoughFreeBlocks(Text))
            {
                FolderSystem.FileClass tempFile = returnAllFileAttributes(fileName);
                Disc.addTextToExistingFile(fileName, Text, tempFile.getLastFileBlock());
            }
        }
        /* Wyświetla tekst pliku */
        public static String printFile(String fileName)
        {
            if (checkIfFileExist(fileName))
            {
                FolderSystem.FileClass tempFile = returnAllFileAttributes(fileName);
                String Text = Disc.getFileText(tempFile.getFirstFileBlock());
                return Text;
            }
            return "";
        }
        /* Wyświetla listę plików */
        public static String printFileNameList()
        {
            var lista = folder.returnAllFileNames();
            String Text = "";

            int counter = 1;
            foreach(String iterator in lista)
            {
                Text += counter + ". " + iterator + "\n";
                counter++;
            }

            return Text;
        }
        /* Wyświetla atrybuty pliku */
        public static String printFileAttributes(String fileName)
        {
            if (checkIfFileExist(fileName))
            {
                String Text = folder.getAllFileParameters(fileName);
                return Text;
            }
            return "";
        }
        /* Wyświetla wszystkie dane pliku */
        public static String printAllFileData(String fileName)
        {
            if (checkIfFileExist(fileName))
            {
                FolderSystem.FileClass tempFile = returnAllFileAttributes(fileName);

                String Text = folder.getAllFileParameters(fileName);
                Text += "\nDane...\n" + Disc.printFileBlocks(tempFile.getFirstFileBlock());
                return Text;
            }
            return "";
        }
        /* Wyświetla pełną listę plików */
        public static String printAllFiles()
        {
            String Text = "";
            int counter = 1;

            foreach(String iterator in folder.returnAllFileNames())
            {
                var tempFile = returnAllFileAttributes(iterator);
                Text += "\n\n-----Plik nr " + counter + "\nNazwa pliku: " + tempFile.getFileName() + "\nNumer pierwszego bloku: " + tempFile.getFirstFileBlock()
                    + "\nNumer ostatniego bloku: " + tempFile.getLastFileBlock() + "\nWielkość pliku: " + tempFile.getFileSize() + "\n\nDane:\n";
                Text += Disc.printFileBlocks(tempFile.getFirstFileBlock());
                counter++;
            }

            return Text;
        }
        /* Wyświetla tablicę bloków */
        public static String printBlocksTable()
        {
            String Text = "";
            int counter = 1;

            foreach(byte iterator in freeBlocks.returnAllBlocks())
            {
                Text += counter + ". " + iterator + "\n";
                counter++;
            }

            return Text;
        }
        /* Wyświetla listę danych wszystkich bloków */
        public static String printAllBlocksData()
        {
            String Text = "";
            int counter = 1;
            char temp = ' ';
        
            foreach(byte[] iterator in Disc.returnAllBlocks())
            {
                Text += counter + ". ";
                for (int i = 0; i < 15; i++)
                {
                    temp = Convert.ToChar(iterator[i]);
                    if (temp == '\n' || temp == '\t' || temp == '\r') temp = ' ';
                    Text += temp;
                }
                Text += "\t| Następny blok: " + iterator[15] + "\n";
                counter++;
            }

            return Text;
        }
        /* Zwraca dany znak z pliku */
        public static String getCharFromFile(String fileName, int position)
        {
            if (checkIfFileExist(fileName))
            {
                String Text;
                var tempFile = returnAllFileAttributes(fileName);
                Text = Disc.getCharFromFile(position, tempFile.getFirstFileBlock());
                return Text;
            }
            return "";
        }
        /* Zapisuje znak do pliku na danej pozycji */
        public static void setCharToFile(String fileName, int position, char x)
        {
            if (checkIfFileExist(fileName))
            {
                var tempFile = returnAllFileAttributes(fileName);
                Disc.setCharToFile(position, tempFile.getFirstFileBlock(), x);
            }
        }
        /* Zwraca dany ciąg znaków z pliku */
        public static String getCharArrayFromFile(String fileName, int positionA, int howManyCharacters)
        {
            String Text = "";
            if (checkIfFileExist(fileName))
            {
                var tempFile = returnAllFileAttributes(fileName);
                Text = Disc.getCharArrayFromFile(tempFile.getFirstFileBlock(), positionA, howManyCharacters);
            }
            return Text;
        }
        /* Zamienia ciąg znaków w pliku na podany znak */
        public static void setCharArrayToFile(String fileName, int position, String Text)
        {
            if (checkIfFileExist(fileName))
            {
                var tempFile = returnAllFileAttributes(fileName);
                Disc.setCharArrayToFile(tempFile.getFirstFileBlock(), position, Text);
            }
        }
        /* Dodanie tekstu do istniejącego pliku */
        public static void addTextToExistingFile(String fileName, String Text)
        {
            if (checkIfFileExist(fileName))
            {
                FolderSystem.FileClass tempFile = returnAllFileAttributes(fileName);
                if (isEnoughSpace(Text, tempFile.getLastFileBlock()))
                {
                    Disc.addTextToExistingFile(fileName, Text, tempFile.getLastFileBlock());
                }
            }
        }
        /* Usuwanie pliku */
        public static void deleteFile(String fileName)
        {
            if (checkIfFileExist(fileName))
            {
                int[] index;

                FolderSystem.FileClass tempFile = new FolderSystem.FileClass();
                tempFile = returnAllFileAttributes(fileName);
                index = Disc.getAllFileBlocksNumbers(tempFile.getFirstFileBlock(), tempFile.getFileSize());
                freeBlocks.deleteBlocksPointers(index);
                folder.deleteFile(fileName);
            }
        } 
        /* Wczytanie na dysk pliku fizycznego o takiej samej nazwie */
        public static void readExternalFile(String fileName)
        {
            reader.readFromExternalFile(fileName);
        }
        /* Wczytanie na dysk pliku fizycznego o innej nazwie */
        public static void readExternalFile(String fileName, String newFileName)
        {
            reader.readFromExternalFile(fileName, newFileName);
        }
        /* Zapis na dysk fizyczny pliku o takiej samej nazwie */
        public static void writeToExternalFile(String fileName)
        {
            if (checkIfFileExist(fileName))
            {
                printer.printToExternalFile(fileName);
            }
        }
        /* Zapis na dysk fizyczny pliku o innej nazwie */
        public static void writeToExternalFile(String fileName, String newFileName)
        {
            if (checkIfFileExist(fileName))
            {
                printer.printToExternalFile(fileName, newFileName);
            }
        }

        /*======================Metody pomocnicze======================*/

        /* Sprawdzenie długości nazwy pliku */
        public static bool checkFileNameLength(String FileName)
        {
            if (FileName.Length > FileNameLength)
            {
                ExceptionClass.returnExceptionMessage("Podana nazwa jest za długa");
                return false;
            }
            else return true;
        }
        /* Sprawdzenie czy istnieje plik o tej samej nazwie */
        public static bool checkFileNameExist(String fileName)
        {
            bool wynik = folder.checkIfFileExist(fileName);
            if (wynik) ExceptionClass.returnExceptionMessage("Plik o podanej nazwie już istnieje.");
            return !wynik;
        }
        /* Sprawdza czy plik istnieje */
        public static bool checkIfFileExist(String fileName)
        {
            bool wynik = folder.checkIfFileExist(fileName);
            if (!wynik) ExceptionClass.returnExceptionMessage("Plik o podanej nazwie nie istnieje.");
            return wynik;
        }
        /* Sprawdza czy istnieje wystarczająca ilość wolnych bloków */
        public static bool isEnoughFreeBlocks(String Text)
        {
            bool wynik = freeBlocks.isEnoughFreeBlocks(Text);
            if (wynik) return true;
            else
            {
                ExceptionClass.returnExceptionMessage("Brak miejsca na dysku");
                return false;
            }
        }
        /* Sprawdza czy istnieje wystarczająca ilość miejsca do dodania tekstu do pliku */
        private static bool isEnoughSpace(String Text, int lastFileBlockID)
        {
            int freeCharactersInLastDisc = Disc.howManyFreeCharacters(lastFileBlockID);
            int length = Text.Length - freeCharactersInLastDisc;
            if (length < 0)
            {
                return true;
            }
            else if (freeCharactersInLastDisc == Text.Length) return true;

            bool wynik = freeBlocks.isEnoughFreeBlocks(length);
            if (wynik) return true;
            else
            {
                ExceptionClass.returnExceptionMessage("Brak miejsca na dysku");
                return false;
            }
        }
        /* Tworzy nowy blok w liście bloków */
        public static void addNewBlock()
        {
            Disc.addEmptyBlock();
        }
        /* Zwraca liczbę istniejących bloków na dysku */
        public static int getBlockCount()
        {
            return Disc.getBlocksCounter();
        }
        /* Zwracanie kolejnego wolnego bloku */
        public static int returnNextFreeBlockID()
        {
            return freeBlocks.getNextFreeBlock();
        }
        /* Ustawia ostatni blok w pliku */
        public static void setLastFileBlock(String fileName, int lastFileBlockID)
        {
            folder.setLastFileBlock(fileName, lastFileBlockID);
        }
        /* Ustawia wielkość pliku */
        public static void setFileSize(String fileName, int fileSize)
        {
            folder.setFileSize(fileName, fileSize);
        }
        /* Uaktualnia wielkość pliku */
        public static void updateFileSize(String fileName, int size)
        {
            folder.updateFileSize(fileName, size);
        }
        /* Zwraca atrybuty pliku */
        public static FolderSystem.FileClass returnAllFileAttributes(String fileName)
        {
            FolderSystem.FileClass tempFile = folder.getAllFileAttributes(fileName);
            return tempFile;
        }
    }
}
