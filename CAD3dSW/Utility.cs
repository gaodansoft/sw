using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAD3dSW
{
    public class Utility
    {
        static public System.Text.StringBuilder ModelErrorList { get; set; }

        //逗号分隔键值字符串分析：'abc','12','cd','12'
        static public bool ParseQuotesVal(string str, Dictionary<string, string> dic)
        {
            List<string> ls = new List<string>();

            int[] quote = { 1, 2,	3,	2};
            int[] comma = { -1, 3, 0, 3};
            int[] other = { -1, 3, -1, 3};

            string tmp = string.Empty;	        
            int n = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\'')
                {
                    n = quote[n];
                }
                else if (str[i] == ',')
                {
                    n = comma[n];
                    if (n == 0)
                    {
                        ls.Add(tmp);
                        tmp = "";
                    }
                }
                else
                {
                    n = other[n];
                }

                if (n < 0)
                {
                    return false;
                }

                if (n == 3)
                {
                    tmp += str[i];
                }
            }

            if ( 2 == n)
            {
                ls.Add(tmp);                
            }
           
            for (int i = 0; i < ls.Count; i += 2)
            {
                string val;
                if (!dic.TryGetValue(ls[i], out val))
                {
                    dic.Add(ls[i], ls[i + 1]);
                }
                else
                {
                    dic[ls[i]] = ls[i + 1];
                }
            }

            return n == 0;

        }



    }
}
