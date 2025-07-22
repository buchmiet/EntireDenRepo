using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class CompleteOrderViewItem : ObservableObject
{
    public string _buyer;
    public string _status;
    public string _country;


    public string Item { get; set; }
    public string Buyer
    {
        get => _buyer; 
        set => SetProperty(ref _buyer,value);
    }

    private string _transactionNo;
    public string TransactionNo
    {
        get => _transactionNo; 
        set => SetProperty(ref _transactionNo,value);
            
    }

    public DateTime PaidOn { get; set; }
    public string Country
    {
        get => _country; 
        set => SetProperty(ref _country,value);
    }
    public string Status
    {
        get =>  _status; 
        set => SetProperty(ref _status,value);
            
    }
    public int id { get; set; }
    public int Number { get; set; }

       

}