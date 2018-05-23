using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TablicaRozszerzana
{
    class Program
    {
        public static void WriteTable(Tablica tab)
        {
            Console.WriteLine("Size " + tab.Size);
            for (int i = 0; i < tab.Size; i++)
            {
                Console.WriteLine("Index " + i + ": " + tab[i]);
            }
        }
        static void Main(string[] args)
        {
            Tablica tab = new Tablica();
           
            tab.Add(7);
            tab.Add(-5);
            tab.AddAt(-100, 0);
            tab.Add(2);
            tab.AddAt(-111, 7);
            tab[14] = -90;
            WriteTable(tab);
            int val, index;
            do
            {
                Console.WriteLine("Podaj index: ");
                index = int.Parse(Console.ReadLine());
                Console.WriteLine("Podaj wartosc: ");
                val = int.Parse(Console.ReadLine());
                if(index != 0 && val != 0)
                {
                    tab[index] = val;
                }
            } while (index !=0 && val != 0);
            WriteTable(tab);
            Console.ReadKey();
        }
 
    }
    class Tablica
    {
        private int[] table;
        private int size;
        private int allocatedSize;
        private int arrayExpandFactor;
        private int defaultValue;

        public int Size
        {
            get
            {
                return size;
            }
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
            set
            {
                AddAt(value, index);
            }
        }
        public Tablica(int size)
        {
            size = 0;
            arrayExpandFactor = 2;
            defaultValue = -1;
            if (size > 4)
            {
                allocatedSize = size * arrayExpandFactor;

            } else
            {
                allocatedSize = 4 * arrayExpandFactor;
            }
            table = new int[allocatedSize];       

        }


        public Tablica():this(0)
        {
        }

        public void Resize() //Zrobic resize do index * 2, zamiast resize'owac wielokrotnie np dla 1000 indexu
        {
            int expandBase = size;
            int newAllocatedSize = expandBase * arrayExpandFactor;
            int[] newTable = new int[newAllocatedSize];
            table.CopyTo(newTable, 0);
            table = newTable;
            allocatedSize = newAllocatedSize;
        }
        public void Add(int value)
        {
            if(size >= allocatedSize)
            {
                Resize();
            }      
            table[size] = value;
            size++;
        }
        public void AddAt(int value, int index)
        {
            if(index == size)
            {
                Add(value);
            } else if (index > size)
            {
                for(int i = size; i < index; i++)
                {
                    Add(defaultValue);
                }
                Add(value);
            } else if(index >=0)
            {
                table[index] = value;
            } else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
