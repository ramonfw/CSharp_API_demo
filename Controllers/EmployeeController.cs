using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using System.Data.SQLite;
//using myapi_cs.Controllers.SQLiteController;


namespace myapi_cs.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        //        private const string ConnectionString = "..\\..\\..\\Data Source=chinook.db; Version = 3; New = True; Compress = True; ";
        //        sqlite_conn = new SQLiteConnection("Data Source=chinook.db; Version = 3; New = True; Compress = True; ");
        //        private const string ConnectionString = "Data Source=..\\..\\..\\chinook.db; Version = 3; New = True; Compress = True; ";
        //        private const string ConnectionString = "Data Source=chinookk3.db; Version = 3; New = True; Compress = True; ";
        private const string ConnectionString = "Data Source=chinook.db; Version = 3; New = True; Compress = True; ";

        private List<string> listOfDummies = new List<string>();

        SQLiteController vSQLiteController = null;

        HttpRequest activeHttpRequest;
        String activeRoute = null;

        void FillDummyArray()
        {
            if (listOfDummies.Count == 0)
            {
                StreamReader sr = new StreamReader("SavedList.txt");
                while (sr.Peek() >= 0)
                    listOfDummies.Add(sr.ReadLine());
                sr.Close();
            }
        }


        // Constructor EmployeeController
#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public EmployeeController()
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            FillDummyArray();
            vSQLiteController = new SQLiteController();
            vSQLiteController.CreateConnection(ConnectionString);
/*
            activeHttpRequest = HttpContext.Request;
#pragma warning disable CS8601 // Posible asignación de referencia nula
            activeRoute = activeHttpRequest.RouteValues.ToString();
#pragma warning restore CS8601 // Posible asignación de referencia nula
*/
        }


        private string FormatJsonReturn(bool pResult, string pMessage, List<string> pData)
        {
            var jsonSerialiser = new JavaScriptSerializer();   // new Nancy.Json.JavaScriptSerializer();
            var result = jsonSerialiser.Serialize(new { result = pResult, message = pMessage, data = pData });
            return result;
        }

        private string FormatJsonReturn(bool pResult, string pMessage, List<List<string>> pData)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var result = jsonSerialiser.Serialize(new { result = pResult, message = pMessage, data = pData });
            return result;
        }

        private string FormatJsonReturn(bool pResult, string pMessage, string pData)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var result = jsonSerialiser.Serialize(new { result = pResult, message = pMessage, data = pData });
            return result;
        }

        private string FormatJsonReturn(bool pResult, string pMessage, long pData)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var result = jsonSerialiser.Serialize(new { result = pResult, message = pMessage, data = pData });
            return result;
        }


        // GET: api/<EmployeeController>
        [HttpGet]
        public string Get()       //  Ok Open Close
        {
            string vActiveRoute = "Method - [" + HttpContext.Request.Method +
                "] Path - [" + HttpContext.Request.Path + "]";

            string vMsgResponse = "Bienvenido a la ruta /api/Employee. Servivio en línea.";
            string vMsgResponse2 = "Para utilizar los endpoints, necesitará autenticación básica o Bearer Token." + vActiveRoute;
            if (vSQLiteController.lastErrorMessage != "")
                vMsgResponse = vSQLiteController.lastErrorMessage;

            return FormatJsonReturn(true, vMsgResponse, vMsgResponse2);
        }


        // GET: api/<EmployeeController>
        [HttpGet("count")]
        public string GetContar()       //  Ok Open Close
        {
            long vCuentaEmployeeAll = vSQLiteController.CountEmployeeAll();

            string vActiveRoute = "Method - [" + HttpContext.Request.Method +
                "] Path - [" + HttpContext.Request.Path +"]";

            string vMsgResponse = "Conteo de empleados realizado satisfactoriamente"+"."+ vActiveRoute;
            if (vSQLiteController.lastErrorMessage != "")
                vMsgResponse = vSQLiteController.lastErrorMessage;

            return FormatJsonReturn(true, vMsgResponse, vCuentaEmployeeAll);
        }


        // GET: api/<EmployeeController>/list/3/7    donde 7: SQL limit
        [HttpGet("list/{inicio}/{cantidad}")]
        public string GetListLimit(int inicio, int cantidad)
        {
            List<List<string>> vListEmployeeList = vSQLiteController.GetDataEmployeeAll(inicio,cantidad);

            string vMsgResponse = "Listado de los primeros " + cantidad.ToString() + " empleados comenzando por el numero " + inicio.ToString() + " obtenido satisfactoriamente";
            if (vSQLiteController.lastErrorMessage != "")
                vMsgResponse = vSQLiteController.lastErrorMessage;

            return FormatJsonReturn(true, vMsgResponse, vListEmployeeList);
        }


        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            List<string> arrDatosEmployeeIni = vSQLiteController.GetDataEmployeeById(id);

            string vMsgResponse = "Datos de empleado " + id.ToString() + " obtenidos satisfactoriamente";
            if (vSQLiteController.lastErrorMessage != "")
                vMsgResponse = vSQLiteController.lastErrorMessage;

            return FormatJsonReturn(true, vMsgResponse, arrDatosEmployeeIni);
        }


        // POST api/<EmployeeController>
        [HttpPost]
        public string Post(string[] employee)
        {
            bool result = vSQLiteController.InsertEmployee(employee);

            if (result)
                return FormatJsonReturn(true, "Datos de nuevo empleado guardados satisfactoriamente", 1);
            else
                return FormatJsonReturn(false, "Datos de nuevo empleado no se han guardado. " + vSQLiteController.lastErrorMessage, 1);
        }


        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public string Put(int id, string[] employee)
        {
            bool result = vSQLiteController.UpdateEmployee(id, employee);

            if (result)
                return FormatJsonReturn(true, "Datos de empleado con id " + id.ToString() + " editado satisfactoriamente", 1);
            else
                return FormatJsonReturn(false, "Datos de empleado con id " + id.ToString() + " no se han guardado. " + vSQLiteController.lastErrorMessage, 1);
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            int borrado = vSQLiteController.DeleteEmployee(id);

            if (borrado>0)
                return FormatJsonReturn(true, "Dato en id " + id.ToString() + " eliminado satisfactoriamente", borrado);
            else
                return FormatJsonReturn(false, "Dato en id " + id.ToString() + " no se ha borrado. "+ vSQLiteController.lastErrorMessage, borrado);
        }
    }

}
