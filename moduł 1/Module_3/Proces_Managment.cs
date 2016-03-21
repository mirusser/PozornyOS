using System;
using System.Collections.Generic;
using System.Linq;
using moduł_pamięci;

namespace Moduł_3_Zarządzanie_procesami_wyższy
{
    class Proces_Managment
    {
        #region Publiczne pola klasy Proces
        public List<PCB> pcb_all;
        public Pamięć pamięć = new Pamięć();
        #endregion

        #region Konstruktor
        public Proces_Managment()
        {
            pcb_all = new List<PCB>();
        }
        #endregion

        #region Tworzenie procesu - XC
        public void XC(string name, string parent_name = null)
        {
            if (XN(name) == null)
            {
                PCB pcb = new PCB(name); // utworzenie PCB o podanaje nazwie  

                if (XN(parent_name) == null) //sprawdzenie czy rodzic już istnieje, ta część kodu jest tylko po to by utworzyć proces root
                {
                    if (pcb_all.Count == 0)
                    {
                        XI(ref pcb); //"root" lub inaczej "init" tworzy początek drzewa
                        //Console.WriteLine("Utworzono proces: {0}", name);                        
                    }
                    else
                    {
                        Console.WriteLine("Nie można utworzyć procesu o nazwie: {0}", name);
                        Console.WriteLine("Rodzic o podanej nazwie \"{0}\" nie istnieje", parent_name);
                        XQUE();
                    }
                }
                else
                {
                    do //wczytywanie wielkości procesu, wielkości danych i wielkości stosu od użytkownika
                    {
                        try // zabezpieczenie przed podaniem pustej wartości przez użytkownika
                        {
                            Console.Write("Podaj wielkość kodu procesu {0}: ", pcb.Name);
                            pcb.Auto_storage_size = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Podaj wielkość danych procesu {0}: ", pcb.Name);
                            pcb.Auto_data_size = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Podaj wielkość stosu procesu {0}: ", pcb.Name);
                            pcb.Auto_stack_size = Convert.ToInt32(Console.ReadLine());
                            break;

                        }
                        catch
                        {
                            Console.WriteLine("Nie podano żadnej wartości.");
                        }
                    } while (true);

                    try
                    {
                        bool czy_przydzielono_pamięc = pamięć.XA(ref pcb);
                        if (czy_przydzielono_pamięc)
                        {
                            XI(ref pcb, parent_name); //dołączenie nowego pcb do dwóch łańcuchów PCB i struktury hierarchicznej
                            Console.WriteLine("Zaalokowano pamięć dla procesu: {0}", name);
                            Console.WriteLine("Utworzono proces: {0}", name);
                        }
                        else
                        {
                            XI(ref pcb, parent_name);
                            Console.WriteLine("Nie można zaalokować pamięci dla procesu: {0}", pcb.Name);
                            Console.WriteLine("Proces utworzono, lecz nie ma zaalokowanej pamięci");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Wymagana pamięć do przydzielenia przekracza możliwości tego systemu.");
                        Console.WriteLine("Nie utworzono procesu");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nie można utworzyć procesu o nazwie: {0}", name);
                Console.WriteLine("Blok PCB o podanej nazwie już istnieje");
                XQUE();
            }
        }
        #endregion

        #region Usuwanie procesu - XD
        public void XD(string name)
        {
            PCB pcb = XN(name); // sprawdzenie czy proces o podanej nazwie w ogóle istnieje
            if (pcb != null) //jeśli istnieje to można go usuwać
            {
                if(pcb.Name == "root") //procesu root nie można normalnie usunać
                {
                    Console.WriteLine("Nie można usunąć procesu \"root\"\nJest on usuwany podczas zamykania systemu.");
                }
                else
                {
                    XZ(name); // zatrzymanie procesu
                    Console.WriteLine("Czytanie komunikatów usuwanego procesu {0}: ", pcb.Name);
                    if (pcb.First_message == null)
                    {
                        Console.WriteLine("Brak komunikatów do przeczytania");
                    }
                    else
                    {
                        while (pcb.First_message!= null)
                        {
                            XR(pcb.Name);
                        }
                    }

                    XJ(ref pcb); // usunięcie procesu ze struktury drzewa i dwóch łańcuchów PCB
                    pcb.Auto_data_size = 0;
                    pcb.Auto_stack_size = 0;
                    pcb.Auto_storage_size = 0;

                    if (pamięć.XF(ref pcb)) // - zwalnia przydzieloną pamięć, zrobione przez Pablo
                    {
                        
                        Console.WriteLine("Zwolniono pamięć dla procesu: {0}", pcb.Name);
                        Console.WriteLine("Usunięto blok PCB o nazwie: {0}", pcb.Name);
                    }
                    else
                    {
                        Console.WriteLine("Błąd zwalniana pamięci procesu: {0}", pcb.Name);
                        XQUE();
                    }
                }
               
            }
            else
            {
                Console.WriteLine("Błąd podczas usuwania procesu");
                Console.WriteLine("Nie można znaleźć bloku PCB o podanej nazwie: {0}", name);
                XQUE();
            }
        }
        #endregion

        #region Uruchomienie procesu - XY
        public void XY(string name) 
        {
            PCB pcb = XN(name);
            if (pcb != null)
            {
                pcb.Stopped = false;
                Console.WriteLine("Proces został uruchomiony!");
            }
            else
            {
                Console.WriteLine("Nie można uruchomić procesu {0}", name + "\nW programie XY");
                XQUE();
            }
        }
        #endregion

        #region Zatrzymanie procesu - XZ
        public void XZ(string name)
        {
            PCB pcb = XN(name);
            if (pcb != null)
            {
                if (pcb.In_smc == 0)
                {
                    pcb.Stopped = true;
                    Console.WriteLine("Zatrzymano proces: {0}", pcb.Name + "\nProces nie był w SMC");
                }
                else
                {
                    pcb.Stop_waiting = true;
                    pcb.Stopper_semaphore.P(ref pcb);
                    Console.WriteLine("Zatrzymanie procesu: {0}", pcb.Name + " Jest w SMC");
                }
            }
            else
            {
                XQUE();
                Console.WriteLine("W XZ");
            }
        }
        #endregion

        #region Wyszukiwanie procesu - XN
        public PCB XN(string name)
        {
            if (pcb_all.Count != 0)//jeżeli lista jest niepusta, to można wyszukiwać w niej jakiś proces
            {
                PCB result = pcb_all.Find(element => element.Name == name);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Dodawanie PCB do dwóch łańcuchów PCB - XI
        public void XI(ref PCB pcb) 
        {
            if (pcb_all.Count == 0) // jeżeli łańcuch wszystkich PCB jest pusty to nie można ustawić wskaźników na kolejny i poprzedni PCB
            {
                pcb_all.Add(pcb);
            }
            else // jeżeli nie pusty to można ;)
            {
                PCB result = pcb_all.FirstOrDefault(element => element.Next_pcb_all == null);
                if (result != null)
                {
                    result.Next_pcb_all = pcb;
                }
                pcb.Last_pcb_all = pcb_all.Last();
                pcb_all.Add(pcb);
            }
        }

        public void XI(ref PCB pcb, string parent_name = null)
        {
            PCB parent = XN(parent_name);
            if (parent != null)
            {
                if (pcb_all.Count == 1)
                {
                    parent.First_son = pcb;
                    pcb.Parent = parent;
                    parent.Next_pcb_all = pcb;
                    pcb.Last_pcb_all = parent;
                    pcb_all.Add(pcb);
                }
                else if (pcb_all.Count > 1)
                {

                    if (parent.First_son != null)
                    {
                        pcb.Parent = parent;
                        pcb.Last_pcb_all = pcb_all.Last();
                        if (pcb_all.Last().Next_pcb_all == null)
                        {
                            pcb_all.Last().Next_pcb_all = pcb;
                        }
                        PCB temporary = parent.First_son;
                        while (temporary != null)
                        {
                            if (temporary.Next_pcb_this_group == null)
                            {
                                pcb.Last_pcb_this_group = temporary;
                                break;
                            }
                            temporary = temporary.Next_pcb_this_group;
                        }
                        PCB temp = parent.First_son;
                        while (temp != null)
                        {
                            if (temp.Next_pcb_this_group == null)
                            {
                                temp.Next_pcb_this_group = pcb;
                                break;
                            }
                            temp = temp.Next_pcb_this_group;
                        }
                        pcb_all.Add(pcb);
                    }
                    else if (parent.First_son == null)
                    {
                        parent.First_son = pcb;
                        pcb.Parent = parent;
                        pcb.Last_pcb_all = pcb_all.Last();
                        if (pcb_all.Last().Next_pcb_all == null)
                        {
                            pcb_all.Last().Next_pcb_all = pcb;
                        }
                        pcb_all.Add(pcb);
                    }
                }
            }
            else
            {
                pcb_all.Add(pcb);
            }

        }
        #endregion

        #region Usuwanie PCB z obydwóch łańcuchów PCB - XJ
        public void XJ(ref PCB pcb)
        {
            if (pcb.Parent == null && pcb.First_son == null)
            {
                if (pcb.Next_pcb_this_group == null)
                {
                    if (pcb.Last_pcb_this_group != null)
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = null;
                        pcb.Last_pcb_this_group = null;
                    }
                }
                else
                {
                    if (pcb.Last_pcb_this_group == null)
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = null;
                        pcb.Next_pcb_this_group = null;
                    }
                    else
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = pcb.Next_pcb_this_group;
                        pcb.Next_pcb_this_group.Last_pcb_this_group = pcb.Last_pcb_this_group;
                        pcb.Last_pcb_this_group = null;
                        pcb.Next_pcb_this_group = null;
                    }
                }

                if (pcb_all.Count == 1) //czyszczenie wskaźników na kolejne,poprzednie PCB w łańcuchach, jeżeli taka potrzeba to odpowiednie jeszcze ich ustawienie
                {
                    pcb.Next_pcb_all = null;
                    pcb.Last_pcb_all = null;
                    pcb_all.Remove(pcb);
                }
                else if (pcb_all.Count == 2)
                {
                    if (pcb.Last_pcb_all == null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
                else if (pcb_all.Count > 2)
                {
                    if (pcb.Last_pcb_all == null && pcb.Next_pcb_all != null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all != null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = pcb.Next_pcb_all;
                        pcb.Next_pcb_all.Last_pcb_all = pcb.Last_pcb_all;
                        pcb.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
            }
            else if (pcb.Parent != null && pcb.First_son == null)
            {
                if (pcb.Next_pcb_this_group == null)
                {
                    if (pcb.Parent.First_son == pcb)
                    {
                        pcb.Parent.First_son = null;
                    }
                    if (pcb.Last_pcb_this_group != null)
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = null;
                        pcb.Last_pcb_this_group = null;
                    }
                }
                else
                {
                    if (pcb.Parent.First_son == pcb)
                    {
                        pcb.Parent.First_son = pcb.Next_pcb_this_group;
                    }

                    if (pcb.Last_pcb_this_group == null)
                    {
                        pcb.Next_pcb_this_group.Last_pcb_this_group = null;
                    }
                    else
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = pcb.Next_pcb_this_group;
                        pcb.Next_pcb_this_group.Last_pcb_this_group = pcb.Last_pcb_this_group;
                        pcb.Last_pcb_this_group = null;
                        pcb.Next_pcb_this_group = null;
                    }
                }
                pcb.Parent = null;
                if (pcb_all.Count == 1) //czyszczenie wskaźników na kolejne,poprzednie PCB w łańcuchach, jeżeli taka potrzeba to odpowiednie jeszcze ich ustawienie
                {
                    pcb.Next_pcb_all = null;
                    pcb.Last_pcb_all = null;
                    pcb_all.Remove(pcb);
                }
                else if (pcb_all.Count == 2)
                {
                    if (pcb.Last_pcb_all == null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
                else if (pcb_all.Count > 2)
                {
                    if (pcb.Last_pcb_all == null && pcb.Next_pcb_all != null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all != null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = pcb.Next_pcb_all;
                        pcb.Next_pcb_all.Last_pcb_all = pcb.Last_pcb_all;
                        pcb.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
            }
            else if (pcb.Parent != null && pcb.First_son != null)
            {
                PCB temporary = pcb.First_son;
                while (temporary != null)
                {
                    temporary.Parent = pcb.Parent;
                    temporary = temporary.Next_pcb_this_group;
                }


                if (pcb.Next_pcb_this_group == null)
                {
                    if (pcb.Last_pcb_this_group == null)
                    {
                        pcb.First_son.Last_pcb_this_group = null;
                    }
                    else
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = pcb.First_son;
                        pcb.First_son.Last_pcb_this_group = pcb.Last_pcb_this_group;
                        pcb.Last_pcb_this_group = null;
                        pcb.Next_pcb_this_group = null;
                    }
                }
                else
                {
                    if (pcb.Parent.First_son == pcb)
                    {
                        pcb.Parent.First_son = pcb.Next_pcb_this_group;
                    }

                    if (pcb.Last_pcb_this_group == null)
                    {
                        pcb.Next_pcb_this_group.Last_pcb_this_group = null;
                    }
                    else
                    {
                        pcb.Last_pcb_this_group.Next_pcb_this_group = pcb.Next_pcb_this_group;
                        pcb.Next_pcb_this_group.Last_pcb_this_group = pcb.Last_pcb_this_group;
                    }

                    PCB temp = pcb;
                    while (temp != null)
                    {
                        if (temp.Next_pcb_this_group == null)
                        {
                            pcb.First_son.Last_pcb_this_group = temp;
                            temp.Next_pcb_this_group = pcb.First_son;
                            break;
                        }
                        temp = temp.Next_pcb_this_group;
                    }
                    pcb.Last_pcb_this_group = null;
                    pcb.Next_pcb_this_group = null;

                }
                if (pcb_all.Count == 1) //czyszczenie wskaźników na kolejne,poprzednie PCB w łańcuchach, jeżeli taka potrzeba to odpowiednie jeszcze ich ustawienie
                {
                    pcb.Next_pcb_all = null;
                    pcb.Last_pcb_all = null;
                    pcb_all.Remove(pcb);
                }
                else if (pcb_all.Count == 2)
                {
                    if (pcb.Last_pcb_all == null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
                else if (pcb_all.Count > 2)
                {
                    if (pcb.Last_pcb_all == null && pcb.Next_pcb_all != null)
                    {
                        pcb.Next_pcb_all.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all != null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = pcb.Next_pcb_all;
                        pcb.Next_pcb_all.Last_pcb_all = pcb.Last_pcb_all;
                        pcb.Last_pcb_all = null;
                        pcb.Next_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                    else if (pcb.Last_pcb_all != null && pcb.Next_pcb_all == null)
                    {
                        pcb.Last_pcb_all.Next_pcb_all = null;
                        pcb.Last_pcb_all = null;
                        pcb_all.Remove(pcb);
                    }
                }
            }
            pcb.Parent = null;
        }
        #endregion

        #region Zatrzymanie zlecenia i powiadomienie programu nadzorczego - XH
        public void XH(string name)
        {
            Console.WriteLine("Wywołano program \"XH\"");
            PCB pcb = XN(name);
            if (pcb != null)
            {
                pcb.Stopped = true;
                Console.WriteLine("Odczytywanie i usuwanie komunikatów: ");
                while (pcb.First_message != null)
                {
                    XR(pcb.Name);
                }

                XS("root","root", "Zatrzymaj proces, wywołał program \"XH\"");
            }
            else
            {
                Console.WriteLine("Nie można znaleźć procesu {0}", pcb.Name + "\nProgram XH został przerwany");
            }
        }
        #endregion

        #region Obsługa błędów - XQUE
        public void XQUE()
        {
            XS("root", "root", "Nienormalne zakończenie zlecenia");
            Console.WriteLine("Wywołanie funkcji XQUE");
        }
        #endregion 

        #region Komunikacja - XS & XR
        #region Nadanie komunikatu - XS
        public void XS(string receiver_name, string sender_name, string text) // uważam za zbędny parametr, jakim jest długość komunikatu, przynajmniej przy braku alokacji pamięci przez Pawła, dodałem nazwę odbiorcy
        {
            PCB sender = XN(sender_name);
            if (sender != null)
            {
                PCB receiver = XN(receiver_name);
                if (receiver != null)
                {
                    Komunikat komunikat = new Komunikat(ref sender, text);
                    receiver.Message_semaphore_common.P();
                    //receiver.Message_semaphore_receiver.P(ref komunikat);
                    receiver.Message_semaphore_receiver.V(ref receiver);
                    if (receiver.First_message == null || receiver.Message_semaphore_receiver.Value == 1)
                        receiver.First_message = komunikat;
                    else if (receiver.Message_semaphore_receiver.Value > 1)
                    {
                        Komunikat temp = receiver.First_message;
                        while (temp != null)
                        {
                            if (temp.Next == null)
                            {
                                temp.Next = komunikat;
                                break;
                            }
                            temp = temp.Next;
                        }
                    }

                    receiver.Message_semaphore_common.V();
                    Console.WriteLine("Wysłano komunikat.");
                }
                else
                {
                    XQUE();
                    Console.WriteLine("Błędny odbioraca. \nW programie XS");
                }
            }
            else
            {
                XQUE();
                Console.WriteLine("Błędny nadawca. \nW programie XS");
            }

        }
        #endregion

        #region Czytanie komunikatu - XR
        public void XR(string reader_name)
        {
            PCB reader = XN(reader_name);
            if (reader != null)
            {
                if (reader.Message_semaphore_receiver.Value > 0 || reader.First_message != null)
                {
                    Console.WriteLine("Odebrano komunikat.");
                    reader.Message_semaphore_common.P();
                    Console.WriteLine("Nadawca komunikatu: {0}", reader.First_message.Sender.Name);
                    Console.WriteLine("Treść komunikatu: {0}", reader.First_message.Text);
                    //reader.Message_semaphore_receiver.V(ref reader, reader_name);
                    reader.Message_semaphore_receiver.P(ref reader);
                        try
                        {
                            if (reader.First_message.Next != null)
                                reader.First_message = reader.First_message.Next;
                            else
                                reader.First_message = null;
                        }
                        catch
                        {
                            reader.First_message = null;
                            Console.WriteLine("First_message.Next == null");
                        }
                    reader.Message_semaphore_common.V();
                }
                else
                {
                    reader.Message_semaphore_common.P();
                    reader.Message_semaphore_receiver.P(ref reader);
                    Console.WriteLine("Brak komunikatu, czekam na nadejście");
                    reader.Message_semaphore_common.V();
                    XZ(reader_name);
                }
            }
            else
            {
                XQUE();
                Console.WriteLine("Nie znaleziono procesu, który ma czytać komunikat/y.\nW XR()");
            }
        }
        #endregion
        #endregion

        #region Metody dodatkowe - Display_PCB, Display_all
        #region Wyświetlanie zawartości bloku PCB
        public void Display_PCB(string name)
        {
            PCB pcb = XN(name);
            if (pcb != null)
            {
                Console.Write("Chcesz wyświetlić zawartość semaforów? [t/n]:");
                try
                {
                    if (Console.ReadLine() == "t" || Console.ReadLine() =="T")
                    {
                        Console.WriteLine("\tMessage_semaphore_common");
                        pcb.Message_semaphore_common.Display_PCBs();
                        Console.WriteLine("\tMessage_semaphore_receiver");
                        pcb.Message_semaphore_receiver.Display_PCBs();
                        Console.WriteLine("\tStopper_semaphore");
                        pcb.Stopper_semaphore.Display_PCBs();
                        Console.WriteLine("\tStoppe_semaphore");
                        pcb.Stoppe_semaphore.Display_PCBs();
                    }
                    else
                    {
                        Console.WriteLine("Nie wyświetlono zawartości semaforów");
                    }
                }
                catch
                {
                    Console.WriteLine("Nie podano żadnej odpowiedzi");
                }             
                Console.WriteLine("Wyświetlanie stanu pól PCB procesu: {0}", name);
                Console.WriteLine("\tStopped: {0}", pcb.Stopped);
                Console.WriteLine("\tBlocked: {0}", pcb.Blocked);
                Console.WriteLine("\tIn_SMC: {0}", pcb.In_smc);
                try
                {
                    Console.WriteLine("\tFirst_message: \"{0}\"", pcb.First_message.Text);
                }
                catch
                {
                    Console.WriteLine("\tFirst_message: Brak wiadomości");
                }
                Console.WriteLine("\tIlość komórek pamięci przydzielona na kod: {0}", pcb.Auto_storage_size);
                Console.WriteLine("\tAdres początku pamięci przydzielonej na kod: {0}", pcb.Auto_storage_addres);
                Console.WriteLine("\tIlość komórek pamięci przydzielona na dane: {0}", pcb.Auto_data_size);
                Console.WriteLine("\tAdres początku pamięci przydzielonej na dane: {0}", pcb.Auto_data_addres);
                Console.WriteLine("\tIlość komórek pamięci przydzielonej na stos: {0}", pcb.Auto_stack_size);
                Console.WriteLine("\tAdres poczatku pamięci przydzielonej na stos: {0}", pcb.Auto_stack_addres);
                try
                {
                    if (pcb.Last_pcb_this_group.Name == null)
                        Console.WriteLine("\tLast_PCB_this_group: null");
                    else
                        Console.WriteLine("\tLast_PCB_this_group: {0}", pcb.Last_pcb_this_group.Name);
                }
                catch
                {
                    Console.WriteLine("\tPusta referecja do Last_PCB_this_group");
                }

                try
                {
                    if (pcb.Next_pcb_this_group.Name == null)
                        Console.WriteLine("\tNext_PCB_this_group: null");
                    else
                        Console.WriteLine("\tNext_PCB_this_group: {0}", pcb.Next_pcb_this_group.Name);
                }
                catch
                {
                    Console.WriteLine("\tPusta referecja do Next_pcb_this_group");
                }

                try
                {
                    if (pcb.Last_pcb_all.Name == null)
                        Console.WriteLine("\tLast_pcb_all: null");
                    else
                        Console.WriteLine("\tLast_pcb_all: {0}", pcb.Last_pcb_all.Name);
                }
                catch
                {
                    Console.WriteLine("\tPusta referecja do Last_pcb_all");
                }

                try
                {
                    if (pcb.Next_pcb_all.Name == null)
                        Console.WriteLine("\tNext_pcb_all: null");
                    else
                        Console.WriteLine("\tNext_pcb_all: {0}", pcb.Next_pcb_all.Name);
                }
                catch
                {
                    Console.WriteLine("\tPusta referecja do Next_pcb_all");
                }
                try
                {
                    Console.WriteLine("\tRodzic: {0}", pcb.Parent.Name);
                }
                catch
                {
                    Console.WriteLine("\tBrak rodzica");
                }
                try
                {
                    Console.WriteLine("\tPierwszy syn: {0}", pcb.First_son.Name);
                }
                catch
                {
                    Console.WriteLine("\tBrak pierwszego syna");
                }
            }
            else
            {
                Console.WriteLine("Nie można wyświetlić pól PCB, wyszukiwane PCB nie powiodło się");
            }
            Console.WriteLine("Ilość bloków PCB w pcb_all: {0}", pcb_all.Count);
        }
        #endregion

        #region Wyświetlanie wszystkich PCB
        public void Display_all()
        {
            if (pcb_all.Count > 0)
            {
                PCB pcb = pcb_all.First();
                Console.WriteLine("Lista wszystkich procesów:");
                while (pcb.Next_pcb_all != null)
                {
                    Console.WriteLine("\t{0}", pcb.Name);
                    pcb = pcb.Next_pcb_all;
                }
                Console.WriteLine("\t{0}", pcb.Name);
            }
            else
            {
                Console.WriteLine("Lista wszystkich procesów jest pusta");
            }
        }
        #endregion
        #endregion
    }
}