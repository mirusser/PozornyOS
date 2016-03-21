using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source.FileBlocksSystem
{
    class FileBlocks1
    {
        private List<byte[]> blocksList;

        /* Konstuktor */
        public FileBlocks1()
        {
            blocksList = new List<byte[]>();
        }

        /*======================Metody dostępowe======================*/

        /* Zwraca liczbę bloków */
        public int getBlocksCounter()
        {
            return blocksList.Count;
        }
        /* Metoda zwracająca dane pliku */
        public String getFileText(byte firstFileBlockID)
        {
            String text = "";

            byte nextBlockID = firstFileBlockID;
            byte[] Data;

            do
            {
                Data = blocksList[nextBlockID-1];

                for (int i = 0; i < 15; i++)
                {
                    if (Data[i] != 0)
                    {
                        text += Convert.ToChar(Data[i]);
                    }
                }

                nextBlockID = Data[15];
            } while (nextBlockID != 0);

            return text;
        }
        /* Metoda zwracająca numery wszystkich bloków danego pliku */
        public int[] getAllFileBlocksNumbers(byte firstFileBlockID, int fileLength)
        {
            int[] tab = new int[fileLength];
            byte nextFileBlockID = firstFileBlockID;
            int tabIndex = 0;

            do
            {
                tab[tabIndex] = nextFileBlockID;
                nextFileBlockID = blocksList[nextFileBlockID-1][15];
                tabIndex++;
            } while (nextFileBlockID != 0);

            return tab;
        }
        /* Metoda zwracająca wszystkie bloki */
        public List<byte[]> returnAllBlocks()
        {
            List<byte[]> lista = new List<byte[]>();

            lista = blocksList;
            return lista;
        }
        /* Metoda tworząca pusty plik */
        public void addEmptyBlock()
        {
            if (blocksList.Count < Controller.getBlockAmount())
            {
                byte[] Data = new byte[16];
                Data[15] = 0;

                blocksList.Add(Data);
            }
        }
        /* Zwraca tekst wszystkich bloków danego pliku*/
        public String printFileBlocks(byte firstFileBlockID)
        {
            byte nextBlockID = firstFileBlockID;
            byte[] Data;
            String Text = "";

            do
            {
                Data = blocksList[nextBlockID-1];
                Text += "\nBlok nr " + (nextBlockID) + " | Dane: ";

                for (int i = 0; i < 15; i++)
                {
                    if (Data[i] != 0)
                    {
                        char temp = Convert.ToChar(Data[i]);
                        if (temp == '\n' || temp == '\t' || temp == '\r') temp = ' ';
                        Text += Convert.ToChar(Data[i]);
                    }
                }
                Text += " | Numer następnego bloku: " + Data[15];

                nextBlockID = Data[15];
            } while (nextBlockID != 0);

            return Text;
        }
        /* Metoda dodająca tekst do istniejące, zapisanego pliku */
        public void addTextToExistingFile(String fileName, String Text, byte tempFileBlockID)
        {
            byte lastFileBlockID = tempFileBlockID;
            String tempText = "";
            int nextBlockID = 0;

            if (!checkIfBlockIsFull(blocksList[lastFileBlockID-1]))
            {
                int textCounter = 0;
                for (int i = 0; i < 15; i++)
                {
                    if (blocksList[lastFileBlockID-1][i] == 0 && textCounter < Text.Length)
                    {
                        blocksList[lastFileBlockID-1][i] = Convert.ToByte(Text[textCounter]);
                        textCounter++;
                    }
                }
                tempText = Text.Substring(textCounter);

                if (tempText.Length > 0)
                {
                    nextBlockID = Controller.returnNextFreeBlockID();
                    blocksList[lastFileBlockID-1][15] = Convert.ToByte(nextBlockID);
                    addNewBlocksToExistingFile(tempText, fileName, nextBlockID);
                }
            }
            else
            {
                nextBlockID = Controller.returnNextFreeBlockID();
                blocksList[lastFileBlockID-1][15] = Convert.ToByte(nextBlockID);
                addNewBlocksToExistingFile(Text, fileName, nextBlockID);
            }
        }
        /* Metoda pobierająca określony znak z pliku */
        public String getCharFromFile(int position, int firstFileBlockID)
        {
            String wynik = "";
            int tempBlockID = firstFileBlockID;

            int counter = 0;
            int blockCounter = 1;
            int blockPosition = 0;
            if (position % 15 == 0)
            {
                blockPosition = position / 15;
            }
            else
            {
                blockPosition = position / 15 + 1;
            }
            int positionInBlock = 0;
            if (position <= 15)
            {
                positionInBlock = position;
            }
            else if (blockPosition - 1 == 0)
            {
                positionInBlock = position - blockPosition * 15;
            }
            else
            {
                positionInBlock = position - (blockPosition - 1) * 15;
            }

        Label:
            counter = 0;
            foreach (byte[] iterator in blocksList)
            {
                counter++;
                if (counter == tempBlockID && blockCounter == blockPosition)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (i == positionInBlock - 1)
                        {
                            char temp = Convert.ToChar(iterator[i]);
                            wynik += temp;
                            break;
                        }
                    }
                }
                else if (counter == tempBlockID && blockCounter != blockPosition)
                {
                    tempBlockID = iterator[15];
                    blockCounter++;
                }
                if (wynik != "") break;
                if (tempBlockID <= counter) goto Label;
            }
            return wynik;
        }
        /* Metoda zapisuje określony znak do pliku na danej pozycji */
        public void setCharToFile(int position, int firstFileBlockID, char x)
        {
            int tempBlockID = firstFileBlockID;

            bool wynik = false;
            int counter = 0;
            int blockCounter = 1;
            int blockPosition = 0;
            if (position % 15 == 0)
            {
                blockPosition = position / 15;
            }
            else
            {
                blockPosition = position / 15 + 1;
            }
            int positionInBlock = 0;
            if (position <= 15)
            {
                positionInBlock = position;
            }
            else if (blockPosition - 1 == 0)
            {
                positionInBlock = position - blockPosition * 15;
            }
            else
            {
                positionInBlock = position - (blockPosition - 1) * 15;
            }

        Label:
            counter = 0;
            foreach (byte[] iterator in blocksList)
            {
                counter++;
                if (counter == tempBlockID && blockCounter == blockPosition)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (i == positionInBlock - 1)
                        {
                            iterator[i] = Convert.ToByte(x);
                            wynik = true;
                            break;
                        }
                    }
                }
                else if (counter == tempBlockID && blockCounter != blockPosition)
                {
                    tempBlockID = iterator[15];
                    blockCounter++;
                }

                if (wynik) break;
                if (tempBlockID <= counter) goto Label;
            }
        }
        /* Metoda pobiera określony ciąg znaków z pliku */
        public String getCharArrayFromFile(int firstFileBlockID, int positionA, int howManyCharacters)
        {
            String charsArray = "";

            int tempBlockID = firstFileBlockID;
            int counter = 0;
            int blockCounter = 1;
            int blockPosition = 0;
            int readChars = 0;

            if (positionA % 15 == 0)
            {
                blockPosition = positionA / 15;
            }
            else
            {
                blockPosition = positionA / 15 + 1;
            }
            int positionInBlock = 0;
            if (positionA <= 15)
            {
                positionInBlock = positionA;
            }
            else if (blockPosition - 1 == 0)
            {
                positionInBlock = positionA - blockPosition * 15;
            }
            else
            {
                positionInBlock = positionA - (blockPosition - 1) * 15;
            }

        Label:
            counter = 1;
            foreach (byte[] iterator in blocksList)
            {
                if (counter == tempBlockID)
                {
                    if (blockCounter >= blockPosition && readChars == 0)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            if (i >= positionInBlock - 1 && readChars < howManyCharacters)
                            {
                                charsArray += Convert.ToChar(iterator[i]);
                                readChars++;
                            }
                        }
                    }
                    else if (blockCounter >= blockPosition && readChars < howManyCharacters)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            if (readChars < howManyCharacters)
                            {
                                charsArray += Convert.ToChar(iterator[i]);
                                readChars++;
                            }
                        }
                    }

                    blockCounter++;
                    tempBlockID = iterator[15];
                    if (tempBlockID < counter) goto Label;
                    if (tempBlockID == 0) break;
                }
                counter++;
            }

            return charsArray;
        }
        /* Metoda zamienia ciąg znaków w pliku na podany */
        public void setCharArrayToFile(int firstFileBlockID, int position, String Text)
        {
            int howManyCharacters = Text.Length;

            int tempBlockID = firstFileBlockID;
            int counter = 0;
            int blockCounter = 1;
            int charCounter = 0;
            int writeChars = 0;

            if (position % 15 == 0)
            {
                charCounter = position / 15;
            }
            else
            {
                charCounter = position / 15 + 1;
            }
            int positionInBlock = 0;
            if (position <= 15)
            {
                positionInBlock = position;
            }
            else if (charCounter - 1 == 0)
            {
                positionInBlock = position - charCounter * 15;
            }
            else
            {
                positionInBlock = position - (charCounter - 1) * 15;
            }

        Label:
            counter = 1;
            foreach (byte[] iterator in blocksList)
            {
                if (counter == tempBlockID)
                {
                    if (blockCounter >= charCounter && writeChars == 0)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            if (i >= positionInBlock - 1 && writeChars < howManyCharacters)
                            {
                                iterator[i] = Convert.ToByte(Text[writeChars]);
                                writeChars++;
                            }
                        }
                    }
                    else if (blockCounter >= charCounter && writeChars < howManyCharacters)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            if (writeChars < howManyCharacters)
                            {
                                iterator[i] = Convert.ToByte(Text[writeChars]);
                                writeChars++;
                            }
                        }
                    }

                    blockCounter++;
                    tempBlockID = iterator[15];
                    if (tempBlockID < counter) goto Label;
                    if (tempBlockID == 0) break;
                }
                counter++;
            }
        }
        /* Usuwa dane z pustego bloku */
        public void resetBlockData(int blockID)
        {
            for(int i = 0; i < 16; i++)
            {
                blocksList[blockID - 1][i] = 0;
            }
        }
        /* Sprawdza ilość miejsca w danym bloku */
        public int howManyFreeCharacters(int tempFileBlockID)
        {
            int Counter = 0;

            for (int i = 0; i < 15; i++)
            {
                if(blocksList[tempFileBlockID-1][i] == 0)
                {
                    Counter++;
                }
            }

            return Counter;
        }

        /*======================Metody pomocnicze======================*/

        /* Sprawdza czy blok jest zapełniony */
        private bool checkIfBlockIsFull(byte[] block)
        {
            for (int i = 0; i < 15; i++)
            {
                if (block[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }
        /* Metoda dodająca bloki do istniejącego pliku */
        private void addNewBlocksToExistingFile(String Text, String fileName, int nextBlockID)
        {
            int tempBlockID = nextBlockID;
            int currentBlockID = tempBlockID;
            String tempText = "";
            int counter = 0;
            int lastBlockID = 0;
            int fileSize = 0;
            byte[] Data;

            foreach (char a in Text)
            {
                tempText += a;
                counter++;
                if (tempText.Length == 15 && counter < Text.Length)
                {
                    Data = new byte[16];
                    for (int i = 0; i < 15; i++)
                    {
                        Data[i] = Convert.ToByte(tempText[i]);
                    }

                    tempBlockID = Controller.returnNextFreeBlockID();
                    lastBlockID = tempBlockID;
                    Data[15] = Convert.ToByte(tempBlockID);

                    if (currentBlockID <= blocksList.Count)
                    {
                        blocksList[currentBlockID - 1] = Data;
                    }
                    else
                    {
                        blocksList.Add(Data);
                    }
                    currentBlockID = tempBlockID;
                    fileSize++;
                    tempText = "";
                }
                else if (counter == Text.Length)
                {
                    Data = new byte[16];
                    for (int i = 0; i < tempText.Length; i++)
                    {
                        Data[i] = Convert.ToByte(tempText[i]);
                    }
                    Data[15] = 0;

                    if (currentBlockID <= blocksList.Count)
                    {
                        blocksList[currentBlockID - 1] = Data;
                    }
                    else
                    {
                        blocksList.Add(Data);
                    }
                    fileSize++;
                    if (lastBlockID == 0) lastBlockID = tempBlockID;
                    Controller.setLastFileBlock(fileName, lastBlockID);
                }
            }
            Controller.updateFileSize(fileName, fileSize);
        }
    }
}
