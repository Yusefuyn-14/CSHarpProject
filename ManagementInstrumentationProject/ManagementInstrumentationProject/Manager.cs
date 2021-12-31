using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Management.Instrumentation;

namespace ManagementInstrumentationProject
{
    public class Manager
    {
        ClassName _className = new ClassName();
        public const string Namespace = @"root\cimv2";
        public List<string> getAllClassToList()
        {
            return _className.getAllClassNameToList();
        }
        public string[] getAllClassToArray()
        {
            return _className.getAllClassNameToArray();
        }
        public Concretes.WMIClass getClass(string Name)
        {
            Concretes.WMIClass returnedClass = new Concretes.WMIClass();
            List<Concretes.WMIClassElement> elementList = new List<Concretes.WMIClassElement>();
            returnedClass.Name = Name;
            ManagementClass ManagementClass1 = new ManagementClass(Name);
            try
            {
                ManagementObjectCollection ManagemenobjCol = ManagementClass1.GetInstances();
                PropertyDataCollection properties = ManagementClass1.Properties;

                foreach (ManagementObject obj in ManagemenobjCol)
                {
                    foreach (PropertyData property in properties)
                    {
                        Concretes.WMIClassElement element = new Concretes.WMIClassElement();
                        element.Name = property.Name;
                        try
                        {
                            element.Value = obj.Properties[property.Name].Value;
                        }
                        catch (Exception)
                        {

                        }
                        element.Type = property.Type;
                        elementList.Add(element);
                    }
                }

            }
            catch (Exception)
            {

            }
            returnedClass.elements = elementList;
            return returnedClass;
        }
        public List<Concretes.WMIClass> getClassPropAndValue() {
            List<Concretes.WMIClass> returnedClass = new List<Concretes.WMIClass>();
            List<string> allClassName = getAllClassToList();
            foreach (string item in allClassName)
                returnedClass.Add(getClass(item));
            return returnedClass;
        }
        public Task<List<Concretes.WMIClass>> getElementSearchValue(string Value,string[] searchClass)
        {
            var t = Task.Run(() =>
            {
                List<Concretes.WMIClass> returnedList = new List<Concretes.WMIClass>();
                foreach (string name in searchClass)
                {

                    Concretes.WMIClass classes = getClass(name);
                    if (classes.elements.Count <= 100)
                    {
                        foreach (Concretes.WMIClassElement item in classes.elements)
                        {
                            if (item.Value != null && item.Value.ToString() == Value)
                            {
                                Concretes.WMIClass newClass = new Concretes.WMIClass();
                                List<Concretes.WMIClassElement> elementList = new List<Concretes.WMIClassElement>();
                                newClass.Name = name;

                                Concretes.WMIClassElement elemnt = new Concretes.WMIClassElement();
                                elemnt.Name = item.Name;
                                string val = "null";
                                if (item.Value != null)
                                    val = item.Value.ToString();
                                elemnt.Value = val;
                                elemnt.Type = item.Type;
                                elementList.Add(elemnt);
                                newClass.elements = elementList;
                                returnedList.Add(newClass);
                            }
                        }
                    }
                }
                return returnedList;
            });
            return t;
        }
    }
}

