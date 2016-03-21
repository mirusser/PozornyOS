using OperationSystem_DiscSystem.Module_4.ExceptionNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source
{
    class Reader
    {
        /* Kontruktor */

        public Reader() { }

        /*======================Metody======================*/

        /* Pobiera tekst z pliku fizycznego i tworzy nowy plik o takiej samej nazwie jak plik */
        public void readFromExternalFile(String fileName)
        {
            String Text = "";
            if (Controller.checkFileNameExist(fileName) && Controller.checkFileNameLength(fileName))
            {
                try
                {
                    using (var streamWriter = new StreamReader(fileName + ".txt"))
                    {
                        Text = streamWriter.ReadToEnd();

                        if (Controller.isEnoughFreeBlocks(Text))
                        {
                            Controller.addNewFile(fileName, Text);
                            ExceptionClass.returnExceptionMessage("Odczytano plik z dysku fizycznego. Utworzono plik o nazwie: " + fileName);
                        }
                    }
                }catch(Exception arg)
                {
                    ExceptionClass.returnExceptionMessage("Nie znaleziono pliku o podanej nazwie.");
                }
            }
        }
        /* Pobiera tekst z pliku fizycznego i tworzy nowy plik o nowej nazwie */
        public void readFromExternalFile(String fileName, String newFileName)
        {
            String Text = "";
            if (Controller.checkFileNameExist(newFileName) && Controller.checkFileNameLength(newFileName))
            {
                try
                {
                    using (var streamWriter = new StreamReader(fileName + ".txt"))
                    {
                        Text = streamWriter.ReadToEnd();

                        if (Controller.isEnoughFreeBlocks(Text))
                        {
                            Controller.addNewFile(newFileName, Text);
                            ExceptionClass.returnExceptionMessage("Odczytano plik z dysku fizycznego. Utworzono plik o nazwie: " + newFileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
