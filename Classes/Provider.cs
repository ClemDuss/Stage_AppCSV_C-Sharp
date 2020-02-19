using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Classes
{
    public class Provider
    {
        string _name;
        List<int> _correspondences;
        string _file;
        string _id;
        bool _toExport;

        public Provider(string name, List<int> correspondances, string file, bool toExport)
        {
            Name = name;
            Correspondences = correspondances;
            File = file;
            ToExport = toExport;
        }

        public Provider()
        {
            Name = "";
            Correspondences = new List<int> { -1, -1, -1, -1};
            File = "";
            ToExport = true;
        }

        public string Name { get => _name; set => _name = value; }
        public List<int> Correspondences { get => _correspondences; set => _correspondences = value; }
        public string File { get => _file; set => _file = value; }
        public string Id { get => _id; set => _id = value; }
        public bool ToExport { get => _toExport; set => _toExport = value; }

        public string ToExportString
        {
            get
            {
                if (ToExport)
                {
                    return "✔";
                }
                else
                {
                    return "❌";
                }
            }
        }
        public string FileExists
        {
            get
            {
                if (System.IO.File.Exists(File))
                {
                    return "✔";
                }
                else
                {
                    return "❌";
                }
            }
        }

        public override string ToString()
        {
            string result = "";
            if (ToExport)
            {
                result += "À exporter\t| ";
            }
            else
            {
                result += "Pas d'export\t| ";
            }

            if (System.IO.File.Exists(File))
            {
                result += " ✔  |  " + Name;
            }
            else
            {
                result += " ❌  |  " + Name;
            }
            return result;
        }
    }
}
