using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TablicaRozszerzana
{
    public class TableResizedEventArgs : EventArgs
    {
        private int initialSize;
        private int currentSize;
        private string description;

        public TableResizedEventArgs(int initialSize, int currentSize)
        {
            this.initialSize = initialSize;
            this.currentSize = currentSize;
            this.description = "Table resized from initial size of " + initialSize + " to current size of " + currentSize;
        }

        public int InitialSize
        {
            get { return this.initialSize; }
        }
        public int CurrentSize
        {
            get { return this.currentSize; }
        }
        public string Description
        {
            get { return this.description; }
        }
    }

    public delegate void TableResizedEventHandler(object sender, TableResizedEventArgs e);

    class Tablica
    {
        private int[] table;
        private int size;
        private int allocatedSize;
        private int arrayExpandFactor;
        private int defaultValue;
        public event TableResizedEventHandler TableResized;
        protected virtual void onTableResized(TableResizedEventArgs e)
        {
            if (TableResized != null)
            {
                TableResized(this, e);
            }
        }
        public int Size
        {
            get { return size; }
        }
        public int this[int index]
        {
            get
            {
                if (index >= 0 && index < size)
                {
                    return table[index];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set { AddAt(index, value); }
        }
        public Tablica(int initialTableSize)
        {
            size = 0;
            arrayExpandFactor = 2;
            defaultValue = -1;
            if (initialTableSize > 4)
            {
                allocatedSize = initialTableSize * arrayExpandFactor;
            }
            else
            {
                allocatedSize = 4 * arrayExpandFactor;
            }
            table = new int[allocatedSize];
        }


        public Tablica() : this(0)
        {
        }

        public void Resize(int targetSize) //Zrobic resize do index * 2, zamiast resize'owac wielokrotnie np dla 1000 indexu
        {
            if (targetSize >= allocatedSize)
            {
                int newAllocatedSize = targetSize * arrayExpandFactor;
                TableResizedEventArgs eventArgs = new TableResizedEventArgs(allocatedSize, newAllocatedSize);
                int[] newTable = new int[newAllocatedSize];
                table.CopyTo(newTable, 0);
                table = newTable;
                allocatedSize = newAllocatedSize;
                onTableResized(eventArgs);
                for (int i = size; i < allocatedSize; i++)
                {
                    table[i] = defaultValue;
                }
            }

        }
        public void Add(int value)
        {
            Resize(size);
            table[size] = value;
            size++;
        }
        public void AddAt(int index, int value)
        {
            if (index >= 0)
            {
                Resize(index);
                table[index] = value;

                if (index >= size)
                {
                    size = index + 1;
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }

    class Program
    {
        static void TableResized(object source, TableResizedEventArgs e)
        {
            Console.WriteLine(e.Description);
        }
        public static void WriteTable(Tablica tab)
        {
            Console.WriteLine("Size " + tab.Size);
            for (int i = 0; i < tab.Size; i++)
            {
                Console.WriteLine("Index " + i + ": " + tab[i]);
            }
        }
        public static void WriteTableWithoutDefaults(Tablica tab)
        {
            Console.WriteLine("Size " + tab.Size);
            for (int i = 0; i < tab.Size; i++)
            {
                if (tab[i] != -1)
                {
                    Console.WriteLine("Index " + i + ": " + tab[i]);
                }
            }
        }
        static void Main(string[] args)
        {
            Tablica tab = new Tablica();
            //tab.TableResized += new TableResizedEventHandler(TableResized);
            tab.TableResized += TableResized;

            tab.Add(7);
            tab.Add(-5);
            tab.AddAt(0, -5);
            tab.Add(2);
            tab.AddAt(7, -45);
            tab[9] = -90;
            tab.AddAt(19, -45);
            WriteTable(tab);
            int val, index;
            do
            {
                Console.WriteLine("Podaj index: ");
                index = int.Parse(Console.ReadLine());
                Console.WriteLine("Podaj wartosc: ");
                val = int.Parse(Console.ReadLine());
                if (index != 0 && val != 0)
                {
                    tab[index] = val;
                }
                WriteTableWithoutDefaults(tab);
            } while (index != 0 && val != 0);
            WriteTable(tab);
            Console.ReadKey();
        }

    }
}
