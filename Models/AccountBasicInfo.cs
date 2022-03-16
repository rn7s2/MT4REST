public class AccountBasicInfo
{
    public double equity { get; set; }
    public double freeMargin { get; set; }
    public double profit { get; set; }

    public AccountBasicInfo(double equity, double freeMargin, double profit)
    {
        this.equity = equity;
        this.freeMargin = freeMargin;
        this.profit = profit;
    }
}
