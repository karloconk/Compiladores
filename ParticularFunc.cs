/* Date: 16-APR-2018
* Authors:
*          A01374526 José Karlo Hurtado Corona
*          A01373890 Gabriela Aguilar Lugo
*          A01375996 Alan Joseph Salazar Romero
*
*          To see special comments regarding this software please refer to the README included.
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace DeepLingo {
    
    public struct DataOfFunc
    {
        public string locale;
        public int paramPos;
        public DataOfFunc(string local,int pos)
        {
            locale    = local;
            paramPos  = pos;
        }
    }

    public class ParticularFunc: IEnumerable<KeyValuePair<string, DataOfFunc>> {

        IDictionary<string, DataOfFunc> data = new SortedDictionary<string, DataOfFunc>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var entry in data) {
                sb.Append(entry.Key);
                DataOfFunc temp = entry.Value;
                sb.Append(String.Format(": [{0}, {1}]\n", 
                                        temp.locale, 
                                        temp.paramPos));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public DataOfFunc this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public List<DataOfFunc> getvalues() {
            List<DataOfFunc> listOne = new List<DataOfFunc>();
            foreach (var entry in data) 
            {
                listOne.Add(entry.Value);
            }
            return listOne;
        }

        //-----------------------------------------------------------
        public List<string> getkeys() {
            List<string> listOne = new List<string>();
            foreach (var entry in data) 
            {
                listOne.Add(entry.Key);
            }
            return listOne;
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }

        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, DataOfFunc>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
