using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineGenerating
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = @"D:\Example.cshtml";

            using (StreamWriter sr = File.AppendText(path))
            {
                sr.WriteLine("demo - ASP.Net");
                sr.Close();

                Console.WriteLine(File.ReadAllText(path));
            }
            GenerateRazor(new List<FieldInfo>()
            {
                new FieldInfo()
                {
                    FieldName = "r1f1",
                    Caption= "RowField 1"
                },
                new FieldInfo()
                {
                    FieldName = "r1f2",
                    Caption= "RowField 2"
                },
                new FieldInfo()
                {
                    FieldName = "r1f3",
                    Caption= "RowField 3"
                },
            }, new List<FieldInfo>()
            {
                new FieldInfo()
                {
                    FieldName = "c1f1",
                    Caption= "ColField 1"
                },
                new FieldInfo()
                {
                    FieldName = "c1f2",
                    Caption= "ColField 2"
                },
                new FieldInfo()
                {
                    FieldName = "c1f3",
                    Caption= "ColField 3"
                },
            }, new List<FieldInfo>()
            {
                new FieldInfo()
                {
                    FieldName = "d1f1",
                    Caption= "datField 1"
                },
                new FieldInfo()
                {
                    FieldName = "d1f2",
                    Caption= "datField 2"
                },
                new FieldInfo()
                {
                    FieldName = "d1f3",
                    Caption= "datField 3"
                },
            }, "data");
            Console.ReadKey();
        }

        public static string GenerateRazor(List<FieldInfo> rows, List<FieldInfo> cols, List<FieldInfo> data, string table_name)
        {

            string header = @"@using System;
@using System.Collections.Generic;
@using System.Linq;
@{
	Dictionary<string,object> a = Model;
    IEnumerable<dynamic> table = a[" + table_name + @"]
}
";
            // get distinct of all the table 
            string table_header = "";
            string loop = "";
            string parent = "table";
            int header_depth = 0;
            IEnumerable<dynamic> datatable = header as dynamic;
            
            
            List<string> SeenCols = new List<string>();
            Func<dynamic,dynamic> multi_selector = (x) => 
            {
                string key = "";
                foreach(string col in SeenCols)
                {
                    key += x[col] + "-_-";
                }
                return key;
            };
            foreach(var col in cols)
            {
                SeenCols.Append(col.FieldName);
                //<tr>
                var elements = datatable.GroupBy(multi_selector).Select(x=>x.First()).ToList();
                //<td><p>
                foreach(var element in elements)
                {
                //@(element[col.FieldName]);
                }
                //</p></td>
                
                //</tr>
            }

            
            /*foreach (var group_field in cols)
            {
                header_depth += 1;
                string group_name = $"{group_field.Caption.Safe()}_group";
                table_header += "\r\n" + new String('\t', header_depth) + $"@foreach(var {group_name} in {parent}.GroupBy(x=>x[\"{group_field.FieldName}\"])) {{";
                table_header += "\r\n<tr>";

                table_header += "\r\n" + new String('\t', header_depth) + $"@foreach(var {group_name} in {parent}.GroupBy(x=>x[\"{group_field.FieldName}\"])) {{";
                table_header+="\r\n</tr>";
                
                parent = group_name;
            }*/
            int depth = 0;
            int col_depth = 0;
            foreach (var group_field in rows)
            {
                depth += 1;
                string group_name = $"{group_field.Caption.Safe()}_group";
                loop += "\r\n" + new String('\t', depth) + $"@foreach(var {group_name} in {parent}.GroupBy(x=>x[\"{group_field.FieldName}\"])) {{";
                parent = group_name;
            }
            loop += "\r\n<tr>\r\n";
            foreach (var group_field in cols)
            {
                col_depth += 1;
                string group_name = $"{group_field.Caption.Safe()}_group";
                loop += "\r\n" + new String('\t', depth) + $"@foreach(var {group_name} in {parent}.GroupBy(x=>x[\"{group_field.FieldName}\"])) {{";
                parent = group_name;
            }
            foreach (var data_field in data)
            {
                loop += "\r\n<td><p>\r\n";
                loop += $"@({parent}.First()[\"{data_field.FieldName}\"])";
                loop += "\r\n</p></td>\r\n";
            }


            while (col_depth > 0)
            {
                loop += "\r\n}";
                col_depth -= 1;
            }
            loop += "\r\n</tr>\r\n";
            while (depth > 0)
            {
                loop += "\r\n}";
                depth -= 1;
            }

            return header + loop;
        }
    }
    static class Extensions
    {
        private static string unsafe_chars = " `!@#$%^&*()=+-[]{}\\|;:.,<>/?\r\t";

        public static string Safe(this string Name)
        {
            string SafeName = Name;
            foreach (char c in unsafe_chars)
            {
                SafeName = SafeName.Replace(c, '_');
            }
            return (SafeName);
        }
    }
    class FieldInfo
    {
        public string Caption { get; set; }
        public string FieldName { get; set; }
        public List<FieldInfo> Summaries { get; set; }
    }
}
