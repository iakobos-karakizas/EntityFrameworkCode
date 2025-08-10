using ComputerInventory.Data;
using ComputerInventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            int result = -1;
            while (result!=9)
            {
                result = MainMenu();
            }
        }

        static int MainMenu()
        {
            int result = -1;
            ConsoleKeyInfo cki;
            bool cont = false;
            do
            {
                Console.Clear();
                WriteHeader("Welcome to Newbie Data Systems");
                WriteHeader("Main Menu");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do today");
                Console.WriteLine("1. List All Machines in Inventory");
                Console.WriteLine("2. List All Operating Systems");
                Console.WriteLine("3. Data Entry Menu");
                Console.WriteLine("4. Data Modification Menu");
                Console.WriteLine("9. Exit");
                cki = Console.ReadKey();
                try
                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    switch (result)
                    {
                        case 1: break;
                        case 2: { DisplayOperatingSystems(); break; }
                        case 3: { DataEntryMenu(); break; }
                        case 4: { DataModificationMenu(); break; }
                        case 9: { cont = true; break; }
                    }
                } 
                catch (System.FormatException)
                { }
            } while (!cont);
            return result;
        }

        static int DataEntryMenu()
        {
            int result = -1;
            ConsoleKeyInfo cki;
            bool cont= false;
            do
            {
                Console.Clear();
                WriteHeader("Welcome to newbie data systems");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do");
                Console.WriteLine("1.Add a New Machine");
                Console.WriteLine("2.Add a New Operating System");
                Console.WriteLine("3.Add a New Warranty Provider");
                Console.WriteLine("9.Exit menu");
                cki = Console.ReadKey();
                try
                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    switch (result)
                    {
                        case 1: break;
                        case 2: { AddOperatingSystem(); break; }
                        case 3: break; 
                        case 9: { cont = true; break; }
                    }
                }
                catch (System.FormatException ex)
                {
                }
            } while (!cont);
            return result;
        }

        static void DataModificationMenu()
        {
            ConsoleKeyInfo cki;
            int result = -1;
            bool cont = false;
            do
            {
                Console.Clear();
                WriteHeader("Data Modification Menu");
                Console.WriteLine("\r\nPlease select from the list below for what you would like to do");
                Console.WriteLine("1. Delete Operating System");
                Console.WriteLine("2. Modify Operating System");
                Console.WriteLine("3. Delete All Unsupported Operating Systems");
                Console.WriteLine("9. Exit Menu");
                cki = Console.ReadKey();
                try
                {
                    result = Convert.ToInt16(cki.KeyChar.ToString());
                    switch (result)
                    {
                        case 1: SelectOperatingSystem("Delete"); break;
                        case 2: break;
                        case 3: DeleteAllUnsupportedOperatingSystems(); break;
                        case 9: cont = true; break;
                    }
                }
                catch (System.FormatException)
                {

                }
            } while (!cont);
        }
        static void WriteHeader(string headerText)
        {
            Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + headerText.Length / 2) + "}", headerText));
        }

        static bool ValidateYorN(string entry)
        {
            return (entry.ToLower() == "y" || entry.ToLower() == "n") ? true : false;
        }

        static bool CheckForExistingOs(string osName)
        {
            using (var context = new MachineContext())
                return context.OperatingSys.Where(p => p.Name == osName).Any();
        }

        static void AddOperatingSystem()
        {
            Console.Clear();
            ConsoleKeyInfo cki;
            string result;
            bool cont = false;
            OperatingSys os = new OperatingSys();
            string osName = "";
            do
            {
                WriteHeader("Add New Operating System");
                Console.WriteLine("Enter the name of the operating system and hot Enter");
                osName = Console.ReadLine();
                if (osName.Length >= 4)
                    cont = true;
                else
                {
                    Console.WriteLine("Please enter a valis OS name of at least 4 characters.\r\nPress any key to continue..");
                    Console.ReadKey();
                }
            } while (!cont);
            cont = false;
            os.Name = osName;
            Console.WriteLine("Is the operating system still supported? [y or n]");
            do
            {
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);
            if (result.ToLower() == "y")
                os.StillSupported = true;
            else
                os.StillSupported = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"You entered {os.Name} as the Operating System name\r\nIs the OS still supported you entered {os.StillSupported}.\r\nDo you wish tocontinue? [y or n]");
                cki = Console.ReadKey();
                result = cki.KeyChar.ToString();
                cont = ValidateYorN(result);
            } while (!cont);
            if (result.ToLower()=="y")
            {
                bool exists = CheckForExistingOs(os.Name);
                if (exists)
                {
                    Console.WriteLine("\r\nOperating system already exists\r\nPress any key to continue..");
                    Console.ReadKey();
                }
                else
                {
                    using (var context = new MachineContext())
                    {
                        Console.WriteLine("\r\nAttempting to save changes");
                        context.OperatingSys.Add(os);
                        int i = context.SaveChanges();
                        if (i==1)
                        {
                            Console.WriteLine("Data saved.\r\nPress any key to continue..");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }

        static void DisplayOperatingSystems()
        {
            Console.Clear();
            Console.WriteLine("Operating Systems");
            using (var context = new MachineContext())
            {
                foreach (var os in context.OperatingSys.ToList())
                {
                    Console.Write($"Name:{os.Name,-39}\tStill supported = ");
                    if (os.StillSupported)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(os.StillSupported);
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            Console.WriteLine("\r\nPress any key to continue...");
            Console.ReadKey();
        }

        static void DeleteOperatingSystem(int id)
        {
            OperatingSys os = GetOperatingSystemById(id);
            if (os!=null)
            {
                Console.WriteLine($"\r\nAre you sure you want to delete {os.Name} [y or n]");
                ConsoleKeyInfo cki;
                string result;
                bool cont;
                do
                {
                    cki = Console.ReadKey(true);
                    result = cki.KeyChar.ToString();
                    cont = ValidateYorN(result);
                } while (!cont);
                if (result.ToLower() == "y")
                {
                    Console.WriteLine("\r\nDeleteing record...");
                    using (var context = new MachineContext())
                    {
                        context.Remove(os);
                        context.SaveChanges();
                    }
                    Console.WriteLine("Record deleted");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("\r\nDelete Aborted\r\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("\r\nOperating system not found");
                Console.ReadKey();
                SelectOperatingSystem("Delete");
            }
        }

        static void DeleteAllUnsupportedOperatingSystems()
        {
            using (var context = new MachineContext())
            {
                var os = context.OperatingSys.Where(p => p.StillSupported == false);
                Console.WriteLine("\r\nDeleteing all unsupported operating systems...");
                context.OperatingSys.RemoveRange(os);
                int i = context.SaveChanges();
                Console.WriteLine($"{i} records deleted");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }
        }
        static OperatingSys GetOperatingSystemById(int id)
        {
            var context = new MachineContext();
            OperatingSys os = context.OperatingSys.FirstOrDefault(p => p.OperatingSysId == id);
            return os;
        }

        static void ModifyOperatingSystem(int id)
        {

        }
        static void SelectOperatingSystem(string operation)
        {
            ConsoleKeyInfo cki;
            Console.ReadKey();
            WriteHeader($"{operation} an Existing Operating System Entry");
            Console.WriteLine($"{"ID",-7}|{"Name",-50}|Still Supported");
            Console.WriteLine("---------------------------------------");
            using (var context = new MachineContext())
            {
                List<OperatingSys> lOperatingSys = context.OperatingSys.ToList();
                foreach (OperatingSys os in lOperatingSys)
                {
                    Console.WriteLine($"{os.OperatingSysId,-7}|{os.Name,-50}|{os.StillSupported}");
                }
                Console.WriteLine($"\r\nEnter the ID of the record you wish to {operation} and hit Enter\r\nYou can hit Esc to exit this menu");
                bool cont = false;
                string id = "";
                do
                {
                    cki = Console.ReadKey(true);
                    if (cki.Key==ConsoleKey.Escape)
                    {
                        cont = true;
                        id ="";
                    }
                    else if (cki.Key==ConsoleKey.Enter)
                    {
                        if (id.Length > 0)
                            cont = true;
                        else
                            Console.WriteLine("Please enter an ID that is at least 1 digit");
                    }
                    else if (cki.Key == ConsoleKey.Backspace)
                    {
                        Console.Write("\b \b");
                        try
                        {
                            id = id.Substring(0, id.Length - 1);
                        }
                        catch(System.ArgumentOutOfRangeException)
                        {
                            //at the 0 position, can not go any further back
                        }
                    }
                    else
                    {
                        if (char.IsNumber(cki.KeyChar))
                        {
                            id += cki.KeyChar.ToString();
                            Console.Write(cki.KeyChar.ToString());
                        }
                    }
                } while (!cont);
                int osId = Convert.ToInt32(id);
                if (operation == "Delete")
                    DeleteOperatingSystem(osId);
                else if (operation == "Modify") {
                    //Modify Operating System 
                }
            }
        }
    }
}
