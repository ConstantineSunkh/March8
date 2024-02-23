using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace March8
{
    class Coordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
    class Order
    {
        public Order()
        {
            Сompleted = false;
            CourierIndex = -1;
            DistanceFromPointAToPointB = 0.0f;
            LeadTime = 0;
        }
        public Coordinates PointA { get; set; }
        public Coordinates PointB { get; set; }
        public int Price { get; set; }
        public bool Сompleted { get; set; }
        public int CourierIndex { get; set; }
        public float DistanceFromPointAToPointB { get; set; }
        public int LeadTime { get; set; }
    }
    class Courier
    {
        public Courier()
        {
            OrderIndex = -1;
            CompletedOrders = 0;
            Earned = 0;
            DistanceTraveled = 0.0f;
            TimeAllTheWay = 0;
        }
        public Coordinates InitialCoordinates { get; set; }
        public Coordinates CurrentCoordinates { get; set; }
        public int OrderIndex { get; set; }
        public int CompletedOrders { get; set; }
        public int Earned { get; set; }
        public float DistanceTraveled { get; set; }
        public int TimeAllTheWay { get; set; }
    }
    class Program
    {
        static List<int> getCouriersIndexesWithMinimalTimeAllTheWay(List<Courier> courierList)
        {
            List<int> couriersIndexes = new List<int>();

            int minimalTimeAllTheWay = courierList[0].TimeAllTheWay;
            for (int i = 0; i < courierList.Count; i++)
            {
                if (courierList[i].TimeAllTheWay < minimalTimeAllTheWay)
                {
                    minimalTimeAllTheWay = courierList[i].TimeAllTheWay;
                }
            }
            for (int i = 0; i < courierList.Count; i++)
            {
                if (courierList[i].TimeAllTheWay <= minimalTimeAllTheWay)
                {
                    couriersIndexes.Add(i);
                }
            }
            return couriersIndexes;
        }
        static float getHypotenuseLength(Coordinates A, Coordinates B)
        {
            float hypotenuseLength = (float)Math.Sqrt(Math.Pow(A.Latitude - B.Latitude, 2) + Math.Pow(A.Longitude - B.Longitude, 2));
            return hypotenuseLength;
        }
        static void Main(string[] args)
        {
            Random rand = new Random();

            List<Courier> courierList = new List<Courier>();
            int couriersNumber = rand.Next(1, 100);
            for (int i = 0; i < couriersNumber; i++)
            {
                Courier courier = new Courier();

                Coordinates currentCoordinates = new Coordinates();

                currentCoordinates.Latitude = rand.Next(400) / 10000.0f + 62.01f;
                currentCoordinates.Longitude = rand.Next(400) / 10000.0f + 129.695f;

                courier.CurrentCoordinates = currentCoordinates;

                Coordinates initialCoordinates = new Coordinates();

                initialCoordinates.Latitude = currentCoordinates.Latitude;
                initialCoordinates.Longitude = currentCoordinates.Longitude;

                courier.InitialCoordinates = currentCoordinates;

                courierList.Add(courier);
            }

            List<Order> orderList = new List<Order>();
            int ordersNumber = rand.Next(1, 500);
            for (int i = 0; i < ordersNumber; i++)
            {
                Order order = new Order();

                Coordinates pointACoordinates = new Coordinates();

                pointACoordinates.Latitude = rand.Next(400) / 10000.0f + 62.01f;
                pointACoordinates.Longitude = rand.Next(400) / 10000.0f + 129.695f;

                order.PointA = pointACoordinates;

                Coordinates pointBCoordinates = new Coordinates();

                pointBCoordinates.Latitude = rand.Next(400) / 10000.0f + 62.01f;
                pointBCoordinates.Longitude = rand.Next(400) / 10000.0f + 129.695f;

                order.PointB = pointBCoordinates;

                float distance = getHypotenuseLength(order.PointA, order.PointB);
                float maxDistance = (float)Math.Sqrt(Math.Pow(0.04f, 2) + Math.Pow(0.04f, 2));
                int maxPrice = 700;
                int price = (maxPrice * (int)(distance * 1000.0f)) / (int)(maxDistance * 1000.0f);
                if (price < 250)
                {
                    price = 250;
                }
                order.Price = price;

                orderList.Add(order);
            }

            if (courierList.Count != 0 && orderList.Count != 0)
            {
                int courierSpeed = 25; // km/h
                bool allOredersCompleted = false;
                while (!allOredersCompleted)
                {
                    List<int> couriersIndexesWithMinimalTimeAllTheWay = getCouriersIndexesWithMinimalTimeAllTheWay(courierList);

                    //searching for the nearest available courier for each order
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].Сompleted == true)
                        {
                            continue;
                        }
                        int nearestAvailableCourierIndex = -1;
                        foreach (int index in couriersIndexesWithMinimalTimeAllTheWay)
                        {
                            if (nearestAvailableCourierIndex < 0)
                            {
                                nearestAvailableCourierIndex = index;
                                continue;
                            }
                            float courierDistanceToPointA = getHypotenuseLength(orderList[i].PointA, courierList[index].CurrentCoordinates);
                            float nearestCourierDistanceToPointA = getHypotenuseLength(orderList[i].PointA, courierList[nearestAvailableCourierIndex].CurrentCoordinates);
                            if (courierDistanceToPointA < nearestCourierDistanceToPointA)
                            {
                                nearestAvailableCourierIndex = index;
                            }
                        }
                        if (nearestAvailableCourierIndex < 0)
                        {
                            return;
                        }
                        orderList[i].CourierIndex = nearestAvailableCourierIndex;
                    }

                    foreach (int index in couriersIndexesWithMinimalTimeAllTheWay)
                    {
                        // searching for the furthest order for each courier who already has potential orders assigned to them
                        for (int i = 0; i < orderList.Count; i++)
                        {
                            if (orderList[i].CourierIndex == index && orderList[i].Сompleted == false)
                            {
                                if (courierList[index].OrderIndex < 0)
                                {
                                    courierList[index].OrderIndex = i;
                                }
                                float courierDistanceToCurrentOrderPointA = getHypotenuseLength(orderList[i].PointA, courierList[index].CurrentCoordinates);
                                float courierDistanceToHighestPriorityOrderPointA = getHypotenuseLength(orderList[courierList[index].OrderIndex].PointA, courierList[index].CurrentCoordinates);
                                if (courierDistanceToHighestPriorityOrderPointA < courierDistanceToCurrentOrderPointA)
                                {
                                    orderList[courierList[index].OrderIndex].CourierIndex = -1;
                                    courierList[index].OrderIndex = i;
                                }
                            }
                        }
                        // assigning an order to a courier
                        if (courierList[index].OrderIndex >= 0 && orderList[courierList[index].OrderIndex].Сompleted == false)
                        {
                            // 111.134861111 km in one degree
                            float distanceFromCourierCoordsToPointA = 111.134861111f * getHypotenuseLength(orderList[courierList[index].OrderIndex].PointA, courierList[index].CurrentCoordinates);
                            float distanceFromPointAToPointB = 111.134861111f * getHypotenuseLength(orderList[courierList[index].OrderIndex].PointA, orderList[courierList[index].OrderIndex].PointB);
                            // total time spent on delivery(minutes): waiting for the order to be accepted by the courier + the courier’s arrival at point A + delivery from point A to point B
                            int deliveryTime = (int)Math.Ceiling((double)((distanceFromCourierCoordsToPointA + distanceFromPointAToPointB) * 60.0f) / (double)courierSpeed);
                            courierList[index].TimeAllTheWay += deliveryTime;
                            courierList[index].DistanceTraveled += distanceFromCourierCoordsToPointA + distanceFromPointAToPointB;
                            courierList[index].CurrentCoordinates = orderList[courierList[index].OrderIndex].PointB;
                            courierList[index].CompletedOrders += 1;
                            courierList[index].Earned += orderList[courierList[index].OrderIndex].Price;
                            orderList[courierList[index].OrderIndex].DistanceFromPointAToPointB = distanceFromPointAToPointB;
                            orderList[courierList[index].OrderIndex].LeadTime = courierList[index].TimeAllTheWay;
                            orderList[courierList[index].OrderIndex].Сompleted = true;
                            courierList[index].OrderIndex = -1;
                        }
                    }

                    allOredersCompleted = true;
                    for (int i = 0; i < orderList.Count; i++)
                    {
                        if (orderList[i].Сompleted == false)
                        {
                            allOredersCompleted = false;
                            break;
                        }
                    }
                }

                for (int i = 0; i < courierList.Count; i++)
                {
                    Console.WriteLine("Courier " + i + ": Initial Coordinates - (" + courierList[i].InitialCoordinates.Latitude + ", " + courierList[i].InitialCoordinates.Longitude
                         + "), Current Coordinates - (" + courierList[i].CurrentCoordinates.Latitude + ", " + courierList[i].CurrentCoordinates.Longitude
                         + "), Completed Orders - " + courierList[i].CompletedOrders
                         + ", Earned - " + courierList[i].Earned
                         + ", Distance Traveled(km) - " + courierList[i].DistanceTraveled
                         + ", Time All The Way(min) - " + courierList[i].TimeAllTheWay);
                }
                Console.WriteLine();
                for (int i = 0; i < orderList.Count; i++)
                {
                    Console.WriteLine("Order " + i + ": Point A - (" + orderList[i].PointA.Latitude + ", " + orderList[i].PointA.Longitude
                        + "), Point B - (" + orderList[i].PointB.Latitude + ", " + orderList[i].PointB.Longitude
                        + "), Price - " + orderList[i].Price
                        + ", Courier Index - " + orderList[i].CourierIndex
                        + ", Distance From Point A To Point B(km) - " + orderList[i].DistanceFromPointAToPointB
                        + ", Lead Time(min) - " + orderList[i].LeadTime);
                }
            }
            Console.ReadKey();
        }
    }
}
