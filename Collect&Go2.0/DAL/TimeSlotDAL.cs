using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.Data.SqlClient;

namespace Collect_Go2._0.DAL
{
    public class TimeSlotDAL : ITimeSlotDAL
    {
        private readonly string _connectionString;

        public TimeSlotDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickCollect")!;
        }

        // Créneaux d'un magasin, à partir de demain (règle : pas de réservation le jour même), non complets
        public async Task<List<TimeSlot>> GetAvailableAsync(int storeId)
        {
            List<TimeSlot> slots = new List<TimeSlot>();

            string query = @"SELECT TimeSlotId, StoreId, Date, StartHour, EndHour, ReservationCount
                              FROM TimeSlot
                              WHERE StoreId = @StoreId
                                AND Date > CAST(GETDATE() AS DATE)
                                AND ReservationCount < @MaxReservation
                              ORDER BY Date, StartHour";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StoreId", storeId);
            command.Parameters.AddWithValue("@MaxReservation", TimeSlot.MaxReservation);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                slots.Add(ReadTimeSlot(reader));
            }

            return slots;
        }

        public async Task<TimeSlot?> GetByIdAsync(int timeSlotId)
        {
            string query = @"SELECT TimeSlotId, StoreId, Date, StartHour, EndHour, ReservationCount
                              FROM TimeSlot
                              WHERE TimeSlotId = @TimeSlotId";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TimeSlotId", timeSlotId);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return ReadTimeSlot(reader);
        }

        private static TimeSlot ReadTimeSlot(SqlDataReader reader)
        {
            TimeSlot slot = new TimeSlot(
                Convert.ToInt32(reader["TimeSlotId"]),
                Convert.ToDateTime(reader["Date"]),
                DateTime.Today.Add((TimeSpan)reader["StartHour"]),
                DateTime.Today.Add((TimeSpan)reader["EndHour"]));

            slot.ReservationCount = Convert.ToInt32(reader["ReservationCount"]);

            return slot;
        }
    }
}
