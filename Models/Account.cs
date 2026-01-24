using FinFlowAPI.Models;

public class Accounts
{
    public int Id {get;set;}
    public decimal accountBalance {get;set;}
    public int userId {get;set;}
    public int accountTypeId {get;set;}
    public DateTime createdAt {get;set;}
    public bool IsActive {get;set;}
}