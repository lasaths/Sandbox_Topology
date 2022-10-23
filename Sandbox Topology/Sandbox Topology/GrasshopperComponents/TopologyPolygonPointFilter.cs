using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Sandbox.Topology.Properties;

namespace Sandbox.Topology.GrasshopperComponents
{
    public class TopologyPolygonPointFilter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public TopologyPolygonPointFilter() 
            : base("Polygon Topology Point Filter", "Poly Topo Point Filter", 
                  "Filter the points in a polygon network based on their connectivity",
                  "Sandbox", "Topology")

        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point list", "P", "Ordered list of points", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Point-Loop structure", "PL", "Ordered structure listing the polylines adjacent to each point", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Valency filter", "V", "Filter points with the specified number of adjacent polylines", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("List of point IDs", "I", "List of point indices matching the valency criteria", GH_ParamAccess.list);
            pManager.AddPointParameter("List of points", "P", "List of points matching the valency criteria", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 1. Declare placeholder variables and assign initial invalid data.
            // This way, if the input parameters fail to supply valid data, we know when to abort.
            List<GH_Point> _P = new List<GH_Point>();
            GH_Structure<GH_Integer> _PF = null/* TODO Change to default(_) if this is not a reference type */;
            Int32 _C = 0;

            // 2. Retrieve input data.
            if ((!DA.GetDataList(0, _P)))
                return;
            if ((!DA.GetDataTree(1,out _PF)))
                return;
            if ((!DA.GetData(2,ref _C)))
                return;

            // 3. Abort on invalid inputs.
            // 3.1. get the number of branches in the trees
            if (!(_P.Count > 0))
                return;
            if (!(_PF.PathCount > 0))
                return;
            if (!(_C > 0))
                return;

            // 4. Do something useful.
            // Dim _ptList As List(Of Point3d) = _P
            GH_Structure<GH_Integer> _pfTree = _PF;

            List<Int32> _idList = new List<Int32>();
            string arrayStrings = "{}";
            char[] charArrayList = arrayStrings.ToCharArray();
            foreach (GH_Path _path in _pfTree.Paths)
            {
                if (_pfTree[_path].Count == _C)
                    _idList.Add(int.Parse(_path.ToString().Trim(charArrayList)));
            }

            List<Point3d> _ptList = new List<Point3d>();
            foreach (Int32 _id in _idList)

                _ptList.Add(_P[_id].Value);

            DA.SetDataList(0, _idList);
            DA.SetDataList(1, _ptList);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.TopologyPolyPointFilter;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A0D6C219-0A46-41EF-A717-E6D69CF808C5"); }
        }
    }
}