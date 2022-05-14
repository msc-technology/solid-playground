using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolidPlaygroundCore.Storage.Entities
{
    public class BookingEntity
    {
        [Key]
        public string BookingNumber { get; set; }

        public BookingEntity(string bookingNumber)
        {
            BookingNumber = bookingNumber;
        }
    }
}
