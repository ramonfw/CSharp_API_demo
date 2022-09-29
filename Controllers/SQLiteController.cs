using System.Data.SQLite;

namespace myapi_cs.Controllers
{
    public class SQLiteController
    {
        public SQLiteConnection sqlite_conn = null;
        public string lastErrorMessage = "";
        public int lastErrorCode = 0;

        public void CreateConnection(string pConnectionString)
        {
            sqlite_conn = new SQLiteConnection(pConnectionString);
        }

        public SQLiteConnection OpenConnection()
        {
            try
            {
                sqlite_conn.Open();
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de Open SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
                sqlite_conn = null;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
                sqlite_conn = null;
            }
            return sqlite_conn;
        }

        public void CloseConnection()
        {
            try
            {
                sqlite_conn.Close();
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de Close SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
                sqlite_conn = null;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
                sqlite_conn = null;
            }
        }

        public bool ValidateDataArray(String[] pArrDatosEmployee)
        {
            int vNoValido = 0;
            for (int i = 0; i < pArrDatosEmployee.Length; i++)
            {
                if (pArrDatosEmployee[i].Trim() == "")
                    vNoValido++;
            }
            return vNoValido == 0;
        }

        public bool ValidaHayCambios(String[] pArrDatosEmployee, String[] pArrDatosEmployeeIni)
        {
            int vCambio = 0;
            for (int i = 0; i < pArrDatosEmployee.Length; i++)
            {
                if (pArrDatosEmployee[i].Trim() != pArrDatosEmployeeIni[i].Trim())
                    vCambio++;
            }
            return vCambio > 0;
        }

        public bool UpdateEmployee(int pEmployeeId, String[] pArrDatosEmployee)
        {
            SQLiteCommand sqlite_cmd;
            lastErrorMessage = "";
            lastErrorCode = 0;

            List<string> pArrDatosEmployeeIni = GetDataEmployeeById(pEmployeeId);
            if (this.lastErrorMessage != "")
            {
                return false;
            }

            if (pArrDatosEmployee == null)
            {
                this.lastErrorMessage = "No hay datos a cambiar del empleado (NULL)";
                return false;
            }
            if (pArrDatosEmployee.Length == 0)
            {
                this.lastErrorMessage = "Faltan datos a cambiar del empleado";
                return false;
            }
            if (pArrDatosEmployee[0] == "string")
            {
                this.lastErrorMessage = "No hay datos a cambiar del empleado";
                return false;
            }

            this.OpenConnection();
            sqlite_cmd = this.sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "UPDATE employees SET ";

            String[] ArrCampos = { "EmployeeId", "LastName", "FirstName", "Title", "ReportsTo", "BirthDate", "HireDate", "Address", "City", "State", "Country", "PostalCode", "Phone", "Fax", "Email" };
            string Cadena = "";
            string Datos = "";

            try
            {
                for (int i = 1; i < ArrCampos.Length; i++)
                {
                    if (pArrDatosEmployee[i] != pArrDatosEmployeeIni[i])
                    {
                        Datos += ArrCampos[i] + ":" + pArrDatosEmployee[i] + "," + pArrDatosEmployeeIni[i] + "][";
                        if (Cadena != "")
                            Cadena += ", ";
                        Cadena += ArrCampos[i] + "= ?";
                        sqlite_cmd.Parameters.AddWithValue(ArrCampos[i], pArrDatosEmployee[i]);
                    }
                }

                sqlite_cmd.CommandText += Cadena + " WHERE EmployeeId = ?";
                sqlite_cmd.Parameters.AddWithValue("EmployeeId", pArrDatosEmployee[0]);

                if (Cadena != "")
                {
                    sqlite_cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de UPDATE SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
            }

            this.CloseConnection();

            return false;
        }

        public bool InsertEmployee(String[] pArrDatosEmployee)    //  Ok Open Close
        {
            SQLiteCommand sqlite_cmd;
            String[] ArrCampos = { "EmployeeId", "LastName", "FirstName", "Title", "ReportsTo", "BirthDate", "HireDate", "Address", "City", "State", "Country", "PostalCode", "Phone", "Fax", "Email" };

            this.OpenConnection();
            sqlite_cmd = this.sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "INSERT INTO employees ";
            String Columnas = "(LastName,FirstName,Title,ReportsTo,BirthDate,HireDate,Address, " +
                "City, State, Country,PostalCode,Phone,Fax,Email)";
            String Valores = "(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            sqlite_cmd.CommandText += Columnas + " VALUES " + Valores;

            for (int i = 1; i < ArrCampos.Length; i++)
            {
                sqlite_cmd.Parameters.AddWithValue(ArrCampos[i], pArrDatosEmployee[i]);
            }

            int resultado = sqlite_cmd.ExecuteNonQuery();
            this.CloseConnection();

            return resultado > 0;
        }


        public int DeleteEmployee(Int64 pEmployeeId)    //  Ok Open Close
        {
            int resultado = -1;
            try
            {
                this.OpenConnection();
                SQLiteDataReader data_reader_to_delete = ReadDataEmployee("ReportsTo", pEmployeeId);
                if (this.lastErrorMessage == "")
                {
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = this.sqlite_conn.CreateCommand();
                    sqlite_cmd.CommandText = "DELETE FROM employees where EmployeeId = ?";

                    sqlite_cmd.Parameters.AddWithValue("EmployeeId", pEmployeeId);
                    resultado = sqlite_cmd.ExecuteNonQuery();

                }
                this.CloseConnection();
                return resultado;
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
            }

            return resultado;
        }

        public SQLiteDataReader ReadDataEmployee(String FieldName, Int64 FieldValue)      //  Ok Open Close
        {
            SQLiteDataReader sqlite_datareader = null;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = this.sqlite_conn.CreateCommand();

            this.lastErrorMessage = "";
            try
            {
                // Creamos la consulta SELECT
                sqlite_cmd.CommandText = "SELECT E1.*, E2.FirstName, E2.LastName FROM employees E1 left join employees E2 on E1.ReportsTo = E2.EmployeeId where E1." + FieldName + " = ?";
                // Añadimos el parámetro E1.FieldName
                sqlite_cmd.Parameters.AddWithValue("E1." + FieldName, FieldValue.ToString());
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                sqlite_datareader.Read();
                if (sqlite_datareader == null)
                {
                    this.lastErrorMessage = "Error SQL: Datos del ID " + FieldValue.ToString() + " no encontrados";
                }
                else if (!sqlite_datareader.HasRows)
                {
                    this.lastErrorMessage = "Datos del ID " + FieldValue.ToString() + " no encontrados";
                }
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
            }
            return sqlite_datareader;
        }

        public SQLiteDataReader ReadDataEmployeeAll(Int64 pLimit)         //  Ok Open Close
        {
            SQLiteDataReader sqlite_datareader = null;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = this.sqlite_conn.CreateCommand();

            lastErrorMessage = "";
            lastErrorCode = 0;

            try
            {
                if (pLimit > 0)
                {
                    sqlite_cmd.CommandText = "SELECT E1.*, E2.FirstName, E2.LastName FROM employees E1 left join employees E2 on E1.ReportsTo = E2.EmployeeId LIMIT ?";
                    // Añadimos el parámetro LIMIT
                    sqlite_cmd.Parameters.AddWithValue("LIMIT", pLimit.ToString());
                }
                else
                {
                    sqlite_cmd.CommandText = "SELECT E1.*, E2.FirstName, E2.LastName FROM employees E1 left join employees E2 on E1.ReportsTo = E2.EmployeeId";
                }

                // Ejecutamos la sentencia SELECT
                sqlite_datareader = sqlite_cmd.ExecuteReader();
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
            }

            return sqlite_datareader;
        }

        public long CountEmployeeAll()       //  Ok Open Close
        {
            SQLiteDataReader sqlite_datareader = null;
            SQLiteCommand sqlite_cmd;

            this.OpenConnection();
            sqlite_cmd = this.sqlite_conn.CreateCommand();
            long conteo = 0;

            try
            {
                // Creamos la consulta SELECT
                sqlite_cmd.CommandText = "SELECT Count() FROM employees E1 left join employees E2 on E1.ReportsTo = E2.EmployeeId";

                // Ejecutamos la sentencia SELECT
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                sqlite_datareader.Read();

                conteo = sqlite_datareader.GetInt64(0);
            }
            catch (SQLiteException e)
            {
                lastErrorMessage = "Error de SQLite " + e.Message.ToString() + "(" + e.ErrorCode.ToString() + " - " + e.ResultCode.ToString() + ")";
                lastErrorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                lastErrorMessage = "Error genérico " + e.Message.ToString() + "(" + e.ToString() + ")";
                lastErrorCode = 1000;
            }
            this.CloseConnection();

            return conteo;
        }

        public List<string> GetDataEmployeeById(int id)       //  Ok Open Close
        {
            List<string> arrDatosEmployeeIni = new List<string>();
            this.lastErrorMessage = "";
            this.lastErrorCode = 0;

            this.OpenConnection();

            SQLiteDataReader data_reader = this.ReadDataEmployee("EmployeeId", id);

            if (this.lastErrorMessage == "")
            {
                long vIdEmployee = data_reader.GetInt64(0);
                arrDatosEmployeeIni.Add(vIdEmployee.ToString());

                String vApellidos = data_reader.GetString(1);
                arrDatosEmployeeIni.Add(vApellidos);

                String vNombres = data_reader.GetString(2);
                arrDatosEmployeeIni.Add(vNombres);

                String vTitle = data_reader.GetString(3);
                arrDatosEmployeeIni.Add(vTitle);

                Object objReportsTo = data_reader.GetValue(4);
                if (objReportsTo is System.DBNull)
                {
                    arrDatosEmployeeIni.Add("No");
                }
                else
                {
                    String vNombresR = data_reader.GetString(15);
                    String vApellidosR = data_reader.GetString(16);
                    arrDatosEmployeeIni.Add(vNombresR + " " + vApellidosR);
                }
                String vBirthDate = data_reader.GetDateTime(5).ToString("yyyy-MM-dd");
                arrDatosEmployeeIni.Add(vBirthDate);

                String vHireDate = data_reader.GetDateTime(6).ToString("yyyy-MM-dd");
                arrDatosEmployeeIni.Add(vHireDate);

                String vAddress = data_reader.GetString(7);
                arrDatosEmployeeIni.Add(vAddress);

                String vCity = data_reader.GetString(8);
                arrDatosEmployeeIni.Add(vCity);

                String vState = data_reader.GetString(9);
                arrDatosEmployeeIni.Add(vState);

                String vCountry = data_reader.GetString(10);
                arrDatosEmployeeIni.Add(vCountry);

                String vPostalCode = data_reader.GetString(11);
                arrDatosEmployeeIni.Add(vPostalCode);

                String vPhone = data_reader.GetString(12);
                arrDatosEmployeeIni.Add(vPhone);

                String vFax = data_reader.GetString(13);
                arrDatosEmployeeIni.Add(vFax);

                String vEmail = data_reader.GetString(14);
                arrDatosEmployeeIni.Add(vEmail);
            }
            this.CloseConnection();
            return arrDatosEmployeeIni;
        }


        public List<List<string>> GetDataEmployeeAll(int pLimit)       //  Ok Open Close
        {
            List<List<string>> arrDatosEmployeeList = new List<List<string>>();
            this.lastErrorMessage = "";
            this.lastErrorCode = 0;
            string message = "";

            this.OpenConnection();
            SQLiteDataReader data_reader = this.ReadDataEmployeeAll(pLimit);

            if (data_reader == null)
            {
                message = this.lastErrorMessage;
            }
            else if (!data_reader.HasRows)
            {
                message = "Datos de empleados no encontrados";
                this.lastErrorMessage = message;
            }
            else
            {
                while (data_reader.Read())
                {
                    List<string> arrDatosEmployeeIni = new List<string>();
                    long vIdEmployee = data_reader.GetInt64(0);
                    arrDatosEmployeeIni.Add(vIdEmployee.ToString());

                    String vApellidos = data_reader.GetString(1);
                    arrDatosEmployeeIni.Add(vApellidos);

                    String vNombres = data_reader.GetString(2);
                    arrDatosEmployeeIni.Add(vNombres);

                    String vTitle = data_reader.GetString(3);
                    arrDatosEmployeeIni.Add(vTitle);

                    Object objReportsTo = data_reader.GetValue(4);
                    if (objReportsTo is System.DBNull)
                    {
                        arrDatosEmployeeIni.Add("No");
                    }
                    else
                    {
                        String vNombresR = data_reader.GetString(15);
                        String vApellidosR = data_reader.GetString(16);
                        arrDatosEmployeeIni.Add(vNombresR + " " + vApellidosR);
                    }
                    String vBirthDate = data_reader.GetDateTime(5).ToString("yyyy-MM-dd");
                    arrDatosEmployeeIni.Add(vBirthDate);

                    String vHireDate = data_reader.GetDateTime(6).ToString("yyyy-MM-dd");
                    arrDatosEmployeeIni.Add(vHireDate);

                    String vAddress = data_reader.GetString(7);
                    arrDatosEmployeeIni.Add(vAddress);

                    String vCity = data_reader.GetString(8);
                    arrDatosEmployeeIni.Add(vCity);

                    String vState = data_reader.GetString(9);
                    arrDatosEmployeeIni.Add(vState);

                    String vCountry = data_reader.GetString(10);
                    arrDatosEmployeeIni.Add(vCountry);

                    String vPostalCode = data_reader.GetString(11);
                    arrDatosEmployeeIni.Add(vPostalCode);

                    String vPhone = data_reader.GetString(12);
                    arrDatosEmployeeIni.Add(vPhone);

                    String vFax = data_reader.GetString(13);
                    arrDatosEmployeeIni.Add(vFax);

                    String vEmail = data_reader.GetString(14);
                    arrDatosEmployeeIni.Add(vEmail);

                    arrDatosEmployeeList.Add(arrDatosEmployeeIni);

                }
            }
            this.CloseConnection();
            return arrDatosEmployeeList;
        }

    }
}
