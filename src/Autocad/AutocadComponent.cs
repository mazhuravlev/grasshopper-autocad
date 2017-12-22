using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Autocad
{
  public class AutocadComponent : GH_Component
  {
    private int _scriptIn;
    private int _fileIn;
    private int _runIn;
    private int _pathIn;

    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public AutocadComponent()
      : base("Autocad", "Nickname",
          "Description",
          "Category", "Subcategory")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      _pathIn = pManager.AddTextParameter("Autocad path", "A", "accoreconsole.exe path", GH_ParamAccess.item);
      _scriptIn = pManager.AddTextParameter("Script", "S", "Autocad script", GH_ParamAccess.list);
      _fileIn = pManager.AddTextParameter("Out file", "O", "Output DWG", GH_ParamAccess.item);
      _runIn = pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item, false);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var autocadPath = "";
      da.GetData(_pathIn, ref autocadPath);
      if (!System.IO.File.Exists(autocadPath))
      {
        throw new Exception("Autocad console executable not found");
      }
      var run = false;
      da.GetData(_runIn, ref run);
      if (!run) return;
      var script = new List<string>();
      da.GetDataList(_scriptIn, script);
      var outFilePath = "";
      da.GetData(_fileIn, ref outFilePath);
      if(System.IO.File.Exists(outFilePath)) System.IO.File.Delete(outFilePath);
      var scriptFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".scr");
      System.IO.File.WriteAllText(scriptFilePath, string.Join("\n", script) + $"_saveas\n\n{outFilePath}\nexit\n");
      var processInfo = new ProcessStartInfo
      {
        FileName = autocadPath,
        Arguments = $"/s {scriptFilePath}"
      };
      var procces = Process.Start(processInfo);
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        // You can add image files to your project resources and access them like this:
        //return Resources.IconForThisComponent;
        return null;
      }
    }

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("ac31ff29-7187-40bf-9864-8681e9e619b3"); }
    }
  }
}
