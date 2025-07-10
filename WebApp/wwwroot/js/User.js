function UserViewController()
{

    this.ViewName = "User";
    this.ApiEndpoint = "User";

    //Metodo Constructor
    this.InitView = function ()
    {

        console.log("UserViewController.InitView --> Ok");
        this.loadTable();


    }
}

