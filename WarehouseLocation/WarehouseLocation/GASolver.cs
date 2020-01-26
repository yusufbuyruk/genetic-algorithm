using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseLocation
{
    //
    // Genetic Algorithm Solver Solution Component
    //

    class Solution
    {
        public List<Customer> Customers { get; set; }
        public List<Warehouse> Warehouses { get; set; }

        public List<int> CChromosome { get; set; }
        public List<bool> WChromosome { get; set; }

        public double Fitness { get; set; }
        public int RepairIndex { get; set; }

        private GASolver solver;

        public Solution(GASolver s, List<int> c_chromosome, List<bool> w_chromosome)
        {
            Warehouses = s.Warehouses;
            Customers = s.Customers;

            CChromosome = c_chromosome;
            WChromosome = w_chromosome;

            RepairIndex = 0;
            solver = s;
        }

        public void Write()
        {
            WriteFitness();

            List<int> solutionOut = new List<int>();
            foreach (var customer in Customers)
                solutionOut.Add(customer.CurrentTravel.Warehouse.WarehouseID);
            Console.WriteLine(String.Join(" ", solutionOut));

        }

        public void WriteFitness()
        {
            solver.CalculateFitness(this);
            Console.WriteLine(Fitness.ToString().Replace(",", "."));
        }
    }


    /// <summary>
    ///  Genetic Algorithm Solver Class
    /// </summary>

    class GASolver
    {
        public int n_population { get; set; }           // population size
        public int n_elite { get; set; }                // elite size
        public int k { get; set; }                      // tournament size
        public int n_generation { get; set; }           // last n generation without improvement STOPPING CRITERIA

        public int n_customer { get; set; }
        public int n_warehouse { get; set; }

        public double pc_w { get; set; }                // WChromosome crossover probability
        public double pc_c { get; set; }                // CChromosome crossover probability

        public double pm_w { get; set; }                // WChromosome mutation probability
        public double pm_c { get; set; }                // CChromosome mutation probability;

        private Random random;
        private int seed;

        public List<Customer> Customers { get; set; }
        public List<Warehouse> Warehouses { get; set; }

        private List<Solution> population { get; set; }
        private List<Solution> nextGeneration { get; set; }

        public Solution BestSolution
        {
            get
            {
                List<Solution> solutionList = population.OrderBy(p => p.Fitness).ToList();
                return solutionList[0];
            }

        }

        public GASolver(List<Customer> c, List<Warehouse> w)
        {
            n_population = 100;
            n_generation = 40;
            n_elite = 1;
            k = 2;

            pc_w = 0.5;
            pc_c = 0.8;
            pm_w = 0.1;
            pm_c = 0.4;


            seed = 0;
            random = new Random(seed);

            Customers = c;
            Warehouses = w;

            n_customer = c.Count;
            n_warehouse = w.Count;

            population = new List<Solution>();
            nextGeneration = new List<Solution>();

            if (n_customer > 999)
                n_population = 10;
        }

        public void Solve()
        {
            InitializePopulation();
            for (int i = 0; i < n_generation; i++)  // n_generation elapsed is to be implemented STOPPING CRITERIA
                ProduceGeneration();

        }

        public void InitializePopulation()
        {
            do
            {
                List<bool> w_chromosome = new List<bool>();
                List<int> c_chromosome = new List<int>();

                for (int c = 0; c < n_customer; c++)
                    c_chromosome.Add(c);
                c_chromosome = c_chromosome.OrderBy(item => random.Next()).ToList();

                w_chromosome = Enumerable.Repeat(true, n_warehouse).ToList();

                Solution solution = new Solution(this, c_chromosome, w_chromosome);

                if (CalculateFitness(solution))
                    population.Add(solution);

            } while (population.Count < n_population);

            GC.Collect();
        }

        public void ProduceGeneration()
        {
            int f1, f2, m1, m2, f, m;
            nextGeneration.Clear();
            nextGeneration.AddRange(population.OrderBy(item => item.Fitness).Take(n_elite).ToList());

            do
            {
                // START TOURNAMENT
                f1 = random.Next(0, n_population);
                f2 = random.Next(0, n_population);
                m1 = random.Next(0, n_population);
                m2 = random.Next(0, n_population);

                f = (population[f1].Fitness < population[f2].Fitness) ? f1 : f2;
                m = (population[m1].Fitness < population[m2].Fitness) ? m1 : m2;


                // BREED OFFSPRING
                Solution offspring = OffSpring(population[f], population[m]);

                //while (!CalculateFitness(offspring))
                //    Repair(offspring);

                //nextGeneration.Add(offspring);

                if (CalculateFitness(offspring))
                    nextGeneration.Add(offspring);


            } while (nextGeneration.Count < n_population);

            population = nextGeneration.ToList();

            BestSolution.WriteFitness();

        }

        public bool CalculateFitness(Solution s)
        {
            foreach (var warehouse in Warehouses)
                warehouse.Reset();

            for (int i = 0; i < n_warehouse; i++)
                Warehouses[i].Open = s.WChromosome[i];

            for (int i = 0; i < n_customer; i++)
                if (!Customers[s.CChromosome[i]].AddTravel())
                {
                    s.RepairIndex = i;
                    return false;
                }


            double cost = 0;
            foreach (var warehouse in Warehouses)
                cost += warehouse.Cost;
            s.Fitness = cost;
            return true;
        }


        // Repair Function

        public void Repair(Solution s)
        {
            int item = s.CChromosome[s.RepairIndex];
            s.CChromosome.Remove(item);
            s.CChromosome.Insert(0, item);

            int index = s.WChromosome.IndexOf(false);
            if (index > 0)
                s.WChromosome[index] = true;
        }

        public Solution OffSpring(Solution p1, Solution p2)
        {
            List<int> cc = (random.Next() < 0.5) ? p1.CChromosome.ToList() : p2.CChromosome.ToList();
            List<bool> wc = (random.Next() < 0.5) ? p1.WChromosome.ToList() : p2.WChromosome.ToList();

            Solution offspring = new Solution(this, cc, wc);

            // RECOMBINE CUSTOMER CHROMOSOME
            if (pc_c < random.Next())
                offspring.CChromosome = CCrossOver(p1, p2);

            // MUTATE CUSTOMER CHROMOSOME
            if (pm_c < random.Next())
                CMutate(offspring);

            // RECOMBINE WAREHOUSE CHROMOSOME
            if (pc_w < random.Next())
                offspring.WChromosome = WCrossOver(p1, p2);

            // MUTATE WAREHOUSE CHROMOSOME
            if (pm_w < random.Next())
                WMutate(offspring);

            return offspring;
        }

        public List<int> CCrossOver(Solution p1, Solution p2)
        {
            // CUSTOMER ORDER CROSSOVER
            List<int> cchromosome = new List<int>();

            for (int i = 0; i < n_customer; i++)
            {
                foreach (var item in (i % 2 == 0) ? p1.CChromosome : p2.CChromosome)
                    if (cchromosome.IndexOf(item) == -1)
                    {
                        cchromosome.Add(item);
                        break;
                    }
            }

            return cchromosome;
        }

        public void CMutate(Solution s)
        {
            // CUSTOMER ORDER MUTATION
            int r = random.Next(0, n_customer);
            s.CChromosome.Remove(r);
            s.CChromosome.Insert(0, r);
        }

        public List<bool> WCrossOver(Solution p1, Solution p2)
        {
            // ONE POINT CROSSOVER
            List<bool> wchromosome = new List<bool>();
            int t = random.Next(1, n_warehouse);

            wchromosome.AddRange(p1.WChromosome.GetRange(0, t));
            wchromosome.AddRange(p2.WChromosome.GetRange(t, n_warehouse - t));

            return wchromosome;
        }

        public void WMutate(Solution s)
        {
            // BITFLIP MUTATION
            int w = random.Next(0, n_warehouse);
            s.WChromosome[w] = (s.WChromosome[w]) ? false : true;

            //// SWAP MUTATION
            //int t1 = random.Next(0, n_warehouse);
            //int t2 = random.Next(0, n_warehouse);

            //bool w1 = s.WChromosome[t1];
            //bool w2 = s.WChromosome[t2];

            //s.WChromosome[t1] = w2;
            //s.WChromosome[t2] = w1;
        }
    }
}
