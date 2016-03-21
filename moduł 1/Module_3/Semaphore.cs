using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduł_3_Zarządzanie_procesami_wyższy
{
    class Semaphore
    {
        #region Prywatne pola
        int value;
        PCB first_waiter;
        #endregion

        #region Properties
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public PCB First_waiter
        {
            get { return first_waiter; }
            set { first_waiter = value; }
        }
        #endregion

        #region Konstruktor
        public Semaphore(int value)
        {
            Value = value;
            First_waiter = null;
        }
        #endregion

        #region Operacje P & V
        #region Operacja P
        public void P()
        {
            value--;
        }
        public void P(ref PCB pcb)
        {
            value--;
           
            if (value == -1)
            {
                First_waiter = pcb;
                pcb.Blocked = true;
            }
            else if (value < -1)
            {
                pcb.Blocked = true;
                PCB temp = First_waiter;
                while (temp != null)
                {
                    if (temp.Next_semaphore_waiter == null)
                    {
                        temp.Next_semaphore_waiter = pcb;
                        break;
                    }
                    if(temp == temp.Next_semaphore_waiter)
                    {
                        temp.Next_semaphore_waiter = null;
                        break;
                    }
                    temp = temp.Next_semaphore_waiter;
                }
            }
        }
        #endregion

        #region Operacja V
        public void V()
        {
            value++;
        }
        public void V(ref PCB pcb)
        {
            value++;
            if (Value <= 0)
            {
                try
                {
                    First_waiter = First_waiter.Next_semaphore_waiter;
                }
                catch
                {
                    Console.WriteLine("Pusta referencja. W programie \"V\"");
                }

                if (Value == 0)
                {
                    First_waiter = null;
                }

            }
            pcb.Blocked = false;
        }
        #endregion
        #endregion

        #region Wyświetlanie zawartości semafora
        public void Display_PCBs()
        {
            Console.WriteLine("\t\tWartość semafora: {0}", Value);
            Console.WriteLine("\t\tObiekty czekające pod semaforem: ");
            if(first_waiter!=null)
            {
                Console.WriteLine("\t\t" + first_waiter.Name);
                PCB temp = First_waiter;
                while (temp != null)
                {
                    if (temp.Next_semaphore_waiter == null)
                    {
                        break;
                    }
                    temp = temp.Next_semaphore_waiter;
                    Console.WriteLine("\t\t" + temp.Name);
                }
            }
            else
            {
                Console.WriteLine("\t\tŻaden obiekt nie czeka pod semaforem");
            }
        }
        #endregion
    }
}
