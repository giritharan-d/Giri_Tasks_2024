using Quartz;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;

namespace Cron_Job
{
    public class Job : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public Job(IServiceProvider _serviceProvider)
        {
            this._serviceProvider = _serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(DateTime.Now);

            var path = @"C:\Users\Mitrah\Downloads\Employee.xlsx";
            var data = ExcelUtility.ExcelDataToDataTable(path, "Sheet1");


            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();

            string con = configuration.GetConnectionString("SQLConnection");

            try
            {
                //Creating the connection object
                using (SqlConnection connection = new SqlConnection(con))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                    {
                        sqlBulkCopy.DestinationTableName = "AllUsers";

                        connection.Open();

                        sqlBulkCopy.WriteToServer(data);

                        string sp_name = "Copy_Data";

                        SqlCommand sp = new(sp_name, connection);
                        sp.CommandType = CommandType.StoredProcedure;
                        sp.CommandText = sp_name;
                        var status = sp.ExecuteScalar();

                        Console.WriteLine("Status of the Job : " + status);

                        SqlCommand cmd = new("Truncate Table Allusers", connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error Message \n" + error);
            }
        }
    }
}
