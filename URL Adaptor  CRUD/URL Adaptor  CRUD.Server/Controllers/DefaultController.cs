using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.Blazor;

namespace URL_Adaptor__CRUD.Server.Controllers
{
    
    [ApiController]
    public class DefaultController : ControllerBase
    {
      public static List<Orders> order = new List<Orders>();
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody]DataManagerRequest dm )
        {
            if(order.Count == 0)
                BindDataSource();
            IEnumerable DataSource = order;
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = DataOperations.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = DataOperations.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Orders>().Count();
            if (dm.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, dm.Take);
            }
            return new { result = DataSource, count = count };

        }
        [HttpPost]
        [Route("api/Default/Update")]
        public object Update([FromBody]CRUDModel<Orders> value)
        {
            var ord = value.Value;
            Orders val = order.Where(or => or.OrderID == ord.OrderID).FirstOrDefault();
            val.OrderID = ord.OrderID;
            val.EmployeeID = ord.EmployeeID;
            val.CustomerID = ord.CustomerID;
            val.Freight = ord.Freight;
            val.ShipCity = ord.ShipCity;
            return value.Value;
        }
        [HttpPost]
        [Route("api/Default/Insert")]
        public object Insert([FromBody]CRUDModel<Orders> value)
        {
            order.Insert(order.Count, value.Value);
            return order;
        }
        [HttpPost]
        [Route("api/Default/Delete")]
        public object Delete([FromBody]CRUDModel<Orders> value)
        {
            order.Remove(order.Where(or => or.OrderID == int.Parse(value.Key.ToString())).FirstOrDefault());
            return value;
        }
        private void BindDataSource()
        {
            int code = 10000;

            for (int i = 1; i < 10; i++)
            {
                order.Add(new Orders(code + 1, "ALFKI", i + 0, 2.3 * i, new DateTime(1991, 05, 15), "Berlin"));
                order.Add(new Orders(code + 2, "ANATR", i + 2, 3.3 * i, new DateTime(1990, 04, 04), "Madrid"));
                order.Add(new Orders(code + 3, "ANTON", i + 1, 4.3 * i, new DateTime(1957, 11, 30), "Cholchester"));
                order.Add(new Orders(code + 4, "BLONP", i + 3, 5.3 * i, new DateTime(1930, 10, 22), "Marseille"));
                order.Add(new Orders(code + 5, "BOLID", i + 4, 6.3 * i, new DateTime(1953, 02, 18), "Tsawassen"));
                code += 5;
            }            
        }
        public class CRUDModel<T> where T : class
        {            

            [JsonProperty("action")]
            public string Action { get; set; }
            [JsonProperty("table")]
            public string Table { get; set; }
            [JsonProperty("keyColumn")]
            public string KeyColumn { get; set; }
            [JsonProperty("key")]
            public object Key { get; set; }
            [JsonProperty("value")]
            public T Value { get; set; }
            [JsonProperty("added")]
            public List<T> Added { get; set; }
            [JsonProperty("changed")]
            public List<T> Changed { get; set; }
            [JsonProperty("deleted")]
            public List<T> Deleted { get; set; }
            [JsonProperty("params")]
            public IDictionary<string, object> Params { get; set; }
        }
        [Serializable]
        public class Orders
        {
            public Orders()
            {

            }
            public Orders(long OrderId, string CustomerId, int EmployeeId, double Freight, DateTime OrderDate, string ShipCity)
            {
                this.OrderID = OrderId;
                this.CustomerID = CustomerId;
                this.EmployeeID = EmployeeId;
                this.Freight = Freight;
                this.OrderDate = OrderDate;
                this.ShipCity = ShipCity;
            }
            public long OrderID { get; set; }
            public string CustomerID { get; set; }
            public int EmployeeID { get; set; }
            public double Freight { get; set; }
            public DateTime OrderDate { get; set; }
            public string ShipCity { get; set; }
        }
    }
}


























@page "/"

<h1>Hello, world!</h1>



@using Syncfusion.EJ2.Blazor
@using Syncfusion.EJ2.Blazor.Grids
@using Syncfusion.EJ2.Blazor.Buttons
@using Syncfusion.EJ2.Blazor.Data
@using Microsoft.JSInterop;
@using Newtonsoft.Json;
@using Microsoft.VisualBasic.CompilerServices;
@using System.Threading;
@using System.Threading.Tasks;
@using System.Net.Http

<EjsGrid TValue="OrdersDetails" ID="Grid" @ref="@defaultGrid" @ref:suppressField AllowSorting="true" AllowFiltering="true" AllowPaging="true" Toolbar="@(new List<string>() { "Add", "Delete", "Update", "Cancel" })">
    @*@if (flag)
    {*@
        <EjsDataManager AdaptorInstance="@typeof(CustomAdaptor)" Adaptor="Adaptors.CustomAdaptor"></EjsDataManager>
    @*  *@}
    <GridEditSettings AllowEditing="true" AllowDeleting="true" AllowAdding="true" Mode="@EditMode.Normal"></GridEditSettings>
    <GridColumns>
        <GridColumn Field=@nameof(OrdersDetails.OrderID) HeaderText="Order ID" IsPrimaryKey="true" TextAlign="@TextAlign.Center" Width="140"></GridColumn>
        <GridColumn Field=@nameof(OrdersDetails.CustomerID) HeaderText="Customer Name" Width="150">
            @*<Template>
                    @{
                        <div class="image">
                            asdf
                        </div>
                    }
                </Template>*@
        </GridColumn>
        <GridColumn Field=@nameof(OrdersDetails.ShipCountry) HeaderText="Ship Country" Width="150"></GridColumn>
        @*<GridColumn Field=@nameof(OrdersDetails.Freight) HeaderText="Freight" EditType="numericedit" Format="C2" Width="140" TextAlign="@TextAlign.Right"></GridColumn>
            <GridColumn Field=@nameof(OrdersDetails.OrderDate) HeaderText="Order Date" EditType="datepickeredit" Format="yMd" Type="date" Width="160"></GridColumn>
            <GridColumn Field=@nameof(OrdersDetails.ShipCountry) HeaderText="Ship Country" EditType="dropdownedit" Width="150"></GridColumn>*@
    </GridColumns>
</EjsGrid>

@functions{
        EjsGrid<OrdersDetails> defaultGrid;
        public bool flag = false;

        public void clickHand()
        {
            this.flag = true;
        }

        public class Orders
        {
            public Orders() { }
            public Orders(int OrderID, string CustomerID)
            {
                this.OrderID = OrderID;
                this.CustomerID = CustomerID;
            }
            public int OrderID { get; set; }
            public string CustomerID { get; set; }
        }
        //class odata
        //{
        //    public object[] value;
        //}
        public class CustomAdaptor : DataAdaptor
        {

            static readonly HttpClient client = new HttpClient();
            public static List<OrdersDetails> order = OrdersDetails.GetAllRecords();

            public override object Read(DataManagerRequest dm, string key = null)
            {
                IEnumerable<OrdersDetails> DataSource = order;
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = DataOperations.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = DataOperations.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = DataOperations.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                int count = DataSource.Cast<OrdersDetails>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);         //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = DataOperations.PerformTake(DataSource, dm.Take);
                }
                return dm.RequiresCounts ? new DataResult() { Result = DataSource, Count = count } : (object)DataSource;
            }
            //public override async Task<object> ReadAsync(DataManagerRequest dmr)
            //{
            //    //object a = await client.GetJsonAsync<object[]>("https://jsonplaceholder.typicode.com/todos/1");
            //    DataResult b = new DataResult();
            //    var path = "https://services.odata.org/V4/Northwind/Northwind.svc/Orders/";
            //    HttpResponseMessage response = await client.GetAsync(path);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string asd = await response.Content.ReadAsStringAsync();
            //        b.Result = JsonConvert.DeserializeObject<odata>(asd).value;
            //        b.Count = 200;
            //    }
            //    return b;
            //}
            public override object Insert(DataManager dm, object value, string key)
            {
                order.Insert(0, value as OrdersDetails);
                return value;
            }
            //public override async Task<object> InsertAsync(DataManager dm, object value, string key)
            //{
            //    order.Add(value as OrdersDetails);
            //    await Task.Delay(3000);
            //    return value;
            //}
            public override object Remove(DataManager dm, object value, string keyField, string key)
            {
                order.Remove(order.Where(or => or.OrderID == int.Parse(value.ToString())).FirstOrDefault());
                return value;
            }
            //public override async Task<object> RemoveAsync(DataManager dm, object value, string keyField, string key)
            //{
            //    var b = (int)value;
            //    order.RemoveAt(0);
            //    await Task.Delay(2000);
            //    return new {OrderID= value, CustomerID= "asdf" };
            //}
            public override object Update(DataManager dm, object value, string keyField, string key)
            {
                var data = order.Where(or => or.OrderID == (value as OrdersDetails).OrderID).FirstOrDefault();
                if(data != null)
                {
                    data.OrderID = (value as OrdersDetails).OrderID;
                    data.CustomerID = (value as OrdersDetails).CustomerID;
                    data.ShipCountry = (value as OrdersDetails).ShipCountry;
                }
                return value;
            }
            //public override async Task<object> UpdateAsync(DataManager dm, object value, string keyField, string key)
            //{
            //    await Task.Delay(2000);
            //    this.dataList[0] = value as OrdersDetails;
            //    return value;
            //}
            public override object BatchUpdate(DataManager dm, object Changed, object Added, object Deleted, string KeyField, string Key)
            {
                if (Changed != null)
                {
                    foreach (var rec in (IEnumerable<OrdersDetails>)Changed)
                    {
                        order[0].CustomerID = rec.CustomerID;
                    }

                }
                if (Added != null)
                {
                    foreach (var rec in (IEnumerable<OrdersDetails>)Added)
                    {
                        order.Add(rec);
                    }

                }
                if (Deleted != null)
                {
                    foreach (var rec in (IEnumerable<OrdersDetails>)Deleted)
                    {
                        order.RemoveAt(0);
                    }

                }
                return order;
            }
            //public override async Task<object> BatchUpdateAsync(DataManager dm, object Changed, object Added, object Deleted, string KeyField, string Key)
            //{
            //    await Task.Delay(3000);
            //    foreach(var rec in (IEnumerable<Orders>)Changed)
            //    {
            //        this.dataList[0].CustomerID = rec.CustomerID;
            //    }
            //    foreach(var rec in (IEnumerable<OrdersDetails>)Added)
            //    {
            //        this.dataList.Add(rec);
            //    }
            //    foreach(var rec in (IEnumerable<Orders>)Deleted)
            //    {
            //        this.dataList.RemoveAt(0);
            //    }
            //    return this.dataList;
            //}

        }

        public class CustomAdaptorTwo : DataAdaptor
        {

            static readonly HttpClient client = new HttpClient();
            public static List<OrdersDetails> order = OrdersDetails.GetAllRecords();

            public override object Read(DataManagerRequest dm, string key = null)
            {
                IEnumerable<OrdersDetails> DataSource = order;
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = DataOperations.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = DataOperations.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = DataOperations.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                int count = DataSource.Cast<OrdersDetails>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = DataOperations.PerformSkip(DataSource, dm.Skip);         //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = DataOperations.PerformTake(DataSource, dm.Take);
                }
                return dm.RequiresCounts ? new DataResult() { Result = DataSource, Count = count } : (object)DataSource;
            }
            //public override async Task<object> ReadAsync(DataManagerRequest dmr)
            //{
            //    //object a = await client.GetJsonAsync<object[]>("https://jsonplaceholder.typicode.com/todos/1");
            //    DataResult b = new DataResult();
            //    var path = "https://services.odata.org/V4/Northwind/Northwind.svc/Orders/";
            //    HttpResponseMessage response = await client.GetAsync(path);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string asd = await response.Content.ReadAsStringAsync();
            //        b.Result = JsonConvert.DeserializeObject<odata>(asd).value;
            //        b.Count = 200;
            //    }
            //    return b;
            //}
            public override object Insert(DataManager dm, object value, string key)
            {
                OrdersDetails data = value as OrdersDetails;
                order.Insert(0, data);
                return value;
            }
            //public override async Task<object> InsertAsync(DataManager dm, object value, string key)
            //{
            //    order.Add(value as OrdersDetails);
            //    await Task.Delay(3000);
            //    return value;
            //}
            public override object Remove(DataManager dm, object value, string keyField, string key)
            {
                order.Remove(order.Where(or => or.OrderID == int.Parse(value.ToString())).FirstOrDefault());
                return value;
            }
            //public override async Task<object> RemoveAsync(DataManager dm, object value, string keyField, string key)
            //{
            //    var b = (int)value;
            //    order.RemoveAt(0);
            //    await Task.Delay(2000);
            //    return new {OrderID= value, CustomerID= "asdf" };
            //}
            public override object Update(DataManager dm, object value, string keyField, string key)
            {
                order.Where(or => or.OrderID == (value as OrdersDetails).OrderID).FirstOrDefault();
                return value;
            }
            //public override async Task<object> UpdateAsync(DataManager dm, object value, string keyField, string key)
            //{
            //    await Task.Delay(2000);
            //    this.dataList[0] = value as OrdersDetails;
            //    return value;
            //}
            public override object BatchUpdate(DataManager dm, object Changed, object Added, object Deleted, string KeyField, string Key)
            {
                List<OrdersDetails> asdf = (List<OrdersDetails>)Changed;
                foreach (var rec in (IEnumerable<OrdersDetails>)Changed)
                {
                    order[0].CustomerID = rec.CustomerID;
                }
                foreach (var rec in (IEnumerable<OrdersDetails>)Added)
                {
                    order.Add(rec);
                }
                foreach (var rec in (IEnumerable<OrdersDetails>)Deleted)
                {
                    order.RemoveAt(0);
                }
                return order;
            }
            //public override async Task<object> BatchUpdateAsync(DataManager dm, object Changed, object Added, object Deleted, string KeyField, string Key)
            //{
            //    await Task.Delay(3000);
            //    foreach(var rec in (IEnumerable<Orders>)Changed)
            //    {
            //        this.dataList[0].CustomerID = rec.CustomerID;
            //    }
            //    foreach(var rec in (IEnumerable<OrdersDetails>)Added)
            //    {
            //        this.dataList.Add(rec);
            //    }
            //    foreach(var rec in (IEnumerable<Orders>)Deleted)
            //    {
            //        this.dataList.RemoveAt(0);
            //    }
            //    return this.dataList;
            //}

        }



        public List<OrdersDetails>
        gridData
        { get; set; }
        protected override void OnInitialized()
    {
        gridData = OrdersDetails.GetAllRecords();
    }
    [JSInvokable]
    public void CreatedHandler(object args)
    {
        this.flag = true;

    }
}