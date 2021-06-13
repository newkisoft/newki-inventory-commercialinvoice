using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using newki_inventory_commercialinvoice.Services;
using newkilibraries;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace newki_inventory_commercialinvoice
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private static string _connectionString;

        static void Main(string[] args)
        {
            //Reading configuration
            var CommercialInvoices = new List<CommercialInvoice>();
            var awsStorageConfig = new AwsStorageConfig();
            var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true);
            var Configuration = builder.Build();
            Configuration.GetSection("AwsStorageConfig").Bind(awsStorageConfig);


            var services = new ServiceCollection();


            var requestQueueName = "CommercialInvoiceRequest";
            var responseQueueName = "CommercialInvoiceResponse";

            _connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_connectionString));
            services.AddTransient<IAwsService, AwsService>();
            services.AddSingleton<IAwsStorageConfig>(awsStorageConfig);

            var serviceProvider = services.BuildServiceProvider();
            InventoryMessage inventoryMessage;

            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "user";
            factory.Password = "password";
            factory.HostName = "localhost";

            var connection = factory.CreateConnection();


            var channel = connection.CreateModel();
            channel.QueueDeclare(requestQueueName, false, false, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updateCustomerFullNameModel = JsonSerializer.Deserialize<InventoryMessage>(content);

                ProcessRequest(updateCustomerFullNameModel);

            }; ;
            channel.BasicConsume(queue: requestQueueName,
                   autoAck: true,
                   consumer: consumer);


            _quitEvent.WaitOne();



        }

        static void ProcessSearch(ICommercialInvoiceService CommercialInvoiceService, ApplicationDbContext appDbContext)
        {
            Console.WriteLine("Loading all the CommercialInvoices...");
            var CommercialInvoices = CommercialInvoiceService.GetCommercialInvoices();

            foreach (var CommercialInvoice in CommercialInvoices)
            {
                if (appDbContext.CommercialInvoiceDataView.Any(p => p.CommercialInvoiceId == CommercialInvoice.CommercialInvoiceId))
                {
                    var existingCommercialInvoice = appDbContext.CommercialInvoiceDataView.Find(CommercialInvoice.CommercialInvoiceId);
                    existingCommercialInvoice.Data = JsonSerializer.Serialize(CommercialInvoice);
                }
                else
                {
                    var CommercialInvoiceDataView = new CommercialInvoiceDataView
                    {
                        CommercialInvoiceId = CommercialInvoice.CommercialInvoiceId,
                        Data = JsonSerializer.Serialize(CommercialInvoice)
                    };
                    appDbContext.CommercialInvoiceDataView.Add(CommercialInvoiceDataView);
                }
                appDbContext.SaveChanges();
            }
        }

        private static void ProcessRequest(InventoryMessage inventoryMessage)
        {

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseNpgsql(_connectionString);

                using (var appDbContext = new ApplicationDbContext(optionsBuilder.Options))
                {
                    var CommercialInvoiceService = new CommercialInvoiceService(appDbContext);


                    var messageType = Enum.Parse<InventoryMessageType>(inventoryMessage.Command);

                    switch (messageType)
                    {
                        case InventoryMessageType.Search:
                            {
                                ProcessSearch(CommercialInvoiceService, appDbContext);
                                break;
                            }
                        case InventoryMessageType.Get:
                            {
                                Console.WriteLine("Loading an CommercialInvoice...");
                                var id = JsonSerializer.Deserialize<int>(inventoryMessage.Message);
                                var CommercialInvoice = CommercialInvoiceService.GetCommercialInvoice(id);
                                var content = JsonSerializer.Serialize(CommercialInvoice);

                                var responseMessageNotification = new InventoryMessage();
                                responseMessageNotification.Command = InventoryMessageType.Get.ToString();
                                responseMessageNotification.RequestNumber = inventoryMessage.RequestNumber;
                                responseMessageNotification.MessageDate = DateTimeOffset.UtcNow;

                                var inventoryResponseMessage = new InventoryMessage();
                                inventoryResponseMessage.Message = content;
                                inventoryResponseMessage.Command = inventoryMessage.Command;
                                inventoryResponseMessage.RequestNumber = inventoryMessage.RequestNumber;

                                Console.WriteLine("Sending the message back");

                                break;

                            }
                        case InventoryMessageType.Insert:
                            {
                                Console.WriteLine("Adding new CommercialInvoice");
                                var CommercialInvoice = JsonSerializer.Deserialize<CommercialInvoice>(inventoryMessage.Message);
                                CommercialInvoice = CommercialInvoiceService.Insert(CommercialInvoice);
                                var newCommercialInvoice = new CommercialInvoiceDataView
                                {
                                    CommercialInvoiceId = CommercialInvoice.CommercialInvoiceId,
                                    Data = JsonSerializer.Serialize(CommercialInvoice)
                                };
                                appDbContext.CommercialInvoiceDataView.Add(newCommercialInvoice);
                                appDbContext.SaveChanges();
                                var status = appDbContext.RequestStatus.FirstOrDefault(p => p.Id == inventoryMessage.RequestNumber);
                                if (status != null)
                                {
                                    status.Status = newCommercialInvoice.CommercialInvoiceId.ToString();
                                    appDbContext.SaveChanges();
                                }
                                break;
                            }
                        case InventoryMessageType.Update:
                            {
                                Console.WriteLine("Updating an CommercialInvoice");
                                var CommercialInvoice = JsonSerializer.Deserialize<CommercialInvoice>(inventoryMessage.Message);
                                CommercialInvoiceService.Update(CommercialInvoice);
                                var existingCommercialInvoice = appDbContext.CommercialInvoiceDataView.Find(CommercialInvoice.CommercialInvoiceId);
                                existingCommercialInvoice.Data = JsonSerializer.Serialize(CommercialInvoice);
                                appDbContext.SaveChanges();
                                var status = appDbContext.RequestStatus.FirstOrDefault(p => p.Id == inventoryMessage.RequestNumber);
                                if (status != null)
                                {
                                    status.Status = existingCommercialInvoice.CommercialInvoiceId.ToString();
                                    appDbContext.SaveChanges();
                                }

                                break;
                            }
                        case InventoryMessageType.Delete:
                            {
                                Console.WriteLine("Deleting an CommercialInvoice");
                                var id = JsonSerializer.Deserialize<int>(inventoryMessage.Message);
                                CommercialInvoiceService.Delete(id);
                                var removeCommercialInvoice = appDbContext.CommercialInvoiceDataView.FirstOrDefault(predicate => predicate.CommercialInvoiceId == id);
                                appDbContext.CommercialInvoiceDataView.Remove(removeCommercialInvoice);
                                appDbContext.SaveChanges();
                                break;
                            }
                        default: break;

                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }
      
    }
}
