using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moduł_3_Zarządzanie_procesami_wyższy
{
    class PCB
    /*Blok kontrolny procesu służy jako magazyn przechowywyujący wszelkie informacje dla różnych procesów*/
    {
        #region Prywatne pola klasy PCB
        private string name; //nazwa danego PCB

        private PCB next_pcb_this_group; // w książce opisane jako wskaźnik do następnego bloku PCB w tej samej grupie, wskaźnik w przód
        private PCB last_pcb_this_group; // wskaźnik na poprzedni pcb w  tej samej grupie
        private PCB next_pcb_all; // wskaźnik na kolejny PCB z wszystkich bloków pcb
        private PCB last_pcb_all; // wskaźnik na poprzedni PCB z wszystkich bloków pcb
        private PCB next_semaphore_waiter;  // wskazuje następny blok PCB czekając pod danym (jakimś) semaforem

        private Komunikat first_message;// wskaźnik na pierwszy komunikat

        private bool stopped; //gdy bit ten ==1 proces nie może być wykonywany
        private bool blocked; //zablokowany gdy bit ten ==1; zablokowany proces nie może być zatrzymany
        private int in_smc; //gdy in_smc>0 proces znajduje się w sekcji SMC, a gdy się w niej znaduje proces nie moze być normalnie zatrzymany, próby zatrzymania go nie dają rezultatów
        private bool stop_waiting; // zatrzymanie czekającego, bit ten ma wartość 1 to istneje żądanie zatrzymania go, gdy znajduję się w sekcji SMC 

        int auto_storage_size;//pole to zawiera liczbę komórek (wielkość) jakie zostały przydzieone procesowi
        int auto_storage_addres;// - adres pamięci własnej - a'la wskaźnik   
        int adres_początku_danych;
        int długość_danych;
        int adres_stosu;
        int wielkość_stostu;

        private PCB parent;
        private PCB first_son;

        #region Semafory
        private Semaphore message_semaphore_common;// wspólny semafor komunikatów, stosuję się go gdy jakiś proces zamierza przesłać komunikat, lub proces chce przeczytać wiadomość, wtedy semafor ten ==1, wartość początkowa to 0
        private Semaphore message_semaphore_receiver;// pobudza proces otrzymujący komunikat, wartość poczatkowa =0, proces A wysyłający komunikat do procesu B, wykonuje operację V na tym semaforze (w PCB procesu B), operację P wykonuję proces B, gdy próbuje odczytać komunikat
        private Semaphore stopper_semaphore; // realizuje czekanie na zatrzymanie procesu znajdującego się w SMC, wartość pocztakowa=0,
        private Semaphore stoppe_semaphore; // semafor zatrzymywanego, wartość poczatkowa = 0; stosuje isę go aby zatrzymać proces, którego zatrzymanie odłożono do momentu wyjści procesu z SMC

        #endregion
        #endregion

        #region Publiczne pola klasy

        #endregion

        #region Publiczne właściwości
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public PCB Next_pcb_this_group
        {
            get { return next_pcb_this_group; }
            set { next_pcb_this_group = value; }
        }
        public PCB Last_pcb_this_group
        {
            get { return last_pcb_this_group; }
            set { last_pcb_this_group = value; }
        }
        public PCB Next_pcb_all
        {
            get { return next_pcb_all; }
            set { next_pcb_all = value; }
        }
        public PCB Last_pcb_all
        {
            get { return last_pcb_all; }
            set { last_pcb_all = value; }
        }
        public PCB Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public PCB First_son
        {
            get { return first_son; }
            set { first_son = value; }
        }
        public bool Stopped
        {
            get { return stopped; }
            set { stopped = value; }
        }
        public bool Blocked
        {
            get { return blocked; }
            set { blocked = value; }
        }
        public int In_smc
        {
            get { return in_smc; }
            set { in_smc = value; }
        }
        public bool Stop_waiting
        {
            get { return stop_waiting; }
            set { stop_waiting = value; }
        }
        public int Auto_storage_size
        {
            get { return auto_storage_size; }
            set { auto_storage_size = value; }
        }
        public int Auto_storage_addres
        {
            get { return auto_storage_addres; }
            set { auto_storage_addres = value; }
        }
        public Semaphore Message_semaphore_common
        {
            get { return message_semaphore_common; }
            set { message_semaphore_common = value; }
        }
        public Semaphore Message_semaphore_receiver
        {
            get { return message_semaphore_receiver; }
            set { message_semaphore_receiver = value; }
        }
        public Komunikat First_message
        {
            get { return first_message; }
            set { first_message = value; }
        }
        public PCB Next_semaphore_waiter
        {
            get { return next_semaphore_waiter; }
            set { next_semaphore_waiter = value; }
        }
        public Semaphore Stopper_semaphore
        {
            get { return stopper_semaphore; }
            set { stopper_semaphore = value; }
        }
        public Semaphore Stoppe_semaphore
        {
            get { return stoppe_semaphore; }
            set { stoppe_semaphore = value; }
        }
        public int Auto_data_addres
        {
            get { return adres_początku_danych; }
            set { adres_początku_danych = value; }
        }
        public int Auto_data_size
        {
            get { return długość_danych; }
            set { długość_danych = value; }
        }
        public int Auto_stack_addres
        {
            get { return adres_stosu; }
            set { adres_stosu = value; }
        }
        public int Auto_stack_size
        {
            get { return wielkość_stostu; }
            set { wielkość_stostu = value; }
        }
        #endregion

        #region Konstruktor
        public PCB(string name)
        {
            Name = name;
            stopped = true;
            blocked = false;
            in_smc = 0;
            stop_waiting = false;
            message_semaphore_common = new Semaphore(0);
            message_semaphore_receiver = new Semaphore(0);
            first_message = null;
            next_semaphore_waiter = null;
            stopper_semaphore = new Semaphore(0);
            Stoppe_semaphore = new Semaphore(0);
            next_pcb_this_group = null;
            last_pcb_this_group = null;
            next_pcb_all = null;
            last_pcb_all = null;
            Parent = null;
            First_son = null;
            auto_storage_addres = -1;
            auto_storage_size = 0;
            Auto_data_addres = -1;
            Auto_data_size = 0;
            Auto_stack_addres = -1;
            Auto_stack_size = 0;
        }
        #endregion
    }
}
