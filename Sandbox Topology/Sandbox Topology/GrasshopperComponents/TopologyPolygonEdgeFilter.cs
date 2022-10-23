using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Sandbox.Topology.Properties;
using System;
using System.Collections.Generic;


namespace Sandbox.Topology.GrasshopperComponents
{
    public class TopologyPolygonEdgeFilter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public TopologyPolygonEdgeFilter()
            : base("Polygon Topology Edge Filter", "Poly Topo Edge Filter",
                  "Filter the edges in a polygon network based on their valency",
                  "Sandbox", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Edge list", "E", "Ordered list of edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Edge-Loop structure", "EL", "Ordered structure listing the polylines adjacent to each edge", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Valency filter", "V", "Filter edges with the specified number of adjacent polylines", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("List of edge IDs", "I", "List of edge indices matching the valency criteria", GH_ParamAccess.list);
            pManager.AddLineParameter("List of edges", "E", "List of edges matching the valency criteria", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 1. Declare placeholder variables and assign initial invalid data.
            // This way, if the input parameters fail to supply valid data, we know when to abort.
            List<GH_Line> _E = new List<GH_Line>();
            GH_Structure<GH_Integer> _EL = null/* TODO Change to default(_) if this is not a reference type */;
            Int32 _C = 0;

            // 2. Retrieve input data.
            if ((!DA.GetDataList(0, _E)))
                return;
            if ((!DA.GetDataTree(1, out _EL)))
                return;
            if ((!DA.GetData(2, ref _C)))
                return;

            // 3. Abort on invalid inputs.
            // 3.1. get the number of branches in the trees
            if (!(_E.Count > 0))
                return;
            if (!(_EL.PathCount > 0))
                return;
            if (!(_C > 0))
                return;

            // 4. Do something useful.

            List<Int32> _idList = new List<Int32>();
            string arrayStrings = "{}";
            char[] charArrayList = arrayStrings.ToCharArray();
            foreach (GH_Path _path in _EL.Paths)
            {
                if (_EL[_path].Count == _C)
                    _idList.Add(int.Parse(_path.ToString().Trim(charArrayList)));
            }

            List<Line> _eList = new List<Line>();
            foreach (Int32 _id in _idList)

                _eList.Add(_E[_id].Value);

            DA.SetDataList(0, _idList);
            DA.SetDataList(1, _eList);
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
                return Resources.TopologyPolyEdgeFilter;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F0B0CCB7-33A9-4909-B009-02BAF41EB1C6"); }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
    }
}