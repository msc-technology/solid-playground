using Core.DTO;

namespace SolidPlayground_SOLID.Repository
{
    public interface IEquipmentActivityEventRepository
    {
        Task<bool> BookingExists(string key);
        Task<bool> Store(EquipmentActivity equipmentActivity);
    }
}
