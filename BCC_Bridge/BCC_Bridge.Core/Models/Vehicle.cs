using SQLite;

namespace BCC_Bridge.Core
{
    [Table("Vehicle")]
    public class Vehicle
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Height { get; set; }
    }
}