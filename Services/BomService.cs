

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Services
{
    public class BomService : IBomService
    {
        public List<string> GetPartNumber(string filePath, int lookForMnf)
        {
            List<string> mnfNumbers = new List<string>();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                string mnf = Convert.ToString(reader.GetValue(lookForMnf));

                                if (mnf != "Buh. Nr.")
                                {
                                    Console.WriteLine("Blogas pavadinimas arba stulpelio vieta \"Buh, Nr.\"!");
                                    reader.Close();
                                }
                                else if (mnf == "Buh. Nr.")
                                {
                                    do
                                    {
                                        if (mnf != "Buh. Nr.")
                                        {
                                            if (mnf != string.Empty)
                                            {
                                                mnfNumbers.Add(mnf);
                                            }
                                        }
                                        mnf = Convert.ToString(reader.GetValue(lookForMnf));
                                    } while (reader.Read());
                                }
                            }

                        } while (reader.NextResult());
                        var result = reader.AsDataSet();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Dokumentas yra atidarytas, uzdarykite ");
            }
            return mnfNumbers;
        }
        public List<string> GetNumber(string filePath, int lookForMnfQuantity)
        {
            List<string> mnfNumbers = new List<string>();
           /* try
            {*/
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                string mnf = Convert.ToString(reader.GetValue(lookForMnfQuantity));

                                if (mnf != "QTY")
                                {
                                    Console.WriteLine("Blogas pavadinimas arba stulpelio vieta \"QTY\"!");
                                    reader.Close();
                                }
                                else if (mnf == "QTY")
                                {
                                    do
                                    {
                                        if (mnf != "QTY")
                                        {
                                            if (mnf != string.Empty)
                                            {
                                                mnfNumbers.Add(mnf);
                                            }
                                        }
                                        mnf = Convert.ToString(reader.GetValue(lookForMnfQuantity));
                                    } while (reader.Read());
                                }
                            }

                        } while (reader.NextResult());
                        var result = reader.AsDataSet();
                    }
                }
            // }
            /* catch (Exception)
             {
                 Console.WriteLine("Dokumentas yra atidarytas, uzdarykite ");
             }
             */
            return mnfNumbers;
        }

        public List<BomList> GetDataFromFile(Stream stream)
        {  
            var empList = new List<BomList>();  
            try  
            {  
                using (var reader = ExcelReaderFactory.CreateReader(stream))  
                {  
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration  
                    {  
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration  
                        {  
                            UseHeaderRow = true // To set First Row As Column Names  
                        }  
                    });  
  
                    if (dataSet.Tables.Count > 0)  
                    {  
                        var dataTable = dataSet.Tables[0];  
                        foreach (DataRow objDataRow in dataTable.Rows)  
                        {  
                            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;  
                            empList.Add(new BomList()  
                            {  
                                
                                BuhNr = objDataRow["Buh. Nr."].ToString(),  
                                Qty = (int) objDataRow["QTY"]
                            });  
                        }  
                    }  
  
                }  
            }  
            catch (Exception)  
            {  
                throw;  
            }  
              
            return empList;  
        }

    }
}