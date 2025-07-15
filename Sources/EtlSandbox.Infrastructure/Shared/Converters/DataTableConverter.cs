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
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in data)
        {
            var row = table.NewRow();
            foreach (var prop in properties)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
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