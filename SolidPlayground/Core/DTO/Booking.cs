namespace Core.DTO
{
    public class Booking
    {
        public string BookingNumber { get; set; }

        public Booking(string bookingNumber)
        {
            BookingNumber = bookingNumber;
        }
    }
}