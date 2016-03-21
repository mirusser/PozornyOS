using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduł_3_Zarządzanie_procesami_wyższy
{
    class Komunikat
    {
        #region Prywatne pola klasy 
        private PCB sender; // nadawca komunikatu, wskażnik do PCB który nadesłał komunikat 
        private Komunikat next; //wskazuje na następny komunikat do przeczytania
        private int size; //rozmiar komunikatu
        private string text;//tekst komunikatu
        #endregion

        #region Publiczne właściwości
        public PCB Sender
        {
            get
            {
                return sender;
            }
            set
            {
                sender = value;
            }

        }
        public Komunikat Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }

        }
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }

        }
        #endregion

        #region Konstruktor
        public Komunikat(ref PCB sender, string text)
        {
            Sender = sender;
            Text = text;
            Size = text.Length;
            Next = null;
        }
        #endregion
    }
}
