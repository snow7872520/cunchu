using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using API;

public class UpdateModel : PageModel {
    /*
      Request URL for Login action
     */
    public string login_url = "/index.php?page=api&module=auth&action=login";
    public string login_method = "POST";
    public string login_request = "";

    /* 
       Request URL for List action
       For this example we using AccountsReceivable/OrderProcessing/ViewOrders Enterprise screen, but you can use any screen from list in file EnterpriseScreens.json
     */
    public string list_url = "/index.php?page=api&module=forms&path=AccountsReceivable/OrderProcessing/ViewOrders&action=list&session_id=";
    public string list_method = "GET";

    /* 
       Request URL for Get action
       id contains '__' separated list from Company, Division, Department and OrderNumber
       We calling it keyString. Each record had own keyString
     */
    public string get_url = "/index.php?page=api&module=forms&path=AccountsReceivable/OrderProcessing/ViewOrders&action=get&id=DINOS__DEFAULT__DEFAULT__2372&session_id=";
    public string get_method = "GET";

    /* 
       Request URL for Update action
     */
    public string update_url = "/index.php?page=api&module=forms&path=AccountsReceivable/OrderProcessing/ViewOrders&action=update&id=DINOS__DEFAULT__DEFAULT__2372&session_id=";
    public string update_method = "POST";

    static HttpClient myAppHTTPClient = new HttpClient();

    public UpdateModel(){
        APIRequests();
    }

    public async void APIRequests(){
        dynamic body = new JObject();
        /*Credentials for Login request*/
        body.CompanyID = "DINOS";
        body.DivisionID = "DEFAULT";
        body.DepartmentID = "DEFAULT";
        body.EmployeeID = "Demo";
        body.EmployeePassword = "Demo";
        body.language = "english";

        /*
          Login request. Request Body is JSON, Response body is JSON
          Response is json like:
          {
          "session_id": "aud8s4l449frcnponmv1ithvoo",
          "companies": [],
          "message": "ok"
          }
          Where session_id is uuid, which used for any other API request
         */
        dynamic sessionResult = JObject.Parse(await(API.doRequest(this.login_method, this.login_url, this.login_request = body.ToString())));
        Console.WriteLine(sessionResult);
        
        /*
          Forms Get Request
          Getting order record for updating
         */
        dynamic order = JObject.Parse(await(API.doRequest(get_method, get_url + sessionResult.session_id, null)));
        order.CustomerID = "dland";
        order.CustomerPhone = "79998888888";

        /*
          Forms Update Request
          Updating order record 
        */
        await(API.doRequest(this.update_method, update_url + sessionResult.session_id, order.ToString()));
        /*                         url: `../index.php?page=api&module=forms&path=AccountsReceivable/OrderProcessing/ViewOrders&action=update&id=DINOS__DEFAULT__DEFAULT__2110&session_id=${session.session_id}`,
                         type: 'POST',
                         data : JSON.stringify(data),
                         contentType : 'application/json'
                     });
                     */
        /*
          Forms List Request.
          Getting list of all Opened Orders
         */
        Console.WriteLine(await(API.doRequest(list_method, list_url + sessionResult.session_id, null)));
    }
}
