namespace GymSystem.DAL.Entities
{
    public class Member:GymUser
    {
        public string Photo { get; set; } = null!;
        public HealthRecord HealthRecord { get; set; } = null!;
        public ICollection<Membership> Memberships = new HashSet<Membership>(); 
        public ICollection<Booking> Bookings = new HashSet<Booking>();
    }
}