Imports Microsoft.Office.Interop.Excel


Public Class zx

    Public Function excelchange() As Boolean


        Dim ExcelSheet As Microsoft.Office.Interop.Excel.Application

        'Set ExcelSheet = GetObject(, "Excel.Application")
        ExcelSheet = GetObject(, "Excel.Application")
        ExcelSheet.ActiveCell.Value = "hying"


    End Function


End Class
