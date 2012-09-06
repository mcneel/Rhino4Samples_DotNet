///<summary>
/// The following class is required for all Rhino.NET plug-ins.
/// These are used to display plug-in information in the plug-in manager.
/// Any string will work for these attributes, so if you don't have a fax
/// number it is OK to enter something like "none"
///</summary>
namespace SampleCsObjectProperties
{
  public class SampleCsObjectPropertiesPlugInAttributes : RMA.Rhino.MRhinoPlugInAttributes
  {
    public override string Address()
    {
      return "3670 Woodland Park Avenue North\nSeattle\nWA\n98115";
    }
    public override string Country()
    {
      return "United States";
    }
    public override string Email()
    {
      return "devsupport@mcneel.com";
    }
    public override string Fax()
    {
      return "206-545-7321";
    }
    public override string Organization()
    {
      return "Robert McNeel & Associates";
    }
    public override string Phone()
    {
      return "206-545-6877";
    }
    public override string UpdateURL()
    {
      return "https://github.com/mcneel/";
    }
    public override string Website()
    {
      return "http://www.rhino3d.com";
    }
  }
}
