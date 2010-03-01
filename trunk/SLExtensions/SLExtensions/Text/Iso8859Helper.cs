using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace SLExtensions.Text
{
    public static class Iso8859Helper
    {
        public static string Convert(Stream stream)
        {
            if (stream == null)
                return null;

            StringBuilder sb = new StringBuilder((int)stream.Length);
            for (long i = stream.Position; i < stream.Length; i++)
            {
                int b = stream.ReadByte();
                if (b == -1)
                    break;

                char c = chars[b];
                if (b != 0 && c == '\0')
                    continue;

                sb.Append(c);
            }
            return sb.ToString();
        }

        private static char[] chars;

        static Iso8859Helper()
        {
            chars = new char[256];            
            chars[10] = '\n';
            chars[13] = '\r';
            chars[32] = ' ';
            chars[33] = '!';
            chars[34] = '"';
            chars[35] = '#';
            chars[36] = '$';
            chars[37] = '%';
            chars[38] = '&';
            chars[39] = '\'';
            chars[40] = '(';
            chars[41] = ')';
            chars[42] = '*';
            chars[44] = ',';
            chars[46] = '.';
            chars[48] = '0';
            chars[50] = '2';
            chars[52] = '4';
            chars[54] = '6';
            chars[56] = '8';
            chars[58] = ':';
            chars[60] = '<';
            chars[62] = '>';
            chars[64] = '@';
            chars[66] = 'B';
            chars[68] = 'D';
            chars[70] = 'F';
            chars[72] = 'H';
            chars[74] = 'J';
            chars[76] = 'L';
            chars[78] = 'N';
            chars[80] = 'P';
            chars[82] = 'R';
            chars[84] = 'T';
            chars[86] = 'V';
            chars[88] = 'X';
            chars[90] = 'Z';
            chars[92] = '\\';
            chars[94] = '^';
            chars[96] = '`';
            chars[98] = 'b';
            chars[100] = 'd';
            chars[102] = 'f';
            chars[104] = 'h';
            chars[106] = 'j';
            chars[108] = 'l';
            chars[110] = 'n';
            chars[112] = 'p';
            chars[114] = 'r';
            chars[116] = 't';
            chars[118] = 'v';
            chars[120] = 'x';
            chars[122] = 'z';
            chars[124] = '|';
            chars[126] = '~';
            chars[146] = '\'';
            chars[160] = ' ';
            chars[162] = '¢';
            chars[164] = '¤';
            chars[166] = '¦';
            chars[168] = '¨';
            chars[170] = 'ª';
            chars[172] = '¬';
            chars[174] = '®';
            chars[176] = '°';
            chars[178] = '²';
            chars[180] = '´';
            chars[182] = '¶';
            chars[184] = '¸';
            chars[186] = 'º';
            chars[188] = '¼';
            chars[190] = '¾';
            chars[192] = 'À';
            chars[194] = 'Â';
            chars[196] = 'Ä';
            chars[198] = 'Æ';
            chars[200] = 'È';
            chars[202] = 'Ê';
            chars[204] = 'Ì';
            chars[206] = 'Î';
            chars[208] = 'Ð';
            chars[210] = 'Ò';
            chars[212] = 'Ô';
            chars[214] = 'Ö';
            chars[216] = 'Ø';
            chars[218] = 'Ú';
            chars[220] = 'Ü';
            chars[222] = 'Þ';
            chars[224] = 'à';
            chars[226] = 'â';
            chars[228] = 'ä';
            chars[230] = 'æ';
            chars[232] = 'è';
            chars[234] = 'ê';
            chars[236] = 'ì';
            chars[238] = 'î';
            chars[240] = 'ð';
            chars[242] = 'ò';
            chars[244] = 'ô';
            chars[246] = 'ö';
            chars[248] = 'ø';
            chars[250] = 'ú';
            chars[252] = 'ü';
            chars[254] = 'þ';

            chars[35] = '#';
            chars[37] = '%';
            chars[39] = '\'';
            chars[41] = ')';
            chars[43] = '+';
            chars[45] = '-';
            chars[47] = '/';
            chars[49] = '1';
            chars[51] = '3';
            chars[53] = '5';
            chars[55] = '7';
            chars[57] = '9';
            chars[59] = ';';
            chars[61] = '=';
            chars[63] = '?';
            chars[65] = 'A';
            chars[67] = 'C';
            chars[69] = 'E';
            chars[71] = 'G';
            chars[73] = 'I';
            chars[75] = 'K';
            chars[77] = 'M';
            chars[79] = 'O';
            chars[81] = 'Q';
            chars[83] = 'S';
            chars[85] = 'U';
            chars[87] = 'W';
            chars[89] = 'Y';
            chars[91] = '[';
            chars[93] = ']';
            chars[95] = '_';
            chars[97] = 'a';
            chars[99] = 'c';
            chars[101] = 'e';
            chars[103] = 'g';
            chars[105] = 'i';
            chars[107] = 'k';
            chars[109] = 'm';
            chars[111] = 'o';
            chars[113] = 'q';
            chars[115] = 's';
            chars[117] = 'u';
            chars[119] = 'w';
            chars[121] = 'y';
            chars[123] = '{';
            chars[125] = '}';
            chars[161] = '¡';
            chars[163] = '£';
            chars[165] = '¥';
            chars[167] = '§';
            chars[169] = '©';
            chars[171] = '«';
            chars[173] = '­';
            chars[175] = '¯';
            chars[177] = '±';
            chars[179] = '³';
            chars[181] = 'µ';
            chars[183] = '·';
            chars[185] = '¹';
            chars[187] = '»';
            chars[189] = '½';
            chars[191] = '¿';
            chars[193] = 'Á';
            chars[195] = 'Ã';
            chars[197] = 'Å';
            chars[199] = 'Ç';
            chars[201] = 'É';
            chars[203] = 'Ë';
            chars[205] = 'Í';
            chars[207] = 'Ï';
            chars[209] = 'Ñ';
            chars[211] = 'Ó';
            chars[213] = 'Õ';
            chars[215] = '×';
            chars[217] = 'Ù';
            chars[219] = 'Û';
            chars[221] = 'Ý';
            chars[223] = 'ß';
            chars[225] = 'á';
            chars[227] = 'ã';
            chars[229] = 'å';
            chars[231] = 'ç';
            chars[233] = 'é';
            chars[235] = 'ë';
            chars[237] = 'í';
            chars[239] = 'ï';
            chars[241] = 'ñ';
            chars[243] = 'ó';
            chars[245] = 'õ';
            chars[247] = '÷';
            chars[249] = 'ù';
            chars[251] = 'û';
            chars[253] = 'ý';
            chars[255] = 'ÿ';
        }




    }
}
