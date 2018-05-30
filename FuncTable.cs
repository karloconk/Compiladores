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

    public struct GFuncStruct
    {
        public string p_or_u;
        public FunContainer refToFunc;
        public int arity;
        public GFuncStruct(string p_u,int arityy, FunContainer refe)
        {
            p_or_u    = p_u;
            arity     = arityy;
            refToFunc = refe;
        }
    }

    public class FuncTable: IEnumerable<KeyValuePair<string, GFuncStruct>> {

        IDictionary<string, GFuncStruct> data = new SortedDictionary<string, GFuncStruct>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Functions Table\n");
            sb.Append("====================\n");
            foreach (var entry in data) {
                sb.Append(entry.Key);
                GFuncStruct temp = entry.Value;
                sb.Append(String.Format(": [{0}, {1}, {2}]\n", 
                                        temp.p_or_u, 
                                        temp.arity,
                                        temp.refToFunc));
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public GFuncStruct this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }

        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
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
        public IEnumerator<KeyValuePair<string, GFuncStruct>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
