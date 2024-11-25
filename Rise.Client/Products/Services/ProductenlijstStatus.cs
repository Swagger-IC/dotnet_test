using System;

namespace Rise.Client.Products.Services;

public class ProductenlijstStatus
{
    public bool TableView {get; set;} = false;
    public bool UitScannen {get; set;} = true;
    public string Filter {get; set;} = string.Empty;
    public int Paginanummer {get; set;} = 1;
    public int Aantal {get; set;} = 10;
    public int TotalCount {get; set;}
    public int TotalPages {get;set;}
    public bool VanDetailPagina {get; set;} = false;
}
