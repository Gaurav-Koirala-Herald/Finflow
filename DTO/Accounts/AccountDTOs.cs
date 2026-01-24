public class AccountListDTO
{
    public int id {get;set;}
    public int accountTypeId {get;set;}
    public string accountName {get;set;}
    public decimal accountBalance {get;set;}
}
public class AccountDTO
{
    public int id {get;set;}
    public int accountTypeId {get;set;}
    public string accountName {get;set;}
    public DateTime createdAt{get;set;}
    public bool IsActive {get;set;}
    public decimal accountBalance {get;set;}
}
public class AddAccountDTO
{
    public int userId {get;set;}
    public string accountName {get;set;}
    public int accountTypeId {get;set;}
    public decimal accountBalance {get;set;}
}
public class UpdateAccountDTO
{
    public int id {get;set;}
    public int userId {get;set;}
    public string accountName {get;set;}
    public int accountTypeId {get;set;}
    public decimal accountBalance {get;set;}
}