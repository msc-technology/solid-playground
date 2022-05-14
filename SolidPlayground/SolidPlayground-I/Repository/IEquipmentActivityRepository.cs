using Core.DTO;

namespace SolidPlayground_I.Repository
{
    public interface IEquipmentActivityRepository
    {
        Task<bool> BookingExists(string key);
        Task Store(EquipmentActivity equipmentActivity);
    }
}
