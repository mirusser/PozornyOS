using OperationSystem_DiscSystem.Module_4.ExceptionNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source.FreeBlocksSystem
{
    class FreeBlocks
    {
        /* Tablica bloków wolnych */
        private byte[] freeBlocks;

        /* Konstruktor */
        public FreeBlocks()
        {
            freeBlocks = new byte[Controller.getBlockAmount()];
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                freeBlocks[i] = 0;
            }
        }

        /*======================Metody dostępowe======================*/

        /* Zwraca kolejny wolny blok */
        public int getNextFreeBlock()
        {
            if (isFreeBlock())
            {
                for (int i = 0; i < freeBlocks.Length; i++)
                {
                    if (freeBlocks[i] == 0)
                    {
                        if (i + 1 > Controller.getBlockCount()) Controller.addNewBlock();
                        freeBlocks[i] = 1;
                        return i + 1;
                    }
                }
            }
            return 0;
        }
        /* Zwraca tablicę wszystkich bloków */
        public byte[] returnAllBlocks()
        {
            byte[] tempTable = new byte[Controller.getBlockAmount()];
            tempTable = freeBlocks;
            return tempTable;
        }
        /* Aktualizuje tablice bloków wolnych */
        public void updateFreeBlocksList(int[] index)
        {
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                if (freeBlocks[i] == index[i])
                {
                    freeBlocks[i] = 0;
                }
            }
        }
        /* Sprawdza czy jest wystarczająco wolnych bloków */
        public bool isEnoughFreeBlocks(String Text)
        {
            int howManyBlocksNeed = Text.Length / 15 + 1;
            if (howManyBlocksNeed <= freeBlocksCounts()) return true;
            else return false;
        }
        /* Sprawdza czy jest wystarczająco wolnych bloków */
        public bool isEnoughFreeBlocks(int length)
        {
            int howManyBlocksNeed = length / 15 + 1;
            if (howManyBlocksNeed <= freeBlocksCounts()) return true;
            else return false;
        }
        /* Ustawia indeksy na wolne po usunięciu pliku */
        public void deleteBlocksPointers(int[] tab)
        {
            foreach (int i in tab)
            {
                freeBlocks[i - 1] = 0;
            }
        }

        /*======================Metody pomocnicze======================*/

        /* Sprawdza czy istnieje wolny blok */
        private bool isFreeBlock()
        {
            for (int i = 0; i < Controller.getBlockAmount(); i++)
            {
                if (freeBlocks[i] == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /* Zwraca liczbę bloków wolnych */
        private int freeBlocksCounts()
        {
            int counter = 0;
            for (int i = 0; i < Controller.getBlockAmount(); i++)
            {
                if (freeBlocks[i] == 0)
                {
                    counter++;
                }
            }

            return counter;
        }
    }
}
