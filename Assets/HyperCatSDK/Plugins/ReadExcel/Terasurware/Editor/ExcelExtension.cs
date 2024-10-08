using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
public static class ExcelExtension
{
    public static T TryGetCell<T>(this ICell cell)
    {
        if (cell == null)
        {
            return default(T);
        }
        System.Type serviceInterface = typeof(T);
        //  Debug.Log(serviceInterface.ToString());
        //STRING:
        if (serviceInterface.Equals(typeof(string)))
        {
            //return (T)(object)new MemberService();
            try
            {
                // return (T)(object)cell.StringCellValue;
                return (T)System.Convert.ChangeType(cell.StringCellValue, typeof(T));
            }
            catch (System.Exception ex)
            {
                // Debug.Log(ex.ToString());
                Debug.Log("<color=red>CELL:</color> " + cell.ToString() + "\n <color=red> Ex:</color> " + ex.ToString() + "\n <color=red> set: </color> " + default(T));
                return default(T);

            }
        }
        else
        //BOOL:
        if (serviceInterface.Equals(typeof(bool)))
        {
            //return (T)(object)new MemberService();
            try
            {
                // return (T)(object)cell.BooleanCellValue;
                return (T)System.Convert.ChangeType(cell.BooleanCellValue, typeof(T));
            }
            catch (System.Exception ex)
            {
                // Debug.Log(ex.ToString());
                Debug.Log("<color=red>CELL:</color> " + cell + "\n <color=red> Ex:</color> " + ex.ToString() + "\n <color=red> set: </color> " + default(T));
                return default(T);
            }
        }
        else
        //ELSE:

        {
            //return (T)(object)new MemberService();
            try
            {
                return (T)System.Convert.ChangeType(cell.NumericCellValue, typeof(T));
            }
            catch (System.Exception ex)
            {
                Debug.Log("<color=red>CELL:</color> " + cell + "\n <color=red> Ex:</color> " + ex.ToString() + "\n <color=red> set: </color> " + default(T));
                return default(T);
            }
        }
        //  return default(T);
        //    ICell cell = null;
        //  return (T)outPut;
    }

    public static int GetSheetLength(this ISheet sheet)
    {
        //  Debug.Log("GEt Sheet Length__________");
        for (int i = 0; i < sheet.LastRowNum; i++)
        {

            // sheet.PhysicalNumberOfRows
            IRow row = sheet.GetRow(i);
            //    Debug.Log(i + " " + row.GetCell(0).ToString());
            if (row == null || row.Cells.Count <= 0)
            {

                return i - 1;
            }
            else
            {
                bool isNull = true;
                foreach (ICell cell in row)
                {
                    //  Debug.Log(cell.ToString());
                    if (cell != null && !string.IsNullOrEmpty(cell.ToString()))
                    {
                        //Debug.Log(cell.ToString());
                        isNull = false;
                        break;
                    }
                }
                if (isNull)
                    return i - 1;
            }
        }
        return sheet.LastRowNum;

    }

}
