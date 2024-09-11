using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SAP_Batch_GR_TR.Class
{
    class CheckJson
    {
        //เช็ค data to json
        public static string ConvertObjectArrayToString(object[] objects)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Object Array:");

            foreach (var obj in objects)
            {
                sb.AppendLine(InspectObject(obj));
            }

            return sb.ToString();
        }

        public static string InspectObject(object obj, int indentLevel = 0)
        {
            if (obj == null)
            {
                return "null";
            }

            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var sb = new StringBuilder();

            string indent = new string(' ', indentLevel * 2);
            sb.AppendLine($"{indent}Type: {type.Name}");

            sb.AppendLine($"{indent}Properties:");
            foreach (var prop in properties)
            {
                object value = prop.GetValue(obj);
                if (value != null && !IsSimpleType(value.GetType()))
                {
                    sb.AppendLine($"{indent}  {prop.Name}:");
                    sb.Append(InspectObject(value, indentLevel + 2));
                }
                else
                {
                    sb.AppendLine($"{indent}  {prop.Name}: {value}");
                }
            }

            sb.AppendLine($"{indent}Fields:");
            foreach (var field in fields)
            {
                object value = field.GetValue(obj);
                if (value != null && !IsSimpleType(value.GetType()))
                {
                    sb.AppendLine($"{indent}  {field.Name}:");
                    sb.Append(InspectObject(value, indentLevel + 2));
                }
                else
                {
                    sb.AppendLine($"{indent}  {field.Name}: {value}");
                }
            }

            return sb.ToString();
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        //end เช็ค data to json

        //string resulta = ConvertObjectArrayToString(ws_res.eMaterailDoc);
        //Console.WriteLine("resultsap: "+ resulta);

    }
}
