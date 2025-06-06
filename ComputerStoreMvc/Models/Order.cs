public class Order {
    public int OrderId { get; set; }
    public List<Computer> Computers { get; set; }
    public decimal TotalPrice { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime OrderDate { get; set; }
}