
namespace ComputerBuilderMvcApp.Models
{
    public class Computer
    {
        public int TotalPrice { get; set; } // Sum of components with a % discount if all slots filled
        public List<Component> Components { get; set; }  = []; // RAM, HDD, CPU, etc.
    }
}