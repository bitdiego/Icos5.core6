using System.Reflection;

namespace Icos5.core6.Models.StringOptions
{
    public class MyStringify<T>
    {
        private List<T> objList;
        public List<T> ObjList()
        {
            return objList;
        }
        public string MyString { get; set; } = "";
        public string MyTitle { get; set; } = "";
        public MyStringify(List<T> list)
        {
            objList = list;
        }

        public void MakeString()
        {
            int i = 0;
            if (objList == null || objList.Count == 0)
            {
                MyString = "";
                return;
            }/**/
            foreach (var v in objList)
            {
                Type myClassType = v.GetType();
                PropertyInfo[] properties = myClassType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //MyTitle += "prop name: " + property.Name + ", Value: " + property.GetValue(v, null).ToString();
                    if (i == 0)
                    {
                        MyTitle += property.Name + ",";
                    }
                    if (property.GetValue(v, null) == null) continue;
                    string _val = property.GetValue(v, null).ToString();
                    MyString += property.GetValue(v, null).ToString() + ",";
                }
                MyString = MyString.Substring(0, MyString.Length - 1);
                if (i == 0)
                {
                    MyTitle = MyTitle.Substring(0, MyTitle.Length - 1);
                    MyTitle += Environment.NewLine;
                }
                if (i < objList.Count - 1)
                {
                    MyString += Environment.NewLine;
                }
                ++i;
            }

        }

        public void MakeStringNewLine()
        {
            int i = 0;
            foreach (var v in objList)
            {
                Type myClassType = v.GetType();
                PropertyInfo[] properties = myClassType.GetProperties();
                int j = 0;
                foreach (PropertyInfo property in properties)
                {
                    // MyString += "prop name: " + property.Name + ", Value: " + property.GetValue(v, null).ToString();
                    MyString += property.GetValue(v, null).ToString();
                    if (j < properties.Length - 1) MyString += Environment.NewLine;
                    ++j;
                }


                if (i < objList.Count - 1)
                {
                    MyString += Environment.NewLine;
                }
                ++i;
            }

        }

        public void MakeString(string key)
        {
            int i = 0;
            foreach (var v in objList)
            {
                Type myClassType = v.GetType();
                PropertyInfo[] properties = myClassType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    // MyString += "prop name: " + property.Name + ", Value: " + property.GetValue(v, null).ToString();
                    if (String.Compare(key, property.Name, true) == 0)
                        MyString += property.GetValue(v, null).ToString() + ",";
                }
                MyString = MyString.Substring(0, MyString.Length - 1);

                if (i < objList.Count - 1)
                {
                    MyString += Environment.NewLine;
                }
                ++i;
            }
        }


        public void MakeByString()
        {
            int i = 0;
            foreach (var v in objList)
            {
                MyString += v;
                ++i;
                if (i < objList.Count)
                {
                    MyString += ",";
                }
            }

        }

        public void MakeByStringNewLine()
        {
            int i = 0;
            foreach (var v in objList)
            {
                MyString += v;

                if (i < objList.Count - 1)
                {
                    MyString += Environment.NewLine;
                }
                ++i;
            }

        }

    }
}
