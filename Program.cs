using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while(year == 0)
            {
                Console.Write("\nEnter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());

                }catch(Exception e)
                {
                    Console.WriteLine("Try again");
                    Console.WriteLine(e.Message);
                    year = 0;
                }
            }
            while(month == 0)
            {
                Console.Write("\nEnter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if((month < 1) || (month > 12)){
                        Console.WriteLine("try again");
                        month = 0;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("Try again");
                    Console.WriteLine(e.Message);
                    month = 0;
                }
            }
            myStaff = fr.ReadFile();
            for(int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.Write("Enter Hours Worked for {0}", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePayslip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();
        }
        class Staff
            //contains info about staff in the company
        {
            //fields
            private float hourlyRate;
            private int hWorked;

            //auto-implemented properties
            public float TotalPay { get; protected set; }
            public float BasicPay { get; private set; }
            public string NameOfStaff { get; private set; }  
            //properties
            public int HoursWorked
            {
                get
                {
                    return hWorked;
                }
                set
                {
                    if (value > 0)
                        hWorked = value;
                    else
                        hWorked = 0;
                }
            }
            //Constructor
            public Staff(string name, float rate)
            {
                NameOfStaff = name;
                hourlyRate = rate;
            }
            public virtual void CalculatePay()
            {
                Console.WriteLine("Calculating Pay...");
                BasicPay = hWorked * hourlyRate;
                TotalPay = BasicPay;
            }
            public override string ToString()
            {
                return "Staff Name: " + NameOfStaff + "\nBasic Pay: " + BasicPay + "\nTotal Pay: "
                    + TotalPay + "\nHours Worked: " + HoursWorked;
            }

        }

        class Manager: Staff
        {
            private const float ManagerHourlyRate = 50;
            public int Allowance { get; private set; }

            public Manager(string name): base(name, ManagerHourlyRate)
            {

            }
            public override void CalculatePay()
            {
                base.CalculatePay();
                Allowance = 1000;
                if (HoursWorked > 160)
                    TotalPay += Allowance;
                else
                    TotalPay = TotalPay;
            }
            public override string ToString()
            {
                return "Staff Name: " + NameOfStaff + "\nBasic Pay: " + BasicPay + "\nTotal Pay: "
                    + TotalPay + "\nHours Worked: " + HoursWorked;
            }
        }

        class Admin: Staff
        {
            private const float overtimeRate = 15.5f;
            private const float adminHourlyRate = 30;

            public float Overtime { get; private set; }

            public Admin(string name): base(name, adminHourlyRate) { }
            public override void CalculatePay()
            {
                base.CalculatePay();
                if (HoursWorked > 160)
                {
                    Overtime = overtimeRate * (HoursWorked - 160);
                    TotalPay = BasicPay + Overtime;
                }
                else
                    TotalPay = TotalPay;
                    
                    
            }
            public override string ToString()
            {
                return "Staff Name: " + NameOfStaff + "\nBasic Pay: " + BasicPay + "\nTotal Pay: "
                    + TotalPay + "\nHours Worked: " + HoursWorked;
            }
        }

        class FileReader
        {
            public List<Staff> ReadFile()
            {
                List<Staff> myStaff = new List<Staff>();
                string[] result = new string[2];
                string path = "staff.txt";
                string[] sep = { ", " };
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (!sr.EndOfStream)
                        {
                            result = sr.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                            if (result[1] == "Manager")
                                myStaff.Add(new Manager(result[0]));
                            else if (result[1] == "Admin")
                                myStaff.Add(new Admin(result[0]));
                        }
                        sr.Close();

                    }
                }
                else
                {
                    Console.WriteLine("Error, file does not exist");
                }
                return myStaff;
            }
        }

        class PaySlip
        {
            private int month;
            private int year;

            enum MonthsOfYear{
                JAN = 1, FEB =2, MAR=3, APR=4, MAY=5, JUNE=6, JULY=7, AUG=8, SEPT=9,
                OCT=10, NOV=11, DEC=12
            }
            public PaySlip(int payMonth, int payYear)
            {
                month = payMonth;
                year = payYear;
            }

            public void GeneratePayslip(List<Staff> myStaff)
            {
                string path;
                foreach(Staff f in myStaff)
                {
                    path = f.NameOfStaff + ".txt";
                    using(StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine("Payslip for {0}{1}", (MonthsOfYear)month, year);
                        sw.WriteLine("=============================================");
                        sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                        sw.WriteLine("Hours Worked: {0}", f.HoursWorked);
                        sw.WriteLine(" ");
                        sw.WriteLine("Basic Pay: {0:C}", f.BasicPay);
                        if (f.GetType() == typeof(Manager))
                            sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                        else if (f.GetType() == typeof(Admin))
                            sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);

                        sw.WriteLine("");
                        sw.WriteLine("=============================================");
                        sw.WriteLine("Total Pay: {0:C}", f.TotalPay);
                        sw.WriteLine("=============================================");
                        sw.Close();
                    }
                }
            }
            public void GenerateSummary(List<Staff> myStaff)
            {
                var result = from f in myStaff
                             where f.HoursWorked < 10
                             orderby f.NameOfStaff ascending
                             select new { f.NameOfStaff, f.HoursWorked };
                string path = "summary.txt";
                using(StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Staff with less than 10 hours");
                    sw.WriteLine("");
                    foreach(var f  in result)
                    {
                        sw.WriteLine("Name of Staff {0} Hours Worked {1} ", f.NameOfStaff, f.HoursWorked);
                        
                    }
                    sw.Close();
                }
            }
            public override string ToString()
            {
                return "month = " + month + " year = " + year;            }
        }
    }
}
