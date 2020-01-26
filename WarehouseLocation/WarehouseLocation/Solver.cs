using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseLocation
{
    class Problem
    {
        private readonly int n_warehouse;
        private readonly int n_customer;

        private List<Warehouse> Warehouses;
        private List<Customer> Customers;

        private GASolver solver;

        public Problem(string file_name)
        {
            Warehouses = new List<Warehouse>();
            Customers = new List<Customer>();
            
            //
            // Load Data File
            //

            using (StreamReader sr = new StreamReader(file_name))
            {
                string line = sr.ReadLine();
                string[] tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                // n_customer
                // n_warehouse

                n_warehouse = Convert.ToInt32(tokens[0]);
                n_customer = Convert.ToInt32(tokens[1]);

                for (int i = 0; i < n_warehouse; i++)
                {
                    line = sr.ReadLine().Replace(".", ",");
                    tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                    int capacity = Convert.ToInt32(tokens[0]);
                    double setupCost = Convert.ToDouble(tokens[1]);

                    Warehouse warehouse = new Warehouse(i, capacity, setupCost);
                    Warehouses.Add(warehouse);
                }

                for (int i = 0; i < n_customer; i++)
                {
                    Customer customer = new Customer();
                    customer.CustomerID = i;
                    List<Travel> travels = new List<Travel>();

                    line = sr.ReadLine().Trim();
                    customer.Demand = Convert.ToInt32(line);

                    line = sr.ReadLine().Replace(".", ",");
                    tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < tokens.Length; j++)
                    {
                        Travel travel = new Travel()
                        {
                            Customer = customer,
                            Warehouse = Warehouses[j],
                            TravelCost = Convert.ToDouble(tokens[j])
                        };
                        travels.Add(travel);
                    }
                    travels = travels.OrderBy(c => c.TravelCost).ToList();
                    customer.Travels = travels;
                    Customers.Add(customer);
                }


            }

            // CHROMOSOME LENGTH PARAMETER SETTINGS
            if (n_customer > 50)
                WChromosomeSize(20);

            solver = new GASolver(Customers, Warehouses);

            // DEFAULT PARAMETER SETTINGS
            // n_population = 100;
            // n_generation = 25;
            // n_elite = 1;
            // k = 2;

            // pc_w = 0.5;
            // pc_c = 0.8;
            // pm_w = 0.1;
            // pm_c = 0.4;

            // FOR 1000-2000 INSTANCES
            // if (n_customer > 999)
            //    solver.n_population = 10;

            solver.Solve();
            Solution s = solver.BestSolution;
            s.Write();

        }

        // 
        // Warehouse Chromosome Length
        //

        public void WChromosomeSize(int wcs)
        {

            Warehouses = Warehouses.OrderByDescending(w => w.SetupCost).ToList();
            Customers = Customers.OrderByDescending(c => c.Demand).ToList();

            foreach (var warehouse in Warehouses)
                warehouse.Reset();

            foreach (var customer in Customers)
                customer.AddTravel();

            List<Warehouse> WarehousesToSearch = new List<Warehouse>();
            foreach (var warehouse in Warehouses)
            {
                foreach (var w in Warehouses)
                    w.PendingTravels.Clear();

                if (WarehousesToSearch.Count + Warehouses.Count - Warehouses.IndexOf(warehouse) == wcs)
                    WarehousesToSearch.Add(warehouse);
                else if (!warehouse.Close())
                    WarehousesToSearch.Add(warehouse);
            }

            Warehouses = WarehousesToSearch.OrderBy(w => w.SetupCost).ToList();
            Customers = Customers.OrderBy(c => c.CustomerID).ToList();

            foreach (var customer in Customers)
                customer.Travels.RemoveAll(t => !Warehouses.Contains(t.Warehouse));

            GC.Collect();
        }

        public void WriteSolution()
        {
        }
    }


    public class Solver
    {
        static void Main(string[] args)
        {
            string file_name = "";

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CSharp Solver input_file\n");
                Environment.Exit(0);
            }

            else
            {
                file_name = args[0];
                Problem problem = new Problem(file_name);
                problem.WriteSolution();
            }

            Console.ReadKey();
        }
    }
}
