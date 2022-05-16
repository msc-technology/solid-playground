namespace Core.DTO
{
    public class EquipmentActivity
    {
        public long ActivityId { get; set; }
        public string BookingNumber { get; set; }

        public EquipmentActivity(
            long activityId,
            string bookingNumber
        )
        {
            ActivityId = activityId;
            BookingNumber = bookingNumber;
        }
    }
}
