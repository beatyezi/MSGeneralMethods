using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSGeneralMethods
{
    public class MethodsClass
    {
        public static int CharToHex(byte ch)
        {
            if ((ch >= '0') && (ch <= '9'))
            {
                return Convert.ToInt32(ch - 0x30);
            }
            else
            {
                if ((ch >= 'A') && (ch <= 'F'))
                {
                    return Convert.ToInt32(ch - 'A' + 10);
                }
                else
                {
                    if ((ch >= 'a') && (ch <= 'f'))
                    {
                        return Convert.ToInt32(ch - 'a' + 10);
                    }
                    else
                    {
                        return Convert.ToInt32(-1);
                    }
                }
            }
        }

        public static int StringToHex(string strSource, byte[] byteTarget)
        {
            int iHexData = 0, iLowHexData = 0, iHexDataLength = 0, iLength = 0;

            iLength = strSource.Length;
            char[] arraySource = strSource.ToCharArray();

            for (int j = 0; j < iLength;)
            {
                byte lstr = 0, hstr = Convert.ToByte(arraySource[j]);
                if (hstr == ' ')
                {
                    j++;
                    continue;
                }
                j++;
                if (j > iLength)
                {
                    break;
                }
                lstr = Convert.ToByte(arraySource[j]);
                iHexData = CharToHex(hstr);
                iLowHexData = CharToHex(lstr);
                if ((16 == iHexData) && (16 == iLowHexData))
                {
                    break;
                }
                else
                {
                    iHexData = iHexData * 16 + iLowHexData;
                    j++;
                    byteTarget[iHexDataLength++] = Convert.ToByte(iHexData);

                }
            }
            return iHexDataLength;
        }
    }
}
