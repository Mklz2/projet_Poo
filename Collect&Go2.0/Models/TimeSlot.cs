namespace Collect_Go2._0.Models
{
    public class TimeSlot
    {
        private int timeSlotId;
        private DateTime date;
        private DateTime startHour;
        private DateTime endHour;
        private int reservationCount;

        public int TimeSlotId
        {
            get => timeSlotId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant du créneau ne peut pas être négatif.");

                timeSlotId = value;
            }
        }

        public DateTime Date
        {
            get => date;
            set => date = value;
        }

        public DateTime StartHour
        {
            get => startHour;
            set => startHour = value;
        }

        public DateTime EndHour
        {
            get => endHour;
            set => endHour = value;
        }

        public int ReservationCount
        {
            get => reservationCount;
            set
            {
                if (value < 0 || value > MaxReservation)
                    throw new ArgumentException(
                        $"Le nombre de réservations doit être compris entre 0 et {MaxReservation}.");

                reservationCount = value;
            }
        }

        public const int MaxReservation = 10;

        public Store? Store { get; set; }

        public List<Order> Orders { get; set; } = new();

        public TimeSlot()
        {
        }

        public TimeSlot(
            int timeSlotId,
            DateTime date,
            DateTime startHour,
            DateTime endHour)
        {
            TimeSlotId = timeSlotId;
            Date = date;
            StartHour = startHour;
            EndHour = endHour;
            ReservationCount = 0;
        }

        public bool IsAvailable()
        {
            return ReservationCount < MaxReservation;
        }
    }
}