using System;
using Microsoft.Data.SqlClient;
using System.IO;

namespace ADO.Net
{
    internal class SportShopApp
    {
        static string connectionString = @"Data Source=DESKTOP-GE7UVHJ\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Initial Catalog=SportShop;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. Add a new sale");
                Console.WriteLine("2. Show all sales within a period");
                Console.WriteLine("3. Show the last purchase of a client");
                Console.WriteLine("4. Delete an employee or client");
                Console.WriteLine("5. Show the employee with the highest sales amount");
                Console.WriteLine("6. Exit");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddSale();
                        break;
                    case "2":
                        ShowSalesByPeriod();
                        break;
                    case "3":
                        ShowLastPurchase();
                        break;
                    case "4":
                        DeletePerson();
                        break;
                    case "5":
                        ShowTopSeller();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void AddSale()
        {
            Console.WriteLine("Enter Product ID:");
            int productId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Price:");
            decimal price = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter Quantity:");
            int quantity = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Employee ID:");
            int employeeId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Client ID:");
            int clientId = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Salles(ProductId, Price, Quantity, EmployeeId, ClientId) VALUES (@ProductId, @Price, @Quantity, @EmployeeId, @ClientId)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Sale added.");
            }
        }

        static void ShowSalesByPeriod()
        {
            //Console.WriteLine("Enter start date (YYYY-MM-DD):");
            //string startDate = Console.ReadLine();

            //Console.WriteLine("Enter end date (YYYY-MM-DD):");
            //string endDate = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //string query = @"
                //    SELECT * FROM Salles
                //    WHERE Date BETWEEN @StartDate AND @EndDate";
                string query = @"
                    SELECT * FROM Salles";
                SqlCommand cmd = new SqlCommand(query, conn);
                //cmd.Parameters.AddWithValue("@StartDate", startDate);
                //cmd.Parameters.AddWithValue("@EndDate", endDate);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("Sales during the specified period:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]} - Product: {reader["ProductId"]}, Price: {reader["Price"]}, Quantity: {reader["Quantity"]}, Employee: {reader["EmployeeId"]}, Client: {reader["ClientId"]}");
                }
            }
        }

        static void ShowLastPurchase()
        {
            Console.WriteLine("Enter client's first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter client's last name:");
            string lastName = Console.ReadLine();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 1 s.*
                    FROM Salles s
                    JOIN Clients c ON s.ClientId = c.Id
                    WHERE c.FullName LIKE @FullName
                    ORDER BY s.Date DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", $"{firstName}% {lastName}%");

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine($"Last purchase: Product: {reader["ProductId"]}, Price: {reader["Price"]}, Quantity: {reader["Quantity"]}");
                }
                else
                {
                    Console.WriteLine("Client not found.");
                }
            }
        }

        static void DeletePerson()
        {
            Console.WriteLine("Select type (1 - Employee, 2 - Client):");
            string type = Console.ReadLine();

            Console.WriteLine("Enter ID:");
            int id = int.Parse(Console.ReadLine());

            string table = type == "1" ? "Employees" : "Clients";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM {table} WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"{table} with ID {id} deleted.");
                }
                else
                {
                    Console.WriteLine("ID not found.");
                }
            }
        }

        static void ShowTopSeller()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 1 e.FullName, SUM(s.Price * s.Quantity) AS TotalSales
                    FROM Salles s
                    JOIN Employees e ON s.EmployeeId = e.Id
                    GROUP BY e.FullName
                    ORDER BY TotalSales DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine($"Top seller: {reader["FullName"]}, Total Sales: {reader["TotalSales"]}");
                }
                else
                {
                    Console.WriteLine("No sales data found.");
                }
            }
        }
    }
}
