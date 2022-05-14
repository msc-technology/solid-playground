﻿using Core.DTO;
using Infrastructure.Storage;
using Infrastructure.Storage.Entities;

namespace SolidPlayground_I.Repository
{
    // Applies:
    // I: Interface segregation
    public class EquipmentActivityEventRepository : IEquipmentActivityRepository
    {
        private readonly BookingEventReadonlyRepository bookingEventReadonlyRepository;

        public EquipmentActivityEventRepository()
        {
            bookingEventReadonlyRepository = new BookingEventReadonlyRepository();
        }

        public async Task<bool> BookingExists(string key)
        {
            return await bookingEventReadonlyRepository.Exists(key);
        }

        public async Task<bool> Store(EquipmentActivity? message)
        {
            if (message is null)
            {
                return false;
            }

            using (var db = new StorageContext())
            {
                await db.EquipmentActivity.AddAsync(new EquipmentActivityEntity(message.ActivityId, message.BookingNumber));
                db.SaveChanges();
                return true;
            }
        }
    }
}
