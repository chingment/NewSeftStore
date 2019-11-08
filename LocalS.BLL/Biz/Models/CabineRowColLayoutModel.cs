using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class CabineRowColLayoutModel
    {
        public CabineRowColLayoutModel()
        {

        }

        public int Rows { get; set; }

        public int[] RowsCols { get; set; }

        public static CabineRowColLayoutModel Convert(string str)
        {
            var cabineRowColLayoutModel = new CabineRowColLayoutModel();

            if (string.IsNullOrEmpty(str))
            {
                cabineRowColLayoutModel.Rows = 0;
                cabineRowColLayoutModel.RowsCols = null;
                return cabineRowColLayoutModel;
            }

            try
            {
                string[] data = str.Split(',');
                if (data.Length > 0)
                {
                    cabineRowColLayoutModel.Rows = int.Parse(data[0]);

                    if (cabineRowColLayoutModel.Rows == data.Length - 1)
                    {
                        int[] rowCols = new int[data.Length - 1];

                        for (int i = 1; i < data.Length; i++)
                        {
                            rowCols[i - 1] = int.Parse(data[i]);
                        }

                        cabineRowColLayoutModel.RowsCols = rowCols;
                    }

                }
            }
            catch (Exception ex)
            {
                cabineRowColLayoutModel.Rows = 0;
                cabineRowColLayoutModel.RowsCols = null;
            }


            return cabineRowColLayoutModel;
        }
    }
}
