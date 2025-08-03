using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;

namespace EtlSandbox.Infrastructure.Shared.Converters;

public static class DataTableConverter
{
    private static readonly ConcurrentDictionary<Type, List<PropertyDescriptor>> PropertyCache = new();

    public static DataTable ToDataTable<T>(IEnumerable<T> data)
    {
        var properties = GetProperties(typeof(T));
        var table = new DataTable();

        foreach (var prop in properties)
        {
            var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var column = new DataColumn(prop.Name, columnType);
            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
            {
                column.AllowDBNull = true;
            }
            table.Columns.Add(column);
        }

        foreach (var item in data)
        {
            var row = table.NewRow();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                if (value != null)
                {
                    row[prop.Name] = value;
                }
                else
                {
                    var propType = prop.PropertyType;
                    if (propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
                    {
                        row[prop.Name] = Activator.CreateInstance(propType);
                    }
                    else
                    {
                        row[prop.Name] = DBNull.Value;
                    }
                }
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static List<PropertyDescriptor> GetProperties(Type type)
    {
        return PropertyCache.GetOrAdd(type, t =>
            TypeDescriptor.GetProperties(t)
                .Cast<PropertyDescriptor>()
                .Where(prop => !prop.IsReadOnly)
                .ToList());
    }
}