using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Storage.Entities
{
    public class EquipmentActivityEntity
    {
        public long Id { get; set; }
        public long ActivityId { get; set; }
        public string BookingNumber { get; set; }

        public EquipmentActivityEntity(
            long activityId,
            string bookingNumber
        )
        {
            ActivityId = activityId;
            BookingNumber = bookingNumber;
        }
    }
}
