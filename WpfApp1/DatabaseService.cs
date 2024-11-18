using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Npgsql;
using WpfApp1;

public class DatabaseService
{
    private readonly string _connectionString =
        "Host=povt-cluster.tstu.tver.ru;" +
        "Port=5432;" +
        "Database=cleaning;" +
        "Username=mpi;" +
        "Password=135a1;";

    public List<Client> GetAllClients()
    {
        var clients = new List<Client>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using var command = new NpgsqlCommand("SELECT * FROM Клиенты", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                clients.Add(new Client
                {
                    Id = Convert.ToInt32(reader["id_клиента"]),
                    FullName = reader["ФИО"].ToString()
                });
            }
        }
        return clients;
    }

    public List<Service> GetAllServices()
    {
        var services = new List<Service>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using var command = new NpgsqlCommand("SELECT * FROM Услуги", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                services.Add(new Service
                {
                    Id = Convert.ToInt32(reader["id_услуги"]),
                    Type = reader["вид_услуги"].ToString(),
                    BasePrice = Convert.ToDecimal(reader["базовая_стоимость"])
                });
            }
        }
        return services;
    }

    public List<Order> GetAllOrders()
    {
        var orders = new List<Order>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using var command = new NpgsqlCommand("SELECT * FROM Заказы", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(reader["id_заказа"]),
                    ClientId = Convert.ToInt32(reader["id_клиента"]),
                    ServiceId = Convert.ToInt32(reader["id_услуги"]),
                    ItemDescription = reader["описание_изделия"].ToString(),
                    OrderDate = Convert.ToDateTime(reader["дата_приема"]),
                    ServiceCost = Convert.ToDecimal(reader["стоимость_услуги"]),
                    Status = reader["статус_заказа"].ToString(),
                    CompletionDate = reader["дата_выполнения"] != DBNull.Value ? Convert.ToDateTime(reader["дата_выполнения"]) : (DateTime?)null
                });
            }
        }
        return orders;
    }

    public List<Receipt> GetAllReceipts()
    {
        var receipts = new List<Receipt>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using var command = new NpgsqlCommand("SELECT * FROM Квитанции", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                receipts.Add(new Receipt
                {
                    Id = Convert.ToInt32(reader["id_квитанции"]),
                    OrderId = Convert.ToInt32(reader["id_заказа"]),
                    PrintDate = Convert.ToDateTime(reader["дата_печати"])
                });
            }
        }
        return receipts;
    }

    // Добавим методы сохранения данных
    public void SaveClients(ObservableCollection<Client> clients)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Получаем текущий список идентификаторов клиентов из базы данных
            var existingClientIds = new List<int>();
            using (var command = new NpgsqlCommand("SELECT id_клиента FROM Клиенты", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    existingClientIds.Add(Convert.ToInt32(reader["id_клиента"]));
                }
            }

            // Определяем удаленные записи
            var clientIds = clients.Select(c => c.Id).ToList();
            var deletedClientIds = existingClientIds.Except(clientIds);

            // Удаляем записи
            foreach (var id in deletedClientIds)
            {
                using var deleteCommand = new NpgsqlCommand("DELETE FROM Клиенты WHERE id_клиента = @Id", connection);
                deleteCommand.Parameters.AddWithValue("Id", id);
                deleteCommand.ExecuteNonQuery();
            }

            // Обновляем или добавляем записи
            foreach (var client in clients)
            {
                if (client.Id == 0)
                {
                    // Вставка нового клиента
                    using var insertCommand = new NpgsqlCommand(
                        "INSERT INTO Клиенты (ФИО) VALUES (@FullName) RETURNING id_клиента", connection);
                    insertCommand.Parameters.AddWithValue("FullName", client.FullName ?? (object)DBNull.Value);
                    var id = Convert.ToInt32(insertCommand.ExecuteScalar());
                    client.Id = id;
                }
                else
                {
                    // Обновление существующего клиента
                    using var updateCommand = new NpgsqlCommand(
                        "UPDATE Клиенты SET ФИО = @FullName WHERE id_клиента = @Id", connection);
                    updateCommand.Parameters.AddWithValue("Id", client.Id);
                    updateCommand.Parameters.AddWithValue("FullName", client.FullName ?? (object)DBNull.Value);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }

    public void SaveServices(ObservableCollection<Service> services)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Получаем текущий список идентификаторов услуг из базы данных
            var existingServiceIds = new List<int>();
            using (var command = new NpgsqlCommand("SELECT id_услуги FROM Услуги", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    existingServiceIds.Add(Convert.ToInt32(reader["id_услуги"]));
                }
            }

            // Определяем удаленные записи
            var serviceIds = services.Select(s => s.Id).ToList();
            var deletedServiceIds = existingServiceIds.Except(serviceIds);

            // Удаляем записи
            foreach (var id in deletedServiceIds)
            {
                using var deleteCommand = new NpgsqlCommand("DELETE FROM Услуги WHERE id_услуги = @Id", connection);
                deleteCommand.Parameters.AddWithValue("Id", id);
                deleteCommand.ExecuteNonQuery();
            }

            // Обновляем или добавляем записи
            foreach (var service in services)
            {
                if (service.Id == 0)
                {
                    // Вставка новой услуги
                    using var insertCommand = new NpgsqlCommand(
                        "INSERT INTO Услуги (вид_услуги, базовая_стоимость) VALUES (@Type, @BasePrice) RETURNING id_услуги", connection);
                    insertCommand.Parameters.AddWithValue("Type", service.Type ?? (object)DBNull.Value);
                    insertCommand.Parameters.AddWithValue("BasePrice", service.BasePrice);
                    var id = Convert.ToInt32(insertCommand.ExecuteScalar());
                    service.Id = id;
                }
                else
                {
                    // Обновление существующей услуги
                    using var updateCommand = new NpgsqlCommand(
                        "UPDATE Услуги SET вид_услуги = @Type, базовая_стоимость = @BasePrice WHERE id_услуги = @Id", connection);
                    updateCommand.Parameters.AddWithValue("Id", service.Id);
                    updateCommand.Parameters.AddWithValue("Type", service.Type ?? (object)DBNull.Value);
                    updateCommand.Parameters.AddWithValue("BasePrice", service.BasePrice);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }

    public void SaveOrders(ObservableCollection<Order> orders)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Получаем текущий список идентификаторов заказов из базы данных
            var existingOrderIds = new List<int>();
            using (var command = new NpgsqlCommand("SELECT id_заказа FROM Заказы", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    existingOrderIds.Add(Convert.ToInt32(reader["id_заказа"]));
                }
            }

            // Определяем удаленные записи
            var orderIds = orders.Select(o => o.Id).ToList();
            var deletedOrderIds = existingOrderIds.Except(orderIds);

            // Удаляем записи
            foreach (var id in deletedOrderIds)
            {
                using var deleteCommand = new NpgsqlCommand("DELETE FROM Заказы WHERE id_заказа = @Id", connection);
                deleteCommand.Parameters.AddWithValue("Id", id);
                deleteCommand.ExecuteNonQuery();
            }

            // Обновляем или добавляем записи
            foreach (var order in orders)
            {
                if (order.Id == 0)
                {
                    // Вставка нового заказа
                    using var insertCommand = new NpgsqlCommand(
                        @"INSERT INTO Заказы (id_клиента, id_услуги, описание_изделия, дата_приема, стоимость_услуги, статус_заказа, дата_выполнения) 
                          VALUES (@ClientId, @ServiceId, @ItemDescription, @OrderDate, @ServiceCost, @Status, @CompletionDate) 
                          RETURNING id_заказа", connection);

                    insertCommand.Parameters.AddWithValue("ClientId", order.ClientId);
                    insertCommand.Parameters.AddWithValue("ServiceId", order.ServiceId);
                    insertCommand.Parameters.AddWithValue("ItemDescription", order.ItemDescription ?? (object)DBNull.Value);
                    insertCommand.Parameters.AddWithValue("OrderDate", order.OrderDate);
                    insertCommand.Parameters.AddWithValue("ServiceCost", order.ServiceCost);
                    insertCommand.Parameters.AddWithValue("Status", order.Status ?? (object)DBNull.Value);
                    insertCommand.Parameters.AddWithValue("CompletionDate", order.CompletionDate.HasValue ? (object)order.CompletionDate.Value : DBNull.Value);

                    var id = Convert.ToInt32(insertCommand.ExecuteScalar());
                    order.Id = id;
                }
                else
                {
                    // Обновление существующего заказа
                    using var updateCommand = new NpgsqlCommand(
                        @"UPDATE Заказы 
                          SET id_клиента = @ClientId, 
                              id_услуги = @ServiceId, 
                              описание_изделия = @ItemDescription, 
                              дата_приема = @OrderDate, 
                              стоимость_услуги = @ServiceCost, 
                              статус_заказа = @Status, 
                              дата_выполнения = @CompletionDate 
                          WHERE id_заказа = @Id", connection);

                    updateCommand.Parameters.AddWithValue("Id", order.Id);
                    updateCommand.Parameters.AddWithValue("ClientId", order.ClientId);
                    updateCommand.Parameters.AddWithValue("ServiceId", order.ServiceId);
                    updateCommand.Parameters.AddWithValue("ItemDescription", order.ItemDescription ?? (object)DBNull.Value);
                    updateCommand.Parameters.AddWithValue("OrderDate", order.OrderDate);
                    updateCommand.Parameters.AddWithValue("ServiceCost", order.ServiceCost);
                    updateCommand.Parameters.AddWithValue("Status", order.Status ?? (object)DBNull.Value);
                    updateCommand.Parameters.AddWithValue("CompletionDate", order.CompletionDate.HasValue ? (object)order.CompletionDate.Value : DBNull.Value);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }

    public void SaveReceipts(ObservableCollection<Receipt> receipts)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            // Получаем текущий список идентификаторов квитанций из базы данных
            var existingReceiptIds = new List<int>();
            using (var command = new NpgsqlCommand("SELECT id_квитанции FROM Квитанции", connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    existingReceiptIds.Add(Convert.ToInt32(reader["id_квитанции"]));
                }
            }

            // Определяем удаленные записи
            var receiptIds = receipts.Select(r => r.Id).ToList();
            var deletedReceiptIds = existingReceiptIds.Except(receiptIds);

            // Удаляем записи
            foreach (var id in deletedReceiptIds)
            {
                using var deleteCommand = new NpgsqlCommand("DELETE FROM Квитанции WHERE id_квитанции = @Id", connection);
                deleteCommand.Parameters.AddWithValue("Id", id);
                deleteCommand.ExecuteNonQuery();
            }

            // Обновляем или добавляем записи
            foreach (var receipt in receipts)
            {
                if (receipt.Id == 0)
                {
                    // Вставка новой квитанции
                    using var insertCommand = new NpgsqlCommand(
                        "INSERT INTO Квитанции (id_заказа, дата_печати) VALUES (@OrderId, @PrintDate) RETURNING id_квитанции", connection);
                    insertCommand.Parameters.AddWithValue("OrderId", receipt.OrderId);
                    insertCommand.Parameters.AddWithValue("PrintDate", receipt.PrintDate);

                    var id = Convert.ToInt32(insertCommand.ExecuteScalar());
                    receipt.Id = id;
                }
                else
                {
                    // Обновление существующей квитанции
                    using var updateCommand = new NpgsqlCommand(
                        "UPDATE Квитанции SET id_заказа = @OrderId, дата_печати = @PrintDate WHERE id_квитанции = @Id", connection);
                    updateCommand.Parameters.AddWithValue("Id", receipt.Id);
                    updateCommand.Parameters.AddWithValue("OrderId", receipt.OrderId);
                    updateCommand.Parameters.AddWithValue("PrintDate", receipt.PrintDate);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }

    // Удаляем методы AddClient, UpdateClient и аналогичные, так как они теперь не нужны отдельно
}
