using System;
using System.Data;
using System.Data.SqlClient;

namespace Traveo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Choose the task:\n 1.Task1 \n 2.Task2");
            string taskNo = Console.ReadLine();
            Helper helper = new Helper();
            if (taskNo == "1")
            {
                helper.GetStudents();
               
            }
            else
            {
                helper.Tracker();
            }

        }
    }
}

