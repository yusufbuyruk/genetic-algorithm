using System.Collections.Generic;

namespace WarehouseLocation
{
    // 
    // BASIC CLASS DEFINITIONS
    //



    // 
    // Travel class means the service from Warehouse to Customer
    //

    class Travel
    {
        public Customer Customer;
        public Warehouse Warehouse;
        public double TravelCost;

        public Travel()
        {
        }

        //
        // Check if warehouse can serve customer
        //

        public bool CanTravel
        {
            get
            {
                if (!Warehouse.Open)
                    return false;

                if (Warehouse.CapacityLeft < Customer.Demand)
                    return false;

                return true;

            }
        }

        //
        // use it after Check CanTravel()
        //

        public void AddTravel()
        {
            Warehouse.Travels.Add(this);
        }


        //
        // This pending section is written only for hybridization for warehouse chromosome length on initial parameter settings
        //

        public bool CanRequestPending
        {
            get
            {
                if (!Warehouse.Open)
                    return false;

                if (Warehouse.CapacityLeft - Warehouse.PendingDemand < Customer.Demand)
                    return false;

                return true;
            }

        }

        public void RequestPending()
        {
            Warehouse.PendingTravels.Add(this);
        }


    }

    class Customer
    {
        public int Demand { get; set; }
        public int CustomerID { get; set; }

        public Travel CurrentTravel { get; set; }
        public Travel PendingTravel { get; set; }

        public List<Travel> Travels { get; set; }

        public Customer()
        {
        }

        //
        // To make best possible choice
        //

        public bool AddTravel()
        {
            foreach (var travel in Travels)
            {
                if (travel.CanTravel)
                {
                    travel.AddTravel();
                    CurrentTravel = travel;
                    return true;
                }
            }
            return false;
        }

        //
        // This pending section is written only for hybridization for warehouse chromosome length on initial parameter settings
        //

        public bool RequestPending()
        {
            int index = Travels.IndexOf(CurrentTravel) + 1;
            int count = Travels.Count;

            foreach (var travel in Travels.GetRange(index, count - index))
            {
                if (travel.CanRequestPending)
                {
                    travel.RequestPending();
                    PendingTravel = travel;
                    return true;
                }
            }
            return false;
        }
    }

    class Warehouse
    {
        public bool Open { get; set; }
        public int WarehouseID { get; set; }
        public int Capacity { get; set; }
        public int FirstChoice { get; set; }

        //
        // Capacity control
        // 

        public int CapacityLeft
        {
            get
            {
                int demand = 0;
                foreach (var travel in Travels)
                    demand += travel.Customer.Demand;
                return Capacity - demand;
            }
        }


        //
        // This pending section is written only for hybridization for warehouse chromosome length on initial parameter settings
        //

        public int PendingDemand
        {
            get
            {
                int demand = 0;
                foreach (var travel in PendingTravels)
                    demand += travel.Customer.Demand;
                return demand;
            }
        }


        //
        // SetupCost
        //

        public double SetupCost { get; set; }

        public double Cost
        {
            get
            {
                double cost = 0;
                if (Open)
                {
                    cost += SetupCost;
                    foreach (var travel in Travels)
                        cost += travel.TravelCost;
                }
                return cost;
            }
        }

        public List<Travel> Travels;
        public List<Travel> PendingTravels;

        public Warehouse(int warehouseID, int capacity, double setupCost)
        {
            WarehouseID = warehouseID;
            Capacity = capacity;
            SetupCost = setupCost;

            Travels = new List<Travel>();
            PendingTravels = new List<Travel>();
            Open = true;
        }

        public void Reset()
        {
            Travels.Clear();
        }


        //
        // This pending and close section is written only for hybridization for warehouse chromosome length on initial parameter settings
        //

        public void ResetPendings()
        {
            PendingTravels.Clear();
        }

        public bool RequestPending()
        {
            foreach (var travel in Travels)
                if (!travel.Customer.RequestPending())
                    return false;
            return true;
        }


        public double CloseCost()
        {
            double cost = 0;
            foreach (var travel in Travels)
            {
                double nextCost = travel.Customer.PendingTravel.TravelCost;
                double currentCost = travel.TravelCost;

                cost += nextCost - currentCost;
            }
            return cost;
        }

        public bool Close()
        {

            if (!Open)
                return false;

            if (!RequestPending())
                return false;

            if (CloseCost() > SetupCost)
                return false;

            List<Customer> customersToMigrate = new List<Customer>();
            foreach (var travel in Travels)
                customersToMigrate.Add(travel.Customer);

            foreach (var customer in customersToMigrate)
            {
                customer.PendingTravel.AddTravel();
                customer.CurrentTravel = customer.PendingTravel;
            }

            Open = false;
            return true;

        }
    }

}
