
namespace ComputerBuilderMvcApp.Models
{
    public class Computer
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; } // Sum of components with a % discount if all slots filled
        public List<Component> StandardComponents { get; set; }  = new List<Component>(); // RAM, HDD, CPU, etc.
    }
}