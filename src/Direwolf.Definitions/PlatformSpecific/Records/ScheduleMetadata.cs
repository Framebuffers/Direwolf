using System.Text.Json;
using Autodesk.Revit.DB;

namespace Direwolf.Definitions.PlatformSpecific.Records;

public readonly record struct ScheduleMetadata(
    string Id,
    ElementId ElementValueId,
    string ElementUniqueId,
    string? Name,
    ViewType ViewType,
    ElementId CategoryId,
    CategoryType CategoryType,
    string CategoryName,
    bool IsTemplate,
    bool IsPrintable,
    DateTime ExtractedAt,
    int Scale,
    string? Title)
{
    public static ScheduleMetadata Create(ViewSchedule schedule)
    {
        return new ScheduleMetadata(Cuid.Create().Value!, schedule.Id, schedule.UniqueId, schedule.Name,
            schedule.ViewType, schedule.Category.Id, schedule.Category.CategoryType, schedule.Category.Name,
            schedule.IsTemplate, schedule.CanBePrinted, DateTime.UtcNow, schedule.Scale,
            schedule.Title ?? schedule.Name);
    }
}

public readonly record struct RevitSchedule(Document doc, ScheduleMetadata Metadata, IDictionary<string, object> Data = null)
{

    public static RevitSchedule Create(ScheduleMetadata metadata, Document doc)
    {
        var scheduleData = new Dictionary<string, object>();
        scheduleData["metadata"] = GetTableInfo(metadata, doc)!;
        scheduleData["data"] = GetTableInfo(metadata, doc)!;
       scheduleData["fields"] = ExtractFieldDefinitions(metadata, doc)!;
       scheduleData["sortGroup"] = ExtractSortingAndGrouping(metadata, doc)!;
       scheduleData["filtering"] = ExtractFiltering(metadata, doc)!; 
        
        return new RevitSchedule(doc, metadata, scheduleData);

    }

    private static IDictionary<string, object>? GetTableInfo(ScheduleMetadata schedule, Document doc)
    {
        var table = doc.GetElement(schedule.ElementUniqueId) as ViewSchedule;
        if (table == null) return null;
        var tableData = table.GetTableData();
        var sectionData = tableData.GetSectionData(SectionType.Body);
        var tableInfo = new Dictionary<string, object>
        {
            ["rowCount"] = sectionData.NumberOfRows,
            ["columnCount"] = sectionData.NumberOfColumns,
            ["hasHeader"] = tableData.GetSectionData(SectionType.Header) != null,
            ["hasFooter"] = tableData.GetSectionData(SectionType.Footer) != null
        };
        var headerChars = new List<object>();
        var headerSection = tableData.GetSectionData(SectionType.Header);
        if (headerSection is not null && headerSection.NumberOfRows > 0)
        {
            for (int col = 0; col < headerSection.NumberOfColumns; col++)
            {
                try
                {
                    var headerCell = headerSection.GetCellText(headerSection.NumberOfRows - 1, col);
                    headerChars.Add(headerCell);
                }
                catch
                {
                    headerChars.Add($"Column_{col + 1}");
                }
            }
        }
        else
        {
            for (int col = 0; col < sectionData.NumberOfColumns; col++)
            {
                headerChars.Add($"Column_{col + 1}");
            }
        }

        var headers = headerChars.ToArray();
        tableInfo["headers"] = headers;
        var rows = new List<object>();
        for (int row = 0; row < sectionData.NumberOfRows; row++)
        {
            var rowData = new Dictionary<string, object>();
            var cellsData = new List<object>();
            for (int col = 0; col < sectionData.NumberOfColumns; col++)
            {
                try
                {
                    var cellText = sectionData.GetCellText(row, col);
                    var cellInfo = new Dictionary<string, object>
                    {
                        ["value"] = cellText, ["columnIndex"] = col, ["columnName"] = headers[col].ToString()
                    };
                    cellsData.Add(cellInfo);
                }
                catch (Exception e)
                {
                    cellsData.Add(new Dictionary<string, object>()
                    {
                        ["value"] = $"[Error: {e.Message}]",
                        ["columnIndex"] = col,
                        ["columnName"] = headers[col].ToString(),
                        ["error"] = true
                    });
                }
            }

            rowData["rowIndex"] = row;
            rowData["cells"] = cellsData;
            rows.Add(rowData);
        }

        tableInfo["rows"] = rows.ToArray();
        var footerSection = tableData.GetSectionData(SectionType.Footer);
        if (footerSection is not null && footerSection.NumberOfRows > 0)
        {
            var footerRows = new List<List<string>>();
            for (int row = 0; row < footerSection.NumberOfRows; row++)
            {
                var footerCells = new List<string>();
                for (int col = 0; col < footerSection.NumberOfColumns; col++)
                {
                    try
                    {
                        var cellText = footerSection.GetCellText(row, col);
                        footerCells.Add(cellText);
                    }
                    catch
                    {
                        footerCells.Add("");
                    }
                }

                footerRows.Add(footerCells);
            }
        }
        return tableInfo;
    }

    private static IEnumerable<IDictionary<string, object>>? ExtractFieldDefinitions(ScheduleMetadata schedule, Document doc)
    {
        var table = doc.GetElement(schedule.ElementUniqueId) as ViewSchedule;
                if (table == null)
                {
                    return null;
                }

        var definition = table.Definition;
        var fields = new List<Dictionary<string, object>>();
        for (int i = 0; i < definition.GetFieldCount(); i++)
        {
            var field = definition.GetField(i);
            var fieldInfo = new Dictionary<string, object>()
            {
                ["index"] = i,
                ["fieldId"] = field.FieldId.IntegerValue,
                ["parameterId"] = field.ParameterId?.IntegerValue,
                ["heading"] = field.GetName(),
                ["isHidden"] = field.IsHidden,
                ["width"] = field.SheetColumnWidth,
                ["alignment"] = field.HorizontalAlignment.ToString()
            };
            if (field.ParameterId != null && field.ParameterId != ElementId.InvalidElementId)
            {
                fieldInfo["parameterName"] = table.Name;
            }

            try
            {
                fieldInfo["fieldType"] = field.FieldType.ToString(); 
            }
            catch
            {
            }

            fields.Add(fieldInfo);
        }
        return fields;
    }

    private static IDictionary<string, object>? ExtractSortingAndGrouping(ScheduleMetadata schedule, Document doc)
    {
        var table = doc.GetElement(schedule.ElementUniqueId) as ViewSchedule;
        var definition = table.Definition;
        var sortingInfo = new Dictionary<string, object>();
        var sortFields = new List<Dictionary<string, object>>();
        for (int i = 0; i < definition.GetSortGroupFieldCount(); i++)
        {
            var sortField = definition.GetSortGroupField(i);
            var sortInfo = new Dictionary<string, object>()
            {
                ["index"] = i,
                ["fieldId"] = sortField.FieldId.IntegerValue,
                ["sortOrder"] = sortField.SortOrder.ToString(),
                ["showHeader"] = sortField.ShowHeader,
                ["showFooter"] = sortField.ShowFooter,
                ["showBlankLine"] = sortField.ShowBlankLine
            };
        sortFields.Add(sortInfo); 
        }

        sortingInfo["sortFields"] = sortFields;
        sortingInfo["sortFieldCount"] = definition.GetSortGroupFieldCount();
        return sortingInfo;
    }

    private static IDictionary<string, object>? ExtractFiltering(ScheduleMetadata schedule, Document doc)
    {
        var table = doc.GetElement(schedule.ElementUniqueId) as ViewSchedule;
        var definition = table.Definition;

        var filteringInfo = new Dictionary<string, object>();

        var filters = new List<Dictionary<string, object>>();
        for (int i = 0; i < definition.GetFilterCount(); i++)
        {
            var filter = definition.GetFilter(i);
            var filterInfo = new Dictionary<string, object>()
            {
                ["index"] = i,
                ["fieldId"] = filter.FieldId.IntegerValue,
                ["filterType"] = filter.FilterType.ToString(),
            };

            try
            {
                if (filter.FilterType == ScheduleFilterType.Equal ||
                    filter.FilterType == ScheduleFilterType.NotEqual ||
                    filter.FilterType == ScheduleFilterType.Contains ||
                    filter.FilterType == ScheduleFilterType.NotContains)
                {
                    filterInfo["stringValue"] = filter.GetStringValue();
                }
                else if (filter.FilterType == ScheduleFilterType.GreaterThan ||
                         filter.FilterType == ScheduleFilterType.GreaterThanOrEqual ||
                         filter.FilterType == ScheduleFilterType.LessThan ||
                         filter.FilterType == ScheduleFilterType.LessThanOrEqual)
                {
                    filterInfo["numericValue"] = filter.GetDoubleValue();
                }
            }
            catch
            {
                filterInfo["nullValue"] = filter.IsNullValue;
            }

            filters.Add(filterInfo);
        }
        
        filteringInfo["filters"] = filters;
        filteringInfo["filterCount"] = definition.GetFilterCount();
        return filteringInfo;
    }

}