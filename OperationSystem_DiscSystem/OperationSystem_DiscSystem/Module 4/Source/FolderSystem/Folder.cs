using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source.FolderSystem
{
    class Folder
    {
        /* Lista plików */
        private List<FileClass> FolderList;

        /* Konstruktor */
        public Folder()
        {
            FolderList = new List<FileClass>();
        }

        /*======================Metody dostępowe======================*/

        /* Utworzenie nowego pliku */
        public void createNewFile(String fileName, byte nextFreeBlock)
        {
            FileClass newFile = new FileClass();

            newFile.setFileName(fileName);
            newFile.setFirstFileBlock(nextFreeBlock);
            newFile.setLastFileBlock(newFile.getFirstFileBlock());
            FolderList.Add(newFile);
        }
        /* Zwraca atrybuty pliku */
        public FileClass getAllFileAttributes(String fileName)
        {
            var tempFile = new FileClass();
            foreach (FileClass iterator in FolderList)
            {
                if (iterator.getFileName() == fileName)
                {
                    tempFile = iterator;
                }
            }
            return tempFile;
        }
        /* Zwraca dane wszystkich plików w postaci listy */
        public List<FileClass> getAllFilesList()
        {
            var tempFile = new List<FileClass>();
            tempFile = FolderList;
            return  tempFile;
        }
        /* Zwraca liste nazw wszystkich plików */
        public List<String> returnAllFileNames()
        {
            var lista = new List<String>();
            foreach (FileClass iterator in FolderList)
            {
                lista.Add(iterator.getFileName());
            }
            return lista;
        }
        /* Zwraca tekst z danymi pliku */
        public String getAllFileParameters(String fileName)
        {
            FileClass tempFile = getAllFileAttributes(fileName);

            String Text = "";
            Text += "\nNazwa pliku: " + tempFile.getFileName() + "\nNumer pierwszego bloku: " + tempFile.getFirstFileBlock() + "\nNumer ostatniego bloku: " + tempFile.getLastFileBlock()
                + "\nWielkosc pliku: " + tempFile.getFileSize();

            return Text;
        }
        /* Usuwa dany plik */
        public void deleteFile(String fileName)
        {
            foreach(FileClass iterator in FolderList)
            {
                if (iterator.getFileName().Equals(fileName))
                {
                    FolderList.Remove(iterator);
                    break;
                }
            }
        }
        /* Sprawdza czy podany plik istnieje */
        public bool checkIfFileExist(String fileName)
        {
            foreach (FileClass iterator in FolderList)
            {
                if (iterator.getFileName().Equals(fileName))
                {
                    return true;
                }
            }
            return false;
        }
        /* Ustawia ostatni blok pliku */
        public void setLastFileBlock(String fileName, int lastFileBlockID)
        {
            foreach (FileClass iterator in FolderList)
            {
                if (iterator.getFileName() == fileName)
                {
                    iterator.setLastFileBlock(Convert.ToByte(lastFileBlockID));
                }
            }
        }
        /* Ustawia wielkość pliku */
        public void setFileSize(String fileName, int fileSize)
        {
            foreach(FileClass iterator in FolderList)
            {
                if (iterator.getFileName().Equals(fileName))
                {
                    iterator.setFileSize(fileSize);
                    break;
                }
            }
        }
        /* Zwiększa wielkość pliku o 1 */
        public void updateFileSize(String fileName)
        {
            foreach (FileClass iterator in FolderList)
            {
                if (iterator.getFileName().Equals(fileName))
                {
                    iterator.updateFileSize();
                    break;
                }
            }
        }
        /* Zwiększa wielkość pliku o podaną wartość */
        public void updateFileSize(String fileName, int size)
        {
            foreach (FileClass iterator in FolderList)
            {
                if (iterator.getFileName().Equals(fileName))
                {
                    iterator.updateFileSize(size);
                    break;
                }
            }
        }
    }
}
