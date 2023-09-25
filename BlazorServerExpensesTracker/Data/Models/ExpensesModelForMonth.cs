namespace BlazorServerExpensesTracker.Data.Models
{
    public class ExpensesModelForMonth
    {
        public int Id { get; set; }
        public string Month { get; set; } = string.Empty;
        public int MonthNumber { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
