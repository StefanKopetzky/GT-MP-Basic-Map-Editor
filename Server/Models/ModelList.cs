using System.Collections.Generic;

namespace GT_MP_Basic_Map_Editor.Server.Models
{
    public class ModelList
    {
        public List<ModelCategory> Categories = new List<ModelCategory>();

        public ModelList()
        {
        }
    }

    public class ModelCategory
    {
        public string Name = "";
        public List<ModelInformation> Models = new List<ModelInformation>();

        public ModelCategory(string name)
        {
            Name = name;
        }

        public ModelCategory()
        {
        }
    }

    public class ModelInformation
    {
        public string Name = "No Name";
        public int ModelHash = 0;

        public ModelInformation()
        {
        }

        public ModelInformation(string name, int modelhash)
        {
            Name = name;
            ModelHash = modelhash;
        }
    }
}