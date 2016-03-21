using Moduł_3_Zarządzanie_procesami_wyższy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semafory_pamięci
{
    class MEMORY //semafor oczekujących
    {
        private Queue<PCB> oczekujące = new Queue<PCB>();
        private int wartość = 0;

        public int getWartość
        {
            get { return wartość; }
        }

        public void P(ref PCB pcb)
        {
            wartość--;

            pcb.Blocked = true;

            oczekujące.Enqueue(pcb);

            
            Console.WriteLine("Dodano proces na semafor oczekujących!");
        }

        public PCB V()
        {
            wartość++;

            Console.WriteLine("Zdjęto proces z semaforu, nastąpi próba alokacji pamięci!");

            PCB pcb = oczekujące.Dequeue();

            pcb.Blocked = false;

            return pcb;
        }

        public void wyświetl_stan_semafora_memory()
        {
            string bufor = "Wartość semafora: " + wartość.ToString() + "\n";

            foreach (PCB pcb in oczekujące)
            {
                bufor += "Nazwa procesu: " + pcb.Name + "\n";
            }

            Console.WriteLine(bufor);
        }

    }
}
