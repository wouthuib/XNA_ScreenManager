using System.Text;
using System.Xml;
using XNA_ScreenManager.ScreenClasses.MainClasses;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace XNA_ScreenManager.Networking
{
    public class ServerProperties
    {
        
        public static string xmlgetvalue(string name)
        {
            XDocument xdoc = XDocument.Load(ResourceManager.GetInstance.Content.RootDirectory + @"\tables\server.xml");
            foreach (XElement currentElement in xdoc.Root.Elements())
            {
                if (currentElement.Name == name)
                {
                    return currentElement.Value.ToString();
                }
            }
            return null;
        }
    }
}
