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

    public class FunContainer
    {
            public ParticularFunc ParticularFunction 
            {
                get;
                private set;
            }

            public FunContainer()
            {
                ParticularFunction  = new ParticularFunc();
            }

    }

    public class FuncMethods: IEnumerable<KeyValuePair<string, FunContainer>> {

        IDictionary<string, FunContainer> data = new SortedDictionary<string, FunContainer>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            //sb.Append("Local Functions tables \n");
            //sb.Append("+++++++++++++++++++++++++++++++\n");
            foreach (var entry in data) {
                sb.Append("\n");
                sb.Append(entry.Key);
                sb.Append(" Table\n====================\n");
                FunContainer temp1 = entry.Value;
                ParticularFunc temp = temp1.ParticularFunction;
                sb.Append(temp.ToString());
            }
            //sb.Append("+++++++++++++++++++++++++++++++\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public FunContainer this[string key] {
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
        public IEnumerator<KeyValuePair<string, FunContainer>> GetEnumerator() {
            return data.GetEnumerator();
        }

        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }

}