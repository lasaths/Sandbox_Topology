using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Sandbox_Topology
{
    public class Sandbox_TopologyInfo : GH_AssemblyInfo
    {
        public override string Name => "Sandbox Topology";

        public override GH_LibraryLicense License
        {
            get { return GH_LibraryLicense.opensource; }
        }

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Tools for experiments in computational architecture";

        public override Guid Id => new Guid("E3C1F0E3-B3EA-4ABD-B68E-770A38932115");

        //Return a string identifying you or your company.
        public override string AuthorName => "Tobias Schwinn";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "tobias.schwinn@gmail.com";

        public override string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}