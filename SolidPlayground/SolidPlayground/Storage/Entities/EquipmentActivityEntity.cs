using System.ComponentModel.DataAnnotations;

namespace SolidPlayground.Storage.Entities
{
    public class EquipmentActivityEntity
    {
        [Key]
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
