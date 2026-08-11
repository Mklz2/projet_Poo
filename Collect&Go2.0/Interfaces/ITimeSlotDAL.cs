using Collect_Go2._0.Models;

namespace Collect_Go2._0.Interfaces
{
    public interface ITimeSlotDAL
    {
        Task<List<TimeSlot>> GetAvailableAsync(int storeId);

        Task<TimeSlot?> GetByIdAsync(int timeSlotId);
    }
}