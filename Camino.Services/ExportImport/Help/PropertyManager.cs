using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.ExportImport.Help
{
    /// <summary>
    /// Class for working with PropertyByName object list
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class PropertyManager<T>
    {
        /// <summary>
        /// All properties
        /// </summary>
        private readonly Dictionary<string, PropertyByName<T>> _properties;

        /// <summary>
        /// Add new property
        /// </summary>
        /// <param name="property">Property to add</param>
        public void AddProperty(PropertyByName<T> property)
        {
            if (_properties.ContainsKey(property.PropertyName))
                return;

            _properties.Add(property.PropertyName, property);
        }

        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        public virtual byte[] ExportToXlsx(IEnumerable<T> itemsToExport)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                    fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                    //create Headers and format them 
                    WriteCaption(worksheet);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        CurrentObject = items;
                        WriteToXlsx(worksheet, row++, fWorksheet: fWorksheet);
                    }

                    xlPackage.Save();
                }

                CurrentObject = default(T);
                return stream.ToArray();
            }
        }

        public virtual void WriteToXlsx(ExcelWorksheet worksheet, int row, int cellOffset = 0, ExcelWorksheet fWorksheet = null)
        {
            if (CurrentObject == null)
                return;

            foreach (var prop in _properties.Values)
            {
                var cell = worksheet.Cells[row, prop.PropertyOrderPosition + cellOffset];
                if (prop.IsDropDownCell)
                {
                    var dropDownElements = prop.GetDropDownElements();
                    if (!dropDownElements.Any())
                    {
                        cell.Value = string.Empty;
                        continue;
                    }

                    cell.Value = prop.GetItemText(prop.GetProperty(CurrentObject));

                    var validator = cell.DataValidation.AddListDataValidation();

                    validator.AllowBlank = prop.AllowBlank;

                    if (fWorksheet == null)
                        continue;

                    var fRow = 1;
                    foreach (var dropDownElement in dropDownElements)
                    {
                        var fCell = fWorksheet.Cells[fRow++, prop.PropertyOrderPosition];

                        if (fCell.Value != null && fCell.Value.ToString() == dropDownElement)
                            break;

                        fCell.Value = dropDownElement;
                    }

                    validator.Formula.ExcelFormula = $"{fWorksheet.Name}!{fWorksheet.Cells[1, prop.PropertyOrderPosition].Address}:{fWorksheet.Cells[dropDownElements.Length, prop.PropertyOrderPosition].Address}";
                }
                else
                {
                    cell.Value = prop.GetProperty(CurrentObject);
                }
            }
        }

        /// <summary>
        /// Current object to access
        /// </summary>
        /// 
        public T CurrentObject { get; set; }
        /// <summary>
        /// Write caption (first row) to XLSX worksheet
        /// </summary>
        /// <param name="worksheet">worksheet</param>
        /// <param name="row">Row number</param>
        /// <param name="cellOffset">Cell offset</param>
        public virtual void WriteCaption(ExcelWorksheet worksheet, int row = 1, int cellOffset = 0)
        {
            foreach (var caption in _properties.Values)
            {
                var cell = worksheet.Cells[row, caption.PropertyOrderPosition + cellOffset];
                cell.Value = caption;

                SetCaptionStyle(cell);
                cell.Style.Hidden = false;
            }
        }

        /// <summary>
        /// Set caption style to excel cell
        /// </summary>
        /// <param name="cell">Excel cell</param>
        public void SetCaptionStyle(ExcelRange cell)
        {
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            cell.Style.Font.Bold = true;
        }

        public PropertyManager(IEnumerable<PropertyByName<T>> properties)
        {
            _properties = new Dictionary<string, PropertyByName<T>>();
            var poz = 1;
            foreach (var propertyByName in properties.Where(p => !p.Ignore))
            {
                propertyByName.PropertyOrderPosition = poz;
                poz++;
                _properties.Add(propertyByName.PropertyName, propertyByName);
            }
        }
    }
}
