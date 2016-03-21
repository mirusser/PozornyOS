using OperationSystem_DiscSystem.Module_4.ExceptionNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source
{
    class Printer
    {

        /* Konstruktor */
        public Printer() { }

        /*======================Metody======================*/

        /* Zapisuje plik na dysku fizycznym o takiej samej nazwie */
        public void printToExternalFile(String fileName)
        {
            String TextToWrite = Controller.printFile(fileName);

            using (var streamWriter = new StreamWriter(fileName + ".txt"))
            {
                streamWriter.WriteLine(TextToWrite);
                ExceptionClass.returnExceptionMessage("Zapisano plik na dysk fizyczny. Nazwa pliku: " + fileName);
            }
        }
        /* Zapisuje plik na dysku fizycznym o nowej nazwie */
        public void printToExternalFile(String fileName, String newFileName)
        {
            String TextToWrite = Controller.printFile(fileName);

            using (var streamWriter = new StreamWriter(newFileName + ".txt"))
            {
                streamWriter.WriteLine(TextToWrite);
                ExceptionClass.returnExceptionMessage("Zapisano plik na dysk fizyczny. Nazwa pliku: " + newFileName);
            }
        }
    }
}
